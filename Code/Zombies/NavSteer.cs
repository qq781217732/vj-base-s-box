namespace ZombieHorde;

public class NavSteer
{
	public NavPath Path { get; private set; }
	public TimeUntil TimeUntilCanMove { get; set; } = 0;

	public NavSteer()
	{
		Path = new NavPath();
	}

	public virtual void Tick( Vector3 currentPosition, Vector3 velocity = new Vector3(), float sharpStartAngle = 60f )
	{
		if ( TimeUntilCanMove > 0 ) return;

		Path.Update( currentPosition, Target, velocity, sharpStartAngle );

		Output.Finished = Path.IsEmpty;

		if ( Output.Finished )
		{
			Output.Direction = Vector3.Zero;
			return;
		}

		Output.Direction = Path.GetDirection( currentPosition );

		var avoid = GetAvoidance( currentPosition, 500 );
		if ( !avoid.IsNearlyZero() )
		{
			Output.Direction = (Output.Direction + avoid).Normal;
		}
	}

	Vector3 GetAvoidance( Vector3 position, float radius )
	{
		var center = position + Output.Direction * radius * 0.5f;

		var objectRadius = 160.0f;
		Vector3 avoidance = default;

		var distToTarget = (position - Target).Length;
		if ( distToTarget < 300 )
		{
			objectRadius -= distToTarget.LerpInverse( 300, 0 ) * 100;
		}

		foreach ( var obj in Game.ActiveScene.FindInPhysics( new Sphere( center, radius ) ) )
		{
			if ( obj.Components.Get<BaseNpc>() is null && obj.Components.Get<HumanPlayer>() is null )
				continue;

			if ( obj.Tags.Has( "world" ) )
				continue;

			var delta = (position - obj.WorldPosition);
			var closeness = delta.Length;
			if ( closeness < 0.001f ) continue;
			var thrust = ((objectRadius - closeness) / objectRadius).Clamp( 0, 1 );
			if ( thrust <= 0 ) continue;

			avoidance += delta.Normal * thrust * thrust;
		}

		return avoidance;
	}

	public virtual void DebugDrawPath()
	{
		Path.DebugDraw( 0.1f, 0.1f );
	}

	public Vector3 Target { get; set; }

	public NavSteerOutput Output;

	public struct NavSteerOutput
	{
		public bool Finished;
		public Vector3 Direction;
	}
}
