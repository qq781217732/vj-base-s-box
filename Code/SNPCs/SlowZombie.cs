using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase.SNPCs;

/// <summary>
/// Slow Zombie SNPC — ported from npc_vj_zss_slow/init.lua.
/// Inherits from CreatureNPC. Minimal test-case for VJBase framework validation.
/// </summary>
[Group( "VJBase" )]
[Title( "Slow Zombie" )]
public partial class SlowZombie : CreatureNPC
{
	// ═══ Model list (picked randomly in OnStart) ═══
	public List<string> ModelList { get; set; } = new()
	{
		"models/vj_zombies/slow1.mdl",  "models/vj_zombies/slow2.mdl",
		"models/vj_zombies/slow3.mdl",  "models/vj_zombies/slow4.mdl",
		"models/vj_zombies/slow5.mdl",  "models/vj_zombies/slow6.mdl",
		"models/vj_zombies/slow7.mdl",  "models/vj_zombies/slow8.mdl",
		"models/vj_zombies/slow9.mdl",  "models/vj_zombies/slow10.mdl",
		"models/vj_zombies/slow11.mdl", "models/vj_zombies/slow12.mdl",
	};

	// ═══ Foot scuff sounds (used by scuff animation event) ═══
	private static readonly List<string> SdFootScuff = new()
	{
		"npc/zombie/foot_slide1.wav", "npc/zombie/foot_slide2.wav", "npc/zombie/foot_slide3.wav"
	};

	public SlowZombie()
	{
		// ── Identity ──
		StartHealth = 100;
		VJ_NPC_Class = new() { "CLASS_ZOMBIE" };
		BloodColor = VJBloodColor.Red;
		VJ_ID_Undead = true;

		// ── Melee Attack ──
		HasMeleeAttack = true;
		AnimTbl_MeleeAttack = new() { "vjseq_attacka", "vjseq_attackb", "vjseq_attackc", "vjseq_attackd", "vjseq_attacke", "vjseq_attackf" };
		MeleeAttackDistance = 32;
		MeleeAttackDamageDistance = 65;
		// TimeUntilMeleeAttackDamage stays 0 (event-based, not timer) — BaseNPC default
		MeleeAttackPlayerSpeed = true;
		MeleeAttackBleedEnemy = true;
		DisableFootStepSoundTimer = true; // played via animation events

		// ── Flinch ──
		CanFlinch = true;
		AnimTbl_Flinch = new() { "ACT_FLINCH_PHYSICS" };
		FlinchHitGroupMap = new()
		{
			new() { HitGroup = 1, Animation = "vjges_flinch_head" },        // HITGROUP_HEAD
			new() { HitGroup = 2, Animation = "vjges_flinch_chest" },       // HITGROUP_CHEST
			new() { HitGroup = 4, Animation = "vjges_flinch_leftArm" },     // HITGROUP_LEFTARM
			new() { HitGroup = 5, Animation = "vjges_flinch_rightArm" },    // HITGROUP_RIGHTARM
			new() { HitGroup = 6, Animation = "ACT_FLINCH_LEFTLEG" },       // HITGROUP_LEFTLEG
			new() { HitGroup = 7, Animation = "ACT_FLINCH_RIGHTLEG" },      // HITGROUP_RIGHTLEG
		};

		// ── Sound Tables ──
		SoundTbl_FootStep = new() { "npc/zombie/foot1.wav", "npc/zombie/foot2.wav", "npc/zombie/foot3.wav" };
		SoundTbl_Idle = new() { "vj_zombies/slow/zombie_idle1.wav", "vj_zombies/slow/zombie_idle2.wav", "vj_zombies/slow/zombie_idle3.wav", "vj_zombies/slow/zombie_idle4.wav", "vj_zombies/slow/zombie_idle5.wav", "vj_zombies/slow/zombie_idle6.wav" };
		SoundTbl_Alert = new() { "vj_zombies/slow/zombie_alert1.wav", "vj_zombies/slow/zombie_alert2.wav", "vj_zombies/slow/zombie_alert3.wav", "vj_zombies/slow/zombie_alert4.wav" };
		SoundTbl_MeleeAttack = new() { "vj_zombies/slow/zombie_attack_1.wav", "vj_zombies/slow/zombie_attack_2.wav", "vj_zombies/slow/zombie_attack_3.wav", "vj_zombies/slow/zombie_attack_4.wav", "vj_zombies/slow/zombie_attack_5.wav", "vj_zombies/slow/zombie_attack_6.wav" };
		SoundTbl_MeleeAttackMiss = new() { "vj_zombies/slow/miss1.wav", "vj_zombies/slow/miss2.wav", "vj_zombies/slow/miss3.wav", "vj_zombies/slow/miss4.wav" };
		SoundTbl_Pain = new() { "vj_zombies/slow/zombie_pain1.wav", "vj_zombies/slow/zombie_pain2.wav", "vj_zombies/slow/zombie_pain3.wav", "vj_zombies/slow/zombie_pain4.wav", "vj_zombies/slow/zombie_pain5.wav", "vj_zombies/slow/zombie_pain6.wav", "vj_zombies/slow/zombie_pain7.wav", "vj_zombies/slow/zombie_pain8.wav" };
		SoundTbl_Death = new() { "vj_zombies/slow/zombie_die1.wav", "vj_zombies/slow/zombie_die2.wav", "vj_zombies/slow/zombie_die3.wav", "vj_zombies/slow/zombie_die4.wav", "vj_zombies/slow/zombie_die5.wav", "vj_zombies/slow/zombie_die6.wav" };
	}

	// ═══ Lifecycle ═══

	protected override void OnStart()
	{
		// Ensure NavMeshAgent exists for Ground movement
		var agent = Components.Get<NavMeshAgent>();
		if ( agent is null )
			agent = Components.Create<NavMeshAgent>();
		agent.UpdatePosition = true;
		agent.UpdateRotation = true;
		agent.MaxSpeed = 150f; // Slow zombie default walking speed

		// Ensure SkinnedModelRenderer
		var renderer = Components.Get<SkinnedModelRenderer>();
		if ( renderer is null )
			renderer = Components.Create<SkinnedModelRenderer>();

		// Pick random model
		var modelPath = VJUtility.PICK( ModelList );
		if ( !string.IsNullOrEmpty( modelPath ) )
			renderer.Model = Model.Load( modelPath );

		// Setup hitboxes for bullet traces
		var hitboxes = Components.Get<ModelHitboxes>() ?? Components.Create<ModelHitboxes>();
		if ( hitboxes.Renderer is null )
			hitboxes.Renderer = renderer;

		// Initialize movement system
		MovementType = VJMoveType.Ground;
		DoChangeMovementType( MovementType );

		// Health
		var hp = ScaleByDifficulty( StartHealth );
		CurrentHealth = hp;
		StartHealth = (int)hp;

		// Senses
		Senses = new AISenses { Outer = this, LookDist = SightDistance };

		EFL_NO_DISSOLVE = true;

		base.OnStart();
	}

	// ═══ TranslateActivity — fire state override ═══

	public override Activity TranslateActivity( Activity act )
	{
		if ( IsOnFire() )
		{
			if ( act == Activity.Idle )
				return Activity.IdleOnFire;
			else if ( act == Activity.Run || act == Activity.Walk )
				return Activity.WalkOnFire;
		}
		return base.TranslateActivity( act );
	}

	// ═══ OnAnimEvent — animation-driven footstep + melee ═══

	public override void OnAnimEvent( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		switch ( name )
		{
			case "step":
				PlayFootstepSound();
				break;

			case "scuff":
				PlayFootstepSound( VJUtility.PICK( SdFootScuff ) );
				break;

			case "melee":
				MeleeAttackDamage = 20;
				ExecuteMeleeAttack( false );
				break;

			case "melee_heavy":
				MeleeAttackDamage = 30;
				ExecuteMeleeAttack( false );
				break;
		}
	}
}
