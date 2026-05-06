using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjK3 : WeaponVjBase
{
    [Property] public string PrintName = "K-3";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool MadeForNPCsOnly = true;
    [Property] public string WorldModel = "models/vj_base/weapons/w_k3.mdl";
    [Property] public bool WorldModel_UseCustomPosition = true;
    [Property] public Vector3 WorldModel_CustomPositionAngle = Vector(-10, 0, 180);
    [Property] public Vector3 WorldModel_CustomPositionOrigin = Vector(-10, -8, -61);
    [Property] public string HoldType = "ar2";
    [Property] public string PrimaryEffects_MuzzleAttachment = "muzzle";
    [Property] public string PrimaryEffects_ShellType = "RifleShellEject";

}