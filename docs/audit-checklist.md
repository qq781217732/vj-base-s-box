# VJ-Base Lua → C# 迁移审计清单

> **自动生成**: 2026-05-05 | **数据源**: GitNexus 知识图谱
> **状态**: `[ ]` 待审计 `[/]` 进行中 `[x]` 已完成 `[-]` 不适用
> **规则**: [audit-template.md](audit-template.md)

---
## 统计面板

| 类 | Lua 符号 | ✅ PASS | ⚠️ SEMI | ❌ FAIL | ➖ N/A | 待审计 | 进度 |
|----|---------|---------|---------|---------|--------|--------|------|
| CreatureNPC | 204 | 0 | 0 | 0 | 0 | 204 | 0% |
| HumanNPC | 233 | 0 | 0 | 0 | 0 | 233 | 0% |
| TankNPC | 64 | 0 | 0 | 0 | 0 | 64 | 0% |
| TankGunner | 47 | 0 | 0 | 0 | 0 | 47 | 0% |
| BaseNPC | 78 | 0 | 0 | 0 | 0 | 78 | 0% |
| AISchedule | 10 | 0 | 0 | 1 | 9 | 0 | 10% |
| VJUtils | 47 | 0 | 0 | 0 | 0 | 47 | 0% |
| NPCHooks | 51 | 0 | 0 | 0 | 0 | 51 | 0% |
| BaseWeapon | 120 | 0 | 0 | 0 | 0 | 120 | 0% |
| **TOTAL** | **854** | **0** | **0** | **0** | **0** | **854** | **0%** |

---
## P1: CreatureNPC — `Code/VJBase/Bases/CreatureNPC.cs`

**Lua source**: 204 symbols

### 方法 Methods (47)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | PreInit | 548 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 2 | Init | 550 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 3 | OnThink | 558 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 4 | OnThinkActive | 560 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 5 | OnFollow | 608 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 6 | OnIdleDialogue | 623 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 7 | OnMedicBehavior | 655 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 8 | OnPlayerSight | 657 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 9 | OnInvestigate | 666 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 10 | OnResetEnemy | 668 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 11 | OnAlert | 670 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 12 | OnCallForHelp | 674 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 13 | OnMeleeAttack | 703 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 14 | MeleeAttackTraceOrigin | 705 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 15 | MeleeAttackTraceDirection | 710 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 16 | MeleeAttackKnockbackVelocity | 714 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 17 | OnMeleeAttackExecute | 741 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 18 | OnRangeAttack | 762 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 19 | OnRangeAttackExecute | 788 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 20 | RangeAttackProjPos | 790 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 21 | RangeAttackProjVel | 795 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 22 | Think | 1856 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 23 | RunCode_OnFinish | 2357 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 24 | MaintainPropInteraction | 2399 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 25 | ExecuteMeleeAttack | 2433 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 26 | DoMeleeAttackPlayerSpeed | 2563 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 27 | ExecuteRangeAttack | 2607 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 28 | ExecuteLeapAttack | 2655 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 29 | LeapAttackJump | 2703 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 30 | StopAttacks | 2714 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 31 | UpdatePoseParamTracking | 2736 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 32 | SelectSchedule | 2786 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 33 | ResetEnemy | 2863 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 34 | OnTakeDamage | 2933 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 35 | BeginDeath | 3170 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 36 | FinishDeath | 3295 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 37 | CreateDeathCorpse | 3309 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 38 | math_angDif | 2731 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 39 | DoBleed | 2981 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 40 | SetAutomaticFrameAdvance | 11 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 41 | MatFootStepQCEvent | 15 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 42 | Init | 27 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 43 | Initialize | 54 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 44 | Draw | 58 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 45 | DrawTranslucent | 66 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 46 | AA_StopMoving | 25 | AAMoveTo | [x] | [x] | [x] | [x] | ❌ FAIL | Claude | **GAP** — C#无此方法。Lua重置所有AA_*状态字段+SetLocalVelocity(0)。CreatureNPC.cs有AAMoveTo但无Stop |
| 47 | AA_MoveAnimation | 358 | — | [x] | [x] | [x] | [x] | ❌ FAIL | Claude | **GAP** — C#无此方法。Lua管理AA移动动画切换(Calm/Alert)，含badACTs过滤、Activity vs Sequence区分、NextMoveAnimTime跟踪 |

### ⚠️ GitNexus遗漏 — base_aa.lua 未导出的AA方法

| Lua 符号 | Lua行 | 重要性 | C# 状态 | 备注 |
|---------|-------|--------|---------|------|
| AA_MoveTo | 57 | 🔴🔴 关键 | ⚠️ SEMI | C#有AAMoveTo(Vector3,bool,string)简化版。**缺失**: entity目标支持、extraOptions(AddPos/FaceDest/FaceDestTarget/ChaseEnemy/IgnoreGround)、水生WaterLevel守卫、TraceHull寻路、LastChasePos回退系统、NaN守卫 |
| AA_IdleWander | 267 | 🔴 高 | ⚠️ SEMI | C#有AAIdleWander()无参简化版。**缺失**: playAnim/moveType/extraOptions参数、水生moveDown逻辑(WaterLevel<3强制下潜)、地面限制TraceLine |
| AA_ChaseEnemy | 360 | 🔴 高 | ❌ GAP | C#完全缺失。Lua包装AA_MoveTo(ene,playAnim,moveType,{FaceDestTarget=true,ChaseEnemy=true})，含Dead/NextChaseTime守卫 |

### ⚠️ GitNexus遗漏 — base_aa.lua 未导出的AA字段 (11个)

C# 均 **GAP**。Lua的AA_*状态字段用于跟踪移动超时/动画/位置：

| Lua 字段 | Lua行 | 默认值 | 备注 |
|---------|-------|--------|------|
| AA_NextMoveAnimTime | 9 | 0 | 动画切换计时 |
| AA_CurrentMoveAnim | 10 | false | 当前移动动画序列ID |
| AA_CurrentMoveAnimType | 11 | "Calm" | "Calm"/"Alert" |
| AA_CurrentMoveMaxSpeed | 12 | 0 | 当前移动最大速度 |
| AA_CurrentMoveTime | 13 | 0 | CurTime()+预计到达时间 |
| AA_CurrentMoveType | 14 | 0 | 0=未定义/1=Wander/2=RegMove/3=Chase |
| AA_CurrentMovePos | 15 | nil | 当前移动目标位置 |
| AA_CurrentMovePosDir | 16 | nil | 移动方向向量 |
| AA_CurrentMoveDist | 17 | -1 | 进度跟踪(-1=未跟踪) |
| AA_LastChasePos | 18 | nil | 上次追击目标(寻路回退用) |
| AA_DoingLastChasePos | 19 | false | 正在前往LastChasePos标志 |

### C# AAMoveTo 简化版 vs Lua AA_MoveTo 完整版 — 关键差异

| 维度 | Lua (57行→257行 | 200行) | C# (394行→421行 | 27行) |
|------|------|------|
| 目标类型 | entity + vector双支持 | 仅Vector3 |
| 水生NPC | WaterLevel分级守卫(≤2返回, ≤1 trace检查) | 仅用MovementType切换速度 |
| 寻路 | TraceHull完整物理检测 | 仅Ray地面检查 |
| 地面限制 | tr_check1+tr_check2双重检测, 修正endPos.z | Ray检测, 简单修正 |
| World Hit | 触发LastChasePos回退系统(DoingLastChasePos状态机) | 缺失 |
| 速度 | SetLocalVelocity(velPos) 直接设 | PhysicsBody.Velocity Lerp |
| 转向 | SetTurnTarget含offsetFacing/entity分派/FaceDestTarget | SetTurnTarget无offset |
| NaN守卫 | velTimeCur==velTimeCur检查 | 缺失 |
| 动画 | playAnim=false设AA_CurrentMoveAnim=-1; moveType变化重置动画 | playAnim参数存在但无动画逻辑 |
| 状态字段 | 更新11个AA_*字段 | 未更新任何AA_*状态字段 |

### 字段 Fields (157)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 48 | attackTimers | 1820 | — | [ ] | [ ] |  |  |  |
| 49 | propColBlacklist | 2397 | — | [ ] | [ ] |  |  |  |
| 50 | phys | 797 | — | [ ] | [ ] |  |  |  |
| 51 | attackTimers | 1820 | — | [ ] | [ ] |  |  |  |
| 52 | curTime | 1864 | — | [ ] | [ ] |  |  |  |
| 53 | selfData | 1865 | — | [ ] | [ ] |  |  |  |
| 54 | doHeavyProcesses | 1869 | — | [ ] | [ ] |  |  |  |
| 55 | pickedSD | 1876 | — | [ ] | [ ] |  |  |  |
| 56 | dur | 1877 | — | [ ] | [ ] |  |  |  |
| 57 | moveType | 1888 | — | [ ] | [ ] |  |  |  |
| 58 | moveTypeAA | 1889 | — | [ ] | [ ] |  |  |  |
| 59 | myVelLen | 1902 | — | [ ] | [ ] |  |  |  |
| 60 | dist | 1905 | — | [ ] | [ ] |  |  |  |
| 61 | moveSpeed | 1909 | — | [ ] | [ ] |  |  |  |
| 62 | velPos | 1916 | — | [ ] | [ ] |  |  |  |
| 63 | velTimeCur | 1917 | — | [ ] | [ ] |  |  |  |
| 64 | followData | 1944 | — | [ ] | [ ] |  |  |  |
| 65 | followEnt | 1945 | — | [ ] | [ ] |  |  |  |
| 66 | followIsLiving | 1946 | — | [ ] | [ ] |  |  |  |
| 67 | distToPly | 1950 | — | [ ] | [ ] |  |  |  |
| 68 | busy | 1951 | — | [ ] | [ ] |  |  |  |
| 69 | isFar | 1955 | — | [ ] | [ ] |  |  |  |
| 70 | schedule | 1969 | — | [ ] | [ ] |  |  |  |
| 71 | healthRegen | 2003 | — | [ ] | [ ] |  |  |  |
| 72 | myHP | 2005 | — | [ ] | [ ] |  |  |  |
| 73 | plyControlled | 2017 | — | [ ] | [ ] |  |  |  |
| 74 | myPos | 2018 | — | [ ] | [ ] |  |  |  |
| 75 | ene | 2019 | — | [ ] | [ ] |  |  |  |
| 76 | eneValid | 2020 | — | [ ] | [ ] |  |  |  |
| 77 | eneData | 2021 | — | [ ] | [ ] |  |  |  |
> *(127 more fields — see gitnexus-lua-reference.json)*

---
## P2: HumanNPC — `Code/VJBase/Bases/HumanNPC.cs`

**Lua source**: 233 symbols

### 方法 Methods (55)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | PreInit | 532 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 2 | Init | 534 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 3 | OnThink | 542 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 4 | OnThinkActive | 544 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 5 | OnFollow | 592 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 6 | OnIdleDialogue | 607 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 7 | OnMedicBehavior | 639 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 8 | OnPlayerSight | 641 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 9 | OnInvestigate | 662 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 10 | OnResetEnemy | 664 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 11 | OnAlert | 666 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 12 | OnCallForHelp | 670 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 13 | OnThinkAttack | 678 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 14 | OnMeleeAttack | 699 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 15 | MeleeAttackTraceOrigin | 701 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 16 | MeleeAttackTraceDirection | 706 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 17 | MeleeAttackKnockbackVelocity | 710 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 18 | OnMeleeAttackExecute | 737 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 19 | OnWeaponChange | 739 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 20 | OnWeaponCanFire | 741 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 21 | OnWeaponAttack | 743 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 22 | OnWeaponStrafe | 745 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 23 | OnWeaponReload | 747 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 24 | OnGrenadeAttack | 779 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 25 | OnGrenadeAttackExecute | 809 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 26 | OnKilledEnemy | 811 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 27 | OnAllyKilled | 813 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 28 | OnDamaged | 826 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 29 | OnBleed | 828 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 30 | OnFlinch | 848 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 31 | OnBecomeEnemyToPlayer | 850 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 32 | OnSetEnemyFromDamage | 852 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 33 | HandleGibOnDeath | 872 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 34 | OnDeath | 885 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 35 | OnDeathWeaponDrop | 887 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 36 | OnCreateDeathCorpse | 889 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 37 | CustomOnRemove | 891 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 38 | Controller_Initialize | 893 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 39 | SetAnimationTranslations | 899 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 40 | SelectSchedule | 3504 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 41 | ResetEnemy | 3824 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 42 | OnTakeDamage | 3902 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 43 | BeginDeath | 4161 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 44 | FinishDeath | 4284 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 45 | CreateDeathCorpse | 4298 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 46 | DeathWeaponDrop | 4464 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 47 | GetAttackSpread | 4495 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 48 | InitConvars | 1736 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 49 | DoBleed | 3950 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 50 | SetAutomaticFrameAdvance | 11 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 51 | MatFootStepQCEvent | 15 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 52 | Init | 27 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 53 | Initialize | 54 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 54 | Draw | 58 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 55 | DrawTranslucent | 66 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |

### 字段 Fields (178)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 56 | PICK | 897 | — | [ ] | [ ] |  |  |  |
| 57 | isFemale | 1065 | — | [ ] | [ ] |  |  |  |
| 58 | defPos | 1609 | — | [ ] | [ ] |  |  |  |
| 59 | StopSD | 1611 | — | [ ] | [ ] |  |  |  |
| 60 | CurTime | 1612 | — | [ ] | [ ] |  |  |  |
| 61 | IsValid | 1613 | — | [ ] | [ ] |  |  |  |
| 62 | GetConVar | 1614 | — | [ ] | [ ] |  |  |  |
| 63 | math_min | 1615 | — | [ ] | [ ] |  |  |  |
| 64 | math_max | 1616 | — | [ ] | [ ] |  |  |  |
| 65 | math_rad | 1617 | — | [ ] | [ ] |  |  |  |
| 66 | math_cos | 1618 | — | [ ] | [ ] |  |  |  |
| 67 | math_angApproach | 1619 | — | [ ] | [ ] |  |  |  |
| 68 | VJ_STATE_FREEZE | 1620 | — | [ ] | [ ] |  |  |  |
| 69 | VJ_STATE_ONLY_ANIMATION | 1621 | — | [ ] | [ ] |  |  |  |
| 70 | VJ_STATE_ONLY_ANIMATION_CONSTANT | 1622 | — | [ ] | [ ] |  |  |  |
| 71 | VJ_STATE_ONLY_ANIMATION_NOATTACK | 1623 | — | [ ] | [ ] |  |  |  |
| 72 | VJ_BEHAVIOR_PASSIVE | 1624 | — | [ ] | [ ] |  |  |  |
| 73 | VJ_BEHAVIOR_PASSIVE_NATURE | 1625 | — | [ ] | [ ] |  |  |  |
| 74 | VJ_MOVETYPE_GROUND | 1626 | — | [ ] | [ ] |  |  |  |
| 75 | VJ_MOVETYPE_AERIAL | 1627 | — | [ ] | [ ] |  |  |  |
| 76 | VJ_MOVETYPE_AQUATIC | 1628 | — | [ ] | [ ] |  |  |  |
| 77 | VJ_MOVETYPE_STATIONARY | 1629 | — | [ ] | [ ] |  |  |  |
| 78 | VJ_MOVETYPE_PHYSICS | 1630 | — | [ ] | [ ] |  |  |  |
| 79 | ANIM_TYPE_GESTURE | 1631 | — | [ ] | [ ] |  |  |  |
| 80 | metaEntity | 1633 | — | [ ] | [ ] |  |  |  |
| 81 | funcGetTable | 1634 | — | [ ] | [ ] |  |  |  |
| 82 | funcGetPoseParameter | 1635 | — | [ ] | [ ] |  |  |  |
| 83 | funcSetPoseParameter | 1636 | — | [ ] | [ ] |  |  |  |
| 84 | metaNPC | 1638 | — | [ ] | [ ] |  |  |  |
| 85 | funcGetEnemy | 1639 | — | [ ] | [ ] |  |  |  |
> *(148 more fields — see gitnexus-lua-reference.json)*

---
## P2: TankNPC — `Code/VJBase/Bases/TankNPC.cs`

**Lua source**: 64 symbols

### 方法 Methods (26)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | Tank_Init | 59 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 2 | Tank_GunnerSpawnPosition | 61 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 3 | Tank_OnThink | 65 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 4 | Tank_OnThinkActive | 67 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 5 | Tank_OnRunOver | 69 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 6 | GetNearDeathSparkPositions | 71 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 7 | Tank_OnInitialDeath | 80 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 8 | Tank_OnDeathCorpse | 113 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 9 | Tank_UpdateIdleParticles | 115 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 10 | Tank_UpdateMoveParticles | 126 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 11 | Init | 157 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 12 | OnTouch | 194 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 13 | Tank_RunOver | 201 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 14 | OnThink | 212 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 15 | OnThinkActive | 246 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 16 | SelectSchedule | 355 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 17 | OnDeath | 383 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 18 | OnCreateDeathCorpse | 409 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 19 | CustomOnRemove | 456 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 20 | Tank_PlaySoundSystem | 464 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 21 | PhysicsCollide | 11 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 22 | PhysicsUpdate | 13 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 23 | SCHEDULE_FACE | 26 | — | [x] | [x] | [x] | [x] | ➖ N/A | Claude | Lua空桩(return end)。坦克不转弯。C#无需覆盖(基类无此方法) |
| 24 | MaintainAlertBehavior | 28 | — | [x] | [x] | [x] | [x] | ➖ N/A | Claude | Lua空桩(return end)。坦克不追逐。C# SelectCombatSchedule替代 |
| 25 | OnDamaged | 30 | — | [x] | [x] | [x] | [x] | ❌ FAIL | Claude | **GAP** — Lua有真实伤害过滤逻辑:跳过physgun/crossbow伤害(Init阶段);近战非boss/弱伤害(<30)归零;boss强近战减半(PreDamage阶段)。C#无OnDamaged覆盖 |
| 26 | Tank_AngleDiffuse | 47 | AngleDiffuse | [x] | [x] | [x] | [x] | ✅ PASS | Claude | C#用while替代if(更鲁棒),参数/返回值等价。静态方法✅ |

### 字段 Fields (38)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 27 | runoverException | 148 | — | [ ] | [ ] |  |  |  |
| 28 | randPos | 72 | — | [ ] | [ ] |  |  |  |
| 29 | runoverException | 148 | — | [ ] | [ ] |  |  |  |
| 30 | defAng | 149 | — | [ ] | [ ] |  |  |  |
| 31 | vj_npc_melee | 151 | — | [ ] | [ ] |  |  |  |
| 32 | vj_npc_reduce_vfx | 152 | — | [ ] | [ ] |  |  |  |
| 33 | metaEntity | 154 | — | [ ] | [ ] |  |  |  |
| 34 | funcGetTable | 155 | — | [ ] | [ ] |  |  |  |
| 35 | phys | 171 | — | [ ] | [ ] |  |  |  |
| 36 | gunner | 179 | — | [ ] | [ ] |  |  |  |
| 37 | selfData | 214 | — | [ ] | [ ] |  |  |  |
| 38 | vec80z | 243 | — | [ ] | [ ] |  |  |  |
| 39 | FACE_NONE | 244 | — | [ ] | [ ] |  |  |  |
| 40 | hasMoved | 253 | — | [ ] | [ ] |  |  |  |
| 41 | myPos | 254 | — | [ ] | [ ] |  |  |  |
| 42 | tr | 255 | — | [ ] | [ ] |  |  |  |
| 43 | eneData | 266 | — | [ ] | [ ] |  |  |  |
| 44 | ene | 267 | — | [ ] | [ ] |  |  |  |
| 45 | plyControlled | 269 | — | [ ] | [ ] |  |  |  |
| 46 | enePos | 270 | — | [ ] | [ ] |  |  |  |
| 47 | angEne | 271 | — | [ ] | [ ] |  |  |  |
| 48 | angDiffuse | 272 | — | [ ] | [ ] |  |  |  |
| 49 | heightRatio | 273 | — | [ ] | [ ] |  |  |  |
| 50 | enemyIsHighUp | 274 | — | [ ] | [ ] |  |  |  |
| 51 | reverse | 281 | — | [ ] | [ ] |  |  |  |
| 52 | driveSpeed | 301 | — | [ ] | [ ] |  |  |  |
| 53 | moveVel | 302 | — | [ ] | [ ] |  |  |  |
| 54 | slopeFactor | 306 | — | [ ] | [ ] |  |  |  |
| 55 | eneValid | 359 | — | [ ] | [ ] |  |  |  |
| 56 | vec500z | 406 | — | [ ] | [ ] |  |  |  |
> *(8 more fields — see gitnexus-lua-reference.json)*

---
## P2: TankGunner — `Code/VJBase/Components/TankGunner.cs`

**Lua source**: 47 symbols

### 方法 Methods (15)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | Tank_Init | 55 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 2 | Tank_OnThink | 57 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 3 | Tank_OnThinkActive | 59 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 4 | Tank_OnPrepareShell | 61 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 5 | Tank_OnFireShell | 97 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 6 | Tank_UpdateIdleParticles | 99 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 7 | Init | 134 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 8 | OnThink | 144 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 9 | OnThinkActive | 151 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 10 | SelectSchedule | 204 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 11 | Tank_PrepareShell | 229 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 12 | Tank_FireShell | 251 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 13 | OnCreateDeathCorpse | 356 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 14 | CustomOnRemove | 364 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 15 | Tank_PlaySoundSystem | 369 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |

### 字段 Fields (32)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 16 | TANK_SHELL_STATUS_EMPTY | 123 | — | [ ] | [ ] |  |  |  |
| 17 | TANK_SHELL_STATUS_RELOADING | 124 | — | [ ] | [ ] |  |  |  |
| 18 | TANK_SHELL_STATUS_READY | 125 | — | [ ] | [ ] |  |  |  |
| 19 | vj_npc_range | 128 | — | [ ] | [ ] |  |  |  |
| 20 | vj_npc_reduce_vfx | 129 | — | [ ] | [ ] |  |  |  |
| 21 | metaEntity | 131 | — | [ ] | [ ] |  |  |  |
| 22 | funcGetTable | 132 | — | [ ] | [ ] |  |  |  |
| 23 | selfData | 152 | — | [ ] | [ ] |  |  |  |
| 24 | parent | 154 | — | [ ] | [ ] |  |  |  |
| 25 | turning | 159 | — | [ ] | [ ] |  |  |  |
| 26 | ene | 160 | — | [ ] | [ ] |  |  |  |
| 27 | myPos | 168 | — | [ ] | [ ] |  |  |  |
| 28 | enePos | 169 | — | [ ] | [ ] |  |  |  |
| 29 | angEne | 170 | — | [ ] | [ ] |  |  |  |
| 30 | angDiffuse | 171 | — | [ ] | [ ] |  |  |  |
| 31 | heightRatio | 172 | — | [ ] | [ ] |  |  |  |
| 32 | eneValid | 208 | — | [ ] | [ ] |  |  |  |
| 33 | eneData | 218 | — | [ ] | [ ] |  |  |  |
| 34 | shell | 259 | — | [ ] | [ ] |  |  |  |
| 35 | onCreateCall | 261 | — | [ ] | [ ] |  |  |  |
| 36 | calculatedVel | 267 | — | [ ] | [ ] |  |  |  |
| 37 | phys | 280 | — | [ ] | [ ] |  |  |  |
| 38 | myAng | 287 | — | [ ] | [ ] |  |  |  |
| 39 | myAngForward | 288 | — | [ ] | [ ] |  |  |  |
| 40 | muzzleFlashPos | 292 | — | [ ] | [ ] |  |  |  |
| 41 | muzzleFlash | 293 | — | [ ] | [ ] |  |  |  |
| 42 | lightFire | 298 | — | [ ] | [ ] |  |  |  |
| 43 | smokePos | 312 | — | [ ] | [ ] |  |  |  |
| 44 | smokeWhite | 313 | — | [ ] | [ ] |  |  |  |
| 45 | dust | 324 | — | [ ] | [ ] |  |  |  |
> *(2 more fields — see gitnexus-lua-reference.json)*

---
## P0: BaseNPC — `Code/VJBase/Core/BaseNPC.cs`

**Lua source**: 78 symbols

### 方法 Methods (1)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | VJ_CheckAllFourSides | 3517 | — | [x] | [x] | [x] | [x] | ❌ FAIL | Claude | **GAP** — C#无此方法。Lua在NPC周围4方向(Forward/Backward/Right/Left)做TraceLine,返回可通行方向表或位置表。sides参数控制检查哪几侧("1111"=全查)。C# VJUtils.cs可能有相关工具但BaseNPC无对应 |

### 字段 Fields (77)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 2 | positions | 3526 | — | [ ] | [ ] |  |  |  |
| 3 | metaEntity | 7 | — | [ ] | [ ] |  |  |  |
| 4 | funcGetTable | 8 | — | [ ] | [ ] |  |  |  |
| 5 | funcSetSaveValue | 9 | — | [ ] | [ ] |  |  |  |
| 6 | funcGetCycle | 10 | — | [ ] | [ ] |  |  |  |
| 7 | funcGetSequenceActivity | 11 | — | [ ] | [ ] |  |  |  |
| 8 | funcVisible | 12 | — | [ ] | [ ] |  |  |  |
| 9 | funcGetClass | 13 | — | [ ] | [ ] |  |  |  |
| 10 | metaNPC | 15 | — | [ ] | [ ] |  |  |  |
| 11 | funcGetEnemy | 16 | — | [ ] | [ ] |  |  |  |
| 12 | funcGetIdealActivity | 17 | — | [ ] | [ ] |  |  |  |
| 13 | funcGetActivity | 18 | — | [ ] | [ ] |  |  |  |
| 14 | funcGetIdealSequence | 19 | — | [ ] | [ ] |  |  |  |
| 15 | funcAddEntityRelationship | 20 | — | [ ] | [ ] |  |  |  |
| 16 | funcIsInViewCone | 21 | — | [ ] | [ ] |  |  |  |
| 17 | defPos | 23 | — | [ ] | [ ] |  |  |  |
| 18 | defAng | 24 | — | [ ] | [ ] |  |  |  |
| 19 | CurTime | 25 | — | [ ] | [ ] |  |  |  |
| 20 | IsValid | 26 | — | [ ] | [ ] |  |  |  |
| 21 | GetConVar | 27 | — | [ ] | [ ] |  |  |  |
| 22 | isnumber | 28 | — | [ ] | [ ] |  |  |  |
| 23 | isvector | 29 | — | [ ] | [ ] |  |  |  |
| 24 | isstring | 30 | — | [ ] | [ ] |  |  |  |
| 25 | tonumber | 31 | — | [ ] | [ ] |  |  |  |
| 26 | string_sub | 32 | — | [ ] | [ ] |  |  |  |
| 27 | string_find | 33 | — | [ ] | [ ] |  |  |  |
| 28 | string_left | 34 | — | [ ] | [ ] |  |  |  |
| 29 | table_concat | 35 | — | [ ] | [ ] |  |  |  |
| 30 | table_remove | 36 | — | [ ] | [ ] |  |  |  |
| 31 | bAND | 37 | — | [ ] | [ ] |  |  |  |
> *(47 more fields — see gitnexus-lua-reference.json)*

---
## P1: AISchedule — `Code/VJBase/Schedule/AISchedule.cs`

**Lua source**: 10 symbols (GitNexus) | 实际 ~32 methods (GitNexus 遗漏 27)
**审计日期**: 2026-05-05 | **审计者**: Claude

### 方法 Methods (5)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | SCHEDULE_FACE | 9 | — | [x] | [x] | [x] | [x] | ❌ FAIL | Claude | **GAP** — C#无对应。Lua构建face schedule并调用StartSchedule；AISchedule.cs只是数据类，不含schedule构建方法。应放在BaseNPC或ScheduleBuilder中 |
| 2 | RunEngineTask | 521 | — | [x] | [x] | [x] | [x] | ➖ N/A | Claude | Lua空桩函数。引擎兼容层，S&box可能不需要 |
| 3 | StartEngineSchedule | 522 | — | [x] | [x] | [x] | [x] | ➖ N/A | Claude | Lua空桩，仅设bDoingEngineSchedule=true。S&box引擎集成方式不同 |
| 4 | EngineScheduleFinish | 523 | — | [x] | [x] | [x] | [x] | ➖ N/A | Claude | Lua空桩。引擎兼容层 |
| 5 | DoingEngineSchedule | 524 | — | [x] | [x] | [x] | [x] | ➖ N/A | Claude | Lua空桩。引擎兼容层 |

### 字段 Fields (5)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 6 | VJ_MOVETYPE_AERIAL | 2 | — | [x] | [x] | ➖ N/A | Claude | Lua局部别名→枚举值。C#直接用enum |
| 7 | VJ_MOVETYPE_AQUATIC | 3 | — | [x] | [x] | ➖ N/A | Claude | Lua局部别名→枚举值 |
| 8 | VJ_MOVETYPE_STATIONARY | 4 | — | [x] | [x] | ➖ N/A | Claude | Lua局部别名→枚举值 |
| 9 | metaEntity | 6 | — | [x] | [x] | ➖ N/A | Claude | Lua meta table引用，C#不需要 |
| 10 | funcGetTable | 7 | — | [x] | [x] | ➖ N/A | Claude | Lua meta table helper，C#用反射/属性 |

### ⚠️ GitNexus 遗漏符号 (实际Lua文件中存在但未导出)

以下 ~27 个方法在 Lua 源文件中存在但 GitNexus 未捕获，均为 C# 中的 **GAP**：

| Lua 符号 | Lua行 | 重要性 | C# 状态 | 备注 |
|---------|-------|--------|---------|------|
| SCHEDULE_GOTO_POSITION | 23 | 🔴 高 | GAP | AA移动分派+地面寻路schedule |
| SCHEDULE_GOTO_TARGET | 39 | 🔴 高 | GAP | 走向目标的schedule，含TASK_FACE_TARGET |
| SCHEDULE_COVER_ENEMY | 54 | 🟡 中 | GAP | 寻找掩体+失败回退逻辑 |
| SCHEDULE_COVER_ORIGIN | 74 | 🟡 中 | GAP | 从原点找掩体+失败回退 |
| SCHEDULE_IDLE_WANDER | 102 | 🔴 高 | GAP | 空闲漫游，引用预构建的schedule_wander |
| SCHEDULE_IDLE_STAND | 107 | 🔴 高 | GAP | 空闲站立，含多层守卫条件 |
| TASK_VJ_PLAY_ACTIVITY | 116 | 🔴 高 | GAP | 自定义Activity播放任务 |
| TASK_VJ_PLAY_SEQUENCE | 142 | 🔴 高 | GAP | 自定义Sequence播放任务 |
| RunAI | 162 | 🔴🔴 关键 | GAP | **主AI循环**，每0.1s引擎调用。含移动动画处理/schedule执行/TurnData系统 |
| OnTaskFailed | 273 | 🔴 高 | GAP | 任务失败处理+timer延迟+RunCode_OnFail触发 |
| OnMovementFailed | 305 | 🟡 中 | GAP | 已注释代码引用，现由OnTaskFailed处理 |
| OnMovementComplete | 322 | 🟢 低 | GAP | 空函数，钩子点 |
| OnStateChange | 326 | 🟢 低 | GAP | 空函数，钩子点 |
| TranslateNavGoal | 330 | 🔴 高 | GAP | 导航目标翻译，对GOALTYPE_ENEMY返回敌人实时位置 |
| StartSchedule | 349 | 🔴🔴 关键 | GAP | **Schedule启动核心**。Gate守卫(静止类型/动画状态/重复schedule/碰撞检测)+TurnData设置+忽略条件+状态清理 |
| DoSchedule | 440 | 🔴 高 | GAP | 执行当前schedule的任务链 |
| StopCurrentSchedule | 446 | 🔴 高 | GAP | 停止当前schedule+清理timer/状态 |
| ScheduleFinished | 462 | 🔴 高 | GAP | Schedule完成清理: RunCode_OnFinish/条件清除/TurnData重置 |
| SetTask | 490 | 🔴 高 | GAP | 设置当前任务+记录开始时间 |
| NextTask | 498 | 🔴 高 | GAP | 推进到下一任务或完成schedule |
| OnTaskComplete | 513 | 🟢 低 | GAP | 标记当前任务完成 |
| TaskFinished | 522 | 🟡 中 | GAP | 返回CurrentTaskComplete |
| IsScheduleFinished | 532 | 🟡 中 | GAP | 检查任务完成+任务ID越界 |
| StartTask | 537 | 🟡 中 | GAP | task:Start(self) |
| RunTask | 541 | 🟡 中 | GAP | task:Run(self) |
| TaskTime | 545 | 🟢 低 | GAP | 返回CurTime()-TaskStartTime |
| schedule_wander | 93 | 🟡 中 | GAP | 预构建的漫游schedule(局部变量) |

### 🔴 关键架构问题

1. **C# 文件映射错误**: `AISchedule.cs` 实际映射 `lua/includes/modules/vj_ai_schedule.lua` (#27)，而非 `lua/vj_base/ai/schedules.lua` (#2)
2. **ENT方法无处安放**: schedules.lua 的 27 个 ENT: 方法需要宿主类，建议放在 `BaseNPC.cs` 或新建 `AI/ScheduleRunner.cs`
3. **RunAI 是整个AI系统的心脏** — 缺失意味着NPC无法自主思考/移动/转向
4. **StartSchedule Gate逻辑** (line 350-437) 极其复杂 — 静止类型守卫、动画状态守卫、重复schedule守卫、TurnData集成、忽略条件、武器状态清除 — 全部GAP

---
## P4: VJUtils — `Code/VJBase/Utilities/VJUtils.cs`

**Lua source**: 47 symbols

### 方法 Methods (11)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | PICK | 23 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 2 | SET | 38 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 3 | HasValue | 49 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 4 | STOPSOUND | 65 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 5 | CreateSound | 69 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 6 | EmitSound | 84 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 7 | GetMoveVelocity | 100 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 8 | GetMoveDirection | 121 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 9 | GetNearestPositions | 141 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 10 | GetNearestDistance | 167 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 11 | runTrace | 213 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |

### 字段 Fields (36)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 12 | trData | 207 | — | [ ] | [ ] |  |  |  |
| 13 | CurTime | 1 | — | [ ] | [ ] |  |  |  |
| 14 | IsValid | 2 | — | [ ] | [ ] |  |  |  |
| 15 | CreateSound | 3 | — | [ ] | [ ] |  |  |  |
| 16 | tonumber | 4 | — | [ ] | [ ] |  |  |  |
| 17 | string_find | 5 | — | [ ] | [ ] |  |  |  |
| 18 | string_gsub | 6 | — | [ ] | [ ] |  |  |  |
| 19 | math_round | 7 | — | [ ] | [ ] |  |  |  |
| 20 | math_floor | 8 | — | [ ] | [ ] |  |  |  |
| 21 | math_min | 9 | — | [ ] | [ ] |  |  |  |
| 22 | math_max | 10 | — | [ ] | [ ] |  |  |  |
| 23 | math_rad | 11 | — | [ ] | [ ] |  |  |  |
| 24 | math_cos | 12 | — | [ ] | [ ] |  |  |  |
| 25 | math_sin | 13 | — | [ ] | [ ] |  |  |  |
| 26 | bShiftL | 14 | — | [ ] | [ ] |  |  |  |
| 27 | funcCustom | 75 | — | [ ] | [ ] |  |  |  |
| 28 | sdID | 76 | — | [ ] | [ ] |  |  |  |
| 29 | funcCustom2 | 80 | — | [ ] | [ ] |  |  |  |
| 30 | entPos | 123 | — | [ ] | [ ] |  |  |  |
| 31 | dir | 124 | — | [ ] | [ ] |  |  |  |
| 32 | ent1NearPos | 142 | — | [ ] | [ ] |  |  |  |
| 33 | ent1Pos | 144 | — | [ ] | [ ] |  |  |  |
| 34 | ent2NearPos | 151 | — | [ ] | [ ] |  |  |  |
| 35 | entPosZ | 203 | — | [ ] | [ ] |  |  |  |
| 36 | entPosCentered | 204 | — | [ ] | [ ] |  |  |  |
| 37 | myForward | 205 | — | [ ] | [ ] |  |  |  |
| 38 | myRight | 206 | — | [ ] | [ ] |  |  |  |
| 39 | trData | 207 | — | [ ] | [ ] |  |  |  |
| 40 | resultIndex | 208 | — | [ ] | [ ] |  |  |  |
| 41 | result | 210 | — | [ ] | [ ] |  |  |  |
> *(6 more fields — see gitnexus-lua-reference.json)*

---
## P4: NPCHooks — `Code/VJBase/Core/NPCHooks.cs`

**Lua source**: 51 symbols

### 方法 Methods (2)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | CanBeEngaged | 58 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 2 | VJ_NPCPLY_DEATH | 325 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |

### 字段 Fields (49)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 3 | entInfos | 15 | — | [ ] | [ ] |  |  |  |
| 4 | ignoredNPCs | 156 | — | [ ] | [ ] |  |  |  |
| 5 | dangerEnts | 157 | — | [ ] | [ ] |  |  |  |
| 6 | grenadeEnts | 158 | — | [ ] | [ ] |  |  |  |
| 7 | grenadeGrabbableEnts | 159 | — | [ ] | [ ] |  |  |  |
| 8 | attackableEnts | 160 | — | [ ] | [ ] |  |  |  |
| 9 | destructibleEnts | 161 | — | [ ] | [ ] |  |  |  |
| 10 | points | 257 | — | [ ] | [ ] |  |  |  |
| 11 | CurTime | 1 | — | [ ] | [ ] |  |  |  |
| 12 | IsValid | 2 | — | [ ] | [ ] |  |  |  |
| 13 | GetConVar | 3 | — | [ ] | [ ] |  |  |  |
| 14 | tonumber | 4 | — | [ ] | [ ] |  |  |  |
| 15 | string_StartWith | 5 | — | [ ] | [ ] |  |  |  |
| 16 | table_remove | 6 | — | [ ] | [ ] |  |  |  |
| 17 | vj_npc_wep_ply_pickup | 8 | — | [ ] | [ ] |  |  |  |
| 18 | metaEntity | 10 | — | [ ] | [ ] |  |  |  |
| 19 | funcGetClass | 11 | — | [ ] | [ ] |  |  |  |
| 20 | funcGetTable | 12 | — | [ ] | [ ] |  |  |  |
| 21 | entInfos | 15 | — | [ ] | [ ] |  |  |  |
| 22 | ignoredNPCs | 156 | — | [ ] | [ ] |  |  |  |
| 23 | dangerEnts | 157 | — | [ ] | [ ] |  |  |  |
| 24 | grenadeEnts | 158 | — | [ ] | [ ] |  |  |  |
| 25 | grenadeGrabbableEnts | 159 | — | [ ] | [ ] |  |  |  |
| 26 | attackableEnts | 160 | — | [ ] | [ ] |  |  |  |
| 27 | destructibleEnts | 161 | — | [ ] | [ ] |  |  |  |
| 28 | entClass | 164 | — | [ ] | [ ] |  |  |  |
| 29 | entData | 165 | — | [ ] | [ ] |  |  |  |
| 30 | entInfo | 166 | — | [ ] | [ ] |  |  |  |
| 31 | isNPC | 167 | — | [ ] | [ ] |  |  |  |
| 32 | entIsVJ | 172 | — | [ ] | [ ] |  |  |  |
> *(19 more fields — see gitnexus-lua-reference.json)*

---
## P3: BaseWeapon — `Code/VJBase/Components/BaseWeapon.cs`

**Lua source**: 120 symbols

### 方法 Methods (47)

| # | Lua 符号 | Lua行 | C# 匹配 | 结构 | 时序 | 副作用 | 边界 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|------|--------|------|------|--------|------|
| 1 | Init | 171 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 2 | OnEquip | 173 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 3 | OnDeploy | 175 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 4 | OnThink | 177 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 5 | OnGetBulletPos | 179 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 6 | OnDrawWorldModel | 181 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 7 | OnAnimEvent | 183 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 8 | OnPrimaryAttack | 210 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 9 | OnPrimaryAttack_BulletCallback | 214 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 10 | NPC_SecondaryFire_BeforeTimer | 216 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 11 | NPC_SecondaryFire | 218 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 12 | OnSecondaryAttack | 240 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 13 | OnReload | 257 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 14 | OnHolster | 259 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 15 | CustomOnRemove | 261 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 16 | DecideAnimationLength | 269 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 17 | Initialize | 299 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 18 | GetCapabilities | 351 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 19 | SetDefaultValues | 355 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 20 | Equip | 383 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 21 | EquipAmmo | 442 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 22 | Deploy | 448 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 23 | GetBulletPos | 481 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 24 | Think | 521 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 25 | NPC_Think | 529 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 26 | NPC_CanFire | 543 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 27 | NPCShoot_Primary | 588 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 28 | PrimaryAttack | 647 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 29 | Callback | 748 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 30 | PrimaryAttackEffects | 807 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 31 | CanSecondaryAttack | 882 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 32 | SecondaryAttack | 886 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 33 | DoIdleAnimation | 903 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 34 | TranslateActivity | 916 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 35 | FireAnimationEvent | 938 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 36 | Reload | 950 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 37 | NPC_Reload | 986 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 38 | Holster | 994 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 39 | OnDrop | 1002 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 40 | OwnerChanged | 1006 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 41 | CanBePickedUpByNPCs | 1013 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 42 | GetWeaponCustomPosition | 1017 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 43 | MaintainWorldModel | 1031 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 44 | SetupDataTables | 1043 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 45 | DrawWorldModel | 1051 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 46 | OnRemove | 1076 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |
| 47 | SetupWeaponHoldTypeForAI | 1082 | — | [ ] | [ ] | [ ] | [ ] |  |  |  |

### 字段 Fields (73)

| # | Lua 符号 | Lua行 | C# 匹配 | 类型 | 默认值 | 判定 | 审计者 | 备注 |
|---|---------|-------|---------|------|--------|------|--------|------|
| 48 | oldShells | 297 | — | [ ] | [ ] |  |  |  |
| 49 | commonAttachmentNames | 472 | — | [ ] | [ ] |  |  |  |
| 50 | bullet | 740 | — | [ ] | [ ] |  |  |  |
| 51 | owner | 220 | — | [ ] | [ ] |  |  |  |
| 52 | spawnPos | 221 | — | [ ] | [ ] |  |  |  |
| 53 | projectile | 222 | — | [ ] | [ ] |  |  |  |
| 54 | phys | 228 | — | [ ] | [ ] |  |  |  |
| 55 | metaEntity | 284 | — | [ ] | [ ] |  |  |  |
| 56 | funcDrawModel | 285 | — | [ ] | [ ] |  |  |  |
| 57 | funcGetTable | 286 | — | [ ] | [ ] |  |  |  |
| 58 | metaNPC | 288 | — | [ ] | [ ] |  |  |  |
| 59 | funcGetActiveWeapon | 289 | — | [ ] | [ ] |  |  |  |
| 60 | metaAngle | 291 | — | [ ] | [ ] |  |  |  |
| 61 | vj_wep_muzzleflash | 293 | — | [ ] | [ ] |  |  |  |
| 62 | vj_wep_muzzleflash_light | 294 | — | [ ] | [ ] |  |  |  |
| 63 | vj_wep_shells | 295 | — | [ ] | [ ] |  |  |  |
| 64 | oldShells | 297 | — | [ ] | [ ] |  |  |  |
| 65 | replacementWep | 385 | — | [ ] | [ ] |  |  |  |
| 66 | actualWeapon | 394 | — | [ ] | [ ] |  |  |  |
| 67 | ammoType | 395 | — | [ ] | [ ] |  |  |  |
| 68 | deploySD | 455 | — | [ ] | [ ] |  |  |  |
| 69 | curTime | 460 | — | [ ] | [ ] |  |  |  |
| 70 | anim | 461 | — | [ ] | [ ] |  |  |  |
| 71 | animTime | 462 | — | [ ] | [ ] |  |  |  |
| 72 | commonAttachmentNames | 472 | — | [ ] | [ ] |  |  |  |
| 73 | customPos | 487 | — | [ ] | [ ] |  |  |  |
| 74 | bulletAttach | 493 | — | [ ] | [ ] |  |  |  |
| 75 | selfData | 534 | — | [ ] | [ ] |  |  |  |
| 76 | ownerData | 546 | — | [ ] | [ ] |  |  |  |
| 77 | ene | 547 | — | [ ] | [ ] |  |  |  |
> *(43 more fields — see gitnexus-lua-reference.json)*

---
## 审计指令

请阅读 [audit-template.md](audit-template.md) 了解单行审计标准。
