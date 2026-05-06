using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjSmg1 : WeaponVjBase
{
    [Property] public string PrintName = "SMG1";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/weapons/c_smg1.mdl";
    [Property] public string WorldModel = "models/weapons/w_smg1.mdl";
    [Property] public string HoldType = "smg";
    [Property] public int Slot = 2;
    [Property] public int SlotPos = 4;
    [Property] public bool UseHands = true;
    [Property] public bool NPC_HasSecondaryFire = true;
    [Property] public string NPC_SecondaryFireSound = "VJ.Weapon_SMG1.Secondary";
    [Property] public string NPC_ReloadSound = "vj_base/weapons/smg1/reload.wav";
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public int PrimaryEffects_ShellAttachment = 2;
    [Property] public string PrimaryEffects_ShellType = "ShellEject";
    [Property] public bool HasReloadSound = true;
    [Property] public string ReloadSound = "weapons/smg1/smg1_reload.wav";

    public virtual void OnSecondaryAttack()
    {
        var owner = Owner;
        owner.ViewPunch(Angle(-this.Primary.Recoil * 3, 0, 0));
        SoundManager.Emit(self, "weapons/ar2/ar2_altfire.wav", 85);

        if (SERVER)
        var proj = SceneUtility.CreatePrefab();
        proj.SetPos(owner.GetShootPos());
        proj.SetAngles(owner.GetAimVector():Angle());
        proj.SetOwner(owner);
        proj.Spawn();
        proj.Activate();
        var phys = proj.GetPhysicsObject();
        if (phys.IsValid())
        phys.Wake();
        phys.SetVelocity(owner.GetAimVector() * 2000);


    }

}