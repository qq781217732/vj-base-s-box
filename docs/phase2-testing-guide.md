# Phase 2 集成测试指南

> 目标：Phase 2 逻辑闭环（感知 → 决策 → 攻击 → 死亡）的完整验证
> 方法：场景测试 + 控制台日志 + 检查清单
> 环境：S&Box 编辑器内运行

---

## 1. 测试架构：三层递进

```
Layer 1: 单元场景（每子系统 1 个场景）
  验证单个子系统的基础功能，隔离外部依赖
  
Layer 2: 集成场景（跨子系统交互）
  验证两个以上子系统协同工作
  
Layer 3: 自由场景（全系统压力测试）
  多 NPC 混战，验证无崩溃/死循环/异常状态
```

---

## 2. 测试工具：TestNPC 变体

### 2.1 测试基类增强

在现有 `TestNPC` 基础上，增加子系统测试开关。不修改生产代码，纯测试用。

```csharp
/// <summary>
/// Phase 2 subsystem test NPC. Set TestMode to isolate and test specific subsystems.
/// </summary>
public class Phase2TestNPC : HumanNPC
{
    [Property] public string TestMode { get; set; } = "FullAI";
    
    // Configurable test parameters
    [Property] public bool AutoEquipWeapon { get; set; } = true;
    [Property] public string WeaponClass { get; set; } = "weapon_smg1";
    [Property] public float ThinkInterval { get; set; } = 0.5f;
    [Property] public bool VerboseLogging { get; set; } = true;
    
    private TimeUntil _nextThink;

    protected override void OnStart()
    {
        base.OnStart();
        _nextThink = 1f;
        
        if (VerboseLogging)
            Log.Info($"[Test:{TestMode}] NPC spawned at {WorldPosition}");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (_nextThink > 0) return;
        _nextThink = ThinkInterval;
        
        switch (TestMode)
        {
            case "Perception":    TestPerception(); break;
            case "Schedule":      TestSchedule(); break;
            case "Movement":      TestMovement(); break;
            case "Combat":        TestCombat(); break;
            case "Weapon":        TestWeapon(); break;
            case "Damage":        TestDamage(); break;
            case "Death":         TestDeath(); break;
            case "Allies":        TestAllies(); break;
            case "FullAI":        TestFullAI(); break;
        }
    }
}
```

### 2.2 日志规范

所有测试日志用统一前缀，方便过滤：

```
[Test:子系统] 描述 = 实际值 | 预期值
```

示例：
```
[Test:Perception] HasCondition(SeeEnemy) = True | Expected: True
[Test:Combat] MeleeAttack fired | Damage = 10 | EnemyHealth = 90
```

---

## 3. Layer 1：九个子系统单元场景

### 3.1 感知系统（Perception）

**场景设置：**
- 生成 1 个 TestNPC，`TestMode = "Perception"`
- 玩家站在不同距离/角度

**测试逻辑：**
```csharp
void TestPerception()
{
    TickSenses();
    var ene = GetEnemy();
    
    Log.Info($"[Test:Perception] Enemy = {(ene.IsValid() ? ene.Name : "null")}");
    Log.Info($"[Test:Perception] SeeEnemy = {HasCondition(Condition.SeeEnemy)}");
    Log.Info($"[Test:Perception] HaveEnemyLOS = {HasCondition(Condition.HaveEnemyLOS)}");
    Log.Info($"[Test:Perception] Enemy.Distance = {Enemy.Distance}");
    Log.Info($"[Test:Perception] Enemy.Visible = {Enemy.Visible}");
    Log.Info($"[Test:Perception] Alerted = {Alerted}");
}
```

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | 玩家在视线内，距离 < SightDistance | `SeeEnemy = true, HaveEnemyLOS = true` | |
| 2 | 玩家在视线内但被墙壁遮挡 | `SeeEnemy = true, HaveEnemyLOS = false` | |
| 3 | 玩家在视线外（角度 > SightAngle） | `SeeEnemy = false` | |
| 4 | 玩家在 SightDistance 之外 | `Enemy = null` | |
| 5 | 多个敌人时 | 选最近可见的作为 Enemy | |
| 6 | 敌人死亡 | `Enemy = null, LostEnemy = true` | |

### 3.2 调度/任务系统（Schedule）

**场景设置：**
- 生成 1 个 TestNPC，`TestMode = "Schedule"`
- 玩家站在可见位置使其成为敌人

**测试逻辑：**
```csharp
void TestSchedule()
{
    TickSenses();
    
    if (GetEnemy().IsValid() && CurrentSchedule == null)
    {
        SelectSchedule();
        if (CurrentSchedule != null)
        {
            Log.Info($"[Test:Schedule] Started: {CurrentSchedule.Name} | Tasks: {CurrentSchedule.NumTasks()}");
            Log.Info($"[Test:Schedule] TaskID: {CurrentTaskID} | Task: {CurrentTask?.TaskName}");
        }
    }
    
    if (CurrentSchedule != null)
    {
        RunAI();
        Log.Info($"[Test:Schedule] Running: {CurrentSchedule.Name} | Task: {CurrentTask?.TaskName} | Finished: {IsScheduleFinished(CurrentSchedule)}");
    }
}
```

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | 敌人可见 | `SelectSchedule` 创建 `SCHEDULE_ALERT_CHASE` 或武器战斗 | |
| 2 | Schedule 启动后 | `CurrentSchedule != null`, `CurrentTask` 有值 | |
| 3 | Task 完成 | 自动 `NextTask` 或 `ScheduleFinished` | |
| 4 | 同一 Schedule 重复启动 | `StartSchedule` 中 `schedule.Name == cur.Name` 守卫阻止 | |
| 5 | Schedule 中断 | `ClearSchedule` 清理正确 | |
| 6 | 没有敌人 | 进入 `SCHEDULE_IDLE_STAND` 或 `SCHEDULE_IDLE_WANDER` | |

### 3.3 移动系统（Movement）

**场景设置：**
- 生成 1 个 TestNPC，`TestMode = "Movement"`
- 手动调 `SetLastPosition(targetPos)` + `SCHEDULE_GOTO_POSITION`

**测试逻辑：**
```csharp
void TestMovement()
{
    if (!IsMoving() && CurrentSchedule == null)
    {
        // Manual move test
        SetLastPosition(WorldPosition + Vector3.Forward * 500);
        SCHEDULE_GOTO_POSITION("TASK_RUN_PATH");
    }
    
    if (CurrentSchedule != null)
    {
        RunAI();
        Log.Info($"[Test:Movement] Moving: {IsMoving()} | Pos: {WorldPosition} | Target: {GetLastPosition()}");
        Log.Info($"[Test:Movement] NavType: {GetNavType()} | MoveType: {MovementType}");
    }
}
```

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | 地面 NPC 调用 GOTO_POSITION | NavMeshAgent 激活，朝目标移动 | |
| 2 | 飞行 NPC（Aerial） | NavMeshAgent 关闭，AA_MoveTo 接管 | |
| 3 | 静止 NPC（Stationary） | `SCHEDULE_FACE` 只能用转向 | |
| 4 | 到达目标 | `ScheduleFinished` 触发，`SelectSchedule` 重新决策 | |
| 5 | 移动中遇到门 | `OpeningDoor.IsValid()` 阻止重复 Schedule | |

### 3.4 战斗系统（Combat）

**场景设置：**
- 生成 1 个 TestNPC，`TestMode = "Combat"`，`HasMeleeAttack = true`
- 玩家站在近战范围内
- 确保 NPC 有敌人

**测试逻辑：**
```csharp
void TestCombat()
{
    TickSenses();
    if (GetEnemy().IsValid() && CurrentSchedule == null)
        SelectSchedule();
    
    RunAI();
    
    // 检测攻击状态
    Log.Info($"[Test:Combat] AttackType: {AttackType} | AttackState: {AttackState}");
    Log.Info($"[Test:Combat] HasMelee: {HasMeleeAttack} | HasRange: {HasRangeAttack} | HasLeap: {HasLeapAttack}");
    Log.Info($"[Test:Combat] IsAbleToMelee: {IsAbleToMeleeAttack} | IsAbleToRange: {IsAbleToRangeAttack}");
    Log.Info($"[Test:Combat] NextDoAnyAttackT: {NextDoAnyAttackT} | NextChaseTime: {NextChaseTime}");
}
```

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | 近战距离内，敌人可见 | `ExecuteMeleeAttack` 触发，`FindInPhysics` 扫描 | |
| 2 | 近战命中 | 目标受到伤害，`hitRegistered = true` | |
| 3 | 近战挥空 | `OnMeleeAttackExecute("Miss")` 回调 + Miss 音效 | |
| 4 | 远程距离内 | `ExecuteRangeAttack` → `SpawnRangeProjectile` | |
| 5 | 攻击冷却 | `AttackResetTime` / `AttackReEnableTime` 正确设置 | |
| 6 | `MeleeAttackStopOnHit` | 命中后立即停止后续扫描 | |
| 7 | Prop 交互 | `PropInteraction` → `FixedJoint.Destroy()` 解焊 | |
| 8 | Boss 免伤 | `VJ_ID_Boss` 实体绕过免疫链 | |

### 3.5 武器系统（Weapon）

**场景设置：**
- 生成 1 个 TestNPC，`TestMode = "Weapon"`，`AutoEquipWeapon = true`
- 玩家站在可见位置

**测试逻辑：**
```csharp
void TestWeapon()
{
    TickSenses();
    if (GetEnemy().IsValid() && CurrentSchedule == null)
        SelectSchedule();
    RunAI();
    
    // 武器状态
    var wep = GetActiveWeapon();
    var wepComp = wep.IsValid() ? wep.Components.Get<IVJBaseWeapon>() : null;
    
    Log.Info($"[Test:Weapon] ActiveWeapon: {(wep.IsValid() ? wep.Name : "null")}");
    Log.Info($"[Test:Weapon] WeaponAttackState: {WeaponAttackState}");
    Log.Info($"[Test:Weapon] WeaponState: {GetWeaponState()}");
    
    if (wepComp != null)
    {
        Log.Info($"[Test:Weapon] Clip1: {wepComp.GetClip1()}/{wepComp.GetMaxClip1()}");
        Log.Info($"[Test:Weapon] IsMelee: {wepComp.IsMeleeWeapon}");
        Log.Info($"[Test:Weapon] NextPrimaryFireT: {(wepComp as VJBaseWeapon)?.NPC_NextPrimaryFireT}");
    }
    
    Log.Info($"[Test:Weapon] WeaponLastShotTime: {WeaponLastShotTime}");
    Log.Info($"[Test:Weapon] NextWeaponAttackT: {NextWeaponAttackT}");
    Log.Info($"[Test:Weapon] NextWeaponAttackT_Base: {NextWeaponAttackT_Base}");
}
```

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | 敌人可见 + 有武器 | `SelectSchedule` C2c-iii 设 `WeaponAttackState = FireStand` | |
| 2 | FireStand 后 | `NPC_Think` → `NPC_CanFire(true)` → `NPCShoot_Primary` → `PrimaryAttack` | |
| 3 | 弹药消耗 | `Clip1` 每次开枪减少 `Primary_TakeAmmo` | |
| 4 | 弹药耗尽 | `WeaponAttackState = Aim`，不再开火 | |
| 5 | 换弹 | `SetWeaponState(Reloading)` → `IsReloading = true` → 阻止开火 | |
| 6 | 换弹完成 | `IsReloading = false`，`Clip1 = MaxClip1` | |
| 7 | 遮蔽延迟 | 敌人躲掩体后 → `WeaponAttackState = AimOcclusion` | |
| 8 | 遮蔽解除 | `AimOcclusion → None → FireStand` 重新开火 | |
| 9 | 友军在火线上 | C2c-ii 触发侧移避开 | |
| 10 | C2c-iv 侧移射击 | `Weapon_Strafe` 生效时随机侧移 | |

### 3.6 伤害系统（Damage）

**场景设置：**
- 生成 1 个 TestNPC，`TestMode = "Damage"`
- 玩家用武器攻击它

**测试逻辑：**
```csharp
void TestDamage()
{
    // IDamageable 接口 — NPC 被动接收伤害
    Log.Info($"[Test:Damage] Health: {CurrentHealth} | Dead: {Dead}");
    Log.Info($"[Test:Damage] GodMode: {GodMode} | Immune_Fire: {Immune_Fire}");
    Log.Info($"[Test:Damage] Alerted: {Alerted} | Flinching: {Flinching}");
}
```

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | 普通伤害 | `OnTakeDamage` → `OnDamaged("Init")` → `CurrentHealth -= Damage` | |
| 2 | 免疫链-火焰 | `IsFireDamage = true` → `Immune_Fire = true` → `return 0` | |
| 3 | 免疫链-子弹 | `IsBulletDamage = true` → 友好 NPC 子弹免疫 | |
| 4 | Boss 绕过免疫 | `VJ_ID_Boss` 攻击者 → 跳过免疫链 | |
| 5 | GodMode | `GodMode = true` → `return 0` | |
| 6 | 0 伤害 | `Damage <= 0` → `return 0` | |
| 7 | 慢速 ragdoll 伤害 | `Velocity <= 100` → `return 0`（踩尸体） | |
| 8 | 高速 prop 伤害 | `Velocity > 100` → 正常结算 | |
| 9 | 盟友伤害响应 | `DamageAllyResponse` → 集结盟友 | |
| 10 | 被动逃跑 | `Passive_AlliesRunOnDamage` → `SCHEDULE_COVER_ORIGIN` | |

### 3.7 死亡系统（Death）

**场景设置：**
- 生成 1 个 TestNPC，`TestMode = "Death"`
- 给予致死伤害

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | `CurrentHealth <= 0 && !Dead` | `BeginDeath(dmginfo, hitgroup)` 调用 | |
| 2 | 死亡序列 | `Dead = true`, `AttackType = None`, `HasMeleeAttack = false` | |
| 3 | 盟友死亡通知 | 附近盟友收到 `OnAllyKilled` + `PlaySoundSystem("AllyDeath")` | |
| 4 | DMG_DISSOLVE | 跳过死亡动画，直接 FinishDeath | |
| 5 | DMG_REMOVENORAGDOLL | 不创建尸体 | |
| 6 | 正常死亡 | `CreateDeathCorpse` 调用 | |
| 7 | 血液贴花 | `Bleeds && HasBloodDecal` → `PlaceBloodDecal` | |
| 8 | 死亡掉落 | `DropDeathLoot` → `CreateDeathLoot` | |

### 3.8 盟友系统（Allies）

**场景设置：**
- 生成 3 个同族 TestNPC，`TestMode = "Allies"`
- 攻击其中一个

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | `Allies_Check(800)` | 返回同族 NPC 列表 | |
| 2 | `Allies_CallHelp` | 附近盟友收到 `ForceSetEnemy` | |
| 3 | `Allies_Bring("Diamond")` | 集结到指定位置 | |
| 4 | 盟友死亡 | 存活盟友 `OnAllyKilled` + `BecomeEnemyToPlayer` 连锁 | |
| 5 | PassiveNature 盟友 | 仅 `SCHEDULE_COVER_ORIGIN`，不反击 | |
| 6 | 同族守卫 | `VJ_NPC_Class` 交集检查防自相残杀 | |

### 3.9 实体标志系统（EntityFlags）

**场景设置：**
- 场景中有带各种标志的实体

**检查清单：**
| # | 场景 | 预期 | 实际 |
|---|------|------|------|
| 1 | `HasEntityFlag(ent, "VJ_ID_Living")` | 正确读取 `VJEntityFlags` Component | |
| 2 | `HasEntityFlag(ent, "VJ_ID_Boss")` | 正确读取 `BaseNPC.VJ_ID_Boss` 字段 | |
| 3 | 未挂 Component 的实体 | 查 `BaseNPC` 字段作为 fallback | |
| 4 | `VJ_ID_Attackable` 实体被近战 | `isProp = true`，走 Prop 交互路径 | |
| 5 | `VJ_ID_Destructible` 实体 | 被攻击目标列表包含 | |

---

## 4. Layer 2：集成场景（跨子系统）

### 4.1 完整战斗循环

**场景：** 1 个 HumanNPC（带 SMG）+ 玩家在视线内

**验证链：**
```
TickSenses → SeeEnemy/HasEnemyLOS
  → SelectSchedule → C2c-iii → WeaponAttackState = FireStand
    → NPC_Think → NPC_CanFire → NPCShoot_Primary → PrimaryAttack
      → Trace bullet + DamageInfo + ammo consumption
        → NPC_NextPrimaryFireT 冷却 → 下一轮
```

**关键断点：**
- `BaseNPC.TickSenses` 后：检查 `Enemy` 和 `Condition`
- `HumanNPC.SelectSchedule` C2c-iii 后：检查 `WeaponAttackState`
- `VJBaseWeapon.PrimaryAttack` 后：检查 `Clip1` 是否减少
- 玩家中弹后：检查 `OnTakeDamage` 是否触发

### 4.2 遮蔽 → 移动 → 重新开火

**场景：** NPC 在掩体后看到敌人，敌人躲到掩体后

**验证链：**
```
SelectSchedule C2b → DoCoverTrace true → AimOcclusion → NextChaseTime
  → 延迟后 → fallback → MaintainAlertBehavior → SCHEDULE_ALERT_CHASE
    → 移动到新位置 → 敌人再次可见 → SelectSchedule C2c-iii → FireStand
```

### 4.3 死亡 → 盟友反应 → 连锁仇恨

**场景：** 2 个同族 NPC，玩家杀死其中一个

**验证链：**
```
NPC1.BeginDeath → Allies_Check → NPC2.OnAllyKilled
  → NPC2.BecomeEnemyToPlayer → AddEntityRelationship(D_HT)
    → NPC2.SetEnemy(player) → NPC2 SCHEDULE_ALERT_CHASE
```

### 4.4 多 NPC 无崩溃压力

**场景：** 10 个 NPC 混战 5 分钟

**验证：**
- 无 NullReferenceException
- 无死循环（检查 `CurrentSchedule` 是否每帧反复创建）
- 无内存泄漏（检查 `_relationshipDisp` 字典是否无限增长）

---

## 5. Layer 3：自由场景

在 Sandbox 模式下手动验证，不需特殊代码：

1. 用 SpawnMenu 生成各类 NPC（CreatureNPC, HumanNPC, TankNPC）
2. 给 HumanNPC 不同类型的武器
3. 在不同距离/掩体情况下观察行为
4. 检查点：
   - NPC 是否卡住不动？（Schedule 死锁）
   - NPC 是否无视玩家？（感知失败）
   - NPC 是否无限开火不耗弹药？（武器系统 Bug）
   - NPC 死后是否清理干净？（尸体/Gib/血液）

---

## 6. 测试产出：Bug 分类标准

| 严重度 | 定义 | 示例 |
|--------|------|------|
| 🔴 Critical | 崩溃/死循环/NPC 完全无反应 | NullReferenceException, 每帧重建 Schedule |
| 🟠 High | 核心行为缺失 | NPC 不攻击、武器不开火、不换弹 |
| 🟡 Medium | 逻辑正确但行为不合理 | 攻击间隔不对、遮蔽延迟时间异常 |
| 🟢 Low | 边界遗漏 | 特定组合下行为偏差 |

**Phase 2 质量门：** 0 个 Critical + High ≤ 3 个 = 可以进 Phase 3。

---

## 7. 快速启动：5 分钟冒烟测试

如果时间紧，至少跑这 5 步：

```
□ 1. 生成 TestNPC，站在它面前
     预期：TickSenses 找到敌人，Console 输出 Enemy + Conditions

□ 2. 给 TestNPC 一把 SMG（HumanNPC + DoChangeWeapon）
     预期：开火，消耗弹药，Console 输出 FireStand → NPCShoot_Primary → PrimaryAttack

□ 3. 躲在墙后面
     预期：NPC 追过来或等待遮蔽延迟

□ 4. 用枪打 NPC
     预期：OnTakeDamage 触发，Flinch，血量减少

□ 5. 杀死 NPC
     预期：BeginDeath → FinishDeath，无崩溃
```

这 5 步通过 = Phase 2 核心闭环验证通过。
