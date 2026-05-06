# 单行审计模板 — 任何 AI 可执行

> 当你认领 `audit-checklist.md` 中的一行时，按此模板执行。

---

## 输入

从 checklist 表获取：
- `Lua 符号` — 函数/字段名
- `Lua行` — 源文件行号
- `C# 匹配` — verify-migration.mjs 给出的最佳 C# 匹配
- `Lua 源文件` — 从对应类的表头获取

---

## Step 1: 读取 Lua 源码

```bash
# 定位源文件（从类表头获取）
# 读取对应行范围的代码
```

从 GitNexus 或直接读文件获取该符号的完整 Lua 实现。

## Step 2: 读取 C# 实现

```bash
# C# 文件路径从类表头获取
# 搜索匹配的方法/属性名
```

## Step 3: 分类

| 分类 | 条件 | 下一步 |
|------|------|--------|
| **N/A** | GMod 引擎专有 API，S&box 无需等价物 | 直接标记 `[-]`，备注原因 |
| **N/A** | Lua 局部变量/别名，不影响公共 API | 直接标记 `[-]` |
| **PASS** | 4 维全绿，对照日志一致 | 标记 `[x]`，填写判定 PASS |
| **SEMI** | ⚠️ 存在但需人工验证 | 标记 `[x]`，填写 SEMI + 缺口 |
| **FAIL** | ❌ 缺失或逻辑错误 | 标记 `[x]`，填写 FAIL + 缺失项 |
| **GAP** | C# 中完全不存在对应 | 标记 `[x]`，填写 GAP |

## Step 4: 4 维检查（仅对 PASS/SEMI/FAIL）

### 结构 `[ ]` → `[x]`
- 方法签名：参数数量和类型是否对应？
- 控制流：if/for/while 结构是否一致？
- 数据字段：类型和默认值是否正确？

### 时序 `[ ]` → `[x]`  
- 调用时机：OnThink vs OnUpdate 间隔是否一致？
- 延迟调用：timer.Simple → Task.Delay 是否正确？
- 状态切换：立即生效 vs 下一帧生效是否一致？

### 副作用 `[ ]` → `[x]`
- 事件触发：OnXxx 回调是否都调用了？
- 伤害结算：DamageInfo 字段是否完整？
- 音效/特效：触发频率和条件是否一致？
- 状态清理：切状态时是否正确清理？

### 边界 `[ ]` → `[x]`
- nil/null：所有 IsValid 检查是否等价？
- 空目标：空表/null 的处理是否一致？
- 实体失效：Dead/已删除实体的守卫？
- 计时器打断：取消令牌是否正确处理？

## Step 5: 填写表格

```
| [x] | [x] | [x] | [x] | PASS | 你的名字 | 可选备注 |
```

---

## 示例

### 输入
```
| ExecuteMeleeAttack | 2449 | ExecuteMeleeAttack | [ ] | [ ] | [ ] | [ ] | | | |
```

### 执行后
```
| ExecuteMeleeAttack | 2449 | ExecuteMeleeAttack | [x] | [x] | [x] | [x] | FAIL | Claude | Gate守卫全缺, Prop交互缺, Bleed机制不同 |
```

---

## N/A 快速判定规则

直接标记 `[-]` 的情况：
- `metaEntity`, `funcGetTable`, `funcGetEnemy` 等 — Lua meta table 引用，C# 用原生反射/属性
- `CurTime`, `IsValid`, `GetConVar` 等 — Lua 标准库别名，C# 直接调用
- `defPos`, `defAng` — 临时默认值
- `string_sub`, `math_rad` 等 — Lua 标准库别名
- `VJ_MOVETYPE_*`, `VJ_STATE_*`, `ALERT_STATE_*` — 枚举值别名（如果 C# 有对应 Enum）
- `vj_npc_gib_*` — ConVar 引用，C# 用不同配置系统
