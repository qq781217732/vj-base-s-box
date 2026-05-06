using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjGrenade : ObjVjProjectileBase
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Grenade";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Default VJ grenade, will explode after 3 seconds.";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public bool VJ_ID_Grenade = true;
    [Property] public bool VJ_ID_Grabbable = true;
    [Property] public string Model = "models/vj_base/weapons/w_grenade.mdl";
    [Property] public object ProjectileType = VJ.PROJ_TYPE_PROP;
    [Property] public bool DoesRadiusDamage = true;
    [Property] public int RadiusDamageRadius = 250;
    [Property] public int RadiusDamage = 80;
    [Property] public bool RadiusDamageUseRealisticRadius = true;
    [Property] public object RadiusDamageType = DMG_BLAST;
    [Property] public int RadiusDamageForce = 90;
    [Property] public object CollisionBehavior = VJ.PROJ_COLLISION_NONE;
    [Property] public string CollisionDecal = "Scorch";
    [Property] public string SoundTbl_OnCollide = "weapons/hegrenade/he_bounce-1.wav";
    [Property] public int FuseTime = 3;

    public virtual void OnInit()
    {
        AddFlags(FL_GRENADE);
        GameTask.DelaySeconds(this.FuseTime).ContinueWith(_ => function();
        if (this.IsValid())
        Destroy();

        end);
    }

    public virtual void OnDamaged(dmginfo)
    {
        var phys = Rigidbody;
        if (phys.IsValid())
        phys.AddVelocity(dmginfo.GetDamageForce() * 0.1);

    }

    public virtual void OnCollision(data, phys)
    {
        var getVel = phys.GetVelocity();
        var curVelSpeed = getVel.Length();
        //print(curVelSpeed)
        if (curVelSpeed > 500)  // Or else it will go flying!
        phys.SetVelocity(getVel * 0.9);


        if (curVelSpeed > 100)  // If the grenade is going faster than 100, then play the touch sound
        PlaySound("OnCollide");

    }

    public virtual void OnDestroy()
    {
        var myPos = Transform.Position;

        SoundManager.Emit(self, "VJ.Explosion");
        Particles.Play("vj_explosion1", myPos, defAngle);
        ScreenShake(myPos, 100, 200, 1, 2500);

        var effectData = new EffectData();
        effectData.SetOrigin(myPos);
        //effectData.SetScale(500)
        //Effects.Play("HelicopterMegaBomb", effectData)
        //Effects.Play("ThumperDust", effectData)
        //Effects.Play("Explosion", effectData)
        Effects.Play("VJ_Small_Explosion1", effectData);

        var expLight = SceneUtility.CreatePrefab();
        expLight.SetKeyValue("brightness", "4");
        expLight.SetKeyValue("distance", "300");
        expLight.SetLocalPos(myPos);
        expLight.SetLocalAngles(Transform.Rotation);
        expLight.Fire("Color", "255 150 0");
        expLight.SetParent(self);
        expLight.Spawn();
        expLight.Activate();
        expLight.Fire("TurnOn");
        DeleteOnRemove(expLight);

        LocalTransform.Position = myPos + vecZ4  // Because the entity is too close to the ground;
        var tr = util.TraceLine({
        start = myPos,;
        endpos = myPos - vezZ100,;
        filter = self;
        });
        Decals.Place(Game.Random.FromList(this.CollisionDecal), tr.HitPos + tr.HitNormal, tr.HitPos - tr.HitNormal);

        DealDamage();
    }

}