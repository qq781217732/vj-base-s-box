# VJ-Base Lua → C# 迁移计划

> **状态**: P0 核心路径完成 | 感知层到位 | 双轨已消除 | MaintainRelationships 翻译完成 | EngineAITaskSystem 填坑完成
> **最后更新**: 2026-05-06
> **架构指南**: `docs/translation-guide.md`

---

## 恢复指令（给下一个 AI / 土豆）

```
1. 读 translation-guide.md → 理解架构规则和文件拆分
2. 读本文件 → 了解当前进度
3. 读 api-mapping.md → 了解 M/Sw/C/X 标记含义
4. 打开要处理的 Lua 文件，对照已有 C# 文件
5. 翻译单文件流程见下方 §翻译执行清单

路径速查:
  Lua 根:  f:/DevProject/Sbox/VJ-Base-master/lua/
  C# 根:   f:/DevProject/Sbox/testzombie/Code/VJBase/
  Engine:  f:/DevProject/Sbox/testzombie/Code/VJBase/Engine/
  Source:  f:/DevProject/Sbox/source-sdk-2013/
  文档:    f:/DevProject/Sbox/testzombie/docs/
```

---

## 翻译规则

```
1. 1:1 机械翻译 — Lua 每行调用对应一行 C# 调用
2. 全是方法，不访问属性 — GetPos() 不写成 .WorldPosition
3. 不翻译的调用必须标注释:
   // SKIP: [Lua文件:行号] — [原因] — Phase 3 [归属系统]
4. 自创逻辑必须标 // Phase 3: 说明
5. 每翻完一个文件 → 编译 → 修报错 → 更新 checklist 状态

标记含义:
  Sw = S&Box 有等价 API → 方法内部转发
  M  = Source 引擎独有 → 先空壳，Phase 3 填
  C  = 纯 C# 逻辑 → 自己写
  X  = S&Box 不需要 → 跳过
```

---

## 翻译执行清单

```
□ 1. 打开 Lua 原文件，数 ENT: 方法数量
□ 2. 确定 C# 落点（Lua include() → C# partial class 映射）
□ 3. 逐行翻译:
     ├─ 遇到 :Method(args) → 查 api-mapping.md 找接口签名
     ├─ 遇到 COND_* → VJEnums.cs Condition 枚举
     ├─ 遇到 TASK_* → EngineConstants.cs EngineTask 常量
     ├─ 遇到 VJ.xxx → VJUtility / VJBehavior 等
     ├─ 翻译不了 → 写 // SKIP 注释留痕
     └─ 参数保持签名 1:1，不删不减不改序
□ 4. 编译通过
□ 5. 对比 Lua 原文件逐行确认无遗漏
□ 6. 更新 migration-master-checklist.md 状态
```

---

## 当前进度

### 翻译阶段: P0 核心路径完成

| 优先级 | 文件数 | 状态 | 代表 |
|--------|--------|------|------|
| P0: AI 核心 | 6 | ✅ 5, ⚠️ 1 | schedules.lua(✅), core.lua(✅), base_aa.lua(✅), base_tank.lua(⚠️) |
| P1: 实体基类 | 15 | 🔵 2, ⬜ 13 | CreatureNPC.Think(🔵), HumanNPC(🔵) |
| P2: 工具设施 | 14 | 🔵 2, ⬜ 12 | VJUtility.cs(✅), VJEnums.cs(✅) |
| P3: 武器 | 18 | 🔵 1, ⬜ 17 | weapon_vj_base(🔵) |
| P4: 低优 | 33 | ⬜ 33 | |

### Phase 3 填坑: 进行中

| # | 模块 | 状态 | 备注 |
|---|------|------|------|
| F1 | ~~IEngineEntity~~ | ❌ 已删除 | 双轨反模式，60+ 未用方法，全部调用已是 GameObject 属性 |
| F2 | IEngineAITaskSystem → EngineAITaskSystem | ✅ **已重写** | ~280 行，Movement/Face/Wait 任务实际驱动 NavMeshAgent |
| F3 | INPCConditions | ✅ | BaseNPC 直接实现 |
| F4 | INPCAttributes | ✅ | BaseNPC 直接实现，新增 Disposition/AddEntityRelationship |
| F5 | INPCSchedule | ✅ | BaseNPC 直接实现，16 个方法 + 双轨已消除 |
| F6 | schedules.lua 全部 32 方法 | ✅ | BaseNPC.Schedule.cs |
| F7 | core.lua 敌人管理 | ✅ | ForceSetEnemy/DoEnemyAlert/DoReadyAlert → BaseNPC.cs |
| F8 | Engine/AISenses.cs | ✅ | Source C++ ai_senses.cpp 翻译 (950 行) |
| F9 | TickSenses / GatherConditions 重构 | ✅ | BaseNPC.Conditions.cs 已删除，感知归 Engine 层 |
| F10 | core.lua MaintainRelationships | ✅ **本次完成** | BaseNPC.Relationships.cs (~390 行)，7/9 功能块 |
| F11 | 动画系统 (16 个序列方法) | ⬜ | Phase 3 |
| F12 | 战斗执行 (Melee/Range/Leap) | ⬜ | Phase 3 |
| F13 | AA 移动 (base_aa.lua) | ✅ **本次完成** | BaseNPC.AA.cs 421 行，5 方法机械翻译，字段从 CreatureNPC 搬到 BaseNPC |
| F14 | 声音/听力系统 | ⬜ | SoundSystem stub 已就位，Phase 3 对接 NoiseSystem |
| F15 | 调查系统 | ⬜ | 声音 + 手电筒检测 → SCHEDULE |

---

## C# 文件清单 (当前实际状态)

```
VJBase/
├── Core/
│   ├── BaseNPC.cs                  ← core.lua: 敌人管理+条件+属性+关系系统+Alive+Senses hook (~410行)
│   ├── BaseNPC.Schedule.cs         ← schedules.lua: 全部 32 个 ENT: 方法 (~390行)
│   ├── BaseNPC.Relationships.cs    ← core.lua:2127-2426 MaintainRelationships (~390行)
│   ├── BaseNPC.AA.cs               ← base_aa.lua: AA_* 桩 (Phase 3)
│   ├── VJEnums.cs                  ← enums.lua: 全部枚举+Condition(70)+VJMemoryKey+Disposition
│   ├── VJUtility.cs                ← funcs.lua: PICK/SET/HasValue/Rand
│   ├── VJInit.cs                   ← 初始化 + ConVars
│   ├── GlobalEngine.cs             ← 全局函数 (时间/日志/物理查询)
│   ├── IEngineAITaskSystem.cs      ← 引擎任务接口 + TaskStatus
│   ├── EngineAITaskSystem.cs       ← IEngineAITaskSystem 实现 (~280行)
│   ├── EngineConstants.cs          ← TASK_* 常量 + MoveTasks 集合
│   ├── INPCConditions.cs           ← 条件系统接口
│   ├── INPCSchedule.cs             ← 调度系统接口 (16 方法)
│   ├── INPCAttributes.cs           ← AI 属性接口 (14 方法)
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

## 下一步 (按优先级)

1. ✅ ~~MaintainRelationships 机械翻译~~ — 已完成 (2026-05-06)
2. ✅ ~~编译验证~~ — 已通过 (2026-05-06)
3. ✅ ~~EngineAITaskSystem 填坑~~ — 已完成 (2026-05-06)
4. ✅ ~~AA 移动~~ — `base_aa.lua` → `BaseNPC.AA.cs` — 已完成 (2026-05-06)
5. **转向系统** — TurnData FACE_POSITION/ENTITY/VISIBLE 全部 SKIP
6. **调查系统** — MaintainRelationships 里声音 + 手电筒检测
7. **动画系统** — 16 个 M 标记动画方法仍是 return 0/false

---

## SKIP 总表 (Phase 3+ 填坑清单)

> 2026-05-06 会话解决了 9 个 SKIP（详见 translation-guide.md §7.4），剩余如下。

| 文件 | SKIP 内容 | 归属系统 |
|------|----------|----------|
| `BaseNPC.Relationships.cs` | `FL_NOTARGET` 标志、`LostEnemy`/`OnPlayerSight`/`Investigate` 音效 | 标志/音效 |
| `BaseNPC.Relationships.cs` | 非 VJ NPC 反向关系、`AlliedWithPlayerAllies`+`IsDefaultNPC` | 关系系统 |
| `BaseNPC.Relationships.cs` | `GetMoveType()`+`m_vecSmoothedVelocity`、`CanBeEngaged` 回调 | Source 引擎 API |
| `BaseNPC.Relationships.cs` | 调查系统（声音 + 手电筒） | 感知/音效 |
| `BaseNPC.cs` | `UpdateEnemyMemory`、`IgnoreEnemyUntil`、`OnAlert`、alert sounds | 敌人记忆/事件/音效 |
| `BaseNPC.Schedule.cs` | `m_hOpeningDoor`、TurnData FACE_POSITION/ENTITY/VISIBLE | 门/转向 |
| `CreatureNPC.Think.cs` | `MaintainActivity()`、doLOSChase RunCode callbacks | 动画/视线追击 |
| `Engine/AISenses.cs` | `LookForObjects`、`GetEyePos_Entity` 硬编码 64 units | 感知物件/模型 |

---

## 关键 Lua → C# 对照

翻译时先读 Lua 原文件，通过本表找到 C# 落点:

| 功能 | Lua 文件 | C# 文件 |
|------|---------|---------|
| 敌人管理 (ForceSetEnemy/DoEnemyAlert) | core.lua:2043-2092 | BaseNPC.cs:314-409 |
| NPC 数据字段 | core.lua:93-210 | BaseNPC.cs:17-132 |
| 条件存储/查询 | core.lua | BaseNPC.cs:202-207 (INPCConditions) |
| 属性 (Enemy/NavType/Move/...) | core.lua | BaseNPC.cs:213-256 (INPCAttributes) |
| 调度生命周期 (32 方法) | schedules.lua:14-553 | BaseNPC.Schedule.cs |
| RunAI | schedules.lua:162-268 | CreatureNPC.Think.cs:58-116 |
| Think | creature_base/init.lua:1861 | CreatureNPC.Think.cs:23-52 |
| SelectSchedule | creature_base/init.lua:2802 | CreatureNPC.Think.cs:119-164 |
| SCHEDULE_ALERT_CHASE | creature_base/init.lua:1724 | CreatureNPC.Think.cs:183-210 |
| 人类 SCHEDULE_ALERT_CHASE | human_base/init.lua:2320 | HumanNPC.cs:56-73 |
| AA 移动 | base_aa.lua:30-391 | BaseNPC.AA.cs (Phase 3) |
| 感知系统 | ai_senses.cpp (Source C++) | Engine/AISenses.cs |
| Schedule 数据结构 | vj_ai_schedule.lua | Schedule/AISchedule.cs |
| Task 数据结构 | vj_ai_task.lua | Schedule/AITask.cs |
| 枚举/常量 | enums.lua | Core/VJEnums.cs |
| 工具函数 | funcs.lua | Core/VJUtility.cs |
