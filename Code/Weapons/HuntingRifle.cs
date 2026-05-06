using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Hunting Rifle" )]
public partial class HuntingRifle : BaseZomWeapon
{
	public override float PrimaryRate => 0.8f;
	public override int ClipSize => 5;
	public override int AmmoMax => 25;
	public override float ReloadTime => 3.5f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override float BulletSpread => 0.005f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_HuntingRifle.png";

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) { DryFire(); if ( AvailableAmmo() > 0 ) Reload(); return; }
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "rifle.shoot", WorldPosition );
		ShootEffects();
		ShootBullet( BulletSpread, 3f, 80.0f );
		Owner.ViewPunch( Game.Random.NextFloat( -0.3f ) + -3.0f, Game.Random.NextFloat( 0.5f ) - 0.25f );
	}

	[Rpc.Broadcast] protected void ShootEffects() { CrosshairLastShoot = 0; ViewModelRenderer?.Set( "fire", true ); }
}
