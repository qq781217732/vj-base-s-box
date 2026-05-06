# Lua → C# 迁移方法论 v2.0

> 从 VJ-Base → S&box 实践提炼。
> 现有产出: ~4,000 行 C# (16 文件)，300 个 gap 待补。
> 新增能力: GitNexus 知识图谱验证层。

---

## 四阶段流水线

```
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
│ Phase 0  │───→│ Phase 1  │───→│ Phase 2  │───→│ Phase 3  │
│ 资产盘点  │    │ 分类映射  │    │ 生成验证  │    │ 持续防护  │
│ GitNexus │    │ 人工决策  │    │ 自动+人工 │    │ 增量检测  │
└──────────┘    └──────────┘    └──────────┘    └──────────┘
```

---

## Phase 0: 资产盘点 (Inventory)

**目标**: 用 GitNexus 提取完整符号清单，建立迁移基线。

### Step 0.1 — 运行索引
```bash
gitnexus analyze --force .
```

### Step 0.2 — 导出符号清单
```bash
# 列出所有 Function/Method 符号
gitnexus cypher "MATCH (n) WHERE n.startLine IS NOT NULL RETURN n.name, n.filePath, n.startLine, n.endLine"

# 列出所有文件及其导入关系
gitnexus cypher "MATCH (a:File)-[r]->(b:File) WHERE r.type = 'IMPORTS' RETURN a.filePath, b.filePath"
```

### Step 0.3 — 生成迁移基线
输出 `migration-baseline.json`:
```jsonc
{
  "totalFiles": 96,
  "totalSymbols": 2424,
  "totalEdges": 2442,
  "symbolsByFile": {
    "lua/weapons/weapon_vj_crossbow.lua": [
      {"name": "OnPrimaryAttack", "kind": "Method", "lines": [32, 51]},
      {"name": "OnReload", "kind": "Method", "lines": [55, 63]},
      {"name": "sdLoadDone", "kind": "Class", "lines": [53, 53]}
    ]
  },
  "dependencies": {
    "lua/autorun/vj_base_autorun.lua": ["vj_base/enums.lua", "vj_base/funcs.lua", ...]
  }
}
```

### Step 0.4 — 确定迁移顺序（拓扑排序）
依赖少的文件先迁，被多文件依赖的基础模块先迁：
```
第1批 (无依赖): convars.lua, enums.lua, debug.lua
第2批 (依赖第1批): funcs.lua, hooks.lua
第3批 (AI 基础): core.lua → schedules.lua → base_aa.lua → base_tank.lua
第4批 (实体): npc_vj_creature_base → npc_vj_human_base → npc_vj_tank_base
第5批 (武器): weapon_vj_base → weapon_vj_crossbow → ... (17 weapons)
第6批 (入口): vj_base_autorun.lua
```

---

## Phase 1: 分类映射 (Classify)

**目标**: 对每个 Lua 符号做出三选一决策。

### 1.1 分类标准

| 分类 | 标记 | 决策 | 验证方法 |
|------|------|------|----------|
| **Delete** | ❌ | 不迁移，S&box 引擎已有等价物 | 确认 API 映射表 |
| **Rewrite** | 🔄 | 保留逻辑，重写为 C# 惯用写法 | 逐行比对 |
| **Map** | ✅ | 直接 API 替换 | 查对照表 |

### 1.2 分类决策树

```
Lua 符号
├─ GMod Engine API? (ents.Create, timer.Simple, etc.)
│  ├─ S&box 有等价物 → ✅ Map (查 api-mapping.md)
│  ├─ S&box 无等价物 → 🔄 Rewrite (自建 C# 实现)
│  └─ S&box 不需要   → ❌ Delete (如 StartEngineTask)
│
├─ VJ 框架函数? (VJ.EmitSound, VJ.PICK, etc.)
│  ├─ 可自建 C# 工具类 → 🔄 Rewrite (见 mapping-verified.md Category B)
│  └─ 依赖 GMod 特有机制 → ❌ Delete
│
├─ 纯数据字段? (SWEP.X = value)
│  ├─ 简单类型 (string, number, bool) → ✅ Map (自动 → [Property])
│  └─ GMod 类型 (Vector, Angle) → 🔄 Rewrite (需类型转换)
│
├─ 业务逻辑方法? (OnPrimaryAttack, SelectSchedule)
│  └─ 🔄 Rewrite (保留控制流，替换内部 API)
│
└─ Lua 特有模式? (metatable, coroutine, ...)
   └─ 🔄 Rewrite (人工重建，风险标记 MANUAL)
```

### 1.3 产出：分类清单

```
weapon_vj_crossbow.lua:
  AddCSLuaFile()        → ❌ Delete
  SWEP.Base = "..."      → ✅ Map → [Weapon(Base = "...")]
  SWEP.PrintName = "..."  → ✅ Map → [DisplayName("...")]
  SWEP.WorldModel = "..." → ✅ Map → public string WorldModel
  SWEP.Primary.ClipSize   → ✅ Map → PrimaryData.ClipSize
  if CLIENT then          → 🔄 Rewrite → if (Game.IsClient)
  VJ.AddKillIcon(...)     → 🔄 Rewrite → VJKillIcon.Register(...)
  function SWEP:OnPrimaryAttack → 🔄 Rewrite (含 ents.Create, Vector 等)
  local sdLoadDone = {...} → ✅ Map → static string[]
  timer.Simple(t, fn)     → 🔄 Rewrite → await Task.Delay (MANUAL)
```

---

## Phase 2: 生成 + 验证 (Generate & Verify)

**目标**: 生成 C# 骨架，然后验证逻辑等价性。

### 2.1 自动生成规则

| Lua 输入 | C# 输出 | 风险 |
|----------|---------|------|
| `ENT.X = 42` | `[Property] public int X { get; set; } = 42;` | AUTO |
| `ENT.X = "str"` | `[Property] public string X { get; set; } = "str";` | AUTO |
| `ENT.X = true` | `[Property] public bool X { get; set; } = true;` | AUTO |
| `ENT.X = {a=1, b=2}` | 生成嵌套 `XData` 类 | AUTO |
| `function ENT:M(args)` | `void M(args) { /* TODO */ }` | SEMI |
| `GMod API 调用` | TODO 注释 + API 映射提示 | SEMI |
| `timer.Simple` | TODO[MANUAL] + async 模式建议 | MANUAL |

### 2.2 验证清单（逐文件）

迁移完成一个文件后，必须通过以下 6 项检查：

```
□ Q1: 符号完整 —— GitNexus 导出的 N 个 Lua 符号，C# 中都有对应
□ Q2: 依赖匹配 —— Lua 的 require/include → C# 的 using/继承
□ Q3: 控制流等价 —— if/else/for/while 结构一致
□ Q4: 副作用顺序 —— 赋值、函数调用顺序一致
□ Q5: 边界条件 —— nil 检查 → null 检查，默认值一致
□ Q6: TODO 闭合 —— 所有 TODO[MIG] 要么已解决，要么有跟踪 issue
```

### 2.3 自动化验证工具

**已有**: `compare_migration.py` — 提取 `[Property]` names，跨引用比对。

**新增能力** (基于 GitNexus):

```bash
# Q1: 符号完整性检查
# 导出 Lua 符号清单
gitnexus cypher "MATCH (n) WHERE n.filePath = 'lua/weapons/weapon_vj_crossbow.lua'
  AND n.startLine IS NOT NULL RETURN n.name" > lua_symbols.txt
# 从 C# 提取方法/属性名
grep -E '(void|bool|int|float|string|class|record|struct) \w+' Crossbow.cs > cs_symbols.txt
# 比对差异
diff <(sort lua_symbols.txt) <(sort cs_symbols.txt)

# Q2: 依赖匹配
# Lua 的 include/require 目标
gitnexus cypher "MATCH (a:File)-[r:IMPORTS]->(b:File)
  WHERE a.filePath = 'lua/weapons/weapon_vj_crossbow.lua'
  RETURN b.filePath"
# C# 的 using/继承 — manual check
grep -E 'using|: ' Crossbow.cs
```

### 2.4 等价性审查模板

```markdown
## [文件] weapon_vj_crossbow.lua → Crossbow.cs
### 审查日期: 2026-05-05
### 审查人: ____

### Q1 符号清单
| Lua 符号 | C# 对应 | 状态 |
|----------|---------|------|
| OnPrimaryAttack | OnPrimaryAttack() | ✅ |
| OnReload | OnReload() | ✅ |
| sdLoadDone | sdLoadDone field | ✅ |

### Q2 依赖
| Lua 依赖 | C# 依赖 | 状态 |
|----------|---------|------|
| weapon_vj_base | VJBaseWeapon (继承) | ✅ |

### Q3 控制流
| Lua 行 | 结构 | C# 行 | 匹配 |
|--------|------|-------|------|
| 33-51 | if status=="Init" | OnPrimaryAttack L12 | ✅ |
| 35 | if CLIENT return | Game.IsClient check | ✅ |
| 45-48 | if owner.IsVJBaseSNPC | if (owner is VJBaseSNPC) | ✅ |
| 58-62 | timer.Simple closure | Task.Delay callback | ⚠️ MANUAL |

### Q4 副作用
- ents.Create → new CrossbowBolt() → Position → Spawn ✅
- phys:SetVelocity 在 if/else 两分支中均有 → 确认等价 ✅

### Q5 边界
- Lua nil → C# null ✅
- status == "Init" 缺失时 return → C# 等效 ✅
- math.Rand range 一致 ✅

### Q6 TODO
- TODO[MIG][SEMI] Verify VJ.CalculateTrajectory → VJPhysics ✅ 已确认
- TODO[MIG][MANUAL] timer.Simple → async Task.Delay ⚠️ 需功能测试
```

---

## Phase 3: 持续防护 (Guard)

**目标**: Lua 源码变更时，自动检测哪些 C# 文件受影响。

### 3.1 增量检测

```bash
# 检测 Lua 变更影响的 C# 文件
gitnexus detect_changes --scope compare --base main

# 输出: 哪些 Lua 文件的哪些符号变了
# → 对应 C# 文件需要同步更新
```

### 3.2 回归检查

每次 Lua 源文件变更后：

1. **GitNexus 重新索引** → `gitnexus analyze`
2. **导出新符号清单** → `migration-baseline-v2.json`
3. **比对基线** → `diff baseline-v1.json baseline-v2.json`
4. **标记受影响 C# 文件** → 更新 `migration-checklist.md`

### 3.3 CI 钩子（建议）

```yaml
# .github/workflows/migration-guard.yml
on:
  push:
    paths: ['lua/**']

jobs:
  check:
    steps:
      - run: gitnexus analyze
      - run: gitnexus detect_changes --scope compare --base main
      - run: python compare_migration.py --json > gaps.json
      - name: Alert if new gaps
        if: steps.gaps.outputs.new_gaps > 0
        run: echo "⚠️ Migration gaps detected"
```

---

## 工具链总结

```
┌──────────────────────────────────────────────────────────┐
│                      现有工具                              │
├─────────────────────┬────────────────────────────────────┤
│ GitNexus 知识图谱     │ 符号清单、导入图、影响分析、搜索       │
│ compare_migration.py │ 属性级别 gap 检测 (300 gaps found)   │
│ api-mapping.md       │ 300+ GMod→S&box API 映射            │
│ mapping-verified.md  │ 已验证的 Delete/Keep/Rewrite 分类    │
│ migration-checklist  │ 逐函数迁移进度追踪                    │
│ npc-migration-...    │ NPC 专用逐方法核对清单模板             │
│ gap-report.md        │ 自动生成的逐类 missing 属性报告        │
├─────────────────────┼────────────────────────────────────┤
│                      新增工具                              │
├─────────────────────┼────────────────────────────────────┤
│ IR spec             │ JSON/IR 中间表示 + 风险标签            │
│ TODO 格式标准        │ TODO[MIG][AUTO/SEMI/MANUAL] + SRC    │
│ 等价性审查模板        │ Q1-Q6 逐文件验证清单                   │
│ 迁移顺序拓扑         │ 基于依赖图的批次规划                    │
│ CI 钩子             │ Lua 变更 → C# 影响自动检测              │
└─────────────────────┴────────────────────────────────────┘
```

---

## 快速检查清单 (日常使用)

迁移一个 Lua 文件时的标准流程：

```bash
# 1. 盘点 — 这个文件有哪些符号？
gitnexus query "<文件名关键词>"  # 搜符号

# 2. 查依赖 — 谁依赖这个文件？这个文件依赖谁？
gitnexus cypher "MATCH (a:File)-[r:IMPORTS]->(b:File) WHERE a.name = 'xxx.lua' RETURN b.name"
gitnexus cypher "MATCH (a:File)-[r:IMPORTS]->(b:File) WHERE b.name = 'xxx.lua' RETURN a.name"

# 3. 查同名函数 — 其他文件有没有同样方法？（需要统一签名）
gitnexus query "OnPrimaryAttack"

# 4. 生成 C# 骨架（按 IR 规范）

# 5. 运行 gap 检测
python compare_migration.py --focus

# 6. 填写等价性审查模板（Q1-Q6）

# 7. 更新 migration-checklist.md
```

---

## 当前状态

| 指标 | 数值 |
|------|------|
| Lua 总行数 | ~24,500 |
| 需迁移行数 | ~13,700 |
| C# 已写 | ~4,000 行 (29%) |
| 已迁移文件 | 16 |
| 待迁移文件 | ~80 |
| Gap 总数 | 300 |
| 已关闭 Gap | 15 (alias-matched) |
| AI 核心(Core) | 60% 完成 |
| 战斗(Combat) | 55% 完成 |
| 武器(Weapons) | 仅 base，17 武器待迁 |
| 实体(Entities) | 仅 base，多子类待迁 |
