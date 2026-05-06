using Sandbox;
using SWB.Shared;
using System.Threading.Tasks;

namespace ZombieHorde;

public partial class BaseNpc
{
	// TODO: clean this up
	public ClothingContainer Clothing { get; protected set; }

	public virtual void UpdateClothes()
	{
		Clothing ??= new();
		string model;

		model = new[] {
			"models/zombie/citizen_zombie/skins/skin_zombie01.clothing",
			"models/zombie/citizen_zombie/skins/skin_zombie02.clothing",
			"models/zombie/citizen_zombie/skins/skin_zombie03.clothing",
		}[Game.Random.NextInt( 3 )];
		if ( ResourceLibrary.TryGet<Clothing>( model, out var item ) ) { Clothing.Clothing.Add( new ClothingContainer.ClothingEntry( item ) ); }

		model = new[] {
			"models/citizen_clothes/trousers/jeans/jeans.clothing",
			"models/citizen_clothes/shorts/summer_shorts/summer shorts.clothing",
			"models/citizen_clothes/trousers/smarttrousers/trousers.smart.clothing",
		}[Game.Random.NextInt( 3 )];
		if ( ResourceLibrary.TryGet<Clothing>( model, out item ) ) { Clothing.Clothing.Add( new ClothingContainer.ClothingEntry( item ) ); }

		model = new[] {
			"models/citizen_clothes/shoes/slippers/slippers.clothing",
			"models/citizen_clothes/shoes/smartshoes/smartshoes.clothing",
			"models/citizen_clothes/shoes/sneakers/sneakers.clothing",
			"models/citizen_clothes/shoes/trainers/trainers.clothing",
			""
		}[Game.Random.NextInt( 5 )];
		if ( ResourceLibrary.TryGet<Clothing>( model, out item ) ) { Clothing.Clothing.Add( new ClothingContainer.ClothingEntry( item ) ); }

		model = new[] {
			"models/citizen_clothes/shirt/flannel_shirt/flannel_shirt.clothing",
			"models/citizen_clothes/shirt/hawaiian_shirt/hawaiian shirt.clothing",
			"models/citizen_clothes/shirt/longsleeve_shirt/longsleeve_shirt.clothing",
			"models/citizen_clothes/shirt/tanktop/tanktop.clothing",
			"models/citizen_clothes/shirt/v_neck_tshirt/v_neck_tshirt.clothing",
			"models/citizen_clothes/shirt/flannel_shirt/variations/blue_shirt/blue_shirt.clothing",
			"models/citizen_clothes/jacket/brown_leather_jacket/brown_leather_jacket.clothing",
			"models/citizen_clothes/jacket/longsleeve/longsleeve.clothing",
			"models/citizen_clothes/jacket/hoodie/hoodie.clothing",
			""
		}[Game.Random.NextInt( 10 )];
		if ( ResourceLibrary.TryGet<Clothing>( model, out item ) ) { Clothing.Clothing.Add( new ClothingContainer.ClothingEntry( item ) ); }

		model = new[] {
			"models/citizen_clothes/hair/hair_balding/hair_baldingbrown.clothing",
			"models/citizen_clothes/hair/hair_balding/hair_baldinggrey.clothing",
			"models/citizen_clothes/hair/hair_bobcut/hair_bobcut.clothing",
			"models/citizen_clothes/hair/hair_fade/hair_fade.clothing",
			"models/citizen_clothes/hair/hair_longbrown/models/hair_longbrown.clothing",
			"models/citizen_clothes/hair/hair_longcurly/hair_longcurly.clothing",
			"models/citizen_clothes/hair/hair_longbrown/models/hair_longgrey.clothing",
			"models/citizen_clothes/hair/hair_wavyblack/hair_wavyblack.clothing",
			"models/citizen_clothes/hair/hair_looseblonde/hair.loose.blonde.clothing",
			"models/citizen_clothes/hair/hair_looseblonde/hair.loose.brown.clothing",
			"models/citizen_clothes/hair/hair_looseblonde/hair.loose.grey.clothing",
			"models/citizen_clothes/hat/baseball_cap/baseball_cap.clothing",
			"models/citizen_clothes/hair/hair_shortscruffy/hair_shortscruffy_brown.clothing",
			"models/citizen_clothes/hair/hair_shortscruffy/hair_shortscruffy_grey.clothing",
			""
		}[Game.Random.NextInt( 15 )];
		if ( ResourceLibrary.TryGet<Clothing>( model, out item ) ) { Clothing.Clothing.Add( new ClothingContainer.ClothingEntry( item ) ); }

		if ( Game.Random.NextInt( 4 ) == 1 )
		{
			model = new[] {
				"models/citizen_clothes/hair/moustache/moustache_brown.clothing",
				"models/citizen_clothes/hair/moustache/moustache_grey.clothing",
				"models/citizen_clothes/hair/scruffy_beard/scruffy_beard_brown.clothing",
				"models/citizen_clothes/hair/scruffy_beard/scruffy_beard_grey.clothing",
				"models/citizen_clothes/hair/stubble/stubble.clothing"
			}[Game.Random.NextInt( 5 )];
			if ( ResourceLibrary.TryGet<Clothing>( model, out item ) ) { Clothing.Clothing.Add( new ClothingContainer.ClothingEntry( item ) ); }
		}

		if ( Game.Random.NextInt( 1 ) == 1 )
		{
			model = new[] {
				"models/citizen_clothes/hair/eyebrows/eyebrows.clothing"
			}[Game.Random.NextInt( 1 )];
			if ( ResourceLibrary.TryGet<Clothing>( model, out item ) ) { Clothing.Clothing.Add( new ClothingContainer.ClothingEntry( item ) ); }
		}
	}

	public async void Dress()
	{
		// Clear any material overrides
		if ( BodyRenderer is not null )
			BodyRenderer.MaterialOverride = null;

		await Task.Delay( 500 );
		if ( !this.IsValid ) return;

		if ( BodyRenderer is not null )
			BodyRenderer.Tint = Color.Parse( "#A3A3A3" ) ?? Color.White;

		// Apply clothing via the Dresser component if available, or fallback to manual application
		var dresser = Components.Get<Dresser>();
		if ( dresser is not null )
		{
			await dresser.Apply();
		}
		else
		{
			// TODO: Clothing API changed - Clothing.Dress removed
		}

		// Color clothing children
		foreach ( var child in GameObject.Children )
		{
			if ( child.Tags.Has( "clothes" ) )
			{
				var clothRenderer = child.Components.Get<SkinnedModelRenderer>();
				if ( clothRenderer is not null )
					clothRenderer.Tint = Color.Parse( "#A3A3A3" ) ?? Color.White;
			}
		}

		await Task.Delay( 1000 );
		if ( !this.IsValid ) return;

		if ( BodyRenderer is not null )
			BodyRenderer.MaterialOverride = null;

		await Task.Delay( 200 );
		if ( !this.IsValid ) return;

		if ( Clothing is not null && Clothing.Clothing.Count > 0 )
		{
			var skinMaterial = Clothing.Clothing
				// TODO: Resource removed from ClothingEntry
					.Select( x => "" )
				.Where( m => !string.IsNullOrEmpty( m ) )
				.Select( m => Material.Load( m ) )
				.FirstOrDefault();

			var eyesMaterial = Clothing.Clothing
				// TODO: Resource removed from ClothingEntry
					.Select( x => "" )
				.Where( m => !string.IsNullOrEmpty( m ) )
				.Select( m => Material.Load( m ) )
				.FirstOrDefault();

			if ( BodyRenderer is not null )
			{
				// TODO: BodyRenderer.Set does not accept Material parameter in this API version
				// The Set() method expects Vector3, not Material
				//if ( skinMaterial is not null )
					// BodyRenderer.Set( "skin", skinMaterial ); // Material→Vector3 not compatible

				//if ( eyesMaterial is not null )
					// BodyRenderer.Set( "eyes", eyesMaterial ); // Material→Vector3 not compatible
			}
		}
	}
}
