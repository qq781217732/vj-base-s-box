namespace ZombieHorde;

public class EncounterInstance
{
	public EncounterType Type { get; set; }
	public EncounterPhase Phase { get; set; } = EncounterPhase.Pending;
	public EncounterZone OwningZone { get; set; }
	public SpawnGroup AssignedGroup { get; set; }
	public List<BaseZombie> SpawnedEnemies { get; set; } = new();
	public TimeSince TimeSinceCreated { get; set; } = 0;
	public float MaxLifetime { get; set; } = 120f;
	public Vector3 SpawnPosition { get; set; }

	public int TotalWaveCount { get; set; } = 1;
	public int CurrentWave { get; set; } = 1;
	public float WaveInterval { get; set; } = 10f;
	public TimeSince TimeSinceWaveStart { get; set; } = 0;
	public SpawnGroup NextWaveGroup { get; set; }
	public GameObject ForceTarget { get; set; }

	public bool HasNextWave => CurrentWave < TotalWaveCount;

	public bool IsExpired()
	{
		return TimeSinceCreated > MaxLifetime;
	}

	public void OnEnemyKilled( BaseZombie enemy )
	{
		SpawnedEnemies.Remove( enemy );
	}

	public bool ShouldResolve()
	{
		if ( IsExpired() )
			return true;

		if ( Phase == EncounterPhase.Searching && TimeSinceCreated > 30f )
			return true;

		if ( Phase == EncounterPhase.Active && SpawnedEnemies.Count == 0 && !HasNextWave )
			return true;

		return false;
	}

	public List<BaseZombie> SpawnNextWave( Vector3 origin )
	{
		if ( NextWaveGroup == null )
			return new List<BaseZombie>();

		var spawned = NextWaveGroup.Spawn( origin, ForceTarget );
		CurrentWave++;
		TimeSinceWaveStart = 0;
		SpawnedEnemies.AddRange( spawned );
		return spawned;
	}

	public int AliveCount => SpawnedEnemies.Count( e => e.GameObject.IsValid() );

		public void Resolve() { Phase = EncounterPhase.Resolved; }
}
