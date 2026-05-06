using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjFlareround : BaseProjectile
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Flare Round";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Flare that will burn for 1 minute.\nIgnites anything it touches.";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public bool PhysicsSounds = true;
    [Property] public int FuseTime = 60;

    public virtual void OnDraw()
    {
        //DrawModel()
    }

    public virtual void OnInitialize()
    {
        ModelRenderer.Model = "models/items/ar2_grenade.mdl";
        PhysicsInit(SOLID_VPHYSICS);
        // SetMoveType removed: MOVETYPE_VPHYSICS
        // SetSolid removed: SOLID_VPHYSICS
        ModelRenderer.Tint = VJ.COLOR_RED;
        SetUseType(SIMPLE_USE);
        SetModelScale(0.5);

        // Physics
        var phys = Rigidbody;
        if (phys.IsValid())
        phys.Wake();
        phys.EnableGravity(true);
        phys.SetBuoyancyRatio(0);


        // Effects
        //util.SpriteTrail(self, 0, Color(90, 90, 90, 255), false, 10, 1, 3, 1 / (15 + 1)*0.5, "trails/smoke.vmt")
        //Particles.Attach("vj_rocket_idle2_smoke2", PATTACH_ABSORIGIN_FOLLOW, self, 0)
        util.SpriteTrail(self, 0, colorTrailRed, false, 1, 100, 5, 5 / ((2 + 10) * 0.5), "trails/smoke.vmt");

        // No longer needed, light is created by env_flare


        var envFlare = SceneUtility.CreatePrefab();
        envFlare.SetPos(Transform.Position);
        envFlare.SetAngles(Transform.Rotation);
        envFlare.SetParent(self);
        envFlare.SetKeyValue("Scale", "5");
        envFlare.SetKeyValue("spawnflags", "4");
        envFlare.Spawn();
        envFlare.Fire("Start", tostring(this.FuseTime));
        envFlare.SetOwner(self);

        envFlare.SetColor(VJ.COLOR_RED);

        this.CurrentIdleSound = CreateSound(self, "weapons/flaregun/burn.wav");
        this.CurrentIdleSound.SetSoundLevel(60);
        this.CurrentIdleSound.PlayEx(1, 100);

        // Make it drop after some time in the air
        GameTask.DelaySeconds(2).ContinueWith(_ => function();
        if (this.IsValid())
        phys = Rigidbody;
        if (phys.IsValid() && phys.GetVelocity():Length() > 500)
        phys.SetMass(0.005);
        GameTask.DelaySeconds(10).ContinueWith(_ => function();
        if (this.IsValid())
        phys.SetMass(5);

        end);


        end);

        // Remove after fuse time
        GameTask.DelaySeconds(this.FuseTime).ContinueWith(_ => function();
        if (this.IsValid())
        GameObject.Destroy();

        end);
    }

    public virtual void Use(activator, caller)
    {
        if (activator.IsValid() && activator.IsPlayer())
        activator.PickupObject(self);

    }

    public virtual void PhysicsCollide(data, physobj)
    {
        // Make players and NPCs take damage
        var hitEnt = data.HitEntity;
        if (hitEnt.IsValid() && (hitEnt.IsNPC() || hitEnt.IsPlayer()))
        //hitEnt.Ignite(1)
        var dmg = DamageInfo();
        dmg.SetDamage(Game.Random.NextInt(4, 8));
        dmg.SetDamageType(DMG_BURN);
        dmg.SetAttacker(self);
        dmg.SetInflictor(self);
        dmg.SetDamagePosition(data.HitPos);
        hitEnt.TakeDamageInfo(dmg, self);

    }

    public virtual void OnTakeDamage(dmginfo)
    {
        Rigidbody.AddVelocity(dmginfo.GetDamageForce() * 0.1);
    }

    public virtual void OnRemove()
    {
        this.CurrentIdleSound?.Stop();
        StopParticles();
    }

}