using SWB.Shared;
using System;

namespace SWB.Player;

public enum MovementState
{
	Normal,
	Slide,
	Ladder,
	Noclip
}

public partial class PlayerBase
{
	[Property, Category( "Movement Feel" )] public float JumpCoyoteTimeThreshold { get; set; } = 0.10f;
	[Property, Category( "Movement Feel" )] public float StandingHeight { get; set; } = 0f;
	[Property, Category( "Movement Feel" )] public float CrouchHeight { get; set; } = 0f;
	[Property, Category( "Movement Feel" )] public float SlideHeight { get; set; } = 0f;
	[Property, Category( "Movement Feel|Crouch" )] public float CrouchTransitionTime { get; set; } = 0.25f;
	[Property, Category( "Movement Feel|Crouch" )] public float UncrouchTransitionTime { get; set; } = 0.125f;

	[Sync] public bool IsSliding { get; set; }
	[Sync] public MovementState CurrentMovementState { get; set; } = MovementState.Normal;

	public TimeSince TimeSinceLeftGround { get; set; } = 999;
	public TimeSince TimeSinceLastLanding { get; set; } = 999;
	public TimeSince TimeSinceSlideStarted { get; set; } = 999;
	public TimeSince TimeSinceSlideEnded { get; set; } = 999;
	public TimeSince TimeSinceBecameAirborne { get; set; } = 999;

	float standingColliderEndZ;
	float standingControllerHeight;
	float crouchControllerHeight;
	float slideControllerHeight;
	float pendingLandingFeedback;
	float landingImpactScaleTarget;
	float landingAnimationTimer;
	Vector3 slideDirection;
	Vector3 postSlideCarryDirection;
	bool preserveCrouchAfterSlide;
	bool postSlideCrouchAwaitingNeutral;
	bool postSlideCrouchReleaseArmed;
	bool suppressDuckAfterSlideStandUntilRelease;

	public float CurrentImpulsePower { get; private set; }
	public float SlideActivationTimeStamp { get; private set; }
	public float SlideActivationValidatedTimeStamp { get; private set; }
	public Vector3 SlideSteeringVelocity { get; private set; }
	public Vector3 InitialSlideImpulse { get; private set; }
	public Vector3 ImpartedSteeringVelocity { get; private set; }
	public Vector3 LastFrameHorizontalVelocity { get; private set; }
	public float PostSlideCarryStartSpeed { get; private set; }
	public bool bImpulsePowerIsTooLowToSlide { get; private set; }
	public bool bIsJumpingFromSlide { get; private set; }
	public bool bJustLanded { get; private set; }
	public bool bJumpedThisFrame { get; private set; }
	public float JustJumpedTimer { get; private set; }
	public bool bActivatedJump { get; private set; }
	public bool bIsZooming { get; private set; }
	public bool bIsScopedZoom { get; private set; }
	public float LastLandingImpactSize { get; private set; }
	public float LandingImpactScale { get; private set; }
	public float LastInAirTime { get; private set; }
	public float JustLandedTimer { get; private set; }
	public float CameraFovAdditive { get; private set; }
	public float JumpActivation { get; private set; }
	public float JumpAnimationProgress { get; private set; }
	public float FallAnimationStrength { get; private set; }
	public float LandingAnimationAlpha { get; private set; }
	public float LandingAnimationProgress { get; private set; }
	public float LandingAnimationStrength { get; private set; }
	public float FirstPersonCameraVerticalOffset { get; private set; }
	public float FirstPersonCameraPitchOffset { get; private set; }
	public float FirstPersonPostureOffset { get; private set; }
	public float SlideAnimationStrength { get; private set; }
	public float LadderAnimationStrength { get; private set; }
	public float ViewModelPostureBlend { get; private set; }
	public float ViewModelWalkAnimationWeight { get; private set; }
	public float ViewModelWalkAnimationCycleRate { get; private set; }
	public float ViewModelWalkAnimationMaxSpeed { get; private set; }
	public float ViewModelSprintAnimationWeight { get; private set; }
	public float ViewModelLadderTuckBlend { get; private set; }
	public float ViewModelTuckRangeScale { get; private set; }
	public float PendingLandingFeedback => pendingLandingFeedback;

	void InitializeMovementFeelState()
	{
		if ( CharacterController is null || BodyCollider is null )
			return;

		if ( StandingHeight <= 0f )
			StandingHeight = CharacterController.Height;
		standingControllerHeight = StandingHeight;

		if ( standingColliderEndZ <= 0f )
			standingColliderEndZ = BodyCollider.End.z > 0f ? BodyCollider.End.z : standingControllerHeight * 0.5f;

		if ( CrouchHeight <= 0f )
			CrouchHeight = standingControllerHeight * 0.5f;

		if ( SlideHeight <= 0f )
			SlideHeight = CrouchHeight * 0.8f;

		crouchControllerHeight = CrouchHeight;
		slideControllerHeight = SlideHeight;

		if ( CurrentImpulsePower <= 0f )
			CurrentImpulsePower = MaxSlideImpulseCap;

		bImpulsePowerIsTooLowToSlide = CurrentImpulsePower < ImpulseTooLowThreshold;
	}

	void ResetMovementFeelState()
	{
		IsSliding = false;
		IsCrouching = false;
		IsRunning = false;
		CurrentMovementState = MovementState.Normal;
		TimeSinceLeftGround = 999;
		TimeSinceLastLanding = 999;
		TimeSinceSlideStarted = 999;
		TimeSinceSlideEnded = 999;
		TimeSinceBecameAirborne = 999;
		pendingLandingFeedback = 0f;
		slideDirection = Vector3.Zero;
		postSlideCarryDirection = Vector3.Zero;
		preserveCrouchAfterSlide = false;
		postSlideCrouchAwaitingNeutral = false;
		postSlideCrouchReleaseArmed = false;
		suppressDuckAfterSlideStandUntilRelease = false;
		SlideSteeringVelocity = Vector3.Zero;
		InitialSlideImpulse = Vector3.Zero;
		ImpartedSteeringVelocity = Vector3.Zero;
		LastFrameHorizontalVelocity = Vector3.Zero;
		PostSlideCarryStartSpeed = 0f;
		CurrentImpulsePower = MaxSlideImpulseCap;
		SlideActivationTimeStamp = 0f;
		SlideActivationValidatedTimeStamp = 0f;
		LastLandingImpactSize = 0f;
		LandingImpactScale = 1f;
		landingImpactScaleTarget = 1f;
		LastInAirTime = 0f;
		JustJumpedTimer = 0f;
		JustLandedTimer = 0f;
		landingAnimationTimer = 0f;
		CameraFovAdditive = 0f;
		JumpActivation = 0f;
		JumpAnimationProgress = 0f;
		FallAnimationStrength = 0f;
		LandingAnimationAlpha = 0f;
		LandingAnimationProgress = 0f;
		LandingAnimationStrength = 0f;
		FirstPersonCameraVerticalOffset = 0f;
		FirstPersonCameraPitchOffset = 0f;
		FirstPersonPostureOffset = 0f;
		SlideAnimationStrength = 0f;
		LadderAnimationStrength = 0f;
		ViewModelPostureBlend = 0f;
		ViewModelWalkAnimationWeight = 1f;
		ViewModelWalkAnimationCycleRate = 16f;
		ViewModelWalkAnimationMaxSpeed = 200f;
		ViewModelSprintAnimationWeight = 0f;
		ViewModelLadderTuckBlend = 0f;
		ViewModelTuckRangeScale = 1f;
		bImpulsePowerIsTooLowToSlide = false;
		bIsJumpingFromSlide = false;
		bJustLanded = false;
		bJumpedThisFrame = false;
		bActivatedJump = false;
		bIsZooming = false;
		bIsScopedZoom = false;

		InitializeMovementFeelState();
		SetCharacterHeight( GetTargetStandingHeight() );
	}

	void UpdateMovementFeelFrameState()
	{
		bJustLanded = false;
		bJumpedThisFrame = false;
		JustJumpedTimer = MathF.Max( JustJumpedTimer - Time.Delta, 0f );
		JustLandedTimer = MathF.Max( JustLandedTimer - Time.Delta, 0f );
		landingAnimationTimer = MathF.Max( landingAnimationTimer - Time.Delta, 0f );

		UpdateLandingImpactScale();
		JumpAnimationProgress = EvaluateAnimationProgress( JustJumpedTimer, 0.31f );
		LandingAnimationAlpha = EvaluateLandingAnimationAlpha();
		LandingAnimationProgress = EvaluateAnimationProgress( landingAnimationTimer, 0.31f );
		LandingAnimationStrength = EvaluateLandingAnimationStrength();

		if ( IsSliding )
			return;

		CurrentImpulsePower = MathF.Min( CurrentImpulsePower + (ImpulseRecoveryPerSecond * Time.Delta), MaxSlideImpulseCap );
		bImpulsePowerIsTooLowToSlide = CurrentImpulsePower < ImpulseTooLowThreshold;
	}

	void UpdateLandingImpactScale()
	{
		var smoothness = 0.1f;
		if ( bIsZooming )
		{
			landingImpactScaleTarget = bIsScopedZoom ? 0.035f : 0.1f;
			if ( bIsScopedZoom )
				smoothness = 0.01f;
		}
		else
		{
			landingImpactScaleTarget = MovementFeelMath.EvaluateLandingImpactScale( LastInAirTime );
		}

		var lerp = 1f - MathF.Exp( -Time.Delta / MathF.Max( smoothness, 0.0001f ) );
		LandingImpactScale += (landingImpactScaleTarget - LandingImpactScale) * lerp;
		pendingLandingFeedback = LastLandingImpactSize * LandingImpactScale;
	}

	static float EvaluateAnimationProgress( float timer, float duration )
	{
		if ( timer <= 0f )
			return 0f;

		return 1f - Math.Clamp( timer / MathF.Max( duration, 0.0001f ), 0f, 1f );
	}

	float EvaluateLandingAnimationAlpha()
	{
		var landingElapsed = 1f - JustLandedTimer;
		return Math.Clamp( 1f - (landingElapsed / 0.15f), 0f, 1f );
	}

	float EvaluateLandingAnimationStrength()
	{
		if ( landingAnimationTimer <= 0f )
			return 0f;

		return Math.Clamp( PendingLandingFeedback + (LandingImpactScale * 0.25f), 0.35f, 1.25f );
	}

	void UpdateFirstPersonPresentationState()
	{
		var horizontalSpeed = Velocity.WithZ( 0 ).Length;
		var slideSpeedNormalized = Math.Clamp( horizontalSpeed / MathF.Max( MaxSlideSpeed, 1f ), 0f, 1f );
		var ladderSpeedNormalized = Math.Clamp( Velocity.Length / MathF.Max( WalkSpeed, 1f ), 0f, 1f );
		var sprintBlend = Math.Clamp( (horizontalSpeed - WalkSpeed) / MathF.Max( RunSpeed - WalkSpeed, 1f ), 0f, 1f );
		var slideExitBlend = !IsSliding ? Math.Clamp( 1f - (TimeSinceSlideEnded / MathF.Max( SlideExitPresentationTime, 0.0001f )), 0f, 1f ) : 0f;
		var slidePresentationBlend = IsSliding
			? Math.Clamp( TimeSinceSlideStarted / 0.12f, 0f, 1f )
			: slideExitBlend;

		SlideAnimationStrength = slidePresentationBlend > 0f
			? (0.35f + (0.65f * slideSpeedNormalized)) * slidePresentationBlend
			: 0f;

		LadderAnimationStrength = CurrentMovementState == MovementState.Ladder
			? 0.2f + (0.8f * ladderSpeedNormalized)
			: 0f;

		var crouchBlend = GetCrouchBlend();
		var crouchPostureOffset = crouchBlend * 32f;
		var slidePostureOffset = 32f + (SlideAnimationStrength * 4f);
		var targetPostureOffset = slideExitBlend > 0f
			? crouchPostureOffset + ((slidePostureOffset - crouchPostureOffset) * slideExitBlend)
			: crouchPostureOffset;
		if ( IsSliding )
			targetPostureOffset = slidePostureOffset;
		if ( CurrentMovementState == MovementState.Ladder )
			targetPostureOffset = Math.Max( targetPostureOffset, 6f + (LadderAnimationStrength * 6f) );

		FirstPersonPostureOffset = targetPostureOffset;

		ViewModelPostureBlend = Math.Clamp( FirstPersonPostureOffset / 32f, 0f, 1.25f );
		ViewModelWalkAnimationWeight = CurrentMovementState == MovementState.Normal && IsOnGround ? 1f : 0f;
		ViewModelWalkAnimationCycleRate = IsRunning ? 18f : 16f;
		ViewModelWalkAnimationMaxSpeed = IsRunning ? 100f : 200f;
		ViewModelSprintAnimationWeight = CurrentMovementState == MovementState.Normal && IsOnGround && IsRunning ? sprintBlend : 0f;
		ViewModelLadderTuckBlend = LadderAnimationStrength;
		ViewModelTuckRangeScale = 1f + (ViewModelLadderTuckBlend * 0.25f);
	}

	void UpdateFirstPersonCameraState()
	{
		FirstPersonCameraVerticalOffset = 0f;
		FirstPersonCameraPitchOffset = 0f;

		if ( JustJumpedTimer > 0f )
		{
			var jumpArc = MovementFeelMath.EvaluatePresentationArc( JumpAnimationProgress );
			var jumpStrength = 0.6f + (0.4f * Math.Clamp( JumpActivation, 0f, 1f ));
			FirstPersonCameraVerticalOffset = jumpArc * 4f * jumpStrength;
			FirstPersonCameraPitchOffset = -jumpArc * 1.5f * jumpStrength;
			return;
		}

		if ( FallAnimationStrength > 0f )
		{
			FirstPersonCameraVerticalOffset = 0.5f + (FallAnimationStrength * 1.0f);
			FirstPersonCameraPitchOffset = FallAnimationStrength * 0.35f;
			return;
		}

		if ( bJustLanded || LandingAnimationProgress > 0f )
		{
			var landingArc = MovementFeelMath.EvaluatePresentationArc( LandingAnimationProgress );
			var landingBlend = Math.Max( LandingAnimationAlpha, 0.25f );
			var landingStrength = landingArc * LandingAnimationStrength * landingBlend;
			FirstPersonCameraVerticalOffset = landingStrength * 2f;
			FirstPersonCameraPitchOffset = landingStrength;
			return;
		}

		if ( SlideAnimationStrength > 0f )
		{
			FirstPersonCameraVerticalOffset = 0.35f + (SlideAnimationStrength * 0.65f);
			FirstPersonCameraPitchOffset = 0.25f + (SlideAnimationStrength * 0.5f);
			return;
		}

		if ( LadderAnimationStrength > 0f )
		{
			var ladderWave = MathF.Sin( (float)RealTime.Now * (6f + (LadderAnimationStrength * 4f)) );
			var ladderLift = Math.Abs( ladderWave ) * LadderAnimationStrength;
			FirstPersonCameraVerticalOffset = ladderLift * 0.6f;
			FirstPersonCameraPitchOffset = ladderWave * LadderAnimationStrength * 0.35f;
		}
	}

	void UpdateMovementFeelContinuousState()
	{
		bActivatedJump = bJumpedThisFrame || ( !IsOnGround && Velocity.z > 10f );
		JumpActivation = bActivatedJump && !IsOnGround ? Math.Clamp( Velocity.z / Math.Max( JumpForce, 1f ), 0f, 1f ) : 0f;
		FallAnimationStrength = 0f;
		if ( bActivatedJump && !IsOnGround )
		{
			var airtimeStrength = Math.Clamp( LastInAirTime / 0.35f, 0f, 1f );
			var descentStrength = Math.Clamp( -Velocity.z / Math.Max( JumpForce, 1f ), 0f, 1f );
			FallAnimationStrength = Math.Clamp( Math.Max( airtimeStrength, descentStrength ), 0.2f, 1f );
		}

		UpdateFirstPersonPresentationState();
		UpdateFirstPersonCameraState();
	}

	void SetJumpedThisFrame()
	{
		bJumpedThisFrame = true;
		JustJumpedTimer = 0.31f;
		bActivatedJump = true;
		JumpActivation = 1f;
		JumpAnimationProgress = 0f;
	}

	void SetLandingState( Vector3 velocity )
	{
		bJustLanded = true;
		JustLandedTimer = 1f;
		landingAnimationTimer = 0.31f;
		LandingAnimationAlpha = 1f;
		LandingAnimationProgress = 0f;
		LastLandingImpactSize = MovementFeelMath.EvaluateLandingFeedback( -velocity.z, SafeFallSpeed, LethalFallSpeed );
		pendingLandingFeedback = LastLandingImpactSize * LandingImpactScale;
		LandingAnimationStrength = EvaluateLandingAnimationStrength();
	}

	public void SetZoomState( bool isZooming, bool isScopedZoom )
	{
		bIsZooming = isZooming;
		bIsScopedZoom = isZooming && isScopedZoom;
	}

	public void SetCameraFovAdditive( float value )
	{
		CameraFovAdditive = value;
	}

	void SetCurrentImpulsePower( float value )
	{
		CurrentImpulsePower = value;
		bImpulsePowerIsTooLowToSlide = CurrentImpulsePower < ImpulseTooLowThreshold;
	}

	float GetTargetStandingHeight()
	{
		return standingControllerHeight > 0f ? standingControllerHeight : CharacterController?.Height ?? 0f;
	}

	float GetTargetCrouchHeight()
	{
		return crouchControllerHeight > 0f ? crouchControllerHeight : CrouchHeight;
	}

	float GetTargetSlideHeight()
	{
		return slideControllerHeight > 0f ? slideControllerHeight : SlideHeight;
	}

	float GetCrouchBlend()
	{
		if ( CharacterController is null )
			return 0f;

		var standingHeight = GetTargetStandingHeight();
		var crouchHeight = GetTargetCrouchHeight();
		var heightRange = MathF.Max( standingHeight - crouchHeight, 0.001f );
		var crouchBlend = (standingHeight - CharacterController.Height) / heightRange;
		return Math.Clamp( crouchBlend, 0f, 1f );
	}

	float GetPostSlideCarryEndSpeed()
	{
		return IsCrouching ? CrouchSpeed : (IsRunning ? RunSpeed : WalkSpeed);
	}

	float GetProtectedSlideExitSpeed()
	{
		return ExitSlideVelocity;
	}

	bool ShouldPreserveCrouchAfterSlide()
	{
		return preserveCrouchAfterSlide && !stickyActiveButtons.Contains( InputButtonHelper.Duck );
	}

	void BeginPostSlideCrouchLock()
	{
		preserveCrouchAfterSlide = true;
		postSlideCrouchAwaitingNeutral = true;
		postSlideCrouchReleaseArmed = false;
	}

	void ClearPostSlideCrouchLock()
	{
		preserveCrouchAfterSlide = false;
		postSlideCrouchAwaitingNeutral = false;
		postSlideCrouchReleaseArmed = false;
	}

	bool IsDuckSuppressed()
	{
		return suppressDuckAfterSlideStandUntilRelease;
	}

	void BeginDuckSuppressUntilRelease()
	{
		suppressDuckAfterSlideStandUntilRelease = Input.Down( InputButtonHelper.Duck );
		stickyActiveButtons.Remove( InputButtonHelper.Duck );
	}

	void UpdateDuckSuppressUntilRelease()
	{
		if ( suppressDuckAfterSlideStandUntilRelease && !Input.Down( InputButtonHelper.Duck ) )
			suppressDuckAfterSlideStandUntilRelease = false;
	}

	void UpdatePostSlideCrouchLock()
	{
		if ( !ShouldPreserveCrouchAfterSlide() )
			return;

		if ( postSlideCrouchAwaitingNeutral )
		{
			if ( !IsDuckHeld() )
				postSlideCrouchAwaitingNeutral = false;
			return;
		}

		if ( !postSlideCrouchReleaseArmed )
		{
			if ( Input.Pressed( InputButtonHelper.Duck ) )
				postSlideCrouchReleaseArmed = true;
			return;
		}

		if ( Input.Released( InputButtonHelper.Duck ) )
			ClearPostSlideCrouchLock();
	}

	bool HasPostSlideCarry()
	{
		return !IsSliding
			&& IsOnGround
			&& MovementFeelMath.ShouldApplyPostSlideCarry( TimeSinceSlideEnded, PostSlideCarryTime, PostSlideCarryStartSpeed, GetPostSlideCarryEndSpeed() );
	}

	float GetPostSlideCarryTargetSpeed()
	{
		return MovementFeelMath.EvaluatePostSlideCarrySpeed( TimeSinceSlideEnded, PostSlideCarryTime, PostSlideCarryStartSpeed, GetPostSlideCarryEndSpeed() );
	}

	void CapturePostSlideCarry( Vector3 velocity )
	{
		var horizontalVelocity = velocity.WithZ( 0 );
		var endSpeed = GetPostSlideCarryEndSpeed();
		if ( !IsOnGround || horizontalVelocity.IsNearZeroLength || horizontalVelocity.Length <= endSpeed )
		{
			ClearPostSlideCarry();
			return;
		}

		PostSlideCarryStartSpeed = horizontalVelocity.Length;
		postSlideCarryDirection = horizontalVelocity.Normal;
	}

	void ClearPostSlideCarry()
	{
		PostSlideCarryStartSpeed = 0f;
		postSlideCarryDirection = Vector3.Zero;
	}

	void ApplyPostSlideCarry()
	{
		if ( CharacterController is null || !HasPostSlideCarry() )
			return;

		var targetSpeed = GetPostSlideCarryTargetSpeed();
		var horizontalVelocity = CharacterController.Velocity.WithZ( 0 );
		var wishDirection = WishVelocity.WithZ( 0 );
		var hasInput = !wishDirection.IsNearZeroLength;
		var inputAlignment = hasInput && !postSlideCarryDirection.IsNearZeroLength
			? wishDirection.Normal.Dot( postSlideCarryDirection )
			: 1f;
		if ( !MovementFeelMath.ShouldPreservePostSlideCarry( hasInput, inputAlignment ) )
		{
			ClearPostSlideCarry();
			return;
		}

		var carryDirection = !postSlideCarryDirection.IsNearZeroLength
			? postSlideCarryDirection
			: (!horizontalVelocity.IsNearZeroLength ? horizontalVelocity.Normal : Vector3.Zero);
		if ( hasInput && inputAlignment > 0.35f )
		{
			var blendedDirection = Vector3.Lerp( carryDirection, wishDirection.Normal, 0.35f );
			if ( !blendedDirection.IsNearZeroLength )
				carryDirection = blendedDirection.Normal;
		}

		if ( carryDirection.IsNearZeroLength )
			return;

		CharacterController.Velocity = (carryDirection * targetSpeed).WithZ( CharacterController.Velocity.z );
	}

	Vector3 GetAnimationWishVelocity()
	{
		if ( !HasPostSlideCarry() )
			return WishVelocity;

		var targetSpeed = GetPostSlideCarryTargetSpeed();
		var wishDirection = WishVelocity.WithZ( 0 );
		if ( !wishDirection.IsNearZeroLength )
		{
			var inputAlignment = postSlideCarryDirection.IsNearZeroLength ? 1f : wishDirection.Normal.Dot( postSlideCarryDirection );
			if ( inputAlignment > 0f )
				return wishDirection.Normal * MathF.Max( WishVelocity.Length, targetSpeed );

			return WishVelocity;
		}

		if ( postSlideCarryDirection.IsNearZeroLength )
			return WishVelocity;

		return postSlideCarryDirection * targetSpeed;
	}

	void UpdateCharacterPosture()
	{
		InitializeMovementFeelState();
		if ( CharacterController is null )
			return;

		var targetHeight = IsSliding ? GetTargetSlideHeight() : (IsCrouching ? GetTargetCrouchHeight() : GetTargetStandingHeight());
		var currentHeight = CharacterController.Height;
		if ( MathF.Abs( currentHeight - targetHeight ) <= 0.01f )
		{
			SetCharacterHeight( targetHeight );
			return;
		}

		// Skip lerp transition when recovering from slide to avoid collision issues
		// (slide sets height instantly, so recovery should also be instant)
		var isRecoveringFromSlide = !IsSliding && TimeSinceSlideEnded < 0.3f;
		if ( isRecoveringFromSlide )
		{
			SetCharacterHeight( targetHeight );
			return;
		}

		var transitionTime = targetHeight < currentHeight ? CrouchTransitionTime : UncrouchTransitionTime;
		var lerp = Math.Clamp( Time.Delta / MathF.Max( transitionTime, 0.0001f ), 0f, 1f );
		SetCharacterHeight( currentHeight + ((targetHeight - currentHeight) * lerp) );
	}
}
