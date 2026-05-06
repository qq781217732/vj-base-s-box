using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVj9mmpistol : WeaponVjBase
{
    [Property] public string PrintName = "9mm Pistol";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/weapons/c_pistol.mdl";
    [Property] public string WorldModel = "models/weapons/w_pistol.mdl";
    [Property] public string HoldType = "pistol";
    [Property] public int Slot = 1;
    [Property] public int SlotPos = 1;
    [Property] public int SwayScale = 4;
    [Property] public bool UseHands = true;
    [Property] public float NPC_NextPrimaryFire = 0.3;
    [Property] public float NPC_CustomSpread = 0.8;
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public int PrimaryEffects_ShellAttachment = 2;
    [Property] public string PrimaryEffects_ShellType = "ShellEject";
    [Property] public bool HasReloadSound = true;
    [Property] public string ReloadSound = "weapons/pistol/pistol_reload1.wav";
    [Property] public int Reload_TimeUntilAmmoIsSet = 1;

}