using SWB.Player;
using SWB.Shared;
using System;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Zombie Network Manager" )]
public class ZombieNetworkManager : Component, Component.INetworkListener
{
	[Property] public PrefabScene PlayerPrefab { get; set; }
	[Property] public PrefabScene BotPrefab { get; set; }
	[Sync] public BaseGamemode Gamemode { get; set; }
	public static ZombieNetworkManager Instance { get; private set; }

	[ConVar( "server" )]
	public static bool zom_encounter_mode { get; set; }
	[ConVar( "server" )]
	public static bool zom_debug_round_info { get; set; }

	[ConCmd]
	public static void zom_count()
	{
		foreach ( var z in Game.ActiveScene.GetAllComponents<BaseZombie>() )
			Log.Info( $"ZOM: {z.GameObject.Name} alive={z.IsAlive} pos={z.WorldPosition}" );
	}

	[ConCmd]
	public static void zom_spawn()
	{
		var pos = Game.ActiveScene.GetAllComponents<HumanPlayer>().FirstOrDefault()?.WorldPosition ?? Vector3.Zero;
		pos += new Vector3( Game.Random.NextFloat( -200, 200 ), Game.Random.NextFloat( -200, 200 ), 0 );
		var go = new GameObject( true, "DebugZombie" );
		go.WorldPosition = pos;
		go.Components.Create<NavMeshAgent>();
		go.Components.Create<SkinnedModelRenderer>().Model = Model.Load( "models/citizen/citizen.vmdl" );
		go.Components.Create<CommonZombie>();
		go.NetworkSpawn();
		Log.Info( $"Spawned debug zombie at {pos}" );
	}

	[ConVar( "zom_debug_overlay", ConVarFlags.Replicated )]
	public static bool DebugOverlayEnabled { get; set; }

	[ConCmd( "zom_debug", ConVarFlags.Server )]
	public static void zom_debug( string mode = "" )
	{
		if ( string.IsNullOrWhiteSpace( mode ) )
			DebugOverlayEnabled = !DebugOverlayEnabled;
		else
			DebugOverlayEnabled = mode.ToLower() switch
			{
				"on" or "1" or "true" => true,
				"off" or "0" or "false" => false,
				_ => !DebugOverlayEnabled
			};
		Log.Info( $"Debug Overlay: {(DebugOverlayEnabled ? "ON" : "OFF")}" );
	}

	protected override void OnAwake()
	{
		Instance = this;
	}

	protected override void OnStart()
	{
		if ( IsProxy ) return;
		if ( zom_encounter_mode )
		{
			Log.Info( "Encounter mode enabled" );
			Gamemode = new EncounterGamemode();
		}
		else
		{
			Gamemode = new SurvivalGamemode();
		}
	}

	void INetworkListener.OnActive( Connection connection )
	{
		if ( PlayerPrefab is null ) { Log.Error( "PlayerPrefab not assigned!" ); return; }
		var playerGO = PlayerPrefab.Clone();
		playerGO.Name = connection.DisplayName;
		playerGO.NetworkSpawn( connection );
		var player = playerGO.Components.Get<HumanPlayer>();
		if ( player is null ) { Log.Error( "No HumanPlayer on prefab!" ); playerGO.Destroy(); return; }
		Log.Info( $"\"{connection.DisplayName}\" joined" );
		var spawnPos = GetSpawnLocation( player );
		if ( Gamemode is not null && Gamemode.EnableRespawning() ) player.Respawn( spawnPos );
		else { player.Respawn( spawnPos ); player.Kill(); }
	}

	public virtual Transform GetSpawnLocation( HumanPlayer player )
	{
		var spawnPoints = Game.ActiveScene.GetAllComponents<SpawnPoint>();
		if ( !spawnPoints.Any() ) return new Transform( Vector3.Zero, Rotation.Identity );
		SpawnPoint best = null; float bestDist = 0;
		foreach ( var sp in spawnPoints )
		{
			var minDist = float.MaxValue;
			foreach ( var other in Game.ActiveScene.GetAllComponents<HumanPlayer>() )
				if ( other != player && other.IsAlive )
					minDist = Math.Min( minDist, (sp.WorldPosition - other.WorldPosition).Length );
			if ( minDist > bestDist || best is null ) { bestDist = minDist; best = sp; }
		}
		return best?.Transform.World ?? new Transform( Vector3.Zero, Rotation.Identity );
	}

	[Rpc.Broadcast]
	public static void Explosion( GameObject weapon, GameObject owner, Vector3 position, float radius, float damage, float forceScale, bool doEffects = true )
	{
		if ( doEffects ) { Sound.Play( "rust_pumpshotgun.shootdouble", position ); NoiseSystem.Emit( position, radius * 2f, 0.8f, NoiseType.Explosion ); }
		var overlaps = Game.ActiveScene.FindInPhysics( new Sphere( position, radius ) );
		foreach ( var obj in overlaps )
		{
			if ( !obj.IsValid() ) continue;
			var body = obj.Components.Get<Rigidbody>(); if ( body is null ) continue;
			var targetPos = body.MassCenter;
			if ( Vector3.DistanceBetween( position, targetPos ) > radius ) continue;
			var tr = Game.ActiveScene.Trace.Ray( position, targetPos ).IgnoreGameObjectHierarchy( weapon ).WithoutTags( "trigger" ).Run();
			if ( tr.Fraction < 0.98f ) continue;
			var dMul = 1.0f - Math.Clamp( Vector3.DistanceBetween( position, targetPos ) / radius, 0f, 1f );
			var dmgInfo = new SWB.Shared.DamageInfo { Attacker = owner, Weapon = weapon, Damage = damage * dMul, Origin = position, Force = (targetPos - position).Normal * forceScale * dMul * body.Mass, Position = targetPos };
			dmgInfo.Tags.Add( TagsHelper.Bullet );
			foreach ( var d in obj.Components.GetAll<IDamageable>() ) d.OnDamage( dmgInfo );
		}
	}

	public static bool ZomCleanupFilter( GameObject obj )
	{
		if ( obj is null || !obj.IsValid() ) return true;
		if ( obj.Components.TryGet<ZombieNetworkManager>( out _ ) ) return false;
		if ( obj.Components.TryGet<BaseGamemode>( out _ ) ) return false;
		if ( obj.Components.TryGet<HumanPlayer>( out _ ) ) return false;
		if ( obj.Tags.Has( TagsHelper.ViewModel ) ) return false;
		return true;
	}
}
