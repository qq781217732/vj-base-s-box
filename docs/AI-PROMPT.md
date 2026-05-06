# AI 迁移审计 — 启动提示词

> 复制此提示词给任意 AI（Claude Code / Cursor / Codex 等）

---

## 完整版 (推荐)

```
你是 VJ-Base Lua → S&box C# 迁移审计助手。

# 你的任务
对照 Lua 源码（地面真相）逐文件审计现有 C# 迁移代码，
找出逻辑差异并标记状态。

# 工作目录
F:\DevProject\Sbox\testzombie\
Lua 源码: F:\DevProject\Sbox\VJ-Base-master\lua\
C# 代码: F:\DevProject\Sbox\testzombie\Code\VJBase\

# 入口文件
打开 docs/migration-master-checklist.md
这是主清单，85 个 Lua 文件按优先级排列。
每行: Lua路径 | C#目标 | 符号数 | 状态 | 备注

状态: ⬜未开始 🔵进行中 ✅已通过 ⚠️有问题 ❌受阻

# 工作流程
1. 从清单选一个 ⬜ 文件 (按 P0→P4 顺序)
2. 将状态改为 🔵
3. 读 docs/audit-template.md 了解审计标准
4. 用 GitNexus 导出该文件的符号:
   gitnexus cypher "MATCH (n) WHERE n.filePath = 'lua/xxx.lua' AND n.startLine IS NOT NULL RETURN n.name, n.startLine, labels(n) AS kind ORDER BY n.startLine"
5. 打开对应的 Lua 源文件和 C# 目标文件
6. 逐方法做 4 维等价检查 (结构/时序/副作用/边界)
7. 将结果写入 docs/audit-checklist.md 对应行
8. 更新主清单状态
9. 保存文件，继续下一个

# GitNexus 工具位置
F:\DevProject\Sbox\GitNexus-1.6.4-rc.64\gitnexus\dist\cli\index.js

# 参考文档
docs/audit-template.md - 审计标准
docs/verification-methodology.md - 4 维等价详解
docs/api-mapping.md - GMod→S&box API 映射
docs/audit-creaturenpc-melee.md - 审计示例 (ExecuteMeleeAttack)

# 重要
- Lua 是地面真相，不能改
- C# 是审计目标，标记问题不改代码
- 每个文件审计完必须更新状态
- 遇到不确定的标记 ⚠️ 并备注原因
```

---

## 精简版 (单个文件)

```
审计 Lua 文件: lua/vj_base/ai/core.lua
C# 目标: Code/VJBase/Core/BaseNPC.cs

流程:
1. 读 docs/audit-template.md
2. 从 GitNexus 导出 lua/vj_base/ai/core.lua 的符号清单
3. 打开 Lua 源 + C# 目标
4. 逐方法检查 4 维等价
5. 更新 docs/migration-master-checklist.md 该行状态
6. 更新 docs/audit-checklist.md 对应行
```

---

## 续跑版 (断点继续)

```
继续审计工作。先读 docs/migration-master-checklist.md
查看当前进度，从第一个 ⬜ 或 ⚠️ 的文件开始。
```

---

## 进度查看

打开 [migration-master-checklist.md](migration-master-checklist.md)：

```
## 统计面板  (目前没有，可以加在文件顶部)

快速看:
- 搜索 ⬜ → 未开始的数量
- 搜索 🔵 → 进行中的
- 搜索 ✅ → 已完成的
- 搜索 ⚠️ → 有问题的
- 搜索 ❌ → 受阻的

P0 全部 ✅ = 核心 AI 审计完成
P0+P1 全部 ✅ = 实体系统审计完成
```
