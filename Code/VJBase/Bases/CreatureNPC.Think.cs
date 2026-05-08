using System;
using System.Linq;
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
                var entBase1 = ent.Components.Get<BaseNPC>();
                if (entBase1 != null && entBase1.VJ_NPC_Class.Any(c => VJ_NPC_Class.Contains(c))) continue;
                // SKIP: lua:2463 — ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled — Phase 3 bullseye system
                // SKIP: lua:2464 — ent:IsPlayer() with VJ_IsControllingNPC / Alive / VJ_CVAR_IGNOREPLAYERS — Phase 3 player system
                // lua:2465 — ((VJ_ID_Living && Disp != D_LI) || VJ_ID_Attackable || VJ_ID_Destructible) && angle
                // Phase 3: isLiving → ent.Components.Get<VJEntityFlags>()?.IsLiving (IsVJBaseSNPC proxy only covers NPCs, not players/props)
                bool isLiving = false;
                bool isAttackable = false; // Phase 3: VJEntityFlags.IsAttackable
                bool isDestructible = false; // Phase 3: VJEntityFlags.IsDestructible
                var delta = new Vector3(ent.WorldPosition.x - myPos.x, ent.WorldPosition.y - myPos.y, 0);
                bool inAngle = traceDir.Dot(delta.Normal) > MathF.Cos(MathF.PI / 180f * MeleeAttackDamageAngleRadius);
                if (((isLiving && Disposition(ent) != (int)VJBase.Disposition.Like) || isAttackable || isDestructible) && inAngle)
                {
                    // lua:2466: prop attack living distance check
                    // SKIP: lua:2466 — VJ.GetNearestDistance(self, ent, true) > MeleeAttackDistance — Phase 3 utility
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
                            if (rb != null)
                            {
                                rb.Enabled = true;
                                rb.Sleeping = false;
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
                            // SKIP: lua:2521 — ent:TakeDamage(dmgInfo) — S&Box damage via IDamageable.OnDamage, Phase 3
                            // ent.TakeDamage(dmgInfo);
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
                // SKIP: lua:2679 — ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled — Phase 3 bullseye
                // SKIP: lua:2680 — ent:IsPlayer() / VJ_IsControllingNPC / Alive / VJ_CVAR_IGNOREPLAYERS — Phase 3 player
                // lua:2681 — (VJ_ID_Living && Disp != D_LI) || VJ_ID_Attackable || VJ_ID_Destructible
                // Phase 3: isLiving → ent.Components.Get<VJEntityFlags>()?.IsLiving (IsVJBaseSNPC proxy only covers NPCs, not players/props)
                bool isLiving = false;
                bool isAttackable = false; // Phase 3: VJEntityFlags.IsAttackable
                bool isDestructible = false; // Phase 3: VJEntityFlags.IsDestructible
                if ((isLiving && Disposition(ent) != (int)VJBase.Disposition.Like) || isAttackable || isDestructible)
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
                        // SKIP: lua:2692 — ent:TakeDamage(dmgInfo) — S&Box damage via IDamageable.OnDamage, Phase 3
                        // ent.TakeDamage(dmgInfo);
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

    // ═══ BeginDeath — creature_base/init.lua:3188-3311 ═══
    public virtual void BeginDeath(DamageInfo dmginfo, int hitgroup)
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
        // SKIP: lua:3200 — OBBMaxs/OBBMins — Phase 3 collision bounds
        float responseDist = 800;
        // lua:3201 — allies = self:Allies_Check(responseDist)
        var allies = Allies_Check(responseDist);
        if (allies != null)
        {
            // lua:3203 — doBecomeEnemyToPlayer (player attacker hostility)
            // SKIP: lua:3203 — IsPlayer() / VJ_CVAR_IGNOREPLAYERS — Phase 3 player detection + convar
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
                // lua:3233-3241 — BecomeEnemyToPlayer chain (player attacker hostility)
                // SKIP: lua:3233-3241 — BecomeEnemyToPlayer/SetRelationshipMemory/ResetFollowBehavior — Phase 3 player + relationship memory
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
                // lua:3256 — decalPos = myPos + vecZ4
                // lua:3257 — self:SetLocalPos(decalPos)
                // lua:3258 — tr = util.TraceLine({start=decalPos, endpos=decalPos-vecZ500, filter=self})
                // lua:3259 — util.Decal(bloodDecal, tr.HitPos+tr.HitNormal, tr.HitPos-tr.HitNormal)
                // SKIP: lua:3256-3259 — TraceLine + util.Decal blood decal — Phase 3 decal system
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
        // SKIP: lua:3269-3272 — dmgAttacker:GetClass()=="npc_barnacle" / AddFrags — Phase 3 DamageInfo + Source engine score

        // lua:3273 — gamemode.Call("OnNPCKilled", self, dmgAttacker, dmgInflictor)
        // SKIP: lua:3273 — gamemode.Call("OnNPCKilled") — S&Box has no gamemode.Call; use Scene event

        // ---- Post-death setup (lua:3274-3277) ----
        // lua:3274 — self:SetCollisionGroup(COLLISION_GROUP_DEBRIS)
        // SKIP: lua:3274 — SetCollisionGroup(COLLISION_GROUP_DEBRIS) — Phase 3 collision groups
        // lua:3275 — self:GibOnDeath(dmginfo, hitgroup)
        GibOnDeath(dmginfo, hitgroup);
        // lua:3276 — self:PlaySoundSystem("Death")
        PlaySoundSystem("Death");
        // lua:3277 — //AA_StopMoving() commented out

        // ---- I/O events (lua:3280-3285) ----
        // lua:3280 — if IsValid(dmgAttacker) then
        // SKIP: lua:3280-3285 — TriggerOutput / Fire("KilledNPC") — Phase 3 I/O system (TriggerOutput stub exists)

        // ---- Death animation + delay → FinishDeath (lua:3288-3310) ----
        // lua:3288 — deathTime = self.DeathDelayTime
        float deathTime = DeathDelayTime;
        // lua:3289 — combine ball → HasDeathAnimation = false
        //   Covered by Dissolve tag check below (combine ball deals DMG_DISSOLVE)
        // lua:3290 — HasDeathAnimation && !DMG_REMOVENORAGDOLL && !DMG_DISSOLVE && NavType!=CLIMB
        if (HasDeathAnimation
            && !dmginfo.Tags.Has(VJDamageTags.Dissolve)
            && GetNavType() != (int)NavType.Climb
            && Game.Random.Next(1, Math.Max(1, DeathAnimationChance) + 1) == 1)
        {
            // lua:3291 — self:RemoveAllGestures()
            RemoveAllGestures();
            // lua:3292 — self:OnDeath(dmginfo, hitgroup, "DeathAnim")
            OnDeath(dmginfo, hitgroup, "DeathAnim");
            // lua:3293 — chosenAnim = PICK(self.AnimTbl_Death)
            // SKIP: lua:3293-3295 — PICK(AnimTbl_Death) / AnimDurationEx / PlayAnim — Phase 3 animation
            // lua:3296 — deathTime = deathTime + animTime
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
        // SKIP: lua:3302-3306 — timer.Simple(deathTime, ...) — Source engine timer; Phase 3 async/Task.Delay
        // Fallback: call FinishDeath immediately (revisit when async delay system is in place)
        FinishDeath(dmginfo, hitgroup);
    }

    // ═══ FinishDeath — creature_base/init.lua:3313-3325 ═══
    public virtual void FinishDeath(DamageInfo dmginfo, int hitgroup)
    {
        // lua:3314 — VJ_DEBUG + GetConVar debug print
        // SKIP: lua:3314 — VJ_DEBUG / GetConVar — Phase 3 debug system

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

        // lua:3321 — if not DMG_REMOVENORAGDOLL && not DMG_DISSOLVE then CreateDeathCorpse
        // S&Box: DMG_REMOVENORAGDOLL ≈ dissolve/removal damage; check via Tags
        if (!dmginfo.Tags.Has(VJDamageTags.Dissolve))
            CreateDeathCorpse(dmginfo, hitgroup);

        // lua:3322 — self:Remove()
        // SKIP: lua:3322 — self:Remove() — S&Box GameObject.Destroy() instead; Phase 3 entity removal lifecycle
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
        // SKIP: lua:3345 — self:GetModel() — Phase 3 model access
        // lua:3346 — corpseMdlCustom = PICK(self.DeathCorpseModel)
        // SKIP: lua:3346-3348 — PICK(DeathCorpseModel) / corpseMdl override — Phase 3
        // lua:3349 — corpseClass = "prop_physics"
        // SKIP: lua:3349 — corpseClass selection — Phase 3

        // ---- Entity class selection (lua:3350-3357) ----
        // lua:3350 — if self.DeathCorpseEntityClass then corpseClass = self.DeathCorpseEntityClass
        // lua:3351-3357 — else IsValidRagdoll/IsValidProp/IsValidModel checks
        // SKIP: lua:3350-3357 — DeathCorpseEntityClass / util.IsValidRagdoll / util.IsValidProp / util.IsValidModel — Phase 3 entity creation

        // ---- Entity creation (lua:3358-3364) ----
        // lua:3358 — corpse = ents.Create(corpseClass)
        // SKIP: lua:3358 — ents.Create — Phase 3 GameObject creation (GameObject.CreateObject/prefab)
        // lua:3359-3364 — corpse:SetModel/SetPos/SetAngles/Spawn/Activate
        // SKIP: lua:3359-3364 — SetModel/SetPos/SetAngles/Spawn/Activate — Phase 3
        GameObject corpse = null; // Phase 3: create GameObject via prefab or new GameObject()

        // ---- Copy appearance (lua:3365-3383) ----
        // SKIP: lua:3365 — corpse:SetSkin(self:GetSkin()) — Phase 3 ModelRenderer
        // SKIP: lua:3366-3367 — for bodygroup loop + corpse:SetBodygroup — Phase 3 ModelRenderer
        // SKIP: lua:3368 — corpse:SetColor(self:GetColor()) — Phase 3 Renderer.Tint
        // SKIP: lua:3369 — corpse:SetMaterial(self:GetMaterial()) — Phase 3 ModelRenderer
        // SKIP: lua:3370-3383 — submaterial copy loop — Phase 3

        // ---- Corpse metadata (lua:3386-3391) ----
        // lua:3386 — corpse.FadeCorpseType = (corpse:GetClass()=="prop_ragdoll" and "FadeAndRemove") or "kill"
        // SKIP: lua:3386 — FadeCorpseType + GetClass() — Phase 3
        // lua:3387 — corpse.IsVJBaseCorpse = true
        // SKIP: lua:3387 — IsVJBaseCorpse flag — Phase 3 entity flags
        // lua:3388 — corpse.DamageInfo = dmginfo
        // SKIP: lua:3388 — DamageInfo assignment — Phase 3
        // lua:3389 — corpse.ChildEnts = self.DeathCorpse_ChildEnts or {}
        // SKIP: lua:3389-3391 — ChildEnts / BloodData — Phase 3

        // ---- Blood pool (lua:3392-3394) ----
        // lua:3392-3394 — if self.Bleeds && self.HasBloodPool && vj_npc_blood_pool:GetInt()==1 then self:SpawnBloodPool(...)
        if (Bleeds && HasBloodPool)
        {
            // SKIP: lua:3393 — vj_npc_blood_pool convar — Phase 3
            SpawnBloodPool(dmginfo, hitgroup, corpse);
        }

        // ---- Collision (lua:3397-3404) ----
        // SKIP: lua:3397 — corpse:SetCollisionGroup(self.DeathCorpseCollisionType) — Phase 3 collision
        // SKIP: lua:3398-3399 — ai_serverragdolls convar + undo.ReplaceEntity — Phase 3
        // SKIP: lua:3400-3403 — VJ.Corpse_Add / undo.ReplaceEntity / cleanup.ReplaceEntity — Phase 3

        // ---- On fire (lua:3407-3413) ----
        // lua:3407 — if self:IsOnFire() then
        if (IsOnFire())
        {
            // lua:3408 — corpse:Ignite(math.Rand(8, 10), 0)
            // SKIP: lua:3408 — corpse:Ignite — Phase 3 (S&Box Prop.Ignite exists but different API)
            // lua:3409-3411 — if !self.Immune_Fire then corpse:SetColor(colorGrey)
            // SKIP: lua:3409-3411 — SetColor(colorGrey) fire darkening — Phase 3 ModelRenderer
        }

        // ---- Dissolve (lua:3416-3418) ----
        // SKIP: lua:3416-3418 — DMG_DISSOLVE / prop_combine_ball check + corpse:Dissolve — Phase 3 (S&Box no dissolve)

        // ---- Bone physics (lua:3422-3448) ----
        // SKIP: lua:3422-3448 — useLocalVel / dmgForce calculation / phys loop:
        //        corpse:GetPhysicsObjectCount / GetPhysicsObjectNum / GetSurfaceArea
        //        self:GetBonePosition / corpse:TranslatePhysBoneToBone
        //        childPhysObj:SetAngles/SetPos/SetVelocity — Phase 3 physics (ModelPhysics)

        // ---- Health & stink (lua:3451-3456) ----
        // SKIP: lua:3451-3455 — corpse:Health()/SetMaxHealth/SetHealth (totalSurface/60) — Phase 3 HealthComponent
        // lua:3456 — VJ.Corpse_AddStinky(corpse, true)
        VJUtility.Corpse_AddStinky(corpse, true);

        // ---- Fade (lua:3458-3460) ----
        // SKIP: lua:3458 — if self.DeathCorpseFade then corpse:Fire(corpse.FadeCorpseType, nil, self.DeathCorpseFade) — Phase 3
        // SKIP: lua:3459 — vj_npc_corpse_fade convar + corpse:Fire — Phase 3
        // lua:3460 — self:OnCreateDeathCorpse(dmginfo, hitgroup, corpse)
        OnCreateDeathCorpse(dmginfo, hitgroup, corpse);

        // ---- Dissolve children (lua:3461-3465) ----
        // SKIP: lua:3461 — corpse:IsFlagSet(FL_DISSOLVING) — Phase 3 flags + Dissolve
        // SKIP: lua:3462-3464 — for child in ChildEnts → child:Dissolve — Phase 3

        // ---- CallOnRemove (lua:3466-3476) ----
        // SKIP: lua:3466-3476 — corpse:CallOnRemove callback + child cleanup loop:
        //        child:GetClass()=="prop_ragdoll" → Fire("FadeAndRemove") else Fire("kill")
        //        — Phase 3 (S&Box: Component.OnDestroy / DestroyAsync lifecycle)
        // lua:3477 — hook.Call("CreateEntityRagdoll", nil, self, corpse)
        // SKIP: lua:3477 — hook.Call — S&Box has no global hook system

        // lua:3478 — return corpse
        return corpse;

        // ---- Else (no corpse) branch (lua:3480-3486) — NOT reached in this path ----
        // The Lua else branch (lines 3480-3486) handles: remove child ents
        // In C#, if HasDeathCorpse is false or HasDeathRagdoll is false, we return null above (line 3344 gate).
        // The else branch cleanup is implicitly handled by the GameObject.Destroy lifecycle.
    }
}
