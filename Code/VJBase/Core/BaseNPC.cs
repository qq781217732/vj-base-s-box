using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase;

/// <summary>
/// Base NPC — ported from vj_base/ai/core.lua.
/// Shared data fields, AI utilities, and lifecycle methods for all VJ NPCs.
/// </summary>
public partial class BaseNPC : Component, INPCConditions, INPCSchedule, INPCAttributes
{
    // ═══ Core Data Fields ═══
    public bool IsVJBaseSNPC { get; set; } = true;
    public float NPCClass { get; set; }
    public float MaxYawSpeed { get; set; }
    public bool VJ_DEBUG { get; set; }
    public bool VJ_IsBeingControlled { get; set; }
    public bool VJ_IsBeingControlled_Tool { get; set; }
    public int SelectedDifficulty { get; set; } = 1;
    public VJState AIState { get; set; } = VJState.None;
    public float NextProcessT { get; set; }
    public bool IsFollowing { get; set; }
    public bool PauseAttacks { get; set; }
    public float AnimLockTime { get; set; }
    public float AnimPlaybackRate { get; set; } = 1;
    public VJAnimSet AnimModelSet { get; set; } = VJAnimSet.None;
    public int LastAnimSeed { get; set; }
    public VJAnimType LastAnimType { get; set; } = VJAnimType.None;
    public int AttackSeed { get; set; }
    public VJAttackType AttackType { get; set; } = VJAttackType.None;
    public VJAttackState AttackState { get; set; } = VJAttackState.None;
    public int AttackAnimDuration { get; set; }
    public float AttackAnimTime { get; set; }
    public float NextDoAnyAttackT { get; set; }
    public bool IsAbleToMeleeAttack { get; set; } = true;
    public bool IsAbleToRangeAttack { get; set; } = true;
    public bool IsAbleToLeapAttack { get; set; } = true;
    public bool MeleeAttack_IsPropAttack { get; set; }

    // ═══ Attack Timer Config — creature init.lua:265-326 ═══
    // "NextAnyAttackTime_*" — <=0 = auto calculate | >0 = fixed time (seconds)
    public float NextAnyAttackTime_Melee { get; set; } = -1f; // -1 = auto (Lua: false)
    public float NextMeleeAttackTime { get; set; } = 0.8f;
    public float NextAnyAttackTime_Range { get; set; } = -1f;
    public float NextRangeAttackTime { get; set; } = 3f;
    public float NextAnyAttackTime_Leap { get; set; } = -1f;
    public float NextLeapAttackTime { get; set; } = 3f;
    public float NextAnyAttackTime_Grenade { get; set; } = 3f;
    public float NextGrenadeAttackTime { get; set; } = 5f;
    public float GrenadeAttackThrowTime { get; set; } = 1f;

    // ═══ Attack Timer Runtime — polled in Think ═══
    public float AttackResetTime { get; set; }
    public float AttackReEnableTime { get; set; }
    public float GrenadeExecTime { get; set; } // Grenade start → execute call
    // Stashed grenade params for delayed execution
    public GameObject StashedGrenadeEnt { get; set; }
    public bool StashedGrenadeDisableOwner { get; set; }
    public object StashedGrenadeLandDir { get; set; }

    // ═══ Melee Attack Config — core.lua:249-285 ═══
    public bool HasMeleeAttack { get; set; } = true;
    public float MeleeAttackDamage { get; set; } = 10;
    public int MeleeAttackDamageType { get; set; } // DMG_SLASH
    public bool HasMeleeAttackKnockBack { get; set; }
    public float MeleeAttackDistance { get; set; }
    public float MeleeAttackAngleRadius { get; set; } = 100;
    public float MeleeAttackDamageDistance { get; set; }
    public float MeleeAttackDamageAngleRadius { get; set; } = 100;
    public int MeleeAttackReps { get; set; } = 1;
    public float[] MeleeAttackExtraTimers { get; set; }
    public bool MeleeAttackStopOnHit { get; set; }
    public float TimeUntilMeleeAttackDamage { get; set; }
    public bool MeleeAttackBleedEnemy { get; set; }
    public int MeleeAttackBleedEnemyChance { get; set; } = 3;
    public float MeleeAttackBleedEnemyDamage { get; set; } = 1;
    public float MeleeAttackBleedEnemyTime { get; set; } = 1;
    public int MeleeAttackBleedEnemyReps { get; set; } = 4;
    public bool DisableDefaultMeleeAttackDamageCode { get; set; }
    public object PropInteraction { get; set; } = true;
    public float PropInteraction_MaxScale { get; set; } = 1;
    public bool MeleeAttackPlayerSpeed { get; set; }
    public float MeleeAttackPlayerSpeedWalk { get; set; } = 100;
    public float MeleeAttackPlayerSpeedRun { get; set; } = 100;
    public float MeleeAttackPlayerSpeedTime { get; set; } = 5;
    public int MeleeAttackDSP { get; set; } = 32;
    public int MeleeAttackDSPLimit { get; set; } = 60;

    // ═══ Range Attack Config — core.lua:289-305 ═══
    public bool HasRangeAttack { get; set; }
    public List<string> RangeAttackProjectiles { get; set; }
    public List<string> RangeAttackEntityToSpawn { get; set; }
    public float RangeAttackMinDistance { get; set; } = 800;
    public float RangeAttackMaxDistance { get; set; } = 2000;
    public float RangeAttackAngleRadius { get; set; } = 100;
    public int RangeAttackReps { get; set; } = 1;
    public float[] RangeAttackExtraTimers { get; set; }
    public float TimeUntilRangeAttackProjectileRelease { get; set; }

    // ═══ Leap Attack Config — core.lua:309-329 ═══
    public bool HasLeapAttack { get; set; }
    public float LeapAttackDamage { get; set; } = 15;
    public int LeapAttackDamageType { get; set; } // DMG_SLASH
    public float LeapAttackMinDistance { get; set; } = 200;
    public float LeapAttackMaxDistance { get; set; } = 500;
    public float LeapAttackDamageDistance { get; set; } = 100;
    public float LeapAttackAngleRadius { get; set; } = 60;
    public int LeapAttackReps { get; set; } = 1;
    public float[] LeapAttackExtraTimers { get; set; }
    public bool LeapAttackStopOnHit { get; set; } = true;
    public bool DisableDefaultLeapAttackDamageCode { get; set; }
    public float TimeUntilLeapAttackDamage { get; set; }

    // ═══ Attack Callbacks (virtual — override in derived NPC types) ═══
    public virtual Vector3 MeleeAttackTraceOrigin() => WorldPosition + WorldRotation.Forward * 20;
    public virtual Vector3 MeleeAttackTraceDirection() => WorldRotation.Forward;
    public virtual Vector3 MeleeAttackKnockbackVelocity(GameObject ent) => (ent.WorldPosition - WorldPosition).Normal * 200;
    public virtual bool OnMeleeAttackExecute(string status, GameObject ent = null, bool isProp = false) => false;
    public virtual Vector3 RangeAttackProjPos(GameObject projectile) => WorldPosition + WorldRotation.Forward * 50;
    public virtual Vector3 RangeAttackProjVel(GameObject projectile) => (GetEnemy()?.WorldPosition - WorldPosition ?? Vector3.Forward).Normal * 800;
    public virtual bool OnRangeAttackExecute(string status, GameObject ent = null, GameObject projectile = null) => false;
    public virtual bool OnLeapAttackExecute(string status, GameObject ent = null) => false;
    public float NextIdleTime { get; set; }
    public float NextWanderTime { get; set; }
    public float NextChaseTime { get; set; }
    public VJAlertState Alerted { get; set; } = VJAlertState.None;
    public (float a, float b) AlertTimeout { get; set; } = (15, 20);
    public float EnemyTimeout { get; set; } = 15;
    public int CurrentReachableEnemies { get; set; }
    public float NextAlertResetT { get; set; }
    public bool Flinching { get; set; }
    public float NextFlinchT { get; set; }
    public float HealthRegenDelayT { get; set; }
    public float NextCombineBallDmgT { get; set; }
    public bool Dead { get; set; }
    public bool GibbedOnDeath { get; set; }
    public bool DeathAnimationCodeRan { get; set; }
    public float TakingCoverT { get; set; }
    public float NextOnPlayerSightT { get; set; }
    public bool LastHiddenZone_CanWander { get; set; } = true;
    public float LastHiddenZoneT { get; set; }
    public float NextFootstepSoundT { get; set; }
    public float NextBreathSoundT { get; set; }
    public float NextIdleSoundT { get; set; }
    public float IdleSoundBlockTime { get; set; }
    public float NextAlertSoundT { get; set; }
    public float NextCallForHelpT { get; set; }
    public float NextCallForHelpAnimationT { get; set; }
    public float NextLostEnemySoundT { get; set; }
    public float NextAllyDeathSoundT { get; set; }
    public float NextKilledEnemySoundT { get; set; }
    public float NextDamageAllyResponseT { get; set; }
    public float NextDamageByPlayerSoundT { get; set; }
    public float NextPainSoundT { get; set; }
    public float MainSoundPitchValue { get; set; }
    public float NextInvestigationMove { get; set; }
    public float NextInvestigateSoundT { get; set; }

    // ═══ Data Tables ═══
    public MedicData Medic { get; set; } = new();
    public FollowData Follow { get; set; } = new();
    public EnemyData Enemy { get; set; } = new();
    public TurnData Turn { get; set; } = new();
    public GuardData Guard { get; set; } = new();
    public AISchedule CurrentSchedule { get; set; }
    public string CurrentScheduleName { get; set; }
    public AITask CurrentTask { get; set; }
    public int? CurrentTaskID { get; set; }
    public bool CurrentTaskComplete { get; set; }
    public float TaskStartTime { get; set; }
    public bool bDoingEngineSchedule { get; set; }
    public float AnimLockT { get; set; }

    // ═══ Model/Appearance ═══
    public VJBloodColor BloodColor { get; set; } = VJBloodColor.Red;
    public bool UsePoseParameterMovement { get; set; }

    // ═══ Movement ═══
    public VJMoveType MovementType { get; set; } = VJMoveType.Ground;
    public bool CanTurnWhileStationary { get; set; }
    public bool CanTurnWhileMoving { get; set; }
    public bool TurningUseAllAxis { get; set; }
    public bool ConstantlyFaceEnemy { get; set; }

    // ═══ Perception Config ═══
    public float SightDistance { get; set; } = 6500;
    public float SightAngle { get; set; } = 156;
    public float ViewOffset { get; set; } = 64f;
    public VJBehavior Behavior { get; set; } = VJBehavior.Aggressive;

    // ═══ Perception System ═══
    public AISenses Senses { get; set; }

    // ═══ Relationship System (core.lua ENT:MaintainRelationships) ═══
    public List<GameObject> RelationshipEnts { get; set; } = new();
    public Dictionary<GameObject, Dictionary<string, object>> RelationshipMemory { get; set; } = new();
    public List<string> VJ_NPC_Class { get; set; } = new();
    private List<string> _cacheRelationshipClasses;
    public bool EnemyDetection { get; set; } = true;
    public bool EnemyXRayDetection { get; set; }
    public bool CanAlly { get; set; }
    public bool AlliedWithPlayerAllies { get; set; }
    public bool YieldToAlliedPlayers { get; set; }
    public bool CanInvestigate { get; set; } = true;
    public bool IsGuard { get; set; }
    public bool HasOnPlayerSight { get; set; }
    public float OnPlayerSightDistance { get; set; } = 1000;
    public int OnPlayerSightDispositionLevel { get; set; }
    public bool OnPlayerSightOnlyOnce { get; set; }
    public (float a, float b) OnPlayerSightNextTime { get; set; } = (1, 3);
    public float InvestigateSoundMultiplier { get; set; } = 1;
    public Action<BaseNPC, GameObject, int?, float> OnMaintainRelationships { get; set; }

    /// Relationship disposition storage: entity → Disposition enum value (core.lua funcAddEntityRelationship)
    private Dictionary<GameObject, int> _relationshipDisp = new();

    // core.lua: HandlePerceivedRelationship — custom perception callback
    // Signature: Func<self=the entity being perceived, perceiver=this NPC, distance, isFriendly>
    // Returns: int? disposition override (null = no override, let normal logic proceed)
    public Func<GameObject, GameObject, float, bool, int?> HandlePerceivedRelationship { get; set; }
    public bool IsDefaultNPC { get; set; }
    public Func<GameObject, GameObject, float, bool> CanBeEngaged { get; set; }
    public Action<GameObject> OnAlert { get; set; }

    // ═══ Attack Config ═══
    public bool MeleeAttackAnimationFaceEnemy { get; set; }
    public bool GrenadeAttackAnimationFaceEnemy { get; set; }
    public bool RangeAttackAnimationFaceEnemy { get; set; }
    public int LeapAttackAnimationFaceEnemy { get; set; }
    public bool LeapAttackHasJumped { get; set; }
    public VJWepAttackState WeaponAttackState { get; set; } = VJWepAttackState.None;

    // ═══ Collision ═══
    public int DeathCorpseCollisionType { get; set; }
    public bool CanGib { get; set; } = true;

    // ═══ List fields ═══
    public List<string> TimersToRemove { get; set; } = new();
    public List<Condition> IgnoreConditions { get; set; } = new();
    public Dictionary<string, object> EntityMemory { get; set; } = new();

    // ═══ INPCAttributes state (was NPCAttributes.cs Dictionary<GameObject, ...>) ═══
    public NPCState NPCState { get; set; } = NPCState.None;
    public int NavTypeVal { get; set; }
    public Vector3 LastPosition { get; set; }
    public float MoveDelay { get; set; }
    public float IdealYaw { get; set; }

    // ═══ INPCConditions state (was NPCConditions.cs Dictionary<GameObject, HashSet>) ═══
    private HashSet<Condition> _conditions = new();
    private List<Condition> _activeIgnoreConditions = new();
    // ═══ AI State ═══
    public virtual void SetState(VJState state, float time = 0) { AIState = state; }
    public virtual VJState GetState() => AIState;

    // ═══ Difficulty ═══
    public virtual float ScaleByDifficulty(float num) => num; // Phase 3: implement scaling

    // ═══ Busy checks ═══
    public virtual bool IsBusy(string checkType = null) => false; // Phase 3

    // ═══ Animation (stubs — Phase 3) ═══
    public virtual void PlayAnim(string animation, bool lockAnim = false, float lockTime = 0,
        bool faceEnemy = false, float delay = 0, object extraOptions = null, Action<object> customFunc = null) { }
    public virtual void MaintainIdleAnimation(bool force) { }
    public virtual void MaintainIdleBehavior(int? idleType = null) { }
    public virtual void PlaySequence(string animation) { }

    // ═══ Phase 3 stubs — called by SelectSchedule ═══
    /// <summary>CanFireWeapon — human init.lua:3476. Returns false (conservative) until Phase 3.</summary>
    public virtual bool CanFireWeapon(bool checkDistance, bool checkDistanceOnly) => false;

    /// <summary>DoCoverTrace — human init.lua:1294. Returns true = "behind cover" (conservative default).</summary>
    public virtual bool DoCoverTrace(Vector3 start, Vector3 end, bool checkForEntity = false, object options = null)
    {
        if (options != null && options is System.Collections.Generic.Dictionary<string, object> opts
            && opts.TryGetValue("SetLastHiddenTime", out var v))
            LastHiddenZoneT = Time.Now;
        return true;
    }

    /// <summary>TranslateActivity — human init.lua:2417. Pass-through until Phase 3 animation.</summary>
    public virtual string TranslateActivity(string animName) => animName;

    /// <summary>UpdatePoseParamTracking — human init.lua:3426. Phase 3 animation.</summary>
    public virtual void UpdatePoseParamTracking(bool reset) { }

    /// <summary>PlayIdleSound — core.lua:2836. Phase 3 sound system.</summary>
    public virtual void PlayIdleSound(float? delayA = null, float? delayB = null, bool hasEnemy = false) { }

    /// <summary>GetActiveWeapon — Source engine NPC:GetActiveWeapon. Returns null until Phase 3 weapon system.</summary>
    public virtual GameObject GetActiveWeapon() => null;

    /// <summary>GetBestSoundHint — Source engine NPC:GetBestSoundHint. Returns null until Phase 3 sound system.</summary>
    public virtual object GetBestSoundHint(int soundMask) => null;

    /// <summary>NearestPoint — Source engine NPC:NearestPoint(navmesh). Pass-through until Phase 3 nav system.</summary>
    public virtual Vector3 NearestPoint(Vector3 pos) => pos;

    /// <summary>SetMovementActivity — Source engine NPC:SetMovementActivity. Phase 3 animation.</summary>
    public virtual void SetMovementActivity(string activity) { }

    /// <summary>GetActivity — Source engine NPC:GetActivity. Returns null until Phase 3 animation.</summary>
    public virtual string GetActivity() => null;

    /// ═══ Attack Timer System — creature init.lua:1825-1859, core.lua:972-994 ═══
    /// <summary>GetAttackTimer — core.lua:972-994. Calculates timer delay for attack reset/re-enable.</summary>
    /// <param name="mainTime">NextAnyAttackTime_* value (<=0 = auto)</param>
    /// <param name="executionTime">TimeUntil*Damage value (<=0 = event-based)</param>
    /// <param name="animDur">AttackAnimDuration</param>
    public virtual float GetAttackTimer(float mainTime, float executionTime, float animDur)
    {
        float rate = MathF.Max(AnimPlaybackRate, 0.01f);
        // lua:974 — mainTime is nil/false → auto-calculate (sentinel: < 0, so 0 is a valid value)
        if (mainTime < 0)
        {
            // lua:976 — executionTime is nil/false (= event-based, <=0 sentinel)
            if (executionTime <= 0)
                return animDur / rate;
            // lua:979 — executionTime > 0 (= timer-based)
            else
            {
                // lua:981 — animDur <= 0 → discard animation, use execution time only
                if (animDur <= 0)
                    return executionTime / rate;
                // lua:984 — animDur > 0 → animation has duration, subtract execution delay
                else
                    return animDur - (executionTime / rate);
            }
        }
        // lua:988-992 — number given, use directly (includes 0 = immediate)
        // SKIP: lua:988 — istable(mainTime) random range (VJ.SET) — Phase 3
        else
        {
            return mainTime / rate;
        }
    }

    /// <summary>ScheduleAttackTimers — replaces Lua attackTimers table, sets polling fields.</summary>
    public virtual void ScheduleAttackTimers(bool skipStopAttacks = false)
    {
        float rate = MathF.Max(AnimPlaybackRate, 0.01f);
        var curTime = Time.Now;

        switch (AttackType)
        {
            case VJAttackType.Melee:
                if (!skipStopAttacks)
                    AttackResetTime = curTime + GetAttackTimer(NextAnyAttackTime_Melee, TimeUntilMeleeAttackDamage, AttackAnimDuration);
                AttackReEnableTime = curTime + GetAttackTimer(NextMeleeAttackTime, 0, 0);
                break;

            case VJAttackType.Range:
                if (!skipStopAttacks)
                    AttackResetTime = curTime + GetAttackTimer(NextAnyAttackTime_Range, TimeUntilRangeAttackProjectileRelease, AttackAnimDuration);
                AttackReEnableTime = curTime + GetAttackTimer(NextRangeAttackTime, 0, 0);
                break;

            case VJAttackType.Leap:
                if (!skipStopAttacks)
                    AttackResetTime = curTime + GetAttackTimer(NextAnyAttackTime_Leap, TimeUntilLeapAttackDamage, AttackAnimDuration);
                AttackReEnableTime = curTime + GetAttackTimer(NextLeapAttackTime, 0, 0);
                break;

            case VJAttackType.Grenade:
                if (!skipStopAttacks)
                    AttackResetTime = curTime + GetAttackTimer(NextAnyAttackTime_Grenade, GrenadeAttackThrowTime, AttackAnimDuration);
                AttackReEnableTime = curTime + 0.5f;
                break;
        }
    }

    /// <summary>StopAttacks — creature init.lua:2730-2745. Resets attack state, optionally cancels pending timers.</summary>
    public virtual void StopAttacks(bool checkTimers = false)
    {
        if (Dead) return;

        // lua:2735 — if checking timers and AttackState < Executed, re-trigger timers with skipStopAttacks
        if (checkTimers && AttackState < VJAttackState.Executed)
        {
            ScheduleAttackTimers(true);
        }

        // lua:2739-2742 — reset attack state
        AttackType = VJAttackType.None;
        AttackState = VJAttackState.Done;
        AttackSeed = 0;
        LeapAttackHasJumped = false;

        AttackResetTime = 0;
        MaintainAlertBehavior(false);
    }

    /// <summary>SpawnRangeProjectile — creature init.lua:2635-2657. Virtual, override in derived types for specific projectiles.</summary>
    public virtual void SpawnRangeProjectile(string projectileClass, GameObject target)
    {
        // Phase 3: actual S&Box prefab spawning — callers should override with game-specific projectile types.
        // Lua flow: ents.Create(projectileClass) → SetPos(spawnPos) → SetAngles(angleTowardEnemy) →
        //           OnRangeAttackExecute("PreSpawn") → SetOwner(self) → Spawn() → Activate() →
        //           SetVelocity(RangeAttackProjVel) → OnRangeAttackExecute("PostSpawn")
    }

    /// <summary>MapDamageTypeToTag — converts Source DMG_* int → VJDamageTags string for DamageInfo.Tags.</summary>
    public virtual string MapDamageTypeToTag(int damageType)
    {
        // Source DMG_* constants → S&Box tag strings
        // Common values: DMG_SLASH=2, DMG_CLUB=128, DMG_GENERIC=0, DMG_BULLET=1, DMG_BLAST=8
        return damageType switch
        {
            1 => VJDamageTags.Bullet,
            2 => VJDamageTags.Slash,
            8 => VJDamageTags.Blast,
            16 => VJDamageTags.Shock,
            32 => VJDamageTags.Burn,
            128 => VJDamageTags.Club,
            _ => VJDamageTags.Generic,
        };
    }

    /// <summary>ProcessAttackTimers — called from Think to check and fire delayed attack callbacks.</summary>
    public virtual void ProcessAttackTimers(float curTime)
    {
        // lua:3178-3183 — grenade start timer (ExecuteGrenadeAttack), fires independently
        if (GrenadeExecTime > 0 && curTime > GrenadeExecTime)
        {
            GrenadeExecTime = 0;
        }

        // lua:1833-1835 — re-enable timer (fires independently, may fire before reset)
        if (AttackReEnableTime > 0 && curTime > AttackReEnableTime)
        {
            AttackReEnableTime = 0;
            // Read AttackType directly — it may still be set even if state changes later
            var atkType = AttackType;
            switch (atkType)
            {
                case VJAttackType.Melee: IsAbleToMeleeAttack = true; break;
                case VJAttackType.Range: IsAbleToRangeAttack = true; break;
                case VJAttackType.Leap: IsAbleToLeapAttack = true; break;
            }
        }

        // lua:1827-1836 — reset timer → StopAttacks + MaintainAlertBehavior
        if (AttackResetTime > 0 && curTime > AttackResetTime)
        {
            AttackResetTime = 0;
            StopAttacks(false);
        }
    }

    // ═══ Target (generic, not enemy) ═══
    public GameObject Target { get; set; }
    public GameObject GetTarget() => Target;
    public void SetTarget(GameObject ent) => Target = ent;
    public Action<GameObject> OnInvestigate { get; set; }

    // ═══ Facing / Turn ═══
    public float TurnResetTime { get; set; }
    public float FacingIdealYawThreshold { get; set; } = 5f;

    public virtual Angles GetTurnAngle(Angles ang)
    {
        return TurningUseAllAxis ? ang : new Angles(0, ang.yaw, 0);
    }

    public virtual bool VisibleVec(Vector3 pos)
    {
        var myEye = EyePosition();
        var tr = Game.ActiveScene.Trace.Ray(myEye, pos)
            .WithoutTags("npc")
            .IgnoreGameObjectHierarchy(GameObject)
            .Run();
        return !tr.Hit || tr.EndPos.Distance(pos) < 4f;
    }

    public virtual bool Visible(GameObject ent)
    {
        return CanSee(ent);
    }

    public virtual bool IsFacingIdealYaw()
    {
        if (Turn.Type == VJFaceStatus.None) return true;
        float diff = Math.Abs(MathX.DeltaDegrees(GetIdealYaw(), GameObject.WorldRotation.Yaw()));
        return diff < FacingIdealYawThreshold;
    }

    public virtual void ResetTurnTarget()
    {
        Turn.Type = VJFaceStatus.None;
        Turn.Target = null;
        Turn.StopOnFace = false;
        Turn.IsSchedule = false;
        Turn.LastYaw = 0;
        TurnResetTime = 0;
    }

    // Compute the target yaw toward a world-space point
    private float YawToward(Vector3 targetPos)
    {
        var dir = (targetPos - GameObject.WorldPosition).Normal;
        return Rotation.LookAt(dir).Yaw();
    }

    // Compute full angles (pitch+yaw+roll) toward a world-space point
    private Angles AnglesToward(Vector3 targetPos)
    {
        var dir = (targetPos - GameObject.WorldPosition).Normal;
        return Rotation.LookAt(dir).Angles();
    }

    // Apply yaw rotation at MaxYawSpeed deg/s. Returns the new yaw.
    private float ApplyYawTurn(float targetYaw)
    {
        float currentYaw = GameObject.WorldRotation.Yaw();
        float delta = MathX.DeltaDegrees(currentYaw, targetYaw);
        float maxTurn = MaxYawSpeed * Time.Delta;
        float turn = Math.Clamp(delta, -maxTurn, maxTurn);
        float newYaw = currentYaw + turn;
        GameObject.WorldRotation = Rotation.FromYaw(newYaw);
        return newYaw;
    }

    // Apply full-axis rotation toward a target rotation at MaxYawSpeed deg/s
    private void ApplyFullAxisTurn(Rotation targetRot)
    {
        Rotation currentRot = GameObject.WorldRotation;
        float angle = currentRot.Distance(targetRot);
        float maxTurn = MaxYawSpeed * Time.Delta;
        float t = angle > 0.001f ? Math.Min(maxTurn / angle, 1f) : 1f;
        GameObject.WorldRotation = Rotation.Slerp(currentRot, targetRot, t, true);
    }

    /// <summary>
    /// SetTurnTarget — core.lua:1043-1112.
    /// target: "Enemy" string, Vector3 position, or GameObject entity.
    /// faceTime: -1 = forever, 0 = single frame, &gt;0 = duration in seconds.
    /// </summary>
    public virtual Angles SetTurnTarget(object target, float faceTime = 0, bool stopOnFace = false, bool visibleOnly = false)
    {
        if (MovementType == VJMoveType.Stationary && !CanTurnWhileStationary) return default;

        Angles resultAng = default;
        bool updateTurn = true;

        if (target is string s && s == "Enemy")
        {
            ResetTurnTarget();
            var ene = GetEnemy();
            if (ene.IsValid())
            {
                if (TurningUseAllAxis)
                {
                    var targetPos = WorldSpaceCenter_Entity(ene); // OBB center, already world-space
                    resultAng = GetTurnAngle(AnglesToward(targetPos));
                    ApplyFullAxisTurn(Rotation.LookAt((targetPos - GameObject.WorldPosition).Normal));
                }
                else
                {
                    resultAng = GetTurnAngle(AnglesToward(ene.WorldPosition)); // feet, per Lua
                    ApplyYawTurn(YawToward(ene.WorldPosition));
                }
            }
            else
            {
                resultAng = GetTurnAngle(GameObject.WorldRotation.Angles());
                updateTurn = false;
            }
            if (faceTime != 0)
                Turn.Type = visibleOnly ? VJFaceStatus.EnemyVisible : VJFaceStatus.Enemy;
        }
        else if (target is Vector3 vec)
        {
            ResetTurnTarget();
            resultAng = GetTurnAngle(AnglesToward(vec));
            if (TurningUseAllAxis)
                ApplyFullAxisTurn(Rotation.LookAt((vec - GameObject.WorldPosition).Normal));
            else
                ApplyYawTurn(YawToward(vec));
            if (faceTime != 0)
            {
                Turn.Type = visibleOnly ? VJFaceStatus.PositionVisible : VJFaceStatus.Position;
                Turn.Target = vec;
            }
        }
        else if (target is GameObject ent && ent.IsValid())
        {
            ResetTurnTarget();
            if (TurningUseAllAxis)
            {
                var targetPos = WorldSpaceCenter_Entity(ent); // OBB center, already world-space
                resultAng = GetTurnAngle(AnglesToward(targetPos));
                ApplyFullAxisTurn(Rotation.LookAt((targetPos - GameObject.WorldPosition).Normal));
            }
            else
            {
                resultAng = GetTurnAngle(AnglesToward(ent.WorldPosition)); // feet, per Lua
                ApplyYawTurn(YawToward(ent.WorldPosition));
            }
            if (faceTime != 0)
            {
                Turn.Type = visibleOnly ? VJFaceStatus.EntityVisible : VJFaceStatus.Entity;
                Turn.Target = ent;
            }
        }
        else
        {
            return default;
        }

        if (updateTurn)
            SetIdealYawAndUpdate(resultAng.yaw);
        else
            IdealYaw = resultAng.yaw;

        if (faceTime != 0)
        {
            Turn.StopOnFace = stopOnFace;
            Turn.LastYaw = resultAng.yaw;
            TurnResetTime = faceTime > 0 ? Time.Now + faceTime : 0;
        }

        return resultAng;
    }

    public virtual void MaintainTurnTarget()
    {
        var turnData = Turn;
        if (turnData.Type == VJFaceStatus.None) return;

        // Auto-reset by faceTime
        if (TurnResetTime > 0 && Time.Now >= TurnResetTime)
        {
            ResetTurnTarget();
            return;
        }

        // StopOnFace: something else took over ideal yaw OR we're already facing target
        if (turnData.StopOnFace && (Math.Abs(MathX.DeltaDegrees(GetIdealYaw(), turnData.LastYaw)) > 1f || IsFacingIdealYaw()))
        {
            ResetTurnTarget();
            return;
        }

        turnData.LastYaw = 0;
        var turnTarget = turnData.Target;
        var ene = GetEnemy();
        bool eneValid = ene.IsValid();

        if (turnData.Type == VJFaceStatus.Position || (turnData.Type == VJFaceStatus.PositionVisible && turnTarget is Vector3 vec && VisibleVec(vec)))
        {
            if (turnTarget is Vector3 v)
            {
                float targetYaw = YawToward(v);
                if (TurningUseAllAxis)
                    ApplyFullAxisTurn(Rotation.LookAt((v - GameObject.WorldPosition).Normal));
                else
                    ApplyYawTurn(targetYaw);
                SetIdealYawAndUpdate(targetYaw);
                turnData.LastYaw = targetYaw;
            }
        }
        else if (turnTarget is GameObject ent && ent.IsValid()
            && (turnData.Type == VJFaceStatus.Entity || (turnData.Type == VJFaceStatus.EntityVisible && Visible(ent))))
        {
            float targetYaw;
            if (TurningUseAllAxis)
            {
                var targetPos = WorldSpaceCenter_Entity(ent); // OBB center, already world-space
                ApplyFullAxisTurn(Rotation.LookAt((targetPos - GameObject.WorldPosition).Normal));
                targetYaw = YawToward(targetPos);
            }
            else
            {
                targetYaw = YawToward(ent.WorldPosition); // feet, per Lua
                ApplyYawTurn(targetYaw);
            }
            SetIdealYawAndUpdate(targetYaw);
            turnData.LastYaw = targetYaw;
        }
        else if (eneValid && !Dead
            && (turnData.Type == VJFaceStatus.Enemy || (turnData.Type == VJFaceStatus.EnemyVisible && Enemy.Visible)))
        {
            float targetYaw;
            if (TurningUseAllAxis)
            {
                var targetPos = WorldSpaceCenter_Entity(ene); // OBB center, already world-space
                ApplyFullAxisTurn(Rotation.LookAt((targetPos - GameObject.WorldPosition).Normal));
                targetYaw = YawToward(targetPos);
            }
            else
            {
                targetYaw = YawToward(ene.WorldPosition); // feet, per Lua
                ApplyYawTurn(targetYaw);
            }
            SetIdealYawAndUpdate(targetYaw);
            turnData.LastYaw = targetYaw;
        }
    }

    // ═══ Damage ═══
    public virtual void Flinch(object dmginfo, int hitgroup) { Flinching = true; }
    public virtual int GetLastDamageHitGroup() => 0;
    public virtual float GetLastDamageTime() => 0;
    public virtual int GetTotalDamageCount() => 0;

    // ═══ Allies ═══
    public virtual void Allies_CallHelp(float dist) { }
    public virtual void Allies_Check(float dist) { }

    // ═══ Death / Gib ═══
    public virtual bool IsGibDamage(int dmgType) => false;
    public virtual void GibOnDeath(object dmginfo, int hitgroup) { GibbedOnDeath = true; }

    // ═══ Phase 3 engine stubs (called by OnTakeDamage) ═══
    public virtual bool IsOnFire() => false;
    public virtual int WaterLevel() => 0;
    public virtual void Extinguish() { }
    public virtual void SpawnBloodParticles(object dmginfo, int hitgroup) { }
    public virtual void SpawnBloodDecals(object dmginfo, int hitgroup) { }
    public virtual void TriggerOutput(string output, GameObject activator) { }
    public virtual void MarkTookDamageFromEnemy(GameObject attacker) { }
    public virtual void SetSaveValue(string key, object value) { }
    public virtual void Allies_Bring(string formation, float dist, object allies, int count) { }
    public virtual void SetRelationshipMemory(GameObject ent, string key, object value) { }
    public virtual float GetMaxLookDistance() => SightDistance;
    public virtual int CheckRelationship(GameObject ent) => (int)VJBase.Disposition.Neutral;
    public virtual bool OnDangerDetected(VJDangerType dangerType, GameObject dangerEnt = null) => false;
    public virtual void OnResetEnemy() { }
    public virtual void MarkEnemyAsEluded(GameObject ent) { }
    public virtual void ClearEnemyMemory(GameObject ent) { }
    public virtual Vector3 GetEnemyLastKnownPos() => Vector3.Zero;

    // ═══ Cleanup ═══
    protected override void OnDestroy()
    {
        StopAllSounds();
        base.OnDestroy();
    }

    // ═══════════════════════════════════════════════
    // INPCConditions — direct implementation (was NPCConditions.cs)
    // ═══════════════════════════════════════════════

    public void SetCondition(Condition cond) { if (cond != Condition.None) _conditions.Add(cond); }
    public void ClearCondition(Condition cond) => _conditions.Remove(cond);
    public bool HasCondition(Condition cond) => _conditions.Contains(cond);
    public void SetIgnoreConditions(List<Condition> conds) => _activeIgnoreConditions = conds != null ? new List<Condition>(conds) : new();
    public void RemoveIgnoreConditions(List<Condition> conds) => _activeIgnoreConditions.Clear();
    public bool IsConditionIgnored(Condition cond) => _activeIgnoreConditions.Contains(cond);

    // ═══════════════════════════════════════════════
    // INPCAttributes — direct implementation (was NPCAttributes.cs)
    // ═══════════════════════════════════════════════

    public int GetNPCState() => (int)NPCState;
    public void SetNPCState(int state) => NPCState = (NPCState)state;
    public int GetNavType() => NavTypeVal;

    public GameObject GetEnemy() => Enemy.Target;
    public void SetEnemy(GameObject enemy) => Enemy.Target = enemy;

    public void SetLastPosition(Vector3 pos) => LastPosition = pos;
    public Vector3 GetLastPosition() => LastPosition;
    public float GetMaxYawSpeed() => MaxYawSpeed;
    public void SetMaxYawSpeed(float speed) => MaxYawSpeed = speed;

    public bool IsMoving()
    {
        var agent = GameObject.Components.Get<NavMeshAgent>();
        if (agent != null) return agent.IsNavigating;
        var rb = GameObject.Components.Get<Rigidbody>();
        return rb != null && rb.Velocity.Length > 10f;
    }

    public void StopMoving()
    {
        var agent = GameObject.Components.Get<NavMeshAgent>();
        agent?.Stop();
    }

    public void ClearGoal() => StopMoving();

    public void ClearSchedule()
    {
        // INPCSchedule: clear schedule state
        CurrentSchedule = null;
        CurrentScheduleName = null;
        CurrentTask = null;
        CurrentTaskID = null;
        CurrentTaskComplete = false;
        // INPCAttributes: stop movement
        StopMoving();
    }

    public float GetIdealYaw() => IdealYaw;
    public bool IsFacingIdealYaw() => false; // Phase 3
    public void SetIdealYawAndUpdate(float yaw) => IdealYaw = yaw;
    public float GetMoveDelay() => MoveDelay;

    // ═══ Relationship System (INPCAttributes) ═══

    public int Disposition(GameObject other)
    {
        if (other == null || !other.IsValid()) return (int)VJBase.Disposition.Error;
        return _relationshipDisp.TryGetValue(other, out var disp) ? disp : (int)VJBase.Disposition.Error;
    }

    public void AddEntityRelationship(GameObject other, int disposition, int priority)
    {
        if (other == null || !other.IsValid()) return;
        _relationshipDisp[other] = disposition;
    }

    // ═══════════════════════════════════════════════
    // Senses hooks — called by Engine/AISenses.cs
    // These are the GetOuter()→Method() calls from Source C++ CAI_Senses
    // ═══════════════════════════════════════════════

    /// <summary>Check if a GameObject is alive. VJ NPCs use !Dead flag; non-VJ entities default to alive.</summary>
    protected bool Alive(GameObject ent)
    {
        var npc = ent.Components.Get<BaseNPC>();
        if (npc != null) return !npc.Dead;
        // Phase 3: IDamageable integration for non-VJ entity alive checks
        return true; // default: assume alive when health status unknown
    }

    /// <summary>Eye position for sensing (traces, FOV checks)</summary>
    public virtual Vector3 EyePosition() => GameObject.WorldPosition + Vector3.Up * 64f;

    /// <summary>Hearing sensitivity multiplier — higher = hears farther</summary>
    public virtual float HearingSensitivity() => 1f;

    /// <summary>Sound interests bitmask — which SoundTypes this NPC cares about</summary>
    public virtual int GetSoundInterests() => 0; // Phase 3: per-NPC sound mask

    /// <summary>Called by CAI_Senses::CanHearSound to let NPC filter sounds</summary>
    public virtual bool QueryHearSound(SoundEvent pSound) => true;

    /// <summary>Called when NPC sees an entity — CAI_Senses::SeeEntity</summary>
    public virtual void OnSeeEntity(GameObject pSightEnt)
    {
        SetCondition(Condition.SeeEnemy);
        SetCondition(Condition.HaveEnemyLOS);
    }

    /// <summary>Called after Look() scan completes</summary>
    public virtual void OnLooked(int iDistance) { }

    /// <summary>Called after Listen() scan completes</summary>
    public virtual void OnListened() { }

    /// <summary>Called by ShouldSeeEntity to let NPC veto a sighting</summary>
    public virtual bool QuerySeeEntity(GameObject pSightEnt, bool bIsLook) => true;

    /// <summary>Called by GetClosestSound to let NPC filter sounds</summary>
    public virtual bool ShouldIgnoreSound(SoundEvent pSound) => false;

    /// <summary>FL_* flag check — Phase 3: proper flag system. Currently returns false.</summary>
    public virtual bool HasEntityFlag(GameObject ent, int flag) => false;

    /// <summary>Sound priority for best-sound selection</summary>
    public virtual int GetSoundPriority(SoundEvent pSound) => (int)SoundPriority.Medium;

    /// <summary>SF_NPC_WAIT_TILL_SEEN flag check — Phase 3: spawn flag system</summary>
    public virtual bool HasSpawnFlag(int flag) => false;

    /// <summary>Clear a spawn flag — Phase 3</summary>
    public virtual void ClearSpawnFlag(int flag) { }

    // ═══════════════════════════════════════════════
    // Perception tick — replaces GatherConditions
    // Condition production is now Engine/AISenses.cs
    // ═══════════════════════════════════════════════

    protected virtual void TickSenses()
    {
        Senses ??= new AISenses { Outer = this, LookDist = SightDistance };
        Senses.PerformSensing();

        // Feed seen entities into relationship system (core.lua: RelationshipEnts)
        var seen = Senses.GetFirstSeenEntity(SeenType.SeenHighPriority);
        while (seen != null)
        {
            if (!RelationshipEnts.Contains(seen))
                RelationshipEnts.Add(seen);
            seen = Senses.GetNextSeenEntity();
        }

        // VJ relationship maintenance: alliance, nearest-enemy, investigation, player push
        MaintainRelationships();
    }

    // ═══════════════════════════════════════════════
    // Enemy Management — core.lua:2043-2092
    // ForceSetEnemy, DoEnemyAlert, DoReadyAlert
    // ═══════════════════════════════════════════════

    /// <summary>UpdateEnemyMemory — core.lua: enemy memory. Phase 3: full memory decay system.</summary>
    protected virtual void UpdateEnemyMemory(GameObject ent, Vector3 pos)
    {
        if (ent == null || !ent.IsValid()) return;
        EntityMemory["enemy_pos"] = pos;
    }

    /// <summary>ForceSetEnemy — core.lua:2043</summary>
    public virtual void ForceSetEnemy(GameObject ent, bool stopMoving = false, bool maxPerf = false, bool hasEnemy = false)
    {
        // core.lua:2044-2048
        if (!maxPerf)
        {
            // core.lua:2045 — validation chain
            if (!ent.IsValid()
                || Behavior == VJBehavior.PassiveNature
                || !Alive(ent)
                || (ent.Tags.Has("player") && VJInit.vj_npc_ignoreplayers))
                return;

            // core.lua:2046: hasEnemy = IsValid(funcGetEnemy(self))
            hasEnemy = GetEnemy().IsValid();

            // core.lua:2047: funcAddEntityRelationship(self, ent, D_HT, 0)
            AddEntityRelationship(ent, (int)VJBase.Disposition.Hate, 0);
        }

        // core.lua:2049: self:SetEnemy(ent)
        SetEnemy(ent);
        Enemy.Target = ent;

        // core.lua:2050: self:UpdateEnemyMemory(ent, ent:GetPos())
        UpdateEnemyMemory(ent, ent.WorldPosition);

        // core.lua:2053: self:IgnoreEnemyUntil(ent, 0)
        // Clears Source engine's built-in reaction delay after first SetEnemy.
        // S&Box NavMeshAgent has no enemy reaction timer — no-op.

        // core.lua:2054: self:SetNPCState(NPC_STATE_COMBAT)
        SetNPCState((int)NPCState.Combat);

        // core.lua:2055: self.EnemyData.TimeSet = CurTime()
        Enemy.TimeSet = Time.Now;

        // core.lua:2056-2062
        if (!hasEnemy || Alerted != VJAlertState.Enemy)
        {
            // core.lua:2057-2059
            if (stopMoving && Alerted == VJAlertState.None)
            {
                ClearGoal();
                StopMoving();
            }
            // core.lua:2061: self:DoEnemyAlert(ent)
            DoEnemyAlert(ent);
        }
    }

    /// <summary>DoReadyAlert — core.lua:2066</summary>
    public virtual void DoReadyAlert()
    {
        // core.lua:2067: self.EnemyData.Reset = false
        Enemy.Reset = false;

        // core.lua:2068: self.Alerted = ALERT_STATE_READY
        Alerted = VJAlertState.Ready;

        // core.lua:2069: self:SetNPCState(NPC_STATE_ALERT)
        SetNPCState((int)NPCState.Alert);
    }

    /// <summary>DoEnemyAlert — core.lua:2072</summary>
    public virtual void DoEnemyAlert(GameObject ent)
    {
        // core.lua:2074: eneData.Distance = self:GetDistance(ent)
        Enemy.Distance = Vector3.DistanceBetween(GameObject.WorldPosition, ent.WorldPosition);

        // core.lua:2075: if self.Alerted == ALERT_STATE_ENEMY then return end
        if (Alerted == VJAlertState.Enemy) return;

        var curTime = Time.Now;

        // core.lua:2077: selfData.Alerted = ALERT_STATE_ENEMY
        Alerted = VJAlertState.Enemy;

        // core.lua:2078-2081: Fixes NPC switching from combat to alert
        if (GetNPCState() != (int)NPCState.Combat)
            SetNPCState((int)NPCState.Alert);

        // core.lua:2082: eneData.TimeAcquired = curTime
        Enemy.TimeAcquired = curTime;
        // core.lua:2083: eneData.VisibleTime = curTime
        Enemy.VisibleTime = curTime;
        // core.lua:2084: eneData.DistanceNearest = VJ.GetNearestDistance(self, ent)
        Enemy.DistanceNearest = VJUtility.GetNearestDistance(GameObject, ent);

        // core.lua:2085: eneData.Reset = false
        Enemy.Reset = false;

        // core.lua:2086: self:OnAlert(ent)
        OnAlert?.Invoke(ent);

        // core.lua:2087-2090: alert sound
        if (curTime > NextAlertSoundT)
        {
            PlaySoundSystem("Alert");
            NextAlertSoundT = curTime + VJUtility.Rand(NextSoundTime_Alert.a, NextSoundTime_Alert.b);
        }
    }
}

// ═══ Sub-Data Classes (ported from Lua tables) ═══

public class MedicData
{
    public string Status { get; set; } = "false";
    public GameObject Target { get; set; }
    public GameObject Prop { get; set; }
    public float Cooldown { get; set; }
}

public class FollowData
{
    public GameObject Target { get; set; }
    public float MinDist { get; set; }
    public bool Moving { get; set; }
    public bool StopAct { get; set; }
    public float NextUpdateT { get; set; }
}

public class EnemyData
{
    public GameObject Target { get; set; }
    public float Distance { get; set; }
    public float DistanceNearest { get; set; }
    public float TimeSet { get; set; }
    public float TimeAcquired { get; set; }
    public bool Visible { get; set; }
    public int VisibleCount { get; set; }
    public float VisibleTime { get; set; }
    public Vector3 VisiblePos { get; set; }
    public Vector3 VisiblePosReal { get; set; }
    public bool Reset { get; set; } = true;
}

public class TurnData
{
    public VJFaceStatus Type { get; set; } = VJFaceStatus.None;
    public object Target { get; set; }
    public bool StopOnFace { get; set; }
    public bool IsSchedule { get; set; }
    public float LastYaw { get; set; }
}

public class GuardData
{
    public object Position { get; set; }
    public object Direction { get; set; }
}
