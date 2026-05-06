using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// CreatureNPC Think + SelectSchedule — ported from npc_vj_creature_base/init.lua.
/// </summary>
public partial class CreatureNPC
{
    // ═══ Additional Fields ═══
    public bool HasBreathSound { get; set; }
    public bool HasSounds { get; set; } = true;
    public int BreathSoundLevel { get; set; }
    public float BreathSoundPitch { get; set; } = 100;
    public float NextSoundTime_Breath { get; set; } = 10;
    public float NextProcessTime { get; set; } = 0.1f;
    public List<string> SoundTbl_Breath { get; set; } = new();
    // ═══ Think — main AI loop ═══
    public virtual void Think()
    {
        var curTime = Time.Now;
        bool doHeavyProcesses = curTime > NextProcessT;
        if (doHeavyProcesses)
            NextProcessT = curTime + NextProcessTime;

        // Breath sounds (Phase 3)
        if (!Dead && HasBreathSound && HasSounds && curTime > NextBreathSoundT)
        {
            NextBreathSoundT = curTime + NextSoundTime_Breath;
        }

        OnThink();
        OnThinkActive();

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

        // SKIP: schedules.lua:228-267 — full TurnData system (VJ.FACE_POSITION/ENTITY/ENEMY + Visible variants)
        // Phase 3: TurningUseAllAxis, GetTurnAngle, SetIdealYawAndUpdate, LerpAngle integration
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
        sched.EngTask(doLOSChase ? EngineTask.GetPathToEnemyLOS : EngineTask.GetPathToEnemy, 0);
        sched.EngTask(EngineTask.RunPath, 0);
        sched.EngTask(EngineTask.WaitForMovement, 0);

        // SKIP: init.lua:1738-1742 — doLOSChase=true branch uses schedule_alert_chaseLOS with RunCode_OnFinish, RunCode_OnFail callbacks. Phase 3.

        StartSchedule(sched);
    }

    // ═══ Attack Stubs (Phase 3) ═══
    public virtual void ExecuteMeleeAttack(bool isPropAttack) { }
    public virtual void ExecuteRangeAttack() { }
    public virtual void ExecuteLeapAttack() { }

    // ═══ Death ═══
    public virtual void BeginDeath(object dmginfo, int hitgroup) { Dead = true; }
    public virtual void FinishDeath(object dmginfo, int hitgroup) { }
}
