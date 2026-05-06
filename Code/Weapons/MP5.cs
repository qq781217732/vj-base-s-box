using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "MP5" )]
public partial class MP5 : BaseZomWeapon
{
	public static readonly Model WorldModel = Model.Load( "weapons/licensed/hqfpsweapons/fp_equipment/smgs/mp5/w_mp5.vmdl" );

	public override float PrimaryRate => 9.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 40;
	public override int AmmoMax => 250;
	public override float ReloadTime => 3.4f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override float BulletSpread => 0.12f;
	public override float ShotSpreadMultiplier => 1.25f;
	public override string Icon => "weapons/licensed/HQFPSWeapons/Icons/Inventory/Items/Equipment/Icon_MP5.png";

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
		Sound.Play( "ar2.shoot", WorldPosition );
		Sound.Play( "smg1.shoot.tail", WorldPosition );
		ShootBullet( BulletSpread, 1.5f, 20.0f );
		Owner.ViewPunch( Game.Random.NextFloat( -0.2f ) + -0.8f, Game.Random.NextFloat( 0.5f ) - 0.25f );
	}

	[Rpc.Broadcast]
	protected void ShootEffects()
	{
		CrosshairLastShoot = 0;
		ViewModelRenderer?.Set( "fire", true );
	}
}
