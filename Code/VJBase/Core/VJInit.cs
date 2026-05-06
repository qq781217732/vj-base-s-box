using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ Base initialization — ported from autorun/vj_base_autorun.lua and vj_controls.lua.
/// </summary>
public static class VJInit
{
    public const string Version = "3.2.0";

    /// <summary>Called on server startup. Registers convars and network strings (Phase 3).</summary>
    [ConVar("server")] public static bool vj_npc_debug { get; set; }
    [ConVar("server")] public static bool vj_npc_disable_ai { get; set; }
    [ConVar("replicated")] public static bool vj_npc_ignoreplayers { get; set; }
}
