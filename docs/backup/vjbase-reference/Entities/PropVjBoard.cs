using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class PropVjBoard : BaseProjectile
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Wooden Board";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Physics entity that VJ NPCs can attack.\nUseful for barricading an area.";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public bool PhysicsSounds = true;
    [Property] public bool VJ_ID_Attackable = true;
    [Property] public int StartHealth = 50;

    public virtual void OnDraw()
    {
        DrawModel();
    }

    public virtual void OnInitialize()
    {
        ModelRenderer.Model = "models/props_debris/wood_board05a.mdl";
        PhysicsInit(SOLID_VPHYSICS);
        // SetMoveType removed: MOVETYPE_VPHYSICS
        // SetSolid removed: SOLID_VPHYSICS
        SetUseType(SIMPLE_USE);
        MaxHealth = this.StartHealth;
        Health = this.StartHealth;

        var phys = Rigidbody;
        if (phys && phys.IsValid())
        phys.Wake();

    }

    public virtual void Use(activator, caller)
    {
        if (activator.IsValid() && activator.IsPlayer())
        activator.PickupObject(self);

    }

    public virtual void OnTakeDamage(dmginfo)
    {
        Rigidbody.AddVelocity(dmginfo.GetDamageForce() * 0.05);
        Health = Health( - dmginfo.GetDamage());
        if (Health() <= 0)
        var effectData = new EffectData();
        effectData.SetOrigin(Transform.Position);
        Effects.Play("VJ_Dust_Small", effectData);
        GameObject.Destroy();

    }

}