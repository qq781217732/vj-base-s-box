# NPC 迁移核对清单

> 使用方式：每迁移一个 NPC 类型，复制这份模板，逐项打勾。不打勾不上报"完成"。

---

## 一、Lua→C# 方法映射表

逐方法确认 Lua 源码的每一行逻辑都有 C# 对应。从 Lua 入口方法开始，按调用链展开。

### 生命周期

| Lua 方法 | Lua 文件:行号 | C# 方法 | 状态 | 备注 |
|----------|-------------|---------|------|------|
| `ENT:Initialize()` | `init.lua:`____ | `OnStart()` | ☐ | |
| `ENT:CustomInitialize()` | `init.lua:`____ | `OnCreatureInit()` | ☐ | |
| `ENT:OnThink()` → `RunAI()` | `core.lua:`____ | `OnUpdate()` | ☐ | 确认 Think 间隔一致 |
| `ENT:SelectSchedule()` | `core.lua:`____ | `SelectSchedule()` | ☐ | |
| `ENT:CustomOnRemove()` | `init.lua:`____ | `OnDestroy()` | ☐ | |

### 感知

| Lua 方法 | Lua 行号 | C# 方法 | 状态 | 备注 |
|----------|---------|---------|------|------|
| `ENT:GatherConditions()` | `core.lua:`____ | `GatherConditions()` | ☐ | |
| `ENT:PerformSensing()` | `core.lua:`____ | `GatherEnemyConditions()` | ☐ | 视野锥/距离/LOS |
| `ENT:Visible(entity)` | `core.lua:`____ | `Senses.CanSee()` | ☐ | 确认射线参数 |
| `ENT:IsInViewCone(entity)` | `core.lua:`____ | `Senses.IsInViewCone()` | ☐ | 确认 FOV 值 |
| `ENT:MaintainConstantlyFaceEnemy()` | `core.lua:`____ | `OnUpdate` 面敌段 | ☐ | |
| `ENT:FindVisibleNPCs()` | `core.lua:`____ | `Senses.FindVisibleNPCs()` | ☐ | |

### 敌人管理

| Lua 方法 | Lua 行号 | C# 方法 | 状态 | 备注 |
|----------|---------|---------|------|------|
| `ENT:MaintainRelationships()` | `core.lua:`____ | `GatherEnemyConditions()` | ☐ | |
| `ENT:IsPotentialEnemy(ent)` | `core.lua:`____ | `IsPotentialEnemy()` | ☐ | 类型过滤/存活检查 |
| `ENT:ForceSetEnemy(ent)` | `core.lua:`____ | `ForceSetEnemy()` | ☐ | |
| `ENT:OnSeeEntity(ent)` | `core.lua:`____ | `OnSeeEnemy()` | ☐ | |
| `ENT:OnLostEnemy()` | `core.lua:`____ | `OnLostEnemy()` | ☐ | |

### 移动

| Lua 方法 | Lua 行号 | C# 方法 | 状态 | 备注 |
|----------|---------|---------|------|------|
| `ENT:MaintainIdleBehavior()` | `init.lua:`____ | `MaintainIdleBehavior()` | ☐ | |
| `TASK_RUN_PATH` | `schedules.lua:`____ | `MoveTo()` + `Agent` | ☐ | |
| `TASK_WALK_PATH` | `schedules.lua:`____ | `MoveTo(speed)` | ☐ | |
| `TASK_FACE_ENEMY` | `schedules.lua:`____ | `FaceEnemy()` | ☐ | |
| `TASK_FACE_TARGET` | `schedules.lua:`____ | `FaceTarget()` | ☐ | |
| `AA_MoveTo()` | `base_aa.lua:`____ | `AAMoveTo()` | ☐ | 仅 AA 类型 |
| `AA_IdleWander()` | `base_aa.lua:`____ | `AAIdleWander()` | ☐ | 仅 AA 类型 |

### 受伤

| Lua 方法 | Lua 行号 | C# 方法 | 状态 | 备注 |
|----------|---------|---------|------|------|
| `ENT:OnDamaged(dmginfo)` | `core.lua:`____ | `OnDamaged()` | ☐ | |
| `ENT:OnInjured(dmginfo)` | `core.lua:`____ | `OnDamaged()` 后续 | ☐ | |
| `ENT:ShouldFlinch(dmginfo)` | `init.lua:`____ | `Flinch()` | ☐ | 确认条件链 |
| `ENT:BleedTimer()` | `init.lua:`____ | `ApplyBleed()` | ☐ | |
| `VJ.ApplyRadiusDamage()` | `funcs.lua:`____ | `RadiusDamage.Apply()` | ☐ | |
| 免疫检查 (Bullet/Melee/Explosive/...) | `init.lua:`____ | `OnDamaged()` 免疫段 | ☐ | 逐类型确认 |

### 战斗 — 近战

| Lua 方法 | Lua 行号 | C# 方法 | 状态 | 备注 |
|----------|---------|---------|------|------|
| `ENT:ExecuteMeleeAttack()` | `init.lua:`____ | `ExecuteMeleeAttack()` | ☐ | |
| 角度检查 | `init.lua:`____ | 内联 dot product | ☐ | vs `MeleeAttackDamageAngleRadius` |
| 距离检查 | `init.lua:`____ | 内联 distance check | ☐ | vs `MeleeAttackDamageDistance` |
| 伤害计算 + 缩放 | `init.lua:`____ | 内联 damage calc | ☐ | `ScaleByDifficulty()` |
| 击退力 | `init.lua:`____ | 内联 knockback | ☐ | vs `HasMeleeAttackKnockBack` |
| 流血施加 | `init.lua:`____ | 内联 bleed | ☐ | vs `MeleeAttackBleedEnemy*` |
| 连击窗口 | `init.lua:`____ | `ExtraMeleeHit()` | ☐ | |
| 近战音效 | `init.lua:`____ | `PlayAttackSound("melee")` | ☐ | |
| 冷却设置 | `init.lua:`____ | `NextMeleeAttack = ...` | ☐ | |

### 战斗 — 远程 (如适用)

| Lua 方法 | Lua 行号 | C# 方法 | 状态 | 备注 |
|----------|---------|---------|------|------|
| `ENT:ExecuteRangeAttack()` | `init.lua:`____ | `ExecuteRangeAttack()` | ☐ | |
| 距离限制检查 | `init.lua:`____ | 内联 min/max check | ☐ | vs `RangeAttackMin/MaxDistance` |
| 射弹数循环 | `init.lua:`____ | for-loop | ☐ | vs `RangeAttackReps` |
| 角度散布 | `init.lua:`____ | 内联 spread | ☐ | vs `RangeAttackAngleRadius` |
| `ENT:FireBullets()` | `weapon_base:`____ | `RangeHandler.Execute()` | ☐ | |
| 远程音效 | `init.lua:`____ | `PlayAttackSound("range")` | ☐ | |

### 战斗 — 跳跃攻击 (如适用)

| Lua 方法 | Lua 行号 | C# 方法 | 状态 | 备注 |
|----------|---------|---------|------|------|
| `ENT:ExecuteLeapAttack()` | `init.lua:`____ | `ExecuteLeapAttack()` | ☐ | |
| 距离限制 | `init.lua:`____ | 内联 check | ☐ | vs `LeapAttackMin/MaxDistance` |
| 弹道计算 | `init.lua:`____ | `Trajectory.Calculate()` | ☐ | |
| 落地伤害 | `init.lua:`____ | 碰撞回调 | ☐ | vs `LeapAttackStopOnHit` |
| 跳跃音效 | `init.lua:`____ | `PlayAttackSound("leap")` | ☐ | |

### 死亡

| Lua 方法 | Lua 行号 | C# 方法 | 状态 | 备注 |
|----------|---------|---------|------|------|
| `ENT:OnDeath(dmginfo)` | `core.lua:`____ | `OnKilled()` | ☐ | |
| 碎尸检查 | `init.lua:`____ | `IsGibDamage()` | ☐ | |
| 布娃娃 | `init.lua:`____ | `OnKilled()` 布娃娃段 | ☐ | |
| 尸体生成 | `init.lua:`____ | `SpawnDeathLoot()` | ☐ | |
| 死亡音效 | `init.lua:`____ | `SoundSystem.PlayDeath()` | ☐ | |
| 掉落武器 | `init.lua:`____ | `DropAllWeapons()` | ☐ | vs `DropWeaponOnDeath` |

### 音效系统 (如适用)

| Lua 功能 | Lua 行号 | C# 实现 | 状态 |
|----------|---------|---------|------|
| 闲置音效循环 | `init.lua:`____ | `PlayAmbientSounds()` | ☐ |
| 呼吸/闲置/警戒/战斗 音效表 | `init.lua:`____ | `SoundTbl_*` 属性 | ☐ |
| 脚步声 | `core.lua:`____ | `AnimationDriver.MaintainFootsteps()` | ☐ |
| `VJ.PlaySoundSystem()` | `init.lua:`____ | `PlayAttackSound()` | ☐ |

---

## 二、属性接线核对表

每加一个 `[Property]`，必须回答：哪个方法读了它？

| 属性名 | 类型 | 默认值 | 读取方法 | 状态 |
|--------|------|--------|---------|------|
| `___` | `___` | `___` | `___` | ☐ |
| `___` | `___` | `___` | `___` | ☐ |

> 规则：如果一个属性没有任何方法读取，要么删掉它，要么在"读取方法"列写清楚计划何时接线。不允许留无计划孤儿属性。

---

## 三、行为验证清单

不用运行时（S&Box 没环境），但可以逐项走查代码逻辑。

### 3.1 初始状态

| 检查项 | 验证方式 | 状态 |
|--------|---------|------|
| Health = MaxHealth | 读 `OnStart` | ☐ |
| State = Idle | 读 `OnStart` | ☐ |
| GuardPosition = 出生点 | 读 `OnStart` | ☐ |
| Agent 组件已缓存 | 读 `OnStart` | ☐ |
| 第一个 Think 间隔正确 | 读 `NextDecisionTime` 初始值 | ☐ |

### 3.2 感知逻辑路径

| 条件 | 预期行为 | 代码路径 | 状态 |
|------|---------|---------|------|
| 敌人在视野内 | SetCondition(SeeEnemy) | `GatherEnemyConditions` → canSee 分支 | ☐ |
| 敌人在视野外 | SetCondition(EnemyOccluded) | `GatherEnemyConditions` → else 分支 | ☐ |
| 敌人超出视距 | SetCondition(EnemyTooFar) → Enemy=null | `GatherEnemyConditions` → dist > SightDistance | ☐ |
| 敌人死亡 | SetCondition(EnemyDead) → Enemy=null | `GatherEnemyConditions` → IsDead 分支 | ☐ |
| 听到声音 | SetCondition(HearDanger) | `PerformSensing` → 声音检测 | ☐ |

### 3.3 状态转换

| 转换 | 触发条件 | 代码路径 | 状态 |
|------|---------|---------|------|
| Idle → Alert | HearDanger / LightDamage / HeavyDamage | `SelectIdealState` | ☐ |
| Alert → Combat | SeeEnemy / NewEnemy | `SelectIdealState` | ☐ |
| Alert → Idle | AlertTimeout 到期 + 无条件 | `SelectIdealState` → TimeSinceAlerted | ☐ |
| Combat → Alert | LostEnemy | `SelectIdealState` | ☐ |
| Any → Dead | Health <= 0 | `OnDamaged` → `OnKilled` | ☐ |

### 3.4 伤害管线

| 场景 | 预期行为 | 代码路径 | 状态 |
|------|---------|---------|------|
| 子弹伤害 | Damage -= dmg, SetCondition(LightDamage) | `OnDamage` → IDamageable | ☐ |
| 重击伤害(>20) | 额外 SetCondition(HeavyDamage) | `OnDamage` → dmg>20 分支 | ☐ |
| 伤害时无敌人 | 设置 attacker 为 Enemy | `OnDamage` → Enemy==null 分支 | ☐ |
| 免疫伤害类型 | return 跳过 | `OnDamaged` → immune 分支 | ☐ |
| Boss 强制伤害 | 跳过免疫 | `OnDamaged` → ForceDamageFromBosses 分支 | ☐ |
| 死亡 (Health<=0) | State=Dead, Runner.Cancel, OnKilled | `OnDamage` → Health<=0 分支 | ☐ |

### 3.5 近战攻击时序 (如适用)

| 步骤 | 预期行为 | 状态 |
|------|---------|------|
| 1. 攻击条件满足 | HasCondition(CanMelee1) + NextMeleeAttack<=0 | ☐ |
| 2. 攻击开始 | AttackState = Started | ☐ |
| 3. 动画延迟 | DelaySeconds(攻击前摇) | ☐ |
| 4. 伤害判定 | Sphere sweep + 角度/距离过滤 | ☐ |
| 5. 攻击命中 | 伤害 + 击退 + 流血 | ☐ |
| 6. 攻击落空 | 播放 miss 音效 | ☐ |
| 7. 攻击结束 | AttackState = Done | ☐ |
| 8. 冷却 | NextMeleeAttack = MeleeAttackDelay | ☐ |
| 9. 连击窗口 | 33% 概率 ExtraMeleeHit | ☐ |

---

## 四、日志埋点清单

> 给每个关键分支加 Log.Info。S&Box 不能跑但可以在编辑器 Console 看到状态输出。

| 日志点 | Log 内容 | 位置 | 状态 |
|--------|---------|------|------|
| NPC 初始化 | `[{Type}] Init: HP={Health}, Pos={Position}` | `OnStart` | ☐ |
| 状态转换 | `[{Type}] State: {Old} → {New} (reason)` | `SelectIdealState` | ☐ |
| 敌人发现 | `[{Type}] Enemy acquired: {Enemy.Name} dist={d}` | `GatherEnemyConditions` | ☐ |
| 敌人丢失 | `[{Type}] Enemy lost after {t}s` | `GatherEnemyConditions` | ☐ |
| 受到伤害 | `[{Type}] Dmg={d} from={attacker} HP={h}` | `OnDamaged` | ☐ |
| 攻击开始 | `[{Type}] Attack={type} target={enemy}` | 对应 Execute* 方法 | ☐ |
| 攻击命中 | `[{Type}] Hit={target} dmg={d}` | 对应 Execute* 方法 | ☐ |
| 攻击落空 | `[{Type}] Attack missed` | 对应 Execute* 方法 | ☐ |
| 死亡 | `[{Type}] Killed by={attacker}` | `OnKilled` | ☐ |
| Schedule 切换 | `[{Type}] Schedule: {name}` | `Runner.Execute` | ☐ |
| 免疫触发 | `[{Type}] Immune to {damageType}` | `OnDamaged` | ☐ |
| 流血触发 | `[{Type}] Bleed started: dmg/s={d}` | `ApplyBleed` | ☐ |
| 回避(被动) | `[{Type}] Fleeing from {threat}` | `MaintainIdleBehavior` 被动段 | ☐ |

---

## 五、回归核对表

> 每次改完一个 NPC 类型，回来确认之前做过的类型没有退化。

| NPC 类型 | 检查日期 | 属性数 | 方法数 | 接线率 | 退化项 | 状态 |
|----------|---------|--------|--------|--------|--------|------|
| `____` | `____` | `__` | `__` | `__%` | 无 | ☐ |
| `____` | `____` | `__` | `__` | `__%` | `____` | ☐ |

---

## 六、完成标准

一个 NPC 类型标记为"完成"必须满足：

- [ ] 一、方法映射表 ≥ 90% 打勾
- [ ] 二、属性接线表 100% 有读取方法
- [ ] 三、行为验证表 ≥ 80% 打勾
- [ ] 四、日志埋点表 100% 打勾
- [ ] `compare_migration.py --focus` 该类型缺口 ≤ 5 且全部分类为 D
- [ ] 回归表无退化
