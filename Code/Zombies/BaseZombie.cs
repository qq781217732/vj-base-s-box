using SWB.Shared;

namespace ZombieHorde;

public partial class BaseZombie : BaseNpc, Component.IDamageable
{
	public int MaxHealth { get; set; } = 100;

	[Sync] public float WalkSpeed { get; set; } = 45f;
	[Sync] public float RunSpeed { get; set; } = 140f;
	[Sync] public float Speed { get; set; } = 300f;

	public GameObject Target { get; set; }
	public NavSteer Steer { get; set; }
	public ZombieBrain Brain { get; set; }

	public Vector3 AnchorPosition { get; set; }
	public EncounterInstance OwningEncounter { get; set; }
	public int PatrolGroupId { get; set; }
	public bool IsStunned => false;

	public Vector3 EyePosition => WorldPosition + Vector3.Up * 64f;
	public Vector3 EyePos => EyePosition;
	public Angles EyeAngles => WorldRotation.Angles();

	public TimeSince TimeSinceClimb = 10;
	public Vector3 ClimbForward;

	// Character
	public NavMeshAgent Agent { get; set; }
	public SkinnedModelRenderer BodyRenderer { get; set; }

	public TimeSince TimeSinceAttacked { get; set; }
	public float AttackDamage { get; set; } = 10f;
	public float AttackSpeed { get; set; } = 1.2f;
	public TimeSince TimeSinceBurnTicked { get; set; }

	/// <summary>Multiplier applied to Agent.MaxSpeed by Brain.ExecuteTask. Reset to 1f each frame by Brain.</summary>
	public float SpeedMultiplier { get; set; } = 1f;

	protected Vector3 lookDir;

	[ConVar( "server" )] public static bool nav_drawpath { get; set; }

	// ========== Lifecycle ==========

	protected override void OnAwake()
	{
		Tags.Add( "zombie" );
		Tags.Add( "npc" );

		// Disable CharacterController if present — NavMeshAgent handles all movement
		var cc = Components.Get<CharacterController>();
		if ( cc is not null )
			cc.Enabled = false;
		CharacterController = null;

		Agent = Components.Get<NavMeshAgent>();
		if ( Agent is null )
			Agent = Components.Create<NavMeshAgent>();
		Agent.UpdatePosition = true;
		Agent.UpdateRotation = false;
		Agent.MaxSpeed = Speed;
		if ( Agent.Height <= 0f ) Agent.Height = 72f;
		if ( Agent.Radius <= 0f ) Agent.Radius = 16f;

		BodyRenderer = Components.Get<SkinnedModelRenderer>();
		BodyRenderer.CreateBoneObjects = true;
		BodyRenderer.CreateAttachments = true;

		// Setup hitboxes for bullet traces
		var hitboxes = Components.Get<ModelHitboxes>() ?? Components.Create<ModelHitboxes>();
		if ( hitboxes.Renderer is null )
			hitboxes.Renderer = BodyRenderer;

		WalkSpeed = Game.Random.NextFloat( 40, 50 );
		RunSpeed = Game.Random.NextFloat( 130, 150 );
		Speed = Game.Random.NextFloat( 270, 320 );
		AnchorPosition = WorldPosition;
		Brain = new ZombieBrain { Owner = this };
	}

	protected override void OnUpdate()
	{
		if ( !IsAlive || IsProxy ) return;
		if ( Agent is null ) return;

		Brain?.Tick();
	}

	protected override void OnFixedUpdate()
	{
		if ( !IsAlive || IsProxy ) return;

		// Prevent physics from pushing zombies upward when crowding
		var navPt = Scene.NavMesh?.GetClosestPoint( WorldPosition );
		if ( navPt.HasValue )
			WorldPosition = WorldPosition.WithZ( navPt.Value.z );

		// NavMeshAgent handles position via Brain.ExecuteTask() — we only manage rotation and animation here
		UpdateRotation();

		if ( BodyRenderer is null ) return;

		var velocity = Agent?.Velocity ?? Vector3.Zero;
		var horizSpeed = velocity.WithZ( 0 ).Length;
		var animInput = horizSpeed > 0.01f ? velocity.WithZ( 0 ).Normal * (horizSpeed < 1f ? horizSpeed : 1f) : Vector3.Zero;

		lookDir = Vector3.Lerp( lookDir, animInput.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
		BodyRenderer.Set( "lookat", lookDir );
		BodyRenderer.Set( "move_velocity", velocity );
		BodyRenderer.Set( "move_rotation", animInput );

		BodyRenderer.Set( "b_climbing", false );
		BodyRenderer.Set( "b_grounded", true );
	}

	void UpdateRotation()
	{
		var velocity = Agent?.Velocity ?? Vector3.Zero;
		var walkVelocity = velocity.WithZ( 0 );

		if ( walkVelocity.Length <= 1f )
		{
			// Stopped — face target if chasing, otherwise hold rotation
			if ( Brain?.TargetEntity.IsValid() == true && Brain.CurrentTask == ZombieTask.ChaseTarget )
			{
				var dirToTarget = (Brain.TargetEntity.WorldPosition - WorldPosition).WithZ( 0 );
				if ( dirToTarget.Length > 1f )
				{
					var lookRotation = Rotation.LookAt( dirToTarget.Normal, Vector3.Up );
					WorldRotation = Rotation.Lerp( WorldRotation, lookRotation, Time.Delta * 10.0f );
				}
			}
			return;
		}

		var turnSpeed = walkVelocity.Length.LerpInverse( 0, 250, true );
		var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );

		if ( TimeSinceClimb < 0.5f )
		{
			targetRotation = Rotation.LookAt( ClimbForward, Vector3.Up );
			WorldRotation = targetRotation;
		}
		else if ( IsStunned )
		{
			if ( WorldRotation.Distance( targetRotation ) > 160 )
				targetRotation = Rotation.LookAt( -walkVelocity.Normal, Vector3.Up );
			WorldRotation = Rotation.Lerp( WorldRotation, targetRotation, turnSpeed * Time.Delta * 5.0f );
		}
		else
		{
			WorldRotation = Rotation.Lerp( WorldRotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
		}
	}

	public void TakeDamage( SWB.Shared.DamageInfo info )
	{
		if ( !IsAlive ) return;
		Health -= info.Damage;
		if ( Health <= 0 ) Kill();
		Brain?.OnDamaged( info.Attacker );
	}

	public virtual void Stun( float duration ) { }
	public virtual void Ignite() { }
	public virtual void HitBreakableObject() { }
	public bool IsOnFire { get; set; }

	public void Kill()
	{
		Health = 0;
		foreach ( var col in Components.GetAll<Collider>() )
			col.Enabled = false;
		GameObject.DestroyAsync( 20 );
	}
}
