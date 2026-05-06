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
}
