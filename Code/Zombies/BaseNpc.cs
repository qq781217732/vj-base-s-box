using SWB.Shared;

namespace ZombieHorde;

/// <summary>
/// Base NPC component. Provides core damage handling, death effects, and sound RPCs.
/// Replaces the old Entity-based BaseNpc. Zombie-specific functionality is in BaseZombie.
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Base Npc" )]
public partial class BaseNpc : Component, Component.IDamageable
{
	// ========== Properties ==========

	[Sync] public float Health { get; set; } = 100;
	public bool IsAlive => Health > 0;

	Sandbox.DamageInfo lastDamage;

	public SkinnedModelRenderer BodyRenderer { get; set; }
	public CharacterController CharacterController { get; set; }

	// ========== Lifecycle ==========

	protected override void OnAwake()
	{
		Tags.Add( "npc" );
		BodyRenderer = Components.Get<SkinnedModelRenderer>();
		CharacterController = Components.Get<CharacterController>();
	}

	// ========== IDamageable ==========

	public void OnDamage( in Sandbox.DamageInfo info )
	{
		if ( !IsAlive ) return;

		lastDamage = info;

		Health -= info.Damage;

		DamagedEffects();

		if ( Health <= 0 )
		{
			OnDeath( info );
		}
	}

	// ========== Death ==========

	public virtual void OnDeath( in Sandbox.DamageInfo info )
	{
		// Disable all colliders so dead body doesn't block players
		foreach ( var col in Components.GetAll<Collider>() )
			col.Enabled = false;

		// Check for blast/gib
		if ( info.Tags.Has( "blast" ) )
		{
			// Gib effect at position
			// TODO: Particles.Create API changed - use SceneParticles
		}
		else
		{
			CreateRagdoll( info );
		}

		GameObject.DestroyAsync( 20 );
	}

	// ========== Ragdoll ==========

	protected virtual void CreateRagdoll( in Sandbox.DamageInfo info )
	{
		if ( BodyRenderer is null ) return;

		var ragdoll = new GameObject( true, "NpcRagdoll" );
		ragdoll.NetworkMode = NetworkMode.Never;
		ragdoll.WorldPosition = WorldPosition;
		ragdoll.WorldRotation = WorldRotation;

		var renderer = ragdoll.Components.Create<SkinnedModelRenderer>();
		renderer.Model = BodyRenderer.Model;
		renderer.UseAnimGraph = false;

		var physics = ragdoll.Components.Create<ModelPhysics>( true );
		physics.Model = renderer.Model;
		physics.Renderer = renderer;
		physics.CopyBonesFrom( BodyRenderer, true );

		// Apply force to ragdoll
		var dmgInfo = lastDamage;
		if ( dmgInfo != null )
		{
			foreach ( var body in physics.Bodies )
			{
				// TODO: physics impulse API changed - ApplyForceAt removed
				// body.ApplyForceAt( dmgInfo.Position, dmgInfo.Force * 100 );
			}
		}

		BodyRenderer.Enabled = false;
	}

	// ========== Sound RPC ==========

	[Rpc.Broadcast]
	public virtual void PlaySoundOnClient( string sound )
	{
		Sound.Play( sound, WorldPosition + Vector3.Up * 60 );
	}

	// ========== Damage Effects ==========

	public virtual void DamagedEffects()
	{
		if ( this is BaseZombie zombie && zombie.Agent is not null )
		{
			zombie.SpeedMultiplier = 0.3f;
		}
		else if ( CharacterController is not null )
		{
			CharacterController.Velocity *= 0.1f;
		}

		if ( Health > 0 )
			PlaySoundOnClient( "zombie.hurt" );
	}
}
