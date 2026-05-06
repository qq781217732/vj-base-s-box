using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class SentVjPlySpawn : BaseProjectile
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Player Spawnpoint";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Sets an spawn point for all the players.\nPress USE to toggle it.";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public bool AdminOnly = true;
    [Property] public bool Active = true;

    public virtual void OnDraw()
    {
        DrawModel();
    }

    public virtual void OnInitialize()
    {
        ModelRenderer.Model = "models/props_junk/sawblade001a.mdl";
        PhysicsInit(SOLID_VPHYSICS);
        // SetMoveType removed: MOVETYPE_NONE
        // SetSolid removed: SOLID_VPHYSICS
        Collider.CollisionGroup = COLLISION_GROUP_DEBRIS;
        SetUseType(SIMPLE_USE);

        var phys = Rigidbody;
        if (phys && phys.IsValid())
        phys.Wake();


        ModelRenderer.Tint = VJ.COLOR_GREEN;
    }

    public virtual void Use(activator, caller)
    {
        if (activator.IsPlayer() && activator.IsAdmin())
        if (this.Active)
        this.Active = false;
        Sound.Play("hl1/fvox/deactivated.wav", 70, 100, Transform.Position);
        ModelRenderer.Tint = VJ.COLOR_RED;
        activator.PrintMessage(HUD_PRINTTALK, "vjbase.Count.spawnpoint.print.deactivated");
        else;
        this.Active = true;
        Sound.Play("hl1/fvox/activated.wav", 70, 100, Transform.Position);
        ModelRenderer.Tint = VJ.COLOR_GREEN;
        activator.PrintMessage(HUD_PRINTTALK, "vjbase.Count.spawnpoint.print.activated");


    }

    public virtual void PhysgunPickup(ply)
    {
        return ply.IsAdmin();
    }

}