using Sandbox.UI;

namespace ZombieHorde;

public class HudRootPanel : RootPanel
{
	public static HudRootPanel Current;

	public HudRootPanel()
	{
		Current = this;

		AddChild<DamageIndicator>();
		AddChild<HitIndicator>();
		AddChild<InventoryBar>();
		AddChild<PickupFeed>();
		AddChild<ZomChatBox>();
		AddChild<StaminaBar>();
		AddChild<HealthBar>();
		AddChild<InfoBar>();
	}
}
