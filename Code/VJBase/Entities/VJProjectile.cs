using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ Projectile base — ported from obj_vj_projectile_base/init.lua.
/// All VJ projectiles (rockets, grenades, bolts) extend this.
/// </summary>
public partial class VJProjectile : Component
{
    // ═══ Core ═══
    public VJProjType ProjectileType { get; set; } = VJProjType.Linear;
    public VJProjCollision CollisionBehavior { get; set; } = VJProjCollision.Remove;
    public bool CollisionFilter { get; set; } = true;
    public object CollisionDecal { get; set; }
    public float RemoveDelay { get; set; }

    // ═══ Damage ═══
    public bool DoesRadiusDamage { get; set; }
    public float RadiusDamageRadius { get; set; } = 250;
    public bool RadiusDamageUseRealisticRadius { get; set; } = true;
    public float RadiusDamage { get; set; } = 30;
    public uint RadiusDamageType { get; set; }
    public bool DoesDirectDamage { get; set; }
    public float DirectDamage { get; set; } = 30;
    public uint DirectDamageType { get; set; }

    // ═══ Sound ═══
    public bool HasStartupSounds { get; set; } = true;
    public bool HasIdleSounds { get; set; } = true;
    public bool HasOnCollideSounds { get; set; } = true;
    public bool HasOnRemoveSounds { get; set; } = true;
    public List<string> SoundTbl_Startup { get; set; }
    public List<string> SoundTbl_Idle { get; set; }
    public List<string> SoundTbl_OnCollide { get; set; }
    public List<string> SoundTbl_OnRemove { get; set; }
    public float NextSoundTime_Idle { get; set; } = 0.3f;

    // ═══ Lifetime ═══
    protected virtual void OnCollide(GameObject other)
    {
        if (CollisionBehavior == VJProjCollision.Remove)
            GameObject.Destroy();
    }

    protected virtual void OnRemove()
    {
        // Play remove sound
    }
}
