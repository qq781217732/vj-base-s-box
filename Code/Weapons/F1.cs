using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "FAMAS" )]
public partial class F1 : BaseZomWeapon
{
	public static readonly Model WorldModel = Model.Load( "weapons/licensed/hqfpsweapons/fp_equipment/assaultrifles/f1/w_f1.vmdl" );

	public override float PrimaryRate => 10.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 25;
	public override int AmmoMax => 250;
	public override float ReloadTime => 3.3f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override float BulletSpread => 0.10f;
	public override float ShotSpreadMultiplier => 1.25f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_F1.png";

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			if ( AvailableAmmo() > 0 ) Reload();
			return;
		}

		Owner.BodyRenderer.Set( "b_attack", true );
		ShootEffects();
		Sound.Play( "ar1.shoot", WorldPosition );
		Sound.Play( "ar1.shoot.tail", WorldPosition );
		ShootBullet( BulletSpread, 1f, 20.0f );
		Owner.ViewPunch( Game.Random.NextFloat( -0.1f ) + -1.2f, Game.Random.NextFloat( 0.5f ) - 0.25f );
	}

	[Rpc.Broadcast]
	protected void ShootEffects()
	{
		CrosshairLastShoot = 0;
		ViewModelRenderer?.Set( "fire", true );
	}
}
