using SWB.Shared;
using ZombieHorde.Nav;
using System;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Common Zombie" )]
public partial class CommonZombie : BaseZombie
{
	[ConCmd]
	public static void ForceHorde()
	{
		foreach ( var npc in Game.ActiveScene.GetAllComponents<CommonZombie>() )
			npc.StartChase();
	}

	[ConCmd]
	public static void ForceWander()
	{
		foreach ( var npc in Game.ActiveScene.GetAllComponents<CommonZombie>() )
			npc.StartWander();
	}

	public TimeSince TimeSinceMoan { get; set; }
	public TimeSince TimeSinceLongIdle { get; set; } = -10;
	public bool JustSpawned { get; set; } = true;

	protected override void OnStart()
	{
		var gm = BaseGamemode.Current;
		if ( gm is not null )
		{
			Health *= gm.ZomHealthMultiplier;
			RunSpeed *= gm.ZomSpeedMultiplier;
		}
		TimeSinceMoan = -Game.Random.NextFloat( 1f );
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		if ( JustSpawned && Brain?.CurrentTask == ZombieTask.Wander )
		{
			StartWander();
			JustSpawned = false;
		}

		// Brain handles movement via ExecuteTask() — just handle combat proximity here
		CheckCombatRange();

		// Burning damage
		if ( Brain?.Conditions.HasFlag( ZombieCondition.Burning ) == true )
			TickBurning();

		// Periodic cleanup check
		if ( Game.Random.NextInt( 500 ) == 1 )
			CheckForDeletion();

		// Sounds
		TickMoanSounds();
	}

	void CheckCombatRange()
	{
		if ( IsStunned || Brain is null ) return;
		if ( Brain.CurrentTask != ZombieTask.Attack ) return;

		TryMeleeAttack();

		// Fire damage directly — bypasses animation event dependency
		var isActive = Brain.TimeSinceAttackStart < 0.15f;
		if ( isActive )
			MeleeAttack();

		// Debug: draw attack area
		var color = isActive ? Color.Red : Color.Yellow;
		var start = EyePosition;
		var end = start + WorldRotation.Forward * 70f;
		var size = new Vector3( 100f, 100f, 70f );
		DebugOverlay.Box( start, size, color.WithAlpha( 0.15f ), 0.1f );
	}

	void TickMoanSounds()
	{
		var interval = Brain?.CurrentTask == ZombieTask.ChaseTarget ? 1.4f : 2.4f;
		if ( TimeSinceMoan > interval )
		{
			TimeSinceMoan = 0 - Game.Random.NextFloat( 0.5f );
			// PlaySoundOnClient( "zombie.moan" );
		}

		// Long idle animation
		if ( Steer?.Path.IsEmpty == true && TimeSinceLongIdle > 5f && Game.Random.NextInt( 30 ) == 0 )
		{
			Steer.TimeUntilCanMove = 5;
			BodyRenderer?.Set( "b_longidle", true );
			TimeSinceLongIdle = 0;
		}
	}

	void TickBurning()
	{
		if ( TimeSinceBurnTicked > 0.5f )
		{
			TimeSinceBurnTicked = 0;
			BodyRenderer?.Set( "b_jump", true );
			SpeedMultiplier *= 0.9f;
			if ( BodyRenderer is not null )
				BodyRenderer.Tint = Color.Lerp( BodyRenderer.Tint, Color.Black, 0.15f );
			// PlaySoundOnClient( "zombie.hurt" );
			Health *= 0.75f;
			if ( Health <= 5 )
				OnDeath( new Sandbox.DamageInfo() );
		}
	}

	// ========== Combat ==========

	public void TryMeleeAttack()
	{
		if ( TimeSinceClimb < 1 || IsStunned ) return;
		BodyRenderer?.Set( "b_attack", true );
		SpeedMultiplier = 0.1f;
	}

	public void MeleeAttack()
	{
		if ( IsProxy || !IsValid || IsStunned ) return;

		SpeedMultiplier = 0;
		TimeSinceAttacked = 0 - Game.Random.NextFloat( 1 );

		var forward = WorldRotation.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 0.1f;
		forward = forward.Normal;

		var tr = Game.ActiveScene.Trace.Ray( EyePosition, EyePosition + forward * 70 )
			.WithoutTags( "zombie" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Radius( 50 )
			.Run();

		if ( tr.Hit && tr.GameObject.IsValid() )
		{
			var damageInfo = new SWB.Shared.DamageInfo
			{
				Attacker = GameObject,
				Damage = AttackDamage,
				Origin = EyePosition,
				Force = forward * 320,
				Position = tr.EndPosition,
				Tags = { TagsHelper.Bullet }
			};


			var dealt = false;
			foreach ( var damageable in tr.GameObject.Components.GetAll<IDamageable>( FindMode.EverythingInSelfAndAncestors ) )
			{
				damageable.OnDamage( damageInfo );
				dealt = true;
			}

			if ( !dealt )
			{
				var player = tr.GameObject.Components.GetInAncestors<HumanPlayer>();
				if ( player is not null )
				{
					player.TakeDamage( damageInfo );
				}
				else
				{
				}
			}
		}

		}

	// ========== Brain Commands ==========

	public void StartChase()
	{
		var players = Game.ActiveScene.GetAllComponents<HumanPlayer>()
			.Where( p => p.IsAlive ).ToList();
		if ( players.Count == 0 ) return;

		var target = players[Game.Random.NextInt( players.Count )];
		StartChase( target.GameObject );
	}

	public void StartChase( GameObject targ )
	{
		if ( targ is null || !targ.IsValid() ) return;
		if ( Brain is not null )
			Brain.ForceEngage( targ );
		else
		{
			Target = targ;
			Speed = RunSpeed;
			Steer = new NavSteer { Target = targ.WorldPosition };
		}
		BodyRenderer?.Set( "b_jump", true );
		// PlaySoundOnClient( "zombie.attack" );
	}

	public void StartWander()
	{
		if ( Brain is not null )
		{
			Brain.CurrentAwareness = Awareness.Idle;
			Brain.CurrentTask = ZombieTask.Wander;
			Brain.TargetEntity = null;
			Brain.InvestigatePosition = null;
			Brain.Conditions &= ~ZombieCondition.Lured;
		}
		Speed = WalkSpeed;
		var wander = new Wander { MinRadius = 150, MaxRadius = 300 };
		Steer = wander;
	}

	public void StartLure( Vector3 position )
	{
		if ( Brain?.Conditions.HasFlag( ZombieCondition.Burning ) == true ) return;
		if ( Brain is not null )
		{
			Brain.Conditions |= ZombieCondition.Lured;
			Brain.InvestigatePosition = position;
		}
		Speed = RunSpeed * 1.2f;
		BodyRenderer?.Set( "b_jump", true );
		Steer = new NavSteer { Target = position };
	}

	public override void Stun( float duration )
	{
		base.Stun( duration );
		if ( Brain is not null )
			Brain.Conditions |= ZombieCondition.Stunned;
	}

	public override void Ignite()
	{
		if ( Brain?.Conditions.HasFlag( ZombieCondition.Burning ) == true ) return;
		if ( Brain is not null )
			Brain.Conditions |= ZombieCondition.Burning;
		Steer = new NavSteer { Target = WorldPosition + WorldRotation.Forward * (Agent?.Velocity.Length ?? 0) * 3f };
		Speed = 0;
		TimeSinceBurnTicked = Game.Random.NextFloat( 0.5f );
		IsOnFire = true;
	}

	public override void HitBreakableObject()
	{
		if ( TimeSinceAttacked > AttackSpeed )
		{
			TimeSinceAttacked = -3;
			TryMeleeAttack();
		}
	}

	// ========== Maintenance ==========

	public void CheckForDeletion()
	{
		var nearby = Game.ActiveScene.GetAllComponents<HumanPlayer>()
			.Any( p => p.IsAlive && p.WorldPosition.Distance( WorldPosition ) < 5000 );
		if ( !nearby )
		{
			Log.Info( "Zombie too far away, deleting: " + GameObject.Name );
			GameObject.Destroy();
		}
	}

	public void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		if ( name == "Attack" ) MeleeAttack();
		else if ( name == "IdleEnded" ) TimeSinceLongIdle = 0;
	}
}
