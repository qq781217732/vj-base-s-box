using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace VJBase;

/// <summary>
/// FindTransitionSequence replacement — pure resource-based, no Animgraph dependency.
///
/// Scans model sequence names at runtime for "{from}_to_{to}" naming patterns,
/// builds a transition graph, and provides lookup to enable smooth animation transitions.
///
/// Modelers just name their transition sequences following the convention and it works.
/// No per-model code config needed. Manual overrides available via Register().
/// </summary>
public static class VJTransitionTable
{
    // ── Cache ──
    // model path → (fromSeq → (toSeq → transitionSeq))
    private static readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _cache = new();

    // ── Known transition suffix patterns for scanning ──
    private static readonly string[] _delimiters = { "_to_", "_TO_", "_To_", "2" }; // "2" handles "idle2walk"

    // ═══ Public API ═══

    /// <summary>
    /// Find a transition sequence from currentSeq to targetSeq on the given model.
    /// Returns the transition sequence name if found, or null.
    /// Matches Source engine semantics: single-hop transition lookup.
    /// </summary>
    public static string FindTransitionSequence(SkinnedModelRenderer renderer, string currentSeq, string targetSeq)
    {
        if (renderer == null || string.IsNullOrEmpty(currentSeq) || string.IsNullOrEmpty(targetSeq))
            return null;

        var path = renderer.Model?.ResourcePath ?? renderer.Model?.Name ?? "";
        var graph = GetOrBuild(renderer, path);

        // Direct lookup: from → to
        if (graph.TryGetValue(currentSeq, out var edges)
            && edges.TryGetValue(targetSeq, out var transition))
        {
            return transition;
        }

        return null;
    }

    /// <summary>
    /// Find a multi-step transition path. Returns the NEXT hop sequence name
    /// (the first transition in the chain), or null if no path exists.
    /// Uses BFS to find shortest path through the transition graph.
    /// </summary>
    public static string FindTransitionPath(SkinnedModelRenderer renderer, string currentSeq, string targetSeq)
    {
        if (renderer == null || string.IsNullOrEmpty(currentSeq) || string.IsNullOrEmpty(targetSeq))
            return null;

        // Fast path: direct transition
        var direct = FindTransitionSequence(renderer, currentSeq, targetSeq);
        if (direct != null) return direct;

        // BFS through the transition graph
        var path = renderer.Model?.ResourcePath ?? renderer.Model?.Name ?? "";
        var graph = GetOrBuild(renderer, path);

        // BFS: each entry is (current, firstHop)
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { currentSeq };
        var queue = new Queue<(string node, string firstHop)>();

        // Seed with all direct neighbors
        if (graph.TryGetValue(currentSeq, out var neighbors))
        {
            foreach (var (toNode, transName) in neighbors)
            {
                if (string.Equals(toNode, targetSeq, StringComparison.OrdinalIgnoreCase))
                    return transName;
                if (visited.Add(toNode))
                    queue.Enqueue((toNode, transName));
            }
        }

        while (queue.Count > 0)
        {
            var (node, firstHop) = queue.Dequeue();
            if (!graph.TryGetValue(node, out var nextEdges)) continue;

            foreach (var (toNode, _) in nextEdges)
            {
                if (string.Equals(toNode, targetSeq, StringComparison.OrdinalIgnoreCase))
                    return firstHop;
                if (visited.Add(toNode))
                    queue.Enqueue((toNode, firstHop));
            }
        }

        return null;
    }

    /// <summary>
    /// Register a manual transition for a model. Call in NPC spawn.
    /// </summary>
    public static void Register(string modelPath, string fromSeq, string toSeq, string transitionSeq)
    {
        if (!_cache.TryGetValue(modelPath, out var graph))
        {
            graph = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            _cache[modelPath] = graph;
        }

        if (!graph.TryGetValue(fromSeq, out var edges))
        {
            edges = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            graph[fromSeq] = edges;
        }

        edges[toSeq] = transitionSeq;
    }

    /// <summary>
    /// Register multiple transitions for a model at once.
    /// Key = (fromSeq, toSeq), Value = transitionSeq.
    /// </summary>
    public static void RegisterMany(string modelPath, Dictionary<(string from, string to), string> transitions)
    {
        foreach (var ((from, to), trans) in transitions)
            Register(modelPath, from, to, trans);
    }

    /// <summary>
    /// Check if a transition exists between two sequences on this model.
    /// </summary>
    public static bool HasTransition(SkinnedModelRenderer renderer, string currentSeq, string targetSeq)
        => FindTransitionSequence(renderer, currentSeq, targetSeq) != null;

    /// <summary>
    /// Clear all cached transition graphs (e.g., after hot-reload).
    /// </summary>
    public static void ClearCache() => _cache.Clear();

    // ═══ Internal: build transition graph from sequence names ═══

    private static Dictionary<string, Dictionary<string, string>> GetOrBuild(
        SkinnedModelRenderer renderer, string path)
    {
        if (_cache.TryGetValue(path, out var cached)) return cached;

        var graph = BuildGraph(renderer);
        _cache[path] = graph;
        return graph;
    }

    private static Dictionary<string, Dictionary<string, string>> BuildGraph(SkinnedModelRenderer renderer)
    {
        var graph = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        var names = renderer.Sequence.SequenceNames;

        foreach (var name in names)
        {
            var (fromSeq, toSeq) = TryParseTransition(name);
            if (fromSeq == null || toSeq == null) continue;

            if (!graph.TryGetValue(fromSeq, out var edges))
            {
                edges = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                graph[fromSeq] = edges;
            }

            // Don't overwrite existing — first match wins
            if (!edges.ContainsKey(toSeq))
                edges[toSeq] = name;
        }

        return graph;
    }

    /// <summary>
    /// Try to parse a sequence name as a transition: "idle_to_walk" → ("idle", "walk").
    /// Handles _to_, _TO_, _To_, and short "2" delimiter (idle2walk).
    /// Returns (null, null) if the name doesn't look like a transition.
    /// </summary>
    private static (string from, string to) TryParseTransition(string sequenceName)
    {
        if (string.IsNullOrEmpty(sequenceName)) return (null, null);

        foreach (var delim in _delimiters)
        {
            var idx = sequenceName.IndexOf(delim, StringComparison.Ordinal);
            if (idx <= 0 || idx + delim.Length >= sequenceName.Length) continue;

            var from = sequenceName[..idx];
            var to = sequenceName[(idx + delim.Length)..];

            // Reject empty parts or parts that are just numbers (likely not transitions)
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to)) continue;
            // Skip if "to" looks like just a number suffix (e.g., "walk2")
            if (int.TryParse(to, out _)) continue;

            return (from, to);
        }

        return (null, null);
    }
}
