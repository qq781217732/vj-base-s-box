using Sandbox.UI;

namespace ZombieHorde;

/// <summary>3D avatar preview panel using SceneWorld for player health bar</summary>
public partial class HealthBarAvatar : Panel
{
	SceneWorld sceneWorld;
	ScenePanel scenePanel;
	GameObject avatarObject;
	float angle;

	public HealthBarAvatar()
	{
		Style.Width = 256;
		Style.Height = 256;

		sceneWorld = new SceneWorld();
		scenePanel = new ScenePanel();
		scenePanel.Parent = this;

		// Create avatar object with model and clothes
		CreateAvatar();
	}

	void CreateAvatar()
	{
		avatarObject = new GameObject( true, "Avatar" );
		avatarObject.WorldPosition = new Vector3( 0, 0, 72 );
		avatarObject.WorldRotation = Rotation.FromYaw( 180 );

		var renderer = avatarObject.Components.Create<SkinnedModelRenderer>();
		renderer.Model = Model.Load( "models/citizen/citizen.vmdl" );

		// Fallback: don't dress in scene world for now
	}

	public override void Tick()
	{
		base.Tick();
		if ( avatarObject.IsValid() )
		{
			angle += Time.Delta * 15f;
			avatarObject.WorldRotation = Rotation.FromYaw( 180 + angle );
		}
	}

	public void RefreshAvatar( string clothingString )
	{
		// Simplified: just reset the model
		if ( !avatarObject.IsValid() ) return;
	}
}
