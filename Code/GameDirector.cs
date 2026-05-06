using System;
using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Game Director" )]
public partial class GameDirector : Component
{
	[ConVar( "replicated" )]
	public static bool zom_disabledirector { get; set; }
	public TimeSince TimeSinceSpawnedZombie { get; set; }

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		if ( zom_disabledirector ) return;
		if ( ZombieNetworkManager.zom_encounter_mode ) return;

		PopulateZombies();
	}

	[ConCmd( "server" )]
	public static void zom_debug_spawnzombie()
	{
		Game.ActiveScene.GetAllComponents<GameDirector>().FirstOrDefault()?.SpawnZombie();
	}

	private void PopulateZombies()
	{
		var playerCount = Game.ActiveScene.GetAllComponents<HumanPlayer>().Count();
		var difficultyMultiplier = 0.5f + playerCount * 0.5f;
		var zombieMultiplier = 0.75f + playerCount * 0.25f;
		var zombieCount = Game.ActiveScene.GetAllComponents<BaseZombie>().Count();
		var currentWave = (BaseGamemode.Current as SurvivalGamemode)?.WaveNumber + 1 ?? 1;
		var maxZombies = BaseGamemode.Current?.ZomMaxZombies ?? 5;
		if ( (BaseGamemode.Current as SurvivalGamemode)?.RoundState != RoundState.WaveActive )
			maxZombies *= 0.5f;

		var spawnRate = 1 / (BaseGamemode.Current?.ZomSpawnRate ?? 1) / difficultyMultiplier;
		if ( zombieCount > 3 * difficultyMultiplier )
			spawnRate *= 2;

		if ( TimeSinceSpawnedZombie > spawnRate )
		{
			if ( zombieCount < (maxZombies * zombieMultiplier).Clamp( 0, 20 ) )
			{
				SpawnZombie();
				TimeSinceSpawnedZombie = 0 - Game.Random.NextFloat( 1f );

				if ( currentWave > 15 )
					TimeSinceSpawnedZombie = 0 - Game.Random.NextFloat( 0.5f );

				if ( currentWave > 18 )
					TimeSinceSpawnedZombie = 0;
			}
		}

		// Chance to spawn a group if there are very few zombies
		if ( zombieCount < 3 && currentWave >= 3 )
		{
			if ( Game.Random.NextInt( 300 ) == 1 )
			{
				var spawnedCount = 0;
				for ( int i = 0; i < 2 + playerCount; i++ )
				{
					var target = Game.ActiveScene.GetAllComponents<HumanPlayer>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();
					if ( target != null )
					{
						SpawnZombie( target );
						spawnedCount++;
					}
				}
				Log.Info( "Spawned Group of " + spawnedCount );
				TimeSinceSpawnedZombie = 0;
			}
		}
	}

	private int ZombieSpawnFails = 0;

	public BaseZombie SpawnZombie( HumanPlayer ply = null )
	{
		var spawnPos = WorldPosition;
		var tries = 0;
		var maxTries = 30;
		var maxRange = 3000;

		if ( ply == null )
		{
			var players = Game.ActiveScene.GetAllComponents<HumanPlayer>().ToList();
			ply = players.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();
			if ( ply == null ) return null;
			if ( !ply.IsAlive )
				ply = players.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();
		}
		if ( ply == null ) return null;

		var minRadius = 1000;

		// Check if player is in an AllowCommonZombieSpawn zone
		var allowTr = Game.ActiveScene.Trace.Ray( ply.WorldPosition, ply.WorldPosition + Vector3.Up * 10 )
			.WithTag( "AllowCommonZombieSpawn" )
			.Radius( 500 )
			.Run();
		if ( allowTr.Hit )
			minRadius = 500;

		while ( tries <= maxTries )
		{
			tries += 1;
			var t = Game.ActiveScene.NavMesh?.GetRandomPoint( ply.WorldPosition, maxRange );
			if ( t.HasValue )
			{
				spawnPos = t.Value;
				if ( spawnPos.Length > 30000 ) return null;

				var blockTr = Game.ActiveScene.Trace.Ray( spawnPos, spawnPos + Vector3.Up * 20 )
					.WithTag( "BlockCommonZombieSpawn" )
					.Radius( 20 )
					.Run();
				if ( blockTr.Hit )
					continue;

				var allowSpawnTr = Game.ActiveScene.Trace.Ray( spawnPos, spawnPos + Vector3.Up * 20 )
					.WithTag( "AllowCommonZombieSpawn" )
					.Radius( 20 )
					.Run();
				if ( allowSpawnTr.Hit )
					break; // Skip LOS trace

				var playerProxTr = Game.ActiveScene.Trace.Ray( spawnPos, spawnPos + Vector3.Up * 20 )
					.WithTag( "player" )
					.Radius( 500 )
					.Run();
				if ( playerProxTr.Hit )
					continue;

				// LOS check against all players
				var blocked = false;
				var addHeight = new Vector3( 0, 0, 70 );
				foreach ( var otherPly in Game.ActiveScene.GetAllComponents<HumanPlayer>() )
				{
					var eyePos = otherPly.WorldPosition + Vector3.Up * 64;
					var tr = Game.ActiveScene.Trace.Ray( spawnPos + addHeight, eyePos )
						.WithoutTags( "trigger" )
						.Run();

					if ( Vector3.DistanceBetween( tr.EndPosition, eyePos ) < 20 )
					{
						blocked = true;
						break;
					}
				}

				if ( !blocked )
					break;
			}
		}

		if ( tries >= maxTries )
		{
			var foundSpawn = false;
			foreach ( var blocker in Game.ActiveScene.GetAllComponents<HammerSpawnBlocker>() )
			{
				if ( blocker.Tags.Has( "AllowCommonZombieSpawn" ) )
				{
					spawnPos = blocker.WorldPosition;
					foundSpawn = true;
					break;
				}
			}
			if ( !foundSpawn )
			{
				Log.Warning( "Can't Find Valid Zombie Spawn" );
				ZombieSpawnFails += 1;

				if ( ZombieSpawnFails > 10 )
					Log.Error( "Can't spawn zombies! Map doesn't have a navmesh or is too small." );
				return null;
			}
		}

		BaseZombie npc = null;
		var currentWave = (BaseGamemode.Current as SurvivalGamemode)?.WaveNumber + 1 ?? 1;

		// Spawn uncommon (armored) zombies from wave 5 onward
		if ( currentWave > 4 )
		{
			if ( Game.Random.NextInt( 5 + (30 / Math.Max( currentWave, 1 )) ) == 0 )
			{
				var go = new GameObject( true, "UncommonZombie" );
				go.WorldPosition = spawnPos;
				go.Components.Create<NavMeshAgent>(); go.Components.Create<SkinnedModelRenderer>().Model = Model.Load("models/citizen/citizen.vmdl"); go.NetworkSpawn(); npc = go.Components.Create<UncommonZombie>();
				Log.Info( "spawning uncommon zombie!" );
			}
		}

		// Default to common zombie
		if ( npc == null )
		{
			var go = new GameObject( true, "CommonZombie" );
			go.WorldPosition = spawnPos;
			go.Components.Create<NavMeshAgent>(); go.Components.Create<SkinnedModelRenderer>().Model = Model.Load("models/citizen/citizen.vmdl"); go.NetworkSpawn(); npc = go.Components.Create<CommonZombie>();
		}

		if ( BaseGamemode.Current != null && BaseGamemode.Current.PopulateZombiesAngry() )
		{
			((CommonZombie)npc).StartChase( ply.GameObject );
		}

		Log.Info( "Spawned Zombie. Population: " + Game.ActiveScene.GetAllComponents<BaseZombie>().Count() );

		ZombieSpawnFails = 0;
		return npc;
	}
}
