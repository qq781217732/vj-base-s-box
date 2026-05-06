# Mock 框架 & S&Box API 映射手册

## 核心规则

**Lua 方法调用 → C# 方法调用，签名一致，机械翻译。**

标记只表示填坑阶段怎么做，翻译阶段全写方法调用即可：

| 标记 | 含义 | 翻译时 | 填坑时（阶段3） |
|------|------|--------|-----------------|
| `M` | Source 引擎独有 | 写 `entity.方法名(args)` | 从零实现 |
| `Sw` | S&Box 有现成 API | 同上，签名不变 | 方法内部调 S&Box |
| `C` | 纯 C# 逻辑 | 同上 | 自己写实现 |
| `X` | S&Box 不需要 | 跳过，不翻译 | — |

> `Sw` 和 `C` 和 `M` 对外暴露的签名全是方法。`GetParent()` 不会变成 `.Parent`，填坑的人自己在方法里决定怎么实现。

---

## Mock 接口 0：GlobalEngine（全局基础函数）

不挂在实体上，包装成静态方法，保证翻译时一律函数调用。

```csharp
public static class GlobalEngine
{
    // ═══ 时间 ═══
    public static float GetCurrentTime()        => Time.Now;            // CurTime()
    public static float GetFrameTime()          => Time.Delta;          // FrameTime()
    public static long GetSystemTime()          => Stopwatch.GetTimestamp(); // SysTime()

    // ═══ 域判断 ═══
    public static bool IsServer()               => !IsProxy;            // SERVER
    public static bool IsClient()               => IsProxy;             // CLIENT

    // ═══ 随机 ═══
    public static int RandomInt(int a, int b)   => Game.Random.NextInt(a, b);  // math.random
    public static float RandomFloat(float a, float b) => Game.Random.NextFloat(a, b); // math.Rand
    public static float Clamp(float v, float min, float max) => Math.Clamp(v, min, max);

    // ═══ 物理查询 ═══
    // util.TraceLine({start, end})
    public static TraceResult TraceLine(Vector3 start, Vector3 end)
        => Game.ActiveScene.Trace.Ray(start, end).Run();

    // util.TraceHull({...})
    public static TraceResult TraceHull(Vector3 start, Vector3 end, float radius)
        => Game.ActiveScene.Trace.Ray(start, end).Radius(radius).Run();

    // ents.FindInSphere(pos, r)
    public static IEnumerable<GameObject> FindInSphere(Vector3 pos, float radius)
        => Game.ActiveScene.FindInPhysics(new Sphere(pos, radius));

    // ents.FindInCone(pos, dir, r, angle)
    public static IEnumerable<GameObject> FindInCone(Vector3 pos, Vector3 dir, float radius, float angle)
        => /* 自建: Trace + 角度过滤 */ new List<GameObject>();

    // ents.GetAll()
    public static IEnumerable<T> GetAllComponents<T>() where T : Component
        => Game.ActiveScene.GetAllComponents<T>();

    // ═══ 日志 ═══
    public static void Print(params object[] args) => Log.Info(string.Join(" ", args)); // print()
    public static void PrintTable(object tbl)       => /* 自建 Dump */ ;               // PrintTable()
    public static void MsgC(Color c, params object[] args) => Log.Info(string.Join(" ", args)); // MsgC — 暂用 Log.Info

    // ═══ 存在性验证 ═══
    public static bool IsValid(GameObject obj)      => obj.IsValid;       // IsValid(obj)

    // ═══ 工具 ═══
    public static float Lerp(float a, float b, float t) => MathX.Lerp(a, b, t);
    public static Angles LerpAngle(Angles a, Angles b, float t) => Angles.Lerp(a, b, t);
    public static Vector3 LerpVector(Vector3 a, Vector3 b, float t) => Vector3.Lerp(a, b, t);
}
```

### 类型构造（用 new，不用包装函数）
```csharp
// Lua              → C#
Vector(x, y, z)       → new Vector3(x, y, z)
Angle(p, y, r)        → new Angles(p, y, r)
Color(r, g, b, a)     → new Color(r, g, b, a)
```

---

## Mock 接口 1：IEngineEntity

**所有实体方法统一走这个接口，全方法签名。** 标记 `Sw`/`C`/`M`/`X` 只影响填坑。

```csharp
public interface IEngineEntity
{
    // ═══ Transform ═══
    Vector3 GetPos();                                    // Sw
    void SetPos(Vector3 v);                              // Sw
    Angles GetAngles();                                  // Sw
    void SetAngles(Angles a);                            // Sw
    Vector3 GetForward();                                // Sw
    Vector3 GetRight();                                  // Sw
    Vector3 GetUp();                                     // Sw
    Vector3 GetVelocity();                               // Sw
    void SetVelocity(Vector3 v);                         // M
    Vector3 EyePos();                                    // C
    Vector3 WorldSpaceCenter();                          // Sw
    Vector3 OBBCenter();                                 // Sw
    Vector3 NearestPoint(Vector3 pos);                   // Sw
    float BoundingRadius();                              // Sw

    // ═══ 生命周期 ═══
    void Spawn();                                        // Sw
    void Remove();                                       // Sw

    // ═══ 模型/外观 ═══
    void SetModel(string model);                         // Sw
    string GetModel();                                   // Sw
    void SetColor(Color c);                              // Sw
    void SetMaterial(string mat);                        // Sw
    void SetModelScale(float s);                         // Sw
    void SetBodygroup(int n, int v);                     // Sw
    void SetSkin(int n);                                 // Sw

    // ═══ 类型/标识 ═══
    string GetClass();                                   // Sw
    int EntIndex();                                      // Sw
    string GetName();                                    // Sw
    void SetName(string n);                              // Sw
    bool IsPlayer();                                     // Sw
    bool IsNPC();                                        // Sw

    // ═══ 父子关系 ═══
    void SetParent(GameObject parent);                   // Sw
    GameObject GetParent();                              // Sw
    void SetOwner(GameObject owner);                     // C
    GameObject GetOwner();                               // C

    // ═══ 碰撞/物理 ═══
    void SetCollisionGroup(int group);                   // Sw
    int GetCollisionGroup();                             // Sw
    void SetSolid(int type);                             // Sw
    int GetSolid();                                      // Sw
    object GetPhysicsObject();                           // Sw
    bool IsOnGround();                                   // Sw
    float WaterLevel();                                  // C

    // ═══ 生命/伤害 ═══
    float Health();                                      // C
    void SetHealth(float v);                             // C
    float GetMaxHealth();                                // C
    void SetMaxHealth(float v);                          // C
    bool Alive();                                        // C
    void TakeDamage(object dmginfo);                     // Sw
    void Ignite(float time);                             // C
    void Extinguish();                                   // C

    // ═══ 标志/存档 ═══
    bool IsFlagSet(int flag);                            // M
    void AddFlags(int flags);                            // M
    void RemoveFlags(int flags);                         // M
    void SetSaveValue(string key, object val);           // M
    object GetSaveValue(string key);                     // M

    // ═══ 动画 ═══
    void SetMovementActivity(int act);                   // M
    int GetMovementActivity();                           // M
    int GetSequence();                                   // M
    void SetSequence(int seq);                           // M
    string GetSequenceName(int seq);                     // M
    int GetSequenceActivity(int seq);                    // M
    bool IsSequenceFinished();                           // M
    float GetCycle();                                    // M
    void SetCycle(float val);                            // M
    float SequenceDuration();                            // M
    float SequenceDuration(int seq);                     // M
    float GetSequenceMoveDist(int seq);                  // M
    int LookupSequence(string name);                     // M
    void FrameAdvance(float time);                       // M
    void AutoMovement(float interval);                   // M
    void SetPlaybackRate(float rate);                    // M
    float GetPlaybackRate();                             // M
    void SetPoseParameter(string name, float val);       // M
    float GetPoseParameter(string name);                 // M
    int LookupAttachment(string name);                   // M
    Vector3 GetAttachment(int id);                       // M

    // ═══ 删除（S&Box 无此概念） ═══
    // SetMoveType()        // X
    // GetMoveType()        // X
    // SetRenderMode()      // X
    // SetRenderFX()        // X
}
```

---

## Mock 接口 2：IEngineAITaskSystem

Source 引擎 Task 调度核心。S&Box 完全没有，全部 `M`。

```csharp
public interface IEngineAITaskSystem
{
    int GetTaskID(string taskName);                      // M
    void StartEngineTask(GameObject npc, int taskId, float data); // M
    void RunEngineTask(GameObject npc, int taskId, float data);   // M
    void TaskComplete(GameObject npc);                   // M
    int GetCurGoalType(GameObject npc);                  // M
}

// 引擎任务状态常量
public enum TaskStatus                                    // M
{
    New = 0,       // TASKSTATUS_NEW
    RunTask = 1    // TASKSTATUS_RUN_TASK
}
```

### TASK_* 常量类（M）

```csharp
public static class EngineTask
{
    // 路径寻找
    public const string GetPathToLastPosition  = "TASK_GET_PATH_TO_LASTPOSITION";
    public const string GetPathToTarget        = "TASK_GET_PATH_TO_TARGET";
    public const string GetPathToEnemy         = "TASK_GET_PATH_TO_ENEMY";
    public const string GetPathToEnemyLOS      = "TASK_GET_PATH_TO_ENEMY_LOS";
    public const string GetPathToRandomNode    = "TASK_GET_PATH_TO_RANDOM_NODE";

    // 移动执行
    public const string RunPath                = "TASK_RUN_PATH";
    public const string WalkPath               = "TASK_WALK_PATH";
    public const string RunPathFlee            = "TASK_RUN_PATH_FLEE";
    public const string RunPathTimed           = "TASK_RUN_PATH_TIMED";
    public const string WalkPathTimed          = "TASK_WALK_PATH_TIMED";
    public const string RunPathForUnits        = "TASK_RUN_PATH_FOR_UNITS";
    public const string WalkPathForUnits       = "TASK_WALK_PATH_FOR_UNITS";
    public const string RunPathWithinDist      = "TASK_RUN_PATH_WITHIN_DIST";
    public const string WalkPathWithinDist     = "TASK_WALK_PATH_WITHIN_DIST";
    public const string WeaponRunPath          = "TASK_WEAPON_RUN_PATH";
    public const string ItemRunPath            = "TASK_ITEM_RUN_PATH";
    public const string MoveToTargetRange      = "TASK_MOVE_TO_TARGET_RANGE";
    public const string MoveToGoalRange        = "TASK_MOVE_TO_GOAL_RANGE";
    public const string MoveAwayPath           = "TASK_MOVE_AWAY_PATH";

    // 面对/旋转
    public const string FaceTarget             = "TASK_FACE_TARGET";
    public const string FaceEnemy              = "TASK_FACE_ENEMY";
    public const string FacePlayer             = "TASK_FACE_PLAYER";
    public const string FaceLastPosition       = "TASK_FACE_LASTPOSITION";
    public const string FaceSavePosition       = "TASK_FACE_SAVEPOSITION";
    public const string FacePath               = "TASK_FACE_PATH";
    public const string FaceHintNode           = "TASK_FACE_HINTNODE";
    public const string FaceIdeal              = "TASK_FACE_IDEAL";
    public const string FaceReasonable         = "TASK_FACE_REASONABLE";

    // 掩护
    public const string FindCoverFromOrigin    = "TASK_FIND_COVER_FROM_ORIGIN";
    public const string FindCoverFromEnemy     = "TASK_FIND_COVER_FROM_ENEMY";
    // 等待
    public const string Wait                   = "TASK_WAIT";
    public const string WaitForMovement        = "TASK_WAIT_FOR_MOVEMENT";

    // 控制
    public const string SetToleranceDistance   = "TASK_SET_TOLERANCE_DISTANCE";
    public const string SetRouteSearchTime     = "TASK_SET_ROUTE_SEARCH_TIME";
    public const string StopMoving             = "TASK_STOP_MOVING";
    public const string Forget                 = "TASK_FORGET";
    public const string IgnoreOldEnemies       = "TASK_IGNORE_OLD_ENEMIES";
    public const string StoreBestSound         = "TASK_STORE_BESTSOUND_REACTORIGIN_IN_SAVEPOSITION";
    public const string PlaySequence           = "TASK_PLAY_SEQUENCE";
    public const string PlaySequenceFaceEnemy  = "TASK_PLAY_SEQUENCE_FACE_ENEMY";
    public const string SetActivity            = "TASK_SET_ACTIVITY";
    public const string ResetActivity          = "TASK_RESET_ACTIVITY";

    // VJ 自定义
    public const string VJPlayActivity         = "TASK_VJ_PLAY_ACTIVITY";
    public const string VJPlaySequence         = "TASK_VJ_PLAY_SEQUENCE";
}
```

---

## Mock 接口 3：INPCConditions

70 个 Source 条件常量 + 3 个方法。全部 `M`。

```csharp
public enum Condition
{
    None = 0,  InPVS = 1,  IdleInterrupt = 2,
    LowPrimaryAmmo = 3,  NoPrimaryAmmo = 4,  NoSecondaryAmmo = 5,
    NoWeapon = 6,  SeeHate = 7,  SeeFear = 8,
    SeeDislike = 9,  SeeEnemy = 10,  LostEnemy = 11,
    EnemyWentNull = 12,  EnemyOccluded = 13,  TargetOccluded = 14,
    HaveEnemyLOS = 15,  HaveTargetLOS = 16,
    LightDamage = 17,  HeavyDamage = 18,  PhysicsDamage = 19,
    RepeatedDamage = 20,  CanRangeAttack1 = 21,  CanRangeAttack2 = 22,
    CanMeleeAttack1 = 23,  CanMeleeAttack2 = 24,  Provoked = 25,
    NewEnemy = 26,  EnemyTooFar = 27,  EnemyFacingMe = 28,
    BehindEnemy = 29,  EnemyDead = 30,  EnemyUnreachable = 31,
    SeePlayer = 32,  LostPlayer = 33,  SeeNemesis = 34,
    TaskFailed = 35,  ScheduleDone = 36,  Smell = 37,
    TooCloseToAttack = 38,  TooFarToAttack = 39,
    NotFacingAttack = 40,  WeaponHasLOS = 41,
    WeaponBlockedByFriend = 42,  WeaponPlayerInSpread = 43,
    WeaponPlayerNearTarget = 44,  WeaponSightOccluded = 45,
    BetterWeaponAvailable = 46,  HealthItemAvailable = 47,
    GiveWay = 48,  WayClear = 49,
    HearDanger = 50,  HearThumper = 51,  HearBugbait = 52,
    HearCombat = 53,  HearWorld = 54,  HearPlayer = 55,
    HearBulletImpact = 56,  HearPhysicsDanger = 57,
    HearMoveAway = 58,  HearSpooky = 59,  NoHearDanger = 60,
    FloatingOffGround = 61,  MobbedByEnemies = 62,
    ReceivedOrders = 63,
    PlayerAddedToSquad = 64,  PlayerRemovedFromSquad = 65,
    PlayerPushing = 66,  NPCFreeze = 67,  NPCUnfreeze = 68,
    TalkerRespondToQuestion = 69,  NoCustomInterrupts = 70
}

public interface INPCConditions
{
    void SetCondition(GameObject npc, Condition cond);       // M
    void ClearCondition(GameObject npc, Condition cond);     // M
    bool HasCondition(GameObject npc, Condition cond);       // M
}
```

---

## Mock 接口 4：INPCSchedule

Source 引擎的 Schedule/Task 编排。全部 `M`。

```csharp
public interface INPCSchedule
{
    void StartSchedule(GameObject npc, AISchedule sched);   // M
    void ClearSchedule(GameObject npc);                     // M
    void StopCurrentSchedule(GameObject npc);               // M
    void NextTask(GameObject npc, AISchedule sched);        // M
    void DoSchedule(GameObject npc, AISchedule sched);      // M
    void ScheduleFinished(GameObject npc, AISchedule sched);// M
    void SetTask(GameObject npc, AITask task);              // M
    void RunTask(GameObject npc, AITask task);              // M
    void StartTask(GameObject npc, AITask task);            // M
    bool TaskFinished(GameObject npc);                      // M
    bool IsScheduleFinished(GameObject npc, AISchedule sched); // M
    float TaskTime(GameObject npc, float taskStartTime);    // M
    void OnTaskComplete(GameObject npc);                    // M
    void OnTaskFailed(GameObject npc, int failCode, string failString); // M
    void OnMovementFailed(GameObject npc);                  // M
    void OnMovementComplete(GameObject npc);                // M
}

// AISchedule — VJ Lua 模块 vj_ai_schedule.lua 翻译
public class AISchedule
{
    public bool CanBeInterrupted;
    public bool ResetOnFail;
    public List<AITask> Tasks;

    public void EngTask(string taskName, float data);       // M
    public void AddTask(string taskName, float data);       // M
}
```

---

## Mock 接口 5：INPCAttributes

```csharp
public interface INPCAttributes
{
    int GetNPCState(GameObject npc);                        // C → 非 Source NPCState，VJ 用 VJState 枚举
    void SetNPCState(GameObject npc, int state);            // C

    // 阵营
    int Disposition(GameObject npc, GameObject other);      // M
    void AddEntityRelationship(GameObject npc, GameObject other, int disp, int priority); // M

    // 寻路辅助
    void SetLastPosition(GameObject npc, Vector3 pos);      // M
    Vector3 GetLastPosition(GameObject npc);                // M
    float GetMaxYawSpeed(GameObject npc);                   // M
    void SetMaxYawSpeed(GameObject npc, float speed);       // M
    bool IsMoving(GameObject npc);                          // M
    bool IsBusy(GameObject npc);                            // M

    // 敌人
    GameObject GetEnemy(GameObject npc);                    // M
    void SetEnemy(GameObject npc, GameObject enemy);        // M
}
```

### 相关常量

```csharp
// 注意: VJ 不用传统 NPCState，用 VJState (None/Freeze/OnlyAnimation/...)
public enum Disposition { Error = 0, Like = 1, Neutral = 2, Hate = 3, Fear = 4, Interest = 100 } // M — D_VJ_INTEREST
public enum NavType { Ground, Fly, None, Jump, Climb }       // M
public enum MoveType { None, VPhysics, Step, Fly, FlyGravity, NoClip, Push, Walk, Observer } // M

public static class RelationshipClass                        // M
{
    public const string PlayerAlly   = "CLASS_PLAYER_ALLY";
    public const string Combine      = "CLASS_COMBINE";
    public const string Zombie       = "CLASS_ZOMBIE";
    public const string Antlion      = "CLASS_ANTLION";
    public const string Xen          = "CLASS_XEN";
    public const string BlackOps     = "CLASS_BLACKOPS";
    public const string UnitedStates = "CLASS_UNITED_STATES";
    public const string Aperture     = "CLASS_APERTURE";
    public const string VJBase       = "CLASS_VJ_BASE";
}
```

---

## Mock 接口 6：IEngineCombat

```csharp
public interface IEngineCombat
{
    void TakeDamageInfo(GameObject npc, object dmginfo);     // M
}

public enum DamageType : uint                                 // M
{
    Blast = 1, Club = 2, Bullet = 4, Dissolve = 8,
    AlwaysGib = 16, EnergyBeam = 32, Vehicle = 64,
    Crush = 128, SlowBurn = 256, Physgun = 512,
    Plasma = 1024, Sonic = 2048, Shock = 4096,
    NeverGib = 8192, RemoveNoRagdoll = 16384, Direct = 32768
}

public enum HitGroup                                           // M
{
    Generic = 0, Head = 1, Chest = 2, Stomach = 3,
    LeftArm = 4, RightArm = 5, LeftLeg = 6, RightLeg = 7
}
```

---

## Mock 接口 7：IEngineSound

```csharp
public enum SoundType                                          // M
{
    Danger, Combat, BulletImpact, World, Player,
    PlayerVehicle, Carcass, Meat, Garbage,
    PhysicsDanger, MoveAway, Thumper, Bugbait,
    ContextReactToSource, ContextExcludeCombine,
    ContextPlayerVehicle, ContextMortar
}
```

---

## 外围 API（不经过 Mock 接口）

这些是独立顶层调用，不挂在实体上。

### 计时器
```csharp
timer.Simple(t, fn)         → GameTask.DelaySeconds(t).ContinueWith(_ => fn())
timer.Create(n, t, r, fn)   → 自建 for + GameTask.DelaySeconds 循环
timer.Remove(n)             → CancellationToken.Cancel()
```

### 音效
```csharp
self:EmitSound(snd)         → Sound.Play(snd, entity.GetPos())
sound.Play(s, pos)          → Sound.Play(s, pos)
VJ.STOPSOUND                → soundHandle.Stop()
```

### 网络
```csharp
net.Start / Write* / Send   → [Rpc.Broadcast] 方法
net.Receive                 → [Rpc.Broadcast] + Component
```

### 输入
```csharp
ply:KeyDown(btn)            → Input.Down(btn)
IN_ATTACK / IN_JUMP 等      → InputButton 常量
```

### NavMesh
```csharp
NavMesh.GetRandomPoint      → Game.ActiveScene.NavMesh.GetRandomPoint(pos, r)
NavMesh.GetClosestPoint     → Game.ActiveScene.NavMesh.GetClosestPoint(pos)
```

---

## Mock 接口汇总

| 接口 | 方法数 | 常量数 | 阶段 |
|------|--------|--------|------|
| `IEngineEntity` | 65 | — | 阶段1 |
| `IEngineAITaskSystem` | 5 | 34 (EngineTask) | 阶段1 |
| `INPCConditions` | 5 | 70 (Condition) | 阶段1 |
| `INPCSchedule` | 16 | — | 阶段1 |
| `INPCAttributes` | 10 | 4 enum | 阶段1 |
| `IEngineCombat` | 1 | 2 enum | 阶段2 |
| `IEngineSound` | — | 1 enum | 阶段2 |
| **合计** | **~91** | **~111** | |

---

## 翻译示例

```lua
-- Lua（schedules.lua）
function ENT:SCHEDULE_FACE(faceTask, customFunc)
    local sched = vj_ai_schedule.New()
    sched:EngTask("TASK_FACE_TARGET", 0)
    sched:EngTask("TASK_WAIT_FOR_MOVEMENT", 0)
    sched:EngTask(faceTask, 0)
    if customFunc then customFunc(sched) end
    self:StartSchedule(sched)
end
```

```csharp
// C# 机械翻译 — 全是方法调用，签名 1:1 对应
public void SCHEDULE_FACE(string faceTask, Action<AISchedule> customFunc)
{
    var sched = new AISchedule();
    sched.EngTask(EngineTask.FaceTarget, 0);
    sched.EngTask(EngineTask.WaitForMovement, 0);
    sched.EngTask(faceTask, 0);
    customFunc?.Invoke(sched);
    _schedule.StartSchedule(GameObject, sched);
}
```

`sched:EngTask()` → `sched.EngTask()`。`self:StartSchedule()` → `_schedule.StartSchedule()`。只变语法，不变签名。

---

## 翻译流程

```
遇到 Lua 调用 x:Method(args)
    │
    1. Ctrl+F 搜 Method → 找到对应接口的方法签名
    │
    2. 写 C#: _接口实例.Method(args)
    │
    3. 编译 → 缺方法就加 → 继续
    │
    4. 文件翻译完，编译通过 → 完成
```
