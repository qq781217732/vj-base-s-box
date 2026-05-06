namespace ZombieHorde;

/// <summary>
/// Fires outputs when waves start/end
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Wave Tracker" )]
public partial class HammerWaveTracker : Component
{
	/// <summary>
	/// Outputs will be fired when this wave is reached
	/// </summary>
	[Property]
	public int TargetWave { get; set; } = 1;

	/// <summary>
	/// Fired when any wave ends
	/// </summary>
	// TODO: Output type doesn't exist in new API
	//protected Output OnWaveEnd { get; set; }

	/// <summary>
	/// Fired when the specified wave ends
	/// </summary>
	// TODO: Output type doesn't exist in new API
	//protected Output OnTargetWaveEnd { get; set; }

	public void WaveEnd()
	{
		// TODO: Output type doesn't exist in new API
		//OnWaveEnd.Fire( this );
		//if ( (BaseGamemode.Current as SurvivalGamemode)?.WaveNumber == TargetWave )
		//	OnTargetWaveEnd.Fire( this );
	}

	/// <summary>
	/// Fired when any wave starts
	/// </summary>
	// TODO: Output type doesn't exist in new API
	//protected Output OnWaveStart { get; set; }

	/// <summary>
	/// Fired when the specified wave starts
	/// </summary>
	// TODO: Output type doesn't exist in new API
	//protected Output OnTargetWaveStart { get; set; }

	public void WaveStart()
	{
		// TODO: Output type doesn't exist in new API
		//OnWaveStart.Fire( this );
		//if ( (BaseGamemode.Current as SurvivalGamemode)?.WaveNumber == TargetWave )
		//	OnTargetWaveStart.Fire( this );
	}

	/// <summary>
	/// Fired when the game ends, at the start of the "Victory" or "Defeat" screen.
	/// </summary>
	// TODO: Output type doesn't exist in new API
	//protected Output OnGameEnd { get; set; }

	public void GameEnd()
	{
		// TODO: Output type doesn't exist in new API
		//OnGameEnd.Fire( this );
	}
}
