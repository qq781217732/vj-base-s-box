# VJ-Base → S&Box 迁移清单

> 估算基准: 需迁移 ~13,700 行 Lua → ~10,000 ± 2,000 行 C#
> 实际产出: ~4,000 行 C# (16文件)，覆盖核心路径，边缘case待补
> 不迁移: ~10,800 行 (武器/Tool/UI/特效/翻译/测试)

---

## 1. BaseNPC Component (Think循环、状态机、感知、关系、敌人选择)
**估算: 2,500 Lua → 2,000-3,000 C# | 实际: ~1,200 行 (60%)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `ENT:OnThink()` / `RunAI()` | `BaseNPC.OnUpdate()` → UpdateSenses → SelectSchedule | BaseNPC:292 |
| `ENT:SelectSchedule()` | `BaseNPC.SelectSchedule()` (战斗/追逐/闲逛/警戒决策) | BaseNPC:449 |
| `ENT:MaintainRelationships()` | `BaseNPC.UpdateSenses()` (扫描敌人/可见性/超时/记忆) | BaseNPC:360 |
| `ENT:Visible(ent)` | `BaseNPC.CanSee()` (视线锥+射线) | BaseNPC:141 |
| `ENT:IsInViewCone(point)` | `BaseNPC.IsInViewCone()` | BaseNPC:163 |
| `ENT:SetTurnTarget()` | `BaseNPC.SetTurnTarget()` (实体/位置/持续旋转) | BaseNPC:199 |
| `ENT:FaceTarget()` | `BaseNPC.FaceTarget()` (异步平滑旋转) | BaseNPC:217 |
| `ENT:GetAimPosition()` | `BaseNPC.GetAimPosition()` (瞄准+预判) | BaseNPC:507 |
| `ENT:GetAimSpread()` | `BaseNPC.GetAimSpread()` (距离+移动+压制) | BaseNPC:527 |
| `ENT:ScaleByDifficulty()` | `BaseNPC.ScaleByDifficulty()` | BaseNPC:252 |
| `ENT:TakeDamage()` / `OnDamage()` | `BaseNPC.OnDamage()` (IDamageable接口+反击) | BaseNPC:255 |
| `ENT:SetModel()` | `BaseNPC.SetModel()` | BaseNPC:93 |
| `ENT:EyePos()` / `ENT:GetShootPos()` | `BaseNPC.EyePosition` / `ShootPosition` | BaseNPC:68 |
| `ENT:WorldSpaceCenter()` | `BaseNPC.WorldSpaceCenter` | BaseNPC:80 |
| 条件系统 `SetCondition`/`HasCondition` | `HashSet<AICondition>` | BaseNPC:43 |
| 属性: 生命/速度/视野/模型 | `[Property]` 字段 | BaseNPC:23-135 |
| 调试可视化 | `BaseNPC.DrawDebug()` | BaseNPC:402 |

### ⚠️ 部分完成 (核心逻辑有，边缘case缺)
| GMod Lua | 状态 | 缺口 |
|-----------|------|------|
| `ENT:MaintainRelationships()` 完整版 | 核心完成 | 缺: class-based alliance表、investigation检测(声/光)、YieldToAlliedPlayers推挤检测、CanBeEngaged完整判断 |
| `ENT:ConstantlyFaceEnemy()` | 简化版 | UpdateSenses中调用SetTurnTarget，但缺距离/姿态/攻击状态的条件判断 |
| `ENT:ForceSetEnemy()` | 未迁移 | 强制设敌+关系记忆+状态+警报 |

### ❌ 未迁移 (GMod专有/低优先级)
| GMod Lua | 理由 |
|-----------|------|
| `ENT:GetTurnAngle()` / `DeltaIdealYaw()` | S&Box用Rotation.Slerp替代 |
| `ENT:OverrideMoveFacing()` / `OverrideMove()` | S&Box引擎自动处理移动朝向 |
| `ENT:AcceptInput()` | GMod Input系统，S&Box用ActionGraph |
| `ENT:Touch()` | GMod物理触碰，S&Box用ICollisionListener |
| `ENT:Controller_Movement()` | 玩家操控NPC，S&Box用PlayerController |
| `ENT:GetSoundInterests()` | GMod位掩码，S&Box不需要 |
| `ENT:SetState()` / `GetState()` | 简化为Condition系统 |

---

## 2. 战斗系统 (近战/远程/手雷/跳跃)
**估算: 1,800 Lua → 1,500-2,000 C# | 实际: ~960 行 (55%)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `ENT:ExecuteMeleeAttack()` | `CreatureNPC.ExecuteMeleeAttack()` (球扫+角度检查+DamageInfo+击退) | CreatureNPC:391 |
| `ENT:ExecuteRangeAttack()` | `CreatureNPC.ExecuteRangeAttack()` (委托BaseWeapon.NPC_Think) | CreatureNPC:463 |
| `ENT:ExecuteLeapAttack()` | `CreatureNPC.ExecuteLeapAttack()` (Trajectory.Calculate抛物线) | CreatureNPC:482 |
| `ENT:LeapAttackJump()` | (合并入ExecuteLeapAttack) | CreatureNPC:482 |
| `ENT:StopAttacks()` | `CreatureNPC.StopAttacks()` | CreatureNPC:501 |
| `ENT:Flinch()` | `CreatureNPC.Flinch()` (概率+冷却+击中硬直动画) | CreatureNPC:174 |
| `ENT:GetAttackTimer()` | 简化为`_nextAttack`/`_nextMelee` TimeUntil | CreatureNPC:456 |
| `ENT:AddExtraAttackTimer()` | `CreatureNPC.ExtraMeleeHit()` (二连击) | CreatureNPC:526 |
| `ENT:MeleeAttackCode()` wrapper | (合并入ExecuteMeleeAttack) | CreatureNPC |
| 攻击状态机 | `AttackState`/`_attackType` | CreatureNPC:370 |
| 攻击优先级链 (Melee→Range→Leap) | `CreatureNPC.RunCombat()` | CreatureNPC:381 |

### ⚠️ 部分完成
| GMod Lua | 状态 | 缺口 |
|-----------|------|------|
| `ENT:ExecuteMeleeAttack()` 完整版 | 核心完成 | 缺: 流血DOT、Prop交互(推/破坏)、ViewPunch(玩家)、DSP音效 |
| `ENT:ExecuteLeapAttack()` 完整版 | 核心完成 | 缺: OnLeapAttack("Jump")回调、SetGroundEntity、AoE自动触发 |
| `ENT:Flinch()` 完整版 | 核心完成 | 缺: HitGroup映射表(不同部位不同动画)、DMG_FORCE_FLINCH特殊处理 |

### ❌ 未迁移
| GMod Lua | 理由 |
|-----------|------|
| `DoWeaponAttackMovementCode()` | 死代码(已注释) |
| 武器attackTimers表 | GMod timer.Create系统，S&Box用TimeUntil+async替代 |
| Prop交互(推/破坏) | GMod物理API专有 |

---

## 3. 移动系统
**估算: 800 Lua → 400-600 C# | 实际: 与BaseNPC/CreatureNPC合并 (~200行)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `TASK_GET_PATH_TO_ENEMY` + `TASK_RUN_PATH` | `BaseNPC.MoveTo()` (NavMeshAgent.MoveTo) | BaseNPC:177 |
| `TASK_FACE_ENEMY` / `TASK_FACE_TARGET` | `BaseNPC.FaceTarget()` / `SetTurnTarget()` | BaseNPC:217 |
| `TASK_WAIT_FOR_MOVEMENT` | `AIScheduleRunner.WaitForDestination()` | AIScheduleRunner:71 |
| `AA_MoveTo()` | `CreatureNPC.AAMoveTo()` (飞行/游泳物理移动) | CreatureNPC:323 |
| `AA_IdleWander()` | `CreatureNPC.AAIdleWander()` | CreatureNPC:359 |
| `AA_StopMoving()` | `CreatureNPC` + `BaseNPC.StopMoving()` | CreatureNPC:315 |
| `ENT:MaintainIdleBehavior()` | `CreatureNPC.MaintainIdleBehavior()` (导航/AA分派) | CreatureNPC:147 |
| 地面限制检测 | AAMoveTo中的ray trace | CreatureNPC:330 |

### ⚠️ 部分完成
| GMod Lua | 状态 | 缺口 |
|-----------|------|------|
| `AA_MoveTo()` 完整版 | 核心完成 | 缺: 地面限制检测的复杂逻辑、last-chase-pos回退、ChaseEnemy标志处理 |
| `AA_ChaseEnemy()` | (内联在SelectSchedule) | 未独立封装 |

### ❌ 未迁移
| GMod Lua | 理由 |
|-----------|------|
| `TASK_GET_PATH_TO_LASTPOSITION` | 内联在LostEnemy处理中 |
| `ForceMoveJump()` | 简化为Rigidbody.Velocity |
| `IsJumpLegal()` | S&Box NavMesh自动处理 |
| Node Graph寻路 | S&Box用NavMesh替代 |
| `MoveJumpStart` / `SetNavType(NAV_JUMP)` | S&Box无NAV_JUMP概念 |

---

## 4. 死亡/受伤/尸体
**估算: 600 Lua → 400-500 C# | 实际: 与BaseNPC/CreatureNPC合并 (~200行)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `ENT:OnDeath()` | `CreatureNPC.OnKilled()` (音效+特效+布娃娃+延迟销毁) | CreatureNPC:216 |
| `ENT:OnDamaged()` | `CreatureNPC.OnDamaged()` (音效+Flinch+反击) | CreatureNPC:199 |
| `ENT:CreateDeathCorpse()` / 布娃娃 | `HasDeathRagdoll` + `UseAnimGraph=false` | CreatureNPC:258 |
| 死亡时掉武器 | `HumanNPC.DropWeapon()` | HumanNPC:494 |

### ⚠️ 部分完成
| GMod Lua | 状态 | 缺口 |
|-----------|------|------|
| `ENT:OnDeath()` 完整版 | 核心完成 | 缺: "Override"/"Soldier"/"Effects"多阶段、CreateExtraDeathCorpse尸体碎片 |
| `ENT:GibOnDeath()` / `IsGibDamage()` | 未迁移 | GMod物理碎片系统 |
| `ENT:CreateGibEntity()` | 未迁移 | 低优先级 |
| `ENT:CreateDeathLoot()` | 未迁移 | 低优先级 |

### ❌ 未迁移
| GMod Lua | 理由 |
|-----------|------|
| `ENT:SetupBloodColor()` / 血液粒子 | S&Box有独立粒子系统 |
| `ENT:SpawnBloodParticles()` / `Decals()` / `Pool()` | S&Box用VFXHelper替代 |
| `ENT:GetLastDamageHitGroup()` | GMod引擎内部变量 |

---

## 5. 音效管理
**估算: 800 Lua → 300-400 C# | 实际: 92 行 (25%)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `VJ.EmitSound()` | `SoundManager.Emit()` (位置+音量+音调) | SoundManager:25 |
| `VJ.CreateSound()` | `SoundManager.CreateHandle()` | SoundManager:48 |
| `VJ.STOPSOUND()` | `handle?.Stop()` | SoundManager:69 |
| `VJ.PICK()` | `SoundManager.Pick<T>()` / `RandomHelper.FromList<T>()` | SoundManager:81 |
| `VJ.SET()` | `SoundManager.Set()` / `new Vector2(a,b)` | SoundManager:91 |
| `self:EmitSound()` | `BaseNPC.PlaySound()` | BaseNPC:209 |
| `self:StopAllSounds()` | `BaseNPC.StopAllSounds()` | BaseNPC:218 |
| `ENT:PlaySoundSystem("MeleeAttack"/"RangeAttack")` | `CreatureNPC.PlayAttackSound()` | CreatureNPC:297 |
| `ENT:PlaySoundSystem("Footstep")` | `AnimationDriver.MaintainFootsteps()` | AnimationDriver:273 |
| `ENT:PlayFootstepSound()` | (同上) | AnimationDriver:273 |
| 攻击音效表 | `SoundTbl_MeleeAttack`/`RangeAttack`/`LeapAttackJump`等属性 | CreatureNPC:267 |

### ⚠️ 部分完成 (核心框架有，30种音效分类只实现了5种)
| 音效类别 | 状态 | 
|----------|------|
| MeleeAttack / MeleeMiss ✅ | `CreatureNPC.PlayAttackSound()` |
| RangeAttack ✅ | `CreatureNPC.PlayAttackSound()` |
| LeapAttack ✅ | `CreatureNPC.PlayAttackSound()` |
| Footstep ✅ | `AnimationDriver.MaintainFootsteps()` |
| Death / Pain / Alert / Idle ✅ | `CreatureNPC.PlayAmbientSounds()` + `OnKilled`/`OnDamaged` |
| IdleDialogue / IdleDialogueAnswer | ❌ 未实现 |
| FollowPlayer / UnFollowPlayer | ❌ 未实现 |
| ReceiveOrder / YieldToPlayer | ❌ 未实现 |
| MedicBeforeHeal / MedicOnHeal / MedicReceiveHeal | ❌ 未实现 |
| GrenadeAttack / BeforeGrenadeAttack | ❌ 未实现 |
| CallForHelp / DangerSight / GrenadeSight | ❌ 未实现 |
| KilledEnemy / AllyDeath / DamageByPlayer | ❌ 未实现 |
| Suppressing / WeaponReload | ❌ 未实现 |
| Impact / BecomeEnemyToPlayer / Speech | ❌ 未实现 |
| Gib / OnPlayerSight / Investigate | ❌ 未实现 |

---

## 6. Human 特有 (武器切换、掩体、换弹、手雷、医疗、跟随、编队)
**估算: 2,000 Lua → 1,500-2,000 C# | 实际: 536 行 (30%)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `SWEP:NPC_Think()` / 武器开火 | `BaseWeapon.NPC_Think()` (爆发射击) | BaseWeapon:234 |
| `SWEP:NPC_CanFire()` | `BaseWeapon.NPC_CanFire()` | BaseWeapon:215 |
| `SWEP:Reload()` | `BaseWeapon.Reload()` (异步) | BaseWeapon:150 |
| `ENT:FindCoverFrom()` / `DoCoverTrace()` | `HumanNPC.IsCoverBetween()` + `FindCoverFrom()` (360°搜索) | HumanNPC:314 |
| `ENT:Follow()` / `ResetFollowBehavior()` | `HumanNPC.Follow()` / `StopFollowing()` / `MaintainFollow()` | HumanNPC:376 |
| `ENT:MaintainMedicBehavior()` | `HumanNPC.MedicCheck()` (找最低血量+移动到治疗距离) | HumanNPC:446 |
| `ENT:GrenadeAttack()` | `HumanNPC.GrenadeAttack()` (抛物线+BaseProjectile) | HumanNPC:485 |
| `ENT:DoGroupFormation("Diamond")` | `HumanNPC.DoDiamondFormation()` | HumanNPC:529 |
| `ENT:Allies_CallHelp()` | `HumanNPC.CallAlliesForHelp()` | HumanNPC:135 |
| 武器拾取/丢弃 | `HumanNPC.PickupWeapon()` / `DropWeapon()` | HumanNPC:504 |
| 生命恢复 | `HumanNPC.HealthRegenTick()` | HumanNPC:114 |
| `ENT:OnPlayerSight()` | `HumanNPC.PlayerInteraction()` | HumanNPC:163 |
| `ENT:SelectSchedule()` 完整版 | `HumanNPC.SelectSchedule()` (近战/远程/掩体/追逐/太近) | HumanNPC:193 |

### ⚠️ 部分完成 (核心逻辑有，细节缺)
| GMod Lua | 状态 | 缺口 |
|-----------|------|------|
| `ENT:MaintainMedicBehavior()` 完整版 | 核心完成 | 缺: 治疗动画、治疗道具生成、音效反馈、治疗冷却动画 |
| `ENT:GrenadeAttack()` 完整版 | 核心完成 | 缺: 投掷动画同步、握持手雷阶段、引擎npc_grenade_frag特殊处理 |
| 武器切换 | 有PickupWeapon/DropWeapon | 缺: 多武器栏位、自动选最佳武器、武器优先级 |
| `ENT:DoGroupFormation()` | 钻石编队完成 | 缺: 其他编队类型 |
| `ActivityTranslateAI` 武器动画翻译表 | ❌ 未实现 | GMod ACT_*枚举→动画序列映射 (S&Box用AnimGraph替代，但逻辑需要) |

### ❌ 未迁移
| GMod Lua | 理由 |
|-----------|------|
| `ENT:ExecuteGrenadeAttack()` 完整版 | 多类型手雷分支(frag/grenade/cpt_grenade) — GMod实体系统专有 |
| `ENT:Allies_Check()` / `Allies_Bring()` | 次要功能，SquadLeader部分覆盖 |
| `ENT:TranslateActivity()` 武器表 | 巨大硬编码表(~100行)，ACT_*枚举在S&Box不存在 |
| `ENT:SetupWeaponHoldTypeForAI()` | 同样是大硬编码表 |

---

## 7. Creature 特有 (跳跃攻击等)
**估算: 600 Lua → 400-500 C# | 实际: 485 行 (~100%)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `ENT:ExecuteLeapAttack()` | `CreatureNPC.ExecuteLeapAttack()` | CreatureNPC:482 |
| `ENT:LeapAttackJump()` | (合并入ExecuteLeapAttack, Trajectory.Calculate) | CreatureNPC:482 |
| `ENT:PlayIdleSound()` | `CreatureNPC.PlayAmbientSounds()` | CreatureNPC:107 |
| `ENT:MaintainIdleBehavior()` | `CreatureNPC.MaintainIdleBehavior()` | CreatureNPC:147 |
| `ENT:MaintainIdleAnimation()` | `AnimationDriver.MaintainIdleAnimation()` | AnimationDriver:258 |
| `AA_MoveTo()` / `AA_IdleWander()` | `CreatureNPC.AAMoveTo()` / `AAIdleWander()` | CreatureNPC:323 |
| Tank特有 `ENT:Tank_AngleDiffuse()` | `TankNPC.AngleDiffuse()` | TankNPC:348 |
| Tank特有 `ENT:OnThinkActive()` | `TankNPC.OnThinkActive()` (驾驶/旋转/碾压决策) | TankNPC:133 |
| Tank特有 `ENT:Tank_RunOver()` | `TankNPC.TryRunOver()` | TankNPC:204 |
| Tank特有 `ENT:Tank_PlaySoundSystem()` | `TankNPC.PlayMovementSounds()` | TankNPC:238 |
| Tank特有 `Tank_DeathExplosion` | `TankNPC.OnKilled()` (多次爆炸序列) | TankNPC:286 |
| Tank特有 `SelectSchedule()` | `TankNPC.SelectSchedule()` (距离→驾驶/炮击) | TankNPC:85 |

---

## 8. Schedule/Task 系统
**估算: 400 Lua → 500-700 C# | 实际: 154 行 (~90%)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `ENT:StartSchedule()` | `AIScheduleRunner.Run()` | AIScheduleRunner:31 |
| `ENT:ClearSchedule()` | `AIScheduleRunner.Cancel()` | AIScheduleRunner:48 |
| `timer.Simple()` | `GameTask.DelaySeconds()` | AIScheduleRunner |
| `timer.Create()` | `AIScheduleRunner.Repeat()` | AIScheduleRunner:83 |
| `coroutine.wait()` | `await GameTask.Delay()` | AIScheduleRunner |
| `TASK_WAIT_FOR_MOVEMENT` | `AIScheduleRunner.WaitForDestination()` | AIScheduleRunner:71 |
| `WaitUntil(condition)` | `AIScheduleRunner.WaitUntil()` | AIScheduleRunner:76 |
| 任务序列 | `AIScheduleRunner.RunSequence()` | AIScheduleRunner:89 |
| `FaceTargetAsync` | `AIScheduleRunner.FaceTargetAsync()` (扩展方法) | AIScheduleRunner:152 |

### ❌ 未迁移
| GMod Lua | 理由 |
|-----------|------|
| `SCHEDULE_FACE` / `SCHEDULE_GOTO_POSITION` 等具体调度 | 不再需要——调度系统改为async/await，SelectSchedule直接调用MoveTo/FaceTarget |
| `TASK_VJ_PLAY_ACTIVITY` / `PLAY_SEQUENCE` | 简化为AnimationDriver.PlayAnim/PlaySequence |
| `OnTaskFailed()` / `ScheduleFinished()` | async/await用try/catch+finally替代 |
| `StartEngineTask()` / `RunEngineTask()` | GMod引擎task系统，S&Box不存在 |

---

## 9. 配置属性 ([Property] 字段)
**估算: 2,000 Lua → 300-500 C# | 实际: 分散在各文件中 (~400行)**

S&Box的`[Property]`属性直接暴露在Inspector中，大量GMod的ENT属性表(table)被简化为C#属性。各文件中的`[Property]`字段已覆盖所有可配置项。

---

## 10. 工具/调试
**估算: 300 Lua → 200-300 C# | 实际: 347 行 (~120%)**

### ✅ 已完成
| GMod | C# | 位置 |
|------|-----|------|
| `debugoverlay.Line/Cross/Text` | `BaseNPC.DrawDebug()` + `DebugOverlay` | BaseNPC:402 |
| `VJ.DEBUG_Print()` | 注释/Log | (各处) |
| 巡逻路径 | `PatrolPath` + `PatrolBehaviour` | PatrolPath.cs |
| 波次生成 | `WaveSpawner` | WaveSpawner.cs |
| 小队指挥 | `SquadLeader` | SquadLeader.cs |
| 测试场景 | `GameManager` | GameManager.cs |
| NPC生成工厂 | `NpcFactory` | NpcFactory.cs |

### ⚠️ 额外产出(估算未覆盖)
| 组件 | 行数 | 用途 |
|------|------|------|
| WaveSpawner | 99 | 递增难度波次生成 |
| SquadLeader | 60 | 小队敌人共享+编队 |
| GameManager | 50 | 测试场景一键生成 |
| NpcFactory | 121 | 一行代码创建NPC |

---

## 11. 枚举/常量
**估算: 350 Lua → 200-300 C# | 实际: 247 行 (~100%)**

### ✅ 已完成
| GMod | C# | 位置 |
|------|-----|------|
| `VJ_MOVETYPE_*` | `MovementType` enum | CreatureNPC |
| `VJ_BEHAVIOR_*` | `VJBehavior` enum | VJEnums:13 |
| `VJ.PROJ_TYPE_*` | `ProjectileType` enum | BaseProjectile |
| `VJ.ATTACK_TYPE_*` | `_attackType` string | CreatureNPC |
| `VJ.FACE_*` | `VJFaceType` enum | VJEnums:44 |
| `VJ.ANIM_SET_*` | `VJAnimationSet` enum | VJEnums:34 |
| `COND_*` 条件 | `AICondition` enum | BaseNPC |
| `DMG_*` 伤害类型 | `DamageTags` 常量 | VJEnums:199 |
| `VJ_COLOR_*` | `VJColors` 常量 | VJEnums:73 |
| `VJ.PICK()` / `VJ.SET()` | `RandomHelper` + `SoundManager` | VJEnums:104 |
| `VJ.CalculateTrajectory()` | `Trajectory.Calculate()` | VJEnums:215 |
| GMod ConVar | `VJConfig` | VJEnums:88 |

---

## 12. 初始化/生命周期
**估算: 300 Lua → 200-300 C# | 实际: 内联在OnStart/OnAwake/OnUpdate中**

GMod的`ENT:Initialize()` → S&Box `OnStart()`，各Component已正确映射。

---

## 13. 武器基类 (1把)
**估算: 1,260 Lua → 500-800 C# | 实际: 247 行 (~40%)**

### ✅ 已完成
| GMod Lua 函数 | C# 实现 | 位置 |
|--------------|---------|------|
| `SWEP:PrimaryAttack()` | `BaseWeapon.PrimaryAttack()` (支持Auto/Semi/Burst) | BaseWeapon:76 |
| `SWEP:FireBullets()` | `BaseWeapon.FireBullet()` (散布+曳光+弹孔) | BaseWeapon:92 |
| `SWEP:Reload()` | `BaseWeapon.Reload()` (异步) | BaseWeapon:150 |
| `SWEP:GetBulletPos()` | `BaseWeapon.GetBulletSpawnPosition()` | BaseWeapon:122 |
| `SWEP:NPC_Think()` | `BaseWeapon.NPC_Think()` (爆发射击) | BaseWeapon:234 |
| `SWEP:NPC_CanFire()` | `BaseWeapon.NPC_CanFire()` | BaseWeapon:215 |
| `SWEP:SecondaryAttack()` | `BaseWeapon.SecondaryAttack()` | BaseWeapon:140 |
| `SWEP:DryFire()` | `BaseWeapon.DryFire()` | BaseWeapon:175 |
| 射击模式 (Auto/Semi/Burst) | `FireMode` enum + `BurstCount` | BaseWeapon:25 |
| 枪口/弹壳特效 | `PlayMuzzleEffects()` + `ShellEject()` | BaseWeapon:163 |
| 曳光弹 | `ShowTracer` + `SpawnTracer()` | BaseWeapon:232 |
| 弹孔 | `ImpactDecal` + `VFXHelper.PlaceDecal()` | BaseWeapon:128 |

### ❌ 未迁移 (玩家端/Viewmodel逻辑)
| GMod Lua | 理由 |
|-----------|------|
| `SWEP:Deploy()` 玩家部署动画 | NPC武器不需要Viewmodel操作 |
| `SWEP:Holster()` 玩家收枪 | NPC不需要 |
| `SWEP:Equip()` / `EquipAmmo()` 玩家逻辑 | 玩家端逻辑，不在NPC框架范围 |
| `SWEP:DrawWorldModel()` | S&Box自动渲染 |
| `SWEP:GetWeaponCustomPosition()` | 玩家武器位置自定义 |
| `SWEP:SetupWeaponHoldTypeForAI()` | 硬编码动画映射表(~200行) |
| `SWEP:TranslateActivity()` | ACT_*→序列映射(~150行) |
| `SWEP:FireAnimationEvent()` | GMod动画事件系统 |
| `SWEP:DoIdleAnimation()` 玩家端 | NPC用AnimationDriver替代 |

---

## 不迁移的部分 (确认删除)

| 类别 | Lua行数 | 文件数 | 理由 |
|------|---------|--------|------|
| 20把具体武器 | 2,160 | 20 | S&Box武器体系不同，保留BaseWeapon接口即可 |
| Menu/UI (VGUI/Derma) | 1,555 | 4 | S&Box用Razor/HTML |
| Toolgun工具 | 1,312 | 9 | GMod专属，S&Box编辑器自带等价功能 |
| 投掷物/发射物实体 (obj_vj_*) | 2,480 | 11 | 行为逻辑保留在BaseProjectile,实体样板删除 |
| 特效 (effects/) | 416 | 9 | S&Box用Particles组件 |
| Localization翻译 | 2,332 | 2 | S&Box本地化系统不同 |
| GMod钩子注册 (hook.Add) | 150 | 1 | Component生命周期替代 |
| 测试NPC | 450 | 3 | 开发时按需 |
| **不迁移合计** | **10,855** | **59** | |

---

## 总结

| 子系统 | 估算C# | 实际C# | 完成度 |
|--------|--------|--------|--------|
| BaseNPC (感知/决策/关系) | 2,000-3,000 | 1,189 | 50% |
| 战斗系统 | 1,500-2,000 | 959 | 55% |
| Human特有 | 1,500-2,000 | 536 | 30% |
| 移动系统 | 400-600 | ~200 | 70% |
| 死亡/受伤 | 400-500 | ~200 | 70% |
| 音效管理 | 300-400 | 92 | 25% |
| Schedule/Task | 500-700 | 154 | 90% |
| Creature特有 | 400-500 | 485 | 100% |
| 配置属性 | 300-500 | ~400 | 100% |
| 工具/调试 | 200-300 | 347 | 120% |
| 枚举/常量 | 200-300 | 247 | 100% |
| 初始化/生命周期 | 200-300 | ~250 | 100% |
| 武器基类 | 500-800 | 247 | 40% |
| **合计** | **8,400-12,300** | **~4,000** | **~40%** |

### 差距分析
- **架构完整度**: 100% — 所有13个子系统都有对应Component
- **核心路径**: ~70% — 关键战斗/AI循环完整可运行
- **边缘case**: ~20% — 冷门分支/特殊交互未移植
- **不迁移**: 10,855行Lua (59文件) 确认删除
