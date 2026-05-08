using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase;

/// <summary>
/// Human NPC — ported from npc_vj_human_base/init.lua (fields from lines 14-323) + shared.lua (client callbacks).
/// Field defaults, constructor, weapon inventory, and virtual callbacks. Behavioral logic lives in HumanNPC.Think.cs.
/// </summary>
public partial class HumanNPC : CreatureNPC
{
    // ═══ Identity ═══
    public bool IsVJBaseSNPC_Human { get; set; } = true;

    // ═══ Weapon Fields ═══
    public VJWepState WeaponState { get; set; } = VJWepState.Ready;
    public GameObject WeaponEntity { get; set; }
    public bool HasWeapon { get; set; }
    public bool AllowWeaponOcclusionDelay { get; set; }
    public float NextThrowGrenadeT { get; set; }

    // ═══ Attack Config (human-specific values set in constructor) ═══
    public bool DisableChasingEnemy { get; set; }
    public bool HasGrenadeAttack { get; set; }

    // ═══ Grenade Config — human_base/init.lua:14-323 ═══
    public List<string> GrenadeAttackEntity { get; set; }
    public List<string> GrenadeAttackModel { get; set; }
    public string GrenadeAttackAttachment { get; set; }
    public string GrenadeAttackBone { get; set; }
    public float GrenadeAttackFuseTime { get; set; } = 2f;
    public float GrenadeAttackMaxDistance { get; set; } = 1500;
    public List<string> AnimTbl_GrenadeAttack { get; set; }
    public Vector3? GrenadeAttack_LastLandDir { get; set; }

    // ═══ Constructor: human-specific defaults (override BaseNPC/CreatureNPC defaults) ═══
    public HumanNPC()
    {
        // Attack defaults
        MeleeAttackDistance = 50;
        MeleeAttackAngleRadius = 45;
        TimeUntilMeleeAttackDamage = 0.3f;
        NextAnyAttackTime_Melee = 1.5f;
        NextMeleeAttackTime = 0.8f;
        NextAnyAttackTime_Grenade = 3f;
        NextGrenadeAttackTime = 5f;

        // Sound defaults
        HasExtraMeleeAttackSounds = true;
        HasSuppressingSounds = true;
        HasWeaponReloadSounds = true;
        HasGrenadeAttackSounds = true;
        HasDangerSightSounds = true;
        IdleSoundChance = 3;
        NextSoundTime_Idle = (8f, 25f);
        NextSoundTime_Suppressing = (7f, 15f);
        FootstepSoundTimerWalk = 0.5f;
        FootstepSoundTimerRun = 0.25f;
        SoundTbl_FootStep = new() { "VJ.Footstep.Human" };
        SoundTbl_MeleeAttackExtra = new() { "Flesh.ImpactHard" };
        SoundTbl_MeleeAttackMiss = new() { "Zombie.AttackMiss" };
        SoundTbl_Impact = new() { "Flesh.BulletImpact" };
    }

    // ═══ Spawn Config — human_base/init.lua:14-323 ═══
    public List<string> Model { get; set; }
    public int StartHealth { get; set; } = 50;

    // ═══ Weapon Inventory Slots ═══
    public class WeaponSlots
    {
        public GameObject Primary { get; set; }
        public GameObject AntiArmor { get; set; }
        public GameObject Melee { get; set; }
    }
    public WeaponSlots WeaponInventory { get; set; } = new();
    public VJWepInventory WeaponInventoryStatus { get; set; } = VJWepInventory.None;
    public List<string> WeaponInventory_AntiArmorList { get; set; }
    public List<string> WeaponInventory_MeleeList { get; set; }

    // ═══ Weapon Config — human_base/init.lua:14-323 ═══
    public bool Weapon_Disabled { get; set; }
    public bool Weapon_IgnoreSpawnMenu { get; set; }
    public bool Weapon_CanMoveFire { get; set; }

    // ═══ Weapon Behavior Config (ported from init.lua:285-297) ═══
    public bool Weapon_UnarmedBehavior { get; set; } = true;
    public bool Weapon_UnarmedBehavior_Active { get; set; }
    public bool Weapon_Strafe { get; set; } = true;
    public (float a, float b) Weapon_StrafeCooldown { get; set; } = (3f, 6f);
    public bool Weapon_OcclusionDelay { get; set; } = true;
    public (float a, float b) Weapon_OcclusionDelayTime { get; set; } = (3f, 5f);
    public float Weapon_OcclusionDelayMinDist { get; set; } = 100f;
    public float Weapon_MaxDistance { get; set; } = 3000f;
    public float Weapon_MinDistance { get; set; } = 10f;
    public float Weapon_RetreatDistance { get; set; } = 150f;
    public float? Weapon_AimTurnDiff { get; set; } // null = disabled (Lua: false)
    public float Weapon_AimTurnDiff_Def { get; set; } = 1f;

    // ═══ Animation Table Config (ported from init.lua:153,301-305) ═══
    public List<string> AnimTbl_MoveToCover { get; set; } = new();
    public List<string> AnimTbl_WeaponAttack { get; set; } = new();
    public List<string> AnimTbl_WeaponAttackCrouch { get; set; } = new();
    public List<string> AnimTbl_WeaponAim { get; set; } = new();

    // ═══ Weapon Runtime State (ported from init.lua:1649-1662) ═══
    public float WeaponLastShotTime { get; set; }
    public string WeaponAttackAnim { get; set; }
    public float NextWeaponAttackT { get; set; }
    public float NextWeaponAttackT_Base { get; set; }
    public float NextWeaponStrafeT { get; set; }
    public float NextMoveOnGunCoveredT { get; set; }
    public float NextMeleeWeaponAttackT { get; set; }
    public float NextDangerDetectionT { get; set; }

    // ═══ Animation Config (ported from init.lua:130,303) ═══
    public bool HasPoseParameterLooking { get; set; } = true;
    public bool Weapon_CanCrouchAttack { get; set; } = true;
    public int Weapon_CrouchAttackChance { get; set; } = 2;

    // ═══ Damage Response Config (ported from init.lua:4020-4135) ═══
    public int BecomeEnemyToPlayer { get; set; }
    public int DamageByPlayerDispositionLevel { get; set; } = 1;
    public object DamageResponse { get; set; } = true; // true / "OnlySearch" / "OnlyMove"
    public bool CombatDamageResponse { get; set; } = true;
    public float NextCombatDamageResponseT { get; set; }
    public (float a, float b) CombatDamageResponse_CoverTime { get; set; } = (3f, 5f);
    public (float a, float b) CombatDamageResponse_Cooldown { get; set; } = (3f, 3.5f);

    // ═══ Ally Damage Response (ported from init.lua:4082-4100) ═══
    public bool DamageAllyResponse { get; set; } = true;
    public (float a, float b) DamageAllyResponse_Cooldown { get; set; } = (9f, 12f);
    public List<string> AnimTbl_DamageAllyResponse { get; set; } = new();
    public List<string> AnimTbl_TakingCover { get; set; } = new();
    public bool Passive_AlliesRunOnDamage { get; set; } = true;

    // ═══ Death / Weapon Drop (ported from human_base init.lua:233) ═══
    public bool DropWeaponOnDeath { get; set; } = true;

    // ═══ Misc Damage Fields (ported from OnTakeDamage) ═══
    public bool CanEat { get; set; }
    public object EatingData { get; set; }
    public bool CanChatMessage { get; set; }

    public bool CanDetectDangers { get; set; } = true;
    public float DangerDetectionDistance { get; set; } = 400f;
    public bool CanRedirectGrenades { get; set; } = true;

    // ═══ Idle / Wander Config ═══
    public bool IdleAlwaysWander { get; set; }

    // ═══ Animation Config — human_base/init.lua ═══
    public Dictionary<int, object> AnimationTranslations { get; set; } = new();

    // ═══ Virtual Callbacks (stubs — override in derived types) ═══
    public virtual bool OnGrenadeAttack(string status, object customEnt, string landDir = null) => false;
    public virtual bool OnGrenadeAttackExecute(string status, object grenade, object customEnt, Vector3? landDir, Vector3? landingPos) => false;
    public virtual void OnWeaponChange(GameObject newWeapon, GameObject oldWeapon, bool invSwitch) { }
    public virtual bool OnWeaponCanFire() => true;
    public virtual void OnWeaponAttack() { }
    public virtual bool OnWeaponStrafe() => true;
    public virtual void OnWeaponReload() { }

    // ═══ Damage Callbacks ═══
    public virtual void OnDamaged(DamageInfo dmginfo, int hitgroup, string status) { }
    public virtual void OnBleed(DamageInfo dmginfo, int hitgroup) { }
    public virtual void OnSetEnemyFromDamage(DamageInfo dmginfo, int hitgroup) { }
    public virtual void OnBecomeEnemyToPlayer(DamageInfo dmginfo, int hitgroup) { }
    public virtual void ResetFollowBehavior() { }
    public virtual void ResetEatingBehavior(string reason) { }

    // ═══ Death Weapon Drop (human_base init.lua:4484-4513) ═══
    public virtual void OnDeathWeaponDrop(DamageInfo dmginfo, int hitgroup, GameObject wep) { }
    public virtual void DropWeapon(GameObject wep, object unused, Vector3 velocity) { }
}
