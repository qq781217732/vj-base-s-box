using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolScanner : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_scanner.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid()) return false 
        var ply = Owner;
        var phys = ent.GetPhysicsObject();
        ply.PrintMessage(HUD_PRINTCONSOLE, "-----------------------------------------------------------------------------------------------");
        ply.PrintMessage(HUD_PRINTCONSOLE, "====> " + tostring(ent) + " / " + VJ.GetName(ent) + " <==== \n\n");
        ply.PrintMessage(HUD_PRINTCONSOLE, "MODEL    ==> " + ent.GetModel() + " ;;; Skin = " + ent.GetSkin() + "\n\n");
        ply.PrintMessage(HUD_PRINTCONSOLE, "POSITION ==> Vector(" + ent.GetPos().x + ", " + ent.GetPos().y + ", " + ent.GetPos().z + ")\n\n");
        ply.PrintMessage(HUD_PRINTCONSOLE, "ANGLE    ==> Angle(" + ent.GetAngles().p + ", " + ent.GetAngles().y + ", " + ent.GetAngles().r + ")\n\n");
        ply.PrintMessage(HUD_PRINTCONSOLE, "SEQUENCE ==> \"" + ent:GetSequenceName(ent.GetSequence()) + "\" [" + ent.GetSequence() + "] ;;; Duration = " + AnimationHelper.Duration(ent, ent.GetSequenceName(ent.GetSequence())) + "\n\n");
        if (phys.IsValid())
        ply.PrintMessage(HUD_PRINTCONSOLE, "VELOCITY ==> Vector(" + phys.GetVelocity().x + ", " + phys.GetVelocity().y + ", " + phys.GetVelocity().z + ") ;;; Length = " + phys.GetVelocity():Length() + "\n\n");
        ply.PrintMessage(HUD_PRINTCONSOLE, "PHYSICS  ==> Mass = " + phys.GetMass() + " ;;; Surface Area = " + phys.GetSurfaceArea() + " ;;; Volume = " + phys.GetVolume() + "\n\n");
        else;
        ply.PrintMessage(HUD_PRINTCONSOLE, "VELOCITY ==> Model doesn't have a physics object!\n\n");
        ply.PrintMessage(HUD_PRINTCONSOLE, "PHYSICS  ==> Model doesn't have a physics object!\n\n");

        ply.PrintMessage(HUD_PRINTCONSOLE, "COLOR    ==> Color(" + ent.GetColor().r + ", " + ent.GetColor().g + ", " + ent.GetColor().b + ", " + ent.GetColor().a + ")");
        ply.PrintMessage(HUD_PRINTCONSOLE, "-----------------------------------------------------------------------------------------------");
        return true;
    }

    public virtual void RightClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid()) return false 
        PrintTable(ent.GetSaveTable(true));
        return true;
    }

    public virtual void OnReload(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid()) return false 
        PrintTable(ent.GetTable());
        return true;
    }

    public virtual void BuildCPanel(panel)
    {
        panel.Help("tool.Count.vj_tool_scanner.menu.label");
    }

}