using System;

namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Med Kit" )]
public partial class MedKit : BaseZomWeapon
{
	public override float PrimaryRate => 4.5f;
	public override int ClipSize => 1;
	public override int AmmoMax => 1;
	public override float ReloadTime => 6.0f;
	public override WeaponSlot WeaponSlot => WeaponSlot.Medkit;
	public override bool UseAlternativeSprintAnimation => true;

	protected override void OnStart()
	{
		base.OnStart();
		AmmoClip = ClipSize; AmmoReserve = 0;
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		if ( !IsProxy && Owner.Health < Owner.MaxHealth )
		{
			Owner.Health += Math.Min( 70, Owner.MaxHealth - Owner.Health );
			Sound.Play( "healthkit.use", WorldPosition );
			if ( --AmmoClip <= 0 ) { ((ZomInventory)Owner.Inventory)?.DropItem( GameObject ); GameObject.Destroy(); }
		}
	}

	public override void AttackSecondary()
	{
		if ( IsProxy ) return;
		var tr = Game.ActiveScene.Trace.Ray( Owner.EyePos, Owner.EyePos + Owner.EyeAngles.ToRotation().Forward * 80 )
			.IgnoreGameObjectHierarchy( GameObject ).WithoutTags( "zombie", "trigger" ).Radius( 10 ).Run();
		var other = tr.GameObject?.Components.Get<HumanPlayer>();
		if ( other is not null && other.Health < other.MaxHealth )
		{
			other.Health += Math.Min( 70, other.MaxHealth - other.Health );
			Sound.Play( "healthkit.use", WorldPosition );
			if ( --AmmoClip <= 0 ) { ((ZomInventory)Owner.Inventory)?.DropItem( GameObject ); GameObject.Destroy(); }
		}
	}
}
