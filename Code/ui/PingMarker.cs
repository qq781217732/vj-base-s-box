using Sandbox.UI;
using SWB.Shared;
using System.Threading.Tasks;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Ping Marker" )]
public partial class PingMarker : Component
{
	[Sync] public string Title { get; set; }
	[Sync] public PingType PingType { get; set; }

	private HudMarker _marker;

	protected override void OnStart()
	{
		if ( !IsProxy )
		{
			Sound.Play( "ui.popup.message.open" );
		}

		// Create UI marker on all clients
		_marker = new HudMarker( this );
		HudRootPanel.Current?.AddChild( _marker );
	}

	protected override void OnUpdate()
	{
		if ( _marker != null )
		{
			var pos = GameObject.Parent.IsValid() ? GameObject.Parent.WorldPosition : WorldPosition;
			_marker.UpdatePosition( pos );
		}
	}

	protected override void OnDestroy()
	{
		_marker?.Delete();
		_marker = null;
	}

	[Rpc.Broadcast]
	public static void Ping( Vector3 pos, PingType type = PingType.Generic, string title = null, float duration = 10, GameObject parent = null )
	{
		var go = new GameObject( true, "Ping" );
		go.WorldPosition = pos;
		var ping = go.Components.Create<PingMarker>();
		ping.Title = title;
		ping.PingType = type;

		// Remove existing pings on the same parent
		if ( parent.IsValid() )
		{
			go.SetParent( parent );

			foreach ( var child in parent.Children )
			{
				foreach ( var existingPing in child.Components.GetAll<PingMarker>() )
				{
					if ( existingPing != ping )
						existingPing.GameObject.Destroy();
				}
			}
		}

		if ( duration > 0 )
		{
			_ = DestroyAfterDelay( go, duration );
		}
	}

	private static async Task DestroyAfterDelay( GameObject go, float delay )
	{
		await GameTask.DelayRealtimeSeconds( delay );
		if ( go.IsValid() )
			go.Destroy();
	}
}

public partial class HudMarker : Panel
{
	private PingMarker _owner;
	private Label _titleLabel;

	public HudMarker( PingMarker owner )
	{
		_owner = owner;
		_titleLabel = AddChild<Label>();
		_titleLabel.Text = _owner.Title ?? "";
		_titleLabel.AddClass( "title" );
	}

	public void UpdatePosition( Vector3 worldPos )
	{
		if ( !_owner.GameObject.IsValid() )
		{
			Delete();
			return;
		}

		var cam = Scene.Camera;
		if ( cam == null ) return;
		var screenPos = cam.PointToScreenPixels( worldPos );

		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
	}
}
