using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Rusty Shovel" )]
public partial class Shovel : BaseZomWeapon
{
	public override float PrimaryRate => 1.2f;
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
