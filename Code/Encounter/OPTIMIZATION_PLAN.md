# 遭遇战系统优化方案 (定稿)

## 总体架构决策

| 决策 | 结论 | 理由 |
|------|------|------|
| `SpawnSource` 实体 | **保留, 改为标签驱动** | 开放地图可纯几何; 固定遭遇战可精确摆放 |
| `SpawnSourceType` 枚举 | **移除** | 几何模式 + 标签全覆盖, 开放地图无法判断语义 |
| 刷兵位置 | **标签优先, 几何回退** | WaveDefinition 优先按 tag 找源, 无匹配走 `SpawnPositionMode` |
| 波次系统 | **`WaveDefinition` 列表** | 替代单一 `NextWaveGroup`, 每波独立配置 |
| 门控标记 | **全局 `Dictionary<string,bool>`** | 遭遇战间链式依赖 |
| ActionGraph 脚本化 | **远期, 非必须** | 增强波次 + 门控已覆盖绝大多数需求 |

---

## 一、刷兵位置系统

### 1.1 EncounterSpawnSource: 标签化

```csharp
[Group("ZombieHorde")]
[Title("Encounter Spawn Source")]
public partial class EncounterSpawnSource : Component
{
    // ★ 核心: 逗号分隔标签, 如 "boss,rooftop,wave2"
    [Property, Title("Tags")]
    [Description("逗号分隔, 如: boss, rooftop, wave2。空=共享池, 程序化系统使用")]
    public string Tags { get; set; } = "";

    [Property, Title("Min Player Distance")]
    public float MinPlayerDistance { get; set; } = 400f;

    [Property, Title("Max Player Distance")]
    public float MaxPlayerDistance { get; set; } = 6000f;

    [Property, Title("Require Out Of Sight")]
    public bool RequireOutOfSight { get; set; } = true;

    public SpawnSource ToSpawnSource()
    {
        var source = new SpawnSource
        {
            Position = WorldPosition,
            Tags = string.IsNullOrWhiteSpace(Tags)
                ? new HashSet<string>()
                : Tags.Split(',').Select(t => t.Trim().ToLower()).ToHashSet()
        };
        var rule = new SpawnRule
        {
            MinPlayerDistance = MinPlayerDistance,
            MaxPlayerDistance = MaxPlayerDistance,
            RequireOutOfSight = RequireOutOfSight
        };
        source.Rules.Add(rule);
        return source;
    }
}
```

### 1.2 SpawnSource: 标签查询

```csharp
public class SpawnSource
{
    public Vector3 Position { get; set; }
    public HashSet<string> Tags { get; set; } = new();       // ★ 新增

    // ★ 标签查询
    public bool HasTag(string tag) => Tags.Contains(tag);
    public bool HasAllTags(IEnumerable<string> tags) => tags.All(t => Tags.Contains(t));
    public bool HasAnyTag(IEnumerable<string> tags) => tags.Any(t => Tags.Contains(t));
    public bool IsShared => Tags.Count == 0;                  // 空标签 = 共享池

    // 原有不变
    public List<SpawnRule> Rules { get; set; } = new();
    public bool IsAvailable(EncounterType forType, float zoneAlertLevel)
        => Rules.All(r => r.Evaluate(Position, forType, null, zoneAlertLevel));
}
```

### 1.3 SpawnPositionMode: 几何回退

当 `WaveDefinition` 没有指定标签、或标签匹配不到任何源时，走几何算法。

```csharp
public enum SpawnPositionMode
{
    ZoneEdge,           // Zone 边界, 离玩家最近侧 — "从外面涌进来"
    BehindPlayer,       // 玩家视线反方向 — "转角遇到怪"
    AroundPlayer,       // 玩家周围环形 — "被包围"
    AroundObjective,    // 触发点/目标点为中心扇形 — "从仓库后门涌出"
    InterceptRoute,     // 玩家与触发点连线的中垂线 — "挡住去路"
    ZoneRandom,         // Zone 内随机 NavMesh — 巡逻兵散布
}

/// <summary>标签源匹配多个时如何选取</summary>
public enum TagPickStrategy
{
    Random,              // 随机选
    NearestToPlayer,     // 离最近玩家
    FarthestFromPlayer,  // 离玩家最远
}
```

### 1.4 WaveDefinition 位置参数

```csharp
public class WaveDefinition
{
    // ... 敌人配置等

    // --- 出生位置 ---
    public SpawnPositionMode PositionMode { get; set; } = SpawnPositionMode.ZoneEdge;
    public float PositionRadius { get; set; } = 400f;        // 环形半径 / 边界外扩距离
    public float PositionArcDegrees { get; set; } = 120f;    // 扇形张角 (0=全向)
    public Vector3? PositionDirection { get; set; }           // 扇形朝向, null=自动推导
    public int PositionCount { get; set; } = 1;              // 取几个位置 (环形包围用)
}
```

### 1.5 算法详情

#### ZoneEdge — Zone 边缘涌入

```
输入: zone, player, radius (外扩距离)
输出: 单个 Vector3

1. 取玩家位置, 找到 Zone 边界上离玩家最近的点
   closestEdge = zone.Bounds.ClosestPoint(player.WorldPosition)
   如果玩家在 zone 内, 此点就是玩家投影到最近边界面上

2. 从玩家位置指向 closestEdge 的方向, 继续外扩 radius 单位
   direction = (closestEdge - player.WorldPosition).Normal
   target = closestEdge + direction * radius

3. 在 target 周围取 NavMesh 最近点
   pos = NavMesh.GetClosestPoint(target)

4. 检查: pos 必须在 zone.Bounds 外 (或边缘上)
   如果 pos 在 zone 内太深 → 沿方向再外推

5. 返回 pos
```

效果: 敌人总是从离玩家最近的边界外侧生成，看起来像从门外/窗外涌进来。

#### BehindPlayer — 玩家背后盲区

```
输入: player, radius (距离), arcDegrees (扇形张角)
输出: 单个 Vector3

1. 取玩家视线反方向
   behind = -player.EyeRotation.Forward
   忽略俯仰, 只取水平分量

2. 在 behind 方向 ± arcDegrees/2 扇形内随机一个方向
   randomAngle = Random.Float(-arcDegrees/2, arcDegrees/2)
   randomDir = RotateHorizontal(behind, randomAngle)

3. 距离 = Random.Float(radius * 0.7f, radius * 1.3f)
   target = player.WorldPosition + randomDir * distance

4. target 处取 NavMesh 最近点
   pos = NavMesh.GetClosestPoint(target)

5. ★ 视线检查: 从 pos 向玩家眼睛 Trace
   如果可见 → 此点不好, 重试 (最多 4 次, 逐步增加距离)

6. 返回 pos (找不到则回退 ZoneRandom)
```

效果: 敌人在玩家背后看不到的地方生成，玩家一转身就发现"刚刚还没有的"。

#### AroundPlayer — 玩家周围环形

```
输入: player, radius (环形半径), count (取点数)
输出: List<Vector3> (count 个)

1. 等分 360°: angleStep = 360f / count
   随机起始偏移: startAngle = Random.Float(0, angleStep)  (避免总是正北)

2. 对 i = 0..count-1:
   a. angle = startAngle + angleStep * i
   b. direction = Vector3.FromYaw(angle)
   c. target = player.WorldPosition + direction * radius
   d. pos = NavMesh.GetClosestPoint(target)
   e. 检查与已有选中点的间距 ≥ MinSeparation
   f. 检查是否在 zone.Bounds 内
   g. 存入列表

3. 任何失败的点 → 微调角度 (±15°) 重试 3 次

4. 返回有效位置列表
```

效果: 敌人从四面八方同时出现，玩家被包围。

#### AroundObjective — 触发点扇形

```
输入: objectivePoint, direction (扇形朝向), arcDegrees, radius
输出: 单个 Vector3

1. 如果没有指定 direction:
   自动推导 = (player.WorldPosition - objectivePoint).Normal (玩家相对目标的方向)
   → 扇形背对玩家, 敌人从目标的"后方"涌出

2. 在 direction ± arcDegrees/2 扇形内随机一个方向
   randomAngle = Random.Float(-arcDegrees/2, arcDegrees/2)
   spawnDir = RotateHorizontal(direction, randomAngle)

3. 距离 = Random.Float(radius * 0.6f, radius)
   target = objectivePoint + spawnDir * distance

4. NavMesh.GetClosestPoint(target)

5. 视线检查 (可选): 如果 RequireOutOfSight, 确保 pos 到玩家不可见

6. 返回 pos
```

效果: 以目标点为参照，"从仓库后门那个方向涌出来"。

#### InterceptRoute — 截击玩家路线

```
输入: player, objectivePoint, radius
输出: 单个 Vector3

1. 计算玩家到目标的连线
   routeDir = (objectivePoint - player.WorldPosition).Normal
   midPoint = (player.WorldPosition + objectivePoint) * 0.5f

2. 在中垂线上取点
   perpendicular = RotateHorizontal(routeDir, ±90°)
   offset = Random.Float(radius * 0.5f, radius)
   side = Random.Sign
   target = midPoint + perpendicular * offset * side

3. NavMesh.GetClosestPoint(target)

4. 额外约束: pos 离玩家 ≥ 200, 离触发点 ≥ 100

5. 返回 pos
```

效果: 敌人出现在玩家去目标的半路上，挡住去路。

#### ZoneRandom — Zone 内随机散布

```
输入: zone, count, existingPositions
输出: List<Vector3> (count 个)
```

即现有的 `SpawnGroup.FindSpreadPosition()` — NavMesh 随机取点 + 互相间距 ≥ `MinSeparation`。兼容旧逻辑，用于巡逻兵散布。

### 1.6 位置选择流程

```
SelectSpawnPositions(zone, waveDef, player, objectivePoint):
    │
    ├─ waveDef.RequiredSourceTags 非空?
    │   收集 zone 中满足 ALL tags 的 SpawnSource
    │   ├─ 找到了 → 选 1 个 (策略: Random / NearestToPlayer)
    │   │           → 对每个源位置调用 FindSpreadPosition() 散布个体
    │   │           → 返回
    │   └─ 没找到 → Log.Warning → 继续往下
    │
    ├─ waveDef.PreferredSourceTags 非空?
    │   收集 zone 中满足 ANY tag 的源
    │   ├─ 找到了 → 选 1 个 → 同上散布
    │   └─ 没找到 → 继续往下
    │
    └─ 几何回退: waveDef.PositionMode
        ┌──────────────────────────────────────────────┐
        │ ZoneEdge         → 1 点, 用于波次涌入       │
        │ BehindPlayer     → 1 点, 用于伏击           │
        │ AroundPlayer     → N 点, 用于包围           │
        │ AroundObjective  → 1 点, 用于目标防御       │
        │ InterceptRoute   → 1 点, 用于截击           │
        │ ZoneRandom       → N 点, 用于巡逻散布       │
        └──────────────────────────────────────────────┘
        每个点再调用 FindSpreadPosition(spawnCenter, existingPositions)
        散布组内个体
```

### 1.7 方向波次轮转 (Wave-Level Strategy)

这不是单次位置算法，而是波次间的位置变化策略。通过 `WaveDefinition` 逐波配置不同 `PositionDirection` 实现：

```csharp
// 示例: 3 波从不同象限来
Waves = new List<WaveDefinition>
{
    new() { Group = ..., PositionMode = SpawnPositionMode.ZoneEdge,
            PositionDirection = new Vector3(0, 1, 0) },    // 北
    new() { Group = ..., PositionMode = SpawnPositionMode.ZoneEdge,
            PositionDirection = new Vector3(1, 0, 0) },    // 东
    new() { Group = ..., PositionMode = SpawnPositionMode.ZoneEdge,
            PositionDirection = new Vector3(0, -1, 0) },   // 南
};
```

也可以自动轮转:

```csharp
// 工具方法: 按波次索引自动计算象限方向
static Vector3 GetQuadrantDirection(int waveIndex, int totalWaves)
{
    float angle = (360f / totalWaves) * waveIndex;
    return Vector3.FromYaw(angle);
}
```

### 1.8 程序化系统与标签源的关系

```
RefreshAmbient / RefreshReinforcements:
    → 遍历 zone.SpawnSources
    → 只看 IsShared == true 的源 (空标签)
    → 有标签的源一律跳过, 留给脚本化遭遇战专用
```

### 1.9 SpawnPositionSelector 实现

```csharp
namespace ZombieHorde;

public static class SpawnPositionSelector
{
    /// <summary>
    /// 为主流程: 按 WaveDefinition 选择生成中心点, 再散布个体位置。
    /// </summary>
    public static List<Vector3> Select(
        EncounterZone zone, WaveDefinition waveDef,
        HumanPlayer player, Vector3? objectivePoint,
        int enemyCount )
    {
        // 1) 标签匹配
        var center = TryMatchTags(zone, waveDef);
        if (center.HasValue)
            return SpreadAround(zone, center.Value, enemyCount);

        // 2) 几何回退: 计算中心点
        var centers = SelectCenters(zone, waveDef, player, objectivePoint);
        if (centers.Count == 0)
            centers.Add(zone.Bounds.Center);

        // 3) 在每个中心点周围散布
        var results = new List<Vector3>();
        int perCenter = Math.Max(1, enemyCount / centers.Count);
        foreach (var c in centers)
            results.AddRange(SpreadAround(zone, c, perCenter));

        return results;
    }

    // ---- 标签匹配 ----

    static Vector3? TryMatchTags(EncounterZone zone, WaveDefinition waveDef)
    {
        // Required: 必须全匹配
        if (waveDef.RequiredSourceTags.Count > 0)
        {
            var matches = zone.SpawnSources
                .Where(s => s.HasAllTags(waveDef.RequiredSourceTags))
                .ToList();
            if (matches.Count > 0)
                return PickSource(matches, waveDef.TagPickStrategy).Position;
            Log.Warning($"[SpawnPos] RequiredSourceTags not matched: {string.Join(",", waveDef.RequiredSourceTags)}");
        }

        // Preferred: 优先匹配
        if (waveDef.PreferredSourceTags.Count > 0)
        {
            var matches = zone.SpawnSources
                .Where(s => s.HasAnyTag(waveDef.PreferredSourceTags))
                .ToList();
            if (matches.Count > 0)
                return PickSource(matches, waveDef.TagPickStrategy).Position;
        }

        return null; // 回退到几何
    }

    static SpawnSource PickSource(List<SpawnSource> sources, TagPickStrategy strategy)
    {
        return strategy switch
        {
            TagPickStrategy.NearestToPlayer => sources.OrderBy(s =>
                (s.Position - GetNearestPlayer(s.Position).WorldPosition).Length).First(),
            TagPickStrategy.FarthestFromPlayer => sources.OrderByDescending(s =>
                (s.Position - GetNearestPlayer(s.Position).WorldPosition).Length).First(),
            _ => sources[Random.Shared.Next(sources.Count)]  // Random (default)
        };
    }

    // ---- 几何中心点 ----

    static List<Vector3> SelectCenters(
        EncounterZone zone, WaveDefinition waveDef,
        HumanPlayer player, Vector3? objectivePoint )
    {
        return waveDef.PositionMode switch
        {
            SpawnPositionMode.ZoneEdge        => new() { SelectZoneEdge(zone, player, waveDef.PositionRadius) },
            SpawnPositionMode.BehindPlayer    => new() { SelectBehindPlayer(player, waveDef.PositionRadius, waveDef.PositionArcDegrees) },
            SpawnPositionMode.AroundPlayer    => SelectAroundPlayer(zone, player, waveDef.PositionRadius, waveDef.PositionCount),
            SpawnPositionMode.AroundObjective => new() { SelectAroundObjective(objectivePoint ?? player.WorldPosition, player, waveDef) },
            SpawnPositionMode.InterceptRoute  => new() { SelectInterceptRoute(player, objectivePoint ?? player.WorldPosition, waveDef.PositionRadius) },
            SpawnPositionMode.ZoneRandom      => new() { SelectZoneRandom(zone) },
            _ => new() { zone.Bounds.Center }
        };
    }

    // ---- 具体算法 ----

    static Vector3 SelectZoneEdge(EncounterZone zone, HumanPlayer player, float pushOut)
    {
        var playerPos = player.WorldPosition;
        var closest = zone.Bounds.ClosestPoint(playerPos);
        var dir = (closest - playerPos).Normal;
        if (dir.Length < 0.01f) dir = Vector3.Forward; // 玩家恰在边界上

        var target = closest + dir * pushOut;
        var navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(target);

        // 确保在 zone 外
        if (navPos.HasValue && !zone.IsPointInBounds(navPos.Value))
            return navPos.Value;

        // 回退: 继续外推
        target += dir * pushOut * 2f;
        navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(target);
        return navPos ?? target;
    }

    static Vector3 SelectBehindPlayer(HumanPlayer player, float radius, float arcDegrees)
    {
        var behind = -player.EyeRotation.Forward;
        behind = new Vector3(behind.x, behind.y, 0).Normal; // 水平面投影

        for (int attempt = 0; attempt < 4; attempt++)
        {
            var angleOffset = Random.Shared.NextFloat(-arcDegrees / 2, arcDegrees / 2);
            var dir = RotateHorizontal(behind, angleOffset);
            var dist = Random.Shared.NextFloat(radius * 0.7f, radius * 1.3f);
            var target = player.WorldPosition + dir * dist;

            var navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(target);
            if (!navPos.HasValue) continue;

            // 视线检查: 生成点到玩家不可见
            var eyePos = player.WorldPosition + Vector3.Up * 64f;
            var spawnEye = navPos.Value + Vector3.Up * 70f;
            var tr = Game.ActiveScene.Trace.Ray(spawnEye, eyePos)
                .WithoutTags("trigger", "gib").Run();
            if (tr.Fraction < 0.95f)
                return navPos.Value; // 有遮挡, 好位置
        }

        // 最终回退: 无视视线, 只要背后
        var finalDir = RotateHorizontal(behind, Random.Shared.NextFloat(-arcDegrees / 2, arcDegrees / 2));
        var finalTarget = player.WorldPosition + finalDir * radius;
        return Game.ActiveScene.NavMesh?.GetClosestPoint(finalTarget) ?? finalTarget;
    }

    static List<Vector3> SelectAroundPlayer(EncounterZone zone, HumanPlayer player, float radius, int count)
    {
        var results = new List<Vector3>();
        if (count <= 0) return results;

        float angleStep = 360f / count;
        float startAngle = Random.Shared.NextFloat(0, angleStep);

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;
            var dir = Vector3.FromYaw(angle);
            var target = player.WorldPosition + dir * radius;

            Vector3? pos = null;
            for (int retry = 0; retry < 3; retry++)
            {
                var adjusted = target + dir * (retry * 50f);
                var navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(adjusted);
                if (!navPos.HasValue) continue;
                if (!zone.IsPointInBounds(navPos.Value)) continue;

                // 间距检查
                if (results.Any(p => (p - navPos.Value).Length < SpawnGroup.MinSeparation))
                    continue;

                pos = navPos.Value;
                break;
            }

            if (pos.HasValue)
                results.Add(pos.Value);
        }

        return results;
    }

    static Vector3 SelectAroundObjective(Vector3 objectivePoint, HumanPlayer player, WaveDefinition waveDef)
    {
        var dir = waveDef.PositionDirection?.Normal
            ?? (player.WorldPosition - objectivePoint).Normal; // 默认: 背对玩家方向

        if (dir.Length < 0.01f) dir = Vector3.Forward;

        float halfArc = waveDef.PositionArcDegrees / 2f;
        float arc = waveDef.PositionArcDegrees;

        for (int attempt = 0; attempt < 4; attempt++)
        {
            var angleOffset = arc > 0
                ? Random.Shared.NextFloat(-halfArc, halfArc)
                : Random.Shared.NextFloat(-180f, 180f);
            var spawnDir = RotateHorizontal(dir, angleOffset);
            var dist = Random.Shared.NextFloat(waveDef.PositionRadius * 0.6f, waveDef.PositionRadius);
            var target = objectivePoint + spawnDir * dist;

            var navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(target);
            if (!navPos.HasValue) continue;

            // 可选视线检查
            if (waveDef.RequireOutOfSight)
            {
                var eyePos = player.WorldPosition + Vector3.Up * 64f;
                var spawnEye = navPos.Value + Vector3.Up * 70f;
                var tr = Game.ActiveScene.Trace.Ray(spawnEye, eyePos)
                    .WithoutTags("trigger", "gib").Run();
                if (tr.Fraction >= 0.95f) continue;
            }

            return navPos.Value;
        }

        var fallback = objectivePoint + dir * waveDef.PositionRadius;
        return Game.ActiveScene.NavMesh?.GetClosestPoint(fallback) ?? fallback;
    }

    static Vector3 SelectInterceptRoute(HumanPlayer player, Vector3 objectivePoint, float radius)
    {
        var routeDir = (objectivePoint - player.WorldPosition).Normal;
        if (routeDir.Length < 0.01f) routeDir = Vector3.Forward;

        var midPoint = (player.WorldPosition + objectivePoint) * 0.5f;
        var perpendicular = new Vector3(-routeDir.y, routeDir.x, 0); // 旋转90°
        float offset = Random.Shared.NextFloat(radius * 0.5f, radius);
        float side = Random.Shared.NextFloat() > 0.5f ? 1 : -1;
        var target = midPoint + perpendicular * offset * side;

        var navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(target);
        if (navPos.HasValue)
        {
            // 约束: 离玩家和触发点不能太近
            if ((navPos.Value - player.WorldPosition).Length >= 200f
                && (navPos.Value - objectivePoint).Length >= 100f)
                return navPos.Value;
        }

        // 回退: 反方向也试试
        target = midPoint - perpendicular * offset * side;
        navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(target);
        return navPos ?? target;
    }

    static Vector3 SelectZoneRandom(EncounterZone zone)
    {
        var center = zone.Bounds.Center;
        var extent = (zone.Bounds.Maxs - zone.Bounds.Mins) * 0.5f;
        for (int attempt = 0; attempt < 8; attempt++)
        {
            var randomPoint = center + new Vector3(
                Random.Shared.NextFloat(-extent.x, extent.x),
                Random.Shared.NextFloat(-extent.y, extent.y),
                0);
            var navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(randomPoint);
            if (navPos.HasValue && zone.IsPointInBounds(navPos.Value))
                return navPos.Value;
        }
        return center;
    }

    // ---- 散布 ----

    /// <summary>在中心点周围散布个体位置 (即现有 FindSpreadPosition 逻辑)</summary>
    public static List<Vector3> SpreadAround(EncounterZone zone, Vector3 center, int count)
    {
        var results = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < 8; attempt++)
            {
                var angle = Random.Shared.NextFloat(0, 360f);
                var radius = Random.Shared.NextFloat(SpawnGroup.InnerRadius, SpawnGroup.OuterRadius);
                var dir = Vector3.FromYaw(angle);
                var target = center + dir * radius;
                var navPos = Game.ActiveScene.NavMesh?.GetClosestPoint(target);

                if (!navPos.HasValue) continue;
                if (!zone.IsPointInBounds(navPos.Value)) continue;
                if (results.Any(p => (p - navPos.Value).Length < SpawnGroup.MinSeparation)) continue;

                results.Add(navPos.Value);
                break;
            }
        }
        return results;
    }

    // ---- 工具 ----

    static Vector3 RotateHorizontal(Vector3 dir, float degrees)
    {
        float rad = degrees * MathF.PI / 180f;
        float cos = MathF.Cos(rad), sin = MathF.Sin(rad);
        return new Vector3(
            dir.x * cos - dir.y * sin,
            dir.x * sin + dir.y * cos,
            0);
    }

    static HumanPlayer GetNearestPlayer(Vector3 pos)
    {
        return Game.ActiveScene.GetAllComponents<HumanPlayer>()
            .Where(p => p.IsAlive)
            .OrderBy(p => (p.WorldPosition - pos).Length)
            .FirstOrDefault();
    }
}
```

### 1.10 使用示例

**场景 A: 纯开放地图, 零个 EncounterSpawnSource 实体**

所有波次 `RequiredSourceTags` 为空 → 全部走几何回退。设计师不用摆任何出生点。

**场景 B: 开放地图 + 几个关键定点**

```
Hammer 中:
  EncounterSpawnSource "gate"   Tags="boss"         ← Boss 出场点

遭遇战定义:
  Wave 0~1: 无标签 → ZoneEdge 涌入
  Wave 2:    RequiredSourceTags=["boss"] → 精确从 gate 出
```

**场景 C: 手工关卡, 全部定点**

每个波次都指定 `RequiredSourceTags`，完全不触发几何算法。

---

## 二、波次系统

### 2.1 WaveDefinition

```csharp
public class WaveDefinition
{
    // --- 敌人配置 ---
    public SpawnGroup Group { get; set; }

    // --- 触发时机 ---
    /// <summary>上一波清空后等多久才出这波。首波的此值无意义。</summary>
    public float DelayAfterPreviousClear { get; set; } = 8f;

    /// <summary>最低全局压力门控, 0=无条件。</summary>
    public float MinPressureRequired { get; set; } = 0f;

    /// <summary>存活敌人少于此数时提前触发 (重叠波次), 0=禁用, 必须 AliveCount==0。</summary>
    public int TriggerWhenAliveBelow { get; set; } = 0;

    // --- 出生位置 ---
    /// <summary>必须全匹配的出生源标签</summary>
    public List<string> RequiredSourceTags { get; set; } = new();

    /// <summary>优先匹配的出生源标签 (无匹配则回退)</summary>
    public List<string> PreferredSourceTags { get; set; } = new();

    /// <summary>标签无匹配时的几何模式</summary>
    public SpawnPositionMode FallbackPositionMode { get; set; } = SpawnPositionMode.ZoneEdge;

    /// <summary>标签匹配多个源时的选取策略</summary>
    public TagPickStrategy TagPickStrategy { get; set; } = TagPickStrategy.Random;

    /// <summary>AroundObjective 模式下是否要求出生点对玩家不可见</summary>
    public bool RequireOutOfSight { get; set; } = false;

    // --- 持续补充 ---
    /// <summary>此波后开启持续补充</summary>
    public bool StartMaintainAfter { get; set; }

    /// <summary>持续补充目标数</summary>
    public int MaintainCount { get; set; }

    /// <summary>持续补充用哪个组 (null=复用本波 Group 的轻量条目)</summary>
    public SpawnGroup MaintainGroup { get; set; }

    // --- 循环 ---
    /// <summary>清完最后一波后跳回此波次索引, null=不循环</summary>
    public int? LoopToWaveIndex { get; set; }

    // --- 预算 ---
    /// <summary>true=无视 Zone 预算和全局上限, 强制生成。默认 false。</summary>
    public bool IgnoreBudget { get; set; } = false;
}
```

### 2.2 EncounterInstance 波次部分

```csharp
public class EncounterInstance
{
    // ★ 替换原有的 TotalWaveCount / CurrentWave / NextWaveGroup / WaveInterval
    public List<WaveDefinition> Waves { get; set; } = new();

    /// <summary>-1 = 首波尚未生成</summary>
    public int CurrentWaveIndex { get; set; } = -1;

    // --- 只读派生 ---
    public bool HasPendingFirstWave => Waves.Count > 0 && CurrentWaveIndex == -1;
    public bool HasNextWave => CurrentWaveIndex >= 0 && CurrentWaveIndex < Waves.Count - 1;
    public bool CanLoop => CurrentWaveIndex >= 0
        && CurrentWaveIndex == Waves.Count - 1
        && Waves[CurrentWaveIndex].LoopToWaveIndex.HasValue;

    // --- 持续补充 (由 WaveDefinition 激活) ---
    public int MaintainCount { get; set; }
    public float MaintainInterval { get; set; } = 2f;
    public TimeSince TimeSinceMaintainCheck { get; set; }
    public SpawnGroup MaintainRespawnGroup { get; set; }

    // ... 其他原有字段
}
```

### 2.3 波次推进算法

```
UpdateEncounterWaves() 每帧:
    │
    ├─ HasPendingFirstWave?
    │   Yes → SpawnWave(Waves[0]) → CurrentWaveIndex=0 → return
    │
    ├─ 无下一波且不循环? → return
    │
    ├─ 检查推进条件:
    │   nextWave = Waves[targetIndex]  (下一波 或 LoopToWaveIndex)
    │
    │   ShouldAdvance?
    │   ├─ 条件A: AliveCount==0 && TimeSinceWaveStart > nextWave.DelayAfterPreviousClear
    │   └─ 条件B: nextWave.TriggerWhenAliveBelow>0
    │             && AliveCount <= nextWave.TriggerWhenAliveBelow
    │             && TimeSinceWaveStart > nextWave.DelayAfterPreviousClear
    │
    │   ├─ 不满足 → return
    │   └─ 满足:
    │       ├─ nextWave.MinPressureRequired > 0
    │       │   && GlobalPressure < nextWave.MinPressureRequired?
    │       │   Yes → 重置 TimeSinceWaveStart, 等下次 → return
    │       │
    │       ├─ 预算检查 (nextWave.IgnoreBudget==true → 跳过):
    │       │   zone.CurrentEnemyCount + nextWave.Group.TotalCost > zone.CombatBudget * 1.3
    │       │   && nextWave.IgnoreBudget==false?
    │       │   Yes → 重置, 等下次 → return
    │       │
    │       ├─ SpawnWave(nextWave)
    │       ├─ CurrentWaveIndex = targetIndex
    │       ├─ TimeSinceWaveStart = 0
    │       └─ nextWave.StartMaintainAfter? → 激活 MaintainCount
```

### 2.4 疏漏处理清单

| # | 疏漏 | 状态 | 实现 |
|---|------|:----:|------|
| 1 | 首波生成职责 | ✓ | `CurrentWaveIndex = -1`, `HasPendingFirstWave` → 2.3 推进算法首行 |
| 2 | 所有波同位置 | ✓ | `WaveDefinition` 逐波独立 `RequiredSourceTags` / `PreferredSourceTags` / `FallbackPositionMode` |
| 3 | 波次不检查预算 | ✓ | 2.3 推进算法: `zone.CurrentEnemyCount + nextWave.TotalCost > CombatBudget * 1.3` → 延迟 |
| 4 | 玩家全灭不终止 | ✓ | 见下方代码 |
| 5 | Delay 命名歧义 | ✓ | `DelayAfterPreviousClear` — 读下一波的此值 |
| 6 | 不支持循环波次 | ✓ | `WaveDefinition.LoopToWaveIndex`, `EncounterInstance.CanLoop` |
| 7 | 不支持重叠波次 | ✓ | `WaveDefinition.TriggerWhenAliveBelow` → 2.3 条件B |

**#4 玩家全灭终止 — `ShouldResolve()` 最终版**:

```csharp
// EncounterInstance.cs
public bool ShouldResolve()
{
    // 超时
    if ( TimeSinceCreated > MaxLifetime )
        return true;

    // 搜索阶段超时
    if ( Phase == EncounterPhase.Searching && TimeSinceCreated > 30f )
        return true;

    // ★ 玩家全灭: 直接终止, 不再补充
    if ( Phase == EncounterPhase.Active || Phase == EncounterPhase.Spawning )
    {
        var anyPlayerAlive = Game.ActiveScene.GetAllComponents<HumanPlayer>()
            .Any( p => p.IsAlive );
        if ( !anyPlayerAlive )
            return true;
    }

    // 活跃中: 当前波清完 + 无下一波 + 不在生成中 → 完成
    if ( Phase == EncounterPhase.Active
         && SpawnedEnemies.Count == 0
         && !HasNextWave
         && !CanLoop
         && !IsSpawning )
    {
        // ★ 写门控标记
        if ( SetFlagsOnComplete.Count > 0 )
        {
            var director = Game.ActiveScene.GetAllComponents<EncounterDirector>().FirstOrDefault();
            foreach ( var flag in SetFlagsOnComplete )
                director?.SetFlag( flag );
        }
        return true;
    }

    return false;
}
```

> 注意: 玩家全灭时 `ShouldResolve()` 返回 `true` 后, `CleanupResolvedEncounters` 会将遭遇战从 `ActiveEncounters` 移除。已生成的敌人**保留**（变成自由敌人），玩家复活后会碰到它们，但不再有波次推进和补充。

---

## 三、门控标记系统

### 3.1 EncounterDirector 标记字典

```csharp
// ★ 新增
public Dictionary<string, bool> EncounterFlags { get; set; } = new();

public void SetFlag(string flag)
{
    EncounterFlags[flag] = true;
    Log.Info($"Flag set: {flag}");
}

public bool CheckFlag(string flag)
    => EncounterFlags.GetValueOrDefault(flag, false);

public void ClearFlag(string flag)
    => EncounterFlags.Remove(flag);
```

### 3.2 SpawnRule 门控

```csharp
// ★ 新增字段
public List<string> RequiredFlags { get; set; } = new();   // AND: 全部满足才可用
public List<string> BlockFlags { get; set; } = new();      // OR: 任一满足则禁止

// Evaluate() 中新增:
if (RequiredFlags.Count > 0 && !RequiredFlags.All(f => director.CheckFlag(f)))
    return false;
if (BlockFlags.Count > 0 && BlockFlags.Any(f => director.CheckFlag(f)))
    return false;
```

### 3.3 EncounterInstance 完成时写标记

```csharp
public List<string> SetFlagsOnComplete { get; set; } = new();

// ShouldResolve() 中, 遭遇战完成时:
if (SetFlagsOnComplete.Count > 0)
{
    foreach (var flag in SetFlagsOnComplete)
        director.SetFlag(flag);
}
```

### 3.4 典型链式流程

```
信号塔防御完成 → SetFlag("tower_defended")
    ↓
Boss 区域 SpawnRule.RequiredFlags=["tower_defended"] → 解锁
    ↓
Boss 战完成 → SetFlag("boss_defeated")
    ↓
撤离点 SpawnRule.RequiredFlags=["boss_defeated"] → 解锁
```

---

## 四、其他改进

### 4.1 错峰生成 (SpawnStaggered)

`SpawnGroup.SpawnStaggered()` — 异步逐个生成, 间隔 `SpawnInterval`。`EncounterInstance.StartStaggeredSpawn()` 驱动, `EncounterPhase.Spawning` 跟踪。

### 4.2 maintainAICount

`EncounterInstance` 中 `MaintainCount` / `MaintainRespawnGroup` 由 `WaveDefinition.StartMaintainAfter` 激活。`EncounterDirector.TickMaintainCounts()` 每帧检查, 低于目标数时补一个。

### 4.3 Zone 体积从碰撞体读取

`EncounterZoneVolume.ToEncounterZone()` 优先级: `OverrideBoundsSize` → `Collider.Extents` → `ModelRenderer.Bounds` → 硬编码 500。

### 4.4 音乐状态

`SetZoneState()` 附带 `BroadcastMusicState(state, zoneName)`, 状态映射: Ambient→"ambient", Warming→"combat_low", Combat→"combat_high", Exhausted→"tension_release", Cooldown→"calm"。

### 4.5 全局敌人计数优化

`EnemyCounter` 静态类, `Register(zombie)` 增量计数, `OnDestroyed` 回调递减。

### 4.6 初始生成简化

删除 `ForceAmbientSpawn()`, `_firstSpawn` 标志直接触发 `RefreshAmbientEncounters`。

---

## 六、现有代码漏洞修复

### 6.1 遭遇战清理后敌人变孤儿

**问题**: `CleanupResolvedEncounters` 将 encounter 从 `ActiveEncounters` 移除后, `zombie.OwningEncounter` 仍指向已不再被管理的实例。后续代码访问 `OwningEncounter.Phase` / `OwningEncounter.OwningZone` 可能读到过期数据或 null ref。

**修法**: `ShouldResolve()` 返回 true 前, 释放存活的敌人:

```csharp
// EncounterInstance.cs — ShouldResolve() 增加:
if ( /* 即将 Resolve */ )
{
    foreach ( var zombie in SpawnedEnemies )
    {
        if ( zombie.IsValid() && zombie.IsAlive )
            zombie.OwningEncounter = null;  // ★ 变成自由敌人
    }
    return true;
}
```

同时在 `CleanupResolvedEncounters` 中增加安全过滤:

```csharp
// 已有: SpawnedEnemies.RemoveAll(e => !e.GameObject.IsValid());
// 新增: 忽略 OwningEncounter 为 null 的 zombie (已经是自由敌人)
```

---

### 6.2 跨 Zone 敌人迁移导致预算失真

**问题**: `CurrentEnemyCount` 纯按坐标统计, 敌人在 Zone A 生成, 追玩家跑进 Zone B, 导致:
- Zone A 计数偏低 → 认为有余量继续刷
- Zone B 计数偏高 → 可能超预算阻塞自己的刷新

**修法**: 敌人归属其生成时的 Zone, 统计时按归属而非坐标:

```csharp
// EncounterZone.cs — 替代原有 CurrentEnemyCount
public int CurrentEnemyCount => Game.ActiveScene.GetAllComponents<BaseZombie>()
    .Count(z => z.OwningEncounter?.OwningZone == this);

// 同时保留按坐标统计用于状态判断 (HasEngagedEnemies 仍用坐标)
public int EnemyCountByPosition => Game.ActiveScene.GetAllComponents<BaseZombie>()
    .Count(z => IsPointInBounds(z.WorldPosition));
```

`RefreshAmbientEncounters` / `RefreshReinforcements` / 波次预算检查 → 改用 `CurrentEnemyCount` (归属)。`HasEngagedEnemies` 保持用坐标, 因为不管敌人属于哪个 Zone, 在 Zone 内打架就应该影响状态。

---

### 6.3 目标防御遭遇战硬编码

**问题**: `StartObjectiveWave()` 把波次配置全部写死:

```csharp
TotalWaveCount = 3,
WaveInterval = 10f,
NextWaveGroup = SpawnGroup.ChargerWave()
```

**修法**: `EncounterObjectivePoint` 暴露 `WaveDefinition`:

```csharp
[Group("ZombieHorde")]
[Title("Encounter Objective Point")]
public partial class EncounterObjectivePoint : Component
{
    // ★ 新增: 直接在实体上配置波次
    [Property, Title("Wave Definitions")]
    public List<WaveDefinition> Waves { get; set; } = new();

    // 如果 Waves 为空 → 回退到旧的硬编码逻辑 (兼容)
}
```

`StartObjectiveWave()` 中:

```csharp
var encounter = new EncounterInstance
{
    Type = EncounterType.ObjectiveDefenseWave,
    Waves = point.Waves.Count > 0
        ? point.Waves
        : GetDefaultObjectiveWaves(),  // 旧硬编码作为后备
    // ...
};
```

---

### 6.4 SpawnGroup 不使用配置

**问题**: `encounter_default.zhcfg` 有 `SpawnGroupTemplates` 数组, 但代码全用硬编码 `SpawnGroup.ScoutPatrol()` 等静态方法, 配置从未被加载。

**修法**: `ApplyConfig()` 中解析模板, 存入字典:

```csharp
// EncounterDirector.cs
public Dictionary<string, SpawnGroup> SpawnGroupLibrary { get; set; } = new();

void ApplyConfig()
{
    // ... 原有字段

    // ★ 从配置加载 SpawnGroup 模板
    SpawnGroupLibrary.Clear();
    if (ConfigAsset?.SpawnGroupTemplates != null)
    {
        foreach (var tmpl in ConfigAsset.SpawnGroupTemplates)
        {
            SpawnGroupLibrary[tmpl.Name] = tmpl.ToSpawnGroup();
            Log.Info($"  SpawnGroup '{tmpl.Name}': TotalCost={tmpl.TotalCost}");
        }
    }
    SpawnGroup.Library = SpawnGroupLibrary; // 静态引用, 全局可见
}

// SpawnGroup.cs — 改为从库取, 静态方法降级为后备
public static SpawnGroup Get(string name)
{
    if (Library.TryGetValue(name, out var group))
        return group;
    Log.Warning($"SpawnGroup '{name}' not found in library, using fallback");
    return ScoutPatrol(); // 后备
}
```

`RefreshAmbientEncounters` 和 `GetReinforcementGroup` 改用 `SpawnGroup.Get("ScoutPatrol")` 替代 `SpawnGroup.ScoutPatrol()`。

---

### 6.5 Zone 状态变化中断待发波次

**问题**: 波次还在等 Delay, 但玩家离开 → Zone 进入 Exhausted → Cooldown, 波次挂着无人处理。

**修法**: `ShouldResolve()` 增加 Zone 状态检查:

```csharp
// EncounterInstance.cs — ShouldResolve() 增加:
if (Phase == EncounterPhase.Active || Phase == EncounterPhase.Spawning)
{
    // Zone 已退场且无玩家 → 波次不再有意义
    if (OwningZone?.State is ZoneState.Exhausted or ZoneState.Cooldown or ZoneState.Dormant
        && OwningZone.PlayerCount == 0)
        return true;
}
```

---

### 6.6 错峰生成缺少取消链路

**问题**: `SpawnStaggered` 异步逐个生成, 如果中途遭遇战被清理（玩家死了 → `ShouldResolve` 返回 true → `CleanupResolvedEncounters` 移除), 协程仍在跑, 继续生成敌人。

**修法**: `ShouldResolve()` 返回 true 时先行取消:

```csharp
// EncounterInstance.cs — ShouldResolve() 开头:
if (/* 任何 Resolve 条件满足 */)
{
    SpawnCancelSource?.Cancel();  // ★ 中止正在进行的错峰生成
    return true;
}
```

同时在 `CleanupResolvedEncounters` 中:

```csharp
if (encounter.ShouldResolve())
{
    encounter.SpawnCancelSource?.Cancel();       // ★ 双保险
    encounter.Phase = EncounterPhase.Resolved;
    zone.ActiveEncounters.RemoveAt(i);
}
```

---

### 6.7 玩家数量不影响生成规模

**问题**: 4 人局和 1 人局刷一样多的怪。`SpawnGroup.Entry.Count` 固定值。

**修法**: `WaveDefinition` 增加倍率字段, 生成时乘玩家数:

```csharp
// WaveDefinition.cs
public float PlayerCountMultiplier { get; set; } = 0.25f;  // 每个玩家 +25% 敌人

// SpawnGroup.Spawn() / SpawnStaggered() 中:
int GetEffectiveCount(int baseCount)
{
    var playerCount = Game.ActiveScene.GetAllComponents<HumanPlayer>().Count(p => p.IsAlive);
    if (playerCount <= 1) return baseCount;
    return Math.Max(baseCount, (int)(baseCount * (1f + PlayerCountMultiplier * (playerCount - 1))));
}
// 1人: baseCount
// 2人: baseCount * 1.25
// 3人: baseCount * 1.50
// 4人: baseCount * 1.75
```

倍率放在 `WaveDefinition` 上让设计师逐波可调（Boss 波可能不需要随玩家数翻倍）。

---

## 更新后的实施顺序

| 轮次 | 内容 |
|------|------|
| **一** | 错峰生成 + maintainAICount + **6.6 取消链路** |
| **二** | WaveDefinition 波次系统 + **6.3 目标防御接入** + **6.5 Zone退场处理** |
| **三** | 标签化出生源 + SpawnPositionMode 几何回退 |
| **四** | 门控标记 + Zone 体积修复 + **6.4 SpawnGroup 配置驱动** |
| **五** | 音乐集成 + 性能优化 + **6.1 孤儿敌人** + **6.2 跨Zone预算** |
| **六** | **6.7 玩家数倍率** + 初始生成简化 |
| **远期** | ActionGraph 脚本化节点库 |

---

## 附录: 关键类型总览

```
EncounterDirector
├── EncounterFlags: Dictionary<string,bool>    ← 全局标记
├── Zones: List<EncounterZone>
│     ├── SpawnSources: List<SpawnSource>
│     │     ├── Tags: HashSet<string>           ← 标签
│     │     └── IsShared: bool                  ← Tags.Count==0
│     └── ActiveEncounters: List<EncounterInstance>
│           ├── Waves: List<WaveDefinition>     ← 多阶段定义
│           │     ├── Group, DelayAfterPreviousClear, MinPressureRequired
│           │     ├── TriggerWhenAliveBelow, LoopToWaveIndex
│           │     ├── RequiredSourceTags, PreferredSourceTags, FallbackPositionMode
│           │     └── StartMaintainAfter, MaintainCount, MaintainGroup
│           ├── CurrentWaveIndex                ← -1=待首波
│           ├── MaintainCount / MaintainRespawnGroup
│           └── SetFlagsOnComplete
│
├── SpawnPositionMode (enum)                    ← 几何回退
│     ZoneEdge | BehindPlayer | AroundPlayer | AroundObjective | InterceptRoute | ZoneRandom
├── TagPickStrategy (enum)                      ← 标签源选择策略
│     Random | NearestToPlayer | FarthestFromPlayer
│
└── SpawnRule
      ├── RequiredFlags / BlockFlags            ← 门控
      └── (原有距离/视线/类型过滤)
```
