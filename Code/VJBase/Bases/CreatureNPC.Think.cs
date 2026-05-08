using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// CreatureNPC Think + SelectSchedule — ported from npc_vj_creature_base/init.lua.
/// </summary>
public partial class CreatureNPC
{
    // ═══ Additional Fields ═══
    public float NextProcessTime { get; set; } = 0.1f;
    // ═══ Think — main AI loop ═══
    public virtual void Think()
    {
        var curTime = Time.Now;
        bool doHeavyProcesses = curTime > NextProcessT;
        if (doHeavyProcesses)
            NextProcessT = curTime + NextProcessTime;

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
        OnThinkActive();

        // Process attack timers — check reset/re-enable polling fields
        ProcessAttackTimers(curTime);

        var moveType = MovementType;
        bool isAA = moveType == VJMoveType.Aerial || moveType == VJMoveType.Aquatic;

        // Perception — Engine/AISenses produces conditions into BaseNPC
        if (doHeavyProcesses)
            TickSenses();

        // SKIP: AA velocity tracking / position checking / acceleration + AA_MoveAnimation — Phase 3 (base_aa.lua:1906-1942)
        if (isAA) { }

        // Follow behavior (Phase 3)

        RunAI();
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

        // Auto-movement: apply walk frames when stationary with movement sequence
        // Phase 3: GetSequenceMoveDist, AutoMovement integration

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

        // SKIP: schedules.lua:208 — self:MaintainActivity() call at end of RunAI. Phase 3.

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

    // ═══ MaintainAlertBehavior ═══
    public virtual void MaintainAlertBehavior(bool alwaysChase)
    {
        var ene = GetEnemy();
        if (!ene.IsValid()) return;

        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        {
            AA_ChaseEnemy(true, "Alert");
            return;
        }

        // Ground: SCHEDULE_ALERT_CHASE
        SCHEDULE_ALERT_CHASE(false);
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
                // SKIP: GetClass() comparison — Source-specific, Phase 3 replace with component type check
                // SKIP: lua:2463 — ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled — Phase 3 bullseye system
                // SKIP: lua:2464 — ent:IsPlayer() with VJ_IsControllingNPC / Alive / VJ_CVAR_IGNOREPLAYERS — Phase 3 player system
                // lua:2465 — disposition check
                bool isLiving = ent.Components.Get<BaseNPC>()?.IsVJBaseSNPC ?? false;
                // SKIP: lua:2465 — ent.VJ_ID_Attackable / ent.VJ_ID_Destructible — Phase 3 entity flags
                var delta = new Vector3(ent.WorldPosition.x - myPos.x, ent.WorldPosition.y - myPos.y, 0);
                bool inAngle = traceDir.Dot(delta.Normal) > MathF.Cos(MathF.PI / 180f * MeleeAttackDamageAngleRadius);
                if (isLiving && Disposition(ent) != (int)VJBase.Disposition.Like && inAngle)
                {
                    // lua:2466: prop attack living distance check
                    // SKIP: lua:2466 — VJ.GetNearestDistance(self, ent, true) > MeleeAttackDistance — Phase 3 utility
                    var applyDmg = true;
                    // SKIP: lua:2468 — ent.VJ_ID_Attackable prop detection — Phase 3 entity flags
                    bool isProp = false;
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
                            if (rb != null)
                            {
                                rb.Enabled = true;
                                rb.Wake();
                                // SKIP: lua:2485 — constraint.RemoveConstraints(ent, "Weld") — Phase 3 joint system
                                // lua:2475 — true/"OnlyDamage" + health → applyDmg = true
                                // SKIP: lua:2475 — ent:Health() / ent:GetInternalVariable("m_takedamage") — Phase 3
                                if (piBool || piStr == "OnlyDamage")
                                {
                                    hitRegistered = true;
                                    applyDmg = true;
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
                        // lua:2499-2511: Knockback (skip IsNextBot/loco — Phase 3)
                        if (HasMeleeAttackKnockBack)
                        {
                            var vel = MeleeAttackKnockbackVelocity(ent);
                            // SKIP: lua:2502-2510 — SetGroundEntity(NULL) / IsNextBot / loco — Phase 3
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
                            // SKIP: lua:2517 — SetDamageForce — Phase 3 (S&Box: apply force separately on Rigidbody)
                            dmgInfo.Attacker = GameObject;
                            // SKIP: lua:2520 — VJ.DamageSpecialEnts — Phase 3 damage utility
                            ent.TakeDamage(dmgInfo);
                        }
                        // lua:2524-2541: Bleeding damage
                        if (MeleeAttackBleedEnemy && isLiving && Game.Random.Next(1, MeleeAttackBleedEnemyChance + 1) == 1)
                        {
                            // SKIP: lua:2525-2541 — timer.Create bleed system — Phase 3 async timers
                        }
                    }
                    // lua:2544-2553: Player-specific effects
                    // SKIP: lua:2544-2553 — ent:IsPlayer() / ViewPunch / SetDSP / DoMeleeAttackPlayerSpeed — Phase 3 player system
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
                var projectileClass = VJUtility.PICK(RangeAttackProjectiles) ?? VJUtility.PICK(RangeAttackEntityToSpawn);
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
                // SKIP: GetClass() comparison — Source-specific, Phase 3 replace with component type check
                // SKIP: lua:2679 — ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled — Phase 3 bullseye
                // SKIP: lua:2680 — ent:IsPlayer() / VJ_IsControllingNPC / Alive / VJ_CVAR_IGNOREPLAYERS — Phase 3 player
                // lua:2681 — disposition check
                bool isLiving = ent.Components.Get<BaseNPC>()?.IsVJBaseSNPC ?? false;
                // SKIP: lua:2681 — ent.VJ_ID_Attackable / ent.VJ_ID_Destructible — Phase 3 entity flags
                if (isLiving && Disposition(ent) != (int)VJBase.Disposition.Like)
                {
                    if (OnLeapAttackExecute("PreDamage", ent)) continue;
                    var dmgAmount = ScaleByDifficulty(LeapAttackDamage);
                    if (!DisableDefaultLeapAttackDamageCode)
                    {
                        var dmgInfo = new DamageInfo();
                        dmgInfo.Damage = dmgAmount;
                        // SKIP: lua:2690 — SetDamageType(LeapAttackDamageType) — Phase 3 damage type mapping
                        // SKIP: lua:2691 — SetDamageForce — Phase 3 damage force
                        dmgInfo.Attacker = GameObject;
                        ent.TakeDamage(dmgInfo);
                    }
                    // SKIP: lua:2694-2695 — ent:IsPlayer() / ViewPunch — Phase 3 player system
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

    // ═══ Death ═══
    public virtual void BeginDeath(object dmginfo, int hitgroup) { Dead = true; }
    public virtual void FinishDeath(object dmginfo, int hitgroup) { }
}
