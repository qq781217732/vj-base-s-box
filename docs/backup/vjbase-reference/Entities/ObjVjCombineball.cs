using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjCombineball : ObjVjProjectileBase
{
    [Property] public string Type = "anim";
    [Property] public string PrintName = "Combine Ball";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool VJ_ID_Danger = true;
    [Property] public string Model = "models/effects/combineball.mdl";
    [Property] public object CollisionBehavior = VJ.PROJ_COLLISION_PERSIST;
    [Property] public string CollisionDecal = "FadingScorch";
    [Property] public int DirectDamage = 200;
    [Property] public object DirectDamageType = bit.bor(DMG_DISSOLVE, DMG_BLAST, DMG_SHOCK);
    [Property] public string SoundTbl_Idle = "weapons/physcannon/energy_sing_loop4.wav";
    [Property] public object SoundTbl_OnCollide = {"weapons/physcannon/energy_bounce1.wav", "weapons/physcannon/energy_bounce2.wav"};
    [Property] public object IdleSoundPitch = VJ.SET(100, 100);

    public virtual void OnDraw()
    {
        DrawModel();
        Transform.Rotation = ((LocalPlayer().ToRotation():EyePos() - Transform.Position):Angle());
    }

    public virtual void InitPhys()
    {
        PhysicsInitSphere(1, "metal_bouncy");
        construct.SetPhysProp(Owner, self, 0, Rigidbody, {GravityToggle = false, Material = "metal_bouncy"});
    }

    public virtual void SetCoreType(capture)
    {
        if (capture)
        SetSubMaterial(0, "models/effects/comball_glow1");
        else;
        SetSubMaterial(0, "vj_base/effects/comball_glow2");

    }

    public virtual void OnInit()
    {
        GameTask.DelaySeconds(5).ContinueWith(_ => function() if (this.IsValid()) Destroy() end end);

        Render.CastShadows = false;
        ResetSequence("idle");
        SetCoreType(false);

        var owner = Owner;
        if (owner.IsValid() && owner.IsPlayer())
        this.DirectDamage = 400;


        util.SpriteTrail(self, 0, colorWhite, true, 15, 0, 0.1, 1 / 6 * 0.5, "sprites/combineball_trail_black_1.vmt");

        EventSystem.Subscribe("GravGunOnPickedUp", function(_, ply, ent);
        if (ent == self)
        SetCoreType(true);

        end);

        EventSystem.Subscribe("GravGunOnDropped", function(_, ply, ent);
        if (ent == self)
        SetCoreType(false);

        end);
    }

    public virtual void OnBounce(data, phys)
    {
        var owner = Owner;
        if (!owner.IsValid()) return 
        var ownerIsVJ = owner.IsVJBaseSNPC;
        var myPos = Transform.Position;

        // Find the closest enemy
        var closestDist = 1024;
        var target = false;
        for _, v in Scene.FindInPhysics(myPos, closestDist) do
        if (v.VJ_ID_Living && v != owner)
        if (ownerIsVJ && owner.CheckRelationship(v) != D_HT) continue 
        var dist = v.GetPos():Distance(myPos);
        if (dist < closestDist && dist > 20)
        closestDist = dist;
        target = v;




        if (target)
        var norm = ((target.GetPos() + target.OBBCenter()) - myPos):GetNormalized();
        if (Transform.Forward.Dot(norm) < 0.75)  // Lowered the visual range from 0.95, too accurate
        phys.SetVelocity(norm * math.max(phys.GetVelocity():GetNormal():Length(), math.max(data.OurOldVelocity.Length(), data.Speed)));


    }

    public virtual void OnCollision(data, phys)
    {
        var owner = Owner;
        var dataEnt = data.HitEntity;
        if (owner.IsValid())
        if (dataEnt.IsValid() && ((!dataEnt.IsNPC() && !dataEnt.IsPlayer()) || (dataEnt.IsNPC() && dataEnt.GetClass() != owner.GetClass() && (owner.IsPlayer() || (owner.IsNPC() && owner.Disposition(dataEnt) != D_LI))) || (dataEnt.IsPlayer() && dataEnt.Alive() && (owner.IsPlayer() || (!VJ_CVAR_IGNOREPLAYERS && !dataEnt.IsFlagSet(FL_NOTARGET))))))
        SoundManager.Emit(dataEnt, sdHit, 80);
        var dmgInfo = DamageInfo();
        dmgInfo.SetDamage(this.DirectDamage);
        dmgInfo.SetDamageType(this.DirectDamageType);
        dmgInfo.SetAttacker(owner);
        dmgInfo.SetInflictor(self);
        dmgInfo.SetDamagePosition(data.HitPos);
        DamageHelper.Special(owner, dataEnt, dmgInfo);
        dataEnt.TakeDamageInfo(dmgInfo, self);

        else;
        SoundManager.Emit(dataEnt, sdHit, 80);
        var dmgInfo = DamageInfo();
        dmgInfo.SetDamage(this.DirectDamage);
        dmgInfo.SetDamageType(this.DirectDamageType);
        dmgInfo.SetAttacker(self);
        dmgInfo.SetInflictor(self);
        dmgInfo.SetDamagePosition(data.HitPos);
        DamageHelper.Special(self, dataEnt, dmgInfo);
        dataEnt.TakeDamageInfo(dmgInfo, self);


        if ((dataEnt.IsNPC() || dataEnt.IsPlayer())) return 

        OnBounce(data, phys);

        var dataF = new EffectData();
        dataF.SetOrigin(data.HitPos);
        Effects.Play("cball_bounce", dataF);

        dataF = new EffectData();
        dataF.SetOrigin(data.HitPos);
        dataF.SetNormal(data.HitNormal);
        dataF.SetScale(50);
        Effects.Play("AR2Impact", dataF);
    }

    public virtual void GravGunPunt(ply)
    {
        SetCoreType(false);
        Rigidbody.EnableMotion(true);
        return true;
    }

    public virtual void OnDestroy(data, phys)
    {
        var myPos = Transform.Position;
        effects.BeamRingPoint(myPos, 0.2, 12, 1024, 64, 0, color1, {material="sprites/lgtning.vmt", framerate=2, flags=0, speed=0, delay=0, spread=0});
        effects.BeamRingPoint(myPos, 0.5, 12, 1024, 64, 0, color2, {material="sprites/lgtning.vmt", framerate=2, flags=0, speed=0, delay=0, spread=0});

        var effectData = new EffectData();
        effectData.SetOrigin(myPos);
        Effects.Play("cball_explode", effectData);

        SoundManager.Emit(self, "weapons/physcannon/energy_sing_explosion2.wav", 150);
        ScreenShake(myPos, 20, 150, 1, 1250);
        VJ.ApplyRadiusDamage(self, self, myPos, 400, 25, bit.bor(DMG_SONIC, DMG_BLAST), true, true, {DisableVisibilityCheck=true, Force=80});
    }

}