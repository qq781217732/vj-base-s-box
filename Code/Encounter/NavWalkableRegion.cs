using Sandbox.Navigation;

namespace ZombieHorde;

/// <summary>
/// Place on a GameObject in the scene to mark a walkable region.
/// Spawn candidates must be able to navigate to at least one marker.
/// Any nav mesh island without a marker is excluded from spawning.
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Nav Walkable Region" )]
public class NavWalkableRegion : Component
{
	[Property] public float Radius { get; set; } = 500f;
	[Property] public Color GizmoColor { get; set; } = new Color( 0.2f, 1f, 0.3f, 0.5f );

	static List<NavWalkableRegion> All = new();

	protected override void OnAwake()    => All.Add( this );
	protected override void OnDestroy()  => All.Remove( this );

	protected override void DrawGizmos()
	{
		var top = Vector3.Up * 128f;

		Gizmo.Draw.Color = GizmoColor;
		Gizmo.Draw.Line( Vector3.Zero, top );
		Gizmo.Draw.LineSphere( Vector3.Zero, Radius );
		Gizmo.Draw.SolidSphere( Vector3.Zero, 16f );
		Gizmo.Draw.SolidSphere( top, 24f );
	}

	public static bool IsSpawnValid( Vector3 candidate )
	{
		if ( All.Count == 0 ) return true;

		var navMesh = Game.ActiveScene.NavMesh;
		foreach ( var region in All )
		{
			var path = navMesh.CalculatePath( new CalculatePathRequest
			{
				Start = candidate,
				Target = region.WorldPosition
			} );
			if ( path.IsValid )
				return true;
		}
		return false;
	}
}
