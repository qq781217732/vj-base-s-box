/**
 * 审计清单重建器
 *
 * 当 Lua 源码变更后运行此脚本：
 *   1. gitnexus analyze    (更新知识图谱)
 *   2. node docs/rebuild-checklist.mjs  (重建审计清单)
 *
 * 已完成的审计结果会被保留（不会覆盖已完成的行）。
 */

import { createRequire } from 'node:module';
import fs from 'node:fs';
import path from 'node:path';

const _require = createRequire(import.meta.url);
const { initLbug, executeQuery, closeLbug } = _require(
  'F:/DevProject/Sbox/GitNexus-1.6.4-rc.64/gitnexus/dist/core/lbug/lbug-adapter.js'
);

const CHECKLIST_PATH = 'F:/DevProject/Sbox/testzombie/docs/audit-checklist.md';
const LBUG_PATH = 'F:/DevProject/Sbox/VJ-Base-master/.gitnexus/lbug';

// ── Lua → C# 类映射 ───────────────────────────────────────
const CLASS_MAP = [
  { pattern: 'lua/vj_base/ai/core.lua',         cs: 'Core/BaseNPC.cs',         cls: 'BaseNPC',     tier: 'P0' },
  { pattern: 'lua/vj_base/ai/schedules.lua',     cs: 'Schedule/AISchedule.cs', cls: 'AISchedule',  tier: 'P1' },
  { pattern: 'lua/entities/npc_vj_creature_base/', cs: 'Bases/CreatureNPC.cs', cls: 'CreatureNPC', tier: 'P1' },
  { pattern: 'lua/vj_base/ai/base_aa.lua',       cs: 'Bases/CreatureNPC.cs',   cls: 'CreatureNPC', tier: 'P1' },
  { pattern: 'lua/entities/npc_vj_human_base/',   cs: 'Bases/HumanNPC.cs',     cls: 'HumanNPC',    tier: 'P2' },
  { pattern: 'lua/entities/npc_vj_tank_base/',    cs: 'Bases/TankNPC.cs',      cls: 'TankNPC',     tier: 'P2' },
  { pattern: 'lua/vj_base/ai/base_tank.lua',     cs: 'Bases/TankNPC.cs',       cls: 'TankNPC',     tier: 'P2' },
  { pattern: 'lua/entities/npc_vj_tankg_base/',   cs: 'Components/TankGunner.cs', cls: 'TankGunner', tier: 'P2' },
  { pattern: 'lua/weapons/weapon_vj_base/',       cs: 'Components/BaseWeapon.cs', cls: 'BaseWeapon', tier: 'P3' },
  { pattern: 'lua/vj_base/funcs.lua',             cs: 'Utilities/VJUtils.cs',  cls: 'VJUtils',     tier: 'P4' },
  { pattern: 'lua/vj_base/hooks.lua',             cs: 'Core/NPCHooks.cs',      cls: 'NPCHooks',    tier: 'P4' },
];

async function main() {
  await initLbug(LBUG_PATH);

  const allSymbols = await executeQuery(`
    MATCH (n)
    WHERE n.filePath IS NOT NULL AND n.startLine IS NOT NULL
    RETURN n.filePath, n.name, n.startLine, n.endLine, labels(n) AS kind
    ORDER BY n.filePath, n.startLine
  `);

  // 按类聚合
  const classes = {};
  for (const s of allSymbols) {
    const fp = s['n.filePath'];
    const kind = Array.isArray(s.kind) ? s.kind[0] : (s.kind || 'Variable');
    const sym = {
      name: s['n.name'],
      line: s['n.startLine'],
      endLine: s['n.endLine'] || s['n.startLine'],
      kind
    };

    for (const map of CLASS_MAP) {
      if (fp.startsWith(map.pattern) || fp === map.pattern) {
        if (!classes[map.cls]) classes[map.cls] = { ...map, methods: [], fields: [] };
        if (kind === 'Method' || kind === 'Function') classes[map.cls].methods.push(sym);
        else classes[map.cls].fields.push(sym);
        break;
      }
    }
  }

  // 读取现有清单（如果有的话）— 保留已审计的结果
  let oldAudit = {};
  if (fs.existsSync(CHECKLIST_PATH)) {
    const old = fs.readFileSync(CHECKLIST_PATH, 'utf-8');
    // 解析旧表格中的已审计行
    const rowRe = /\| (\d+) \| (.+?) \| (\d+) \| (.+?) \| \[(.)\] \| \[(.)\] \| \[(.)\] \| \[(.)\] \| (.+?) \| (.+?) \| (.+?)\|/g;
    let m;
    while ((m = rowRe.exec(old)) !== null) {
      const key = m[2].trim();
      oldAudit[key] = {
        struct: m[5], timing: m[6], sidefx: m[7], boundary: m[8],
        verdict: m[9].trim(), auditor: m[10].trim(), notes: m[11].trim()
      };
    }
  }

  // 生成新清单
  let md = `# VJ-Base Lua → C# 迁移审计清单

> **自动生成**: ${new Date().toISOString().split('T')[0]} | **数据源**: GitNexus 知识图谱
> **状态**: \`[ ]\` 待审计 \`[/]\` 进行中 \`[x]\` 已完成 \`[-]\` 不适用
> **规则**: [audit-template.md](audit-template.md)

---
## 统计面板

| 类 | Lua 符号 | ✅ PASS | ⚠️ SEMI | ❌ FAIL | ➖ N/A | 待审计 | 进度 |
|----|---------|---------|---------|---------|--------|--------|------|
`;

  let totalAll = 0, totalPass = 0, totalSemi = 0, totalFail = 0, totalNa = 0, totalTodo = 0;

  for (const [cls, data] of Object.entries(classes)) {
    const total = data.methods.length + data.fields.length;
    let pass = 0, semi = 0, fail = 0, na = 0, todo = total;
    for (const s of [...data.methods, ...data.fields]) {
      const old = oldAudit[s.name];
      if (old && old.verdict) {
        todo--;
        if (old.verdict === 'PASS') pass++;
        else if (old.verdict === 'SEMI') semi++;
        else if (old.verdict === 'FAIL') fail++;
        else if (old.verdict === 'N/A' || old.verdict === 'NA') na++;
      }
    }
    totalAll += total; totalPass += pass; totalSemi += semi; totalFail += fail; totalNa += na; totalTodo += todo;

    const pct = total > 0 ? Math.round((total - todo) / total * 100) : 0;
    md += `| ${cls} | ${total} | ${pass} | ${semi} | ${fail} | ${na} | ${todo} | ${pct}% |\n`;
  }

  const totalPct = totalAll > 0 ? Math.round((totalAll - totalTodo) / totalAll * 100) : 0;
  md += `| **TOTAL** | **${totalAll}** | **${totalPass}** | **${totalSemi}** | **${totalFail}** | **${totalNa}** | **${totalTodo}** | **${totalPct}%** |\n`;

  // 每类详细表格
  for (const [cls, data] of Object.entries(classes)) {
    md += `\n---
## ${data.tier}: ${cls} — \`Code/VJBase/${data.cs}\`

**Lua source**: ${data.methods.length + data.fields.length} symbols\n\n`;

    if (data.methods.length > 0) {
      md += `### 方法 Methods (${data.methods.length})\n\n`;
      md += '| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |\n';
      md += '|---|---------|-------|---------|------|------|--------|------|------|--------|------|\n';
      let i = 1;
      for (const s of data.methods) {
        const old = oldAudit[s.name] || {};
        const st = old.struct === 'x' ? 'x' : ' ';
        const ti = old.timing === 'x' ? 'x' : ' ';
        const si = old.sidefx === 'x' ? 'x' : ' ';
        const bo = old.boundary === 'x' ? 'x' : ' ';
        md += `| ${i++} | ${s.name} | ${s.line} | — | [${st}] | [${ti}] | [${si}] | [${bo}] | ${old.verdict || ''} | ${old.auditor || ''} | ${old.notes || ''} |\n`;
      }
    }

    if (data.fields.length > 0) {
      md += `\n### 字段 Fields (${data.fields.length})\n\n`;
      md += '| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |\n';
      md += '|---|---------|-------|---------|------|--------|------|--------|------|\n';
      let i = data.methods.length + 1;
      for (const s of data.fields.slice(0, 30)) { // 只列前 30 个字段
        const old = oldAudit[s.name] || {};
        md += `| ${i++} | ${s.name} | ${s.line} | — | [ ] | [ ] | ${old.verdict || ''} | ${old.auditor || ''} | ${old.notes || ''} |\n`;
      }
      if (data.fields.length > 30) {
        md += `> *(${data.fields.length - 30} more fields — see gitnexus-lua-reference.json)*\n`;
      }
    }
  }

  md += '\n---\n## 审计指令\n\n';
  md += '请阅读 [audit-template.md](audit-template.md) 了解单行审计标准。\n';

  fs.writeFileSync(CHECKLIST_PATH, md);
  console.log(`Checklist rebuilt: ${totalAll} symbols across ${Object.keys(classes).length} classes`);
  console.log(`Preserved: ${totalPass + totalSemi + totalFail + totalNa} previously audited`);
  console.log(`Remaining: ${totalTodo} to audit`);

  await closeLbug();
}

main().catch(console.error);
