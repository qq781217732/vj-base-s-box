using Sandbox;

namespace ZombieHorde;

public partial class BaseNpc
{
	[Rpc.Broadcast]
	void BecomeRagdollOnClient( Vector3 force, int forceBone )
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

		ragdoll.Tags.Add( "gib" );

		if ( BodyRenderer is not null )
			renderer.Tint = BodyRenderer.Tint;

		// Copy clothing to ragdoll
		foreach ( var child in GameObject.Children )
		{
			if ( !child.Tags.Has( "clothes" ) )
				continue;

			var childRenderer = child.Components.Get<SkinnedModelRenderer>();
			if ( childRenderer is null )
				continue;

			var clothingRagdoll = new GameObject( true, "ClothingRagdoll" );
			var clothingRenderer = clothingRagdoll.Components.Create<SkinnedModelRenderer>();
			clothingRenderer.Model = childRenderer.Model;
			clothingRenderer.Tint = childRenderer.Tint;
			clothingRagdoll.SetParent( ragdoll, true );
		}

		// Apply force
		foreach ( var body in physics.Bodies )
		{
			// TODO: physics impulse API changed - ApplyForce is not available on Body in this API version
		}

		if ( forceBone >= 0 )
		{
			// TODO: ModelPhysics.GetBonePhysicsBody API may not exist in new API
		}

		ragdoll.Destroy();
	}
}
