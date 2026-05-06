using System.Collections.Generic;

namespace ZombieHorde;

/// <summary>
/// Central config asset for Encounter mode. Place on EncounterDirector [Property].
/// All values have fallback defaults — assign a .zhconfig asset in editor to override.
/// </summary>
[GameResource( "Encounter Config", "zhcfg", "Encounter mode configuration" )]
public class EncounterConfig : GameResource
{
	// ========== Director ==========

	public int GlobalMaxEnemies { get; set; } = 30;
	public int MaxActiveEncounters { get; set; } = 6;
	public float AmbientRefreshInterval { get; set; } = 5f;
	public float PressureDecayRate { get; set; } = 1f;
	public float PressureAccumulationRate { get; set; } = 0.5f;
	public float HighPressureThreshold { get; set; } = 70f;
	public float CrossZonePursuitThreshold { get; set; } = 60f;

	// ========== Spawn Defaults ==========

	public float SpawnMinSeparation { get; set; } = 80f;
	public float SpawnInnerRadius { get; set; } = 200f;
	public float SpawnOuterRadius { get; set; } = 500f;

	// ========== SpawnRule Defaults ==========

	public float DefaultMinPlayerDistance { get; set; } = 0f;
	public float DefaultMaxPlayerDistance { get; set; } = 50000f;
	public bool DefaultRequireOutOfSight { get; set; } = false;

	// ========== Enemy Archetype ==========

	public float EnemyBaseHealth { get; set; } = 50;
	public float EnemyWalkSpeed { get; set; } = 45;
	public float EnemyRunSpeed { get; set; } = 140;
	public float EnemyAttackDamage { get; set; } = 6;
	public float EnemyAttackSpeed { get; set; } = 1.0f;

	// ========== Perception ==========

	public float SightRange { get; set; } = 1500f;
	public float SightHalfAngle { get; set; } = 60f;
	public float HearingRange { get; set; } = 800f;
	public float ProximityRange { get; set; } = 150f;
	public float SprintProximityBonus { get; set; } = 80f;
	public float SightMemory { get; set; } = 4f;
	public float SearchDuration { get; set; } = 5f;

	// ========== SpawnGroup Templates (serializable) ==========

	public List<SpawnGroupTemplate> SpawnGroupTemplates { get; set; } = new()
	{
		new() { Name = "ScoutPatrol", TotalCost = 3, Entries = new() { new() { EnemyTypeName = "ZombieHorde.CommonZombie", Count = 3 } }, DefaultTask = "Patrol" },
		new() { Name = "WanderPack", TotalCost = 4, Entries = new() { new() { EnemyTypeName = "ZombieHorde.CommonZombie", Count = 4 } }, DefaultTask = "Wander" },
		new() { Name = "LightResponse", TotalCost = 5, Entries = new() { new() { EnemyTypeName = "ZombieHorde.CommonZombie", Count = 4 }, new() { EnemyTypeName = "ZombieHorde.UncommonZombie", Count = 1 } }, DefaultTask = "MoveToSound" },
		new() { Name = "ChargerWave", TotalCost = 7, Entries = new() { new() { EnemyTypeName = "ZombieHorde.CommonZombie", Count = 4 }, new() { EnemyTypeName = "ZombieHorde.ChargerZombie", Count = 1 } }, DefaultTask = "ChaseTarget" },
	};
}

// ========== Sub-types for serialization ==========

public class SpawnGroupTemplate
{
	public string Name { get; set; } = "Unnamed";
	public int TotalCost { get; set; } = 1;
	public List<SpawnGroupEntryTemplate> Entries { get; set; } = new();
	public string DefaultTask { get; set; } = "Wander";
}

public class SpawnGroupEntryTemplate
{
	public PrefabScene EnemyPrefab { get; set; }
	public string EnemyTypeName { get; set; } = "ZombieHorde.CommonZombie";
	public int Count { get; set; } = 1;
}
