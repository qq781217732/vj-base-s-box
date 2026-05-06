using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class PropVjFlag : PropVjAnimatable
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Flag";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Extremely based Armenian flag!";
    [Property] public string Category = "VJ Base";
    [Property] public bool PhysicsSounds = true;

    public virtual void OnInitialize()
    {
        ModelRenderer.Model = "models/vj_base/flag_armenia.mdl";
        PhysicsInit(SOLID_VPHYSICS);
        // SetMoveType removed: MOVETYPE_VPHYSICS
        ResetSequence("Idle");

        this.WaveSound = VJ.CreateSound(self, "vj_base/ambience/flag_loop.wav", 60);
    }

    public virtual void OnRemove()
    {
        this.WaveSound?.Stop();
    }

}