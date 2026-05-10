# VJ Base → S&Box 翻译架构指南

> **读者：土豆（执行落地）**
> **撰写：阿纳金（Anakin）**
> 这份文档记录了本项目的翻译架构、规则、踩过的坑和清单。读通再动手。

## 文档索引

| 文档 | 用途 |
|------|------|
| **[phase3-progress.md](phase3-progress.md)** | **Phase 3 填坑进度 — 当前任务的主战场** |
| [migration-master-checklist.md](migration-master-checklist.md) | 85 文件逐行状态总清单 |
| [migration-plan.md](migration-plan.md) | 迁移计划与文件清单 |
| [api-mapping.md](api-mapping.md) | M/Sw/C/X 方法签名映射 |
| [audit-template.md](audit-template.md) | 单行审计标准（4 维等价检查） |
| [animation-system-analysis.md](animation-system-analysis.md) | 动画系统 API 对照 + 迁移路线 |
| [phase2-testing-guide.md](phase2-testing-guide.md) | Phase 2 集成测试指南 |

---

## 1. 核心哲学

**Lua 方法调用 → C# 方法调用，签名 1:1，机械翻译。**

| 标记 | 含义 | 翻译阶段 | 填坑阶段 |
|------|------|---------|---------|
| `M` | Source 引擎独有 | 写 `entity.Method(args)` | 从零实现 |
| `Sw` | S&Box 有现成 API | 同上 | 方法内部调 S&Box |
| `C` | 纯 C# 逻辑 | 同上 | 自己写实现 |
| `X` | S&Box 不需要 | 跳过不翻译 | — |

### 1.1 项目阶段定义

**只有两个阶段：Phase 1（翻译）和 Phase 3（填坑）。没有 Phase 2。**

| 阶段 | 目标 | 输入 | 输出 |
|------|------|------|------|
| **Phase 1** | 机械翻译，每条 Lua 语句在 C# 有对应行 | Lua 源码 | C# 代码 + SKIP 注释 |
| **Phase 3** | 填坑，把 SKIP 注释里的方法实现 | Phase 1 产物 + S&Box API | 完整的 C# 实现 |

Phase 3 内部可以分优先级（P0 > P1 > P2），但那是**填坑阶段内部的优先级**，不是独立的项目阶段。

**Phase 3 内部优先级：**
```
P0 — 使 NPC 能跑起来的核心能力（感知、移动、攻击、伤害）
P1 — 使 NPC 行为完整（音效、UI、配置）
P2 — 非核心、可延后的能力（动画、调试、特殊武器）
```

### 1.2 "接线到空壳"的处理

当 Phase 3 方法体尚未实现，但调用方可以提前接好时：

```csharp
// ✅ 正确：保留 SKIP 注释，说明桩已存在
// SKIP: lua:3668 — UpdatePoseParamTracking(true) — Phase 3 animation (stub wired)
UpdatePoseParamTracking(true);

// ✅ 正确：方法体是空壳，注释标注 Phase 3
public virtual void UpdatePoseParamTracking(bool reset) { } // Phase 3: skeletal pose params

// ❌ 错误：删掉 SKIP，调用空壳当作"已完成"
UpdatePoseParamTracking(true);  // 无 SKIP，但方法体是 {} — 静默无行为
```

**接线到空壳 ≠ 完成填坑。** SKIP 注释必须保留直到 Phase 3 填了方法体。

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
    ├── HumanNPC.cs                 ← human shared.lua (字段+构造器, 102 行)
    ├── HumanNPC.Think.cs           ← human init.lua (逻辑方法, ~485 行)
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

> 最后更新：2026-05-11（P0+P1 全部完成 + Animation Route A 93% 完成 + SKIP ~8 残余 + PX 45 处）

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
| ✅ | `GetSoundDuration(sdSet)` | SoundFile.Load().Duration 真实时长替换硬编码 fallback |
| ✅ | `StopAllSounds()` | 停止全部 8 个活跃 SoundHandle |
| ✅ | `OnPlaySound` / `OnCreateSound` / `OnEmitSound` | virtual 回调 |
| ✅ | `SoundLevel` 映射 | DbToDistance() + handle.Distance 设置 (CreateSound/EmitSound) |
| ⚠️ | `PlayFootstepSound` / `PlayIdleSound` | Phase 3 辅助音效系统（非 PlaySoundSystem，独立的 think 循环音效） |
| ✅ | 音效配置字段 | 全部 ~200 字段（Has* / SoundTbl_* / *SoundChance / *SoundLevel / *SoundPitch / NextSoundTime_*） |

> S&Box API 映射与已知限制：
> - `CreateSound` → `Sound.Play()` + 手动 `handle.Parent = GameObject`（Phase 3 可切 `GameObject.PlaySound()` 自动 parent）
> - `EmitSound` → `Sound.Play(eventName, WorldPosition)` 不 parent，fire-and-forget
> - `SoundLevel` (dB) → `DbToDistance()` + `handle.Distance` 设置 ✅
> - `SoundDuration` → `SoundFile.Load().Duration` 真实时长 ✅
> - `math.random(1, chance)` → `Game.Random.Next(1, chance + 1)` 防 throw + 匹配 Lua inclusive-max
> - Lua `customSD` 可传 table → C# 仅 `string`，调用方需先 `PickSound()` 选好

### 7.1d HumanNPC init.lua → HumanNPC.Think.cs（18 方法 + 2 local 函数）

> init.lua:2131-4515，HumanNPC 独有逻辑：初始化/武器库存/装弹/姿态/伤害/死亡/手雷。
> 文件已拆分：HumanNPC.cs（字段+构造器+回调桩, 102 行）+ HumanNPC.Think.cs（逻辑方法, ~485 行）

| 状态 | 方法 | Lua 行 | 备注 |
|------|------|--------|------|
| ✅ | `Initialize` | 2131-2282 | ~150 行机械翻译，SKIP ~28（hull/caps/flags/pose-params/hooks） |
| ✅ | `DoChangeMovementType` | 2287-2319 | **2026-05-09 重构**：Source MoveType → S&Box NavMeshAgent/Rigidbody Component 映射。从 HumanNPC.Think.cs 提升至 BaseNPC.cs，全部 NPC 共用。地面→NavMeshAgent 开，飞行/水生→关+AA 接管，静止→关，物理→Rigidbody 开。TODO: CAP_MOVE_JUMP(动画), CAP_MOVE_SHOOT(武器) |
| ✅ | `ProcessAttackTimers` | — | 覆写：加 grenade exec 轮询 |
| ✅ | `SetWeaponState` / `GetWeaponState` | 2520-2534 | 空壳（timer-based reset Phase 3） |
| ✅ | `SCHEDULE_ALERT_CHASE` | 2340-2357 | doLOSChase 双分支 + RunCodeOnFinish re-chase 回环 |
| ✅ | `MaintainAlertBehavior` | 2359-2415 | 人类覆写：unreachable + 武器检测 + melee range/angle 判定 |
| ✅ | `GrenadeAttack` | 3070-3186 | 完整骨架（~90 行，22 SKIP：动画/骨骼/可见性/实体 parent） |
| ✅ | `ExecuteGrenadeAttack` | 3204-3331 | 完整骨架（~85 行，18 SKIP：spawn/bone/fuse/ownership） |
| ✅ | `SelectSchedule` | 3520-3838 | ~275 行机械翻译。空闲/调查/无武器战斗/避让全部翻译；C2 武器战斗树（~190 行）逐行 SKIP 标记，Phase 3 武器系统就位后展开。需 +28 字段（HumanNPC.cs）+ 10 辅助桩（BaseNPC.cs）+ 2 动画桩（VJUtility.cs） |
| ✅ | `OnTakeDamage` | 3918-4172 | ~255 行（15 块 A-O）。**2026-05-09 填坑**：Block A 友好 NPC 子弹免疫，Block B ragdoll 免伤(Velocity<=100)，Block C GodMode+dmg<=0，Block E Boss 绕过免疫链，Block F 免疫链 8 类型完整落地(Is*Damage helpers)，Block J PreDamage guard，Block M4 盟友伤害响应，Block M6 被动盟友逃跑。仍 SKIP: 免疫链细粒度(Source DMG_*)/玩家反应/combine ball/死亡 |
| ❌ | `TranslateActivity` | 2417-2466 | 纯动画（ACT_* 翻译表/ACT_INVALID/PlayAnim/AnimationTranslations）— 跳过，Phase 3 |
| ✅ | `DoChangeWeapon` | 2470-2518 | ~50 行。6 真调用（SetWeaponState/OnWeaponChange/WeaponEntity/WeaponInventory），6 SKIP（Give/Remove/SelectWeapon/Equip/EmitSound/UpdateAnimationTranslations — Phase 3 武器系统） |
| ✅ | `ResetEnemy` | 3840-3916 | ~75 行（11 功能块）。**2026-05-09 接线**：Block 1 盟友敌人继承 real Allies_Check + 条件判断，Block 8 ClearEnemyMemory 非玩家死敌清理。BaseNPC 字段 AlertTimeout(15,20)/EnemyTimeout(15)/CurrentReachableEnemies/NextAlertResetT + 4 桩 |
| ✅ | `CheckForDangers` | 3356-3403 | ~50 行。**2026-05-09 接线**：实体标志系统落地后 isDanger/isGrenade 真实读取 + VJ_ID_Grabbable/VJ_ST_Grabbed 手雷重定向 + SCHEDULE_COVER_ORIGIN。新增 HumanNPC: CanDetectDangers/DangerDetectionDistance/CanRedirectGrenades，BaseNPC: OnDangerDetected 回调 |
| ✅ | `CanFireWeapon` | 3476-3510 | ~35 行。6 真调用（OnWeaponCanFire/WeaponEntity/GetWeaponState/Enemy.Distance），2 SKIP（IsMeleeWeapon/IsCurrentAnim — Phase 3 weapon+animation）。新增 HumanNPC Weapon_MinDistance=10f，BaseNPC 签名修正 |
| ❌ | `UpdatePoseParamTracking` | 3426-3467 | 纯动画（90% 依赖 Source SetPoseParameter/GetPoseParameter/EyePos/GetAimPosition）— 跳过，Phase 3 |
| ✅ | `BeginDeath` | 4177-4298 | ~122 行（human override）。**2026-05-09**: 死亡动画 guard DMG_DISSOLVE+NavType | 
| ✅ | `FinishDeath` | 4300-4310 | ~11 行（human override）。**2026-05-09**: DMG_REMOVENORAGDOLL→Tags.Has(Dissolve) 守卫 |
| ✅ | `CreateDeathCorpse` | 4314-4482 | ~168 行（human override）。**2026-05-09**: SavedDmgInfo 真实 DamageInfo 字段 |
| ✅ | `DeathWeaponDrop` | 4484-4513 | ~30 行机械翻译（human only，8 SKIP） |
| ✅ | `GetAttackSpread` | 4515 | 1 行 `=> 0f`（Lua `return end` 等效） |
| ❌ | `ExecuteMeleeAttack` (覆写) | 2993-3058 | ~65 行，与 CreatureNPC 版差异小 — 跳过，Phase 3 |
| ✅ | `attackTimers` local table | 2536-2560 | 已由 BaseNPC.ScheduleAttackTimers + ProcessAttackTimers 替代 |
| ✅ | `playReloadAnimation` local func | 2562-2582 | protected virtual 桩（Phase 3: PlayAnim/IsVJBaseWeapon/NPC_Reload/timer） |

> 新增字段（HumanNPC.cs）：`Model`, `StartHealth`, `WeaponInventory` (含 `WeaponSlots` 子类), `WeaponInventoryStatus`, `WeaponInventory_AntiArmorList`, `WeaponInventory_MeleeList`, `Weapon_Disabled`, `Weapon_IgnoreSpawnMenu`, `Weapon_CanMoveFire`, `IdleAlwaysWander`, `AnimationTranslations`
> 
> SelectSchedule 新增字段（HumanNPC.cs, 28 个）：`Weapon_UnarmedBehavior`, `Weapon_UnarmedBehavior_Active`, `Weapon_Strafe`, `Weapon_StrafeCooldown`, `Weapon_OcclusionDelay`, `Weapon_OcclusionDelayTime`, `Weapon_OcclusionDelayMinDist`, `Weapon_MaxDistance`, `Weapon_RetreatDistance`, `Weapon_AimTurnDiff`, `Weapon_AimTurnDiff_Def`, `AnimTbl_MoveToCover`, `AnimTbl_WeaponAttack`, `AnimTbl_WeaponAttackCrouch`, `AnimTbl_WeaponAim`, `WeaponLastShotTime`, `WeaponAttackAnim`, `NextWeaponAttackT`, `NextWeaponAttackT_Base`, `NextWeaponStrafeT`, `NextMoveOnGunCoveredT`, `NextMeleeWeaponAttackT`, `NextDangerDetectionT`, `HasPoseParameterLooking`, `Weapon_CanCrouchAttack`, `Weapon_CrouchAttackChance`
> 
> SelectSchedule 新增辅助桩（BaseNPC.cs, 10 个）：`CanFireWeapon`, `DoCoverTrace`, `TranslateActivity`, `UpdatePoseParamTracking`, `PlayIdleSound`, `GetActiveWeapon`, `GetBestSoundHint`, `NearestPoint`, `SetMovementActivity`, `GetActivity` — 全部返回安全默认值
> 
> SelectSchedule 新增动画桩（VJUtility.cs, 2 个）：`AnimExists`, `IsCurrentAnim`
> 
> 2026-05-08 会话新增字段：
> - HumanNPC.cs（+5）：`CanDetectDangers=true`, `DangerDetectionDistance=400`, `CanRedirectGrenades=true`, `Weapon_MinDistance=10`, `PlayReloadAnimation`（virtual）
> - BaseNPC.cs（+8）：`AlertTimeout(15,20)`, `EnemyTimeout(15)`, `CurrentReachableEnemies`, `NextAlertResetT`, `OnDangerDetected`, `OnResetEnemy`, `MarkEnemyAsEluded`, `ClearEnemyMemory`, `GetEnemyLastKnownPos`
> - BaseNPC.cs CanFireWeapon 签名修正：`(checkState,checkLOS)` → `(checkDistance,checkDistanceOnly)`
>
> ### 7.1e DamageInfo 落地 + 免疫链（2026-05-09）
>
> > 全局替换 `object dmginfo` → `DamageInfo`，实现 OnTakeDamage 免疫链。
>
> | 状态 | 内容 | 备注 |
> |------|------|------|
> | ✅ | `VJDamageTags` 补全 | +13 tag (Dissolve/Sonic/SlowBurn/Acid/Radiation/NerveGas/Paralyze/Airboat/Buckshot/Sniper/BlastSurface/MissileDefense/EnergyBeam) |
> | ✅ | `BaseNPC.Is*Damage` 8 helper | IsBulletDamage/IsFireDamage/IsToxicDamage/IsExplosiveDamage/IsElectricDamage/IsMeleeDamage/IsDissolveDamage/IsSonicDamage |
> | ✅ | 全局签名 object→DamageInfo | BaseNPC(Flinch/SpawnBloodParticles/SpawnBloodDecals/GibOnDeath), HumanNPC(OnDamaged/OnBleed/OnSetEnemyFromDamage/OnBecomeEnemyToPlayer), CreatureNPC(BeginDeath/FinishDeath), TankNPC(OnDamaged/Tank_OnInitialDeath) |
> | ✅ | `OnTakeDamage` Block A | 友好 NPC 子弹豁免 — DamageInfo.Attacker + IsBulletDamage + VJ_NPC_Class 交集 |
> | ✅ | `OnTakeDamage` Block B | ragdoll 免伤 — dmgInflictor→Weapon, Rigidbody.Velocity<=100 + non-NPC guard |
> | ✅ | `OnTakeDamage` Block C | GodMode or dmgInfo.Damage<=0 → return 0 |
> | ✅ | `OnTakeDamage` Block E | Boss 绕过免疫链 — ForceDamageFromBosses + VJ_ID_Boss |
> | ✅ | `OnTakeDamage` Block F | 免疫链 8 类型 (Fire/Toxic/Bullet/Explosive/Dissolve/Electric/Melee/Sonic) |
> | ✅ | `OnTakeDamage` Block J | PreDamage: dmgInfo.Damage<=0 → return 0 |
> | ✅ | `TankNPC.OnDamaged` | ~15 行 Lua→C# (Physgun 免疫, Melee+Generic 过滤, Boss>=30 减半) |
> | ✅ | `CreatureNPC BeginDeath` | dmgAttacker/dmgInflictor → dmginfo.Attacker/Weapon |
> | ✅ | `ResetEnemy` | VJUtility.Rand(3,5) 替代手动 NextDouble |
>
> ### 7.1f 实体标志系统（2026-05-09）
>
> > 新建 `VJEntityFlags.cs` Component + BaseNPC 标志字段 + `HasEntityFlag` 静态 helper。
>
> | 状态 | 内容 | 备注 |
> |------|------|------|
> | ✅ | `VJEntityFlags.cs` | 新建 Component (VJ_ID_Danger/Grenade/Grabbable/Living/Attackable/Destructible/Boss + VJ_ST_Grabbed/Eating) |
> | ✅ | `BaseNPC` +9 标志字段 | VJ_ID_*/VJ_ST_* 字段 + `HasEntityFlag(GameObject, string)` 静态 helper (查 BaseNPC + VJEntityFlags) |
> | ✅ | `CheckForDangers` | isDanger/isGrenade 真实读取, VJ_ID_Grabbable/VJ_ST_Grabbed 手雷重定向, Rigidbody.Velocity, SCHEDULE_COVER_ORIGIN |
> | ✅ | `ExecuteMeleeAttack` | VJ_ID_Attackable/Destructible → 攻击目标扩展, isProp = isAttackable |
> | ✅ | `ExecuteLeapAttack` | 同上 VJ_ID_Attackable/Destructible |
>
> ### 7.1g 盟友系统（2026-05-09）
>
> > core.lua:2438-2584 → BaseNPC.cs 完整实现 + 5 处接线。
>
> | 状态 | 方法 | 备注 |
> |------|------|------|
> | ✅ | `Allies_Check(dist)` | 扫描同族/友好 NPC → `List<GameObject>?` |
> | ✅ | `Allies_Bring(form, dist, allies, limit, onlyVis)` | 集结盟友 + SetLastPosition + GOTO_POSITION/COVER_ORIGIN |
> | ✅ | `Allies_CallHelp(dist)` | 召唤盟友攻击自己的敌人 + PassiveNature 守卫 + 同族守卫 |
> | ✅ | `BaseNPC` 新增字段 | `CanReceiveOrders=true`, `IsGuard` (去重), `OpeningDoor` |
> | ✅ | `TankNPC` 去重 | `CanReceiveOrders` 从 TankNPC 移除 (继承 BaseNPC) |
> | ✅ | `ResetEnemy Block 1` | 盟友敌人继承 — 真实 Allies_Check + 时效/距离/关系检查 |
> | ✅ | `OnTakeDamage Block M4` | DamageAllyResponse — Allies_Check + Allies_Bring("Diamond") + DoReadyAlert + cooldown |
> | ✅ | `OnTakeDamage Block M6` | Passive_AlliesRunOnDamage — Allies_Check + SCHEDULE_COVER_ORIGIN + PlaySoundSystem("Alert") |
> | ✅ | `CreatureNPC BeginDeath` | 死亡盟友反应 — Allies_Check + OnAllyKilled + Allies_Bring + DoReadyAlert |
> | ✅ | `HumanNPC BeginDeath` | 死亡盟友反应 (人类版, 同上模式) |
> | ⚠️ | `Allies_CallHelp` | Animation (AnimTbl_CallForHelp/PlayAnim) 仍 SKIP — Phase 3 动画 |
>
> ### 7.1h 移动类型 + 物理/门（2026-05-09）
>
> > DoChangeMovementType 重构：Source MoveType → S&Box Component 映射。门系统落地。
>
> | 状态 | 内容 | 备注 |
> |------|------|------|
> | ✅ | `DoChangeMovementType` | 从 HumanNPC.Think.cs 提升至 BaseNPC.cs (virtual), 全部 NPC 共用 |
> | ✅ | Ground | NavMeshAgent.Enabled=true, UpdatePosition/UpdateRotation=true |
> | ✅ | Aerial/Aquatic | agent.Stop()+Enabled=false (AA system 接管 Position) |
> | ✅ | Stationary | agent.Stop()+Enabled=false |
> | ✅ | Physics | agent.Stop()+Enabled=false, Rigidbody.Enabled=true |
> | ✅ | `OpeningDoor` | BaseNPC +OpeningDoor 字段, StartSchedule 门检查 |
> | ✅ | WaterLevel() 本体 | RedSnail WaterTool 对接 — `IsPositionInsideAny` + `GetWaterHeightAt` → 0/1/2/3 |
> | ✅ | MASK_WATER trace + aquatic AA | `IsPositionInsideAny(destVec)` 临时方案(标注SKIP), AA_MoveTo ×4 aquatic守卫, AA_IdleWander ×3 aquatic守卫 |
> | ❌ | MoveType/VPhysics(3 SKIP) | MOVETYPE_STEP/VPHYSICS Source 专有 — 永久保留 |

### 7.1i Animation System — Route A 落地（2026-05-11）

> 6 个文件，~1900 行。PlayAnim / TranslateActivity / PoseParams / 27 AnimTbl_* / SequenceToActivity / FollowBone。

| 文件 | 内容 | 状态 |
|------|------|------|
| `Core/BaseNPC.Animation.cs` | PlayAnim (Activity→序列名映射→Animgraph)、TranslateActivity (表查找+ResolveAnimation+PICK)、UpdatePoseParamTracking (门控+AngleDelta+ApproachAngle+回调)、MaintainIdleAnimation、DetectPoseParameters、GetAttachmentPos/GetBoneTransform/GetShootPos/FollowBone/ParentToAttachment | ✅ |
| `Core/VJAnimationMapper.cs` | 静态工具：MapActivity(Activity→序列名)、MapActivity reverse、AnimDuration、SequenceToActivity (序列名→Activity反向查找+缓存)、AnimExists、IsCurrentAnim (3重载)、GetDirectPlayback | ✅ |
| `Core/VJAnimationEnums.cs` | Activity 枚举 (~175值)、VJAnimType、VJAnimSet | ✅ |
| `Bases/HumanNPC.Think.cs` | TranslateActivity 覆写 (5层战斗上下文: Cower/Angry/Aim/Protected/Agitated)、SetAnimationTranslations (Combine 6 holdType + Metrocop 3 holdType + Rebel/Player 桩)、27 AnimTbl_* 字段默认值 | ✅ |
| `Bases/CreatureNPC.Think.cs` | MaintainIdleAnimation Think 钩子、MaintainActivity 接线、SelectSchedule WeaponAttackState 驱动 | ✅ |
| `Core/VJBaseWeapon.cs` | NPC_CanFire FIRE_STAND + IsCurrentAnim 动画门控、UpdatePoseParamTracking 接线 | ✅ |

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
| 31 | **HumanNPC OnTakeDamage 逐行展开** — 粗骨架（~150 行稀疏注释）→ 1:1 完整翻译（A-O 15 块独立 SKIP 标记） | ✅ 2026-05-08 |
| 32 | **HumanNPC ResetEnemy** — 11 功能块机械翻译，13 处真调用，新增 AlertTimeout/EnemyTimeout 等 8 字段 | ✅ 2026-05-08 |
| 33 | **HumanNPC CanFireWeapon** — 机械翻译，BaseNPC 签名修正 (checkState,checkLOS)→(checkDistance,checkDistanceOnly)，新增 Weapon_MinDistance | ✅ 2026-05-08 |
| 34 | **HumanNPC CheckForDangers** — 机械翻译 + fix 魔法数字 → VJDangerType 枚举 | ✅ 2026-05-08 |
| 35 | **HumanNPC DoChangeWeapon** — 武器库存管理机械翻译，6 真调用/6 SKIP | ✅ 2026-05-08 |
| 36 | **HumanNPC 尾方法收尾** — GetAttackSpread + PlayReloadAnimation + attackTimers 替代标注 | ✅ 2026-05-08 |
| 37 | **HumanNPC 18/18 方法全部翻译完成** | ✅ 2026-05-08 |
| 38 | **DamageInfo 落地** — 全局 `object dmginfo`→`DamageInfo`，VJDamageTags+13，8 Is*Damage helper | ✅ 2026-05-09 |
| 39 | **OnTakeDamage 免疫链** — Block A/C/E/F/J 填坑，Boss 绕过，ragdoll 免伤 | ✅ 2026-05-09 |
| 40 | **TankNPC OnDamaged** — ~15 行 Lua→C# (Physgun 免疫/Boss 减半/Melee 过滤) | ✅ 2026-05-09 |
| 41 | **实体标志系统** — VJEntityFlags Component + HasEntityFlag helper + CheckForDangers/ExecuteMeleeAttack/ExecuteLeapAttack 接线 | ✅ 2026-05-09 |
| 42 | **盟友系统** — Allies_Check/Allies_Bring/Allies_CallHelp 完整实现 + 5 处接线 (ResetEnemy/OnTakeDamage M4+M6/死亡序列) | ✅ 2026-05-09 |
| 43 | **盟友 3 处修正** — Allies_Bring+SetLastPosition, PassiveNature+同族守卫 | ✅ 2026-05-09 |
| 44 | **门系统** — OpeningDoor 字段 + StartSchedule 门检查 | ✅ 2026-05-09 |
| 45 | **DoChangeMovementType 重构** — Source MoveType→NavMeshAgent/Rigidbody, 提升至 BaseNPC | ✅ 2026-05-09 |
| 46 | **武器系统 Phase 1** — IVJBaseWeapon 接口 + VJBaseWeapon Component + GetActiveWeapon/DoChangeWeapon/SetWeaponState/CanFireWeapon/PlayReloadAnimation/SelectSchedule C2 填坑, ~18 SKIP 消除 | ✅ 2026-05-09 |
| 47 | **死亡序列 DMG_REMOVENORAGDOLL** — SavedDmgInfo 真实字段 + Tags.Has(Dissolve) + NavType!=Climb (CreatureNPC+HumanNPC) | ✅ 2026-05-09 |
| 48 | **对照审查: DMG_REMOVENORAGDOLL/DMG_DISSOLVE 拆分** — 合并为一个 Tag 阻止了 dissolve 伤害的合法尸体创建 | ✅ 2026-05-09 |
| 49 | **对照审查: VJ_ID_Boss 窄化** — `Components.Get<TankNPC>()`→`HasEntityFlag` 通用检测 | ✅ 2026-05-09 |
| 50 | **对照审查: Allies_CallHelp 4处遗漏** — IsGuard+Visible守卫, Disposition守卫, 距离判断修正(ally→caller改为ally→ene), GOTO偏移 | ✅ 2026-05-09 |
| 51 | **对照审查: Block O Health 守卫** — 无条件调 BeginDeath→加 `CurrentHealth<=0 && !Dead` 守卫 | ✅ 2026-05-09 |
| 52 | **掩体/玩家交互/射线三子系统填坑** — DoCoverTrace TraceLine + TraceDirections + IsPlayerDetection + DoMeleeAttackPlayerSpeed + BecomeEnemyToPlayer/M2 + C2a/b/c-ii/c-iv 掩体 SKIP 消 + 血渍贴花 | ✅ 2026-05-10 |
| 53 | **水系统 WaterLevel** — RedSnail WaterTool 对接, aquatic AA_MoveTo ×4 + AA_IdleWander ×3 SKIP 消 | ✅ 2026-05-10 |
| 54 | **LookForObjects 感知物件** — FL_OBJECT 迭代循环落地, AISensedObjectsManager.Init 实装 | ✅ 2026-05-10 |
| 55 | **审查修复** — C2a FACE_ENEMY lambda/return→goto, C2c-ii wepInCoverEntLiving, Alerted 层级, 多轮审查 | ✅ 2026-05-10 |
| 56 | **OnPlayerSight 回调** — BaseNPC +virtual OnPlayerSight + MaintainRelationships 接线 | ✅ 2026-05-10 |
| 57 | **Allies_CallHelp ReceiveOrder 块** — NextChaseTime gate / SetTarget / SCHEDULE_FACE / PlaySoundSystem("ReceiveOrder") 填坑 | ✅ 2026-05-10 |
| 58 | **GetAttackTimer VJ.SET range 重载** — (float a, float b) 重载 + VJUtility.Rand(a,b)/rate | ✅ 2026-05-10 |
| 59 | **prop_ragdoll 检查** — ModelPhysics 组件替代 GetClass()=="prop_ragdoll" | ✅ 2026-05-10 |
| 60 | **IsVJBaseSNPC_Tank 检查** — Components.Get\<TankNPC\>() 替代 Source 字段 | ✅ 2026-05-10 |
| 61 | **GetEnemyLastKnownPos 填坑** — EntityMemory["enemy_pos"] / Enemy.VisiblePos 返回真实数据 | ✅ 2026-05-10 |
| 62 | **GetAimPosition 增强** — 玩家 Z 偏移 + VisibleVec 遮挡回退 + Enemy.VisiblePos 兜底 | ✅ 2026-05-10 |
| 63 | **GetHeadDirection** — BaseNPC +virtual, NPC_CanFire + HumanNPC alert chase 接线 | ✅ 2026-05-10 |
| 64 | **VisibleVec GrenadeAttack 接线** — canFlush 真实 VisibleVec 判定 | ✅ 2026-05-10 |
| 65 | **SoundLevel 衰减映射** — DbToDistance() + handle.Distance 设置 (CreateSound/EmitSound) | ✅ 2026-05-10 |
| 66 | **SoundDuration 真实时长** — SoundFile.Load().Duration 替换硬编码 fallback, 5 调用方传入音效文件 | ✅ 2026-05-10 |
| 67 | **GetBestSoundHint 注册表** — VJSoundType [Flags] 11bit + WorldSoundEvent + SoundEventRegistry (Register/GetClosestSound/过期清理) | ✅ 2026-05-10 |
| 68 | **调查系统完整接线** — bitsDanger 掩码 + Owner/Disposition 过滤 (dead NPC combat + vehicle SKIP) + SetLastPosition/OnInvestigate | ✅ 2026-05-10 |
| 69 | **武器射击 SoundEvent 注册** — NPCShoot_Primary/melee hit → Register(VJSoundType.Combat) | ✅ 2026-05-10 |
| 70 | **CreatureNPC ResetEnemy 填坑** — 1:1 对照 creature init.lua:2881-2949, 9 功能块 (ally 继承/OnResetEnemy/moveToEnemy/MarkEnemyAsEluded/ClearEnemyMemory/SCHEDULE_GOTO_POSITION) | ✅ 2026-05-10 |
| 71 | **VJBaseWeapon 武器链路 SKIP 消** — UpdatePoseParamTracking/BulletCallback delegate/PrimaryAttackEffects stub/GetBulletPos fallback | ✅ 2026-05-10 |
| 72 | **SelectSchedule C2c-ii 友军火线距离** — wepInCoverEnt.WorldPosition.Distance(bulletPos) <= 3000f | ✅ 2026-05-10 |
| 73 | **Weapon_UnarmedBehavior_Active 移至 BaseNPC** — 修复 CS0103 (基类访问派生类字段) | ✅ 2026-05-10 |
| 74 | **TankNPC crossbow_bolt** — dmginfo.Weapon.Tags.Has("crossbow_bolt") 替代 GetClass() | ✅ 2026-05-10 |
| 75 | **HumanNPC OnReload("Finish")** — vjbWep.OnReloadAction?.Invoke() 接线 | ✅ 2026-05-10 |
| 76 | **Weapon Phase 2 核心回路** — NPC_Think 接入 Think loop + C2b 遮蔽延迟/隐藏区/回退 if→elseif→else + C2c-iii 重入守卫/射击驱动/BeforeFireSound/MaintainIdleBehavior | ✅ 2026-05-10 |
| 77 | **PrimaryAttack 9 守卫/时序修复** — SetNextPrimaryFire 移至顶部 + IsReloading/CanPrimaryAttack/OnPrimaryAttack("Init")/NextSecondaryFireT 守卫 + Melee hit/miss Sound.Play + OnMeleeAttackExecute("Miss") + Class exclusion + NPC_ExtraFireSound Sound.Play + DryFireSound | ✅ 2026-05-10 |
| 78 | **NPCShoot_Primary Visibility 守卫** — npc.Enemy.Visible 阻止隔墙开火 (shared.lua:594) | ✅ 2026-05-10 |
| 79 | **NPC_CanFire isControlled/IN_ATTACK2 SKIP 留痕** — Phase 3 玩家控制器输入 (shared.lua:575) | ✅ 2026-05-10 |
| 80 | **Prop joint weld 移除** — constraint.RemoveConstraints("Weld") → FixedJoint.Destroy() + MaintainPropInteraction(ent) virtual | ✅ 2026-05-11 |
| 81 | **门系统 Phase 3 预备** — OnOpenDoor(door) virtual + StartSchedule guard 注释完善 | ✅ 2026-05-11 |
| 82 | **GetAttackTimer(range) 修正** — 删除 animDur>0 分支, 纯 Rand(a,b)/rate 对齐 Lua "discard" 语义 | ✅ 2026-05-11 |
| 83 | **prop_ragdoll 速度守卫** — isRagdoll 路径补 rb.Velocity.Length<=100, 对齐 Lua 三条件 | ✅ 2026-05-11 |
| 84 | **动画系统分析文档** — animation-system-analysis.md: 6 API + 27 AnimTbl_* + S&Box API 对照 + 迁移路线 A/B | ✅ 2026-05-11 |
| 85 | **Phase 2 集成测试指南** — phase2-testing-guide.md: 三层递进 + 9 子系统检查清单 + Bug 分类标准 | ✅ 2026-05-11 |

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

#### 已解决（2026-05-07 会话 — SelectSchedule 机械翻译）
| 文件 | 原 SKIP/缺失 | 解决方案 |
|------|---------|----------|
| `HumanNPC.Think.cs` | `SelectSchedule` 方法缺失 | ~275 行 override 机械翻译。Block A/B/C1/D 全部翻译为真实 C#；Block C2（武器战斗树 ~190 行）逐行 SKIP 标记 |
| `HumanNPC.cs` | SelectSchedule 所需字段缺失（28 个） | 武器行为配置（11）+ 动画表（4）+ 运行时状态（9）+ 动画配置（4）全部添加 |
| `BaseNPC.cs` | SelectSchedule 所需辅助方法缺失 | `CanFireWeapon` / `DoCoverTrace` / `TranslateActivity` / `UpdatePoseParamTracking` / `PlayIdleSound` / `GetActiveWeapon` / `GetBestSoundHint` / `NearestPoint` / `SetMovementActivity` / `GetActivity` — 全部 Phase 3 桩返回安全默认值 |
| `VJUtility.cs` | `AnimExists` / `IsCurrentAnim` 缺失 | 动画存在/当前动画检查桩 |

#### 已解决（2026-05-09 会话 — Weapon Phase 1 + DamageInfo + 实体标志 + 盟友 + 移动类型）
| 文件 | 原 SKIP | 解决方案 |
|------|---------|----------|
| 全局 | `object dmginfo` 占位参数 | 全部替换为 `DamageInfo`，8 BaseNPC method + 5 HumanNPC callback + 2 CreatureNPC + 2 TankNPC 签名改版 |
| `VJEnums.cs` | 免疫链 sub-type 缺失 | +13 VJDamageTags (Dissolve/Sonic/SlowBurn/Acid/Radiation/等) |
| `BaseNPC.cs` | 免疫链 8 类型全 SKIP | +8 Is*Damage helper (Fire/Toxic/Bullet/Explosive/Electric/Melee/Dissolve/Sonic) |
| `HumanNPC.Think.cs` | OnTakeDamage Block A/C/E/F/J 全 SKIP | 完整落地 → DamageInfo.Attacker/GodMode/Damage/免疫链/Boss 绕过 |
| `TankNPC.cs` | OnDamaged 全 SKIP | ~15 行翻译 (Physgun/Melee immunity + Boss halve) |
| `VJEntityFlags.cs` | 无此文件 | 新建 Component (9 个 VJ_ID_*/VJ_ST_* 标志) |
| `BaseNPC.cs` | VJ_ID_*/VJ_ST_* 字段缺失 | +9 标志字段 + `HasEntityFlag(GameObject, string)` 静态 helper |
| `HumanNPC.Think.cs` | CheckForDangers isDanger/isGrenade 硬编码 false | HasEntityFlag 真实读取 + 手雷重定向(VJ_ID_Grabbable/Velocity/GrenadeAttack) |
| `CreatureNPC.Think.cs` | ExecuteMeleeAttack/ExecuteLeapAttack VJ_ID_Attackable/Destructible SKIP | 真实读取 + isProp 判定 |
| `BaseNPC.cs` | Allies_Check/Allies_Bring/Allies_CallHelp 空壳 | 完整实现 (core.lua:2438-2584) + CanReceiveOrders/IsGuard |
| `HumanNPC.Think.cs` | ResetEnemy Block 1 + OnTakeDamage M4/M6 全 SKIP | 盟友接线 (Allies_Check/Allies_Bring/DoReadyAlert/SCHEDULE_COVER_ORIGIN) |
| `CreatureNPC.Think.cs` | BeginDeath 死亡盟友反应 SKIP | OnAllyKilled + Allies_Bring + DoReadyAlert + SetTurnTarget |
| `HumanNPC.Think.cs` | BeginDeath 死亡盟友反应 SKIP | 人类版, 同上模式 |
| `BaseNPC.Schedule.cs` | StartSchedule 门检查 SKIP | `OpeningDoor.IsValid()` 门检查 |
| `HumanNPC.Think.cs` | DoChangeMovementType 20 行全 SKIP Source caps | 重构为 NavMeshAgent/Rigidbody Component 映射, 提升至 BaseNPC |
| `CreatureNPC.Think.cs` | CreateDeathCorpse SavedDmgInfo 7 SKIP + FinishDeath DMG_REMOVENORAGDOLL + BeginDeath 死亡动画 guard | DamageInfo 真实字段 + Tags.Has(Dissolve) + NavType!=Climb |
| `HumanNPC.Think.cs` | CreateDeathCorpse SavedDmgInfo 7 SKIP + FinishDeath DMG_REMOVENORAGDOLL (含 DeathWeaponDrop) + BeginDeath 死亡动画 guard | 同上 |

#### 已解决（2026-05-11 会话 — Animation Route A + Weapon/Spawn/Misc 收尾, ~74 SKIP）

> 起始 ~94 SKIP → 当前 20 SKIP。Animation 系统从零构建 (~58 SKIP 消)。Spawn/Weapon/Misc 残余清扫 (~16 SKIP)。

| 系统 | 消 SKIP | 关键提交 |
|------|---------|---------|
| Animation Route A | ~58 | `6b821e6` PlayAnim/TranslateActivity/PoseParams/Think 钩子、`ed5d333` TranslateActivity 覆写+骨骼附着清零+翻译表、`ed4a147` 7 项对照修复、`6d05a64` SequenceToActivity+FollowBone、`eefa049` CoverLow vjseq 解析、`d846790` 27 AnimTbl_* 默认值、`f3cc619` spawnAng 覆盖修复、`dabd987` callback 返回值+FrameTime |
| Spawn | ~6 | `ae307d3` grenade spawn 回调+landDir 语义修正+Creator |
| Weapon | ~6 | `2c334ee` Initialize 武器装配+BulletCallback+Force、`0dc4b68` 重复 Equip、`a8bd332` DamageInfo Weapon |
| Animation 全部 | ~27 | 动画系统 Route A 完整落地（本会话 9 提交 ~1800 行） |
| Misc 残余 | ~8 | `d1823ec` OnNPCKilled+Bullseye+MaintainActivity、`002ba79` 死亡动画时长 |
| **合计** | **~86** | SKIP: 94 → ~8 |

#### 剩余待填（~8 SKIP）

| 类别 | 数量 | 说明 |
|------|------|------|
| Bullseye 标志 | 4 | `IsVJBaseBullseye` — CreatureNPC×2, HumanNPC×1, Relationships×1, VJBaseWeapon×1 |
| Phase 3 独占 | 2 | follow×2/eating/fire/dissolve/OBB/idle dialogue |
| AA animation | 2 | velocity tracking + AA_MoveAnimation |

#### 已解决（2026-05-10 会话 — Phase 2 清扫，20 项）
| 文件 | 原 SKIP | 解决方案 |
|------|---------|----------|
| `BaseNPC.Relationships.cs` | OnPlayerSight 回调 | `OnPlayerSight(ent)` virtual 方法 + MaintainRelationships 接线 |
| `BaseNPC.Relationships.cs` | ResetEnemy 空壳 | 1:1 对照 creature init.lua:2881-2949, 9 功能块完整实现 |
| `BaseNPC.cs` | Allies_CallHelp ReceiveOrder 块 | NextChaseTime gate + Visible→SetTarget+SCHEDULE_FACE / else→PlaySoundSystem+MaintainAlertBehavior |
| `BaseNPC.cs` | GetAttackTimer istable(mainTime) | (float a, float b) 重载 + VJUtility.Rand(a,b)/rate |
| `BaseNPC.cs` | GetEnemyLastKnownPos 返回 Zero | EntityMemory["enemy_pos"] / Enemy.VisiblePos 返回真实数据 |
| `BaseNPC.cs` | GetHeadDirection 缺失 | +virtual GetHeadDirection() (Phase 2: body forward; Phase 3: skeletal) |
| `BaseNPC.cs` | GetBestSoundHint 返回 null | 新建 SoundEventRegistry.cs (VJSoundType + WorldSoundEvent + 全局注册表) |
| `BaseNPC.Sound.cs` | SoundLevel 未映射 | DbToDistance() + handle.Distance 设置 (CreateSound/EmitSound) |
| `BaseNPC.Sound.cs` | GetSoundDuration 硬编码 | SoundFile.Load().Duration 真实时长, 5 调用方传入音效文件 |
| `HumanNPC.Think.cs` | IsVJBaseSNPC_Tank 检查 | Components.Get\<TankNPC\>() == null |
| `HumanNPC.Think.cs` | prop_ragdoll 检查 | ModelPhysics 组件检测替代 GetClass() |
| `HumanNPC.Think.cs` | GrenadeAttack VisibleVec | VisibleVec(eneData.VisiblePos) + distance 真实接线 |
| `HumanNPC.Think.cs` | 调查块全 SKIP | bitsDanger 掩码 + Owner/Disposition 过滤 + SetLastPosition/OnInvestigate |
| `HumanNPC.Think.cs` | SelectSchedule C2c-ii HitPos 距离 | wepInCoverEnt.WorldPosition.Distance(bulletPos) <= 3000f |
| `HumanNPC.cs` | OnReload("Finish") | vjbWep.OnReloadAction?.Invoke() |
| `VJBaseWeapon.cs` | NPC_CanFire GetAimPosition | 调用已有 GetAimPosition() 替代 ene.WorldPosition |
| `VJBaseWeapon.cs` | GetAimPosition 简陋 | 玩家 Z 偏移 + VisibleVec 遮挡回退 + Enemy.VisiblePos 兜底 |
| `VJBaseWeapon.cs` | UpdatePoseParamTracking/BulletCallback/PrimaryAttackEffects/GetBulletPos | 4 SKIP 一次性消 (stub 调用/delegate/fallback) |
| `TankNPC.cs` | crossbow_bolt GetClass() | dmginfo.Weapon.Tags.Has("crossbow_bolt") tag 检测 |
| `BaseNPC.cs` | Weapon_UnarmedBehavior_Active CS0103 | 从 HumanNPC 移至 BaseNPC, 修复基类编译错误 |

#### 已解决（2026-05-10/11 会话 — Weapon Phase 2 完整闭环 + PrimaryAttack 守卫修复 + Prop/Door 填坑, 10 项）
| 文件 | 原 SKIP | 解决方案 |
|------|---------|----------|
| `CreatureNPC.Think.cs` | NPC_Think 从未被调用 | Think loop 末尾调用 GetActiveWeapon() → NPC_Think() |
| `HumanNPC.Think.cs` | C2b 遮蔽延迟/隐藏区/回退全 SKIP | 1:1 对照 init.lua:3635-3649: AimOcclusion+MaintainIdleBehavior+NextChaseTime / 隐藏区 goto_checkwep / else WeaponAttackState>=Fire→None+MaintainAlertBehavior |
| `HumanNPC.Think.cs` | C2c-iii 重入无守卫 + BeforeFireSound SKIP | if WeaponAttackState!=FireStand && !=Aim 守卫 + EmitWeaponSound(wepComp) |
| `VJBaseWeapon.cs` | PrimaryAttack 9 项缺失 | SetNextPrimaryFire 移至顶部 + IsReloading/CanPrimaryAttack/OnPrimaryAttack("Init")/NextSecondaryFireT 守卫 + Melee hit/miss Sound.Play + OnMeleeAttackExecute("Miss") + Class exclusion (VJ_NPC_Class) + DryFireSound + OnPrimaryAttack("PostFire") |
| `VJBaseWeapon.cs` | NPCShoot_Primary Visibility 缺失 | npc.Enemy.Visible 守卫 (shared.lua:594) |
| `CreatureNPC.Think.cs` | constraint.RemoveConstraints("Weld") SKIP | FixedJoint.Destroy() + MaintainPropInteraction(ent) virtual |
| `BaseNPC.cs` | OpeningDoor 门系统 | OnOpenDoor(door) virtual + StartSchedule guard 注释完善 |
| `BaseNPC.cs` | GetAttackTimer(range) animDur>0 分支 | 删除多余分支, 纯 Rand(a,b)/rate 对齐 Lua "discard" 语义 |
| `HumanNPC.Think.cs` | prop_ragdoll 无速度守卫 | isRagdoll 路径补 rb.Velocity.Length<=100, 对齐 Lua 三条件 |
| `VJBaseWeapon.cs` | NPC_CanFire isControlled 守卫缺失 | 标注 SKIP: Phase 3 玩家控制器输入 (shared.lua:575) |

#### 剩余待填（Phase 3 独占，~150 SKIP）
| 系统 | 数量 | 说明 |
|------|------|------|
| 动画 | 0 | ✅ Route A 完整落地（PlayAnim/TranslateActivity/PoseParams/27 AnimTbl/4 模型集） |
| 实体生成/Model | ~40 | ents.Create/SetModel/SetSkin/bodygroup/Dissolve |
| 物理/力 | ~15 | SetDamageForce/GetPhysicsObject/ApplyForceCenter |
| 玩家/DSP/特效 | ~10 | ViewPunch/SetDSP/MuzzleFlash |
| Source 永久独占 | ~10 | IsNextBot/gamemode.Call/SetNPCState/hook.Call |
| 调试/convars | ~5 | VJ_DEBUG/convar/PrintMessage |
| 其他 (timer/attachment/碰撞等) | ~70 | timer.Create/武器附件/碰撞组/TriggerOutput |

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
| `Core/IVJBaseWeapon.cs` | weapon_vj_base/shared.lua SWEP 契约 | ~23 | ✅ Phase 1 |
| `Core/VJBaseWeapon.cs` | weapon_vj_base/shared.lua SWEP 默认实现 | ~41 | ✅ Phase 1 |
| `Entities/VJEntityFlags.cs` | 实体标志系统 | ~23 | ✅ |
| `Core/SoundEventRegistry.cs` | VJSoundType 11bitflags + WorldSoundEvent + 全局注册表 (Register/GetClosestSound) | ~80 | ✅ Phase 2 |
| `Core/BaseNPC.Animation.cs` | PlayAnim/TranslateActivity/ResolveAnimation/PoseParams/骨骼附着 helpers/FollowBone/ParentToAttachment/MaintainIdleAnimation | ~700 | ✅ 2026-05-11 |
| `Core/VJAnimationMapper.cs` | MapActivity/AnimDuration/SequenceToActivity(反向查找+缓存)/AnimExists/IsCurrentAnim/GetDirectPlayback | ~150 | ✅ 2026-05-11 |
| `Core/VJAnimationEnums.cs` | Activity 枚举 (~175值)/VJAnimType/VJAnimSet | ~200 | ✅ 2026-05-11 |

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
做到哪：~98%。Animation Route A 完整落地 (PlayAnim/TranslateActivity/PoseParams/SequenceToActivity/FollowBone)。
        Weapon/Spawn/Misc 全部清扫完毕。SKIP: 235 → 20。
        剩余 20 SKIP：Bullseye 标志(4) + Source PX(4) + Phase 3 独占 follow/eating/fire/dissolve(10) + AA animation(2)。
怎么验：git log --oneline -20 秒级概览（§11 Git 提交规范）
        python verify_api_mapping.py 交叉验证 Lua↔文档
        grep -rn "SKIP:" VJBase/ | wc -l  → 应为 20
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

**坑 14: Route A 适配不是"删掉重写"。** `dp.Play()` 替代 `StartSchedule(TASK_VJ_PLAY_*)` 是播放方式变化，不是删除功能。锁定计时器（AnimLockTime/NextChaseTime/NextIdleTime）仍需 1:1 维护——它们才是行为门控的核心。

**坑 15: SequenceToActivity 需要反向查询。** Lua 的 `VJ.SequenceToActivity(self, "walkeasy_all")` 调用 Source `GetSequenceActivity(LookupSequence(name))` 查询引擎内部活动映射表。S&Box 无此数据。需要运行时扫描 `SequenceNames` + 反向匹配 `Activity→序列名` 映射表。不存在时返回 null 让调用方 fallback，不能硬编码。

**坑 16: AnimTbl_* 默认值不能为空列表。** Phase 1 翻译只建了字段壳（`= new()`），必须填入 Lua 默认值。空列表 → `VJUtility.PICK(空) → null → PlayAnim 返回 Invalid`，所有动画静默跳过，没有任何编译错误或运行时异常。

**坑 17: IsBusy 空壳让动画锁全部失效。** `IsBusy()` 返回 false 意味着 NPC 永远不忙——动画播放期间 SelectSchedule 可以随时抢走控制权。必须检查 `PauseAttacks`/`AnimLockTime`/`AttackAnimTime`。

**坑 18: TranslateActivity 不是简单 key→value 查表。** HumanNPC 覆写有 5 层前置 if/elseif 判断（Cower/Angry/Aim-Move/Protected/Agitated），必须严格按 Lua 分支顺序实现，否则战斗动画选择错误。

**坑 19: "还原度"评估必须有对照表。** 笼统的百分比（60%/88%/93%）没有意义。必须列出每个 Lua 方法/块的 C# 对应行和差异点，否则评估是自欺欺人。

### 10.5 机械翻译 vs 自己造（红线）

| 可以 | 不可以 |
|------|--------|
| Lua 调用 → C# 调用，签名 1:1 | 觉得某行不重要就跳过 |
| 翻译不了写 `// SKIP:` 留痕 | 用 Source C++ 逻辑替代 Lua 逻辑 |
| 空壳标 `// Phase 3:` | 自创 Simpler 版本（如 ScanForEnemy） |
| C# 枚举 1:1 对 Lua 常量 | 编造 Lua 里不存在的枚举值 |

**如果 Lua 有，就翻译。Lua 没有的，Phase 3 才造。**

### 10.6 当前优先级（土豆看这）

> **Phase 1 翻译 + Phase 2 清扫 + Animation Route A 全部完成。SKIP: 235 → 20。**
> **剩余 20 SKIP 全部是 Phase 3 独占或 Source PX，无阻塞项。**

```
1-29. ✅ 全部完成  ← 2026-05-06 ~ 2026-05-11

30. ✅ Animation Route A 完整落地  ← 2026-05-11
    PlayAnim (Activity→序列名→Animgraph) / TranslateActivity (5层战斗上下文+表查找)
    UpdatePoseParamTracking (门控+AngleDelta+ApproachAngle+FrameTime+回调)
    SequenceToActivity (序列名→Activity 反向查找+缓存) / FollowBone (GetBoneObject)
    SetAnimationTranslations (Combine 6 holdType + Metrocop 3 holdType + Rebel/Player 桩)
    27 AnimTbl_* 字段默认值 / MaintainIdleAnimation Think 钩子

31. ✅ Weapon/Spawn/Misc 残余全部清扫  ← 2026-05-11
    OnNPCKilled 静态事件 / Initialize 武器初始装配 / BulletCallback dmginfo 修复
    Bullseye VJ_IsBeingControlled 守卫 / MaintainActivity 接线
    grenade spawn 回调 + landDir 语义修正 + Creator 字段

剩余 20 SKIP（全部非阻塞）:
    - Bullseye IsVJBaseBullseye 标志 (4)
    - Source PX: SetDSP/SetMoveType/RemoveEffects/Animgraph (4)
    - Phase 3 独占: follow/eating/fire/dissolve/OBB/idle dialogue/tool (10)
    - AA animation: velocity tracking + AA_MoveAnimation (2)
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

*最后更新：2026-05-11*
*翻译阶段 ~98%。Animation Route A 完整落地 (PlayAnim/TranslateActivity/PoseParams/27 AnimTbl_*/SequenceToActivity/FollowBone) + Weapon/Spawn/Misc 全部清扫。SKIP: 235 → 20。剩余 20 全部非阻塞 (Bullseye 4 + Source PX 4 + Phase 3 独占 10 + AA animation 2)。*

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

---

## 12. Agent 协作翻译方法论

> 总结自 2026-05-07 SelectSchedule 翻译会话。核心原则：**Agent 负责"看"，主 Agent 负责"写"。**

### 12.1 三阶段流程

```
Phase 1: 探索（并行 3 Agent，信息收集）
  Agent 1: 读 Lua 源码 — 分支结构、方法调用、条件检查、行号
  Agent 2: 读 C# 现有代码 — 已有模式、字段签名、接口、override 关系
  Agent 3: 追溯辅助调用 — 缺失桩、VJ.* 映射、SKIP 归属系统

  → 3 个 Agent 同时跑，互不依赖。每个返回结构化报告。

Phase 2: 设计（1 Plan Agent，方案产出）
  喂入 Phase 1 全部发现 → 字段清单、方法结构、goto 处理、SKIP 分块估算
  Plan Agent 产出比人工更完整（不易遗漏边界情况）

Phase 3: 执行 + 自审（主 Agent 直接操作，不委托）
  代码由主 Agent 逐行编写。Agent 不参与"写"——翻译质量依赖逐行注意力，不能委托。
  写完立即执行 §5.5 提交前自审。
```

### 12.2 质量门：§5.5 逐行对照

```
1. 打开 Lua 源文件（左）+ C# 文件（右）
2. 从第一个 Lua 行开始，确认每个行号在 C# 里都有对应（翻译 或 SKIP）
3. 逐行检查 4 项：
   ├─ 分支结构一致 (if/else/elseif)
   ├─ 参数顺序一致
   ├─ 变量语义一致 (存目标值 ≠ 存当前值)
   └─ 无死代码 (写入永不读取的字段/字典)
```

**反例（今天踩的坑）：** SelectSchedule C2 块第一次用 5 行概要注释覆盖了 ~200 行 Lua，违反 1:1 映射。用户指出后才拆成逐行 SKIP。如果写完就做逐行对照，这个问题在第一次提交前就能发现。

### 12.3 常见陷阱

| 陷阱 | 表现 | 防范 |
|------|------|------|
| **概要注释** | 用 1 个 SKIP 覆盖 50 行 Lua | 每个 Lua 行号必须有独立 C# 行 |
| **Agent 产出信任过高** | Agent 说 "~40 SKIP"，实际需要 ~110 行 | 基于 Lua 源码自己数行数，不依赖估算 |
| **C2 大块被塞进 else** | 武器战斗树 ~200 行全标 SKIP 但有复杂分支结构 | 分支骨架必须保留（if/elseif/else/标签），即使内部全是 SKIP |
| **字段默认值编造** | `Weapon_CrouchAttackChance=3` 实际 Lua=2 | 每个字段默认值去 Lua 源文件 grep 确认 |

### 12.4 效率数据

| 阶段 | 耗时 | 方式 |
|------|------|------|
| 探索（3 Agent 并行） | ~2 分钟 | 自动化 |
| 设计（1 Plan Agent） | ~1 分钟 | 自动化 |
| 编写（主 Agent） | ~15 分钟 | 手动 |
| 自审（逐行对照） | ~5 分钟 | 手动 |
| **总计** | **~23 分钟** | 320 行 Lua 翻译 |
