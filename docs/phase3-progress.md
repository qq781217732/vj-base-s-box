# Phase 3 填坑进度

> **目标**：把 Phase 1 翻译产物中的 SKIP 注释逐个实现，使 NPC 能在 S&Box 中完整运行。
> **前序文档**：[translation-guide.md](translation-guide.md) — 翻译架构、Phase 1/3 定义、文件映射
> **PX 排除清单**：[px-permanent-exclusions.md](px-permanent-exclusions.md)

**最后更新**：2026-05-11
**当前状态**：P0 ✅ | P1 ✅ | P2 Animation ✅ | 剩余 19 SKIP + 45 PX | ~98%

---

## 总体进度

| 优先级 | 子系统 | 状态 | 说明 |
|--------|-------|------|------|
| **P0** | VJUtility / Collision / Flag / Damage / Timer / AI / Sound / Perception | ✅ | 核心回路全部就位 |
| **P1** | Weapon / Spawn / Movement / Prop / I/O | ✅ | 武器双阶段 + 完整移动系统 |
| **P1** | Player 交互 / Physics | ➖ PX | ViewPunch/SetDSP/IsNextBot → PX |
| **P2** | **Animation** | ✅ | Route A 完整落地，4 模型集翻译表就位 |
| **P2** | Effects / Corpse | ⬜ | MuzzleFlash/ShellEject/Dissolve/GibOnDeath |
| **P3** | Follow / Fire / Eating / Bullseye / Idle | ⬜ | 边缘系统，~16 行 SKIP |
| PX | Source 永久独占 | ➖ 45 处 | 见 [px-permanent-exclusions.md](px-permanent-exclusions.md) |

---

## 已完成子系统摘要

| 子系统 | 文件 | 关键内容 |
|--------|------|---------|
| **Schedule** | BaseNPC.Schedule.cs | 32 方法，双轨已消除 |
| **AA Movement** | BaseNPC.AA.cs | 5 方法（4 完成 + 1 Phase 3 stub） |
| **Sound** | BaseNPC.Sound.cs | PlaySoundSystem 35 分支 + SoundLevel/Duration 真实映射 + SoundEventRegistry |
| **Relationships** | BaseNPC.Relationships.cs | MaintainRelationships 9/9 块 + 敌人选择 + 调查系统 |
| **Damage** | 全局 | DamageInfo 落地 + 免疫链 8 类型 + 8 Is*Damage helper |
| **Entity Flags** | VJEntityFlags.cs | 9 VJ_ID_*/VJ_ST_* 标志 + HasEntityFlag helper |
| **Allies** | BaseNPC.cs | Allies_Check/Bring/CallHelp + 5 处接线 |
| **Movement** | BaseNPC.cs | DoChangeMovementType → NavMeshAgent/Rigidbody 映射 + Water + Door |
| **Weapon** | VJBaseWeapon.cs | IVJBaseWeapon 接口 + NPC_Think 射击回路 + PrimaryAttack 9 守卫 |
| **Spawn** | VJEntitySpawner.cs | grenade spawn 回调 + landDir + Creator |
| **Cover/Trace** | VJUtility.cs | DoCoverTrace + TraceDirections + IsPlayerDetection |
| **Animation** | BaseNPC.Animation.cs + 3 新文件 | 详见下方 |

### 动画系统（2026-05-11，~1800 行，9 提交）

| 层 | 文件 | 内容 |
|----|------|------|
| 枚举 | VJAnimationEnums.cs | 175+ ACT_* 常量 + VJAnimType + VJAnimSet |
| 映射 | VJAnimationMapper.cs | 运行时序列探测，Activity↔序列名双向映射，AnimExists/AnimDuration/IsCurrentAnim/SequenceToActivity |
| 核心 | BaseNPC.Animation.cs | PlayAnim、TranslateActivity、MaintainIdleAnimation、UpdatePoseParamTracking、IsBusy、FollowBone、ParentToAttachment、GetAttachmentPos/GetBoneTransform |
| 人类覆写 | HumanNPC.Think.cs | TranslateActivity 5 层战斗上下文、SetAnimationTranslations 4 模型集（Combine/Metrocop/Rebel/Player） |
| 默认值 | BaseNPC.cs + HumanNPC.cs | 27 个 AnimTbl_* 字段全部填入 Lua 默认值 |

**已知限制（2 项，无法还原）**：Gesture 叠加层（S&Box 无 AddGesture API）、Sequence 过渡动画（Source 引擎 FindTransitionSequence 独占）。均不影响 NPC 行为。

---

## 剩余任务

### Phase 3 可执行（~16 SKIP，~8 小时）

| # | 系统 | SKIP | 文件 | 估算 |
|---|------|------|------|------|
| 1 | **Follow 跟随** | 3 | CreatureNPC.Think.cs + HumanNPC.Think.cs ×2 | 2h |
| 2 | **AA_MoveAnimation** | 1 | CreatureNPC.Think.cs | 1h |
| 3 | **OBB + MoveType** | 2 | HumanNPC.Think.cs（OBB 偏移 + SetMoveType restore） | 1h |
| 4 | **Idle dialogue** | 1 | BaseNPC.cs（FindInSphere + timer + OnIdleDialogue） | 0.5h |
| 5 | **Fire 系统** | 1 | HumanNPC.Think.cs（!isFireEnt guard） | 1h |
| 6 | **Eating 系统** | 1 | HumanNPC.Think.cs（CanEat + ResetEatingBehavior） | 1h |
| 7 | **Immune_Dissolve** | 1 | HumanNPC.Think.cs（特定 NPC 溶解免疫守卫） | 0.5h |
| 8 | **RemoveEffects** | 1 | HumanNPC.Think.cs（EF_FOLLOWBONE 移除） | 0.5h |

### 可延后（~7 SKIP）

| # | 系统 | SKIP | 原因 |
|---|------|------|------|
| 9 | Bullseye 靶子 | 5 | VJ Base 工具链，非核心 NPC 行为 |
| 10 | SetDSP | 1 | S&Box 音频效果系统未成熟 |
| 11 | Controller/Tool | 2 | 玩家控制 NPC 工具，不在当前范围 |

### 效果/视觉层（无 SKIP 标记，Phase 3 stub）

| # | 内容 | 估算 | 说明 |
|---|------|------|------|
| 12 | **MuzzleFlash** | 2h | 枪口火焰，ParticleSystem |
| 13 | **ShellEject** | 1h | 弹壳弹出 |
| 14 | **Dissolve** | 3h | 死亡溶解效果，Material 参数动画 |
| 15 | **GibOnDeath** | 2h | 碎尸/布娃娃 |
| 16 | **BloodDecals** | 1h | GPU decal 投影 |

### PX 永久排除（45 处）

详见 [px-permanent-exclusions.md](px-permanent-exclusions.md)。

| 类别 | 数量 | 原因 |
|------|------|------|
| 碰撞/物理初始化 | ~7 | S&Box 自动生成 |
| Convar / 难度 / 调试 | ~13 | Inspector Property 替代 |
| ViewPunch | 4 | 无原生 API |
| 控制器输入 | 2 | 不在范围 |
| Source 实体/计分/其他 | ~19 | HL2 独有或各自原因 |

---

## 动画系统经验总结

### 新增陷阱

14. **Route A 适配不是"删掉重写"**：`dp.Play()` 替代 `StartSchedule(TASK_VJ_PLAY_*)` 是播放方式变化，不是删除功能。锁定计时器（AnimLockTime/NextChaseTime/NextIdleTime）仍需 1:1 维护——它们才是行为门控的核心。
15. **SequenceToActivity 需要反向查询**：Lua 的 `VJ.SequenceToActivity(self, "name")` 调用 Source `GetSequenceActivity(LookupSequence(name))` 查询引擎内部活动表。S&Box 无此数据，需要运行时扫描 `SequenceNames` + 反向匹配 `Activity→序列名` 映射表。不存在时返回 null 让调用方 fallback。
16. **AnimTbl_* 默认值不能为空**：Phase 1 翻译只建了字段壳（`= new()`），必须填入 Lua 默认值，否则 `VJUtility.PICK(空列表) → null → PlayAnim 返回 Invalid`，所有动画静默跳过。
17. **IsBusy 空壳会让动画锁失效**：`IsBusy()` 返回 false 意味着 NPC 永远不忙——动画播放期间 SelectSchedule 可以随时抢走控制权。必须检查 `PauseAttacks`/`AnimLockTime`/`AttackAnimTime`。
18. **TranslateActivity 是查表入口点**：不是简单的 key→value 映射。HumanNPC 覆写有 5 层前置判断（Cower/Angry/Aim-Move/Protected/Agitated），必须严格按 if/elseif 顺序实现。

### 自审强化

- 动画系统的 bug 全部是 **"看上去能编译、跑起来静默无效"** 类型——空列表 PICK 返回 null、IsBusy 永远 false、翻译表空导致 AnimExists 失败。逐行对照 Lua 原文的 `§5.5` 自审流程对这类问题最有效。
- **"还原度"评估必须有对照表支撑。** 笼统的百分比（60%/88%/93%）没有意义——必须列出每个 Lua 方法/块的 C# 对应行和差异点。

---

## 下一步建议（按优先级）

1. **Follow 跟随系统** — 功能性，NavMeshAgent 跟随，2h
2. **AA_MoveAnimation** — 飞行/水生移动动画，1h
3. **OBB + MoveType + RemoveEffects + Idle** — 残余 SKIP 批量清扫，2h
4. **Fire + Eating + Immune_Dissolve** — 边缘系统守卫，2h
5. **MuzzleFlash + ShellEject** — 视觉效果，ParticleSystem，3h
6. **Dissolve + GibOnDeath** — 死亡效果，Material/布娃娃，5h
