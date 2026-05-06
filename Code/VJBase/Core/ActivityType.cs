namespace VJBase;

/// <summary>
/// Source engine ACT_* activity constants.
/// S&Box uses Animgraph parameters instead, but these are needed for Lua translation.
/// </summary>
public enum ActivityType
{
    Invalid = -1,
    Idle = 0,
    Walk = 1,
    Run = 2,
    WalkAim = 3,
    RunAim = 4,
    DoNotDisturb = 5,
    Cower = 6,
    Jump = 7,
    ClimbUp = 8,
    // Add more as needed during translation
}
