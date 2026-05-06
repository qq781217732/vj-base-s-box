using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ Hook/Event system — ported from vj_base/hooks.lua.
/// In S&Box, hooks are replaced by Component lifecycle and events.
/// </summary>
public static class VJHookSystem
{
    /// <summary>Register default NPC relationship overrides for known entities (Phase 3)</summary>
    public static void SetupDefaultRelationships()
    {
        // Phase 3: register entity class → relationship class mappings
        // Example: citizen → CLASS_PLAYER_ALLY, combine → CLASS_COMBINE, etc.
    }
}
