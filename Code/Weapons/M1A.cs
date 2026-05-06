using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "M1A Rifle" )]
public partial class M1A : BaseZomWeapon
{
	public override float PrimaryRate => 5.0f;
	public override int ClipSize => 10;
	public override int AmmoMax => 60;
	public override float ReloadTime => 3.0f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override float BulletSpread => 0.03f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_M1A.png";

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) { DryFire(); if ( AvailableAmmo() > 0 ) Reload(); return; }
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "rifle.shoot", WorldPosition );
		ShootEffects();
		ShootBullet( BulletSpread, 2f, 45.0f );
		Owner.ViewPunch( Game.Random.NextFloat( -0.1f ) + -1.5f, Game.Random.NextFloat( 0.4f ) - 0.2f );
	}

	[Rpc.Broadcast] protected void ShootEffects() { CrosshairLastShoot = 0; ViewModelRenderer?.Set( "fire", true ); }
}
