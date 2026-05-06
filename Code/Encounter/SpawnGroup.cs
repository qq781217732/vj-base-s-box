using Sandbox.Navigation;
using SWB.Shared;
using System;

namespace ZombieHorde;

public class SpawnGroupEntry
{
	public System.Type EnemyType { get; set; } = typeof(CommonZombie);
	public PrefabScene EnemyPrefab { get; set; }
	public int Count { get; set; } = 1;

	public BaseZombie CreateEnemy( Vector3 spawnPos )
	{
		GameObject go;
		if ( EnemyPrefab is not null )
		{
			go = EnemyPrefab.Clone();
			go.WorldPosition = spawnPos;
		}
		else
		{
			go = new GameObject( true, EnemyType.Name );
			go.WorldPosition = spawnPos;
			go.Components.Create<NavMeshAgent>();
			go.Components.Create<SkinnedModelRenderer>().Model = Model.Load( "models/citizen/citizen.vmdl" );
		}
		go.Name = EnemyType.Name;
		go.NetworkSpawn();
		return go.Components.Create( TypeLibrary.GetType( EnemyType.FullName ) ) as BaseZombie;
	}
}

public class SpawnGroup
{
	public string GroupName { get; set; } = "Unnamed";
	public int TotalCost { get; set; } = 1;
	public List<SpawnGroupEntry> Entries { get; set; } = new();
	public float SpawnInterval { get; set; } = 0.5f;
	public string DefaultTask { get; set; } = "Wander";

	public static float MinSeparation = 80f;
	public static float InnerRadius = 200f;
	public static float OuterRadius = 500f;

	public static SpawnGroup FromTemplate( SpawnGroupTemplate template )
		{
			var group = new SpawnGroup
			{
				GroupName = template.Name,
				TotalCost = template.TotalCost,
				DefaultTask = template.DefaultTask
			};
			foreach ( var entry in template.Entries )
			{
				var enemyType = TypeLibrary.GetType( entry.EnemyTypeName )?.TargetType ?? typeof(CommonZombie);
				group.Entries.Add( new SpawnGroupEntry { EnemyType = enemyType, EnemyPrefab = entry.EnemyPrefab, Count = entry.Count } );
			}
			return group;
		}

		public List<BaseZombie> Spawn( Vector3 origin, GameObject forceTarget = null )
	{
		Log.Info( string.Concat( "[Spawn] Group=", GroupName, " count=", Entries.Sum(e => e.Count), " origin=", origin ) );
		var spawned = new List<BaseZombie>();
		var usedPositions = new List<Vector3>();

		foreach ( var entry in Entries )
		{
			for ( int i = 0; i < entry.Count; i++ )
			{
				var pos = FindSpreadPosition( origin, usedPositions );
				var enemy = entry.CreateEnemy( pos );
				if ( enemy is null ) continue;

				usedPositions.Add( pos );

				if ( forceTarget.IsValid() && enemy.Brain != null )
					enemy.Brain.ForceEngage( forceTarget );

				spawned.Add( enemy );
			}
		}

		return spawned;
	}

	static Vector3 FindSpreadPosition( Vector3 origin, List<Vector3> existing )
	{
		var navMesh = Game.ActiveScene.NavMesh;

		for ( int tries = 0; tries < 20; tries++ )
		{
			var angle = Game.Random.NextFloat( 0, MathF.PI * 2 );
			var dist = Game.Random.NextFloat( InnerRadius, OuterRadius );
			var pos = origin + new Vector3( MathF.Cos( angle ) * dist, MathF.Sin( angle ) * dist, 0 );

			var navPt = navMesh.GetClosestPoint( pos );
			if ( !navPt.HasValue ) continue;

			if ( TryValidateOrPushOut( navPt.Value, origin, out var validPt ) )
			{
				if ( !existing.Any( p => (p - validPt).Length < MinSeparation ) )
					return validPt;
			}
		}

		var fallback = origin + new Vector3( Game.Random.NextFloat( -300, 300 ), Game.Random.NextFloat( -300, 300 ), 0 );
		var fbNav = navMesh.GetClosestPoint( fallback );
		return fbNav ?? fallback;
	}

	/// <summary>
	/// If the candidate passes marker + path checks, use it directly.
	/// Otherwise push toward origin (off the island edge) up to 3 times.
	/// </summary>
	static bool TryValidateOrPushOut( Vector3 candidate, Vector3 origin, out Vector3 result )
	{
		if ( NavWalkableRegion.IsSpawnValid( candidate ) && IsPathConnected( candidate, origin ) )
		{
			result = candidate;
			return true;
		}

		var navMesh = Game.ActiveScene.NavMesh;
		var pushDir = (origin - candidate).Normal;

		for ( int push = 0; push < 3; push++ )
		{
			var pushed = candidate + pushDir * (200f + push * 150f);
			var pushedNav = navMesh.GetClosestPoint( pushed );
			if ( !pushedNav.HasValue ) continue;

			if ( NavWalkableRegion.IsSpawnValid( pushedNav.Value ) && IsPathConnected( pushedNav.Value, origin ) )
			{
				result = pushedNav.Value;
				return true;
			}
		}

		result = default;
		return false;
	}

	static bool IsPathConnected( Vector3 from, Vector3 to )
	{
		var path = Game.ActiveScene.NavMesh.CalculatePath( new CalculatePathRequest
		{
			Start = from,
			Target = to
		} );
		return path.IsValid;
	}
}
