# vj_base/ai/base_tank.lua 对照审计

> 日期：2026-05-10  
> Lua: `F:/DevProject/Sbox/VJ-Base-master/lua/vj_base/ai/base_tank.lua`  
> C#: `F:/DevProject/Sbox/testzombie/Code/VJBase/Bases/TankNPC.cs`

## 结论

`base_tank.lua` 的字段默认值和 4 个方法已对照到 `TankNPC.cs`。本次修正了两个翻译偏差：

- `SCHEDULE_FACE` 改为真正 override `BaseNPC.Schedule.cs` 的签名，确保坦克不走普通转向 schedule。
- 删除 `TankNPC` 中遮蔽 `BaseNPC` 的 `VJ_ID_Boss` / `YieldToAlliedPlayers` 同名字段，让通用 `HasEntityFlag("VJ_ID_Boss")` 能读到 Boss 标志。

## 行级对照

| Lua 行 | Lua 内容 | C# 对应 | 判定 | 备注 |
|---|---|---|---|---|
| 8 | `VJ_ID_Boss = true` | `BaseNPC.VJ_ID_Boss = true` | PASS | 使用基类字段，避免同名遮蔽 |
| 9-11 | 视野/转向默认值 | `SightAngle` / `SightDistance` / `TurningSpeed` | PASS | `TurningSpeed` 为坦克局部配置 |
| 12 | `HullType = HULL_LARGE` | `HullType = SourceHull.Large` | PASS | 新增命名常量，避免魔法数字 |
| 13 | `HasMeleeAttack = false` | `HasMeleeAttack = false` | PASS | 继承 BaseNPC 字段 |
| 16-21 | 免疫/死亡/痛声默认值 | `Bleeds` / `Immune_*` / `DeathCorpseCollisionType` / `HasPainSounds` | PASS | `COLLISION_GROUP_NONE` 映射为 `SourceCollisionGroup.None` |
| 24-29 | 行为默认值 | `DisableWandering` / `CanReceiveOrders` / `DeathAllyResponse` / `DamageAllyResponse` / `CombatDamageResponse` / `YieldToAlliedPlayers` | PASS | `YieldToAlliedPlayers` 使用 BaseNPC 字段 |
| 31 | `SCHEDULE_FACE(...) return end` | `override SCHEDULE_FACE(...)` no-op | PASS | 坦克不走普通转向 |
| 33 | `MaintainAlertBehavior(...) return end` | `override MaintainAlertBehavior(...)` no-op | PASS | 坦克不追逐 |
| 35-50 | `OnDamaged` | `OnDamaged(DamageInfo, int, string)` | SEMI | 结构已对齐；`crossbow_bolt` 依赖 Phase 3 projectile tag 填充 |
| 52-57 | `Tank_AngleDiffuse` | `Tank_AngleDiffuse(float,float)` | PASS | 角度 wrap 逻辑一致 |

## 验证

- `python verify_api_mapping.py`：已跑完。
- `dotnet build Code/testzombie.csproj`：未能执行到编译阶段，本机 SDK 为 .NET 8，项目目标为 .NET 10，报 `NETSDK1045`。
