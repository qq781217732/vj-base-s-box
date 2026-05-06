using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class NpcVjTestAerial : CreatureNPC
{
    [Property] public string Model = "models/mortarsynth.mdl";
    [Property] public int StartHealth = 150;
    [Property] public object HullType = HULL_TINY;
    [Property] public object MovementType = VJ_MOVETYPE_AERIAL;
    [Property] public string Aerial_AnimTbl_Calm = "mortar_back";
    [Property] public string Aerial_AnimTbl_Alerted = "mortar_forward";
    [Property] public object VJ_NPC_Class = {"CLASS_COMBINE"};
    [Property] public object BloodColor = VJ.BLOOD_COLOR_OIL;
    [Property] public bool HasDeathCorpse = false;
    [Property] public bool HasMeleeAttack = true;
    [Property] public object AnimTbl_MeleeAttack = ACT_RANGE_ATTACK1;
    [Property] public int MeleeAttackDistance = 60;
    [Property] public int MeleeAttackDamageDistance = 80;
    [Property] public float TimeUntilMeleeAttackDamage = 0.7;
    [Property] public bool NextAnyAttackTime_Melee = false;
    [Property] public int MeleeAttackDamage = 30;
    [Property] public bool HasExtraMeleeAttackSounds = true;
    [Property] public string SoundTbl_Breath = "npc/scanner/scanner_combat_loop1.wav";
    [Property] public object SoundTbl_Idle = {"npc/scanner/scanner_talk1.wav", "npc/scanner/scanner_talk2.wav"};
    [Property] public string SoundTbl_Alert = "npc/scanner/combat_scan5.wav";
    [Property] public string SoundTbl_MeleeAttack = "npc/scanner/scanner_electric1.wav";
    [Property] public object SoundTbl_MeleeAttackMiss = {"npc/zombie/claw_miss1.wav", "npc/zombie/claw_miss2.wav"};
    [Property] public object SoundTbl_Pain = {"npc/scanner/scanner_pain1.wav", "npc/scanner/scanner_pain2.wav"};
    [Property] public string SoundTbl_Death = "npc/waste_scanner/grenade_fire.wav";
    [Property] public string Type = "ai";
    [Property] public string PrintName = "Aerial NPC";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Aerial NPC demo for developers.\nBased on the Mortar Synth from Half-Life 2.";
    [Property] public string Category = "VJ Base";

    public virtual void OnInit()
    {
        Collider.SetBounds(Vector(33, 33, 26), Vector(-33, -33, -30));
        Transform.Position = Transform.Position + spawnPos;
    }

    public virtual void OnDeath(dmginfo, hitgroup, status)
    {
        if (status == "Init")
        var myPos = Transform.Position;
        Particles.Play("explosion_turret_break", myPos, defAng);
        BlastDamage(self, self, myPos, 80, 20);

    }

}