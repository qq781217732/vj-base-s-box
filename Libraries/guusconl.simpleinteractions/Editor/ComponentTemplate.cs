using Editor;


[Icon( "touch_app" )]
[Title( "SimpleInteraction" )]
[Description( "A simple interaction component so you can interact with objects" )]
public partial class SimpleInteractionTemplate : ComponentTemplate
{
	public override void Create( string componentName, string path )
	{
		var content = $$"""
		using Sandbox;

		public sealed class {{componentName}} : SimpleInteractions.SimpleInteraction
		{
			protected override void OnStart()
			{
				base.OnStart();

				// Put your initialization code here if you have any
			}

			protected override void OnUpdate()
			{
				base.OnUpdate();

				// Put your update code here if you have any
			}


			[Rpc.Broadcast]
			protected override void OnInteract()
			{
				Log.Info($"{Rpc.Caller.DisplayName} interacted with {this.GameObject.Name}!");
			}
		}

		""";

		var directory = System.IO.Path.GetDirectoryName( path );
		System.IO.File.WriteAllText( System.IO.Path.Combine( directory, componentName + Suffix ), content );
	}
}