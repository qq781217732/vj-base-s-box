using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class SentVjCampfire : BaseProjectile
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Campfire";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Gives a warm feeling, especially in cold maps.";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public bool IsOn = false;

    public virtual void SetupDataTables()
    {
        [Net] "Bool" "Activated"
        if (SERVER)
        SetActivated(false);

    }

    public virtual void OnDraw()
    {
        DrawModel();
    }

    public virtual void Think()
    {
        var curTime = Time.Now;
        if (curTime > this.NextActivationCheckT)
        if (GetActivated())
        // Needs to be done here instead of server side due to a GMod bug where particles don't show for the server host and certain clients while in multiplayer
        if (!this.CreatedParticles)
        Particles.Attach("env_fire_tiny_smoke", PATTACH_ABSORIGIN_FOLLOW, self, 0);
        Particles.Attach("env_embers_large", PATTACH_ABSORIGIN_FOLLOW, self, 0);
        this.CreatedParticles = true;

        var dynLight = DynamicLight(EntIndex());
        if (dynLight)
        dynLight.pos = Transform.Position + Transform.Up * 15;
        dynLight.r = 255;
        dynLight.g = 100;
        dynLight.b = 0;
        dynLight.brightness = 2;
        dynLight.size = 400;
        dynLight.decay = 400;
        dynLight.dietime = curTime + 1;

        else if (this.CreatedParticles)
        StopParticles()  // Sometimes server side call doesn't work while in multiplayer;
        this.CreatedParticles = false;

        this.NextActivationCheckT = curTime + 0.2;

    }

    public virtual void OnInitialize()
    {
        ModelRenderer.Model = "models/vj_base/fireplace.mdl";
        //PhysicsInit(SOLID_VPHYSICS)
        // SetMoveType removed: MOVETYPE_NONE
        // SetSolid removed: SOLID_VPHYSICS
        SetUseType(SIMPLE_USE);
        Collider.SetBounds(Vector(25, 25, 25), Vector(-25, -25, 1));

        // Supports spawning it activated (such as saves or duplicator)
        if (GetActivated())
        CampfireToggle(true);

    }

    public virtual void CampfireToggle(activate)
    {
        if (activate)
        SetActivated(true);
        this.IsOn = true;
        Sound.Play("ambient/fire/ignite.wav", 60, 100, Transform.Position);
        this.CurrentFireSound = CreateSound(self, "ambient/fire/fire_small_loop1.wav");
        this.CurrentFireSound.SetSoundLevel(60);
        this.CurrentFireSound.PlayEx(1, 100);
        else;
        SetActivated(false);
        this.IsOn = false;
        Sound.Play("ambient/fire/mtov_flame2.wav", 60, 100, Transform.Position);
        StopParticles();
        this.CurrentFireSound?.Stop();

    }

    public virtual void Use(activator, caller, useType, value)
    {
        if (!this.IsOn)
        CampfireToggle(true);
        if (activator.IsValid())
        activator.PrintMessage(HUD_PRINTTALK, "vjbase.Count.campfire.print.activated");

        else;
        CampfireToggle(false);
        if (activator.IsValid())
        activator.PrintMessage(HUD_PRINTTALK, "vjbase.Count.campfire.print.deactivated");


    }

    public virtual void Touch(entity)
    {
        if (entity.IsValid() && this.IsOn && entity.VJ_ID_Living && entity.GetPos():Distance(Transform.Position) <= 38)
        entity.Ignite(Game.Random.NextFloat(3, 5));

    }

    public virtual void OnRemove()
    {
        StopParticles();
        this.CurrentFireSound?.Stop();
    }

}