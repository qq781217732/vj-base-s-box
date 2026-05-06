using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjAr2 : WeaponVjBase
{
    [Property] public string PrintName = "AR2";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/weapons/c_irifle.mdl";
    [Property] public string WorldModel = "models/weapons/w_irifle.mdl";
    [Property] public string HoldType = "ar2";
    [Property] public int Slot = 2;
    [Property] public int SlotPos = 4;
    [Property] public bool UseHands = true;
    [Property] public bool NPC_HasSecondaryFire = true;
    [Property] public string NPC_SecondaryFireEnt = "obj_vj_combineball";
    [Property] public int NPC_SecondaryFireDistance = 3000;
    [Property] public int NPC_SecondaryFireChance = 4;
    [Property] public object NPC_SecondaryFireNext = VJ.SET(15, 20);
    [Property] public string NPC_SecondaryFireSound = "VJ.Weapon_AR2.Secondary";
    [Property] public float NPC_NextPrimaryFire = 0.9;
    [Property] public float NPC_TimeUntilFire = 0.1;
    [Property] public object NPC_TimeUntilFireExtraTimers = {0.1, 0.2, 0.3, 0.4, 0.5, 0.6};
    [Property] public string NPC_ReloadSound = "vj_base/weapons/ar2/reload.wav";
    [Property] public object PrimaryEffects_MuzzleParticles = {"vj_rifle_full_blue"};
    [Property] public bool PrimaryEffects_SpawnShells = false;
    [Property] public Color PrimaryEffects_DynamicLightColor = Color(0, 31, 225);
    [Property] public string DryFireSound = "weapons/ar2/ar2_empty.wav";
    [Property] public bool HasReloadSound = false;
    [Property] public string ReloadSound = "weapons/ar2/ar2_reload.wav";
    [Property] public float Reload_TimeUntilAmmoIsSet = 0.8;

    public virtual void NPC_SecondaryFire_BeforeTimer(eneEnt, fireTime)
    {
        SoundManager.Emit(self, "weapons/cguard/charging.wav", 70);
    }

    public virtual void OnSecondaryAttack()
    {
        var owner = Owner;
        var vm = owner.GetViewModel();
        var fidgetTime = AnimationHelper.Duration(vm, ACT_VM_FIDGET);
        var fireTime = AnimationHelper.Duration(vm, ACT_VM_SECONDARYATTACK);
        var totalTime = fidgetTime + fireTime;
        var curTime = Time.Now;
        NextSecondaryFireTime = curTime + totalTime;
        this.PLY_NextIdleAnimT = curTime + totalTime;
        this.PLY_NextReloadT = curTime + totalTime;
        SendWeaponAnim(ACT_VM_FIDGET);
        VJ.CreateSound(self, "weapons/cguard/charging.wav", 85);

        GameTask.DelaySeconds(fidgetTime).ContinueWith(_ => function();
        if (this.IsValid() && owner.IsValid() && owner.GetActiveWeapon() == self)
        VJ.CreateSound(self, "weapons/irifle/irifle_fire2.wav", 90);

        if (SERVER)
        var projectile = SceneUtility.CreatePrefab();
        projectile.SetPos(owner.GetShootPos());
        projectile.SetAngles(owner.GetAimVector():Angle());
        projectile.SetOwner(owner);
        projectile.Spawn();
        projectile.Activate();
        var phys = projectile.GetPhysicsObject();
        if (phys.IsValid())
        phys.Wake();
        phys.SetVelocity(owner.GetAimVector() * 2000);



        owner.ViewPunch(Angle(-this.Primary.Recoil * 3, 0, 0));
        owner.SetAnimation(PLAYER_ATTACK1);
        SendWeaponAnim(ACT_VM_SECONDARYATTACK);
        TakeSecondaryAmmo(1);

        end);
        return true;
    }

}