using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Double Barrel Shotgun" )]
public partial class DoubleBarrel : BaseZomWeapon
{
	public override float PrimaryRate => 0.5f;
	public override int ClipSize => 2;
	public override int AmmoMax => 20;
	public override float ReloadTime => 2.0f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override float BulletSpread => 0.1f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_DoubleBarrel.png";

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) { DryFire(); if ( AvailableAmmo() > 0 ) Reload(); return; }
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "rust_pumpshotgun.shootdouble", WorldPosition );
		ShootEffects();
		ShootBullet( BulletSpread, 2f, 18.0f, 3, 12 );
		Owner.ViewPunch( Game.Random.NextFloat( -0.3f ) + -3.0f, Game.Random.NextFloat( 1.0f ) - 0.5f );
	}

	[Rpc.Broadcast] protected void ShootEffects() { CrosshairLastShoot = 0; ViewModelRenderer?.Set( "fire", true ); }
}
