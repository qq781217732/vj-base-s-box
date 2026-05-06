namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Health Kit" )]
public partial class HealthKit : Component
{
	protected override void OnAwake()
	{
		Tags.Add( "item" );
		var renderer = Components.Get<ModelRenderer>();
		if ( renderer is not null )
			renderer.Model = Model.Load( "models/gameplay/healthkit/healthkit.vmdl" );
	}

	// Called when another object touches this one
	public void OnTriggerEnter( Collider other )
	{
		var ply = other.GameObject?.Components.Get<HumanPlayer>();
		if ( ply is null ) return;
		if ( ply.Health >= ply.MaxHealth ) return;

		var newHealth = ply.Health + 35;
		newHealth = newHealth.Clamp( 0, ply.MaxHealth );
		ply.Health = newHealth;

		Sound.Play( "dm.item_health", WorldPosition );

		if ( !IsProxy )
			GameObject.Destroy();
	}
}
