namespace ZombieHorde;
using Sandbox;

public sealed class MyComponent : Component
{
	[Property] public string SequenceName { get; set; } = "flinch_rightLeg";

	[Property] public bool PlayOnStart { get; set; } = false;

	bool _playing;
	bool _playStarted;  // true once DP.TimeNormalized > 0 (animation actually running)

	protected override void OnStart()
	{
		if ( PlayOnStart && !string.IsNullOrEmpty( SequenceName ) )
			PlayAnim( SequenceName );
	}

	protected override void OnUpdate()
	{
		var renderer = Components.Get<SkinnedModelRenderer>();
		if ( renderer == null ) return;

		// Check if current direct playback has finished
		if ( _playing )
		{
			var dp = renderer.SceneModel?.DirectPlayback;
			Log.Info( dp.TimeNormalized);
			// TimeNormalized: 0~1, 0.99 = animation almost done
			if ( dp != null && dp.TimeNormalized >= 0.9f )
			{
				_playing = false;
				renderer.Set( "is_flinching", false );
				Log.Info( "Flinch finished" );
			}
		}

		// Press 1~9 to test animations
		var input = "";
		if ( Input.Pressed( "Slot1" ) ) input = "flinch_head";
		if ( Input.Pressed( "Slot2" ) ) input = "flinch_chest";
		if ( Input.Pressed( "Slot3" ) ) input = "flinch_leftArm";
		if ( Input.Pressed( "Slot4" ) ) input = "flinch_rightArm";
		if ( Input.Pressed( "Slot5" ) ) input = "flinch_leftLeg";
		if ( Input.Pressed( "Slot6" ) ) input = "flinch_rightLeg";
		if ( Input.Pressed( "Slot7" ) ) input = "attackA";
		if ( Input.Pressed( "Slot8" ) ) input = "physflinch1";
		if ( Input.Pressed( "Slot9" ) ) input = "Idle01";

		if ( !string.IsNullOrEmpty( input ) )
			PlayAnim( input );
	}

	void PlayAnim( string sequenceName )
	{
		var renderer = Components.Get<SkinnedModelRenderer>();
		if ( renderer == null || renderer.SceneModel == null )
		{
			Log.Warning( "No SkinnedModelRenderer found on this GameObject" );
			return;
		}

		// Trigger the flinch state
		renderer.Set( "is_flinching", true );

		// Play the sequence via DirectPlayback
		renderer.SceneModel.DirectPlayback.Play( sequenceName );

		_playing = true;

		Log.Info( $"Playing: {sequenceName}" );
	}
}