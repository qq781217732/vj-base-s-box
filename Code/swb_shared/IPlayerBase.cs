using System;

namespace SWB.Shared;

/// <summary>
/// Implement this interface on a `Component` to integrate with SWB
/// </summary>
public interface IPlayerBase : IValid, Sandbox.Component.IDamageable
{
	/// <summary>
	/// Unique identifier for the player
	/// Typically implemented by `Component`
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	/// The game object that owns this component
	/// Typically implemented by `Component`
	/// </summary>
	public GameObject GameObject { get; }

	/// <summary>
	/// The camera to use when renderering the weapon's view model on the client side
	/// If none is provided and first person mode is enabled, then a camera will be created
	/// </summary>
	public CameraComponent? ViewModelCamera { get; set; }

	/// <summary>
	/// The camera used for rendering the player's first person view
	/// Used to calculate view model sway
	/// The Render exclude tag "viewmodel" will be automatically applied to prevent render issues
	/// </summary>
	public CameraComponent? Camera { get; }

	/// <summary>
	/// Whether the player is in first person view
	/// </summary>
	public bool IsFirstPerson { get; }

	/// <summary>
	/// The player's current velocity
	/// </summary>
	public Vector3 Velocity { get; }

	/// <summary>
	/// Whether the player is crouching, this will effect aim and recoil
	/// </summary>
	public bool IsCrouching { get; }

	/// <summary>
	/// Whether the player is running, this will effect aim and recoil
	/// </summary>
	public bool IsRunning { get; }

	/// <summary>
	/// Whether the player is sliding, this can alter aim, view model and movement feel logic
	/// </summary>
	public bool IsSliding { get; }

	/// <summary>
	/// Whether the player is on the ground, this will effect aim and recoil
	/// </summary>
	public bool IsOnGround { get; }

	/// <summary>
	/// Whether the player is actively climbing a ladder
	/// </summary>
	public bool IsClimbingLadder { get; }

	/// <summary>
	/// Whether the player is alive.
	/// Damage will not be dealt to dead players.
	/// </summary>
	public bool IsAlive { get; }

	/// <summary>
	/// Is this player considered a bot
	/// Bots have input movement and other functionalities disabled
	/// </summary>
	public bool IsBot { get; set; }

	/// <summary>
	/// View angle of the player, used to determine the direction to shoot a bullet
	/// </summary>
	public Angles EyeAngles { get; }

	/// <summary>
	/// View position of the player, used to determine the origin point of a fired bullet
	/// </summary>
	public Vector3 EyePos { get; }

	/// <summary>
	/// Input sensitivity modifier based on player ADS (aim down sights) state
	/// </summary>
	public float InputSensitivity { get; set; }

	/// <summary>
	/// The suggested FOV to be used by the player camera, affected by a weapon zoom
	/// This assumes a first-person perspective and will default to `Preferences.FieldOfView`
	/// </summary>
	public float FieldOfView { get; set; }

	/// <summary>
	/// Whether a jump was activated this frame.
	/// </summary>
	public bool bJumpedThisFrame { get; }

	/// <summary>
	/// Remaining timer for first-person jump presentation.
	/// </summary>
	public float JustJumpedTimer { get; }

	/// <summary>
	/// Whether the player is currently in an activated jump state.
	/// </summary>
	public bool bActivatedJump { get; }

	/// <summary>
	/// Whether the player landed this frame.
	/// </summary>
	public bool bJustLanded { get; }

	/// <summary>
	/// Last recorded in-air duration.
	/// </summary>
	public float LastInAirTime { get; }

	/// <summary>
	/// Smoothed landing impact scale.
	/// </summary>
	public float LandingImpactScale { get; }

	/// <summary>
	/// Normalized jump activation amount for first-person presentation.
	/// </summary>
	public float JumpActivation { get; }

	/// <summary>
	/// Progress through the first-person jump presentation arc.
	/// </summary>
	public float JumpAnimationProgress { get; }

	/// <summary>
	/// Strength of the in-air first-person falling presentation.
	/// </summary>
	public float FallAnimationStrength { get; }

	/// <summary>
	/// Active landing feedback alpha for first-person presentation.
	/// </summary>
	public float LandingAnimationAlpha { get; }

	/// <summary>
	/// Progress through the first-person landing presentation arc.
	/// </summary>
	public float LandingAnimationProgress { get; }

	/// <summary>
	/// Strength of the first-person landing presentation.
	/// </summary>
	public float LandingAnimationStrength { get; }

	/// <summary>
	/// Aggregated first-person camera vertical offset derived from movement-feel state.
	/// </summary>
	public float FirstPersonCameraVerticalOffset { get; }

	/// <summary>
	/// Aggregated first-person camera pitch offset derived from movement-feel state.
	/// </summary>
	public float FirstPersonCameraPitchOffset { get; }

	/// <summary>
	/// Base first-person posture offset used for crouch, slide and ladder lowering.
	/// </summary>
	public float FirstPersonPostureOffset { get; }

	/// <summary>
	/// Strength of the first-person slide presentation.
	/// </summary>
	public float SlideAnimationStrength { get; }

	/// <summary>
	/// Strength of the first-person ladder presentation.
	/// </summary>
	public float LadderAnimationStrength { get; }

	/// <summary>
	/// Blend applied to the lowered first-person viewmodel posture.
	/// </summary>
	public float ViewModelPostureBlend { get; }

	/// <summary>
	/// Weight applied to first-person walk bob.
	/// </summary>
	public float ViewModelWalkAnimationWeight { get; }

	/// <summary>
	/// Oscillation rate applied to first-person walk bob.
	/// </summary>
	public float ViewModelWalkAnimationCycleRate { get; }

	/// <summary>
	/// Speed normalization divisor applied to first-person walk bob.
	/// </summary>
	public float ViewModelWalkAnimationMaxSpeed { get; }

	/// <summary>
	/// Weight applied to first-person sprint pose.
	/// </summary>
	public float ViewModelSprintAnimationWeight { get; }

	/// <summary>
	/// Extra ladder-driven blend towards the tucked first-person pose.
	/// </summary>
	public float ViewModelLadderTuckBlend { get; }

	/// <summary>
	/// Scale applied to viewmodel tuck range for movement-state presentation.
	/// </summary>
	public float ViewModelTuckRangeScale { get; }

	/// <summary>
	/// Current landing feedback after impact and scale are applied.
	/// </summary>
	public float PendingLandingFeedback { get; }

	/// <summary>
	/// The Hold Type for the currently equipped weapon
	/// </summary>
	public HoldTypes HoldType { set; }

	/// <summary>
	/// Called when the weapon wants to know how much ammo is available
	/// </summary>
	/// <param name="type">The type of ammo</param>
	/// <returns>How much ammo is available</returns>
	public int AmmoCount( string type );

	/// <summary>
	/// Called when the weapon wants to trigger an animation on the player object
	/// </summary>
	/// <param name="animation">The animation to trigger</param>
	void TriggerAnimation( Animations animation );

	/// <summary>
	/// Triggered when the weapon wants to apply an angular offset to the player's view to simulate recoil.
	/// Called when a weapon is fired.
	/// </summary>
	/// <param name="offset">The suggested angular offset to apply</param>
	public void ApplyEyeAnglesOffset( Angles offset );

	/// <summary>
	/// Called when the weapon object should be attached/parented to the player's body
	/// </summary>
	/// <param name="object">The game object to parent</param>
	/// <param name="boneName">The suggested bone to parent to</param>
	public void ParentToBone( GameObject @object, string boneName );

	/// <summary>
	/// Called when the weapon is trying to take ammo.
	/// </summary>
	/// <param name="type">The type of ammo</param>
	/// <param name="amount">The amount of ammo requested</param>
	/// <returns>How much ammo was actually taken</returns>
	public int TakeAmmo( string type, int amount );

	/// <summary>
	/// Shakes the camera
	/// </summary>
	/// <param name="screenShake">Information about the shake</param>
	public void ShakeScreen( ScreenShake screenShake );
}
