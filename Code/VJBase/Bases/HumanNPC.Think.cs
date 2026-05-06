using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// HumanNPC Think methods — ported from npc_vj_human_base/init.lua.
/// Chase/alert behavior, grenade system, weapon state, attack timers.
/// </summary>
public partial class HumanNPC
{
    // ═══ ProcessAttackTimers override — adds grenade execution ═══
    public override void ProcessAttackTimers(float curTime)
    {
        // Handle grenade start timer (attack_grenade_start)
        if (GrenadeExecTime > 0 && curTime > GrenadeExecTime)
        {
            GrenadeExecTime = 0;
            ExecuteGrenadeAttack(StashedGrenadeEnt, StashedGrenadeDisableOwner, StashedGrenadeLandDir);
        }

        base.ProcessAttackTimers(curTime);
    }

    // ═══ SetWeaponState / GetWeaponState ═══
    public virtual void SetWeaponState(VJWepState state = VJWepState.Ready, float time = -1)
    {
        WeaponState = state;
        // Phase 3: timer-based state reset
    }

    public virtual VJWepState GetWeaponState() => WeaponState;

    // ═══ SCHEDULE_ALERT_CHASE — human_base/init.lua:2340 ═══
    public virtual void SCHEDULE_ALERT_CHASE(bool doLOSChase)
    {
        // init.lua:2341: self:ClearCondition(COND_ENEMY_UNREACHABLE)
        ClearCondition(Condition.EnemyUnreachable);

        // init.lua:2342: AA branch
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        { AA_ChaseEnemy(); return; }

        // init.lua:2343: CurrentScheduleName guard
        if (CurrentScheduleName == "SCHEDULE_ALERT_CHASE") return;

        // init.lua:2344: navtype check
        var navType = GetNavType();
        if (navType == (int)NavType.Jump || navType == (int)NavType.Climb) return;

        if (doLOSChase)
        {
            // init.lua:2345-2353 — schedule_alert_chaseLOS + RunCode_OnFinish
            var sched = new AISchedule();
            sched.Init("SCHEDULE_ALERT_CHASE_LOS");
            sched.EngTask(EngineTask.GetPathToEnemyLOS, 0);
            sched.EngTask(EngineTask.WaitForMovement, 0);
            sched.CanShootWhenMoving = true;
            sched.CanBeInterrupted = true;
            sched.RunCodeOnFinish = () =>
            {
                var ene = GetEnemy();
                if (ene.IsValid())
                {
                    // SKIP: RememberUnreachable(ene, 0) — Source engine API, Phase 3 enemy memory
                    SCHEDULE_ALERT_CHASE(false);
                }
            };
            StartSchedule(sched);
        }
        else
        {
            // init.lua:2354-2356 — schedule_alert_chase (no RunCode_OnFail in human version)
            var sched = new AISchedule();
            sched.Init("SCHEDULE_ALERT_CHASE");
            sched.EngTask(EngineTask.GetPathToEnemy, 0);
            sched.EngTask(EngineTask.RunPath, 0);
            sched.EngTask(EngineTask.WaitForMovement, 0);
            sched.CanShootWhenMoving = true;
            sched.CanBeInterrupted = true;
            StartSchedule(sched);
        }
    }

    // ═══ MaintainAlertBehavior (human version) ═══
    public override void MaintainAlertBehavior(bool alwaysChase)
    {
        var curTime = Time.Now;
        if (NextChaseTime > curTime || Dead || VJ_IsBeingControlled || Flinching || GetState() == VJState.OnlyAnimationConstant) return;

        var ene = Enemy.Target;
        if (!ene.IsValid() || TakingCoverT > curTime || (AttackAnimTime > curTime && MovementType != VJMoveType.Aerial && MovementType != VJMoveType.Aquatic)) return;

        // Melee range check — stand and attack (lua:2368-2375)
        if (HasMeleeAttack && Enemy.DistanceNearest < MeleeAttackDistance && Enemy.Visible)
        {
            // lua:2369 — angle check: enemy within melee angle radius
            var toEnemy = (ene.WorldPosition - WorldPosition).Normal;
            var headDir = WorldRotation.Forward; // GetHeadDirection() → Phase 3 skeleton
            bool inAngle = headDir.Dot(toEnemy) > MathF.Cos(MathF.PI / 180f * MeleeAttackAngleRadius);
            if (inAngle)
            {
                if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
                    AA_StopMoving();
                // lua:2373: self:SCHEDULE_IDLE_STAND()
                SCHEDULE_IDLE_STAND();
                return;
            }
        }

        if (MovementType == VJMoveType.Stationary || IsFollowing || Medic.Status != "false" || GetState() == VJState.OnlyAnimation)
            return;

        if (Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature)
        {
            NextChaseTime = curTime + 3;
            return;
        }

        if (!alwaysChase && (DisableChasingEnemy || IsGuard)) return;

        // init.lua:2394-2399 — If enemy unreachable and we have a ranged weapon → LOS chase
        // SKIP: IsUnreachable(ene) — Source engine API, Phase 3 enemy memory
        // SKIP: IsMeleeWeapon check — Phase 3 weapon system
        if (HasCondition(Condition.EnemyUnreachable) && HasWeapon)
        {
            SCHEDULE_ALERT_CHASE(true);
            // SKIP: RememberUnreachable(ene, 2) — Source engine API, Phase 3 enemy memory
        }
        else
        {
            SCHEDULE_ALERT_CHASE(false);
        }

        // init.lua:2402-2403
        if (NextChaseTime > curTime) return;
        NextChaseTime = curTime + (Enemy.Distance > 2000 ? 1f : 0.1f);
    }

    // ═══ GrenadeAttack — human_base/init.lua:3070-3186 ═══
    public virtual bool GrenadeAttack(GameObject customEnt = null, bool disableOwner = false)
    {
        // lua:3071: guard
        if (Dead || Flinching || AttackType == VJAttackType.Melee) return false;
        var eneData = Enemy;
        var ene = eneData.Target;
        var isLiveEnt = customEnt != null && customEnt.IsValid();
        var landDir = "FindBest";

        // lua:3083-3097: Determine landing direction
        if (ene.IsValid())
        {
            if (eneData.Visible)
            {
                landDir = "Enemy";
            }
            else
            {
                // lua:3089: attempt to flush enemy from hiding
                // SKIP: lua:3089 — self:VisibleVec(eneData.VisiblePos) — Phase 3 visibility
                // SKIP: lua:3089 — ene:GetPos():Distance(eneData.VisiblePos) <= GrenadeAttackMaxDistance
                bool canFlush = false;
                if (canFlush)
                {
                    landDir = "EnemyLastVis";
                }
                else if (!isLiveEnt)
                {
                    return false;
                }
            }
        }

        // lua:3099: OnGrenadeAttack("Init")
        if (OnGrenadeAttack("Init", customEnt)) return false;
        var seed = Time.Now;
        AttackSeed = (int)(seed * 1000);

        // SKIP: lua:3103-3113 — AnimTbl_GrenadeAttack / PlayAnim / ACT_INVALID / AttackAnim — Phase 3 animation system

        // lua:3115-3127: Turn toward target
        if (landDir == "Enemy")
        {
            if (GrenadeAttackAnimationFaceEnemy)
                SetTurnTarget("Enemy");
        }
        else if (landDir == "EnemyLastVis")
        {
            // SKIP: lua:3120 — SetTurnTarget(eneData.VisiblePos, AttackAnimDuration or 1.5) — Phase 3
        }
        else
        {
            // SKIP: lua:3122-3126 — VJ.TraceDirections / PICK / SetTurnTarget(bestPos) — Phase 3
        }

        // lua:3130-3164: Handle live entity positioning
        if (isLiveEnt)
        {
            // SKIP: lua:3131-3164 — customEnt.VJ_ST_Grabbed / SetMoveType / FollowBone / GetAttachment / LookupBone / GetBonePosition — Phase 3 entity parenting
        }

        // lua:3167-3169: Attack state + sound
        AttackType = VJAttackType.Grenade;
        AttackState = VJAttackState.Started;
        PlaySoundSystem("GrenadeAttack");

        // lua:3171-3183: Timer-based release
        var releaseTime = GrenadeAttackThrowTime;
        if (releaseTime <= 0)
        {
            // lua:3173: event-based attack — execute immediately + schedule reset timer
            ScheduleAttackTimers();
            ExecuteGrenadeAttack(customEnt, disableOwner, landDir);
        }
        else
        {
            // lua:3178-3183 — attack_grenade_start timer → polling via GrenadeExecTime
            float rate = MathF.Max(AnimPlaybackRate, 0.01f);
            StashedGrenadeEnt = customEnt;
            StashedGrenadeDisableOwner = disableOwner;
            StashedGrenadeLandDir = landDir;
            GrenadeExecTime = Time.Now + releaseTime / rate;
        }
        // lua:3184: OnGrenadeAttack("PostInit")
        OnGrenadeAttack("PostInit", customEnt, landDir);
        return true;
    }

    // ═══ ExecuteGrenadeAttack — human_base/init.lua:3204-3331 ═══
    public virtual GameObject ExecuteGrenadeAttack(GameObject customEnt = null, bool disableOwner = false, object landDir = null)
    {
        // lua:3205: guard
        if (Dead || PauseAttacks || Flinching || AttackType == VJAttackType.Melee) return null;
        GameObject grenade;
        var isLiveEnt = customEnt != null && customEnt.IsValid();
        var fuseTime = GrenadeAttackFuseTime;

        // lua:3215-3234: Determine spawn position and angle
        Vector3? spawnPos = null;
        Angles spawnAng = Angles.Zero;
        // SKIP: lua:3215 — OnGrenadeAttack("SpawnPos") custom position — Phase 3
        // SKIP: lua:3218-3233 — LookupAttachment / GetAttachment / LookupBone / GetBonePosition / GetShootPos — Phase 3 bone/animation system
        spawnPos = WorldPosition + WorldRotation.Forward * 30;

        // lua:3238-3254: Determine landing position
        var landingPos = WorldPosition + WorldRotation.Forward * 200;
        if (landDir is string ld)
        {
            if (ld == "Enemy")
            {
                // SKIP: landingPos = GetEnemyLastKnownPos() — Phase 3
            }
            else if (ld == "EnemyLastVis")
            {
                // SKIP: landingPos = Enemy.VisiblePos — Phase 3
            }
        }
        else if (landDir is Vector3 vec)
        {
            landingPos = vec;
        }
        else
        {
            // SKIP: lua:3249-3253 — VJ.TraceDirections / PICK — Phase 3 utility
        }

        // lua:3258-3274: Create or reuse grenade entity
        if (isLiveEnt)
        {
            // lua:3259-3263: clean up parent/move type/follow bone
            // SKIP: lua:3259-3263 — SetParent(NULL) / SetMoveType / RemoveEffects(EF_FOLLOWBONE) — Phase 3 entity system
            grenade = customEnt;
        }
        else
        {
            // lua:3267-3273: ents.Create(customEnt or PICK(GrenadeAttackEntity)) + SetModel
            // SKIP: lua:3267-3273 — ents.Create / PICK(GrenadeAttackEntity) / SetModel — Phase 3 spawning system
            return null;
        }

        // lua:3276-3278: SetOwner + position
        if (!disableOwner)
        {
            // SKIP: lua:3276 — grenade:SetOwner(self) — Phase 3 entity ownership
        }
        grenade.WorldPosition = spawnPos.Value;
        grenade.WorldRotation = spawnAng;

        // SKIP: lua:3280-3308 — GetClass-based fuse timer dispatch (npc_grenade_frag / obj_vj_grenade / etc.) — Phase 3 grenade class system

        // lua:3305-3307: OnGrenadeAttackExecute + Spawn
        // SKIP: lua:3305-3307 — OnGrenadeAttackExecute("PreSpawn") / grenade:Spawn() / grenade:Activate() — Phase 3 spawning

        // lua:3311-3322: Throw velocity
        {
            // SKIP: lua:3312 — OnGrenadeAttackExecute("PostSpawn") — Phase 3
            var vel = (landingPos - grenade.WorldPosition) + (Vector3.Up * 200 + WorldRotation.Forward * 500 + WorldRotation.Right * Game.Random.Next(-20, 21));
            var rb = grenade.Components.Get<Rigidbody>();
            if (rb != null)
            {
                rb.Wake();
                // SKIP: lua:3317 — AddAngleVelocity — Phase 3
                rb.Velocity = vel;
            }
        }

        // lua:3324-3329: AttackState
        if (AttackState < VJAttackState.Executed)
        {
            AttackState = VJAttackState.Executed;
            // lua:3326-3328 — attackTimers[VJ.ATTACK_TYPE_GRENADE](self)
            ScheduleAttackTimers();
        }
        return grenade;
    }

    // ═══ Callback stubs ═══
    public virtual void Init() { }

    // ═══ Helper: GetActiveWeapon — returns the currently active weapon GameObject ═══
    public virtual GameObject GetActiveWeapon() => WeaponEntity;

    // ═══ PreInit callback (empty — override in derived types) ═══
    public virtual void PreInit() { }

    // ═══ DoChangeMovementType — human_base/init.lua:2287-2319 ═══
    public virtual void DoChangeMovementType(VJMoveType? movType = null)
    {
        // lua:2288: if movType then self.MovementType = movType
        if (movType == null) return;
        MovementType = movType.Value;

        if (movType == VJMoveType.Ground)
        {
            // SKIP: lua:2291 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2292 — CapabilitiesRemove(CAP_MOVE_FLY) — Source engine capabilities
            // SKIP: lua:2293 — SetNavType(NAV_GROUND) — S&Box uses NavMeshAgent component
            // SKIP: lua:2294 — SetMoveType(MOVETYPE_STEP) — Source engine move type
            // SKIP: lua:2295 — CapabilitiesAdd(CAP_MOVE_GROUND) — Source engine capabilities
            // SKIP: lua:2296 — CapabilitiesAdd(CAP_MOVE_JUMP) via AnimExists/ACT_JUMP/convar — Phase 3 animation/jump
            // lua:2298: Weapon_CanMoveFire → CapabilitiesAdd(CAP_MOVE_SHOOT)
            // SKIP: lua:2298 — CapabilitiesAdd(CAP_MOVE_SHOOT) — Source engine capabilities, Weapon_CanMoveFire field is set
        }
        else if (movType == VJMoveType.Aerial || movType == VJMoveType.Aquatic)
        {
            // SKIP: lua:2300 — CapabilitiesRemove(capBitsGround) — Source engine capabilities
            // SKIP: lua:2301 — SetGroundEntity(NULL) — Source engine
            // SKIP: lua:2302 — AddFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2303 — SetNavType(NAV_FLY) — S&Box NavMeshAgent
            // SKIP: lua:2304 — SetMoveType(MOVETYPE_STEP) — Source engine move type
            // SKIP: lua:2305 — CapabilitiesAdd(CAP_MOVE_FLY) — Source engine capabilities
        }
        else if (movType == VJMoveType.Stationary)
        {
            // SKIP: lua:2307 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2308 — CapabilitiesRemove(capBitsShared) — Source engine capabilities
            // SKIP: lua:2309 — SetNavType(NAV_NONE) — S&Box NavMeshAgent
            // SKIP: lua:2310-2311 — GetParent check → SetMoveType(MOVETYPE_FLY) — Source engine parenting
        }
        else if (movType == VJMoveType.Physics)
        {
            // SKIP: lua:2314 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2315 — CapabilitiesRemove(capBitsShared) — Source engine capabilities
            // SKIP: lua:2316 — SetNavType(NAV_NONE) — S&Box NavMeshAgent
            // SKIP: lua:2317 — SetMoveType(MOVETYPE_VPHYSICS) — Source engine physics move type
        }
    }

    // ═══ TranslateActivity — human_base/init.lua:2417-2466 ═══
    /// <summary>
    /// Override activity based on alert/weapon/scared state. Falls back to AnimationTranslations table.
    /// Returns the translated activity ID (ACT_* constant).
    /// </summary>
    public virtual int TranslateActivity(int act)
    {
        // lua:2421-2428: Handle idle scared and angry animations
        if (act == ACT_IDLE)
        {
            // lua:2422: Weapon_UnarmedBehavior_Active → ACT_COWER
            if (Weapon_UnarmedBehavior_Active)
                return ACT_COWER;
            // lua:2425: Alerted + weapon → ACT_IDLE_ANGRY
            else if (Alerted != VJAlertState.None && GetWeaponState() != VJWepState.Holstered && GetActiveWeapon().IsValid())
                return ACT_IDLE_ANGRY;
        }
        // lua:2430-2432: Handle running while scared → ACT_RUN_PROTECTED
        else if (act == ACT_RUN && Weapon_UnarmedBehavior_Active && !VJ_IsBeingControlled)
        {
            return ACT_RUN_PROTECTED;
        }
        // lua:2433-2452: Handle walk/run while alerted
        else if ((act == ACT_RUN || act == ACT_WALK) && Alerted != VJAlertState.None)
        {
            var eneData = Enemy;
            // lua:2436: Can move-fire while aiming
            // SKIP: CurrentSchedule.CanShootWhenMoving — depends on schedule system
            if (Weapon_CanMoveFire && eneData.Target.IsValid()
                && (eneData.Visible || (eneData.VisibleTime + 5) > Time.Now)
                && CanFireWeapon(true, false))
            {
                int aimAct = TranslateActivity(act == ACT_RUN ? ACT_RUN_AIM : ACT_WALK_AIM);
                // SKIP: lua:2438 — VJ.AnimExists(self, anim) — Phase 3 animation system
                bool animExists = aimAct != act; // fallback: assume different means exists
                if (animExists)
                {
                    // lua:2439-2442: Set weapon attack state
                    if (eneData.Visible)
                        WeaponAttackState = VJWepAttackState.Fire;
                    else
                        WeaponAttackState = VJWepAttackState.AimMove;
                    return aimAct;
                }
            }
            // lua:2448-2451: Walk/run angry fallback
            int agitatedAct = TranslateActivity(act == ACT_RUN ? ACT_RUN_AGITATED : ACT_WALK_AGITATED);
            // SKIP: lua:2449 — VJ.AnimExists(self, anim) — Phase 3 animation system
            if (agitatedAct != 0)
                return agitatedAct;
        }

        // lua:2455-2464: AnimationTranslations table lookup
        if (AnimationTranslations.TryGetValue(act, out var translation))
        {
            // lua:2457: if istable(translation)... handle list vs single value
            if (translation is List<int> list && list.Count > 0)
            {
                if (act == ACT_IDLE)
                {
                    // SKIP: lua:2459 — self:ResolveAnimation(translation) — Phase 3 animation resolution
                }
                // lua:2461: return translation[math.random(1, #translation)] or act
                return list[Game.Random.Next(0, list.Count)];
            }
            // lua:2463: return translation (single value)
            return (int)translation;
        }
        // lua:2465: return act (unmodified)
        return act;
    }

    // ═══ CanFireWeapon — Phase 3 stub (Lua:3476-3510) ═══
    public virtual bool CanFireWeapon(bool checkDistance, bool checkDistanceOnly) => true;

    // ═══ Placeholder ACT_* constants (Phase 3: move to ActivityType.cs or animation system) ═══
    const int ACT_IDLE = 0;
    const int ACT_COWER = 1;
    const int ACT_IDLE_ANGRY = 2;
    const int ACT_RUN = 3;
    const int ACT_WALK = 4;
    const int ACT_RUN_PROTECTED = 5;
    const int ACT_RUN_AIM = 6;
    const int ACT_WALK_AIM = 7;
    const int ACT_RUN_AGITATED = 8;
    const int ACT_WALK_AGITATED = 9;

    // ═══ DoChangeWeapon — human_base/init.lua:2470-2518 ═══
    /// <summary>
    /// Change or setup the NPC's weapon. wep=null to just setup current weapon.
    /// invSwitch=true preserves the old weapon (inventory swap).
    /// </summary>
    public virtual GameObject DoChangeWeapon(GameObject wep = null, bool invSwitch = false)
    {
        var curWep = GetActiveWeapon();

        // lua:2476-2479: If weapon disabled, remove and return null
        if (Weapon_Disabled && curWep.IsValid())
        {
            // SKIP: curWep:Remove() — Phase 3 weapon destruction
            return null;
        }

        // lua:2482-2493: Give or select weapon
        if (wep != null)
        {
            if (invSwitch)
            {
                // lua:2484: self:SelectWeapon(wep)
                // SKIP: lua:2484 — self:SelectWeapon(wep) — Phase 3 weapon system
                // SKIP: lua:2485 — VJ.EmitSound(self, sdWepSwitch, 70) — Phase 3 sound
                curWep = wep;
            }
            else
            {
                // lua:2488-2489: Remove current primary weapon
                // SKIP: lua:2488-2489 — curWep:Remove() — Phase 3 weapon destruction
                if (curWep.IsValid() && WeaponInventoryStatus <= VJWepInventory.Primary)
                {
                    // curWep:Remove() → SKIP
                }
                // lua:2491: curWep = self:Give(wep)
                // SKIP: lua:2491 — self:Give(wep) — Phase 3 weapon spawning
                curWep = wep; // fallback: use the passed weapon directly
                WeaponInventory["Primary"] = curWep;
            }
        }

        // lua:2497-2516: Setup the (new or existing) weapon
        if (curWep.IsValid())
        {
            // lua:2498: self.WeaponAttackAnim = ACT_INVALID
            // SKIP: lua:2498 — WeaponAttackAnim = ACT_INVALID — Phase 3 animation
            // lua:2499: self:SetWeaponState() — reset state
            SetWeaponState();

            if (invSwitch)
            {
                // lua:2501: if curWep.IsVJBaseWeapon then curWep:Equip(self)
                // SKIP: lua:2501 — IsVJBaseWeapon/Equip — Phase 3 weapon system
            }
            else
            {
                // lua:2503: WeaponInventoryStatus = VJ.WEP_INVENTORY_PRIMARY
                WeaponInventoryStatus = VJWepInventory.Primary;
                // lua:2505-2509: Remove old primary if this is a new weapon
                var curPrimary = WeaponInventory.GetValueOrDefault("Primary") as GameObject;
                if (curWep != curPrimary)
                {
                    // SKIP: lua:2507 — curPrimary:Remove() — Phase 3 weapon destruction
                    WeaponInventory["Primary"] = curWep;
                }
            }
            // lua:2511: self:UpdateAnimationTranslations(curWep:GetHoldType())
            // SKIP: lua:2511 — UpdateAnimationTranslations/GetHoldType — Phase 3 animation system
            // lua:2512: self:OnWeaponChange(curWep, self.WeaponEntity, invSwitch)
            OnWeaponChange(curWep, WeaponEntity, invSwitch);
            // lua:2513: self.WeaponEntity = curWep
            WeaponEntity = curWep;
        }
        else
        {
            // lua:2515: self.WeaponInventoryStatus = VJ.WEP_INVENTORY_NONE
            WeaponInventoryStatus = VJWepInventory.None;
        }
        // lua:2517: return curWep
        return curWep;
    }

    // ═══ Initialize — human_base/init.lua:2131-2282 ═══
    public virtual void Initialize()
    {
        // lua:2132: self:PreInit()
        PreInit();
        // SKIP: lua:2133 — CustomOnPreInitialize backwards compatibility — deprecated

        var curTime = Time.Now;

        // lua:2135: self:SetSpawnEffect(false)
        // SKIP: lua:2135 — SetSpawnEffect — Source engine spawn effect, S&Box uses Prefab spawn
        // lua:2136: self:SetRenderMode(RENDERMODE_NORMAL)
        // SKIP: lua:2136 — SetRenderMode — Source engine render mode
        // lua:2137: self:AddEFlags(EFL_NO_DISSOLVE)
        // SKIP: lua:2137 — AddEFlags(EFL_NO_DISSOLVE) — Phase 3 dissolve system
        // lua:2138: self:SetUseType(SIMPLE_USE)
        // SKIP: lua:2138 — SetUseType — S&Box uses Interaction component

        // lua:2139-2144: Set model
        if (GameObject.Model == null || GameObject.Model.IsError)
        {
            // SKIP: lua:2140 — PICK(self.Model) — Model list selection
        }

        // lua:2145-2148: Collision setup
        // SKIP: lua:2145 — SetHullType(self.HullType) — Source engine hull system
        // SKIP: lua:2146 — SetHullSizeNormal() — Source engine hull
        // SKIP: lua:2147 — SetSolid(SOLID_BBOX) — S&Box uses Collider components
        // SKIP: lua:2148 — SetCollisionGroup(COLLISION_GROUP_NPC) — S&Box uses collision layers

        // lua:2149: self:SetMaxYawSpeed(self.TurningSpeed)
        // SKIP: lua:2149 — SetMaxYawSpeed — S&Box rotation handled via NavMeshAgent/TurningSpeed field

        // SKIP: lua:2150 — SetSaveValue("m_HackedGunPos", defShootVec) — Source engine save system

        // lua:2152-2158: Set name if empty
        // SKIP: lua:2152-2158 — GetName/SetName/list.Get("NPC") — S&Box GameObject.Name is set on prefab

        // lua:2160-2161: InitConvars + process time
        // SKIP: lua:2161 — InitConvars(self) — handled by VJInit.cs / component initialization
        // SKIP: lua:2162 — NextProcessTime = vj_npc_processtime:GetInt() — convar, use field default

        // lua:2163: self.SelectedDifficulty = vj_npc_difficulty:GetInt()
        // SKIP: lua:2163 — SelectedDifficulty from convar — Phase 3 difficulty system

        // lua:2164-2165: RelationshipEnts / RelationshipMemory init — already handled by BaseNPC field initializers

        // lua:2166: self.AnimationTranslations = {}
        AnimationTranslations.Clear();

        // lua:2167: self.WeaponInventory = {}
        WeaponInventory.Clear();

        // lua:2168: self.IdleSoundBlockTime = CurTime() + math.random(0.3, 6)
        IdleSoundBlockTime = curTime + (0.3f + (float)Game.Random.NextDouble() * 5.7f);

        // lua:2169: self.MainSoundPitchValue = ...
        // SKIP: lua:2169 — MainSoundPitchValue static/table computation — Phase 3 sound system (BaseNPC already has field default 0)

        // lua:2170: SightDistance convar override
        // SKIP: lua:2170 — vj_npc_sight_distance convar override — handled by VJInit

        // lua:2172-2173: DoChangeMovementType
        DoChangeMovementType(MovementType);

        // lua:2174-2185: Capabilities
        // SKIP: lua:2174 — CapabilitiesAdd(capBitsDefault) — Source engine CAP_* bits
        // SKIP: lua:2175 — CanOpenDoors → CapabilitiesAdd(capBitsDoors) — Phase 3 door system
        // SKIP: lua:2177-2178 — LookupAttachment("eyes"/"forward") → CAP_ANIMATEDFACE — Phase 3 animation
        if (Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature)
        {
            // lua:2181-2182
            Weapon_Disabled = true;
            Weapon_IgnoreSpawnMenu = true;
        }
        // SKIP: lua:2183-2184 — CapabilitiesAdd(capBitsWeapons) — Source engine capabilities

        // lua:2187-2191: Health
        // SKIP: lua:2188 — vj_npc_health convar — handled by VJInit
        var hp = ScaleByDifficulty(StartHealth);
        GameObject.Health = hp;
        StartHealth = hp;

        // lua:2193: self:Init()
        Init();

        // SKIP: lua:2194 — ApplyBackwardsCompatibility(self) — deprecated

        // lua:2197-2204: Collision bounds & auto-compute melee distances
        // SKIP: lua:2197 — GetCollisionBounds — Phase 3 (S&Box: Model.PhysicsBounds or Collider bounds)
        // SKIP: lua:2200-2201 — GetSurroundingBounds/SetSurroundingBounds — Source engine bounds system
        // lua:2203: if !self.MeleeAttackDistance then auto-compute
        if (MeleeAttackDistance <= 0)
        {
            // SKIP: lua:2203 — MeleeAttackDistance = abs(collisionMax.x) + 30 — Phase 3: compute from model bounds
            MeleeAttackDistance = 80; // fallback
        }
        // lua:2204: if !self.MeleeAttackDamageDistance then auto-compute
        if (MeleeAttackDamageDistance <= 0)
        {
            // SKIP: lua:2204 — MeleeAttackDamageDistance = abs(collisionMax.x) + 60 — Phase 3
            MeleeAttackDamageDistance = 110; // fallback
        }

        // lua:2205: self:SetupBloodColor(self.BloodColor)
        // SKIP: lua:2205 — SetupBloodColor — Phase 3 blood system

        // lua:2207: self.NextWanderTime = ...
        NextWanderTime = (NextWanderTime != 0 ? NextWanderTime : curTime + (IdleAlwaysWander ? 0 : 1));

        // ═══ lua:2210-2281: Delayed init (timer.Simple(0.1, ...)) ═══
        // In S&Box we can run post-init synchronously since component lifecycle handles ordering
        // lua:2212: self:SetMaxLookDistance(self.SightDistance)
        // SKIP: lua:2212 — SetMaxLookDistance — Phase 3 (BaseNPC SightDistance field is set, consumed by AISenses)

        // lua:2213: self:SetFOV(self.SightAngle)
        // SKIP: lua:2213 — SetFOV — Phase 3 (BaseNPC SightAngle field is set, consumed by AISenses)

        // lua:2214: if self:GetNPCState() <= NPC_STATE_NONE then self:SetNPCState(NPC_STATE_IDLE)
        // SKIP: lua:2214 — GetNPCState/SetNPCState — Source engine NPC state machine

        // lua:2215: if IsValid(self:GetCreator()) && creator:GetInfoNum("vj_npc_spawn_guard", 0) == 1 then self.IsGuard = true
        // SKIP: lua:2215 — GetCreator() spawn guard detection — Phase 3 spawner system

        // lua:2216: self:StartSoundTrack()
        // SKIP: lua:2216 — StartSoundTrack — Phase 3 sound track system

        // lua:2218-2236: Register common pose parameters
        // SKIP: lua:2219-2236 — LookupPoseParameter / PoseParameterLooking_Names — Phase 3 animation/pose system

        // lua:2237-2265: Weapon setup
        if (Weapon_Disabled)
        {
            // lua:2238: self:UpdateAnimationTranslations()
            // SKIP: lua:2238 — UpdateAnimationTranslations — Phase 3 animation system
        }
        else
        {
            // lua:2240: local wep = funcGetActiveWeapon(self)
            var wep = GetActiveWeapon();
            if (wep != null && wep.IsValid())
            {
                // lua:2242: self.WeaponEntity = self:DoChangeWeapon()
                WeaponEntity = DoChangeWeapon();
                // lua:2243: self.WeaponInventory.Primary = wep
                WeaponInventory["Primary"] = wep;

                // lua:2244-2247: Non-VJ weapon warning (skip — Phase 3)

                // lua:2247-2252: Anti-armor weapon setup
                var antiArmor = VJUtility.PICK(WeaponInventory_AntiArmorList);
                if (antiArmor != null && wep.Name != antiArmor)
                {
                    // SKIP: lua:2249 — self:Give(antiArmor) — Phase 3 weapon spawning
                    // SKIP: lua:2250 — self:SelectWeapon(wep) — Phase 3 weapon system
                    // SKIP: lua:2251 — wep:Equip(self) — Phase 3 weapon equip
                }

                // lua:2253-2258: Melee weapon setup
                var melee = VJUtility.PICK(WeaponInventory_MeleeList);
                if (melee != null && wep.Name != melee)
                {
                    // SKIP: lua:2255 — self:Give(melee) — Phase 3 weapon spawning
                    // SKIP: lua:2256 — self:SelectWeapon(wep) — Phase 3 weapon system
                    // SKIP: lua:2257 — wep:Equip(self) — Phase 3 weapon equip
                }
            }
            else
            {
                // lua:2260: self:UpdateAnimationTranslations()
                // SKIP: lua:2260 — UpdateAnimationTranslations — Phase 3 animation
                // SKIP: lua:2261-2262 — Creator warning message — Phase 3
            }
        }

        // lua:2266-2268: Reset idle animation
        // SKIP: lua:2266 — GetIdealActivity/MaintainIdleAnimation — Phase 3 animation system

        // lua:2270-2279: Think hook registration
        // SKIP: lua:2270-2279 — hook.Add("Think", self, ...) — S&Box uses OnUpdate/component tick, handled by CreatureNPC.Think.cs
    }
}
