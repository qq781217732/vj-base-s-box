namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Special Zombie" )]
public partial class SpecialZombie : BaseZombie
{
	protected override void OnStart()
	{
		base.OnStart();

		// TODO: Clothing API changed - UpdateClothes/Dress removed
	}
}
