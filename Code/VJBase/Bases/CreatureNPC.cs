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

	public CreatureNPC()
	{
		// Creature-specific sound defaults (npc_vj_creature_base/init.lua)
		SoundTbl_MeleeAttackExtra = new() { "Zombie.AttackHit" };
		SoundTbl_Impact = new() { "VJ.Impact.Flesh_Alien" };
		DeathAllyResponse = true; // creature default (human overrides in constructor)
	}

	public virtual void SetAutomaticFrameAdvance(bool val)
	{
		AutomaticFrameAdvance = val;
	}

	public virtual bool? MatFootStepQCEvent(object data)
	{
		return false;
	}
}
