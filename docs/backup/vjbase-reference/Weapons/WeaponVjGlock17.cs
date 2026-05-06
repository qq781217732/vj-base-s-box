using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjGlock17 : WeaponVjBase
{
    [Property] public string PrintName = "Glock 17";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/vj_base/weapons/v_glock.mdl";
    [Property] public string WorldModel = "models/vj_base/weapons/w_glock.mdl";
    [Property] public string HoldType = "pistol";
    [Property] public int ViewModelFOV = 70;
    [Property] public int Slot = 1;
    [Property] public int SlotPos = 1;
    [Property] public int SwayScale = 2;
    [Property] public float NPC_NextPrimaryFire = 0.3;
    [Property] public float NPC_CustomSpread = 0.8;
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public string PrimaryEffects_ShellType = "ShellEject";
    [Property] public object AnimTbl_Deploy = ACT_VM_IDLE_TO_LOWERED;
    [Property] public bool HasReloadSound = true;
    [Property] public string ReloadSound = "vj_base/weapons/glock17/reload.wav";
    [Property] public float Reload_TimeUntilAmmoIsSet = 1.5;

    public virtual void OnSecondaryAttack()
    {
        this.Primary.Delay = 0.175;
        this.Primary.Cone = 20;
        PrimaryAttack();
        this.Primary.Delay = 0.25;
        this.Primary.Cone = 5;

        NextSecondaryFireTime = CurTime( + 0.175);
        return true;
    }

}