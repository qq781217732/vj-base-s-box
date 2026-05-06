namespace ZombieHorde;

public enum EncounterType
{
	AmbientPatrol,
	AmbientWanderPack,
	TriggeredAmbush,
	ObjectiveDefenseWave,
	DynamicReinforcement,
	PursuitPack
}

public enum EncounterPhase
{
	Pending,
	Spawning,
	Active,
	Searching,
	Resolved,
	Cooldown
}
