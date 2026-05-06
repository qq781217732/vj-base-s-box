using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolFollower : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_follower.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid() || !ent.IsNPC() || !ent.IsVJBaseSNPC) return 
        var ply = Owner;
        var selectedNPC = GetEnt(1);

        // Unselect the NPC
        if (selectedNPC.IsValid() && selectedNPC == ent)
        ClearObjects();
        ply.ChatPrint(VJ.GetName(ent) + " Has been unselected!");
        // Select the NPC
        else;
        ClearObjects();
        SetObject(1, ent, tr.HitPos, null, tr.PhysicsBone, tr.HitNormal);
        ply.ChatPrint(VJ.GetName(ent) + " Has been selected!");

        return true;
    }

    public virtual void RightClick(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid()) return 
        var ply = Owner;
        var selectedNPC = GetEnt(1);

        if (selectedNPC.IsValid())
        var followed, failureReason = selectedNPC.Follow(ent, false);

        // SUCCESS
        if (followed)
        ClearObjects();
        ply.ChatPrint(VJ.GetName(selectedNPC) + " is now following " + VJ.GetName(ent));
        // FAILURES
        else if (failureReason == 1)
        ply.ChatPrint("ERROR: " + VJ.GetName(selectedNPC) + " NPC is stationary && currently unable to follow!");
        else if (failureReason == 2)
        ply.ChatPrint("ERROR: " + VJ.GetName(selectedNPC) + " is already following another entity!");
        else if (failureReason == 3)
        ply.ChatPrint("ERROR: " + VJ.GetName(selectedNPC) + " is NOT friendly to the other entity!");
        else;
        ply.ChatPrint("ERROR: " + VJ.GetName(selectedNPC) + " is currently unable to follow!");

        else;
        ply.ChatPrint("tool.Count.vj_tool_follower.print.noselection");

        return true;
    }

    public virtual void OnReload(tr)
    {
        if (CLIENT) return true 
        var ent = tr.Entity;
        if (!ent.IsValid() || !ent.IsNPC() || !ent.IsVJBaseSNPC) return 
        ent.ResetFollowBehavior();
        Owner.ChatPrint("tool.Count.vj_tool_follower.print.reset");
        return true;
    }

    public virtual void BuildCPanel(panel)
    {
        panel.Help("vjbase.Count.tool.general.note.recommend");
    }

}