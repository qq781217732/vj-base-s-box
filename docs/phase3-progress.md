# Phase 3 填坑进度

> **目标**：把 Phase 1 翻译产物中的 SKIP 注释逐个实现，使 NPC 能在 S&Box 中完整运行。
> **前序文档**：[translation-guide.md](translation-guide.md) — 翻译架构、Phase 1/3 定义、文件映射
> **PX 排除清单**：[px-permanent-exclusions.md](px-permanent-exclusions.md)

**最后更新**：2026-05-11
**当前状态**：P0 ✅ | P1 ✅ | P2 Animation ✅ (93%) | 剩余 ~8 SKIP + 45 PX

---

## 总体进度

| 优先级 | 子系统 | 状态 | 说明 |
|--------|-------|------|------|
| **P0** | VJUtility 缺口 | ✅ | GetNearestDistance/TraceDirections/DamageSpecialEnts |
| **P0** | Collision/Bounds | ✅ | OBB 包围盒/碰撞组 |
| **P0** | Flag System | ✅ | FL_* 标志位 + 难度系统 |
| **P0** | Damage/Health | ✅ | NPC 伤害/死亡核心回路 + 免疫链 |
| **P0** | Timer 替换 | ✅ | Bleed/Death/Alert timer → 轮询 |
| **P0** | AI/Nav/Schedule | ✅ | 掩体/关系记忆/可达性/盟友系统 |
| **P0** | Sound 核心 | ✅ | PlaySoundSystem 35 分支 + SoundLevel/Duration 真实映射 |
| **P0** | Perception | ✅ | AISenses 感知层 + LookForObjects |
| **P1** | Weapon 系统 | ✅ | IVJBaseWeapon + VJBaseWeapon + 射击回路 + PrimaryAttack 守卫 |
| **P1** | Entity Spawn | ✅ | grenade spawn 回调 + landDir + Creator |
| **P1** | Movement | ✅ | DoChangeMovementType NavMeshAgent 映射 + Water + Door |
| **P1** | Prop 交互 | ✅ | FixedJoint.Destroy + MaintainPropInteraction |
| **P1** | I/O 系统 | ✅ | TriggerOutput + OnNPCKilled static event |
| **P1** | Player 交互 | ➖ PX / 延后 | ViewPunch→PX, Controller→PX, SetDSP→延后 |
| **P1** | Physics/Force | ➖ PX | SetDamageForce→S&Box Rigidbody 已覆盖, IsNextBot→PX |
| **P2** | Animation 系统 | ✅ 完成 | PlayAnim/TranslateActivity/SetAnimationTranslations/PoseParam/IsBusy/IsCurrentAnim 全部 1:1；27 AnimTbl_* 默认值补全；Combine/Metrocop/Rebel/Player 4 模型集翻译表完整 |
| **P2** | Effects | ⬜ | MuzzleFlash/Particles/ShellEject/BloodDecals |
| **P2** | Corpse 系统 | ⚠️ 部分完成 | CreateDeathCorpse 骨架, Dissolve→Phase 3 |
| PX | Source 永久独占 | ➖ 45 处 | 见 [px-permanent-exclusions.md](px-permanent-exclusions.md) |

---

## P0 完成摘要（2026-05-06 ~ 2026-05-11）

| 会话 | 子系统 | 关键提交 |
|------|--------|---------|
| 2026-05-06 | MaintainRelationships 全翻译 + 关系系统 + EngineAITaskSystem | BaseNPC.Relationships.cs |
| 2026-05-06 | base_aa 机械翻译 + TurnData 转向系统 + 调查系统 | BaseNPC.AA.cs |
| 2026-05-06 | PlaySoundSystem 35 分支完整翻译 | BaseNPC.Sound.cs |
| 2026-05-07 | CreatureNPC 攻击系统 ExecuteMelee/ExecuteRange/ExecuteLeap | CreatureNPC.Think.cs |
| 2026-05-07 | HumanNPC 手雷系统 + 攻击配置字段 | HumanNPC.Think.cs |
| 2026-05-07 | HumanNPC SelectSchedule ~275 行机械翻译 | HumanNPC.Think.cs |
| 2026-05-08 | HumanNPC OnTakeDamage A-O 15 块逐行展开 | HumanNPC.Think.cs |
| 2026-05-08 | HumanNPC ResetEnemy + CanFireWeapon + CheckForDangers + DoChangeWeapon | HumanNPC.Think.cs |
| 2026-05-09 | DamageInfo 全局落地 + 免疫链 8 类型 | 全局 |
| 2026-05-09 | 实体标志系统 VJEntityFlags + 盟友系统 Allies_* | Core + Bases |
| 2026-05-09 | DoChangeMovementType 重构 + 门系统 + 武器 Phase 1 | 全局 |
| 2026-05-10 | 掩体/射线/玩家交互 3 子系统 | 全局 |
| 2026-05-10 | 水系统 WaterLevel + SoundEvent 注册表 + 调查系统接线 | 全局 |
| 2026-05-10 | Weapon Phase 2 核心回路 NPC_Think + C2b/C2c-iii + PrimaryAttack 9 守卫 | VJBaseWeapon.cs |
| 2026-05-10 | GetAttackTimer 语义修正 + prop_ragdoll 速度守卫 | BaseNPC.cs |
| 2026-05-11 | Prop joint weld + 门系统 Phase 3 预备 | CreatureNPC.Think.cs |
| 2026-05-11 | Animation Route A 落地: VJAnimationEnums + Mapper + PlayAnim + PoseParams | 3 新文件 ~1500 行 |
| 2026-05-11 | Animation 内容层: TranslateActivity 覆写 + Combine/Metrocop/Rebel/Player 翻译表 | HumanNPC.Think.cs |
| 2026-05-11 | Animation 修复: IsBusy + SequenceToActivity + FollowBone + AnimTbl_* 默认值 + 7 bug | 全局 |
| 2026-05-11 | OnNPCKilled event + Bullseye 守卫 + MaintainActivity | CreatureNPC + HumanNPC |
| 2026-05-11 | Initialize 武器初始装配 + BulletCallback dmginfo 修复 + Force | HumanNPC.Think.cs + VJBaseWeapon.cs |
| 2026-05-11 | Entity Spawn grenade spawn 回调 + landDir + Creator | VJEntitySpawner.cs |
| 2026-05-11 | PX 分类：~45 处 Source 独有 SKIP → PX | 全局 |
| 2026-05-11 | DamageInfo Weapon 构造器修复 | VJBaseWeapon.cs |

---

## 当前剩余 SKIP（~8 行）

### P1 剩余

| # | 任务 | 文件 | 说明 |
|---|------|------|------|
| 1 | SetDSP 耳鸣音效 | CreatureNPC.Think.cs:458 | 延后，等音频系统成熟 |
| 2 | Immune_Dissolve | HumanNPC.Think.cs:1995 | 特定 NPC 溶解免疫 |

### P2 剩余

| # | 类别 | 数量 | 说明 |
|---|------|------|------|
| 3 | AA_MoveAnimation | 1 | 飞行/水上移动时根据速度选动画 (base_aa.lua:1906) |
| 4 | Idle dialogue 定时器 | 1 | 闲逛音效循环 |
| 5 | Follow 跟随系统 | 2 | NPC 跟随玩家/盟友 |
| 6 | Bullseye 靶子系统 | 3 | VJ Base 工具链 |
| 7 | Fire/Eating 子系统 | 2 | 着火反应 + 进食行为 |
| 8 | MoveType 跟踪 + Effects | 2 | SetMoveType restore + RemoveEffects |

### MuzzleFlash / Effects / Corpse（无 SKIP 标记，但有大量 Phase 3 stub）

| 类别 | 说明 |
|------|------|
| MuzzleFlash | 枪口火焰粒子，S&Box ParticleSystem |
| ShellEject | 弹壳弹出，粒子或物理 |
| BloodDecals | GPU decal 投影 |
| Dissolve | Entity:Dissolve → shader/Material 渐变 |
| GibOnDeath | 碎尸/布娃娃 |

---

## PX 永久排除（45 处）

详见 [px-permanent-exclusions.md](px-permanent-exclusions.md)。

| 类别 | 数量 | 原因 |
|------|------|------|
| 碰撞/物理初始化 | ~7 | S&Box 自动生成 |
| 引擎能力标记 | ~3 | NavMeshAgent 替代 |
| Convar / 难度 | ~8 | Inspector Property 替代 |
| 聊天/消息 | ~4 | 非核心 |
| 调试系统 | ~5 | 可换 Log |
| 视野引擎设置 | ~2 | AISenses 已覆盖 |
| Save/Restore | ~2 | 不同持久化系统 |
| ViewPunch | 4 | 无原生 API |
| 控制器输入 | 2 | 不在范围 |
| Source 实体/计分 | ~5 | HL2 独有 |
| Blood/DSP/Misc | ~3 | 各自原因 |

---

## 下一步建议

1. **Follow 系统** — 功能性，用 NavMeshAgent 跟随，1-2 小时
2. **AA_MoveAnimation** — 飞行/水生移动动画，~50 行，1 小时
3. **MuzzleFlash + ShellEject** — 纯视觉效果，用 S&Box ParticleSystem，1-2 小时
4. **Dissolve** — 死亡效果，Material 参数动画，2-3 小时

### 动画系统已知限制（无法还原）

1. **Gesture 叠加层** — S&Box 无 `AddGesture` API，手势当普通序列播放
2. **Sequence 过渡动画** — Source 引擎 `FindTransitionSequence` 独占
