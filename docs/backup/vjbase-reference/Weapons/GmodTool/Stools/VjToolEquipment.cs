using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolEquipment : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_equipment.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;
    [Property] public object ClientConVar = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid() || !ent.IsNPC()) return 
        if (ent.GetActiveWeapon(.IsValid())) ent.GetActiveWeapon():Remove() 
        var equipment = GetClientInfo("weaponclass");
        if (equipment != "None")
        ApplyWeapon(Owner, ent, {equipment});
        // ent.SetSaveValue("additionalequipment", equipment)

        return true;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function TOOL.RightClick(tr);
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid() || !ent.IsNPC()) return 
        if (ent.GetActiveWeapon(.IsValid())) ent.GetActiveWeapon():Remove() 
        return true;
    }

    public virtual void BuildCPanel(panel)
    {
        ControlPanel(panel);
    }

}