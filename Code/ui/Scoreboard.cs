using Sandbox.UI;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Scoreboard" )]
public class Scoreboard : Panel
{
	public Scoreboard()
	{
		StyleSheet.Load( "/resource/styles/scoreboard.scss" );
	}
}
