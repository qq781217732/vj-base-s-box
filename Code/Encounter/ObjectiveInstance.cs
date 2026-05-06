using SWB.Shared;

namespace ZombieHorde;

/// <summary>
/// Runtime instance for an active objective. Tracks phase, spawns
/// reinforcement waves during defense, and resolves on completion.
/// </summary>
public class ObjectiveInstance
{
	public EncounterObjectivePoint Point { get; set; }
	public GameObject Activator { get; set; }
	public EncounterZone OwningZone { get; set; }
	public ObjectivePhase Phase { get; set; } = ObjectivePhase.Spawning;

	public TimeSince TimeSinceStarted { get; set; }
	public TimeSince TimeSinceLastReinforce { get; set; }

	public List<EncounterInstance> Waves { get; set; } = new();
	public List<BaseZombie> SpawnedEnemies { get; set; } = new();

	public float Duration => Point?.Duration ?? 30f;
	public float ReinforceInterval => Point?.ReinforceInterval ?? 6f;
	public int ReinforceCount => Point?.ReinforceCount ?? 3;

	public void Tick( EncounterDirector director )
	{
		if ( Phase != ObjectivePhase.Active ) return;

		// Check for player presence — if nobody nearby, pause
		var nearby = Game.ActiveScene.GetAllComponents<HumanPlayer>()
			.Any( p => p.IsAlive && p.WorldPosition.Distance( Point.WorldPosition ) < 1000 );

		if ( !nearby )
		{
			Phase = ObjectivePhase.Abandoned;
			return;
		}

		// Reinforcement waves
		if ( Waves.Count < ReinforceCount && TimeSinceLastReinforce > ReinforceInterval )
		{
			TimeSinceLastReinforce = 0;
			SpawnReinforcement( director );
		}
	}

	void SpawnReinforcement( EncounterDirector director )
	{
		if ( OwningZone is null ) return;

		var spawnPos = Point.WorldPosition;
		if ( OwningZone.SpawnSources.Count > 0 )
			spawnPos = OwningZone.SpawnSources[Game.Random.NextInt( OwningZone.SpawnSources.Count )].Position;
		else
			spawnPos = OwningZone.Bounds.Center + new Vector3( Game.Random.NextFloat( -300, 300 ), Game.Random.NextFloat( -300, 300 ), 0 );

		var groupName = OwningZone.DangerLevel >= 3 ? "ChargerWave" : "LightResponse";
		var group = director?.GetSpawnGroup( groupName );
		if ( group is null ) return;

		var spawned = group.Spawn( spawnPos, Activator );
		var encounter = new EncounterInstance
		{
			Type = EncounterType.ObjectiveDefenseWave,
			Phase = EncounterPhase.Active,
			OwningZone = OwningZone,
			AssignedGroup = group,
			SpawnedEnemies = spawned,
			SpawnPosition = spawnPos
		};

		foreach ( var z in spawned )
			z.OwningEncounter = encounter;

		Waves.Add( encounter );
		SpawnedEnemies.AddRange( spawned );
		OwningZone.ActiveEncounters.Add( encounter );

		Log.Info( $"Objective reinforce wave {Waves.Count}/{ReinforceCount}: +{spawned.Count} enemies" );
	}

	public void Resolve()
	{
		Phase = ObjectivePhase.Completed;
		foreach ( var enc in Waves )
			enc.Resolve();
		Log.Info( $"Objective completed with {Waves.Count} waves, {SpawnedEnemies.Count} total enemies" );
	}
}

public enum ObjectivePhase
{
	Spawning,
	Active,
	Abandoned,
	Completed
}
