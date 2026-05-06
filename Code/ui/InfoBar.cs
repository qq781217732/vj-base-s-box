using Sandbox.UI;

namespace ZombieHorde;

internal class InfoBar : Panel
{
	public Label Timer;
	public Label State;
	TimeSince timeSinceCheck;

	public InfoBar()
	{
		Timer = AddChild<Label>( "timer" );
		State = AddChild<Label>( "state" );
	}

	public override void Tick()
	{
		base.Tick();
		if ( timeSinceCheck < 0.2f ) return;
		timeSinceCheck = 0;

		var gm = BaseGamemode.Current;
		if ( gm is null ) return;
		Timer.Text = gm.RoundName ?? "";
		State.Text = gm.RoundInfo ?? "";
	}
}
