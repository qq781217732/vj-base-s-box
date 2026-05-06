using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "R870 Shotgun" )]
public partial class R870 : BaseZomWeapon
{
	public override float PrimaryRate => 1.0f;
	public override int ClipSize => 6;
	public override int AmmoMax => 32;
	public override float ReloadTime => 3.0f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override float BulletSpread => 0.08f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_R870Tactical.png";

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) { DryFire(); if ( AvailableAmmo() > 0 ) Reload(); return; }
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "rust_pumpshotgun.shoot", WorldPosition );
		ShootEffects();
		ShootBullet( BulletSpread, 1.5f, 16.0f, 3, 8 );
		Owner.ViewPunch( Game.Random.NextFloat( -0.2f ) + -2.0f, Game.Random.NextFloat( 0.7f ) - 0.35f );
	}

	[Rpc.Broadcast] protected void ShootEffects() { CrosshairLastShoot = 0; ViewModelRenderer?.Set( "fire", true ); }
}
