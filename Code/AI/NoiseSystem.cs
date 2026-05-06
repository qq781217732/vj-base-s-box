namespace ZombieHorde;

public enum NoiseType
{
	Gunshot,
	Explosion,
	ZombieAlert,
	TaskDevice,
	Vehicle
}

public class NoiseEvent
{
	public Vector3 Position { get; set; }
	public float Radius { get; set; }
	public float Intensity { get; set; }
	public NoiseType Type { get; set; }
	public TimeSince TimeSinceEmitted { get; set; }

	public bool IsExpired()
	{
		return TimeSinceEmitted > 3f;
	}

	public float EffectiveIntensity( Vector3 listenerPos )
	{
		var dist = (listenerPos - Position).Length;
		if ( dist > Radius )
			return 0;
		return Intensity * (1f - dist / Radius);
	}
}

public static class NoiseSystem
{
	private static List<NoiseEvent> _activeEvents = new();
	private static bool _initialized;

	public static void Initialize()
	{
		if ( _initialized )
			return;
		_initialized = true;
		_activeEvents = new List<NoiseEvent>();
	}

	public static void Emit( Vector3 position, float radius, float intensity, NoiseType type )
	{
		Initialize();

		_activeEvents.Add( new NoiseEvent
		{
			Position = position,
			Radius = radius,
			Intensity = intensity,
			Type = type,
			TimeSinceEmitted = 0
		} );
	}

	public static NoiseEvent GetLoudestInRange( Vector3 listenerPos, float hearingRange )
	{
		CleanupExpired();

		NoiseEvent loudest = null;
		float loudestIntensity = 0;

		foreach ( var ev in _activeEvents )
		{
			var dist = (listenerPos - ev.Position).Length;
			if ( dist > ev.Radius || dist > hearingRange )
				continue;

			var effectiveIntensity = ev.EffectiveIntensity( listenerPos );
			if ( effectiveIntensity > loudestIntensity )
			{
				loudestIntensity = effectiveIntensity;
				loudest = ev;
			}
		}

		return loudest;
	}

	public static List<NoiseEvent> GetEventsInRange( Vector3 listenerPos, float range )
	{
		CleanupExpired();
		return _activeEvents
			.Where( e => (listenerPos - e.Position).Length < e.Radius + range )
			.ToList();
	}

	private static void CleanupExpired()
	{
		_activeEvents.RemoveAll( e => e.IsExpired() );
	}
}
