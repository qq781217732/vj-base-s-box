using Sandbox;

namespace VJBase;

/// <summary>
/// Creature NPC — ported from npc_vj_creature_base/shared.lua.
/// Default field values and creature-specific identity flags.
/// </summary>
public partial class CreatureNPC : BaseNPC
{
	// ═══ Entity Identity ═══
	public bool IsVJBaseSNPC { get; set; } = true;
	public bool IsVJBaseSNPC_Creature { get; set; } = true;
	public bool AutomaticFrameAdvance { get; set; }
}
