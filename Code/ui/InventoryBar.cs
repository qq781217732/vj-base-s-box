using Sandbox.UI;
using SWB.Shared;
using System;

namespace ZombieHorde;

public class InventoryBar : Panel
{
	readonly List<InventoryIcon> slots = new();

	public InventoryBar()
	{
		for ( int i = 0; i < 6; i++ )
		{
			var icon = new InventoryIcon( i + 1, this );
			slots.Add( icon );
			icon.SetClass( "small", i >= 3 );
		}
	}

	GameObject GetSlotItem( ZomInventory inventory, int index )
	{
		var slot = index switch
		{
			0 => WeaponSlot.Secondary,
			1 => WeaponSlot.Primary,
			2 => WeaponSlot.Primary,
			3 => WeaponSlot.Grenade,
			4 => WeaponSlot.Medkit,
			5 => WeaponSlot.Pills,
			_ => WeaponSlot.Prop,
		};

		if ( slot == WeaponSlot.Primary )
		{
			if ( index == 1 && inventory.Primary1.IsValid() )
				return inventory.Primary1;
			if ( index == 2 && inventory.Primary2.IsValid() )
				return inventory.Primary2;
			return null;
		}

		return inventory.GetSlot( slot );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Game.ActiveScene.GetAllComponents<HumanPlayer>()
			.FirstOrDefault( p => !p.IsProxy );
		if ( player == null ) return;

		var inventory = player.Inventory as ZomInventory;
		if ( inventory == null ) return;

		for ( int i = 0; i < slots.Count; i++ )
		{
			UpdateIcon( GetSlotItem( inventory, i ), slots[i], i );
		}
	}

	private static void UpdateIcon( GameObject ent, InventoryIcon inventoryIcon, int i )
	{
		var player = Game.ActiveScene.GetAllComponents<HumanPlayer>()
			.FirstOrDefault( p => !p.IsProxy );
		if ( player == null ) return;

		if ( ent == null )
		{
			inventoryIcon.Clear();
			inventoryIcon.SetClass( "hidden", true );
			return;
		}

		inventoryIcon.SetClass( "hidden", false );

		var weapon = ent.Components.Get<BaseZomWeapon>();
		var activeWep = player.ActiveWeapon;

		inventoryIcon.TargetEnt = ent;
		inventoryIcon.SetClass( "active", activeWep is not null && activeWep.GameObject == ent );

		// Slot glyph
		var slotName = $"Slot{i + 1}";
		var glyphTexture = Input.GetGlyph( slotName, InputGlyphSize.Medium, false );
		inventoryIcon.Glyph.Texture = glyphTexture;

		if ( weapon != null )
		{
			inventoryIcon.Bullets.Text = weapon.AmmoClip.ToString();
			inventoryIcon.BulletReserve.Text = weapon.AmmoMax == 0 ? "" : weapon.AmmoMax == -1 ? "∞" : weapon.AmmoReserve.ToString();
			inventoryIcon.Icon.SetTexture( weapon.Icon );
			inventoryIcon.RarityBar.Style.BackgroundColor = weapon.RarityColor;

			if ( weapon.AmmoMax == -2 )
			{
				inventoryIcon.Bullets.Text = "";
				inventoryIcon.BulletReserve.Text = "";
			}

			if ( i >= 3 )
			{
				if ( weapon.AmmoMax > 0 )
				{
					inventoryIcon.Bullets.Text = $"{weapon.AmmoClip + weapon.AmmoReserve}";
					inventoryIcon.BulletReserve.Text = $"/{weapon.AmmoMax + weapon.ClipSize}";
					inventoryIcon.Bullets.Style.Right = 81;
				}
				else if ( weapon.AmmoMax == 0 )
				{
					inventoryIcon.Bullets.Text = "";
				}
				else
				{
					inventoryIcon.Bullets.Style.Right = 60;
				}
			}
		}
	}
}
