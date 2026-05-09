using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase;

/// <summary>
/// Default VJ weapon component. Implements IVJBaseWeapon with configurable properties.
/// Covers: inventory/lifecycle, NPC autonomous fire (NPC_Think, NPCShoot_Primary, PrimaryAttack).
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

    // ═══ NPC Firing Config (weapon_vj_base/shared.lua:35-63) ═══
    [Property] public float NPC_NextPrimaryFire { get; set; } = 0.11f;
    [Property] public float NPC_TimeUntilFire { get; set; }
    [Property] public List<float> NPC_TimeUntilFireExtraTimers { get; set; } = new();
    [Property] public float NPC_CustomSpread { get; set; } = 1f;
    [Property] public string NPC_BulletSpawnAttachment { get; set; } = "";
    [Property] public bool NPC_StandingOnly { get; set; }
    [Property] public float NPC_FiringDistanceScale { get; set; } = 1f;
    [Property] public float NPC_FiringDistanceMax { get; set; } = 100000f;
    [Property] public float NPC_FiringCone { get; set; } = 0.9f;

    // ═══ Reload Sound ═══
    [Property] public bool NPC_HasReloadSound { get; set; } = true;
    [Property] public string NPC_ReloadSound { get; set; }
    [Property] public float NPC_ReloadSoundLevel { get; set; } = 60f;

    // ═══ Before-Fire Sound ═══
    [Property] public string NPC_BeforeFireSound { get; set; }
    [Property] public float NPC_BeforeFireSoundLevel { get; set; } = 70f;
    [Property] public float NPC_BeforeFireSoundPitchA { get; set; } = 90f;
    [Property] public float NPC_BeforeFireSoundPitchB { get; set; } = 100f;
    public (float a, float b) NPC_BeforeFireSoundPitch => (NPC_BeforeFireSoundPitchA, NPC_BeforeFireSoundPitchB);

    // ═══ Extra Fire Sound ═══
    [Property] public string NPC_ExtraFireSound { get; set; }
    [Property] public float NPC_ExtraFireSoundTime { get; set; } = 0.4f;
    [Property] public float NPC_ExtraFireSoundLevel { get; set; } = 70f;
    [Property] public float NPC_ExtraFireSoundPitchA { get; set; } = 90f;
    [Property] public float NPC_ExtraFireSoundPitchB { get; set; } = 100f;

    // ═══ Secondary Fire ═══
    [Property] public bool NPC_HasSecondaryFire { get; set; }
    [Property] public string NPC_SecondaryFireEnt { get; set; } = "obj_vj_grenade_rifle";
    [Property] public int NPC_SecondaryFireChance { get; set; } = 3;
    [Property] public float NPC_SecondaryFireNextA { get; set; } = 12f;
    [Property] public float NPC_SecondaryFireNextB { get; set; } = 15f;
    [Property] public float NPC_SecondaryFireDistance { get; set; } = 1000f;
    [Property] public string NPC_SecondaryFireSound { get; set; }
    [Property] public float NPC_SecondaryFireSoundLevel { get; set; } = 90f;

    // ═══ Firing Guards + Melee Sounds ═══
    /// <summary>IsReloading — Lua:665 guard: blocks PrimaryAttack during reload.</summary>
    public bool IsReloading { get; set; }
    /// <summary>NextSecondaryFireT — Lua:665 guard: blocks PrimaryAttack when secondary fire cooldown active.</summary>
    public float NextSecondaryFireT { get; set; }
    /// <summary>Melee weapon hit sound table.</summary>
    [Property] public List<string> MeleeWeaponSound_Hit { get; set; } = new();
    /// <summary>Melee weapon miss sound table.</summary>
    [Property] public List<string> MeleeWeaponSound_Miss { get; set; } = new();
    /// <summary>Dry fire sound table — played when out of ammo.</summary>
    [Property] public List<string> DryFireSound { get; set; } = new();
    [Property] public float DryFireSoundLevel { get; set; } = 70f;
    [Property] public float DryFireSoundPitchA { get; set; } = 90f;
    [Property] public float DryFireSoundPitchB { get; set; } = 100f;

    // ═══ Primary Attack Config (weapon_vj_base/shared.lua Primary table) ═══
    [Property] public float Primary_Damage { get; set; } = 10f;
    [Property] public float Primary_Delay { get; set; } = 0.11f;
    [Property] public int Primary_NumberOfShots { get; set; } = 1;
    [Property] public float Primary_Force { get; set; } = 2f;
    [Property] public int Primary_TakeAmmo { get; set; } = 1;
    [Property] public float MeleeWeaponDistance { get; set; } = 75f;

    // ═══ Runtime State ═══
    public float NPC_NextPrimaryFireT { get; set; }
    public float NPC_NextDrySoundT { get; set; }
    public float NPC_SecondaryFireNextT { get; set; }
    public float NPC_DelayedFireTime { get; set; }
    public float NPC_ExtraFireSoundTime_T { get; set; }

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

    // ═══ Weapon lifecycle callbacks ═══
    /// <summary>MaintainWorldModel — weapon_vj_base/shared.lua:477-506. Positions weapon model on owner bone. Phase 3: model attachment.</summary>
    protected virtual void MaintainWorldModel(GameObject owner) { }
    /// <summary>OnThink — weapon_vj_base/shared.lua. Custom per-weapon think callback.</summary>
    protected virtual void OnThink() { }

    // ═══ Per-frame auto-fire (called from HumanNPC.Think or Component Update) ═══
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

        // lua:540-541 — MaintainWorldModel + OnThink callbacks
        MaintainWorldModel(owner);
        OnThink();

        // Only fire if this is the owner's active weapon
        var activeWep = npc.GetActiveWeapon();
        if (activeWep != GameObject) return;

        // Delayed fire completion FIRST (lua:629 — timer.Simple expired → PrimaryAttack)
        // Must check before auto-fire to avoid NPCShoot_Primary re-arming delay and overwriting the pending shot
        if (NPC_DelayedFireTime > 0 && Time.Now >= NPC_DelayedFireTime)
        {
            NPC_DelayedFireTime = 0;
            DoPrimaryFire();
        }
        // Non-melee weapons auto-fire on timer (only when no delayed fire is pending)
        else if (NPC_DelayedFireTime <= 0 && !IsMeleeWeapon && NPC_NextPrimaryFire >= 0 && Time.Now > NPC_NextPrimaryFireT && NPC_CanFire(npc))
        {
            NPCShoot_Primary();
        }

        // Extra fire sound (bolt action, shotgun pump) — lua:680-685 timer.Simple(NPC_ExtraFireSoundTime, ...)
        if (NPC_ExtraFireSoundTime_T > 0 && Time.Now > NPC_ExtraFireSoundTime_T)
        {
            NPC_ExtraFireSoundTime_T = 0;
            if (!string.IsNullOrEmpty(NPC_ExtraFireSound))
            {
                var pitch = Game.Random.Float(NPC_ExtraFireSoundPitchA, NPC_ExtraFireSoundPitchB);
                Sound.Play(NPC_ExtraFireSound, owner.WorldPosition);
            }
        }
    }

    // ═══ Firing condition check (weapon_vj_base/shared.lua:548-591) ═══
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

        // lua:575 — Firing cone check (always runs for non-controlled NPCs)
        // SKIP: lua:575 — isControlled && IN_ATTACK2 guard — Phase 3 player controller input
        // When VJ_IsBeingControlled: Lua requires IN_ATTACK2 (right mouse) to fire; C# always allows when conditions met.
        if (ene.IsValid())
        {
            var spawnPos = GameObject.WorldPosition;
            var aimPos = GetAimPosition(npc, ene, spawnPos);
            var aimDir = (aimPos - spawnPos).Normal;
            var sightDir = npc.GetHeadDirection();
            aimDir = aimDir.WithZ(0).Normal;
            sightDir = sightDir.WithZ(0).Normal;
            float dot = Vector3.Dot(sightDir, aimDir);
            return dot > NPC_FiringCone;
        }

        // lua:590 — reached end without entering any return branch → false
        return false;
    }

    // ═══ Weapon callbacks (shared.lua:676-677) ═══
    /// <summary>
    /// CanPrimaryAttack — Lua:676: weapon-specific guard before PrimaryAttack.
    /// Override to add custom fire-prevention logic (e.g. cooldowns, ammo checks).
    /// Return false to block the attack.
    /// </summary>
    public virtual bool CanPrimaryAttack() => true;

    /// <summary>
    /// OnPrimaryAttack — Lua:677/808: called with "Init" (pre-fire, return true to block)
    /// and "PostFire" (post-fire notification). Also "MeleeHit" with the hit entity for melee.
    /// </summary>
    public virtual bool OnPrimaryAttack(string type, GameObject ent = null) => false;

    /// <summary>OnPrimaryAttack_BulletCallback — shared.lua:753-755. Fired when bullet trace hits.</summary>
    public Action<GameObject, TraceResult, DamageInfo> OnPrimaryAttack_BulletCallback { get; set; }

    /// <summary>PrimaryAttackEffects — shared.lua:812. Muzzle flash, shell eject. Phase 3 effects.</summary>
    public virtual void PrimaryAttackEffects(GameObject owner) { }

    // ═══ Execute primary fire (weapon_vj_base/shared.lua:593-650) ═══
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
        // lua:594 — NPC without enemy or enemy not visible → abort
        if (!npc.VJ_IsBeingControlled && (!ene.IsValid() || !npc.Enemy.Visible)) return;

        // lua:259 — UpdatePoseParamTracking(true)
        npc.UpdatePoseParamTracking(true);

        // Secondary fire chance (lua:603-625)
        if (NPC_HasSecondaryFire && npc.Weapon_CanSecondaryFire && Time.Now > NPC_SecondaryFireNextT
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

        // lua:633 — owner:IsNPC() → S&Box: BaseNPC component presence = NPC identity
        var npc = owner.Components.Get<BaseNPC>();
        if (npc == null) return;

        float curTime = Time.Now;
        var ene = npc.GetEnemy();

        // Only fire if timer allows (Lua: NPC_NextPrimaryFire is truthy for 0, false disables)
        if (NPC_NextPrimaryFire >= 0 && curTime <= NPC_NextPrimaryFireT) return;

        // Check NPC_CanFire again for safety
        if (!NPC_CanFire(npc)) return;

        PrimaryAttack(npc, ene);

        // Register combat sound for NPC hearing/investigation
        SoundEventRegistry.Register(GameObject.WorldPosition, VJSoundType.Combat, GameObject, 1f);

        var human = npc as HumanNPC;
        if (human != null)
            human.WeaponLastShotTime = curTime;
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
        bool isNPC = npc != null;

        // lua:660 — SetNextPrimaryFire FIRST, before any guards
        if (NPC_NextPrimaryFire >= 0)
            NPC_NextPrimaryFireT = curTime + NPC_NextPrimaryFire;

        // lua:665 — Reloading / SecondaryFire guard
        if (IsReloading || NextSecondaryFireT > curTime) return;

        // lua:666 — NPC without enemy guard (non-controlled NPC only)
        if (isNPC && !npc.VJ_IsBeingControlled && !ene.IsValid()) return;

        // lua:667-674 — Out of ammo → dry fire sound + return (non-melee only)
        if (!IsMeleeWeapon && GetClip1() <= 0)
        {
            var drySd = VJUtility.PICK(DryFireSound);
            if (drySd != null)
            {
                var dryPitch = Game.Random.Float(DryFireSoundPitchA, DryFireSoundPitchB);
                Sound.Play(drySd, owner.WorldPosition);
            }
            return;
        }

        // lua:676 — CanPrimaryAttack callback
        if (!CanPrimaryAttack()) return;

        // lua:677 — OnPrimaryAttack("Init") guard (return true to block)
        if (OnPrimaryAttack("Init") == true) return;

        // lua:679-685 — NPC_ExtraFireSound timer (bolt action, shotgun pump)
        if (isNPC && !IsMeleeWeapon && !string.IsNullOrEmpty(NPC_ExtraFireSound) && NPC_ExtraFireSoundTime > 0)
            NPC_ExtraFireSoundTime_T = curTime + NPC_ExtraFireSoundTime;

        // lua:687-693 — Primary firing sound
        // SKIP: lua:687-705 — Primary.Sound/EmitSound + DistantSound + gesture — Phase 3 sound system

        // ═══ lua:707-742 — MELEE WEAPON ═══
        if (IsMeleeWeapon)
        {
            bool meleeHit = false;
            var ownersPos = owner.WorldPosition;
            var myClass = npc?.VJ_NPC_Class;

            foreach (var ent in Scene.FindInPhysics(new Sphere(ownersPos, MeleeWeaponDistance + 20f)))
            {
                if (!ent.IsValid() || ent == owner) continue;

                // lua:713 — Skip bullseye-controlled / player-controlling-NPC
                // SKIP: lua:713 — IsVJBaseBullseye + VJ_IsBeingControlled / VJ_IsControllingNPC — Phase 3

                bool isPlayer = ent.Components.Get<PlayerBase>() != null;
                bool isENTNPC = ent.Components.Get<BaseNPC>() != null;
                var entData = ent.Components.Get<BaseNPC>();

                // lua:714 — Target validity: isPlayer owner → any target; isNPC owner → NPCs / Players / Attackable / Destructible
                // lua:714 — + Disposition guard + class exclusion + angle
                if (!isPlayer && isNPC)
                {
                    // NPC owner: validate target type
                    if (!isENTNPC && !isPlayer && !BaseNPC.HasEntityFlag(ent, "VJ_ID_Attackable") && !BaseNPC.HasEntityFlag(ent, "VJ_ID_Destructible"))
                        continue;
                    // Disposition guard: don't hit friends
                    if (entData != null && npc.Disposition(ent) == (int)VJBase.Disposition.Like) continue;
                    // Class exclusion: ent:GetClass() != owner:GetClass()
                    if (entData != null && myClass != null && entData.VJ_NPC_Class.Any(c => myClass.Contains(c))) continue;
                }

                // lua:714 — Angle check
                var toEnt = (ent.WorldPosition - ownersPos).Normal;
                float dot = Vector3.Dot(owner.WorldRotation.Forward, toEnt);
                float angleRad = MathF.Acos(Math.Clamp(dot, -1f, 1f));
                float meleeAngleRadius = npc?.MeleeAttackDamageAngleRadius > 0 ? npc.MeleeAttackDamageAngleRadius : 90f;
                if (angleRad > MathF.PI / 180f * meleeAngleRadius) continue;

                // lua:715-723 — Apply damage
                float dmgAmount = isNPC ? npc.ScaleByDifficulty(Primary_Damage) : Primary_Damage;
                var dmginfo = new DamageInfo();
                dmginfo.Damage = dmgAmount;
                // SKIP: lua:718 — SetDamageForce for VJ_ID_Living — Phase 3
                // SKIP: lua:719 — SetInflictor(owner) — Phase 3
                dmginfo.Attacker = owner;
                dmginfo.Tags.Add("melee");
                // SKIP: lua:721 — DMG_CLUB — Phase 3 damage type
                // SKIP: lua:722 — VJ.DamageSpecialEnts — Phase 3

                foreach (var d in ent.Components.GetAll<IDamageable>())
                    d.OnDamage(dmginfo);

                // SKIP: lua:724-726 — Player ViewPunch — Phase 3

                // lua:727 — OnPrimaryAttack("MeleeHit", ent)
                OnPrimaryAttack("MeleeHit", ent);
                meleeHit = true;
                // Register combat sound for NPC hearing/investigation
                SoundEventRegistry.Register(ent.WorldPosition, VJSoundType.Combat, owner);
            }

            // lua:731-741 — Melee hit/miss sound + miss callback
            if (meleeHit)
            {
                var hitSd = VJUtility.PICK(MeleeWeaponSound_Hit);
                if (hitSd != null)
                    Sound.Play(hitSd, owner.WorldPosition);
            }
            else
            {
                // lua:737 — NPC:OnMeleeAttackExecute("Miss")
                if (isNPC) npc.OnMeleeAttackExecute("Miss");
                var missSd = VJUtility.PICK(MeleeWeaponSound_Miss);
                if (missSd != null)
                    Sound.Play(missSd, owner.WorldPosition);
            }
        }
        // ═══ lua:743-786 — RANGED WEAPON ═══
        else
        {
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

                // lua:786 — owner:FireBullets(bullet) → C# Trace
                var result = Game.ActiveScene.Trace.Ray(spawnPos, spawnPos + dir * NPC_FiringDistanceMax)
                    .IgnoreGameObjectHierarchy(owner)
                    .UseHitPosition(true)
                    .Run();

                // lua:753-755 — OnPrimaryAttack_BulletCallback(attacker, tr, dmginfo)
                OnPrimaryAttack_BulletCallback?.Invoke(owner, result, dmginfo);
                if (result.Hit && result.GameObject.IsValid())
                {
                    var dmginfo = new DamageInfo();
                    dmginfo.Damage = npc.ScaleByDifficulty(Primary_Damage);
                    dmginfo.Attacker = owner;
                    dmginfo.Position = result.HitPosition;
                    dmginfo.Tags.Add("bullet");
                    // SKIP: lua:749 — Force — Phase 3
                    foreach (var d in result.GameObject.Components.GetAll<IDamageable>())
                        d.OnDamage(dmginfo);
                }
            }
        }

        // lua:789-795 — Ammo consumption (melee weapons skip this)
        if (!IsMeleeWeapon)
            SetClip1(GetClip1() - Primary_TakeAmmo);

        // lua:797 — PrimaryAttackEffects(self, owner) (NPC path only; player ViewPunch/animation deferred Phase 3)
        PrimaryAttackEffects(owner);

        // lua:808 — OnPrimaryAttack("PostFire")
        OnPrimaryAttack("PostFire");
    }

    // ═══ Helpers ═══

    /// <summary>
    /// Get bullet spawn position. Uses attachment position if configured, else weapon position + forward.
    /// </summary>
    protected virtual Vector3 GetBulletPos(GameObject owner)
    {
        if (!string.IsNullOrEmpty(NPC_BulletSpawnAttachment))
        {
            // SKIP: attachment lookup (LookupAttachment/GetAttachment) — Phase 3 animation/model system
        }
        return GameObject.WorldPosition + WorldRotation.Forward * 40f;
    }

    /// <summary>
    /// Get the aim position for the NPC. Lua: ENT:GetAimPosition(target, aimOrigin, predictionRate, projectileSpeed).
    /// core.lua:1185-1206. predictionRate/projectileSpeed deferred to Phase 3.
    /// </summary>
    protected virtual Vector3 GetAimPosition(BaseNPC npc, GameObject ene, Vector3 spawnPos)
    {
        if (ene.IsValid())
        {
            // lua:1189 — if visible, use body target; else fall back to last known position
            if (npc.Visible(ene))
            {
                var npcOnEnemy = ene.Components.Get<BaseNPC>();
                Vector3 result = npcOnEnemy != null ? npcOnEnemy.WorldSpaceCenter() : ene.WorldPosition;

                // lua:1191 — player Z offset fix
                if (ene.Components.Get<PlayerBase>() != null)
                    result.z -= 15f;

                // lua:1193 — if body target is occluded, fall back to head/eye target
                if (!npc.VisibleVec(result))
                {
                    var eyeNpc = ene.Components.Get<BaseNPC>();
                    if (eyeNpc != null)
                        result = eyeNpc.EyePosition();
                    else
                        result = ene.WorldPosition + Vector3.Up * 72f; // approximate eye/head height
                }
                return result;
            }
            // lua:1197 — not visible, use last known position
            return npc.Enemy.VisiblePos;
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
