using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Fire Axe" )]
public partial class FireAxe : BaseZomWeapon
{
	public override float PrimaryRate => 1.0f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Primary;
	public override bool UseAlternativeSprintAnimation => true;

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "dm.crowbar_attack", WorldPosition );
	}

	public override bool CanPrimaryAttack() => Input.Down( InputButtonHelper.PrimaryAttack );
}
