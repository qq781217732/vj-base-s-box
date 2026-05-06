namespace ZombieHorde;

/// <summary>
/// Placeable objective point in the map. When activated by a player,
/// creates a defense encounter with timed reinforcements.
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Encounter Objective Point" )]
public partial class EncounterObjectivePoint : Component, Component.ITriggerListener
{
	[Property] public string ObjectiveName { get; set; } = "Objective";
	[Property] public float Duration { get; set; } = 30f;
	[Property] public float ReinforceInterval { get; set; } = 6f;
	[Property] public int ReinforceCount { get; set; } = 3;

	[Sync] public bool IsActive { get; set; }
	[Sync] public bool IsCompleted { get; set; }
	[Sync] public TimeUntil TimeUntilComplete { get; set; }

	public ObjectiveInstance CurrentObjective { get; set; }

	protected override void OnAwake()
	{
		GameObject.Tags.Add( "objective" );
	}

	public void OnTriggerEnter( Collider other )
	{
		if ( IsActive || IsCompleted || IsProxy ) return;
		var player = other.GameObject.Components.Get<HumanPlayer>();
		if ( player is null ) return;

		Activate( player.GameObject );
	}

	[Rpc.Broadcast]
	public void Activate( GameObject activator )
	{
		if ( IsActive || IsCompleted ) return;
		IsActive = true;
		TimeUntilComplete = Duration;

		var director = Game.ActiveScene.GetAllComponents<EncounterDirector>().FirstOrDefault();
		if ( director is not null )
		{
			CurrentObjective = director.CreateObjective( this, activator );
			Log.Info( $"Objective '{ObjectiveName}' activated!" );
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( !IsActive || IsCompleted || IsProxy ) return;

		if ( TimeUntilComplete <= 0 )
		{
			Complete();
		}
	}

	[Rpc.Broadcast]
	void Complete()
	{
		if ( IsCompleted ) return;
		IsCompleted = true;
		IsActive = false;

		CurrentObjective?.Resolve();
		Log.Info( $"Objective '{ObjectiveName}' completed!" );
	}
}
