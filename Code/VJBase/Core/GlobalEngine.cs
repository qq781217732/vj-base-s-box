using Sandbox;

namespace VJBase;

/// <summary>
/// Global engine functions — wraps S&Box APIs behind method calls.
/// Lua global functions map to static methods here.
/// </summary>
public static class GlobalEngine
{
    // Time
    public static float GetCurrentTime() => Time.Now;
    public static float GetFrameTime() => Time.Delta;

    // Realm — use IsProxy directly in Component subclasses
    // (IsProxy is a Component instance member, not available from static context)

    // Logging
    public static void MsgC(Color c, params object[] args) => Log.Info(string.Join(" ", args));
}
