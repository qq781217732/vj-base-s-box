using System.Threading.Tasks;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Planted Tripmine" )]
public partial class Tripmine : Component, Component.IDamageable
{
	public static readonly Model WorldModel = Model.Load( "models/dm_tripmine.vmdl" );

	public GameObject Owner { get; set; }
	SkinnedModelRenderer renderer;
	ModelPhysics physics;
	bool exploding;

	protected override void OnAwake()
	{
		renderer = Components.Get<SkinnedModelRenderer>();
		if ( renderer is not null )
			renderer.Model = WorldModel;
	}

	public async Task Arm( float seconds )
	{
		await GameTask.DelaySeconds( 0.01f );
		Sound.Play( "dm.tripmine_arming", WorldPosition );
		await GameTask.DelaySeconds( seconds );
		if ( !GameObject.IsValid() ) return;

		physics = Components.Create<ModelPhysics>( true );
		physics.Model = WorldModel;
		physics.Renderer = renderer;

		var tr = Game.ActiveScene.Trace.Ray( WorldPosition, WorldPosition + WorldRotation.Forward * 4000.0f )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "trigger" )
			.Run();

		Sound.Play( "dm.tripmine_armed", WorldPosition );

		if ( tr.GameObject is not null )
			_ = Explode( 0.5f );
	}

	public void OnDamage( in Sandbox.DamageInfo info )
	{
		if ( info.Attacker is not null )
			Owner = info.Attacker;

		_ = Explode( 0.3f );
	}

	async Task Explode( float delay )
	{
		if ( exploding ) return;
		exploding = true;

		Sound.Play( "dm.tripmine_activated", WorldPosition );
		await GameTask.DelaySeconds( delay );

		ZombieNetworkManager.Explosion( GameObject, Owner, WorldPosition, 400, 150, 1.0f );
		GameObject.Destroy();
	}
}
