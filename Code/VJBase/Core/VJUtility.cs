using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ utility functions — ported from vj_base/funcs.lua.
/// PICK, SET, HasValue, debug print, etc.
/// </summary>
public static class VJUtility
{
    /// <summary>VJ.PICK — random element from list, or return the value if not a list</summary>
    public static T PICK<T>(IList<T> values)
    {
        if (values == null || values.Count == 0) return default;
        return values[Game.Random.Next(values.Count)];
    }

    /// <summary>VJ.PICK overload for single value (just returns it)</summary>
    public static T PICK<T>(T value) where T : class => value;

    /// <summary>VJ.SET — creates a min/max pair (like {a, b} in Lua)</summary>
    public static (float a, float b) SET(float a, float b) => (a, b);

    /// <summary>VJ.HasValue — checks if table contains value</summary>
    public static bool HasValue<T>(IEnumerable<T> items, T value) where T : class
        => items?.Contains(value) ?? false;

    /// <summary>Random float in range [a, b]</summary>
    public static float Rand(float a, float b) => a + Game.Random.NextSingle() * (b - a);

    /// <summary>Random int in range [a, b]</summary>
    public static int RandInt(int a, int b) => Game.Random.Next(a, b + 1);

    /// <summary>Get animation duration (Phase 3)</summary>
    public static float AnimDuration(GameObject ent, string anim)
    {
        // Phase 3: query SkinnedModelRenderer for sequence duration
        return 1f;
    }

    /// <summary>Nearest positions between two entities (Phase 3)</summary>
    public static Vector3 GetNearestPositions(GameObject self, GameObject other, bool centerNPC = false)
        => other.WorldPosition;

    /// <summary>Nearest distance between two entities (Phase 3)</summary>
    public static float GetNearestDistance(GameObject self, GameObject other, bool centerNPC = false)
        => Vector3.DistanceBetween(self.WorldPosition, other.WorldPosition);
}
