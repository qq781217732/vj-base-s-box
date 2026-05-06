namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Tripmine" )]
public partial class TripmineWeapon : BaseZomWeapon
{
	public override float PrimaryRate => 1.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 1;
	public override int AmmoMax => 0;
	public override WeaponSlot WeaponSlot => WeaponSlot.Grenade;

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !TakeAmmo( 1 ) ) return;

		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "rust_boneknife.attack", WorldPosition );

		AsyncDeploy();
	}

	async void AsyncDeploy()
	{
		await GameTask.Delay( 300 );
		if ( !IsProxy && GameObject.IsValid() )
		{
			var mine = new GameObject( true, "Tripmine" );
			mine.WorldPosition = Owner.EyePos + Owner.EyeAngles.ToRotation().Forward * 3.0f;
			var mineComp = mine.Components.Create<Tripmine>();
			mineComp.Owner = Owner.GameObject;
			mine.NetworkSpawn();

			Sound.Play( "tripmine.deployed", mine.WorldPosition );
		}

		if ( IsProxy ) return;
		if ( AmmoClip == 0 )
		{
			((ZomInventory)Owner.Inventory)?.DropItem( GameObject );
			GameObject.Destroy();
		}
	}
}
