using Sandbox;

namespace VJBase;

/// <summary>
/// Contract for VJ Base weapons (NPC-held). Maps to Lua weapon_vj_base/shared.lua SWEP methods.
/// Attach VJBaseWeapon component to any GameObject that an NPC should treat as a weapon.
/// </summary>
public interface IVJBaseWeapon
{
    bool IsVJBaseWeapon { get; }
    bool IsMeleeWeapon { get; }
    string HoldType { get; }

    void Equip(GameObject owner);
    void Unequip();

    int GetClip1();
    int GetMaxClip1();
    void SetClip1(int amount);

    void NPC_Reload();
}
