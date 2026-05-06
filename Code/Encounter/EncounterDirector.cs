using System.Collections.Generic;
using System.Linq;
using System;
using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Encounter Director" )]
public partial class EncounterDirector : Component
{
		[Property] public EncounterConfig ConfigAsset { get; set; }
	public DirectorConfig Config { get; set; } = new();
	public List<EncounterZone> Zones { get; set; } = new();
	public float GlobalPressure { get; set; } = 0f;

	private TimeSince _timeSinceAmbientCheck = 0;
	private TimeSince _timeSinceReinforceCheck = 0;
	private TimeSince _timeSincePressureCheck = 0;
	private int _nextPatrolGroupId = 1;
	private int _lastEnemyCount = 0;
	private List<ObjectiveInstance> _activeObjectives = new();
	private bool _firstSpawn = true;

	protected override void OnStart()
	{
		NoiseSystem.Initialize();
		Config = new DirectorConfig();
			ApplyConfig();

		if ( !TryLoadZonesFromEntities() )
		{
			Log.Warning( "No EncounterZoneVolume entities found, using test zones" );
			CreateTestZones();
		}

		Log.Info( $"EncounterDirector ready with {Zones.Count} zones" );
		foreach ( var zone in Zones )
		{
			Log.Info( $"  Zone '{zone.ZoneName}': {zone.SpawnSources.Count} sources, danger={zone.DangerLevel}, ambientBudget={zone.AmbientBudget}" );
		}
	}

	private bool TryLoadZonesFromEntities()
	{
		var zoneVolumes = Game.ActiveScene.GetAllComponents<EncounterZoneVolume>().ToList();
		if ( zoneVolumes.Count == 0 )
			return false;

		foreach ( var vol in zoneVolumes )
			Zones.Add( vol.ToEncounterZone() );

		var spawnSourceEnts = Game.ActiveScene.GetAllComponents<EncounterSpawnSource>().ToList();
		foreach ( var srcEnt in spawnSourceEnts )
		{
			var src = srcEnt.ToSpawnSource();
			foreach ( var zone in Zones )
			{
				if ( zone.IsPointInBounds( src.Position ) )
				{
					zone.RegisterSpawnSource( src );
					break;
				}
			}
		}

		Log.Info( $"Loaded {Zones.Count} zones and {spawnSourceEnts.Count} spawn sources from map entities" );
		return true;
	}

	public void AddPressure( float amount )
	{
		GlobalPressure = Math.Min( GlobalPressure + amount, Config.PressureMax );
	}

	[ConCmd]
	public static void zom_trigger_objective()
	{
		var director = Game.ActiveScene.GetAllComponents<EncounterDirector>().FirstOrDefault();
		if ( director == null )
		{
			Log.Warning( "No EncounterDirector found" );
			return;
		}

		var player = Game.ActiveScene.GetAllComponents<HumanPlayer>()
			.Where( p => p.IsAlive )
			.FirstOrDefault();

		if ( player == null )
			return;

		var zone = director.Zones.FirstOrDefault( z => z.IsPointInBounds( player.WorldPosition ) );
		if ( zone == null )
		{
			Log.Warning( "Player not in any zone" );
			return;
		}

		director.StartObjectiveWave( zone, player );
	}

	private void StartObjectiveWave( EncounterZone zone, HumanPlayer player )
	{
		zone.State = ZoneState.Combat;
		Log.Info( $"Objective wave triggered in zone '{zone.ZoneName}'" );

		NoiseSystem.Emit( player.WorldPosition, 3000f, 1f, NoiseType.TaskDevice );
		AddPressure( 10f );

		var group1 = GetSpawnGroup("WanderPack");
		var available = zone.GetAvailableSources( EncounterType.ObjectiveDefenseWave );
		if ( available.Count == 0 )
			available = zone.SpawnSources;

		var source = available[Game.Random.NextInt( available.Count )];

		if ( group1 is null ) return;
		var spawned = group1.Spawn( source.Position, player.GameObject );

		var encounter = new EncounterInstance
		{
			Type = EncounterType.ObjectiveDefenseWave,
			Phase = EncounterPhase.Active,
			OwningZone = zone,
			AssignedGroup = group1,
			SpawnedEnemies = spawned,
			SpawnPosition = source.Position,
			TotalWaveCount = 3,
			CurrentWave = 1,
			WaveInterval = 10f,
			TimeSinceWaveStart = 0,
			NextWaveGroup = GetSpawnGroup("ChargerWave"),
			ForceTarget = player.GameObject
		};

		foreach ( var zombie in spawned )
		{
			zombie.OwningEncounter = encounter;
			zombie.PatrolGroupId = _nextPatrolGroupId;
		}

		zone.ActiveEncounters.Add( encounter );
		_nextPatrolGroupId++;

		Log.Info( $"Objective wave 1/3: +{spawned.Count} enemies in '{zone.ZoneName}'" );
	}

	static HashSet<string> _warnedTemplates = new();

	public SpawnGroup GetSpawnGroup( string name )
	{
		var template = ConfigAsset?.SpawnGroupTemplates?.FirstOrDefault( t => t.Name == name );
		if ( template is not null )
			return SpawnGroup.FromTemplate( template );

		if ( _warnedTemplates.Add( name ) )
			Log.Info( $"No config template '{name}' — spawn skipped" );
		return null;
	}

	private void CreateTestZones()
	{
		var mapOrigin = Vector3.Zero;

		var spawnPoints = Game.ActiveScene.GetAllComponents<SpawnPoint>().ToList();
		if ( spawnPoints.Count > 0 )
			mapOrigin = spawnPoints[0].WorldPosition;

		var zone1 = new EncounterZone
		{
			ZoneName = "Central",
			Bounds = new BBox( mapOrigin - new Vector3( 2000, 2000, 500 ), mapOrigin + new Vector3( 2000, 2000, 500 ) ),
			State = ZoneState.Ambient,
			AmbientBudget = 5,
			CombatBudget = 12,
			DangerLevel = 2
		};
		GenerateSpawnSources( zone1, 6 );
		Zones.Add( zone1 );

		var offset2 = new Vector3( 2500, 0, 0 );
		var zone2 = new EncounterZone
		{
			ZoneName = "East",
			Bounds = new BBox( mapOrigin + offset2 - new Vector3( 1500, 1500, 500 ), mapOrigin + offset2 + new Vector3( 1500, 1500, 500 ) ),
			State = ZoneState.Ambient,
			AmbientBudget = 4,
			CombatBudget = 10,
			DangerLevel = 1
		};
		GenerateSpawnSources( zone2, 4 );
		Zones.Add( zone2 );

		var offset3 = new Vector3( -2500, 0, 0 );
		var zone3 = new EncounterZone
		{
			ZoneName = "West",
			Bounds = new BBox( mapOrigin + offset3 - new Vector3( 1500, 1500, 500 ), mapOrigin + offset3 + new Vector3( 1500, 1500, 500 ) ),
			State = ZoneState.Ambient,
			AmbientBudget = 4,
			CombatBudget = 10,
			DangerLevel = 1
		};
		GenerateSpawnSources( zone3, 4 );
		Zones.Add( zone3 );
	}

	private void GenerateSpawnSources( EncounterZone zone, int count )
	{
		var center = (zone.Bounds.Mins + zone.Bounds.Maxs) * 0.5f;

		for ( int i = 0; i < count; i++ )
		{
			for ( int attempt = 0; attempt < 8; attempt++ )
			{
				var t = Game.ActiveScene.NavMesh?.GetRandomPoint( center, 2000 );
				if ( !t.HasValue )
					continue;

				var spawnPos = t.Value;

				if ( !zone.IsPointInBounds( spawnPos ) )
					continue;

				if ( zone.SpawnSources.Any( s => (s.Position - spawnPos).Length < 400 ) )
					continue;

				var source = new SpawnSource
				{
					Position = spawnPos,
					SourceType = i < count / 2 ? SpawnSourceType.HiddenInterior : SpawnSourceType.EdgeEntry
				};

				source.Rules.Add( new SpawnRule
				{
					MinPlayerDistance = 400f,
					MaxPlayerDistance = 6000f,
					RequireOutOfSight = true
				} );

				zone.RegisterSpawnSource( source );
				break;
			}
		}
	}
	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		if ( Zones.Count == 0 ) return;
		// Force first spawn after 3 seconds
		if ( _firstSpawn && _timeSinceAmbientCheck > 3f ) { _firstSpawn = false; ForceAmbientSpawn(); }
		UpdatePlayerZonePresence();
		UpdatePressure();
		UpdateZoneStates();
		CleanupResolvedEncounters();
		UpdateEncounterWaves();
			TickObjectives();
		if ( _timeSinceAmbientCheck > Config.AmbientRefreshInterval ) { _timeSinceAmbientCheck = 0; RefreshAmbientEncounters(); }
			_timeSinceAmbientCheck = 0;

			RefreshAmbientEncounters();
		{
			_timeSinceReinforceCheck = 0;
			RefreshReinforcements();
		}
	}

	private void UpdatePlayerZonePresence()
	{
		foreach ( var zone in Zones )
		{
			zone.PlayerCount = Game.ActiveScene.GetAllComponents<HumanPlayer>()
				.Count( p => p.IsAlive && zone.IsPointInBounds( p.WorldPosition ) );

			if ( zone.PlayerCount > 0 )
				zone.TimeSinceNoPlayers = 0;
		}
	}

	private void UpdatePressure()
	{
		var currentCount = GlobalEnemyCount;

		if ( currentCount < _lastEnemyCount )
		{
			var killed = _lastEnemyCount - currentCount;
			GlobalPressure = Math.Min( GlobalPressure + killed * 2f, Config.PressureMax );
		}
		_lastEnemyCount = currentCount;

		var anyCombat = Zones.Any( z => z.State == ZoneState.Combat );
		if ( anyCombat )
			GlobalPressure = Math.Min( GlobalPressure + Config.PressureAccumulationRate * Time.Delta, Config.PressureMax );
		else
			GlobalPressure = Math.Max( GlobalPressure - Config.PressureDecayRate * Time.Delta, 0 );

		if ( _timeSincePressureCheck > 5f )
		{
			_timeSincePressureCheck = 0;
			if ( GlobalPressure > 5f )
				Log.Info( $"Global Pressure: {GlobalPressure:F0}/{Config.PressureMax}" );
		}
	}

	private void UpdateZoneStates()
	{
		foreach ( var zone in Zones )
		{
			if ( zone.State == ZoneState.Dormant )
				continue;

			var hasPlayers = zone.PlayerCount > 0;

			if ( !zone.HasEngagedEnemies )
				zone.TimeSinceNoEngagement = 0;

			switch ( zone.State )
			{
				case ZoneState.Ambient:
					if ( hasPlayers )
						SetZoneState( zone, ZoneState.Warming );
					break;

				case ZoneState.Warming:
					if ( zone.HasEngagedEnemies )
						SetZoneState( zone, ZoneState.Combat );
					else if ( !hasPlayers && zone.TimeSinceNoPlayers > 30f )
						SetZoneState( zone, ZoneState.Ambient );
					break;

				case ZoneState.Combat:
					if ( !zone.HasEngagedEnemies && zone.TimeSinceNoEngagement > 15f )
						SetZoneState( zone, ZoneState.Exhausted );
					break;

				case ZoneState.Exhausted:
					if ( zone.TimeSinceNoEngagement > 10f )
						SetZoneState( zone, ZoneState.Cooldown );
					break;

				case ZoneState.Cooldown:
					if ( zone.TimeSinceNoEngagement > 30f )
						SetZoneState( zone, ZoneState.Ambient );
					break;
			}
		}
	}

	private void SetZoneState( EncounterZone zone, ZoneState newState )
	{
		if ( zone.State == newState )
			return;

		Log.Info( $"Zone '{zone.ZoneName}': {zone.State} → {newState}" );

		if ( newState == ZoneState.Combat && zone.State != ZoneState.Combat )
			AddPressure( 3f );

		if ( newState == ZoneState.Cooldown )
			AddPressure( -5f );

		zone.State = newState;
		zone.TimeSinceNoEngagement = 0;
	}

	private void CleanupResolvedEncounters()
	{
		foreach ( var zone in Zones )
		{
			for ( int i = zone.ActiveEncounters.Count - 1; i >= 0; i-- )
			{
				var encounter = zone.ActiveEncounters[i];
				encounter.SpawnedEnemies.RemoveAll( e => !e.GameObject.IsValid() );

				if ( encounter.ShouldResolve() )
				{
					encounter.Phase = EncounterPhase.Resolved;
					zone.ActiveEncounters.RemoveAt( i );
				}
			}
		}
	}

	private void UpdateEncounterWaves()
	{
		foreach ( var zone in Zones )
		{
			foreach ( var encounter in zone.ActiveEncounters )
			{
				if ( !encounter.HasNextWave )
					continue;

				if ( encounter.Phase != EncounterPhase.Active )
					continue;

				if ( encounter.AliveCount == 0 && encounter.TimeSinceWaveStart > encounter.WaveInterval )
				{
					var spawned = encounter.SpawnNextWave( encounter.SpawnPosition );
					Log.Info( $"Wave {encounter.CurrentWave}/{encounter.TotalWaveCount}: +{spawned.Count} enemies for '{zone.ZoneName}'" );
					NoiseSystem.Emit( encounter.SpawnPosition, 2000f, 0.7f, NoiseType.TaskDevice );
				}
			}
		}
	}

	private int GlobalEnemyCount => Game.ActiveScene.GetAllComponents<BaseZombie>().Count();

	private float GetReinforceInterval()
	{
		return Math.Max( 3f, 8f - GlobalPressure / 25f );
	}

	private SpawnGroup GetReinforcementGroup( EncounterZone zone )
	{
		if ( GlobalPressure > 80 )
			return GetSpawnGroup("ChargerWave");

		if ( GlobalPressure > 60 && Game.Random.NextFloat() < Config.SpecialEnemyWeightHigh )
			return GetSpawnGroup("ChargerWave");

		if ( GlobalPressure > 30 && Game.Random.NextFloat() < Config.SpecialEnemyWeightLow )
			return GetSpawnGroup("ChargerWave");

		return zone.DangerLevel >= 2
			? GetSpawnGroup("LightResponse")
			: GetSpawnGroup("WanderPack");
	}

	private void RefreshAmbientEncounters()
	{
		foreach ( var zone in Zones )
		{
			if ( zone.State is not (ZoneState.Ambient or ZoneState.Warming) ) continue;
			var current = zone.CurrentEnemyCount;
			if ( current >= zone.AmbientBudget ) continue;
			if ( GlobalEnemyCount >= Config.GlobalMaxEnemies ) continue;
			if ( !zone.CanAcceptEncounter() ) continue;
			var available = zone.GetAvailableSources( EncounterType.AmbientPatrol );
			Vector3 spawnPos;
			if ( available.Count > 0 )
				spawnPos = SnapToNavMesh( available[Game.Random.NextInt( available.Count )].Position, zone );
			else
				spawnPos = FindNavMeshPosition( zone );

			var group = zone.DangerLevel >= 2 ? GetSpawnGroup("WanderPack") : GetSpawnGroup("ScoutPatrol");
			if ( group is null ) continue;
			var spawned = group.Spawn( spawnPos );

			var encounter = new EncounterInstance
			{
				Type = EncounterType.AmbientPatrol,
				Phase = EncounterPhase.Active,
				OwningZone = zone,
				AssignedGroup = group,
				SpawnedEnemies = spawned,
				SpawnPosition = spawnPos
			};

			foreach ( var zombie in spawned )
			{
				zombie.OwningEncounter = encounter;
				zombie.PatrolGroupId = _nextPatrolGroupId;
			}

			zone.ActiveEncounters.Add( encounter );
			_nextPatrolGroupId++;

			Log.Info( $"Ambient spawn: '{zone.ZoneName}' +{spawned.Count} enemies ({current}→{current + spawned.Count}/{zone.AmbientBudget})" );
		}
	}

	private void RefreshReinforcements()
	{
		foreach ( var zone in Zones )
		{
			if ( zone.State != ZoneState.Combat )
				continue;

			var current = zone.CurrentEnemyCount;
			if ( current >= zone.CombatBudget )
				continue;

			if ( GlobalEnemyCount >= Config.GlobalMaxEnemies )
				continue;

			if ( zone.ActiveEncounters.Count >= 4 )
				continue;

			var available = zone.GetAvailableSources( EncounterType.DynamicReinforcement );
			if ( available.Count == 0 )
				available = zone.SpawnSources;

			if ( available.Count == 0 )
				continue;

			var source = available[Game.Random.NextInt( available.Count )];
			var group = GetReinforcementGroup( zone );
			if ( group is null ) continue;

			var nearestPlayer = Game.ActiveScene.GetAllComponents<HumanPlayer>()
				.Where( p => p.IsAlive && zone.IsPointInBounds( p.WorldPosition ) )
				.OrderBy( p => (p.WorldPosition - source.Position).Length )
				.FirstOrDefault();

			var spawned = group.Spawn( SnapToNavMesh( source.Position, zone ), nearestPlayer?.GameObject );

			var encounter = new EncounterInstance
			{
				Type = EncounterType.DynamicReinforcement,
				Phase = EncounterPhase.Active,
				OwningZone = zone,
				AssignedGroup = group,
				SpawnedEnemies = spawned,
				SpawnPosition = source.Position,
				ForceTarget = nearestPlayer?.GameObject
			};

			foreach ( var zombie in spawned )
			{
				zombie.OwningEncounter = encounter;
				zombie.PatrolGroupId = _nextPatrolGroupId;
				if ( nearestPlayer.IsValid() && zombie.Brain != null )
					zombie.Brain.ForceEngage( nearestPlayer.GameObject );
			}

			zone.ActiveEncounters.Add( encounter );
			_nextPatrolGroupId++;
			zone.AlertLevel = Math.Min( zone.AlertLevel + 0.15f, 1f );

			Log.Info( $"Reinforcement: '{zone.ZoneName}' +{spawned.Count} enemies [{group.GroupName}] ({current}→{current + spawned.Count}/{zone.CombatBudget})" );
		}
	}
	private void ForceAmbientSpawn()
	{
		Log.Info( "[Director] Force spawning test zombies..." );
		foreach ( var zone in Zones )
		{
			if ( zone.State is not (ZoneState.Ambient or ZoneState.Warming) ) continue;
			Log.Info( $"  Force spawn in {zone.ZoneName} (state={zone.State}, budget={zone.AmbientBudget}, sources={zone.SpawnSources.Count})" );
			var group = zone.DangerLevel >= 2 ? GetSpawnGroup("WanderPack") : GetSpawnGroup("ScoutPatrol");
			if ( group is null ) continue;
			var source = zone.FindNearestSource( zone.Bounds.Center );
			var pos = source.Position;
				pos = Game.ActiveScene.NavMesh.GetClosestPoint( pos ) ?? pos;
			var spawned = group.Spawn( pos );
			Log.Info( $"  Spawned {spawned.Count} zombies at {pos}" );
			if ( spawned.Count > 0 ) return;
		}
	}


	public ObjectiveInstance CreateObjective( EncounterObjectivePoint point, GameObject activator )
	{
		var zone = Zones.FirstOrDefault( z => z.IsPointInBounds( point.WorldPosition ) ) ?? Zones.FirstOrDefault();
		if ( zone is null ) return null;
		var obj = new ObjectiveInstance { Point = point, Activator = activator, OwningZone = zone };
		obj.Phase = ObjectivePhase.Active;
		obj.TimeSinceLastReinforce = 0;
		_activeObjectives.Add( obj );
		zone.State = ZoneState.Combat;
		return obj;
	}

	void TickObjectives()
	{
		foreach ( var obj in _activeObjectives ) obj.Tick( this );
		_activeObjectives.RemoveAll( o => o.Phase is ObjectivePhase.Completed or ObjectivePhase.Abandoned );
	}



	void ApplyConfig()
	{
		if ( ConfigAsset is null ) return;
		Log.Info( "[EncounterConfig] Applying config asset..." );
		Config.GlobalMaxEnemies = ConfigAsset.GlobalMaxEnemies;
		Config.MaxActiveEncounters = ConfigAsset.MaxActiveEncounters;
		Config.AmbientRefreshInterval = ConfigAsset.AmbientRefreshInterval;
		Config.PressureDecayRate = ConfigAsset.PressureDecayRate;
		Config.PressureAccumulationRate = ConfigAsset.PressureAccumulationRate;
		Config.HighPressureThreshold = ConfigAsset.HighPressureThreshold;
		Config.CrossZonePursuitThreshold = ConfigAsset.CrossZonePursuitThreshold;
		SpawnGroup.MinSeparation = ConfigAsset.SpawnMinSeparation;
		SpawnGroup.InnerRadius = ConfigAsset.SpawnInnerRadius;
		SpawnGroup.OuterRadius = ConfigAsset.SpawnOuterRadius;
		SpawnRule.DefaultMinPlayerDistance = ConfigAsset.DefaultMinPlayerDistance;
		SpawnRule.DefaultMaxPlayerDistance = ConfigAsset.DefaultMaxPlayerDistance;
		SpawnRule.DefaultRequireOutOfSight = ConfigAsset.DefaultRequireOutOfSight;
		ZombiePerception.DefaultVisionRange = ConfigAsset.SightRange;
		ZombiePerception.DefaultVisionConeAngle = ConfigAsset.SightHalfAngle * 2f;
		ZombiePerception.DefaultHearingRange = ConfigAsset.HearingRange;
		ZombiePerception.DefaultProximityRadius = ConfigAsset.ProximityRange;
		ZombiePerception.SprintProximityBonus = ConfigAsset.SprintProximityBonus;
		ZombiePerception.SightMemory = ConfigAsset.SightMemory;
		ZombiePerception.SearchDuration = ConfigAsset.SearchDuration;
	}

	private static Vector3 SnapToNavMesh( Vector3 pos, EncounterZone zone )
	{
		// Find ground-level nav point at bottom of the zone trigger — avoids rooftops
		var bottomCenter = zone.Bounds.Center.WithZ( zone.Bounds.Mins.z );
		var groundNav = Game.ActiveScene.NavMesh.GetClosestPoint( bottomCenter );
		if ( groundNav.HasValue && zone.IsPointInBounds( groundNav.Value ) )
			return groundNav.Value;

		var navPt = Game.ActiveScene.NavMesh.GetClosestPoint( pos );
		return navPt.HasValue && zone.IsPointInBounds( navPt.Value ) ? navPt.Value : pos;
	}

	private static Vector3 FindNavMeshPosition( EncounterZone zone )
	{
		for ( int tries = 0; tries < 10; tries++ )
		{
			var candidate = zone.Bounds.Center + new Vector3(
				Game.Random.NextFloat( -500, 500 ),
				Game.Random.NextFloat( -500, 500 ), 0 );

			var navPt = Game.ActiveScene.NavMesh.GetClosestPoint( candidate );
			if ( navPt.HasValue && zone.IsPointInBounds( navPt.Value ) )
				return navPt.Value;
		}

		var centerNav = Game.ActiveScene.NavMesh.GetClosestPoint( zone.Bounds.Center );
		return centerNav ?? zone.Bounds.Center;
	}
}
