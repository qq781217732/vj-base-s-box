using Sandbox;

namespace VJBase;

/// <summary>
/// Lightweight component that holds VJ_ID_* / VJ_ST_* flags for non-NPC entities
/// (grenades, projectiles, bombs, props). NPCs carry these flags directly on BaseNPC.
/// </summary>
public class VJEntityFlags : Component
{
    // ═══ Identity flags (VJ_ID_*) ═══
    public bool VJ_ID_Danger { get; set; }
    public bool VJ_ID_Grenade { get; set; }
    public bool VJ_ID_Grabbable { get; set; }
    public bool VJ_ID_Living { get; set; }
    public bool VJ_ID_Attackable { get; set; }
    public bool VJ_ID_Destructible { get; set; }
    public bool VJ_ID_Boss { get; set; }
    public bool VJ_ID_Vehicle { get; set; }

    // ═══ State flags (VJ_ST_*) ═══
    public bool VJ_ST_Grabbed { get; set; }
    public bool VJ_ST_Eating { get; set; }
    public bool VJ_ST_Healing { get; set; }

    // ═══ Source engine flags (FL_* / EFL_*) ═══
    public bool FL_NOTARGET { get; set; }
    public bool FL_DISSOLVING { get; set; }
    public bool FL_OBJECT { get; set; }
    public bool EFL_NO_DISSOLVE { get; set; }
}
