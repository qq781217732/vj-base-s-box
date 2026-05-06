namespace ZombieHorde;

public class NavPath
{
	public Vector3 TargetPosition;
	public List<Vector3> Points = new List<Vector3>();

	public bool IsEmpty => Points.Count <= 1;

	public void Update( Vector3 from, Vector3 to, Vector3 velocity = new Vector3(), float sharpStartAngle = 45f )
	{
		bool needsBuild = false;

		if ( !TargetPosition.AlmostEqual( to, 5 ) )
		{
			TargetPosition = to;
			needsBuild = true;
		}

		if ( needsBuild )
		{
			var fromFixed = Game.ActiveScene.NavMesh.GetClosestPoint( from );
			var toFixed = Game.ActiveScene.NavMesh.GetClosestPoint( to );
			if ( fromFixed == null || toFixed == null )
			{
				Log.Warning( "Nav out of bounds! Did a zombie fall out of the map?" );
				return;
			}

			Points.Clear();

			// TODO: NavMesh.PathBuilder API removed in new S&box
				//var path = Game.ActiveScene.NavMesh.PathBuilder( fromFixed.Value )
				//.WithSharpStartAngle( sharpStartAngle )
				//.WithStartVelocity( velocity / 2 )
				//.WithStepHeight( 16 )
				//.WithMaxClimbDistance( 500 )
				//.WithMaxDropDistance( 3000 )
				//.WithMaxDetourDistance( 100 )
				//.WithDropDistanceCostScale( 2f )
				//.WithPartialPaths()
				//.Build( toFixed.Value );

			//var segments = path.Segments;
			//Points = segments.Select( s => s.Position ).ToList();
			Points.Add( fromFixed.Value );
			Points.Add( toFixed.Value );
		}

		if ( Points.Count <= 1 )
		{
			return;
		}

		var deltaToCurrent = from - Points[0];
		var deltaToNext = from - Points[1];
		var delta = Points[1] - Points[0];
		var deltaNormal = delta.Normal;

		if ( deltaToNext.WithZ( 0 ).Length < 45 )
		{
			Points.RemoveAt( 0 );
			return;
		}

		if ( deltaToNext.Normal.Dot( deltaNormal ) >= 1.0f )
		{
			Points.RemoveAt( 0 );
		}
	}

	public float Distance( int point, Vector3 from )
	{
		if ( Points.Count <= point ) return float.MaxValue;

		return Points[point].WithZ( from.z ).Distance( from );
	}

	public Vector3 GetDirection( Vector3 position )
	{
		if ( Points.Count == 1 )
		{
			return (Points[0] - position).WithZ( 0 ).Normal;
		}

		return (Points[1] - position).WithZ( 0 ).Normal;
	}

	public void DebugDraw( float time, float opacity = 1.0f )
	{
		var draw = Sandbox.Debug.Draw.ForSeconds( time );
		var lift = Vector3.Up * 2;

		draw.WithColor( Color.White.WithAlpha( opacity ) ).Circle( lift + TargetPosition, Vector3.Up, 20.0f );

		int i = 0;
		var lastPoint = Vector3.Zero;
		foreach ( var point in Points )
		{
			if ( i > 0 )
			{
				draw.WithColor( i == 1 ? Color.Green.WithAlpha( opacity ) : Color.Cyan.WithAlpha( opacity ) ).Arrow( lastPoint + lift, point + lift, Vector3.Up, 5.0f );
			}

			lastPoint = point;
			i++;
		}
	}
}
