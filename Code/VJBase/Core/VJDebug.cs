using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ Debug utilities — ported from vj_base/debug.lua.
/// </summary>
[Flags]
public enum VJDebugFlags
{
	None = 0,
	Touch = 1 << 0,
	Damage = 1 << 1,
	Enemy = 1 << 2,
	Attack = 1 << 3,
	ResetEnemy = 1 << 4,
	TakingCover = 1 << 5,
	Weapon = 1 << 6,
	Engine = 1 << 7,
}

public static class VJDebug
{
	/// <summary>Check if VJ_DEBUG is enabled for a given flags category on an NPC.</summary>
	public static bool IsEnabled(BaseNPC npc, VJDebugFlags flag = VJDebugFlags.None)
	{
		if (npc == null || !npc.VJ_DEBUG) return false;
		if (flag == VJDebugFlags.None) return true;
		return (npc.DebugFlags & flag) != 0;
	}

	/// <summary>VJ.DEBUG_Print — colored debug output. If type is "error" → Log.Error, "warn" → Log.Warning, else Log.Info.</summary>
	public static void Print(GameObject ent, string name, string type, params object[] args)
	{
		var prefix = ent.IsValid() ? $"[{ent.Name}]" : "[VJ]";
		if (!string.IsNullOrEmpty(name))
			prefix += $" | {name}";

		var msg = $"{prefix} {string.Join(" ", args)}";

		if (type == "error")
			Log.Error(msg);
		else if (type == "warn")
			Log.Warning(msg);
		else
			Log.Info(msg);
	}

	/// <summary>VJ.DEBUG_TempEnt — spawns a temporary debug marker entity at position. Removed after time seconds.</summary>
	public static async void TempEnt(Vector3 pos, Angles ang = default, Color? color = null, float time = 3f, string model = "models/hunter/blocks/cube025x025x025.vmdl")
	{
		var prop = new GameObject();
		prop.Name = "VJDebug_TempEnt";
		prop.WorldPosition = pos;
		prop.WorldRotation = ang.ToRotation();
		var renderer = prop.AddComponent<ModelRenderer>();
		renderer.Model = Model.Load(model);
		renderer.Tint = color ?? Color.Red;

		await Task.Delay((int)(time * 1000));

		if (prop.IsValid())
			prop.Destroy();
	}

	/// <summary>VJ.DEBUG_Stress — runs func count times and prints timing info.</summary>
	public static void Stress(int count, Action func)
	{
		var sw = Stopwatch.StartNew();
		for (int i = 0; i < count; i++)
			func();
		sw.Stop();
		var total = sw.Elapsed.TotalSeconds;
		Log.Info($"Total: {total:F6} sec | Average: {total / count:F6} sec");
	}
}
