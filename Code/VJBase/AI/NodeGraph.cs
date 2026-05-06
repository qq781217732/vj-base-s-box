using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// AI Node Graph — ported from vj_ai_nodegraph.lua.
/// Source engine node graph for AI navigation (replaced by NavMesh in S&Box).
/// Kept as stub for compatibility; Phase 3 uses NavMesh instead.
/// </summary>
public static class NodeGraph
{
    /// <summary>Get nearest node to position (Phase 3: use NavMesh.GetClosestPoint)</summary>
    public static Vector3? GetNearestNode(Vector3 pos)
        => Game.ActiveScene.NavMesh.GetClosestPoint(pos);

    /// <summary>Get random node within radius (Phase 3: use NavMesh.GetRandomPoint)</summary>
    public static Vector3? GetRandomNode(Vector3 center, float radius)
        => Game.ActiveScene.NavMesh.GetRandomPoint(center, radius);
}
