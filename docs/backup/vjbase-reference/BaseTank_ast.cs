using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class BaseTank : BaseNPC
{
    [Property] public bool VJ_ID_Boss = true;
    [Property] public int SightAngle = 360;
    [Property] public int SightDistance = 10000;
    [Property] public int TurningSpeed = 0;
    [Property] public object HullType = HULL_LARGE;
    [Property] public bool HasMeleeAttack = false;
    [Property] public bool Bleeds = false;
    [Property] public bool Immune_Dissolve = true;
    [Property] public bool Immune_Toxic = true;
    [Property] public bool Immune_Bullet = true;
    [Property] public object DeathCorpseCollisionType = COLLISION_GROUP_NONE;
    [Property] public bool HasPainSounds = false;
    [Property] public bool DisableWandering = true;
    [Property] public bool CanReceiveOrders = false;
    [Property] public string DeathAllyResponse = "OnlyAlert";
    [Property] public bool DamageAllyResponse = false;
    [Property] public bool CombatDamageResponse = false;
    [Property] public bool YieldToAlliedPlayers = false;

    public virtual void SCHEDULE_FACE(faceType, customFunc)
    {
        return;
    }

    public virtual void MaintainAlertBehavior(alwaysChase)
    {
        return;
    }

    public virtual void OnDamaged(dmginfo, hitgroup, status)
    {
        if (status == "Init")
        var dmgInflictor = dmginfo:GetInflictor();
        if (dmginfo:IsDamageType(DMG_PHYSGUN) || (dmgInflictor.IsValid() && dmgInflictor:GetClass() == "crossbow_bolt"))
        dmginfo:SetDamage(0);

        // Skip melee damages unless it's caused by a boss && is strong enough
        else if (status == "PreDamage" && (dmginfo:IsDamageType(DMG_SLASH) || dmginfo:IsDamageType(DMG_CLUB) || dmginfo:IsDamageType(DMG_GENERIC)))
        if (dmginfo:GetDamage() >= 30 && dmginfo:GetAttacker().VJ_ID_Boss)
        dmginfo:SetDamage(dmginfo:GetDamage() / 2);
        else
        dmginfo:SetDamage(0);

    }

    public virtual void Tank_AngleDiffuse(ang1, ang2)
    {
        var outcome = ang1 - ang2;
        if (outcome < -180) outcome = outcome + 360 
        if (outcome > 180) outcome = outcome - 360 
        return outcome;
    }

}