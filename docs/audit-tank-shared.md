# npc_vj_tank_base/shared.lua 对照审计

> 日期：2026-05-10  
> Lua: `F:/DevProject/Sbox/VJ-Base-master/lua/entities/npc_vj_tank_base/shared.lua`  
> C#: `F:/DevProject/Sbox/testzombie/Code/VJBase/Bases/TankNPC.cs`

## 结论

`npc_vj_tank_base/shared.lua` 已对照到 `TankNPC.cs`。身份字段补齐，两个物理回调按 Lua 空函数保留 no-op。

## 行级对照

| Lua 行 | Lua 内容 | C# 对应 | 判定 | 备注 |
|---|---|---|---|---|
| 1-6 | `Base`/`Type`/`PrintName`/作者元数据 | N/A | N/A | GMod spawn menu 元数据 |
| 8 | `IsVJBaseSNPC_Tank = true` | `IsVJBaseSNPC_Tank = true` | PASS | TankNPC 身份 |
| 9 | `IsVJBaseSNPC_TankChassis = true` | `IsVJBaseSNPC_TankChassis = true` | PASS | TankGunner 构造器会改回 `false`，匹配 gunner shared.lua |
| 10 | `VJ_ID_Vehicle = true` | `VJ_ID_Vehicle = true` | PASS | 已加入 BaseNPC/VJEntityFlags 通用标志 |
| 12 | `PhysicsCollide(data, physobj) end` | `PhysicsCollide(object, object)` | PASS | Lua 空函数 → C# no-op |
| 14 | `PhysicsUpdate(physobj) end` | `PhysicsUpdate(object)` | PASS | Lua 空函数 → C# no-op |

## 附：npc_vj_tankg_base/shared.lua

TankGunner shared 文件仅声明 `IsVJBaseSNPC_Tank = true`、`IsVJBaseSNPC_TankGun = true`、`VJ_ID_Vehicle = true`。`TankGNPC : TankNPC` 继承 Tank/VJ vehicle 标志，并在构造器中将 `IsVJBaseSNPC_TankChassis` 置回 `false`，避免继承 chassis 语义。
