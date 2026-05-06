namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Crossbow Bolt" )]
public partial class CrossbowBolt : Component
{
	[Sync] public GameObject Owner { get; set; }
	public float Damage { get; set; } = 75;

	Vector3 velocity;
	public Vector3 Velocity { get => velocity; set => velocity = value; }

	TimeSince timeSinceSpawned;

	protected override void OnFixedUpdate()
	{
		if ( timeSinceSpawned < 10f )
		{
			WorldPosition += velocity * Time.Delta;
			velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
			WorldRotation = Rotation.LookAt( velocity.Normal );

			var tr = Game.ActiveScene.Trace.Ray( WorldPosition - velocity * Time.Delta, WorldPosition )
				.IgnoreGameObjectHierarchy( GameObject )
				.Radius( 4 )
				.Run();

			if ( tr.Hit )
			{
				if ( !IsProxy && tr.GameObject.IsValid() )
				{
					var dmgInfo = new Sandbox.DamageInfo
					{
						Attacker = Owner,
						Damage = Damage,
						Origin = WorldPosition,
						Position = tr.HitPosition,
					};
					foreach ( var dmg in tr.GameObject.Components.GetAll<Component.IDamageable>() )
						dmg.OnDamage( dmgInfo );
				}
				GameObject.Destroy();
			}
		}
		else
		{
			GameObject.Destroy();
		}
	}
}
