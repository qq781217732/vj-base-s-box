using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjBlaster : WeaponVjBase
{
    [Property] public string PrintName = "Blaster";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/vj_base/weapons/c_e5.mdl";
    [Property] public string WorldModel = "models/vj_base/weapons/w_e5.mdl";
    [Property] public string HoldType = "ar2";
    [Property] public int Slot = 2;
    [Property] public int SlotPos = 4;
    [Property] public bool UseHands = true;
    [Property] public float NPC_NextPrimaryFire = 0.3;
    [Property] public string NPC_ReloadSound = "vj_base/weapons/blaster/reload.wav";
    [Property] public object PrimaryEffects_MuzzleParticles = {"vj_muzzle_blaster_red"};
    [Property] public bool PrimaryEffects_MuzzleParticlesAsOne = true;
    [Property] public string PrimaryEffects_MuzzleAttachment = "muzzle";
    [Property] public bool PrimaryEffects_SpawnShells = false;
    [Property] public object PrimaryEffects_DynamicLightColor = VJ.COLOR_RED;
    [Property] public bool HasReloadSound = true;
    [Property] public string ReloadSound = "vj_base/weapons/blaster/reload.wav";
    [Property] public float Reload_TimeUntilAmmoIsSet = 0.8;

}