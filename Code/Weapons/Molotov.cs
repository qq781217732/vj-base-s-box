using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Molotov Cocktail" )]
public partial class Molotov : BaseZomWeapon
{
	public override float PrimaryRate => 1.0f;
	public override int ClipSize => 1;
	public override float ReloadTime => 1.0f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Grenade;
	public override int AmmoMax => 0;
	public override string Icon => "/ui/weapons/zom_molotov.png";
	public override bool UseAlternativeSprintAnimation => true;

	public override bool CanPrimaryAttack()
	{
		return Input.Released( InputButtonHelper.PrimaryAttack );
	}

	public async override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( Owner is null ) return;
		if ( !TakeAmmo( 1 ) ) { Reload(); return; }

		ViewModelRenderer?.Set( "fire", true );
		Sound.Play( "rust_boneknife.attack", WorldPosition );
		Owner.BodyRenderer.Set( "b_attack", true );

		await GameTask.Delay( 300 );

		if ( !IsProxy )
		{
			var mollyGO = new GameObject( true, "Molotov" );
			mollyGO.WorldPosition = Owner.EyePos + Owner.EyeAngles.ToRotation().Forward * 3.0f;
			mollyGO.Components.Create<ThrownMolotov>();
			mollyGO.NetworkSpawn();
			var rb = mollyGO.Components.Get<Rigidbody>();
			if ( rb is not null )
				rb.Velocity = Owner.EyeAngles.ToRotation().Forward * 500.0f + Owner.EyeAngles.ToRotation().Up * 150.0f + Owner.Velocity.WithZ( 0 );
		}

		await GameTask.Delay( 100 );
		Reload();
		if ( !IsProxy && AmmoClip == 0 && AmmoReserve == 0 ) GameObject.Destroy();
	}
}
