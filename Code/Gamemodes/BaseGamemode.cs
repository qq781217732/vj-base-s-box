using SWB.Shared;

namespace ZombieHorde;

/// <summary>
/// Base gamemode for Zombie Horde. Manages round state, zombie stats,
/// and provides hooks for derived gamemodes (Survival, Encounter).
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Base Gamemode" )]
public partial class BaseGamemode : Component
{
	public static BaseGamemode Current { get; set; }

	[Sync] public int ZombiesRemaining { get; set; }
	[Sync] public string RoundInfo { get; set; } = "unknown";
	[Sync] public string RoundName { get; set; } = "unknown";
	[Sync] public int HumanMaxRevives { get; set; } = 3;

	public float ZomHealthMultiplier { get; set; } = 1;
	public float ZomSpeedMultiplier { get; set; } = 1;
	public float ZomSpawnRate { get; set; } = 1;
	public float ZomMaxZombies { get; set; } = 5;

	[ConVar( "server" )]
	public static bool zom_debug_round_info { get; set; }

	protected override void OnAwake()
	{
		Current = this;
	}

	protected override void OnUpdate()
	{
		if ( !zom_debug_round_info ) return;

		var i = 9;
		DebugOverlay.ScreenText( new Vector2( 0, 300 ), $"HealthMultiplier: {ZomHealthMultiplier}" );
		DebugOverlay.ScreenText( new Vector2( 0, 320 ), $"SpeedMultiplier: {ZomSpeedMultiplier}" );
		DebugOverlay.ScreenText( new Vector2( 0, 340 ), $"SpawnRate: {1 / ZomSpawnRate}" );
		DebugOverlay.ScreenText( new Vector2( 0, 360 ), $"Zombies: {GetZombieCount()}/{ZomMaxZombies}" );
	}

	public int GetLivePlayerCount()
	{
		var count = 0;
		foreach ( var ply in Game.ActiveScene.GetAllComponents<HumanPlayer>() )
		{
			if ( ply.IsAlive ) count++;
		}
		return count;
	}

	public int GetZombieCount()
	{
		return Game.ActiveScene.GetAllComponents<BaseZombie>().Count();
	}

	public int GetPlayerCount()
	{
		return Game.ActiveScene.GetAllComponents<HumanPlayer>().Count();
	}

	public virtual bool EnableRespawning()
	{
		return true;
	}

	public virtual bool PopulateZombiesAngry()
	{
		return false;
	}
}
