# npc_vj_creature_base/shared.lua 对照审计

> 日期：2026-05-10  
> Lua: `F:/DevProject/Sbox/VJ-Base-master/lua/entities/npc_vj_creature_base/shared.lua`  
> C#: `F:/DevProject/Sbox/testzombie/Code/VJBase/Bases/CreatureNPC.cs`

## 结论

`shared.lua` 的服务器/共享部分已补齐到 `CreatureNPC.cs`。GMod 客户端绘制、LOD、IK 和旧 `CustomOnDraw` 兼容逻辑属于 Source/GMod 客户端渲染生命周期，当前不迁入 S&Box 逻辑层。

## 行级对照

| Lua 行 | Lua 内容 | C# 对应 | 判定 | 备注 |
|---|---|---|---|---|
| 1-6 | `Base`/`Type`/`PrintName`/作者元数据 | N/A | N/A | GMod spawn menu 元数据，S&Box 不用同形字段 |
| 7 | `AutomaticFrameAdvance = false` | `AutomaticFrameAdvance` | PASS | 默认 `false` |
| 9 | `IsVJBaseSNPC = true` | `IsVJBaseSNPC = true` | PASS | 共享给 HumanNPC |
| 10 | `IsVJBaseSNPC_Creature = true` | `IsVJBaseSNPC_Creature = true` | PASS | creature 专属标志 |
| 12-14 | `SetAutomaticFrameAdvance(val)` | `SetAutomaticFrameAdvance(bool val)` | PASS | 1:1 设置字段 |
| 16-21 | `MatFootStepQCEvent(data) return false` | `MatFootStepQCEvent(object data) => false` | PASS | C# 返回 `bool?`，为 Lua 的 `nil/true/false` 语义预留 |
| 23-25 | 注释掉的 `FireAnimationEvent` | N/A | N/A | Lua 中本身已注释 |
| 27-68 | `if CLIENT then ... Draw/LOD/IK` | N/A | N/A | GMod 客户端渲染和 convar 逻辑；不在当前 S&Box NPC 逻辑层迁移 |

## 验证

- `python verify_api_mapping.py`：已跑完。
- `dotnet build Code/testzombie.csproj`：未能执行到编译阶段，本机 SDK 为 .NET 8，项目目标为 .NET 10，报 `NETSDK1045`。
