using System;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using SWB.Player;

namespace VJBase;

/// <summary>
/// CreatureNPC Think + SelectSchedule — ported from npc_vj_creature_base/init.lua.
/// </summary>
public partial class CreatureNPC
{
    // ═══ Additional Fields ═══
    public float NextProcessTime { get; set; } = 0.1f;

    // ═══ OnUpdate: drives Think loop every frame ═══
    protected override void OnUpdate()
    {
        if ( !Dead )
            Think();
    }

    // ═══ Think — main AI loop ═══
    public virtual void Think()
    {
        var curTime = Time.Now;
        bool doHeavyProcesses = curTime > NextProcessT;
        if (doHeavyProcesses)
            NextProcessT = curTime + NextProcessTime;

        // Footstep sounds — core.lua:1131 (shared Think)
        if (!DisableFootStepSoundTimer)
            PlayFootstepSound();

        // Health regen — human init.lua:2706-2710 / creature init.lua:2013-2017
        if (!Dead && HealthRegenEnabled && curTime > HealthRegenDelayT)
        {
            CurrentHealth = MathF.Min(CurrentHealth + HealthRegenAmount, StartHealth);
            HealthRegenDelayT = curTime + VJUtility.Rand(HealthRegenDelay.a, HealthRegenDelay.b);
        }

        // Breath sounds — core.lua creature init:1879-1889
        if (!Dead && HasBreathSound && HasSounds && curTime > NextBreathSoundT)
        {
            var pickedSD = PickSound(SoundTbl_Breath);
            if (pickedSD != null)
            {
                StopSD(CurrentBreathSound);
                CurrentBreathSound = CreateSound(pickedSD, BreathSoundLevel, GetSoundPitch(BreathSoundPitch));
            }
            NextBreathSoundT = curTime + NextSoundTime_Breath;
        }

        OnThink();
        // lua:1896-1899 — VJ_DEBUG per-frame: enemy/cover state
        if (VJ_DEBUG)
        {
            if ((DebugFlags & VJDebugFlags.Enemy) != 0)
                VJDebug.Print(GameObject, null, null, "Enemy ->", GetEnemy()?.Name ?? "NULL", "| Alerted?", Alerted);
            if ((DebugFlags & VJDebugFlags.TakingCover) != 0)
            {
                if (curTime > TakingCoverT)
                    VJDebug.Print(GameObject, null, null, "NOT taking cover");
                else
                    VJDebug.Print(GameObject, null, null, "Taking cover (" + (TakingCoverT - curTime) + ")");
            }
        }
        OnThinkActive();

        // Process attack timers — check reset/re-enable/bleed polling fields
        ProcessAttackTimers(curTime);

        // Timer polling: alert reset (Source timer.Create → polling)
        if (NextAlertResetT > 0 && curTime > NextAlertResetT)
        {
            NextAlertResetT = 0;
            if (!GetEnemy().IsValid()) { Alerted = (int)VJAlertState.None; SetNPCState((int)NPCState.Idle); }
        }

        // Timer polling: death delay (Source timer.Simple → polling)
        if (NextDeathFinishT > 0 && curTime > NextDeathFinishT)
        {
            NextDeathFinishT = 0;
            if (PendingDeathDmgInfo != null)
                FinishDeath(PendingDeathDmgInfo, PendingDeathHitgroup);
        }

        var moveType = MovementType;
        bool isAA = moveType == VJMoveType.Aerial || moveType == VJMoveType.Aquatic;

        // Perception — Engine/AISenses produces conditions into BaseNPC
        if (doHeavyProcesses)
            TickSenses();

        // lua:1906-1942 — AA per-frame velocity tracking + acceleration + animation
        if (isAA)
        {
            var rb = Components.Get<Rigidbody>();
            var myVelLen = rb?.Velocity.Length ?? 0f;
            if (myVelLen > 0.1f)
            {
                // lua:1909-1930 — progress tracking & acceleration lerp toward target
                if (AA_CurrentMovePos.HasValue)
                {
                    var dist = AA_CurrentMovePos.Value.Distance(WorldPosition);
                    if (AA_CurrentMoveDist < 0 || AA_CurrentMoveDist >= dist)
                    {
                        AA_CurrentMoveDist = dist;
                        var moveSpeed = AA_CurrentMoveMaxSpeed;
                        // lua:1916-1917 — decelerate approaching target
                        if (AA_MoveDecelerate > 1 && dist < moveSpeed)
                        {
                            moveSpeed = Math.Clamp(moveSpeed / AA_MoveDecelerate, dist, moveSpeed);
                        }
                        // lua:1918-1919 — acceleration lerp
                        else if (AA_MoveAccelerate > 0 && rb != null)
                        {
                            moveSpeed = MathX.Lerp(myVelLen, moveSpeed, Time.Delta * AA_MoveAccelerate);
                        }
                        // lua:1921-1925 — velocity + time
                        var velDir = AA_CurrentMovePosDir ?? Vector3.Zero;
                        var vlen = velDir.Length;
                        if (vlen > 0.001f) velDir /= vlen;
                        var velPos = velDir * moveSpeed;
                        var velTimeCur = curTime + dist / velPos.Length;
                        if (!float.IsNaN(velTimeCur))
                            AA_CurrentMoveTime = velTimeCur;
                        // lua:1926 — SetLocalVelocity
                        if (rb != null) rb.Velocity = velPos;
                    }
                    else
                    {
                        // lua:1928-1929 — stuck, not making progress
                        AA_StopMoving();
                    }
                }

                // lua:1932-1934 — aquatic not fully submerged → wander down
                if (moveType == VJMoveType.Aquatic && WaterLevel() <= 2)
                    AA_IdleWander();

                // lua:1936-1938 — update movement animation
                // lua:1936 — AA_CurrentMoveAnim != -1 (false=none→animate, -1=skip, string seq name→animate)
                if (!(AA_CurrentMoveAnim is int i && i == -1))
                    AA_MoveAnimation();
            }
            else
            {
                // lua:1941 — not moving, reset move time
                AA_CurrentMoveTime = 0;
            }
        }

        // lua:1948-1999 — Follow system update
        if (IsFollowing && NavTypeVal != (int)NavType.Jump && NavTypeVal != (int)NavType.Climb)
        {
            var followEnt = Follow.Target;
            if (followEnt.IsValid())
            {
                var followLiving = HasEntityFlag(followEnt, "VJ_ID_Living");
                if (!followLiving || (followLiving && (Disposition(followEnt) == (int)VJBase.Disposition.Like
                    || (followEnt.Components.Get<BaseNPC>()?.VJ_NPC_Class.Any(c => VJ_NPC_Class.Contains(c)) ?? false))
                    && Alive(followEnt)))
                {
                    // lua:1954 — time gate + VJ_ST_Healing guard
                    if (curTime > Follow.NextUpdateT && !VJ_ST_Healing)
                    {
                        var dist = WorldPosition.Distance(followEnt.WorldPosition);
                        var busy = IsBusy("Activities");
                        SetTarget(followEnt);
                        Follow.StopAct = false;
                        // lua:1959 — entity is far away, move towards it
                        if (dist > Follow.MinDist)
                        {
                            bool isFar = dist > (Follow.MinDist * 4);
                            // lua:1961-1962 — IF (busy but far) OR (not busy) THEN move
                            if ((busy && isFar) || !busy)
                            {
                                Follow.Moving = true;
                                // lua:1965-1967 — if far: stop all activities (attacks, etc.) and just go
                                if (isFar)
                                    Follow.StopAct = true;
                                if (isAA)
                                {
                                    var aaOpts = new Dictionary<string, object>
                                    {
                                        ["FaceDestTarget"] = true
                                    };
                                    AA_MoveTo(followEnt, true, dist < Follow.MinDist * 1.5f ? "Calm" : "Alert", aaOpts);
                                }
                                // lua:1970-1983 — ground Nav: full schedule with engine tasks
                                else if (!IsMoving() || GetCurGoalType() != 1)
                                {
                                    var schedule = new AISchedule();
                                    schedule.Init("SCHEDULE_FOLLOW");
                                    schedule.EngTask("TASK_GET_PATH_TO_TARGET", 0);
                                    schedule.EngTask("TASK_MOVE_TO_TARGET_RANGE", Follow.MinDist * 0.8f);
                                    schedule.EngTask("TASK_WAIT_FOR_MOVEMENT", 0);
                                    schedule.EngTask("TASK_FACE_TARGET", 1);
                                    schedule.CanShootWhenMoving = true;
                                    if (GetActiveWeapon().IsValid())
                                        schedule.TurnData = new TurnData { Type = VJFaceStatus.EnemyVisible };
                                    StartSchedule(schedule);
                                }
                            }
                        }
                        // lua:1995-2001 — entity is close, stop moving if not busy
                        else if (Follow.Moving)
                        {
                            if (!busy)
                            {
                                OnTaskComplete();
                                StopMoving();
                                SelectSchedule();
                            }
                            Follow.Moving = false;
                        }
                    }
                }
                else
                {
                    ResetFollowBehavior();
                }
            }
        }

        RunAI();

        // ═══ Weapon NPC_Think — auto-fire loop (VJBaseWeapon.NPC_Think) ═══
        // Called after RunAI so SelectSchedule's C2c-iii has already set WeaponAttackState=FireStand.
        var activeWep = GetActiveWeapon();
        if (activeWep.IsValid())
        {
            var wepThink = activeWep.Components.Get<VJBaseWeapon>();
            if (wepThink != null)
                wepThink.NPC_Think();
        }

        // ═══ Animation Think hook — replaces Lua hook.Add("Think", funcAnimThink) ═══
        // Called every frame to maintain idle animation cycling.
        // Phase 3: gate on VJ_CVAR_AI_ENABLED convar (assume enabled for now)
        MaintainIdleAnimation(false);

        // Ground NPC move animation: bridges NavMeshAgent.Velocity → animation selection.
        // Source engine handled this via SetIdealActivity + SelectWeightedSequence in C++.
        // Aerial/Aquatic NPCs use AA_MoveAnimation (called from the Think loop above).
        if (MovementType == VJMoveType.Ground)
            UpdateGroundMoveAnimation();
    }

    protected virtual void OnThink() { }
    protected virtual void OnThinkActive() { }

    // ═══ RunAI — main AI loop, ported from schedules.lua ENT:RunAI ═══
    protected virtual void RunAI()
    {
        // Freeze state → maintain activity only
        if (GetState() == VJState.Freeze) return;

        // Engine schedule running → skip VJ logic
        if (bDoingEngineSchedule) return;

        // Auto-movement: Source engine used GetSequenceMoveDist + AutoMovement to advance NPC
        // position based on walk/run sequence frame data. S&Box NavMeshAgent handles movement;
        // animation selection is driven by UpdateGroundMoveAnimation() in the Think loop.

        var curSchedule = CurrentSchedule;
        if (curSchedule != null)
        {
            DoSchedule(curSchedule);

            // Check if schedule should end
            if (curSchedule.CanBeInterrupted
                || IsScheduleFinished(curSchedule)
                || (curSchedule.HasMovement && !IsMoving()))
            {
                SelectSchedule();
            }
        }
        else
        {
            SelectSchedule();
        }

        // Turn / facing system
        var ene = GetEnemy();
        bool eneValid = ene.IsValid();

        if (eneValid && !Dead)
        {
            if (ConstantlyFaceEnemy)
            {
                SetTurnTarget("Enemy");
                return;
            }

            // Face enemy for stationary or attacking NPCs
            bool shouldFace = (MovementType == VJMoveType.Stationary && CanTurnWhileStationary)
                || (AttackType == VJAttackType.Melee && MeleeAttackAnimationFaceEnemy && !MeleeAttack_IsPropAttack)
                || (AttackType == VJAttackType.Grenade && GrenadeAttackAnimationFaceEnemy && Enemy.Visible)
                || (AttackType == VJAttackType.Range && RangeAttackAnimationFaceEnemy);

            if (shouldFace)
            {
                SetTurnTarget("Enemy");
                return;
            }
        }

        // lua:208 — self:MaintainActivity(); Phase 3 animation (activity/pose update, stub wired)
        MaintainActivity();

        MaintainTurnTarget();
    }

    // ═══ SelectSchedule — decides what to do next based on conditions ═══
    public virtual void SelectSchedule()
    {
        if (VJ_IsBeingControlled || Dead) return;

        var curTime = Time.Now;
        var ene = GetEnemy();
        bool eneValid = ene.IsValid();

        // Player pushing → yield
        if (HasCondition(Condition.PlayerPushing) && curTime > TakingCoverT)
        {
            TakingCoverT = curTime + 2;
            // Start yield schedule
        }

        if (eneValid)
        {
            // Has visible enemy → chase/attack
            MaintainAlertBehavior(false);
        }
        else if (Alerted != VJAlertState.None)
        {
            // Alerted but no enemy — investigate or idle
            bool shouldInvestigate = CanInvestigate
                && (HasCondition(Condition.HearBulletImpact)
                    || HasCondition(Condition.HearCombat)
                    || HasCondition(Condition.HearWorld)
                    || HasCondition(Condition.HearDanger))
                && TakingCoverT < curTime;

            if (shouldInvestigate)
            {
                DoReadyAlert();
                StopMoving();
                TakingCoverT = curTime + 1;
            }

            MaintainIdleBehavior();
        }
        else
        {
            // Not alerted — normal idle
            TakingCoverT = 0;
            MaintainIdleBehavior();
        }
    }

    // ═══ MaintainAlertBehavior — creature_base/init.lua:1760-1797 ═══
    public override void MaintainAlertBehavior(bool alwaysChase)
    {
        var ene = GetEnemy();
        if (!ene.IsValid()) return;

        // lua:1763-1765 — stationary / following / medic / only-animation → force idle
        if (MovementType == VJMoveType.Stationary || IsFollowing || Medic.Status != "false"
            || GetState() == VJState.OnlyAnimation)
        {
            SCHEDULE_IDLE_STAND();
            return;
        }

        // lua:1768-1773 — Passive / PassiveNature → cover + delay
        if (Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature)
        {
            SCHEDULE_COVER_ENEMY("TASK_RUN_PATH");
            NextChaseTime = Time.Now + 3;
            return;
        }

        // lua:1776 — !alwaysChase && (DisableChasingEnemy or IsGuard) → idle
        if (!alwaysChase && (DisableChasingEnemy || IsGuard)) { SCHEDULE_IDLE_STAND(); return; }

        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        {
            AA_ChaseEnemy(true, "Alert");
            return;
        }

        // lua:1779 — If the enemy is not reachable then wander around
        if (IsUnreachable(ene))
        {
            // lua:1780-1781 — HasRangeAttack → LOS chase
            if (HasRangeAttack)
            {
                SCHEDULE_ALERT_CHASE(true);
            }
            // lua:1782-1785 — 1/30 chance + not moving → wander + remember unreachable for 4s
            else if (Game.Random.Next(1, 31) == 1 && !IsMoving())
            {
                NextWanderTime = 0;
                MaintainIdleBehavior(1);
                RememberUnreachable(ene, 4);
            }
            // lua:1787 — fallback: idle stand
            else
            {
                SCHEDULE_IDLE_STAND();
            }
        }
        // lua:1789 — Is reachable, so chase the enemy!
        else
        {
            SCHEDULE_ALERT_CHASE(false);
        }

        // lua:1793-1796 — Set next chase time (don't override if already set)
        var enemyDist = ene.WorldPosition.Distance(WorldPosition);
        NextChaseTime = Time.Now + (enemyDist > 2000 ? 1f : 0.1f);
    }

    // ═══ SCHEDULE_ALERT_CHASE — creature_base/init.lua:1724 ═══
    public virtual void SCHEDULE_ALERT_CHASE(bool doLOSChase)
    {
        // init.lua:1726: self:ClearCondition(COND_ENEMY_UNREACHABLE)
        ClearCondition(Condition.EnemyUnreachable);

        // init.lua:1728-1730: AA branch
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        { AA_ChaseEnemy(); return; }

        // init.lua:1732: self.CurrentScheduleName guard
        if (CurrentScheduleName == "SCHEDULE_ALERT_CHASE") return;

        // init.lua:1734: navtype check
        int navType = GetNavType();
        if (navType == (int)NavType.Jump || navType == (int)NavType.Climb) return;

        var sched = new AISchedule();
        sched.Init("SCHEDULE_ALERT_CHASE");

        // init.lua:1736-1737
        if (doLOSChase)
        {
            // init.lua:1729-1743 — LOS chase with re-chase loop
            sched.EngTask(EngineTask.GetPathToEnemyLOS, 0);
            sched.EngTask(EngineTask.WaitForMovement, 0);
            sched.CanShootWhenMoving = true;
            sched.CanBeInterrupted = true;
            sched.RunCodeOnFinish = () =>
            {
                var ene = GetEnemy();
                if (ene.IsValid())
                    SCHEDULE_ALERT_CHASE(false);
            };
        }
        else
        {
            sched.EngTask(EngineTask.GetPathToEnemy, 0);
            sched.EngTask(EngineTask.RunPath, 0);
            sched.EngTask(EngineTask.WaitForMovement, 0);
        }

        StartSchedule(sched);
    }

    // ═══ ExecuteMeleeAttack — creature_base/init.lua:2449-2577 ═══
    public virtual void ExecuteMeleeAttack(bool isPropAttack)
    {
        // lua:2451: guard
        if (Dead || PauseAttacks || Flinching || (MeleeAttackStopOnHit && AttackState == VJAttackState.ExecutedHit)) return;
        // lua:2452: isPropAttack = isPropAttack or self.MeleeAttack_IsPropAttack
        isPropAttack = isPropAttack || MeleeAttack_IsPropAttack;
        // lua:2453: OnMeleeAttackExecute("Init")
        var skip = OnMeleeAttackExecute("Init");
        var hitRegistered = false;
        if (!skip)
        {
            var myPos = WorldPosition;
            // lua:2462: FindInSphere loop
            var traceOrigin = MeleeAttackTraceOrigin();
            var traceDir = MeleeAttackTraceDirection();
            var hits = Scene.FindInPhysics(new Sphere(traceOrigin, MeleeAttackDamageDistance > 0 ? MeleeAttackDamageDistance : 80));
            foreach (var ent in hits)
            {
                // lua:2463: ent == self or ent:GetClass() == myClass → skip self, skip same NPC type
                if (ent == GameObject) continue;
                var entBase1 = ent.Components.Get<BaseNPC>();
                if (entBase1 != null && entBase1.VJ_NPC_Class.Any(c => VJ_NPC_Class.Contains(c))) continue;
                // SKIP: lua:2463 — IsVJBaseBullseye flag — Phase 3 bullseye system
                // lua:2464 — skip VJ_IsControllingNPC, skip player when VJ_IsControllingNPC / dead / ignoreplayers
                bool isPlayer = ent.Components.Get<PlayerBase>() != null;
                if (isPlayer)
                {
                    // PX: ent.VJ_IsControllingNPC — Source player field, no S&Box equivalent
                    if (!Alive(ent) || VJInit.vj_npc_ignoreplayers) continue;
                }
                else if (entBase1?.VJ_IsBeingControlled == true) continue;
                // lua:2465 — ((VJ_ID_Living && Disp != D_LI) || VJ_ID_Attackable || VJ_ID_Destructible) && angle
                bool isLiving = HasEntityFlag(ent, "VJ_ID_Living");
                if (isPlayer && !isLiving) isLiving = true; // players are living targets by default
                bool isAttackable = HasEntityFlag(ent, "VJ_ID_Attackable");
                bool isDestructible = HasEntityFlag(ent, "VJ_ID_Destructible");
                var delta = new Vector3(ent.WorldPosition.x - myPos.x, ent.WorldPosition.y - myPos.y, 0);
                bool inAngle = traceDir.Dot(delta.Normal) > MathF.Cos(MathF.PI / 180f * MeleeAttackDamageAngleRadius);
                if (((isLiving && Disposition(ent) != (int)VJBase.Disposition.Like) || isAttackable || isDestructible) && inAngle)
                {
                    // lua:2466: prop attack living distance check
                    if (VJUtility.GetNearestDistance(GameObject, ent, true) > MeleeAttackDistance) continue;
                    var applyDmg = true;
                    // lua:2468 — VJ_ID_Attackable → isProp
                    bool isProp = isAttackable;
                    // lua:2469: OnMeleeAttackExecute("PreDamage")
                    if (OnMeleeAttackExecute("PreDamage", ent, isProp)) continue;
                    var dmgAmount = ScaleByDifficulty(MeleeAttackDamage);
                    // lua:2472-2496: Prop interaction block
                    if (isProp)
                    {
                        bool piBool = PropInteraction is bool b && b;
                        string piStr = PropInteraction as string;
                        if (PropInteraction == null || PropInteraction.Equals(false))
                            applyDmg = false;
                        else if (piBool || piStr != null)
                        {
                            var rb = ent.Components.Get<Rigidbody>();
                            // lua:2482 — MaintainPropInteraction callback (return false to block prop manipulation)
                            if (rb != null && MaintainPropInteraction(ent))
                            {
                                rb.Enabled = true;
                                rb.Sleeping = false;
                                // lua:2485 — constraint.RemoveConstraints(ent, "Weld") → destroy FixedJoint components
                                foreach (var joint in ent.Components.GetAll<FixedJoint>())
                                    joint.Destroy();
                                // lua:2475 — true/"OnlyDamage" + health → applyDmg = true (only if target alive/has health)
                                if (piBool || piStr == "OnlyDamage")
                                {
                                    var targetNPC = ent.Components.Get<BaseNPC>();
                                    // Lua: !isLiving or (isLiving && (Health()>0 or m_takedamage>1)). S&Box: skip m_takedamage.
                                    if (!isLiving || targetNPC == null || targetNPC.CurrentHealth > 0)
                                    {
                                        hitRegistered = true;
                                        applyDmg = true;
                                    }
                                }
                                // lua:2477-2478 — OnlyPush → applyDmg = false
                                else if (piStr == "OnlyPush")
                                {
                                    applyDmg = false;
                                }
                                // lua:2486-2491: physics push (true or "OnlyPush")
                                if (piBool || piStr == "OnlyPush")
                                {
                                    hitRegistered = true;
                                    var pushDir = (GetEnemy()?.WorldPosition ?? myPos);
                                    rb.ApplyForce((pushDir + WorldRotation.Forward * (rb.Mass * 700) + Vector3.Up * (rb.Mass * 200)).Normal * rb.Mass * 100);
                                }
                            }
                        }
                    }
                    if (applyDmg)
                    {
                        // lua:2499 — Knockback guards: MOVETYPE_PUSH→kinematic, Stationary, Boss non-Tank
                        var entBase = ent.Components.Get<BaseNPC>();
                        if (HasMeleeAttackKnockBack
                            && (ent.Components.Get<Rigidbody>() is { MotionEnabled: true })  // MOVETYPE_PUSH
                            && entBase?.MovementType != VJMoveType.Stationary
                            && (!HasEntityFlag(ent, "VJ_ID_Boss") /* || IsVJBaseSNPC_Tank — Phase 3 */))
                        {
                            var vel = MeleeAttackKnockbackVelocity(ent);
                            // lua:2502-2510 — SetGroundEntity(NULL) / IsNextBot / loco — S&Box Rigidbody.Velocity covers all Source knockback mechanics
                            var rb = ent.Components.Get<Rigidbody>();
                            if (rb != null) rb.Velocity = vel;
                        }
                        // lua:2513-2522: Apply damage with type tags
                        if (!DisableDefaultMeleeAttackDamageCode)
                        {
                            var dmgInfo = new DamageInfo();
                            dmgInfo.Damage = ScaleByDifficulty(dmgAmount);
                            // lua:2516 — SetDamageType → S&Box Tags
                            dmgInfo.Tags.Add(MapDamageTypeToTag(MeleeAttackDamageType));
                            // lua:2517 — SetDamageForce(forward * ((dmg+100)*70)) → S&Box Rigidbody.ApplyForce
                            if (BaseNPC.HasEntityFlag(ent, "VJ_ID_Living"))
                                ent.Components.Get<Rigidbody>()?.ApplyForce(WorldRotation.Forward * ((dmgInfo.Damage + 100) * 70));
                            // LIMITATION: S&Box DamageInfo has no Inflictor; Weapon=null means attacker-is-inflictor (correct for melee)
                            dmgInfo.Attacker = GameObject;
                            VJUtility.DamageSpecialEnts(GameObject, ent, dmgInfo);
                            // lua:2521 — ent:TakeDamage(dmgInfo) → S&Box IDamageable
                            foreach (var d in ent.Components.GetAll<IDamageable>())
                                d.OnDamage(dmgInfo);
                        }
                        // lua:2524-2541: Bleeding damage
                        // lua:2524 — (!ent.VJ_ID_Boss or self.VJ_ID_Boss) — boss guard: only bleed non-bosses (or if attacker is boss)
                        bool targetIsBoss = BaseNPC.HasEntityFlag(ent, "VJ_ID_Boss");
                        if (MeleeAttackBleedEnemy && isLiving && (!targetIsBoss || VJ_ID_Boss) && Game.Random.Next(1, MeleeAttackBleedEnemyChance + 1) == 1)
                        {
                            // lua:2525-2541 — bleed timer → polling (polled in ProcessAttackTimers)
                            // lua:2526 — bleedDmg = ScaleByDifficulty(MeleeAttackBleedEnemyDamage)
                            BleedTarget = ent;
                            BleedRepsRemaining = MeleeAttackBleedEnemyReps;
                            BleedDmgAmount = ScaleByDifficulty(MeleeAttackBleedEnemyDamage);
                            NextBleedT = Time.Now + MeleeAttackBleedEnemyTime;
                        }
                    }
                    // lua:2544-2553: Player-specific effects
                    if (isPlayer)
                    {
                        // PX: lua:2545 — ent:ViewPunch(Angle(...)) — no native S&Box camera shake API
                        // PX: lua:2547-2548 — ent:SetDSP(MeleeAttackDSP) — Source engine DSP, no S&Box equivalent
                        if (MeleeAttackPlayerSpeed)
                            DoMeleeAttackPlayerSpeed(ent, MeleeAttackPlayerSpeedWalk, MeleeAttackPlayerSpeedRun, MeleeAttackPlayerSpeedTime);
                    }
                    if (!isProp)
                    {
                        hitRegistered = true;
                        if (MeleeAttackStopOnHit) break;
                    }
                }
            }
        }
        // lua:2562-2567: AttackState management
        if (AttackState < VJAttackState.Executed)
        {
            AttackState = VJAttackState.Executed;
            // lua:2564-2566 — attackTimers[VJ.ATTACK_TYPE_MELEE](self)
            ScheduleAttackTimers();
        }
        // lua:2568-2576: Sound feedback
        if (!skip)
        {
            if (hitRegistered)
            {
                PlaySoundSystem("MeleeAttack");
                AttackState = VJAttackState.ExecutedHit;
            }
            else
            {
                OnMeleeAttackExecute("Miss");
                PlaySoundSystem("MeleeAttackMiss");
            }
        }
    }

    // ═══ ExecuteRangeAttack — creature_base/init.lua:2623-2669 ═══
    public virtual void ExecuteRangeAttack()
    {
        // lua:2625: guard
        if (Dead || PauseAttacks || Flinching || AttackType == VJAttackType.Melee) return;
        var ene = GetEnemy();
        var eneValid = ene.IsValid();
        if (eneValid)
        {
            AttackType = VJAttackType.Range;
            // lua:2632: OnRangeAttackExecute("Init")
            if (!OnRangeAttackExecute("Init", ene))
            {
                // lua:2633: PICK projectile class
                var projectileClass = VJUtility.PICK<string>(RangeAttackProjectiles) ?? VJUtility.PICK<string>(RangeAttackEntityToSpawn);
                if (projectileClass != null)
                {
                    // lua:2635-2657 — spawn projectile via virtual dispatch
                    SpawnRangeProjectile(projectileClass, ene);
                }
            }
        }
        // lua:2660-2668: AttackState + sound
        if (AttackState < VJAttackState.Executed)
        {
            if (eneValid)
                PlaySoundSystem("RangeAttack");
            AttackState = VJAttackState.Executed;
            // lua:2665-2667 — attackTimers[VJ.ATTACK_TYPE_RANGE](self)
            ScheduleAttackTimers();
        }
    }

    // ═══ ExecuteLeapAttack — creature_base/init.lua:2671-2717 ═══
    public virtual void ExecuteLeapAttack()
    {
        // lua:2673: guard
        if (Dead || PauseAttacks || Flinching || (LeapAttackStopOnHit && AttackState == VJAttackState.ExecutedHit)) return;
        var skip = OnLeapAttackExecute("Init");
        var hitRegistered = false;
        if (!skip)
        {
            var hits = Scene.FindInPhysics(new Sphere(WorldPosition, LeapAttackDamageDistance));
            foreach (var ent in hits)
            {
                // lua:2678: ent == self or ent:GetClass() == myClass → skip self, skip same NPC type
                if (ent == GameObject) continue;
                var entBase2 = ent.Components.Get<BaseNPC>();
                if (entBase2 != null && entBase2.VJ_NPC_Class.Any(c => VJ_NPC_Class.Contains(c))) continue;
                // SKIP: lua:2679 — IsVJBaseBullseye flag — Phase 3 bullseye system
                // lua:2680 — skip VJ_IsControllingNPC, skip player when VJ_IsControllingNPC / dead / ignoreplayers
                bool isPlayer = ent.Components.Get<PlayerBase>() != null;
                if (isPlayer)
                {
                    // PX: ent.VJ_IsControllingNPC — Source player field, no S&Box equivalent
                    if (!Alive(ent) || VJInit.vj_npc_ignoreplayers) continue;
                }
                else if (entBase2?.VJ_IsBeingControlled == true) continue;
                // lua:2681 — (VJ_ID_Living && Disp != D_LI) || VJ_ID_Attackable || VJ_ID_Destructible
                bool isLiving = HasEntityFlag(ent, "VJ_ID_Living");
                if (isPlayer && !isLiving) isLiving = true; // players are living targets by default
                bool isAttackable = HasEntityFlag(ent, "VJ_ID_Attackable");
                bool isDestructible = HasEntityFlag(ent, "VJ_ID_Destructible");
                if ((isLiving && Disposition(ent) != (int)VJBase.Disposition.Like) || isAttackable || isDestructible)
                {
                    if (OnLeapAttackExecute("PreDamage", ent)) continue;
                    var dmgAmount = ScaleByDifficulty(LeapAttackDamage);
                    if (!DisableDefaultLeapAttackDamageCode)
                    {
                        var dmgInfo = new DamageInfo();
                        dmgInfo.Damage = dmgAmount;
                        // lua:2688 — SetInflictor(self) — S&Box DamageInfo has no Inflictor field; attacker=self suffices for NPC leap
                        // lua:2690 — SetDamageType(LeapAttackDamageType) → S&Box Tags
                        dmgInfo.Tags.Add(MapDamageTypeToTag(LeapAttackDamageType));
                        // lua:2691 — SetDamageForce(forward * ((dmg+100)*70)) → S&Box Rigidbody.ApplyForce
                        if (BaseNPC.HasEntityFlag(ent, "VJ_ID_Living"))
                            ent.Components.Get<Rigidbody>()?.ApplyForce(WorldRotation.Forward * ((dmgInfo.Damage + 100) * 70));
                        dmgInfo.Attacker = GameObject;
                        // lua:2692 — ent:TakeDamage(dmgInfo) → S&Box IDamageable
                        foreach (var d in ent.Components.GetAll<IDamageable>())
                            d.OnDamage(dmgInfo);
                    }
                    if (isPlayer)
                    {
                        // PX: lua:2694-2695 — ent:ViewPunch(Angle(...)) — no native S&Box camera shake API, not in scope
                    }
                    hitRegistered = true;
                    if (LeapAttackStopOnHit) break;
                }
            }
        }
        // lua:2702-2707: AttackState management
        if (AttackState < VJAttackState.Executed)
        {
            AttackState = VJAttackState.Executed;
            // lua:2704-2705 — attackTimers[VJ.ATTACK_TYPE_LEAP](self)
            ScheduleAttackTimers();
        }
        // lua:2708-2716: Sound feedback
        if (!skip)
        {
            if (hitRegistered)
            {
                PlaySoundSystem("LeapAttackDamage");
                AttackState = VJAttackState.ExecutedHit;
            }
            else
            {
                OnLeapAttackExecute("Miss");
                PlaySoundSystem("LeapAttackDamageMiss");
            }
        }
    }

    // ═══ BeginDeath — creature_base/init.lua:3188-3311 ═══
    public override void BeginDeath(DamageInfo dmginfo, int hitgroup)
    {
        // lua:3189 — self.Dead = true
        Dead = true;
        // lua:3190 — self.DoNotDuplicate = true
        DoNotDuplicate = true;
        // lua:3191 — self:SetSaveValue("m_lifeState", 1) — LIFE_DYING
        SetSaveValue("m_lifeState", 1);
        // lua:3192 — self:OnDeath(dmginfo, hitgroup, "Init")
        OnDeath(dmginfo, hitgroup, "Init");

        // lua:3193 — if self.MedicData.Status then self:ResetMedicBehavior() end
        if (Medic.Status != "false" && Medic.Status != null)
            ResetMedicBehavior();
        // lua:3194 — if self.IsFollowing then self:ResetFollowBehavior() end
        if (IsFollowing) ResetFollowBehavior();

        // lua:3195 — dmgInflictor = dmginfo:GetInflictor()
        var dmgInflictor = dmginfo.Weapon; // S&Box: Weapon ≈ Source CTakeDamageInfo inflictor
        // lua:3196 — dmgAttacker = dmginfo:GetAttacker()
        var dmgAttacker = dmginfo.Attacker;
        // lua:3197 — myPos = self:GetPos()
        Vector3 myPos = WorldPosition;

        // ---- Ally death response (lua:3199-3249) ----
        // lua:3200 — responseDist = math_max(800, self:OBBMaxs():Distance(self:OBBMins()) * 12)
        float responseDist = Math.Max(800, Vector3.DistanceBetween(OBBMaxs(), OBBMins()) * 12);
        // lua:3201 — allies = self:Allies_Check(responseDist)
        var allies = Allies_Check(responseDist);
        if (allies != null)
        {
            // lua:3203 — doBecomeEnemyToPlayer = (self.BecomeEnemyToPlayer && dmgAttacker:IsPlayer() && !VJ_CVAR_IGNOREPLAYERS)
            var doBecomeEnemyToPlayer = BecomeEnemyToPlayer > 0
                && dmgAttacker.IsValid()
                && dmgAttacker.Components.Get<PlayerBase>() != null
                && !VJInit.vj_npc_ignoreplayers;
            var responseType = DeathAllyResponse;
            foreach (var ally in allies)
            {
                var allyBase = ally.Components.Get<BaseNPC>();
                if (allyBase == null) continue;
                // lua:3210-3212 — OnAllyKilled callback + PlaySoundSystem("AllyDeath")
                allyBase.OnAllyKilled(GameObject);
                allyBase.PlaySoundSystem("AllyDeath");
                // lua:3217-3221 — bring ally to death location
                if (responseType != "OnlyAlert")
                    Allies_Bring("Diamond", responseDist, new List<GameObject> { ally }, 4);
                // lua:3226-3227 — alert ally
                allyBase.DoReadyAlert();
                allyBase.SetTurnTarget("Enemy");
                // lua:3233-3241 — BecomeEnemyToPlayer chain
                if (doBecomeEnemyToPlayer && allyBase.BecomeEnemyToPlayer > 0
                    && allyBase.CheckRelationship(dmgAttacker) == (int)VJBase.Disposition.Like)
                {
                    allyBase.SetRelationshipMemory(dmgAttacker, "hostility", 1f);
                    var hostility = allyBase.GetRelationshipMemory(dmgAttacker, "hostility");
                    if (hostility > allyBase.BecomeEnemyToPlayer)
                    {
                        // lua:3235 — if ally:Disposition(dmgAttacker) != D_HT then
                        if (allyBase.Disposition(dmgAttacker) != (int)VJBase.Disposition.Hate)
                        {
                            allyBase.OnBecomeEnemyToPlayer(dmginfo, hitgroup);
                            allyBase.SetRelationshipMemory(dmgAttacker, "override_disposition", (int)VJBase.Disposition.Hate);
                            allyBase.AddEntityRelationship(dmgAttacker, (int)VJBase.Disposition.Hate, 2);
                            allyBase.PlaySoundSystem("BecomeEnemyToPlayer");
                            // lua:3236-3239 — if ally.IsFollowing then ally:ResetFollowBehavior() end
                            if (allyBase.IsFollowing) allyBase.ResetFollowBehavior();
                            // PX: lua:3240 — CanChatMessage — Source chat system, no S&Box equivalent
                        }
                        // lua:3245 — ally.Alerted = true
                        allyBase.Alerted = VJAlertState.Enemy;
                    }
                }
            }
        }

        // ---- Blood decal (lua:3253-3261) ----
        // lua:3253 — if self.Bleeds && self.HasBloodDecal then
        if (Bleeds && HasBloodDecal)
        {
            // lua:3254 — bloodDecal = PICK(self.BloodDecal)
            var bloodDecal = VJUtility.PICK(BloodDecal);
            // lua:3255 — if bloodDecal then
            if (bloodDecal != null)
            {
                // lua:3256-3259 — TraceLine downward + util.Decal
                var decalPos = myPos + Vector3.Up * 4f;
                var tr = Game.ActiveScene.Trace.Ray(decalPos, decalPos + Vector3.Down * 500f)
                    .IgnoreGameObjectHierarchy(GameObject)
                    .Run();
                if (tr.Hit)
                    PlaceBloodDecal(bloodDecal, tr.HitPosition + tr.Normal, tr.HitPosition - tr.Normal);
            }
        }

        // ---- Cleanup (lua:3263-3268) ----
        // lua:3263 — self:RemoveTimers()
        RemoveTimers();
        // lua:3264 — self:StopAllSounds()
        StopAllSounds();
        // lua:3265 — self.AttackType = VJ.ATTACK_TYPE_NONE
        AttackType = VJAttackType.None;
        // lua:3266 — self.HasMeleeAttack = false
        HasMeleeAttack = false;
        // lua:3267 — self.HasRangeAttack = false
        HasRangeAttack = false;
        // lua:3268 — self.HasLeapAttack = false
        HasLeapAttack = false;

        // ---- Attacker check (lua:3269-3272) ----
        // lua:3269 — if IsValid(dmgAttacker) then
        // PX: lua:3269-3272 — npc_barnacle GetClass() (HL2 entity, check via Tags) / AddFrags (Source score system, no S&Box equivalent)

        // lua:3273 — gamemode.Call("OnNPCKilled", self, dmgAttacker, dmgInflictor)
        OnNPCKilled?.Invoke(GameObject, dmginfo.Attacker, dmginfo.Weapon);

        // ---- Post-death setup (lua:3274-3277) ----
        // lua:3274 — self:SetCollisionGroup(COLLISION_GROUP_DEBRIS)
        SetCollisionGroup(1); // COLLISION_GROUP_DEBRIS
        // lua:3275 — self:GibOnDeath(dmginfo, hitgroup)
        GibOnDeath(dmginfo, hitgroup);
        // lua:3276 — self:PlaySoundSystem("Death")
        PlaySoundSystem("Death");
        // lua:3277 — //AA_StopMoving() commented out

        // ---- I/O events (lua:3280-3285) ----
        // lua:3281-3284 — if dmgAttacker:IsValid() then TriggerOutput("OnDeath", dmgAttacker) else TriggerOutput("OnDeath", self)
        var deathAttacker = dmginfo.Attacker;
        if (deathAttacker.IsValid())
            TriggerOutput("OnDeath", deathAttacker);
        else
            TriggerOutput("OnDeath", GameObject);
        // lua:3285 — self:Fire("KilledNPC") — PX: Source I/O, use OnTriggerOutput("OnDeath") instead

        // ---- Death animation + delay → FinishDeath (lua:3288-3310) ----
        // lua:3288 — deathTime = self.DeathDelayTime
        float deathTime = DeathDelayTime;
        // lua:3289 — combine ball → HasDeathAnimation = false
        //   Covered by Dissolve tag check below (combine ball deals DMG_DISSOLVE)
        // lua:3290 — HasDeathAnimation && !DMG_REMOVENORAGDOLL && !DMG_DISSOLVE && NavType!=CLIMB
        if (HasDeathAnimation
            && !dmginfo.Tags.Has(VJDamageTags.Dissolve)
            && !dmginfo.Tags.Has(VJDamageTags.RemoveNoRagdoll)
            && GetNavType() != (int)NavType.Climb
            && Game.Random.Next(1, Math.Max(1, DeathAnimationChance) + 1) == 1)
        {
            // lua:3291 — self:RemoveAllGestures()
            RemoveAllGestures();
            // lua:3292 — self:OnDeath(dmginfo, hitgroup, "DeathAnim")
            OnDeath(dmginfo, hitgroup, "DeathAnim");
            // lua:3293-3295 — PICK(AnimTbl_Death) / AnimDurationEx / PlayAnim
            var deathAnimPick = VJUtility.PICK(AnimTbl_Death);
            if (deathAnimPick != null)
            {
                var deathDur = VJUtility.AnimDurationEx(GameObject, deathAnimPick, null, 0f);
                PlayAnim(deathAnimPick, true, deathDur, true);
            }
            // lua:3297 — self.DeathAnimationCodeRan = true
            DeathAnimationCodeRan = true;
        }
        // lua:3298 — else
        else
        {
            // lua:3300 — self:SetSaveValue("m_lifeState", 2) — LIFE_DEAD
            SetSaveValue("m_lifeState", 2);
        }

        // lua:3302-3310 — if deathTime > 0 then timer.Simple → FinishDeath else FinishDeath
        // S&Box: polling pattern → NextDeathFinishT (polled in Think)
        PendingDeathDmgInfo = dmginfo;
        PendingDeathHitgroup = hitgroup;
        if (deathTime > 0)
            NextDeathFinishT = Time.Now + deathTime;
        else
            FinishDeath(dmginfo, hitgroup);
    }

    // ═══ FinishDeath — creature_base/init.lua:3313-3325 ═══
    public virtual void FinishDeath(DamageInfo dmginfo, int hitgroup)
    {
        // lua:3314 — VJ_DEBUG damage print
        if (VJDebug.IsEnabled(this, VJDebugFlags.Damage))
            VJDebug.Print(GameObject, "FinishDeath", null, "Attacker =", dmginfo.Attacker, "| Inflictor =", dmginfo.Weapon);

        // lua:3315 — self:SetSaveValue("m_lifeState", 2) — LIFE_DEAD
        SetSaveValue("m_lifeState", 2);
        // lua:3316 — //SetNPCState(NPC_STATE_DEAD) — commented out
        // lua:3317 — self:OnDeath(dmginfo, hitgroup, "Finish")
        OnDeath(dmginfo, hitgroup, "Finish");

        // lua:3318 — if self.DropDeathLoot then
        if (DropDeathLoot)
        {
            // lua:3319 — self:CreateDeathLoot(dmginfo, hitgroup)
            CreateDeathLoot(dmginfo, hitgroup);
        }

        // lua:3321 — if not DMG_REMOVENORAGDOLL then CreateDeathCorpse
        // Note: Lua only checks DMG_REMOVENORAGDOLL (not DMG_DISSOLVE) for corpse creation
        if (!dmginfo.Tags.Has(VJDamageTags.RemoveNoRagdoll))
            CreateDeathCorpse(dmginfo, hitgroup);

        // lua:3322 — self:Remove() — S&Box: GameObject.Destroy() handles cleanup, children auto-destroyed
    }

    // ═══ CreateDeathCorpse — creature_base/init.lua:3327-3487 ═══
    public virtual GameObject CreateDeathCorpse(DamageInfo dmginfo, int hitgroup)
    {
        // ---- SavedDmgInfo guard (lua:3328-3342) ----
        // lua:3330 — if !self.SavedDmgInfo then
        if (SavedDmgInfo == null)
        {
            // lua:3331-3341 — SavedDmgInfo snapshot (GMod resets dmginfo after tick)
            SavedDmgInfo = new SavedDmgInfoData
            {
                dmginfo = dmginfo,
                attacker = dmginfo.Attacker,        // lua:3333 — GetAttacker()
                inflictor = dmginfo.Weapon,          // lua:3334 — GetInflictor() → S&Box Weapon
                amount = dmginfo.Damage,             // lua:3335 — GetDamage()
                pos = dmginfo.Position,              // lua:3336 — GetDamagePosition()
                // lua:3337 — GetDamageType() → S&Box no DMG_* bitmask; use Tags for type checks
                // lua:3338 — GetDamageForce() → S&Box no direct force on DamageInfo
                // lua:3339 — GetAmmoType() → S&Box no ammo type on DamageInfo
                hitgroup = hitgroup,
            };
        }

        // ---- Corpse gate (lua:3344) ----
        // lua:3344 — if self.HasDeathCorpse && self.HasDeathRagdoll != false then
        if (!HasDeathCorpse || HasDeathRagdoll == false) return null;

        // ---- Model selection (lua:3345-3348) ----
        // lua:3345 — corpseMdl = self:GetModel()
        var corpseMdl = VJEntitySpawner.GetModelPath(GameObject);
        // lua:3346-3348 — corpseMdlCustom = PICK(self.DeathCorpseModel); if corpseMdlCustom then corpseMdl = corpseMdlCustom end
        var corpseMdlCustom = VJUtility.PICK(DeathCorpseModel);
        if (!string.IsNullOrEmpty(corpseMdlCustom)) corpseMdl = corpseMdlCustom;
        // lua:3349 — corpseClass = "prop_physics"
        var corpseClass = "prop_physics";

        // ---- Entity class selection (lua:3350-3357) ----
        // lua:3350 — if self.DeathCorpseEntityClass then corpseClass = self.DeathCorpseEntityClass
        if (!string.IsNullOrEmpty(DeathCorpseEntityClass))
            corpseClass = DeathCorpseEntityClass;
        else
        {
            // lua:3352-3354 — util.IsValidRagdoll → "prop_ragdoll"
            Model loaded;
            if (VJEntitySpawner.TryLoadModel(corpseMdl, out loaded))
            {
                if (VJEntitySpawner.IsValidRagdollModel(loaded))
                    corpseClass = "prop_ragdoll";
            }
            // lua:3354 — !util.IsValidProp || !util.IsValidModel → return false
            else if (!string.IsNullOrEmpty(corpseMdl))
            {
                return null;
            }
        }

        // ---- Entity creation (lua:3358-3364) ----
        // lua:3358 — corpse = ents.Create(corpseClass)
        // lua:3359-3364 — corpse:SetModel(corpseMdl)/SetPos(self:GetPos())/SetAngles(self:GetAngles())/Spawn/Activate
        GameObject corpse;
        if (corpseClass == "prop_ragdoll")
            corpse = VJEntitySpawner.CreateRagdollEntity(corpseMdl, WorldPosition, WorldRotation);
        else
            corpse = VJEntitySpawner.CreateModelEntity(corpseMdl, WorldPosition, WorldRotation, withPhysics: true);

        // ---- Copy appearance (lua:3365-3383) ----
        // lua:3365 — corpse:SetSkin(self:GetSkin())
        // lua:3366-3367 — for i=0,GetNumBodyGroups() do corpse:SetBodygroup(i, GetBodygroup(i)) end
        // lua:3368 — corpse:SetColor(self:GetColor()) → Tint
        // lua:3369 — corpse:SetMaterial(self:GetMaterial()) → MaterialOverride
        // lua:3370-3383 — submaterial copy (Source-only, S&Box MaterialAccessor approach differs)
        VJEntitySpawner.CopyAppearance(GameObject, corpse);

        // ---- Corpse metadata (lua:3386-3391) ----
        // lua:3386 — corpse.FadeCorpseType assign
        // lua:3387 — corpse.IsVJBaseCorpse = true
        corpse.Tags.Add("vj_corpse");
        // lua:3388 — corpse.DamageInfo = dmginfo (Source table; S&Box uses dmginfo arg directly)
        // lua:3389 — corpse.ChildEnts = self.DeathCorpse_ChildEnts or {}
        // lua:3390-3391 — BloodData assignment

        // ---- Blood pool (lua:3392-3394) ----
        // lua:3392-3394 — if self.Bleeds && self.HasBloodPool && vj_npc_blood_pool:GetInt()==1 then self:SpawnBloodPool(...)
        if (Bleeds && HasBloodPool)
        {
            // lua:3393 — vj_npc_blood_pool convar (S&Box: always enabled via Bleeds+HasBloodPool)
            SpawnBloodPool(dmginfo, hitgroup, corpse);
        }

        // ---- Collision (lua:3397-3404) ----
        // lua:3397 — corpse:SetCollisionGroup(self.DeathCorpseCollisionType)
        var corpseNPC = corpse?.Components.Get<BaseNPC>();
        corpseNPC?.SetCollisionGroup(DeathCorpseCollisionType);
        // lua:3398-3399 — ai_serverragdolls convar (N/A) + undo.ReplaceEntity (Source sandbox)
        // lua:3400-3403 — VJ.Corpse_Add(corpse)
        VJUtility.Corpse_Add(corpse);

        // ---- On fire (lua:3407-3413) ----
        // lua:3407 — if self:IsOnFire() then
        if (IsOnFire())
        {
            // lua:3408 — corpse:Ignite(math.Rand(8, 10), 0)
            VJEntitySpawner.IgniteEntity(corpse);
            // lua:3409-3411 — if !self.Immune_Fire then corpse:SetColor(colorGrey)
            if (!Immune_Fire)
            {
                var corpseRenderer = corpse.Components.Get<ModelRenderer>();
                if (corpseRenderer != null) corpseRenderer.Tint = new Color(0.35f, 0.35f, 0.35f, 1f);
            }
        }

        // ---- Dissolve (lua:3416-3418) ----
        // lua:3416-3418 — DMG_DISSOLVE type or prop_combine_ball inflictor → corpse:Dissolve(0, 1) → VJEntitySpawner.DissolveEntity
        if (dmginfo.Tags.Has(VJDamageTags.Dissolve) || (dmginfo.Weapon.IsValid() && dmginfo.Weapon.Tags.Has("prop_combine_ball")))
        {
            VJEntitySpawner.DissolveEntity(corpse, 2f, 1f);
            FL_DISSOLVING = true;
        }

        // ---- Bone physics (lua:3422-3448) ----
        // lua:3422-3428 — useLocalVel + dmgForce = (SavedDmgInfo.force/40) + move vel + rb vel
        if (DeathCorpseApplyForce && SavedDmgInfo != null)
        {
            var selfRb = Components.Get<Rigidbody>();
            var selfVel = selfRb?.Velocity ?? Vector3.Zero;
            var dmgForce = SavedDmgInfo.force / 40f + selfVel;
            var corpseRb = corpse.Components.Get<Rigidbody>();
            if (corpseRb != null)
            {
                corpseRb.ApplyForce(dmgForce * 50f);
            }
            // lua:3429-3448 — per-bone physics loop (Source GetPhysicsObjectCount/GetSurfaceArea/TranslatePhysBoneToBone)
            // S&Box ModelPhysics differs; simplified to whole-body Rigidbody force. Per-bone deferred.
        }

        // ---- Health & stink (lua:3451-3456) ----
        // lua:3451-3455 — corpse:SetMaxHealth/SetHealth(totalSurface/60) — S&Box HealthComponent deferred
        // lua:3456 — VJ.Corpse_AddStinky(corpse, true)
        VJUtility.Corpse_AddStinky(corpse, true);

        // ---- Fade (lua:3458-3460) ----
        // lua:3458-3459 — DeathCorpseFade timer + vj_npc_corpse_fade convar
        if (DeathCorpseFade > 0)
        {
            // Source: corpse:Fire("FadeAndRemove", "", DeathCorpseFade) → S&Box: delayed Destroy
            _ = Task.Delay((int)(DeathCorpseFade * 1000f)).ContinueWith(_ => { if (corpse.IsValid()) corpse.Destroy(); });
        }
        // lua:3460 — self:OnCreateDeathCorpse(dmginfo, hitgroup, corpse)
        OnCreateDeathCorpse(dmginfo, hitgroup, corpse);

        // ---- Dissolve children (lua:3461-3465) ----
        // lua:3461 — if corpse:IsFlagSet(FL_DISSOLVING) then
        if (FL_DISSOLVING && DeathCorpse_ChildEnts != null)
        {
            // lua:3462-3464 — child entities dissolve (S&Box: VJEntitySpawner.DissolveEntity)
            foreach (var child in DeathCorpse_ChildEnts)
                VJEntitySpawner.DissolveEntity(child, 2f, 1f);
        }

        // ---- CallOnRemove (lua:3466-3476) ----
        // lua:3466-3476 — corpse:CallOnRemove callback cleans up child ents on remove
        // (S&Box: child entities destroyed automatically when parent is destroyed)
        // lua:3477 — hook.Call("CreateEntityRagdoll", nil, self, corpse)

        // lua:3478 — return corpse
        return corpse;

        // ---- Else (no corpse) branch (lua:3480-3486) — NOT reached in this path ----
        // The Lua else branch (lines 3480-3486) handles: remove child ents
        // In C#, if HasDeathCorpse is false or HasDeathRagdoll is false, we return null above (line 3344 gate).
        // The else branch cleanup is implicitly handled by the GameObject.Destroy lifecycle.
    }
}
