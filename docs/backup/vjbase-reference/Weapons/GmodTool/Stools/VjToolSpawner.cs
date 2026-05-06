using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolSpawner : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_spawner.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;
    [Property] public object ClientConVar = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        net.Start("vj_tool_spawner_cl_create");
        net.WriteVector(tr.HitPos);
        net.WriteBit(0);
        net.Send(Owner);
        return true;
    }

    public virtual void RightClick(tr)
    {
        if (CLIENT) return true 
        net.Start("vj_tool_spawner_cl_create");
        net.WriteVector(tr.HitPos);
        net.WriteBit(1);
        net.Send(Owner);
        return true;
    }

    public virtual void BuildCPanel(panel)
    {
        ControlPanel(panel);
    }

}