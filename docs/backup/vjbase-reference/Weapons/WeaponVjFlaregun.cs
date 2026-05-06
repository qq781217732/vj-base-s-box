using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjFlaregun : WeaponVjBase
{
    [Property] public string PrintName = "Flare Gun";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/vj_base/weapons/c_flaregun.mdl";
    [Property] public string WorldModel = "models/vj_base/weapons/w_flaregun.mdl";
    [Property] public string HoldType = "revolver";
    [Property] public int Slot = 1;
    [Property] public int SlotPos = 1;
    [Property] public int SwayScale = 4;
    [Property] public bool UseHands = true;
    [Property] public float NPC_NextPrimaryFire = 0.9;
    [Property] public float NPC_TimeUntilFire = 0.5;
    [Property] public object PrimaryEffects_MuzzleParticles = {"vj_rifle_smoke", "vj_rifle_smoke_dark", "vj_rifle_smoke_flash", "vj_rifle_sparks2"};
    [Property] public bool PrimaryEffects_MuzzleParticlesAsOne = true;
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public bool PrimaryEffects_SpawnShells = false;

    public virtual void OnPrimaryAttack(status, statusData)
    {
        if (status == "Init")
        if (CLIENT) return 
        var owner = Owner;
        var projectile = SceneUtility.CreatePrefab();
        var spawnPos = BulletPosition;
        if (owner.IsPlayer())
        projectile.SetPos(owner.GetShootPos());
        else;
        projectile.SetPos(spawnPos);

        projectile.SetOwner(owner);
        projectile.Activate();
        projectile.Spawn();

        var phys = projectile.GetPhysicsObject();
        if (phys.IsValid())
        if (owner.IsVJBaseSNPC)
        phys.SetVelocity(Trajectory.Calculate(owner, owner.GetEnemy(), "Line", spawnPos, 1, 15000));
        else if (owner.IsPlayer())
        phys.SetVelocity(owner.GetAimVector() * 15000);
        else;
        phys.SetVelocity(Trajectory.Calculate(owner, owner.GetEnemy(), "Line", spawnPos, owner.GetEnemy():GetPos() + owner.GetEnemy():OBBCenter(), 15000));

        projectile.SetAngles(projectile.GetVelocity():GetNormal():Angle());


    }

}