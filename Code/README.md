# VJ Base → s&box C# 翻译

将 [DrVrej's VJ Base](https://steamcommunity.com/sharedfiles/filedetails/?id=131759821)（Garry's Mod Lua NPC AI 框架）机械翻译为 s&box C# Component。

## 状态

**~98% 完成** — P0 核心回路 ✅ | P1 武器/移动 ✅ | P2 动画 ✅

| 子系统 | 状态 |
|--------|------|
| Schedule（32 方法） | ✅ |
| AA 移动（5 方法） | ✅ |
| 音效（35 分支 + SoundEvent 注册表） | ✅ |
| HumanNPC（18 方法 + SelectSchedule ~275 行） | ✅ |
| DamageInfo + 免疫链（8 类型） | ✅ |
| 实体标志 + 盟友 + 移动类型 | ✅ |
| 武器（NPC_Think 射击回路 + PrimaryAttack 9 守卫） | ✅ |
| **动画系统**（Route A, ~1800 行新代码） | ✅ |
| 残余边缘系统 SKIP | ~19 行 |
| Source 永久排除 | 45 处 |

## 结构

```
VJBase/
├── Core/
│   ├── BaseNPC.cs                  — 敌人管理、条件、属性
│   ├── BaseNPC.Schedule.cs         — 32 个 schedule 方法
│   ├── BaseNPC.AA.cs               — 飞行/水中移动
│   ├── BaseNPC.Animation.cs        — PlayAnim、TranslateActivity、Pose 参数
│   ├── BaseNPC.Relationships.cs    — 关系系统（9 功能块）
│   ├── BaseNPC.Sound.cs            — PlaySoundSystem（35 分支）
│   ├── VJAnimationEnums.cs         — 175+ ACT_* 常量
│   ├── VJAnimationMapper.cs        — 运行时序列→Activity 映射
│   ├── VJEnums.cs                  — Condition/VJState/Behavior 枚举
│   ├── VJUtility.cs                — PICK/SET/工具函数
│   ├── VJBaseWeapon.cs             — 默认武器 Component
│   ├── IVJBaseWeapon.cs            — 武器接口
│   └── VJTransitionTable.cs        — 序列过渡动画查找
├── Engine/
│   └── AISenses.cs                 — Source C++ ai_senses 翻译
├── Schedule/
│   ├── AISchedule.cs
│   └── AITask.cs
├── Bases/
│   ├── CreatureNPC.cs / .Think.cs  — 生物 NPC 基类
│   ├── HumanNPC.cs / .Think.cs     — 人类 NPC 基类
│   └── TankNPC.cs / .Think.cs      — 坦克 NPC 基类
└── Entities/
    └── VJEntityFlags.cs            — 实体标志 Component
```

## 翻译方法

- **M**：Source 引擎独有 → 从零实现
- **Sw**：s&box 有现成 API → 方法内部调 s&box
- **C**：纯 C# 逻辑 → 自己写
- **X**：s&box 不需要 → 跳过

每个 Lua 调用在 C# 有对应行——要么翻译，要么 `// SKIP: 原因`。

## 文档

| 文档 | 用途 |
|------|------|
| [translation-guide.md](docs/translation-guide.md) | 翻译架构、规则、坑点记录 |
| [phase3-progress.md](docs/phase3-progress.md) | Phase 3 剩余任务 |
| [animation-system-analysis.md](docs/animation-system-analysis.md) | 动画系统 API 对照 + 踩坑 |
| [api-mapping.md](docs/api-mapping.md) | M/Sw/C/X 方法签名映射 |
| [px-permanent-exclusions.md](docs/px-permanent-exclusions.md) | 永久排除清单 |

## 动画系统

Route A（序列直驱）：`AnimgraphDirectPlayback.Play()`，无需 Animgraph 资产。

- PlayAnim（lock / delay / OnFinish / faceEnemy）1:1 翻译
- TranslateActivity（HumanNPC 5 层战斗上下文 + 4 模型集翻译表）
- UpdatePoseParamTracking（Pose 参数实时追踪）
- SequenceToActivity（运行时反向序列查询）
- VJTransitionTable（序列过渡动画）

已知限制：Gesture 叠加层（s&box 无 `AddGesture` API），不影响 NPC 行为。

## 路径

- Lua 源码：`f:/DevProject/Sbox/VJ-Base-master/lua/`
- C# 目标：`f:/DevProject/Sbox/testzombie/Code/VJBase/`
- Source C++ 参考：`f:/DevProject/Sbox/source-sdk-2013/`
