using Sandbox.UI;

namespace ZombieHorde;

/// <summary>
/// Displays round/wave information on the HUD.
/// Reads from BaseGamemode's synced properties.
/// </summary>
public class RoundInfo : Panel
{
	public Label RoundNameLabel;
	public Label RoundInfoLabel;

	public RoundInfo()
	{
		StyleSheet.Load( "/resource/styles/roundinfo.scss" );

		RoundNameLabel = AddChild<Label>();
		RoundNameLabel.AddClass( "round-name" );
		RoundInfoLabel = AddChild<Label>();
		RoundInfoLabel.AddClass( "round-info" );
	}

	public override void Tick()
	{
		base.Tick();

		var gm = BaseGamemode.Current;
		if ( gm == null ) return;

		RoundNameLabel.Text = gm.RoundName;
		RoundInfoLabel.Text = gm.RoundInfo;

		SetClass( "hidden", string.IsNullOrEmpty( gm.RoundName ) );
	}
}
