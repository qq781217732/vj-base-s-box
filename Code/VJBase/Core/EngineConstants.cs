namespace VJBase;

/// <summary>
/// Source engine task name constants — from TASK_* strings in Lua.
/// </summary>
public static class EngineTask
{
    // Path finding
    public const string GetPathToLastPosition = "TASK_GET_PATH_TO_LASTPOSITION";
    public const string GetPathToTarget       = "TASK_GET_PATH_TO_TARGET";
    public const string GetPathToEnemy        = "TASK_GET_PATH_TO_ENEMY";
    public const string GetPathToEnemyLOS     = "TASK_GET_PATH_TO_ENEMY_LOS";
    public const string GetPathToRandomNode   = "TASK_GET_PATH_TO_RANDOM_NODE";

    // Movement execution
    public const string RunPath               = "TASK_RUN_PATH";
    public const string WalkPath              = "TASK_WALK_PATH";
    public const string RunPathFlee           = "TASK_RUN_PATH_FLEE";
    public const string RunPathTimed          = "TASK_RUN_PATH_TIMED";
    public const string WalkPathTimed         = "TASK_WALK_PATH_TIMED";
    public const string RunPathForUnits       = "TASK_RUN_PATH_FOR_UNITS";
    public const string WalkPathForUnits      = "TASK_WALK_PATH_FOR_UNITS";
    public const string RunPathWithinDist     = "TASK_RUN_PATH_WITHIN_DIST";
    public const string WalkPathWithinDist    = "TASK_WALK_PATH_WITHIN_DIST";
    public const string WeaponRunPath         = "TASK_WEAPON_RUN_PATH";
    public const string ItemRunPath           = "TASK_ITEM_RUN_PATH";
    public const string MoveToTargetRange     = "TASK_MOVE_TO_TARGET_RANGE";
    public const string MoveToGoalRange       = "TASK_MOVE_TO_GOAL_RANGE";
    public const string MoveAwayPath          = "TASK_MOVE_AWAY_PATH";

    // Facing
    public const string FaceTarget            = "TASK_FACE_TARGET";
    public const string FaceEnemy             = "TASK_FACE_ENEMY";
    public const string FacePlayer            = "TASK_FACE_PLAYER";
    public const string FaceLastPosition      = "TASK_FACE_LASTPOSITION";
    public const string FaceSavePosition      = "TASK_FACE_SAVEPOSITION";
    public const string FacePath              = "TASK_FACE_PATH";
    public const string FaceHintNode          = "TASK_FACE_HINTNODE";
    public const string FaceIdeal             = "TASK_FACE_IDEAL";
    public const string FaceReasonable        = "TASK_FACE_REASONABLE";

    // Cover
    public const string FindCoverFromOrigin   = "TASK_FIND_COVER_FROM_ORIGIN";
    public const string FindCoverFromEnemy    = "TASK_FIND_COVER_FROM_ENEMY";
    // Wait
    public const string Wait                  = "TASK_WAIT";
    public const string WaitForMovement       = "TASK_WAIT_FOR_MOVEMENT";

    // Control
    public const string SetToleranceDistance  = "TASK_SET_TOLERANCE_DISTANCE";
    public const string SetRouteSearchTime    = "TASK_SET_ROUTE_SEARCH_TIME";
    public const string StopMoving            = "TASK_STOP_MOVING";
    public const string Forget                = "TASK_FORGET";
    public const string IgnoreOldEnemies      = "TASK_IGNORE_OLD_ENEMIES";
    public const string StoreBestSound        = "TASK_STORE_BESTSOUND_REACTORIGIN_IN_SAVEPOSITION";
    public const string PlaySequence          = "TASK_PLAY_SEQUENCE";
    public const string PlaySequenceFaceEnemy = "TASK_PLAY_SEQUENCE_FACE_ENEMY";
    public const string SetActivity           = "TASK_SET_ACTIVITY";
    public const string ResetActivity         = "TASK_RESET_ACTIVITY";

    // VJ custom
    public const string VJPlayActivity        = "TASK_VJ_PLAY_ACTIVITY";
    public const string VJPlaySequence        = "TASK_VJ_PLAY_SEQUENCE";

    /// <summary>
    /// Tasks that involve movement — used by schedule to set HasMovement flag.
    /// </summary>
    public static readonly HashSet<string> MoveTasks = new()
    {
        WaitForMovement, MoveToTargetRange, MoveToGoalRange,
        WalkPath, WalkPathTimed, WalkPathForUnits, WalkPathWithinDist,
        RunPath, RunPathFlee, RunPathTimed, RunPathForUnits, RunPathWithinDist,
        WeaponRunPath, ItemRunPath,
    };
}
