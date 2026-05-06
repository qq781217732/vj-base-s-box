using System;
using SWB.Shared;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Loot Box" )]
public partial class LootBox : Component, Component.IDamageable
{
	[Sync] public float Health { get; set; } = 15;
	public int WaveNumber { get; set; } = 100;

	Sandbox.DamageInfo lastDamage;

	protected override void OnAwake()
	{
		GameObject.Tags.Add( "item" );
	}

	protected override void OnFixedUpdate()
	{
		if ( WorldPosition.z < -20000 )
			GameObject.Destroy();
	}

	async void AsyncPing( float time )
	{
		await Task.Delay( (int)(time * 1000) );
		if ( !GameObject.IsValid() ) return;
		PingMarker.Ping( WorldPosition, PingType.Lootbox, "Treasure!", 300, GameObject );
	}

	public void OnDamage( in Sandbox.DamageInfo info )
	{
		lastDamage = info;
		Health -= info.Damage;
		if ( Health <= 0 )
		{
			SpawnLoot();
			GameObject.Destroy();
		}
	}

	void SpawnLoot()
	{
		var lootTable = new List<System.Type>
		{
			typeof(F1), typeof(AKM), typeof(M1A), typeof(MP5),
			typeof(BaseballBat), typeof(FireAxe), typeof(Revolver), typeof(Shovel),
			typeof(R870), typeof(CompactShotgun), typeof(DoubleBarrel), typeof(HuntingRifle),
			typeof(TripmineWeapon), typeof(PipeBomb), typeof(Molotov), typeof(ImpactGrenade),
		};

		// Filter based on wave number
		if ( WaveNumber < 2 )
			lootTable = new() { typeof(F1), typeof(M1A), typeof(MP5) };
		else if ( WaveNumber < 3 )
			lootTable = new() { typeof(Revolver), typeof(BaseballBat), typeof(FireAxe), typeof(Shovel) };
		else if ( WaveNumber < 4 )
			lootTable = new() { typeof(TripmineWeapon), typeof(PipeBomb), typeof(ImpactGrenade), typeof(Molotov) };
		else if ( WaveNumber < 5 )
			lootTable = new() { typeof(F1), typeof(M1A), typeof(MP5), typeof(Revolver), typeof(TripmineWeapon), typeof(Molotov), typeof(ImpactGrenade), typeof(R870) };

		for ( var i = 0; i < Game.Random.NextInt( 1 ); i++ )
		{
			var weaponType = lootTable[Game.Random.NextInt( lootTable.Count )];
			var obj = new GameObject( true, "Loot" );
			obj.WorldPosition = WorldPosition + Vector3.Up * 24;
			obj.Components.Create( TypeLibrary.GetType( weaponType.FullName ) );
			obj.NetworkSpawn();
			var rb = obj.Components.Get<Rigidbody>();
			if ( rb is not null ) rb.Velocity = Vector3.Random * 100;
		}

		var healTypes = new List<System.Type> { typeof(HealthKit), typeof(HealthSyringe), typeof(Adrenaline) };
		var healType = healTypes[Game.Random.NextInt( healTypes.Count )];
		var healObj = new GameObject( true, "HealLoot" );
		healObj.WorldPosition = WorldPosition + Vector3.Up * 16;
		healObj.Components.Create( TypeLibrary.GetType( healType.FullName ) );
		healObj.NetworkSpawn();
		var healRb = healObj.Components.Get<Rigidbody>();
		if ( healRb is not null ) healRb.Velocity = Vector3.Random * 100;
	}
}
