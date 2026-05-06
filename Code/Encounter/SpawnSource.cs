namespace ZombieHorde;

public enum SpawnSourceType
{
	HiddenInterior,
	EdgeEntry,
	TunnelExit,
	ObjectiveSource,
	ReinforcementLane
}

public class SpawnSource
{
	public Vector3 Position { get; set; }
	public SpawnSourceType SourceType { get; set; }
	public EncounterZone OwningZone { get; set; }
	public List<SpawnRule> Rules { get; set; } = new();

	public bool IsAvailable( EncounterType forType, float zoneAlertLevel )
	{
		return Rules.All( r => r.Evaluate( Position, forType, OwningZone, zoneAlertLevel ) );
	}
}

public class SpawnRule
{
	public static float DefaultMinPlayerDistance = 0f;
	public static float DefaultMaxPlayerDistance = 50000f;
	public static bool DefaultRequireOutOfSight = false;

		public SpawnRule()
		{
			MinPlayerDistance = DefaultMinPlayerDistance;
			MaxPlayerDistance = DefaultMaxPlayerDistance;
			RequireOutOfSight = DefaultRequireOutOfSight;
		}

	public float MinPlayerDistance { get; set; } = 0f;
	public float MaxPlayerDistance { get; set; } = 50000f;
	public bool RequireOutOfSight { get; set; } = false;
	public List<EncounterType> AllowedEncounterTypes { get; set; } = new();
	public float MinAlertLevel { get; set; } = 0f;
	public bool AllowSpecialEnemies { get; set; } = true;

	public bool Evaluate( Vector3 spawnPos, EncounterType forType, EncounterZone zone, float zoneAlertLevel )
	{
		if ( AllowedEncounterTypes.Count > 0 && !AllowedEncounterTypes.Contains( forType ) )
			return false;

		if ( zoneAlertLevel < MinAlertLevel )
			return false;

		var players = Game.ActiveScene.GetAllComponents<HumanPlayer>().Where( p => p.IsAlive ).ToList();
		if ( players.Count == 0 ) return true;

		bool anyPlayerInRange = false;
		foreach ( var player in players )
		{
			var dist = (spawnPos - player.WorldPosition).Length;
			if ( dist < MinPlayerDistance ) return false;
			if ( dist <= MaxPlayerDistance ) anyPlayerInRange = true;

			if ( RequireOutOfSight )
			{
				var eyePos = player.WorldPosition + Vector3.Up * 64;
				var spawnEye = spawnPos + new Vector3( 0, 0, 70 );
				var tr = Game.ActiveScene.Trace.Ray( spawnEye, eyePos )
					.WithoutTags( "trigger", "gib" ).Run();
				if ( tr.Fraction >= 0.95f ) return false;
			}
		}
		return anyPlayerInRange;
	}
}
