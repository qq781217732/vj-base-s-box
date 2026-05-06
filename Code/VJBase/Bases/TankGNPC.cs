using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// Tank Gunner NPC — ported from npc_vj_tankg_base/init.lua.
/// Controls the tank turret: prepare shell, fire shell.
/// </summary>
public partial class TankGNPC : TankNPC
{
    public bool IsVJBaseSNPC_TankGun { get; set; } = true;

    // ═══ Shell System ═══
    public bool IsPreparingShell { get; set; }
    public float ShellPrepareTime { get; set; } = 1.5f;
    public float NextShellTime { get; set; }

    public virtual void Tank_PrepareShell()
    {
        IsPreparingShell = true;
        // Phase 3: animation + sound + timer → Tank_FireShell
    }

    public virtual void Tank_FireShell()
    {
        IsPreparingShell = false;
        // Phase 3: spawn projectile, apply force
    }

    public virtual bool Tank_OnPrepareShell() => false;
    public virtual void Tank_OnFireShell(string status, object statusData) { }

    // ═══ SelectSchedule (gunner-specific) ═══
    public override void SelectSchedule()
    {
        if (Dead) return;
        var ene = GetEnemy();
        if (ene.IsValid())
            MaintainAlertBehavior(false);
        else
            MaintainIdleBehavior();
    }
}
