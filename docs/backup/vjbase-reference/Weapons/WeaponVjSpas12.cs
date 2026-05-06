using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjSpas12 : WeaponVjBase
{
    [Property] public string PrintName = "SPAS-12";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/weapons/c_shotgun.mdl";
    [Property] public string WorldModel = "models/weapons/w_shotgun.mdl";
    [Property] public string HoldType = "shotgun";
    [Property] public int Slot = 3;
    [Property] public int SlotPos = 4;
    [Property] public bool UseHands = true;
    [Property] public float NPC_NextPrimaryFire = 0.9;
    [Property] public float NPC_TimeUntilFire = 0.2;
    [Property] public float NPC_CustomSpread = 2.5;
    [Property] public string NPC_ExtraFireSound = "vj_base/weapons/cycle_shotgun_pump.wav";
    [Property] public float NPC_FiringDistanceScale = 0.5;
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public int PrimaryEffects_ShellAttachment = 2;
    [Property] public string PrimaryEffects_ShellType = "ShotgunShellEject";
    [Property] public bool HasReloadSound = true;
    [Property] public object ReloadSound = {"weapons/shotgun/shotgun_reload1.wav", "weapons/shotgun/shotgun_reload2.wav", "weapons/shotgun/shotgun_reload3.wav"};
    [Property] public float Reload_TimeUntilAmmoIsSet = 0.3;

    public virtual void OnPrimaryAttack(status, statusData)
    {
        if (status == "PostFire")
        var owner = Owner;
        if (owner.IsValid() && owner.IsPlayer())
        GameTask.DelaySeconds(0.25).ContinueWith(_ => function();
        if (this.IsValid() && owner.IsValid() && owner.IsPlayer())
        Sound.Play("Weapon_Shotgun.Special1", Transform.Position);
        var animTime = AnimationHelper.Duration(owner.GetViewModel(), ACT_SHOTGUN_PUMP);
        SendWeaponAnim(ACT_SHOTGUN_PUMP);
        this.PLY_NextIdleAnimT = Time.Now + animTime;
        this.PLY_NextReloadT = Time.Now + animTime;

        end);


    }

    public virtual void OnSecondaryAttack()
    {
        if (Clip1 > 1)
        this.Primary.Delay = 1;
        this.Primary.Cone = 20;
        this.Primary.NumberOfShots = 14;
        this.Primary.TakeAmmo = 2;
        this.AnimTbl_PrimaryFire = ACT_VM_SECONDARYATTACK;

        PrimaryAttack();
        this.Primary.Delay = 0.8;
        this.Primary.Cone = 12;
        this.Primary.NumberOfShots = 7;
        this.Primary.TakeAmmo = 1;
        this.AnimTbl_PrimaryFire = ACT_VM_PRIMARYATTACK;

        NextSecondaryFireTime = CurTime( + 1);
        return true;
    }

    public virtual void OnReload(status)
    {
        if (status == "Finish")
        var owner = Owner;
        if (!owner.IsPlayer()) return true 
        Owner.RemoveAmmo(1, this.Primary.Ammo);
        Clip1 = Clip1( + 1);
        if (this.Primary.ClipSize > Clip1)
        GameTask.DelaySeconds(0.1).ContinueWith(_ => function();
        if (this.IsValid() && Owner.IsValid())
        this.Reloading = false;
        Reload();

        end);

        return true;

    }

}