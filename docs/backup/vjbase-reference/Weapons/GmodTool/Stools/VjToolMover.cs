using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolMover : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_mover.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsNPC()) return 
        net.Start("vj_tool_mover_cl_select");
        net.WriteEntity(ent);
        net.WriteString(VJ.GetName(ent));
        net.Send(Owner);
        return true;
    }

    public virtual void RightClick(tr)
    {
        if (CLIENT) return true 
        net.Start("vj_tool_mover_cl_move");
        net.WriteBit(1);
        net.WriteVector(tr.HitPos);
        net.Send(Owner);
        return true;
    }

    public virtual void OnReload(tr)
    {
        if (CLIENT) return true 
        net.Start("vj_tool_mover_cl_move");
        net.WriteBit(0);
        net.WriteVector(tr.HitPos);
        net.Send(Owner);
        return true;
    }

    public virtual void BuildCPanel(panel)
    {
        ControlPanel(panel);
    }

}