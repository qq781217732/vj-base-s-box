using SWB.Player;
using SWB.Shared;
using System;

namespace ZombieHorde;

/// <summary>
/// Base weapon component for Zombie Horde. Adds zombie-specific features
/// (melee shove, zombie alerts, weapon slots) on top of SWB Weapon.
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Base Zombie Weapon" )]
public partial class BaseZomWeapon : Component, IInventoryItem
{
	// ========== IInventoryItem ==========
	[Sync] public int Slot { get; set; }
	[Sync] public virtual string Icon { get; set; } = "";
	[Sync] public string DisplayName { get; set; } = "";
	[Sync] public float Mobility { get; set; } = 1;

	// ========== Weapon settings ==========
	public virtual int ClipSize => 16;
	public virtual float ReloadTime => 3.0f;
	public virtual WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public virtual int AmmoMax => 60;
	public virtual float BulletSpread => 0.05f;
	public virtual float ShotSpreadMultiplier => 2f;
	public virtual float ShotSpreadLerp => 0.2f;
	public virtual bool UseAlternativeSprintAnimation => false;
	public virtual Color RarityColor => Color.White;

	// ========== Networked state ==========
	[Sync] public int AmmoClip { get; set; }
	[Sync] public int AmmoReserve { get; set; }
	[Sync] public TimeSince TimeSinceReload { get; set; }
	[Sync] public bool IsReloading { get; set; }
	[Sync] public TimeSince TimeSinceDeployed { get; set; }
	[Sync] public TimeSince TimeSinceShove { get; set; }
	[Sync] public TimeSince TimeSincePrimaryAttack { get; set; }
	[Sync] public TimeSince TimeSinceSecondaryAttack { get; set; }
	[Sync] public float SpreadMultiplier { get; set; } = 1;
	[Sync] public bool OverridingAnimator { get; set; }

	public virtual float PrimaryRate => 5.0f;
	public virtual float SecondaryRate => 10.0f;

	// Owner reference
	public HumanPlayer Owner { get; private set; }
	public SkinnedModelRenderer EffectRenderer { get; private set; }

	protected TimeSince CrosshairLastShoot { get; set; }
	protected TimeSince CrosshairLastReload { get; set; }

	// ========== Lifecycle ==========

	protected override void OnAwake()
	{
		Tags.Add( "weapon" );
	}

	protected override void OnStart()
	{
		Owner = Components.GetInAncestors<HumanPlayer>();
		EffectRenderer = Components.Get<SkinnedModelRenderer>();
	}

	// ========== IInventoryItem ==========

	[Rpc.Broadcast]
	public virtual void OnCarryStart()
	{
		GameObject.Enabled = true;
		TimeSinceDeployed = 0;

		if ( ViewModelRenderer is not null )
			ViewModelRenderer.Set( "deploy", true );
	}

	[Rpc.Broadcast]
	public virtual void OnCarryStop()
	{
		GameObject.Enabled = false;
		IsReloading = false;
	}

	public virtual bool CanCarryStop()
	{
		return TimeSinceDeployed > 0;
	}

	public SkinnedModelRenderer ViewModelRenderer { get; set; }

	protected override void OnUpdate()
	{
		if ( !Owner.IsValid() || Owner.IsProxy || Owner.IsBot ) return;

		if ( TimeSinceDeployed < 0.6f ) return;

		if ( !IsReloading )
		{
			if ( CanPrimaryAttack() )
			{
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}
			else if ( CanSecondaryAttack() )
			{
				TimeSinceSecondaryAttack = 0;
				AttackSecondary();
			}
			else if ( Input.Down( InputButtonHelper.Reload ) )
			{
				Reload();
			}
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
			OnReloadFinish();

		AdjustAccuracyMultiplier();
	}

	// ========== Attack ==========

	public virtual bool CanPrimaryAttack()
	{
		if ( TimeSinceShove < 0.5f ) return false;
		if ( !Owner.IsValid() || !Input.Down( InputButtonHelper.PrimaryAttack ) ) return false;

		var rate = PrimaryRate;
		if ( rate <= 0 ) return true;
		return TimeSincePrimaryAttack > (1 / rate);
	}

	public virtual bool CanSecondaryAttack()
	{
		if ( TimeSinceShove > 1 ) return true;
		return false;
	}

	public virtual void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		ShootBullet( BulletSpread, 1, 15 );
	}

	public virtual void AttackSecondary()
	{
		if ( TimeSinceShove > 1 )
			MeleeAttack();
	}

	// ========== Melee Attack (Shove) ==========

	public virtual async void MeleeAttack()
	{
		var ply = Owner;
		if ( !ply.IsValid() ) return;

		var speedMultiplier = 1f;
		TimeSinceShove = 0;

		if ( !ply.TakeStamina( 5 ) )
		{
			speedMultiplier *= 0.25f;
			ply.Stamina = 0;
			ply.TimeSinceUsedStamina = 0;
			TimeSinceShove = -1f;
		}

		// View punch
		ply.ViewPunch( Game.Random.NextFloat( 0.5f ) + -0.25f, Game.Random.NextFloat( 0.25f ) + 0.25f );

		OverridingAnimator = true;

		var forward = ply.EyeAngles.ToRotation().Forward;
		var eyePos = ply.EyePos;

		// Trace for melee hit
		var tr = Game.ActiveScene.Trace.Ray( eyePos, eyePos + forward * 90 )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "zombie", "trigger" )
			.Radius( 8 )
			.Run();

		if ( tr.Hit && tr.GameObject.IsValid() )
		{
			if ( !IsProxy )
			{
				var damageInfo = new SWB.Shared.DamageInfo
				{
					Attacker = ply.GameObject,
					Weapon = GameObject,
					Damage = 20,
					Origin = eyePos,
					Force = forward * 100,
					Position = tr.EndPosition,
					Tags = { "slash" }
				};

				foreach ( var damageable in tr.GameObject.Components.GetAll<IDamageable>() )
					damageable.OnDamage( damageInfo );
			}
		}

		if ( !IsProxy )
			TryAlertZombies( ply.GameObject, 1f, 50f, eyePos + forward * 60 );

		Sound.Play( "dm.crowbar_attack", WorldPosition );
		ply.BodyRenderer.Set( "b_attack", true );

		await GameTask.Delay( (int)(300 / speedMultiplier) );
		OverridingAnimator = false;
	}

	// ========== Shoot ==========

	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize = 4, int bulletCount = 1 )
	{
		var ply = Owner;
		if ( !ply.IsValid() ) return;

		spread *= SpreadMultiplier;
		SpreadMultiplier *= ShotSpreadMultiplier;

		for ( int i = 0; i < bulletCount; i++ )
		{
			var forward = ply.EyeAngles.ToRotation().Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			var eyePos = ply.EyePos;
			var endPos = eyePos + forward * 5000;

			var tr = Game.ActiveScene.Trace.Ray( eyePos, endPos )
				.IgnoreGameObjectHierarchy( GameObject )
				.WithoutTags( "trigger" )
				.Radius( bulletSize )
				.Run();

			if ( tr.Hit && tr.GameObject.IsValid() && !IsProxy )
			{
				var damageInfo = new SWB.Shared.DamageInfo
				{
					Attacker = ply.GameObject,
					Weapon = GameObject,
					Damage = damage,
					Origin = eyePos,
					Force = forward * 100 * force,
					Position = tr.EndPosition,
					Tags = { TagsHelper.Bullet }
				};

				foreach ( var damageable in tr.GameObject.Components.GetAll<IDamageable>() )
					damageable.OnDamage( damageInfo );

				TryAlertZombies( damageInfo.Attacker, 0.2f, 500f, tr.HitPosition );
			}
		}


			NoiseSystem.Emit( WorldPosition, 500f, 0.6f, NoiseType.Gunshot );

		if ( !TakeAmmo( 1 ) )
			DryFire();
	}

	// ========== Reload ==========

	public virtual void Reload()
	{
		if ( IsReloading ) return;
		if ( AmmoClip >= ClipSize ) return;
		if ( AmmoReserve <= 0 && AmmoMax != -1 ) return;

		TimeSinceReload = 0;

		if ( Owner.TimeUntilAdrenalineExpires > 0.5f )
			TimeSinceReload = ReloadTime / 1.5f;

		IsReloading = true;
		Owner.BodyRenderer.Set( "b_reload", true );
	}

	public virtual void OnReloadFinish()
	{
		IsReloading = false;

		if ( AmmoMax == -1 )
		{
			AmmoClip = ClipSize;
			return;
		}

		var ammo = Math.Min( AmmoReserve, ClipSize - AmmoClip );
		AmmoReserve -= ammo;
		AmmoClip += ammo;
	}

	// ========== Ammo ==========

	public bool TakeAmmo( int amount )
	{
		if ( AmmoClip < amount ) return false;
		AmmoClip -= amount;
		return true;
	}

	public int AvailableAmmo()
	{
		if ( Owner is null ) return 0;
		if ( AmmoMax == -1 ) return -1;
		return AmmoReserve;
	}

	public bool IsUsable()
	{
		if ( AmmoClip > 0 ) return true;
		return AvailableAmmo() > 0;
	}

	public bool IsUsable( GameObject user )
	{
		var ply = user?.Components.Get<HumanPlayer>();
		if ( ply is null ) return false;
		if ( ply.timeSinceDropped < 0.5f ) return false;
		return true;
	}

	public bool OnUse( GameObject user )
	{
		var ply = user?.Components.Get<HumanPlayer>();
		if ( ply is null || !ply.IsAlive ) return false;

		var inv = ply.Inventory as ZomInventory;
		if ( inv is null ) return false;

		GameObject dropped = null;

		switch ( WeaponSlot )
		{
			case WeaponSlot.Secondary:
			case WeaponSlot.Grenade:
			case WeaponSlot.Medkit:
			case WeaponSlot.Pills:
				dropped = inv.DropItem( inv.GetSlot( WeaponSlot ) );
				break;
			case WeaponSlot.Primary:
				if ( inv.Primary1 is null || !inv.Primary1.IsValid() )
				{
					inv.Add( GameObject, true );
					ply.timeSinceDropped = 0;
					return true;
				}
				if ( inv.Active is not null && inv.Active.Components.TryGet<BaseZomWeapon>( out var activeWep )
					&& activeWep.WeaponSlot == WeaponSlot.Primary )
				{
					dropped = inv.DropActive();
					inv.Add( GameObject, true );
				}
				else
				{
					dropped = inv.DropItem( inv.Primary1 );
				}
				break;
		}

		inv.Add( GameObject, WeaponSlot == WeaponSlot.Secondary || WeaponSlot == WeaponSlot.Primary );

		if ( dropped is not null && dropped.IsValid() )
		{
			var rb = dropped.Components.Get<Rigidbody>();
			if ( rb is not null )
			{
				rb.Velocity = ply.Velocity + (ply.EyeAngles.ToRotation().Forward + ply.EyeAngles.ToRotation().Up) * 200;
			}
		}

		ply.timeSinceDropped = 0;
		return true;
	}

	// ========== Accuracy ==========

	public virtual void AdjustAccuracyMultiplier()
	{
		if ( !Owner.IsValid() ) return;

		if ( !Owner.IsAlive )
		{
			SpreadMultiplier = SpreadMultiplier.LerpTo( 1, ShotSpreadLerp );
			SpreadMultiplier = SpreadMultiplier.Clamp( 0, 12 );
			return;
		}

		var targetMultiplier = 1f;
		var adjustedVelocity = MathF.Floor( Owner.Velocity.WithZ( 0 ).Length );

		targetMultiplier = Math.Min( adjustedVelocity / 220f + 1, 2f ) * 0.5f + 0.5f;

		if ( !Owner.IsOnGround )
			targetMultiplier *= 1.2f;
		else if ( Owner.IsCrouching )
			targetMultiplier *= 0.75f;

		SpreadMultiplier = SpreadMultiplier.LerpTo( targetMultiplier, ShotSpreadLerp );
		SpreadMultiplier = SpreadMultiplier.Clamp( 0, 12 );
	}

	// ========== Zombie alert ==========

	[Rpc.Broadcast]
	public void TryAlertZombies( GameObject target, float percent, float radius, Vector3 position )
	{
		if ( IsProxy ) return;

		var sphere = new Sphere( position, radius );
		foreach ( var obj in Game.ActiveScene.FindInPhysics( sphere ) )
		{
			if ( !obj.IsValid() ) continue;

			var zom = obj.Components.Get<CommonZombie>();
			if ( zom is not null )
			{
				zom.Brain?.ForceEngage( target );
			}
		}
	}

	// ========== DRY FIRE ==========

	[Rpc.Broadcast]
	public virtual void DryFire()
	{
		Sound.Play( "dm.dryfire", WorldPosition );
	}
}
