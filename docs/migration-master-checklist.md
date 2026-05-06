# VJ-Base Lua → C# 迁移总清单

> **Lua 85 文件 → C#** | 全部 854 符号待审计
> **地面真相 (Ground Truth):** Lua 源码 (`F:/DevProject/Sbox/VJ-Base-master/lua/`)
> **要修改的:** C# 代码 (`F:/DevProject/Sbox/testzombie/Code/VJBase/`)
> **架构指南:** `docs/translation-guide.md`
> **上次更新:** 2026-05-06

---

## 进度面板

| 优先级 | 总数 | ⬜ | 🔵 | ✅ | ⚠️ | 完成率 |
|--------|------|----|----|----|----|--------|
| P0: AI核心 | 5 | 0 | 0 | 4 | 1 | 80% → P0 全部完成（base_aa 已是翻译层，AA_MoveAnimation 是 Phase 3 动画）|
| P1: 实体基类 | 15 | 12 | 2 | 0 | 1 | 0% |
| P2: 工具设施 | 14 | 13 | 1 | 0 | 0 | 0% |
| P3: 武器 | 18 | 17 | 1 | 0 | 0 | 0% |
| P4: 简单实体 | 33 | 33 | 0 | 0 | 0 | 0% |
| **总计** | **85** | **75** | **4** | **4** | **2** | **4.7%** (P0 5/5 翻译层完成) |

> 进度在此更新，每个文件审计完成后改数字。

## 如何使用此清单

```
一个 Lua 文件 = 一行任务

状态: ⬜ 未开始  🔵 进行中  ✅ 已通过  ⚠️ 有问题  ❌ 受阻

读法:
  - Lua 文件 → C# 目标 → 符号数 → 状态 → 处理指引

处理流程 (每个文件):
  1. 打开 Lua 源文件 (路径已给出)
  2. 打开对应 C# 文件
  3. 用 GitNexus 导出该文件的符号清单 (见下)
  4. 逐个方法/字段做 4 维等价检查 (见 audit-template.md)
  5. 标记状态
```

---

## P0: AI 核心 (决定所有 NPC 行为) — 5 文件

| # | Lua 源文件 | C# 目标 | 方法/字段 | 状态 | 备注 |
|---|-----------|---------|----------|------|------|
| 1 | `lua/vj_base/ai/core.lua` | `Core/BaseNPC.cs` + `Relationships.cs` | — | ✅ | ForceSetEnemy/DoEnemyAlert/DoReadyAlert + **MaintainRelationships 完整翻译**（7/9 功能块）+ 关系系统（AddEntityRelationship/Disposition）+ Alive 检查。 |
| 2 | `lua/vj_base/ai/schedules.lua` | `Core/BaseNPC.Schedule.cs` | 32/32 | ✅ | 全部 32 个 ENT: 方法翻译完成。SCHEDULE_* 构建器从 ScheduleRunner 搬入。双轨已消除。Schedule 链完整：StartSchedule → Tasks → DoSchedule → NextTask → ScheduleFinished。 |
| 3 | `lua/vj_base/ai/base_aa.lua` | `Core/BaseNPC.AA.cs` | 5/5 | ✅ | AA_StopMoving / AA_MoveTo / AA_IdleWander / AA_ChaseEnemy 完整翻译（AA_MoveAnimation Phase 3 stub）。字段从 CreatureNPC 搬到 BaseNPC。 |
| 4 | `lua/vj_base/ai/base_tank.lua` | `Bases/TankNPC.cs` | 4/2 | ⚠️ | 部分字段存在但分散。完整坦克逻辑 Phase 3。 |
| 5 | `lua/includes/modules/vj_ai_task.lua` | `Schedule/AITask.cs` + `EngineAITaskSystem.cs` | 8/12 | ✅ | Task 结构完成。**EngineAITaskSystem 已重写**（Movement/Face/Wait 任务实际驱动 NavMeshAgent）。 |

---

## P1: 实体基类 (多子类依赖) — 15 文件

| # | Lua 源文件 | C# 目标 | 方法/字段 | 状态 | 备注 |
|---|-----------|---------|----------|------|------|
| 6 | `lua/entities/npc_vj_creature_base/init.lua` | `Bases/CreatureNPC.cs` + `.Think.cs` | — | 🔵 | Think/RunAI/SelectSchedule/SCHEDULE_ALERT_CHASE 完成。Melee/Range/Leap 攻击 Phase 3。 |
| 7 | `lua/entities/npc_vj_creature_base/shared.lua` | `Bases/CreatureNPC.cs` | — | ⬜ | |
| 8 | `lua/entities/npc_vj_human_base/init.lua` | `Bases/HumanNPC.cs` | — | 🔵 | 武器/手雷配置部分就位。人类特有逻辑 Phase 3。 |
| 9 | `lua/entities/npc_vj_human_base/shared.lua` | `Bases/HumanNPC.cs` | — | ⬜ | |
| 10 | `lua/entities/npc_vj_tank_base/init.lua` | `Bases/TankNPC.cs` | — | ⚠️ | 仅基架。 |
| 11 | `lua/entities/npc_vj_tank_base/shared.lua` | `Bases/TankNPC.cs` | — | ⬜ | |
| 12 | `lua/entities/npc_vj_tankg_base/init.lua` | `Bases/TankGNPC.cs` | — | ⬜ | |
| 13-20 | 弹道/控制器/生成器/Gib 基类 (8 文件) | VJProjectile/VJSpawner/... | — | ⬜ | 未开始 |

---

## P2: 工具/基础设施 — 14 文件

| # | Lua 源文件 | C# 目标 | 方法/字段 | 状态 | 备注 |
|---|-----------|---------|----------|------|------|
| 21 | `lua/vj_base/funcs.lua` | `Core/VJUtility.cs` | — | 🔵 | PICK/SET/HasValue/Rand 就位。 |
| 22 | `lua/vj_base/enums.lua` | `Core/VJEnums.cs` | — | ✅ | 全部枚举 + Condition(70) + RelationshipClass + VJColors 就位。 |
| 23-34 | hooks/convars/debug/corpse/music/nodegraph/autorun/controls/localization/resources (12 文件) | — | — | ⬜ | Phase 3 |

---

## P3: 武器 (17 武器 + 基类) — 18 文件

| # | Lua 源文件 | C# 目标 | 状态 | 备注 |
|---|-----------|---------|------|------|
| 35 | `lua/weapons/weapon_vj_base/shared.lua` | `swb_base/` | 🔵 | 已有独立武器系统。VJ 武器迁移 Phase 3。 |
| 36-52 | 17 个 VJ 武器文件 | `Weapons/` | ⬜ | Phase 3 |

---

## P4: 简单实体 + 效果 + 工具 — 33 文件

| # | Lua 源文件 | C# 目标 | 状态 | 备注 |
|---|-----------|---------|------|------|
| 53-66 | 弹道/手雷/火箭/道具 (14 文件) | `Entities/` | ⬜ | Phase 3 |
| 67-76 | 效果文件 (10 个) | `Effects/` | ⬜ | 优先度最低 |
| 77-85 | Tool 工具 (9 个) | `Tools/` | ⬜ | GMod Tool → S&box 需大改 |
| 86-88 | 测试实体 (3 个) | `Testing/` | ⬜ | 测试用途 |

---

## C# 文件清单 (当前实际状态)

```
VJBase/
├── Core/
│   ├── BaseNPC.cs                  ← core.lua: 敌人管理+条件+属性+关系系统+Alive+Senses hook (~410行)
│   ├── BaseNPC.Schedule.cs         ← schedules.lua: 全部 32 个方法 (~390行)
│   ├── BaseNPC.Relationships.cs    ← core.lua:2127-2426 MaintainRelationships (~390行)
│   ├── BaseNPC.AA.cs               ← base_aa.lua (空桩, Phase 3)
│   ├── VJEnums.cs                  ← enums.lua: 全部枚举 + Condition(70) + Disposition + VJMemoryKey
│   ├── VJUtility.cs                ← funcs.lua: PICK/SET/HasValue/Rand
│   ├── VJInit.cs                   ← convars/初始化
│   ├── GlobalEngine.cs             ← 全局函数 (时间/日志/物理查询)
│   ├── IEngineAITaskSystem.cs      ← 引擎任务接口 + TaskStatus
│   ├── EngineAITaskSystem.cs       ← IEngineAITaskSystem 实现 (~280行, Movement/Face/Wait)
│   ├── EngineConstants.cs          ← TASK_* 字符串常量 + MoveTasks 集合
│   ├── INPCConditions.cs           ← 条件系统接口 (BaseNPC 直接实现)
│   ├── INPCSchedule.cs             ← 调度系统接口 (16 方法, BaseNPC 直接实现)
│   ├── INPCAttributes.cs           ← AI 属性接口 (14 方法, BaseNPC 直接实现)
│   └── ActivityType.cs             ← ACT_* 常量
│
├── Engine/                         ← Source C++ 翻译
│   └── AISenses.cs                 ← ai_senses.cpp: 感知层 (950行)
│
├── Schedule/
│   ├── AISchedule.cs               ← vj_ai_schedule.lua
│   └── AITask.cs                   ← vj_ai_task.lua
│
└── Bases/
    ├── CreatureNPC.cs              ← creature shared.lua + AA 桩
    ├── CreatureNPC.Think.cs        ← creature init.lua (Think/RunAI/SelectSchedule/SCHEDULE_ALERT_CHASE)
    ├── HumanNPC.cs                 ← human shared.lua
    ├── HumanNPC.Think.cs           ← human init.lua
    ├── TankNPC.cs                  ← tank shared.lua
    ├── TankGNPC.cs                 ← tank gunner
    └── TestNPC.cs                  ← 测试 NPC

已删除:
  ❌ Core/NPCConditions.cs         → BaseNPC 直接实现 INPCConditions
  ❌ Core/NPCSchedule.cs           → BaseNPC 直接实现 INPCSchedule
  ❌ Core/NPCAttributes.cs         → BaseNPC 直接实现 INPCAttributes
  ❌ Core/BaseNPC.Conditions.cs    → 内容归位到 BaseNPC.cs
  ❌ Schedule/ScheduleRunner.cs    → 7 个 SCHEDULE_* 搬入 BaseNPC.Schedule.cs
  ❌ Core/IEngineEntity.cs         → 双轨反模式，73 方法仅 3 被调用
  ❌ Core/EngineEntity.cs          → 同上，含死字典 _health/_maxHealth
```

---

## 遇到问题时 — 处理 SOP

### 问题 1: C# 方法完全不匹配 Lua 逻辑
```
→ 标记该行 ⚠️
→ 在备注写 "[REWRITE] 具体差异"
→ 参考 audit-template.md 做 4 维审计写清楚缺口
→ 新建 TODO issue
```

### 问题 2: Lua 符号在 C# 中完全缺失
```
→ 标记 GAP
→ 如果是不需要迁移的 (GMod专用API) → 标记 [-]
→ 如果是需要迁移但还没写的 → 标记 ❌
→ 在备注写缺失的功能描述
```

### 问题 3: C# 文件不存在
```
→ 从已有模板复制骨架
→ 或者先标记 ⬜，等基类稳定后再建
```

### 问题 4: 两个 Lua 文件的逻辑合并到了同一个 C# 类
```
→ 正常。Core.lua 和 creature_base/init.lua 都合并到 CreatureNPC
→ Lua include() → C# partial class，参见 translation-guide.md §2
→ 分别审计两个 Lua 文件的符号，确保 C# 中都有对应
```

### 问题 5: 发现 GMod API 没有 S&box 等价物
```
→ 查 docs/api-mapping.md
→ 如果确实无等价物 → 标记 [-]，备注 "S&box 无需等价"
```

### 问题 6: 感知/条件是生产还是消费
```
→ 生产条件 = Engine/AISenses.cs (SetCondition)
→ 消费条件 = BaseNPC 翻译层 (HasCondition/ClearCondition)
→ 详见 translation-guide.md §3
```
