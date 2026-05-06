using Sandbox.UI;

namespace ZombieHorde;

/// <summary>
/// Main HUD for Zombie Horde. Integrates all UI components.
/// Attach this to a RootPanel to set up the encounter/survival HUD.
/// </summary>
public class ZomHud : Panel
{
	public static ZomHud Current { get; private set; }

	public RoundInfo RoundInfo { get; set; }
	public Notification Notification { get; set; }
	public InventoryBar InventoryBar { get; set; }
	public PickupFeed PickupFeed { get; set; }
	public ZomChatBox ChatBox { get; set; }

	public ZomHud()
	{
		Current = this;

		StyleSheet.Load( "/resource/styles/hud.scss" );

		RoundInfo = AddChild<RoundInfo>();
		Notification = AddChild<Notification>();
		InventoryBar = AddChild<InventoryBar>();
		PickupFeed = AddChild<PickupFeed>();
		ChatBox = AddChild<ZomChatBox>();

		AddChild<DamageIndicator>();
		AddChild<HitIndicator>();
		AddChild<StaminaBar>();
	}
}
