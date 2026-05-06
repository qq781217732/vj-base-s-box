using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class SentVjPlyHealthkit : BaseProjectile
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Admin Health Kit";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Gives players 1000000 health when picked up.";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public bool AdminOnly = true;
    [Property] public bool PhysicsSounds = true;

    public virtual void OnDraw()
    {
        DrawModel();

        var myAng = Transform.Rotation;
        myAng.RotateAroundAxis(myAng.Right(), vec.x);
        myAng.RotateAroundAxis(myAng.Up(), vec.y);
        myAng.RotateAroundAxis(myAng.Forward(), vec.z);
        cam.Start3D2D(Transform.Position + Transform.Forward * 7 + Transform.Up * 6 + Transform.Right * 2, myAng, 0.07);
        draw.SimpleText("Admin Health Kit", "DermaLarge", 31, -22, textColor, 1, 1);
        cam.End3D2D();
    }

    public virtual void OnInitialize()
    {
        ModelRenderer.Model = "models/items/healthkit.mdl";
        PhysicsInit(SOLID_VPHYSICS);
        // SetMoveType removed: MOVETYPE_VPHYSICS
        // SetSolid removed: SOLID_VPHYSICS
        SetUseType(SIMPLE_USE);

        var phys = Rigidbody;
        if (phys.IsValid())
        phys.Wake();

    }

    public virtual void Use(activator, caller)
    {
        if (activator.IsPlayer())
        Sound.Play("items/smallmedkit1.wav", 70, 100, Transform.Position);
        activator.SetHealth(activator.Health() + 1000000);
        activator.PrintMessage(HUD_PRINTTALK, "vjbase.Count.adminhealth.print.pickup");
        GameObject.Destroy();

    }

    public virtual void OnTakeDamage(dmginfo)
    {
        Rigidbody.AddVelocity(dmginfo.GetDamageForce() * 0.1);
    }

}