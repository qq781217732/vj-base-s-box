using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjRpg : WeaponVjBase
{
    [Property] public string PrintName = "RPG";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/vj_base/weapons/c_rpg7.mdl" // "models/weapons/c_rpg.mdl";
    [Property] public string WorldModel = "models/vj_base/weapons/w_rpg7.mdl" // "models/weapons/w_rocket_launcher.mdl";
    [Property] public bool WorldModel_UseCustomPosition = true;
    [Property] public Vector3 WorldModel_CustomPositionAngle = Vector(-10, 0, 180);
    [Property] public Vector3 WorldModel_CustomPositionOrigin = Vector(-1.5, -0.5, 1);
    [Property] public string HoldType = "rpg";
    [Property] public int ViewModelFOV = 60;
    [Property] public int Slot = 4;
    [Property] public int SlotPos = 4;
    [Property] public bool UseHands = true;
    [Property] public int NPC_NextPrimaryFire = 5;
    [Property] public float NPC_TimeUntilFire = 0.8;
    [Property] public string NPC_BulletSpawnAttachment = "missile";
    [Property] public float NPC_FiringDistanceScale = 2.5;
    [Property] public bool NPC_StandingOnly = true;
    [Property] public int PrimaryEffects_MuzzleAttachment = 1;
    [Property] public bool PrimaryEffects_SpawnShells = false;
    [Property] public bool HasReloadSound = true;
    [Property] public float Reload_TimeUntilAmmoIsSet = 0.8;
    [Property] public string ReloadSound = "vj_base/weapons/reload_rpg.wav";

    public virtual void OnPrimaryAttack(status, statusData)
    {
        if (status == "Init")
        if (CLIENT) return 
        var owner = Owner;
        var projectile = SceneUtility.CreatePrefab();
        var spawnPos = BulletPosition;
        if (owner.IsPlayer())
        var plyAng = owner.GetAimVector():Angle();
        projectile.SetPos(owner.GetShootPos() + plyAng.Forward()*-20 + plyAng.Up()*-9 + plyAng.Right()*10);
        owner.GetViewModel():SetBodygroup(1, 1);
        else;
        projectile.SetPos(spawnPos);

        projectile.SetOwner(owner);
        projectile.Activate();
        projectile.Spawn();

        var phys = projectile.GetPhysicsObject();
        if (phys.IsValid())
        if (owner.IsVJBaseSNPC)
        phys.SetVelocity(Trajectory.Calculate(owner, owner.GetEnemy(), "Line", spawnPos, 1, 2500));
        else if (owner.IsPlayer())
        phys.SetVelocity(owner.GetAimVector() * 2500);
        else;
        phys.SetVelocity(Trajectory.Calculate(owner, owner.GetEnemy(), "Line", spawnPos, owner.GetEnemy():GetPos() + owner.GetEnemy():OBBCenter(), 2500));

        projectile.SetAngles(projectile.GetVelocity():GetNormal():Angle());


        SetBodygroup(1, 1);

    }

    public virtual void PrimaryAttackEffects(owner)
    {
        Particles.Attach("smoke_exhaust_01a", PATTACH_POINT_FOLLOW, self, 2);
        Particles.Attach("smoke_exhaust_01a", PATTACH_POINT_FOLLOW, self, 2);
        Particles.Attach("smoke_exhaust_01a", PATTACH_POINT_FOLLOW, self, 2);
        GameTask.DelaySeconds(4).ContinueWith(_ => function() if (this.IsValid()) StopParticles() end end);
        this.BaseClass.PrimaryAttackEffects(self, owner);
    }

    public virtual void OnReload(status)
    {
        if (status == "Finish")
        SetBodygroup(1, 0);
        var owner = Owner;
        if (owner.IsValid() && owner.IsPlayer())
        owner.GetViewModel():SetBodygroup(1, 0);


    }

}