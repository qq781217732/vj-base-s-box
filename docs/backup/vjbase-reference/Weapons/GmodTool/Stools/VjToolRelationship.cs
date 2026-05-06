using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolRelationship : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_relationship.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;
    [Property] public object ClientConVar = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (ent.IsValid() && ent.VJ_ID_Living)
        net.Start("vj_tool_relationship_cl_apply");
        net.WriteEntity(ent);
        net.WriteString(VJ.GetName(ent));
        net.WriteBit(0);
        net.Send(Owner);
        return true;

    }

    public virtual void RightClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (ent.IsValid() && ent.VJ_ID_Living)
        var owner = Owner;
        var entName = VJ.GetName(ent);
        var entClasses = ent.VJ_NPC_Class;
        if (!entClasses)
        owner.ChatPrint("ERROR! Failed to get " + entName + "'s class list!");
        return false;
        else if (entClasses.Count <= 0)
        owner.ChatPrint("ERROR! " + entName + " has no classes assigned!");
        return false;

        net.Start("vj_tool_relationship_cl_select");
        net.WriteString(entName);
        net.WriteUInt(entClasses.Count, 9);
        for _, v in ent.VJ_NPC_Class do
        net.WriteString(v);

        net.Send(owner);
        return true;

    }

    public virtual void OnReload(tr)
    {
        if (CLIENT) return true 
        net.Start("vj_tool_relationship_cl_apply");
        net.WriteEntity(Owner);
        net.WriteString("Me");
        net.WriteBit(1);
        net.Send(Owner);
        return true;
    }

    public virtual void BuildCPanel(panel)
    {
        ControlPanel(panel);
    }

}