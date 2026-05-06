/**
 * GitNexus LadybugDB → C# 参考清单导出器
 *
 * 直接从 LadybugDB 导出所有 Lua 符号，按 C# 目标类分组。
 * 输出: gitnexus-lua-reference.json
 *
 * Usage: node docs/export-gitnexus-ref.mjs
 */

import { createRequire } from 'node:module';
import fs from 'node:fs';
import path from 'node:path';

const _require = createRequire(import.meta.url);
const { initLbug, executeQuery, closeLbug } = _require('F:/DevProject/Sbox/GitNexus-1.6.4-rc.64/gitnexus/dist/core/lbug/lbug-adapter.js');

const REPO = 'F:/DevProject/Sbox/VJ-Base-master';
const LBUG = path.join(REPO, '.gitnexus/lbug');

// ── Lua → C# 类映射 ───────────────────────────────────────
// Key: Lua file pattern → C# class info
const CLASS_MAP = [
  { lua: 'lua/vj_base/ai/core.lua',           cs: 'Core/BaseNPC.cs',         cls: 'BaseNPC',      cat: 'P0_Core' },
  { lua: 'lua/vj_base/ai/schedules.lua',       cs: 'Schedule/AISchedule.cs', cls: 'AISchedule',    cat: 'P1_Schedule' },
  { lua: 'lua/vj_base/ai/base_aa.lua',         cs: 'Bases/CreatureNPC.cs',   cls: 'CreatureNPC',   cat: 'P1_Combat' },
  { lua: 'lua/vj_base/ai/base_tank.lua',       cs: 'Bases/TankNPC.cs',       cls: 'TankNPC',       cat: 'P2_Tank' },
  { lua: 'lua/entities/npc_vj_creature_base/', cs: 'Bases/CreatureNPC.cs',   cls: 'CreatureNPC',   cat: 'P1_Combat' },
  { lua: 'lua/entities/npc_vj_human_base/',    cs: 'Bases/HumanNPC.cs',      cls: 'HumanNPC',      cat: 'P2_Human' },
  { lua: 'lua/entities/npc_vj_tankg_base/',    cs: 'Components/TankGunner.cs', cls: 'TankGunner',  cat: 'P2_Tank' },
  { lua: 'lua/entities/npc_vj_tank_base/',     cs: 'Bases/TankNPC.cs',       cls: 'TankNPC',       cat: 'P2_Tank' },
  { lua: 'lua/weapons/weapon_vj_base/',         cs: 'Components/BaseWeapon.cs', cls: 'BaseWeapon',   cat: 'P3_Weapon' },
  { lua: 'lua/vj_base/funcs.lua',              cs: 'Utilities/VJUtils.cs',   cls: 'VJUtils',       cat: 'P4_Utils' },
  { lua: 'lua/vj_base/enums.lua',              cs: 'VJEnums.cs',             cls: 'VJEnums',       cat: 'P4_Utils' },
  { lua: 'lua/vj_base/hooks.lua',              cs: 'Core/NPCHooks.cs',       cls: 'NPCHooks',      cat: 'P4_Utils' },
];

// ── 主流程 ─────────────────────────────────────────────────
async function main() {
  await initLbug(LBUG);

  // 获取所有含行号的符号
  const allSymbols = await executeQuery(`
    MATCH (n)
    WHERE n.filePath IS NOT NULL AND n.startLine IS NOT NULL
    RETURN n.filePath, n.name, n.startLine, n.endLine, labels(n) AS kind
    ORDER BY n.filePath, n.startLine
  `);

  // 按 Lua 文件分组
  const byFile = {};
  for (const s of allSymbols) {
    const fp = s['n.filePath'];
    if (!byFile[fp]) byFile[fp] = [];
    byFile[fp].push({
      name: s['n.name'],
      line: s['n.startLine'],
      endLine: s['n.endLine'] || s['n.startLine'],
      kind: Array.isArray(s.kind) ? s.kind[0] : (s.kind || 'Variable')
    });
  }

  // 按 C# 类聚合
  const byClass = {};
  for (const { lua, cs, cls, cat } of CLASS_MAP) {
    if (!byClass[cls]) byClass[cls] = { csharpFile: cs, category: cat, luaFiles: [], symbols: [] };

    for (const [fp, syms] of Object.entries(byFile)) {
      if (fp.startsWith(lua) || fp === lua) {
        byClass[cls].luaFiles.push(fp);
        byClass[cls].symbols.push(...syms);
      }
    }

    // 去重按名称
    const seen = new Set();
    byClass[cls].symbols = byClass[cls].symbols.filter(s => {
      if (seen.has(s.name)) return false;
      seen.add(s.name);
      return true;
    }).sort((a, b) => a.line - b.line);
  }

  // ── 统计 ─────────────────────────────────────────────────
  let total = 0;
  for (const [cls, data] of Object.entries(byClass)) {
    const methods = data.symbols.filter(s => s.kind === 'Method' || s.kind === 'Function').length;
    const fields = data.symbols.filter(s => s.kind === 'Variable' || s.kind === 'Class').length;
    data.stats = { methods, fields, total: data.symbols.length };
    total += data.symbols.length;
  }

  // ── 输出 ─────────────────────────────────────────────────
  const out = {
    generated: new Date().toISOString(),
    source: 'GitNexus LadybugDB (lua knowledge graph)',
    repo: REPO,
    totalLuaFiles: Object.keys(byFile).length,
    totalSymbols: total,
    classes: byClass,
  };

  const outPath = 'F:/DevProject/Sbox/testzombie/docs/gitnexus-lua-reference.json';
  fs.writeFileSync(outPath, JSON.stringify(out, null, 2));

  // ── 摘要 ─────────────────────────────────────────────────
  console.log(`Exported ${total} symbols from ${Object.keys(byFile).length} Lua files`);
  console.log(`Output: ${outPath}\n`);
  console.log('By C# class:');
  for (const [cls, data] of Object.entries(byClass)) {
    console.log(`  ${data.category} ${cls}: ${data.stats.methods} methods, ${data.stats.fields} fields (${data.luaFiles.length} lua files)`);
  }

  await closeLbug();
}

main().catch(console.error);
