namespace ZombieHorde;

public enum ZoneState
{
	Dormant,
	Ambient,
	Warming,
	Combat,
	Exhausted,
	Cooldown
}

public class EncounterZone
{
	public string ZoneName { get; set; } = "Unnamed";
	public BBox Bounds { get; set; }
	public ZoneState State { get; set; } = ZoneState.Dormant;

	public List<SpawnSource> SpawnSources { get; set; } = new();
	public int AmbientBudget { get; set; } = 5;
	public int CombatBudget { get; set; } = 12;
	public int DangerLevel { get; set; } = 1;
	public float AlertLevel { get; set; } = 0f;
	public List<EncounterInstance> ActiveEncounters { get; set; } = new();
	public float CooldownRemaining { get; set; } = 0f;
	public int PlayerCount { get; set; } = 0;

	public bool HasEngagedEnemies => Game.ActiveScene.GetAllComponents<BaseZombie>()
		.Any( z => IsPointInBounds( z.WorldPosition )
			&& z.Brain != null
			&& z.Brain.CurrentAwareness >= Awareness.Engaged );

	public TimeSince TimeSinceNoPlayers { get; set; } = 0;
	public TimeSince TimeSinceNoEngagement { get; set; } = 0;

	public int CurrentEnemyCount => Game.ActiveScene.GetAllComponents<BaseZombie>()
		.Count( z => IsPointInBounds( z.WorldPosition ) );

	public bool CanAcceptEncounter()
	{
		return State is ZoneState.Ambient or ZoneState.Warming or ZoneState.Combat
			&& ActiveEncounters.Count < 3;
	}

	public void RegisterSpawnSource( SpawnSource source )
	{
		source.OwningZone = this;
		SpawnSources.Add( source );
	}

	public List<SpawnSource> GetAvailableSources( EncounterType forType )
	{
		return SpawnSources.Where( s => s.IsAvailable( forType, AlertLevel ) ).ToList();
	}


	public SpawnSource FindNearestSource( Vector3 pos )
	{
		return SpawnSources.OrderBy( s => (s.Position - pos).Length ).FirstOrDefault()
			?? new SpawnSource { Position = Bounds.Center };
	}
	public bool IsPointInBounds( Vector3 point )
	{
		return point.x >= Bounds.Mins.x && point.x <= Bounds.Maxs.x
			&& point.y >= Bounds.Mins.y && point.y <= Bounds.Maxs.y
			&& point.z >= Bounds.Mins.z && point.z <= Bounds.Maxs.z;
	}
}
