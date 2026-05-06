using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ NPC Controller — ported from obj_vj_controller/init.lua.
/// Player-controlled NPC management entity.
/// </summary>
public partial class VJController : Component
{
    public GameObject ControlledNPC { get; set; }
    public GameObject ControllingPlayer { get; set; }
    public int CameraMode { get; set; } = 1; // 1=Third Person, 2=First Person
    public Vector3 ThirdP_Offset { get; set; }
    public string FirstP_Bone { get; set; } = "ValveBiped.Bip01_Head1";
    public Vector3 FirstP_Offset { get; set; } = new(0, 0, 5);

    // Phase 3: camera control + input forwarding to NPC
}
