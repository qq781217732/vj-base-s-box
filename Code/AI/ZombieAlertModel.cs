using System.Threading.Tasks;
using SWB.Shared;

namespace ZombieHorde;

public static class ZombieAlertModel
{
	public static void TryAlertNearby( BaseZombie source, float baseChance )
	{
		if ( source == null || !source.IsValid )
			return;

		var director = Game.ActiveScene.GetAllComponents<EncounterDirector>().FirstOrDefault();
		var crossZoneEnabled = director != null
			&& director.GlobalPressure > director.Config.CrossZonePursuitThreshold;

		var nearbyObjects = Game.ActiveScene.FindInPhysics( new Sphere( source.WorldPosition, 1000f ) );

		var nearbyZombies = new List<BaseZombie>();
		foreach ( var obj in nearbyObjects )
		{
			var z = obj.Components.Get<BaseZombie>();
			if ( z != null && z.IsValid && z != source )
				nearbyZombies.Add( z );
		}

		foreach ( var other in nearbyZombies )
		{
			if ( other.Brain == null )
				continue;

			if ( other.Brain.CurrentAwareness >= Awareness.Alerted )
				continue;

			if ( other.Brain.Conditions.HasFlag( ZombieCondition.Burning ) )
				continue;

			var relationLevel = GetRelationLevel( source, other );
			var chance = relationLevel switch
			{
				3 => baseChance * 0.8f,
				2 => baseChance * 0.5f,
				1 => baseChance * 0.2f,
				0 => crossZoneEnabled ? baseChance * 0.1f : 0f,
				_ => 0f
			};

			if ( chance <= 0 )
				continue;

			var dist = (other.WorldPosition - source.WorldPosition).Length;
			var delay = dist < 400f ? 0f : Game.Random.NextFloat( 0.5f, 2f );

			if ( Game.Random.NextFloat( 1f ) < chance )
			{
				if ( delay <= 0 )
				{
					other.Brain.ForceEngage( source.Brain.TargetEntity ?? source.Target );
				}
				else
				{
					_ = AlertWithDelay( other, source.Brain.TargetEntity ?? source.Target, delay );
				}
			}
		}
	}

	private static async Task AlertWithDelay( BaseZombie zombie, GameObject target, float delay )
	{
		await Task.Delay( (int)(delay * 1000) );
		if ( zombie.IsValid && zombie.Brain != null && target.IsValid() )
		{
			zombie.Brain.ForceEngage( target );
		}
	}

	private static int GetRelationLevel( BaseZombie a, BaseZombie b )
	{
		if ( a.PatrolGroupId != 0 && a.PatrolGroupId == b.PatrolGroupId )
			return 3;

		if ( a.OwningEncounter != null && a.OwningEncounter == b.OwningEncounter )
			return 2;

		return 1;
	}
}
