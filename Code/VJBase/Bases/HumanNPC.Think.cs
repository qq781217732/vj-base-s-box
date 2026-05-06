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
            // ═══ C2: Has valid weapon (lua:3604-3816) ═══
            else
            {
                if (wep == null) goto goto_conditions; // compiler guard (wep is null from stub)

                // lua:3605
                Weapon_UnarmedBehavior_Active = false;

                // lua:3607-3609: Position calculations
                // SKIP: lua:3607 — enePos_Eye = ene:EyePos() — Phase 3 eye position
                var myPos = WorldPosition;                                  // lua:3608
                // SKIP: lua:3609 — myPosCentered = myPos + OBBCenter() — Phase 3 OBB/GetBonePosition
                var myPosCentered = myPos;

                // ═══ C2a: Retreat from too-close enemy (lua:3611-3620) ═══
                // lua:3612: if eneData.Distance <= Weapon_RetreatDistance && !wep.IsMeleeWeapon
                //          && curTime > TakingCoverT && curTime > NextChaseTime && !AttackType
                //          && !IsFollowing && ene.Behavior != VJ_BEHAVIOR_PASSIVE
                //          && !DoCoverTrace(myPosCentered, enePos_Eye)
                // SKIP: lua:3612 — wep.IsMeleeWeapon, ene.Behavior, DoCoverTrace — Phase 3 weapon + cover
                {
                    // SKIP: lua:3613 — moveCheck = PICK(VJ.TraceDirections(self, "Quick", 200, true, false, 8, true)) — Phase 3 utility
                    // SKIP: lua:3614 — if moveCheck then
                    // SKIP: lua:3615 — SetLastPosition(moveCheck) — Phase 3
                    // SKIP: lua:3616 — GetWeaponState() == VJ.WEP_STATE_RELOADING → SetWeaponState() — Phase 3 weapon state
                    // SKIP: lua:3617 — TakingCoverT = curTime + 2
                    // SKIP: lua:3618 — SCHEDULE_GOTO_POSITION("TASK_RUN_PATH", function(x) x:EngTask("TASK_FACE_ENEMY", 0) x.CanShootWhenMoving = true x.TurnData = {Type = VJ.FACE_ENEMY} end)
                    // SKIP: lua:3619 — goto goto_conditions
                }

                // ═══ C2b: CanFireWeapon checks + occlusion (lua:3623-3651) ═══
                // lua:3623: if CanFireWeapon(false, false) && GetState() != VJ_STATE_ONLY_ANIMATION_NOATTACK then
                // SKIP: lua:3623 — CanFireWeapon(false, false) — Phase 3 weapon
                {
                    // lua:3625: if eneData.Distance > Weapon_MaxDistance or curTime < NextWeaponAttackT then
                    // SKIP: lua:3625 — Weapon_MaxDistance, NextWeaponAttackT — Phase 3 weapon config
                    // SKIP: lua:3626 — MaintainAlertBehavior() — Phase 3
                    // SKIP: lua:3627 — AllowWeaponOcclusionDelay = false

                    // lua:3629: elseif CanFireWeapon(true, true) then
                    // SKIP: lua:3629 — CanFireWeapon(true, true) — Phase 3 weapon
                    {
                        // lua:3631: if DoCoverTrace(EyePos, enePos_Eye, true) then
                        // SKIP: lua:3631 — DoCoverTrace(EyePos, enePos_Eye, true) — Phase 3 cover + eye position
                        {
                            // SKIP: lua:3632 — TakingCoverT > curTime → return — Phase 3
                            // lua:3633: if GetWeaponState() != VJ.WEP_STATE_RELOADING then
                            // SKIP: lua:3633 — GetWeaponState() — Phase 3 weapon state
                            {
                                // lua:3635-3638: Occlusion delay
                                // SKIP: lua:3635 — Weapon_OcclusionDelay && WeaponAttackState != VJ.WEP_ATTACK_STATE_AIM_OCCLUSION && !wep.IsMeleeWeapon && AllowWeaponOcclusionDelay && (curTime - WeaponLastShotTime) <= 4.5 && eneData.Distance > Weapon_OcclusionDelayMinDist
                                // SKIP: lua:3636 — WeaponAttackState = VJ.WEP_ATTACK_STATE_AIM_OCCLUSION
                                // SKIP: lua:3637 — MaintainIdleBehavior(2) → ACT_IDLE_ANGRY
                                // SKIP: lua:3638 — NextChaseTime = curTime + math.Rand(Weapon_OcclusionDelayTime.a, Weapon_OcclusionDelayTime.b)

                                // lua:3640-3641: Hidden zone stand-up
                                // SKIP: lua:3640 — curTime < LastHiddenZoneT && !DoCoverTrace(myPosCentered + GetUp()*30, enePos_Eye + GetUp()*30, true) — Phase 3 cover + GetUp
                                // SKIP: lua:3641 — MaintainIdleBehavior(2) → ACT_IDLE_ANGRY
                                // SKIP: lua:3642 — goto goto_checkwep

                                // lua:3643-3649: Everything failed → chase
                                // SKIP: lua:3645-3646 — WeaponAttackState >= VJ.WEP_ATTACK_STATE_FIRE && CurrentScheduleName != "SCHEDULE_ALERT_CHASE" → WeaponAttackState = VJ.WEP_ATTACK_STATE_NONE
                                // SKIP: lua:3648 — MaintainAlertBehavior() — Phase 3
                            }
                            // SKIP: lua:3651 — goto goto_conditions
                        }
                        // lua:3653: -- I can see the enemy...
                        // ═══ C2c: Enemy visible — weapon combat loop (lua:3654-3816) ═══
                        // lua:3654: ::goto_checkwep::
                    goto_checkwep:
                        _ = 0;

                        // lua:3655: if wep.IsVJBaseWeapon then — VJ Base weapons
                        // SKIP: lua:3655 — wep.IsVJBaseWeapon — Phase 3 weapon interface
                        {
                            // ═══ C2c-i: Aim turning (lua:3656-3670) ═══
                            // lua:3657: if !HasPoseParameterLooking then
                            // SKIP: lua:3657-3669 — HasPoseParameterLooking, FInAimCone, SetTurnTarget("Enemy"), GetAngles():Forward(), Dot, UpdatePoseParamTracking(true) — Phase 3 animation + turning
                            // lua:3671: // self:MaintainAlertBehavior() — commented out

                            // ═══ C2c-ii: Cover/obstruction check (lua:3673-3734) ═══
                            // lua:3673-3676: inCover + wepInCover via DoCoverTrace
                            // SKIP: lua:3673 — DoCoverTrace(myPosCentered, enePos_Eye, false, {SetLastHiddenTime = true}) — Phase 3 cover
                            // SKIP: lua:3675 — DoCoverTrace(wep:GetBulletPos(), enePos_Eye, false) — Phase 3 weapon + cover
                            // lua:3679-3682: inCoverEntLiving = IsValid(inCoverEnt) && inCoverEnt.VJ_ID_Living
                            // SKIP: lua:3679-3682 — inCoverEnt.VJ_ID_Living — Phase 3 entity flags

                            // lua:3683: if !wep.IsMeleeWeapon then — ranged weapons only
                            // SKIP: lua:3683 — wep.IsMeleeWeapon — Phase 3 weapon interface
                            {
                                // lua:3685-3693: Friendly in line of fire → move
                                // SKIP: lua:3685 — inCoverEntLiving && WeaponAttackState == VJ.WEP_ATTACK_STATE_FIRE_STAND && IsValid(wepInCoverEnt) && wepInCoverEnt:IsNPC() && Disposition checks — Phase 3
                                // SKIP: lua:3686 — moveCheck = PICK(VJ.TraceDirections(self, "Quick", 50, ...)) — Phase 3 utility
                                // SKIP: lua:3688-3692 — StopMoving / IsGuard guard data / SetLastPosition / SCHEDULE_GOTO_POSITION("TASK_WALK_PATH", lambda) — Phase 3

                                // lua:3697-3734: Behind cover logic
                                // SKIP: lua:3697 — if inCover then
                                // SKIP: lua:3699 — curTime < TakingCoverT → goto goto_conditions
                                // SKIP: lua:3701 — curTime > NextMoveOnGunCoveredT && distance > 150 || wepInCover → reposition
                                // SKIP: lua:3703-3710 — nearestPos/nearestEntPos via VJ.GetNearestPositions / NearestPoint — Phase 3 utility
                                // SKIP: lua:3716-3727 — custom vj_ai_schedule.New("SCHEDULE_GOTO_POSITION") + TranslateActivity(PICK(AnimTbl_MoveToCover)) + AnimExists + SetMovementActivity + StartSchedule — Phase 3 animation + schedule
                                // SKIP: lua:3730 — NextMoveOnGunCoveredT = curTime + 2
                                // SKIP: lua:3731 — return
                            }

                            // ═══ C2c-iii: Weapon attack (lua:3737-3794) ═══
                            // lua:3737: if curTime > NextWeaponAttackT && curTime > NextWeaponAttackT_Base then
                            // SKIP: lua:3737 — NextWeaponAttackT, NextWeaponAttackT_Base — Phase 3 weapon timers
                            {
                                // lua:3739-3751: Melee weapons
                                // SKIP: lua:3739 — wep.IsMeleeWeapon — Phase 3 weapon
                                // SKIP: lua:3740 — OnWeaponAttack() — Phase 3 weapon callback
                                // SKIP: lua:3741 — TranslateActivity(PICK(AnimTbl_WeaponAttack)) — Phase 3 animation
                                // SKIP: lua:3742-3743 — AnimExists + AnimDuration — Phase 3 animation
                                // SKIP: lua:3744 — wep.NPC_NextPrimaryFire = animDur — Phase 3 weapon
                                // SKIP: lua:3745 — wep:NPCShoot_Primary() — Phase 3 weapon
                                // SKIP: lua:3746 — VJ.EmitSound(self, wep.NPC_BeforeFireSound, ...) — Phase 3 sound
                                // SKIP: lua:3747 — NextMeleeWeaponAttackT = curTime + animDur
                                // SKIP: lua:3748 — WeaponAttackAnim = finalAnim
                                // SKIP: lua:3749 — PlayAnim(finalAnim, "LetAttacks", false, true) — Phase 3 animation
                                // SKIP: lua:3750 — WeaponAttackState = VJ.WEP_ATTACK_STATE_FIRE_STAND

                                // lua:3753-3793: Ranged weapons
                                // SKIP: lua:3754 — AllowWeaponOcclusionDelay = true
                                // SKIP: lua:3755 — hasAmmo = wep:Clip1() > 0 — Phase 3 weapon
                                // SKIP: lua:3756-3757 — !hasAmmo && WeaponAttackState != VJ.WEP_ATTACK_STATE_AIM → WeaponAttackAnim = ACT_INVALID
                                // SKIP: lua:3760 — VJ.IsCurrentAnim(self, TranslateActivity(WeaponAttackAnim)) — Phase 3 animation
                                // SKIP: lua:3763 — GetActivity() != WeaponAttackAnim && GetActivity() != ACT_TRANSITION — Phase 3 animation
                                // SKIP: lua:3764 — OnWeaponAttack() — Phase 3 weapon callback
                                // SKIP: lua:3765-3766 — WeaponAttackState == VJ.WEP_ATTACK_STATE_AIM_OCCLUSION → WeaponAttackState = VJ.WEP_ATTACK_STATE_NONE
                                // SKIP: lua:3768 — WeaponLastShotTime = curTime
                                // SKIP: lua:3772-3775 — !hasAmmo → MaintainIdleBehavior(2) + WeaponAttackState = VJ.WEP_ATTACK_STATE_AIM
                                // SKIP: lua:3778-3783 — TranslateActivity(PICK(AnimTbl_WeaponAttackCrouch)) vs TranslateActivity(PICK(AnimTbl_WeaponAttack)) — Phase 3 animation
                                // SKIP: lua:3779 — crouch condition: Weapon_CanCrouchAttack && !inCover && !wepInCover && distance > 500 && AnimExists + random + DoCoverTrace — Phase 3
                                // SKIP: lua:3786-3792 — AnimExists + VJ.EmitSound + PlayAnim + WeaponAttackAnim + WeaponAttackState + NextWeaponAttackT_Base — Phase 3 animation + sound
                            }

                            // ═══ C2c-iv: Random strafing while shooting (lua:3797-3806) ═══
                            // lua:3797: if Weapon_Strafe && !inCover && !IsGuard && !IsFollowing && !wep.IsMeleeWeapon
                            //          && (!wep.NPC_StandingOnly) && WeaponAttackState == VJ.WEP_ATTACK_STATE_FIRE_STAND
                            //          && curTime > NextWeaponStrafeT && (curTime - eneData.TimeAcquired) > 2
                            //          && eneData.Distance < (Weapon_MaxDistance / 1.25)
                            // SKIP: lua:3797 — all weapon/cover/state checks — Phase 3 weapon + cover
                            {
                                // SKIP: lua:3798 — OnWeaponStrafe() != false — Phase 3 weapon callback
                                // SKIP: lua:3799 — moveCheck = PICK(VJ.TraceDirections(self, "Radial", math.random(150, 400), true, false, 12, true)) — Phase 3 utility
                                // SKIP: lua:3801-3803 — StopMoving / SetLastPosition / SCHEDULE_GOTO_POSITION(random walk/run, lambda with FACE_ENEMY) — Phase 3
                                // SKIP: lua:3806 — NextWeaponStrafeT = curTime + math.Rand(Weapon_StrafeCooldown.a, Weapon_StrafeCooldown.b)
                            }

                            // ═══ C2c-v: Non-VJ weapons (lua:3808-3816) ═══
                            // lua:3808: else — None VJ Base weapons
                            // SKIP: lua:3809 — SetTurnTarget("Enemy") — Phase 3 turning
                            // SKIP: lua:3810 — WeaponAttackState = VJ.WEP_ATTACK_STATE_FIRE_STAND
                            // SKIP: lua:3811 — OnWeaponAttack() — Phase 3 weapon callback
                            // SKIP: lua:3812 — WeaponLastShotTime = curTime
                            // SKIP: lua:3814 — self:SetSchedule(SCHED_RANGE_ATTACK1) — Source engine schedule, no S&Box equivalent
                        }
                    }
                }
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

    // ═══ OnTakeDamage — human_base/init.lua:3918-4172 ═══
    /// <summary>Source engine damage callback. Returns 0 to block damage, 1 to allow.</summary>
    public virtual int OnTakeDamage(object dmginfo, int hitgroup)
    {
        // ---- Block A: Entry guards (lua:3918-3923) ----
        // lua:3919-3920: Get/validate attacker
        // SKIP: lua:3919 — dmginfo:GetAttacker() → dmgAttacker — Phase 3 S&Box DamageInfo
        // SKIP: lua:3920 — !IsValid(dmgAttacker) → dmgAttacker = false
        // lua:3922-3923: Friendly bullet damage filter
        // SKIP: lua:3923 — dmgAttacker && dmginfo:IsBulletDamage() && dmgAttacker:IsNPC()
        //        && dmgAttacker:Disposition(self) != D_HT
        //        && (dmgAttacker:GetClass() == self:GetClass() || self:Disposition(dmgAttacker) == D_LI)
        //        → return 0 — Phase 3 damage system + entity type

        // ---- Block B: Inflictor + ragdoll guard (lua:3925-3929) ----
        // lua:3925-3926: Get/validate inflictor
        // SKIP: lua:3925 — dmginfo:GetInflictor() → dmgInflictor — Phase 3 S&Box DamageInfo
        // SKIP: lua:3929 — dmgInflictor && dmgInflictor:GetClass() == "prop_ragdoll"
        //        && dmgInflictor:GetVelocity():Length() <= 100 → return 0 — Phase 3 entity type

        // ---- Block C: Init + Guard (lua:3931-3934) ----
        // lua:3932: hitgroup already passed as parameter (GetLastDamageHitGroup in Lua)
        // lua:3933
        OnDamaged(dmginfo, hitgroup, "Init");
        // lua:3934: GodMode or damage <= 0 → skip
        // SKIP: lua:3934 — GodMode || dmginfo:GetDamage() <= 0 → return 0 — Phase 3 godmode + damage info

        // ---- Block D: Fire entity detection (lua:3936-3942) ----
        // SKIP: lua:3939 — self:IsOnFire() → isFireEnt detection — Phase 3 fire system
        // SKIP: lua:3940 — dmgInflictor:GetClass() == "entityflame" + dmgAttacker:GetClass() — Phase 3
        // SKIP: lua:3941 — WaterLevel() > 1 → self:Extinguish() — Phase 3 fire/water
        bool isFireEnt = false; // stub

        // ---- Block E: Boss bypass (lua:3944-3947) ----
        // lua:3945: if ForceDamageFromBosses && dmgAttacker.VJ_ID_Boss → goto skip_immunity
        // SKIP: lua:3945-3946 — dmgAttacker.VJ_ID_Boss → goto skip_immunity — Phase 3 boss flag

        // ---- Block F: Immunity chain (lua:3949-3951) ----
        // lua:3950: Fire ignition block
        // SKIP: lua:3950 — isFireEnt && !AllowIgnition → Extinguish() + return 0 — Phase 3 fire
        // lua:3951: Full immunity OR-chain (Fire/Toxic/Bullet/Explosive/Dissolve/Electricity/Melee/Sonic)
        // SKIP: lua:3951 — Immune_Fire && (DMG_BURN||DMG_SLOWBURN||isFireEnt)
        //        || Immune_Toxic && (DMG_ACID||RADIATION||POISON||NERVEGAS||PARALYZE)
        //        || Immune_Bullet && (IsBulletDamage||DMG_BULLET||AIRBOAT||BUCKSHOT||SNIPER)
        //        || Immune_Explosive && (DMG_BLAST||BLAST_SURFACE||MISSILEDEFENSE)
        //        || Immune_Dissolve && IsDamageType(DMG_DISSOLVE)
        //        || Immune_Electricity && (DMG_SHOCK||ENERGYBEAM||PHYSGUN)
        //        || Immune_Melee && (DMG_CLUB||SLASH)
        //        || Immune_Sonic && DMG_SONIC → return 0
        //        — Phase 3 damage type tags (use VJDamageTags in future)

        // ---- Block G: skip_immunity label + combine ball (lua:3953-3964) ----
        skip_immunity:
        // SKIP: lua:3954-3963 — combine ball class check / Immune_Dissolve / NextCombineBallDmgT
        //        dmginfo:SetDamage / dmginfo:SetDamageType(DMG_DISSOLVE) — Phase 3 damage system

        // ---- Block H: DoBleed helper (lua:3966-3974) ----
        void DoBleed()
        {
            // lua:3967
            if (Bleeds)
            {
                // lua:3968
                OnBleed(dmginfo, hitgroup);
                // lua:3970: SpawnBloodParticles — Phase 3 blood effects
                // SKIP: lua:3970 — HasBloodParticle && !isFireEnt → SpawnBloodParticles(dmginfo, hitgroup)
                // lua:3971: SpawnBloodDecals — Phase 3 blood effects
                // SKIP: lua:3971 — HasBloodDecal → SpawnBloodDecals(dmginfo, hitgroup)
                // lua:3972
                PlaySoundSystem("Impact");
            }
        }

        // ---- Block I: Dead guard (lua:3975) ----
        // lua:3975
        if (Dead) { DoBleed(); return 0; }

        // ---- Block J: PreDamage + damage application (lua:3977-4000) ----
        // lua:3977
        OnDamaged(dmginfo, hitgroup, "PreDamage");
        // SKIP: lua:3978 — dmginfo:GetDamage() <= 0 → return 0 — Phase 3 damage info
        // SKIP: lua:3980-3990 — SavedDmgInfo table (dmginfo, attacker, inflictor, amount, pos, type, force, ammoType, hitgroup) — Phase 3
        // SKIP: lua:3991 — self:SetHealth(self:Health() - dmginfo:GetDamage()) — Phase 3 HealthComponent
        // SKIP: lua:3992 — VJ_DEBUG damage print — Phase 3 debug
        // SKIP: lua:3993-3995 — HealthRegenParams.Enabled && ResetOnDmg → HealthRegenDelayT = curTime + ... — Phase 3 regen
        // SKIP: lua:3997-3998 — SetSaveValue("m_iDamageCount"/"m_flLastDamageTime") — Phase 3 save/restore

        // lua:3999
        OnDamaged(dmginfo, hitgroup, "PostDamage");
        // lua:4000
        DoBleed();

        // ---- Block K: I/O events (lua:4002-4008) ----
        // SKIP: lua:4003-4007 — TriggerOutput("OnDamaged", ...) / MarkTookDamageFromEnemy(dmgAttacker) — Phase 3 I/O system

        // ---- Block L: Pain sounds (lua:4010-4011) ----
        // SKIP: lua:4010 — stillAlive = self:Health() > 0 — Phase 3 HealthComponent
        // lua:4011
        PlaySoundSystem("Pain");

        // ---- Block M: AI response (lua:4013-4151) ----
        // lua:4013: if VJ_CVAR_AI_ENABLED && GetState() != VJ_STATE_FREEZE
        // SKIP: VJ_CVAR_AI_ENABLED — Phase 3 convar system (stub: always enabled for now)
        if (GetState() != VJState.Freeze)
        {
            // lua:4014
            var isPassive = Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature;

            // M1: Flinch (lua:4016-4017)
            // SKIP: lua:4016 — !isFireEnt check — Phase 3
            // lua:4017
            Flinch(dmginfo, hitgroup);

            // M2: Player attacker → BecomeEnemyToPlayer (lua:4020-4052)
            // SKIP: lua:4021-4051 — dmgAttacker:IsPlayer() / CheckRelationship / SetRelationshipMemory
            //        / BecomeEnemyToPlayer hostility counter / OnBecomeEnemyToPlayer
            //        / ResetFollowBehavior / AddEntityRelationship(D_HT)
            //        / PlaySoundSystem("BecomeEnemyToPlayer") / StopMoving / SetTarget / SCHEDULE_FACE
            //        / CanChatMessage / PrintMessage / DamageByPlayer sounds
            //        — Phase 3 player system + relationship

            // M3: Combat damage response — take cover from visible enemy (lua:4056-4076) [HUMAN ONLY]
            // SKIP: lua:4056-4076 — CombatDamageResponse / DoCoverTrace / AnimTbl_TakingCover
            //        / PlayAnim / SCHEDULE_COVER_ENEMY / funcGetActiveWeapon / IsMoving
            //        — Phase 3 cover + weapon system

            // M4: No enemy response — ally alert + damage response (lua:4078-4128)
            // SKIP: lua:4078-4128 — DamageAllyResponse / Allies_Check / Allies_Bring
            //        / AnimTbl_DamageAllyResponse / DoReadyAlert / PlayAnim
            //        / DamageResponse → CheckRelationship → ForceSetEnemy / SCHEDULE_COVER_ORIGIN
            //        — Phase 3 ally system + damage response

            // M5: Passive NPC run away (lua:4130-4134)
            // SKIP: lua:4131-4134 — isPassive + TakingCoverT + DamageResponse → SCHEDULE_COVER_ORIGIN — Phase 3
        }

        // M6: Passive allies signal danger (lua:4138-4151) — OUTSIDE stillAlive block
        // SKIP: lua:4138-4151 — isPassive + TakingCoverT + Passive_AlliesRunOnDamage
        //        + Allies_Check + allies:SCHEDULE_COVER_ORIGIN + ally:PlaySoundSystem("Alert")
        //        — Phase 3 ally system

        // ---- Block N: Stop eating (lua:4154-4158) ----
        // SKIP: lua:4155-4157 — CanEat && VJ_ST_Eating → EatingData.NextCheck + ResetEatingBehavior("Injured") — Phase 3 eating

        // ---- Block O: Death (lua:4160-4171) ----
        // SKIP: lua:4160 — self:Health() <= 0 && !Dead — Phase 3 HealthComponent
        // SKIP: lua:4161 — RemoveEFlags(EFL_NO_DISSOLVE) — Phase 3 flags
        // SKIP: lua:4162-4168 — IsDamageType(DMG_DISSOLVE) / dissolve DamageInfo / TakeDamageInfo — Phase 3
        // SKIP: lua:4169 — BeginDeath(dmginfo, hitgroup) — Phase 3 (exists as stub in CreatureNPC.Think.cs)

        // lua:4171
        return 1;
    }
}
