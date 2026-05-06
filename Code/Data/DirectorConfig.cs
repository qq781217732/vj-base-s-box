namespace ZombieHorde;

public class DirectorConfig
{
	public int GlobalMaxEnemies { get; set; } = 30;
	public int MaxActiveEncounters { get; set; } = 8;
	public int MaxActiveZones { get; set; } = 3;
	public float AmbientRefreshInterval { get; set; } = 5.0f;
	public float PlayerCountMultiplier { get; set; } = 1.0f;
	public float PressureMax { get; set; } = 100f;
	public float HighPressureThreshold { get; set; } = 70f;
	public float CrossZonePursuitThreshold { get; set; } = 60f;
	public float PressureDecayRate { get; set; } = 1f;
	public float PressureAccumulationRate { get; set; } = 0.5f;
	public float SpecialEnemyWeightLow { get; set; } = 0.2f;
	public float SpecialEnemyWeightHigh { get; set; } = 0.4f;
}
