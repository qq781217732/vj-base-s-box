using Sandbox.UI;
using System.Threading.Tasks;

namespace ZombieHorde;
public partial class PickupFeed : Panel
{
	public static PickupFeed Current;

	public PickupFeed()
	{
		Current = this;
	}

	[Rpc.Broadcast]
	public static void OnPickup( string text )
	{
		Current?.AddEntry( text );
	}

	[Rpc.Broadcast]
	public static void OnPickupWeapon( string text )
	{
		Current?.AddWeaponEntry( text );
	}

	private async void AddEntry( string text )
	{
		var panel = new Panel();
		panel.Parent = this;
		panel.AddClass( "entry" );

		var label = panel.AddChild<Label>();
		label.Text = text;
		await Task.DelayRealtimeSeconds( 2.0f );
		panel.Delete();
	}

	private async void AddWeaponEntry( string text )
	{
		var panel = new Panel();
		panel.Parent = this;
		panel.AddClass( "entry" );
		panel.AddClass( text );

		var icon = new Panel();
		icon.Parent = panel;
		icon.AddClass( "icon" );
		await Task.DelayRealtimeSeconds( 2.0f );
		panel.Delete();
	}
}
