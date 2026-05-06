using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "AKM" )]
public partial class AKM : BaseZomWeapon
{
	public static readonly Model WorldModel = Model.Load( "weapons/licensed/hqfpsweapons/fp_equipment/assaultrifles/akm/w_akm.vmdl" );

	public override float PrimaryRate => 9.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 30;
	public override int AmmoMax => 320;
	public override float ReloadTime => 2.8f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override float BulletSpread => 0.1f;
	public override float ShotSpreadMultiplier => 1.5f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_AKM.png";

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
		Sound.Play( "akm.shoot", WorldPosition );
		Sound.Play( "akm.shoot.tail", WorldPosition );
		ShootBullet( BulletSpread, 1.5f, 35.0f );
		Owner.ViewPunch( Game.Random.NextFloat( -0.1f ) + -1.3f, Game.Random.NextFloat( 0.5f ) - 0.25f );
	}

	[Rpc.Broadcast]
	protected void ShootEffects()
	{
		CrosshairLastShoot = 0;
		ViewModelRenderer?.Set( "fire", true );
	}
}
