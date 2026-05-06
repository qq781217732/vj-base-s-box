namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Encounter Zone" )]
public partial class EncounterZoneVolume : Component
{
	[Property] public string ZoneName { get; set; } = "Unnamed Zone";
	[Property] public int DangerLevel { get; set; } = 1;
	[Property] public int AmbientBudget { get; set; } = 5;
	[Property] public int CombatBudget { get; set; } = 12;
	[Property] public ZoneInitialState InitialState { get; set; } = ZoneInitialState.Ambient;

	protected override void OnAwake()
	{
		GameObject.Tags.Add( "trigger" );
		GameObject.Tags.Add( "encounter_zone" );
	}

	public EncounterZone ToEncounterZone()
	{
		var bounds = new BBox( WorldPosition - Vector3.One * 500, WorldPosition + Vector3.One * 500 );

		return new EncounterZone
		{
			ZoneName = ZoneName,
			Bounds = bounds,
			State = InitialState == ZoneInitialState.Ambient ? ZoneState.Ambient : ZoneState.Dormant,
			AmbientBudget = AmbientBudget,
			CombatBudget = CombatBudget,
			DangerLevel = DangerLevel
		};
	}
}

public enum ZoneInitialState
{
	Dormant,
	Ambient
}
