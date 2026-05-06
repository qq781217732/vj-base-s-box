# VJ Base → S&Box 翻译架构指南

> **读者：土豆（执行落地）**
> **撰写：阿纳金（Anakin）**
> 这份文档记录了本项目的翻译架构、规则、踩过的坑和清单。读通再动手。

---

## 1. 核心哲学

**Lua 方法调用 → C# 方法调用，签名 1:1，机械翻译。**

| 标记 | 含义 | 翻译阶段 | 填坑阶段 |
|------|------|---------|---------|
| `M` | Source 引擎独有 | 写 `entity.Method(args)` | 从零实现 |
| `Sw` | S&Box 有现成 API | 同上 | 方法内部调 S&Box |
| `C` | 纯 C# 逻辑 | 同上 | 自己写实现 |
| `X` | S&Box 不需要 | 跳过不翻译 | — |

关键约束：
- `GetParent()` 不会变成 `.Parent`，永远保持方法调用
- 每个 Lua 调用在 C# 里都有对应行——要么是翻译，要么是 `// SKIP: 原因`
- 翻译阶段**只变语法，不变签名，不删逻辑**

---

## 2. 文件拆分：Lua `include()` → C# `partial class`

Lua 靠运行时 `include()` 把文件合到一起。C# 用 `partial class` 做同样的事——编译时合并。

### 2.1 实体基类链

```
Lua: npc_vj_creature_base/init.lua
  │
  ├─ include("vj_base/ai/core.lua")        →  BaseNPC.cs
  │   关系系统、敌人维护、GatherConditions        (所有 NPC 共享)
  │
  ├─ include("vj_base/ai/schedules.lua")    →  BaseNPC.Schedule.cs
  │   全部 ENT:SCHEDULE_* + 任务生命周期         (32 个方法，单文件)
  │
  ├─ include("vj_base/ai/base_aa.lua")      →  BaseNPC.AA.cs
  │   AA_MoveTo / AA_ChaseEnemy / 飞行移动       (Phase 3)
  │
  ├─ include("shared.lua")                  →  CreatureNPC.cs
  │   默认字段值 (StartHealth=50, ...)
  │
  └─ init.lua 正文                          →  CreatureNPC.Think.cs
      SCHEDULE_ALERT_CHASE / creature 特有逻辑

Lua: npc_vj_human_base/init.lua
  ├─ core + schedules + base_aa            →  BaseNPC (共享，同上)
  ├─ shared.lua                             →  HumanNPC.cs
  └─ init.lua 正文                          →  HumanNPC.Think.cs
```

### 2.2 模块层（独立文件，不挂在 ENT 上）

```
Lua                                 C#
───                                 ──
includes/modules/
  vj_ai_schedule.lua             → Schedule/AISchedule.cs   (schedule 数据结构)
  vj_ai_task.lua                 → Schedule/AITask.cs       (task 数据结构)
  vj_ai_nodegraph.lua            → Phase 3

独立的常量/配置
  vj_base/enums.lua              → Core/EngineConstants.cs  (所有 enum)
  vj_base/funcs.lua              → Core/VJUtility.cs        (工具函数)
```

### 2.3 最终目标目录结构

```
VJBase/
├── Core/
│   ├── BaseNPC.cs                  ← core.lua: 敌人管理、感知 hook、条件存储、属性
│   ├── BaseNPC.Schedule.cs         ← schedules.lua: 全部 32 个方法
│   ├── BaseNPC.AA.cs               ← base_aa.lua: 飞行/水中移动 (Phase 3)
│   ├── VJEnums.cs                  ← enums.lua: Condition/VJState/VJBehavior/NavType/...
│   ├── VJUtility.cs                ← funcs.lua: VJ.PICK/VJ.SET/...
│   ├── EngineEntity.cs             ← IEngineEntity 实现
│   └── EngineAITaskSystem.cs       ← IEngineAITaskSystem 实现 (Phase 3)
│
├── Engine/                         ← Source C++ 翻译（替代引擎 C++ 黑盒）
│   └── AISenses.cs                 ← ai_senses.cpp: 感知层，生产条件
│
├── Schedule/
│   ├── AISchedule.cs               ← vj_ai_schedule.lua
│   └── AITask.cs                   ← vj_ai_task.lua
│
└── Bases/
    ├── CreatureNPC.cs              ← creature shared.lua
    ├── CreatureNPC.Think.cs        ← creature init.lua
    ├── HumanNPC.cs                 ← human shared.lua
    ├── HumanNPC.Think.cs           ← human init.lua
    ├── TankNPC.cs                  ← tank shared.lua
    └── TankNPC.Think.cs            ← tank init.lua
```

---

## 3. 翻译层 vs 感知层（关键分离）

这是最容易搞混的地方。Source 引擎有个 C++ 层每帧**自动**设置条件（COND_SEE_ENEMY 等），VJ Base 的 Lua 只负责**检查**这些条件。所以：

| 层 | 做什么 | 例子 |
|----|--------|------|
| **翻译层**（机械） | 检查条件、响应条件、清除条件 | `HasCondition(SEE_ENEMY)` / `ClearCondition(TASK_FAILED)` |
| **感知层**（Phase 3 新造） | 生产条件、空间查询、射线检测 | `SetCondition(SEE_ENEMY)` —— 原来由 Source C++ 产 |

**翻译层不产条件，只消费条件。** 感知层生产条件后通过 `Cond.SetCondition()` 喂给翻译层。两层互不污染：

```
AISenses.PerformSensing()                           ← Engine/ 层（Source C++ 翻译）
  → Look(HighPriority/NPCs/Objects)
  → FInViewCone + FVisible                          ← 手工实现
  → OnSeeEntity → SetCondition(SeeEnemy)            ← 喂给翻译层

BaseNPC.TickSenses()                                ← 翻译层入口
  → Senses.PerformSensing()
  → HasCondition(SeeEnemy)                          ← 翻译层消费
  → 决策 → SelectSchedule → SCHEDULE_ALERT_CHASE
```

Phase 3 填坑时**只改 Engine/AISenses.cs**，不动翻译文件。

---

## 4. 双轨问题（踩过的最大的坑）

### 4.1 问题描述

项目一度有**两套并行的 schedule 系统**：

| 轨道 | 路径 | 存储 |
|------|------|------|
| Track 1: 接口路径 | `Sched.StartSchedule()` → `NPCSchedule._current` 字典 | `Dictionary<GameObject, ...>` |
| Track 2: partial class | `this.StartSchedule()` → `BaseNPC.CurrentSchedule` 字段 | `BaseNPC` 实例字段 |

`RunAI()` 读 Track 2 的 `CurrentSchedule`，但所有调用方走 Track 1。结果：`CurrentSchedule` 永远 null，每帧重新创建 schedule，无限循环。

### 4.2 教训

- **不要为 ENT 方法创建独立的接口实现类**（如 `NPCSchedule : INPCSchedule`）
- `BaseNPC` 直接实现 `INPCSchedule` 接口，方法写在 partial class 里
- 所有 `Sched.XXX(GameObject, ...)` → `this.XXX(...)`
- `GameObject` 参数不需要传——`this` 就是 NPC

### 4.3 接口的正确用法

接口存在是为了**测试时的 mock 替换**，不是为了**把 ENT 方法从类里拆出去**：

```csharp
// ✅ 正确：BaseNPC 直接实现接口，方法在 partial class 里
public partial class BaseNPC : INPCConditions, INPCSchedule, INPCAttributes
{
    // BaseNPC.Schedule.cs
    public virtual void StartSchedule(AISchedule schedule) { ... }
    public virtual void ClearSchedule() { ... }
    // ...
}

// ❌ 错误：创建独立服务类，把状态拆到字典里
public class NPCSchedule : INPCSchedule
{
    Dictionary<GameObject, AISchedule> _current;  // 跟 BaseNPC 字段不同步！
}
```

---

## 5. 翻译纪律

### 5.1 SKIP 注释规范

遇到暂时翻不了的调用，**必须**写 SKIP 注释留痕：

```csharp
// ✅ 正确
// SKIP: lua:359 — IsValid(self:GetInternalVariable("m_hOpeningDoor")) — Phase 3 door system

// ❌ 错误（无声删逻辑）
（什么都不写，这行就消失了）
```

格式：`// SKIP: [Lua 文件:行号] — [原因] — Phase 3 [归属系统]`

### 5.2 魔法数字

```csharp
// ❌ 错误
if (Alerted != 2)

// ✅ 正确
if (Alerted != VJAlertState.Enemy)
```

用到枚举值的地方都用命名常量。

### 5.3 签名对齐

```csharp
// Lua: function ENT:ForceSetEnemy(ent, stopMoving, maxPerf, hasEnemy)
//   ↓
// ✅ C# 参数 1:1
public virtual void ForceSetEnemy(GameObject ent, bool stopMoving = false, 
    bool maxPerf = false, bool hasEnemy = false)

// ❌ 不能在翻译阶段删参数、改顺序、合并逻辑
```

### 5.4 空壳标记

```csharp
// Phase 3 才实现的方法，放空壳但写注释
public virtual void StartEngineTask(int taskId, float taskData) 
{
    // Phase 3: delegate to NavMeshAgent or animation system
}
```

### 5.5 提交前自审（焊进流程）

**写完一个功能块 → 提交前 → 逐行对照 Lua 原文 diff 一遍。**

不做这一步的代价已在 2026-05-06 会话验证：4 个 bug 全是提交后用户抓出来的（分支放错、坐标 double-add、yaw-only 路径用错变量、死字典写入永不读取）。每个都是对照 Lua 原文 30 秒就能发现的。

**2026-05-07 新增反面教材**（攻击系统翻译时自纠的 4 个错误）：

| # | 错误 | 为什么是错的 | 查 Lua 30 秒 → |
|---|------|-------------|----------------|
| 1 | `ent.Name == myClass` 替代 `ent:GetClass()` | S&Box `GameObject.Name` 是实例名（"GameObject #123"），不是类型名（"npc_vj_creature_base"） | `if ent == self or ent:GetClass() == myClass then continue end` |
| 2 | 编造 `PickRandom()` 方法 | 项目已有 `VJUtility.PICK()`，签名为 `PICK<T>(IList<T>)` | `PICK(selfData.RangeAttackProjectiles)` |
| 3 | `HasMeleeAttack` 重复定义 | BaseNPC.AA.cs 和 BaseNPC.cs 都是 `partial class BaseNPC`，同名属性冲突 CS0102 | 每个字段只在一个 partial file 定义 |
| 4 | `Scene.FindInPhysics(Vector3, float)` 签名编造 | S&Box API 签名为 `FindInPhysics(Sphere)`，`Sphere` 构造器接受 `(Vector3 center, float radius)` | 先查 `sbox_search_api` 再写调用 |

```
□ 打开对应的 Lua 源文件
□ 从入口方法开始，Lua 一行 → C# 一行，确认:
   ├─ 分支结构一致 (if/else/elseif 对应)
   ├─ 参数顺序一致
   ├─ 变量语义一致 (Lua 存目标值 → C# 也必须存目标值，不能存当前值)
   └─ 无死代码 (写入但永不读取的字段/字典)
□ 改了辅助方法 → 检查所有调用方是否适配新语义
   (例: WorldSpaceCenter 从 feet→OBB 中心 → 调用方是否重复加了 WorldPosition)
□ git diff --stat 确认改动文件数和预期一致
```

**这不是可选项。** 翻译阶段的 bug 不是逻辑设计错误——是粗心。对照 Lua 原文 diff 能拦住 90% 的粗心错。`git log --oneline` 里 `fix` 开头的提交应该极少。如果 `fix` 比 `translate` 还多，流程有问题。

---

## 6. 幻觉防范

### 6.1 已验证的痛点

翻译过程中 AI/人工容易产生以下幻觉：

| 类型 | 例子 | 后果 |
|------|------|------|
| 编造枚举值 | `NPCState { None, Idle, Alert, Combat, Dead }` — Lua 实际是 `VJ_STATE_NONE=0, VJ_STATE_FREEZE=1, ...` | 彻底错误的语义 |
| 编造方法 | `AISchedule.IsInterrupted()` — Lua 实际是字段 `CanBeInterrupted` | 编译通过但调用不到 |
| 编造 TASK 常量 | `TASK_FIND_COVER_FROM_BEST_SOUND` — Lua 里不存在 | 运行时字符串匹配不到 |
| 无声删逻辑 | 翻译时觉得某行不重要就跳过了 | 行为对不上，最难排查 |

### 6.2 验证流程

1. 每翻译完一个 Lua 文件，跑交叉验证脚本（`verify_api_mapping.py`）
2. 重点检查：方法签名、枚举值、TASK_* 字符串、CLASS_* 常量
3. 任何"我觉得应该有"的内容都要去 Lua 源文件搜一下确认
4. 搜索不到 = 不存在 = 要么标 Phase 3，要么别写

---

## 7. 当前状态清单

> 最后更新：2026-05-07（攻击系统填坑：计时器 + 伤害标签 + Prop 交互 + 弹体框架 + OnlyPush fix）

### 7.1a schedules.lua → BaseNPC.Schedule.cs（32 个方法）

| 状态 | 方法 | 备注 |
|------|------|------|
| ✅ | `StartSchedule`, `DoSchedule`, `StopCurrentSchedule`, `ScheduleFinished` | 已搬，双轨已消除 |
| ✅ | `SetTask`, `NextTask`, `OnTaskComplete`, `TaskFinished` | 同上 |
| ✅ | `IsScheduleFinished`, `StartTask`, `RunTask`, `TaskTime` | 同上 |
| ✅ | `OnTaskFailed`, `OnMovementFailed`, `OnMovementComplete` | 同上 |
| ✅ | `SCHEDULE_FACE`, `SCHEDULE_GOTO_POSITION`, `SCHEDULE_GOTO_TARGET` | 从 ScheduleRunner 搬入 |
| ✅ | `SCHEDULE_COVER_ENEMY`, `SCHEDULE_COVER_ORIGIN` | 同上 |
| ✅ | `SCHEDULE_IDLE_WANDER`, `SCHEDULE_IDLE_STAND` | 同上 |
| ✅ | `TASK_VJ_PLAY_ACTIVITY`, `TASK_VJ_PLAY_SEQUENCE` | Phase 3 stub |
| ✅ | `StartEngineTask`, `RunEngineTask`, `StartEngineSchedule`, ... | Phase 3 stub |
| ⚠️ | `OnStateChange`, `TranslateNavGoal` | Phase 3 stub |

### 7.1b base_aa.lua → BaseNPC.AA.cs（5 个方法）

| 状态 | 方法 | 行数 | 备注 |
|------|------|------|------|
| ✅ | `AA_StopMoving` | base_aa:30-41 | Rigidbody.Velocity 替代 SetLocalVelocity |
| ✅ | `AA_MoveTo` | base_aa:57-257 | ~160 行，TraceHull/地面回避/LastChasePos/加速 lerp |
| ✅ | `AA_IdleWander` | base_aa:267-353 | ~80 行，随机游荡/地面回避/加速 lerp |
| ✅ | `AA_ChaseEnemy` | base_aa:360-365 | 委托 AA_MoveTo + chase 选项 |
| ⚠️ | `AA_MoveAnimation` | base_aa:373-391 | Phase 3 空壳（动画选表/PlayAnim/ACT_*） |

> AA 字段（11 个状态 + 10 个配置）从 `CreatureNPC.cs` 搬到 `BaseNPC` 层。`DoAA_*` 覆写从 CreatureNPC 移到 BaseNPC.AA.cs。

### 7.1c PlaySoundSystem → BaseNPC.Sound.cs（35 分支 + 辅助方法）

| 状态 | 内容 | 备注 |
|------|------|------|
| ✅ | `PlaySoundSystem(sdSet, customSD, sdType)` | core.lua:2944-3375, 35 分支完整实现 |
| ✅ | `CreateSound(sdFile, sdLevel, sdPitch)` | funcs.lua:74-87 → `Sound.Play()` + `handle.Parent` + `handle.Pitch` |
| ✅ | `EmitSound(sdFile, sdLevel, sdPitch)` | funcs.lua:89-98 → `Sound.Play(sdFile, WorldPosition)` |
| ✅ | `StopSD(SoundHandle)` | funcs.lua:70-72 → `handle.Stop()` |
| ✅ | `GetSoundPitch(object)` | core.lua:940-961, 完整替换旧 stub |
| ✅ | `GetSoundDuration(sdSet)` | SoundDuration fallback — 硬编码 2/3/3.5s（Phase 3 改进） |
| ✅ | `StopAllSounds()` | 停止全部 8 个活跃 SoundHandle |
| ✅ | `OnPlaySound` / `OnCreateSound` / `OnEmitSound` | virtual 回调 |
| ⚠️ | `SoundLevel` 映射 | 接收参数但未用于 S&Box 衰减 — Phase 3 待调整 |
| ⚠️ | `PlayFootstepSound` / `PlayIdleSound` | Phase 3 辅助音效系统（非 PlaySoundSystem，独立的 think 循环音效） |
| ✅ | 音效配置字段 | 全部 ~200 字段（Has* / SoundTbl_* / *SoundChance / *SoundLevel / *SoundPitch / NextSoundTime_*） |

> S&Box API 映射与已知限制：
> - `CreateSound` → `Sound.Play()` + 手动 `handle.Parent = GameObject`（Phase 3 可切 `GameObject.PlaySound()` 自动 parent）
> - `EmitSound` → `Sound.Play(eventName, WorldPosition)` 不 parent，fire-and-forget
> - `SoundLevel` (dB) 接收参数但未映射到 `SoundHandle.Distance`/`Decibels` — Phase 3
> - `SoundDuration` → `GetSoundDuration(sdSet)` 硬编码 fallback — Phase 3
> - `math.random(1, chance)` → `Game.Random.Next(1, chance + 1)` 防 throw + 匹配 Lua inclusive-max
> - Lua `customSD` 可传 table → C# 仅 `string`，调用方需先 `PickSound()` 选好

### 7.2 接口体系

| 接口 | 方法数 | 实现 | 状态 |
|------|--------|------|------|
| `INPCConditions` | 5 | `BaseNPC` 直接实现 | ✅ 已删除 NPCConditions.cs |
| `INPCSchedule` | 16 | `BaseNPC` 直接实现 | ✅ 已删除 NPCSchedule.cs |
| `INPCAttributes` | 14 | `BaseNPC` 直接实现 | ✅ 新增 Disposition/AddEntityRelationship |
| `IEngineAITaskSystem` | 5 | `EngineAITaskSystem` | ✅ 已重写（Movement/Face/Wait 任务实际执行） |
| `IEngineCombat` | 1 | — | ⏳ Phase 3 |
| `IEngineSound` | — | — | ⏳ Phase 3 |

> ❌ `IEngineEntity`/`EngineEntity` 已删除 — 双轨反模式，73 方法中仅 3 个被调用且全是 GameObject 属性转发。BaseNPC 是 Component，直接持有 GameObject 引用。

### 7.3 已完成的致命/高优修复

| # | 问题 | 状态 |
|----|------|------|
| 1 | 删 `ScheduleRunner.cs`，内容搬进 `BaseNPC.Schedule.cs` | ✅ |
| 2 | 双轨消除：所有 `Sched.StartSchedule()` → `this.StartSchedule()` | ✅ |
| 3 | 删 `NPCSchedule.cs` / `NPCConditions.cs` / `NPCAttributes.cs` | ✅ |
| 4 | 删 `BaseNPC.Conditions.cs`，内容归位到 `BaseNPC.cs` | ✅ |
| 5 | `ScanForEnemy` + `CheckLineOfSight` 删除 → `Engine/AISenses.cs` 替代 | ✅ |
| 6 | `PerceptionSystem.cs` → `Engine/AISenses.cs`（Source C++ ai_senses.cpp 翻译） | ✅ |
| 7 | `ForceSetEnemy` / `DoEnemyAlert` / `DoReadyAlert` 带 SKIP 注释搬入 `BaseNPC.cs` | ✅ |
| 8 | `NPCState` 保留（Source 引擎内置，VJ 仍用 `SetNPCState(NPC_STATE_COMBAT)`） | ✅ |
| 9 | `SightDistance` / `SightAngle` / `Behavior` 搬入 `BaseNPC.cs` 字段区 | ✅ |
| 10 | `NavType` 枚举补回 `VJEnums.cs`；`NavType` 属性改名 `NavTypeVal` | ✅ |
| 11 | `SensingFlags` 属性改名 `SensingFlagBits`（消除与静态类命名冲突） | ✅ |
| 12 | `AISenses.LookForHighPriorityEntities` 用 `GetAllComponents<PlayerBase>()` 替代 tag 过滤 | ✅ |
| 13 | **MaintainRelationships 机械翻译** — 9 个功能块中 7 个已实现，2 个 Phase 3 SKIP | ✅ 2026-05-06 |
| 14 | **关系系统补全** — `AddEntityRelationship`/`Disposition` 方法 + `_relationshipDisp` 字典 | ✅ 2026-05-06 |
| 15 | **EngineAITaskSystem 填坑** — 重写，Movement/Face/Wait 任务实际驱动 NavMeshAgent | ✅ 2026-05-06 |
| 16 | **删 `IEngineEntity`/`EngineEntity`** — 双轨反模式，60+ 未用方法 | ✅ 2026-05-06 |
| 17 | **`Disposition` 枚举与 `Disposition()` 方法冲突** — 全局改用 `VJBase.Disposition.XXX` | ✅ 2026-05-06 |
| 18 | 补 `using SWB.Player;` — `PlayerBase` 命名空间缺失 | ✅ 2026-05-06 |
| 19 | **base_aa.lua → BaseNPC.AA.cs** — 5 方法全部机械翻译，字段从 CreatureNPC 搬到 BaseNPC | ✅ 2026-05-06 |
| 20 | **转向系统** — TurnData 6 种 FACE 类型 + MaintainTurnTarget + SetTurnTarget 完整实现 | ✅ 2026-05-06 |
| 21 | **调查系统** — 声音 + 手电筒检测，OnInvestigate 回调 + SCHEDULE_FACE/GOTO_POSITION | ✅ 2026-05-06 |
| 22 | **批量消 SKIP (8 项)** — FL_NOTARGET/AlliedWithPlayerAllies/CanBeEngaged/OnAlert/doLOSChase/isVJBaseSNPC 等 | ✅ 2026-05-06 |
| 23 | **WorldSpaceCenter OBB** — (Mins+Maxs)/2 包围盒中心，替代 feet-only WorldPosition | ✅ 2026-05-06 |
| 24 | **IgnoreEnemyUntil 删除** — Source 引擎 reaction delay 在 S&Box 不存在，删死字典 | ✅ 2026-05-06 |
| 25 | **HumanNPC SCHEDULE_ALERT_CHASE** — doLOSChase 双分支 + MaintainAlertBehavior unreachable-weapon 逻辑 | ✅ 2026-05-07 |
| 26 | **CreatureNPC 攻击系统** — ExecuteMeleeAttack/ExecuteRangeAttack/ExecuteLeapAttack 从空壳翻译为完整骨架（~170 行） | ✅ 2026-05-07 |
| 27 | **HumanNPC 手雷系统** — GrenadeAttack/ExecuteGrenadeAttack 从空壳翻译为完整骨架（~170 行） | ✅ 2026-05-07 |
| 28 | **TankNPC OnDamaged** — base_tank.lua:35-49 机械翻译，Init/PreDamage 分支 | ✅ 2026-05-07 |
| 29 | **Attack config fields (30+)** — core.lua:249-329 全部 Melee/Range/Leap 配置字段 + 9 个 virtual 回调搬入 BaseNPC.cs | ✅ 2026-05-07 |
| 30 | **攻击系统填坑** — GetAttackTimer + ScheduleAttackTimers + StopAttacks + ProcessAttackTimers + 伤害标签 + Prop 交互 + 弹体框架 | ✅ 2026-05-07 |

### 7.4 SKIP 总表（Phase 3+ 填坑清单）

#### 已解决（2026-05-06 会话）
| 文件 | 原 SKIP | 解决方案 |
|------|---------|----------|
| `BaseNPC.cs` | `!ent:Alive()` — 死目标过滤 | `BaseNPC.Alive(ent)` 组件化检查，VJ NPC 读 Dead 标志，非 VJ 默认活着 |
| `BaseNPC.cs` | `AddEntityRelationship` — 全部 6 处 | `AddEntityRelationship(ent, disp, priority)` 写入 `_relationshipDisp` 字典 |
| `BaseNPC.Schedule.cs` | Engine task 永不完结 | `EngineAITaskSystem` 重写，Movement/Face/Wait 实际驱动 NavMeshAgent |
| `BaseNPC.Relationships.cs` | `funcVisible` — 缺失 | `CanSee()` 实现射线检测 |
| `BaseNPC.Relationships.cs` | `funcIsInViewCone` — 缺失 | `IsInViewCone()` FOV 点积检查 |
| `BaseNPC.Relationships.cs` | `HandlePerceivedRelationship` 回调 | 组件化读取 + 调用 |
| `BaseNPC.Relationships.cs` | Disposition 回落逻辑 | 完整实现 D_NU/D_VJ_INTEREST 分支 |
| `BaseNPC.Relationships.cs` | `ent.VJ_NPC_Class` 跨实体读取 | `ent.Components.Get<BaseNPC>()?.VJ_NPC_Class` |
| `BaseNPC.Relationships.cs` | 实体类型检测用 Tags | 改为组件检测 `Get<BaseNPC>()`/`Get<PlayerBase>()` |
| `BaseNPC.Relationships.cs` | `PlaySoundSystem("LostEnemy")` / `("Investigate")` / `("OnPlayerSight")` — 3 处 | `BaseNPC.Sound.cs` — PlaySoundSystem 完整翻译 35 分支 |
| `BaseNPC.Relationships.cs` | 行 126 `FL_NOTARGET` 标志检查 | `HasEntityFlag(ent, FL_NOTARGET)` stub 到位（Phase 3 flag system 填） |
| `BaseNPC.Relationships.cs` | 行 242 非 VJ NPC 反向关系 | 组件化 `entBase.AddEntityRelationship(GameObject, ...)`，已在 else 分支正确位置 |
| `BaseNPC.Relationships.cs` | 行 287 `ent.CanBeEngaged` 回调 | `entBaseNPC?.CanBeEngaged(ent, GameObject, distance)` 委托调用 |
| `BaseNPC.Relationships.cs` | 行 192 `AlliedWithPlayerAllies` + `IsDefaultNPC` 完整逻辑 | 跨实体 `ent.Components.Get<BaseNPC>()?.AlliedWithPlayerAllies` + `IsDefaultNPC` |
| `BaseNPC.Relationships.cs` | 行 269-270 `GetMoveType()` + `m_vecSmoothedVelocity` | Rigidbody 等效；velocity → rb.Velocity（Phase 3 优化平滑） |
| `BaseNPC.cs` | 行 376 `UpdateEnemyMemory(ent, ent:GetPos())` | `EntityMemory["enemy_pos"] = pos` 基础实现 |
| `BaseNPC.cs` | 行 379 `IgnoreEnemyUntil(ent, 0)` | **已删除** — Source reaction delay 在 S&Box 不存在（9ced735） |
| `BaseNPC.cs` | `OnAlert` 回调 | `OnAlert?.Invoke(ent)` 委托 |
| `BaseNPC.Schedule.cs` | TurnData FACE_POSITION/ENTITY/VISIBLE | 全部 6 种 FACE 类型完整实现（fdb90cf + c39dafc） |
| `CreatureNPC.Think.cs` | doLOSChase=true RunCode callbacks | `RunCodeOnFinish` re-chase 循环（8d7537d） |
| `Engine/AISenses.cs` | 行 846 `GetEyePos_Entity` 硬编码 64 units | `BaseNPC.ViewOffset` 字段，有组件读 ViewOffset，无则 fallback 64 |
| `BaseNPC.AA.cs` | `WorldSpaceCenter()` 仅返回 origin，缺 OBB 中心偏移 | (Mins+Maxs)/2 OBB 中心（33e5d36 + 5b990d8 fix double-add） |
| `BaseNPC.cs` | alert sounds (DoEnemyAlert) | PlaySoundSystem("Alert") + cooldown timer |
| `BaseNPC.cs` | DoEnemyAlert NPCState fix (Lua:2080-2083) | 补回 NPCState 检查，阻止 combat→alert→combat 切换 |
| `CreatureNPC.Think.cs` | Breath sounds 空壳 (仅 timer 无播放) | StopSD + CreateSound + CurrentBreathSound 存储 |
| `Core/` (新文件) | 整个 `PlaySoundSystem` 系统从未翻译 | `BaseNPC.Sound.cs` — 音效配置字段 + 35 分支 PlaySoundSystem + StopAllSounds + GetSoundPitch + 回调 |
| `HumanNPC.cs` | SCHEDULE_ALERT_CHASE doLOSChase 双分支 SKIP | doLOSChase=true → schedule_alert_chaseLOS + RunCodeOnFinish re-chase 回环；false → schedule_alert_chase |
| `HumanNPC.cs` | MaintainAlertBehavior 缺 unreachable-weapon 逻辑 | `HasCondition(Condition.EnemyUnreachable) && HasWeapon` → SCHEDULE_ALERT_CHASE(true) |
| `CreatureNPC.Think.cs` | ExecuteMeleeAttack 空壳 | FindInPhysics 扫描 + 角度判定 + DamageInfo + 流血结构（~80 行），prop/击退/玩家特效标 Phase 3 SKIP |
| `CreatureNPC.Think.cs` | ExecuteRangeAttack 空壳 | VJUtility.PICK 弹体选择 + AttackState 管理（~40 行），弹体生成/物理标 Phase 3 SKIP |
| `CreatureNPC.Think.cs` | ExecuteLeapAttack 空壳 | FindInPhysics 扫描 + 伤害应用 + 命中/未命中回调（~50 行） |
| `HumanNPC.cs` | GrenadeAttack 空壳 | 落地方向判定 + 转向 + 攻态 + 计时器骨架（~80 行），动画/骨骼/弹体生成标 Phase 3 SKIP |
| `HumanNPC.cs` | ExecuteGrenadeAttack 空壳 | spawn 位姿 + 投掷速度 + 物理 + AttackState（~90 行），骨折/弹体生成/熔丝标 Phase 3 SKIP |
| `TankNPC.cs` | OnDamaged Phase 3 stub | Init/PreDamage 分支机械翻译 + Source 伤害系统 SKIP 注释 |
| `BaseNPC.cs` | 攻击配置字段缺失 (core.lua:249-329) | 新增 ~30 字段（MeleeDamage/MeleeDamageType/HasRangeAttack/LeapDamageDistance 等）+ 9 个 virtual 回调 |
| `BaseNPC.AA.cs` | `HasMeleeAttack` 重复定义 | 删除重复，统一由 BaseNPC.cs 定义 |

#### 已解决（2026-05-07 会话 — 攻击系统填坑）
| 文件 | 原 SKIP | 解决方案 |
|------|---------|----------|
| `CreatureNPC.Think.cs` | `attackTimers[MELEE/RANGE/LEAP](self)` ×3 | `ScheduleAttackTimers()` 轮询（GetAttackTimer → AttackResetTime / AttackReEnableTime） |
| `CreatureNPC.Think.cs` | `SetDamageType(MeleeAttackDamageType)` | `MapDamageTypeToTag(int)` → `DamageInfo.Tags.Add(tag)` (18 constants in VJDamageTags) |
| `CreatureNPC.Think.cs` | `PropInteraction` / `GetPhysicsObject` / `ApplyForceCenter` | `Rigidbody.Enabled/Wake/ApplyForce` + OnlyPush/OnlyDamage 分支 |
| `CreatureNPC.Think.cs` | `ents.Create(projectileClass)` 空壳 | `SpawnRangeProjectile(string, target)` virtual 方法 |
| `HumanNPC.cs` | `timer.Create("attack_grenade_start")` / `attackTimers[GRENADE]` | `GrenadeExecTime` 轮询 + `StashedGrenadeEnt/Dir` |
| `BaseNPC.cs` | `timer.Create("attack_*_reset")` / `timer.Create("attack_*_reset_able")` | `ScheduleAttackTimers()` → `ProcessAttackTimers()` Think 轮询 |
| `BaseNPC.cs` | 计时器字段缺失 | 10 timer config + 4 timer runtime + 3 grenade stash 字段 |

#### 剩余待填
| 文件 | 行/位置 | SKIP 内容 | 归属系统 |
|------|---------|----------|----------|
| `BaseNPC.Relationships.cs` | 行 126 | `FL_NOTARGET` — stub 到位，Phase 3 flag system 填 `HasEntityFlag` | 标志系统 |
| `BaseNPC.Relationships.cs` | 行 269-270 | `m_vecSmoothedVelocity` — 当前用 rb.Velocity 瞬时值，Phase 3 优化 | Source 引擎 API |
| `BaseNPC.Schedule.cs` | — | `m_hOpeningDoor` door system | 门系统 |
| `CreatureNPC.Think.cs` | — | `MaintainActivity()` call | 动画维持 |
| `Engine/AISenses.cs` | 599 | `LookForObjects` — FL_OBJECT 系统 | 感知物件 |
| `BaseNPC.AA.cs` | 95-103 | `WaterLevel()` 水源检查 — 整个 aquatic 分支 | 水系统 |
| `BaseNPC.AA.cs` | 98-101 | `MASK_WATER` trace + aquatic 可达性检查 | 水系统 |
| `BaseNPC.AA.cs` | — | `AA_MoveAnimation` 动画选表/PlayAnim/ACT_* | 动画系统 |
| `BaseNPC.Sound.cs` | — | `SoundLevel` (dB) 未映射到 S&Box 衰减 (`Distance`/`Decibels`) | 音效 |
| `BaseNPC.Sound.cs` | — | `GetSoundDuration()` 硬编码 fallback，非真实音效文件时长 | 音效 |
| `BaseNPC.Schedule.cs` | — | `RememberUnreachable` / `IsUnreachable` — Source 引擎敌人记忆 API | 敌人记忆 |
| `HumanNPC.cs` | — | `IsMeleeWeapon` — 武器近战检测，Phase 3 武器系统 | 武器系统 |
| `CreatureNPC.Think.cs` | — | `IsNextBot` / `loco:Approach` / `ViewPunch` / `SetDSP` / `DoMeleeAttackPlayerSpeed` | 玩家系统 |
| `HumanNPC.cs` | — | `LookupAttachment` / `GetAttachment` / `LookupBone` / `GetBonePosition` / `GetShootPos` | 骨骼动画 |
| `HumanNPC.cs` | — | `VisibleVec` / `VJ.TraceDirections` 可见性/空间查询 | 感知系统 |
| `CreatureNPC.Think.cs` | — | `GetClass()` entity type comparison → component type check | 实体类型 |
| `BaseNPC.cs` | — | `constraint.RemoveConstraints(ent, "Weld")` — S&Box joint system | Prop系统 |
| `BaseNPC.cs` | — | `istable(mainTime)` VJ.SET random range in GetAttackTimer | 计时器 |

### 7.5 新增文件清单

| 文件 | 来源 | 行数 | 状态 |
|------|------|------|------|
| `Engine/AISenses.cs` | ai_senses.cpp 机械翻译 | ~950 | ✅ |
| `Core/VJEnums.cs` | enums.lua | ~147 | ✅ |
| `Core/BaseNPC.cs` | core.lua 合并（敌人管理+条件+属性+hook+关系系统+Alive+攻击配置+计时器） | ~670 | ✅ |
| `Core/BaseNPC.Schedule.cs` | schedules.lua 全部 32 方法 | ~390 | ✅ |
| `Core/BaseNPC.Relationships.cs` | core.lua:2127-2426 MaintainRelationships | ~390 | ✅ 9 功能块中 7 已实现 |
| `Core/EngineAITaskSystem.cs` | Phase 3 重写（Movement/Face/Wait 任务） | ~280 | ✅ |
| `Core/EngineConstants.cs` | TASK_* 字符串常量 + MoveTasks 集合 | ~75 | ✅ |
| `Core/BaseNPC.AA.cs` | base_aa.lua 5 方法机械翻译 | ~421 | ✅ 4/5 完整翻译，1 Phase 3 stub |
| `Core/BaseNPC.Sound.cs` | core.lua:2944-3375 PlaySoundSystem + 音效配置字段 | ~1050 | ✅ 35 分支 + 辅助方法全部翻译 |

### 7.5b 已删除文件

| 文件 | 原因 |
|------|------|
| `Core/IEngineEntity.cs` | 双轨反模式，73 方法仅 3 被调用，全是 GameObject 属性转发 |
| `Core/EngineEntity.cs` | 同上，含死字典 `_health`/`_maxHealth` |
| `Core/NPCSchedule.cs` | 双轨消除（历史） |
| `Core/NPCConditions.cs` | 双轨消除（历史） |
| `Core/NPCAttributes.cs` | 双轨消除（历史） |
| `Core/BaseNPC.Conditions.cs` | 内容归位到 BaseNPC.cs（历史） |
| `Schedule/ScheduleRunner.cs` | 内容搬入 BaseNPC.Schedule.cs（历史） |

### 7.6 MaintainRelationships 翻译完成度

> core.lua:2127-2426，300 行，9 个功能块。**2026-05-06 会话完成 9/9 块。**

| 功能块 | Lua 行 | 状态 | 备注 |
|--------|--------|------|------|
| 入口守卫 | 2128-2132 | ✅ | PassiveNature 返回、RelationshipEnts 空检查 |
| 清理无效实体 | 2160-2174 | ✅ | IsValid 清理 + `Alive(ent)` 组件化检查 + AddEntityRelationship(D_NU) |
| 距离裁剪 | 2178-2185 | ✅ | 超出 SightDist → ResetEnemy + PlaySoundSystem("LostEnemy") |
| 友军识别 | 2212-2257 | ✅ | CLASS_* 对比 via `ent.Components.Get<BaseNPC>()?.VJ_NPC_Class` |
| HandlePerceivedRelationship | 2263-2272 | ✅ | 组件化调用 `ent.Components.Get<BaseNPC>()?.HandlePerceivedRelationship` |
| 玩家推挤 | 2300-2319 | ✅ | 核心碰撞检测 OK（m_vecSmoothedVelocity → Rigidbody.Velocity，Phase 3 优化） |
| 敌人检测 | 2347-2358 | ✅ | **选最近可见敌人 → ForceSetEnemy** + AddEntityRelationship(D_HT) |
| 调查系统 | 2379-2408 | ✅ | 声音检测 + 手电筒 + PlaySoundSystem("Investigate") |
| OnPlayerSight | 2412-2424 | ✅ | 检测逻辑 + `OnPlayerSight(ent)` 回调 + PlaySoundSystem("OnPlayerSight") |

**2026-05-06 会话关键新增：**
- `BaseNPC.Alive(ent)` — 查 VJ NPC 的 Dead 标志，非 VJ 默认活着
- `AddEntityRelationship(ent, disp, priority)` → `_relationshipDisp` 字典
- `Disposition(ent)` — 查询已存储的关系
- `HandlePerceivedRelationship` 委托 — 实体自定义感知回调
- `Disposition` 回落逻辑 — 中立 NPC → D_NU，敌对 NPC → D_VJ_INTEREST
- `Disposition.XXX` → `VJBase.Disposition.XXX` — 解决与 `Disposition()` 方法的命名冲突
- **转向系统** — TurnData 6 种 FACE 类型 + `SetTurnTarget` + `MaintainTurnTarget` + `ApplyYawTurn`/`ApplyFullAxisTurn`
- **调查系统** — 声音 + 手电筒检测 (`GetEntitySoundInvestLevel`/`IsEntityShiningFlashlightOnMe`)
- **批量消 SKIP (8 项)** — `AlliedWithPlayerAllies`/`IsDefaultNPC` 跨实体读、`CanBeEngaged` 委托、`OnAlert` 回调、非 VJ 反向关系组件化、`doLOSChase` RunCodeOnFinish、`FL_NOTARGET` stub、`GetTarget`/`SetTarget` guard
- **WorldSpaceCenter OBB** — `(Mins+Maxs)/2` 包围盒中心 + double-add bugfix
- **IgnoreEnemyUntil 删除** — Source 引擎 reaction delay，S&Box 无等价机制
- **§5.5 提交前自审** + **§11 Git 提交规范** — 焊进流程

**2026-05-07 会话关键新增：**
- **HumanNPC SCHEDULE_ALERT_CHASE** — doLOSChase 双分支（schedule_alert_chaseLOS + RunCodeOnFinish re-chase 回环 / schedule_alert_chase）
- **HumanNPC MaintainAlertBehavior** — unreachable enemy + 武器检测（`HasCondition(Condition.EnemyUnreachable) && HasWeapon`）
- **CreatureNPC ExecuteMeleeAttack** — FindInPhysics 扫描 + 角度判定 + DamageInfo + 流血结构（~80 行）
- **CreatureNPC ExecuteRangeAttack** — VJUtility.PICK 弹体选 + AttackState 管理（~40 行）
- **CreatureNPC ExecuteLeapAttack** — FindInPhysics 扫描 + 伤害 + hit/miss 回调（~50 行）
- **HumanNPC GrenadeAttack** — 落地方向判定（Enemy/EnemyLastVis/FindBest）+ 转向 + 计时器骨架（~80 行）
- **HumanNPC ExecuteGrenadeAttack** — spawn 位姿 + 投掷速度物理 + 熔丝分发（~90 行）
- **TankNPC OnDamaged** — base_tank.lua:35-49 机械翻译，Init/PreDamage 分支 + DMG_* SKIP
- **Attack config fields 30+** — core.lua:249-329 全部 Melee/Range/Leap 配置 + 9 个 virtual 回调搬入 BaseNPC.cs
- **HasMeleeAttack 去重** — BaseNPC.AA.cs 删除重复定义，统一在 BaseNPC.cs

---

## 8. 翻译执行清单

拿到一个 Lua 文件后，按这个顺序做：

```
□ 1. 数 ENT: 方法：一共 _____ 个
□ 2. 确定 C# 落点（用 2.1 的 include→partial 映射表）
□ 3. 逐行翻译：
     ├─ 遇到 :Method(args) → 查 api-mapping.md 找接口签名
     ├─ 遇到 COND_* → 对应 Condition 枚举
     ├─ 遇到 TASK_* → 对应 EngineTask 常量
     ├─ 遇到 VJ.xxx → 对应 VJUtility / VJBehavior 等
     ├─ 翻译不了 → 写 // SKIP: [文件:行号] — 原因
     └─ 参数用 literal（暂不优化）
□ 4. 编译通过
□ 5. 对比 Lua 原文件逐行确认无遗漏
□ 6. 跑 verify_api_mapping.py 交叉验证
□ 7. 提交前自审（§5.5）：逐行对照 Lua diff，检查分支/参数/变量语义/死代码
```

---

## 9. 查漏脚本

验证脚本位置：`f:/DevProject/Sbox/testzombie/verify_api_mapping.py`

用途：
- 扫描 `api-mapping.md` 中声明的所有方法/常量/枚举
- 在 VJ Base Lua 源文件中逐条搜索确认
- 输出：Lua 中存在但文档缺失的 / 文档声明但 Lua 中不存在的

运行：
```bash
cd f:/DevProject/Sbox/testzombie
python verify_api_mapping.py
```

## 10. 给下一个 AI 的恢复指令

> **如果你是新接手的 AI，读完这章就能开工。**

### 10.1 3 分钟速览

```
你是谁：帮阿纳金和土豆把 VJ Base (GMod Lua) 机械翻译成 S&Box C#
做到哪：~65%。P0 + 攻击骨架 + 攻击填坑（计时器/伤害/Prop/弹体）+ 手雷全部完成。
        剩余：动画（16 M 方法）/ 门 / 水 / FL_OBJECT 4 个 Phase 3 系统。
怎么验：git log --oneline -20 秒级概览（§11 Git 提交规范）
        python verify_api_mapping.py 交叉验证 Lua↔文档
```

### 10.2 必读文件（按顺序）

| # | 文件 | 为什么读 |
|---|------|---------|
| 1 | `docs/translation-guide.md` | 本文。架构规则、文件拆分、翻译纪律 |
| 2 | `docs/migration-plan.md` | 当前进度、文件清单、下一步 |
| 3 | `docs/migration-master-checklist.md` | 85 文件逐行状态、SOP |
| 4 | `docs/api-mapping.md` | M/Sw/C/X 标记、方法签名映射 |

### 10.3 路径速查

```
Lua 源码:     f:/DevProject/Sbox/VJ-Base-master/lua/
C# 目标:     f:/DevProject/Sbox/testzombie/Code/VJBase/
Source C++:  f:/DevProject/Sbox/source-sdk-2013/
文档:        f:/DevProject/Sbox/testzombie/docs/
验证脚本:    f:/DevProject/Sbox/testzombie/verify_api_mapping.py
```

### 10.4 踩过的坑（别再踩）

**坑 1: 双轨。** 不要为 ENT 方法创建独立服务类 (`NPCSchedule.cs`)。BaseNPC 直接实现接口，方法写 partial class。

**坑 2: 幻觉。** 任何"我觉得应该有"的东西必须去 Lua 源文件搜确认。搜不到 = 不存在。

**坑 3: 无声删逻辑。** 翻译不了的调用写 `// SKIP: [文件:行号] — 原因`，不能直接跳过。

**坑 4: 感知是拉模型。** VJ Lua 是 NPC 主动扫描 → ForceSetEnemy。不要用 Source C++ 的 OnSeeEntity 推模型回调替代。

**坑 5: 敌人选最近的不是第一个。** `MaintainRelationships` 遍历所有可见实体，选 `distanceToEnt < nearestDist` 的。

**坑 6: 枚举和字段同名。** C# 里 `enum NavType` 和 `int NavType` 冲突 → 字段改名 `NavTypeVal`。同理 `SensingFlags` → `SensingFlagBits`。

**坑 7: GetAllComponents<Component>() 是性能炸弹。** 用 `GetAllComponents<PlayerBase>()` 或 `FindInSphere()`。

**坑 8: 枚举和方法同名。** `enum Disposition` 和 `int Disposition(GameObject)` 在 partial class 内部冲突，编译器找方法不找枚举。统一用 `VJBase.Disposition.XXX` 完全限定名。

**坑 9: S&Box Vector3 没有 2D 方法。** `Length2DSqr()` 不存在，手动展开 `delta.x*delta.x + delta.y*delta.y`。

**坑 10: S&Box NavMeshAgent API。** `MoveTo(Vector3)` 存在但无重载，`IsNavigating`/`MaxSpeed`/`Velocity` 都是正确的。`UpdateRotation` 默认 true（自动面朝移动方向），手动控制旋转时需关掉。

**坑 11: Game.Random.Next(1, 1) 抛异常。** Lua `math.random(1, 1)` 返回 1（安全）。.NET `Random.Next(1, 1)` 因为 min==max 抛 ArgumentOutOfRangeException。全局用 `Next(1, chance + 1)` 防 throw，同时匹配 Lua 的 inclusive-max 概率语义。

**坑 12: Sound.Play() vs GameObject.PlaySound()。** S&Box 文档提及 `GameObject.PlaySound()` 自动 parent SoundHandle 到 GameObject。当前用 `Sound.Play()` + 手动 `handle.Parent = GameObject`，功能等效。如果 `GameObject.PlaySound()` API 稳定，Phase 3 可切过去。

**坑 13: Lua table → C# string 类型窄化。** Lua `PlaySoundSystem` 的 `customSD` 可以是 string 或 table（PICK 随机选）。C# 签名是 `string`，不支持 table。调用方需先 `PickSound()` 选好再传入。Lua 的 `StartsWith("{")` table-string hack 已删除。

### 10.5 机械翻译 vs 自己造（红线）

| 可以 | 不可以 |
|------|--------|
| Lua 调用 → C# 调用，签名 1:1 | 觉得某行不重要就跳过 |
| 翻译不了写 `// SKIP:` 留痕 | 用 Source C++ 逻辑替代 Lua 逻辑 |
| 空壳标 `// Phase 3:` | 自创 Simpler 版本（如 ScanForEnemy） |
| C# 枚举 1:1 对 Lua 常量 | 编造 Lua 里不存在的枚举值 |

**如果 Lua 有，就翻译。Lua 没有的，Phase 3 才造。**

### 10.6 当前优先级（土豆看这）

```
1. ✅ MaintainRelationships 机械翻译  ← 已完成
   core.lua:2127-2426 → BaseNPC.Relationships.cs (390 行，9/9 功能块)

2. ✅ 编译验证  ← 已通过

3. ✅ EngineAITaskSystem 填坑  ← 已完成
   重写 ~280 行，Movement/Face/Wait 任务实际驱动 NavMeshAgent

4. ✅ BaseNPC.AA.cs  ← 已完成
   base_aa.lua 5 方法机械翻译 (421 行)，字段从 CreatureNPC 搬到 BaseNPC

5. ✅ 调查系统填坑  ← 已完成
   MaintainRelationships 里声音 + 手电筒检测 (bcd6ed7)

6. ✅ 转向系统  ← 已完成
   TurnData + MaintainTurnTarget (fdb90cf, c39dafc)

7. ✅ 音效系统  ← 已完成
   PlaySoundSystem 35 分支 + BaseNPC.Sound.cs (~1050 行)

8. ✅ HumanNPC chase + grenade + 攻击骨架  ← 已完成 2026-05-07
   HumanNPC SCHEDULE_ALERT_CHASE 双分支 + MaintainAlertBehavior unreachable-weapon
   GrenadeAttack/ExecuteGrenadeAttack 完整骨架 (~170 行)
   ExecuteMeleeAttack/RangeAttack/LeapAttack 完整骨架 (~170 行)
   TankNPC OnDamaged 机械翻译 + attack config fields 30+

9. ✅ 攻击系统填坑（Phase 3）  ← 已完成 2026-05-07
   GetAttackTimer + ScheduleAttackTimers + StopAttacks + ProcessAttackTimers (轮询)
   VJDamageTags 18 常量 + MapDamageTypeToTag + Prop 交互 + SpawnRangeProjectile 框架
   解 9 处 SKIP（attackTimers ×3 + SetDamageType + PropInteraction + projectile spawn + grenade timer ×2 + grenade exec）

10. 动画系统
   16 个 M 标记动画方法仍是 return 0/false
   11. CreatureNPC.MaintainAlertBehavior 缺口（~35 行未翻译）
```

### 10.7 验证命令

```bash
# 交叉验证：文档声明 vs Lua 实存
cd f:/DevProject/Sbox/testzombie && python verify_api_mapping.py

# 搜某个符号在所有 Lua 文件中的出现
cd f:/DevProject/Sbox/VJ-Base-master && grep -r "ForceSetEnemy" lua/

# 搜 C# 文件中的 SKIP 标记
cd f:/DevProject/Sbox/testzombie/Code && grep -rn "SKIP:" VJBase/
```

---

*最后更新：2026-05-07*
*翻译阶段：~65%，P0 + 攻击骨架 + 攻击填坑 + 手雷全部完成。下一步：动画（16 M 方法），然后 MaintainAlertBehavior 缺口。*

---

## 11. AI 协作 Git 提交规范

> **目标：让任何 AI 或人 10 秒内从 `git log` 看懂项目发生了什么。**

### 11.1 何时提交

```
一个功能块翻译完 → commit
一个 ENTi 方法搬完 → commit
一个 Phase 填坑完成 → commit
修了一个编译错误 → commit
删了一组死代码     → commit
```

**不要在翻译中途提交。** 提交边界 = 功能边界，不是时间边界。

### 11.2 提交消息格式

```
<type>(<scope>): <中文简述>

Source: <lua文件:行号>
Target: <cs文件>
Methods: N/N
SKIPs: N
```

### 11.3 type 类型

| type | 含义 | 示例 |
|------|------|------|
| `translate` | 机械翻译 Lua→C# | `translate(P0): core.lua MaintainRelationships 全部 9 功能块` |
| `fill` | Phase 3+ 填坑（从空壳变成实际逻辑） | `fill(Engine): EngineAITaskSystem 重写，Movement/Face/Wait 任务` |
| `fix` | 修 bug（逻辑错误、编译错误、语义不对齐） | `fix(Relationships): Disposition 枚举与方法同名冲突` |
| `cleanup` | 删死代码、消除双轨、重构 | `cleanup(Core): 删除 IEngineEntity/EngineEntity 双轨` |
| `field` | 补字段、加配置项 | `field(BaseNPC): 新增 AA 移动状态字段` |
| `docs` | 更新进度文档、修复文档错误 | `docs: 更新 §7 状态清单，MaintainRelationships 全部完成` |

### 11.4 scope 范围

```
P0-P4      — 翻译阶段优先级
Core       — BaseNPC.cs, VJEnums.cs, 接口文件等
Engine     — AISenses.cs, EngineAITaskSystem.cs
Schedule   — AISchedule.cs, AITask.cs
Bases      — CreatureNPC, HumanNPC, TankNPC
Relationships — MaintainRelationships 相关
AA         — 飞行/水中移动
docs       — 文档更新
```

### 11.5 完整示例

```bash
# 翻译一个完整方法
git commit -m "$(cat <<'EOF'
translate(Relationships): MaintainRelationships 核心翻译

Source: core.lua:2127-2426
Target: BaseNPC.Relationships.cs (~390 行)
Methods: 1 (MaintainRelationships)
Blocks: 7/9 (调查系统 + FL_NOTARGET SKIP)
SKIPs: 2 (声音调查、手电筒检测 → Phase 3)
EOF
)"

# 修一个编译错误
git commit -m "$(cat <<'EOF'
fix(Relationships): 3 个编译错误 — Disposition 冲突 + PlayerBase using + LengthSquared2D

- Disposition.XXX → VJBase.Disposition.XXX (与 Disposition() 方法冲突)
- 补 using SWB.Player (PlayerBase 未找到)
- delta.LengthSquared2D() → delta.x*delta.x + delta.y*delta.y
EOF
)"

# 删死代码
git commit -m "$(cat <<'EOF'
cleanup(Core): 删除 IEngineEntity + EngineEntity

73 方法中仅 3 个被调用，全是 GameObject 属性转发。
BaseNPC 是 Component，直接持有 GameObject 引用。
与 NPCSchedule/NPCConditions/NPCAttributes 同样的双轨反模式。
EOF
)"
```

### 11.6 多人/多 AI 协作标记

在 body 里标注作者，方便追溯：

```
Author: 阿纳金 (architect + audit)
Author: 土豆 (executor, AA translation)
Author: AI-Session-20260506 (MaintainRelationships + EngineAITaskSystem)
```

### 11.7 为什么这样做

```
git log --oneline  →  项目进度的秒级概览
git log --grep="SKIPs:" → 找出所有引入新 SKIP 的提交
git log --grep="Source: core.lua" → 某个 Lua 文件的所有翻译历史
git diff HEAD~1 --stat → 每次提交改了什么文件
git revert <hash> → 一键回滚某个翻译块，不影响其他
```

### 11.8 禁止事项

```
❌ git add -A && git commit -m "update"        — 不可追溯
❌ git commit --amend 修改已推送的提交          — 破坏历史
❌ 一个 commit 包含翻译 + 修复 + 重构混合内容    — 无法单独回滚
❌ 翻译一半就 commit                             — 功能不完整
❌ 提交编译不过的代码                            — 破坏 bisect
```
