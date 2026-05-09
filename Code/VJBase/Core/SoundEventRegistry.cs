using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase;

/// <summary>Source engine sound type bitflags for NPC hearing/investigation filtering.</summary>
[Flags]
public enum VJSoundType
{
    None = 0,
    Combat = 1,
    World = 2,
    Player = 4,
    Danger = 8,
    BulletImpact = 16,
    PhysicsDanger = 32,
    PlayerVehicle = 64,
    MoveAway = 128,
    Carcass = 256,
    Meat = 512,
    Garbage = 1024,
}

/// <summary>Represents a world sound event for NPC hearing/investigation. core.lua CSound equivalent.</summary>
public class WorldSoundEvent
{
    public Vector3 Origin { get; set; }
    public Vector3 ReactOrigin { get; set; }
    public GameObject Owner { get; set; }
    public VJSoundType Type { get; set; }
    public float Volume { get; set; } = 1f;
    public float ExpiryTime { get; set; }
}

/// <summary>Global sound event registry — Source CSoundEnt equivalent. NPCs query via GetBestSoundHint.</summary>
public static class SoundEventRegistry
{
    private static readonly List<WorldSoundEvent> Events = new();
    private const float DefaultLifetime = 5f;

    public static void Register(Vector3 origin, VJSoundType type, GameObject owner = null, float volume = 1f, float lifetime = DefaultLifetime)
    {
        Events.Add(new WorldSoundEvent
        {
            Origin = origin,
            ReactOrigin = origin,
            Owner = owner,
            Type = type,
            Volume = volume,
            ExpiryTime = Time.Now + lifetime,
        });
    }

    /// <summary>Returns the closest unexpired sound matching typeMask, or null.</summary>
    public static WorldSoundEvent GetClosestSound(Vector3 listenerPos, VJSoundType typeMask)
    {
        Cleanup();
        WorldSoundEvent closest = null;
        float closestDist = float.MaxValue;
        foreach (var se in Events)
        {
            if ((se.Type & typeMask) == 0) continue;
            float dist = listenerPos.DistanceSquared(se.Origin);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = se;
            }
        }
        return closest;
    }

    private static void Cleanup()
    {
        float now = Time.Now;
        Events.RemoveAll(se => se.ExpiryTime < now);
    }
}
