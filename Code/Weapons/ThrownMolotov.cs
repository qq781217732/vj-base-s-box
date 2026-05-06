using SWB.Shared;
using System.Threading.Tasks;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Thrown Molotov" )]
public partial class ThrownMolotov : Component
{
	[Sync] public GameObject Owner { get; set; }
	public Vector3 Velocity { get; set; }

	public static readonly Model WorldModel = Model.Load( "weapons/licensed/hqfpsweapons/fp_equipment/throwables/molotovcocktail/w_molotov.vmdl" );
	public static readonly Model BurntModel = Model.Load( "weapons/licensed/hqfpsweapons/fp_equipment/throwables/molotovcocktail/w_molotov_burnt.vmdl" );

	public async Task BlowIn( float seconds )
	{
		await GameTask.DelaySeconds( seconds );
		if ( !GameObject.IsValid() ) return;
		Explode();
	}

	void Explode()
	{
		Sound.Play( "grenade.explode", WorldPosition );

		var fire = new GameObject( true, "MolotovFire" );
		fire.WorldPosition = WorldPosition;
		fire.Components.Create<Flames>();
		fire.NetworkSpawn();

		GameObject.Destroy();
	}
}
