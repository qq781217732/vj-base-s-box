using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Revolver" )]
public partial class Revolver : BaseZomWeapon
{
	public override float PrimaryRate => 3.0f;
	public override int ClipSize => 6;
	public override int AmmoMax => 40;
	public override float ReloadTime => 2.5f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Secondary;
	public override float BulletSpread => 0.01f;
	public override float ShotSpreadMultiplier => 3f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_Magnum.png";

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) { DryFire(); if ( AvailableAmmo() > 0 ) Reload(); return; }
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "magnum.shoot", WorldPosition );
		ShootEffects();
		ShootBullet( BulletSpread, 2f, 50.0f );
		Owner.ViewPunch( Game.Random.NextFloat( -0.5f ) + -2.5f, Game.Random.NextFloat( 0.5f ) - 0.25f );
	}

	[Rpc.Broadcast] protected void ShootEffects() { CrosshairLastShoot = 0; ViewModelRenderer?.Set( "fire", true ); }
}
