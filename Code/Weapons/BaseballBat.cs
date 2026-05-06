using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Baseball Bat" )]
public partial class BaseballBat : BaseZomWeapon
{
	public override float PrimaryRate => 1.5f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Secondary;
	public override bool UseAlternativeSprintAnimation => true;

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		Owner.BodyRenderer.Set( "b_attack", true );
		Sound.Play( "dm.crowbar_attack", WorldPosition );
	}

	public override bool CanPrimaryAttack() => Input.Down( InputButtonHelper.PrimaryAttack );
}
