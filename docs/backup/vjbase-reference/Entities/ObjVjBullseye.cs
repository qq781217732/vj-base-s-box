using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjBullseye : BaseNPC
{
    [Property] public string Type = "ai";
    [Property] public string PrintName = "VJ Base Bullseye";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Target for VJ Base NPCs.";
    [Property] public string Category = "VJ Base";
    [Property] public bool IsVJBaseBullseye = true;
    [Property] public string SolidMovementType = "Dynamic";
    [Property] public bool CanToggle = false;
    [Property] public bool ToggleDisplayColors = true;
    [Property] public bool ForceEntAsEnemy = false;
    [Property] public bool Activated = true;

    public virtual void OnDraw()
    {
        DrawModel();
    }

    public virtual void OnInitialize()
    {
        //ModelRenderer.Model = "models/hunter/plates/plate.mdl"
        if (this.SolidMovementType == "Dynamic")
        PhysicsInit(SOLID_VPHYSICS);
        // SetMoveType removed: MOVETYPE_NONE
        // SetSolid removed: SOLID_VPHYSICS
        else if (this.SolidMovementType == "Static")
        PhysicsInit(SOLID_NONE);
        // SetMoveType removed: MOVETYPE_NONE
        // SetSolid removed: SOLID_NONE
        else if (this.SolidMovementType == "Physics")
        PhysicsInit(SOLID_VPHYSICS);
        // SetMoveType removed: MOVETYPE_VPHYSICS
        // SetSolid removed: SOLID_VPHYSICS

        SetUseType(SIMPLE_USE);
        MaxHealth = 999999;
        Health = 999999  // So SNPCs won't think it's dead;
    }

    public virtual void AcceptInput(key, activator, caller, data)
    {
        if (!activator.IsPlayer()) return 
        if (!this.Activated)
        this.Activated = true;
        activator.PrintMessage(HUD_PRINTTALK, "vjbase.Count.bullseye.print.activated");
        Sound.Play(sdActivated, 70, 100, Transform.Position);
        else if (this.Activated)
        this.Activated = false;
        activator.PrintMessage(HUD_PRINTTALK, "vjbase.Count.bullseye.print.deactivated");
        Sound.Play(sdDeactivated, 70, 100, Transform.Position);

    }

    public virtual void Think()
    {
        var selfData =;
        if (selfData.ForceEntAsEnemy) return 
        if (selfData.CanToggle)
        if (!selfData.Activated)
        AddFlags(FL_NOTARGET);
        if (selfData.ToggleDisplayColors) ModelRenderer.Tint = VJ.COLOR_RED 
        else if (selfData.Activated)
        RemoveFlags(FL_NOTARGET);
        if (selfData.ToggleDisplayColors) ModelRenderer.Tint = VJ.COLOR_GREEN 


    }

    public virtual void OnTakeDamage(dmginfo)
    {
        return 0  // Take no damage;
    }

}