using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Molotov Flames" )]
public partial class Flames : Component
{
	public TimeUntil TimeUntilExpire { get; set; } = 16;
	public float BurnRadius { get; set; } = 150;

	TimeSince timeSinceTickedPlayers;

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		if ( TimeUntilExpire < 0 )
		{
			GameObject.Destroy();
			return;
		}

		// Burn zombies
		foreach ( var obj in Game.ActiveScene.FindInPhysics( new Sphere( WorldPosition, BurnRadius ) ) )
		{
			var zom = obj.Components.Get<CommonZombie>();
			if ( zom is not null )
				zom.Ignite();
		}

		// Burn players
		if ( timeSinceTickedPlayers > 0.5f )
		{
			timeSinceTickedPlayers = 0;
			foreach ( var obj in Game.ActiveScene.FindInPhysics( new Sphere( WorldPosition, 100 ) ) )
			{
				var ply = obj.Components.Get<HumanPlayer>();
				if ( ply is not null )
				{
					var dmgInfo = new SWB.Shared.DamageInfo { Damage = 1 };
					ply.TakeDamage( dmgInfo );
					Sound.Play( "sounds/impacts/impact-bullet-flesh.sound", ply.WorldPosition );
				}
			}
		}
	}
}
