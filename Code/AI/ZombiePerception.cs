using System;

namespace ZombieHorde;

public class ZombiePerception
{
	// Configurable defaults — set by Director.ApplyConfig() before any zombie spawns
	public static float DefaultVisionRange = 1500f;
	public static float DefaultVisionConeAngle = 120f;
	public static float DefaultHearingRange = 2000f;
	public static float DefaultProximityRadius = 150f;
	public static float SprintProximityBonus = 80f;
	public static float SightMemory = 4f;
	public static float SearchDuration = 5f;

	public ZombieBrain Brain { get; set; }
	public BaseZombie Owner => Brain?.Owner;

	public float VisionRange { get; set; }
	public float VisionConeAngle { get; set; }
	public float ProximityRadius { get; set; }
	public float HearingRange { get; set; }

	public bool HasVisualContact { get; private set; }
	public HumanPlayer ClosestVisiblePlayer { get; private set; }
	public bool IsPlayerInProximity { get; private set; }
	public HumanPlayer ClosestProximityPlayer { get; private set; }
	public NoiseEvent ClosestNoise { get; private set; }

	private TimeSince _timeSinceVisionCheck = 0;
	private const float VisionCheckInterval = 0.3f;

	public ZombiePerception()
	{
		VisionRange = DefaultVisionRange;
		VisionConeAngle = DefaultVisionConeAngle;
		HearingRange = DefaultHearingRange;
		ProximityRadius = DefaultProximityRadius;
	}

	public void Tick()
	{
		if ( Owner == null || !Owner.IsValid ) return;
		CheckProximity();
		CheckHearing();
		if ( _timeSinceVisionCheck > VisionCheckInterval )
		{
			_timeSinceVisionCheck = 0;
			CheckVision();
		}
	}

	void CheckVision()
	{
		HasVisualContact = false;
		ClosestVisiblePlayer = null;
		var players = Game.ActiveScene.GetAllComponents<HumanPlayer>().Where( p => p.IsAlive ).ToList();
		float closestDist = float.MaxValue;
		foreach ( var player in players )
		{
			var dirToPlayer = (player.EyePos - Owner.EyePosition).Normal;
			var distToPlayer = (player.EyePos - Owner.EyePosition).Length;
			if ( distToPlayer > VisionRange ) continue;
			var forward = Owner.WorldRotation.Forward.Normal;
			var dot = Vector3.Dot( forward, dirToPlayer );
			var halfAngle = VisionConeAngle / 2f;
			if ( dot < MathF.Cos( halfAngle * MathF.PI / 180f ) ) continue;
			var tr = Game.ActiveScene.Trace.Ray( Owner.EyePosition, player.EyePos )
				.WithoutTags( "trigger", "gib", "zombie", "npc" )
				.IgnoreGameObjectHierarchy( Owner.GameObject )
				.Run();
			if ( tr.Fraction >= 0.95f && distToPlayer < closestDist )
			{
				closestDist = distToPlayer;
				HasVisualContact = true;
				ClosestVisiblePlayer = player;
			}
		}
		if ( HasVisualContact ) Brain.TimeSinceLastSight = 0;
	}

	void CheckProximity()
	{
		IsPlayerInProximity = false;
		ClosestProximityPlayer = null;
		var players = Game.ActiveScene.GetAllComponents<HumanPlayer>().Where( p => p.IsAlive ).ToList();
		float closestDist = float.MaxValue;
		foreach ( var player in players )
		{
			var dist = (player.WorldPosition - Owner.WorldPosition).Length;
			var effectiveRadius = ProximityRadius;
			var vel = player.Velocity.WithZ( 0 ).Length;
			if ( vel > 200f ) effectiveRadius *= 1.5f;
			else if ( vel < 50f && player.CharacterController?.IsOnGround == true ) effectiveRadius *= 0.5f;
			if ( dist < effectiveRadius && dist < closestDist )
			{
				closestDist = dist;
				IsPlayerInProximity = true;
				ClosestProximityPlayer = player;
			}
		}
	}

	void CheckHearing()
	{
		ClosestNoise = NoiseSystem.GetLoudestInRange( Owner.WorldPosition, HearingRange );
	}
}
