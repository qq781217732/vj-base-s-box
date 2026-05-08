using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// Default VJ weapon component. Implements IVJBaseWeapon with configurable properties.
/// Phase 1: inventory/lifecycle only. Phase 2+ adds NPC autonomous fire logic.
/// </summary>
public partial class VJBaseWeapon : Component, IVJBaseWeapon
{
    [Property] public bool IsVJBaseWeapon { get; set; } = true;
    [Property] public bool IsMeleeWeapon { get; set; }
    [Property] public string HoldType { get; set; } = "pistol";

    public GameObject WeaponOwner { get; private set; }

    [Property] public int Clip1 { get; set; } = 30;
    [Property] public int MaxClip1 { get; set; } = 30;

    public Action OnReloadAction { get; set; }

    public virtual void Equip(GameObject owner)
    {
        WeaponOwner = owner;
    }

    public virtual void Unequip()
    {
        WeaponOwner = null;
    }

    public int GetClip1() => Clip1;
    public int GetMaxClip1() => MaxClip1;
    public void SetClip1(int amount) => Clip1 = Math.Clamp(amount, 0, MaxClip1);

    public virtual void NPC_Reload()
    {
        OnReloadAction?.Invoke();
    }
}
