using Sandbox.UI;
using System.Threading.Tasks;

namespace ZombieHorde;

/// <summary>
/// Simple notification toast system for on-screen messages.
/// </summary>
public class Notification : Panel
{
	public static Notification Current;

	public Notification()
	{
		Current = this;
	}

	/// <summary>
	/// Show a notification message that fades out after duration.
	/// </summary>
	public static void Show( string message, float duration = 3f, string iconClass = null )
	{
		Current?.AddNotification( message, duration, iconClass );
	}

	private async void AddNotification( string message, float duration, string iconClass )
	{
		var panel = new Panel();
		panel.Parent = this;
		panel.AddClass( "notification" );
		var label = panel.AddChild<Label>();
		label.Text = message;
		label.AddClass( "notification-text" );

		if ( !string.IsNullOrEmpty( iconClass ) )
		{
			var iconPanel = new Panel();
			iconPanel.Parent = panel;
			iconPanel.AddClass( "notification-icon" );
			iconPanel.AddClass( iconClass );
		}

		await Task.DelayRealtimeSeconds( duration );
		panel.AddClass( "fade-out" );
		await Task.DelayRealtimeSeconds( 0.5f );
		panel.Delete();
	}
}
