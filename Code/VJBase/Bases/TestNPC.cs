using Sandbox;

namespace VJBase;

/// <summary>
/// Test NPC — validates F6+F7: Think → TickSenses → ForceSetEnemy → SelectSchedule → StartSchedule.
/// Spawn near a player and watch console.
/// </summary>
public class TestNPC : CreatureNPC
{
    private TimeUntil _nextThink;

    protected override void OnStart()
    {
        base.OnStart();
        _nextThink = 2f; // Wait 2s for player to spawn in

        SightDistance = 100000; // Can see anything
        SightAngle = 360;
        Log.Info("[TestNPC] Spawned. Will scan for enemies every 1s.");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (_nextThink > 0) return;
        _nextThink = 1f;

        // Step 1: Scan
        var eneBefore = GetEnemy();
        bool hadEnemy = eneBefore.IsValid();

        TickSenses();

        var eneAfter = GetEnemy();
        bool hasEnemy = eneAfter.IsValid();

        if (!hadEnemy && hasEnemy)
        {
            Log.Info($"[TestNPC] ✅ Found enemy: {eneAfter.Name} at {eneAfter.WorldPosition}");
            Log.Info($"[TestNPC]    HasCondition(SeeEnemy) = {HasCondition(Condition.SeeEnemy)}");
            Log.Info($"[TestNPC]    HasCondition(HaveEnemyLOS) = {HasCondition(Condition.HaveEnemyLOS)}");
            Log.Info($"[TestNPC]    Enemy.Distance = {Enemy.Distance}");
            Log.Info($"[TestNPC]    Enemy.Visible = {Enemy.Visible}");
            Log.Info($"[TestNPC]    Alerted = {Alerted}");
            Log.Info($"[TestNPC]    NPCState = {(NPCState)GetNPCState()}");
        }
        else if (hadEnemy && !hasEnemy)
        {
            Log.Info($"[TestNPC] Enemy lost. HasCondition(LostEnemy) = {HasCondition(Condition.LostEnemy)}");
        }
        else if (!hasEnemy)
        {
            Log.Info($"[TestNPC] No enemy found this tick.");
        }

        // Step 2: If enemy acquired, run full AI loop
        if (hasEnemy && (!hadEnemy || CurrentSchedule == null))
        {
            Log.Info("[TestNPC] Running SelectSchedule → StartSchedule...");
            SelectSchedule();

            if (CurrentSchedule != null)
            {
                Log.Info($"[TestNPC] ✅ Schedule started: {CurrentSchedule.Name}, Tasks: {CurrentSchedule.NumTasks()}");
                Log.Info($"[TestNPC]    CurrentTaskID: {CurrentTaskID}");
                Log.Info($"[TestNPC]    CurrentTask: {CurrentTask?.TaskName}");
            }
            else
            {
                Log.Info("[TestNPC] ❌ SelectSchedule did not create a schedule!");
            }
        }

        // Step 3: Run tasks
        if (CurrentSchedule != null)
        {
            RunAI();
        }
    }
}
