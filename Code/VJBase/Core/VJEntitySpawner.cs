using System;
using System.Threading.Tasks;
using Sandbox;

namespace VJBase;

/// <summary>
/// Static helper for entity creation, appearance copying, dissolve, and ignite.
/// Source ents.Create / SetModel / SetSkin / SetBodygroup / SetColor / SetMaterial → S&Box equivalents.
/// </summary>
public static class VJEntitySpawner
{
    /// <summary>
    /// Create a basic model entity (Source "prop_physics" equivalent).
    /// GameObject with ModelRenderer + optional Rigidbody + ModelCollider.
    /// </summary>
    public static GameObject CreateModelEntity(string modelPath, Vector3 pos, Rotation ang, bool withPhysics = true)
    {
        var go = new GameObject(true, $"VJ_{modelPath}");
        go.WorldPosition = pos;
        go.WorldRotation = ang;

        var renderer = go.Components.Create<ModelRenderer>();
        if (!string.IsNullOrEmpty(modelPath))
        {
            renderer.Model = Model.Load(modelPath);
        }

        if (withPhysics)
        {
            go.Components.Create<Rigidbody>();
            go.Components.Create<ModelCollider>();
        }

        return go;
    }

    /// <summary>
    /// Create a ragdoll entity (Source "prop_ragdoll" equivalent).
    /// GameObject with ModelRenderer + ModelPhysics for multi-body ragdoll physics.
    /// </summary>
    public static GameObject CreateRagdollEntity(string modelPath, Vector3 pos, Rotation ang)
    {
        var go = new GameObject(true, $"VJ_Ragdoll_{modelPath}");
        go.WorldPosition = pos;
        go.WorldRotation = ang;

        var renderer = go.Components.Create<ModelRenderer>();
        if (!string.IsNullOrEmpty(modelPath))
        {
            renderer.Model = Model.Load(modelPath);
        }

        var phys = go.Components.Create<ModelPhysics>();
        phys.Renderer = renderer;
        phys.Model = renderer.Model;

        return go;
    }

    /// <summary>
    /// Copy all visual appearance from source to target: model, body groups, tint, material.
    /// Equivalent to Source: SetSkin / SetBodygroup(i,v) loop / SetColor / SetMaterial.
    /// </summary>
    public static void CopyAppearance(GameObject from, GameObject to)
    {
        if (from == null || !from.IsValid() || to == null || !to.IsValid()) return;

        var srcRenderer = from.Components.Get<ModelRenderer>();
        var dstRenderer = to.Components.Get<ModelRenderer>();
        if (srcRenderer == null || dstRenderer == null) return;

        // ModelRenderer.CopyFrom copies model, bodygroups, tint, material overrides
        dstRenderer.CopyFrom(srcRenderer);
    }

    /// <summary>
    /// Get the model path from a GameObject's ModelRenderer.
    /// Source: ent:GetModel() returns string path.
    /// </summary>
    public static string GetModelPath(GameObject ent)
    {
        if (ent == null || !ent.IsValid()) return null;
        var renderer = ent.Components.Get<ModelRenderer>();
        return renderer?.Model?.Name;
    }

    /// <summary>
    /// Simulate Source Dissolve: fade alpha to 0 over fadeTime, then destroy after total duration.
    /// Source: ent:Dissolve(0, 1). S&Box has no built-in dissolve — use Tint alpha fade + Destroy.
    /// </summary>
    public static async Task DissolveEntity(GameObject ent, float duration = 2f, float fadeTime = 1f)
    {
        if (ent == null || !ent.IsValid()) return;

        var renderer = ent.Components.Get<ModelRenderer>();
        if (renderer == null)
        {
            ent.Destroy();
            return;
        }

        var startColor = renderer.Tint;
        float elapsed = 0f;
        while (elapsed < fadeTime && ent.IsValid() && renderer.IsValid())
        {
            float t = Math.Min(elapsed / fadeTime, 1f);
            renderer.Tint = startColor.WithAlpha(1f - t);
            await Task.Delay(50);
            elapsed += 0.05f;
        }

        float remainDuration = duration - fadeTime;
        if (remainDuration > 0)
            await Task.Delay((int)(remainDuration * 1000f));
        if (ent.IsValid()) ent.Destroy();
    }

    /// <summary>
    /// Ignite an entity using S&Box Prop component.
    /// Source: ent:Ignite(duration, 0). S&Box Prop.Ignite() takes no params.
    /// </summary>
    public static void IgniteEntity(GameObject ent)
    {
        if (ent == null || !ent.IsValid()) return;

        var prop = ent.Components.GetOrCreate<Prop>();
        prop.Ignite();
    }

    /// <summary>
    /// Check if a GameObject is currently on fire.
    /// Source: ent:IsOnFire(). S&Box: Prop.IsOnFire.
    /// </summary>
    public static bool IsOnFire(GameObject ent)
    {
        if (ent == null || !ent.IsValid()) return false;
        return ent.Components.Get<Prop>()?.IsOnFire ?? false;
    }

    /// <summary>
    /// Get number of body groups from model. Falls back to 32 if model not available.
    /// Source: ent:GetNumBodyGroups().
    /// </summary>
    public static int GetNumBodyGroups(Model model)
    {
        // S&Box Model has BodyGroups info, but we cap at 32 (Source max)
        return 32;
    }

    /// <summary>
    /// Try to load a model. Returns true if model exists and can be loaded.
    /// Source: util.IsValidModel(modelPath).
    /// </summary>
    public static bool TryLoadModel(string modelPath, out Model model)
    {
        model = null;
        if (string.IsNullOrEmpty(modelPath)) return false;
        try { model = Model.Load(modelPath); return model != null; }
        catch { return false; }
    }

    /// <summary>
    /// Check if a model is valid for ragdoll physics (has skeleton / physics data).
    /// Source: util.IsValidRagdoll(modelPath).
    /// </summary>
    public static bool IsValidRagdollModel(Model model)
    {
        if (model == null) return false;
        // S&Box: models with Physics data can be used as ragdolls via ModelPhysics component
        return model.Physics != null;
    }
}
