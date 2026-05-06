using System.Threading.Tasks;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Hand Grenade" )]
public partial class HandGrenade : Component
{
	[Sync] public GameObject Owner { get; set; }

	protected override void OnAwake()
	{
		Tags.Add( "Grenade" );
	}

	public async Task BlowIn( float seconds )
	{
		await GameTask.DelaySeconds( seconds );
		if ( !GameObject.IsValid() ) return;
		ZombieNetworkManager.Explosion( GameObject, Owner, WorldPosition, 400, 100, 1.0f );
		GameObject.Destroy();
	}
}
