using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase;

/// <summary>
/// Human NPC — ported from npc_vj_human_base/shared.lua.
/// Field defaults, constructor, and virtual callbacks. Behavioral logic lives in HumanNPC.Think.cs.
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

    // ═══ Shared Config — human_base/init.lua:14-153 ═══
    public float StartHealth { get; set; } = 50;
    public bool CanOpenDoors { get; set; } = true;
    public bool HasOnPlayerSight { get; set; }
    public int OnPlayerSightDispositionLevel { get; set; } = 1;
    public bool OnPlayerSightOnlyOnce { get; set; } = true;
    public (float a, float b) OnPlayerSightNextTime { get; set; } = (15f, 20f);
    public string DamageResponse { get; set; } = "true";
    public bool DamageAllyResponse { get; set; } = true;
    public (float a, float b) DamageAllyResponse_Cooldown { get; set; } = (9f, 12f);
    public bool CombatDamageResponse { get; set; } = true;
    public (float a, float b) CombatDamageResponse_CoverTime { get; set; } = (3f, 5f);
    public (float a, float b) CombatDamageResponse_Cooldown { get; set; } = (3f, 3.5f);
    public bool CallForHelp { get; set; } = true;
    public float CallForHelpDistance { get; set; } = 2000;
    public float CallForHelpCooldown { get; set; } = 4;
    public float CallForHelpAnimCooldown { get; set; } = 30;
    public bool CallForHelpAnimFaceEnemy { get; set; } = true;
    public bool CanInvestigate { get; set; } = true;
    public bool FollowPlayer { get; set; } = true;
    public bool CanReceiveOrders { get; set; } = true;
    public bool Passive_RunOnTouch { get; set; } = true;
    public bool Passive_AlliesRunOnDamage { get; set; } = true;
    public float DangerDetectionDistance { get; set; } = 400;
    public bool CanDetectDangers { get; set; } = true;
    public bool CanRedirectGrenades { get; set; } = true;
    public bool CanAlly { get; set; } = true;
    public float BecomeEnemyToPlayer { get; set; } // false=don't, number=threshold
    public bool AllowIgnition { get; set; } = true;
    public bool ForceDamageFromBosses { get; set; }
    public bool CanFlinch { get; set; }
    public int FlinchChance { get; set; } = 16;
    public float FlinchCooldown { get; set; } = 5;
    public bool HasDamageByPlayerSounds { get; set; }
    public float NextDamageByPlayerSoundT { get; set; }
    public int DamageByPlayerDispositionLevel { get; set; }
    public bool Weapon_Disabled { get; set; }
    public bool Weapon_IgnoreSpawnMenu { get; set; }
    public bool Weapon_CanMoveFire { get; set; } = true;
    public bool Weapon_CanReload { get; set; } = true;
    public bool Weapon_CanCrouchAttack { get; set; }
    public int Weapon_CrouchAttackChance { get; set; } = 4;
    public bool Weapon_FindCoverOnReload { get; set; } = true;
    public bool Weapon_Strafe { get; set; } = true;
    public (float a, float b) Weapon_StrafeCooldown { get; set; } = (3f, 6f);
    public float Weapon_Accuracy { get; set; } = 0.6f;
    public float Weapon_MaxDistance { get; set; } = 2500;
    public float Weapon_MinDistance { get; set; }
    public float Weapon_RetreatDistance { get; set; } = 200;
    public float Weapon_OcclusionDelay { get; set; } = 3f; // Phase 3: may be bool false to disable
    public float Weapon_OcclusionDelayMinDist { get; set; } = 500;
    public (float a, float b) Weapon_OcclusionDelayTime { get; set; } = (1.2f, 3.5f);
    public float Weapon_AimTurnDiff { get; set; } = 0.2f;
    public float Weapon_AimTurnDiff_Def { get; set; } = 0.95f;

    // ═══ Grenade Config — human_base/init.lua ═══
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

    // ═══ Weapon Inventory — human_base/init.lua:2167 ═══
    public Dictionary<string, object> WeaponInventory { get; set; } = new();
    public VJWepInventory WeaponInventoryStatus { get; set; } = VJWepInventory.None;
    public List<string> WeaponInventory_AntiArmorList { get; set; }
    public List<string> WeaponInventory_MeleeList { get; set; }
    public Dictionary<int, object> AnimationTranslations { get; set; } = new();

    // ═══ Weapon Config Extras ═══
    public bool Weapon_UnarmedBehavior { get; set; } = true;
    public bool Weapon_UnarmedBehavior_Active { get; set; }

    // ═══ Virtual Callbacks (stubs — override in derived types) ═══
    public virtual bool OnGrenadeAttack(string status, object customEnt, Vector3? landDir = null) => false;
    public virtual bool OnGrenadeAttackExecute(string status, object grenade, object customEnt, Vector3? landDir, Vector3? landingPos) => false;
    public virtual void OnWeaponChange(GameObject newWeapon, GameObject oldWeapon, bool invSwitch) { }
    public virtual bool OnWeaponCanFire() => true;
    public virtual void OnWeaponAttack() { }
    public virtual bool OnWeaponStrafe() => true;
    public virtual void OnWeaponReload() { }
}
