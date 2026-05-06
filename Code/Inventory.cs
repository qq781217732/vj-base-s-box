using SWB.Player;
using SWB.Shared;

namespace ZombieHorde;

/// <summary>
/// Zombie Horde inventory with dedicated weapon slots.
/// Extends SWB Inventory with slot management for:
/// Secondary (1), Primary1 (2), Primary2 (3), Grenade (4), Medkit (5), Pills (6)
/// </summary>
public class ZomInventory : Inventory
{
	// Weapon slot references (GameObjects)
	public GameObject Secondary { get; set; }
	public GameObject Primary1 { get; set; }
	public GameObject Primary2 { get; set; }
	public GameObject Grenade { get; set; }
	public GameObject Medkit { get; set; }
	public GameObject Pills { get; set; }

	HumanPlayer player;

	protected override void OnAwake()
	{
		base.OnAwake();
		player = Components.Get<HumanPlayer>();
	}

	public new void Add( GameObject obj, bool makeActive = false )
	{
		var wep = obj?.Components.Get<BaseZomWeapon>();
		if ( wep is null )
		{
			base.Add( obj, makeActive );
			return;
		}

		// Check if slot is already occupied
		switch ( wep.WeaponSlot )
		{
			case WeaponSlot.Secondary:
				if ( Secondary.IsValid() ) return;
				Secondary = obj;
				break;
			case WeaponSlot.Primary:
				if ( !Primary1.IsValid() )
					Primary1 = obj;
				else if ( !Primary2.IsValid() )
					Primary2 = obj;
				else
					return;
				break;
			case WeaponSlot.Grenade:
				if ( Grenade.IsValid() ) return;
				Grenade = obj;
				break;
			case WeaponSlot.Medkit:
				if ( Medkit.IsValid() ) return;
				Medkit = obj;
				break;
			case WeaponSlot.Pills:
				if ( Pills.IsValid() ) return;
				Pills = obj;
				break;
		}

		base.Add( obj, makeActive );

		if ( wep is not null && !player.SupressPickupNotices )
		{
			Sound.Play( "dm.pickup_weapon", obj.WorldPosition );
			PickupFeed.OnPickupWeapon( wep.DisplayName );
		}
	}

	public new void Clear()
	{
		base.Clear();
		Secondary = null;
		Primary1 = null;
		Primary2 = null;
		Grenade = null;
		Medkit = null;
		Pills = null;
	}

	public GameObject GetSlot( WeaponSlot slot )
	{
		return slot switch
		{
			WeaponSlot.Secondary => Secondary,
			WeaponSlot.Primary => Primary1.IsValid() ? Primary1 : null,
			WeaponSlot.Grenade => Grenade,
			WeaponSlot.Medkit => Medkit,
			WeaponSlot.Pills => Pills,
			_ => null,
		};
	}

	public bool IsCarryingType( System.Type t )
	{
		return Items.Any( x => x.IsValid() && x.Components.Get( t ) is not null );
	}

	public GameObject DropActive()
	{
		if ( Active is null ) return null;

		var dropped = Active;

		// Clear slot
		if ( dropped == Secondary ) Secondary = null;
		if ( dropped == Primary1 ) Primary1 = null;
		if ( dropped == Primary2 ) Primary2 = null;
		if ( dropped == Grenade ) Grenade = null;
		if ( dropped == Medkit ) Medkit = null;
		if ( dropped == Pills ) Pills = null;

		Items.Remove( dropped );
		Active = null;
		ActiveItem = null;

		return dropped;
	}

	public GameObject DropItem( GameObject obj )
	{
		if ( obj is null ) return null;

		if ( obj == Secondary ) Secondary = null;
		if ( obj == Primary1 ) Primary1 = null;
		if ( obj == Primary2 ) Primary2 = null;
		if ( obj == Grenade ) Grenade = null;
		if ( obj == Medkit ) Medkit = null;
		if ( obj == Pills ) Pills = null;

		Items.Remove( obj );
		if ( Active == obj )
		{
			Active = null;
			ActiveItem = null;
		}

		return obj;
	}
}

public enum WeaponSlot
{
	Secondary,
	Primary,
	Grenade,
	Medkit,
	Pills,
	Prop
}
