# VJ Base Lua 动画系统分析

> 撰写：2026-05-10 | 目的：Phase 3 动画系统迁移参考

---

## 1. 总览

VJ Base 动画系统建立在 Source 引擎的 **ACT_* 活动系统** 之上。核心流程：

```
AI 决策 → TranslateActivity(ACT_generic) → AnimationTranslations 查表
→ ACT_model_specific → PlayAnim() → 引擎 SelectWeightedSequence → 播放
```

关键特征：
- **动画驱动 AI 门控**：攻击/移动/空闲行为受动画锁定计时器约束
- **模型适配层**：AnimationTranslations 将通用 ACT_* 映射为模型特定活动
- **Pose 参数**：每帧平滑跟踪敌人位置，驱动 head/aim 骨骼

---

## 2. 核心 API（6 个函数）

### 2.1 PlayAnim — 动画播放入口

**文件：** `lua/vj_base/ai/core.lua:631`  
**签名：**
```lua
function ENT:PlayAnim(animation, lockAnim, lockAnimTime, faceEnemy, animDelay, extraOptions, customFunc)
    → animation, animTime, animType
```

**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| `animation` | ACT_* / string / table | 活动常量、"vjseq_xxx"序列、"vjges_xxx"手势、或 table（随机选） |
| `lockAnim` | bool / "LetAttacks" | `true`=完全锁定 / `false`=可中断 / `"LetAttacks"`=仅攻击可中断 |
| `lockAnimTime` | number / false | `false`=自动从动画时长计算 / number=手动覆盖 |
| `faceEnemy` | bool / "Visible" | 播放期间是否面朝敌人 |
| `animDelay` | number | 延迟开始时间（秒） |
| `extraOptions` | table | 子选项：`OnFinish`, `AlwaysUseSequence`, `AlwaysUseGesture`, `PlayBackRate` |
| `customFunc` | function | 回调 `(schedule, animation)` |

**内部流程：**
1. 类型识别：`vjges_` 前缀 → 手势，`vjseq_` 前缀 → 序列，其余 → 活动
2. 调用 `self:TranslateActivity(animation)` 做活动翻译
3. `VJ.AnimExists` 最终有效性检查
4. 锁定逻辑：
   - `lockAnim=true`：设 `AnimLockTime`、`NextChaseTime`、`NextIdleTime` = `CurTime + animDur`
   - `lockAnim ~= "LetAttacks"`：调 `StopAttacks(true)` + `PauseAttacks=true` + `timer.Create("attack_pause_reset", ...)`
5. 播放路径：
   - 手势 → `AddGesture(anim, 1, 0.5 * playbackRate)`
   - 活动 → 创建 `TASK_VJ_PLAY_ACTIVITY` schedule
   - 序列 → 创建 `TASK_VJ_PLAY_SEQUENCE` schedule
6. OnFinish 回调：`timer.Simple(animTime, extraOptions.OnFinish)`

### 2.2 TranslateActivity — 活动翻译

三个实现，按优先级覆盖：

**生物基类** `npc_vj_creature_base/init.lua:1809`：
```lua
function ENT:TranslateActivity(act)
    local translation = self.AnimationTranslations[act]
    if translation then
        if istable(translation) then
            if act == ACT_IDLE then return self:ResolveAnimation(translation) end
            return translation[math.random(1, #translation)] or act
        end
        return translation
    end
    return act
end
```
纯查表翻译，无上下文逻辑。

**人类基类** `npc_vj_human_base/init.lua:2417`：
在查表前增加**战斗上下文判断**：
- `ACT_IDLE` + 无武器/害怕 → `ACT_COWER`；有武器+警戒 → `ACT_IDLE_ANGRY`
- `ACT_RUN` + 无武器/害怕 → `ACT_RUN_PROTECTED`
- `ACT_RUN`/`ACT_WALK` + 警戒 + 敌人可见 + `Weapon_CanMoveFire` → `ACT_RUN_AIM`/`ACT_WALK_AIM`
- 否则 → `ACT_RUN_AGITATED`/`ACT_WALK_AGITATED`
- 最后回落查 `AnimationTranslations` 表

**武器基类** `weapon_vj_base/shared.lua:921`：
```lua
function SWEP:TranslateActivity(act)
```
按优先级查：owner 是 VJ NPC → owner.AnimationTranslations；非 VJ NPC → ActivityTranslateAI[act]；玩家 → ActivityTranslate[act]

### 2.3 AnimExists — 动画有效性

**文件：** `lua/vj_base/funcs.lua:310`

```lua
function VJ.AnimExists(ent, anim)
```
- 活动（number）：`SelectWeightedSequence(anim) != -1` 且序列名非 "Not Found!"
- 序列（string）：`LookupSequence(anim) != -1`
- 自动剥离 `vjges_` 前缀并回退到活动检测

### 2.4 AnimDuration / AnimDurationEx — 动画时长

**文件：** `lua/vj_base/funcs.lua:351, 382`

```lua
function VJ.AnimDuration(ent, anim) → seconds
```
- 活动 → `SequenceDuration(SelectWeightedSequence(anim))`
- 序列 → `SequenceDuration(LookupSequence(anim))`

```lua
function VJ.AnimDurationEx(ent, anim, override, decrease) → seconds
```
- `override` 为 number → `override / AnimPlaybackRate`
- `override` 为 falsy → `(AnimDuration(anim) - decrease) / AnimPlaybackRate`

### 2.5 IsCurrentAnim — 当前动画匹配

**文件：** `lua/vj_base/funcs.lua:424`

```lua
function VJ.IsCurrentAnim(ent, anim)
```
- 支持 table 参数：遍历条目，任一匹配返回 true
- 活动：`anim == GetActivity()` 或 `TranslateActivity(anim) == GetActivity()`
- 序列：`LookupSequence(anim) == GetSequence()`

### 2.6 UpdatePoseParamTracking — Pose 参数跟踪

**文件：** `npc_vj_human_base/init.lua:3426`（人类版，更完整）  
**文件：** `npc_vj_creature_base/init.lua:2752`（生物版）

每帧计算视线到目标的三轴差值（pitch/yaw/roll），用 `math.ApproachAngle` 平滑过渡，通过 `SetPoseParameter` 写入 pose parameter。

人类版额外门控：仅在 `WeaponAttackState >= FIRE` 或 `VJ_IsBeingControlled` 时更新。

---

## 3. 动画锁定/状态机

### 3.1 六个锁定计时器

| 字段 | 设置位置 | 作用 |
|------|---------|------|
| `AnimLockTime` | PlayAnim | 动画锁定，阻止 `IsBusy("Activities")` |
| `AttackAnimTime` | Execute*Attack | 攻击动画剩余时间 = `CurTime + AnimDuration - DecreaseLength` |
| `NextChaseTime` | PlayAnim / 多处 | 下一次追击触发时间 |
| `NextIdleTime` | PlayAnim | 下一次空闲行为触发时间 |
| `PauseAttacks` | PlayAnim | 攻击系统全局暂停（timer 异步恢复） |
| `NextDoAnyAttackT` | ScheduleAttackTimers | 下一次任何攻击可触发时间 |

### 3.2 IsBusy 检查

**文件：** `core.lua:881`

```lua
function ENT:IsBusy(bypass)
```
检查 `PauseAttacks` / `AnimLockTime > CurTime` / `AttackAnimTime > CurTime`，任一为真返回 true。

### 3.3 WeaponAttackState 与动画绑定

**枚举定义：** `enums.lua:105-111`

| 状态 | 值 | 含义 |
|------|-----|------|
| `NONE` | `false` | 无攻击状态 |
| `AIM` | 1 | 瞄准中 |
| `AIM_MOVE` | 2 | 移动瞄准 |
| `AIM_OCCLUSION` | 3 | 敌人遮蔽延迟 |
| `FIRE` | 10 | 射击中（可移动） |
| `FIRE_STAND` | 11 | 射击中（站立/蹲伏） |

**状态流转：**
```
NONE → AIM_OCCLUSION（遮蔽延迟触发）
NONE → FIRE_STAND（进入射击动画成功）
AIM_OCCLUSION → NONE（遮蔽解除，重新进攻击）
FIRE_STAND → FIRE（IsCurrentAnim 为真时维持）
FIRE_STAND → NONE（Flinch / Schedule 中断）
```

**NPC_CanFire 的动画门控：** `weapon_vj_base/shared.lua:557`
```lua
-- 仅当 WeaponAttackState == FIRE，或 (FIRE_STAND && IsCurrentAnim(WeaponAttackAnim)) 时允许开火
if (WeaponAttackState == FIRE) or (WeaponAttackState == FIRE_STAND && IsCurrentAnim(owner, WeaponAttackAnim)) then
    -- 允许
end
```

---

## 4. 动画配置表（27 个 AnimTbl_* 字段）

### 4.1 生物基类（CreatureNPC）— 12 个

| 字段 | 默认值 | 用途 |
|------|--------|------|
| `Aerial_AnimTbl_Calm` | `ACT_FLY` | 飞行-平静 |
| `Aerial_AnimTbl_Alerted` | `ACT_FLY` | 飞行-警戒 |
| `Aquatic_AnimTbl_Calm` | `ACT_SWIM` | 游泳-平静 |
| `Aquatic_AnimTbl_Alerted` | `ACT_SWIM` | 游泳-警戒 |
| `AnimTbl_CallForHelp` | `false` | 呼叫支援 |
| `AnimTbl_Medic_GiveHealth` | `ACT_SPECIAL_ATTACK1` | 医疗动画 |
| `AnimTbl_Flinch` | `ACT_FLINCH_PHYSICS` | 受击硬直 |
| `AnimTbl_DamageAllyResponse` | `false` | 盟友受伤响应 |
| `AnimTbl_Death` | `{}` | 死亡序列集 |
| `AnimTbl_MeleeAttack` | `ACT_MELEE_ATTACK1` | 近战攻击 |
| `AnimTbl_RangeAttack` | `ACT_RANGE_ATTACK1` | 远程攻击 |
| `AnimTbl_LeapAttack` | `ACT_SPECIAL_ATTACK1` | 跳跃攻击 |

### 4.2 人类基类（HumanNPC）— 额外 15 个

| 字段 | 默认值 | 用途 |
|------|--------|------|
| `AnimTbl_CallForHelp` | `{ACT_SIGNAL_ADVANCE, ACT_SIGNAL_FORWARD}` | 重写生物版 |
| `AnimTbl_TakingCover` | `ACT_COVER_LOW` | 进入掩体 |
| `AnimTbl_MoveToCover` | `ACT_RUN_CROUCH` | 跑向掩体 |
| `AnimTbl_GrenadeAttack` | `"grenThrow"` | 投掷手雷 |
| `AnimTbl_WeaponAttack` | `ACT_RANGE_ATTACK1` | 武器射击 |
| `AnimTbl_WeaponAttackGesture` | `ACT_GESTURE_RANGE_ATTACK1` | 射击手势层 |
| `AnimTbl_WeaponAttackCrouch` | `ACT_RANGE_ATTACK1_LOW` | 蹲射 |
| `AnimTbl_WeaponAttackSecondary` | `ACT_RANGE_ATTACK2` | 副武器攻击 |
| `AnimTbl_WeaponReload` | `ACT_RELOAD` | 换弹 |
| `AnimTbl_WeaponReloadCovered` | `ACT_RELOAD_LOW` | 蹲姿换弹 |
| `AnimTbl_Flinch` | `ACT_FLINCH_PHYSICS` | 重写生物版 |
| `AnimTbl_DamageAllyResponse` | `ACT_SIGNAL_GROUP` | 重写生物版 |
| `AnimTbl_Death` | `{}` | 重写生物版 |
| `AnimTbl_MeleeAttack` | `ACT_MELEE_ATTACK1` | 重写生物版 |
| `AnimTbl_Medic_GiveHealth` | `ACT_SPECIAL_ATTACK1` | 重写生物版 |

### 4.3 武器基类（VJBaseWeapon）— 5 个

| 字段 | 默认值 | 用途 |
|------|--------|------|
| `AnimTbl_Deploy` | `ACT_VM_DRAW` | 掏出武器 |
| `AnimTbl_Idle` | `ACT_VM_IDLE` | 武器待机 |
| `AnimTbl_Reload` | `ACT_VM_RELOAD` | 换弹（Viewmodel） |
| `AnimTbl_SecondaryFire` | `ACT_VM_SECONDARYATTACK` | 副攻击 |
| `AnimTbl_PrimaryFire` | `ACT_VM_PRIMARYATTACK` | 主攻击（Viewmodel） |

---

## 5. 活动翻译系统（ACT_* 映射）

### 5.1 ACT_* 常量

VJ Base 使用 Source 引擎的 ~60 个 ACT_* 常量。分类：

| 类别 | 常量示例 | 约数 |
|------|---------|------|
| 空闲 | `ACT_IDLE`, `ACT_IDLE_ANGRY`, `ACT_COWER`, `ACT_IDLE_AIM_RELAXED` | ~15 |
| 移动 | `ACT_WALK`, `ACT_RUN`, `ACT_RUN_AIM`, `ACT_RUN_CROUCH`, `ACT_SWIM`, `ACT_FLY` | ~15 |
| 战斗 | `ACT_MELEE_ATTACK1`, `ACT_RANGE_ATTACK1`, `ACT_GESTURE_RANGE_ATTACK1` | ~10 |
| 换弹 | `ACT_RELOAD`, `ACT_RELOAD_LOW`, `ACT_GESTURE_RELOAD` | ~6 |
| 掩体 | `ACT_COVER_LOW`, `ACT_COVER_PISTOL_LOW` | ~4 |
| ViewModel | `ACT_VM_DRAW`, `ACT_VM_PRIMARYATTACK`, `ACT_VM_RELOAD` | ~6 |
| 特殊 | `ACT_INVALID`, `ACT_TRANSITION`, `ACT_RESET`, `ACT_FLINCH_PHYSICS` | ~4 |

### 5.2 AnimationTranslations 构建

**文件：** `npc_vj_human_base/init.lua:904` — `SetAnimationTranslations(wepHoldType)`

根据 4 个模型集（Combine/Metrocop/Rebel/Player）× 7 个武器持握类型（ar2/smg/rpg/pistol/revolver/shotgun/melee），构建 `{ACT_generic → ACT_model_specific}` 映射表。

示例（Combine 模型 + AR2 持握）：
```lua
AnimationTranslations = {
    [ACT_IDLE] = {ACT_IDLE, ACT_IDLE_ANGRY_SMG1},  -- table = 随机选
    [ACT_RUN]  = ACT_RUN_AIM_RIFLE,                   -- 固定映射
    [ACT_WALK] = ACT_WALK_AIM_RIFLE,
    ...
}
```

### 5.3 查询链

```
NPC:TranslateActivity(ACT_RUN)
  → HumanNPC 上下文判断（武器/敌人/状态）
    → ACT_RUN_AIM
  → AnimationTranslations[ACT_RUN_AIM]
    → ACT_RUN_AIM_RIFLE
  → PlayAnim(ACT_RUN_AIM_RIFLE, ...)
    → SelectWeightedSequence(ACT_RUN_AIM_RIFLE) → 引擎播放
```

---

## 6. Pose 参数系统

### 6.1 初始化检测

**文件：** `npc_vj_human_base/init.lua:2219-2236`

自动检测模型上的 6 个常见 pose parameter：
```lua
"aim_pitch", "head_pitch"   -- Pitch 候选
"aim_yaw",   "head_yaw"     -- Yaw 候选
"aim_roll",  "head_roll"    -- Roll 候选
```

用 `LookupPoseParameter` 探测，取第一个存在的存入 `PoseParameterLooking_Names`。

### 6.2 每帧更新

**文件：** `npc_vj_human_base/init.lua:3426`

```
计算 eyePos → targetPos 的 pitch/yaw/roll 差值
  → math.ApproachAngle(当前值, 目标值, 逼近速度 * frameTime)
    → SetPoseParameter(name, value)
```

门控条件：`WeaponAttackState >= FIRE` 或 `VJ_IsBeingControlled` 或 `HasPoseParameterLooking == false`

---

## 7. Think 钩子系统

每个实体注册一个 per-tick Think 钩子来维护空闲动画：

**文件：** `npc_vj_human_base/init.lua:2118-2129`
```lua
local function funcAnimThink(self)
    if VJ_CVAR_AI_ENABLED then MaintainIdleAnimation(self) end
end
hook.Add("Think", self, funcAnimThink)
```

有骨骼跟随器时（如市民背包）使用 `funcAnimThinkExtra`，额外调用 `UpdateBoneFollowers()`。使用 `hook.Add("Think")` 而非 `NextThink` 以避免性能问题。

### MaintainIdleAnimation

**文件：** `core.lua:526`

- Force 模式：重置 `LastAnimSeed`，设 `ACT_IDLE`，清 cycle
- Auto 模式：cycle >= 0.98 或 `TranslateActivity(ACT_IDLE)` 返回不同动画时重启空闲，否则 `m_bSequenceLoops = true`

---

## 8. S&Box API 对照：不缺底层，缺活动抽象层

### 8.1 S&Box 已有接口 = Source 等价物

| 需求 | Source (GMod) | S&Box | 结论 |
|------|--------------|-------|------|
| 播放序列 | `SelectWeightedSequence(ACT) + SetIdealActivity` | `SkinnedModelRenderer.Sequence` + `AnimGraphDirectPlayback.Play(name)` | ✅ |
| 序列时长 | `SequenceDuration(seq)` | `SequenceAccessor.Duration` | ✅ |
| 序列是否存在 | `LookupSequence(name) != -1` | `SequenceAccessor.SequenceNames.Contains(name)` | ✅ |
| 当前序列 | `GetSequence()` | `AnimGraphDirectPlayback.Name` | ✅ |
| 播放进度 | `GetCycle()` | `SequenceAccessor.TimeNormalized` | ✅ |
| 播放速率 | `SetPlaybackRate(rate)` | `SkinnedModelRenderer.PlaybackRate` / `SequenceAccessor.PlaybackRate` | ✅ |
| Pose 参数读写 | `LookupPoseParameter` / `SetPoseParameter` | `ParameterAccessor.Contains(name)` / `Set(name, float)` | ✅ |
| 骨骼查找 | `LookupBone(name)` | `SkinnedModelRenderer.GetBoneObject(name)` | ✅ |
| Attachment | `LookupAttachment` / `GetAttachment` | `SkinnedModelRenderer.GetAttachment(name, worldSpace)` | ✅ |
| 动画事件 | `SetAnimationEvent(key, func)` | `OnFootstepEvent` / `OnSoundEvent` / `OnAnimTagEvent` | ✅ |
| 每帧更新 | `hook.Add("Think", ...)` | `Component.OnUpdate()` | ✅ |

**结论：底层接口全覆盖。** 序列播放、参数控制、骨骼查询、动画事件 — S&Box 全有对应。

### 8.2 真正缺失的：ACT_* 活动抽象层

S&Box **故意**去掉了 Source 的 ACT_* 系统。这不是 API 缺口，是设计哲学差异：

| 缺失层 | Source 做法 | S&Box 做法 |
|--------|------------|-----------|
| **活动常量** | 60+ ACT_* 引擎枚举，所有 HL2 模型通用 | 不存在。每个模型有独立序列名/Animgraph 参数名 |
| **加权序列选择** | `SelectWeightedSequence(ACT_RUN)` → 自动从模型序列中选最优 | 不存在。需手动从 `SequenceNames` 列表里挑选 |
| **手势叠加层** | `AddGesture(anim, slot, rate)` → 在基础动画上叠加独立手势轨道 | 无独立 API。需 Animgraph blend layer 实现 |

Source ACT_* 是 2004 年 Half-Life 2 遗产，深度绑定 HL2 角色模型。S&Box 的 Animgraph 是现代化参数驱动方案。

### 8.3 两条迁移路线

#### 路线 A：序列直驱（简单，Source 风格）

关掉 `UseAnimGraph`，用 `SequenceAccessor` + `AnimGraphDirectPlayback` 按名播放：

```csharp
var renderer = GameObject.Components.Get<SkinnedModelRenderer>();

// 检查序列是否存在（替代 AnimExists）
if (renderer.Sequence.SequenceNames.Contains("run_rifle"))
{
    // 方式1: 通过 SkinnedModelRenderer 直接控制 Sequence
    renderer.UseAnimGraph = false;

    // 方式2: 通过 Animgraph DirectPlayback 节点播放
    var dp = renderer.AnimationGraph?.GetDirectPlayback();
    dp?.Play("run_rifle");
}

// 获取当前播放信息（替代 GetSequence / GetCycle）
var seq = renderer.Sequence;
float duration = seq.Duration;           // 替代 SequenceDuration
float progress = seq.TimeNormalized;     // 替代 GetCycle
bool finished = seq.IsFinished;          // 替代 OnFinish 回调
seq.PlaybackRate = 1.5f;                 // 替代 SetPlaybackRate
```

优点：行为最接近 Source，逻辑简单直接  
缺点：放弃 Animgraph 混合/过渡/IK；需为每个 NPC 模型建 `ACT_* → 序列名` 映射表

#### 路线 B：Animgraph 参数驱动（现代化）

保留 Animgraph，用参数触发状态切换：

```csharp
renderer.Set("move_speed", 1.0f);     // 触发 Run 动画
renderer.Set("aim_pitch", angle);     // 驱动 aim_pitch
renderer.Set("b_attack", true);       // 触发攻击
```

优点：S&Box 原生方案，混合/过渡/IK 自动处理  
缺点：需为每类 NPC 制作 Animgraph 资产；参数名因模型而异

### 8.4 路线 A 可行性确认

> **路线 A 能跑通。** 不依赖 Animgraph 资产，纯代码驱动。

需要补齐的：

| 组件 | 工作量 | 说明 |
|------|--------|------|
| ACT_* 枚举 | ~80 行 C# | 从 Source SDK 搬运 60+ 常量 |
| 序列映射数据库 | ~150 行 JSON/C# | 每个模型 `{ACT_RUN: "run_forward", ACT_IDLE: "idle_01", ...}` |
| `PlayAnim(act, ...)` 重写 | ~200 行 C# | `TranslateActivity → 查映射表 → 找到序列名 → Sequence.Play(name)` |
| `AnimExists(act)` | ~20 行 | 查 `SequenceNames.Contains(mappedName)` |
| `AnimDuration(act)` | ~15 行 | `Sequence.Duration` |
| `IsCurrentAnim(act)` | ~15 行 | `DirectPlayback.Name == mappedName` |
| Pose 参数 | ~100 行 | `ParameterAccessor.Set(name, value)` 替代 `SetPoseParameter` |
| Think 钩子 | ~20 行 | `Component.OnUpdate()` 替代 `hook.Add("Think")` |
| 替换 ~45 个 SKIP | ~120 行 | 现有 SKIP 注释 → `PlayAnim(AnimTbl_*)` 调用 |

**总计：~800 行 C# + 每模型序列映射表。**

阻塞项：**需要模型序列名数据**。每个 VJ NPC 模型（Combine 士兵、市民、僵尸等）都得知道它的 `ACT_RUN` / `ACT_IDLE` / `ACT_MELEE_ATTACK1` 等分别对应哪个序列名。这个数据可以通过：
- 运行时调用 `SequenceAccessor.SequenceNames` 拿到全部序列列表
- 用命名约定自动匹配（大多数 HL2 模型序列名遵循 `act_name` 模式）
- 对无法自动匹配的模型手动配置

### 8.5 放弃混合的影响

如果走路线 A（无 Animgraph 混合），以下是**会丢失**的表现效果：

| 丢失的能力 | 影响程度 | 说明 |
|-----------|---------|------|
| 动画混合/过渡 | 中 | 序列切换时无平滑过渡，NPC 会"突然切换"姿势 |
| 手势叠加 | 低 | 无法在基础动画上叠武器开火手势（`ACT_GESTURE_RANGE_ATTACK1`） |
| Animgraph IK | 低 | 骨骼 IK（脚贴地、手抓武器）丢失 |
| 参数驱动的姿势微调 | 低 | `aim_pitch/yaw` 的平滑 Look-At 需要手动实现 |

**逻辑层完全不受影响** — AI 行为、攻击、移动、死亡序列都是通过状态字段门控，不依赖动画混合。

---

## 9. 文件索引

| 文件 | 角色 | 关键行 |
|------|------|--------|
| `lua/vj_base/ai/core.lua` | PlayAnim, MaintainIdleAnimation, MaintainIdleBehavior, UpdateAnimationTranslations, IsBusy, 状态变量定义 | 143-167, 485, 526, 561, 631, 881 |
| `lua/vj_base/funcs.lua` | AnimExists, AnimDuration, AnimDurationEx, IsCurrentAnim, GetPoseParameters | 310, 351, 382, 424, 455 |
| `lua/vj_base/enums.lua` | VJ.ANIM_TYPE_*, VJ.ANIM_SET_*, VJ.WEP_ATTACK_STATE_* | 100-140 |
| `lua/vj_base/ai/schedules.lua` | TASK_VJ_PLAY_ACTIVITY/SEQUENCE, RunAI, MaintainActivity | 112-162, 424 |
| `lua/entities/npc_vj_creature_base/init.lua` | TranslateActivity, funcAnimThink, UpdatePoseParamTracking, AnimTbl_* 默认值 | 72-314, 1535-1665, 1809, 2752 |
| `lua/entities/npc_vj_human_base/init.lua` | TranslateActivity(战斗上下文), SetAnimationTranslations, UpdatePoseParamTracking | 103-314, 904-1050, 2417, 3426 |
| `lua/weapons/weapon_vj_base/shared.lua` | SWEP:TranslateActivity, NPC_CanFire, ActivityTranslateAI, AnimTbl_* | 88-140, 548-590, 921-941, 1092-1254 |
| `lua/entities/obj_vj_controller/init.lua` | 玩家控制 NPC 的动画/WeaponAttackState 集成 | 396-425 |

---

## 10. 实现踩坑记录

> 2026-05-11 动画系统 Route A 落地过程中发现的 6 个陷阱。

**坑 1: Route A 适配不是"删掉重写"。** `dp.Play()` 替代 `StartSchedule(TASK_VJ_PLAY_*)` 是播放方式变化，不是删除功能。锁定计时器（AnimLockTime/NextChaseTime/NextIdleTime）仍需 1:1 维护——它们才是行为门控的核心。

**坑 2: SequenceToActivity 需要反向查询。** Lua 的 `VJ.SequenceToActivity(self, "walkeasy_all")` 调用 Source `GetSequenceActivity(LookupSequence(name))` 查询引擎内部活动映射表。S&Box 无此数据。需要运行时扫描 `SequenceNames` + 反向匹配 `Activity→序列名` 映射表。不存在时返回 null 让调用方 fallback，不能硬编码。

**坑 3: AnimTbl_* 默认值不能为空列表。** Phase 1 翻译只建了字段壳（`= new()`），必须填入 Lua 默认值。空列表 → `VJUtility.PICK(空) → null → PlayAnim 返回 Invalid`，所有动画静默跳过，没有任何编译错误或运行时异常。

**坑 4: IsBusy 空壳让动画锁全部失效。** `IsBusy()` 返回 false 意味着 NPC 永远不忙——动画播放期间 SelectSchedule 可以随时抢走控制权。必须检查 `PauseAttacks`/`AnimLockTime`/`AttackAnimTime`。

**坑 5: TranslateActivity 不是简单 key→value 查表。** HumanNPC 覆写有 5 层前置 if/elseif 判断（Cower/Angry/Aim-Move/Protected/Agitated），必须严格按 Lua 分支顺序实现，否则战斗动画选择错误。

**坑 6: "还原度"评估必须有对照表。** 笼统的百分比（60%/88%/93%）没有意义。必须列出每个 Lua 方法/块的 C# 对应行和差异点，否则评估是自欺欺人。

---

*分析完成于 2026-05-10，实现完成于 2026-05-11。基于 VJ-Base-master Lua 源码 89 文件全量扫描。*
