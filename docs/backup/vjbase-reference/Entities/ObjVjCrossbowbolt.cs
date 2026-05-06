using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjCrossbowbolt : ObjVjProjectileBase
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Crossbow Bolt";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public object PhysicsSolidMask = MASK_SHOT;
    [Property] public string Model = "models/crossbow_bolt.mdl";
    [Property] public bool DoesDirectDamage = true;
    [Property] public int DirectDamage = 90;
    [Property] public string CollisionDecal = "Impact.Concrete";
    [Property] public string SoundTbl_Idle = "weapons/fx/nearmiss/bulletltor03.wav";
    [Property] public string SoundTbl_OnCollide = "weapons/crossbow/hit1.wav";
    [Property] public int IdleSoundLevel = 60;

    public virtual void InitPhys()
    {
        PhysicsInitSphere(1, "metal_bouncy");
    }

    public virtual void OnCollision(data, phys)
    {
        var hitEnt = data.HitEntity;
        if (hitEnt.IsValid())
        this.SoundTbl_OnCollide = sdHitEnt;
        // Ignite small entities
        if (hitEnt.IsNPC() && hitEnt.GetHullType() == HULL_TINY)
        hitEnt.Ignite(3);

        else;
        var bolt = SceneUtility.CreatePrefab();
        bolt.SetModel("models/crossbow_bolt.mdl");
        bolt.SetPos(data.HitPos + data.HitNormal + Transform.Forward*-15);
        bolt.SetAngles(Transform.Rotation);
        bolt.Activate();
        bolt.Spawn();
        GameTask.DelaySeconds(15).ContinueWith(_ => function() if (bolt.IsValid()) bolt.Remove() end end);

    }

}