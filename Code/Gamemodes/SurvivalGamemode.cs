using SWB.Shared;
using System;

namespace ZombieHorde;

/// <summary>
/// Survival wave-based gamemode. Players fight through escalating zombie waves.
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Survival Gamemode" )]
public partial class SurvivalGamemode : BaseGamemode
{
	[Sync] public TimeUntil TimeUntilNextState { get; set; }
	public int WaveNumber { get; set; }
	public RoundState RoundState { get; set; }

	[ConCmd]
	public static void zom_skipround()
	{
		Log.Info( "Skipping round!" );
		var gm = Current as SurvivalGamemode;
		if ( gm is null ) return;
		gm.ZombiesRemaining = 0;
		gm.TimeUntilNextState = 0;
	}

	protected override void OnStart()
	{
		Log.Info( "Survival gamemode active!" );
		RoundState = RoundState.PreGame;
		TimeUntilNextState = 60;
		HumanMaxRevives = 1;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		UpdateRoundDisplay();

		switch ( RoundState )
		{
			case RoundState.PreGame:
				if ( TimeUntilNextState <= 0 ) StartWave();
				break;
			case RoundState.WaveActive:
				if ( ZombiesRemaining <= 0 ) StartIntermission();
				if ( GetLivePlayerCount() <= 0 ) StartPostGame();
				break;
			case RoundState.Intermission:
				if ( TimeUntilNextState <= 0 ) StartWave();
				break;
			case RoundState.PostGame:
				if ( TimeUntilNextState <= 0 ) RestartGame();
				break;
		}
	}

	void UpdateRoundDisplay()
	{
		var roundAmount = TimeUntilNextState > 10 ? 0 : 1;
		var roundedAmount = Math.Round( (double)TimeUntilNextState, roundAmount );
		var suffix = roundedAmount < 10 && roundedAmount % 1 == 0 ? ".0s" : "s";
		RoundInfo = roundedAmount + suffix;

		switch ( RoundState )
		{
			case RoundState.PreGame:
				RoundName = "Get Ready to Survive the Horde!";
				RoundInfo = "Wave coming in " + RoundInfo;
				break;
			case RoundState.WaveActive:
				RoundName = "Wave " + WaveNumber;
				RoundInfo = ZombiesRemaining + " Remain";
				break;
			case RoundState.Intermission:
				RoundName = "Intermission";
				RoundInfo = $"Wave {WaveNumber + 1} coming in " + RoundInfo;
				break;
			case RoundState.PostGame:
				RoundName = "Game over! Waves Survived: " + (WaveNumber - 1);
				break;
		}
	}

	public void StartWave()
	{
		if ( !IsProxy ) Sound.Play( "wave.start" );

		WaveNumber++;

		// Notify wave trackers
		foreach ( var hammerEnt in Game.ActiveScene.GetAllComponents<HammerWaveTracker>() )
			hammerEnt.WaveStart();

		var playerCount = GetPlayerCount();
		var difficultyMultiplier = 0.5f + playerCount * 0.5f;

		ZombiesRemaining += 10 + (int)(3 * (WaveNumber - 1) * difficultyMultiplier);
		var minZombies = (int)(5 * difficultyMultiplier);
		if ( ZombiesRemaining < minZombies ) ZombiesRemaining = minZombies;

		RoundState = RoundState.WaveActive;

		// Anger all zombies
		foreach ( var npc in Game.ActiveScene.GetAllComponents<CommonZombie>() )
			npc.StartChase();
	}

	public void StartIntermission()
	{
		if ( !IsProxy ) Sound.Play( "wave.end" );

		TimeUntilNextState = 40;
		RoundState = RoundState.Intermission;

		foreach ( var hammerEnt in Game.ActiveScene.GetAllComponents<HammerWaveTracker>() )
			hammerEnt.WaveEnd();

		UpdateZombieStats();

		if ( IsProxy ) return;

		// Revive all incapacitated players
		foreach ( var ply in Game.ActiveScene.GetAllComponents<HumanPlayer>() )
		{
			ply.RevivesRemaining = HumanMaxRevives;
			if ( ply.Health >= 200 && !ply.IsAlive )
				ply.Revive();
		}

		// Kill angry zombies
		foreach ( var zom in Game.ActiveScene.GetAllComponents<CommonZombie>() )
		{
			zom.Agent?.Stop();
			Sound.Play( "rust_pumpshotgun.shootdouble", zom.WorldPosition );
			var dmgInfo = new Sandbox.DamageInfo
			{
				Damage = 10000,
				Origin = zom.WorldPosition,
			};
			zom.OnDamage( dmgInfo );
		}

		ZombiesRemaining = 0;

		// Spawn loot boxes
		foreach ( var ply in Game.ActiveScene.GetAllComponents<HumanPlayer>() )
			SpawnLootbox( ply );
	}

	public void SpawnLootbox( HumanPlayer ply )
	{
		var minRadius = 1000f;

		// Check if player is in a valid spawn zone
		var testTr = Game.ActiveScene.Trace.Ray( ply.WorldPosition, ply.WorldPosition + Vector3.Up * 10 )
			.WithTag( "AllowLootBoxSpawn" )
			.Radius( 500 )
			.Run();

		if ( testTr.Hit )
			minRadius = 10;

		bool spawnedBox = false;
		for ( int i = 0; i < 30; i++ )
		{
			var t = Game.ActiveScene.NavMesh.GetRandomPoint( ply.WorldPosition, 4000 );
			if ( t.HasValue && t.Value.Length < 30000 )
			{
				var spawnPos = t.Value;

				var blockTr = Game.ActiveScene.Trace.Ray( spawnPos, spawnPos + Vector3.Up * 20 )
					.WithTag( "BlockLootBoxSpawn" )
					.Radius( 20 )
					.Run();

				if ( blockTr.Hit )
				{
					Log.Info( "lootbox spawn blocked! Tries: " + i );
					continue;
				}

				var box = new GameObject( true, "LootBox" );
				box.WorldPosition = spawnPos;
				box.Components.Create<LootBox>();
				box.NetworkSpawn();
				spawnedBox = true;
				break;
			}
		}

		if ( !spawnedBox )
		{
			// Fallback: spawn on a spawn blocker
			foreach ( var blocker in Game.ActiveScene.GetAllComponents<HammerSpawnBlocker>() )
			{
				if ( blocker.Tags.Has( "AllowLootBoxSpawn" ) )
				{
					var box = new GameObject( true, "LootBox" );
					box.WorldPosition = blocker.WorldPosition;
					box.Components.Create<LootBox>();
					box.NetworkSpawn();
					break;
				}
			}
		}
	}

	public void StartPostGame()
	{
		TimeUntilNextState = 20;
		RoundState = RoundState.PostGame;

		foreach ( var hammerEnt in Game.ActiveScene.GetAllComponents<HammerWaveTracker>() )
			hammerEnt.GameEnd();

		if ( IsProxy ) return;

		foreach ( var ply in Game.ActiveScene.GetAllComponents<HumanPlayer>() )
		{
			ply.Kill();
		}
	}

	public void RestartGame()
	{
		if ( IsProxy ) return;

		Sound.Play( "bell" );

		// Kill all players
		foreach ( var ply in Game.ActiveScene.GetAllComponents<HumanPlayer>() )
			ply.Kill();

		// Destroy all zombies
		foreach ( var npc in Game.ActiveScene.GetAllComponents<BaseZombie>() )
			npc.GameObject.Destroy();

		// Clear world items
		ClearWorldItems();

		// Reset map - cleanup entities
		MapResetCleanup();

		WaveNumber = 0;
		ZombiesRemaining = 0;
		TimeUntilNextState = 60;
		RoundState = RoundState.PreGame;

		UpdateZombieStats();
	}

	void ClearWorldItems()
	{
		var types = new System.Type[] {
			typeof(BaseZomWeapon), typeof(LootBox), typeof(AmmoPile),
			typeof(HealthKit), typeof(Tripmine), typeof(Flames), typeof(PingMarker)
		};

		foreach ( var t in types )
		{
			foreach ( var comp in Game.ActiveScene.GetAllComponents( t ) )
			{
				comp.GameObject.Destroy();
			}
		}
	}

	void MapResetCleanup()
	{
		// Disable nav blockers on static props
		// In the new API, this needs to be handled differently
	}

	public void UpdateZombieStats()
	{
		var w = WaveNumber + 1;
		ZomMaxZombies = 5;

		switch ( w )
		{
			case 0:
			case 1:
				ZomHealthMultiplier = 1; ZomSpeedMultiplier = 1; ZomSpawnRate = 1; ZomMaxZombies = 5; break;
			case 2:
				ZomSpeedMultiplier = 1.25f; ZomMaxZombies = 7; break;
			case 3:
				ZomSpeedMultiplier = 1.5f; ZomSpawnRate = 1.5f; ZomMaxZombies = 6; break;
			case 4:
				ZomMaxZombies = 7; break;
			case 5:
				ZomSpeedMultiplier = 1.75f; break;
			case 6:
				ZomSpawnRate = 1.8f; ZomMaxZombies = 10; break;
			case 10:
				ZomMaxZombies = 11; break;
			case 13:
				ZomSpawnRate = 2f; break;
			case 14:
				ZomSpawnRate = 3f; break;
			case 15:
				ZomSpawnRate = 1000f; break;
			case 16:
				ZomMaxZombies = 12; break;
			default:
				ZomSpeedMultiplier += 0.005f;
				ZomHealthMultiplier += 0.02f;
				break;
		}
		ZomMaxZombies = ZomMaxZombies.Clamp( 5, 25 );
	}

	public override bool EnableRespawning()
	{
		return RoundState == RoundState.PreGame || RoundState == RoundState.Intermission;
	}

	public override bool PopulateZombiesAngry()
	{
		return RoundState == RoundState.WaveActive;
	}
}

public enum RoundState
{
	PreGame,
	WaveActive,
	Intermission,
	PostGame
}
