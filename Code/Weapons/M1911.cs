using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "M1911 Pistol" )]
public partial class M1911 : BaseZomWeapon
{
	public override float PrimaryRate => 12.0f;
	public override float SecondaryRate => 4.5f;
	public override int ClipSize => 12;
	public override int AmmoMax => 120;
	public override float ReloadTime => 2.2f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Secondary;
	public override float BulletSpread => 0.08f;
	public override float ShotSpreadMultiplier => 2.0f;

	protected override void OnStart()
	{
		base.OnStart();
		AmmoClip = ClipSize;
		AmmoReserve = AmmoMax;
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) { DryFire(); if ( AvailableAmmo() > 0 ) Reload(); return; }
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "rust_pistol.shoot", WorldPosition );
		ShootEffects();
		ShootBullet( BulletSpread, 0.5f, 18.0f );
		Owner.ViewPunch( Game.Random.NextFloat( -0.1f ) + -1f, Game.Random.NextFloat( 0.3f ) - 0.15f );
	}

	[Rpc.Broadcast] void ShootEffects() { CrosshairLastShoot = 0; ViewModelRenderer?.Set( "fire", true ); }
}
