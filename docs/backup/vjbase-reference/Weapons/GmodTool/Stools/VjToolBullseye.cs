using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class VjToolBullseye : BaseTool
{
    [Property] public string Name = "#tool.vj_tool_bullseye.name";
    [Property] public string Tab = "DrVrej";
    [Property] public string Category = "Tools";
    [Property] public object Information = {;
    [Property] public object ClientConVar = {;

    public virtual void LeftClick(tr)
    {
        if (CLIENT) return true 
        var spawner = SceneUtility.CreatePrefab();
        spawner.SetPos(tr.HitPos);
        spawner.SetModel(GetClientInfo("model"));
        spawner.SolidMovementType = GetClientInfo("type");
        spawner.CanToggle = true;
        spawner.ToggleDisplayColors = GetClientBool("usecolor");
        spawner.Activated = GetClientBool("startactivate");
        spawner.Spawn();
        spawner.Activate();
        undo.Create("NPC Bullseye");
        undo.AddEntity(spawner);
        undo.SetPlayer(Owner);
        undo.Finish();
        return true;
    }

    public virtual void BuildCPanel(panel)
    {
        ControlPanel(panel);
    }

}