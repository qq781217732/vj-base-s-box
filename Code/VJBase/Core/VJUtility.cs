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
    /// <summary>VJ.PICK — random element from list</summary>
    public static T PICK<T>(IList<T> values)
    {
        if (values == null || values.Count == 0) return default;
        return values[Game.Random.Next(values.Count)];
    }

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

    /// <summary>VJ.Corpse_Add — funcs.lua corpse.lua:64-83. Corpse list management with limit (default 5).</summary>
    private static readonly List<GameObject> _corpseList = new();
    public static void Corpse_Add(GameObject corpse)
    {
        if (corpse == null || !corpse.IsValid()) return;
        _corpseList.Add(corpse);
        // lua: limit to 5 corpses, destroy oldest
        while (_corpseList.Count > 5)
        {
            var oldest = _corpseList[0];
            _corpseList.RemoveAt(0);
            if (oldest.IsValid()) oldest.Destroy();
        }
    }

    /// <summary>VJ.Corpse_AddStinky — funcs.lua corpse.lua:43-58. Mark corpse for stink timer. P2: timer deferred.</summary>
    public static void Corpse_AddStinky(GameObject corpse, bool checkMat = true)
    {
        if (corpse == null || !corpse.IsValid()) return;
        corpse.Tags.Add("vj_corpse_stinky");
    }

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

    // ═══ Trajectory Calculation (funcs.lua:497-625) ═══

    /// <summary>
    /// VJ.CalculateTrajectory — calculates projectile launch velocity.
    /// funcs.lua:497-625. Supports "Line" (straight, no-gravity) and "Curve" (parabolic arc).
    /// </summary>
    public static Vector3 CalculateTrajectory(GameObject self, GameObject target,
        string algorithmType, Vector3 startPos, Vector3 targetPos, float strength,
        float gravityOverride = 800f)
    {
        var npc = self?.Components.Get<BaseNPC>();
        bool debug = npc != null && npc.VJ_DEBUG;

        // lua:619-624 — prediction re-run: Phase 3 (needs GetAimPosition integration)
        // if predict is set, recalculate using GetAimPosition

        if (algorithmType == "Line")
        {
            return (targetPos - startPos).Normal * strength;
        }
        else if (algorithmType == "Curve")
        {
            float gravity = gravityOverride > 0 ? gravityOverride : 800f;
            float dist = startPos.Distance(targetPos);
            var midPoint = startPos + (targetPos - startPos) * 0.5f;

            float verticalAdjustment = MathF.Abs(startPos.z - targetPos.z)
                + Math.Clamp(strength, -dist, dist);
            if (dist > strength * 9.5f && dist > 2000f)
            {
                // lua:532 — debug: target too far for given arc
                if (debug) VJDebug.Print(self, "CalculateTrajectory", "warn", "Target is too far for the given arc strength, applying adjustment to avoid failure!");
                verticalAdjustment += dist * 0.1f;
            }

            midPoint.z += verticalAdjustment;

            // lua:551-553 — not enough trajectory space
            if (midPoint.z < startPos.z || midPoint.z < targetPos.z)
            {
                if (debug) VJDebug.Print(self, "CalculateTrajectory", "warn", "Not enough space, applying fail case velocity!");
                midPoint = targetPos;
            }

            float distance1 = midPoint.z - startPos.z;
            float distance2 = midPoint.z - targetPos.z;

            float time1 = MathF.Sqrt(MathF.Max(0.001f, distance1) / (0.5f * gravity));
            float time2 = MathF.Sqrt(MathF.Max(0.001f, distance2) / (0.5f * gravity));

            // lua:568-569 — debug: trajectory time too short
            if (debug && time1 < 0.1f)
                VJDebug.Print(self, "CalculateTrajectory", "error", "Probably failed because the trajectory time is below 0.1!");

            var result = (targetPos - startPos) / (time1 + time2);
            result.z = gravity * time1;
            return result;
        }

        // lua:613-615 — invalid algorithm type (always prints, no VJ_DEBUG guard)
        Log.Error($"[VJ] CalculateTrajectory | Called without a valid algorithm type!");
        return default;
    }
}
