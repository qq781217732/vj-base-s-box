using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// Schedule lifecycle — StartSchedule, DoSchedule, NextTask, etc.
/// Ported from vj_base/ai/schedules.lua ENT methods.
/// BaseNPC directly implements INPCSchedule — no separate service class.
/// </summary>
public partial class BaseNPC
{
    // ═══ StartSchedule ═══
    public virtual void StartSchedule(AISchedule schedule)
    {
        if (schedule == null) return;

        // Stationary NPCs should not run movement schedules
        if (MovementType == VJMoveType.Stationary && schedule.HasMovement) return;

        // Certain states only allow animation schedules
        if (!schedule.IsPlayActivity)
        {
            var state = GetState();
            if (state >= VJState.OnlyAnimation) return;
        }

        // schedules.lua:359-361: same schedule + door/move-delay → skip
        var cur = CurrentSchedule;
        if (cur != null)
        {
            // SKIP: IsValid(self:GetInternalVariable("m_hOpeningDoor")) — Phase 3 door system
            if (schedule.Name == cur.Name && GetMoveDelay() > 0)
                return;

            if (!Dead)
                ScheduleFinished(cur);
        }

        ClearCondition(Condition.TaskFailed);

        // schedules.lua:398-413 — TurnData from schedule
        if (schedule.TurnData is TurnData turnData)
        {
            var faceType = turnData.Type;
            if (CanTurnWhileMoving && faceType != VJFaceStatus.None)
            {
                ResetTurnTarget();
                Turn.Type = faceType;
                Turn.Target = turnData.Target; // entity target or null (enemy types don't need a target)
                Turn.IsSchedule = true;
                Turn.LastYaw = 1;
            }
        }

        // Ignore conditions during this schedule
        if (schedule.IgnoreConditions != null)
            SetIgnoreConditions(schedule.IgnoreConditions);

        // schedules.lua:422-431 — Movement schedule → clear weapon state
        if (schedule.HasMovement)
        {
            LastHiddenZoneT = 0;
            // schedules.lua:424 — only clear if !CanShootWhenMoving or standing-fire
            if (!schedule.CanShootWhenMoving || WeaponAttackState == VJWepAttackState.FireStand)
                WeaponAttackState = VJWepAttackState.None;
            // schedules.lua:428 — gestures unaffected
            if (LastAnimType != VJAnimType.Gesture)
                LastAnimSeed = 0;
        }

        CurrentSchedule = schedule;
        CurrentScheduleName = schedule.Name;
        CurrentTaskID = 1;

        var firstTask = schedule.GetTask(1);
        SetTask(firstTask);
    }

    // ═══ DoSchedule ═══
    public virtual void DoSchedule(AISchedule schedule)
    {
        if (TaskFinished()) NextTask(schedule);
        var task = CurrentTask;
        if (task != null) RunTask(task);
    }

    // ═══ StopCurrentSchedule ═══
    public virtual void StopCurrentSchedule()
    {
        var schedule = CurrentSchedule;
        if (schedule == null) return;

        NextIdleTime = 0;
        NextChaseTime = 0;
        AnimLockTime = 0;
        ClearSchedule();
        ClearGoal();
        ScheduleFinished(schedule);
    }

    // ═══ ScheduleFinished ═══
    public virtual void ScheduleFinished(AISchedule schedule)
    {
        if (schedule == null) return;

        // RunCode_OnFinish
        if (!schedule.OnFinishExecuted && schedule.RunCodeOnFinish != null)
        {
            schedule.OnFinishExecuted = true;
            schedule.RunCodeOnFinish();
        }

        // COND_TASK_FAILED cleanup
        if (schedule.FailureHandled)
            ClearCondition(Condition.TaskFailed);

        // Reset turn data from schedule
        if (Turn.IsSchedule)
            ResetTurnTarget();

        // Remove ignored conditions
        if (schedule.IgnoreConditions != null)
            RemoveIgnoreConditions(schedule.IgnoreConditions);

        CurrentSchedule = null;
        CurrentScheduleName = null;
        CurrentTask = null;
        CurrentTaskID = null;
    }

    // ═══ SetTask ═══
    public virtual void SetTask(AITask task)
    {
        CurrentTask = task;
        CurrentTaskComplete = false;
        TaskStartTime = Time.Now;
        StartTask(task);
    }

    // ═══ NextTask ═══
    public virtual void NextTask(AISchedule schedule)
    {
        var id = (CurrentTaskID ?? 0) + 1;
        if (id > schedule.NumTasks())
        {
            ScheduleFinished(schedule);
            return;
        }
        CurrentTaskID = id;
        SetTask(schedule.GetTask(id));
    }

    // ═══ OnTaskComplete ═══
    public virtual void OnTaskComplete()
    {
        CurrentTaskComplete = true;
    }

    // ═══ TaskFinished ═══
    public virtual bool TaskFinished() => CurrentTaskComplete;

    // ═══ IsScheduleFinished ═══
    public virtual bool IsScheduleFinished(AISchedule schedule)
    {
        return CurrentTaskComplete && (!CurrentTaskID.HasValue || CurrentTaskID >= schedule.NumTasks());
    }

    // ═══ StartTask / RunTask ═══
    public virtual void StartTask(AITask task) => task?.Start(GameObject);
    public virtual void RunTask(AITask task) => task?.Run(GameObject);

    // ═══ TaskTime ═══
    public virtual float TaskTime() => Time.Now - TaskStartTime;
    float INPCSchedule.TaskTime(float taskStartTime) => Time.Now - taskStartTime;

    // ═══ OnTaskFailed ═══
    public virtual void OnTaskFailed(int failCode, string failString)
    {
        var curSchedule = CurrentSchedule;
        if (curSchedule == null) return;

        // Delay 0.05s to let engine settle values (Lua: timer.Simple(0.05, ...))
        // Phase 3: use GameTask.DelaySeconds for async
        HandleTaskFailed(curSchedule, failCode);
    }

    private void HandleTaskFailed(AISchedule curSchedule, int failCode)
    {
        if (curSchedule != CurrentSchedule) return; // Schedule changed during delay

        if (curSchedule.ResetOnFail)
        {
            curSchedule.FailureHandled = true;
            StopMoving();
        }

        // Skip FAIL_NO_ROUTE_ILLEGAL for pose-parameter movement
        if (failCode != 14 || (failCode == 14 && !UsePoseParameterMovement))
        {
            ClearGoal();
            NextTask(curSchedule);
        }

        // RunCode_OnFail
        if (!curSchedule.OnFailExecuted && curSchedule.RunCodeOnFail != null)
        {
            curSchedule.OnFailExecuted = true;
            curSchedule.RunCodeOnFail();
        }
    }

    // ═══ OnMovementFailed / OnMovementComplete ═══
    public virtual void OnMovementFailed() { /* Now handled in OnTaskFailed */ }
    public virtual void OnMovementComplete() { }

    // ═══ Engine stubs ═══
    public virtual void StartEngineTask(int taskId, float taskData) { }
    public virtual void RunEngineTask(int taskId, float taskData) { }

    // ═══ Pre-built wander schedule (lazy-init, shared across calls like Lua's SCHEDULE_IDLE_WANDER table) ═══
    private AISchedule _cachedWanderSchedule;
    private AISchedule CachedWanderSchedule
    {
        get
        {
            if (_cachedWanderSchedule == null)
            {
                _cachedWanderSchedule = new AISchedule();
                _cachedWanderSchedule.Init("SCHEDULE_IDLE_WANDER");
                _cachedWanderSchedule.EngTask(EngineTask.GetPathToRandomNode, 350);
                _cachedWanderSchedule.EngTask(EngineTask.WalkPath, 0);
                _cachedWanderSchedule.EngTask(EngineTask.WaitForMovement, 0);
                _cachedWanderSchedule.ResetOnFail = true;
                _cachedWanderSchedule.CanBeInterrupted = true;
            }
            return _cachedWanderSchedule;
        }
    }

    // ═══ AA movement — delegates to BaseNPC.AA.cs ═══
    protected virtual void DoAA_IdleWander() => AA_IdleWander(true, "Calm");
    protected virtual void DoAA_MoveTo(object dest, bool playAnim, string moveType) => AA_MoveTo(dest, playAnim, moveType);

    // ═══════════════════════════════════════════════
    // SCHEDULE builders — 1:1 from schedules.lua
    // ═══════════════════════════════════════════════

    // schedules.lua:14
    public virtual void SCHEDULE_FACE(string faceTask = null, Action<AISchedule> customFunc = null)
    {
        if (MovementType == VJMoveType.Stationary && !CanTurnWhileStationary) return;
        var sched = new AISchedule();
        sched.Init("SCHEDULE_FACE");
        sched.EngTask(faceTask ?? EngineTask.FaceTarget, 0);
        customFunc?.Invoke(sched);
        StartSchedule(sched);
    }

    // schedules.lua:22
    public virtual void SCHEDULE_GOTO_POSITION(string moveTask = null, Action<AISchedule> customFunc = null)
    {
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        {
            DoAA_MoveTo(GetLastPosition(), true, moveTask == EngineTask.RunPath ? "Alert" : "Calm");
            return;
        }
        var sched = new AISchedule();
        sched.Init("SCHEDULE_GOTO_POSITION");
        sched.EngTask(EngineTask.GetPathToLastPosition, 0);
        sched.EngTask(moveTask ?? EngineTask.RunPath, 0);
        sched.EngTask(EngineTask.WaitForMovement, 0);
        customFunc?.Invoke(sched);
        StartSchedule(sched);
    }

    // schedules.lua:43
    public virtual void SCHEDULE_GOTO_TARGET(string moveTask = null, Action<AISchedule> customFunc = null)
    {
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        {
            DoAA_MoveTo(GetEnemy(), true, moveTask == EngineTask.RunPath ? "Alert" : "Calm");
            return;
        }
        var sched = new AISchedule();
        sched.Init("SCHEDULE_GOTO_TARGET");
        sched.EngTask(EngineTask.GetPathToTarget, 0);
        sched.EngTask(moveTask ?? EngineTask.RunPath, 0);
        sched.EngTask(EngineTask.WaitForMovement, 0);
        sched.EngTask(EngineTask.FaceTarget, 1);
        customFunc?.Invoke(sched);
        StartSchedule(sched);
    }

    // schedules.lua:63
    public virtual void SCHEDULE_COVER_ENEMY(string moveTask = null, Action<AISchedule> customFunc = null)
    {
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        {
            DoAA_IdleWander();
            return;
        }
        var sched = new AISchedule();
        sched.Init("SCHEDULE_COVER_ENEMY");
        sched.EngTask(EngineTask.FindCoverFromOrigin, 0);
        sched.EngTask(moveTask ?? EngineTask.RunPath, 0);
        sched.EngTask(EngineTask.WaitForMovement, 0);
        sched.RunCodeOnFail = () =>
        {
            var fail = new AISchedule();
            fail.Init("SCHEDULE_COVER_ENEMY_FAIL");
            fail.EngTask(EngineTask.SetRouteSearchTime, 2);
            fail.EngTask(EngineTask.GetPathToRandomNode, 500);
            fail.EngTask(moveTask ?? EngineTask.RunPath, 0);
            fail.EngTask(EngineTask.WaitForMovement, 0);
            customFunc?.Invoke(fail);
            StartSchedule(fail);
        };
        customFunc?.Invoke(sched);
        StartSchedule(sched);
    }

    // schedules.lua:85
    public virtual void SCHEDULE_COVER_ORIGIN(string moveTask = null, Action<AISchedule> customFunc = null)
    {
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        {
            DoAA_IdleWander();
            return;
        }
        var sched = new AISchedule();
        sched.Init("SCHEDULE_COVER_ORIGIN");
        sched.EngTask(EngineTask.FindCoverFromOrigin, 0);
        sched.EngTask(moveTask ?? EngineTask.RunPath, 0);
        sched.EngTask(EngineTask.WaitForMovement, 0);
        sched.RunCodeOnFail = () =>
        {
            var fail = new AISchedule();
            fail.Init("SCHEDULE_COVER_ORIGIN_FAIL");
            fail.EngTask(EngineTask.SetRouteSearchTime, 2);
            fail.EngTask(EngineTask.GetPathToRandomNode, 500);
            fail.EngTask(moveTask ?? EngineTask.RunPath, 0);
            fail.EngTask(EngineTask.WaitForMovement, 0);
            customFunc?.Invoke(fail);
            StartSchedule(fail);
        };
        customFunc?.Invoke(sched);
        StartSchedule(sched);
    }

    // schedules.lua:106
    public virtual void SCHEDULE_IDLE_WANDER()
    {
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        {
            DoAA_IdleWander();
            return;
        }
        StartSchedule(CachedWanderSchedule);
    }

    // schedules.lua:114
    public virtual bool SCHEDULE_IDLE_STAND()
    {
        if (IsMoving() || NextIdleTime > Time.Now) return false;
        var navType = GetNavType();
        if (navType == (int)NavType.Jump || navType == (int)NavType.Climb) return false;
        if ((MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic) && IsBusy("IdleStand")) return false;
        MaintainIdleAnimation(true);
        return true;
    }

    // ═══════════════════════════════════════════════
    // VJ Custom Tasks — Phase 3 stubs

    // SKIP: S&Box Animgraph — Phase 3
    public virtual void TASK_VJ_PLAY_ACTIVITY(string activityName, float data = 0) { OnTaskComplete(); }

    // SKIP: S&Box Animgraph — Phase 3
    public virtual void TASK_VJ_PLAY_SEQUENCE(string sequenceName, float data = 0) { OnTaskComplete(); }

    // ═══════════════════════════════════════════════
    // Phase 3 stubs — not yet ported from schedules.lua

    // schedules.lua ENT:OnStateChange — called when NPCState changes
    public virtual void OnStateChange(NPCState oldState, NPCState newState) { }

    // schedules.lua ENT:TranslateNavGoal — resolves nav goal to position
    public virtual Vector3 TranslateNavGoal(int goalType) => GameObject.WorldPosition;
}
