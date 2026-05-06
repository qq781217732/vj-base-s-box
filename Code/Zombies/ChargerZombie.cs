using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Charger Zombie" )]
public partial class ChargerZombie : SpecialZombie
{
	protected override void OnStart()
	{
		base.OnStart();

		if ( BodyRenderer is not null )
			BodyRenderer.Model = Model.Load( "models/zombie/charger/charger_zombie.vmdl" );

		GameObject.WorldScale = 1.25f;
		Health = MaxHealth * 2;
		RunSpeed *= .4f;
		AttackDamage = 12;

		// Disable torso and feet body groups
		if ( BodyRenderer is not null )
		{
			BodyRenderer.Set( "bodygroup_torso", 1 );
			BodyRenderer.Set( "bodygroup_feet", 1 );
		}
	}

	// Provide damage modification logic.
	public float ModifyDamageTaken( float incomingDamage, in Sandbox.DamageInfo info )
	{
		var dmg = incomingDamage;

		if ( info.Tags.Has( "bullet" ) )
		{
			dmg *= 0.5f;
		}

		return dmg;
	}

	public new void DamagedEffects()
	{
		if ( Agent is not null )
			SpeedMultiplier = 0.5f;

		if ( Health > 0 )
			PlaySoundOnClient( "zombie.hurt" );
	}

	[Rpc.Broadcast]
	public new void PlaySoundOnClient( string sound )
	{
		var snd = Sound.Play( sound, WorldPosition + Vector3.Up * 60 );
		if ( snd is not null )
			snd.Pitch = .9f;
	}

	// TODO: Clothing API changed - UpdateClothes removed
}
