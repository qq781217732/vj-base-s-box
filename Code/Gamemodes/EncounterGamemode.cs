namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Encounter Gamemode" )]
public partial class EncounterGamemode : BaseGamemode
{
	public EncounterDirector Director { get; set; }

	protected override void OnStart()
	{
		Log.Info( "EncounterGamemode active" );
		RoundName = "Encounter";
		HumanMaxRevives = 3;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	public override bool EnableRespawning()
	{
		return true;
	}

	public override bool PopulateZombiesAngry()
	{
		return false;
	}
}
