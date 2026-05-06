using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjMp40 : WeaponVjBase
{
    [Property] public string PrintName = "MP 40";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/vj_base/weapons/c_mp40.mdl";
    [Property] public string WorldModel = "models/vj_base/weapons/w_mp40.mdl";
    [Property] public string HoldType = "smg";
    [Property] public int ViewModelFOV = 45;
    [Property] public int Slot = 2;
    [Property] public int SlotPos = 4;
    [Property] public int SwayScale = 1;
    [Property] public bool UseHands = true;
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public string PrimaryEffects_ShellType = "ShellEject";
    [Property] public float Reload_TimeUntilAmmoIsSet = 2.1;

}