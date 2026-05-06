using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjSsg08 : WeaponVjBase
{
    [Property] public string PrintName = "SSG-08";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool MadeForNPCsOnly = true;
    [Property] public string WorldModel = "models/vj_base/weapons/w_ssg08.mdl";
    [Property] public bool WorldModel_UseCustomPosition = true;
    [Property] public Vector3 WorldModel_CustomPositionAngle = Vector(-8, 90, 180);
    [Property] public Vector3 WorldModel_CustomPositionOrigin = Vector(-4.4, -1, -0.5);
    [Property] public string HoldType = "ar2";
    [Property] public int NPC_NextPrimaryFire = 2;
    [Property] public float NPC_TimeUntilFire = 0.5;
    [Property] public float NPC_CustomSpread = 0.5;
    [Property] public float NPC_FiringDistanceScale = 2.5;
    [Property] public bool NPC_StandingOnly = true;
    [Property] public string NPC_ReloadSound = "vj_base/weapons/reload_rifle_bolt.wav";
    [Property] public string NPC_ExtraFireSound = "vj_base/weapons/cycle_rifle_bolt.wav";
    [Property] public float NPC_ExtraFireSoundTime = 0.4;

}