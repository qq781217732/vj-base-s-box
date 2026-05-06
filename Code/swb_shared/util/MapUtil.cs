using System.Collections.Generic;
using System.Linq;

namespace SWB.Shared;

public static class MapUtil
{
	public static void TagLights()
	{
		var mapInstance = Game.ActiveScene.GetComponentInChildren<MapInstance>();
		if ( mapInstance is null ) return;
		var envProbes = mapInstance.GetComponentsInChildren<EnvmapProbe>();
		if ( envProbes is null || !envProbes.Any() ) return;
		TagLights( envProbes );
	}

	static async void TagLights( IEnumerable<Component> components )
	{
		foreach ( var comp in components )
		{
			if ( comp is null ) continue;
			comp.Tags.Add( TagsHelper.Light );
		}
		await GameTask.DelaySeconds( 1 );
		var count = components.Count( c => c is not null && c.Tags.Has( TagsHelper.Light ) );
		if ( count < components.Count() )
			TagLights( components );
	}
}
