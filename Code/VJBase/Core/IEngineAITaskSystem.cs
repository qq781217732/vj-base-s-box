using Sandbox;

namespace VJBase;

// ═══ Task Status ═══
public enum TaskStatus
{
    New = 0,    // TASKSTATUS_NEW
    RunTask = 1 // TASKSTATUS_RUN_TASK
}

// ═══ Engine AI Task System ═══
public interface IEngineAITaskSystem
{
    int GetTaskID(string taskName);
    void StartEngineTask(GameObject npc, int taskId, float data);
    void RunEngineTask(GameObject npc, int taskId, float data);
    void TaskComplete(GameObject npc);
    int GetCurGoalType(GameObject npc);
}
