using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjAk47 : WeaponVjBase
{
    [Property] public string PrintName = "AK-47";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/weapons/cstrike/c_rif_ak47.mdl";
    [Property] public string WorldModel = "models/vj_base/weapons/w_ak47.mdl";
    [Property] public bool WorldModel_UseCustomPosition = true;
    [Property] public Vector3 WorldModel_CustomPositionAngle = Vector(-8, 90, 180);
    [Property] public Vector3 WorldModel_CustomPositionOrigin = Vector(-3.4, -1, -0.5);
    [Property] public string HoldType = "ar2";
    [Property] public bool ViewModelFlip = false;
    [Property] public int Slot = 2;
    [Property] public int SlotPos = 4;
    [Property] public int SwayScale = 1;
    [Property] public bool UseHands = true;
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public string PrimaryEffects_ShellType = "RifleShellEject";
    [Property] public float Reload_TimeUntilAmmoIsSet = 1.8;

    public virtual void OnAnimEvent(pos, ang, event, options)
    {
        if (event == 5001) return true end  // Asiga hose vor shtke gedervadz flash-e
    }

}