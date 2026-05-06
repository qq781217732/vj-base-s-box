# 审计报告: ExecuteMeleeAttack

> Lua: `lua/entities/npc_vj_creature_base/init.lua:2449-2578` (130 行)
> C#: `Code/VJBase/Bases/CreatureNPC.cs:608-677` (70 行)
> 审计日期: 2026-05-05 | 审计标准: 4 维等价

---

## 总体判定: **FAIL — 覆盖率 ~25%**

| 维度 | 判定 | 关键缺口数 |
|------|------|-----------|
| 结构等价 | ❌ FAIL | 缺失 6 个控制流分支 |
| 时序等价 | ❌ FAIL | Bleed 从 timer 变成每帧 while，时机完全不同 |
| 副作用等价 | ❌ FAIL | 缺 prop 交互、缺 player 效果、缺 knockback |
| 边界等价 | ❌ FAIL | 无 Dead/Flinching/PauseAttacks 守卫 |

---

## 维度 1: 结构等价

### 1.1 Gate 守卫 (Lua:2450-2451 → C#: 缺失)

```lua
if selfData.Dead or selfData.PauseAttacks or selfData.Flinching
   or (selfData.MeleeAttackStopOnHit && selfData.AttackState == VJ.ATTACK_STATE_EXECUTED_HIT)
then return end
```

| 守卫 | C# | 判定 |
|------|-----|------|
| `Dead` | ❌ 无 | **FAIL** — 死亡 NPC 仍可攻击 |
| `PauseAttacks` | `RunCombat()` 中有，但 `ExecuteMeleeAttack()` 无 | **FAIL** — 直接调用会绕过 |
| `Flinching` | ❌ 无 | **FAIL** |
| `MeleeAttackStopOnHit` | ❌ 无 | **FAIL** |

### 1.2 钩子回调 (Lua:2453 → C#: 缺失)

```lua
local skip = self:OnMeleeAttackExecute("Init")
```
C# 无任何 `OnMeleeAttackExecute` 调用。 **FAIL**

### 1.3 目标过滤 (Lua:2462-2466 → C#: 极简)

| Lua 过滤 | C# | 判定 |
|----------|-----|------|
| `ent == self` | `obj == GameObject` | ✅ |
| `ent:GetClass() == myClass` | ❌ 无 | **FAIL** — 同族误伤 |
| `ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled` | ❌ 无 | **FAIL** |
| `ent:IsPlayer() && VJ_CVAR_IGNOREPLAYERS` | ❌ 无 | **FAIL** |
| 角度检查 (dot product) | `MathF.Cos(MathX.DegreeToRadian(meleeAngle*0.5))` | ⚠️ **SEMI** — 角度计算差异 |

### 1.4 Prop 交互 (Lua:2472-2496 → C#: 缺失)

Lua 有完整的 prop 处理：
- `OnlyDamage` 模式
- `OnlyPush` 模式 → `phys:ApplyForceCenter()`
- Weld 移除 → `constraint.RemoveConstraints(ent, "Weld")`

C# 无任何 prop 交互逻辑。 **FAIL**

### 1.5 伤害结算 (Lua:2512-2522 → C#: 简化)

| Lua | C# | 判定 |
|-----|-----|------|
| `DamageInfo()` → 设置 Damage/DamageType/DamageForce/Inflictor/Attacker | `new DamageInfo { Damage, Attacker, Position }` | ⚠️ **SEMI** — 缺 DamageForce/DamageType/Inflictor |
| `VJ.DamageSpecialEnts()` | ❌ 无 | **FAIL** |
| `ent:TakeDamageInfo(dmgInfo, self)` | `obj.TakeDamage(dmg)` | ⚠️ **SEMI** — 缺 inflictor 参数 |

### 1.6 Bleed DOT (Lua:2524-2542 → C#: 简化)

Lua: `timer.Create()` 定时重复伤害，`timer.Remove()` 清理
C#: `ApplyBleed(damage, time)` 单次调用

| Lua | C# | 判定 |
|-----|-----|------|
| Reps 循环 | ❌ 无 | **FAIL** |
| Timer 命名 (`timer_melee_bleed` + EntIndex) | ❌ 无 | **FAIL** |
| 实体死亡时 `timer.Remove()` | ❌ 无 | **FAIL** |

### 1.7 Player 效果 (Lua:2544-2554 → C#: 缺失)

```lua
ent:ViewPunch(Angle(...))       -- 视角震动
ent:SetDSP(...)                 -- DSP 音效
self:DoMeleeAttackPlayerSpeed() -- 减速效果
```
C# 完全缺失。 **FAIL**

---

## 维度 2: 时序等价

| 时序点 | Lua | C# | 判定 |
|--------|-----|-----|------|
| Gate 检查 | 每次调用立即检查 | RunCombat 中有部分，ExecuteMeleeAttack 无 | **FAIL** |
| Hook 时机 | OnMeleeAttackExecute("Init") → 在伤害前 | ❌ 无 | **FAIL** |
| Damage 时机 | 每个命中实体逐个结算 | foreach 顺序一致 | ✅ |
| Bleed 时机 | timer 异步，独立于攻击帧 | while 循环每帧扣血 | **FAIL** — Lua 是独立 timer，C# 可能阻塞 |
| Cooldown | `_nextAttack = delay` | `_nextAttack = Config?.MeleeAttackDelay` | ✅ |
| ExtraMelee | timer.Simple 延迟 | `_ = ExtraMeleeHit()` 无 await | ⚠️ **SEMI** |

---

## 维度 3: 副作用等价

### 3.1 事件触发

| 事件 | Lua 调用 | C# | 判定 |
|------|---------|-----|------|
| OnMeleeAttackExecute("Init") | 1 次/attack | 0 | **FAIL** |
| OnMeleeAttackExecute("PreDamage", ent) | 1 次/entity | 0 | **FAIL** |
| VJ.DamageSpecialEnts | 1 次/entity | 0 | **FAIL** |

### 3.2 音效

Lua 在攻击后播放命中/未命中两种音效。C# 有 `PlaySound()` 但音效路径硬编码，不从 `SoundTbl_MeleeAttack` 读取。 ⚠️ **SEMI**

### 3.3 状态副作用

| 状态 | Lua | C# | 判定 |
|------|-----|-----|------|
| AttackState | `VJ.ATTACK_STATE_EXECUTED` / `EXECUTED_HIT` | `AttackState.ExecutedHit` / `Executed` | ✅ |
| Flinch 锁 | 无（但 gate 检查 Flinching） | 无 | ⚠️ |
| ExtraMelee 33% 概率 | `timer.Simple` 异步 | `if (Game.Random.Next(1,4) == 1)` 概率不对(25%≠33%) | ⚠️ **SEMI** |

---

## 维度 4: 边界等价

| 边界 | Lua | C# | 判定 |
|------|-----|-----|------|
| Dead 时调用 | 立即 return | 照常执行 | **FAIL** |
| Flinching 时调用 | 立即 return | 照常执行 | **FAIL** |
| 空目标 (0 个 hits) | skip 整个 loop，状态仍更新 | hits 为空则跳过 | ✅ |
| 目标已死亡 | `ent:Health() > 0` 检查 | ❌ 无 | **FAIL** |
| Bleed 目标死亡 | `timer.Remove()` | ❌ 无清理 | **FAIL** |
| Prop 无物理对象 | `IsValid(phys)` 检查 | ❌ 无 | **FAIL** |
| NextBot knockback | `loco:Approach()` / `loco:Jump()` | ❌ 无 | **FAIL** |
| MeleeAttackStopOnHit | `break` 退出循环 | ❌ 无 | **FAIL** |

---

## 审计总结

```
ExecuteMeleeAttack (Lua 130 行 → C# 70 行)

✅ PASS: 2 / 28 检查点 (7%)
   - self 跳过
   - 基础 sphere sweep

⚠️ SEMI: 5 / 28 检查点 (18%)
   - 角度计算参数差异
   - DamageInfo 字段不完整
   - 音效从硬编码而非配置表
   - ExtraMelee 概率计算差异
   - TakeDamage 缺 inflictor

❌ FAIL: 21 / 28 检查点 (75%)
   - Gate 守卫全缺 (4 项)
   - Hook 回调全缺 (2 项)
   - 目标过滤缺 4 项
   - Prop 交互全缺
   - Bleed 机制不同
   - Player 效果全缺
   - 边界检查缺 7 项
```

### 建议修复优先级

| P0 | Gate 守卫 (Dead/PauseAttacks/Flinching) | 1 行 |
| P0 | 目标过滤 (class check, player check) | ~10 行 |
| P1 | OnMeleeAttackExecute 钩子 | ~5 行 |
| P1 | Prop 交互 (OnlyDamage/OnlyPush) | ~30 行 |
| P1 | Bleed 从 ApplyBleed 改为 timer 模式 | ~20 行 |
| P2 | Player 效果 (ViewPunch/DSP/Speed) | ~15 行 |
| P2 | Knockback NextBot 处理 | ~10 行 |
