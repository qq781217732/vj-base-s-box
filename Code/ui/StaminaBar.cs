using Sandbox.UI;
using SWB.Shared;

namespace ZombieHorde;

public partial class StaminaBar : Panel
{
	Panel fill;

	HumanPlayer GetLocalPlayer()
	{
		return Game.ActiveScene.GetAllComponents<HumanPlayer>()
			.FirstOrDefault( p => !p.IsProxy );
	}

	public override void Tick()
	{
		base.Tick();
		var player = GetLocalPlayer();
		if ( player is null || !player.IsAlive ) { Style.Opacity = 0; return; }
		Style.Opacity = 1;

		var pct = (float)player.Stamina / player.MaxStamina;

		if ( fill is null )
		{
			fill = new Panel();
			fill.Parent = this;
			fill.AddClass( "fill" );
		}
		var maxW = Box.Rect.Width;
		fill.Style.Width = Length.Pixels( maxW * pct );
	}
}
