using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// Human NPC — ported from npc_vj_human_base/init.lua.
/// Adds weapon system, grenades, and human-specific chase/alert behavior.
/// </summary>
public partial class HumanNPC : CreatureNPC
{
    // ═══ Identity ═══
    public bool IsVJBaseSNPC_Human { get; set; } = true;

    // ═══ Weapon Fields ═══
    public VJWepState WeaponState { get; set; } = VJWepState.Ready;
    public GameObject WeaponEntity { get; set; }
    public bool HasWeapon { get; set; }
    public bool AllowWeaponOcclusionDelay { get; set; }
    public float NextThrowGrenadeT { get; set; }

    // ═══ Attack Config ═══
    public float MeleeAttackDistance { get; set; } = 50;
    public float MeleeAttackAngleRadius { get; set; } = 45;
    public float NextAnyAttackTime_Melee { get; set; } = 1.5f;
    public float TimeUntilMeleeAttackDamage { get; set; } = 0.3f;
    public float NextMeleeAttackTime { get; set; } = 0.8f;
    public float NextAnyAttackTime_Grenade { get; set; } = 3f;
    public float GrenadeAttackThrowTime { get; set; } = 1f;
    public float NextGrenadeAttackTime { get; set; } = 5f;
    public bool DisableChasingEnemy { get; set; }
    public bool HasGrenadeAttack { get; set; }

    public HumanNPC()
    {
        // Human-specific sound defaults (npc_vj_human_base/init.lua)
        HasExtraMeleeAttackSounds = true;
        HasSuppressingSounds = true;
        HasWeaponReloadSounds = true;
        HasGrenadeAttackSounds = true;
        HasDangerSightSounds = true;
        IdleSoundChance = 3;
        NextSoundTime_Idle = (8f, 25f);
        NextSoundTime_Suppressing = (7f, 15f);
        FootstepSoundTimerWalk = 0.5f;
        FootstepSoundTimerRun = 0.25f;
        SoundTbl_FootStep = new() { "VJ.Footstep.Human" };
        SoundTbl_MeleeAttackExtra = new() { "Flesh.ImpactHard" };
        SoundTbl_MeleeAttackMiss = new() { "Zombie.AttackMiss" };
        SoundTbl_Impact = new() { "Flesh.BulletImpact" };
    }

    // Note: Behavior is defined on BaseNPC, default Aggressive
    // HumanNPC sets it via constructor if needed

    // ═══ SetWeaponState ═══
    public virtual void SetWeaponState(VJWepState state = VJWepState.Ready, float time = -1)
    {
        WeaponState = state;
        // Phase 3: timer-based state reset
    }

    public virtual VJWepState GetWeaponState() => WeaponState;

    // ═══ SCHEDULE_ALERT_CHASE — human_base/init.lua:2340 ═══
    public virtual void SCHEDULE_ALERT_CHASE(bool doLOSChase)
    {
        // init.lua:2342: self:ClearCondition(COND_ENEMY_UNREACHABLE)
        ClearCondition(Condition.EnemyUnreachable);

        // init.lua:2344: AA branch
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        { AA_ChaseEnemy(); return; }

        // init.lua:2346: CurrentScheduleName guard
        if (CurrentScheduleName == "SCHEDULE_ALERT_CHASE") return;

        // init.lua:2347: navtype check
        var navType = GetNavType();
        if (navType == (int)NavType.Jump || navType == (int)NavType.Climb) return;

        var sched = new AISchedule();
        sched.Init("SCHEDULE_ALERT_CHASE");
        sched.EngTask(doLOSChase ? EngineTask.GetPathToEnemyLOS : EngineTask.GetPathToEnemy, 0);
        sched.EngTask(EngineTask.RunPath, 0);
        sched.EngTask(EngineTask.WaitForMovement, 0);
        sched.CanShootWhenMoving = true;

        // SKIP: init.lua:2349-2353 — doLOSChase=true uses schedule_alert_chaseLOS with RunCode_OnFinish → SCHEDULE_ALERT_CHASE(false)
        // SKIP: init.lua:2355-2356 — doLOSChase=false uses schedule_alert_chase (pre-built) with RunCode_OnFail → SCHEDULE_IDLE_STAND

        StartSchedule(sched);
    }

    // ═══ MaintainAlertBehavior (human version) ═══
    public override void MaintainAlertBehavior(bool alwaysChase)
    {
        var curTime = Time.Now;
        if (NextChaseTime > curTime || Dead || VJ_IsBeingControlled || Flinching || GetState() == VJState.OnlyAnimationConstant) return;

        var ene = Enemy.Target;
        if (!ene.IsValid() || TakingCoverT > curTime || (AttackAnimTime > curTime && MovementType != VJMoveType.Aerial && MovementType != VJMoveType.Aquatic)) return;

        // Melee range check — stand and attack
        if (HasMeleeAttack && Enemy.DistanceNearest < MeleeAttackDistance && Enemy.Visible)
        {
            if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
                AA_StopMoving();
            // Idle stand while in melee range
            return;
        }

        if (MovementType == VJMoveType.Stationary || IsFollowing || Medic.Status != "false" || GetState() == VJState.OnlyAnimation)
            return;

        if (Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature)
        {
            NextChaseTime = curTime + 3;
            return;
        }

        if (!alwaysChase && (DisableChasingEnemy || IsGuard)) return;

        SCHEDULE_ALERT_CHASE(false);
        NextChaseTime = curTime + (Enemy.Distance > 2000 ? 1f : 0.1f);
    }

    // ═══ Grenade Attack Stub ═══
    public virtual void GrenadeAttack(GameObject customEnt = null, bool disableOwner = false) { /* Phase 3 */ }
    public virtual void ExecuteGrenadeAttack(GameObject customEnt = null, bool disableOwner = false, object landDir = null) { /* Phase 3 */ }

    // ═══ Weapon Events (stubs — Phase 3) ═══
    public virtual void OnWeaponChange(GameObject newWeapon, GameObject oldWeapon, bool invSwitch) { }
    public virtual bool OnWeaponCanFire() => true;
    public virtual void OnWeaponAttack() { }
    public virtual bool OnWeaponStrafe() => true;
    public virtual void OnWeaponReload() { }
}
