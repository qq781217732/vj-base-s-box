using Sandbox;
using Sandbox.UI;
using System;
using System.Runtime.CompilerServices;

public sealed class PlayerInteract : Component
{
	[Property] public string ActionName { get; set; } = "Use";
	[Property] public string TagName { get; set; } = "interact";
	[Property] public float RayLength { get; set; } = 125.0f;
	[Property] public float InteractCooldown { get; set; } = 2.0f;
	public event Action<SceneTraceResult> OnInteract;
	public event Action<SceneTraceResult> OnCanInteract;
	public event Action<SceneTraceResult> OnCanInteractEnd;
	private CameraComponent _camera;
	private TimeSince _timeSince;
	private bool _prevCanInteract;

	protected override void OnStart()
	{
		base.OnStart();

		_camera = GetComponentInChildren<PlayerController>().UseCameraControls ? Scene.Camera : GetComponentInChildren<CameraComponent>();
		if ( _camera == null )
		{
			Log.Error( "Failed to get CameraComponent" );
			return;
		}

		_timeSince = InteractCooldown; // allow first interact instantly
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		UpdateInteract();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public bool CanInteract( SceneTraceResult trace ) => 
		_timeSince > InteractCooldown && trace.Hit && trace.GameObject.Tags.Has( TagName );

	public void Interact( SceneTraceResult trace )
	{
		_timeSince = 0;
		OnInteract?.Invoke( trace );
	}

	private void UpdateInteract()
	{
		SceneTraceResult trace = CastRay();
		bool canInteract = CanInteract( trace );

		if ( canInteract != _prevCanInteract )
		{
			if ( canInteract )
				OnCanInteract?.Invoke( trace );
			else
				OnCanInteractEnd?.Invoke( trace );

			_prevCanInteract = canInteract;
		}

		if ( canInteract && Input.Released( ActionName ) )
		{
			Interact( trace );
		}
	}

	private SceneTraceResult CastRay()
	{
		Vector3 direction = ( _camera.ScreenToWorld( Screen.Size * 0.5f ) - _camera.WorldPosition ).Normal;
		Vector3 start = GameObject.WorldPosition + new Vector3( 0.0f, 0.0f, 64.0f );
		Vector3 end = start + direction * RayLength;

		return Scene.Trace.Ray( start, end ).IgnoreGameObjectHierarchy( GameObject ).Run();
	}
}

