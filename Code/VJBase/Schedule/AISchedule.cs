using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ AI Schedule — ported from vj_ai_schedule.lua.
/// Contains a list of tasks and execution metadata.
/// </summary>
public class AISchedule
{
    public string Name { get; set; }
    public List<AITask> Tasks { get; private set; }
    public int TaskCount { get; private set; }
    public bool HasMovement { get; set; }
    public bool ResetOnFail { get; set; }
    public bool CanBeInterrupted { get; set; }
    public bool CanShootWhenMoving { get; set; }
    public bool IsPlayActivity { get; set; }
    public bool FailureHandled { get; set; }
    public object TurnData { get; set; }
    public List<Condition> IgnoreConditions { get; set; }
    public Action RunCodeOnFail { get; set; }
    public Action RunCodeOnFinish { get; set; }
    public bool OnFailExecuted { get; set; }
    public bool OnFinishExecuted { get; set; }

    private readonly IEngineAITaskSystem _engine;

    public AISchedule(IEngineAITaskSystem engine = null)
    {
        _engine = engine ?? new EngineAITaskSystem();
        Init("");
    }

    public void Init(string name)
    {
        Name = name ?? "";
        Tasks = new List<AITask>();
        TaskCount = 0;
        HasMovement = false;
        ResetOnFail = false;
        CanBeInterrupted = false;
        CanShootWhenMoving = false;
        RunCodeOnFail = null;
        RunCodeOnFinish = null;
        OnFailExecuted = false;
        OnFinishExecuted = false;
    }

    /// <summary>Engine-defined task — Lua: schedule:EngTask(taskName, taskData)</summary>
    public void EngTask(string taskName, float taskData = 0)
    {
        var task = new AITask(_engine);
        task.InitEngine(taskName, taskData);
        Tasks.Add(task);
        TaskCount = Tasks.Count;
        if (EngineTask.MoveTasks.Contains(taskName))
            HasMovement = true;
    }

    /// <summary>Custom task with same name for start/run — Lua: schedule:AddTask(taskName, data)</summary>
    public void AddTask(string taskName, float data = 0)
    {
        var task = new AITask(_engine);
        task.InitCustom(taskName, taskName, taskName, data);
        Tasks.Add(task);
        TaskCount = Tasks.Count;
        if (EngineTask.MoveTasks.Contains(taskName))
            HasMovement = true;
    }

    /// <summary>Custom task with explicit start/run function names — Lua: schedule:AddTaskEx(taskName, startFunc, runFunc, data)</summary>
    public void AddTaskEx(string taskName, string startFunc, string runFunc, float data = 0)
    {
        var task = new AITask(_engine);
        task.InitCustom(taskName, startFunc, runFunc, data);
        Tasks.Add(task);
        TaskCount = Tasks.Count;
        if (EngineTask.MoveTasks.Contains(taskName))
            HasMovement = true;
    }

    public int NumTasks() => TaskCount;

    public AITask GetTask(int num) => num > 0 && num <= Tasks.Count ? Tasks[num - 1] : null;
}
