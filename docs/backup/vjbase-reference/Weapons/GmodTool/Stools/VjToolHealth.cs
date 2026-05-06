using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolHealth : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_health.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;
    [Property] public object ClientConVar = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid()) return 
        var ply = Owner;
        var heal = true;

        if ((ent.Health() != 0) || (ent.IsNPC() || ent.IsPlayer() || ent.IsNextBot()))
        if (ent.IsPlayer() && !ply.IsAdmin())
        heal = false;

        if (heal)
        ent.SetHealth(GetClientNumber("health"));
        ply.ChatPrint("Set " + ent.GetClass() + "'s health to " + GetClientNumber("health"));
        if (ent.IsNPC())
        if (GetClientNumber("godmode") == 1) ent.GodMode = true else ent.GodMode = false 
        if (ent.IsVJBaseSNPC && GetClientNumber("healthregen") == 1)
        var healthRegen = ent.HealthRegenParams;
        healthRegen.Enabled = true;
        healthRegen.Amount = GetClientNumber("healthregen_amt");
        healthRegen.Delay = new Vector2(GetClientNumber("healthregen_delay"), GetClientNumber("healthregen_delay"));


        return true;


    }

    public virtual void RightClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid()) return 
        var ply = Owner;
        var heal = true;

        if ((ent.Health() != 0) || (ent.IsNPC() || ent.IsPlayer()))
        if (ent.IsPlayer() && !ply.IsAdmin())
        heal = false;

        if (heal)
        ent.SetHealth(GetClientNumber("health"));
        ent.SetMaxHealth(GetClientNumber("health"));
        ply.ChatPrint("Set " + ent.GetClass() + "'s health && max health to " + GetClientNumber("health"));
        if (ent.IsNPC())
        if (GetClientNumber("godmode") == 1) ent.GodMode = true else ent.GodMode = false 
        if (ent.IsVJBaseSNPC && GetClientNumber("healthregen") == 1)
        var healthRegen = ent.HealthRegenParams;
        healthRegen.Enabled = true;
        healthRegen.Amount = GetClientNumber("healthregen_amt");
        healthRegen.Delay = new Vector2(GetClientNumber("healthregen_delay"), GetClientNumber("healthregen_delay"));


        return true;


    }

    public virtual void OnReload(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid()) return 
        var ply = Owner;
        var heal = true;

        if ((ent.Health() != 0) || (ent.IsNPC() || ent.IsPlayer()))
        if (ent.IsPlayer() && !ply.IsAdmin())
        heal = false;

        if (heal)
        ent.SetHealth(ent.GetMaxHealth());
        ply.ChatPrint("Healed " + ent.GetClass() + " to its max health (" + ent.GetMaxHealth() + ")");
        return true;


    }

    public virtual void BuildCPanel(panel)
    {
        ControlPanel(panel);
    }

}