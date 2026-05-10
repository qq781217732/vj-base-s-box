# Phase 3 填坑进度

> **目标**：把 Phase 1 翻译产物中的 SKIP 注释逐个实现，使 NPC 能在 S&Box 中完整运行。
> **前序文档**：[translation-guide.md](translation-guide.md) — 翻译架构、Phase 1/3 定义、文件映射、Submit 规范
> **创建日期**：2026-05-10
> **最后更新**：2026-05-10（P0 全部完成，~43 SKIP 消，当前剩余 ~192）
> **数据源**：`grep -c "SKIP:" VJBase/` = 192 SKIP 行（起始 235，-43）

---
## P0 完成摘要（2026-05-10）

| 步骤 | 子系统 | SKIP 消 | 关键提交 |
|------|--------|---------|---------|
| P0-5 | VJUtility | 5 | GetNearestDistance/Positions (FindNearestPoint), DamageSpecialEnts |
| P0-3 | Collision/Bounds | 10 | OBBMins/OBBMaxs→ModelRenderer.LocalBounds, SetCollisionGroup(int), DeathCorpseCollisionType=1 |
| P0-1 | Damage/Health | 12 | SetInflictor→LIMITATION, Health守卫, IsOnFire/Ignite/Extinguish, HealthRegen, M3 CombatDamageResponse, isFireEnt守卫修复 |
| P0-4 | Timer | 7 | Bleed/Death/Alert timer→轮询 (NextBleedT/NextDeathFinishT/NextAlertResetT) |
| P0-2 | AI/Nav/Schedule | 7 | SCHEDULE_COVER_RELOAD, GuardData, VisibleCount, SetRelationshipMemory |
| 零依赖 | Flag/Difficulty/Sound/TriggerOutput | ~15 | FL_*标志, ScaleByDifficulty 17档, PlayIdleSound/FootstepSound/SoundTrack, TriggerOutput委托 |
| **P0 合计** | | **~56** | 起始 235 SKIP → 当前 ~192 |

> P0 剩余合理 SKIP：ResetEatingBehavior(P2), CombineBall(P2), Dissolve(P2), SetSaveValue(P2), MASK_WATER trace(P2), SetDamageForce(P2)

---

## P0 阻塞链路（快速视图）

```
                 ┌─────────────────────────────────────────────┐
                 │  P0-5: VJUtility 工具函数                   │
                 │  GetNearestDistance / GetNearestPositions   │
                 │  TraceDirections / DamageSpecialEnts        │
                 └──────────────┬──────────────────────────────┘
                                │ 前置依赖
        ┌───────────────────────┼───────────────────────┐
        ▼                       ▼                       ▼
┌───────────────┐    ┌──────────────────┐    ┌──────────────────┐
│ P0-3: Collision│    │ P0-1: Damage/     │    │ P1-1: Weapon      │
│ OBBMaxs/Mins  │    │ Health            │    │ Melee 距离判定    │
│ WorldSpaceAABB│    │ SetDamageForce    │    │ Grenade 落点选择  │
│ CollisionGroup│    │ HealthRegen       │    │                   │
└───────┬───────┘    │ CombatDmgResponse │    └──────────────────┘
        │            │ CombineBall       │
        │            └────────┬──────────┘
        │                     │
        ▼                     ▼
┌──────────────────────────────────────────┐
│ P0-2: AI/Nav 剩余                         │
│ DoCoverTrace OBB ← 依赖 OBBMaxs/OBBMins   │
│ SCHEDULE_COVER_ENEMY / COVER_RELOAD       │
│ SetRelationshipMemory / GuardData         │
└──────────────┬───────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────┐
│ P0-4: Timer 替换                          │
│ Bleed 定时器 / 死亡延迟 / ResetEnemy 延迟  │
│ 手雷熔丝 / attack_grenade_start           │
└──────────────────────────────────────────┘
```

> **依赖顺序**：P0-5 → P0-3 → P0-1 → P0-4 → P0-2 → P1
> 先补齐工具函数和包围盒数据源，Damage/Health 实现时就能直接用。

---

## 总体进度

| 优先级 | 子系统 | SKIP 数 | 状态 | 目标 |
|--------|-------|---------|------|------|
| **P0** | **VJUtility 缺口** | **~5** | ✅ | GetNearestDistance/TraceDirections/DamageSpecialEnts |
| **P0** | **Collision/Bounds** | **~11** | ✅ | OBB 包围盒/碰撞组 |
| **P0** | **Flag System** | **~10** | ✅ | FL_* 标志位 + 难度系统 |
| **P0** | **Damage/Health** | **~37** | ✅ | NPC 伤害/死亡核心回路 |
| **P0** | **Timer 替换** | **~8** | ✅ | 异步 timer → 轮询模式 |
| **P0** | **AI/Nav/Schedule 剩余** | **~7** | ✅ | 掩体/关系记忆/可达性 |
| *独立* | *Sound 增强* | *~15* | ✅ | PlayIdleSound/FootstepSound/SoundTrack 填充 |
| *独立* | *Entity Spawn* | *~43* | ⬜ | 可提前：API 映射，P1-1 弹体/手雷的前置 |
| P1 | Weapon 系统 | ~56 | ⬜ | 武器创建/弹体/手雷 |
| P1 | Physics/Force | ~14 | ⬜ | Rigidbody/速度/地面 |
| P1 | Player 交互 | ~6 | ⬜ | 相机/音频/玩家控制器 |
| P1 | I/O 系统 | ~2 | ✅ | TriggerOutput Scene Event 替代 |
| P2 | Animation | ~58 | ⬜ | PlayAnim/ACT_*/pose params |
| P2 | Corpse 系统 | ~32 | ⬜ | 尸体创建/褪色/碰撞 |
| P2 | Effects | ~23 | ⬜ | 粒子/弹壳/血渍/MuzzleFlash |
| P2 | Sound 增强 | ~15 | ⬜ | SoundTrack/idle sound/wep sound |
| P2 | Flag System | ~10 | ⬜ | FL_* 标志位 + 持久化 |
| P2 | 难度系统 | ~1 | ⬜ | GetConVar/vj_npc_difficulty → [Property] 替代 |
| PX | Source 永久独占 | ~10 | ➖ | gamemode.Call / hook.Call / SetNPCState / ... |
| P2 | Effects | ~23 | ⬜ | P2-2 | 粒子/弹壳/血渍/MuzzleFlash（需先有 Spawn） |
| PX | Source 永久独占 | ~10 | ➖ | — | gamemode.Call / hook.Call / SetNPCState / ... |

---

## P0: 核心逻辑层（NPC 能跑起来）

### P0-5: VJUtility 缺口（~5 SKIP）← 执行顺序第 1 步

**涉及文件**：[VJUtility.cs](VJBase/Core/VJUtility.cs)

| # | 任务 | 说明 | 被调用方 |
|---|------|------|---------|
| 1 | `GetNearestDistance(ent1, ent2, useWorldSpaceCenter)` | 两个实体间最近距离（考虑包围盒） | ExecuteMeleeAttack 距离判定、HumanNPC 多个地方 |
| 2 | `GetNearestPositions(ent1, ent2)` | 两个实体间最近点对 | 手雷投掷选点（GrenadeAttack 落点计算） |
| 3 | `TraceDirections(ent, ...)` | 多方向 Trace 扫描 | 手雷投掷选点、掩体搜索 |
| 4 | `DamageSpecialEnts(ent, dmginfo, hitgroup)` | 对特殊实体的额外伤害逻辑 | 3 处（melee/leap/weapon） |
| 5 | `Corpse_Add/Corpse_AddStinky` | 尸体列表管理（数量限制/腐烂） | CreateDeathCorpse (P2) |

> **为什么第一步**：这些工具函数是 P0-1 和 P1-1 的前置依赖。CreatureNPC 近战攻击调用 `GetNearestDistance` 做距离判定，当前返回 0 导致所有近战判定永远通过。手雷落点选择依赖 `GetNearestPositions` + `TraceDirections`。

### P0-3: Collision/Bounds（~11 SKIP）← 执行顺序第 2 步

**涉及文件**：[CreatureNPC.Think.cs](VJBase/Bases/CreatureNPC.Think.cs), [HumanNPC.Think.cs](VJBase/Bases/HumanNPC.Think.cs)

| # | 任务 | 文件:行 | 说明 |
|---|------|---------|------|
| 1 | `OBBMaxs/OBBMins` 读取 | 8 处 (死亡/伤害/距离) | `ModelRenderer.GetBounds()` 获取包围盒 |
| 2 | `SetCollisionGroup` | CreatureNPC.Think.cs:676,822, HumanNPC.Think.cs:1908 | 尸体碰撞组切换 |
| 3 | `WorldSpaceAABB` | 多处 | 世界空间轴对齐包围盒 |

> **为什么第二步**：OBBMaxs/OBBMins 是 P0-2 `DoCoverTrace` OBB 版本的数据源，也是 Damage/Health 中死亡物理和伤害距离计算的前提。

### P0-1: Damage/Health 系统（~37 SKIP）← 执行顺序第 3 步

**涉及文件**：[BaseNPC.cs](VJBase/Core/BaseNPC.cs), [HumanNPC.Think.cs](VJBase/Bases/HumanNPC.Think.cs), [CreatureNPC.Think.cs](VJBase/Bases/CreatureNPC.Think.cs), [VJBaseWeapon.cs](VJBase/Core/VJBaseWeapon.cs)

| # | 任务 | 文件:行 | Lua 源 | 说明 |
|---|------|---------|--------|------|
| 1 | `SetInflictor(self)` → S&Box 等价 | CreatureNPC.Think.cs:391, HumanNPC.Think.cs:2223, VJBaseWeapon.cs:423 | melee/leap/weapon 攻击 | S&Box DamageInfo 无 Inflictor 字段；Weapon=null 时 attacker 即 inflictor。确认当前行为正确并消除 SKIP |
| 2 | `DMG_CLUB` 伤害类型映射 | VJBaseWeapon.cs:426 | melee 攻击类型 | VJDamageTags 已有枚举体系，映射 DMG_CLUB → 对应 tag |
| 3 | `Health()/m_takedamage` 替代 | CreatureNPC.Think.cs:352, HumanNPC.Think.cs:1610,1623 | 实体血量查询 | 当前用 `float CurrentHealth` 追踪。需要决定：继续用简单 float 还是迁移到 `HealthComponent` |
| 4 | `HealthRegen` 系统 | HumanNPC.Think.cs:1596 | core.lua:3993-3995 | HealthRegenParams(Enabled, ResetOnDmg, Delay) — 受伤后延迟回血 |
| 5 | `IsOnFire` + combine ball | HumanNPC.Think.cs:1525-1560 | 火焰/电球伤害类型 | IsOnFire() 检测 + prop_combine_ball 特殊伤害缩放 + spam 防护 |
| 6 | `SavedDmgInfo` 快照 | HumanNPC.Think.cs:1590 | GMod dmginfo tick 后重置 | S&Box DamageInfo 是持久对象，可能不需要快照——确认后消除 |
| 7 | `CombatDamageResponse` M3 块 | HumanNPC.Think.cs:1688-1698 | 受伤后反制 | 受伤→检查武器→找掩体→反击的完整链条 |
| 8 | `DamageResponse` (玩家) | HumanNPC.Think.cs:1731 | 玩家对 NPC 伤害响应 | VJ_ID_Living/sightDist/Visible/ForceSetEnemy/cover |
| 9 | `MarkTookDamageFromEnemy` | HumanNPC.Think.cs:1606 | I/O 系统 | TriggerOutput stub 已存在 |
| 10 | `ResetEatingBehavior("Injured")` | HumanNPC.Think.cs:1769 | 进食系统 | CanEat + VJ_ST_Eating 受伤中断 |
| 11 | `Dissolve` 伤害路径 | HumanNPC.Think.cs:1776 | 溶解伤害 | RemoveEFlags(EFL_NO_DISSOLVE) → S&Box dissolve 系统 |
| 12 | `SetSaveValue` 持久化 | HumanNPC.Think.cs:1598 | Source save/restore | S&Box 持久化方案待定 |

> **为什么第三步**：P0-5 工具函数和 P0-3 包围盒就位后，Damage/Health 模块可以直接调用，不需要后续回头改。

### P0-4: Timer 替换（~8 SKIP）← 执行顺序第 4 步

**涉及文件**：[CreatureNPC.Think.cs](VJBase/Bases/CreatureNPC.Think.cs), [HumanNPC.Think.cs](VJBase/Bases/HumanNPC.Think.cs), [BaseNPC.Relationships.cs](VJBase/Core/BaseNPC.Relationships.cs)

| # | 任务 | 文件:行 | 替代方案 |
|---|------|---------|---------|
| 1 | Bleed 系统定时器 | CreatureNPC.Think.cs:403 | 轮询 `ProcessAttackTimers` 模式或 `async Task.Delay` |
| 2 | 死亡延迟 `timer.Simple(deathTime)` | CreatureNPC.Think.cs:717, HumanNPC.Think.cs:1946 | `async Task.Delay` 然后 FinishDeath |
| 3 | ResetEnemy 延迟 | BaseNPC.Relationships.cs:129, HumanNPC.Think.cs:1380 | 轮询 `NextAlertResetT` 字段 |
| 4 | 手雷熔丝定时器 | HumanNPC.Think.cs:1276 | 轮询或 `Task.Delay` |
| 5 | `timer.Create("attack_grenade_start")` | HumanNPC.Think.cs:2306 | 当前用轮询 `CheckWeaponState`，待评估是否切 async |

> **设计决策**：翻译阶段已对攻击系统采用轮询模式（`ProcessAttackTimers`），P0 其余 timer 是否继续统一用轮询，还是引入 `async Task.Delay`？影响一致性。
>
> **注意**：`BaseNPC.Schedule.cs:186` 的 task 失败延迟已在 `EngineAITaskSystem` 重写时一并处理（NavMeshAgent 驱动），不在 Timer 替换范围内。

### P0-2: AI/Nav/Schedule 剩余（~7 SKIP）← 执行顺序第 5 步

**涉及文件**：[BaseNPC.Relationships.cs](VJBase/Core/BaseNPC.Relationships.cs), [HumanNPC.Think.cs](VJBase/Bases/HumanNPC.Think.cs)

| # | 任务 | 文件:行 | Lua 源 | 说明 |
|---|------|---------|--------|------|
| 1 | `SCHEDULE_COVER_RELOAD` | HumanNPC.Think.cs:688 | TASK_FIND_COVER_FROM_ENEMY | 装弹时找掩体 |
| 2 | `SCHEDULE_COVER_ENEMY("TASK_RUN_PATH")` | HumanNPC.Think.cs:1697 | M3 战斗响应 | FACE_ENEMY + NextCombatDamageResponseT |
| 3 | `DoCoverTrace` OBB 版本 | HumanNPC.Think.cs:1693 | OBBCenter + EyePos | 当前基础 TraceLine 版已实现，需要包围盒版本（依赖 P0-3 OBB） |
| 4 | `GuardData.Position/Direction` | HumanNPC.Think.cs:920 | guard system | NPC 驻守位置/朝向配置 |
| 5 | `CurrentReachableEnemies` / VisibleCount | HumanNPC.Think.cs:1360-1366 | ResetEnemy | 可达敌人计数 + 条件判定 |
| 6 | `SetRelationshipMemory` | BaseNPC.cs:1058 | MEM_OVERRIDE_DISPOSITION | 关系系统记忆覆盖 |
| 7 | `VJ_SD_InvestLevel/VJ_SD_InvestTime` | BaseNPC.Relationships.cs:483 | 实体声音数据 | 调查系统声音分级 |

> **已移除项**：
> - ~~`EyePosition()` on target~~ — 实际在 `AISenses.cs` 中已处理：`BaseNPC.ViewOffset` 字段 + 组件读取 + fallback 64f 逻辑。
> - ~~`m_vecSmoothedVelocity`~~ — 当前 `rb.Velocity` 即时速度替代已可用，注释为 "Phase 3 优化平滑"，属优化而非阻塞 SKIP。

---

## P1: 战斗行为质量层

### P1-1: Weapon 系统（~56 SKIP）

**涉及文件**：[HumanNPC.Think.cs](VJBase/Bases/HumanNPC.Think.cs), [VJBaseWeapon.cs](VJBase/Core/VJBaseWeapon.cs), [BaseNPC.cs](VJBase/Core/BaseNPC.cs)

| # | 任务 | 说明 |
|---|------|------|
| 1 | `DoChangeWeapon` Give/SelectWeapon/Equip 链路 | [HumanNPC.Think.cs:98-101](VJBase/Bases/HumanNPC.Think.cs#L98) — 武器 GameObject 创建→装备→绑定 |
| 2 | `SpawnRangeProjectile` 实现 | [BaseNPC.cs:631](VJBase/Core/BaseNPC.cs#L631) — 弹体实体生成 |
| 3 | `ExecuteGrenadeAttack` 手雷 spawn | [HumanNPC.Think.cs:1258-1289](VJBase/Bases/HumanNPC.Think.cs#L1258) — 手雷实体生成+物理+熔丝 |
| 4 | `GrenadeAttack` 落点选择 | [HumanNPC.Think.cs:1238-1251](VJBase/Bases/HumanNPC.Think.cs#L1238) — 敌人位置→投掷方向/力量（依赖 P0-5 TraceDirections） |
| 5 | `NPC_Reload` 真实实现 | [VJBaseWeapon.cs](VJBase/Core/VJBaseWeapon.cs) — 弹药计数 + 装弹状态机 |
| 6 | `GetBulletPos` 真实 muzzle position | [VJBaseWeapon.cs](VJBase/Core/VJBaseWeapon.cs) — 当前用 WorldPosition + Up*60 近似 |
| 7 | `PrimaryAttackEffects` | [VJBaseWeapon.cs:249](VJBase/Core/VJBaseWeapon.cs#L249) — muzzle flash, shell eject (视觉部分 P2) |
| 8 | `BulletCallback` delegate | [VJBaseWeapon.cs](VJBase/Core/VJBaseWeapon.cs) — fire bullet / hitscan / projectile dispatch |
| 9 | `Weapon_CanCrouchAttack` / `Weapon_CrouchAttackChance` | 蹲下射击逻辑 |
| 10 | `NPC_SecondaryFire` | 副攻击模式 |
| 11 | `IsMeleeWeapon` 判定 | 当前用 tag/component 检查，待验证准确性 |

### P1-2: Physics/Force（~14 SKIP）

| # | 任务 | 说明 |
|---|------|------|
| 1 | `SetDamageForce` / `ApplyForceCenter` | S&Box Rigidbody.ApplyForce 替代 |
| 2 | `SetGroundEntity(NULL)` | Source 引擎地面实体解除 |
| 3 | `IsNextBot` / `loco:Approach/Jump/SetVelocity` | Source NextBot 移动系统 |
| 4 | `MASK_WATER` trace | [BaseNPC.AA.cs:100](VJBase/Core/BaseNPC.AA.cs#L100) — 水面检测 |
| 5 | `AddAngleVelocity` | 手雷旋转速度 |

### P1-3: Player 交互（~6 SKIP）

| # | 任务 | 说明 |
|---|------|------|
| 1 | `ViewPunch(Angle)` | 玩家受击相机抖动 — S&Box 无原生 API |
| 2 | `SetDSP(MeleeAttackDSP)` | 玩家音频 DSP 效果 — S&Box 无原生 API |
| 3 | `KeyDown(IN_RELOAD)` | [HumanNPC.Think.cs:635](VJBase/Bases/HumanNPC.Think.cs#L635) — 玩家控制 NPC 时装弹输入 |
| 4 | `isControlled && IN_ATTACK2` | [VJBaseWeapon.cs:214](VJBase/Core/VJBaseWeapon.cs#L214) — 玩家控制 NPC 时副攻击 |
| 5 | `VJController` | [Entities/VJController.cs](VJBase/Entities/VJController.cs) — 玩家控制 NPC 的相机/输入转发 |
| 6 | `FlashlightIsOn()` | [BaseNPC.Relationships.cs:586](VJBase/Core/BaseNPC.Relationships.cs#L586) — 玩家手电筒检测 |

### P1-4: I/O 系统（~2 SKIP）

| # | 任务 | 说明 |
|---|------|------|
| 1 | `TriggerOutput` / `Fire("KilledNPC")` | VJEnums 已有 TriggerOutput 字段，用 S&Box Scene 事件替代 Source I/O |
| 2 | `MarkTookDamageFromEnemy` | 同上，Scene 事件分发 |

---

## 独立可做项（不依赖 P0，可并行启动）

> 这些任务不依赖 P0/P1 的产出，可以在 P0 推进的同时独立开始。

### P2-5: Sound 增强（~15 SKIP）← 随时可做

**涉及文件**：[BaseNPC.Sound.cs](VJBase/Core/BaseNPC.Sound.cs), [HumanNPC.Think.cs](VJBase/Bases/HumanNPC.Think.cs), [VJBaseWeapon.cs](VJBase/Core/VJBaseWeapon.cs)

| # | 任务 | 当前状态 | 说明 |
|---|------|---------|------|
| 1 | `PlayIdleSound` / `PlayFootstepSound` | `BaseNPC.cs:517` 空壳 `{ }` | Think 循环已接线，填充 `Sound.Play()` 即可 |
| 2 | `EmitWeaponSound` | `HumanNPC.Think.cs:2268` 已实现基础版 | 已工作，可能需要补充 sound event 映射 |
| 3 | `NPC_ExtraFireSound` | `VJBaseWeapon.cs:157-160` 轮询驱动已有 | 字段和 [Property] 已配好，`Sound.Play()` 即可 |
| 4 | `StartSoundTrack` | `HumanNPC.Think.cs:89` SKIP | 背景音轨循环 |
| 5 | `DistantSound` / `Primary.Sound` | `VJBaseWeapon.cs:378` SKIP | 远距离枪声 |
| 6 | `OnPlaySound` / `OnCreateSound` / `OnEmitSound` 回调 | `BaseNPC.Sound.cs` 已定义 virtual | 回调钩子已存在，实体覆写即可 |

> **零依赖**：`Sound.Play()` API 已验证可用（PlaySoundSystem 35 分支已全部实现）。只是填空壳。

### P2-6: Flag System（~10 SKIP）← 应升入 P0

**涉及文件**：[BaseNPC.cs](VJBase/Core/BaseNPC.cs), [AISenses.cs](VJBase/Engine/AISenses.cs)

| # | 任务 | 说明 | 被谁消费 |
|---|------|------|---------|
| 1 | `HasEntityFlag(int flag)` 实现 | `BaseNPC.cs:1464` + `AISenses.cs:877` 当前返回 false | P0 Relationships（FL_NOTARGET）、AISenses（FL_OBJECT） |
| 2 | `FL_NOTARGET` 标志 | 不可被瞄准 | `BaseNPC.Relationships.cs:242` |
| 3 | `FL_OBJECT` 标志 | 可交互物件 | `AISenses.cs:949` |
| 4 | `FL_DISSOLVING` / `EFL_NO_DISSOLVE` | 溶解状态 | `HumanNPC.Think.cs:1776` |
| 5 | `SF_NPC_WAIT_TILL_SEEN` spawn flag | 出生后等看到玩家才激活 | `BaseNPC.cs:1470` |

> **零依赖**：纯数据系统。可以用 `[Property]` bool 字段或 `Dictionary<string, bool>` 或 bit mask。当前 `VJEntityFlags` Component 已为字符串 Key 提供支持，int 版本待补。

### P2-7: 难度系统（~1 SKIP）← 应升入 P0

| # | 任务 | 说明 |
|---|------|------|
| 1 | `ScaleByDifficulty(float)` 实现 | `BaseNPC.cs:382` 当前 pass-through。用 `[Property]` 设 Easy/Medium/Hard/Survival 四级系数，替换 Source `GetConVar("vj_npc_difficulty")` |

> **零依赖**：1 个方法 + 4 个系数。已有多处调用（StartHealth、MeleeDamage、LeapDamage）。

### P2-2: Entity Spawn / Model（~43 SKIP）← 可提前，P1-1 的前置

**涉及文件**：[BaseNPC.cs](VJBase/Core/BaseNPC.cs), [HumanNPC.Think.cs](VJBase/Bases/HumanNPC.Think.cs)

| # | 任务 | 说明 |
|---|------|------|
| 1 | `SpawnRangeProjectile` 实现 | `BaseNPC.cs:631` — `ents.Create(projectileClass)` → `GameObject.CreateObject`/`Prefab.Clone` |
| 2 | `ExecuteGrenadeAttack` spawn | `HumanNPC.Think.cs:1263` — 手雷实体创建 + SetModel + physics |
| 3 | `ents.Create` 通用等价 | 建立 Lua class → S&Box prefab/GameObject 的映射约定 |

> **部分可做**：S&Box 实体创建 API 映射是机械工作，不需要 P0 逻辑。但实际有用要等到 P1-1（弹体/手雷）和 P2-3（尸体）的消费者就位。
>
> **建议**：先做完 API 映射方案（1-2 小时），然后等到 P1-1 阶段再实际接线。

---

## P2: 视听表现层（P0/P1 完成后才能做）

> 以下任务依赖 P0/P1 或依赖 P2-2 Entity Spawn，不能提前启动。

### P2-1: Animation（~58 SKIP）

`TranslateActivity` / `PlayAnim` / `UpdatePoseParamTracking` / `ACT_*` 映射 / `AnimTbl_*` 选表 / `SetAnimationTranslations` / `PlayReloadAnimation` / `AA_MoveAnimation`

> [animation-system-analysis.md](animation-system-analysis.md) 已有完整分析和迁移路线 A/B。

### P2-2: Entity Spawn/Model（~43 SKIP）

`ents.Create` → `GameObject.CreateObject`/`Prefab.Clone` / `SetModel` → `ModelRenderer.Model` / `SetSkin` / `bodygroup` / `Dissolve` / `EntityFlags` 持久化

### P2-3: Corpse 系统（~32 SKIP）

`CreateDeathCorpse` 完整实现 / `FadeCorpseType` / `DeathCorpseFade` / `Corpse_Add` / `undo.ReplaceEntity` / `cleanup.ReplaceEntity`

### P2-4: Effects（~23 SKIP）

`MuzzleFlash` / `ShellEject` / `Particles` / `BloodDecals` / `SpawnBloodPool` / `GibOnDeath` / `Ragdoll` / `Decal` (GPU decal)

### P2-5: Sound 增强（~15 SKIP）

`StartSoundTrack` / `PlayIdleSound` / `PlayFootstepSound` / `EmitWeaponSound` / `DistantSound` / `NPC_ExtraFireSound` / `Primary.Sound`

### P2-6: Flag System（~10 SKIP）

`FL_*` 标志位系统（`FL_NOTARGET`, `FL_DISSOLVING`, `FL_OBJECT`, `EFL_NO_DISSOLVE`） / `HasEntityFlag(int)` 实现 / `SF_NPC_WAIT_TILL_SEEN` / 持久化

### P2-7: 难度系统（~1 SKIP）

| # | 任务 | 说明 |
|---|------|------|
| 1 | `GetConVar("vj_npc_difficulty")` → `[Property]` | `ScaleByDifficulty(float)` 已是 pass-through。用 S&Box `[Property]` 配置面板替代 Source CVar，让每级难度有实际缩放系数 |

---

## PX: Source 引擎永久独占（不做）

| # | 项 | 原因 |
|---|----|------|
| 1 | `gamemode.Call("OnNPCKilled")` | S&Box 无 gamemode.Call，用 Scene 事件替代 |
| 2 | `hook.Call("CreateEntityRagdoll")` | S&Box 无全局 hook 系统 |
| 3 | `SetNPCState(NPC_STATE_*)` | Source 引擎内置 NPC 状态机 |
| 4 | `GetNPCState()` | 同上 |
| 5 | `VJ_IsControllingNPC` | Source 玩家控制字段 |
| 6 | `GetMoveType()` / `MOVETYPE_PUSH` | Source 引擎移动类型系统 |
| 7 | `SetMaxLookDistance` / `SetFOV` | Source 引擎视觉参数（S&Box 用 AISenses 替代） |
| 8 | `SetLocalPos(decalPos)` | Source 引擎 GPU decal 定位 |
| 9 | `AddFrags` / `vj_npc_ply_frag` | Source 引擎分数系统 |
| 10 | `SetSaveValue` | Source 引擎 save/restore 系统 |
| 11 | `GetClass()=="npc_barnacle"` | Source 实体类型系统（S&Box 用 component/tag 识别） |

> **已移出 PX 的项目**：
> - `TriggerOutput` / `Fire("KilledNPC")` → P1-4（可用 S&Box Scene 事件替代，非永久不做）
> - `GetConVar` / `vj_npc_difficulty` → P2-7（难度系统影响 NPC 参数，用 [Property] 替代 CVar）

---

## 执行顺序

```
                     ┌─── 独立可做（与 P0 并行）───┐
                     │ P2-5 Sound   P2-6 Flag        │
                     │ P2-7 难度    P2-2 Spawn(API)  │
                     │ P1-4 I/O     P1-3 Player      │
                     └───────────────────────────────┘

P0-5 (VJUtility)      ← 第1步：工具函数是一切的基石
  │
  ├→ P0-3 (Collision)  ← 第2步：包围盒是 Cover/Damage 的数据源
  │
  ├→ P0-1 (Damage/Health) ← 第3步：伤害计算，此时能用 OBB + 工具函数
  │
  ├→ P0-4 (Timer)      ← 第4步：Bleed/Death/Reset 定时器替换
  │
  └→ P0-2 (AI/Nav)     ← 第5步：掩体/关系记忆（依赖 OBB + Timer）
        │
        └→ P1-1 (Weapon)   ← 弹体/手雷（依赖 Spawn + TraceDirections）
              │
              ├→ P1-2 (Physics)  ← 物理力/水面检测
              │
              └→ P2-3/4 (Corpse/Effects) ← 依赖 Spawn 链路
                    │
                    └→ P2-1 (Animation) ← 依赖 Model
```

| 优先级 | 周次 | 范围 | 说明 |
|--------|------|------|------|
| **并行** | — | P2-5 Sound + P2-6 Flag + P2-7 难度 + P2-2 Spawn(API) + P1-4 I/O | 零依赖，什么时候都行 |
| **P0** | Week 1 | P0-5 + P0-3 | 工具函数 + 包围盒 |
| **P0** | Week 2 | P0-1 | Damage/Health 核心回路 |
| **P0** | Week 3 | P0-4 + P0-2 | Timer 替换 + AI/Nav 剩余 |
| **P1** | Week 4-5 | P1-1 + P1-2 | Weapon + Physics（Spawn 在此阶段接线） |
| **P1** | Week 6 | P1-3 | Player 交互 |
| **P2** | Week 7+ | P2-3 → P2-4 → P2-1 | Corpse → Effects → Animation（严格按依赖链） |

## 每次提交后更新此文档

- 修改 `SKIP 数` 列的数字
- 已完成的行标记 `✅`
- 新增备注/发现
