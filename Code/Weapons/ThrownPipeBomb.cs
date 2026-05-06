using System.Threading.Tasks;

namespace ZombieHorde;

/// <summary>
/// Thrown pipe bomb that lures zombies before exploding.
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Thrown Pipe Bomb" )]
public partial class ThrownPipeBomb : Component
{
	public static readonly Model WorldModel = Model.Load( "weapons/licensed/hqfpsweapons/fp_equipment/throwables/pipebomb/w_pipebomb.vmdl" );

	TimeSince timeSinceBeeped;

	protected override void OnAwake()
	{
		Tags.Add( "grenade" );
			Tags.Add( "gib" );

		var renderer = Components.Get<ModelRenderer>();
		if ( renderer is not null )
			renderer.Model = WorldModel;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		if ( timeSinceBeeped > 0.4f )
		{
			timeSinceBeeped = 0;
			Sound.Play( "pipebomb.beep", WorldPosition );

			foreach ( var obj in Game.ActiveScene.FindInPhysics( new Sphere( WorldPosition, 1400 ) ) )
			{
				var zom = obj.Components.Get<CommonZombie>();
				if ( zom is not null )
					zom.StartLure( WorldPosition );
			}
		}
	}

	public async Task BlowIn( float seconds )
	{
		await GameTask.DelaySeconds( seconds );
		if ( !GameObject.IsValid() ) return;

		Sound.Play( "grenade.explode", WorldPosition );
		ZombieNetworkManager.Explosion( GameObject, null, WorldPosition, 300, 60, 5f );
		GameObject.Destroy();
	}
}
