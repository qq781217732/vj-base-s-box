using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ Spawner base — ported from obj_vj_spawner_base/init.lua.
/// Spawns NPCs and entities on a timer or triggered.
/// </summary>
public partial class VJSpawner : Component
{
    public bool SingleSpawner { get; set; }
    public bool PauseSpawning { get; set; }
    public float RespawnCooldown { get; set; } = 3;
    public List<SpawnEntry> EntitiesToSpawn { get; set; } = new();

    public List<string> SoundTbl_Idle { get; set; }
    public float IdleSoundChance { get; set; } = 1;
    public (float a, float b) IdleSoundPitch { get; set; } = (80, 100);
    public (float a, float b) IdleSoundCooldown { get; set; } = (4, 10);

    protected virtual void SpawnEntities()
    {
        if (PauseSpawning) return;
        // Phase 3: pick from EntitiesToSpawn, NetworkSpawn
    }
}

public class SpawnEntry
{
    public List<string> Entities { get; set; } = new();
    public Vector3 SpawnPosition { get; set; }
    public Angles SpawnAngle { get; set; }
    public List<string> WeaponsList { get; set; }
    public string NPC_Class { get; set; }
    public bool FriToPlyAllies { get; set; }
}
