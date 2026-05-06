using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class PropVjAnimatable : BaseProjectile
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "VJ Base Animatable Prop";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool AutomaticFrameAdvance = true;

    public virtual void OnInitialize()
    {
        // SetSolid removed: SOLID_OBB
    }

    public virtual void Think()
    {
        NextThink(Time.Now);
        return true;
    }

    public virtual void OnDraw()
    {
    }

    public virtual void OnDrawTranslucent()
    {
    }

}