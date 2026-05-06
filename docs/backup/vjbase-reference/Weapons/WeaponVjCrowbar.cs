using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjCrowbar : WeaponVjBase
{
    [Property] public string PrintName = "Crowbar";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool MadeForNPCsOnly = true;
    [Property] public string ReplacementWeapon = "weapon_crowbar";
    [Property] public string WorldModel = "models/weapons/w_crowbar.mdl";
    [Property] public string HoldType = "melee";
    [Property] public float NPC_TimeUntilFire = 0.4;
    [Property] public bool IsMeleeWeapon = true;

}