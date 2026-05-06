using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjController : WeaponVjBase
{
    [Property] public string PrintName = "NPC Controller";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Purpose = "Made to control VJ NPCs.";
    [Property] public string Instructions = "Press PRIMARY FIRE to control the NPC you are looking at.";
    [Property] public string Category = "VJ Base";
    [Property] public bool Spawnable = true;
    [Property] public string ViewModel = "models/vj_base/weapons/c_controller.mdl";
    [Property] public string WorldModel = "models/vj_base/gibs/human/brain.mdl";
    [Property] public bool WorldModel_UseCustomPosition = true;
    [Property] public Vector3 WorldModel_CustomPositionAngle = Vector(0, 0, 0);
    [Property] public Vector3 WorldModel_CustomPositionOrigin = Vector(0, 4, -1.1);
    [Property] public string HoldType = "pistol";
    [Property] public int Slot = 5;
    [Property] public int SlotPos = 7;
    [Property] public bool UseHands = true;
    [Property] public object DeploySound = {"physics/flesh/flesh_squishy_impact_hard1.wav", "physics/flesh/flesh_squishy_impact_hard2.wav", "physics/flesh/flesh_squishy_impact_hard3.wav", "physics/flesh/flesh_squishy_impact_hard4.wav"};

    public virtual void OnPrimaryAttack()
    {
        var owner = Owner;
        if (CLIENT || owner.IsNPC()) return 

        owner.SetAnimation(PLAYER_ATTACK1);
        var delayTime = Time.Now + AnimationHelper.Duration(owner.GetViewModel(), fireAnim);
        SendWeaponAnim(fireAnim);
        this.PLY_NextIdleAnimT = delayTime;
        this.PLY_NextReloadT = delayTime;
        NextPrimaryFireTime = delayTime;

        var fireSd = Game.Random.FromList(this.Primary.Sound);
        if (fireSd != false)
        sound.Play(fireSd, owner.GetPos(), this.Primary.SoundLevel, Game.Random.NextInt(this.Primary.SoundPitch.a, this.Primary.SoundPitch.b), this.Primary.SoundVolume);


        var tr = SceneTrace.Ray(util.GetPlayerTrace(owner)).Run();
        var trEnt = tr.Entity;
        if (trEnt.IsValid())
        if (trEnt.IsPlayer())
        owner.ChatPrint("That's a player dumbass.");
        return;
        else if (trEnt.GetClass() == "prop_ragdoll")
        owner.ChatPrint("You are about to become that corpse.");
        return;
        else if (trEnt.GetClass() == "prop_physics")
        owner.ChatPrint("Uninstall your game. Now.");
        return;
        else if (!trEnt.IsNPC())
        owner.ChatPrint("This isn't an NPC, therefore you can't control it.");
        return;
        else if (trEnt.VJ_IsBeingControlled)
        owner.ChatPrint("You can't control this NPC, it's already being controlled by someone else.");
        return;

        if (!trEnt.IsVJBaseSNPC)
        owner.ChatPrint("NOTE: NPC Controller is mainly made for VJ Base NPCs!");

        var ent_controller = SceneUtility.CreatePrefab();
        ent_controller.VJCE_Player = owner;
        ent_controller.SetControlledNPC(trEnt);
        ent_controller.Spawn();
        //ent_controller.Activate()
        ent_controller.StartControlling();

    }

    public virtual void OnSecondaryAttack()
    {
    }

}