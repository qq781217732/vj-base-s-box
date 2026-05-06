/**
 * GitNexus → C# 验证比对器
 *
 * 对比 GitNexus 导出的 Lua 符号清单 vs 现有 C# 迁移代码，
 * 使用智能命名映射（snake_case → PascalCase 等）。
 *
 * Usage: node docs/verify-migration.mjs
 */

import fs from 'node:fs';
import path from 'node:path';

const REF_PATH = 'F:/DevProject/Sbox/testzombie/docs/gitnexus-lua-reference.json';
const CS_ROOT = 'F:/DevProject/Sbox/testzombie/Code/VJBase';

// ── 命名标准化 ────────────────────────────────────────────
function normalize(name) {
  return name
    .toLowerCase()
    .replace(/[^a-z0-9]/g, '');  // 去掉所有非字母数字字符
}

// snake_case → PascalCase 关键词提取
function extractKeywords(name) {
  return name
    .toLowerCase()
    .split(/[_\-.]+/)
    .filter(k => k.length > 0 && !['get', 'set', 'func', 'met', 'tbl', 'ent', 'is', 'has'].includes(k));
}

// ── 解析 C# 文件 ──────────────────────────────────────────
function parseCsFile(filePath) {
  if (!fs.existsSync(filePath)) return { methods: [], properties: [], fields: [] };

  const content = fs.readFileSync(filePath, 'utf-8');

  // C# 方法 (含签名信息)
  const methods = [];
  const methodRe = /(?:(?:public|private|protected|internal|static|virtual|override|async)\s+)+(?:Task|void|bool|int|float|string|double|Vector3|Rotation|GameObject|BaseNPC|List|HashSet|IEnumerable)\S*\s+(\w+)\s*\(([^)]*)\)/g;
  let m;
  while ((m = methodRe.exec(content)) !== null) {
    methods.push({ name: m[1], signature: m[2], line: content.slice(0, m.index).split('\n').length });
  }

  // C# [Property]
  const properties = [];
  const propRe = /\[Property\]\s+public\s+(\S+)\s+(\w+)\s*\{/g;
  while ((m = propRe.exec(content)) !== null) {
    properties.push({ type: m[1], name: m[2], line: content.slice(0, m.index).split('\n').length });
  }

  // C# 普通 public 字段 (非 Property attribute)
  const fields = [];
  const fieldRe = /public\s+(\S+)\s+(\w+)\s*[=;]/g;
  while ((m = fieldRe.exec(content)) !== null) {
    if (!['class', 'struct', 'record', 'void', 'static', 'const', 'readonly', 'partial', 'enum', 'interface'].includes(m[1])) {
      const name = m[2];
      if (!properties.some(p => p.name === name) && !methods.some(mt => mt.name === name)) {
        fields.push({ type: m[1], name, line: content.slice(0, m.index).split('\n').length });
      }
    }
  }

  return { methods, properties, fields };
}

// ── 模糊匹配评分 ──────────────────────────────────────────
function fuzzyScore(luaName, csName) {
  const luaNorm = normalize(luaName);
  const csNorm = normalize(csName);

  // 完全相同
  if (luaNorm === csNorm) return 100;

  // 包含关系
  if (luaNorm.includes(csNorm) || csNorm.includes(luaNorm)) return 80;

  // 关键词重叠
  const luaKW = new Set(extractKeywords(luaName));
  const csKW = new Set(extractKeywords(csName));
  if (luaKW.size === 0 || csKW.size === 0) return 0;

  const intersection = [...luaKW].filter(k => csKW.has(k)).length;
  const union = new Set([...luaKW, ...csKW]).size;
  return Math.round((intersection / union) * 100);
}

// ── 主比对 ────────────────────────────────────────────────
function compareClass(refData, csRoot) {
  const luaSymbols = refData.symbols;
  const luaMethods = luaSymbols.filter(s => s.kind === 'Method' || s.kind === 'Function');
  const luaFields = luaSymbols.filter(s => s.kind === 'Variable' || s.kind === 'Class');

  // 找对应的 C# 文件
  const csFile = path.join(csRoot, refData.csharpFile);
  const csAll = parseCsFile(csFile);

  // 读取依赖的其他 C# 文件（如 BaseNPC.cs 被 CreatureNPC 继承）
  const allCsMethods = [...csAll.methods];
  const allCsProps = [...csAll.properties, ...csAll.fields];

  // 对每个 Lua 方法找最佳 C# 匹配
  const methodResults = luaMethods.map(lm => {
    let best = null, bestScore = 0;
    for (const cm of allCsMethods) {
      const score = fuzzyScore(lm.name, cm.name);
      if (score > bestScore) { best = cm; bestScore = score; }
    }
    return {
      lua: lm.name,
      luaLine: lm.line,
      cs: best?.name || null,
      csLine: best?.line || null,
      score: bestScore,
      verdict: bestScore >= 80 ? 'MATCH' : bestScore >= 40 ? 'FUZZY' : 'MISSING'
    };
  });

  // 对每个 Lua 字段找最佳 C# 匹配
  const fieldResults = luaFields.map(lf => {
    let best = null, bestScore = 0;
    for (const cp of allCsProps) {
      const score = fuzzyScore(lf.name, cp.name);
      if (score > bestScore) { best = cp; bestScore = score; }
    }
    return {
      lua: lf.name,
      luaLine: lf.line,
      cs: best?.name || null,
      csLine: best?.line || null,
      score: bestScore,
      verdict: bestScore >= 80 ? 'MATCH' : bestScore >= 40 ? 'FUZZY' : 'MISSING'
    };
  });

  // 找出 C# 中有但 Lua 中没有的 (可能是冗余代码)
  const luaMethodNames = new Set(luaMethods.map(m => normalize(m.name)));
  const extraMethods = allCsMethods.filter(cm => {
    const score = [...luaMethodNames].reduce((max, ln) => Math.max(max, fuzzyScore(cm.name, ln)), 0);
    return score < 40;
  });

  const stats = {
    match: methodResults.filter(r => r.verdict === 'MATCH').length + fieldResults.filter(r => r.verdict === 'MATCH').length,
    fuzzy: methodResults.filter(r => r.verdict === 'FUZZY').length + fieldResults.filter(r => r.verdict === 'FUZZY').length,
    missing: methodResults.filter(r => r.verdict === 'MISSING').length + fieldResults.filter(r => r.verdict === 'MISSING').length,
    extraCs: extraMethods.length,
  };

  return { methods: methodResults, fields: fieldResults, extraMethods, stats };
}

// ── 执行 ──────────────────────────────────────────────────
const ref = JSON.parse(fs.readFileSync(REF_PATH, 'utf-8'));

console.log('=== GitNexus Lua → C# 迁移验证报告 ===');
console.log(`Generated: ${new Date().toISOString()}`);
console.log(`Lua source: ${ref.repo}`);
console.log(`C# code: ${CS_ROOT}\n`);

const allResults = {};
let totalMatch = 0, totalFuzzy = 0, totalMissing = 0, totalExtra = 0;

for (const [cls, data] of Object.entries(ref.classes)) {
  if (data.symbols.length === 0) continue;
  const result = compareClass(data, CS_ROOT);
  allResults[cls] = result;

  totalMatch += result.stats.match;
  totalFuzzy += result.stats.fuzzy;
  totalMissing += result.stats.missing;
  totalExtra += result.stats.extraCs;

  console.log(`\n## ${data.category} — ${cls} (${data.csharpFile})`);
  console.log(`   Lua: ${data.symbols.length} symbols → C#: ${result.stats.match} MATCH, ${result.stats.fuzzy} FUZZY, ${result.stats.missing} MISSING`);

  // 列出 MISSING 的方法
  const missingMethods = result.methods.filter(r => r.verdict === 'MISSING');
  if (missingMethods.length > 0 && missingMethods.length <= 15) {
    console.log(`   ⚠ Missing methods (${missingMethods.length}):`);
    for (const mm of missingMethods) {
      console.log(`      - ${mm.lua} (lua:${mm.luaLine})`);
    }
  } else if (missingMethods.length > 15) {
    console.log(`   ⚠ Missing methods (${missingMethods.length}) — too many to list`);
  }

  // 列出 C# 中多余的
  if (result.extraMethods.length > 0 && result.extraMethods.length <= 10) {
    console.log(`   ❓ Extra C# methods (not in Lua): ${result.extraMethods.map(m => m.name).join(', ')}`);
  }
}

console.log(`\n=== TOTAL ===`);
console.log(`MATCH: ${totalMatch} | FUZZY: ${totalFuzzy} | MISSING: ${totalMissing} | EXTRA C#: ${totalExtra}`);
console.log(`Coverage: ${Math.round(totalMatch / (totalMatch + totalFuzzy + totalMissing) * 100)}% exact, ${Math.round((totalMatch + totalFuzzy) / (totalMatch + totalFuzzy + totalMissing) * 100)}% fuzzy+`);
