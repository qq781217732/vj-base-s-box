using SWB.Shared;
using System.Linq;
using System.Collections.Generic;

namespace ZombieHorde;

public static class ClothingExtensions
{
	public static void LoadRandom( this ClothingContainer container )
	{
		var allClothes = ResourceLibrary.GetAll<Clothing>().ToList();
		var grouped = allClothes
			.Where( c => c.Category != Clothing.ClothingCategory.Skin && c.Category != Clothing.ClothingCategory.Eyes )
			.GroupBy( x => x.Category )
			.ToList();

		foreach ( var group in grouped )
		{
			if ( Game.Random.NextInt( 0, 10 ) == 0 )
				continue;

			var items = group.ToArray();
			var item = items[Game.Random.NextInt( items.Length )];

			container.Clothing.Add( new ClothingContainer.ClothingEntry( item ) );
		}
	}
}
