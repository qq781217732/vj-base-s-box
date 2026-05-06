using System;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Ammo Pile" )]
public partial class AmmoPile : Component
{
	[Property] public float AmmoRemaining { get; set; } = 400;
	[Property] public bool IsInfinite { get; set; }

	[Sync] public float SyncedAmmoRemaining { get; set; } = 400;

	ModelRenderer renderer;

	protected override void OnAwake()
	{
		Tags.Add( "item" );
		renderer = Components.Get<ModelRenderer>();
		if ( renderer is not null )
			renderer.Model = Model.Load( "assets/ammobox/ammo_box.vmdl" );
	}

	protected override void OnUpdate()
	{
		DebugOverlay.Text( WorldPosition + Vector3.Up * 8, SyncedAmmoRemaining + "%" );
	}

	public bool IsUsable( GameObject user )
	{
		return true;
	}

	public bool OnUse( GameObject user )
	{
		var ply = user?.Components.Get<HumanPlayer>();
		if ( ply is null ) return false;

		var inv = ply.Inventory as ZomInventory;
		if ( inv is null ) return false;

		var activeGO = inv.Active;
		if ( activeGO is null ) return false;

		var weapon = activeGO.Components.Get<BaseZomWeapon>();
		if ( weapon is null || weapon.WeaponSlot != WeaponSlot.Primary ) return false;

		var maxAmmo = weapon.AmmoMax + weapon.ClipSize;
		var currentAmmo = weapon.AmmoClip + weapon.AmmoReserve;
		if ( currentAmmo >= maxAmmo ) return false;

		var requestAmount = maxAmmo - currentAmmo;
		var requestPercent = (float)Math.Ceiling( (float)requestAmount / maxAmmo * 100f );

		if ( !IsInfinite )
		{
			SyncedAmmoRemaining -= requestPercent;
			if ( SyncedAmmoRemaining < 0 )
			{
				requestPercent += SyncedAmmoRemaining;
				requestAmount = (int)(requestPercent / 100 * maxAmmo);
			}
		}

		Sound.Play( "ammobox.replenish", WorldPosition );
		weapon.AmmoReserve += requestAmount;
		CheckForDeletion();
		return false;
	}

	void CheckForDeletion()
	{
		if ( !IsInfinite && SyncedAmmoRemaining <= 0 )
			GameObject.Destroy();
	}
}
