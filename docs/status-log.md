# VJ Base → S&Box 翻译状态日志

> 逐系统、逐方法的翻译完成状态。每次功能块完成时更新。
> 总体进度见 [phase3-progress.md](phase3-progress.md)。

**最后更新**：2026-05-11（P0+P1 全部完成 + Animation Route A 完成 + ~8 SKIP 残余 + 45 PX）

---

## schedules.lua → BaseNPC.Schedule.cs（32 个方法）

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

## base_aa.lua → BaseNPC.AA.cs（5 个方法）

| 状态 | 方法 | 行数 | 备注 |
|------|------|------|------|
| ✅ | `AA_StopMoving` | base_aa:30-41 | Rigidbody.Velocity 替代 SetLocalVelocity |
| ✅ | `AA_MoveTo` | base_aa:57-257 | ~160 行，TraceHull/地面回避/LastChasePos/加速 lerp |
| ✅ | `AA_IdleWander` | base_aa:267-353 | ~80 行，随机游荡/地面回避/加速 lerp |
| ✅ | `AA_ChaseEnemy` | base_aa:360-365 | 委托 AA_MoveTo + chase 选项 |
| ⚠️ | `AA_MoveAnimation` | base_aa:373-391 | Phase 3 空壳（动画选表/PlayAnim/ACT_*） |

## PlaySoundSystem → BaseNPC.Sound.cs（35 分支 + 辅助方法）

| 状态 | 内容 | 备注 |
|------|------|------|
| ✅ | `PlaySoundSystem(sdSet, customSD, sdType)` | core.lua:2944-3375, 35 分支完整实现 |
| ✅ | `CreateSound`, `EmitSound`, `StopSD`, `StopAllSounds` | 全部辅助方法 |
| ✅ | `GetSoundPitch`, `GetSoundDuration`, `SoundLevel` 映射 | DbToDistance + SoundFile.Load().Duration |
| ✅ | `OnPlaySound` / `OnCreateSound` / `OnEmitSound` | virtual 回调 |
| ⚠️ | `PlayFootstepSound` / `PlayIdleSound` | Phase 3 辅助音效 |

## HumanNPC init.lua → HumanNPC.Think.cs（18 方法 + 2 local 函数）

| 状态 | 方法 | 备注 |
|------|------|--------|
| ✅ | `Initialize` | ~150 行，SKIP ~28（hull/caps/flags/pose-params/hooks） |
| ✅ | `DoChangeMovementType` | 提升至 BaseNPC，NavMeshAgent/Rigidbody 映射 |
| ✅ | `SCHEDULE_ALERT_CHASE` | doLOSChase 双分支 + RunCodeOnFinish re-chase 回环 |
| ✅ | `MaintainAlertBehavior` | 人类覆写：unreachable + 武器检测 + melee range/angle |
| ✅ | `GrenadeAttack` / `ExecuteGrenadeAttack` | 完整骨架 + 骨骼/附着 |
| ✅ | `SelectSchedule` | ~275 行，武器战斗树逐行 SKIP→fill |
| ✅ | `OnTakeDamage` | A-O 15 块，免疫链 8 类型完整 |
| ✅ | `ResetEnemy` | 11 功能块，盟友继承 + 时效/距离检查 |
| ✅ | `CanFireWeapon` | IsCurrentAnim + IsMeleeWeapon 双分支 |
| ✅ | `CheckForDangers` | VJEntityFlags 真实读取 + VJDangerType 枚举 |
| ✅ | `DoChangeWeapon` | 武器库存管理 + UpdateAnimationTranslations |
| ✅ | `BeginDeath` / `FinishDeath` / `CreateDeathCorpse` | SavedDmgInfo 真实字段 + Tags 溶解守卫 |
| ✅ | `DeathWeaponDrop` / `GetAttackSpread` | 机械翻译 |
| ✅ | `PlayReloadAnimation` (local func) | PlayAnim 完整接线 |
| ✅ | `attackTimers` (local table) | ScheduleAttackTimers + ProcessAttackTimers |
| ✅ | **TranslateActivity (human override)** | 5 层战斗上下文 + SetAnimationTranslations 4 模型集 |
| ❌ | `ExecuteMeleeAttack` (覆写) | 跳过，Phase 3 |
| ❌ | `UpdatePoseParamTracking` | 已实现在 BaseNPC.Animation.cs |

## 动画系统（2026-05-11，3 新文件 ~1500 行）

详见 [animation-system-analysis.md](animation-system-analysis.md) §10。

## DamageInfo + 免疫链（2026-05-09）

| 状态 | 内容 | 备注 |
|------|------|------|
| ✅ | `VJDamageTags` 补全 | +13 tag |
| ✅ | `BaseNPC.Is*Damage` 8 helper | Bullet/Fire/Toxic/Explosive/Electric/Melee/Dissolve/Sonic |
| ✅ | 全局签名 object→DamageInfo | 全部 17 处调用方更新 |
| ✅ | `OnTakeDamage` Block A/C/E/F/J | 友好 NPC 子弹豁免/GodMode/Boss 绕过/免疫链/PreDamage |
| ✅ | 实体标志/盟友/移动类型/武器/死亡 子系统 | 完整填坑 |

## 已解决的致命/高优修复（56 项）

完整列表过长，略。详见 git log `--grep="fill\|fix\|cleanup\|field"`。

---

*此文件是 translation-guide.md §7 的扩展。翻译规范和流程见 [translation-guide.md](translation-guide.md)。*
