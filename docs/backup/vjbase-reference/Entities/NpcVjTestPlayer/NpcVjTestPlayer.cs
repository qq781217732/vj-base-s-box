using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class NpcVjTestPlayer : CreatureNPC
{
    [Property] public object Model = {"models/player/kleiner.mdl"};
    [Property] public int StartHealth = 100;
    [Property] public object BloodColor = VJ.BLOOD_COLOR_RED;
    [Property] public bool UsePoseParameterMovement = true;
    [Property] public object VJ_NPC_Class = {"CLASS_PLAYER_ALLY"};
    [Property] public bool AlliedWithPlayerAllies = true;
    [Property] public bool HasMeleeAttack = true;
    [Property] public string AnimTbl_MeleeAttack = "vjseq_seq_meleeattack01";
    [Property] public object WeaponInventory_AntiArmorList = {"weapon_vj_rpg"};
    [Property] public object WeaponInventory_MeleeList = {"weapon_vj_crowbar"};
    [Property] public bool HasGrenadeAttack = true;
    [Property] public float GrenadeAttackThrowTime = 0.85;
    [Property] public string GrenadeAttackModel = "models/weapons/w_npcnade.mdl";
    [Property] public string AnimTbl_GrenadeAttack = "vjges_gesture_item_throw";
    [Property] public string AnimTbl_Medic_GiveHealth = "vjges_gesture_item_drop";
    [Property] public object AnimTbl_CallForHelp = {"vjges_gesture_signal_group", "vjges_gesture_signal_forward"};
    [Property] public string AnimTbl_DamageAllyResponse = "vjges_gesture_signal_halt";
    [Property] public bool Weapon_OcclusionDelay = false;
    [Property] public float FootstepSoundTimerRun = 0.3;
    [Property] public float FootstepSoundTimerWalk = 0.5;
    [Property] public bool CanFlinch = true;
    [Property] public int FlinchCooldown = 1;
    [Property] public object AnimTbl_Flinch = {"vjges_flinch_01", "vjges_flinch_02"};
    [Property] public object FlinchHitGroupMap = {;
    [Property] public bool HasDeathAnimation = true;
    [Property] public object AnimTbl_Death = {"vjseq_death_02", "vjseq_death_03", "vjseq_death_04"};
    [Property] public int DeathAnimationChance = 2;
    [Property] public object SoundTbl_FootStep = {"npc/footsteps/hardboot_generic1.wav", "npc/footsteps/hardboot_generic2.wav", "npc/footsteps/hardboot_generic3.wav", "npc/footsteps/hardboot_generic4.wav", "npc/footsteps/hardboot_generic5.wav", "npc/footsteps/hardboot_generic6.wav", "npc/footsteps/hardboot_generic8.wav"};
    [Property] public object SoundTbl_IdleDialogue = {"common/wpn_denyselect.wav", "common/wpn_select.wav"};
    [Property] public object SoundTbl_IdleDialogueAnswer = {"common/wpn_denyselect.wav", "common/wpn_select.wav"};
    [Property] public string SoundTbl_FollowPlayer = "common/wpn_select.wav";
    [Property] public string SoundTbl_UnFollowPlayer = "common/wpn_denyselect.wav";
    [Property] public object SoundTbl_Death = {"player/pl_pain5.wav", "player/pl_pain6.wav", "player/pl_pain7.wav"};
    [Property] public string Type = "ai";
    [Property] public string PrintName = "Player NPC";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Information = "Playermodel human NPC demo for developers.\nPicks a random model from the installed playermodels list.";
    [Property] public string Category = "VJ Base";

    public virtual void PreInit()
    {
        // Set all the player models into the model variable
        // WARNING: Do NOT use "ipairs", this is NOT a sequential table!
        for _, v in pairs(player_manager.AllValidModels()) do
        this.Model[this.Count.Model + 1] = v;

    }

    public virtual void OnInit()
    {
        SoundManager.Emit(self, "player/pl_drown1.wav")  // Player connect sound;

        // Random bodygroups and skins
        for i = 1, GetNumBodyGroups() -1 do
        SetBodygroup(i, Game.Random.NextInt(0, this.GetBodygroupCount(i - 1)));

        SkinnedModelRenderer.Skin = Game.Random.NextInt(0, SkinCount( - 1));

        // Random playermodel color
        SetPlayerColor(Color(Game.Random.NextFloat(0, 255), Game.Random.NextFloat(0, 255), Game.Random.NextFloat(0, 255)):ToVector());
    }

    public virtual void OnGrenadeAttackExecute(status, grenade, overrideEnt, landDir, landingPos)
    {
        if (status == "PostSpawn" && !overrideEnt.IsValid())
        // Glow and trail are both based on the original: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/hl2/grenade_frag.cpp#L158
        var redGlow = SceneUtility.CreatePrefab();
        redGlow.SetKeyValue("model", "sprites/redglow1.vmt");
        redGlow.SetKeyValue("scale", "0.2");
        redGlow.SetKeyValue("rendermode", "3")  // kRenderGlow;
        redGlow.SetKeyValue("renderfx", "14")  // kRenderFxNoDissipation;
        redGlow.SetKeyValue("renderamt", "200");
        redGlow.SetKeyValue("rendercolor", "255 255 255");
        redGlow.SetKeyValue("GlowProxySize", "4.0");
        redGlow.SetParent(grenade);
        redGlow.Fire("SetParentAttachment", "fuse");
        redGlow.Spawn();
        redGlow.Activate();
        grenade.DeleteOnRemove(redGlow);
        var redTrail = util.SpriteTrail(grenade, 1, VJ.COLOR_RED, true, 8, 1, 0.5, 0.0555, "sprites/bluelaser1.vmt");
        redTrail.SetKeyValue("rendermode", "5")  // kRenderTransAdd;
        redTrail.SetKeyValue("renderfx", "0")  // kRenderFxNone;
        grenade.SoundTbl_Idle = "Grenade.Blip";
        grenade.IdleSoundPitch = new Vector2(100, 100);
        grenade.NextBeepSoundT = 0;
        function grenade.OnThink();
        if (Time.Now > this.NextBeepSoundT)
        this.CurrentIdleSound?.Stop();
        PlaySound("Idle");
        this.NextBeepSoundT = Time.Now + 0.5;



    }

    public virtual void OnDeath(dmginfo, hitgroup, status)
    {
        if (status == "Init")
        var pos = Transform.Position;
        var pitch = Game.Random.NextInt(95, 105);
        var function deathSound(time, snd);
        GameTask.DelaySeconds(time).ContinueWith(_ => function();
        sound.Play(snd, pos, 65, pitch);
        end);

        deathSound(0, "hl1/fvox/beep.wav");
        deathSound(0.25, "hl1/fvox/beep.wav");
        deathSound(0.75, "hl1/fvox/beep.wav");
        deathSound(1.25, "hl1/fvox/beep.wav");
        deathSound(1.7, "hl1/fvox/flatline.wav");

    }

    public virtual void SetupDataTables()
    {
        [Net] "Vector" 0, "PlayerColor"
    }

}