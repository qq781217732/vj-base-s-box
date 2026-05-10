# NPC Zombie Horde

Left 4 Dead 风格 s&box 游戏模式，包含 TTT 模式资源。

本项目基于 [DrVrej's VJ Base](https://steamcommunity.com/sharedfiles/filedetails/?id=131759821)（Garry's Mod Lua NPC AI 框架），将其完整移植到 s&box C#，并在此基础上构建僵尸生存 / TTT 玩法。

> **注意**：武器模型和僵尸动画源文件不在本仓库分发。

---

## 状态

VJ Base 翻译进度约 **98%**。核心 AI 回路（感知、调度、移动、攻击、音效、动画）已完整运行。

| 子系统 | 状态 |
|--------|------|
| Schedule（32 方法） | ✅ |
| AA 移动（5 方法） | ✅ |
| 音效（35 分支 + SoundEvent 注册表） | ✅ |
| HumanNPC（18 方法 + SelectSchedule） | ✅ |
| DamageInfo + 免疫链（8 类型） | ✅ |
| 实体标志 + 盟友 + 移动类型 | ✅ |
| 武器（NPC_Think 射击回路 + PrimaryAttack） | ✅ |
| 动画系统（Route A, ~1800 行） | ✅ |
| 边缘系统（Follow / Fire / Eating / Bullseye） | ⬜ ~19 SKIP |
| Source 引擎永久排除 | 45 处 |

---

## 结构

```
Code/
├── VJBase/                        — VJ Base C# 翻译（NPC AI 框架）
│   ├── Core/
│   │   ├── BaseNPC.cs             — 敌人管理、条件、属性
│   │   ├── BaseNPC.Schedule.cs    — 32 个 schedule 方法
│   │   ├── BaseNPC.AA.cs          — 飞行/水中移动
│   │   ├── BaseNPC.Animation.cs   — PlayAnim / TranslateActivity / Pose 参数
│   │   ├── BaseNPC.Relationships.cs — 关系系统（9 功能块）
│   │   ├── BaseNPC.Sound.cs       — PlaySoundSystem（35 分支）
│   │   ├── VJAnimationEnums.cs    — 175+ ACT_* 常量
│   │   ├── VJAnimationMapper.cs   — 运行时序列→Activity 映射
│   │   ├── VJBaseWeapon.cs        — 默认武器 Component
│   │   └── ...
│   ├── Engine/
│   │   └── AISenses.cs            — Source C++ ai_senses 翻译
│   ├── Schedule/
│   ├── Bases/                     — CreatureNPC / HumanNPC / TankNPC
│   └── Entities/
├── Zombies/                       — 僵尸 NPC 类型
├── Gamemodes/                     — TTT / Zombie Horde 模式
├── Weapons/                       — 自定义武器
├── Player/                        — 玩家相关
├── AI/                            — AI 导演 / 遭遇系统
├── swb_base/ / swb_player/ / ...  — SWB 武器基础集成
├── ui/                            — UI 组件
└── docs/                          — 翻译规范、进度、API 映射
```

---

## 参与贡献

欢迎提交 Issue 和 Pull Request。

- 翻译规范见 [docs/translation-guide.md](docs/translation-guide.md)
- 剩余任务见 [docs/phase3-progress.md](docs/phase3-progress.md)
- 动画系统分析见 [docs/animation-system-analysis.md](docs/animation-system-analysis.md)

### 提交规范

```
type(scope): 中文简述
```

| type | 含义 |
|------|------|
| `translate` | 机械翻译 Lua→C# |
| `fill` | Phase 3 填坑 |
| `fix` | 修 bug |
| `cleanup` | 删死代码、重构 |
| `field` | 补字段、加配置 |
| `docs` | 文档更新 |

---

## 协议

本项目代码仅供学习参考。VJ Base 版权归 [DrVrej](https://steamcommunity.com/sharedfiles/filedetails/?id=131759821) 所有。武器模型和动画资源不包含在本仓库中。

---

## 相关链接

- [VJ Base (Steam Workshop)](https://steamcommunity.com/sharedfiles/filedetails/?id=131759821)
- [s&box](https://sbox.game)
