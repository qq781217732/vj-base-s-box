# Lua AST → C# 迁移骨架规范

## 1. JSON/IR 中间表示

### 1.1 顶层结构

```jsonc
{
  "version": "1.0",
  "source": "lua/weapons/weapon_vj_crossbow.lua",
  "target": "Weapons/Crossbow.cs",
  "timestamp": "2026-05-05T17:00:00Z",
  "entities": [...],      // 迁移单元
  "dependencies": [...],   // 跨文件依赖
  "risk_summary": { "auto": 12, "semi": 8, "manual": 5 }
}
```

### 1.2 Entity — 单个迁移单元

```jsonc
{
  "id": "e001",
  "kind": "method",                    // method | field | table | require | hook | conditional
  "name": "OnPrimaryAttack",
  "sourceLines": [33, 52],
  "targetLines": "[generated]",
  "risk": "semi",                      // auto | semi | manual
  "riskReasons": ["GMod API: ents.Create", "GMod type: Vector"],
  "luaAST": {                          // 精简 AST 摘要
    "nodeType": "function_declaration",
    "params": ["status", "statusData"],
    "receiver": "SWEP",
    "bodySize": 19
  },
  "ir": { ... },                       // 见 1.3
  "csharpSkeleton": "...",            // 生成的 C# 骨架
  "todos": [...]                       // 人工待办
}
```

### 1.3 IR 指令集

| IR 指令 | Lua 模式 | 说明 |
|---------|----------|------|
| `FIELD_ASSIGN` | `SWEP.X = Y` | 表字段赋值 |
| `METHOD_DEF` | `function SWEP:M(args)` | 方法定义 |
| `REQUIRE_CALL` | `require("path")` | 模块导入 |
| `INCLUDE_CALL` | `include("path")` | 文件包含 |
| `API_CALL` | `ents.Create("x")` | GMod API 调用 |
| `VJ_CALL` | `VJ.SomeFunc()` | VJ 框架调用 |
| `COND_COMPILE` | `if CLIENT then` | 编译期条件 |
| `TABLE_LITERAL` | `{a=1, b=2}` | 表字面量 |
| `CLOSURE` | `function() ... end` | 匿名闭包 |
| `SELF_ACCESS` | `self:Method()` | self 引用 |
| `LUA_STDLIB` | `math.Rand()`, `table.insert()` | Lua 标准库 |
| `TIMER_CALL` | `timer.Simple()` | 定时器 |

---

## 2. 风险标签体系

```
┌─────────────────────────────────────────────────────┐
│  AUTO (绿色)  — 可完全自动转换                        │
│  ├─ 表字段赋值 (SWEP.X = "literal")                   │
│  ├─ 简单数值/字符串常量                                │
│  ├─ 空方法骨架 (function X() end)                     │
│  ├─ require/include 路径映射                          │
│  └─ 布尔配置项                                        │
├─────────────────────────────────────────────────────┤
│  SEMI (黄色)  — 骨架可生成，内容需人工                  │
│  ├─ 包含 GMod API 调用的方法体                          │
│  ├─ VJ 框架调用 (VJ.*)                                │
│  ├─ self 引用链                                       │
│  ├─ 条件编译块 (if CLIENT)                             │
│  └─ 复杂表字面量 (嵌套表)                               │
├─────────────────────────────────────────────────────┤
│  MANUAL (红色) — 必须人工重建                           │
│  ├─ timer.Simple() 闭包 (捕获上下文)                    │
│  ├─ 动态表构建 (运行时期决定键名)                        │
│  ├─ Lua 元表操作 (setmetatable)                        │
│  ├─ 协程 (coroutine)                                  │
│  └─ string.dbg / debug 库调用                         │
└─────────────────────────────────────────────────────┘
```

### 风险矩阵

| Lua 模式 | 风险 | 置信度 | 典型行数比 (C#/Lua) |
|----------|------|--------|-------------------|
| `ENT.X = 42` | AUTO | 0.98 | 1:1 |
| `ENT.X = Vector(1,2,3)` | SEMI | 0.85 | 2:1 |
| `function ENT:M() body end` | SEMI | 0.70 | 3:1 |
| `timer.Simple(t, fn)` | MANUAL | 0.20 | 5:1 |
| `setmetatable(t, mt)` | MANUAL | 0.10 | 10:1 |

---

## 3. C# 代码生成规则

### 3.1 命名约定

| Lua | C# |
|-----|-----|
| `weapon_vj_crossbow.lua` | `Weapons/Crossbow.cs` |
| `SWEP` | `[Weapon]` 特性类 |
| `ENT` | `[Entity]` 特性类 |
| `snake_case` 字段 | `PascalCase` 属性 |
| `function SWEP:OnXxx` | `void OnXxx(...)` |
| `self` | `this` |
| `local x = y` | `var x = y;` |

### 3.2 文件结构模板

```csharp
// [AUTO] Generated from lua/weapons/weapon_vj_crossbow.lua
// Source: VJ-Base-master, indexed 2026-05-05
// Risk: 5 auto, 2 semi, 1 manual

using SboxWeapon = Sandbox.Weapon;
using VJBase = VJ.Base;

namespace VJ.Weapons;

[Weapon(Base = "weapon_vj_base")]
[DisplayName("Crossbow")]
public partial class Crossbow : VJBaseWeapon
{
    // === AUTO: 配置字段 ===
    [Property] public string WorldModel { get; set; } = "models/weapons/w_crossbow.mdl";
    [Property] public string HoldType { get; set; } = "crossbow";
    [Property] public bool MadeForNPCsOnly { get; set; } = true;
    [Property] public float NPC_NextPrimaryFire { get; set; } = 1f;
    [Property] public float NPC_TimeUntilFire { get; set; } = 0.15f;
    [Property] public float NPC_FiringDistanceScale { get; set; } = 2.5f;
    [Property] public bool NPC_StandingOnly { get; set; } = true;

    // === AUTO: 嵌套表 → 子类 ===
    public PrimaryData Primary { get; set; } = new()
    {
        ClipSize = 1,
        Ammo = "XBowBolt",
        Sound = "weapons/crossbow/fire1.wav",
        DisableBulletCode = true,
    };

    // === SEMI: 方法骨架保留，内容需迁移 ===
    /// <summary>
    /// [SEMI] 迁移自 SWEP:OnPrimaryAttack (lua:33-52)
    /// TODO: GMod API 调用需要替换
    ///   - ents.Create("obj_vj_crossbowbolt") → new CrossbowBolt()
    ///   - self:GetBulletPos() → GetMuzzlePosition()
    ///   - VJ.CalculateTrajectory(...) → VJPhysics.CalculateTrajectory(...)
    /// </summary>
    protected void OnPrimaryAttack(string status, object statusData)
    {
        // TODO[MANUAL]: 迁移 timer 闭包逻辑
        throw new NotImplementedException("Manual migration required");
    }
}
```

### 3.3 各模式映射表

| Lua 模式 | C# 生成 |
|----------|---------|
| `SWEP.X = "str"` | `[Property] public string X { get; set; } = "str";` |
| `SWEP.X = 42` | `[Property] public int X { get; set; } = 42;` |
| `SWEP.X = true` | `[Property] public bool X { get; set; } = true;` |
| `SWEP.X = Vector(1,2,3)` | `// SEMI: Vector3(1,2,3) — verify coordinate system` |
| `SWEP.X = {a=1, b=2}` | 生成嵌套子类 `XData` |
| `function SWEP:M(a,b)` | `void M(string a, string b) { /* TODO */ }` |
| `if CLIENT then` | `if (Game.IsClient) { ... }` (SEMI) |
| `local x = ents.Create("y")` | `// TODO: var x = new Y();` |
| `self:GetOwner()` | `this.Owner` 或 `// TODO` |
| `VJ.EmitSound(...)` | `VJAudio.EmitSound(...)` (需包装) |
| `timer.Simple(t, fn)` | `// MANUAL: await Task.Delay; fn()` |
| `require("path")` | `using VJ.Path;` |
| `include("file.lua")` | `// include → using / partial class` |

---

## 4. TODO 注释格式

### 4.1 标准格式

```csharp
// TODO[MIG][risk]: description
//   - action_item_1
//   - action_item_2
// SRC: source_file.lua:line_start-line_end
```

### 4.2 风险级别

```csharp
// TODO[MIG][AUTO]:  Verify type mapping
// TODO[MIG][SEMI]:  Replace GMod API call
// TODO[MIG][MANUAL]: Rewrite timer closure
```

### 4.3 示例

```csharp
// TODO[MIG][SEMI]: Replace GMod ents.Create with S&box entity spawn
//   - ents.Create("obj_vj_crossbowbolt") → new CrossbowBolt()
//   - projectile:SetPos() → projectile.Position = ...
//   - projectile:Spawn() → projectile.Spawn()
// SRC: weapon_vj_crossbow.lua:36-42

// TODO[MIG][MANUAL]: Lua closure captures self — rewrite as async method
//   - timer.Simple(duration, function() ... end)
//   - captured: self, self:GetOwner(), sdLoadDone table
//   - S&box equivalent: await Task.Delay; callback pattern
// SRC: weapon_vj_crossbow.lua:58-62

// TODO[MIG][SEMI]: Verify Vector coordinate system (GMod → S&box)
//   - GMod: Vector(x, y, z) where z is up
//   - S&box: Vector3(x, y, z) — confirm axis mapping
//   - math.Rand(-30, 30) → Random.Shared.Next(-30, 30)
// SRC: weapon_vj_crossbow.lua:46-48
```

---

## 5. 实时演示：weapon_vj_crossbow.lua

### 源文件 (Lua, 64行)

```lua
AddCSLuaFile()
SWEP.Base = "weapon_vj_base"
SWEP.PrintName = "Crossbow"
SWEP.WorldModel = "models/weapons/w_crossbow.mdl"
SWEP.Primary.ClipSize = 1
SWEP.Primary.Ammo = "XBowBolt"
-- ... (配置省略)

function SWEP:OnPrimaryAttack(status, statusData)       -- line 33
    if status == "Init" then
        if CLIENT then return end
        local projectile = ents.Create("obj_vj_crossbowbolt")
        local spawnPos = self:GetBulletPos()
        local owner = self:GetOwner()
        projectile:SetPos(spawnPos)
        projectile:Activate()
        projectile:Spawn()
        local phys = projectile:GetPhysicsObject()
        if owner.IsVJBaseSNPC then
            phys:SetVelocity(VJ.CalculateTrajectory(owner, owner:GetEnemy(),
                "Line", spawnPos + Vector(math.Rand(-30,30), math.Rand(-30,30), math.Rand(-30,30)), 1, 4000))
        else
            phys:SetVelocity(VJ.CalculateTrajectory(owner, owner:GetEnemy(),
                "Line", spawnPos, owner:GetEnemy():GetPos() + owner:GetEnemy():OBBCenter(), 4000))
        end
    end
end

function SWEP:OnReload(status)                            -- line 56
    if status == "Start" then
        timer.Simple(SoundDuration("weapons/crossbow/reload1.wav"), function()
            if IsValid(self) && IsValid(self:GetOwner()) then
                VJ.EmitSound(self:GetOwner(), sdLoadDone, self.NPC_ReloadSoundLevel)
            end
        end)
    end
end
```

### 迁移分析 (逐行)

```
Line 1:  AddCSLuaFile()
         → [AUTO] [CSharpIgnore] — S&box 不需要客户端注册

Line 2:  SWEP.Base = "weapon_vj_base"
         → [AUTO] [Weapon(Base = "weapon_vj_base")] 或继承 VJBaseWeapon

Line 3:  SWEP.PrintName = "Crossbow"
         → [AUTO] [DisplayName("Crossbow")]

Line 6:  SWEP.WorldModel = "models/weapons/w_crossbow.mdl"
         → [AUTO] public string WorldModel { get; set; } = "models/...";

Line 9:  if CLIENT then
         → [SEMI] #if CLIENT / if (Game.IsClient) — 需确认 S&box 等价物

Line 10: VJ.AddKillIcon(...)
         → [SEMI] VJKillIcon.Register(...) — VJ API 需要包装层

Line 24-31: SWEP.Primary.X = Y (嵌套表)
         → [AUTO] 生成 PrimaryData 子类

Line 33: function SWEP:OnPrimaryAttack(status, statusData)
         → [SEMI] 方法签名可自动，body 中大量 GMod API

Line 35: if CLIENT then return end
         → [SEMI] 同 Line 9

Line 36: ents.Create("obj_vj_crossbowbolt")
         → [SEMI] var bolt = new CrossbowBoltEntity();

Line 37: self:GetBulletPos()
         → [SEMI] GetMuzzlePosition() — S&box 对应 API

Line 40-42: projectile:SetPos/Activate/Spawn
         → [SEMI] bolt.Position = / bolt.Spawn() — API 映射

Line 44: projectile:GetPhysicsObject()
         → [SEMI] bolt.PhysicsBody — S&box 属性访问

Line 46: VJ.CalculateTrajectory(owner, owner:GetEnemy(), "Line", ...)
         → [SEMI] VJPhysics.CalculateTrajectory(...) — 需包装

Line 48: Vector(math.Rand(-30,30), ...)
         → [AUTO] new Vector3(Random.Shared.Next(-30,30), ...)

Line 54: local sdLoadDone = {"sound1.wav", "sound2.wav"}
         → [AUTO] string[] sdLoadDone = {"sound1.wav", "sound2.wav"};

Line 56: function SWEP:OnReload(status)
         → [SEMI] 方法骨架自动，timer 闭包需手动

Line 58-62: timer.Simple(duration, function() ... end)
         → [MANUAL] 闭包捕获了 self 和 sdLoadDone
            需要重写为：async Task + CancellationToken 模式
            这是整个文件中唯一标记为 MANUAL 的部分
```

### 迁移统计

| 行 | 模式 | 风险 | 占比 |
|----|------|------|------|
| 1 | AddCSLuaFile | AUTO | — |
| 2-8 | 顶层配置 | AUTO | 11% |
| 9-11 | 条件编译 | SEMI | 5% |
| 24-31 | 嵌套表 | AUTO | 13% |
| 33-51 | OnPrimaryAttack | SEMI | 30% |
| 54 | 表变量 | AUTO | 2% |
| 56-63 | OnReload + timer | SEMI/MANUAL | 13% |

**总计: AUTO 26% | SEMI 62% | MANUAL 12%**

### 生成的 C# 骨架

```csharp
// [MIG] Generated from lua/weapons/weapon_vj_crossbow.lua
// Stats: AUTO:7 SEMI:3 MANUAL:1 | Migration effort: ~30min

using SboxWeapon = Sandbox.Weapon;

namespace VJ.Weapons;

[Weapon(Base = "weapon_vj_base")]
[DisplayName("Crossbow")]                          // AUTO: SWEP.PrintName
[Category("VJ Base")]                              // AUTO: SWEP.Category
public partial class Crossbow : VJBaseWeapon
{
    // ── AUTO: 顶层配置 ──────────────────────────
    [Property] public override string WorldModel { get; set; }
        = "models/weapons/w_crossbow.mdl";
    [Property] public override string HoldType { get; set; } = "crossbow";
    [Property] public bool MadeForNPCsOnly { get; set; } = true;
    [Property] public float NPC_NextPrimaryFire { get; set; } = 1f;
    [Property] public float NPC_TimeUntilFire { get; set; } = 0.15f;
    [Property] public float NPC_FiringDistanceScale { get; set; } = 2.5f;

    // ── AUTO: 嵌套表 → PrimaryData ──────────────
    public PrimaryData Primary { get; set; } = new()
    {
        ClipSize = 1,
        Ammo = "XBowBolt",
        Sound = "weapons/crossbow/fire1.wav",
        DisableBulletCode = true,
    };

    // ── SEMI: 方法体需 API 映射 ──────────────────
    // SRC: weapon_vj_crossbow.lua:33-52
    protected void OnPrimaryAttack(string status, object statusData)
    {
        // TODO[MIG][SEMI]: if CLIENT then return end
        //   → S&box equivalent: if (Game.IsClient) return;
        if (status != "Init") return;
        if (Game.IsClient) return;

        // TODO[MIG][SEMI]: Replace GMod entity creation
        //   ents.Create("obj_vj_crossbowbolt") → CrossbowBoltEntity
        var bolt = new CrossbowBoltEntity();
        var muzzlePos = GetMuzzlePosition();       // self:GetBulletPos()
        bolt.Position = muzzlePos;                 // projectile:SetPos()
        bolt.Owner = this.Owner;                   // projectile:SetOwner()
        bolt.Spawn();                              // projectile:Spawn()

        var phys = bolt.PhysicsBody;               // projectile:GetPhysicsObject()
        var owner = this.Owner;
        var enemy = owner.GetEnemy();

        // TODO[MIG][SEMI]: Verify VJ.CalculateTrajectory port
        Vector3 velocity;
        if (owner is VJBaseSNPC snpc)
        {
            velocity = VJPhysics.CalculateTrajectory(
                owner, enemy, TrajectoryType.Line,
                muzzlePos + new Vector3(
                    Random.Shared.Next(-30, 30),
                    Random.Shared.Next(-30, 30),
                    Random.Shared.Next(-30, 30)),
                1f, 4000f);
        }
        else
        {
            velocity = VJPhysics.CalculateTrajectory(
                owner, enemy, TrajectoryType.Line,
                muzzlePos,
                enemy.Position + enemy.OBBcenter, 4000f);
        }
        phys.Velocity = velocity;

        // TODO[MIG][SEMI]: projectile:SetAngles(velocity:GetNormal():Angle())
        bolt.Rotation = Rotation.LookAt(velocity.Normal);
    }

    // ── AUTO: 静态数据 ──────────────────────────
    private static readonly string[] sdLoadDone =
        { "weapons/crossbow/bolt_load1.wav", "weapons/crossbow/bolt_load2.wav" };

    // ── SEMI + MANUAL: timer 闭包需重写 ──────────
    // SRC: weapon_vj_crossbow.lua:56-63
    protected async void OnReload(string status)
    {
        // TODO[MIG][SEMI]: if status == "Start"
        if (status != "Start") return;

        // TODO[MIG][MANUAL]: Rewrite timer.Simple as async
        //   Original: timer.Simple(SoundDuration(...), function()
        //       if IsValid(self) && IsValid(self:GetOwner()) then
        //           VJ.EmitSound(self:GetOwner(), sdLoadDone, ...)
        //       end
        //   end)
        //
        //   Issues:
        //     1. SoundDuration() — needs S&box audio API
        //     2. Closure captures this + sdLoadDone — check lifetime
        //     3. VJ.EmitSound → VJAudio.EmitSound wrapper
        //
        //   Suggested S&box pattern:
        var duration = Sound.Duration("weapons/crossbow/reload1.wav");
        await Task.Delay(TimeSpan.FromSeconds(duration));
        if (this.IsValid && this.Owner.IsValid)
        {
            VJAudio.EmitSound(this.Owner, sdLoadDone, NPC_ReloadSoundLevel);
        }
    }
}

// ── AUTO: 嵌套数据类 ────────────────────────────
public class PrimaryData
{
    public int ClipSize { get; set; } = 1;
    public string Ammo { get; set; } = "XBowBolt";
    public string Sound { get; set; } = "";
    public bool DisableBulletCode { get; set; } = true;
    public string[] MuzzleParticles { get; set; } = Array.Empty<string>();
    public bool MuzzleParticlesAsOne { get; set; } = true;
    public string MuzzleAttachment { get; set; } = "muzzle";
    public bool SpawnShells { get; set; } = false;
}
```

### 核心发现

1. **AUTO (26%)** — 配置字段、嵌套表、简单字面量可以完全自动化
2. **SEMI (62%)** — 方法骨架和大部分 API 调用可生成但需人工验证
3. **MANUAL (12%)** — 主要是 timer.Simple 闭包，需要完全重写为 async/await

**整个文件的预估迁移时间**: ~30分钟（配置自动生成，方法体逐行对照，timer 重写）

---

## 附录 A: GMod → S&box API 速查表

| GMod API | S&box 等价 |
|----------|-----------|
| `ents.Create("class")` | `new ClassName()` |
| `ent:SetPos(v)` | `ent.Position = v` |
| `ent:GetPos()` | `ent.Position` |
| `ent:Spawn()` | `ent.Spawn()` |
| `ent:GetOwner()` | `ent.Owner` |
| `ent:GetPhysicsObject()` | `ent.PhysicsBody` |
| `phys:SetVelocity(v)` | `phys.Velocity = v` |
| `Vector(x,y,z)` | `new Vector3(x,y,z)` |
| `math.Rand(a,b)` | `Random.Shared.Next(a,b)` |
| `IsValid(ent)` | `ent.IsValid` |
| `timer.Simple(t, fn)` | `await Task.Delay(t); fn()` |
| `VJ.EmitSound(...)` | `VJAudio.EmitSound(...)` |
| `if CLIENT` | `if (Game.IsClient)` |
| `if SERVER` | `if (Game.IsServer)` |

## 附录 B: 工具链建议

```
[Lua 源文件]
    │
    ├─→ GitNexus 扫描 → 知识图谱 (符号/调用/导入)
    │
    ├─→ tree-sitter-lua AST → IR 提取
    │       │
    │       ├─ [AUTO] → C# 代码直接生成
    │       ├─ [SEMI] → C# 骨架 + TODO 注释
    │       └─ [MANUAL] → TODO 注释 + 原始 Lua 引用
    │
    └─→ 输出: .cs 文件 + migration_report.json
```
