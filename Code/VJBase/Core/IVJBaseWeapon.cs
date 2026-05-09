using Sandbox;

namespace VJBase;

/// <summary>
/// Contract for VJ Base weapons (NPC-held). Maps to Lua weapon_vj_base/shared.lua SWEP methods.
/// Attach VJBaseWeapon component to any GameObject that an NPC should treat as a weapon.
/// </summary>
public interface IVJBaseWeapon
{
    // ---- Phase 1: inventory & lifecycle ----
    bool IsVJBaseWeapon { get; }
    bool IsMeleeWeapon { get; }
    string HoldType { get; }

    void Equip(GameObject owner);
    void Unequip();

    int GetClip1();
    int GetMaxClip1();
    void SetClip1(int amount);

    void NPC_Reload();

    // ---- Phase 2: NPC autonomous fire ----
    float NPC_NextPrimaryFire { get; set; }
    bool NPC_StandingOnly { get; }
    string NPC_BeforeFireSound { get; }
    float NPC_BeforeFireSoundLevel { get; }
    (float a, float b) NPC_BeforeFireSoundPitch { get; }
    void NPCShoot_Primary();

    // ---- Phase 2: Firing guards ----
    bool IsReloading { get; set; }
    float NextSecondaryFireT { get; set; }
    bool CanPrimaryAttack();
    bool OnPrimaryAttack(string type, GameObject ent = null);
}
