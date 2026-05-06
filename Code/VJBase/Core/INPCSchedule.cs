namespace VJBase;

public interface INPCSchedule
{
    void StartSchedule(AISchedule sched);
    void ClearSchedule();
    void StopCurrentSchedule();
    void NextTask(AISchedule sched);
    void DoSchedule(AISchedule sched);
    void ScheduleFinished(AISchedule sched);
    void SetTask(AITask task);
    void RunTask(AITask task);
    void StartTask(AITask task);
    bool TaskFinished();
    bool IsScheduleFinished(AISchedule sched);
    float TaskTime(float taskStartTime);
    void OnTaskComplete();
    void OnTaskFailed(int failCode, string failString);
    void OnMovementFailed();
    void OnMovementComplete();
}
