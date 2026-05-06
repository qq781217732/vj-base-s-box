using SWB.Shared;
using System;

namespace SWB.Player;

public partial class PlayerBase
{
	[Property, Category( "Movement Feel|Slide" )] public float RequiredHorizonalVelocityForBackwardsLandSlide { get; set; } = 800f;
	[Property, Category( "Movement Feel|Slide" )] public float RequiredHorizontalVelocity { get; set; } = 300f;
	[Property, Category( "Movement Feel|Slide" )] public float RequiredSpeed { get; set; } = 300f;
	[Property, Category( "Movement Feel|Slide" )] public float MaxLookAtVelocityDeviationAngle { get; set; } = 80f;
	[Property, Category( "Movement Feel|Slide" )] public float InitialImpulseDuration { get; set; } = 0.15f;
	[Property, Category( "Movement Feel|Slide" )] public float InitialImpulseAcceleration { get; set; } = 10000f;
	[Property, Category( "Movement Feel|Slide" )] public float ExitSlideVelocity { get; set; } = 480f;
	[Property, Category( "Movement Feel|Slide" )] public float MaxSlideSpeed { get; set; } = 1250f;
	[Property, Category( "Movement Feel|Slide" )] public float BlockStandUpTime { get; set; } = 0.5f;
	[Property, Category( "Movement Feel|Slide" )] public float SlideExitPresentationTime { get; set; } = 0.12f;
	[Property, Category( "Movement Feel|Slide" )] public float PostSlideCarryTime { get; set; } = 0.20f;
	[Property, Category( "Movement Feel|Slide" )] public float PostSlideCarryFrictionScale { get; set; } = 0.25f;
	[Property, Category( "Movement Feel|Slide" )] public float TimeBeforeEnableJump { get; set; } = 0.15f;
	[Property, Category( "Movement Feel|Slide" )] public float SlideJumpHorizontalBoostScale { get; set; } = 0f;
	[Property, Category( "Movement Feel|Slide" )] public float SlideJumpVelocityCarryScale { get; set; } = 0.75f;
	[Property, Category( "Movement Feel|Slide" )] public float MinSlopeAngleToAffectMovement { get; set; } = 3.5f;
	[Property, Category( "Movement Feel|Slide" )] public float SlideSteeringSpeed { get; set; } = 250f;
	[Property, Category( "Movement Feel|Slide" )] public float SlideSteeringAcceleration { get; set; } = 500f;
	[Property, Category( "Movement Feel|Slide" )] public float SlideSteeringBrakingDeceleration { get; set; } = 50f;
	[Property, Category( "Movement Feel|Slide" )] public float SlideSteeringTurnDeceleration { get; set; } = 800f;
	[Property, Category( "Movement Feel|Slide" )] public float SlideCooldown { get; set; } = 0.25f;
	[Property, Category( "Movement Feel|Slide|Slope Curves" )] public Curve SlopeTargetSpeedCurve { get; set; } = new Curve( new Curve.Frame( 0f, 0f ), new Curve.Frame( 20f, 1000f ) );
	[Property, Category( "Movement Feel|Slide|Slope Curves" )] public Curve SlopeAccelerationCurve { get; set; } = new Curve( new Curve.Frame( 0f, 1000f ), new Curve.Frame( 20f, 1000f ) );
	[Property, Category( "Movement Feel|Slide|Slope Curves" )] public Curve SlopeDecelerationCurve { get; set; } = new Curve( new Curve.Frame( 0f, 500f ), new Curve.Frame( 20f, 500f ) );
	[Property, Category( "Movement Feel|Slide" )] public float MaxSlideImpulseCap { get; set; } = 950f;
	[Property, Category( "Movement Feel|Slide" )] public float ImpulseReductionPerSlide { get; set; } = 250f;
	[Property, Category( "Movement Feel|Slide" )] public float MinSlideImpulseCap { get; set; } = 480f;
	[Property, Category( "Movement Feel|Slide" )] public float ImpulseTooLowThreshold { get; set; } = 500f;
	[Property, Category( "Movement Feel|Slide" )] public float ImpulseRecoveryPerSecond { get; set; } = 100f;
	[Property, Category( "Movement Feel|Slide" )] public bool AutoCrouchWhenSlideUnavailable { get; set; } = true;

	bool CanStartSlide()
	{
		InitializeMovementFeelState();

		if ( bImpulsePowerIsTooLowToSlide )
			return false;

		var horizontalVelocity = Velocity.WithZ( 0 );
		var groundMovementSpeed = horizontalVelocity.Length;
		var cooldownReady = TimeSinceSlideEnded >= SlideCooldown;
		if ( !MovementFeelMath.CanEnterSlide( groundMovementSpeed, IsOnGround, IsRunning, IsCrouching, IsClimbingLadder, cooldownReady, RequiredHorizontalVelocity ) )
			return false;

		if ( Velocity.Length < RequiredSpeed )
			return false;

		var lookAtVelocityDegrees = MovementFeelMath.GetLookAtVelocityDegrees( Camera.WorldRotation, horizontalVelocity );
		if ( lookAtVelocityDegrees > MaxLookAtVelocityDeviationAngle && groundMovementSpeed < RequiredHorizonalVelocityForBackwardsLandSlide )
			return false;

		return true;
	}

	void StartSlide()
	{
		InitializeMovementFeelState();
		if ( Inventory?.ActiveItem is SWB.Base.Weapon activeWeapon && activeWeapon.AdsCancelOnSlideStart )
			activeWeapon.CancelAim();

		IsSliding = true;
		bIsJumpingFromSlide = false;
		CurrentMovementState = MovementState.Slide;
		TimeSinceSlideStarted = 0;
		SlideActivationTimeStamp = (float)RealTime.Now;
		SlideActivationValidatedTimeStamp = (float)RealTime.Now;
		SlideSteeringVelocity = Vector3.Zero;
		ImpartedSteeringVelocity = Vector3.Zero;
		LastFrameHorizontalVelocity = Velocity.WithZ( 0 );
		ClearPostSlideCarry();
		ClearPostSlideCrouchLock();

		slideDirection = Velocity.WithZ( 0 );
		if ( slideDirection.IsNearZeroLength )
			slideDirection = Camera.WorldRotation.Forward.WithZ( 0 );
		if ( !slideDirection.IsNearZeroLength )
			slideDirection = slideDirection.Normal;

		var slopeDirection2D = GetDownSlopeDirection();
		var slopeAngle = GetGroundSlopeAngle();
		var velocityDirection2D = slideDirection.IsNearZeroLength ? Vector3.Zero : slideDirection.Normal;
		var initialImpulsePower = GetInitiateSlideImpulsePower( slopeDirection2D, velocityDirection2D, slopeAngle );
		InitialSlideImpulse = slideDirection * initialImpulsePower;

		var cappedCurrentImpulsePower = MathF.Min( CurrentImpulsePower, MaxSlideImpulseCap );
		SetCurrentImpulsePower( MathF.Max( MinSlideImpulseCap, cappedCurrentImpulsePower - ImpulseReductionPerSlide ) );

		SetCharacterHeight( GetTargetSlideHeight() );
		stickyActiveButtons.Remove( InputButtonHelper.Run );
	}

	void StopSlide( bool restoreCrouch )
	{
		InitializeMovementFeelState();
		var jumpedFromSlide = bIsJumpingFromSlide;
		IsSliding = false;
		bIsJumpingFromSlide = false;
		TimeSinceSlideEnded = 0;
		CurrentMovementState = IsClimbingLadder ? MovementState.Ladder : (Noclip ? MovementState.Noclip : MovementState.Normal);
		IsRunning = false;

		// Preserve horizontal velocity for smooth transition
		var preservedVelocity = CharacterController?.Velocity ?? Vector3.Zero;

		if ( restoreCrouch )
		{
			IsCrouching = true;
			TimeSinceCrouch = 0;
			stickyActiveButtons.Remove( InputButtonHelper.Run );
			BeginPostSlideCrouchLock();
		}
		else
		{
			IsCrouching = false;
			ClearPostSlideCrouchLock();
		}

		if ( jumpedFromSlide )
			ClearPostSlideCarry();
		else
			CapturePostSlideCarry( preservedVelocity );

		// Restore velocity after state changes
		if ( CharacterController is not null )
			CharacterController.Velocity = preservedVelocity;

		slideDirection = Vector3.Zero;
		SlideSteeringVelocity = Vector3.Zero;
		InitialSlideImpulse = Vector3.Zero;
		ImpartedSteeringVelocity = Vector3.Zero;
		LastFrameHorizontalVelocity = Vector3.Zero;
	}

	float EvaluateSlopeTargetSpeed( float slopeAngle )
	{
		return SlopeTargetSpeedCurve.Evaluate( slopeAngle );
	}

	float EvaluateSlopeAcceleration( float slopeAngle )
	{
		return SlopeAccelerationCurve.Evaluate( slopeAngle );
	}

	float EvaluateSlopeDeceleration( float slopeAngle )
	{
		return SlopeDecelerationCurve.Evaluate( slopeAngle );
	}

	bool WantsToStandUpFromSlideThisFrame( bool hasHeadroom )
	{
		if ( TimeSinceSlideStarted <= BlockStandUpTime || !hasHeadroom )
			return false;

		return WantsToEnterLowProfileThisFrame();
	}

	void SlideMove()
	{
		InitializeMovementFeelState();

		var groundNormal = GetGroundNormal();
		var horizontalVelocity = CalculateHorizontallyProjectedVelocity( groundNormal, CharacterController.Velocity );
		var characterSpeed = horizontalVelocity.Length;
		var hasHeadroom = CanStandFromSlide();
		var wantsToStandUp = WantsToStandUpFromSlideThisFrame( hasHeadroom );
		if ( wantsToStandUp || ShouldExitSlide( characterSpeed, true, hasHeadroom ) )
		{
			StopSlide( !wantsToStandUp );
			if ( wantsToStandUp )
				BeginDuckSuppressUntilRelease();
			UpdateCharacterPosture();
			BuildWishVelocity();

			if ( Noclip )
				NoclipMove();
			else if ( IsClimbingLadder )
				LadderMove();
			else
				Move();
			return;
		}

		UpdateSlideSteeringVelocity();

		var slopeDirection2D = GetDownSlopeDirection();
		var slopeAngle = GetGroundSlopeAngle();
		var nonSteeringVelocity = CalculateSlideVelocityExcludingSteering( horizontalVelocity, slopeDirection2D, slopeAngle ).ClampLength( MaxSlideSpeed );
		var velocitySum = nonSteeringVelocity + SlideSteeringVelocity;
		var totalSlideSpeedSquared = MathF.Min( velocitySum.LengthSquared, nonSteeringVelocity.LengthSquared );
		var totalSlideVelocity = velocitySum.IsNearZeroLength ? Vector3.Zero : velocitySum.Normal * MathF.Sqrt( totalSlideSpeedSquared );
		var protectedExitSpeed = GetProtectedSlideExitSpeed();
		if ( totalSlideVelocity.Length <= protectedExitSpeed )
		{
			var exitDirection = !totalSlideVelocity.IsNearZeroLength
				? totalSlideVelocity.Normal
				: (!horizontalVelocity.IsNearZeroLength ? horizontalVelocity.Normal : slideDirection);
			var protectedExitVelocity = exitDirection.IsNearZeroLength
				? totalSlideVelocity
				: exitDirection * protectedExitSpeed;

			CharacterController.Velocity = CalculateGroundProjectedVelocity( groundNormal, protectedExitVelocity );
			StopSlide( true );
			UpdateCharacterPosture();
			BuildWishVelocity();

			if ( Noclip )
				NoclipMove();
			else if ( IsClimbingLadder )
				LadderMove();
			else
				Move();
			return;
		}

		ImpartedSteeringVelocity = totalSlideVelocity - nonSteeringVelocity;
		CharacterController.Velocity = CalculateGroundProjectedVelocity( groundNormal, totalSlideVelocity );
		LastFrameHorizontalVelocity = CalculateHorizontallyProjectedVelocity( groundNormal, CharacterController.Velocity );
		if ( !CharacterController.Velocity.IsNearZeroLength )
			CharacterController.Move();
	}

	void SlideJump()
	{
		if ( !IsSliding || TimeSinceSlideStarted < TimeBeforeEnableJump )
			return;

		var slopeDirection2D = GetDownSlopeDirection();
		var slopeAngle = GetGroundSlopeAngle();
		var velocity2D = CharacterController.Velocity.WithZ( 0 );
		var velocityDirection2D = velocity2D.IsNearZeroLength ? slideDirection : velocity2D.Normal;
		var downwardsAlignment = MovementFeelMath.CalculateDownwardsAlignment( 5f, slopeDirection2D, velocityDirection2D, slopeAngle );
		var horizontalMultiplier = 0.8f + (0.2f * downwardsAlignment);
		var horizontalJumpVelocity = velocity2D * (SlideJumpHorizontalBoostScale * horizontalMultiplier);
		var retainedHorizontalVelocity = velocity2D * SlideJumpVelocityCarryScale;
		var jumpVelocity = horizontalJumpVelocity + (Vector3.Up * JumpForce);
		var recoveredImpulsePower = CurrentImpulsePower + ((MaxSlideImpulseCap - CurrentImpulsePower) * downwardsAlignment);

		bIsJumpingFromSlide = true;
		SetCurrentImpulsePower( recoveredImpulsePower );
		StopSlide( false );
		CharacterController.Velocity = retainedHorizontalVelocity.WithZ( CharacterController.Velocity.z );
		CharacterController.Punch( jumpVelocity );
		AnimationHelper?.TriggerJump();
		SetJumpedThisFrame();
		TimeSinceLeftGround = 0;
		TimeSinceBecameAirborne = 0;
		groundedCheck = false;

		var tr = GetSurfaceTrace();
		if ( tr.Hit )
			PlayFootLaunchSound( tr.Surface, jumpVelocity );
	}

	Vector3 GetGroundNormal()
	{
		var tr = GetSurfaceTrace();
		return tr.Hit ? tr.Normal : Vector3.Up;
	}

	float GetGroundSlopeAngle()
	{
		var groundNormal = GetGroundNormal();
		var dot = groundNormal.Dot( Vector3.Up ).Clamp( -1f, 1f );
		return MathF.Acos( dot ) * 180f / MathF.PI;
	}

	Vector3 GetDownSlopeDirection()
	{
		var groundNormal = GetGroundNormal();
		var gravityDir = Scene.PhysicsWorld.Gravity.Normal;
		var downSlope = gravityDir - (groundNormal * gravityDir.Dot( groundNormal ));
		return downSlope.IsNearZeroLength ? Vector3.Zero : downSlope.Normal;
	}

	Vector3 CalculateHorizontallyProjectedVelocity( Vector3 groundNormal, Vector3 velocity )
	{
		var projectedVelocity = velocity - (groundNormal * velocity.Dot( groundNormal ));
		var horizontalDirection = velocity.WithZ( 0 );
		if ( projectedVelocity.IsNearZeroLength || horizontalDirection.IsNearZeroLength )
			return Vector3.Zero;

		return horizontalDirection.Normal * projectedVelocity.Length;
	}

	Vector3 CalculateGroundProjectedVelocity( Vector3 groundNormal, Vector3 velocity )
	{
		var groundAlignedDirection = velocity - (groundNormal * velocity.Dot( groundNormal ));
		if ( groundAlignedDirection.IsNearZeroLength || velocity.IsNearZeroLength )
			return Vector3.Zero;

		return groundAlignedDirection.Normal * velocity.Length;
	}

	bool CanStandFromSlide()
	{
		return CanStandAtHeight( GetTargetStandingHeight() );
	}

	bool CanStandAtHeight( float targetHeight )
	{
		var additionalHeight = Math.Max( 0f, targetHeight - CharacterController.Height ) + 4f;
		if ( additionalHeight <= 0f )
			return true;

		var upTrace = CharacterController.TraceDirection( Vector3.Up * additionalHeight );
		return !upTrace.Hit;
	}

	void SetCharacterHeight( float height )
	{
		if ( CharacterController is null || BodyCollider is null || height <= 0f )
			return;

		CharacterController.Height = height;
		var scale = standingControllerHeight > 0f ? height / standingControllerHeight : 1f;
		BodyCollider.End = BodyCollider.End.WithZ( standingColliderEndZ * scale );
	}

	bool ShouldExitSlide( float characterSpeed, bool wantsToStayLow, bool hasHeadroom )
	{
		return MovementFeelMath.ShouldExitSlide( characterSpeed, LastFrameHorizontalVelocity.Length, ExitSlideVelocity, IsOnGround, wantsToStayLow, hasHeadroom, TimeSinceSlideStarted <= BlockStandUpTime );
	}

	void UpdateSlideSteeringVelocity()
	{
		var desiredSteeringVelocity = Vector3.Zero;
		var steerDirection = WishVelocity.WithZ( 0 );
		if ( !steerDirection.IsNearZeroLength )
			desiredSteeringVelocity = steerDirection.Normal * SlideSteeringSpeed;

		var steeringAcceleration = SlideSteeringAcceleration;
		if ( desiredSteeringVelocity.IsNearZeroLength )
		{
			steeringAcceleration = SlideSteeringBrakingDeceleration;
		}
		else if ( !SlideSteeringVelocity.IsNearZeroLength && SlideSteeringVelocity.Dot( desiredSteeringVelocity ) < 0f )
		{
			steeringAcceleration = SlideSteeringTurnDeceleration;
		}

		var lerp = Math.Clamp( Time.Delta * (steeringAcceleration / MathF.Max( SlideSteeringSpeed, 1f )), 0f, 1f );
		SlideSteeringVelocity = Vector3.Lerp( SlideSteeringVelocity, desiredSteeringVelocity, lerp ).ClampLength( SlideSteeringSpeed );
	}

	Vector3 CalculateSlideVelocityExcludingSteering( Vector3 velocity, Vector3 slopeDirection2D, float slopeAngle )
	{
		var flatSlopeDeceleration = EvaluateSlopeDeceleration( 0f );
		var velocityExcludingSteering = velocity - ImpartedSteeringVelocity;
		if ( ((float)RealTime.Now - SlideActivationValidatedTimeStamp) < InitialImpulseDuration )
		{
			var targetVelocity = InitialSlideImpulse.ClampLength( MaxSlideSpeed );
			var lerp = Math.Clamp( Time.Delta * (InitialImpulseAcceleration / MathF.Max( MaxSlideSpeed, 1f )), 0f, 1f );
			velocityExcludingSteering = Vector3.Lerp( velocityExcludingSteering, targetVelocity, lerp );
		}

		if ( slopeAngle < MinSlopeAngleToAffectMovement || slopeDirection2D.IsNearZeroLength )
			return DecelerateVelocity( velocityExcludingSteering, flatSlopeDeceleration );

		var movementDirection2D = velocity.IsNearZeroLength ? Vector3.Zero : velocity.Normal;
		var velocitySlopeAlignment = movementDirection2D.IsNearZeroLength ? 0f : movementDirection2D.Dot( slopeDirection2D );
		if ( velocitySlopeAlignment < 0f )
		{
			var deceleratedVelocity = DecelerateVelocity( velocityExcludingSteering, flatSlopeDeceleration );
			var additionalUphillDeceleration = slopeDirection2D * MovementFeelMath.CalculateAdditionalUphillDeceleration( velocitySlopeAlignment, slopeAngle ) * Time.Delta;
			return deceleratedVelocity + additionalUphillDeceleration;
		}

		var slopeDirectedVelocityLength = velocityExcludingSteering.Dot( slopeDirection2D );
		var velocityPerpendicularToSlope = velocityExcludingSteering - (slopeDirection2D * slopeDirectedVelocityLength);
		var deceleratedVelocityPerpendicularToSlope = DecelerateVelocity( velocityPerpendicularToSlope, flatSlopeDeceleration );
		var desiredSlopeSpeed = EvaluateSlopeTargetSpeed( slopeAngle );
		var deltaToDesiredSpeed = desiredSlopeSpeed - slopeDirectedVelocityLength;

		float downhillAccelerationAmount;
		if ( velocity.Dot( slopeDirection2D ) <= desiredSlopeSpeed )
		{
			var downhillAcceleration = EvaluateSlopeAcceleration( slopeAngle );
			var uncappedAcceleration = downhillAcceleration * CalculateDownhillAccelerationMultiplier( velocitySlopeAlignment, slopeDirection2D ) * Time.Delta;
			downhillAccelerationAmount = MathF.Min( deltaToDesiredSpeed, uncappedAcceleration );
		}
		else
		{
			var slopeDeceleration = EvaluateSlopeDeceleration( slopeAngle );
			downhillAccelerationAmount = MathF.Max( deltaToDesiredSpeed, slopeDeceleration * Time.Delta * -1f );
		}

		var slopeVelocity = slopeDirection2D * (slopeDirectedVelocityLength + downhillAccelerationAmount);
		return deceleratedVelocityPerpendicularToSlope + slopeVelocity;
	}

	float CalculateDownhillAccelerationMultiplier( float velocitySlopeAlignment, Vector3 slopeDirection2D )
	{
		var inputDirection = WishVelocity.WithZ( 0 );
		var inputAlignment = inputDirection.IsNearZeroLength ? 0f : inputDirection.Normal.Dot( slopeDirection2D );
		var targetAlignment = MathF.Max( inputAlignment, velocitySlopeAlignment );
		return Math.Clamp( 0.5f + (MathF.Max( targetAlignment, 0f ) * 0.5f), 0.25f, 1f );
	}

	float GetInitiateSlideImpulsePower( Vector3 slopeDirection2D, Vector3 velocityDirection2D, float slopeAngle )
	{
		var cappedCurrentImpulsePower = MathF.Min( CurrentImpulsePower, MaxSlideImpulseCap );
		var slopeRecoveredImpulsePower = cappedCurrentImpulsePower;
		if ( !slopeDirection2D.IsNearZeroLength && !velocityDirection2D.IsNearZeroLength )
		{
			var downwardsAlignment = MovementFeelMath.CalculateDownwardsAlignment( 10f, slopeDirection2D, velocityDirection2D, slopeAngle );
			slopeRecoveredImpulsePower = cappedCurrentImpulsePower + ((MaxSlideImpulseCap - cappedCurrentImpulsePower) * downwardsAlignment);
		}

		return Math.Clamp( slopeRecoveredImpulsePower, MinSlideImpulseCap, MaxSlideImpulseCap );
	}

	Vector3 DecelerateVelocity( Vector3 velocity, float deceleration )
	{
		if ( velocity.IsNearZeroLength )
			return Vector3.Zero;

		var speed = MathF.Max( 0f, velocity.Length - (deceleration * Time.Delta) );
		if ( speed <= 0f )
			return Vector3.Zero;

		return velocity.Normal * speed;
	}
}