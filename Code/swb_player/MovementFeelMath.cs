using System;

namespace SWB.Player;

public static class MovementFeelMath
{
	public static bool CanUseGroundOrCoyoteJump( bool isGrounded, float timeSinceLeftGround, float jumpCoyoteTimeThreshold )
	{
		return isGrounded || timeSinceLeftGround <= jumpCoyoteTimeThreshold;
	}

	public static bool CanEnterSlide( float horizontalSpeed, bool isGrounded, bool isRunning, bool isCrouching, bool isOnLadder, bool cooldownReady, float minSpeed )
	{
		return isGrounded && isRunning && !isCrouching && !isOnLadder && cooldownReady && horizontalSpeed >= minSpeed;
	}

	public static float GetLookAtVelocityDegrees( Rotation viewRotation, Vector3 velocity2D )
	{
		if ( velocity2D.IsNearZeroLength )
			return 0f;

		var viewDirection = viewRotation.Forward.WithZ( 0 );
		if ( viewDirection.IsNearZeroLength )
			return 0f;

		var dot = viewDirection.Normal.Dot( velocity2D.Normal );
		dot = Math.Clamp( dot, -1f, 1f );
		return MathF.Acos( dot ) * 180f / MathF.PI;
	}

	public static float CalculateDownwardsAlignment( float targetSlopeAngle, Vector3 slopeDirection2D, Vector3 velocityDirection2D, float slopeAngle )
	{
		if ( slopeAngle <= 0f || slopeDirection2D.IsNearZeroLength || velocityDirection2D.IsNearZeroLength )
			return 0f;

		var normalizedSlopeAngle = Math.Clamp( slopeAngle / MathF.Max( targetSlopeAngle, 0.001f ), 0f, 1f );
		var directionAlignment = Math.Max( 0f, slopeDirection2D.Normal.Dot( velocityDirection2D.Normal ) );
		return directionAlignment * normalizedSlopeAngle;
	}

	public static float CalculateAdditionalUphillDeceleration( float velocitySlopeAlignment, float slopeAngle )
	{
		var normalizedSlopeAngle = Math.Clamp( slopeAngle / 45f, 0f, 1f );
		var deceleration = 50f + ((800f - 50f) * normalizedSlopeAngle);
		return deceleration * MathF.Abs( velocitySlopeAlignment );
	}

	public static bool ShouldExitSlide( float horizontalSpeed, float previousHorizontalSpeed, float exitSpeed, bool isGrounded, bool wantsToSlide, bool hasHeadroom, bool isStandUpBlocked )
	{
		if ( !isGrounded )
			return true;

		var isAccelerating = horizontalSpeed > 0f && horizontalSpeed >= previousHorizontalSpeed;
		var hasEnoughVelocity = horizontalSpeed > exitSpeed;
		if ( !isAccelerating && !hasEnoughVelocity )
			return true;

		if ( !wantsToSlide && !isStandUpBlocked && hasHeadroom )
			return true;

		return false;
	}

	public static bool ShouldApplyPostSlideCarry( float elapsed, float duration, float startSpeed, float endSpeed )
	{
		return duration > 0f && elapsed < duration && startSpeed > endSpeed;
	}

	public static float EvaluatePostSlideCarrySpeed( float elapsed, float duration, float startSpeed, float endSpeed )
	{
		if ( duration <= 0f || startSpeed <= endSpeed )
			return endSpeed;

		var t = Math.Clamp( elapsed / duration, 0f, 1f );
		var easedT = t * t * (3f - (2f * t));
		return startSpeed + ((endSpeed - startSpeed) * easedT);
	}

	public static bool ShouldPreservePostSlideCarry( bool hasInput, float inputAlignment )
	{
		return !hasInput || inputAlignment > 0f;
	}

	public static float EvaluateLandingImpactScale( float lastInAirTime )
	{
		var normalized = Math.Clamp( lastInAirTime / 0.8f, 0f, 1f );
		return 0.2f + (0.8f * normalized);
	}

	public static float EvaluateLandingFeedback( float downwardSpeed, float safeFallSpeed, float lethalFallSpeed )
	{
		if ( downwardSpeed <= safeFallSpeed )
			return 0f;

		var normalized = (downwardSpeed - safeFallSpeed) / MathF.Max( 1f, lethalFallSpeed - safeFallSpeed );
		return Math.Clamp( normalized, 0f, 1f );
	}

	public static float EvaluatePresentationArc( float progress )
	{
		var normalized = Math.Clamp( progress, 0f, 1f );
		if ( normalized <= 0f || normalized >= 1f )
			return 0f;

		return MathF.Sin( normalized * MathF.PI );
	}
}
