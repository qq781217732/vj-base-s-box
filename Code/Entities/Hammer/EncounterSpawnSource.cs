using System;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Encounter Spawn Source" )]
public partial class EncounterSpawnSource : Component
{
	[Property, Title( "Source Type" )]
	public SpawnSourceType SourceType { get; set; } = SpawnSourceType.HiddenInterior;

	[Property, Title( "Min Player Distance" )]
	public float MinPlayerDistance { get; set; } = 400f;

	[Property, Title( "Max Player Distance" )]
	public float MaxPlayerDistance { get; set; } = 6000f;

	[Property, Title( "Require Out Of Sight" )]
	public bool RequireOutOfSight { get; set; } = true;

	[Property, Title( "Allow Special Enemies" )]
	public bool AllowSpecialEnemies { get; set; } = true;

	[Property, Title( "Allowed Encounter Types" )]
	public EncounterTypeFlags AllowedEncounterTypes { get; set; } = EncounterTypeFlags.All;

	public SpawnSource ToSpawnSource()
	{
		var source = new SpawnSource
		{
			Position = WorldPosition,
			SourceType = SourceType
		};

		var rule = new SpawnRule
		{
			MinPlayerDistance = MinPlayerDistance,
			MaxPlayerDistance = MaxPlayerDistance,
			RequireOutOfSight = RequireOutOfSight,
			AllowSpecialEnemies = AllowSpecialEnemies
		};

		if ( AllowedEncounterTypes != EncounterTypeFlags.All )
		{
			rule.AllowedEncounterTypes = AllowedEncounterTypes.ToList();
		}

		source.Rules.Add( rule );
		return source;
	}
}

[Flags]
public enum EncounterTypeFlags
{
	AmbientPatrol = 1,
	AmbientWanderPack = 2,
	TriggeredAmbush = 4,
	ObjectiveDefenseWave = 8,
	DynamicReinforcement = 16,
	PursuitPack = 32,
	All = AmbientPatrol | AmbientWanderPack | TriggeredAmbush | ObjectiveDefenseWave | DynamicReinforcement | PursuitPack
}

public static class EncounterTypeFlagsExtensions
{
	public static List<EncounterType> ToList( this EncounterTypeFlags flags )
	{
		var list = new List<EncounterType>();
		if ( flags.HasFlag( EncounterTypeFlags.AmbientPatrol ) ) list.Add( EncounterType.AmbientPatrol );
		if ( flags.HasFlag( EncounterTypeFlags.AmbientWanderPack ) ) list.Add( EncounterType.AmbientWanderPack );
		if ( flags.HasFlag( EncounterTypeFlags.TriggeredAmbush ) ) list.Add( EncounterType.TriggeredAmbush );
		if ( flags.HasFlag( EncounterTypeFlags.ObjectiveDefenseWave ) ) list.Add( EncounterType.ObjectiveDefenseWave );
		if ( flags.HasFlag( EncounterTypeFlags.DynamicReinforcement ) ) list.Add( EncounterType.DynamicReinforcement );
		if ( flags.HasFlag( EncounterTypeFlags.PursuitPack ) ) list.Add( EncounterType.PursuitPack );
		return list;
	}
}
