using System.Linq;
using Sandbox;
using SWB.Player;

namespace VJBase;

/// <summary>
/// Engine task execution — maps Source engine TASK_* strings to S&Box NavMeshAgent + transform actions.
/// Phase 3: runs real movement, face, wait, and control tasks. Animation/cover tasks are Phase 4 stubs.
/// </summary>
public class EngineAITaskSystem : IEngineAITaskSystem
{
    private readonly Dictionary<string, int> _taskIDs = new();
    private int _nextID = 1;

    // Per-NPC task state — stored by (npc, taskId)
    private readonly Dictionary<(GameObject, int), float> _waitUntil = new();
    private readonly Dictionary<(GameObject, int), Vector3> _moveTarget = new();

    public int GetTaskID(string taskName)
    {
        if (_taskIDs.TryGetValue(taskName, out var id))
            return id;
        id = _nextID++;
        _taskIDs[taskName] = id;
        return id;
    }

    public void StartEngineTask(GameObject npc, int taskId, float data)
    {
        if (npc == null || !npc.IsValid()) return;
        var taskName = ResolveTaskName(taskId);
        var agent = npc.Components.Get<NavMeshAgent>();

        // ── Path finding tasks: compute NavMesh path ──
        if (taskName == EngineTask.GetPathToLastPosition)
        {
            var baseNpc = npc.Components.Get<BaseNPC>();
            var target = baseNpc?.GetLastPosition() ?? npc.WorldPosition;
            agent?.MoveTo(target);
            _moveTarget[(npc, taskId)] = target;
        }
        else if (taskName == EngineTask.GetPathToTarget)
        {
            var baseNpc = npc.Components.Get<BaseNPC>();
            var enemy = baseNpc?.GetEnemy();
            if (enemy != null && enemy.IsValid())
            {
                agent?.MoveTo(enemy.WorldPosition);
                _moveTarget[(npc, taskId)] = enemy.WorldPosition;
            }
        }
        else if (taskName == EngineTask.GetPathToEnemy
              || taskName == EngineTask.GetPathToEnemyLOS)
        {
            var baseNpc = npc.Components.Get<BaseNPC>();
            var enemy = baseNpc?.GetEnemy();
            if (enemy != null && enemy.IsValid())
            {
                agent?.MoveTo(enemy.WorldPosition);
                _moveTarget[(npc, taskId)] = enemy.WorldPosition;
            }
        }
        else if (taskName == EngineTask.GetPathToRandomNode)
        {
            var radius = data > 0 ? data : 350f;
            var randomPoint = Game.ActiveScene.NavMesh.GetRandomPoint(npc.WorldPosition, radius);
            if (randomPoint.HasValue)
            {
                agent?.MoveTo(randomPoint.Value);
                _moveTarget[(npc, taskId)] = randomPoint.Value;
            }
        }

        // ── Movement execution tasks: set speed and follow existing path ──
        else if (taskName == EngineTask.RunPath || taskName == EngineTask.RunPathFlee
              || taskName == EngineTask.WeaponRunPath || taskName == EngineTask.ItemRunPath)
        {
            if (agent != null) agent.MaxSpeed = 520f; // Source default run speed (~320 units/s ≈ 520 hammer)
            // Path already computed by preceding GET_PATH task; agent is already navigating
        }
        else if (taskName == EngineTask.WalkPath || taskName == EngineTask.WalkPathTimed
              || taskName == EngineTask.WalkPathForUnits || taskName == EngineTask.WalkPathWithinDist)
        {
            if (agent != null) agent.MaxSpeed = 220f; // Source default walk speed
        }
        else if (taskName == EngineTask.RunPathTimed)
        {
            if (agent != null) agent.MaxSpeed = 520f;
            _waitUntil[(npc, taskId)] = Time.Now + (data > 0 ? data : 1f);
        }
        else if (taskName == EngineTask.RunPathForUnits || taskName == EngineTask.WalkPathForUnits
              || taskName == EngineTask.RunPathWithinDist || taskName == EngineTask.WalkPathWithinDist)
        {
            // Phase 4: precise unit-distance movement. For now, let agent run to destination.
        }
        else if (taskName == EngineTask.MoveToTargetRange || taskName == EngineTask.MoveToGoalRange
              || taskName == EngineTask.MoveAwayPath)
        {
            // Phase 4: tactical positioning. For now, complete immediately.
            OnTaskDone(npc, taskId);
        }

        // ── Face tasks: rotate to target ──
        else if (IsFaceTask(taskName))
        {
            // Face rotation handled in Run step — just record start time for timeout
            _waitUntil[(npc, taskId)] = Time.Now + 3f; // 3s timeout
        }

        // ── Wait tasks ──
        else if (taskName == EngineTask.WaitForMovement)
        {
            // Will check IsNavigating in Run step
        }
        else if (taskName == EngineTask.Wait)
        {
            _waitUntil[(npc, taskId)] = Time.Now + (data > 0 ? data : 1f);
        }

        // ── Cover tasks: Phase 4 stub ──
        else if (taskName == EngineTask.FindCoverFromOrigin || taskName == EngineTask.FindCoverFromEnemy)
        {
            // Phase 4: cover finding. Complete immediately so schedule continues.
            OnTaskDone(npc, taskId);
        }

        // ── Control tasks ──
        else if (taskName == EngineTask.StopMoving)
        {
            agent?.Stop();
            OnTaskDone(npc, taskId);
        }
        else if (taskName == EngineTask.Forget || taskName == EngineTask.IgnoreOldEnemies
              || taskName == EngineTask.SetToleranceDistance || taskName == EngineTask.SetRouteSearchTime)
        {
            OnTaskDone(npc, taskId);
        }
        else if (taskName == EngineTask.StoreBestSound)
        {
            // Phase 4: sound system integration
            OnTaskDone(npc, taskId);
        }

        // ── Animation/Sequence tasks: Phase 4 stub ──
        else if (taskName == EngineTask.PlaySequence || taskName == EngineTask.PlaySequenceFaceEnemy
              || taskName == EngineTask.SetActivity || taskName == EngineTask.ResetActivity)
        {
            // Phase 4: Animgraph integration. Complete immediately for now.
            OnTaskDone(npc, taskId);
        }
    }

    public void RunEngineTask(GameObject npc, int taskId, float data)
    {
        if (npc == null || !npc.IsValid()) return;
        var taskName = ResolveTaskName(taskId);
        var agent = npc.Components.Get<NavMeshAgent>();

        // ── Path tasks: check if path was found ──
        if (IsPathTask(taskName))
        {
            if (agent == null || !agent.IsNavigating)
            {
                // If agent has a pending path, it's still computing. Otherwise done.
                OnTaskDone(npc, taskId);
            }
        }

        // ── Movement tasks: check arrival ──
        else if (IsMoveTask(taskName))
        {
            if (taskName == EngineTask.RunPathTimed || taskName == EngineTask.WalkPathTimed)
            {
                if (_waitUntil.TryGetValue((npc, taskId), out var deadline) && Time.Now >= deadline)
                    OnTaskDone(npc, taskId);
            }
            else
            {
                // Agent arrived at destination?
                if (agent == null || (!agent.IsNavigating && agent.Velocity.Length < 10f))
                    OnTaskDone(npc, taskId);
            }
        }

        // ── Face tasks: rotate toward target each frame, check angle ──
        else if (IsFaceTask(taskName))
        {
            var baseNpc = npc.Components.Get<BaseNPC>();
            if (baseNpc == null) { OnTaskDone(npc, taskId); return; }

            var targetPos = GetFaceTarget(npc, taskName, baseNpc);
            var dir = (targetPos - npc.WorldPosition).Normal;
            dir = dir.WithZ(0); // Face in horizontal plane

            if (dir.Length > 0.01f)
            {
                var targetRot = Rotation.LookAt(dir);
                npc.WorldRotation = Rotation.Lerp(npc.WorldRotation, targetRot, Time.Delta * 8f);
            }

            // Check if facing target (within ~10 degrees)
            var forward = npc.WorldRotation.Forward.WithZ(0).Normal;
            if (forward.Length > 0.01f && Vector3.Dot(forward, dir) > 0.98f)
                OnTaskDone(npc, taskId);

            // Timeout
            if (_waitUntil.TryGetValue((npc, taskId), out var timeout) && Time.Now > timeout)
                OnTaskDone(npc, taskId);
        }

        // ── Wait tasks ──
        else if (taskName == EngineTask.WaitForMovement)
        {
            if (agent == null || !agent.IsNavigating)
                OnTaskDone(npc, taskId);
        }
        else if (taskName == EngineTask.Wait)
        {
            if (_waitUntil.TryGetValue((npc, taskId), out var deadline) && Time.Now >= deadline)
                OnTaskDone(npc, taskId);
        }
    }

    public void TaskComplete(GameObject npc)
    {
        if (npc == null || !npc.IsValid()) return;
        var agent = npc.Components.Get<NavMeshAgent>();
        agent?.Stop();
        npc.Components.Get<BaseNPC>()?.OnTaskComplete();
    }

    public int GetCurGoalType(GameObject npc)
    {
        return npc?.Components.Get<NavMeshAgent>()?.IsNavigating == true ? 3 : 0;
    }

    // ═══ Helpers ═══

    private string ResolveTaskName(int taskId)
    {
        foreach (var kv in _taskIDs)
            if (kv.Value == taskId) return kv.Key;
        return "";
    }

    private void OnTaskDone(GameObject npc, int taskId)
    {
        // Clean up per-task state
        _waitUntil.Remove((npc, taskId));
        _moveTarget.Remove((npc, taskId));
        TaskComplete(npc);
    }

    private static bool IsPathTask(string name) =>
        name.Contains("GET_PATH_");

    private static bool IsMoveTask(string name) =>
        name.Contains("_PATH") || name == EngineTask.MoveToTargetRange
        || name == EngineTask.MoveToGoalRange || name == EngineTask.MoveAwayPath;

    private static bool IsFaceTask(string name) =>
        name.Contains("FACE_");

    private static Vector3 GetFaceTarget(GameObject npc, string taskName, BaseNPC baseNpc)
    {
        return taskName switch
        {
            var t when t == EngineTask.FaceTarget || t == EngineTask.FaceEnemy =>
                baseNpc.GetEnemy()?.WorldPosition ?? npc.WorldPosition + npc.WorldRotation.Forward * 100f,

            EngineTask.FacePlayer =>
                Game.ActiveScene.GetAllComponents<PlayerBase>().FirstOrDefault()?.GameObject.WorldPosition
                    ?? npc.WorldPosition + npc.WorldRotation.Forward * 100f,

            EngineTask.FaceLastPosition =>
                baseNpc.GetLastPosition(),

            EngineTask.FaceSavePosition =>
                baseNpc.GetLastPosition(), // Phase 4: separate save position

            EngineTask.FacePath =>
                npc.WorldPosition + npc.WorldRotation.Forward * 100f, // Phase 4: path direction

            _ => npc.WorldPosition + npc.WorldRotation.Forward * 100f,
        };
    }
}
