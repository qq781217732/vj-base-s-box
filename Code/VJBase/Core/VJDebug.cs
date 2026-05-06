using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ Debug utilities — ported from vj_base/debug.lua.
/// </summary>
public static class VJDebug
{
    /// <summary>VJ.DEBUG_Print — colored debug output</summary>
    public static void Print(GameObject ent, string name, string type, params object[] args)
    {
        var prefix = ent.IsValid() ? $"[{ent.Name}]" : "[VJ]";
        if (!string.IsNullOrEmpty(name))
            prefix += $" | {name}";

        var color = VJColors.Server;
        if (type == "error") color = VJColors.Red;
        else if (type == "warn") color = VJColors.Yellow;

        GlobalEngine.MsgC(color, $"{prefix} {string.Join(" ", args)}");
    }
}
