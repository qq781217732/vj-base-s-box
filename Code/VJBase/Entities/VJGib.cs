using System;
using Sandbox;

namespace VJBase;

/// <summary>
/// VJ Gib entity — ported from obj_vj_gib/init.lua.
/// Bloody gib pieces spawned on NPC death.
/// </summary>
public partial class VJGib : Component
{
    public VJBloodColor BloodType { get; set; } = VJBloodColor.Red;
    public object CollisionDecal { get; set; }
    public object CollisionSound { get; set; }
    public float GibFadeTime { get; set; } = 10f;
    public bool IsVJBaseCorpse_Gib { get; set; } = true;

    // Phase 3: Rigidbody physics, blood decals, fade-out
}
