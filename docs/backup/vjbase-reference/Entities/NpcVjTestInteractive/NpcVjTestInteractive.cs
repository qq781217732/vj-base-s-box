using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class NpcVjTestInteractive : CreatureNPC
{
    [Property] public bool Model = false;
    [Property] public int StartHealth = 50;
    [Property] public object BloodColor = VJ.BLOOD_COLOR_RED;
    [Property] public object Behavior = VJ_BEHAVIOR_PASSIVE;
    [Property] public object VJ_NPC_Class = {"CLASS_PLAYER_ALLY"};
    [Property] public bool AlliedWithPlayerAllies = true;
    [Property] public bool YieldToAlliedPlayers = false;
    [Property] public bool FollowPlayer = false;
    [Property] public bool HasFootstepSounds = false;
    [Property] public object SoundTbl_Pain = {"vo/npc/male01/pain01.wav", "vo/npc/male01/pain02.wav", "vo/npc/male01/pain03.wav", "vo/npc/male01/pain04.wav", "vo/npc/male01/pain05.wav", "vo/npc/male01/pain06.wav", "vo/npc/male01/pain07.wav", "vo/npc/male01/pain08.wav", "vo/npc/male01/pain09.wav"};
    [Property] public string Type = "ai";
    [Property] public string PrintName = "Interactive NPC";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Interactive human NPC demo for developers.\nPress USE on it to open the menu!";
    [Property] public string Category = "VJ Base";
    [Property] public bool VJ_ID_Civilian = true;

    public virtual void PreInit()
    {
        this.Model = "models/humans/group01/male_0" + Game.Random.NextInt(1, 9) + ".mdl";
    }

    public virtual void OnInput(key, activator, caller, data)
    {
        if (key == "Use" && activator.IsValid() && activator.IsPlayer() && activator.Alive())
        net.Start("vj_npc_test_interactive_cl");
        net.WriteEntity(self);
        net.Send(activator);
        activator.EmitSound("vj_base/player/illuminati.mp3", 75);
        PlaySoundSystem("Speech", "vo/npc/male01/hi0" + Game.Random.NextInt(1, 2) + ".wav");
        StopMoving();
        Target = activator;
        SetTurnTarget(activator, -1, true);

    }

    public virtual void MatFootStepQCEvent(data)
    {
        return;
    }

}