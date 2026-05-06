using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class Core : BaseNPC
{
    [Property] public bool VJ_ID_Healable = true;
    [Property] public bool VJ_DEBUG = false;
    [Property] public bool VJ_IsBeingControlled = false;
    [Property] public bool VJ_IsBeingControlled_Tool = false;
    [Property] public object VJ_TheController = NULL;
    [Property] public object VJ_TheControllerEntity = NULL;
    [Property] public object VJ_TheControllerBullseye = NULL;
    [Property] public int SelectedDifficulty = 1;
    [Property] public object AIState = VJ_STATE_NONE;
    [Property] public int NextProcessT = 0;
    [Property] public object MedicData = {;
    [Property] public bool IsFollowing = false;
    [Property] public object FollowData = {;
    [Property] public object EnemyData = {;
    [Property] public object TurnData = {;
    [Property] public object GuardData = {;
    [Property] public bool PauseAttacks = false;
    [Property] public int AnimLockTime = 0;
    [Property] public int AnimPlaybackRate = 1;
    [Property] public object AnimModelSet = VJ.ANIM_SET_NONE;
    [Property] public int LastAnimSeed = 0;
    [Property] public object LastAnimType = VJ.ANIM_TYPE_NONE;
    [Property] public int AttackSeed = 0;
    [Property] public object AttackType = VJ.ATTACK_TYPE_NONE;
    [Property] public object AttackState = VJ.ATTACK_STATE_NONE;
    [Property] public object AttackAnim = ACT_INVALID;
    [Property] public int AttackAnimDuration = 0;
    [Property] public int AttackAnimTime = 0;
    [Property] public int NextDoAnyAttackT = 0;
    [Property] public bool IsAbleToMeleeAttack = true;
    [Property] public bool MeleeAttack_IsPropAttack = false;
    [Property] public int NextIdleTime = 0;
    [Property] public int NextWanderTime = 0;
    [Property] public int NextChaseTime = 0;
    [Property] public bool Alerted = false;
    [Property] public bool Flinching = false;
    [Property] public int NextFlinchT = 0;
    [Property] public int HealthRegenDelayT = 0;
    [Property] public int NextCombineBallDmgT = 0;
    [Property] public bool Dead = false;
    [Property] public bool GibbedOnDeath = false;
    [Property] public bool DeathAnimationCodeRan = false;
    [Property] public int TakingCoverT = 0;
    [Property] public int NextOnPlayerSightT = 0;
    [Property] public bool LastHiddenZone_CanWander = true;
    [Property] public int LastHiddenZoneT = 0;
    [Property] public int NextInvestigationMove = 0;
    [Property] public int NextInvestigateSoundT = 0;
    [Property] public int NextFootstepSoundT = 0;
    [Property] public int NextBreathSoundT = 0;
    [Property] public int NextIdleSoundT = 0;
    [Property] public int IdleSoundBlockTime = 0;
    [Property] public int NextAlertSoundT = 0;
    [Property] public int NextCallForHelpT = 0;
    [Property] public int NextCallForHelpAnimationT = 0;
    [Property] public int NextLostEnemySoundT = 0;
    [Property] public int NextAllyDeathSoundT = 0;
    [Property] public int NextKilledEnemySoundT = 0;
    [Property] public int NextDamageAllyResponseT = 0;
    [Property] public int NextDamageByPlayerSoundT = 0;
    [Property] public int NextPainSoundT = 0;
    [Property] public int MainSoundPitchValue = 0;
    [Property] public object TimersToRemove = {;
    [Property] public int LatestEnemyDistance = 0;
    [Property] public int NearestPointToEnemyDistance = 0;
    [Property] public object FootStepPitch = VJ.SET(80, 100);

    public virtual void CreateExtraDeathCorpse(class, models, extraOptions, customFunc)
    {
        // Should only be ran after self.Corpse has been created!
        var corpse = this.Corpse;
        if (!corpse.IsValid()) return 
        var dmginfo = corpse.DamageInfo;
        if (dmginfo == null) return 
        extraOptions = extraOptions || {}
        var ent = SceneUtility.CreatePrefab();
        if (models != "None") ent.SetModel(PICK(models)) 
        ent.SetPos(extraOptions.Pos || Transform.Position);
        ent.SetAngles(extraOptions.Ang || Transform.Rotation);
        ent.Spawn();
        ent.Activate();
        ent.SetColor(corpse.GetColor());
        ent.SetMaterial(corpse.GetMaterial());
        ent.SetCollisionGroup(this.DeathCorpseCollisionType);
        if (corpse.IsOnFire())
        ent.Ignite(Game.Random.NextFloat(8, 10), 0);
        ent.SetColor(colorGrey);

        if (extraOptions.HasVel != false)
        var dmgForce = (this.SavedDmgInfo.force / 40) + GetMoveVelocity() + Rigidbody.Velocity;
        if (this.DeathAnimationCodeRan)
        dmgForce = GetGroundSpeedVelocity();

        ent.GetPhysicsObject():AddVelocity(extraOptions.Vel || dmgForce);

        if (extraOptions.ShouldFade == true)
        var fadeTime = extraOptions.ShouldFadeTime || 0;
        if (funcGetClass(ent) == "prop_ragdoll")
        ent.Fire("FadeAndRemove", null, fadeTime);
        else;
        ent.Fire("kill", null, fadeTime);


        if (extraOptions.RemoveOnCorpseDelete != false) //corpse.DeleteOnRemove(ent)
        corpse.ChildEnts[corpse.Count.ChildEnts + 1] = ent;

        if ((customFunc)) customFunc(ent) 
        return ent;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Creates a gib entity, use this function to create gib!;
        - class = The object class to use, recommended to use "obj_vj_gib", && for ragdoll type of gib use "prop_ragdoll";
        - models = Model(s) to use, can be a table which it will pick randomly from it OR a string;
        - Defined strings: "UseAlien_Small", "UseAlien_Big", "UseHuman_Small", "UseHuman_Big";
        - extraOptions = Table that holds extra options to modify parts of the code;
        - Pos = Sets the spawn position;
        - Ang = Sets the spawn angle | DEFAULT = Random angle;
        - Vel = Sets the velocity | "UseDamageForce" = To use the damage's force only | DEFAULT = Random velocity;
        - Vel_ApplyDmgForce = If set to false, it won't add the damage force to the given velocity | DEFAULT = true;
        - AngVel = Angle velocity, basically the speed it rotates as it's flying | DEFAULT = Random velocity;
        - BloodType = Sets the blood type of the gib | Overrides "CollisionDecal" option | Works only with "obj_vj_gib";
        - CollisionDecal = Decal it spawns when it collides with something | false = Disable decals | DEFAULT = Base decides;
        - CollisionSound = Sound(s) it plays when it collides with something | false = Disable collision sounds | DEFAULT = Base decides;
        - NoFade = Should it let the base make it fade & remove (Adjusted in the NPC settings menu) | DEFAULT = false;
        - RemoveOnCorpseDelete = Should the entity get removed if the corpse is removed? | DEFAULT = false;
        - customFunc(gib) = Use this to edit the entity which is given as parameter "gib";
        //---------------------------------------------------------]]
        var gib_mdlAAll = {"models/vj_base/gibs/alien/gib_small1.mdl", "models/vj_base/gibs/alien/gib_small2.mdl", "models/vj_base/gibs/alien/gib_small3.mdl", "models/vj_base/gibs/alien/gib1.mdl", "models/vj_base/gibs/alien/gib2.mdl", "models/vj_base/gibs/alien/gib3.mdl", "models/vj_base/gibs/alien/gib4.mdl", "models/vj_base/gibs/alien/gib5.mdl", "models/vj_base/gibs/alien/gib6.mdl", "models/vj_base/gibs/alien/gib7.mdl"}
        var gib_mdlASmall = {"models/vj_base/gibs/alien/gib_small1.mdl", "models/vj_base/gibs/alien/gib_small2.mdl", "models/vj_base/gibs/alien/gib_small3.mdl"}
        var gib_mdlABig = {"models/vj_base/gibs/alien/gib1.mdl", "models/vj_base/gibs/alien/gib2.mdl", "models/vj_base/gibs/alien/gib3.mdl", "models/vj_base/gibs/alien/gib4.mdl", "models/vj_base/gibs/alien/gib5.mdl", "models/vj_base/gibs/alien/gib6.mdl", "models/vj_base/gibs/alien/gib7.mdl"}
        var gib_mdlHSmall = {"models/vj_base/gibs/human/gib_small1.mdl", "models/vj_base/gibs/human/gib_small2.mdl", "models/vj_base/gibs/human/gib_small3.mdl"}
        var gib_mdlHBig = {"models/vj_base/gibs/human/gib1.mdl", "models/vj_base/gibs/human/gib2.mdl", "models/vj_base/gibs/human/gib3.mdl", "models/vj_base/gibs/human/gib4.mdl", "models/vj_base/gibs/human/gib5.mdl", "models/vj_base/gibs/human/gib6.mdl", "models/vj_base/gibs/human/gib7.mdl"}
        //
        function ENT.CreateGibEntity(class, models, extraOptions, customFunc);
        if (!this.CanGib) return 
        var bloodType = false;
        if (models == "UseAlien_Small")
        models =  PICK(gib_mdlASmall);
        bloodType = VJ.BLOOD_COLOR_YELLOW;
        else if (models == "UseAlien_Big")
        models =  PICK(gib_mdlABig);
        bloodType = VJ.BLOOD_COLOR_YELLOW;
        else if (models == "UseHuman_Small")
        models =  PICK(gib_mdlHSmall);
        bloodType = VJ.BLOOD_COLOR_RED;
        else if (models == "UseHuman_Big")
        models =  PICK(gib_mdlHBig);
        bloodType = VJ.BLOOD_COLOR_RED;
        else  // Custom models
        models = PICK(models);
        if (VJ.HasValue(gib_mdlAAll, models))
        bloodType = VJ.BLOOD_COLOR_YELLOW;


        extraOptions = extraOptions || {}
        var vel = extraOptions.Vel || Vector(Game.Random.NextFloat(-100, 100), Game.Random.NextFloat(-100, 100), Game.Random.NextFloat(150, 250));
        if (this.SavedDmgInfo)
        var dmgForce = this.SavedDmgInfo.force / 70;
        if (extraOptions.Vel_ApplyDmgForce != false && extraOptions.Vel != "UseDamageForce")  // Use both damage force AND given velocity
        vel = vel + dmgForce;
        else if (extraOptions.Vel == "UseDamageForce")  // Use damage force
        vel = dmgForce;


        bloodType = (extraOptions.BloodType || bloodType || this.BloodColor)  // Certain entities such as the VJ Gib entity, you can use this to set its gib type;

        var gib = SceneUtility.CreatePrefab();
        gib.SetModel(models);
        gib.SetPos(extraOptions.Pos || (Transform.Position + OBBCenter()));
        gib.SetAngles(extraOptions.Ang || Angle(Game.Random.NextFloat(-180, 180), Game.Random.NextFloat(-180, 180), Game.Random.NextFloat(-180, 180)));
        if (funcGetClass(gib) == "obj_vj_gib")
        gib.BloodType = bloodType;
        if (extraOptions.CollisionDecal != null)
        gib.CollisionDecal = extraOptions.CollisionDecal;
        else if (extraOptions.BloodDecal)  // Backwards compatibility
        gib.CollisionDecal = extraOptions.BloodDecal;

        if (extraOptions.CollisionSound != null)
        gib.CollisionSound = extraOptions.CollisionSound;
        else if (extraOptions.CollideSound)  // Backwards compatibility
        gib.CollisionSound = extraOptions.CollideSound;

        //gib.BloodData = {Color = bloodType, Particle = this.BloodParticle, Decal = this.CollisionDecal}  // For eating system

        gib.Spawn();
        gib.Activate();
        gib.IsVJBaseCorpse_Gib = true;
        if (vj_npc_gib_collision.GetInt() == 0) gib.SetCollisionGroup(COLLISION_GROUP_DEBRIS) 
        var phys = gib.GetPhysicsObject();
        if (phys.IsValid())
        phys.AddVelocity(vel);
        phys.AddAngleVelocity(extraOptions.AngVel || Vector(Game.Random.NextFloat(-200, 200), Game.Random.NextFloat(-200, 200), Game.Random.NextFloat(-200, 200)));

        if (extraOptions.NoFade != true && vj_npc_gib_fade.GetInt() == 1)
        var gibClass = funcGetClass(gib);
        if (gibClass == "obj_vj_gib")
        GameTask.DelaySeconds(vj_npc_gib_fadetime.GetInt()).ContinueWith(_ => function() if (gib.IsValid()) gib.Remove() end end);
        else if (gibClass == "prop_ragdoll")
        gib.Fire("FadeAndRemove", null, vj_npc_gib_fadetime.GetInt());
        else if (gibClass == "prop_physics")
        gib.Fire("kill", null, vj_npc_gib_fadetime.GetInt());


        if (removeOnCorpseDelete) //this.Corpse.DeleteOnRemove(extraent)
        if (!this.DeathCorpse_ChildEnts) this.DeathCorpse_ChildEnts = {} end  // If it doesn't exist, then create it!
        this.DeathCorpse_ChildEnts[this.Count.DeathCorpse_ChildEnts + 1] = gib;

        if ((customFunc)) customFunc(gib) 
        return gib;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[
        More info about sound hints: https://github.com/DrVrej/VJ-Base/wiki/Developer-Notessound.Count-hints;
        // Condition --					-- Sound bit --								-- Suggested Use --
        COND_HEAR_DANGER				SOUND_DANGER								Danger;
        COND_HEAR_PHYSICS_DANGER		SOUND_PHYSICS_DANGER						Danger;
        COND_HEAR_MOVE_AWAY				SOUND_MOVE_AWAY								Danger;
        COND_HEAR_COMBAT				SOUND_COMBAT								Interest;
        COND_HEAR_WORLD					SOUND_WORLD									Interest;
        COND_HEAR_BULLET_IMPACT			SOUND_BULLET_IMPACT							Interest;
        COND_HEAR_PLAYER				SOUND_PLAYER								Interest;
        COND_SMELL						SOUND_CARCASS/SOUND_MEAT/SOUND_GARBAGE		Smell;
        COND_HEAR_THUMPER				SOUND_THUMPER								Special case;
        COND_HEAR_BUGBAIT				SOUND_BUGBAIT								Special case;
        COND_NO_HEAR_DANGER				none										No danger detected;
        COND_HEAR_SPOOKY 				none										Not possible in GMod due to the missing SOUNDENT_CHANNEL_SPOOKY_NOISE;
        //]]
        var sdInterests = bit.bor(SOUND_COMBAT, SOUND_DANGER, SOUND_BULLET_IMPACT, SOUND_PHYSICS_DANGER, SOUND_MOVE_AWAY, SOUND_PLAYER_VEHICLE, SOUND_PLAYER, SOUND_WORLD, SOUND_CARCASS, SOUND_MEAT, SOUND_GARBAGE);
        //
        function ENT.GetSoundInterests();
        return sdInterests;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Reset && stop the eating behavior;
        - statusData = Status info to pass to "OnEat" (info types defined in that function);
        //---------------------------------------------------------]]
        function ENT.ResetEatingBehavior(statusData);
        var eatingData = this.EatingData;
        SetState(VJ_STATE_NONE);
        OnEat("StopEating", statusData);
        this.VJ_ST_Eating = false;
        this.AnimationTranslations[ACT_IDLE] = eatingData.OrgIdle  // Reset the idle animation table in case it changed!;
        var food = eatingData.Target;
        if (food.IsValid())
        var foodData = food.FoodData;
        // if we are the last person eating, then reset the food data!
        if (foodData.NumConsumers <= 1)
        food.VJ_ST_BeingEaten = false;
        foodData.NumConsumers = 0;
        foodData.SizeRemaining = foodData.Size;
        else;
        foodData.NumConsumers = foodData.NumConsumers - 1;
        foodData.SizeRemaining = foodData.SizeRemaining + OBBMaxs():Distance(OBBMins());


        this.EatingData = {Target = NULL, NextCheck = eatingData.NextCheck, AnimStatus = "None", OrgIdle = null}
        // AnimStatus: "None" = Not prepared (Probably moving to food location) | "Prepared" = Prepared (Ex: Played crouch down anim) | "Eating" = Prepared and is actively eating

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Called every time a change occurs in the eating system;
        - status = Type of update that is occurring, holds one of the following states:
        - "CheckFood"		= Possible food found, check if it's good;
        - "StartBehavior"	= Food found, start the eating behavior;
        - "BeginEating"		= Food location reached;
        - "Eat"				= Actively eating food;
        - "StopEating"		= Food may have moved, removed, || finished;
        - statusData = Some status may have extra data:
        - "CheckFood": SoundHintData table, more info: https://wiki.facepunch.com/gmod/Structures/SoundHintData;
        - "StopEating": String, holding one of the following states:
        - "HaltOnly"	= This is ONLY a halt, !complete reset!		| Recommendation: Play normal get up anim;
        - "Unspecified"	= Ex: Food suddenly removed || moved far away	| Recommendation: Play normal get up anim;
        - "Devoured"	= Has completely devoured the food!				| Recommendation: Play normal get up anim && play a sound;
        - "Enemy"		= Has been alerted || detected an enemy			| Recommendation: Play scared get up anim;
        - "Injured"		= Has been injured by something					| Recommendation: Play scared get up anim;
        - "Dead"		= Has died, usually called in "OnRemove"		| Recommendation: Do NOT play any animation!;
        Returns;
        - Boolean, ONLY used for "CheckFood", returning true will tell the base the possible food is valid;
        - Number, Delay to add before moving to another status, useful to make sure animations aren't cut off!;
        //---------------------------------------------------------]]
        var vecZ50 = Vector(0, 0, -50);
        //
        function ENT.OnEat(status, statusData);
        // NOTE: The following code is a ideal example based on Half-Life 1 Zombie
        //// VJ.DEBUG: self, "OnEat", status, statusData
        if (status == "CheckFood")
        return true //statusData.owner.BloodData && statusData.owner.BloodData.Color == VJ.BLOOD_COLOR_RED;
        else if (status == "BeginEating")
        this.AnimationTranslations[ACT_IDLE] = ACT_GESTURE_RANGE_ATTACK1  // Eating animation;
        return select(2, PlayAnim(ACT_ARM, true, false));
        else if (status == "Eat")
        SoundManager.Emit(self, "barnacle/bcl_chew" + Game.Random.NextInt(1, 3) + ".wav", 55);
        // Health changes
        var food = this.EatingData.Target;
        var damage = 15  // How much damage food will receive;
        var foodHP = food.Health()  // Food's health;
        var myHP = Health()  // NPC's current health;
        Health = Math.Clamp(myHP + ((damage > foodHP && foodHP || damage), myHP, MaxHealth < myHP && myHP || MaxHealth))  // Give health to the NPC;
        food.SetHealth(foodHP - damage)  // Decrease corpse health;
        // Blood effects
        var bloodData = food.BloodData;
        if (bloodData)
        var bloodPos = food.GetPos() + food.OBBCenter();
        var bloodParticle = PICK(bloodData.Particle);
        if (bloodParticle)
        Particles.Play(bloodParticle, bloodPos, Transform.Rotation);

        var bloodDecal = PICK(bloodData.Decal);
        if (bloodDecal)
        var tr = SceneTrace.Ray({start = bloodPos, endpos = bloodPos + vecZ50, filter = {food, self}}).Run();
        Decals.Place(bloodDecal, tr.HitPos + tr.HitNormal + Vector(Game.Random.NextInt(-45, 45), Game.Random.NextInt(-45, 45), 0), tr.HitPos - tr.HitNormal, food);


        return 2  // Eat every this seconds;
        else if (status == "StopEating")
        if (statusData != "Dead" && this.EatingData.AnimStatus != "None")  // Do NOT play anim while dead || has NOT prepared to eat
        return select(2, PlayAnim(ACT_DISARM, true, false));


        return 0;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.UpdateAnimationTranslations(wepHoldType);
        // Decide what type of animation set to use
        if (!this.AnimModelSet)
        if (AnimationHelper.Exists(self, "signal_takecover") && AnimationHelper.Exists(self, "grenthrow") && AnimationHelper.Exists(self, "bugbait_hit"))
        this.AnimModelSet = VJ.ANIM_SET_COMBINE  // Combine;
        else if (AnimationHelper.Exists(self, ACT_WALK_AIM_PISTOL) && AnimationHelper.Exists(self, ACT_RUN_AIM_PISTOL) && AnimationHelper.Exists(self, ACT_POLICE_HARASS1))
        this.AnimModelSet = VJ.ANIM_SET_METROCOP  // Metrocop;
        else if (AnimationHelper.Exists(self, "coverlow_r") && AnimationHelper.Exists(self, "wave_smg1") && AnimationHelper.Exists(self, ACT_BUSY_SIT_GROUND))
        this.AnimModelSet = VJ.ANIM_SET_REBEL  // Rebel;
        else if (AnimationHelper.Exists(self, "gmod_breath_layer"))
        this.AnimModelSet = VJ.ANIM_SET_PLAYER  // Player;


        this.AnimationTranslations = {}  // Reset all translated animations;
        SetAnimationTranslations(wepHoldType);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Helper function used in `TranslateActivity` when randomly picking from a table;
        NOTE: ALWAYS use this when overriding ACT_IDLE from a table!;
        - tbl = Table to retrieve an animation from;
        Returns;
        - Activity it picked;
        //---------------------------------------------------------]]
        function ENT.ResolveAnimation(tbl);
        // Returns the current animation if it's found in the table and is not done playing it
        if (funcGetCycle(self) < 0.99)
        var curAnim = funcGetSequenceActivity(self, funcGetIdealSequence(self));
        for _, anim in tbl do
        if (curAnim == anim)
        return anim;



        return tbl[Game.Random.NextInt(1, tbl.Count)];

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Maintains && applies the idle animation;
        - force = Forcibly apply the idle animation without checking if it's already playing ACT_IDLE;
        //---------------------------------------------------------]]
        function ENT.MaintainIdleAnimation(force);
        // Animation cycle needs to be set to 0 to make sure engine does NOT attempt to switch sequence multiple times in this code: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/ai_basenpc.cpp#L2987
        // "self:IsSequenceFinished()" should NOT be used as it's broken, it returns "true" even though the animation hasn't finished, especially for non-looped animations
        //bit.band(GetSequenceInfo(SkinnedModelRenderer.CurrentSequence).flags, 1) == 0  // Checks if animation is none-looping
        //print(GetIdealActivity(), GetActivity(), GetSequenceName(GetIdealSequence()), GetSequenceName(SkinnedModelRenderer.CurrentSequence), IsSequenceFinished(), GetInternalVariable("m_bSequenceLoops"), SkinnedModelRenderer.GetFloat("cycle"))
        if (force)
        //// VJ.DEBUG: self, "MaintainIdleAnimation", "force"
        this.LastAnimSeed = 0;
        SetIdealActivity(ACT_IDLE) // ResetIdealActivity;
        // Need this check otherwise it may quickly repeat the last animation that was NOT an ACT_IDLE !
        if (funcGetIdealActivity(self) == ACT_IDLE && funcGetActivity(self) == ACT_IDLE)
        SkinnedModelRenderer.Set(\"cycle\", 0)  // This is to make sure this destructive code doesn't override it: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/ai_basenpc.cppL2987.Count;
        funcSetSaveValue(self, "m_bSequenceLoops", false)  // Otherwise it will stutter && play an idle sequence at 999x playback speed for 0.001 second when changing from one idle to another!;

        else if (funcGetIdealActivity(self) == ACT_IDLE && funcGetActivity(self) == ACT_IDLE)  // Check both ideal && current to make sure we are 100% playing an idle, otherwise transitions, certain movements, && animations will break!
        // If animation has finished OR idle animation has changed then play a new idle!
        if ((funcGetCycle(self) >= 0.98) || (TranslateActivity(ACT_IDLE) != funcGetSequenceActivity(self, funcGetIdealSequence(self))))
        //// VJ.DEBUG: self, "MaintainIdleAnimation", "auto"
        this.LastAnimSeed = 0;
        SetIdealActivity(ACT_IDLE) // ResetIdealActivity;
        SkinnedModelRenderer.Set(\"cycle\", 0)  // This is to make sure this destructive code doesn't override it: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/ai_basenpc.cppL2987.Count;
        funcSetSaveValue(self, "m_bSequenceLoops", false)  // Otherwise it will stutter && play an idle sequence at 999x playback speed for 0.001 second when changing from one idle to another!;
        else;
        funcSetSaveValue(self, "m_bSequenceLoops", true)  // "m_bSequenceLoops" has to be true because non-looped animations tend to cut off near the end, usually after the cycle passes 0.8;



        // Alternative system: Directly sets the translated activity, but has other downsides
        //if (this.CurrentIdleAnimation != GetIdealSequence() || Time.Now > this.NextIdleStandTime)
        //this.CurrentIdleAnimation = GetIdealSequence()
        //this.NextIdleStandTime = Time.Now + (SequenceDuration(GetIdealSequence()) / GetPlaybackRate())
        //ResetIdealActivity(this.TranslateActivity(ACT_IDLE))
        //

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.MaintainIdleBehavior(idleType)  // idleType: null = Random | 1 = Wander | 2 = Idle Stand;
        var curTime = Time.Now;
        var selfData = funcGetTable(self);
        if (selfData.Dead || selfData.VJ_IsBeingControlled || (selfData.AttackAnimTime > curTime) || (selfData.NextIdleTime > curTime) || (selfData.AA_CurrentMoveTime > curTime) || GetState() == VJ_STATE_ONLY_ANIMATION_CONSTANT) return 

        // Things that override can't bypass, Forces the NPC to ONLY idle stand!
        if (IsGoalActive() || selfData.DisableWandering || selfData.IsGuard || selfData.MovementType == VJ_MOVETYPE_STATIONARY || !selfData.LastHiddenZone_CanWander || selfData.NextWanderTime > curTime || selfData.IsFollowing || selfData.MedicData.Status)
        SCHEDULE_IDLE_STAND();
        return  // Don't set NextWanderTime below;
        else if (!idleType && selfData.IdleAlwaysWander)
        idleType = 1;


        // Random (Wander & Idle Stand)
        if (!idleType)
        if (Game.Random.NextInt(1, 3) == 1)
        SCHEDULE_IDLE_WANDER();
        else;
        SCHEDULE_IDLE_STAND();

        // Wander
        else if (idleType == 1)
        SCHEDULE_IDLE_WANDER();
        // Idle Stand
        else if (idleType == 2)
        SCHEDULE_IDLE_STAND();
        return  // Don't set NextWanderTime below;


        selfData.NextWanderTime = curTime + Game.Random.NextFloat(3, 6) // this.NextIdleTime;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        The main animation function, it can play activities, sequences && gestures;
        - animation = The animation to play, it can be a table OR string OR ACT_*;
        - Adding "vjseq_" to a string will make it play as a sequence;
        - Adding "vjges_" to a string will make it play as a gesture;
        - If it's a string AND "vjseq_" || "vjges_" is NOT added:
        - The base will attempt to convert it activity, if it fails, it will play it as a sequence;
        - This behavior can be overridden by AlwaysUseSequence & AlwaysUseGesture options;
        - lockAnim = Should the animation be locked && !interrupted? | Includes activities, behaviors, idle, chasing, attacking, etc. | DEFAULT: false;
        - NOTE: This automatically turns off for gestures, it only works for activities && sequences!;
        - false = Interruptible by everything!;
        - true = Interruptible by nothing, completely locked!;
        - "LetAttacks" = Interruptible ONLY by attacks!;
        - lockAnimTime = How long should it lock the animation? | DEFAULT: 0;
        - false = Base calculates the time (recommended);
        - faceEnemy = Should it constantly face the enemy while playing this animation? | DEFAULT: false;
        - false = Don't face the enemy;
        - true = Constantly face the enemy even behind walls, objects, etc.;
        - "Visible" = Only face the enemy while it's visible;
        - animDelay = Delays the animation by the given amount of time | DEFAULT: 0;
        - extraOptions = Table that holds extra options to modify parts of the code;
        - OnFinish(interrupted, anim) = A function that runs when the animation finishes | DEFAULT: null;
        - interrupted = Was the animation cut off? (Something stopped it before the animation completed);
        - anim = Animation it played, can be a string || an activity enum;
        - AlwaysUseSequence = Force attempt to play this animation as a sequence regardless of the other options | DEFAULT: false;
        - AlwaysUseGesture = Force attempt to play this animation as a gesture regardless of the other options | DEFAULT: false;
        - NOTE: Combining "AlwaysUseSequence" && "AlwaysUseGesture" will force it to play a gesture-sequence;
        - PlayBackRate = How fast should the animation play? | DEFAULT: Whatever the current playback rate is;
        - PlayBackRateCalculated = If the playback rate is already calculated in the "lockAnimTime", then set this to true! | DEFAULT: false;
        - customFunc(schedule, animation) = TODO: NOT FINISHED;
        Returns;
        - Animation, this may be an activity number || a string depending on how the animation played;
        - ACT_INVALID = No animation was played || found;
        - Number, Accurate animation play time after taking everything in account;
        - WARNING: If "animDelay" parameter is used, result may be inaccurate!;
        - Enum, Type of animation it played, such as activity, sequence, && gesture;
        - Enums are VJ.ANIM_TYPE_*;
        //---------------------------------------------------------]]
        function ENT.PlayAnim(animation, lockAnim, lockAnimTime, faceEnemy, animDelay, extraOptions, customFunc);
        animation = PICK(animation);
        if (!animation) return ACT_INVALID, 0, ANIM_TYPE_NONE 

        lockAnim = lockAnim || false;
        if (lockAnimTime == null)  // If user didn't put anything, then default it to 0
        lockAnimTime = 0;

        faceEnemy = faceEnemy || false;
        animDelay = tonumber(animDelay) || 0;
        extraOptions = extraOptions || {}
        var isGesture = false;
        var isSequence = false;
        var isString = isstring(animation);
        var isRecheck = false;

        ::recheck::
        // Handle "vjges_" and "vjseq_"
        if (isString)
        // animation = string.gsub(animation, "[vjges_|vjseq_]", "")  // Too slow
        var finalString;  // Only define a table if we need to!;
        var posCur = 1;
        for i = 1, animation.Count do
        var posStartGes, posEndGes = string_find(animation, "vjges_", posCur)  // Check for "vjges_";
        var posStartSeq, posEndSeq = string_find(animation, "vjseq_", posCur)  // Check for "vjseq_";
        if (!posStartGes && !posStartSeq)  // No ges || seq was found, end the loop!
        if (finalString)
        finalString[finalString.Count + 1] = string_sub(animation, posCur);

        break;

        if (!finalString) finalString = {} end  // Found a match, create table if needed
        if (posStartGes)
        isGesture = true;
        finalString[i] = string_sub(animation, posCur, posStartGes - 1);
        posCur = posEndGes + 1;

        if (posStartSeq)
        isSequence = true;
        finalString[i] = string_sub(animation, posCur, posStartSeq - 1);
        posCur = posEndSeq + 1;


        if (finalString)
        animation = table_concat(finalString);

        // If animation is -1 then it's probably an activity, so turn it into an activity
        // EX: "vjges_" .. ACT_MELEE_ATTACK1
        if (isGesture && !isSequence && LookupSequence(animation) == -1)
        animation = tonumber(animation);
        isString = false;



        if (extraOptions.AlwaysUseGesture) isGesture = true end  // Must play as a gesture
        if (extraOptions.AlwaysUseSequence)  // Must play as a sequence
        //isGesture = false  // Leave this alone to allow gesture-sequences to play even when "AlwaysUseSequence" is true!
        isSequence = true;
        if (isnumber(animation))  // If it's an activity, then convert it to a string
        animation = GetSequenceName(this.SelectWeightedSequence(animation));
        isString = true;

        else if (isString && !isSequence)  // Only for regular & gesture strings
        // If it can be played as an activity, then convert it!
        var result = funcGetSequenceActivity(self, LookupSequence(animation));
        if (result == null || result == -1)  // Leave it as string
        isSequence = true;
        else  // Set it as an activity
        animation = result;
        isString = false;



        // Check for activity translations
        if (!isString && !isRecheck)
        var translation = TranslateActivity(animation);
        if (translation != animation)
        animation = translation;
        // The translation is a string, recheck as it might be a gesture activity
        if (isstring(translation))
        isString = true;
        isRecheck = true;
        goto recheck;




        // Check if the animation actually exists
        if (AnimationHelper.Exists(self, animation) == false)
        return ACT_INVALID, 0, ANIM_TYPE_NONE;


        var animType = ((isGesture && ANIM_TYPE_GESTURE) || isSequence && ANIM_TYPE_SEQUENCE) || ANIM_TYPE_ACTIVITY  // Find the animation type;
        var seed = Time.Now  // Seed the current animation, used for animation delaying & on complete check;
        this.LastAnimType = animType;
        this.LastAnimSeed = seed;

        var function PlayAct();
        var originalPlaybackRate = this.AnimPlaybackRate;
        var customPlaybackRate = extraOptions.PlayBackRate;
        var playbackRate = customPlaybackRate || originalPlaybackRate;
        SkinnedModelRenderer.Set(\"speed\", playbackRate)  // Call this to change "this.AnimPlaybackRate" so "VJ.AnimDurationEx" can be calculated correctly;
        var animTime = VJ.AnimDurationEx(self, animation, false);
        this.AnimPlaybackRate = originalPlaybackRate  // Change it back to the true rate;
        var doRealAnimTime = true  // Only for activities, recalculate the animTime after the schedule starts to get the real sequence time, if `lockAnimTime` is NOT set!;

        if (lockAnim && !isGesture)
        if (isbool(lockAnimTime))  // false = Let the base calculate the time
        lockAnimTime = animTime;
        else  // Manually calculated
        doRealAnimTime = false;
        if (!extraOptions.PlayBackRateCalculated)  // Make sure !to calculate the playback rate when it already has!
        lockAnimTime = lockAnimTime / playbackRate;

        animTime = lockAnimTime;


        var curTime = Time.Now;
        this.NextChaseTime = curTime + lockAnimTime;
        this.NextIdleTime = curTime + lockAnimTime;
        this.AnimLockTime = curTime + lockAnimTime;

        if (lockAnim != "LetAttacks")
        StopAttacks(true);
        this.PauseAttacks = true;
        TimerLoop("attack_pause_reset" + EntIndex(), lockAnimTime, 1, () => function() this.PauseAttacks = false end);


        this.LastAnimSeed = seed  // We need to set it again because StopAttacks() above will reset it when it calls to chase enemy!;

        if (isGesture)
        // If it's an activity gesture AND it's already playing it, then remove it! Fixes same activity gestures bugging out when played right after each other!
        if (!isSequence && IsPlayingGesture(animation))
        RemoveGesture(animation);
        //RemoveAllGestures()  // Disallows the ability to layer multiple gestures!

        var gesture = isSequence && AddGestureSequence(this.LookupSequence(animation)) || AddGesture(animation);
        if (gesture != -1)
        SetLayerPriority(gesture, 1) // 2;
        //SetLayerWeight(gesture, 1)
        SetLayerPlaybackRate(gesture, playbackRate * 0.5);

        else  // Sequences & Activities
        var schedule = vj_ai_schedule.New("PlayAnim_" + animation);

        // For humans NPCs, internally the base will set these variables back to true after this function if it's called by weapon attack animations!
        this.WeaponAttackState = VJ.WEP_ATTACK_STATE_NONE;

        //StartEngineTask(ai.GetTaskID("TASK_RESET_ACTIVITY"), 0) //schedule.EngTask("TASK_RESET_ACTIVITY", 0)
        //if (this.Dead) schedule.EngTask("TASK_STOP_MOVING", 0) 
        //FrameAdvance(0)
        TaskComplete();
        StopMoving();
        ClearSchedule();
        ClearGoal();

        if (isSequence)
        doRealAnimTime = false  // Sequences already have the correct time;
        var seqID = LookupSequence(animation);
        //
        // START: Experimental transition system for sequences
        var transitionAnim = FindTransitionSequence(SkinnedModelRenderer.CurrentSequence, seqID)  // Find the transition sequence;
        var transitionAnimTime = 0;
        if (transitionAnim != -1 && seqID != transitionAnim)  // If it exists AND it's !the same as the animation
        transitionAnimTime = SequenceDuration(transitionAnim) / playbackRate;
        schedule:AddTask("TASK_VJ_PLAY_SEQUENCE", {
        animation = transitionAnim,;
        playbackRate = customPlaybackRate || false,;
        duration = transitionAnimTime;
        });

        // END: Experimental transition system for sequences
        //
        schedule:AddTask("TASK_VJ_PLAY_SEQUENCE", {
        animation = animation,;
        playbackRate = customPlaybackRate || false,;
        duration = animTime;
        });
        //PlaySequence(animation, playbackRate, extraOptions.SequenceDuration != false, dur)
        animTime = animTime + transitionAnimTime  // Adjust the animation time in case we have a transition animation!;
        else  // Only if activity
        //SetActivity(ACT_RESET)
        schedule:AddTask("TASK_VJ_PLAY_ACTIVITY", {
        animation = animation,;
        playbackRate = customPlaybackRate || false,;
        duration = doRealAnimTime || animTime;
        });
        // Old engine task animation system


        schedule.IsPlayActivity = true;
        schedule.CanBeInterrupted = !lockAnim;
        if ((customFunc)) customFunc(schedule, animation) 
        StartSchedule(schedule);
        if (doRealAnimTime)
        // Get the calculated duration (Only done in Activity type)
        animTime = this.CurrentTask.TaskData.duration;

        if (faceEnemy)
        SetTurnTarget("Enemy", animTime, false, faceEnemy == "Visible");



        // If it has a OnFinish function, then set the timer to run it when it finishes!
        if ((extraOptions.OnFinish))
        GameTask.DelaySeconds(animTime).ContinueWith(_ => function();
        if (this.IsValid() && !this.Dead)
        extraOptions.OnFinish(this.LastAnimSeed != seed, animation);

        end);

        return animTime;


        // For delay system
        if (animDelay > 0)
        GameTask.DelaySeconds(animDelay).ContinueWith(_ => function();
        if (this.IsValid() && this.LastAnimSeed == seed)
        PlayAct();

        end);
        return animation, animDelay + VJ.AnimDurationEx(self, animation, false), animType  // Approximation, this may be inaccurate!;
        else;
        return animation, PlayAct(), animType;


        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks if the NPC is busy with an animation || activity || behavior;
        - checkType = Type of busy check should it do | DEFAULT = false (all);
        // "Behaviors" = Behaviors only such as following a player or moving to heal an ally
        // "Activities" = Activities only such playing an animation that shouldn't be interrupted OR playing an attack animation!
        //- NAV_JUMP & NAV_CLIMB is based on "IsInterruptable" from engine: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/ai_navigator.h#L397
        Returns;
        - false, NPC is NOT busy;
        - true, NPC is Busy;
        //---------------------------------------------------------]]
        function ENT.IsBusy(checkType);
        var checkAll = !checkType;
        var selfData = funcGetTable(self);

        // Check behaviors
        if (checkAll)
        if (selfData.FollowData.Moving || selfData.MedicData.Status) return true 
        else if (checkType == "Behaviors")
        return selfData.FollowData.Moving || selfData.MedicData.Status;


        // Check activities
        if (checkAll || checkType == "Activities")
        if (selfData.PauseAttacks) return true 
        var curTime = Time.Now;
        if (selfData.AnimLockTime > curTime || selfData.AttackAnimTime > curTime) return true 
        var navType = GetNavType();
        return navType == NAV_JUMP || navType == NAV_CLIMB;


        return false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Sets the state of the NPC, states are prefixed with VJ_STATE_*;
        - state = The state it should set it to | DEFAULT = VJ_STATE_NONE;
        - time = How long should the state apply before it's reset to VJ_STATE_NONE?  | DEFAULT = -1;
        -1 = State stays indefinitely until reset || changed;
        //---------------------------------------------------------]]
        function ENT.SetState(state, time);
        state = state || VJ_STATE_NONE;
        time = time || -1;
        this.AIState = state;
        if (state == VJ_STATE_FREEZE || IsEFlagSet(EFL_IS_BEING_LIFTED_BY_BARNACLE))  // Reset the tasks
        TaskComplete();
        SCHEDULE_IDLE_STAND();

        if (time >= 0)
        TimerLoop("state_reset" + EntIndex(), time, 1, () => function();
        SetState();
        end);
        else;
        timer.Remove("state_reset" + EntIndex());


        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Returns the current state of the NPC;
        //---------------------------------------------------------]]
        function ENT.GetState();
        return this.AIState;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Decides the pitch for the NPC, very useful for speech-type of sounds!;
        - pitchVar = Pitch value to check;
        Returns;
        - Number, the chosen pitch number;
        //---------------------------------------------------------]]
        function ENT.GetSoundPitch(pitchVar);
        // We have been given "false",  use general sound pitch
        if (!pitchVar)
        // It's set to use the same sound pitch all the time, so check if we have it
        var pickedNum = this.MainSoundPitchValue;
        if (this.MainSoundPitchStatic && pickedNum != 0)
        return pickedNum;
        else;
        var mainPitch = this.MainSoundPitch;
        if (istable(mainPitch))
        return Game.Random.NextInt(mainPitch.a, mainPitch.b);

        return mainPitch;

        // We have been given table (VJ.SET), pick randomly between them
        else if (istable(pitchVar))
        return Game.Random.NextInt(pitchVar.a, pitchVar.b);
        // Most likely a number, just return it
        else;
        return pitchVar;


        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Decides the attack time;
        - mainTime = Main time to base this the timer off of;
        - executionTime = Used for timer-based attacks, decreases mainTime;
        - animDur = Used when mainTime is set to "false";
        // NOTE: Assumes playback rate is already calculated for this!
        Returns;
        - Number, the decided time;
        //---------------------------------------------------------]]
        function ENT.GetAttackTimer(mainTime, executionTime, animDur);
        // Let the base decide
        if (!mainTime)
        // Execution was event-based
        if (executionTime == false)
        return animDur;
        // Execution was timer-based
        else;
        // If it's 0 or less, then this attack probably did NOT play an animation, discard "animDur"
        if (animDur <= 0)
        return executionTime / this.AnimPlaybackRate;
        else;
        return animDur - (executionTime / this.AnimPlaybackRate);


        // Table has been given, discard "executionTime" and "animDur", then pick randomly
        else if (istable(mainTime))
        return Game.Random.NextFloat(mainTime.a, mainTime.b) / this.AnimPlaybackRate;
        // Number has been given, discard "executionTime" and "animDur"
        else;
        return mainTime / this.AnimPlaybackRate;


        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Stops most sounds played by the NPC | Excludes: Death, impact, attack misses, attack impacts;
        //---------------------------------------------------------]]
        function ENT.StopAllSounds();
        var selfData = funcGetTable(self);
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentExtraSpeechSound);
        StopSD(selfData.CurrentBreathSound);
        StopSD(selfData.CurrentIdleSound);
        StopSD(selfData.CurrentMedicAfterHealSound);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Quickly patches the given angle to the rotations the NPC is allowed to use (pitch, yaw, roll);
        - ang = The angle to patch;
        Returns;
        - Angle, the turn angle it should use;
        //---------------------------------------------------------]]
        function ENT.GetTurnAngle(ang);
        return this.TurningUseAllAxis && ang || Angle(0, ang.y, 0);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Resets the current turn target;
        //---------------------------------------------------------]]
        function ENT.ResetTurnTarget();
        var turnData = this.TurnData;
        turnData.Type = VJ.FACE_NONE;
        turnData.Target = null;
        turnData.StopOnFace = false;
        turnData.IsSchedule = false;
        turnData.LastYaw = 0;
        timer.Remove("turn_reset" + EntIndex());

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Makes the NPC turn && face the given target;
        - target = The turn target | Valid inputs: Entity, Vector, "Enemy";
        - faceTime = How long should it face the given target? | DEFAULT = 0 | -1 : face forever unless overridden, 0 : Only set it for a single frame!;
        - stopOnFace = If at any point the NPC ends up facing the target it will complete the facing! | DEFAULT: false;
        - This will also be triggered if something else (ex: movements) overrides the ideal yaw!;
        - If called on "Enemy" target && there is currently no active enemy, this will be triggered instantly!;
        - visibleOnly = Should it only face if the given target is visible? | DEFAULT: false;
        Returns;
        - Angle, the final angle it's going to face;
        - false, turning failed;
        //---------------------------------------------------------]]
        function ENT.SetTurnTarget(target, faceTime, stopOnFace, visibleOnly);
        if (this.MovementType == VJ_MOVETYPE_STATIONARY && !this.CanTurnWhileStationary) return false 
        var resultAng = false  // The final angle it's going to face;
        var updateTurn = true  // An override to disallow applying the angle now;
        var turnData = this.TurnData;
        // Enemy facing
        if (target == "Enemy")
        //// VJ.DEBUG: self, "SetTurnTarget", "ENEMY"
        ResetTurnTarget();
        var ene = funcGetEnemy(self);
        // If enemy is valid do normal facing otherwise return my angles because we didn't actually face an enemy
        if (ene.IsValid())
        if (this.TurningUseAllAxis)
        resultAng = GetTurnAngle(((ene.GetPos() + ene.OBBCenter()) - Transform.Position):Angle());
        else;
        resultAng = GetTurnAngle((ene.GetPos() - Transform.Position):Angle());

        else;
        resultAng = GetTurnAngle(Transform.Rotation);
        updateTurn = false;

        if (faceTime != 0)  // 0 = Face only this frame, so don't actually set turning data!
        turnData.Type = visibleOnly && VJ.FACE_ENEMY_VISIBLE || VJ.FACE_ENEMY;

        // Vector facing
        else if (isvector(target))
        //// VJ.DEBUG: self, "SetTurnTarget", "VECTOR"
        ResetTurnTarget();
        resultAng = GetTurnAngle((target - Transform.Position):Angle());
        if (faceTime != 0)  // 0 = Face only this frame, so don't actually set turning data!
        turnData.Type = visibleOnly && VJ.FACE_POSITION_VISIBLE || VJ.FACE_POSITION;
        turnData.Target = target;

        // Entity facing
        else if (target.IsValid())
        //// VJ.DEBUG: self, "SetTurnTarget", "ENTITY"
        ResetTurnTarget();
        if (this.TurningUseAllAxis)
        resultAng = GetTurnAngle(((target.GetPos() + target.OBBCenter()) - Transform.Position):Angle());
        else;
        resultAng = GetTurnAngle((target.GetPos() - Transform.Position):Angle());

        if (faceTime != 0)  // 0 = Face only this frame, so don't actually set turning data!
        turnData.Type = visibleOnly && VJ.FACE_ENTITY_VISIBLE || VJ.FACE_ENTITY;
        turnData.Target = target;


        if (resultAng)
        if (updateTurn)
        if (this.TurningUseAllAxis)
        var myAng = Transform.Rotation;
        Transform.Rotation = (LerpAngle(FrameTime().ToRotation() * GetMaxYawSpeed(), myAng, Angle(resultAng.p, myAng.y, resultAng.r)));

        SetIdealYawAndUpdate(resultAng.y);
        //if (IsSequenceFinished()) UpdateTurnActivity() 
        else  // Only set it, do NOT update it!
        SetIdealYaw(resultAng.y);

        if (faceTime != 0)  // 0 = Face only this frame, so don't actually set turning data!
        turnData.StopOnFace = stopOnFace || false;
        turnData.LastYaw = resultAng.y;
        if (faceTime != -1)  // -1 = Face forever && never reset unless overridden
        TimerLoop("turn_reset" + EntIndex(), faceTime || 0.2, 1, () => function();
        ResetTurnTarget();
        end);



        return resultAng;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // Based on: https://github.com/ValveSoftware/source-sdk-2013/blob/master/sp/src/game/server/ai_motor.cpp#L780
        function ENT.DeltaIdealYaw();
        var flCurrentYaw = (360 / 65536) * (math.floor(LocalTransform.Rotation.y * (65536 / 360)) % 65535);
        if (flCurrentYaw == GetIdealYaw())
        return 0;

        return math_angDif(GetIdealYaw(), flCurrentYaw);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        var function UTIL_VecToYaw(vec)  // Based on: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/shared/util_shared.cppL44.Count;
        if (vec.y == 0 && vec.x == 0) return 0 
        var yaw = math_deg(math_atan2(vec.y, vec.x));
        return yaw < 0 && yaw + 360 || yaw;

        //
        function ENT.OverrideMoveFacing(flInterval, move);
        var selfData = funcGetTable(self);
        if (!selfData.DisableFootStepSoundTimer) PlayFootstepSound() 
        //// VJ.DEBUG: self, "OverrideMoveFacing", flInterval
        //PrintTable(move)

        // Maintain turning
        var curTurnData = selfData.TurnData;
        if (curTurnData.Type && curTurnData.LastYaw != 0)
        UpdateYaw()  // Use "UpdateYaw" instead of "SetIdealYawAndUpdate" to avoid pose parameter glitches!;
        SetPoseParameter("move_yaw", math_angDif(UTIL_VecToYaw(move.dir), LocalTransform.Rotation.y));
        // Need to set the yaw pose parameter, otherwise when face moving, certain directions will look broken (such as Combine soldier facing forward while moving backwards)
        // Based on: "CAI_Motor::MoveFacing( const AILocalMoveGoal_t &move )" | Link: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/ai_motor.cpp#L631
        return true  // Disable engine move facing;


        // Handle the unique movement system for player models | Only face move direction if I have NOT faced anything else!
        if (selfData.UsePoseParameterMovement && selfData.MovementType == VJ_MOVETYPE_GROUND)
        //SetTurnTarget(GetCurWaypointPos())  // Because it will reset the current turning (if any), this will break "firing while moving" turning
        var resultAng = GetTurnAngle((GetCurWaypointPos() - Transform.Position):Angle());
        if (selfData.TurningUseAllAxis)
        var myAng = Transform.Rotation;
        Transform.Rotation = (LerpAngle(FrameTime().ToRotation() * GetMaxYawSpeed(), myAng, Angle(resultAng.p, myAng.y, resultAng.r)));

        SetIdealYawAndUpdate(resultAng.y);
        return true  // Disable engine move facing;


        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.OverrideMove(flInterval);
        // Maintain and handle jumping movements | Handle here instead of "RunAI" to fix landing problems
        // If (Nav type == NAV_JUMP and Goal type == GOALTYPE_NONE) then we are probably running a custom/forced jump! (non-task based jump)
        if (GetNavType() == NAV_JUMP && GetCurGoalType() == 0)
        if (OnGround())
        var result = MoveJumpStop();
        if (result == AIMR_CHANGE_TYPE)  // Landed && completed ACT_LAND animation
        SetNavType(NAV_GROUND);
        else  // AIMR_OK, still landing || playing ACT_LAND animation
        MoveJumpExec();

        else;
        MoveJumpExec();



        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Get the aim position of the given entity for the NPC to aim at | EX: Position the NPC should fire at;
        - target = The entity to aim at;
        - aimOrigin = The starting point of the aim | EX: Muzzle of a gun the NPC is holding;
        - predictionRate = Predication rate | DEFAULT = 0;
        // 0 : No prediction   |   0 < to > 1 : Closer to target   |   1 : Perfect prediction   |   1 < : Ahead of the prediction (will be very ahead/inaccurate)
        - projectileSpeed = Used if prediction is being used, helps it properly calculate the predicted aim position | DEFAULT = 1;
        Returns;
        - Vector, the best aim position it found | Normalize this return to get the aim direction!;
        //---------------------------------------------------------]]
        function ENT.GetAimPosition(target, aimOrigin, predictionRate, projectileSpeed);
        var result;
        if (funcVisible(self, target))
        result = target.BodyTarget(aimOrigin);
        if (target.IsPlayer())  // Decrease player's Z axis as it's placed very high by the engine
        result.z = result.z - 15;

        if (!VisibleVec(result))
        result = target.HeadTarget(aimOrigin) || target.EyePos()  // Certain non player/NPC targets will return null, so just use "EyePos";

        else  // If !visible, use the last known position!
        result = this.EnemyData.VisiblePos;
        predictionRate = 0  // Enemy is !visible, do NOT predict!;

        if ((predictionRate || 0) > 0)  // If prediction is enabled
        // 1. Calculate the distance between the origin and enemy position
        // 2. Calculate the time it takes for the projectile to reach the enemy
        // 3. Calculate the predicted enemy position based on their current position and velocity
        result = result + (VJ.GetMoveVelocity(target) * ((aimOrigin - result):Length() / (projectileSpeed || 1))) * predictionRate;

        return result;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Calculate the aim spread of the NPC depending on the given factors (Useful for bullets!);
        - target = When given, it will apply more modifiers based on the given entity (Assumes its an enemy!) | DEFAULT: NULL;
        - goalPos = Position we are trying to hit;
        - modifier = Final spread will be multiplied by this number | DEFAULT = 1 (no change);
        Returns;
        - Number, the aim spread;
        Calculation;
        // Target distance modifier
        1. Get Distance from NPC to goal position;
        2. Multiply it by the max distance at which the bullet spread is at its max;
        3. Normalize it between the calculated value && 0.05 where 0 is bullseye && 0.05 is max inaccuracy from distance;
        //
        // Target movement modifier
        4. Get the given target's movement speed (If target exists);
        5. Multiply it by the move speed at which the bullet spread is at its max;
        6. Normalize it between the calculated value && 0.05 where 0 is bullseye && 0.05 is max inaccuracy from move speed;
        7. Add it to the spread result;
        //
        // Suppression modifier
        8. Get the elapsed time since the NPC was last damaged based on "CurTime";
        9. Divide it by the cooldown time, amount of time until this modifier no longer affects the spread;
        10. Normalize it between the calculated value && 1.5 as it should never go above 1.5!;
        11. Negate the calculated value && subtract it against 2.5;
        -> This will make sure it will return 1 if cooldown is over, otherwise it will cause the final spread result to be 0!;
        12. Multiply the spread result by the calculated value;
        //
        // Other modifiers
        13. Multiply it by the owner's weapon accuracy (Weapon_Accuracy);
        14. Apply the modifier parameter, if any;
        //---------------------------------------------------------]]
        // To convert division to multiplication do (1 / division_number) | NOTE: Multiplication a bit faster!
        var aimMaxDist = 0.0000001  // Distance at which the bullet spread is at its max (most inaccurate) | Equivalent = Dividing by 10000000;
        var aimMaxMove = 0.0000001  // Move speed at which the bullet spread is at its max (most inaccurate) | Equivalent = Dividing by 10000000;
        var damageCooldown = 4  // Cooldown time in seconds, amount of time until this modifier no longer affects the spread;
        //
        function ENT.GetAimSpread(target, goalPos, modifier);
        var result = math_min(Transform.Position.DistToSqr(goalPos) * aimMaxDist, 0.05)  // Target distance modifier;
        if (target)
        result = result + math_min(VJ.GetMoveVelocity(target):LengthSqr() * aimMaxMove, 0.05)  // Target movement modifier;
        result = result * (2.5 - math_min((Time.Now - GetLastDamageTime()) / damageCooldown, 1.5))  // Suppression modifier (Inverse effect over time);

        return result * (this.Weapon_Accuracy || 1) * modifier;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Performs a group formation;
        - formType = Type of formation it should do;
        - Types: "Diamond";
        - baseEnt = The entity to base its position on, should be the same for all the members in the group!;
        - it = The place of the NPC in the group | DEFAULT = 0;
        - spacing = How far apart should they be?  | DEFAULT = 50;
        //---------------------------------------------------------]]
        function ENT.DoGroupFormation(formType, baseEnt, it, spacing);
        it = it || 0;
        spacing = spacing || 50;
        if (formType == "Diamond")
        if (it == 0)
        SetLastPosition(baseEnt.GetPos() + baseEnt.GetForward() * spacing + baseEnt.GetRight() * spacing);
        else if (it == 1)
        SetLastPosition(baseEnt.GetPos() + baseEnt.GetForward() * -spacing + baseEnt.GetRight() * spacing);
        else if (it == 2)
        SetLastPosition(baseEnt.GetPos() + baseEnt.GetForward() * spacing + baseEnt.GetRight() * -spacing);
        else if (it == 3)
        SetLastPosition(baseEnt.GetPos() + baseEnt.GetForward() * -spacing + baseEnt.GetRight() * -spacing);
        else;
        SetLastPosition(baseEnt.GetPos() + baseEnt.GetForward() * (spacing + (3 * it)) + baseEnt.GetRight() * (spacing + (3 * it)));



        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks if the front of the NPC can be used to take cover.;
        - startPos = Start position of the trace | DEFAULT = Center of the NPC;
        - endPos = End position of the trace | DEFAULT = Enemy's eye position;
        - acceptWorld = If it hits the world, it will accept it as a cover | DEFAULT = false;
        - extraOptions = Table that holds extra options to modify parts of the code;
        - SetLastHiddenTime = If true, it will reset the "LastHidden" time, which makes the NPC stick to a position if it's well covered | DEFAULT = false;
        - Debug = Used for debugging, spawns a cube at the hit position && prints the trace result | DEFAULT = false;
        Returns 2 values;
        - 1:
        - true, Hidden;
        - false, NOT hidden;
        - 2:
        - Table, trace result;
        //---------------------------------------------------------]]
        function ENT.DoCoverTrace(startPos, endPos, acceptWorld, extraOptions);
        var ene = funcGetEnemy(self);
        if (!ene.IsValid()) return false, {} 
        startPos = startPos || (Transform.Position + OBBCenter());
        endPos = endPos || ene.EyePos();
        extraOptions = extraOptions || {}
        var setLastHiddenTime = extraOptions.SetLastHiddenTime || false;
        var tr = util.TraceLine({
        start = startPos,;
        endpos = endPos,;
        filter = self,;
        mask = MASK_SHOT, // bit.bor(CONTENTS_SOLID, CONTENTS_WINDOW, CONTENTS_BLOCKLOS, CONTENTS_MOVEABLE, CONTENTS_MONSTER);
        collisiongroup = COLLISION_GROUP_NPC  // Otherwise it will collide with debris, ground weapons, etc;
        });
        var hitPos = tr.HitPos;
        var hitEnt = tr.Entity;
        if (extraOptions.Debug)
        DebugOverlay.Box(startPos, Vector(-2, -2, -2), Vector(2, 2, 2), 1, VJ.COLOR_GREEN);
        DebugOverlay.Text(startPos, "DoCoverTrace - startPos", 1);
        DebugOverlay.Box(endPos, Vector(-2, -2, -2), Vector(2, 2, 2), 1, VJ.COLOR_RED);
        DebugOverlay.Text(endPos, "DoCoverTrace - endPos", 1);
        DebugOverlay.Box(hitPos, Vector(-2, -2, -2), Vector(2, 2, 2), 1, VJ.COLOR_YELLOW);
        DebugOverlay.Line(startPos, hitPos, 1, VJ.COLOR_YELLOW);
        DebugOverlay.Text(hitPos, "DoCoverTrace - tr.HitPos", 1);


        // Sometimes tracing isn't 100%, a tiny find in sphere check fixes this issue...
        var sphereInvalidate = false;
        for _, v in Scene.FindInPhysics(hitPos, 5) do
        if (v == ene || v.VJ_ID_Living)
        sphereInvalidate = true;



        // Hiding zone: It hit world AND it's close, override "acceptWorld" option!
        if (tr.HitWorld && startPos.Distance(hitPos) < 200)
        if (setLastHiddenTime) this.LastHiddenZoneT = Time.Now + 20 
        return true, tr;
        // Not a hiding zone: (Sphere found current enemy or a living entity) OR (World is NOT accepted as a hiding zone) OR (Trace ent is current enemy or a living entity or is moving fast) OR (Trace hit very close to the end position)
        else if (sphereInvalidate || (!acceptWorld && tr.HitWorld) || (hitEnt.IsValid() && (hitEnt == ene || hitEnt.VJ_ID_Living || hitEnt.GetVelocity():LengthSqr() > 1000)) || endPos.Distance(hitPos) <= 10)
        if (setLastHiddenTime) this.LastHiddenZoneT = 0 
        return false, tr;
        else  // Hidden!
        if (setLastHiddenTime) this.LastHiddenZoneT = Time.Now + 20 
        return true, tr;


        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Forces the NPC to jump.;
        - vel = Velocity for the jump;
        EX: Force the NPC to jump to the location of another entity:
        ForceMoveJump((activator.GetPos() - Transform.Position):GetNormal()*200 + Vector(0, 0, 300));
        //---------------------------------------------------------]]
        function ENT.ForceMoveJump(vel);
        SetNavType(NAV_JUMP);
        MoveJumpStart(vel);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        The last damage hit group that the NPC received.;
        Returns;
        - number, the hit group;
        //---------------------------------------------------------]]
        function ENT.GetLastDamageHitGroup();
        return GetInternalVariable("m_LastHitGroup");

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Time since the NPC has been damaged (Used CurTime!);
        Returns;
        - number, time;
        //---------------------------------------------------------]]
        function ENT.GetLastDamageTime();
        return GetInternalVariable("m_flLastDamageTime");

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Number of times NPC has been damaged. Useful for tracking 1-shot kills;
        Returns;
        - number, the damage count;
        //---------------------------------------------------------]]
        function ENT.GetTotalDamageCount();
        return GetInternalVariable("m_iDamageCount");

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Scale the amount of energy used to calculate damage this NPC takes due to physics;
        - EXAMPLES: 0 = Take no physics damage | 0.001 = Take extremely minimum damage (manhack level) | 0.1 = Take little damage | 999999999 = Instant death;
        //---------------------------------------------------------]]
        function ENT.SetPhysicsDamageScale(scale);
        funcSetSaveValue(self, "m_impactEnergyScale", scale);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Takes the given number && returns a scaled number according to the difficulty that NPC is set to;
        - num = The number to scale;
        Returns;
        - number, the scaled number;
        //---------------------------------------------------------]]
        var DIFFICULTY_NEANDERTHAL        = VJ.DIFFICULTY_NEANDERTHAL;
        var DIFFICULTY_PUNY               = VJ.DIFFICULTY_PUNY;
        var DIFFICULTY_TRIVIAL            = VJ.DIFFICULTY_TRIVIAL;
        var DIFFICULTY_EASY               = VJ.DIFFICULTY_EASY;
        var DIFFICULTY_BEGINNER           = VJ.DIFFICULTY_BEGINNER;
        var DIFFICULTY_NORMAL             = VJ.DIFFICULTY_NORMAL;
        var DIFFICULTY_DIFFICULT          = VJ.DIFFICULTY_DIFFICULT;
        var DIFFICULTY_HARD               = VJ.DIFFICULTY_HARD;
        var DIFFICULTY_EXPERT             = VJ.DIFFICULTY_EXPERT;
        var DIFFICULTY_INSANE             = VJ.DIFFICULTY_INSANE;
        var DIFFICULTY_IMPOSSIBLE         = VJ.DIFFICULTY_IMPOSSIBLE;
        var DIFFICULTY_LUNATIC            = VJ.DIFFICULTY_LUNATIC;
        var DIFFICULTY_NIGHTMARE          = VJ.DIFFICULTY_NIGHTMARE;
        var DIFFICULTY_HELL_ON_EARTH      = VJ.DIFFICULTY_HELL_ON_EARTH;
        var DIFFICULTY_TOTAL_ANNIHILATION = VJ.DIFFICULTY_TOTAL_ANNIHILATION;
        var DIFFICULTY_EXTINCTION         = VJ.DIFFICULTY_EXTINCTION;
        //
        function ENT.ScaleByDifficulty(num);
        var dif = this.SelectedDifficulty;
        if (dif == DIFFICULTY_NORMAL)
        return num;
        else if (dif == DIFFICULTY_NEANDERTHAL)
        return math_max(num * 0.01, 1);
        else if (dif == DIFFICULTY_PUNY)
        return math_max(num * 0.10, 1);
        else if (dif == DIFFICULTY_TRIVIAL)
        return math_max(num * 0.25, 1);
        else if (dif == DIFFICULTY_EASY)
        return math_max(num * 0.50, 1);
        else if (dif == DIFFICULTY_BEGINNER)
        return math_max(num * 0.75, 1);
        else if (dif == DIFFICULTY_DIFFICULT)
        return num * 1.25;
        else if (dif == DIFFICULTY_HARD)
        return num * 1.5;
        else if (dif == DIFFICULTY_EXPERT)
        return num * 1.75;
        else if (dif == DIFFICULTY_INSANE)
        return num * 2;
        else if (dif == DIFFICULTY_IMPOSSIBLE)
        return num * 2.5;
        else if (dif == DIFFICULTY_LUNATIC)
        return num * 3;
        else if (dif == DIFFICULTY_NIGHTMARE)
        return num * 3.5;
        else if (dif == DIFFICULTY_HELL_ON_EARTH)
        return num * 4.5;
        else if (dif == DIFFICULTY_TOTAL_ANNIHILATION)
        return num * 6;
        else if (dif == DIFFICULTY_EXTINCTION)
        return num * 10;

        return num  // Unknown difficulty;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        var vecZN100 = Vector(0, 0, -100);
        //
        function ENT.IsJumpLegal(startPos, apex, endPos);
        var jumpData = this.JumpParams;
        if (!jumpData.Enabled) return false 
        if (((endPos.z - startPos.z) > jumpData.MaxRise) || ((apex.z - startPos.z) > jumpData.MaxRise) || ((startPos.z - endPos.z) > jumpData.MaxDrop) || (startPos.Distance(endPos) > jumpData.MaxDistance))
        return false;


        // Make sure there is a ground under where it will land!
        var tr = util.TraceLine({
        start = endPos,;
        endpos = endPos + vecZN100,;
        });

        return tr.Hit;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.OnChangeActivity(newAct);
        //// VJ.DEBUG: self, "OnChangeActivity", newAct
        //if (newAct == ACT_TURN_LEFT || newAct == ACT_TURN_RIGHT)
        //this.NextIdleStandTime = Time.Now + AnimationHelper.Duration(self, GetSequenceName(SkinnedModelRenderer.CurrentSequence))
        //

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // When engine saves or map transitions are loaded
        function ENT.OnRestore();
        //// VJ.DEBUG: self, "OnRestore"
        StopMoving();
        ResetMoveCalc();
        // Reset the current schedule because often times GMod attempts to run it before AI task modules have loaded!
        if (this.CurrentSchedule)
        this.CurrentSchedule = null;
        this.CurrentScheduleName = null;
        this.CurrentTask = null;
        this.CurrentTaskID = null;

        // Readd the weapon think hook because the transition / save does NOT do it!
        var wep = CurrentWeapon;
        if (wep.IsValid())
        EventSystem.Subscribe("Think", wep.NPC_Think);


        //-------------------------------------------------------------------------------------------------------------------------------------------
        // When GMod saves or duplicator tool are loaded
        function ENT.OnDuplicated(entTable);
        //// VJ.DEBUG: self, "OnDuplicated"

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // When GMod saves or duplicator tool are used to copy this NPC
        function ENT.OnEntityCopyTableFinish(data);
        //// VJ.DEBUG: self, "OnEntityCopyTableFinish"
        data.CurrentSchedule = null;
        data.CurrentScheduleName = null;
        data.CurrentTask = null;
        data.CurrentTaskID = null;
        data.RelationshipEnts = null;
        data.RelationshipMemory = null;
        data.PoseParameterLooking_Names = null;
        data.NextProcessT = null;
        data.TurnData = null;
        data.GuardData = null;
        data.PauseAttacks = null;
        data.AnimLockTime = null;
        data.AnimPlaybackRate = null;
        data.AnimModelSet = null;
        data.LastAnimSeed = null;
        data.LastAnimType = null;
        data.AttackSeed = null;
        data.AttackType = null;
        data.AttackState = null;
        data.AttackAnim = null;
        data.AttackAnimDuration = null;
        data.AttackAnimTime = null;
        data.NextDoAnyAttackT = null;
        data.IsAbleToMeleeAttack = null;
        data.MeleeAttack_IsPropAttack = null;
        data.NextIdleTime = null;
        data.NextWanderTime = null;
        data.NextChaseTime = null;
        data.EnemyData = null;
        data.Alerted = null;
        data.Flinching = null;
        data.NextFlinchT = null;
        data.HealthRegenDelayT = null;
        data.NextCombineBallDmgT = null;
        data.Dead = null;
        data.GibbedOnDeath = null;
        data.DeathAnimationCodeRan = null;
        data.TakingCoverT = null;
        data.NextOnPlayerSightT = null;
        data.LastHiddenZone_CanWander = null;
        data.LastHiddenZoneT = null;
        data.NextInvestigationMove = null;
        data.NextInvestigateSoundT = null;
        data.NextFootstepSoundT = null;
        data.NextBreathSoundT = null;
        data.NextIdleSoundT = null;
        data.IdleSoundBlockTime = null;
        data.NextAlertSoundT = null;
        data.NextCallForHelpT = null;
        data.NextCallForHelpAnimationT = null;
        data.NextLostEnemySoundT = null;
        data.NextAllyDeathSoundT = null;
        data.NextKilledEnemySoundT = null;
        data.NextDamageAllyResponseT = null;
        data.NextDamageByPlayerSoundT = null;
        data.NextPainSoundT = null;
        data.TimersToRemove = null;

        // Creature
        data.PropInteraction_Found = null;
        data.PropInteraction_NextCheckT = null;
        data.IsAbleToRangeAttack = null;
        data.IsAbleToLeapAttack = null;
        data.LeapAttackHasJumped = null;
        data.EatingData = null;

        // Human
        data.WeaponInventory = null;
        data.UpdatedPoseParam = null;
        data.Weapon_UnarmedBehavior_Active = null;
        data.WeaponEntity = null;
        data.WeaponState = null;
        data.WeaponInventoryStatus = null;
        data.AllowWeaponOcclusionDelay = null;
        data.WeaponLastShotTime = null;
        data.WeaponAttackState = null;
        data.WeaponAttackAnim = null;
        data.Weapon_AimTurnDiff_Def = null;
        data.NextWeaponAttackT = null;
        data.NextWeaponAttackT_Base = null;
        data.NextWeaponStrafeT = null;
        data.NextMeleeWeaponAttackT = null;
        data.NextMoveOnGunCoveredT = null;
        data.NextThrowGrenadeT = null;
        data.NextGrenadeAttackSoundT = null;
        data.NextSuppressingSoundT = null;
        data.NextDangerDetectionT = null;
        data.NextDangerSightSoundT = null;
        data.NextCombatDamageResponseT = null;

        // AA move types
        data.AA_NextMoveAnimTime = null;
        data.AA_CurrentMoveAnim = null;
        data.AA_CurrentMoveAnimType = null;
        data.AA_CurrentMoveMaxSpeed = null;
        data.AA_CurrentMoveTime = null;
        data.AA_CurrentMoveType = null;
        data.AA_CurrentMovePos = null;
        data.AA_CurrentMovePosDir = null;
        data.AA_CurrentMoveDist = null;
        data.AA_LastChasePos = null;
        data.AA_DoingLastChasePos = null;

        // Tank bases
        data.Tank_IsMoving = null;
        data.Tank_Status = null;
        data.Tank_NextLowHealthSparkT = null;
        data.Tank_NextRunOverSoundT = null;
        data.Tank_NextIdleParticles = null;
        data.Tank_FacingTarget = null;
        data.Tank_ReachableHeight = null;
        data.Tank_Shell_NextFireT = null;
        data.Tank_Shell_Status = null;
        data.Tank_TurningLerp = null;
        data.Gunner = null;

        // Following should be saved because:
        // Duplicator: Useful for duplicating NPCs without needing to set the behavior values individually (Ex: following another entity)
        // Saves: Usually intended targets will be NULL, and so the respective systems will reset without errors
        //data.MedicData = null
        //data.IsFollowing = null
        //data.FollowData = null
        //data.MainSoundPitchValue = null
        //data.AnimationTranslations = null

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.KeyValue(k, v);
        //// VJ.DEBUG: self, "KeyValue", k, v
        if (string_left(k, 2) == "On")
        StoreOutput(k, v);


        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.AcceptInput(key, activator, caller, data);
        //// VJ.DEBUG: self, "AcceptInput", key, activator, caller, data
        var funcCustom = this.OnInput; if (funcCustom) funcCustom(self, key, activator, caller, data);
        if (key == "Use")
        // 1. Add a delay so the game registers other key presses
        // 2. Check for mouse 1, mouse 2, and reload
        GameTask.DelaySeconds(0.1).ContinueWith(_ => function();
        if (this.IsValid() && this.FollowPlayer && !activator.KeyDown(IN_ATTACK) && !activator.KeyDownLast(IN_ATTACK) && !activator.KeyPressed(IN_ATTACK) && !activator.KeyReleased(IN_ATTACK) && !activator.KeyDown(IN_ATTACK2) && !activator.KeyDownLast(IN_ATTACK2) && !activator.KeyPressed(IN_ATTACK2) && !activator.KeyReleased(IN_ATTACK2) && !activator.KeyDown(IN_RELOAD) && !activator.KeyDownLast(IN_RELOAD) && !activator.KeyPressed(IN_RELOAD) && !activator.KeyReleased(IN_RELOAD))
        Follow(activator, true);

        end);
        else if (key == "StartScripting")
        SetState(VJ_STATE_FREEZE);
        else if (key == "StopScripting")
        SetState(VJ_STATE_NONE);
        else if (key == "break")
        var dmginfo = DamageInfo();
        dmginfo.SetDamage(Health());
        dmginfo.SetDamageType(DMG_ALWAYSGIB);
        dmginfo.SetAttacker(activator);
        dmginfo.SetInflictor(activator);
        TakeDamageInfo(dmginfo);
        return true;
        //else if (key == "SetHealth")
        //Health = data
        //MaxHealth = data

        return false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.HandleAnimEvent(ev, evTime, evCycle, evType, evOptions);
        //// VJ.DEBUG: self, "HandleAnimEvent", ev, evTime, evCycle, evType, evOptions
        var funcCustom = this.OnAnimEvent; if (funcCustom) funcCustom(self, ev, evTime, evCycle, evType, evOptions);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.Touch(entity);
        var selfData = funcGetTable(self);
        if (selfData.VJ_DEBUG && ConVar.GetInt("vj_npc_debug_touch") == 1) // VJ.DEBUG: self, "Touch", funcGetClass(entity) 
        var funcCustom = this.OnTouch; if (funcCustom) funcCustom(self, entity);
        if (!VJ_CVAR_AI_ENABLED || selfData.VJ_IsBeingControlled) return 

        // If it's a passive SNPC...
        if (selfData.Behavior == VJ_BEHAVIOR_PASSIVE || selfData.Behavior == VJ_BEHAVIOR_PASSIVE_NATURE)
        if (selfData.Passive_RunOnTouch && entity.VJ_ID_Living && Time.Now > selfData.TakingCoverT && entity.Behavior != VJ_BEHAVIOR_PASSIVE && entity.Behavior != VJ_BEHAVIOR_PASSIVE_NATURE && CheckRelationship(entity) != D_LI)
        SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH");
        PlaySoundSystem("Alert");
        selfData.TakingCoverT = Time.Now + Game.Random.NextFloat(3, 4);
        return;

        else if (selfData.EnemyTouchDetection && !selfData.IsFollowing && entity.VJ_ID_Living && !funcGetEnemy(this.IsValid()) && CheckRelationship(entity) != D_LI && !IsBusy())
        StopMoving();
        Target = entity;
        SCHEDULE_FACE("TASK_FACE_TARGET");
        return;


        // Handle "YieldToAlliedPlayers" system
        if (selfData.YieldToAlliedPlayers && !selfData.IsGuard)
        // entity is player
        if (entity.IsPlayer())
        if (CheckRelationship(entity) == D_LI)
        SetCondition(COND_PLAYER_PUSHING);
        if (!GetTarget(.IsValid()))  // Only set the target if it does NOT have one to !interfere with other behaviors!
        Target = entity;


        // entity is held by a player
        else if (entity.IsPlayerHolding())
        var findPly = entity.GetOwner();
        if (!findPly.IsValid())  // No owner found, try physics attacker
        findPly = entity.GetPhysicsAttacker();
        if (!findPly.IsValid())  // No physics attacker found, return it
        findPly = false;
        return;


        // Player was found, check if we are allied
        if (findPly && CheckRelationship(findPly) == D_LI)
        SetCondition(COND_PLAYER_PUSHING);
        if (!GetTarget(.IsValid()))  // Only set the target if it does NOT have one to !interfere with other behaviors!
        Target = findPly;





        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Resets && stops following the current entity (if any);
        //---------------------------------------------------------]]
        function ENT.ResetFollowBehavior();
        var followData = this.FollowData;
        var followEnt = followData.Target;
        if (followEnt.IsValid() && followEnt.IsPlayer() && this.CanChatMessage)
        if (this.Dead)
        followEnt.PrintMessage(HUD_PRINTTALK, VJ.GetName(self) + " has been killed.");
        else;
        followEnt.PrintMessage(HUD_PRINTTALK, VJ.GetName(self) + " is no longer following you.");


        this.IsFollowing = false;
        followData.Target = NULL;
        followData.MinDist = 0;
        followData.Moving = false;
        followData.StopAct = false;
        followData.NextUpdateT = 0;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Attempts to follow the given entity;
        - ent = Entity to follow;
        - doToggle = Should it stop following if it's already following the same entity? | DEFAULT = false;
        Returns;
        1 = Boolean;
        - true, successfully started following the entity;
        - false, failed || stopped following the entity;
        2 = Failure reason (if it failed);
        0 = Unknown / misc reasons;
        1 = NPC is stationary && unable to follow;
        2 = NPC is already following another entity;
        3 = NPC is hostile || neutral towards ent;
        //---------------------------------------------------------]]
        function ENT.Follow(ent, doToggle);
        if (!ent.IsValid() || this.Dead || !VJ_CVAR_AI_ENABLED || self == ent) return false, 0 

        var isPly = ent.IsPlayer();
        var isLiving = ent.VJ_ID_Living;
        if ((!isLiving) || (ent.Alive() && ((isPly && !VJ_CVAR_IGNOREPLAYERS) || (!isPly))))
        // Refusals
        var followData = this.FollowData;
        // Check for enemy/neutral
        if (isLiving && funcGetClass(self) != funcGetClass(ent) && (Disposition(ent) == D_HT || Disposition(ent) == D_NU))
        if (isPly && this.CanChatMessage)
        ent.PrintMessage(HUD_PRINTTALK, VJ.GetName(self) + " isn't friendly so it won't follow you.");

        return false, 3;
        // Check if it's already following another entity
        else if (this.IsFollowing && ent != followData.Target)
        if (isPly && this.CanChatMessage)
        ent.PrintMessage(HUD_PRINTTALK, VJ.GetName(self) + " is following another entity so it won't follow you.");

        return false, 2;
        // Check for invalid move types
        else if (this.MovementType == VJ_MOVETYPE_STATIONARY || this.MovementType == VJ_MOVETYPE_PHYSICS)
        if (isPly && this.CanChatMessage)
        ent.PrintMessage(HUD_PRINTTALK, VJ.GetName(self) + " is currently stationary so it can't follow you.");

        return false, 1;


        if (!this.IsFollowing)
        if (isPly)
        if (this.CanChatMessage)
        ent.PrintMessage(HUD_PRINTTALK, VJ.GetName(self) + " is now following you.");

        PlaySoundSystem("FollowPlayer");
        // Reset the guarding data
        this.GuardData.Position = false;
        this.GuardData.Direction = false;

        followData.Target = ent;
        followData.MinDist = this.FollowMinDistance + OBBMaxs().y + ent.OBBMaxs().y;
        this.IsFollowing = true;
        Target = ent;
        if (!IsBusy("Activities"))  // Face the entity && then move to it
        StopMoving();
        SCHEDULE_FACE("TASK_FACE_TARGET", function(x);
        x.RunCode_OnFinish = function();
        if (this.FollowData.Target.IsValid())
        SCHEDULE_GOTO_TARGET(((Transform.Position.Distance(this.FollowData.Target.GetPos()) < (followData.MinDist * 1.5)) && "TASK_WALK_PATH") || "TASK_RUN_PATH", function(y) y.CanShootWhenMoving = true y.TurnData = {Type = VJ.FACE_ENEMY} end);


        end);

        OnFollow("Start", ent);
        return true, 0;
        else if (doToggle)  // Unfollow the entity
        if (isPly)
        PlaySoundSystem("UnFollowPlayer");

        StopMoving();
        this.NextWanderTime = Time.Now + 2;
        if (!IsBusy("Activities"))
        SCHEDULE_FACE("TASK_FACE_TARGET");

        ResetFollowBehavior();
        OnFollow("Stop", ent);


        return false, 0;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.ResetMedicBehavior();
        OnMedicBehavior("OnReset", "End");
        var medicData = this.MedicData;
        if (medicData.Target.IsValid()) medicData.Target.VJ_ST_Healing = false 
        if (medicData.Prop.IsValid()) medicData.Prop.Remove() 
        medicData.Status = false;
        medicData.Target = NULL;
        medicData.Cooldown = Time.Now + Game.Random.NextFloat(this.Medic_NextHealTime.a, this.Medic_NextHealTime.b);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.MaintainMedicBehavior();
        var selfData = funcGetTable(self);
        if (selfData.Weapon_UnarmedBehavior_Active) return end  // Do NOT heal if playing scared animations!
        var medicData = selfData.MedicData;

        // Not healing anyone, check around for allies
        if (!medicData.Status)
        if (Time.Now < medicData.Cooldown) return 
        for _, ent in Scene.FindInPhysics(GetPos(, selfData.Medic_CheckDistance)) do
        if (ent != self && (ent.IsVJBaseSNPC || ent.IsPlayer()) && ent.VJ_ID_Healable && !ent.VJ_ST_Healing && !ent.VJ_ID_Vehicle && ent.Health() <= (ent.GetMaxHealth() * 0.75) && ((ent.IsNPC() && !funcGetEnemy(this.IsValid()) && (!funcGetEnemy(ent.IsValid()) || ent.VJ_IsBeingControlled)) || (ent.IsPlayer() && !VJ_CVAR_IGNOREPLAYERS)) && CheckRelationship(ent) == D_LI)
        medicData.Target = ent;
        medicData.Status = "Active";
        ent.VJ_ST_Healing = true;
        StopMoving();
        MaintainMedicBehavior();
        return;


        else if (medicData.Status != "Healing")
        var ally = medicData.Target;
        if (!ally.IsValid() || !ally.Alive() || (ally.Health() > ally.GetMaxHealth() * 0.75) || CheckRelationship(ally) != D_LI) ResetMedicBehavior() return 

        // Heal them!
        if (funcVisible(self, ally) && VJ.GetNearestDistance(self, ally) <= selfData.Medic_HealDistance)
        medicData.Status = "Healing";
        OnMedicBehavior("BeforeHeal");
        PlaySoundSystem("MedicBeforeHeal");

        // Spawn the prop
        if (selfData.Medic_SpawnPropOnHeal && Model.GetAttachmentIndex(selfData.Medic_SpawnPropOnHealAttachment) != 0)
        var prop = SceneUtility.CreatePrefab();
        prop.SetModel(selfData.Medic_SpawnPropOnHealModel);
        prop.SetLocalPos(Transform.Position);
        prop.SetOwner(self);
        prop.SetParent(self);
        prop.Fire("SetParentAttachment", selfData.Medic_SpawnPropOnHealAttachment);
        prop.SetCollisionGroup(COLLISION_GROUP_IN_VEHICLE);
        prop.Spawn();
        prop.Activate();
        prop.SetSolid(SOLID_NONE);
        //prop.AddEffects(EF_BONEMERGE)
        prop.SetRenderMode(RENDERMODE_TRANSALPHA);
        DeleteOnRemove(prop);
        medicData.Prop = prop;


        // Handle the heal time and animation
        var timeUntilHeal = selfData.Medic_TimeUntilHeal;
        var anims = selfData.AnimTbl_Medic_GiveHealth;
        if (anims)
        var _, animTime = PlayAnim(anims, true, false);
        if (!timeUntilHeal)  // Only change the heal time if "this.Medic_TimeUntilHeal" is set to false!
        timeUntilHeal = animTime;



        SetTurnTarget(ally, timeUntilHeal);

        // Make the ally turn and look at me
        if (!ally.IsPlayer() && (ally.MovementType != VJ_MOVETYPE_STATIONARY || (ally.MovementType == VJ_MOVETYPE_STATIONARY && ally.CanTurnWhileStationary == false)))
        selfData.NextWanderTime = Time.Now + 2;
        selfData.NextChaseTime = Time.Now + 2;
        ally.StopMoving();
        ally.SetTarget(self);
        ally.SCHEDULE_FACE("TASK_FACE_TARGET");


        GameTask.DelaySeconds(timeUntilHeal).ContinueWith(_ => function();
        if (this.IsValid())
        if (!ally.IsValid())  // Ally doesn't exist anymore, reset
        ResetMedicBehavior();
        else  // If it exists+.
        if (CheckRelationship(ally) != D_LI) ResetMedicBehavior() return end  // I no longer like them, stop healing them!
        if (VJ.GetNearestDistance(self, ally) <= (selfData.Medic_HealDistance + 20))  // Are we still in healing distance?
        if (OnMedicBehavior("OnHeal", ally) != false)
        var friCurHP = ally.Health();
        ally.SetHealth(math_min(math_max(friCurHP + selfData.Medic_HealAmount, friCurHP), ally.GetMaxHealth()));
        timer.Remove("timer_melee_bleed" + ally.EntIndex());
        timer.Adjust("timer_melee_slowply" + ally.EntIndex(), 0);
        ally.VJ_SpeedEffectT = 0;
        ally.RemoveAllDecals();

        PlaySoundSystem("MedicOnHeal");
        if (ally.IsVJBaseSNPC)
        ally.PlaySoundSystem("MedicReceiveHeal");

        ResetMedicBehavior();
        else  // If we are no longer in healing distance, go after the ally again
        medicData.Status = "Active";
        if (medicData.Prop.IsValid()) medicData.Prop.Remove() 
        OnMedicBehavior("OnReset", "Retry");



        end);
        // We aren't in healing distance, go after the ally!
        else if (!IsBusy("Activities"))
        selfData.NextIdleTime = Time.Now + 4;
        selfData.NextChaseTime = Time.Now + 4;
        Target = ally;
        SetMovementActivity(ACT_RUN)  // We run this constantly, set the movement activity constantly in case it never reaches "TASK_RUN_PATH";
        SCHEDULE_GOTO_TARGET("TASK_RUN_PATH");



        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.MaintainConstantlyFaceEnemy();
        var selfData = funcGetTable(self);
        var eneData = selfData.EnemyData;
        if (eneData.Distance < selfData.ConstantlyFaceEnemy_MinDistance)
        // Handle "IfVisible" and "IfAttacking" cases
        if ((selfData.ConstantlyFaceEnemy_IfVisible && !eneData.Visible) || (!selfData.ConstantlyFaceEnemy_IfAttacking && selfData.AttackType)) return 
        var postures = selfData.ConstantlyFaceEnemy_Postures;
        if ((postures == "Both") || (postures == "Moving" && Rigidbody.Velocity.Length > 0.1f) || (postures == "Standing" && !Rigidbody.Velocity.Length > 0.1f))
        SetTurnTarget("Enemy");
        return true;



        //-------------------------------------------------------------------------------------------------------------------------------------------
        var angY45 = Angle(0, 45, 0);
        var angYN45 = Angle(0, -45, 0);
        var angY90 = Angle(0, 90, 0);
        var angYN90 = Angle(0, -90, 0);
        //
        function ENT.Controller_Movement(cont, ply, bullseyePos);
        if (this.MovementType == VJ_MOVETYPE_STATIONARY) return false 
        var left = ply.KeyDown(IN_MOVELEFT);
        var right = ply.KeyDown(IN_MOVERIGHT);
        var sprint = ply.KeyDown(IN_SPEED);
        var aimVector = ply.GetAimVector();

        if (ply.KeyDown(IN_FORWARD))
        if (this.MovementType == VJ_MOVETYPE_AERIAL || this.MovementType == VJ_MOVETYPE_AQUATIC)
        AA_MoveTo(cont.VJCE_Bullseye, true, sprint && "Alert" || "Calm", {IgnoreGround = true});
        else;
        if (left)
        cont.StartMovement(aimVector, angY45);
        else if (right)
        cont.StartMovement(aimVector, angYN45);
        else;
        cont.StartMovement(aimVector, defAng);


        else if (ply.KeyDown(IN_BACK))
        if (left)
        cont.StartMovement(aimVector*-1, angYN45);
        else if (right)
        cont.StartMovement(aimVector*-1, angY45);
        else;
        cont.StartMovement(aimVector*-1, defAng);

        else if (left)
        cont.StartMovement(aimVector, angY90);
        else if (right)
        cont.StartMovement(aimVector, angYN90);
        else;
        StopMoving();
        if (this.MovementType == VJ_MOVETYPE_AERIAL || this.MovementType == VJ_MOVETYPE_AQUATIC)
        AA_StopMoving();


        return true;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.PlaySequence(animation);
        if (!animation) return false 
        //this.VJ_PlayingSequence = true  // No longer needed as it is handled by ACT_DO_NOT_DISTURB
        SetActivity(ACT_DO_NOT_DISTURB)  // So `GetActivity()` will return the current result (alongside other immediate calls after `PlaySequence`);
        SetIdealActivity(ACT_DO_NOT_DISTURB)  // Avoids the engine from progressing to an ideal activity that was set very recently | EX: Fixes melee attack anims breaking when called right after `SCHEDULE_IDLE_STAND()`;
        // Keeps MaintainActivity from overriding sequences as seen here: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/ai_basenpc.cpp#L6331
        // If `m_IdealActivity` is set to ACT_DO_NOT_DISTURB, the engine will understand it's a sequence and will avoid messing with it, described here: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/shared/ai_activity.h#L215
        var seqID = isstring(animation) && LookupSequence(animation) || animation;
        ResetSequence(seqID);
        ResetSequenceInfo();
        SkinnedModelRenderer.Set(\"cycle\", 0)  // Start from the beginning;

        return seqID;

        //------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Creates more timers for an attack | Note: it calculates playback rate!;
        - name = The name of the timer, ent index is concatenated at the end | DEFAULT: "timer_unknown";
        - time = How long until the timer expires | DEFAULT: 0.5;
        - func = The function to run when timer expires;
        //---------------------------------------------------------]]
        function ENT.AddExtraAttackTimer(name, time, func);
        name = name || "timer_unknown";
        this.TimersToRemove[this.Count.TimersToRemove + 1] = name;
        TimerLoop(name + EntIndex(), (time || 0.5) / this.AnimPlaybackRate, 1, () => func);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Forces the NPC to switch to the given entity as the enemy if certain criteria passes;
        - ent = The entity to set as the enemy;
        - stopMoving = Should it stop moving? Will !run it already has an enemy! | DEFAULT = false;
        - maxPerf = Used in "MaintainRelationships", skips all the initial checks for max performance | DEFAULT = false;
        - hasEnemy = Used alongside "maxPerf", determines if it has an enemy || !| DEFAULT = false;
        //---------------------------------------------------------]]
        function ENT.ForceSetEnemy(ent, stopMoving, maxPerf, hasEnemy);
        if (!maxPerf)
        if ((!ent.IsValid() || this.Behavior == VJ_BEHAVIOR_PASSIVE_NATURE || !ent.Alive() || (ent.IsPlayer() && VJ_CVAR_IGNOREPLAYERS))) return 
        hasEnemy = funcGetEnemy(this.IsValid());
        funcAddEntityRelationship(self, ent, D_HT, 0);

        Enemy = ent;
        UpdateEnemyMemory(ent, ent.GetPos());
        // Must be called after "UpdateEnemyMemory"
        // Let the engine know that our reaction time is instant otherwise it will reset the enemy if it's the first time it has seen this
        IgnoreEnemyUntil(ent, 0);
        SetNPCState(NPC_STATE_COMBAT);
        this.EnemyData.TimeSet = Time.Now;
        if (!hasEnemy || this.Alerted != ALERT_STATE_ENEMY)
        if (stopMoving && !this.Alerted)
        ClearGoal();
        StopMoving();

        DoEnemyAlert(ent);


        //-------------------------------------------------------------------------------------------------------------------------------------------
        // Makes the NPC alerted but only as ready, useful when it's alerted by something unknown
        function ENT.DoReadyAlert();
        this.EnemyData.Reset = false;
        this.Alerted = ALERT_STATE_READY;
        SetNPCState(NPC_STATE_ALERT);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.DoEnemyAlert(ent);
        //// VJ.DEBUG: self, "DoEnemyAlert", ent, funcGetEnemy(self, this.Alerted)
        var selfData = funcGetTable(self);
        var eneData = selfData.EnemyData;
        eneData.Distance = Transform.Position.Distance(ent.GetPos());
        if (selfData.Alerted == ALERT_STATE_ENEMY) return 
        var curTime = Time.Now;
        selfData.Alerted = ALERT_STATE_ENEMY;
        // Fixes the NPC switching from combat to alert to combat after it sees an enemy because `DoEnemyAlert` is called after NPC_STATE_COMBAT is set
        if (GetNPCState() != NPC_STATE_COMBAT)
        SetNPCState(NPC_STATE_ALERT);

        eneData.TimeAcquired = curTime;
        eneData.VisibleTime = curTime;
        eneData.DistanceNearest = VJ.GetNearestDistance(self, ent, true);
        OnAlert(ent);
        if (curTime > selfData.NextAlertSoundT)
        PlaySoundSystem("Alert");
        selfData.NextAlertSoundT = curTime + Game.Random.NextFloat(selfData.NextSoundTime_Alert.a, selfData.NextSoundTime_Alert.b);


        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Sets an specific data about the relationship between the NPC && another entity;
        - ent = Entity to set the relationship data;
        - memoryName = Name of the data (key);
        - memoryValue = Value of the data;
        //---------------------------------------------------------]]
        function ENT.SetRelationshipMemory(ent, memoryName, memoryValue);
        if (!ent.IsValid()) return 
        if (!this.RelationshipMemory[ent]) this.RelationshipMemory[ent] = {} 
        this.RelationshipMemory[ent][memoryName] = memoryValue;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks the relationship towards the given entity && converts custom dispositions such as "D_VJ_INTEREST" to the closest default source engine disposition;
        - ent = The entity to check its relation with;
        Returns;
        - Disposition value, list: https://wiki.facepunch.com/gmod/Enums/D;
        //---------------------------------------------------------]]
        function ENT.CheckRelationship(ent);
        if (ent.IsFlagSet(FL_NOTARGET) || !ent.Alive() || (ent.IsPlayer() && VJ_CVAR_IGNOREPLAYERS)) return D_ER 
        if (funcGetClass(self) == funcGetClass(ent)) return D_LI 
        var myDisp = Disposition(ent);
        if (myDisp == D_VJ_INTEREST) return D_HT 
        return myDisp;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        var cosRad20 = math_cos(math_rad(20));
        var ENT_TYPE_OTHER = 0;
        var ENT_TYPE_NPC = 1;
        var ENT_TYPE_PLAYER = 2;
        var ENT_TYPE_NEXTBOT = 3;
        //
        // Returns: Whether or not it found an enemy
        function ENT.MaintainRelationships();
        var selfData = funcGetTable(self);
        var myBehavior = selfData.Behavior;
        if (myBehavior == VJ_BEHAVIOR_PASSIVE_NATURE) return false 
        var entities = selfData.RelationshipEnts;
        if (!entities) return false 
        var memories = selfData.RelationshipMemory;
        //print("---------------------------------------")
        //// VJ.DEBUG: self, "MaintainRelationships"
        //PrintTable(entities)
        //print("----------")
        var myClasses = selfData.VJ_NPC_Class;
        var myClassesChanged = false;
        if (selfData.CacheRelationshipClasses != myClasses)
        myClassesChanged = true;
        selfData.CacheRelationshipClasses = myClasses;


        var eneVisCount = 0;
        var myPos = Transform.Position;
        var mySightDist = GetMaxLookDistance();
        var myHandlePerceived = this.HandlePerceivedRelationship;
        var myCanAlly = selfData.CanAlly;
        var myFriPlyAllies = selfData.AlliedWithPlayerAllies;
        var notIsNeutral = myBehavior != VJ_BEHAVIOR_NEUTRAL;
        var customFunc = this.OnMaintainRelationships;
        var nearestDist = false;
        var it = 1;
        //for k, ent in entities do
        //for it = 1, entities.Count do
        while it <= entities.Count do;
        var ent = entities[it];
        var entMemory = memories[ent];
        if (!ent.IsValid())
        table_remove(entities, it);
        memories[ent] = null;
        else;
        it = it + 1;

        // Handle no target and dead entities
        if (ent.IsFlagSet(FL_NOTARGET) || !ent.Alive())
        // If ent is our current enemy then reset it!
        if (funcGetEnemy(self) == ent)
        ResetEnemy(true, false);

        funcAddEntityRelationship(self, ent, D_NU, 0);
        continue;


        var entPos = ent.GetPos();
        var distanceToEnt = myPos.Distance(entPos);
        if (distanceToEnt > mySightDist)
        // If ent is our current enemy then reset it!
        if (funcGetEnemy(self) == ent)
        PlaySoundSystem("LostEnemy");
        ResetEnemy(true, false);

        continue;

        var calculatedDisp = entMemory[MEM_OVERRIDE_DISPOSITION] || false;
        var entType = entMemory[MEM_CACHE_ENT_TYPE];

        // Handle entity type caching
        if (!entType)
        if (ent.IsNPC())
        entType = ENT_TYPE_NPC;
        SetRelationshipMemory(ent, MEM_CACHE_ENT_TYPE, ENT_TYPE_NPC);
        else if (ent.IsPlayer())
        entType = ENT_TYPE_PLAYER;
        SetRelationshipMemory(ent, MEM_CACHE_ENT_TYPE, ENT_TYPE_PLAYER);
        else if (ent.IsNextBot())
        entType = ENT_TYPE_NEXTBOT;
        SetRelationshipMemory(ent, MEM_CACHE_ENT_TYPE, ENT_TYPE_NEXTBOT);
        else  // Other
        entType = ENT_TYPE_OTHER;
        SetRelationshipMemory(ent, MEM_CACHE_ENT_TYPE, ENT_TYPE_OTHER);



        //if (entType != ENT_TYPE_PLAYER)
        //	print(ent.GetFOV())
        //	ent.SetSaveValue("m_debugOverlays", bit.bor(0x00000001, 0x00000002, 0x00000004, 0x00000008, 0x00000010, 0x00000020, 0x00000040, 0x00000080, 0x00000100, 0x00000200, 0x00001000, 0x00002000, 0x00004000, 0x00008000, 0x00020000, 0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x04000000, 0x08000000, 0x10000000, 0x20000000, 0x40000000))
        //

        // Handle alliances
        if (myCanAlly && !calculatedDisp) // ent.VJ_ID_Living
        var entCachedClasses = entMemory[MEM_CACHE_CLASSES];
        var entClasses = ent.VJ_NPC_Class;
        // No cache found or the classes have changed, then recalculate the class disposition!
        if (myClassesChanged || entCachedClasses != entClasses)
        // Handle "self.VJ_NPC_Class"
        for _, friClass in myClasses do
        //if (friClass == "CLASS_PLAYER_ALLY" && !this.PlayerFriendly) this.PlayerFriendly = true end  // If player ally then set the PlayerFriendly to true
        if (entClasses && VJ.HasValue(entClasses, friClass))
        if (entType == ENT_TYPE_PLAYER)
        calculatedDisp = D_LI;
        else;
        // Since we both have "CLASS_PLAYER_ALLY" then we need to do a special check if we both also have "self.AlliedWithPlayerAllies"
        // If we both do NOT have that, then we both like players but not each other!
        if (friClass == "CLASS_PLAYER_ALLY")
        if ((myFriPlyAllies && ent.AlliedWithPlayerAllies) || ent.IsDefaultNPC)
        calculatedDisp = D_LI;

        else;
        calculatedDisp = D_LI;





        // Handle "self.PlayerFriendly" AND "self.AlliedWithPlayerAllies" (As a backup in case the NPC doesn't have the "CLASS_PLAYER_ALLY" class)
        //if (!calculatedDisp && this.PlayerFriendly && (entType == ENT_TYPE_PLAYER || (entType == ENT_TYPE_NPC && myFriPlyAllies && ent.PlayerFriendly && ent.AlliedWithPlayerAllies)))
        //calculatedDisp = D_LI
        //

        // Handle caching
        //// VJ.DEBUG: self, false, "!cached", ent, calculatedDisp
        SetRelationshipMemory(ent, MEM_CACHE_CLASSES, entClasses);
        if (calculatedDisp)
        SetRelationshipMemory(ent, MEM_CACHE_DISPOSITION, calculatedDisp);
        else  // No value set, then clear the cache!
        SetRelationshipMemory(ent, MEM_CACHE_DISPOSITION, null);

        else;
        // Class cache found! Check if we also have a disposition cache
        var entCachedDisposition = entMemory[MEM_CACHE_DISPOSITION];
        if (entCachedDisposition)
        calculatedDisp = entCachedDisposition;




        //print(HasEnemyEluded(ent), HasEnemyMemory(ent))
        //print(Time.Now - GetEnemyLastTimeSeen(ent))
        //print(Time.Now - GetEnemyFirstTimeSeen(ent))

        var entHandlePerceived = ent.HandlePerceivedRelationship;
        if (entHandlePerceived)
        // Return false to let rest of the function run otherwise return a disposition to override
        var result = entHandlePerceived(ent, self, distanceToEnt, calculatedDisp == D_LI);
        if (result)
        funcAddEntityRelationship(self, ent, result, 0);
        calculatedDisp = result;
        //continue



        // If the ent is a friend then set the relation as D_LI
        if (calculatedDisp == D_LI)
        //print("MaintainRelationships 2 - friendly!")
        // Reset the enemy if it's currently this friendly ent
        if (funcGetEnemy(self) == ent)
        ResetEnemy(true, false);


        //ent.AddEntityRelationship(self, D_LI, 0)
        funcAddEntityRelationship(self, ent, D_LI, 0);

        // Handle how non-VJ NPCs feel towards us
        if (entType == ENT_TYPE_NPC && !ent.IsVJBaseSNPC)
        // This is here to make sure non VJ NPCs will respect how entities should feel towards this NPC in case it's overridden
        if (myHandlePerceived)
        var result = myHandlePerceived(self, ent, distanceToEnt, true);
        if (result)
        ent.AddEntityRelationship(self, result, 0);
        else;
        ent.AddEntityRelationship(self, D_LI, 0);

        else;
        ent.AddEntityRelationship(self, D_LI, 0);



        // YieldToAlliedPlayers system, Based on:
        // "CNPC_PlayerCompanion::PredictPlayerPush"	--> https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/hl2/npc_playercompanion.cpp#L548
        // "CAI_BaseNPC::TestPlayerPushing"				--> https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/server/ai_basenpc.cpp#L12676
        if (entType == ENT_TYPE_PLAYER && selfData.YieldToAlliedPlayers && !selfData.IsGuard && ent.GetMoveType() != MOVETYPE_NOCLIP) // && !IsBusy("Activities")
        var plyVel = ent.GetInternalVariable("m_vecSmoothedVelocity");
        if (plyVel.LengthSqr() >= 19600)  // 140 * 140 = 19600
        var delta = WorldSpaceCenter() - (ent.WorldSpaceCenter() + plyVel * 0.4);
        var myMaxs = OBBMaxs();
        var myMins = OBBMins();
        var zCalc = (myMaxs.z - myMins.z) * 0.5;
        var yCalc = myMaxs.y - myMins.y;
        // (ply not under me) + (ply not very above me) + (ply is close to me)   |   All calculations depend on the NPC's collision size AND player's current speed
        if (delta.z < zCalc && (delta.z + zCalc + 150) > zCalc && delta.Length2DSqr() < ((yCalc * yCalc) * 1.999396))  // 1.414 * 1.414 = 1.999396
        SetCondition(COND_PLAYER_PUSHING);
        if (!GetTarget(.IsValid()))  // Only set the target if it does NOT have one to !interfere with other behaviors!
        Target = ent;




        else;
        // Handle how non-VJ NPCs feel towards us
        if (entType == ENT_TYPE_NPC && !ent.IsVJBaseSNPC)
        // This is here to make sure non VJ NPCs will respect how entities should feel towards this NPC in case it's overridden
        if (myHandlePerceived)
        var result = myHandlePerceived(self, ent, distanceToEnt, false);
        if (result)
        ent.AddEntityRelationship(self, result, 0);
        else;
        ent.AddEntityRelationship(self, D_HT, 0);

        else;
        ent.AddEntityRelationship(self, D_HT, 0);



        var ene = funcGetEnemy(self);
        var eneValid = ene.IsValid();
        if (!calculatedDisp || calculatedDisp == D_VJ_INTEREST || calculatedDisp == D_HT)
        // Check if this NPC should be engaged, if not then set it as an interest but don't engage it
        // Restriction: If the current enemy is this entity then skip as it we want to engage regardless
        var entCanEngage = ent.CanBeEngaged;
        if (entCanEngage && !entCanEngage(ent, self, distanceToEnt) && (!eneValid || ene != ent))
        //print("MaintainRelationships 2 - entCanEngage")
        funcAddEntityRelationship(self, ent, D_VJ_INTEREST, 0);
        calculatedDisp = D_VJ_INTEREST;
        else;
        // SetEnemy: In order - Can find enemy + Not neutral or Is alerted + Is visible + In sight cone
        if (selfData.EnemyDetection && (notIsNeutral || selfData.Alerted == ALERT_STATE_ENEMY) && (selfData.EnemyXRayDetection || funcVisible(self, ent)) && funcIsInViewCone(self, entPos))
        //print("MaintainRelationships 2 - set enemy")
        funcAddEntityRelationship(self, ent, D_HT, 0);
        calculatedDisp = D_HT;
        eneValid = true;
        eneVisCount = eneVisCount + 1;
        // If the detected enemy is closer than the previous enemies, the set this as the enemy!
        if (!nearestDist || (distanceToEnt < nearestDist))
        nearestDist = distanceToEnt;
        ForceSetEnemy(ent, true, true, eneValid);

        // If all else failed then check if we hate this entity
        else if (Disposition(ent) != D_HT)
        // Neutral NPCs will not engage enemies without a reason, so keep it as neutral
        if (!notIsNeutral)
        //print("MaintainRelationships 2 - regular D_NU")
        funcAddEntityRelationship(self, ent, D_NU, 0);
        calculatedDisp = D_NU;
        // Everyone else will set potential enemies as interest
        else;
        //print("MaintainRelationships 2 - regular D_VJ_INTEREST")
        funcAddEntityRelationship(self, ent, D_VJ_INTEREST, 0);
        calculatedDisp = D_VJ_INTEREST;



        else;
        calculatedDisp = D_NU;


        // Investigation detection: Sound and player flashlight systems
        if (!eneValid && selfData.CanInvestigate && selfData.NextInvestigationMove < Time.Now)
        // Investigation: Sound detection
        if (ent.VJ_SD_InvestLevel && distanceToEnt < (selfData.InvestigateSoundMultiplier * ent.VJ_SD_InvestLevel) && ((Time.Now - ent.VJ_SD_InvestTime) <= 1))
        DoReadyAlert();
        if (funcVisible(self, ent))
        StopMoving();
        Target = ent;
        SCHEDULE_FACE("TASK_FACE_TARGET");
        selfData.NextInvestigationMove = Time.Now + 0.3  // Short delay, since it's only turning;
        else if (!selfData.IsFollowing)
        SetLastPosition(entPos);
        SCHEDULE_GOTO_POSITION("TASK_WALK_PATH", function(schedule);
        //if (eneValid) schedule.EngTask("TASK_FORGET", ene) 
        //schedule.EngTask("TASK_IGNORE_OLD_ENEMIES", 0)
        schedule.CanShootWhenMoving = true;
        //schedule.CanBeInterrupted = true
        schedule.TurnData = {Type = VJ.FACE_ENEMY}
        end);
        selfData.NextInvestigationMove = Time.Now + 2  // Long delay, so it doesn't spam movement;

        OnInvestigate(ent);
        PlaySoundSystem("Investigate");
        // Investigation: Player shining flashlight onto the NPC
        else if (entType == ENT_TYPE_PLAYER && distanceToEnt < 350 && ent.FlashlightIsOn() && (ent.GetForward():Dot((myPos - entPos):GetNormalized()) > cosRad20))
        StopMoving();
        Target = ent;
        SCHEDULE_FACE("TASK_FACE_TARGET");
        selfData.NextInvestigationMove = Time.Now + 0.1  // Short delay, since it's only turning;




        // HasOnPlayerSight system, used to do certain actions when it sees the player
        if (entType == ENT_TYPE_PLAYER && selfData.HasOnPlayerSight && Time.Now > selfData.NextOnPlayerSightT && distanceToEnt < selfData.OnPlayerSightDistance && funcVisible(self, ent) && funcIsInViewCone(self, entPos))
        // 0 = Run it every time | 1 = Run it only when friendly to player | 2 = Run it only when enemy to player
        var disp = selfData.OnPlayerSightDispositionLevel;
        if ((disp == 0) || (disp == 1 && (Disposition(ent) == D_LI || Disposition(ent) == D_NU)) || (disp == 2 && Disposition(ent) != D_LI))
        OnPlayerSight(ent);
        PlaySoundSystem("OnPlayerSight");
        if (selfData.OnPlayerSightOnlyOnce)  // If it's only suppose to play it once then turn the system off
        selfData.HasOnPlayerSight = false;
        else;
        selfData.NextOnPlayerSightT = Time.Now + Game.Random.NextFloat(selfData.OnPlayerSightNextTime.a, selfData.OnPlayerSightNextTime.b);




        if (customFunc) customFunc(self, ent, calculatedDisp, distanceToEnt) 


        selfData.EnemyData.VisibleCount = eneVisCount;
        //print("---------------------------------------")
        return eneVisCount > 0;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks allies around the NPC && call them to come to help the NPC;
        - dist = Radius of the call | DEFAULT: 800;
        //---------------------------------------------------------]]
        function ENT.Allies_CallHelp(dist);
        var selfData = funcGetTable(self);
        var ene = funcGetEnemy(self);
        var myClass = funcGetClass(self);
        var myPos = metaEntity.GetPos(self);
        var curTime = Time.Now;
        var isFirst = true  // Is this the first ent that received a call?;
        for _, ent in Scene.FindInPhysics(myPos, dist || 800) do
        var entData = funcGetTable(ent);
        if (ent != self && entData.IsVJBaseSNPC && entData.CanReceiveOrders && metaEntity.Alive(ent) && (funcGetClass(ent) == myClass || metaNPC.Disposition(ent, self) == D_LI) && entData.Behavior != VJ_BEHAVIOR_PASSIVE_NATURE && funcGetClass(ene) != funcGetClass(ent) && !funcGetEnemy(ent.IsValid()))
        // If it's guarding and enemy is not visible, then don't call!
        if (entData.IsGuard && !funcVisible(ent, ene)) continue 

        var eneIsPlayer = ene.IsPlayer();
        if (((!eneIsPlayer && metaNPC.Disposition(ent, ene) != D_LI) || eneIsPlayer))
        // Enemy too far away for ent
        var entsPos = metaEntity.GetPos(ent);
        if (entsPos.Distance(metaEntity.GetPos(ene)) > metaNPC.GetMaxLookDistance(ent))
        // See if you can move to the ent's location to get closer
        if (!entData.IsFollowing && !entData.IsBusy(ent))
        // If it's wandering, then just override it as it's not important
        if (metaNPC.IsMoving(ent) && selfData.CurrentScheduleName != "SCHEDULE_IDLE_WANDER")
        continue;

        metaNPC.SetLastPosition(ent, myPos + Transform.Right * Game.Random.NextInt(-50, 50) + Transform.Forward * Game.Random.NextInt(-50, 50));
        entData.SCHEDULE_GOTO_POSITION(ent, "TASK_RUN_PATH", function(x) x.CanShootWhenMoving = true x.TurnData = {Type = VJ.FACE_ENEMY} end);
        else;
        continue;

        else;
        // If the enemy is a player and the ent is player-friendly then make that player an enemy to the ent
        if (eneIsPlayer && metaNPC.Disposition(ent, ene) == D_LI)
        entData.SetRelationshipMemory(ent, ene, VJ.MEM_OVERRIDE_DISPOSITION, D_HT);

        entData.ForceSetEnemy(ent, ene, true);
        if (curTime > entData.NextChaseTime)
        if (entData.Behavior != VJ_BEHAVIOR_PASSIVE && funcVisible(ent, ene))
        metaNPC.SetTarget(ent, ene);
        entData.SCHEDULE_FACE(ent, "TASK_FACE_TARGET");
        else;
        entData.PlaySoundSystem(ent, "ReceiveOrder");
        entData.MaintainAlertBehavior(ent);




        selfData.OnCallForHelp(self, ent, isFirst);
        selfData.PlaySoundSystem(self, "CallForHelp");
        // Play the animation
        if (curTime > selfData.NextCallForHelpAnimationT)
        var anims = selfData.AnimTbl_CallForHelp;
        if (anims)
        selfData.PlayAnim(self, anims, true, false, selfData.CallForHelpAnimFaceEnemy);
        selfData.NextCallForHelpAnimationT = curTime + selfData.CallForHelpAnimCooldown;


        isFirst = false;




        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks allies around the NPC that can receive orders && return all of them as a table;
        - dist = How far to check for allies | DEFAULT: 800;
        Returns;
        - false, Failed to find any allies;
        - Table, table of allies it found;
        //---------------------------------------------------------]]
        function ENT.Allies_Check(dist);
        var allies = {}
        var alliesNum = 0;
        var isPassive = this.Behavior == VJ_BEHAVIOR_PASSIVE || this.Behavior == VJ_BEHAVIOR_PASSIVE_NATURE;
        var myClass = funcGetClass(self);
        for _, ent in Scene.FindInPhysics(GetPos(, dist || 800)) do
        var entData = funcGetTable(ent);
        if (ent != self && entData.IsVJBaseSNPC && entData.CanReceiveOrders && ent.Alive() && (funcGetClass(ent) == myClass || (ent.Disposition(self) == D_LI || entData.Behavior == VJ_BEHAVIOR_PASSIVE_NATURE)))
        if (isPassive)
        if (entData.Behavior == VJ_BEHAVIOR_PASSIVE || entData.Behavior == VJ_BEHAVIOR_PASSIVE_NATURE)
        alliesNum = alliesNum + 1;
        allies[alliesNum] = ent;

        else;
        alliesNum = alliesNum + 1;
        allies[alliesNum] = ent;



        return alliesNum > 0 && allies || false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks allies around the NPC && brings them to the NPC if they can receive orders;
        - formType = Type of formation the allies should do | DEFAULT: "Random";
        - Types: "Random" | "Diamond";
        - dist = How far to check for allies | DEFAULT: 800;
        - entsTbl = Pass in a table of entities to use, otherwise it will run a sphere check | DEFAULT: Sphere check;
        - limit = How many allies can it bring? | DEFAULT: 3;
        - 0 = Unlimited;
        - onlyVis = Should it only allow allies that are visible? | DEFAULT: false;
        Returns;
        - false, Failed to find any allies;
        - true, Found at least 1 ally;
        //---------------------------------------------------------]]
        function ENT.Allies_Bring(formType, dist, entsTbl, limit, onlyVis);
        var myPos = Transform.Position;
        formType = formType || "Random";
        dist = dist || 800;
        limit = limit || 3;
        var myClass = funcGetClass(self);
        var it = 0;
        var curTime = Time.Now;
        for _, ent in entsTbl || Scene.FindInPhysics(myPos, dist) do
        var entData = funcGetTable(ent);
        if (ent != self && entData.IsVJBaseSNPC && entData.CanReceiveOrders && ent.Alive() && (funcGetClass(ent) == myClass || ent.Disposition(self) == D_LI) && entData.Behavior != VJ_BEHAVIOR_PASSIVE && entData.Behavior != VJ_BEHAVIOR_PASSIVE_NATURE && !entData.IsFollowing && !entData.IsGuard && curTime > entData.TakingCoverT)
        if (onlyVis && !funcVisible(ent, self)) continue 
        if (!funcGetEnemy(ent.IsValid()) && myPos.Distance(ent.GetPos()) < dist)
        this.NextWanderTime = curTime + 8;
        entData.NextWanderTime = curTime + 8;
        it = it + 1;
        // Formation
        if (formType == "Random")
        var randPos = Game.Random.NextInt(1, 4);
        if (randPos == 1)
        ent.SetLastPosition(myPos + Transform.Right*Game.Random.NextInt(20, 50));
        else if (randPos == 2)
        ent.SetLastPosition(myPos + Transform.Right*Game.Random.NextInt(-20, -50));
        else if (randPos == 3)
        ent.SetLastPosition(myPos + Transform.Forward*Game.Random.NextInt(20, 50));
        else if (randPos == 4)
        ent.SetLastPosition(myPos + Transform.Forward*Game.Random.NextInt(-20, -50));

        else if (formType == "Diamond")
        ent.DoGroupFormation("Diamond", self, it);

        // Move type
        if (entData.IsVJBaseSNPC_Human && !ent.GetActiveWeapon(.IsValid()))
        ent.SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH");
        else;
        ent.SCHEDULE_GOTO_POSITION("TASK_WALK_PATH", function(x) x.CanShootWhenMoving = true x.TurnData = {Type = VJ.FACE_ENEMY} end);


        if (limit != 0 && it >= limit) return true end  // Reached the limit


        return it > 0;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        var function flinchDamageTypeCheck(checkTbl, dmgType);
        for k = 1, checkTbl.Count do
        if (bAND(dmgType, checkTbl[k]) != 0)
        return true;



        //
        function ENT.Flinch(dmginfo, hitgroup);
        var curTime = Time.Now;
        var selfData = funcGetTable(self);
        var flinchType = selfData.CanFlinch;
        if (!flinchType || flinchType == 0 || selfData.Flinching || selfData.AnimLockTime > curTime || selfData.NextFlinchT > curTime || GetNavType() == NAV_JUMP || GetNavType() == NAV_CLIMB || selfData.AttackType == VJ.ATTACK_TYPE_GRENADE) return 

        // DMG_FORCE_FLINCH: Skip secondary checks, flinch chance, and damage types!
        var customDmgType = dmginfo.GetDamageCustom();
        if (customDmgType == VJ.DMG_FORCE_FLINCH || (customDmgType != VJ.DMG_BLEED && selfData.TakingCoverT < curTime && Game.Random.NextInt(1, selfData.FlinchChance) == 1 && (flinchType == true || flinchType == 1 || ((flinchType == "DamageTypes" || flinchType == 2) && flinchDamageTypeCheck(selfData.FlinchDamageTypes, dmginfo.GetDamageType())))))
        if (OnFlinch(dmginfo, hitgroup, "Init")) return 

        var function executeFlinch(hitgroupAnim);
        selfData.Flinching = true;
        StopAttacks(true);
        selfData.AttackAnimTime = 0;
        var _, animDur = PlayAnim(hitgroupAnim || selfData.AnimTbl_Flinch, true, false, false);
        TimerLoop("flinch_reset" + EntIndex(), animDur, 1, () => function() this.Flinching = false end);
        OnFlinch(dmginfo, hitgroup, "Execute");
        selfData.NextFlinchT = curTime + (!selfData.FlinchCooldown && animDur || selfData.FlinchCooldown);


        var hitgroupTbl = selfData.FlinchHitGroupMap;
        // Hitgroup flinching
        if (hitgroupTbl)
        for _, v in hitgroupTbl do
        var hitGroups = v.HitGroup;
        if (istable(hitGroups))  // Sub-table hitgroup
        for hitgroupX = 1, hitGroups.Count do
        if (hitGroups[hitgroupX] == hitgroup)
        executeFlinch(v.Animation);
        return;


        else  // non-table hitrgoup
        if (hitGroups == hitgroup)
        executeFlinch(v.Animation);
        return;



        if (selfData.FlinchHitGroupPlayDefault)
        executeFlinch();

        // Non-hitgroup flinching
        else;
        executeFlinch();



        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Sets the NPC's blood color (particle, decal, blood pool);
        - blColor = The blood color to set it to | Must be a string, check the list below;
        //---------------------------------------------------------]]
        var bloodNames = {
        [VJ.BLOOD_COLOR_RED] = {
        particle = "blood_impact_red_01", // vj_blood_impact_red;
        decal = "VJ_Blood_Red",;
        decal_gmod = "Blood",;
        pool = {
        [0] = "vj_blood_pool_red_tiny",
        [1] = "vj_blood_pool_red_small",
        [2] = "vj_blood_pool_red"
        }
        },;
        [VJ.BLOOD_COLOR_YELLOW] = {
        particle = "blood_impact_yellow_01", // vj_blood_impact_yellow;
        decal = "VJ_Blood_Yellow",;
        decal_gmod = "YellowBlood",;
        pool = {
        [0] = "vj_blood_pool_yellow_tiny",
        [1] = "vj_blood_pool_yellow_small",
        [2] = "vj_blood_pool_yellow"
        }
        },;
        [VJ.BLOOD_COLOR_GREEN] = {
        particle = "vj_blood_impact_green",;
        decal = "VJ_Blood_Green",;
        pool = {
        [0] = "vj_blood_pool_green_tiny",
        [1] = "vj_blood_pool_green_small",
        [2] = "vj_blood_pool_green"
        }
        },;
        [VJ.BLOOD_COLOR_ORANGE] = {
        particle = "vj_blood_impact_orange",;
        decal = "VJ_Blood_Orange",;
        pool = {
        [0] = "vj_blood_pool_orange_tiny",
        [1] = "vj_blood_pool_orange_small",
        [2] = "vj_blood_pool_orange"
        }
        },;
        [VJ.BLOOD_COLOR_BLUE] = {
        particle = "vj_blood_impact_blue",;
        decal = "VJ_Blood_Blue",;
        pool = {
        [0] = "vj_blood_pool_blue_tiny",
        [1] = "vj_blood_pool_blue_small",
        [2] = "vj_blood_pool_blue"
        }
        },;
        [VJ.BLOOD_COLOR_PURPLE] = {
        particle = "vj_blood_impact_purple",;
        decal = "VJ_Blood_Purple",;
        pool = {
        [0] = "vj_blood_pool_purple_tiny",
        [1] = "vj_blood_pool_purple_small",
        [2] = "vj_blood_pool_purple"
        }
        },;
        [VJ.BLOOD_COLOR_WHITE] = {
        particle = "vj_blood_impact_white",;
        decal = "VJ_Blood_White",;
        pool = {
        [0] = "vj_blood_pool_white_tiny",
        [1] = "vj_blood_pool_white_small",
        [2] = "vj_blood_pool_white"
        }
        },;
        [VJ.BLOOD_COLOR_OIL] = {
        particle = "vj_blood_impact_oil",;
        decal = "VJ_Blood_Oil",;
        pool = {
        [0] = "vj_blood_pool_oil_tiny",
        [1] = "vj_blood_pool_oil_small",
        [2] = "vj_blood_pool_oil"
        }
        },;
        }
        //
        function ENT.SetupBloodColor(blColor);
        if (!isstring(blColor)) return end  // Only strings allowed!
        var npcSize = OBBMaxs():Distance(OBBMins());
        npcSize = ((npcSize < 25 && 0) || npcSize < 50 && 1) || 2  // 0 = tiny | 1 = small | 2 = normal;
        var blood = bloodNames[blColor];
        if (blood)
        var selfData = funcGetTable(self);
        if (!PICK(selfData.BloodParticle))
        selfData.BloodParticle = blood.particle;

        if (!PICK(selfData.BloodDecal))
        selfData.BloodDecal = selfData.BloodDecalUseGMod && blood.decal_gmod || blood.decal;

        if (!PICK(selfData.BloodPool))
        selfData.BloodPool = blood.pool[npcSize];



        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.SpawnBloodParticles(dmginfo, hitgroup);
        var particleName = PICK(this.BloodParticle);
        if (particleName)
        var dmgPos = dmginfo.GetDamagePosition();
        var particle = SceneUtility.CreatePrefab();
        particle.SetKeyValue("effect_name", particleName);
        particle.SetPos((dmgPos == defPos && (Transform.Position + OBBCenter())) || dmgPos);
        particle.Spawn();
        particle.Activate();
        particle.Fire("Start");
        particle.Fire("Kill", null, 0.1);


        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.SpawnBloodDecals(dmginfo, hitgroup);
        var decals = this.BloodDecal;
        if (!PICK(decals)) return 

        var dmgForce = dmginfo.GetDamageForce();
        var dmgPos = dmginfo.GetDamagePosition();
        if (dmgPos == defPos) dmgPos = Transform.Position + OBBCenter() 
        var clampedLength = math_min(math_max(dmgForce.Length() * 10, 100), this.BloodDecalDistance);

        // Badi ayroun
        var tr = SceneTrace.Ray({start = dmgPos, endpos = dmgPos + dmgForce.GetNormal() * clampedLength, filter = self}).Run();
        var trNormalP = tr.HitPos + tr.HitNormal;
        var trNormalN = tr.HitPos - tr.HitNormal;
        Decals.Place(PICK(decals), trNormalP, trNormalN, self);
        for _ = 1, 2 do
        if (Game.Random.NextInt(1, 2) == 1) Decals.Place(PICK(decals), trNormalP + Vector(Game.Random.NextInt(-70, 70), Game.Random.NextInt(-70, 70), 0), trNormalN, self) 


        // Kedni ayroun
        if (Game.Random.NextInt(1, 2) == 1)
        var d2_endpos = dmgPos + Vector(0, 0, - clampedLength);
        Decals.Place(PICK(decals), dmgPos, d2_endpos, self);
        if (Game.Random.NextInt(1, 2) == 1) Decals.Place(PICK(decals), dmgPos, d2_endpos + Vector(Game.Random.NextInt(-120, 120), Game.Random.NextInt(-120, 120), 0), self) 


        //-------------------------------------------------------------------------------------------------------------------------------------------
        var vecZ30 = Vector(0, 0, 30);
        var vecZ1 = Vector(0, 0, 1);
        //
        function ENT.SpawnBloodPool(dmginfo, hitgroup, corpse);
        var getBloodPool = PICK(this.BloodPool);
        if (getBloodPool)
        GameTask.DelaySeconds(2.2).ContinueWith(_ => function();
        if (corpse.IsValid())
        var pos = corpse.GetPos() + corpse.OBBCenter();
        var tr = util.TraceLine({
        start = pos,;
        endpos = pos - vecZ30,;
        filter = corpse,;
        mask = CONTENTS_SOLID;
        });
        if (tr.HitWorld && (tr.HitNormal == vecZ1)) // (tr.Fraction <= 0.405)
        Particles.Play(getBloodPool, tr.HitPos, defAng, null);


        end);


        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.PlayFootstepSound(customSD);
        var selfData = funcGetTable(self);
        if (selfData.HasSounds && selfData.HasFootstepSounds && selfData.MovementType != VJ_MOVETYPE_STATIONARY && IsOnGround())
        if (selfData.DisableFootStepSoundTimer)
        // Use custom table if available, if none found then use the footstep sound table
        var pickedSD = customSD && PICK(customSD) || PICK(selfData.SoundTbl_FootStep);
        if (pickedSD)
        SoundManager.Emit(self, pickedSD, selfData.FootstepSoundLevel, GetSoundPitch(selfData.FootstepSoundPitch));
        var funcCustom = this.OnFootstepSound; if (funcCustom) funcCustom(self, "Event", pickedSD);

        else if (Rigidbody.Velocity.Length > 0.1f && Time.Now > selfData.NextFootstepSoundT && GetMoveDelay() <= 0)
        // Use custom table if available, if none found then use the footstep sound table
        var pickedSD = customSD && PICK(customSD) || PICK(selfData.SoundTbl_FootStep);
        if (pickedSD)
        if (selfData.FootstepSoundTimerRun && GetMovementActivity() == ACT_RUN)
        SoundManager.Emit(self, pickedSD, selfData.FootstepSoundLevel, GetSoundPitch(selfData.FootstepSoundPitch));
        var funcCustom = this.OnFootstepSound; if (funcCustom) funcCustom(self, "Run", pickedSD);
        selfData.NextFootstepSoundT = Time.Now + selfData.FootstepSoundTimerRun;
        else if (selfData.FootstepSoundTimerWalk && GetMovementActivity() == ACT_WALK)
        SoundManager.Emit(self, pickedSD, selfData.FootstepSoundLevel, GetSoundPitch(selfData.FootstepSoundPitch));
        var funcCustom = this.OnFootstepSound; if (funcCustom) funcCustom(self, "Walk", pickedSD);
        selfData.NextFootstepSoundT = Time.Now + selfData.FootstepSoundTimerWalk;





        //-------------------------------------------------------------------------------------------------------------------------------------------
        // combatIdle = Play combat idle if possible
        function ENT.PlayIdleSound(customSD, sdType, combatIdle);
        var selfData = funcGetTable(self);
        if (!selfData.HasSounds || !selfData.HasIdleSounds) return false 

        var curTime = Time.Now;
        if (selfData.IdleSoundBlockTime < curTime && selfData.NextIdleSoundT < curTime)
        var setTimer = true;
        if (customSD)
        customSD = PICK(customSD);


        // Yete CombatIdle tsayn chouni YEV gerna barz tsayn hanel, ere vor barz tsayn han e
        if (combatIdle && !PICK(selfData.SoundTbl_CombatIdle) && !selfData.IdleSoundsRegWhileAlert)
        combatIdle = false;


        if (combatIdle)
        var pickedSD = PICK(selfData.SoundTbl_CombatIdle);
        if ((pickedSD && Game.Random.NextInt(1, selfData.CombatIdleSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentIdleSound);
        selfData.CurrentIdleSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.CombatIdleSoundLevel, GetSoundPitch(selfData.CombatIdleSoundPitch));

        else if (Game.Random.NextInt(1, selfData.IdleSoundChance) == 1 || customSD)
        var pickedSD = PICK(selfData.SoundTbl_Idle);
        var pickedDialogueSD = PICK(selfData.SoundTbl_IdleDialogue);
        var playRegular = true;
        if (pickedDialogueSD && selfData.HasIdleDialogueSounds && Game.Random.NextInt(1, 2) == 1)
        var foundEnt;
        var canAnswer = false;
        // Don't break the loop unless we hit a VJ NPC that can answer break
        // If above failed, then simply return the last checked ally
        for _, ent in Scene.FindInPhysics(GetPos(, selfData.IdleDialogueDistance)) do
        if (ent != self)
        if (ent.IsPlayer())
        if (CheckRelationship(ent) == D_LI && !OnIdleDialogue(ent, "CheckEnt", false))
        foundEnt = ent;

        else if (ent.IsNPC() && !ent.Dead && ((funcGetClass(self) == funcGetClass(ent)) || (CheckRelationship(ent) == D_LI)) && funcVisible(self, ent))
        var hasDialogueAnswer = (ent.IsVJBaseSNPC && PICK(ent.SoundTbl_IdleDialogueAnswer)) || false;
        if (!OnIdleDialogue(ent, "CheckEnt", hasDialogueAnswer))
        foundEnt = ent;
        if (hasDialogueAnswer)
        canAnswer = true;
        break;






        if (foundEnt)
        playRegular = false;
        StopSD(selfData.CurrentIdleSound);
        selfData.CurrentIdleSound = (sdType || VJ.CreateSound)(self, pickedDialogueSD, selfData.IdleDialogueSoundLevel, GetSoundPitch(selfData.IdleDialogueSoundPitch));
        if (canAnswer)  // If we have a VJ NPC that can answer
        var dur = SoundDuration(pickedDialogueSD);
        if (dur == 0) dur = 3 end  // Since some file types don't return a proper duration =(
        var talkTime = curTime + (dur + 0.5);
        setTimer = false;
        selfData.NextIdleSoundT = talkTime;
        selfData.NextWanderTime = talkTime;
        foundEnt.NextIdleSoundT = talkTime;
        foundEnt.NextWanderTime = talkTime;

        OnIdleDialogue(foundEnt, "Speak", talkTime);

        // Stop moving and face each other
        if (selfData.IdleDialogueCanTurn)
        StopMoving();
        Target = foundEnt;
        SCHEDULE_FACE("TASK_FACE_TARGET");

        if (foundEnt.IdleDialogueCanTurn)
        foundEnt.StopMoving();
        foundEnt.SetTarget(self);
        foundEnt.SCHEDULE_FACE("TASK_FACE_TARGET");


        // For the other NPC to answer back:
        GameTask.DelaySeconds(dur + 0.3).ContinueWith(_ => function();
        if (this.IsValid() && foundEnt.IsValid() && !foundEnt.OnIdleDialogue(self, "Answer"))
        var response = foundEnt.PlaySoundSystem("IdleDialogueAnswer") || 0;
        if (response > 0)  // If the ally responded, then make sure both SNPCs stand still & don't play another idle sound until the whole conversation is finished!
        var curTime2 = Time.Now;
        selfData.NextIdleSoundT = curTime2 + response + 0.5;
        selfData.NextWanderTime = curTime2 + response + 1;
        foundEnt.NextIdleSoundT = curTime2 + response + 0.5;
        foundEnt.NextWanderTime = curTime2 + response + 1;


        end);



        // Didn't play a dialogue so play regular
        if (playRegular && (pickedSD || customSD))
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentIdleSound);
        selfData.CurrentIdleSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.IdleSoundLevel, GetSoundPitch(selfData.IdleSoundPitch));


        if (setTimer)
        selfData.NextIdleSoundT = curTime + Game.Random.NextFloat(selfData.NextSoundTime_Idle.a, selfData.NextSoundTime_Idle.b);



        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.PlaySoundSystem(sdSet, customSD, sdType);
        var selfData = funcGetTable(self);
        if (!selfData.HasSounds || !sdSet) return false 
        if (customSD)
        customSD = PICK(customSD);


        if (sdSet == "IdleDialogueAnswer")
        if (selfData.HasIdleDialogueAnswerSounds)
        var pickedSD = PICK(selfData.SoundTbl_IdleDialogueAnswer);
        if ((pickedSD && Game.Random.NextInt(1, selfData.IdleDialogueAnswerSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentExtraSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(2, 3);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.IdleDialogueSoundLevel, GetSoundPitch(selfData.IdleDialogueSoundPitch));
        return SoundDuration(pickedSD)  // Return the duration of the sound, which will be used to make the other NPC stand still;

        return 0;

        return 0;
        else if (sdSet == "FollowPlayer")
        if (selfData.HasFollowPlayerSounds)
        var pickedSD = PICK(selfData.SoundTbl_FollowPlayer);
        if ((pickedSD && Game.Random.NextInt(1, selfData.FollowPlayerSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(3, 4);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.FollowPlayerSoundLevel, GetSoundPitch(selfData.FollowPlayerPitch));


        else if (sdSet == "UnFollowPlayer")
        if (selfData.HasFollowPlayerSounds)
        var pickedSD = PICK(selfData.SoundTbl_UnFollowPlayer);
        if ((pickedSD && Game.Random.NextInt(1, selfData.FollowPlayerSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(3, 4);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.FollowPlayerSoundLevel, GetSoundPitch(selfData.FollowPlayerPitch));


        else if (sdSet == "ReceiveOrder")
        if (selfData.HasReceiveOrderSounds)
        var pickedSD = PICK(selfData.SoundTbl_ReceiveOrder);
        if ((pickedSD && Game.Random.NextInt(1, selfData.ReceiveOrderSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.NextIdleSoundT = selfData.NextIdleSoundT + 2;
        selfData.NextAlertSoundT = Time.Now + 2;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.ReceiveOrderSoundLevel, GetSoundPitch(selfData.ReceiveOrderSoundPitch));


        else if (sdSet == "YieldToPlayer")
        if (selfData.HasYieldToPlayerSounds)
        var pickedSD = PICK(selfData.SoundTbl_YieldToPlayer);
        if ((pickedSD && Game.Random.NextInt(1, selfData.YieldToPlayerSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(3, 4);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.YieldToPlayerSoundLevel, GetSoundPitch(selfData.YieldToPlayerSoundPitch));


        else if (sdSet == "MedicBeforeHeal")
        if (selfData.HasMedicSounds)
        var pickedSD = PICK(selfData.SoundTbl_MedicBeforeHeal);
        if ((pickedSD && Game.Random.NextInt(1, selfData.MedicBeforeHealSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(3, 4);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.MedicBeforeHealSoundLevel, GetSoundPitch(selfData.MedicBeforeHealSoundPitch));


        else if (sdSet == "MedicOnHeal")
        if (selfData.HasMedicSounds)
        var pickedSD = PICK(selfData.SoundTbl_MedicOnHeal);
        if ((pickedSD && Game.Random.NextInt(1, selfData.MedicOnHealSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(3, 4);
        selfData.CurrentMedicAfterHealSound = (sdType || VJ.EmitSound)(self, pickedSD, selfData.MedicOnHealSoundLevel, GetSoundPitch(selfData.MedicOnHealSoundPitch));


        else if (sdSet == "MedicReceiveHeal")
        if (selfData.HasMedicSounds)
        var pickedSD = PICK(selfData.SoundTbl_MedicReceiveHeal);
        if ((pickedSD && Game.Random.NextInt(1, selfData.MedicReceiveHealSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(3, 4);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.MedicReceiveHealSoundLevel, GetSoundPitch(selfData.MedicReceiveHealSoundPitch));


        else if (sdSet == "OnPlayerSight")
        if (selfData.HasOnPlayerSightSounds)
        var pickedSD = PICK(selfData.SoundTbl_OnPlayerSight);
        if ((pickedSD && Game.Random.NextInt(1, selfData.OnPlayerSightSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        var dur = Time.Now + ((((SoundDuration(pickedSD) > 0) && SoundDuration(pickedSD)) || 3.5) + 1);
        selfData.IdleSoundBlockTime = dur;
        selfData.NextAlertSoundT = Time.Now + Game.Random.NextInt(1, 2);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.OnPlayerSightSoundLevel, GetSoundPitch(selfData.OnPlayerSightSoundPitch));


        else if (sdSet == "Investigate")
        if (selfData.HasInvestigateSounds && Time.Now > selfData.NextInvestigateSoundT)
        var pickedSD = PICK(selfData.SoundTbl_Investigate);
        if ((pickedSD && Game.Random.NextInt(1, selfData.InvestigateSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.NextIdleSoundT = selfData.NextIdleSoundT + 2;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.InvestigateSoundLevel, GetSoundPitch(selfData.InvestigateSoundPitch));

        selfData.NextInvestigateSoundT = Time.Now + Game.Random.NextFloat(selfData.NextSoundTime_Investigate.a, selfData.NextSoundTime_Investigate.b);

        else if (sdSet == "LostEnemy")
        if (selfData.HasLostEnemySounds && Time.Now > selfData.NextLostEnemySoundT)
        var pickedSD = PICK(selfData.SoundTbl_LostEnemy);
        if ((pickedSD && Game.Random.NextInt(1, selfData.LostEnemySoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.NextIdleSoundT = selfData.NextIdleSoundT + 2;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.LostEnemySoundLevel, GetSoundPitch(selfData.LostEnemySoundPitch));

        selfData.NextLostEnemySoundT = Time.Now + Game.Random.NextFloat(selfData.NextSoundTime_LostEnemy.a, selfData.NextSoundTime_LostEnemy.b);

        else if (sdSet == "Alert")
        if (selfData.HasAlertSounds)
        var pickedSD = PICK(selfData.SoundTbl_Alert);
        if ((pickedSD && Game.Random.NextInt(1, selfData.AlertSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        var dur = Time.Now + ((((SoundDuration(pickedSD) > 0) && SoundDuration(pickedSD)) || 2) + 1);
        selfData.NextIdleSoundT = dur;
        selfData.NextPainSoundT = dur;
        selfData.NextSuppressingSoundT = Time.Now + 4;
        selfData.NextAlertSoundT = Time.Now + Game.Random.NextFloat(selfData.NextSoundTime_Alert.a, selfData.NextSoundTime_Alert.b);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.AlertSoundLevel, GetSoundPitch(selfData.AlertSoundPitch));


        else if (sdSet == "CallForHelp")
        if (selfData.HasCallForHelpSounds)
        var pickedSD = PICK(selfData.SoundTbl_CallForHelp);
        if ((pickedSD && Game.Random.NextInt(1, selfData.CallForHelpSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.NextIdleSoundT = selfData.NextIdleSoundT + 2;
        selfData.NextSuppressingSoundT = Time.Now + Game.Random.NextInt(2.5, 4);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.CallForHelpSoundLevel, GetSoundPitch(selfData.CallForHelpSoundPitch));


        else if (sdSet == "BeforeMeleeAttack")
        if (selfData.HasMeleeAttackSounds)
        var pickedSD = PICK(selfData.SoundTbl_BeforeMeleeAttack);
        if ((pickedSD && Game.Random.NextInt(1, selfData.BeforeMeleeAttackSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentExtraSpeechSound);
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentExtraSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.BeforeMeleeAttackSoundLevel, GetSoundPitch(selfData.BeforeMeleeAttackSoundPitch));


        else if (sdSet == "MeleeAttack")
        if (selfData.HasMeleeAttackSounds)
        var pickedSD = PICK(selfData.SoundTbl_MeleeAttack);
        if ((pickedSD && Game.Random.NextInt(1, selfData.MeleeAttackSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.MeleeAttackSoundLevel, GetSoundPitch(selfData.MeleeAttackSoundPitch));

        if (selfData.HasExtraMeleeAttackSounds)
        pickedSD = PICK(selfData.SoundTbl_MeleeAttackExtra);
        if ((pickedSD && Game.Random.NextInt(1, selfData.ExtraMeleeSoundChance) == 1) || customSD)
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        SoundManager.Emit(self, pickedSD, selfData.ExtraMeleeAttackSoundLevel, GetSoundPitch(selfData.ExtraMeleeSoundPitch));



        else if (sdSet == "MeleeAttackMiss")
        if (selfData.HasMeleeAttackMissSounds)
        var pickedSD = PICK(selfData.SoundTbl_MeleeAttackMiss);
        if ((pickedSD && Game.Random.NextInt(1, selfData.MeleeAttackMissSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        StopSD(selfData.CurrentMeleeAttackMissSound);
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentMeleeAttackMissSound = (sdType || VJ.EmitSound)(self, pickedSD, selfData.MeleeAttackMissSoundLevel, GetSoundPitch(selfData.MeleeAttackMissSoundPitch));


        else if (sdSet == "BecomeEnemyToPlayer")
        if (selfData.HasBecomeEnemyToPlayerSounds)
        var pickedSD = PICK(selfData.SoundTbl_BecomeEnemyToPlayer);
        if ((pickedSD && Game.Random.NextInt(1, selfData.BecomeEnemyToPlayerChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        var dur = Time.Now + ((((SoundDuration(pickedSD) > 0) && SoundDuration(pickedSD)) || 2) + 1);
        selfData.NextPainSoundT = dur;
        selfData.NextAlertSoundT = dur;
        selfData.NextInvestigateSoundT = Time.Now + 2;
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(2, 3);
        selfData.NextSuppressingSoundT = Time.Now + Game.Random.NextInt(2.5, 4);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.BecomeEnemyToPlayerSoundLevel, GetSoundPitch(selfData.BecomeEnemyToPlayerPitch));


        else if (sdSet == "KilledEnemy")
        if (selfData.HasKilledEnemySounds && Time.Now > selfData.NextKilledEnemySoundT)
        var pickedSD = PICK(selfData.SoundTbl_KilledEnemy);
        if ((pickedSD && Game.Random.NextInt(1, selfData.KilledEnemySoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.NextIdleSoundT = selfData.NextIdleSoundT + 2;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.KilledEnemySoundLevel, GetSoundPitch(selfData.KilledEnemySoundPitch));

        selfData.NextKilledEnemySoundT = Time.Now + Game.Random.NextFloat(selfData.NextSoundTime_KilledEnemy.a, selfData.NextSoundTime_KilledEnemy.b);

        else if (sdSet == "AllyDeath")
        if (selfData.HasKilledEnemySounds && Time.Now > selfData.NextAllyDeathSoundT)
        var pickedSD = PICK(selfData.SoundTbl_AllyDeath);
        if ((pickedSD && Game.Random.NextInt(1, selfData.AllyDeathSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.NextIdleSoundT = selfData.NextIdleSoundT + 2;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.AllyDeathSoundLevel, GetSoundPitch(selfData.AllyDeathSoundPitch));

        selfData.NextAllyDeathSoundT = Time.Now + Game.Random.NextFloat(selfData.NextSoundTime_AllyDeath.a, selfData.NextSoundTime_AllyDeath.b);

        else if (sdSet == "Pain")
        if (selfData.HasPainSounds && Time.Now > selfData.NextPainSoundT)
        var pickedSD = PICK(selfData.SoundTbl_Pain);
        var sdDur = 2;
        if ((pickedSD && Game.Random.NextInt(1, selfData.PainSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.PainSoundLevel, GetSoundPitch(selfData.PainSoundPitch));
        sdDur = (SoundDuration(pickedSD) > 0 && SoundDuration(pickedSD)) || sdDur;

        selfData.NextPainSoundT = Time.Now + sdDur;

        else if (sdSet == "Impact")
        if (selfData.HasImpactSounds)
        var pickedSD = PICK(selfData.SoundTbl_Impact);
        if ((pickedSD && Game.Random.NextInt(1, selfData.ImpactSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        selfData.CurrentImpactSound = (sdType || VJ.EmitSound)(self, pickedSD, selfData.ImpactSoundLevel, GetSoundPitch(selfData.ImpactSoundPitch));


        else if (sdSet == "DamageByPlayer")
        //if (selfData.HasDamageByPlayerSounds && Time.Now > selfData.NextDamageByPlayerSoundT)  // This is done in the call instead
        var pickedSD = PICK(selfData.SoundTbl_DamageByPlayer);
        var sdDur = 2;
        if ((pickedSD && Game.Random.NextInt(1, selfData.DamageByPlayerSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        sdDur = (SoundDuration(pickedSD) > 0 && SoundDuration(pickedSD)) || sdDur;
        selfData.NextPainSoundT = Time.Now + sdDur;
        selfData.IdleSoundBlockTime = Time.Now + sdDur;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.DamageByPlayerSoundLevel, GetSoundPitch(selfData.DamageByPlayerPitch));

        selfData.NextDamageByPlayerSoundT = Time.Now + sdDur;
        //
        else if (sdSet == "Death")
        if (selfData.HasDeathSounds)
        var pickedSD = PICK(selfData.SoundTbl_Death);
        if ((pickedSD && Game.Random.NextInt(1, selfData.DeathSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        (sdType || VJ.EmitSound)(self, pickedSD, selfData.DeathSoundLevel, GetSoundPitch(selfData.DeathSoundPitch));


        else if (sdSet == "Gib")
        if (selfData.HasGibOnDeathSounds)
        sdType = VJ.EmitSound;
        if (customSD)
        sdType(self, customSD, 80, Game.Random.NextInt(80, 100));
        else;
        sdType(self, "vj_base/gib/splat.wav", 80, Game.Random.NextInt(85, 100));
        sdType(self, "vj_base/gib/break1.wav", 80, Game.Random.NextInt(85, 100));
        sdType(self, "vj_base/gib/break2.wav", 80, Game.Random.NextInt(85, 100));
        sdType(self, "vj_base/gib/break3.wav", 80, Game.Random.NextInt(85, 100));


        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-- Creature Base Sound Systems --=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--
        else if (sdSet == "BeforeRangeAttack")
        if (selfData.HasRangeAttackSounds)
        var pickedSD = PICK(selfData.SoundTbl_BeforeRangeAttack);
        if ((pickedSD && Game.Random.NextInt(1, selfData.BeforeRangeAttackSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentExtraSpeechSound);
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentExtraSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.BeforeRangeAttackSoundLevel, GetSoundPitch(selfData.BeforeRangeAttackPitch));


        else if (sdSet == "RangeAttack")
        if (selfData.HasRangeAttackSounds)
        var pickedSD = PICK(selfData.SoundTbl_RangeAttack);
        if ((pickedSD && Game.Random.NextInt(1, selfData.RangeAttackSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.RangeAttackSoundLevel, GetSoundPitch(selfData.RangeAttackPitch));


        else if (sdSet == "BeforeLeapAttack")
        if (selfData.HasBeforeLeapAttackSounds)
        var pickedSD = PICK(selfData.SoundTbl_BeforeLeapAttack);
        if ((pickedSD && Game.Random.NextInt(1, selfData.BeforeLeapAttackSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentExtraSpeechSound);
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentExtraSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.BeforeLeapAttackSoundLevel, GetSoundPitch(selfData.BeforeLeapAttackSoundPitch));


        else if (sdSet == "LeapAttackJump")
        if (selfData.HasLeapAttackJumpSounds)
        var pickedSD = PICK(selfData.SoundTbl_LeapAttackJump);
        if ((pickedSD && Game.Random.NextInt(1, selfData.LeapAttackJumpSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.LeapAttackJumpSoundLevel, GetSoundPitch(selfData.LeapAttackJumpSoundPitch));


        else if (sdSet == "LeapAttackDamage")
        if (selfData.HasLeapAttackDamageSounds)
        var pickedSD = PICK(selfData.SoundTbl_LeapAttackDamage);
        if ((pickedSD && Game.Random.NextInt(1, selfData.LeapAttackDamageSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        StopSD(selfData.CurrentSpeechSound);
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentSpeechSound = (sdType || VJ.EmitSound)(self, pickedSD, selfData.LeapAttackDamageSoundLevel, GetSoundPitch(selfData.LeapAttackDamageSoundPitch));


        else if (sdSet == "LeapAttackDamageMiss")
        if (selfData.HasLeapAttackDamageMissSounds)
        var pickedSD = PICK(selfData.SoundTbl_LeapAttackDamageMiss);
        if ((pickedSD && Game.Random.NextInt(1, selfData.LeapAttackDamageMissSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        selfData.IdleSoundBlockTime = Time.Now + 1;
        selfData.CurrentLeapAttackDamageMissSound = (sdType || VJ.EmitSound)(self, pickedSD, selfData.LeapAttackDamageMissSoundLevel, GetSoundPitch(selfData.LeapAttackDamageMissSoundPitch));


        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-- Human Base Sound Systems --=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--
        else if (sdSet == "Suppressing")
        if (selfData.HasSuppressingSounds && Time.Now > selfData.NextSuppressingSoundT)
        var pickedSD = PICK(selfData.SoundTbl_Suppressing);
        if ((pickedSD && Game.Random.NextInt(1, selfData.SuppressingSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + 2;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.SuppressingSoundLevel, GetSoundPitch(selfData.SuppressingPitch));

        selfData.NextSuppressingSoundT = Time.Now + Game.Random.NextFloat(selfData.NextSoundTime_Suppressing.a, selfData.NextSoundTime_Suppressing.b);

        else if (sdSet == "WeaponReload")
        if (selfData.HasWeaponReloadSounds)
        var pickedSD = PICK(selfData.SoundTbl_WeaponReload);
        if ((pickedSD && Game.Random.NextInt(1, selfData.WeaponReloadSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + ((SoundDuration(pickedSD) > 0 && SoundDuration(pickedSD)) || 3.5);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.WeaponReloadSoundLevel, GetSoundPitch(selfData.WeaponReloadSoundPitch));


        else if (sdSet == "GrenadeAttack")
        if (selfData.HasGrenadeAttackSounds && Time.Now > selfData.NextGrenadeAttackSoundT)
        var pickedSD = PICK(selfData.SoundTbl_GrenadeAttack);
        if ((pickedSD && Game.Random.NextInt(1, selfData.GrenadeAttackSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        if (selfData.IdleSoundsWhileAttacking == false) StopSD(selfData.CurrentIdleSound) end  // Don't stop idle sounds if we aren't suppose to
        selfData.IdleSoundBlockTime = Time.Now + Game.Random.NextInt(3, 4);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.GrenadeAttackSoundLevel, GetSoundPitch(selfData.GrenadeAttackSoundPitch));


        else if (sdSet == "DangerSight" || sdSet == "GrenadeSight")
        if (selfData.HasDangerSightSounds && Time.Now > selfData.NextDangerSightSoundT)
        var pickedSD = PICK(selfData.SoundTbl_DangerSight);
        if (sdSet == "GrenadeSight")
        var grenSDs = PICK(selfData.SoundTbl_GrenadeSight);
        if (grenSDs)
        pickedSD = grenSDs;


        var sdDur = 3;
        if ((pickedSD && Game.Random.NextInt(1, selfData.DangerSightSoundChance) == 1) || customSD)
        if (customSD) pickedSD = customSD 
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        sdDur = (SoundDuration(pickedSD) > 0 && SoundDuration(pickedSD)) || sdDur;
        selfData.IdleSoundBlockTime = Time.Now + sdDur;
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, pickedSD, selfData.DangerSightSoundLevel, GetSoundPitch(selfData.DangerSightSoundPitch));

        selfData.NextDangerSightSoundT = Time.Now + sdDur;

        else  // Such as "Speech"
        if (customSD)
        StopSD(selfData.CurrentSpeechSound);
        StopSD(selfData.CurrentIdleSound);
        selfData.IdleSoundBlockTime = Time.Now + ((((SoundDuration(customSD) > 0) && SoundDuration(customSD)) || 2) + 1);
        selfData.CurrentSpeechSound = (sdType || VJ.CreateSound)(self, customSD, 80, GetSoundPitch(false));



        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.RemoveTimers();
        var myIndex = EntIndex();
        for _, name in this.TimersToRemove do
        timer.Remove(name + myIndex);

        if (this.AttackTimersCustom)  // !!!!!!!!!!!!!! DO NOT USE THIS VARIABLE !!!!!!!!!!!!!! [Backwards Compatibility!]
        for _, name in this.AttackTimersCustom do
        timer.Remove(name + myIndex);



        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Check if (the given entity is in the "this.EntitiesToNoCollide" table, if it's) apply no collide;
        - ent = Entity to check && apply no collide to if it's in the table;
        //---------------------------------------------------------]]
        function ENT.ValidateNoCollide(ent);
        var noCollTbl = this.EntitiesToNoCollide;
        if (noCollTbl && self != ent)
        var entClass = funcGetClass(ent);
        for i = 1, noCollTbl.Count do
        if (noCollTbl[i] == entClass)
        // TODO: The returned logic_collision_pair created here could be removed as it continues working without issues, but I have no idea
        // what kind of side effects it could cause, best to leave as is until further testing or someone with more info can confirm it's safe
        // Alternatively, Facepunch should just directly bind "PhysEnableEntityCollisions" and "PhysDisableEntityCollisions" to Lua, which is
        // what Valve uses for the default NPCs: https://github.com/ValveSoftware/source-sdk-2013/blob/master/src/game/shared/physics_shared.h#L142
        constraint.NoCollide(self, ent, 0, 0);
        // Check for bone followers
        var boneFollowers = ent.GetBoneFollowers();
        if (boneFollowers.Count > 0)
        for _, v in boneFollowers do
        constraint.NoCollide(self, v.follower, 0, 0);


        break;




        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks if the given damage type(s) contains 1 || more of the default gibbing damage types.;
        - dmgType = The damage type(s) to check for;
        EX: dmginfo.GetDamageType();
        Returns;
        - true, At least 1 damage type is included;
        - false, NO damage type is included;
        Notes;
        - DMG_ALWAYSGIB = Skip if it's a bullet because engine sets DMG_ALWAYSGIB for "FireBullets" if it's more than 16 otherwise it sets DMG_NEVERGIB;
        //---------------------------------------------------------]]
        var GIB_DAMAGE_MASK = bit.bor(DMG_ALWAYSGIB, DMG_ENERGYBEAM, DMG_BLAST, DMG_VEHICLE, DMG_CRUSH, DMG_DISSOLVE, DMG_SLOWBURN, DMG_PHYSGUN, DMG_PLASMA, DMG_SONIC);
        //DMG_DIRECT  // Disabled because default fire && intended weapons use it!
        //
        function ENT.IsGibDamage(dmgType);
        return bAND(dmgType, DMG_NEVERGIB) == 0 && bAND(dmgType, GIB_DAMAGE_MASK) != 0 && (bAND(dmgType, DMG_ALWAYSGIB) == 0 || bAND(dmgType, DMG_BULLET) == 0);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.GibOnDeath(dmginfo, hitgroup);
        var selfData = funcGetTable(self);
        if (!selfData.CanGib || !selfData.CanGibOnDeath || selfData.GibbedOnDeath) return false 
        if (!selfData.GibOnDeathFilter || (selfData.GibOnDeathFilter && IsGibDamage(dmginfo.GetDamageType())))
        var gibbed, overrides = HandleGibOnDeath(dmginfo, hitgroup);
        if (gibbed)
        selfData.GibbedOnDeath = true;
        if (overrides)
        if (!overrides.AllowCorpse) selfData.HasDeathCorpse = false 
        if (!overrides.AllowAnim) selfData.HasDeathAnimation = false 
        if (overrides.AllowSound != false) PlaySoundSystem("Gib") end  // null/true = Play gib sound
        else  // Default
        selfData.HasDeathCorpse = false;
        selfData.HasDeathAnimation = false;
        PlaySoundSystem("Gib");

        return true;


        return false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.StartSoundTrack();
        if (!this.HasSounds || !this.HasSoundTrack) return 
        if (Game.Random.NextInt(1, this.SoundTrackChance) == 1)
        this.VJ_SD_PlayingMusic = true;
        net.Start("vj_music_cl");
        net.WriteEntity(self);
        net.WriteString(PICK(this.SoundTbl_SoundTrack));
        net.WriteFloat(this.SoundTrackVolume);
        net.WriteFloat(this.SoundTrackPlaybackRate);
        //net.WriteFloat(this.SoundTrackFadeOutTime)
        net.Broadcast();


        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.CreateDeathLoot(dmginfo, hitgroup);
        if (Game.Random.NextInt(1, this.DeathLootChance) == 1)
        var pickedEnt = PICK(this.DeathLoot);
        if (pickedEnt != false)
        var ent = SceneUtility.CreatePrefab();
        ent.SetPos(Transform.Position + OBBCenter());
        ent.SetAngles(Transform.Rotation);
        ent.Spawn();
        ent.Activate();
        var phys = ent.GetPhysicsObject();
        if (phys.IsValid())
        var dmgForce = (this.SavedDmgInfo.force / 40) + GetMoveVelocity() + Rigidbody.Velocity;
        if (this.DeathAnimationCodeRan)
        dmgForce = GetGroundSpeedVelocity();

        phys.SetMass(1);
        phys.ApplyForceCenter(dmgForce);




        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.OnRemove();
        CustomOnRemove();
        EventSystem.Unsubscribe("Think");
        this.Dead = true;
        if (this.MedicData.Status) ResetMedicBehavior() 
        if (this.VJ_ST_Eating) ResetEatingBehavior("Dead") 
        RemoveTimers();
        SoundManager.StopAll(this);
        StopParticles();
        DestroyBoneFollowers();

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //---- ///// Backwards Compatibility | Do not to use! \\\\\ ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        var dispToVal = {[D_LI] = false, [D_HT] = true, [D_NU] = "Neutral"}
        function ENT.DoRelationshipCheck(ent) return dispToVal[CheckRelationship(ent)];
        function ENT.FaceCertainPosition(target, faceTime) return SetTurnTarget(target, faceTime);
        function ENT.FaceCertainEntity(target, faceCurEnemy, faceTime) return SetTurnTarget(faceCurEnemy && "Enemy" || target, faceTime);
        function ENT.VJ_DoSetEnemy(ent, stopMoving, doQuickIfActiveEnemy) return ForceSetEnemy(ent, stopMoving);
        function ENT.DoChaseAnimation(alwaysChase) MaintainAlertBehavior(alwaysChase);
        function ENT.VJ_TASK_CHASE_ENEMY(doLOSChase) SCHEDULE_ALERT_CHASE(doLOSChase);
        function ENT.VJ_TASK_FACE_X(faceType, customFunc) SCHEDULE_FACE(faceType, customFunc);
        function ENT.VJ_TASK_GOTO_LASTPOS(moveType, customFunc) SCHEDULE_GOTO_POSITION(moveType, customFunc);
        function ENT.VJ_TASK_GOTO_TARGET(moveType, customFunc) SCHEDULE_GOTO_TARGET(moveType, customFunc);
        function ENT.VJ_TASK_COVER_FROM_ENEMY(moveType, customFunc) SCHEDULE_COVER_ENEMY(moveType, customFunc);
        function ENT.VJ_TASK_COVER_FROM_ORIGIN(moveType, customFunc) SCHEDULE_COVER_ORIGIN(moveType, customFunc);
        function ENT.VJ_TASK_IDLE_WANDER() SCHEDULE_IDLE_WANDER();
        function ENT.VJ_TASK_IDLE_STAND() SCHEDULE_IDLE_STAND();
        function ENT.VJ_ACT_PLAYACTIVITY(animation, lockAnim, lockAnimTime, faceEnemy, animDelay, extraOptions, customFunc) return PlayAnim(animation, lockAnim, lockAnimTime, faceEnemy, animDelay, extraOptions, customFunc);
        function ENT.VJ_DecideSoundPitch(pitch1, pitch2) return GetSoundPitch(pitch1);
        function ENT.VJ_GetDifficultyValue(num) return ScaleByDifficulty(num);
        function ENT.VJ_GetNearestPointToEntity(ent, centerNPC) return VJ.GetNearestPositions(self, ent, centerNPC);
        function ENT.VJ_GetNearestPointToEntityDistance(ent, centerNPC) return VJ.GetNearestDistance(self, ent, centerNPC);
        function ENT.BusyWithActivity() return IsBusy("Activities");
        function ENT.IsBusyWithBehavior() return IsBusy("Behaviors");
        function ENT.FootStepSoundCode(customSD) PlayFootstepSound(customSD);
        function ENT.MeleeAttackCode(isPropAttack) ExecuteMeleeAttack(isPropAttack);
        function ENT.RangeAttackCode() ExecuteRangeAttack();
        function ENT.LeapDamageCode() ExecuteLeapAttack();
        function ENT.DecideAnimationLength(anim, override, decrease) return VJ.AnimDurationEx(self, anim, override, decrease);
        function ENT.StopAllCommonSpeechSounds() SoundManager.StopAll(this);
        function ENT.GetFaceAngle(ang) return GetTurnAngle(ang);
        function ENT.DoFlinch(dmginfo, hitgroup) Flinch(dmginfo, hitgroup);
        ENT.LatestEnemyDistance = 0  // Only here to avoid errors;
        ENT.NearestPointToEnemyDistance = 0  // Only here to avoid errors;
        ENT.FootStepPitch = new Vector2(80, 100)  // Only here to avoid errors;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks all 4 sides around the NPC;
        - checkDist = How far should each trace go? | DEFAULT = 200;
        - returnPos = Instead of returning a table of sides, it will return a table of actual positions | DEFAULT: false;
        - Use this whenever possible as it is much more optimized to utilize!;
        - sides = Use this to disable checking certain positions by setting the 1 to 0, "Forward-Backward-Right-Left" | DEFAULT = "1111";
        Returns;
        - When returnPos is true:
        - Table of positions (4 max);
        - When returnPos is false:
        - Table dictionary, includes 4 values, if (true) that side isn't blocked!;
        - Values: Forward, Backward, Right, Left;
        //---------------------------------------------------------]]
        var str1111 = "1111";
        var str1 = "1";
        //
        function ENT.VJ_CheckAllFourSides(checkDist, returnPos, sides);
        checkDist = checkDist || 200;
        sides = sides || str1111;
        var result = returnPos == true && {} || {Forward = false, Backward = false, Right = false, Left = false}
        var i = 0;
        var myPos = Transform.Position;
        var myPosCentered = myPos + OBBCenter();
        var myForward = Transform.Forward;
        var myRight = Transform.Right;
        var positions = {  // Set the positions that we need to check;
        string_sub(sides, 1, 1) == str1 && myForward || 0,;
        string_sub(sides, 2, 2) == str1 && -myForward || 0,;
        string_sub(sides, 3, 3) == str1 && myRight || 0,;
        string_sub(sides, 4, 4) == str1 && -myRight || 0;
        }
        for _, v in positions do
        i = i + 1;
        if (v == 0) continue end  // If 0 then we have the tag to skip this!
        var tr = util.TraceLine({
        start = myPosCentered,;
        endpos = myPosCentered + v*checkDist,;
        filter = self;
        });
        var hitPos = tr.HitPos;
        if (myPos.Distance(hitPos) >= checkDist)
        if (returnPos)
        hitPos.z = myPos.z  // Reset it to Transform.Position z-axis;
        result[result.Count + 1] = hitPos;
        else if (i == 1)
        result.Forward = true;
        else if (i == 2)
        result.Backward = true;
        else if (i == 3)
        result.Right = true;
        else if (i == 4)
        result.Left = true;



        return result;

    }

    public virtual void VJ_CheckAllFourSides(checkDist, returnPos, sides)
    {
        checkDist = checkDist || 200;
        sides = sides || str1111;
        var result = returnPos == true && {} || {Forward = false, Backward = false, Right = false, Left = false}
        var i = 0;
        var myPos = Transform.Position;
        var myPosCentered = myPos + OBBCenter();
        var myForward = Transform.Forward;
        var myRight = Transform.Right;
        var positions = {  // Set the positions that we need to check;
        string_sub(sides, 1, 1) == str1 && myForward || 0,;
        string_sub(sides, 2, 2) == str1 && -myForward || 0,;
        string_sub(sides, 3, 3) == str1 && myRight || 0,;
        string_sub(sides, 4, 4) == str1 && -myRight || 0;
        }
        for _, v in positions do
        i = i + 1;
        if (v == 0) continue end  // If 0 then we have the tag to skip this!
        var tr = util.TraceLine({
        start = myPosCentered,;
        endpos = myPosCentered + v*checkDist,;
        filter = self;
        });
        var hitPos = tr.HitPos;
        if (myPos.Distance(hitPos) >= checkDist)
        if (returnPos)
        hitPos.z = myPos.z  // Reset it to Transform.Position z-axis;
        result[result.Count + 1] = hitPos;
        else if (i == 1)
        result.Forward = true;
        else if (i == 2)
        result.Backward = true;
        else if (i == 3)
        result.Right = true;
        else if (i == 4)
        result.Left = true;



        return result;
    }

}