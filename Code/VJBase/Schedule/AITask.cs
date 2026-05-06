using Sandbox;

namespace VJBase;

/// <summary>
/// VJ AI Task — ported from vj_ai_task.lua.
/// Represents a single task in a schedule (engine task or custom task).
/// </summary>
public class AITask
{
    public const int TypeEngine = 1;
    public const int TypeCustom = 2;

    public string TaskName { get; set; }
    public int? TaskID { get; set; }
    public float TaskData { get; set; }
    public int TaskType { get; set; }

    // Custom task callbacks — stored as string names for dynamic dispatch from Lua.
    // Phase 3: replace with Action/Func delegates.
    public string TaskFuncStart { get; set; }
    public string TaskFuncRun { get; set; }

    private readonly IEngineAITaskSystem _engine;

    public AITask(IEngineAITaskSystem engine = null)
    {
        _engine = engine;
        Init();
    }

    public void Init()
    {
        TaskType = 0;
    }

    public void InitEngine(string taskName, float taskData)
    {
        TaskName = taskName;
        TaskID = null;
        TaskData = taskData;
        TaskType = TypeEngine;
    }

    public void InitCustom(string taskName, string startFunc, string runFunc, float taskData)
    {
        TaskName = taskName;
        TaskFuncStart = startFunc;
        TaskFuncRun = runFunc;
        TaskID = null;
        TaskData = taskData;
        TaskType = TypeCustom;
    }

    public void Start(GameObject npc)
    {
        if (TaskType == TypeCustom)
        {
            if (string.IsNullOrEmpty(TaskFuncStart)) return;
            // Phase 3: replace string dispatch with proper delegate invocation
            // npc[TaskFuncStart](npc, TaskStatus.New, TaskData)
            Log.Info($"[AITask] Custom Start: {TaskFuncStart} on {npc.Name}");
        }
        else if (TaskType == TypeEngine)
        {
            if (!TaskID.HasValue)
                TaskID = _engine?.GetTaskID(TaskName) ?? 0;
            _engine?.StartEngineTask(npc, TaskID.Value, TaskData);
        }
    }

    public void Run(GameObject npc)
    {
        if (TaskType == TypeCustom)
        {
            if (string.IsNullOrEmpty(TaskFuncRun)) return;
            // Phase 3: replace string dispatch with proper delegate invocation
            // npc[TaskFuncRun](npc, TaskStatus.RunTask, TaskData)
            Log.Info($"[AITask] Custom Run: {TaskFuncRun} on {npc.Name}");
        }
        else if (TaskType == TypeEngine)
        {
            _engine?.RunEngineTask(npc, TaskID ?? 0, TaskData);
        }
    }

    public bool IsEngineType() => TaskType == TypeEngine;
    public bool IsCustomType() => TaskType == TypeCustom;
}
