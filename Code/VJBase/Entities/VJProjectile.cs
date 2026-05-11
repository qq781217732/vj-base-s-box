using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ Projectile base — ported from obj_vj_projectile_base/init.lua.
/// Supports Linear, Gravity, Prop, and Seeker (combine ball / homing) projectile types.
/// </summary>
public partial class VJProjectile : Component
{
    // ═══ Core ═══
    public VJProjType ProjectileType { get; set; } = VJProjType.Linear;
    public VJProjCollision CollisionBehavior { get; set; } = VJProjCollision.Remove;
    public bool CollisionFilter { get; set; } = true;
    public object CollisionDecal { get; set; }
    public float RemoveDelay { get; set; }
    public float Lifetime { get; set; } // If > 0, auto-destroy after N seconds

    // ═══ Damage ═══
    public bool DoesRadiusDamage { get; set; }
    public float RadiusDamageRadius { get; set; } = 250;
    public bool RadiusDamageUseRealisticRadius { get; set; } = true;
    public float RadiusDamage { get; set; } = 30;
    public uint RadiusDamageType { get; set; }
    public float RadiusDamageForce { get; set; }
    public bool RadiusDamageDisableVisibilityCheck { get; set; }
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
    private float _nextIdleSoundT;

    // ═══ Seeker ═══ (obj_vj_combineball)
    public float SeekRange { get; set; } = 1024f;
    public float SeekDotMin { get; set; } = 0.75f; // Min dot product between forward and target direction
    public float SeekBounceVelocityFactor { get; set; } = 1f; // Multiplier on redirect velocity
    public GameObject SeekerOwner { get; set; }

    // ═══ Internal ═══
    private float _spawnTime;
    private bool _destroyed;

    // ═══ Lifecycle ═══

    protected override void OnStart()
    {
        _spawnTime = (float)Time.Now;
        if (Lifetime > 0)
            _ = Task.Delay((int)(Lifetime * 1000f)).ContinueWith(_ =>
            {
                if (GameObject.IsValid() && !_destroyed) DestroyProjectile(null, null);
            });

        if (HasIdleSounds && SoundTbl_Idle?.Count > 0)
            _nextIdleSoundT = _spawnTime + NextSoundTime_Idle;
    }

    protected override void OnUpdate()
    {
        if (_destroyed) return;

        var curTime = (float)Time.Now;

        // Idle sounds
        if (HasIdleSounds && curTime >= _nextIdleSoundT && SoundTbl_Idle?.Count > 0)
        {
            var picked = VJUtility.PICK(SoundTbl_Idle);
            if (picked != null) Sound.Play(picked, GameObject.WorldPosition);
            _nextIdleSoundT = curTime + NextSoundTime_Idle;
        }

        // Seeker continuous homing (adjust velocity toward target each frame)
        if (ProjectileType == VJProjType.Seeker)
        {
            SeekTarget();
        }
    }

    /// <summary>
    /// Find closest valid target and redirect velocity toward it.
    /// Called on bounce (combine ball) or continuously (homing missile).
    /// </summary>
    public bool SeekTarget()
    {
        var rb = Components.Get<Rigidbody>();
        if (rb == null) return false;

        var myPos = GameObject.WorldPosition;
        var owner = SeekerOwner;
        var ownerIsVJ = owner?.Components.Get<BaseNPC>() is { } ownerNpc;

        // Find closest valid target within SeekRange
        GameObject bestTarget = null;
        float bestDist = SeekRange;
        var myForward = GameObject.WorldRotation.Forward;

        foreach (var ent in Scene.GetAllComponents<BaseNPC>())
        {
            var go = ent.GameObject;
            if (!go.IsValid() || go == owner) continue;
            if (!ent.VJ_ID_Living || ent.Dead) continue;

            // Relationship check for VJ owners
            if (ownerIsVJ && ownerNpc.CheckRelationship(go) == 2 /* D_HT */) continue;

            var dist = go.WorldPosition.Distance(myPos);
            if (dist < 20f || dist >= bestDist) continue;

            // FOV check: is target within our forward cone?
            var toTarget = ((go.WorldPosition + ent.OBBCenter()) - myPos).Normal;
            if (myForward.Dot(toTarget) < SeekDotMin) continue;

            bestDist = dist;
            bestTarget = go;
        }

        if (bestTarget != null)
        {
            var targetCenter = bestTarget.WorldPosition
                + (bestTarget.Components.Get<BaseNPC>()?.OBBCenter() ?? Vector3.Zero);
            var toTarget = (targetCenter - myPos).Normal;
            var currentSpeed = rb.Velocity.Length;

            rb.Velocity = toTarget * Math.Max(currentSpeed, rb.Velocity.Length) * SeekBounceVelocityFactor;
            GameObject.WorldRotation = Rotation.LookAt(toTarget);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Handle collision — damage + bounce/remove.
    /// Called externally from Rigidbody collision handler or weapon system.
    /// </summary>
    public virtual void OnCollide(GameObject other)
    {
        if (_destroyed) return;

        bool hitEntity = other?.Components.Get<ModelRenderer>() != null || other?.Components.Get<PlayerBase>() != null;

        if (hitEntity)
        {
            // Direct damage
            if (DirectDamage > 0)
            {
                var dmgInfo = new DamageInfo
                {
                    Damage = DirectDamage,
                    Attacker = SeekerOwner ?? GameObject,
                    Weapon = other,
                    Position = other.WorldPosition
                };
                dmgInfo.Tags.Add(VJDamageTags.Dissolve);

                foreach (var d in other.Components.GetAll<IDamageable>())
                    d.OnDamage(dmgInfo);
            }

            if (HasOnCollideSounds && SoundTbl_OnCollide?.Count > 0)
            {
                var picked = VJUtility.PICK(SoundTbl_OnCollide);
                if (picked != null) Sound.Play(picked, other.WorldPosition);
            }
        }

        // Decide persistence
        if (CollisionBehavior == VJProjCollision.Remove)
        {
            DestroyProjectile(other, hitEntity ? other.WorldPosition : (Vector3?)null);
        }
        else if (CollisionBehavior == VJProjCollision.Persist && !hitEntity)
        {
            // Bounce: seeker redirects after bouncing off world surfaces
            if (ProjectileType == VJProjType.Seeker)
                SeekTarget();
        }
    }

    /// <summary>
    /// Destroy projectile — apply radius damage + effects.
    /// </summary>
    public void DestroyProjectile(GameObject hitEnt, Vector3? hitPos)
    {
        if (_destroyed) return;
        _destroyed = true;

        var pos = hitPos ?? GameObject.WorldPosition;

        // Radius damage
        if (DoesRadiusDamage && RadiusDamage > 0)
        {
            foreach (var ent in Scene.GetAllComponents<BaseNPC>())
            {
                var go = ent.GameObject;
                if (!go.IsValid() || go == SeekerOwner) continue;
                var dist = go.WorldPosition.Distance(pos);
                if (dist > RadiusDamageRadius) continue;
                var frac = RadiusDamageUseRealisticRadius ? (1f - dist / RadiusDamageRadius) : 1f;
                var dmg = new DamageInfo { Damage = RadiusDamage * frac, Attacker = SeekerOwner ?? GameObject, Position = pos };
                foreach (var d in go.Components.GetAll<IDamageable>())
                    d.OnDamage(dmg);
            }
        }

        // Radius force
        if (RadiusDamageForce > 0)
        {
            foreach (var ent in Scene.GetAllComponents<BaseNPC>())
            {
                var go = ent.GameObject;
                if (!go.IsValid() || (SeekerOwner != null && go == SeekerOwner)) continue;
                var dist = go.WorldPosition.Distance(pos);
                if (dist > RadiusDamageRadius || dist < 1f) continue;
                var rb = go.Components.Get<Rigidbody>();
                if (rb != null)
                {
                    var dir = (go.WorldPosition + (ent.OBBCenter()) - pos).Normal;
                    rb.ApplyForce(dir * RadiusDamageForce * 100f);
                }
            }
        }

        // Remove sounds
        if (HasOnRemoveSounds && SoundTbl_OnRemove?.Count > 0)
        {
            var picked = VJUtility.PICK(SoundTbl_OnRemove);
            if (picked != null) Sound.Play(picked, pos);
        }

        if (GameObject.IsValid())
            GameObject.Destroy();
    }
}
