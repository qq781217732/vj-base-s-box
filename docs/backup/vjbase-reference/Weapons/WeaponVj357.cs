using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVj357 : WeaponVjBase
{
    [Property] public string PrintName = ".357 Magnum";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/weapons/c_357.mdl";
    [Property] public string WorldModel = "models/weapons/w_357.mdl";
    [Property] public string HoldType = "revolver";
    [Property] public int Slot = 1;
    [Property] public int SlotPos = 1;
    [Property] public int SwayScale = 4;
    [Property] public bool UseHands = true;
    [Property] public float NPC_NextPrimaryFire = 0.95;
    [Property] public float NPC_CustomSpread = 0.5;
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public bool PrimaryEffects_SpawnShells = false;
    [Property] public bool HasReloadSound = false;
    [Property] public float Reload_TimeUntilAmmoIsSet = 2.7;

}