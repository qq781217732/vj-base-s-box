using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Compact Shotgun" )]
public partial class CompactShotgun : BaseZomWeapon
{
	public override float PrimaryRate => 1.2f;
	public override int ClipSize => 4;
	public override int AmmoMax => 24;
	public override float ReloadTime => 2.5f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Secondary;
	public override float BulletSpread => 0.12f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_CompactShotgun.png";

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) { DryFire(); if ( AvailableAmmo() > 0 ) Reload(); return; }
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "rust_pumpshotgun.shoot", WorldPosition );
		ShootEffects();
		ShootBullet( BulletSpread, 1.2f, 12.0f, 3, 6 );
		Owner.ViewPunch( Game.Random.NextFloat( -0.2f ) + -1.5f, Game.Random.NextFloat( 0.5f ) - 0.25f );
	}

	[Rpc.Broadcast] protected void ShootEffects() { CrosshairLastShoot = 0; ViewModelRenderer?.Set( "fire", true ); }
}
