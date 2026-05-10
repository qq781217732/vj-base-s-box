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

    /// <summary>
    /// Get world-space OBB center for any GameObject.
    /// Source: ent:GetPos() + ent:OBBCenter(). S&Box: ModelRenderer.Bounds.Center or WorldPosition.
    /// </summary>
    private static Vector3 GetOBBWorldCenter(GameObject ent)
    {
        if (ent == null || !ent.IsValid()) return Vector3.Zero;
        var npc = ent.Components.Get<BaseNPC>();
        if (npc != null) return npc.WorldSpaceCenter();
        var renderer = ent.Components.Get<ModelRenderer>();
        if (renderer != null) return renderer.Bounds.Center;
        return ent.WorldPosition;
    }

    /// <summary>
    /// Find nearest point on entity to a target position.
    /// Source: ent:NearestPoint(targetPos). S&Box: Rigidbody.FindClosestPoint or BBox clamp.
    /// </summary>
    private static Vector3 FindNearestPoint(GameObject ent, Vector3 targetPos)
    {
        if (ent == null || !ent.IsValid()) return targetPos;
        var rb = ent.Components.Get<Rigidbody>();
        if (rb != null && rb.PhysicsBody != null)
            return rb.FindClosestPoint(targetPos);
        var renderer = ent.Components.Get<ModelRenderer>();
        if (renderer != null)
        {
            var b = renderer.Bounds;
            return new Vector3(
                Math.Clamp(targetPos.x, b.Mins.x, b.Maxs.x),
                Math.Clamp(targetPos.y, b.Mins.y, b.Maxs.y),
                Math.Clamp(targetPos.z, b.Mins.z, b.Maxs.z));
        }
        return ent.WorldPosition;
    }

    /// <summary>VJ.GetNearestPositions — funcs.lua:146-159. Two entities' nearest points to each other.</summary>
    public static (Vector3 nearSelf, Vector3 nearOther) GetNearestPositions(GameObject self, GameObject other, bool centerNPC = false)
    {
        var otherCenter = GetOBBWorldCenter(other);
        var nearSelf = FindNearestPoint(self, otherCenter);
        if (centerNPC)
        {
            nearSelf.x = self.WorldPosition.x;
            nearSelf.y = self.WorldPosition.y;
        }
        var nearOther = FindNearestPoint(other, nearSelf);
        return (nearSelf, nearOther);
    }

    /// <summary>VJ.GetNearestDistance — funcs.lua:172-182. Distance between two entities' nearest surface points.</summary>
    public static float GetNearestDistance(GameObject self, GameObject other, bool centerNPC = false)
    {
        var (nearSelf, nearOther) = GetNearestPositions(self, other, centerNPC);
        return Vector3.DistanceBetween(nearSelf, nearOther);
    }

    /// <summary>VJ.AnimExists — checks if animation exists on entity (Phase 3)</summary>
    public static bool AnimExists(GameObject ent, string anim) => true;

    /// <summary>VJ.IsCurrentAnim — checks if entity is currently playing animation (Phase 3)</summary>
    public static bool IsCurrentAnim(GameObject ent, string anim) => false;

    /// <summary>VJ.Corpse_Add — funcs.lua corpse.lua:64-83. Corpse list management with limit. P2 (depends on timer + convar).</summary>
    public static void Corpse_Add(GameObject corpse) { /* P2: corpse limit system */ }

    /// <summary>VJ.Corpse_AddStinky — funcs.lua corpse.lua:43-58. Stink timer + material check. P2 (depends on timer).</summary>
    public static void Corpse_AddStinky(GameObject corpse, bool checkMat = true) { /* P2: stink timer system */ }

    /// <summary>VJ.AnimDurationEx — Calculate animation duration with override (Phase 3)</summary>
    public static float AnimDurationEx(GameObject ent, string anim, object overrideVal, float decrease = 0) => 0f;

    /// <summary>
    /// VJ.DamageSpecialEnts — funcs.lua:762-771. Apply extra damage effects to special entities.
    /// Source: npc_turret_floor selfdestruct + physics force. S&Box: component/tag-based check.
    /// </summary>
    public static void DamageSpecialEnts(GameObject attacker, GameObject ent, DamageInfo dmgInfo)
    {
        if (ent == null || !ent.IsValid()) return;
        // Source: ent:GetClass() == "npc_turret_floor". S&Box: check VJ_NPC_Class
        var npc = ent.Components.Get<BaseNPC>();
        if (npc == null || !npc.VJ_NPC_Class.Contains("npc_turret_floor")) return;
        if (ent.Tags.Has("self_destructing")) return;
        ent.Tags.Add("self_destructing");
        npc.TriggerOutput("selfdestruct", attacker);
        var rb = ent.Components.Get<Rigidbody>();
        if (rb != null)
        {
            rb.MotionEnabled = true;
            rb.ApplyForce(attacker.WorldRotation.Forward * 1000f);
        }
    }

    /// <summary>VJ.TraceDirections — funcs.lua:204-300. Multi-direction trace to find valid movement positions.</summary>
    public static List<Vector3> TraceDirections(BaseNPC npc, string trType, float maxDist = 200f,
        bool requireFullDist = true, int numDirections = 4,
        bool excludeForward = false, bool excludeBack = false,
        bool excludeLeft = false, bool excludeRight = false)
    {
        maxDist = maxDist <= 0 ? 200f : maxDist;
        numDirections = numDirections <= 0 ? 4 : numDirections;

        var entPos = npc.WorldPosition;
        var entPosZ = entPos.z;
        var entPosCentered = npc.WorldSpaceCenter();
        var myForward = npc.GameObject.WorldRotation.Forward;
        var myRight = npc.GameObject.WorldRotation.Right;
        var result = new List<Vector3>();

        if (trType == "Quick")
        {
            void RunTrace(Vector3 dir)
            {
                var tr = Game.ActiveScene.Trace.Ray(entPosCentered, entPosCentered + dir * maxDist)
                    .IgnoreGameObjectHierarchy(npc.GameObject)
                    .Run();
                if (!requireFullDist || entPos.Distance(tr.EndPosition) >= maxDist)
                {
                    var hitPos = tr.EndPosition;
                    hitPos.z = entPosZ;
                    result.Add(hitPos);
                }
            }

            if (!excludeForward)
            {
                RunTrace(myForward);
                if (numDirections >= 5)
                {
                    RunTrace((myForward - myRight).Normal);
                    RunTrace((myForward + myRight).Normal);
                }
            }
            if (!excludeBack)
            {
                RunTrace(-myForward);
                if (numDirections >= 5)
                {
                    RunTrace((-myForward - myRight).Normal);
                    RunTrace((-myForward + myRight).Normal);
                }
            }
            if (!excludeLeft) RunTrace(-myRight);
            if (!excludeRight) RunTrace(myRight);
        }
        else // "Radial"
        {
            var angleIncrement = (2f * MathF.PI) / numDirections;
            for (int i = 0; i < numDirections; i++)
            {
                var angle = i * angleIncrement;
                var dir = myForward * MathF.Cos(angle) + myRight * MathF.Sin(angle);
                var forwardDot = Vector3.Dot(dir, myForward);
                var rightDot = Vector3.Dot(dir, myRight);

                if ((excludeForward && forwardDot > 0.7f) ||
                    (excludeBack && forwardDot < -0.7f) ||
                    (excludeLeft && rightDot < -0.7f) ||
                    (excludeRight && rightDot > 0.7f))
                    continue;

                var tr = Game.ActiveScene.Trace.Ray(entPosCentered, entPosCentered + dir * maxDist)
                    .IgnoreGameObjectHierarchy(npc.GameObject)
                    .Run();
                if (!requireFullDist || entPos.Distance(tr.EndPosition) >= maxDist)
                {
                    var hitPos = tr.EndPosition;
                    hitPos.z = entPosZ;
                    result.Add(hitPos);
                }
            }
        }
        return result;
    }

    /// <summary>VJ.TraceDirections overload with GameObject for convenience.</summary>
    public static List<Vector3> TraceDirections(GameObject ent, string trType, float maxDist = 200f,
        bool requireFullDist = true, int numDirections = 4,
        bool excludeForward = false, bool excludeBack = false,
        bool excludeLeft = false, bool excludeRight = false)
    {
        var npc = ent.Components.Get<BaseNPC>();
        if (npc == null) return new List<Vector3>();
        return TraceDirections(npc, trType, maxDist, requireFullDist, numDirections,
            excludeForward, excludeBack, excludeLeft, excludeRight);
    }
}
