using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Uncommon Zombie" )]
public partial class UncommonZombie : CommonZombie
{
	protected override void OnStart()
	{
		base.OnStart();
		Health = MaxHealth * 2;
		RunSpeed *= .4f;
		AttackDamage = 12;
	}

	// Provide damage modification logic that should be applied when damage is received.
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
