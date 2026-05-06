using Sandbox.UI;

namespace ZombieHorde;

/// <summary>
/// Manages the lifetime of name tag panels on players.
/// Attached to each player GameObject. Shows/hides based on distance.
/// </summary>
public class NameTagComponent : Component
{
	private Label _nameLabel;
	private float _maxViewDistance = 1000f;

	protected override void OnStart()
	{
		var playerName = Components.Get<HumanPlayer>()?.Network?.Owner?.DisplayName ?? GameObject.Name;

		// Create a 3D label using TextRenderer or keep simple with just a GameObject name
	}

	protected override void OnUpdate()
	{
		if ( _nameLabel == null ) return;

		// Check if we should be visible
		var cam = Scene.Camera;
		if ( cam == null ) return;

		var dist = WorldPosition.Distance( cam.WorldPosition );
		var owner = Components.Get<HumanPlayer>()?.Network?.Owner;
		var isLocal = owner is not null && owner == Connection.Local;

		if ( isLocal || dist > _maxViewDistance )
		{
			_nameLabel.Style.Display = DisplayMode.None;
			return;
		}

		_nameLabel.Style.Display = DisplayMode.Flex;

		// Position via screen projection
		var screenPos = WorldPosition + Vector3.Up * 15f;
		// Screen position handled by parent
	}

	protected override void OnDestroy()
	{
		_nameLabel?.Delete();
		_nameLabel = null;
	}
}
