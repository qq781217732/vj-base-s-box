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

## 7. 当前状态

> 整体进度：P0 ✅ | P1 ✅ | P2 ✅ | ~19 SKIP + 45 PX | ~98%
> 逐方法状态 → [status-log.md](status-log.md) | 剩余任务 → [phase3-progress.md](phase3-progress.md)

### 已完成的子系统

| 系统 | 关键文件 | 状态 |
|------|---------|------|
| Schedule（32/32 方法） | BaseNPC.Schedule.cs | ✅ |
| AA Movement（5/5 方法） | BaseNPC.AA.cs | ✅ |
| PlaySoundSystem（35 分支） | BaseNPC.Sound.cs | ✅ |
| HumanNPC（18/18 方法 + SelectSchedule ~275 行） | HumanNPC.Think.cs | ✅ |
| DamageInfo + 免疫链（8 类型） | 全局 | ✅ |
| 实体标志 + 盟友 + 移动类型 + 武器 Phase 1+2 | 全局 | ✅ |
| **动画系统**（Route A, ~1800 行, 3 新文件） | BaseNPC.Animation.cs + VJAnimationEnums.cs + VJAnimationMapper.cs | ✅ |

### 剩余

| 类别 | 数量 | 说明 |
|------|------|------|
| Phase 3 可执行 | ~12 | Follow/AA_MoveAnimation/OBB/Idle/Fire/Eating/Immune |
| 可延后 | ~7 | Bullseye/SetDSP/Controller |
| 效果层 | 5 项 | MuzzleFlash/ShellEject/Dissolve/GibOnDeath/BloodDecals |
| PX 永久排除 | 45 处 | 见 [px-permanent-exclusions.md](px-permanent-exclusions.md) |

### 动画系统

Route A 完整落地。详见 [animation-system-analysis.md](animation-system-analysis.md)。
已知限制 1 项（Gesture 叠加，S&Box 无 AddGesture API）。Sequence 过渡已通过 VJTransitionTable.cs 实现。不影响 NPC 行为。

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

> 动画系统的专项踩坑记录见 [animation-system-analysis.md](animation-system-analysis.md) §10。

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

> 已移至 [agent-methodology.md](agent-methodology.md)。
