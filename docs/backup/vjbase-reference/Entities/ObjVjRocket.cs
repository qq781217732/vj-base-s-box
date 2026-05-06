using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjRocket : ObjVjProjectileBase
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Tank Shell";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool VJ_ID_Danger = true;
    [Property] public string Model = "models/weapons/w_missile_launch.mdl";
    [Property] public bool DoesRadiusDamage = true;
    [Property] public int RadiusDamageRadius = 250;
    [Property] public int RadiusDamage = 110;
    [Property] public bool RadiusDamageUseRealisticRadius = true;
    [Property] public object RadiusDamageType = DMG_BLAST;
    [Property] public int RadiusDamageForce = 90;
    [Property] public string CollisionDecal = "Scorch";
    [Property] public string SoundTbl_Idle = "weapons/rpg/rocket1.wav";
    [Property] public string SoundTbl_OnCollide = "ambient/explosions/explode_8.wav";

    public virtual void OnInit()
    {
        //util.SpriteTrail(self, 0, Color(90, 90, 90, 255), false, 10, 1, 3, 1 / (15 + 1)*0.5, "trails/smoke.vmt")
        Particles.Attach("vj_rocket_idle1", PATTACH_ABSORIGIN_FOLLOW, self, 0);
        Particles.Attach("vj_rocket_idle2", PATTACH_ABSORIGIN_FOLLOW, self, 0);
        //Particles.Attach("rocket_smoke", PATTACH_ABSORIGIN_FOLLOW, self, 0)
        //Particles.Attach("smoke_burning_engine_01", PATTACH_ABSORIGIN_FOLLOW, self, 0)

        //local dynLight = SceneUtility.CreatePrefab()
        //dynLight.SetKeyValue("brightness", "1")
        //dynLight.SetKeyValue("distance", "200")
        //dynLight.SetLocalPos(Transform.Position)
        //dynLight.SetLocalAngles( Transform.Rotation )
        //dynLight.Fire("Color", "255 150 0")
        //dynLight.SetParent(self)
        //dynLight.Spawn()
        //dynLight.Activate()
        //dynLight.Fire("TurnOn")
        //DeleteOnRemove(dynLight)
    }

    public virtual void OnDestroy(data, phys)
    {
        SoundManager.Emit(self, "VJ.Explosion");
        Particles.Play("vj_explosion3", data.HitPos, defAngle);
        ScreenShake(data.HitPos, 16, 200, 1, 3000);

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