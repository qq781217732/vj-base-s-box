using SWB.Base;
using SWB.Player;
using SWB.Shared;
using Sandbox.Citizen;
using System;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Human Player" )]
public partial class HumanPlayer : PlayerBase
{
	// Zombie-specific networked properties
	[Sync] public int RevivesRemaining { get; set; }
	[Sync] public TimeUntil TimeUntilAdrenalineExpires { get; set; }
	[Sync] public float Stamina { get; set; } = 40;
	[Sync] public float MaxStamina { get; set; } = 40;
	[Sync] public TimeSince TimeSinceUsedStamina { get; set; }
	[Sync] public TimeSince timeSinceDropped { get; set; }

	public bool SupressPickupNotices { get; private set; }
	public TimeSince TimeSinceLastKill { get; set; }
	public TimeSince TimeSinceDamage { get; set; } = 1.0f;
	public TimeSince TimeSinceHit { get; set; }

	float damageResistance;
	TimeUntil timeUntilResistanceExpires;
	TimeSince timeSincePassiveHealed;
	TimeSince timeSinceHeavyBreathing;
	public TimeSince TimeSinceStaminaDepleted { get; set; }
	TimeSince timeSincePinged;
	TimeSince timeSinceHeartBeat;
	TimeSince timeSincePerspectiveSwitch;

	[Sync] public Angles ViewPunchOffset { get; set; }
	[Sync] public Angles ViewPunchVelocity { get; set; }

	Sandbox.DamageInfo lastDamage;
	public GameObject Corpse { get; set; }

	public IEnumerable<GameObject> TouchingEntities => touchingEntities;
	public int TouchingEntityCount => touchingEntities.Count;
	readonly List<GameObject> touchingEntities = new();

	public SWB.Base.Weapon ActiveWeapon =>
		Inventory?.ActiveItem as SWB.Base.Weapon;

	void GiveWeapon( string className, bool setActive = false )
	{
		var weapon = WeaponRegistry.Instance.Get( className );

		if ( weapon is null )
		{
			Log.Error( $"[ZombieHorde] {className} not found in WeaponRegistry!" );
			return;
		}

		Inventory.AddClone( weapon.GameObject, setActive );
		SetAmmo( weapon.Primary.AmmoType, 360 );
	}

	// ========== Lifecycle ==========

	protected override void OnAwake()
	{
		base.OnAwake();
		Tags.Add( "player" );
		// Fix broken [Property] references from prefab type change
		BodyRenderer ??= Components.Get<SkinnedModelRenderer>();
		Camera ??= Components.Get<CameraComponent>( FindMode.EverythingInSelfAndChildren );
		ViewModelCamera ??= Components.GetInChildrenOrSelf<CameraComponent>();
		Dresser ??= Components.Get<Dresser>( FindMode.EnabledInSelf );
	}

	protected override void OnStart()
	{
		try { base.OnStart(); }
		catch ( Exception e ) { Log.Warning( $"PlayerBase.OnStart failed (may be missing Input Actions): {e.Message}" ); }
	}

	public override void Respawn( Transform? respawnAt = null )
	{
		try { base.Respawn( respawnAt ); }
		catch ( Exception e ) { Log.Warning( $"Respawn failed: {e.Message}" ); }
		if ( IsProxy ) return;
		SupressPickupNotices = true;
		RevivesRemaining = 3;
		timeSincePassiveHealed = 0;
		timeSinceDropped = 999;
		Stamina = MaxStamina;
		TimeSinceUsedStamina = 999;
		SupressPickupNotices = false;

		// Give SWB weapons
		GiveWeapon( "swb_revolver" );
		GiveWeapon( "swb_remington" );
		GiveWeapon( "swb_veresk" );
		GiveWeapon( "swb_scarh", true );
		GiveWeapon( "swb_l96a1" );
	}

	// ========== Update ==========

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if ( IsProxy ) return;
		UpdateViewOffset();
		if ( !IsAlive )
		{
			if ( !IsProxy && BaseGamemode.Current.EnableRespawning() )
				Respawn();
			return;
		}
		TickViewToggle();
		TickStamina();
		TickHeartBeat();
		TickDamageResistance();
		if ( !IsProxy ) TickPassiveHealth();
		if ( !IsProxy ) CheckFallOutOfMap();
	}

	void TickViewToggle()
	{
		if ( timeSincePerspectiveSwitch < 0.5f ) return;

		if ( Input.Pressed( InputButtonHelper.View ) )
		{
			var cam = CameraMovement as CameraMovement;
			if ( cam is not null )
			{
				cam.Distance = cam.Distance > 0 ? 0 : 60;
				timeSincePerspectiveSwitch = 0;
			}
		}
	}

	[ConCmd( "thirdperson", Help = "Toggles thirdperson" )]
	public static void CmdThirdperson()
	{
		var player = Local;
		if ( player is null || !player.IsAlive ) return;
		var cam = player.CameraMovement as CameraMovement;
		if ( cam is not null )
			cam.Distance = cam.Distance > 0 ? 0 : 60;
	}

	void TickPassiveHealth()
	{
		if ( Health < 20 && timeSincePassiveHealed > 1f )
		{
			timeSincePassiveHealed = 0;
			Health += 1;
		}
	}

	void CheckFallOutOfMap()
	{
		if ( WorldPosition.z < -20000 ) Kill();
	}

	// ========== Stamina ==========

	public bool TakeStamina( float amount )
	{
		if ( (Stamina - amount) >= 0 )
		{
			Stamina -= amount;
			Stamina = Stamina.Clamp( 0, MaxStamina );
			TimeSinceUsedStamina = 0;
			return true;
		}
		if ( timeSinceHeavyBreathing > 8f )
		{
			timeSinceHeavyBreathing = 0;
			PlaySoundEvent( null, "human.heavybreathing", 500 );
		}
		TimeSinceStaminaDepleted = 0;
		return false;
	}

	void TickStamina()
	{
		if ( TimeUntilAdrenalineExpires > 0.5f || TimeSinceUsedStamina > 1.5f )
		{
			var recoveryRate = TimeUntilAdrenalineExpires > 0.5f ? 6f : 2f;
			Stamina += Time.Delta * recoveryRate * TimeSinceUsedStamina;
		}
		Stamina = Stamina.Clamp( 0, MaxStamina );
	}

	void TickHeartBeat()
	{
		if ( IsProxy ) return;
		if ( Health > 19 ) return;
		var time = 0.4f + Health / 80;
		if ( timeSinceHeartBeat > time )
		{
			timeSinceHeartBeat = 0;
			var handle = Sound.Play( "human.heartbeat" );
			if ( handle is null ) return;
			handle.Volume = 1.25f - Health / 20;
			handle.Position = WorldPosition;
		}
	}

	// ========== Damage ==========

	public void AddDamageResistance( float time )
	{
		if ( timeUntilResistanceExpires < time )
			timeUntilResistanceExpires = time;
	}

	void TickDamageResistance()
	{
		if ( IsProxy ) return;
		damageResistance = 0;
		if ( timeUntilResistanceExpires > 0 ) damageResistance += 3;
		if ( TimeSinceHit < 0.5f ) damageResistance += 1;
	}

	public void ViewPunch( Angles angles ) { ViewPunchVelocity += angles; }
	public void ViewPunch( float pitch, float yaw = 0, float roll = 0 ) { ViewPunchVelocity += new Angles( pitch, yaw, roll ); }

	void UpdateViewOffset()
	{
		ViewPunchOffset += ViewPunchVelocity;
		ViewPunchOffset = Angles.Lerp( ViewPunchOffset, Angles.Zero, Time.Delta * 8f );
		ViewPunchVelocity = Angles.Lerp( ViewPunchVelocity, Angles.Zero, Time.Delta * 4f );
	}

	public override void TakeDamage( SWB.Shared.DamageInfo info )
	{
		if ( !IsAlive ) return;


		lastDamage = info;
		timeSincePassiveHealed = -2;
		TimeSinceHit = 0;
		if ( TimeUntilAdrenalineExpires < 0 ) CharacterController.Velocity = 0;
		ViewPunch( Game.Random.NextFloat( 0.5f ) + -0.25f, (Game.Random.NextFloat( 0.5f ) + 1) * (Game.Random.NextInt( 2 ) * 2 - 1) );
		if ( info.Attacker is not null )
		{
			var attacker = info.Attacker.Components.Get<HumanPlayer>();
			if ( attacker is not null && attacker != this )
			{
				info.Damage *= 0.1f;
				attacker.DidDamage( info.Position, info.Damage, ((float)Health).LerpInverse( 100, 0 ) );
			}
		}
		if ( info.Damage > 1 ) { info.Damage -= damageResistance; if ( info.Damage < 1 ) info.Damage = 1; }
		if ( Health > 0 && info.Damage > 0 )
		{
			Health -= (int)info.Damage;
			if ( Health <= 0 ) { Health = 200; Incapacitate(); }
		}
		TookDamage( info.Position );
	}

	public override void OnDeath( SWB.Shared.DamageInfo info )
	{
		if ( !IsValid ) return;
		ZomChatBox.AddInformation( $"{DisplayName} died!", $"avatar:{Network.Owner?.SteamId}" );
		DropInventory();
		base.OnDeath( info );
	}

	public void DropInventory( bool dropPistol = false ) { }

	// ========== Incapacitate / Revive ==========

	public void Incapacitate()
	{
		if ( RevivesRemaining > 0 )
		{
			ZomChatBox.AddInformation( $"{DisplayName} is incapacitated!", $"avatar:{Network.Owner?.SteamId}" );
			BodyRenderer.Set( "sit", 2 );
			BodyRenderer.Set( "sit_pose", Game.Random.NextInt( 3 ) );
			CharacterController.Velocity = 0;
			Sound.Play( "human.incapacitate", WorldPosition );
			RevivesRemaining -= 1;
			Health = 200;
		}
		else { Health = 0; Kill(); }
	}

	public void Revive()
	{
		BodyRenderer.Set( "sit", 0 );
		Health = 20;
		AddDamageResistance( 5 );
	}

	// ========== Ping ==========

	public void TryPing()
	{
		if ( timeSincePinged < 0.5f ) return;
		timeSincePinged = 0;
		var forward = EyeAngles.ToRotation().Forward;
		var tr = Scene.Trace.Ray( EyePos, EyePos + forward * 5000 )
			.IgnoreGameObjectHierarchy( GameObject ).WithoutTags( "trigger", "gib" ).Radius( 2 ).Run();
		var pos = tr.EndPosition + Vector3.Up * 10;
		PingMarker.Ping( pos, PingType.Generic, "Ping!", 5, null );
	}

	// ========== Nudge ==========

	public void NudgeNearbyPlayers()
	{
		foreach ( var obj in TouchingEntities )
		{
			if ( !obj.IsValid() ) continue;
			var ply = obj.Components.Get<HumanPlayer>();
			if ( ply is not null && ply != this )
				CharacterController.Velocity += (WorldPosition - ply.WorldPosition).WithZ( 0 ).Normal * 20;
		}
		if ( !IsProxy )
		{
			foreach ( var obj in TouchingEntities )
			{
				if ( !obj.IsValid() ) continue;
				var zom = obj.Components.Get<BaseZombie>();
				if ( zom is not null )
				{
					var knockback = (zom.WorldPosition - WorldPosition).WithZ( 0 ).Normal * 10;
					if ( zom.Agent is not null )
						zom.Agent.Velocity += knockback;
				}
			}
		}
	}

	protected void AddToucher( GameObject toucher )
	{
		if ( !toucher.IsValid() || touchingEntities.Contains( toucher ) ) return;
		touchingEntities.Add( toucher );
	}

	// ========== Damage feedback ==========

	[Rpc.Broadcast]
	public void DidDamage( Vector3 pos, float amount, float healthinv )
	{
		var snd = Sound.Play( "dm.ui_attacker" );
		if ( snd is not null ) snd.Pitch = 1 + healthinv;
		HitIndicator.Current?.OnHit( pos, amount );
	}

	[Rpc.Broadcast]
	public void TookDamage( Vector3 pos )
	{
		TimeSinceDamage = 0;
		DamageIndicator.Current?.OnHit( pos );
	}

	// ========== Footsteps ==========

	void OnAnimEventFootstep( SceneModel.FootstepEvent e )
	{
		if ( !IsAlive || IsProxy ) return;
		var tr = Scene.Trace.Ray( e.Transform.Position, e.Transform.Position + Vector3.Down * 20 )
			.Radius( 1 ).IgnoreGameObjectHierarchy( GameObject ).Run();
		if ( !tr.Hit ) return;
		var handle = Sound.Play( "footstep-concrete" );
		handle.Position = e.Transform.Position;
		handle.Volume = FootstepVolume();
	}

	public virtual float FootstepVolume() => Velocity.WithZ( 0 ).Length.LerpInverse( 0.0f, 200.0f );
}
