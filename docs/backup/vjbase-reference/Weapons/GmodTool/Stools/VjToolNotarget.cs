using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolNotarget : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_notarget.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        var owner = Owner;
        if (owner.IsFlagSet(FL_NOTARGET) != true)
        owner.ChatPrint("tool.Count.vj_tool_notarget.print.yourselfon");
        owner.AddFlags(FL_NOTARGET);
        return true;
        else;
        owner.ChatPrint("tool.Count.vj_tool_notarget.print.yourselfoff");
        owner.RemoveFlags(FL_NOTARGET);
        return true;

    }

    public virtual void RightClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid()) return false 
        if (ent.IsFlagSet(FL_NOTARGET) != true)
        Owner.ChatPrint("Set no target to " + VJ.GetName(ent) + ": ON");
        ent.AddFlags(FL_NOTARGET);
        return true;
        else;
        Owner.ChatPrint("Set no target to " + VJ.GetName(ent) + ": OFF");
        ent.RemoveFlags(FL_NOTARGET);
        return true;

    }

    public virtual void BuildCPanel(panel)
    {
        panel.Help("tool.Count.vj_tool_notarget.menu.label");
    }

}