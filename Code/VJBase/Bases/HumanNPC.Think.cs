using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// HumanNPC Think methods — ported from npc_vj_human_base/init.lua.
/// Initialize, movement type, chase/alert behavior, grenade system, weapon state.
/// </summary>
public partial class HumanNPC
{
    // ═══ Initialize — human_base/init.lua:2131-2282 ═══
    public virtual void Initialize()
    {
        // lua:2132: PreInit + CustomOnPreInitialize (backwards compat)
        // Phase 3: delegate to OnInit/OnSpawn lifecycle

        // lua:2135-2137: SetSpawnEffect(false) + SetRenderMode + AddEFlags(EFL_NO_DISSOLVE)
        // SKIP: lua:2135-2137 — Source engine render/effects system
        // lua:2138: SetUseType(SIMPLE_USE)
        // SKIP: lua:2138 — Source engine use type

        // lua:2139-2144: self:GetModel() → PICK(self.Model) → SetModel
        // SKIP: lua:2139-2144 — model post-spawn assignment (S&Box prefab handles model) — Phase 3

        // lua:2145-2148: SetHullType / SetHullSizeNormal / SetSolid(SOLID_BBOX) / SetCollisionGroup
        // SKIP: lua:2145-2148 — Source engine collision/hull system
        // lua:2149: SetMaxYawSpeed(self.TurningSpeed)
        // SKIP: lua:2149 — Source engine yaw speed
        // lua:2150: SetSaveValue("m_HackedGunPos", defShootVec)
        // SKIP: lua:2150 — Source engine save value

        // lua:2152-2158: Set a name if it doesn't have one
        // SKIP: lua:2152-2158 — Source engine NPC naming (list.Get("NPC"))

        // lua:2160-2170: Initialize variables
        // SKIP: lua:2161 — InitConvars(self) — Phase 3 convar system
        NextProcessTime = 0.1f; // lua:2162: vj_npc_processtime:GetInt()
        // SKIP: lua:2163 — SelectedDifficulty = vj_npc_difficulty:GetInt() — Phase 3 difficulty system
        RelationshipEnts ??= new();
        RelationshipMemory ??= new();
        AnimationTranslations = new();
        WeaponInventory = new();
        IdleSoundBlockTime = Time.Now + Game.Random.Next(3, 61) / 10f; // lua:2168: math.random(0.3, 6)
        MainSoundPitchValue = 0; // lua:2169: default 0
        // SKIP: lua:2170 — vj_npc_sight_distance convar override — Phase 3 convar

        // lua:2173: DoChangeMovementType(MovementType)
        DoChangeMovementType(MovementType);

        // lua:2174-2175: CapabilitiesAdd(capBitsDefault) + door caps
        // SKIP: lua:2174-2175 — Source engine bit-flag capabilities (CAP_MOVE_*, capBitsDefault)
        // lua:2176-2179: LookupAttachment("eyes"/"forward") → CAP_ANIMATEDFACE
        // SKIP: lua:2176-2179 — Source engine attachment-based capability
        // lua:2180-2182: Passive → Weapon_Disabled + Weapon_IgnoreSpawnMenu
        // lua:2183-2184: elseif CapabilitiesAdd(capBitsWeapons) — SKIP (Source engine bit-flag caps)
        if (Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature)
        {
            Weapon_Disabled = true;
            Weapon_IgnoreSpawnMenu = true;
        }

        // lua:2187-2191: Health setup
        var hp = ScaleByDifficulty(StartHealth); // lua:2189: convar override or ScaleByDifficulty
        // SKIP: lua:2190 — SetHealth(hp) — S&Box health via HealthComponent
        StartHealth = (int)hp; // lua:2191: StartHealth = hp

        // lua:2193-2194: Init() callback + ApplyBackwardsCompatibility
        // Phase 3: delegate to child class override; backwards compat not needed

        // lua:2196-2205: Collision-based computations
        // SKIP: lua:2196-2205 — GetCollisionBounds / SetSurroundingBounds / WorldSpaceAABB — Source engine collision
        // lua:2203-2204: Auto-compute MeleeAttackDistance / MeleeAttackDamageDistance from collision bounds
        // Phase 3: compute from model bounds when animation system is in place

        // lua:2205: SetupBloodColor(BloodColor)
        // SKIP: lua:2205 — Source engine blood system

        // lua:2207: preserve spawner-set value, else compute default
        NextWanderTime = (NextWanderTime != 0 ? NextWanderTime : Time.Now + (IdleAlwaysWander ? 0 : 1));

        // lua:2209-2281: Delayed init (timer.Simple 0.1)
        // SKIP: lua:2211 — SetMaxLookDistance(SightDistance) — S&Box engine handles vision range
        // SKIP: lua:2212 — SetFOV(SightAngle) — S&Box engine handles FOV
        // SKIP: lua:2213 — SetNPCState / GetNPCState — Source engine NPC state
        // SKIP: lua:2214 — GetCreator / IsGuard from creator — Phase 3 spawn system
        // SKIP: lua:2215 — StartSoundTrack() — Phase 3 sound system
        // SKIP: lua:2218-2235 — LookupPoseParameter("aim_pitch"/"head_pitch"/"aim_yaw"/"head_yaw"/"aim_roll"/"head_roll") — Phase 3 animation pose params
        // lua:2237-2265: Weapon setup
        if (Weapon_Disabled)
        {
            // SKIP: lua:2238 — UpdateAnimationTranslations() — Phase 3 animation
        }
        else
        {
            // SKIP: lua:2240 — funcGetActiveWeapon(self) — Phase 3 weapon system
            // lua:2247-2258: AntiArmor + Melee weapon inventory from PICK of lists
            // SKIP: lua:2247-2258 — Give(weaponClass) / SelectWeapon / Equip — Phase 3 weapon inventory
            // SKIP: lua:2261-2263 — CanChatMessage / PrintMessage warnings — Phase 3 messaging
        }
        // SKIP: lua:2266-2268 — GetIdealActivity() / MaintainIdleAnimation(true) — Phase 3 animation
        // SKIP: lua:2269-2279 — hook.Add("Think", self, funcAnimThink) — Phase 3 animation hook system
    }

    // ═══ DoChangeMovementType — human_base/init.lua:2287-2319 ═══
    public virtual void DoChangeMovementType(VJMoveType movType)
    {
        // lua:2288-2289
        MovementType = movType;

        if (movType == VJMoveType.Ground)
        {
            // SKIP: lua:2291 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2292 — CapabilitiesRemove(CAP_MOVE_FLY) — Source engine caps
            // SKIP: lua:2293 — SetNavType(NAV_GROUND) — handled by NavMeshAgent in S&Box
            // SKIP: lua:2294 — SetMoveType(MOVETYPE_STEP) — S&Box physics handles this
            // SKIP: lua:2295 — CapabilitiesAdd(CAP_MOVE_GROUND) — Source engine caps
            // SKIP: lua:2296 — CapabilitiesAdd(CAP_MOVE_JUMP) — Source engine caps (AnimExists ACT_JUMP/pj_npc_human_jump/PoseParamMovement)
            // SKIP: lua:2298 — CapabilitiesAdd(CAP_MOVE_SHOOT) if !Weapon_Disabled && Weapon_CanMoveFire
        }
        else if (movType == VJMoveType.Aerial || movType == VJMoveType.Aquatic)
        {
            // SKIP: lua:2300 — CapabilitiesRemove(capBitsGround) — Source engine caps
            // SKIP: lua:2301 — SetGroundEntity(NULL) — Source engine ground entity
            // SKIP: lua:2302 — AddFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2303 — SetNavType(NAV_FLY) — handled by AA system
            // SKIP: lua:2304 — SetMoveType(MOVETYPE_STEP) — AA system handles this
            // SKIP: lua:2305 — CapabilitiesAdd(CAP_MOVE_FLY) — Source engine caps
        }
        else if (movType == VJMoveType.Stationary)
        {
            // SKIP: lua:2307 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2308 — CapabilitiesRemove(capBitsShared) — Source engine caps
            // SKIP: lua:2309 — SetNavType(NAV_NONE)
            // SKIP: lua:2310-2312 — GetParent check / SetMoveType(MOVETYPE_FLY)
        }
        else if (movType == VJMoveType.Physics)
        {
            // SKIP: lua:2314 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2315 — CapabilitiesRemove(capBitsShared) — Source engine caps
            // SKIP: lua:2316 — SetNavType(NAV_NONE)
            // SKIP: lua:2317 — SetMoveType(MOVETYPE_VPHYSICS)
        }
    }

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

    // ═══ SelectSchedule — human_base/init.lua:3520-3838 ═══
    public override void SelectSchedule()
    {
        // lua:3520-3522: entry guard
        if (VJ_IsBeingControlled || Dead) return;

        // lua:3524-3527: local variables
        float curTime = Time.Now;
        var ene = GetEnemy();
        bool eneValid = ene.IsValid();
        PlayIdleSound(null, null, eneValid);

        // ═══ Idle Behavior (lua:3529-3577) ═══
        if (!eneValid)
        {
            // lua:3531-3532: MaintainIdleBehavior for non-grenade attack types
            if (AttackType != VJAttackType.Grenade)
                MaintainIdleBehavior();

            // lua:3534-3535: Reset TakingCoverT if not alerted
            if (Alerted == VJAlertState.None)
                TakingCoverT = 0;

            // lua:3537
            Weapon_UnarmedBehavior_Active = false;

            // lua:3539-3576: Investigation block
            // SKIP: lua:3518 — bitsDanger = bit.bor(SOUND_BULLET_IMPACT, SOUND_COMBAT, SOUND_WORLD, SOUND_DANGER) — Phase 3 sound constants
            if (CanInvestigate
                && (HasCondition(Condition.HearBulletImpact)
                    || HasCondition(Condition.HearCombat)
                    || HasCondition(Condition.HearWorld)
                    || HasCondition(Condition.HearDanger))
                && NextInvestigationMove < curTime
                && TakingCoverT < curTime
                && !IsBusy())
            {
                // lua:3541: GetBestSoundHint(bitsDanger) — Source engine sound location
                var sdSrc = GetBestSoundHint(0); // SKIP: bitsDanger mask — Phase 3 sound system
                // lua:3542
                if (sdSrc != null)
                {
                    // lua:3544: allowed flag
                    bool allowed = true;
                    // SKIP: lua:3545-3553 — sdSrc.owner, .type, IsVehicle(), GetDriver(), Disposition checks — Phase 3 sound event system
                    // lua:3556-3560 — commented-out player sound check

                    // lua:3561-3574: Execute investigation
                    if (allowed)
                    {
                        DoReadyAlert();                              // lua:3562
                        StopMoving();                                 // lua:3563
                        // SKIP: lua:3564 — SetLastPosition(sdSrc.origin) — sdSrc is null (Phase 3 sound)
                        SCHEDULE_FACE("TASK_FACE_LASTPOSITION");     // lua:3565
                        // lua:3567-3571: commented-out custom schedule
                        // SKIP: lua:3572 — OnInvestigate(sdOwner) — sdOwner is null (Phase 3)
                        PlaySoundSystem("Investigate");              // lua:3573
                        TakingCoverT = curTime + 1;                  // lua:3574
                    }
                }
            }
        }
        // ═══ Combat Behavior (lua:3579-3819) ═══
        else
        {
            // lua:3581-3582: Get active weapon + enemy data
            var wep = GetActiveWeapon(); // SKIP: returns null — Phase 3 weapon system
            var eneData = Enemy;

            // ═══ C1: No valid weapon (lua:3585-3603) ═══
            if (!wep.IsValid())
            {
                // lua:3587-3594: Scared behavior (Weapon_UnarmedBehavior)
                if (Weapon_UnarmedBehavior)
                {
                    if (!IsBusy() && curTime > NextChaseTime)
                    {
                        Weapon_UnarmedBehavior_Active = true;               // lua:3589
                        if (!IsFollowing && eneData.Visible)
                        {
                            SCHEDULE_COVER_ENEMY("TASK_RUN_PATH");          // lua:3591
                            return;                                         // lua:3592
                        }
                    }
                }
                // lua:3596-3600: No scared behavior but has melee — chase
                else if (HasMeleeAttack)
                {
                    Weapon_UnarmedBehavior_Active = false;                  // lua:3597
                    NextDangerDetectionT = curTime + 4;                    // lua:3598
                    MaintainAlertBehavior();                                // lua:3599
                    return;                                                 // lua:3600
                }
                // lua:3602: Fallback — idle, then fall through to goto_conditions
                MaintainIdleBehavior(2);
                // lua:3603: //return — Allow other behaviors (COND_PLAYER_PUSHING) to run
            }
            // ═══ C2: Has valid weapon (lua:3604-3816) — ALL SKIP (wep is null, Phase 3) ═══
            else
            {
                if (wep == null) goto goto_conditions; // compiler guard (dead code)

                Weapon_UnarmedBehavior_Active = false;                      // lua:3605

                // SKIP: lua:3607 — enePos_Eye = ene:EyePos() — Phase 3 eye position
                var myPos = WorldPosition;                                  // lua:3608
                // SKIP: lua:3609 — myPosCentered = myPos + OBBCenter() — Phase 3 OBB
                var myPosCentered = myPos; // stub

                // --- lua:3611-3620: Retreat from too-close enemy ---
                // SKIP: lua:3612 — DoCoverTrace(myPosCentered, enePos_Eye), wep.IsMeleeWeapon, ene.Behavior — Phase 3 weapon + cover
                // SKIP: lua:3613 — VJ.TraceDirections(self, "Quick", 200, ...) — Phase 3 utility
                // SKIP: lua:3616 — GetWeaponState/SetWeaponState — Phase 3 weapon state
                // SKIP: lua:3618 — SCHEDULE_GOTO_POSITION("TASK_RUN_PATH", lambda) — Phase 3
                // SKIP: lua:3619 — goto goto_conditions — Phase 3

                // --- lua:3623-3651: CanFireWeapon checks + occlusion ---
                // SKIP: lua:3623-3629 — CanFireWeapon(false,false) / (true,true) — Phase 3 weapon
                // SKIP: lua:3625-3626 — Weapon_MaxDistance / NextWeaponAttackT check → MaintainAlertBehavior
                // SKIP: lua:3631 — DoCoverTrace(EyePos, enePos_Eye, true) occlusion check
                // SKIP: lua:3632 — TakingCoverT guard
                // SKIP: lua:3633-3650 — Weapon_OcclusionDelay / GetWeaponState / LastHiddenZoneT / GetUp / MaintainAlertBehavior / WeaponAttackState
                // SKIP: lua:3642 — goto goto_checkwep — Phase 3
                // SKIP: lua:3651 — goto goto_conditions — Phase 3

                // SKIP: lua:3654 — ::goto_checkwep:: label (C#: goto_checkwep:)
            goto_checkwep:
                _ = 0; // label anchor — Phase 3 weapon combat loop will start here
                // SKIP: lua:3655-3816 — VJ weapon combat loop — Phase 3 weapon system
                // C2c-i (lua:3655-3670): if wep.IsVJBaseWeapon → aim turning (FInAimCone, SetTurnTarget, UpdatePoseParamTracking)
                // C2c-ii (lua:3673-3734): cover/obstruction check (DoCoverTrace ×2, friendly-in-LOS reposition, behind-cover reposition, VJ.GetNearestPositions / NearestPoint, custom GOTO_POSITION schedule)
                // C2c-iii (lua:3737-3794): weapon attack (melee vs ranged, TranslateActivity/PICK/AnimExists/AnimDuration/PlayAnim, crouch fire, ammo check)
                // C2c-iv (lua:3797-3806): random strafing while shooting (VJ.TraceDirections Radial, OnWeaponStrafe)
                // C2c-v (lua:3808-3816): non-VJ weapons → SetSchedule(SCHED_RANGE_ATTACK1)
            }
        }

        // ═══ goto_conditions: Handle player pushing yield (lua:3821-3837) ═══
        goto_conditions:
        // lua:3823
        if (HasCondition(Condition.PlayerPushing) && curTime > TakingCoverT && !IsBusy("Activities"))
        {
            // lua:3824
            PlaySoundSystem("YieldToPlayer");

            // Build schedule_yield_player inline (lua:3512-3517 — module-level shared in Lua)
            var yieldSched = new AISchedule();
            yieldSched.Init("SCHEDULE_YIELD_PLAYER");
            yieldSched.EngTask(EngineTask.MoveAwayPath, 120);
            yieldSched.EngTask(EngineTask.RunPath, 0);
            yieldSched.EngTask(EngineTask.WaitForMovement, 0);
            yieldSched.CanShootWhenMoving = true;

            // lua:3825-3834: Set turn data based on enemy/target status
            if (eneValid)
            {
                // lua:3825-3826: Face current enemy (visible only)
                yieldSched.TurnData = new TurnData { Type = VJFaceStatus.EnemyVisible, Target = null };
            }
            else if (GetTarget().IsValid())
            {
                // lua:3828-3829: Face current target (visible only)
                yieldSched.TurnData = new TurnData { Type = VJFaceStatus.EntityVisible, Target = GetTarget() };
            }
            else
            {
                // lua:3831-3832: Reset turn data (nil = none)
                yieldSched.TurnData = new TurnData { Type = VJFaceStatus.None };
            }

            // lua:3835
            StartSchedule(yieldSched);
            TakingCoverT = curTime + 2; // lua:3836
        }
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
}
