using SWB.Shared;
using System.Threading.Tasks;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Thrown Impact Grenade" )]
public partial class ThrownImpactGrenade : Component
{
	[Sync] public GameObject Owner { get; set; }
	public Vector3 Velocity { get; set; }

	public async Task BlowIn( float seconds )
	{
		await GameTask.DelaySeconds( seconds );
		if ( !GameObject.IsValid() ) return;
		Sound.Play( "frag.explode", WorldPosition );
		ZombieNetworkManager.Explosion( GameObject, Owner, WorldPosition, 250, 50, 10f );
		GameObject.Destroy();
	}
}
