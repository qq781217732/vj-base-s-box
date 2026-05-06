using System.Threading.Tasks;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "SMG Grenade" )]
public partial class SMGGrenade : Component
{
	[Sync] public GameObject Owner { get; set; }

	public async Task BlowIn( float seconds )
	{
		await GameTask.DelaySeconds( seconds );
		if ( !GameObject.IsValid() ) return;
		Sound.Play( "frag.explode", WorldPosition );
		ZombieNetworkManager.Explosion( GameObject, Owner, WorldPosition, 400, 80, 1.0f );
		GameObject.Destroy();
	}
}
