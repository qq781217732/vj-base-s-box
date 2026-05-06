using Sandbox.UI;
using System;

namespace ZombieHorde;

public partial class HealthBar : Panel
{
	public static HealthBar Instance;
	public HealthBar() { Instance = this; }

	Label hpLabel;

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

		var pct = (float)player.Health / player.MaxHealth;
		SetClass( "low", pct < 0.3f );
		SetClass( "critical", pct < 0.15f );

		if ( hpLabel == null )
		{
			hpLabel = AddChild<Label>();
			hpLabel.AddClass( "hp-text" );
		}
		int hp = Math.Max( 0, (int)Math.Ceiling( (float)player.Health ) );
		hpLabel.Text = hp.ToString() + " HP";

		if ( bar is null )
		{
			bar = new Panel();
			bar.Parent = this;
			bar.AddClass( "fill" );
		}
		var maxW = Box.Rect.Width;
		bar.Style.Width = Length.Pixels( maxW * pct );
	}
	Panel bar;
}
