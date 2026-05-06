# 远征模式 (类潜兵) 原型落地方案

## 目标

用最少的新代码跑通循环：

```
大厅进房 → 落地 → 自由行动 → 完成目标 (战斗) → 撤离 → 结算 → 重开
```

**不动现有系统**：`EncounterDirector`、`HumanPlayer`、`Zone`、`SpawnGroup` 全部直接复用。

---

## 新增类总览

| 类 | 职责 | 估行 |
|---|------|:---:|
| `ExpeditionGamemode` | 5 阶段状态机 + 目标计数 + 结算 + 撤离触发 | ~120 |
| `MissionObjective` | 5 种目标行为, 每种一个流程分支 | ~250 |
| `ExtractionPoint` | 激活/呼叫/倒计时/登机/全员撤离检测 | ~80 |
| `PickupResource` | 碰了捡, 存玩家背包 | ~30 |
| **合计** | | **~480** |

---

## 一、ExpeditionGamemode — 状态机

### 1.1 阶段定义

```csharp
public enum ExpeditionPhase
{
    WaitingForPlayers,   // 加载中
    DropSequence,        // 进场动画
    Active,              // 玩家自由行动, 目标开放
    ExtractionReady,     // 主目标全部完成, 撤离点激活
    Extracting,          // 玩家呼叫撤离, 倒计时中
    Complete,            // 有人成功登机
    Failed               // 全员死亡且无复活
}
```

### 1.2 状态流转

```
WaitingForPlayers
    │  所有玩家加载完毕
    ▼
DropSequence
    │  动画播完 / 跳过 / 3秒超时
    ▼
Active ──────────────────────────────────────────┐
    │  主目标完成数 >= MainObjectiveCount         │
    ▼                                             │
ExtractionReady                                   │
    │  玩家交互 ExtractionPoint "呼叫撤离"        │
    ▼                                             │
Extracting                                       │
    │  倒计时归零                                 │
    ▼                                             │
Complete → 结算 → 返回大厅                        │
                                                  │
Active / ExtractionReady / Extracting (任意时刻)   │
    │  全员死亡 AND 复活次数用尽                   │
    └──────────────────────────────────────────→ Failed
```

### 1.3 实现

```csharp
[Group("ZombieHorde")]
[Title("Expedition Gamemode")]
public partial class ExpeditionGamemode : BaseGamemode
{
    [Sync] public ExpeditionPhase CurrentPhase { get; set; } = ExpeditionPhase.WaitingForPlayers;
    [Sync] public int ObjectivesComplete { get; set; }
    [Sync] public int MainObjectiveCount { get; set; } = 2;  // 启动时从场景统计
    [Sync] public int ExtractionCountdown { get; set; }       // 显示用
    [Sync] public int PlayersExtracted { get; set; }
    [Sync] public int TotalPlayers { get; set; }

    [Property] public float DropSequenceDuration { get; set; } = 3f;
    [Property] public ExtractionPoint ExtractionPoint { get; set; }  // 场景中的撤离点

    public List<MissionObjective> MainObjectives { get; set; } = new();
    public List<MissionObjective> SideObjectives { get; set; } = new();

    // ---- 生命周期 ----

    protected override void OnStart()
    {
        if (IsProxy) return;
        CollectObjectivesFromScene();
        _ = BeginDropSequence();
    }

    void CollectObjectivesFromScene()
    {
        var all = Game.ActiveScene.GetAllComponents<MissionObjective>().ToList();
        MainObjectives = all.Where(o => o.IsMainObjective).ToList();
        SideObjectives = all.Where(o => !o.IsMainObjective).ToList();
        MainObjectiveCount = MainObjectives.Count;

        Log.Info($"Expedition: {MainObjectives.Count} main + {SideObjectives.Count} side objectives");
    }

    // ---- 阶段切换 ----

    async Task BeginDropSequence()
    {
        SetPhase(ExpeditionPhase.DropSequence);
        // TODO: 播放降落舱动画
        await GameTask.DelaySeconds(DropSequenceDuration);
        SetPhase(ExpeditionPhase.Active);
    }

    public void OnObjectiveComplete(MissionObjective obj)
    {
        if (!obj.IsMainObjective) return;

        ObjectivesComplete++;
        Log.Info($"Main objective: {ObjectivesComplete}/{MainObjectiveCount}");

        if (ObjectivesComplete >= MainObjectiveCount)
        {
            SetPhase(ExpeditionPhase.ExtractionReady);
            ExtractionPoint?.Activate();
        }
    }

    public void OnExtractionCalled()
    {
        if (CurrentPhase != ExpeditionPhase.ExtractionReady) return;
        SetPhase(ExpeditionPhase.Extracting);

        // 全图高压 — 接入已有遭遇战系统
        var director = Game.ActiveScene.GetAllComponents<EncounterDirector>().FirstOrDefault();
        director?.AddPressure(80f);
    }

    public void OnPlayerExtracted(HumanPlayer player)
    {
        PlayersExtracted++;
        if (PlayersExtracted >= TotalPlayers)
            SetPhase(ExpeditionPhase.Complete);
    }

    public void OnAllPlayersDead()
    {
        if (CurrentPhase is ExpeditionPhase.Complete) return;
        SetPhase(ExpeditionPhase.Failed);
        _ = RestartAfterDelay(5f);
    }

    void SetPhase(ExpeditionPhase phase)
    {
        var prev = CurrentPhase;
        CurrentPhase = phase;
        Log.Info($"Phase: {prev} → {phase}");
    }

    async Task RestartAfterDelay(float seconds)
    {
        await GameTask.DelaySeconds(seconds);
        // P0: 重新加载场景
        // Game.ActiveScene.LoadFromFile(Game.ActiveScene.SceneFile);
    }

    void DoMissionComplete()
    {
        // 结算留桩, 后续接 MetaProgression
        int totalResources = 0;
        foreach (var p in Game.ActiveScene.GetAllComponents<HumanPlayer>())
            totalResources += p.GetResource("samples");
        Log.Info($"Mission Complete — {PlayersExtracted} extracted, {totalResources} samples");
    }
}
```

---

## 二、MissionObjective — 5 种目标类型

### 2.1 枚举

```csharp
public enum ObjectiveType
{
    Interact,        // 到点 + 长按E + 遭遇战刷怪
    DefendArea,      // 进入区域 + 守N秒 + 持续刷怪
    DestroyTarget,   // 目标有HP + 打掉 + 刷怪防守
    RetrieveItem,    // A→B搬运 + 搬的过程中刷怪追
    EliminateTarget, // 击杀特定Boss敌人
}

public enum ObjectiveState
{
    Inactive,        // 尚未开放
    Ready,           // 可交互, UI标记
    Active,          // 进行中
    Complete,        // 完成
    Failed           // 失败 (超时/玩家死亡等)
}
```

### 2.2 基类

```csharp
[Group("ZombieHorde")]
[Title("Mission Objective")]
public partial class MissionObjective : Component, Component.ITriggerListener
{
    // --- 配置 ---
    [Property] public ObjectiveType Type { get; set; } = ObjectiveType.Interact;
    [Property] public string DisplayName { get; set; } = "目标";
    [Property] public bool IsMainObjective { get; set; } = true;
    [Property] public EncounterObjectivePoint LinkedEncounter { get; set; }

    // --- 通用 ---
    [Property] public float ActivationRadius { get; set; } = 300f;

    // --- Interact/Defend ---
    [Property] public float HoldDuration { get; set; } = 3f;
    [Property] public float DefendDuration { get; set; } = 60f;

    // --- DestroyTarget ---
    [Property] public GameObject TargetObject { get; set; }
    [Property] public float TargetHealth { get; set; } = 500f;

    // --- RetrieveItem ---
    [Property] public GameObject PickupPoint { get; set; }
    [Property] public GameObject DropoffPoint { get; set; }
    [Property] public float CarrySpeedMultiplier { get; set; } = 0.6f;

    // --- EliminateTarget ---
    [Property] public string BossEnemyName { get; set; } = "ChargerZombie";

    // --- 运行时 ---
    [Sync] public ObjectiveState State { get; set; } = ObjectiveState.Inactive;
    [Sync] public float Progress { get; set; }       // 0~1, UI用
    [Sync] public bool IsInteracting { get; set; }

    public ExpeditionGamemode Gamemode => ZombieNetworkManager.Instance?.Gamemode as ExpeditionGamemode;

    protected override void OnStart()
    {
        if (IsProxy) return;
        // Tamemode 进入 Active 后, 激活所有目标
        _ = WaitForActive();
    }

    async Task WaitForActive()
    {
        while (Gamemode?.CurrentPhase != ExpeditionPhase.Active)
            await GameTask.DelaySeconds(0.5f);
        State = ObjectiveState.Ready;
    }

    // ---- 触发器: 玩家接近 ----

    void ITriggerListener.OnTriggerEnter(Collider other)
    {
        if (State != ObjectiveState.Ready) return;
        var player = other.GameObject.Components.Get<HumanPlayer>();
        if (player == null || !player.IsAlive) return;

        // 显示 UI "长按E <DisplayName>"
        player.SetInteractionPrompt(DisplayName, () => OnInteract(player));
    }

    void ITriggerListener.OnTriggerExit(Collider other)
    {
        var player = other.GameObject.Components.Get<HumanPlayer>();
        if (player == null) return;
        player.ClearInteractionPrompt();
    }

    // ---- 路由 ----

    public void OnInteract(HumanPlayer player)
    {
        if (State != ObjectiveState.Ready) return;

        switch (Type)
        {
            case ObjectiveType.Interact:        _ = RunInteract(player); break;
            case ObjectiveType.DefendArea:      _ = RunDefend(player); break;
            case ObjectiveType.DestroyTarget:   _ = RunDestroy(player); break;
            case ObjectiveType.RetrieveItem:    _ = RunRetrieve(player); break;
            case ObjectiveType.EliminateTarget: _ = RunEliminate(player); break;
        }
    }

    void Complete()
    {
        State = ObjectiveState.Complete;
        // 停止关联的遭遇战
        if (LinkedEncounter != null) { /* 标记 encounter 不应再刷新波 */ }
        Gamemode?.OnObjectiveComplete(this);
    }
}
```

### 2.3 五种类型实现

#### Interact — 启动终端

```
流程:
  Ready → 玩家按E → Active
    → EncounterObjectivePoint 开始刷怪
    → 玩家长按E, 进度条 3秒
    → 松手: 进度重置, 怪继续刷
    → 进度满 → Complete
```

```csharp
async Task RunInteract(HumanPlayer player)
{
    State = ObjectiveState.Active;
    ActivateEncounter();

    float held = 0f;
    while (held < HoldDuration)
    {
        if (!player.IsAlive || State == ObjectiveState.Failed)
            return;

        // 玩家必须持续按住且在范围内
        if (IsInteracting && IsPlayerInRange(player))
        {
            held += Time.Delta;
            Progress = held / HoldDuration;
        }
        else
        {
            held = 0f;  // 松手重置
            Progress = 0f;
        }

        await GameTask.DelayFrame();
    }

    Complete();
}
```

#### DefendArea — 防守阵地

```
流程:
  Ready → 玩家进入区域 → Active
    → EncounterObjectivePoint 开始刷波次
    → 倒计时 N 秒, 玩家必须在区域内
    → 离开: 倒计时暂停, UI 警告
    → 倒计时归零 → Complete
```

```csharp
async Task RunDefend(HumanPlayer player)
{
    State = ObjectiveState.Active;
    ActivateEncounter();

    float remaining = DefendDuration;
    while (remaining > 0)
    {
        if (!player.IsAlive || State == ObjectiveState.Failed)
            return;

        if (IsPlayerInRange(player))
        {
            remaining -= Time.Delta;
            Progress = 1f - remaining / DefendDuration;
        }
        // 离开范围 → 倒计时暂停, Progress 不更新

        await GameTask.DelayFrame();
    }

    Complete();
}
```

#### DestroyTarget — 破坏目标

```
流程:
  Ready → 玩家进入范围或首次造成伤害 → Active
    → EncounterObjectivePoint 刷怪
    → 目标物件有独立 HP, 显示血条
    → HP 归零 → Complete
```

```csharp
async Task RunDestroy(HumanPlayer player)
{
    State = ObjectiveState.Active;
    ActivateEncounter();

    if (TargetObject == null) { Complete(); return; }

    // 在目标上挂一个 DamageTracker 来监听 HP
    var tracker = TargetObject.Components.GetOrCreate<ObjectiveDamageTracker>();
    tracker.MaxHealth = TargetHealth;
    tracker.CurrentHealth = TargetHealth;

    while (tracker.CurrentHealth > 0)
    {
        if (State == ObjectiveState.Failed) return;
        Progress = 1f - tracker.CurrentHealth / TargetHealth;
        await GameTask.DelaySeconds(0.5f);
    }

    // 播放爆炸特效
    Complete();
}

// 附属组件: 挂到目标物件上
public partial class ObjectiveDamageTracker : Component, IDamageable
{
    [Sync] public float MaxHealth { get; set; } = 500f;
    [Sync] public float CurrentHealth { get; set; } = 500f;

    public void OnDamage(DamageInfo info)
    {
        CurrentHealth = Math.Max(0, CurrentHealth - info.Damage);
    }
}
```

#### RetrieveItem — 搬运

```
流程:
  Ready → 玩家走到PickupPoint, 交互 → Active
    → EncounterObjectivePoint 刷怪 (巡逻兵开始追)
    → 玩家变为搬运状态: 移速×0.6, 不能开枪
    → UI 标记 DropoffPoint
    → 玩家到达 DropoffPoint → 自动交付 → Complete
    → 中途死亡: 物品掉落原地, 其他玩家可拾取
```

```csharp
async Task RunRetrieve(HumanPlayer player)
{
    State = ObjectiveState.Active;
    ActivateEncounter();

    // 拾取
    var carrier = player;
    carrier.ApplyStatusEffect(new CarryStatusEffect { SpeedMultiplier = CarrySpeedMultiplier });
    // 在玩家身上挂载物品 model / UI 标记

    // 等待到达交付点
    while (carrier.IsAlive && State != ObjectiveState.Failed)
    {
        var dist = (carrier.WorldPosition - DropoffPoint.WorldPosition).Length;
        Progress = 1f - Math.Clamp(dist / 500f, 0f, 1f);

        if (dist < 80f)  // 到达交付点
        {
            carrier.RemoveStatusEffect<CarryStatusEffect>();
            Complete();
            return;
        }

        // 如果携带者死亡
        if (!carrier.IsAlive)
        {
            carrier.RemoveStatusEffect<CarryStatusEffect>();
            // 掉落: 在原地生成一个可交互的拾取物, 其他玩家来捡
            var drop = new GameObject(true, "DroppedItem");
            drop.WorldPosition = carrier.WorldPosition;
            drop.Components.Create<DroppedObjectiveItem>().Setup(PickupPoint, DropoffPoint, LinkedEncounter);
            State = ObjectiveState.Ready;  // 回到 Ready, 等其他玩家来捡
            return;
        }

        await GameTask.DelaySeconds(0.5f);
    }
}
```

#### EliminateTarget — 击杀目标

```
流程:
  Ready → 玩家进入范围 → Active
    → EncounterObjectivePoint 刷怪, 最后一波包含 Boss
    → Boss 带特殊标记, UI 显示 Boss 血条
    → Boss 死亡 → Complete (不需要清完小怪)
```

```csharp
async Task RunEliminate(HumanPlayer player)
{
    State = ObjectiveState.Active;
    ActivateEncounter();

    // 等待波次系统生成带标记的 Boss
    BaseZombie boss = null;
    while (boss == null && State != ObjectiveState.Failed)
    {
        boss = Game.ActiveScene.GetAllComponents<BaseZombie>()
            .FirstOrDefault(z => z.IsAlive && z.IsObjectiveTarget);
        await GameTask.DelaySeconds(0.5f);
    }

    if (boss == null) return;

    // 等待 Boss 死亡
    while (boss.IsAlive && State != ObjectiveState.Failed)
    {
        Progress = 1f - boss.Health / boss.MaxHealth;
        await GameTask.DelaySeconds(0.5f);
    }

    if (boss.IsAlive) return;
    Complete();
}
```

### 2.4 PlayerBase 需要的最小交互接口

```csharp
// HumanPlayer / PlayerBase 新增 (不改现有逻辑, 只加桩):

public void SetInteractionPrompt(string text, Action onInteract) { /* UI显示 */ }
public void ClearInteractionPrompt() { /* UI清除 */ }
public void ApplyStatusEffect(StatusEffect effect) { /* 状态效果系统 */ }
public void RemoveStatusEffect<T>() { /* 移除效果 */ }
public int GetResource(string id) => 0;  // P0桩, 后续接背包系统
```

---

## 三、ExtractionPoint — 撤离机制

### 3.1 状态

```csharp
public enum ExtractionState
{
    Inactive,       // 目标未完成, 不可用
    Callable,       // 目标完成, 玩家可呼叫撤离
    CountingDown,   // 撤离倒计时中
    Arrived,        // 撤离载具到达, 可登机
    Departed        // 已出发 (所有存活玩家登机后)
}
```

### 3.2 实现

```csharp
[Group("ZombieHorde")]
[Title("Extraction Point")]
public partial class ExtractionPoint : Component, Component.ITriggerListener
{
    [Property] public float CountdownDuration { get; set; } = 120f;
    [Property] public float BoardingRadius { get; set; } = 120f;

    [Sync] public ExtractionState State { get; set; } = ExtractionState.Inactive;
    [Sync] public float CountdownRemaining { get; set; }

    ExpeditionGamemode Gamemode => ZombieNetworkManager.Instance?.Gamemode as ExpeditionGamemode;

    public void Activate()
    {
        if (State != ExtractionState.Inactive) return;
        State = ExtractionState.Callable;
        // 放信号弹特效 / UI 图标
        Log.Info("Extraction point is now callable");
    }

    void ITriggerListener.OnTriggerEnter(Collider other)
    {
        var player = other.GameObject.Components.Get<HumanPlayer>();
        if (player == null || !player.IsAlive) return;

        switch (State)
        {
            case ExtractionState.Callable:
                player.SetInteractionPrompt("呼叫撤离", () => CallExtraction());
                break;
            case ExtractionState.Arrived:
                player.SetInteractionPrompt("登机撤离", () => BoardExtraction(player));
                break;
        }
    }

    void ITriggerListener.OnTriggerExit(Collider other)
    {
        var player = other.GameObject.Components.Get<HumanPlayer>();
        player?.ClearInteractionPrompt();
    }

    public void CallExtraction()
    {
        if (State != ExtractionState.Callable) return;
        _ = RunCountdown();
    }

    async Task RunCountdown()
    {
        State = ExtractionState.CountingDown;
        Gamemode?.OnExtractionCalled();

        CountdownRemaining = CountdownDuration;
        while (CountdownRemaining > 0)
        {
            CountdownRemaining -= Time.Delta;
            await GameTask.DelayFrame();
        }

        State = ExtractionState.Arrived;
        // 载具到达特效 / 音效
        Log.Info("Extraction shuttle has arrived");
    }

    void BoardExtraction(HumanPlayer player)
    {
        if (State != ExtractionState.Arrived) return;

        // 播放登机动画
        Gamemode?.OnPlayerExtracted(player);

        // 如果所有存活玩家都在范围内 → 立刻出发
        var allAlive = Game.ActiveScene.GetAllComponents<HumanPlayer>()
            .Where(p => p.IsAlive).ToList();
        var inRange = allAlive.Where(p =>
            (p.WorldPosition - WorldPosition).Length <= BoardingRadius).ToList();

        if (inRange.Count >= allAlive.Count)
        {
            State = ExtractionState.Departed;
            Log.Info("All players extracted — shuttle departed");
        }
    }
}
```

---

## 四、PickupResource — 局内资源

```csharp
[Group("ZombieHorde")]
[Title("Pickup Resource")]
public partial class PickupResource : Component, Component.ITriggerListener
{
    [Property] public string ResourceId { get; set; } = "samples";
    [Property] public int Amount { get; set; } = 1;
    [Property] public bool DestroyOnPickup { get; set; } = true;

    [Sync] public bool IsPickedUp { get; set; }

    void ITriggerListener.OnTriggerEnter(Collider other)
    {
        if (IsPickedUp) return;
        var player = other.GameObject.Components.Get<HumanPlayer>();
        if (player == null || !player.IsAlive) return;

        player.AddResource(ResourceId, Amount);
        IsPickedUp = true;

        if (DestroyOnPickup)
            GameObject.Destroy();
    }
}
```

---

## 五、Quick Reference — 5 种目标行为

| 类型 | 怎么激活 | 进行中干什么 | 怎么完成 | 失败条件 |
|------|---------|------------|---------|---------|
| **Interact** | 走进范围 + 按E | 长按E, 进度条 3秒, 松手重置 | 进度条满 | 玩家死亡 |
| **DefendArea** | 走进范围 | 待在区域内, 倒计时 60-90秒 | 倒计时归零 | 玩家离开太久或死亡 |
| **DestroyTarget** | 走进范围 或 攻击目标 | 攻击目标物件直到 HP=0 | HP 归零 | 玩家全灭 |
| **RetrieveItem** | 走进PickupPoint + 交互 | 搬运到DropoffPoint, 移速降低 | 到达交付点 | 携带者死亡→物掉落 |
| **EliminateTarget** | 走进范围 | 波次最后一波刷出Boss | Boss死亡 | 玩家全灭 |

## 六、Hammer 摆放示例

```
场景:

  SpawnPoint ×4                              [已有]
  EncounterZoneVolume "Zone_A"               [已有]
  EncounterZoneVolume "Zone_B"               [已有]

  // 主目标1: 启动终端
  MissionObjective "终端目标"
    ├── Type = Interact
    ├── DisplayName = "启动通讯终端"
    ├── IsMainObjective = true
    ├── HoldDuration = 3.0
    └── LinkedEncounter = → EncounterObjectivePoint "终端波次"

  // 主目标2: 摧毁虫巢
  MissionObjective "虫巢目标"
    ├── Type = DestroyTarget
    ├── DisplayName = "摧毁虫巢"
    ├── IsMainObjective = true
    ├── TargetObject = → 场景中的 Breakable 物件
    ├── TargetHealth = 500
    └── LinkedEncounter = → EncounterObjectivePoint "虫巢波次"

  // 支线: 回收补给箱
  MissionObjective "补给箱"
    ├── Type = RetrieveItem
    ├── DisplayName = "回收补给"
    ├── IsMainObjective = false
    ├── PickupPoint = → Transform A
    ├── DropoffPoint = → Transform B
    └── LinkedEncounter = → EncounterObjectivePoint "搬运波次"

  // 撤离点
  ExtractionPoint "撤离点"
    └── CountdownDuration = 120

  // 资源散落
  PickupResource ×8  (散布在地图各处)
    └── ResourceId = "samples", Amount = 1
```

## 七、数据流

```
ExpeditionGamemode
    │
    ├─ Phase: DropSequence → Active
    │   └─ 所有 MissionObjective.State → Ready
    │
    ├─ Phase: Active
    │   ├─ MissionObjective.OnInteract(player)
    │   │   └─ 激活 EncounterObjectivePoint → EncounterDirector 接管刷怪
    │   │   └─ Run[Type]() → Complete() → Gamemode.OnObjectiveComplete()
    │   │
    │   └─ ObjectivesComplete >= MainObjectiveCount?
    │       └─ Phase → ExtractionReady
    │           └─ ExtractionPoint.Activate()
    │
    ├─ Phase: ExtractionReady → Extracting
    │   └─ 玩家交互 ExtractionPoint → CallExtraction()
    │       └─ Gamemode.OnExtractionCalled() → AddPressure(80)
    │       └─ 倒计时 120s
    │
    ├─ Phase: Extracting → Complete
    │   └─ ExtractionPoint.State = Arrived
    │   └─ 玩家交互登机 → BoardExtraction()
    │       └─ 全员登机 → Gamemode.OnPlayerExtracted()
    │           └─ Phase → Complete → DoMissionComplete()
    │
    └─ 任意时刻: AllPlayersDead
        └─ Phase → Failed → 5s 后重开

EncounterDirector (不改)
    ├─ Zone 状态机正常运行
    ├─ 目标激活时 → EncounterObjectivePoint 波次生成
    └─ 撤离时 → AddPressure(80) → 全图 Combat

PlayerBase (最小改动)
    ├─ SetInteractionPrompt / ClearInteractionPrompt (UI桩)
    ├─ ApplyStatusEffect / RemoveStatusEffect (搬运状态)
    └─ AddResource / GetResource (资源计数)
```

## 八、不做的 (P0 范围外)

| 项目 | 理由 |
|------|------|
| 持久化 / 局外成长 | 用纯 UI 大厅代替 |
| 配装系统 | 写死武器/装备 |
| 复活/增援机制 | 死了重开, P0 不做 |
| 支线目标奖励 | 支线只打 Log, 暂不影响结算 |
| 多任务地图池 | 一张 Hammer 图硬编码目标 |
| ActionGraph 脚本化 | 目标行为直接写代码 |
| MetaProgressionService | 结算时打 Log, 后续接 |
| UI | 优先用 DebugOverlay 文本, 不做正式 UI |
