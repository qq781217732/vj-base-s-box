using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase;

/// <summary>
/// Default VJ weapon component. Implements IVJBaseWeapon with configurable properties.
/// Phase 1: inventory/lifecycle. Phase 2: NPC autonomous fire logic (NPC_Think, NPCShoot_Primary, PrimaryAttack).
/// </summary>
public partial class VJBaseWeapon : Component, IVJBaseWeapon
{
    // ═══ Phase 1: Identity ═══
    [Property] public bool IsVJBaseWeapon { get; set; } = true;
    [Property] public bool IsMeleeWeapon { get; set; }
    [Property] public string HoldType { get; set; } = "pistol";

    public GameObject WeaponOwner { get; private set; }

    // ═══ Phase 1: Ammo ═══
    [Property] public int Clip1 { get; set; } = 30;
    [Property] public int MaxClip1 { get; set; } = 30;

    public Action OnReloadAction { get; set; }

    // ═══ Phase 2: NPC Firing Config (weapon_vj_base/shared.lua:35-63) ═══
    [Property] public float NPC_NextPrimaryFire { get; set; } = 0.11f;
    [Property] public float NPC_TimeUntilFire { get; set; }
    [Property] public List<float> NPC_TimeUntilFireExtraTimers { get; set; } = new();
    [Property] public float NPC_CustomSpread { get; set; } = 1f;
    [Property] public string NPC_BulletSpawnAttachment { get; set; } = "";
    [Property] public bool NPC_StandingOnly { get; set; }
    [Property] public float NPC_FiringDistanceScale { get; set; } = 1f;
    [Property] public float NPC_FiringDistanceMax { get; set; } = 100000f;
    [Property] public float NPC_FiringCone { get; set; } = 0.9f;

    // ═══ Phase 2: Reload Sound ═══
    [Property] public bool NPC_HasReloadSound { get; set; } = true;
    [Property] public string NPC_ReloadSound { get; set; }
    [Property] public float NPC_ReloadSoundLevel { get; set; } = 60f;

    // ═══ Phase 2: Before-Fire Sound ═══
    [Property] public string NPC_BeforeFireSound { get; set; }
    [Property] public float NPC_BeforeFireSoundLevel { get; set; } = 70f;
    [Property] public float NPC_BeforeFireSoundPitchA { get; set; } = 90f;
    [Property] public float NPC_BeforeFireSoundPitchB { get; set; } = 100f;
    public (float a, float b) NPC_BeforeFireSoundPitch => (NPC_BeforeFireSoundPitchA, NPC_BeforeFireSoundPitchB);

    // ═══ Phase 2: Extra Fire Sound ═══
    [Property] public string NPC_ExtraFireSound { get; set; }
    [Property] public float NPC_ExtraFireSoundTime { get; set; } = 0.4f;
    [Property] public float NPC_ExtraFireSoundLevel { get; set; } = 70f;
    [Property] public float NPC_ExtraFireSoundPitchA { get; set; } = 90f;
    [Property] public float NPC_ExtraFireSoundPitchB { get; set; } = 100f;

    // ═══ Phase 2: Secondary Fire ═══
    [Property] public bool NPC_HasSecondaryFire { get; set; }
    [Property] public string NPC_SecondaryFireEnt { get; set; } = "obj_vj_grenade_rifle";
    [Property] public int NPC_SecondaryFireChance { get; set; } = 3;
    [Property] public float NPC_SecondaryFireNextA { get; set; } = 12f;
    [Property] public float NPC_SecondaryFireNextB { get; set; } = 15f;
    [Property] public float NPC_SecondaryFireDistance { get; set; } = 1000f;
    [Property] public string NPC_SecondaryFireSound { get; set; }
    [Property] public float NPC_SecondaryFireSoundLevel { get; set; } = 90f;

    // ═══ Phase 2: Primary Attack Config (weapon_vj_base/shared.lua Primary table) ═══
    [Property] public float Primary_Damage { get; set; } = 10f;
    [Property] public float Primary_Delay { get; set; } = 0.11f;
    [Property] public int Primary_NumberOfShots { get; set; } = 1;
    [Property] public float Primary_Force { get; set; } = 2f;
    [Property] public int Primary_TakeAmmo { get; set; } = 1;
    [Property] public float MeleeWeaponDistance { get; set; } = 75f;

    // ═══ Phase 2: Runtime State ═══
    public float NPC_NextPrimaryFireT { get; set; }
    public float NPC_NextDrySoundT { get; set; }
    public float NPC_SecondaryFireNextT { get; set; }
    public float NPC_DelayedFireTime { get; set; }

    // ═══ Phase 1: Lifecycle ═══
    public virtual void Equip(GameObject owner)
    {
        WeaponOwner = owner;
    }

    public virtual void Unequip()
    {
        WeaponOwner = null;
    }

    public int GetClip1() => Clip1;
    public int GetMaxClip1() => MaxClip1;
    public void SetClip1(int amount) => Clip1 = Math.Clamp(amount, 0, MaxClip1);

    public virtual void NPC_Reload()
    {
        OnReloadAction?.Invoke();
    }

    // ═══ Phase 2: Per-frame auto-fire (called from HumanNPC.Think or Component Update) ═══
    /// <summary>
    /// NPC_Think — weapon_vj_base/shared.lua:534-545.
    /// Called every frame when equipped by an NPC. Checks fire conditions and auto-fires.
    /// </summary>
    public virtual void NPC_Think()
    {
        if (!GameObject.IsValid()) return;
        var owner = WeaponOwner;
        if (!owner.IsValid()) return;

        var npc = owner.Components.Get<BaseNPC>();
        if (npc == null) return;

        // Only fire if this is the owner's active weapon
        var activeWep = npc.GetActiveWeapon();
        if (activeWep != GameObject) return;

        // Non-melee weapons auto-fire on timer
        if (!IsMeleeWeapon && NPC_NextPrimaryFire >= 0 && Time.Now > NPC_NextPrimaryFireT && NPC_CanFire(npc))
        {
            // lua:593 — weapon_vj_base/shared.lua NPCShoot_Primary schedules PrimaryAttack
            if (NPC_DelayedFireTime > 0 && Time.Now < NPC_DelayedFireTime) return;
            NPCShoot_Primary();
        }

        // Delayed fire (from NPC_TimeUntilFire > 0)
        if (NPC_DelayedFireTime > 0 && Time.Now > NPC_DelayedFireTime)
        {
            NPC_DelayedFireTime = 0;
            DoPrimaryFire();
        }
    }

    // ═══ Phase 2: Firing condition check (weapon_vj_base/shared.lua:548-591) ═══
    /// <summary>
    /// NPC_CanFire — checks standing-only, CanFireWeapon, attack state, ammo, firing cone.
    /// </summary>
    public virtual bool NPC_CanFire(BaseNPC npc = null)
    {
        var owner = WeaponOwner;
        if (!owner.IsValid()) return false;

        npc ??= owner.Components.Get<BaseNPC>();
        if (npc == null) return false;

        var human = npc as HumanNPC;
        var ene = npc.GetEnemy();
        bool isVJHuman = human != null;

        // Standing-only check
        if (NPC_StandingOnly && npc.IsMoving())
            return false;

        // Human: check CanFireWeapon
        if (isVJHuman && ene.IsValid() && !human.CanFireWeapon(true, true))
            return false;

        // Attack state check
        if (isVJHuman)
        {
            bool inFireState = human.WeaponAttackState == VJWepAttackState.Fire
                || (human.WeaponAttackState == VJWepAttackState.FireStand /* && VJ.IsCurrentAnim — Phase 3 */);
            if (!inFireState) return false;
        }
        // Non-VJ-human: skip attack state check (lua:557 — (!isVJHuman) → bypass animation guard)

        if (IsMeleeWeapon) return true;

        // Ammo check (humans only)
        if (isVJHuman && human.Weapon_CanReload && GetClip1() <= 0)
        {
            if (NPC_NextPrimaryFire >= 0)
                NPC_NextDrySoundT = Time.Now + NPC_NextPrimaryFire;
            // Dry fire sound — Phase 3 sound system
            return false;
        }

        // Firing cone check
        if (ene.IsValid())
        {
            var spawnPos = GameObject.WorldPosition;
            // SKIP: GetAimPosition — Phase 3 aim system; use enemy WorldPosition for now
            var aimPos = ene.WorldPosition;
            var aimDir = (aimPos - spawnPos).Normal;
            var sightDir = owner.WorldRotation.Forward;
            aimDir = aimDir.WithZ(0).Normal;
            sightDir = sightDir.WithZ(0).Normal;
            float dot = Vector3.Dot(sightDir, aimDir);
            return dot > NPC_FiringCone;
        }

        // lua:590 — reached end without entering any return branch → false
        return false;
    }

    // ═══ Phase 2: Execute primary fire (weapon_vj_base/shared.lua:593-650) ═══
    /// <summary>
    /// NPCShoot_Primary — handles secondary fire chance, then schedules PrimaryAttack with TimeUntilFire delay.
    /// Called either from SelectSchedule (melee) or NPC_Think auto-fire timer (ranged).
    /// </summary>
    public virtual void NPCShoot_Primary()
    {
        var owner = WeaponOwner;
        if (!owner.IsValid()) return;

        var npc = owner.Components.Get<BaseNPC>();
        if (npc == null) return;

        var ene = npc.GetEnemy();
        if (!npc.VJ_IsBeingControlled && (!ene.IsValid() /* || !Visible — Phase 3 */)) return;

        // SKIP: UpdatePoseParamTracking — Phase 3 animation

        // Secondary fire chance
        if (NPC_HasSecondaryFire)
        {
            var human = npc as HumanNPC;
            if (human != null && human.Weapon_CanSecondaryFire && Time.Now > NPC_SecondaryFireNextT
                && ene.IsValid() && ene.WorldPosition.Distance(owner.WorldPosition) <= NPC_SecondaryFireDistance)
            {
                if (Game.Random.Next(1, NPC_SecondaryFireChance + 1) == 1)
                {
                    NPC_SecondaryFireNextT = Time.Now + Game.Random.Float(NPC_SecondaryFireNextA, NPC_SecondaryFireNextB);
                    // Secondary fire animation + delayed spawn — Phase 3 animation + entity system
                    // SKIP: lua:605-621 — secondary fire animation, timer, NPC_SecondaryFire
                    return;
                }
                else
                {
                    NPC_SecondaryFireNextT = Time.Now + Game.Random.Float(NPC_SecondaryFireNextA, NPC_SecondaryFireNextB);
                }
            }
        }

        // Primary fire — schedule with TimeUntilFire delay (Lua uses timer.Simple)
        float delay = NPC_TimeUntilFire;
        if (delay <= 0)
        {
            DoPrimaryFire();
        }
        else
        {
            // lua:629 — timer.Simple(self.NPC_TimeUntilFire, function() ... PrimaryAttack ... end)
            NPC_DelayedFireTime = Time.Now + delay;
        }
    }

    /// <summary>
    /// Execute the actual primary fire (bullet trace or melee sweep).
    /// </summary>
    protected virtual void DoPrimaryFire()
    {
        var owner = WeaponOwner;
        if (!owner.IsValid()) return;

        var npc = owner.Components.Get<BaseNPC>();
        if (npc == null) return;

        float curTime = Time.Now;
        var ene = npc.GetEnemy();

        // Only fire if timer allows (Lua: NPC_NextPrimaryFire is truthy for 0, false disables)
        if (NPC_NextPrimaryFire >= 0 && curTime <= NPC_NextPrimaryFireT) return;

        // Check NPC_CanFire again for safety
        if (!NPC_CanFire(npc)) return;

        PrimaryAttack(npc, ene);

        var human = npc as HumanNPC;
        if (human != null)
            human.WeaponLastShotTime = curTime;

        // Set next fire timer (Lua: if NPC_NextPrimaryFire != false then)
        if (NPC_NextPrimaryFire >= 0)
        {
            NPC_NextPrimaryFireT = curTime + NPC_NextPrimaryFire;
            // Extra fire timers (bolt action, shotgun pump, etc.) — Phase 3 async
        }
    }

    /// <summary>
    /// PrimaryAttack — weapon_vj_base/shared.lua:652-810.
    /// Fires bullets (trace) or melee sweep, plays sounds, consumes ammo.
    /// </summary>
    protected virtual void PrimaryAttack(BaseNPC npc, GameObject ene)
    {
        var owner = WeaponOwner;
        if (!owner.IsValid()) return;

        float curTime = Time.Now;

        // Melee weapon
        if (IsMeleeWeapon)
        {
            var ownersPos = owner.WorldPosition;
            var sphere = new Sphere(ownersPos, MeleeWeaponDistance + 20f);
            foreach (var ent in Scene.FindInPhysics(sphere))
            {
                if (!ent.IsValid() || ent == owner) continue;

                bool isPlayer = ent.Tags.Has("player");
                bool isNPC = ent.Components.Get<BaseNPC>() != null;
                bool isValidTarget = isNPC || isPlayer || ent.Tags.Has("attackable") || ent.Tags.Has("destructible");

                if (!isValidTarget) continue;

                // Friendly check
                var entNPC = ent.Components.Get<BaseNPC>();
                if (entNPC != null && npc.Disposition(ent) == (int)VJBase.Disposition.Like) continue;

                // Angle check
                var toEnt = (ent.WorldPosition - ownersPos).Normal;
                float dot = Vector3.Dot(owner.WorldRotation.Forward, toEnt);
                float angleRad = MathF.Acos(Math.Clamp(dot, -1f, 1f));
                if (angleRad > MathF.PI * 0.5f) continue; // 90° default guard

                // Apply damage
                var dmginfo = new DamageInfo();
                dmginfo.Damage = npc.ScaleByDifficulty(Primary_Damage);
                dmginfo.Attacker = owner;
                dmginfo.Tags.Add("melee");
                // SKIP: DMG_CLUB — Phase 3 damage type mapping
                foreach (var d in ent.Components.GetAll<IDamageable>())
                    d.OnDamage(dmginfo);

                // SKIP: Player ViewPunch — Phase 3 player system
                // SKIP: OnPrimaryAttack_BulletCallback — Phase 3 callbacks
            }
        }
        // Ranged weapon
        else
        {
            if (GetClip1() <= 0) return; // No ammo

            var spawnPos = GetBulletPos(owner);
            var aimPos = GetAimPosition(npc, ene, spawnPos);
            var aimDir = (aimPos - spawnPos).Normal;

            float spread = GetAimSpread(npc, ene, aimPos, NPC_CustomSpread);

            for (int i = 0; i < Primary_NumberOfShots; i++)
            {
                var dir = aimDir;
                if (spread > 0)
                {
                    var randOff = new Vector3(
                        Game.Random.Float(-spread, spread),
                        Game.Random.Float(-spread, spread),
                        0
                    );
                    dir = (aimDir + randOff).Normal;
                }

                var result = Game.ActiveScene.Trace.Ray(spawnPos, spawnPos + dir * NPC_FiringDistanceMax)
                    .IgnoreGameObjectHierarchy(owner)
                    .UseHitPosition(true)
                    .Run();

                // Always apply damage along direction (even if no hit) — like Lua FireBullets
                var traceEnd = result.Hit ? result.HitPosition : spawnPos + dir * NPC_FiringDistanceMax;

                // Damage target along the trace
                // SKIP: Lua FireBullets structure (bullet.Callback, tracer, etc.) — Phase 3
                if (result.Hit && result.GameObject.IsValid())
                {
                    var dmginfo = new DamageInfo();
                    dmginfo.Damage = npc.ScaleByDifficulty(Primary_Damage);
                    dmginfo.Attacker = owner;
                    dmginfo.Position = result.HitPosition;
                    dmginfo.Tags.Add("bullet");
                    // SKIP: Force — Phase 3 (S&Box: apply force separately on Rigidbody)
                    foreach (var d in result.GameObject.Components.GetAll<IDamageable>())
                        d.OnDamage(dmginfo);
                }
            }

            // Ammo consumption
            SetClip1(GetClip1() - Primary_TakeAmmo);

            // Extra fire sound (bolt action, shotgun pump) — Phase 3 async timer
            // SKIP: PrimaryAttackEffects + MuzzleFlash — Phase 3 effects system
            // SKIP: OnPrimaryAttack_BulletCallback — Phase 3 callbacks
        }
    }

    // ═══ Phase 2: Helpers ═══

    /// <summary>
    /// Get bullet spawn position. Uses attachment position if configured, else weapon position + forward.
    /// </summary>
    protected virtual Vector3 GetBulletPos(GameObject owner)
    {
        if (!string.IsNullOrEmpty(NPC_BulletSpawnAttachment))
        {
            // SKIP: attachment lookup — Phase 3 animation/model system
        }
        return GameObject.WorldPosition;
    }

    /// <summary>
    /// Get the aim position for the NPC. Lua: GetAimPosition(owner, ene, spawnPos, 0).
    /// </summary>
    protected virtual Vector3 GetAimPosition(BaseNPC npc, GameObject ene, Vector3 spawnPos)
    {
        if (ene.IsValid())
        {
            // Use enemy WorldSpaceCenter as aim target
            var npcOnEnemy = ene.Components.Get<BaseNPC>();
            if (npcOnEnemy != null)
                return npcOnEnemy.WorldSpaceCenter();
            return ene.WorldPosition;
        }
        return spawnPos + npc.WorldRotation.Forward * 1000f;
    }

    /// <summary>
    /// Get bullet spread. Lua: GetAimSpread(owner, ene, aimPos, NPC_CustomSpread).
    /// </summary>
    protected virtual float GetAimSpread(BaseNPC npc, GameObject ene, Vector3 aimPos, float customSpread)
    {
        // Base spread: 0.02 * customSpread
        // NPC with enemy: scaled by distance (farther = more spread)
        float spread = 0.02f * customSpread;
        if (ene.IsValid())
        {
            float dist = npc.WorldPosition.Distance(ene.WorldPosition);
            spread *= Math.Clamp(dist / 1000f, 0.5f, 5f);
        }
        return spread;
    }
}
