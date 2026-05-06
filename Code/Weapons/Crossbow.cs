using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Crossbow" )]
public partial class Crossbow : BaseZomWeapon
{
	public override float PrimaryRate => 1.0f;
	public override int ClipSize => 1;
	public override int AmmoMax => 15;
	public override float ReloadTime => 2.5f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override float BulletSpread => 0.005f;

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) { DryFire(); if ( AvailableAmmo() > 0 ) Reload(); return; }
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "rust_crossbow.shoot", WorldPosition );
		ShootEffects();
		ShootBullet( BulletSpread, 1f, 75.0f, 8 );
		Owner.ViewPunch( Game.Random.NextFloat( -0.5f ) + -3.0f, Game.Random.NextFloat( 0.5f ) - 0.25f );
	}

	[Rpc.Broadcast] protected void ShootEffects() { CrosshairLastShoot = 0; ViewModelRenderer?.Set( "fire", true ); }
}
