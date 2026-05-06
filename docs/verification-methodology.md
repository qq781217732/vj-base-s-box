# Lua → C# 迁移验收方法论

> 目标：验证 ~10,000 行 C# 迁移代码是否忠实于原始 Lua 逻辑。
> 地基：[GitNexus Lua 知识图谱](#gitnexus-锚点) | 4 维等价标准 | 对照日志系统

---

## Part 1: 验收标准 — 4 维等价

### 1.1 结构等价 (Structural)

> 输入输出、状态切换、数据关系是否一致？

| 检查项 | Lua 侧 (GitNexus) | C# 侧 | 判定 |
|--------|-------------------|-------|------|
| 方法签名 | query 导出参数数量 | 逐参数比对类型/顺序 | 参数数量一致，类型合理映射 |
| 状态枚举 | `Condition` 值集合 | `HashSet<Condition>` 成员 | 一 一对 应，无遗漏 |
| 数据字段 | `[Property]` 清单 | `Field/Prop` 清单 | 字段名映射，默认值一致 |
| 类继承链 | `SWEP.Base = "xxx"` | `: VJBaseWeapon` | 继承关系一致 |
| 嵌套表结构 | `{a=1, b=2}` | 子类/子 record | 嵌套层级一致，字段完整 |

**GitNexus 锚点**:
```bash
# 导出 Lua 源文件中所有命名字段（ENT.X = ...）
gitnexus query "(file name or keyword)"  
# 对比 C# [Property] 数量
python compare_migration.py --focus
```

### 1.2 时序等价 (Temporal)

> 原来这一帧/这一拍发生的事，现在还是同一时机吗？

| 检查项 | Lua 时序 | C# 时序 | 风险 |
|--------|---------|---------|------|
| Think 间隔 | `NextProcessTime` 秒 | `OnUpdate()` 帧率 | GMod Think ≠ Unity Update |
| 延迟调用 | `timer.Simple(t, fn)` | `await Task.Delay(t)` | 需确认取消令牌 |
| 条件求值顺序 | 自上而下 | 同序 | 若重排可能影响决策 |
| 状态切换时机 | `SetState()` 立即生效 | 下一帧 vs 立即 | S&box 组件生命周期 |
| 动画事件回调 | `OnAnimEvent` 即时 | 动画事件异步 | 时序偏移 |

**对照日志锚点**:
```
[TS] Think.Start|npc=human_base_01|frame=1423
[TS] GatherConditions|npc=human_base_01|hasEnemy=true|canSee=false
[TS] SelectSchedule|npc=human_base_01|state=Combat|sched=SCHED_CHASE_ENEMY
[TS] Think.End|npc=human_base_01|frame=1423|delta=1.2ms
```

### 1.3 副作用等价 (Side-effect)

> 会不会多发一次事件、少清一次状态、多打一段伤害、少触发一次打断？

| 检查项 | 验证方式 |
|--------|----------|
| 伤害结算次数 | 一次 OnTakeDamage → 一次 ApplyDamage |
| 声音触发 | 进入状态时 PLAY ONCE（非每帧） |
| 粒子/特效 | Spawn ONCE → Destroy when done |
| 状态清理 | 切状态时 ClearCondition + CancelToken |
| 事件通知 | OnDeath 回调数 = Lua 调用点数量 |
| 敌人切换 | SetEnemy → 旧的 Clear + 新的 Notify |

**对照日志锚点**:
```
[FX] MeleeAttack.Start|npc=creature_01|target=player_03|damage=15
[FX] Damage.Applied|victim=player_03|amount=15|type=Slash
[FX] Flinch.Trigger|npc=player_03|damage=15
[FX] MeleeAttack.End|npc=creature_01|hit=true
[FX] Sound.Play|npc=creature_01|sound=melee_swing|once=true
```

### 1.4 边界等价 (Boundary)

> nil/null、空目标、实体失效、计时器打断、切状态瞬间

| 检查项 | Lua 行为 | C# 必须行为 |
|--------|---------|------------|
| 空目标 | `if !IsValid(enemy) then return end` | `if (enemy == null || !enemy.IsValid) return;` |
| 实体失效 | `IsValid(self)` 检查 | `this.IsValid` (S&box 自动 GC 但需防御) |
| 计时器打断 | 新 timer 覆盖旧引用 | CancellationToken.Cancel() + new |
| 切状态瞬态 | Schedule 中断 → 清理 | try/finally 确保清理 |
| 表为空 | `#tbl == 0` → 跳过逻辑 | `list.Count == 0` → 等价 |
| 无导航路径 | `TASK_RUN_PATH` fail → fallback | `NavAgent` 失败回调 |

**对照日志锚点**:
```
[EDGE] EnemyLost|npc=creature_01|enemy=null|state=Combat→Alert
[EDGE] TimerInterrupt|npc=creature_01|timer=ReloadSound|canceled=true
[EDGE] NavFail|npc=creature_01|dest=Vector(123,456,78)|fallback=Wander
```

---

## Part 2: 验收方式 — 3 类验证

### 2.1 对照日志 (Primary)

Lua 旧逻辑和 C# 新逻辑在**关键节点**打印统一格式日志，逐拍对比。

**日志格式规范**:
```
[TAG] EventName|key1=val1|key2=val2|...
```

**必需日志点** (每个 NPC 至少埋 8 类):

| 类 | 日志点 | TAG |
|----|--------|-----|
| 状态切换 | Initialize, OnRemove, SetState, ScheduleStart/End | `[STATE]` |
| 目标选择 | GatherConditions, SetEnemy, EnemyLost, TargetFound | `[TARGET]` |
| 攻击开始/结束 | MeleeAttack, RangeAttack, LeapAttack, GrenadeAttack | `[ATK]` |
| 伤害结算 | OnDamage, ApplyDamage, Flinch, Death | `[DMG]` |
| timer / 延迟触发 | TimerStart, TimerFire, TimerCancel | `[TIMER]` |
| 移动/寻路 | MoveStart, MoveEnd, NavFail, FaceTarget | `[MOVE]` |
| 音效/特效 | SoundPlay, ParticleSpawn, EffectTrigger | `[FX]` |
| 边界 ± 异常 | NavFail, EnemyInvalid, TimerInterrupt, EmptyTarget | `[EDGE]` |

### 2.2 符号巡检 (GitNexus 驱动)

```bash
# Step 1: 从 Lua 源导出符号清单
gitnexus query "METHOD_NAME_KEYWORDS" --json > lua-symbols.json

# Step 2: 从 C# 代码提取方法签名
grep -rE '(void|bool|int|float|Task|override|virtual) \w+\(' Code/VJBase/ > cs-methods.txt

# Step 3: 逐方法比对
python compare_migration.py --methods

# Step 4: 生成缺失报告
python compare_migration.py --json > verification-gaps.json
```

### 2.3 结构对比 (AST 骨架)

对每个 `function ENT:XXX(args)`:
1. 提取 Lua AST 控制流骨架（if/for/while/call 序列）
2. 提取 C# 控制流骨架（同方法）
3. 逐节点比对
4. 标记差异

```
Lua: OnPrimaryAttack
  ├─ if status=="Init"
  │   ├─ if CLIENT return
  │   ├─ local projectile = ents.Create(...)
  │   ├─ projectile:SetPos() → Activate() → Spawn()
  │   ├─ local phys = projectile:GetPhysicsObject()
  │   └─ if owner.IsVJBaseSNPC
  │       ├─ phys:SetVelocity(VJ.CalculateTrajectory(...))
  │       └─ phys:SetVelocity(VJ.CalculateTrajectory(...))
  └─ end

C#: OnPrimaryAttack
  ├─ if (status != "Init") return         ✅ matches
  ├─ if (Game.IsClient) return            ✅ matches
  ├─ var bolt = new CrossbowBoltEntity()   ✅ mapped
  ├─ bolt.Position → bolt.Spawn()         ✅ mapped
  ├─ var phys = bolt.PhysicsBody           ✅ mapped
  └─ if (owner is VJBaseSNPC)             ✅ matches
      ├─ phys.Velocity = VJPhysics.CalculateTrajectory(...)  ⚠️ need verify
      └─ phys.Velocity = VJPhysics.CalculateTrajectory(...)   ⚠️ need verify
```

---

## Part 3: GitNexus 锚点

GitNexus 知识图谱是整个验收流程的**唯一 Lua 真值源**。

### 3.1 为什么用 GitNexus 而非直接读 Lua 文件

| 方式 | 问题 |
|------|------|
| 直接读 .lua | 96 文件 × 平均 250 行 = 手动翻找，容易漏 |
| Grep 搜索 | C 风格注释污染，多文件同名函数混乱 |
| GitNexus 图谱 | 符号索引 + 行号 + 跨文件关系，一次查询全集 |

### 3.2 验证流程中的 GitNexus 用法

```bash
# 1. 查某个方法在所有文件中的定义
gitnexus query "OnPrimaryAttack"          # → 5 个 override 点

# 2. 查文件的依赖链
gitnexus cypher "MATCH (a:File)-[r:IMPORTS]->(b:File)
  WHERE a.name = 'init.lua' RETURN b.name"

# 3. 查某个文件的所有符号
gitnexus cypher "MATCH (n) WHERE n.filePath CONTAINS 'creature_base'
  AND n.startLine IS NOT NULL RETURN n.name, n.startLine ORDER BY n.startLine"

# 4. 变更检测 — Lua 代码改了哪些
gitnexus detect_changes --scope all          # 检测所有变更

# 5. 影响分析 — 改了 base 会影响谁
gitnexus impact "SetDefaultValues" --direction upstream
```

### 3.3 验收时的标准查询序列

对一个待验证的 C# 文件，执行 5 步查询：

```
Step 1: gitnexus query "<类名关键词>"          → 定位对应的 Lua 源文件
Step 2: gitnexus cypher "MATCH (n) WHERE n.filePath = 'XXX.lua'
           AND n.startLine IS NOT NULL RETURN ..." → 导出该文件的全部符号
Step 3: gitnexus cypher "MATCH (a:File)-[r:IMPORTS]->(b)
           WHERE a.name = 'XXX.lua' RETURN b"     → 该文件的依赖
Step 4: gitnexus query "<关键方法名>"            → 查找所有 override 点
Step 5: 对比 Step 2 的符号清单 vs C# 方法清单   → 生成 gap
```

---

## Part 4: 逐文件审计模板

### 4.1 审计表头

```markdown
## [审计] Lua 文件: `lua/XXX.lua` → C# 文件: `Code/VJBase/XXX.cs`
| 审计日期 | 审计人 | 版本 | 总符号数 | PASS | FAIL | GAP |
|----------|--------|------|----------|------|------|-----|
| 2026-05-05 | —— | cb1e20b | N | — | — | — |
```

### 4.2 逐符号审计

```markdown
| # | Lua 符号 | Lua 行 | C# 符号 | 结构 | 时序 | 副作用 | 边界 | 判定 | 备注 |
|---|----------|--------|---------|------|------|--------|------|------|------|
| 1 | ENT:Initialize | core:93 | OnStart() | ✅ | ✅ | ✅ | ✅ | PASS | 生命周期等效 |
| 2 | ENT:RunAI | core:162 | OnUpdate() | ✅ | ⚠️ | ✅ | ✅ | SEMI | Think 间隔 0.1s → 帧率依赖 |
| 3 | ENT:SetEnemy | core:312 | Enemy setter | ✅ | ✅ | ❌ | ✅ | FAIL | 缺少旧敌人 ClearEnemyMemory |
| 4 | ENT:CanSee | core:141 | CanSee() | ✅ | ✅ | ✅ | ⚠️ | SEMI | nil 实体检查缺失 |
```

### 4.3 最终判定规则

| 判定 | 条件 |
|------|------|
| **PASS** | 4 维全绿 ✅，对照日志对拍一致 |
| **SEMI** | ⚠️ 存在，但不影响核心逻辑（如 API 映射方式差异） |
| **FAIL** | ❌ 存在，会导致行为不一致（漏逻辑/错逻辑） |
| **GAP** | Lua 有此符号，C# 中完全缺失 |

---

## Part 5: 对照日志系统实现

### 5.1 统一日志接口

```csharp
// C# side — 与 Lua 侧使用相同 TAG|EventName|key=value 格式
public static class MigrationLog
{
    [ConVar("migration_log")] public static bool Enabled { get; set; } = false;

    public static void State(string npc, string eventName, string detail = "")
        => Log("STATE", $"{eventName}|npc={npc}|{detail}");

    public static void Target(string npc, string eventName, string detail = "")
        => Log("TARGET", $"{eventName}|npc={npc}|{detail}");

    public static void Attack(string npc, string eventName, string detail = "")
        => Log("ATK", $"{eventName}|npc={npc}|{detail}");

    public static void Damage(string victim, string eventName, string detail = "")
        => Log("DMG", $"{eventName}|victim={victim}|{detail}");

    public static void Timer(string npc, string eventName, string detail = "")
        => Log("TIMER", $"{eventName}|npc={npc}|{detail}");

    public static void Edge(string npc, string eventName, string detail = "")
        => Log("EDGE", $"{eventName}|npc={npc}|{detail}");

    private static void Log(string tag, string msg)
    {
        if (!Enabled) return;
        Log.Info($"[{tag}] {msg}");
    }
}
```

### 5.2 Lua 侧对应日志

```lua
-- 在关键节点插入同样格式
if VJ_MIGRATION_LOG then
    print(string.format("[STATE] Think.Start|npc=%s|frame=%d", self:GetName(), CurTime()))
end
```

### 5.3 对拍流程

```
1. 同时运行 Lua (GMod) 和 C# (S&box) 相同场景
2. 开启 migration_log 1
3. 执行相同操作序列（生成同一个 NPC，让它进入战斗）
4. 收集 Lua 日志 → lua_trace.txt
5. 收集 C# 日志 → cs_trace.txt
6. 逐 TAG 逐 EventName 对比：事件顺序是否一致？key=value 是否一致？
```

---

## Part 6: 审计优先级

基于 GitNexus 依赖图，按影响范围排序：

| 优先级 | 模块 | 依赖者数量 | C# 行数 | 原因 |
|--------|------|-----------|---------|------|
| P0 | Core: BaseNPC.cs | 全部 NPC | ~800 | 所有 NPC 的基类，错一处全错 |
| P0 | Senses | BaseNPC | ~300 | 感知系统影响所有 AI 决策 |
| P1 | Combat: CreatureNPC | 3 子类 | ~850 | 战斗逻辑，伤害结算 |
| P1 | Schedule | BaseNPC | ~200 | 行为调度核心 |
| P2 | Bases: HumanNPC | 1 | ~750 | 单个子类 |
| P2 | Bases: TankNPC | 2 | ~300 | 单个子类 |
| P3 | Weapons: BaseWeapon | 17 | ~500 | 17 武器模板 |
| P4 | 其他 (Utils, FX, etc.) | 分散 | ~500 | 工具类，独立性强 |
