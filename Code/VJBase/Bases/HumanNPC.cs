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

    // ═══ Idle / Wander Config ═══
    public bool IdleAlwaysWander { get; set; }

    // ═══ Animation Config — human_base/init.lua ═══
    public Dictionary<int, object> AnimationTranslations { get; set; } = new();

    // ═══ Virtual Callbacks (stubs — override in derived types) ═══
    public virtual bool OnGrenadeAttack(string status, object customEnt, Vector3? landDir = null) => false;
    public virtual bool OnGrenadeAttackExecute(string status, object grenade, object customEnt, Vector3? landDir, Vector3? landingPos) => false;
    public virtual void OnWeaponChange(GameObject newWeapon, GameObject oldWeapon, bool invSwitch) { }
    public virtual bool OnWeaponCanFire() => true;
    public virtual void OnWeaponAttack() { }
    public virtual bool OnWeaponStrafe() => true;
    public virtual void OnWeaponReload() { }
}
