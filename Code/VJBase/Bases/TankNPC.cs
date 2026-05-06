using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// Tank NPC — ported from vj_base/ai/base_tank.lua.
/// Heavy boss-type NPC with damage filtering and custom turning logic.
/// </summary>
public partial class TankNPC : CreatureNPC
{
    public TankNPC()
    {
        // Defaults from base_tank.lua
        VJ_ID_Boss = true;
        SightAngle = 360;
        SightDistance = 10000;
        TurningSpeed = 0;
        HasMeleeAttack = false;
        Bleeds = false;
        Immune_Dissolve = true;
        Immune_Toxic = true;
        Immune_Bullet = true;
        HasPainSounds = false;
        DisableWandering = true;
        CanReceiveOrders = false;
        DeathAllyResponse = "OnlyAlert";
        DamageAllyResponse = false;
        CombatDamageResponse = false;
        YieldToAlliedPlayers = false;
    }

    // ═══ Additional Fields ═══
    public bool VJ_ID_Boss { get; set; }
    public float TurningSpeed { get; set; }
    public int HullType { get; set; }
    public bool Bleeds { get; set; }
    public bool Immune_Dissolve { get; set; }
    public bool Immune_Toxic { get; set; }
    public bool Immune_Bullet { get; set; }
    public bool HasPainSounds { get; set; }
    public bool DisableWandering { get; set; }
    public bool CanReceiveOrders { get; set; }
    public string DeathAllyResponse { get; set; }
    public bool DamageAllyResponse { get; set; }
    public bool CombatDamageResponse { get; set; }
    public bool YieldToAlliedPlayers { get; set; }

    // ═══ SCHEDULE_FACE override — tanks don't turn like normal NPCs ═══
    public void SCHEDULE_FACE(string faceType, Action<AISchedule> customFunc) { /* no-op */ }

    // ═══ MaintainAlertBehavior override — tanks don't chase (base_tank.lua:33) ═══
    public override void MaintainAlertBehavior(bool alwaysChase) { /* no-op: tanks don't chase */ }

    // ═══ Identity ═══
    public bool IsVJBaseSNPC_Tank { get; set; } = true;

    // ═══ Tank-Specific Fields ═══
    public float RunOverDistance { get; set; } = 80;
    public float RunOverDamage { get; set; } = 50;
    public bool EnableIdleParticles { get; set; }
    public bool EnableMoveParticles { get; set; }

    // ═══ Tank Init ═══
    public virtual void Tank_Init() { }
    public virtual Vector3 Tank_GunnerSpawnPosition() => WorldPosition + WorldRotation.Forward * 50;

    // ═══ OnTouch — detect enemies to run over ═══
    public virtual void OnTouch(GameObject other)
    {
        if (Dead) return;
        if (other.Tags.Has("player") || other.Tags.Has("npc"))
            Tank_RunOver(other);
    }

    public virtual void Tank_RunOver(GameObject other)
    {
        // Phase 3: apply RunOverDamage to other
    }

    // ═══ Tank Think ═══
    public virtual bool Tank_OnThink() => false;
    public virtual void Tank_OnThinkActive() { }
    public virtual void Tank_OnRunOver(GameObject ent) { }

    // ═══ Particles (Phase 3: S&Box particle system) ═══
    public virtual void Tank_UpdateIdleParticles() { }
    public virtual void Tank_UpdateMoveParticles() { }

    // ═══ Death Sequence ═══
    public virtual bool Tank_OnInitialDeath(object dmginfo, int hitgroup) => false;

    // ═══ SelectSchedule (tank-specific) ═══
    public override void SelectSchedule()
    {
        if (Dead) return;
        var ene = GetEnemy();
        if (ene.IsValid())
            MaintainAlertBehavior(false);
        else if (Alerted == VJAlertState.None)
            MaintainIdleBehavior();
    }

    // ═══ OnDamaged — filter damage by type (base_tank.lua:35-49) ═══
    public virtual void OnDamaged(object dmginfo, int hitgroup, string status)
    {
        // lua:37-41: Init — skip gravity gun and crossbow bolt damage
        if (status == "Init")
        {
            // SKIP: lua:37-41 — dmginfo:GetInflictor() / IsDamageType(DMG_PHYSGUN) / GetClass("crossbow_bolt") — Source damage system, Phase 3
        }
        // lua:43-48: PreDamage — filter melee damage unless from boss
        else if (status == "PreDamage")
        {
            // SKIP: lua:43 — IsDamageType(DMG_SLASH) or IsDamageType(DMG_CLUB) or IsDamageType(DMG_GENERIC) — Source damage types, Phase 3
            // lua:44: damage >= 30 AND attacker is boss → halve damage
            // SKIP: lua:44-48 — dmginfo:GetDamage() / GetAttacker().VJ_ID_Boss / SetDamage — Source damage system, Phase 3
        }
    }

    // ═══ Tank_AngleDiffuse ═══
    public static float Tank_AngleDiffuse(float ang1, float ang2)
    {
        float outcome = ang1 - ang2;
        if (outcome < -180) outcome += 360;
        if (outcome > 180) outcome -= 360;
        return outcome;
    }
}
