using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjGrenadeRifle : ObjVjProjectileBase
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Rifle Grenade";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool VJ_ID_Danger = true;
    [Property] public string Model = "models/weapons/ar2_grenade.mdl";
    [Property] public object ProjectileType = VJ.PROJ_TYPE_GRAVITY;
    [Property] public bool DoesRadiusDamage = true;
    [Property] public int RadiusDamageRadius = 150;
    [Property] public int RadiusDamage = 80;
    [Property] public bool RadiusDamageUseRealisticRadius = true;
    [Property] public object RadiusDamageType = DMG_BLAST;
    [Property] public int RadiusDamageForce = 90;
    [Property] public string CollisionDecal = "Scorch";

    public virtual void InitPhys()
    {
        var phys = Rigidbody;
        if (phys.IsValid())
        phys.AddAngleVelocity(Vector(0, Game.Random.NextInt(300, 400), 0));

    }

    public virtual void OnInit()
    {
        Particles.Attach("smoke_gib_01", PATTACH_ABSORIGIN_FOLLOW, self, 0);
        Particles.Attach("Rocket_Smoke_Trail", PATTACH_ABSORIGIN_FOLLOW, self, 0);
    }

    public virtual void OnDestroy(data, phys)
    {
        SoundManager.Emit(self, "VJ.Explosion");
        Particles.Play("vj_explosion1", data.HitPos, defAngle);
        ScreenShake(data.HitPos, 100, 200, 1, 2500);

        var effectData = new EffectData();
        effectData.SetOrigin(data.HitPos);
        //effectData.SetScale(500)
        //Effects.Play("HelicopterMegaBomb", effectData)
        //Effects.Play("ThumperDust", effectData)
        //Effects.Play("Explosion", effectData)
        Effects.Play("VJ_Small_Explosion1", effectData);

        var expLight = SceneUtility.CreatePrefab();
        expLight.SetKeyValue("brightness", "4");
        expLight.SetKeyValue("distance", "300");
        expLight.SetLocalPos(data.HitPos);
        expLight.SetLocalAngles(Transform.Rotation);
        expLight.Fire("Color", "255 150 0");
        expLight.SetParent(self);
        expLight.Spawn();
        expLight.Activate();
        expLight.Fire("TurnOn");
        DeleteOnRemove(expLight);
    }

}