# VJBASE-S&BOX

VJ Base NPC AI framework ported to s&box C# — community-driven, open for contribution.

基于 [DrVrej's VJ Base](https://steamcommunity.com/sharedfiles/filedetails/?id=131759821)（GMod Lua NPC AI 框架），完整移植到 s&box C#，并在此基础上构建僵尸生存 / TTT 玩法。持续开发中。

> **注意**：武器模型和僵尸动画源文件不在本仓库分发。

---

## 状态

VJ Base 翻译进度约 **98%**。核心 AI 回路（感知、调度、移动、攻击、音效、动画）已完整运行。

| 子系统 | 状态 |
|--------|------|
| Schedule（32 方法） | ✅ |
| AA 移动 / 音效 / 关系系统 | ✅ |
| HumanNPC（18 方法 + SelectSchedule） | ✅ |
| DamageInfo + 免疫链 / 实体标志 / 盟友 | ✅ |
| 武器系统 + 动画系统（Route A, ~1800 行） | ✅ |
| 边缘系统（Follow / Fire / Eating / Bullseye） | ⬜ ~19 SKIP |

---

## 结构

```
├── Code/
│   ├── VJBase/                     — VJ Base C# 翻译（NPC AI 框架）
│   │   ├── Core/                   — BaseNPC、Schedule、Animation、Sound 等
│   │   ├── Engine/                 — AISenses 感知层
│   │   ├── Schedule/               — 调度/任务数据结构
│   │   └── Bases/                  — CreatureNPC / HumanNPC / TankNPC
│   ├── Zombies/                    — 僵尸 NPC 类型
│   ├── Gamemodes/                  — TTT / Zombie Horde 模式
│   ├── Weapons/                    — 自定义武器
│   ├── Player/                     — 玩家相关
│   ├── AI/                         — AI 导演 / 遭遇系统
│   ├── swb_base/ / swb_player/     — SWB 武器基础
│   └── ui/                         — UI 组件
├── Assets/                         — 游戏资源（模型、材质、音效等）
├── docs/                           — 翻译规范、进度、API 映射
└── tools/                          — 辅助脚本
```

---

## 参与贡献

欢迎提交 Issue 和 Pull Request。

- 翻译规范：[docs/translation-guide.md](docs/translation-guide.md)
- 剩余任务：[docs/phase3-progress.md](docs/phase3-progress.md)
- 动画系统：[docs/animation-system-analysis.md](docs/animation-system-analysis.md)

### 提交规范

```
type(scope): 中文简述
```

`type`: `translate` / `fill` / `fix` / `cleanup` / `field` / `docs`

### 开发环境

- [s&box](https://sbox.game) 客户端
- 用 s&box IDE 打开 `testzombie.sbproj`

---

## 协议

本项目代码仅供学习参考。VJ Base 版权归 [DrVrej](https://steamcommunity.com/sharedfiles/filedetails/?id=131759821) 所有。
