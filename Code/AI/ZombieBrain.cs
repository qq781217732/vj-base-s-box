using System;

namespace ZombieHorde;

public enum Awareness
{
	Dormant, Idle, Suspicious, Investigating, Alerted, Engaged, Attacking, Searching, Returning
}

public enum ZombieTask
{
	Patrol, Wander, GuardPoint, MoveToSound, MoveToLastSeen, ChaseTarget,
	Attack, HoldObjective, Reinforce, ReturnToAnchor
}

[Flags]
public enum ZombieCondition
{
	None = 0, Burning = 1, Stunned = 2, KnockedBack = 4, Lured = 8
}

public class ZombieBrain
{
	public BaseZombie Owner { get; set; }
	public ZombiePerception Perception { get; set; }

	public Awareness CurrentAwareness { get; set; } = Awareness.Idle;
	public ZombieTask CurrentTask { get; set; } = ZombieTask.Wander;
	public ZombieCondition Conditions { get; set; } = ZombieCondition.None;

	public GameObject TargetEntity { get; set; }
	public Vector3? InvestigatePosition { get; set; }
	public Vector3 AnchorPosition { get; set; }

	public EncounterInstance OwningEncounter { get; set; }
	public int PatrolGroupId { get; set; }

	public TimeSince TimeSinceLastSight { get; set; }
	public TimeSince TimeSinceSearchStart { get; set; }
	public TimeSince TimeSinceInvestigateStart { get; set; }
	public TimeSince TimeSinceAttackStart { get; set; }
	public float SearchDuration { get; set; } = 8f;
	public float SightMemory { get; set; } = 5f;

	// Surround formation
	public float PreferredAngle { get; set; }
	public TimeSince TimeSinceRetreated { get; set; }
	const float SurroundRadius = 80f;
	const float MeleeRange = 55f;
	const float MinApproachDist = 50;
	const int MaxAttackSlots = 2;

	public ZombieBrain()
	{
		Perception = new ZombiePerception { Brain = this };
		PreferredAngle = (float)new Random().NextDouble() * 360f;
		TimeSinceRetreated = 999;
	}

	public void Tick()
	{
		if ( Conditions.HasFlag( ZombieCondition.Stunned ) ) return;
		if ( Conditions.HasFlag( ZombieCondition.Burning ) ) return;
		Perception.Tick();
		UpdateAwareness();
		SelectTask();
		ExecuteTask();
	}

	private void UpdateAwareness()
	{
		switch ( CurrentAwareness )
		{
			case Awareness.Dormant:
				break;

			case Awareness.Idle:
				if ( Perception.HasVisualContact )
					TransitionTo( Awareness.Alerted, Perception.ClosestVisiblePlayer?.GameObject );
				else if ( Perception.IsPlayerInProximity )
					TransitionTo( Awareness.Alerted, Perception.ClosestProximityPlayer?.GameObject );
				else if ( ShouldInvestigateNoise( Perception.ClosestNoise ) )
				{
					InvestigatePosition = Perception.ClosestNoise.Position;
					TransitionTo( Awareness.Investigating, null );
				}
				break;

			case Awareness.Suspicious:
				if ( Perception.HasVisualContact )
					TransitionTo( Awareness.Alerted, Perception.ClosestVisiblePlayer?.GameObject );
				else if ( Perception.IsPlayerInProximity )
					TransitionTo( Awareness.Alerted, Perception.ClosestProximityPlayer?.GameObject );
				else if ( ShouldInvestigateNoise( Perception.ClosestNoise ) )
				{
					InvestigatePosition = Perception.ClosestNoise.Position;
					TransitionTo( Awareness.Investigating, null );
				}
				else if ( TimeSinceLastSight > 3f )
					TransitionTo( Awareness.Returning, null );
				break;

			case Awareness.Investigating:
				if ( Perception.HasVisualContact || Perception.IsPlayerInProximity )
					TransitionTo( Awareness.Alerted, Perception.ClosestVisiblePlayer?.GameObject ?? Perception.ClosestProximityPlayer?.GameObject );
				else if ( Perception.ClosestNoise != null && Perception.ClosestNoise.Intensity > 0.6f )
				{
					InvestigatePosition = Perception.ClosestNoise.Position;
					TimeSinceInvestigateStart = 0;
				}
				else if ( TimeSinceInvestigateStart > 8f )
					TransitionTo( Awareness.Suspicious, null );
				break;

			case Awareness.Alerted:
				TargetEntity = Perception.ClosestVisiblePlayer?.GameObject ?? TargetEntity;
				TransitionTo( Awareness.Engaged, TargetEntity );
				ZombieAlertModel.TryAlertNearby( Owner, 0.8f );
				NoiseSystem.Emit( Owner.WorldPosition, 600f, 0.4f, NoiseType.ZombieAlert );
				break;

			case Awareness.Engaged:
				if ( Perception.HasVisualContact )
				{
					TimeSinceLastSight = 0;
					TargetEntity = Perception.ClosestVisiblePlayer?.GameObject ?? TargetEntity;
				}
				else if ( TimeSinceLastSight > SightMemory )
				{
					TimeSinceSearchStart = 0;
					if ( TargetEntity.IsValid() )
						InvestigatePosition = TargetEntity.WorldPosition;
					TransitionTo( Awareness.Searching, null );
				}

				// Attack when close — doesn't require vision (zombie is circling)
				if ( TargetEntity.IsValid() )
				{
					var dist = (Owner.WorldPosition - TargetEntity.WorldPosition).Length;
					if ( dist < MeleeRange && TimeSinceAttackStart > 1.2f && TimeSinceRetreated > 0.6f )
						TransitionTo( Awareness.Attacking, TargetEntity );
				}
				break;

			case Awareness.Attacking:
				if ( !TargetEntity.IsValid() )
				{
					TransitionTo( Awareness.Engaged, null );
				}
				else if ( TimeSinceAttackStart > 0.5f )
				{
					// Retreat briefly after attack, then re-engage
					TimeSinceRetreated = 0;
					TransitionTo( Awareness.Engaged, TargetEntity );
				}
				break;

			case Awareness.Searching:
				if ( Perception.HasVisualContact )
					TransitionTo( Awareness.Engaged, Perception.ClosestVisiblePlayer?.GameObject );
				else if ( Perception.IsPlayerInProximity )
					TransitionTo( Awareness.Alerted, Perception.ClosestProximityPlayer?.GameObject );
				else if ( TimeSinceSearchStart > SearchDuration )
					TransitionTo( Awareness.Returning, null );
				break;

			case Awareness.Returning:
				if ( Perception.HasVisualContact )
					TransitionTo( Awareness.Alerted, Perception.ClosestVisiblePlayer?.GameObject );
				else if ( Perception.IsPlayerInProximity )
					TransitionTo( Awareness.Alerted, Perception.ClosestProximityPlayer?.GameObject );
				else if ( TimeSinceInvestigateStart > 15f )
					TransitionTo( Awareness.Idle, null );
				break;
		}
	}

	bool ShouldInvestigateNoise( NoiseEvent noise )
	{
		if ( noise is null ) return false;
		if ( noise.Intensity < 0.15f ) return false;
		return true;
	}

	public void TransitionTo( Awareness newState, GameObject target )
	{
		CurrentAwareness = newState;
		if ( target != null ) TargetEntity = target;
		if ( newState == Awareness.Investigating ) TimeSinceInvestigateStart = 0;
		if ( newState == Awareness.Attacking ) TimeSinceAttackStart = 0;
		if ( newState == Awareness.Idle )
		{
			TargetEntity = null;
			InvestigatePosition = null;
		}
		if ( newState == Awareness.Engaged || newState == Awareness.Alerted || newState == Awareness.Suspicious )
			TimeSinceLastSight = 0;
	}

	private void SelectTask()
	{
		switch ( CurrentAwareness )
		{
			case Awareness.Idle:
			case Awareness.Dormant:       CurrentTask = ZombieTask.Wander; break;
			case Awareness.Suspicious:    CurrentTask = ZombieTask.GuardPoint; break;
			case Awareness.Investigating: CurrentTask = ZombieTask.MoveToSound; break;
			case Awareness.Alerted:
			case Awareness.Engaged:       CurrentTask = ZombieTask.ChaseTarget; break;
			case Awareness.Attacking:     CurrentTask = ZombieTask.Attack; break;
			case Awareness.Searching:     CurrentTask = ZombieTask.MoveToLastSeen; break;
			case Awareness.Returning:     CurrentTask = ZombieTask.ReturnToAnchor; break;
		}
		if ( Conditions.HasFlag( ZombieCondition.Lured ) )
			CurrentTask = ZombieTask.MoveToSound;
	}

	private Vector3 _wanderTarget;
	private TimeSince _wanderCooldown;

	private void ExecuteTask()
	{
		var agent = Owner.Agent;
		if ( agent is null ) return;

		Owner.SpeedMultiplier = 1f;

		var speed = 300f;
		var targetPos = Owner.AnchorPosition;

		switch ( CurrentTask )
		{
			case ZombieTask.GuardPoint:
				agent.Stop();
				return;

			case ZombieTask.Wander:
				if ( _wanderCooldown > 3f || !agent.IsNavigating )
				{
					var wp = Owner.Scene.NavMesh?.GetRandomPoint( Owner.AnchorPosition, 500 );
					if ( wp.HasValue ) { _wanderTarget = wp.Value; _wanderCooldown = 0; }
					else if ( _wanderTarget.IsNearZeroLength )
					{
						_wanderTarget = Owner.AnchorPosition;
					}
				}
				targetPos = _wanderTarget;
				speed = 200f;
				break;

			case ZombieTask.MoveToSound:
				targetPos = InvestigatePosition ?? Owner.AnchorPosition;
				speed = 250f;
				break;

			case ZombieTask.MoveToLastSeen:
				targetPos = InvestigatePosition ?? Owner.AnchorPosition;
				speed = 250f;
				break;

			case ZombieTask.ChaseTarget:
				if ( TargetEntity.IsValid() )
				{
					var dist = (Owner.WorldPosition - TargetEntity.WorldPosition).Length;

					// Approach from current direction — no circling around to a fixed angle
					var dir = (Owner.WorldPosition - TargetEntity.WorldPosition).Normal;
					var targetDist = dist > SurroundRadius ? SurroundRadius : MinApproachDist;
					targetPos = TargetEntity.WorldPosition + dir * targetDist;
				}
				speed = 400f;
				break;

			case ZombieTask.Attack:
				agent.Stop();

				if ( !TargetEntity.IsValid() )
				{
					TransitionTo( Awareness.Engaged, null );
					return;
				}

				var activeAttacks = 0;
				foreach ( var obj in Owner.Scene.FindInPhysics( new Sphere( TargetEntity.WorldPosition, MeleeRange + 20f ) ) )
				{
					if ( !obj.Tags.Has( "zombie" ) ) continue;
					var zb = obj.Components.Get<BaseZombie>()?.Brain;
					if ( zb is not null && zb.CurrentAwareness == Awareness.Attacking )
						activeAttacks++;
				}
				if ( activeAttacks > MaxAttackSlots )
				{
					TransitionTo( Awareness.Engaged, TargetEntity );
				}
				return;

			case ZombieTask.ReturnToAnchor:
				targetPos = Owner.AnchorPosition;
				speed = 250f;
				break;
		}

		agent.MaxSpeed = speed * Owner.SpeedMultiplier;
		agent.MoveTo( targetPos );
	}

	public void OnDamaged( GameObject attacker )
	{
		if ( CurrentAwareness == Awareness.Dormant ) return;
		if ( Conditions.HasFlag( ZombieCondition.Burning ) ) return;

		var player = attacker?.Components.Get<HumanPlayer>();
		if ( player != null )
		{
			TargetEntity = player.GameObject;
			TimeSinceLastSight = 0;
			if ( CurrentAwareness < Awareness.Alerted )
				TransitionTo( Awareness.Alerted, player.GameObject );
		}
	}

	public void TryAlert( GameObject target )
	{
		if ( CurrentAwareness >= Awareness.Alerted ) return;
		TransitionTo( Awareness.Alerted, target );
	}

	public void ForceEngage( GameObject target )
	{
		TargetEntity = target;
		TimeSinceLastSight = 0;
		TransitionTo( Awareness.Engaged, target );
	}
}
