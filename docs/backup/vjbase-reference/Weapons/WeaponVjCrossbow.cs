using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjCrossbow : WeaponVjBase
{
    [Property] public string PrintName = "Crossbow";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public string WorldModel = "models/weapons/w_crossbow.mdl";
    [Property] public string HoldType = "crossbow";
    [Property] public bool MadeForNPCsOnly = true;
    [Property] public string ReplacementWeapon = "weapon_crossbow";
    [Property] public int NPC_NextPrimaryFire = 1;
    [Property] public float NPC_TimeUntilFire = 0.15;
    [Property] public string NPC_ReloadSound = "weapons/crossbow/reload1.wav";
    [Property] public float NPC_FiringDistanceScale = 2.5;
    [Property] public bool NPC_StandingOnly = true;
    [Property] public object PrimaryEffects_MuzzleParticles = {"vj_rifle_smoke", "vj_rifle_smoke_dark", "vj_rifle_smoke_flash", "vj_rifle_sparks2"};
    [Property] public bool PrimaryEffects_MuzzleParticlesAsOne = true;
    [Property] public string PrimaryEffects_MuzzleAttachment = "muzzle";
    [Property] public bool PrimaryEffects_SpawnShells = false;

    public virtual void OnPrimaryAttack(status, statusData)
    {
        if (status == "Init")
        if (CLIENT) return 
        var projectile = SceneUtility.CreatePrefab();
        var spawnPos = BulletPosition;
        var owner = Owner;
        projectile.SetPos(spawnPos);
        projectile.SetOwner(owner);
        projectile.Activate();
        projectile.Spawn();

        var phys = projectile.GetPhysicsObject();
        if (owner.IsVJBaseSNPC)
        phys.SetVelocity(Trajectory.Calculate(owner, owner.GetEnemy(), "Line", spawnPos + Vector(Game.Random.NextFloat(-30, 30), Game.Random.NextFloat(-30, 30), Game.Random.NextFloat(-30, 30)), 1, 4000));
        else;
        phys.SetVelocity(Trajectory.Calculate(owner, owner.GetEnemy(), "Line", spawnPos, owner.GetEnemy():GetPos() + owner.GetEnemy():OBBCenter(), 4000));

        projectile.SetAngles(projectile.GetVelocity():GetNormal():Angle());

    }

    public virtual void OnReload(status)
    {
        if (status == "Start")
        GameTask.DelaySeconds(SoundDuration("weapons/crossbow/reload1.wav")).ContinueWith(_ => function();
        if (this.IsValid() && Owner.IsValid())
        SoundManager.Emit(Owner, sdLoadDone, this.NPC_ReloadSoundLevel);

        end);

    }

}