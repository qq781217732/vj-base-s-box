namespace ZombieHorde;
using Sandbox;
public sealed class MyComponent : Component
{
	[Property] public string StringProperty { get; set; }

	protected override void OnUpdate()
	{	
	
		DebugOverlay.Box( Vector3.Zero, new Vector3( 100f, 100f, 100f ), Color.Yellow, 0f, default, true );
		DebugOverlay.ScreenText( new Vector2( 32f, 32f ), "DebugOverlay works!" );
	
	}
}
