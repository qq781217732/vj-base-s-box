using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjController : BaseNPC
{
    [Property] public bool VJC_Player_CanExit = true;
    [Property] public bool VJC_Player_CanRespawn = true;
    [Property] public bool VJC_Player_CanChatMessage = true;
    [Property] public bool VJC_Player_DrawHUD = true;
    [Property] public bool VJC_NPC_CanTurn = true;
    [Property] public bool VJC_Bullseye_RefreshPos = true;
    [Property] public bool VJC_BullseyeTracking = false;
    [Property] public object VJC_SavedVars_PLY = null;
    [Property] public object VJC_SavedVars_NPC = null;
    [Property] public int VJC_Camera_Mode = 1;
    [Property] public Vector3 VJC_Camera_CurZoom = Vector(0, 0, 0);
    [Property] public object VJC_Key_Last = BUTTON_CODE_NONE;
    [Property] public int VJC_Key_LastTime = 0;
    [Property] public Vector3 VJC_NPC_LastPos = Vector(0, 0, 0);
    [Property] public bool VJC_Removed = false;
    [Property] public string Type = "anim";
    [Property] public string PrintName = "VJ Base NPC Controller";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public int VJC_Camera_Zoom = 100;

    public virtual void OnInit()
    {
    }

    public virtual void OnThink()
    {
    }

    public virtual void OnKeyPressed(key)
    {
    }

    public virtual void OnKeyBindPressed(key)
    {
    }

    public virtual void OnStopControlling(keyPressed)
    {
    }

    public virtual void OnInitialize()
    {
        // SetMoveType removed: MOVETYPE_NONE
        // SetSolid removed: SOLID_NONE
        Render.CastShadows = false;
        // RenderMode removed: RENDERMODE_NONE  // Disable shadow for dynamic lights
        Init();
        if (this.CustomOnInitialize) CustomOnInitialize() end  // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (this.CustomOnThink) this.OnThink = function() CustomOnThink() end end  // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
    }

    public virtual void UpdateTransmitState()
    {
        return TRANSMIT_ALWAYS  // This entity should always transmit as its client side code is essential!;
    }

    public virtual void SetControlledNPC(npc)
    {
        // Set the bullseye entity values
        var bullseye = SceneUtility.CreatePrefab();
        bullseye.SetPos(npc.GetPos() + npc.GetForward() * 100 + npc.GetUp() * 50) //Vector(npc.OBBMaxs().x + 20, 0, npc.OBBMaxs().z + 20));
        bullseye.SetModel("models/hunter/blocks/cube025x025x025.mdl");
        //bullseye.SetParent(npc)
        bullseye.SetRenderMode(RENDERMODE_NONE);
        bullseye.Spawn();
        bullseye.SetCollisionGroup(COLLISION_GROUP_IN_VEHICLE);
        bullseye.SetNoDraw(false);
        bullseye.DrawShadow(false);
        bullseye.ForceEntAsEnemy = npc;
        bullseye.HandlePerceivedRelationship = function(_, otherEnt, distance, isFriendly);
        if (otherEnt == npc)
        return D_HT;

        return D_NU;

        bullseye.VJ_IsBeingControlled = true;
        DeleteOnRemove(bullseye);
        this.VJCE_Bullseye = bullseye;
        SetBullseye(bullseye);

        // Set the NPC
        if (!npc.ControllerParams)
        npc.ControllerParams = {
        CameraMode = 1,;
        ThirdP_Offset = Vector(0, 0, 0),;
        FirstP_Bone = "ValveBiped.Bip01_Head1",;
        FirstP_Offset = Vector(0, 0, 5),;
        FirstP_ShrinkBone = true,;
        }

        var ply = this.VJCE_Player;
        this.VJC_Camera_Mode = npc.ControllerParams.CameraMode  // Get the NPC's default camera mode;
        this.VJC_NPC_LastPos = npc.GetPos();
        npc.VJ_IsBeingControlled = true;
        npc.VJ_TheController = ply;
        npc.VJ_TheControllerEntity = self;
        npc.VJ_TheControllerBullseye = bullseye;
        npc.SetEnemy(NULL);
        if (npc.IsVJBaseSNPC)
        var funcCustom = npc.Controller_IntMsg; if (funcCustom) funcCustom(npc, ply, self) end  // !!!!!!!!!!!!!! DO NOT USE THIS FUNCTION !!!!!!!!!!!!!! [Backwards Compatibility!];
        npc.Controller_Initialize(ply, self);
        var npcEnemy = npc.GetEnemy();
        if (npcEnemy.IsValid())
        npc.AddEntityRelationship(npcEnemy, D_NU, 10);
        npcEnemy.AddEntityRelationship(npc, D_NU, 10);
        npc.ResetEnemy();
        npc.SetEnemy(bullseye);

        this.VJC_SavedVars_NPC = {
        [1] = npc.DisableWandering,
        [2] = npc.DisableChasingEnemy,
        [3] = npc.DamageResponse,
        [4] = npc.EnemyTouchDetection,
        [5] = npc.CallForHelp,
        [6] = npc.DamageAllyResponse,
        [7] = npc.DeathAllyResponse,
        [8] = npc.FollowPlayer,
        [9] = npc.CanDetectDangers,
        [10] = npc.Passive_RunOnTouch,
        //[11] = npc.Passive_RunOnDamage,
        [12] = npc.IsGuard,
        [13] = npc.CanReceiveOrders,
        [14] = npc.EnemyXRayDetection,
        [15] = npc.GetFOV(),
        [16] = npc.CombatDamageResponse,
        [17] = npc.BecomeEnemyToPlayer,
        [18] = npc.CanEat,
        [19] = npc.LimitChaseDistance,
        [20] = npc.ConstantlyFaceEnemy,
        [21] = npc.IsMedic,
        [22] = npc.EnemyDetection,
        }
        npc.DisableWandering = true;
        npc.DisableChasingEnemy = true;
        npc.DamageResponse = false;
        npc.EnemyTouchDetection = false;
        npc.CallForHelp = false;
        npc.DamageAllyResponse = false;
        npc.DeathAllyResponse = npc.DeathAllyResponse == true && "OnlyAlert" || false;
        npc.FollowPlayer = false;
        npc.CanDetectDangers = false;
        npc.Passive_RunOnTouch = false;
        npc.IsGuard = false;
        npc.CanReceiveOrders = false;
        npc.EnemyXRayDetection = true;
        npc.SetFOV(360);
        npc.CombatDamageResponse = false;
        npc.BecomeEnemyToPlayer = false;
        npc.CanEat = false;
        npc.LimitChaseDistance = false;
        npc.ConstantlyFaceEnemy = false;
        npc.IsMedic = false;
        npc.PauseAttacks = true;
        npc.NextThrowGrenadeT = 0;
        npc.EnemyDetection = false;
        for _, v in npc.RelationshipEnts do
        if (v.IsValid())
        npc.AddEntityRelationship(v, D_NU);


        // Apply a delay to VJ NPCs so they don't attack right away
        if (npc.NextDoAnyAttackT < Time.Now)
        npc.NextDoAnyAttackT = Time.Now + 0.5;

        if (npc.MedicData.Status) npc.ResetMedicBehavior() 
        if (npc.VJ_ST_Eating)
        npc.OnEat("StopEating", "Unspecified")  // So it plays the get up animation;
        npc.ResetEatingBehavior("Unspecified");

        // Apply a small delay to assure that the bullseye is in the NPC's "RelationshipEnts"
        GameTask.DelaySeconds(0.1).ContinueWith(_ => function();
        if (this.IsValid() && npc.IsValid())
        npc.MaintainRelationships();

        end);

        // !!!!!!!!!!!!!! DO NOT USE THESE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (this.CustomOnKeyPressed) this.OnKeyPressed = function(_, key) CustomOnKeyPressed(key) end 
        if (this.CustomOnKeyBindPressed) this.OnKeyBindPressed = function(_, key) CustomOnKeyBindPressed(key) end 
        if (this.CustomOnStopControlling) this.OnStopControlling = function(_, keyPressed) CustomOnStopControlling(keyPressed) end 
        //
        npc.ClearSchedule();
        npc.StopMoving();
        this.VJCE_NPC = npc;
        SetNPC(npc);
        GameTask.DelaySeconds(0).ContinueWith(_ => function()  // This only needs to be 0 seconds because we just need a tick to pass;
        if (this.IsValid() && this.VJCE_NPC.IsValid())
        if (npc.IsVJBaseSNPC)
        this.VJCE_NPC.PauseAttacks = false;
        this.VJCE_NPC.ForceSetEnemy(this.VJCE_Bullseye, false);

        this.VJCE_NPC.SetEnemy(this.VJCE_Bullseye);

        end);
        SendDataToClient();
    }

    public virtual void StartControlling()
    {
        var npc = this.VJCE_NPC;

        // Set up the camera entity
        var camera = SceneUtility.CreatePrefab();
        camera.SetPos(npc.GetPos() + Vector(0, 0, npc.OBBMaxs().z)) //npc.EyePos();
        camera.SetModel("models/props_junk/watermelon01_chunk02c.mdl");
        camera.SetParent(npc);
        camera.SetRenderMode(RENDERMODE_NONE);
        camera.Spawn();
        camera.SetNoDraw(false);
        camera.DrawShadow(false);
        DeleteOnRemove(camera);
        this.VJCE_Camera = camera;
        SetCamera(camera);

        // Set up the player
        var ply = this.VJCE_Player;
        SetPlayer(ply);
        ply.VJ_IsControllingNPC = true;
        ply.VJ_TheControllerEntity = self;
        ply.Spectate(OBS_MODE_CHASE);
        ply.SpectateEntity(camera);
        ply.DrawShadow(false);
        ply.SetNoDraw(true);
        ply.SetMoveType(MOVETYPE_OBSERVER);
        ply.DrawViewModel(false);
        ply.DrawWorldModel(false);
        var weps = {}
        for _, v in ply.GetWeapons() do
        weps[weps.Count + 1] = v.GetClass();

        this.VJC_SavedVars_PLY = {
        health = ply.Health(),;
        armor = ply.Armor(),;
        weapons = weps,;
        activeWep = (ply.GetActiveWeapon(.IsValid()) && ply.GetActiveWeapon():GetClass()) || "",;
        godMode = ply.HasGodMode(),  // Maintain the player's God mode status after exiting the controller;
        noTarget = ply.IsFlagSet(FL_NOTARGET);
        }
        ply.SetNoTarget(true);
        ply.StripWeapons();
        if (ply.GetInfoNum("vj_npc_cont_diewithnpc", 0) == 1) this.VJC_Player_CanRespawn = false 

        EventSystem.Subscribe("PlayerButtonDown", function(ent, ply2, button);
        if (ent.IsValid() && ply2.VJ_IsControllingNPC && ent.VJCE_Player == ply2)
        ent.VJC_Key_Last = button;
        ent.VJC_Key_LastTime = Time.Now;
        ent.OnKeyPressed(button);

        // Stop Controlling
        if (ent.VJC_Player_CanExit && button == KEY_END)
        ent.StopControlling(true);


        // Tracking
        if (button == KEY_T)
        ent.ToggleBullseyeTracking();


        // Camera mode
        if (button == KEY_H)
        ent.VJC_Camera_Mode = (ent.VJC_Camera_Mode == 1 && 2) || 1;


        // Allow movement jumping
        if (button == KEY_J)
        ent.ToggleMovementJumping();


        // Zoom
        var zoom = ply2.GetInfoNum("vj_npc_cont_cam_zoom_dist", 5);
        if (button == KEY_LEFT)
        ent.VJC_Camera_CurZoom = ent.VJC_Camera_CurZoom - Vector(0, zoom, 0);
        else if (button == KEY_RIGHT)
        ent.VJC_Camera_CurZoom = ent.VJC_Camera_CurZoom + Vector(0, zoom, 0);
        else if (button == KEY_UP)
        ent.VJC_Camera_CurZoom = ent.VJC_Camera_CurZoom + (ply2.KeyDown(IN_SPEED) && Vector(0, 0, zoom) || Vector(zoom, 0, 0));
        else if (button == KEY_DOWN)
        ent.VJC_Camera_CurZoom = ent.VJC_Camera_CurZoom - (ply2.KeyDown(IN_SPEED) && Vector(0, 0, zoom) || Vector(zoom, 0, 0));

        if (button == KEY_BACKSPACE)
        ent.VJC_Camera_CurZoom = vecDef;


        end);

        EventSystem.Subscribe("KeyPress", function(ent, ply2, key);
        //print(key)
        if (ent.IsValid() && ply2.VJ_IsControllingNPC && ent.VJCE_Player == ply2)
        ent.OnKeyBindPressed(key);

        end);
    }

    public virtual void SendDataToClient(reset)
    {
        var npc = this.VJCE_NPC;
        var npcData = npc.ControllerParams;
        SetHUDEnabled(this.VJC_Player_DrawHUD);
        SetCameraMode((reset && 1) || this.VJC_Camera_Mode);
        SetCameraTP_Offset((reset && vecDef) || (npcData.ThirdP_Offset + this.VJC_Camera_CurZoom));
        SetCameraFP_Offset((reset && vecDef) || npcData.FirstP_Offset);
        if (npc.IsValid())
        SetCameraFP_Bone(npc.LookupBone(npcData.FirstP_Bone) || -1);

        SetCameraFP_ShrinkBone((reset != true && npcData.FirstP_ShrinkBone) || false);
        SetCameraFP_BoneAng((reset != true && npcData.FirstP_CameraBoneAng) || 0);
        SetCameraFP_BoneAngOffset((reset != true && npcData.FirstP_CameraBoneAng_Offset) || 0);

        if (!reset && npc.IsValid())
        SetNPCName(npc.GetName());
        SetNPCAttackMelee(npc.HasMeleeAttack && (((npc.IsAbleToMeleeAttack != true || npc.AttackType == VJ.ATTACK_TYPE_MELEE) && 2) || 1) || 0);
        SetNPCRangeAttack(npc.HasRangeAttack && (((npc.IsAbleToRangeAttack != true || npc.AttackType == VJ.ATTACK_TYPE_RANGE) && 2) || 1) || 0);
        SetNPCLeapAttack(npc.HasLeapAttack && (((npc.IsAbleToLeapAttack != true || npc.AttackType == VJ.ATTACK_TYPE_LEAP) && 2) || 1) || 0);
        SetNPCGrenadeAttack(npc.HasGrenadeAttack && ((Time.Now <= npc.NextThrowGrenadeT && 2) || 1) || 0);
        var npcWeapon = npc.GetActiveWeapon();
        if (npcWeapon.IsValid())
        SetNPCWeapon(npcWeapon);
        SetNPCWeaponAmmo(npcWeapon.IsValid() && npcWeapon.Clip1() || 0);
        else;
        SetNPCWeapon(NULL);
        SetNPCWeaponAmmo(0);


    }

    public virtual void Think()
    {
        var ply = this.VJCE_Player;
        var npc = this.VJCE_NPC;
        var camera = this.VJCE_Camera;
        var bullseye = this.VJCE_Bullseye;
        if (!ply.IsValid() || !npc.IsValid() || !camera.IsValid() || !bullseye.IsValid() || !ply.VJ_IsControllingNPC || !ply.Alive() || !npc.Alive()) StopControlling() return 
        var curTime = Time.Now;
        var npcWeapon = npc.GetActiveWeapon();
        var npcEnemy = npc.GetEnemy();
        var bullseyePos = bullseye.GetPos();

        // Keep bullseye as the enemy
        if (npcEnemy != bullseye)
        if (npc.IsVJBaseSNPC)
        npc.ResetEnemy();
        npc.ForceSetEnemy(bullseye, false);

        npc.AddEntityRelationship(bullseye, D_HT, 99);
        npc.SetEnemy(bullseye);


        this.VJC_NPC_LastPos = npc.GetPos();
        ply.SetPos(this.VJC_NPC_LastPos + vecZ20)  // Set player's location;
        if (ply.Count.GetWeapons() > 0) ply.StripWeapons() 
        SendDataToClient();

        // Debug
        if (ply.GetInfoNum("vj_npc_cont_debug", 0) == 1)
        DebugOverlay.Box(ply.GetPos(), Vector(-2, -2, -2), Vector(2, 2, 2), 1, VJ.COLOR_BLUE);
        DebugOverlay.Text(ply.GetPos(), "Player", 1, false);
        DebugOverlay.Box(camera.GetPos(), Vector(-2, -2, -2), Vector(2, 2, 2), 1, VJ.COLOR_CYAN);
        DebugOverlay.Text(camera.GetPos(), "Camera", 1, false);
        DebugOverlay.Box(bullseyePos, Vector(-2, -2, -2), Vector(2, 2, 2), 1, VJ.COLOR_RED);
        DebugOverlay.Text(bullseyePos, "Bullseye", 1, false);


        OnThink();

        var canTurn = true;
        if (npc.Flinching || (((npc.CurrentSchedule && !npc.CurrentSchedule.IsPlayActivity) || !npc.CurrentSchedule) && npc.GetNavType() == NAV_JUMP)) return 

        // NPC Weapon attack
        if (npc.IsVJBaseSNPC_Human)
        if (npcWeapon.IsValid() && !npc.IsMoving() && npcWeapon.IsVJBaseWeapon && ply.KeyDown(IN_ATTACK2) && !npc.AttackType && !npc.PauseAttacks && npc.GetWeaponState() == VJ.WEP_STATE_READY)
        //npc.SetAngles(Angle(0, math.ApproachAngle(npc.GetAngles().y, ply.GetAimVector():Angle().y, 100), 0))
        npc.SetTurnTarget(bullseyePos, 0.2);
        canTurn = false;
        if (npcWeapon.IsMeleeWeapon)
        if (curTime > npc.NextMeleeWeaponAttackT)
        npc.OnWeaponAttack();
        var anim = npc.TranslateActivity(Game.Random.FromList(npc.AnimTbl_WeaponAttack));
        var animDur = AnimationHelper.Duration(npc, anim);
        npc.NextMeleeWeaponAttackT = curTime + animDur;
        npc.WeaponAttackAnim = anim;
        npc.PlayAnim(anim, "LetAttacks", false, false);
        npc.WeaponAttackState = VJ.WEP_ATTACK_STATE_FIRE_STAND;
        npcWeapon.NPC_NextPrimaryFire = animDur  // Make melee weapons dynamically change the next primary fire;
        npcWeapon.NPCShoot_Primary();

        else if (!AnimationHelper.IsPlaying(npc, npc.TranslateActivity(npc.WeaponAttackAnim)) && !AnimationHelper.IsPlaying(npc, npc.AnimTbl_WeaponAttack))
        npc.OnWeaponAttack();
        var anim = npc.TranslateActivity(Game.Random.FromList(npc.AnimTbl_WeaponAttack));
        npc.WeaponAttackAnim = anim;
        npc.PlayAnim(anim, false, 2, false);
        npc.WeaponAttackState = VJ.WEP_ATTACK_STATE_FIRE_STAND;


        if (!ply.KeyDown(IN_ATTACK2))
        npc.WeaponAttackState = VJ.WEP_ATTACK_STATE_NONE;



        if (npc.IsVJBaseSNPC && npc.AttackAnimTime < curTime && curTime > npc.NextChaseTime && !npc.IsVJBaseSNPC_Tank)
        // NPC Turning
        if (!npc.IsMoving() && canTurn && npc.MovementType != VJ_MOVETYPE_PHYSICS && ((npc.IsVJBaseSNPC_Human && npc.GetWeaponState() != VJ.WEP_STATE_RELOADING) || (!npc.IsVJBaseSNPC_Human)))
        npc.SCHEDULE_IDLE_STAND();
        if (this.VJC_NPC_CanTurn)
        var turnData = npc.TurnData;
        if (turnData.Target != bullseye)
        npc.SetTurnTarget(bullseye, 1);
        else if (npc.GetActivity() == ACT_IDLE && npc.GetIdealActivity() == ACT_IDLE && npc.DeltaIdealYaw() <= -45 || npc.DeltaIdealYaw() >= 45)  // Check both current act AND ideal act because certain activities only change the current act (Ex: UpdateTurnActivity function)
        npc.UpdateTurnActivity();
        if (npc.GetIdealActivity() != ACT_IDLE)  // If ideal act is no longer idle, then we have selected a turn activity!
        npc.NextIdleTime = curTime + VJ.AnimDurationEx(npc, npc.GetIdealActivity());



        //this.TestLerp = npc.GetAngles().y
        //npc.SetAngles(Angle(0, Lerp(100*FrameTime(), this.TestLerp, ply.GetAimVector():Angle().y), 0))


        // NPC Movement
        npc.Controller_Movement(self, ply, bullseyePos);

        NextThink(curTime);
    }

    public virtual void StartMovement(Dir, Rot)
    {
        var npc = this.VJCE_NPC;
        var ply = this.VJCE_Player;
        if (npc.GetState() != VJ_STATE_NONE) return 

        var DEBUG = ply.GetInfoNum("vj_npc_cont_debug", 0) == 1;
        var plyAimVec = Dir;
        plyAimVec.z = 0;
        plyAimVec.Rotate(Rot);
        var selfPos = npc.GetPos();
        var centerToPos = npc.OBBCenter():Distance(npc.OBBMins()) + 20 // npc.OBBMaxs().z;
        var NPCPos = selfPos + npc.GetUp()*centerToPos;
        var groundSpeed = math_min(math_max(npc.GetSequenceGroundSpeed(npc.GetSequence()), 300), 9999);
        var defaultFilter = {self, npc, ply}
        var forwardTr = SceneTrace.Ray({start = NPCPos, endpos = NPCPos + plyAimVec * groundSpeed, filter = defaultFilter}).Run();
        var forwardDist = NPCPos.Distance(forwardTr.HitPos);
        var wallToSelf = forwardDist - (npc.OBBMaxs().y)  // Use Y instead of X because X is left/right whereas Y is forward/backward;
        if (DEBUG)
        DebugOverlay.Box(NPCPos, Vector(-2, -2, -2), Vector(2, 2, 2), 3, VJ.COLOR_BLUE_SKY)  // NPC's calculated position;
        DebugOverlay.Text(NPCPos, "NPCPos", 3, false);
        DebugOverlay.Box(forwardTr.HitPos, Vector(-2, -2, -2), Vector(2, 2, 2), 3, VJ.COLOR_YELLOW)  // forward trace position;
        DebugOverlay.Text(forwardTr.HitPos, "forwardTr.HitPos", 3, false);

        if (forwardDist >= 25)
        var finalPos = Vector((selfPos + plyAimVec * wallToSelf).x, (selfPos + plyAimVec * wallToSelf).y, forwardTr.HitPos.z);
        // Check if ground is valid!
        var downTr = SceneTrace.Ray({start = finalPos, endpos = finalPos + Transform.Up*-(200 + centerToPos), filter = defaultFilter}).Run();
        var downDist = (finalPos.z - centerToPos) - downTr.HitPos.z;
        if (downDist >= 150)  // If the drop is this big, then don't move!
        //wallToSelf = wallToSelf - downDist  // No need, we are returning anyway
        return;

        if (DEBUG)
        DebugOverlay.Box(downTr.HitPos, Vector(-2, -2, -2), Vector(2, 2, 2), 3, VJ.COLOR_PINK)  // Down trace position;
        DebugOverlay.Text(downTr.HitPos, "downTr.HitPos", 3, false);
        DebugOverlay.Box(finalPos, Vector(-2, -2, -2), Vector(2, 2, 2), 3, VJ.COLOR_PURPLE)  // Final move position;
        DebugOverlay.Text(finalPos, "finalPos", 3, false);

        npc.SetLastPosition(finalPos);
        npc.SCHEDULE_GOTO_POSITION(ply:KeyDown(IN_SPEED) && "TASK_RUN_PATH" || "TASK_WALK_PATH", function(x);
        // Since are constantly setting the schedule, we need to manually update the movement activity every time to avoid stuttering between walk/run
        npc.SetMovementActivity(ply:KeyDown(IN_SPEED) && ACT_RUN || ACT_WALK);
        if (ply.KeyDown(IN_ATTACK2) && npc.IsVJBaseSNPC_Human)
        x.TurnData = {Type = VJ.FACE_ENEMY}
        x.CanShootWhenMoving = true;
        else;
        if (this.VJC_BullseyeTracking)
        x.TurnData = {Type = VJ.FACE_ENEMY}
        else;
        npc.ResetTurnTarget();
        x.EngTask("TASK_FACE_LASTPOSITION", 0);


        end);

    }

    public virtual void ToggleBullseyeTracking()
    {
        if (!this.VJC_BullseyeTracking)
        if (this.VJC_Player_CanChatMessage) this.VJCE_Player.ChatPrint("vjbase.Count.controller.print.tracking.activated") 
        this.VJC_BullseyeTracking = true;
        else;
        if (this.VJC_Player_CanChatMessage) this.VJCE_Player.ChatPrint("vjbase.Count.controller.print.tracking.deactivated") 
        this.VJC_BullseyeTracking = false;

    }

    public virtual void ToggleMovementJumping()
    {
        if (!this.VJCE_NPC.JumpParams.Enabled)
        if (this.VJC_Player_CanChatMessage) this.VJCE_Player.ChatPrint("vjbase.Count.controller.print.jump.enable") 
        this.VJCE_NPC.JumpParams.Enabled = true;
        else;
        if (this.VJC_Player_CanChatMessage) this.VJCE_Player.ChatPrint("vjbase.Count.controller.print.jump.disable") 
        this.VJCE_NPC.JumpParams.Enabled = false;

    }

    public virtual void StopControlling(keyPressed)
    {
        //if (!this.VJCE_Player.IsValid()) return GameObject.Destroy() 
        keyPressed = keyPressed || false;

        var npc = this.VJCE_NPC;
        var ply = this.VJCE_Player;
        if (ply.IsValid())
        var plyData = this.VJC_SavedVars_PLY;
        ply.UnSpectate();
        ply.KillSilent()  // If we don't, we will get bugs like !being able to pick up weapons when walking over them;
        if (this.VJC_Player_CanRespawn || keyPressed)
        ply.Spawn();
        ply.SetHealth(plyData.health);
        ply.SetArmor(plyData.armor);
        for _, v in plyData.weapons do
        ply.Give(v);

        ply.SelectWeapon(plyData.activeWep);
        if (plyData.godMode)
        ply.GodEnable();


        if (npc.IsValid())
        ply.SetPos(npc.GetPos() + npc.OBBMaxs() + vecZ20);
        else;
        ply.SetPos(this.VJC_NPC_LastPos);


        ply.SetNoDraw(false);
        ply.DrawShadow(true);
        ply.SetNoTarget(plyData.noTarget);
        //ply.Spectate(OBS_MODE_NONE)
        ply.DrawViewModel(true);
        ply.DrawWorldModel(true);
        //ply.SetMoveType(MOVETYPE_WALK)
        ply.VJ_IsControllingNPC = false;
        ply.VJ_TheControllerEntity = NULL;
        SendDataToClient(true);

        this.VJCE_Player = NULL;

        if (npc.IsValid())
        var npcData = this.VJC_SavedVars_NPC;
        //npc.StopMoving()
        npc.VJ_IsBeingControlled = false;
        npc.VJ_TheController = NULL;
        npc.VJ_TheControllerEntity = NULL;
        //npc.ClearSchedule()
        if (npc.IsVJBaseSNPC)
        npc.DisableWandering = npcData[1];
        npc.DisableChasingEnemy = npcData[2];
        npc.DamageResponse = npcData[3];
        npc.EnemyTouchDetection = npcData[4];
        npc.CallForHelp = npcData[5];
        npc.DamageAllyResponse = npcData[6];
        npc.DeathAllyResponse = npcData[7];
        npc.FollowPlayer = npcData[8];
        npc.CanDetectDangers = npcData[9];
        npc.Passive_RunOnTouch = npcData[10];
        //npc.Passive_RunOnDamage = npcData[11]
        npc.IsGuard = npcData[12];
        npc.CanReceiveOrders = npcData[13];
        npc.EnemyXRayDetection = npcData[14];
        npc.SetFOV(npcData[15]);
        npc.CombatDamageResponse = npcData[16];
        npc.BecomeEnemyToPlayer = npcData[17];
        npc.CanEat = npcData[18];
        npc.LimitChaseDistance = npcData[19];
        npc.ConstantlyFaceEnemy = npcData[20];
        npc.IsMedic = npcData[21];
        npc.EnemyDetection = npcData[22];


        OnStopControlling(keyPressed);
        //this.VJCE_Camera.Remove()
        this.VJC_Removed = true;
        GameObject.Destroy();
    }

    public virtual void OnRemove()
    {
        if (!this.VJC_Removed)
        StopControlling();

    }

    public virtual void SetupDataTables()
    {
        // Entities
        [Net] "Entity" "Bullseye"
        [Net] "Entity" "Camera"
        [Net] "Entity" "Player"
        [Net] "Entity" "NPC"

        // Camera values
        [Net] "Int" "CameraMode"
        [Net] "Vector" "CameraTP_Offset"
        [Net] "Vector" "CameraFP_Offset"
        [Net] "Int" "CameraFP_Bone"
        [Net] "Bool" "CameraFP_ShrinkBone"
        [Net] "Int" "CameraFP_BoneAng"
        [Net] "Int" "CameraFP_BoneAngOffset"

        // NPC values
        [Net] "String" "NPCName"
        [Net] "Int" "NPCAttackMelee"
        [Net] "Int" "NPCRangeAttack"
        [Net] "Int" "NPCLeapAttack"
        [Net] "Int" "NPCGrenadeAttack"
        [Net] "Entity" "NPCWeapon"
        [Net] "Int" "NPCWeaponAmmo"

        // HUD values
        [Net] "Bool" "HUDEnabled"
    }

    public virtual void OnDraw()
    {
        return false;
    }

    public virtual void OnInitialize()
    {
        EventSystem.Subscribe("CalcView", this.CalcView);
        EventSystem.Subscribe("PlayerBindPress", this.PlayerBindPress);
        EventSystem.Subscribe("HUDPaint", this.HUD);

        var ply = GetPlayer();
        if (ply.IsValid())
        ply.VJ_IsControllingNPC = true;

    }

    public virtual void OnRemove()
    {
        var ply = GetPlayer();
        if (ply.IsValid())
        ply.VJ_IsControllingNPC = false;

        // Reset the NPC's bone manipulation!
        var npc = GetNPC();
        if (npc.IsValid())
        npc.ManipulateBoneScale(GetCameraFP_Bone(), vec1);

    }

    public virtual void CalcView(ply, origin, angles, fov)
    {
        if (!this.IsValid() || GetPlayer() != ply) return 
        var camera = GetCamera();
        var npc = GetNPC();
        if (!camera.IsValid() || !npc.IsValid()) return 
        var viewEnt = ply.GetViewEntity();
        if (viewEnt.IsValid() && viewEnt.GetClass() == "gmod_cameraprop") return 
        var cameraMode = GetCameraMode();
        var customData = npc.Controller_OnCalcView && npc.Controller_OnCalcView(self, ply, origin, angles, fov) || false;
        // !!!!!!!!!!!!!! DO NOT USE THESE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (npc.Controller_CalcView)
        ply.VJCE_Camera = camera;
        ply.VJCE_Camera.Zoom = this.VJC_Camera_Zoom;
        ply.VJCE_NPC = GetNPC();
        ply.VJC_Camera_Mode = GetCameraMode();
        ply.VJC_TP_Offset = GetCameraTP_Offset();
        ply.VJC_FP_Offset = GetCameraFP_Offset();
        ply.VJC_FP_Bone = GetCameraFP_Bone();
        ply.VJC_FP_ShrinkBone = GetCameraFP_ShrinkBone();
        ply.VJC_FP_CameraBoneAng = GetCameraFP_BoneAng();
        ply.VJC_FP_CameraBoneAng_Offset = GetCameraFP_BoneAngOffset();
        var oldFunc = npc.Controller_CalcView(ply, origin, angles, fov, camera, cameraMode);
        if (oldFunc)
        customData = oldFunc;


        //
        var lerpSpeed = ply.GetInfoNum("vj_npc_cont_cam_speed", 6);
        var pos = origin  // The position that will be set;
        var ang = ply.EyeAngles();

        // MODE: Custom
        if (customData)
        if (istable(customData))
        pos = customData.origin || origin;
        ang = customData.angles || angles;
        fov = customData.fov || fov;
        lerpSpeed = customData.speed || lerpSpeed;

        // MODE: First person
        else if (cameraMode == 2)
        var setPos = npc.EyePos() + npc.GetForward() * 20;
        var offset = GetCameraFP_Offset();
        //camera.SetLocalPos(camera.GetLocalPos() + GetCameraTP_Offset())  // Help keep the camera stable
        if (GetCameraFP_Bone() != -1)  // If the bone does exist, then use the bone position
        var bonePos, boneAng = npc.GetBonePosition(GetCameraFP_Bone());
        setPos = bonePos;
        if (GetCameraFP_BoneAng() > 0)
        ang[3] = boneAng[GetCameraFP_BoneAng()] + GetCameraFP_BoneAngOffset();

        if (GetCameraFP_ShrinkBone())
        npc.ManipulateBoneScale(GetCameraFP_Bone(), vec0)  // Bone manipulate to make it easier to see;


        pos = setPos + (npc.GetForward() * offset.x + npc.GetRight() * offset.y + npc.GetUp() * offset.z);
        // MODE: Third person
        else;
        if (GetCameraFP_Bone() != -1)  // Reset the NPC's bone manipulation
        npc.ManipulateBoneScale(GetCameraFP_Bone(), vec1);

        var offset = GetCameraTP_Offset() + Vector(0, 0, npc.OBBMaxs().z - npc.OBBMins().z);
        //camera.SetLocalPos(camera.GetLocalPos() + GetCameraTP_Offset())  // Help keep the camera stable
        var tr = util.TraceHull({
        start = npc.GetPos() + npc.OBBCenter(),;
        endpos = npc.GetPos() + npc.OBBCenter() + angles.Forward() * -this.VJC_Camera_Zoom + (npc.GetForward() * offset.x + npc.GetRight() * offset.y + npc.GetUp() * offset.z),;
        filter = {ply, camera, npc},;
        mins = Vector(-5, -5, -5),;
        maxs = Vector(5, 5, 5),;
        mask = MASK_BLOCKLOS,;
        });
        pos = tr.HitPos + tr.HitNormal * 2;


        // Lerp the position and the angle
        viewLerpVec = (cameraMode == 2 && pos) || (lerpSpeed != 0 && LerpVector(FrameTime() * lerpSpeed, viewLerpVec, pos) || pos);
        viewLerpAng = (lerpSpeed != 0 && LerpAngle(FrameTime() * lerpSpeed, viewLerpAng, ang) || ang);

        // Send the player's hit position to the controller entity
        var tr = util.TraceLine({
        start = viewLerpVec,;
        endpos = viewLerpVec + viewLerpAng.Forward() * 32768,;
        filter = function(ent) //{ply, camera, npc}
        if (ent == ply || ent == camera || ent == npc) return false 
        if (ent.GetClass() == "phys_bone_follower" && ent.GetOwner() == npc) return false 
        return true;
        end,;
        });
        //Particles.Play("vj_impact_dirty", tr.HitPos, Angle(0, 0, 0), npc)
        net.Start("vj_controller_sv");
        net.WriteVector(tr.HitPos);
        net.SendToServer();

        return {
        origin = viewLerpVec,;
        angles = viewLerpAng,;
        fov = fov,;
        drawviewer = false, //(cameraMode == 2 && true) || false;
        }
    }

    public virtual void PlayerBindPress(ply, bind, pressed)
    {
        // Scroll wheel zooming
        if (this.IsValid() && GetPlayer() == ply && (bind == "invprev" || bind == "invnext") && GetCamera(.IsValid()) && GetCameraMode() != 2)
        if (bind == "invprev")
        this.VJC_Camera_Zoom = math_clamp(this.VJC_Camera_Zoom - ply.GetInfoNum("vj_npc_cont_cam_zoom_speed", 10), 0, 500);
        else;
        this.VJC_Camera_Zoom = math_clamp(this.VJC_Camera_Zoom + ply.GetInfoNum("vj_npc_cont_cam_zoom_speed", 10), 0, 500);


    }

    public virtual void HUD()
    {
        var ply = LocalPlayer();
        if (!this.IsValid() || GetPlayer() != ply) return 
        if (!GetHUDEnabled() || ply.GetInfoNum("vj_npc_cont_hud", 1) == 0) return 
        var npc = GetNPC();
        if (!npc.IsValid()) return 
        var srcW, srcH = ScrW(), ScrH();
        var health = npc.Health();
        var healthMax = npc.GetMaxHealth();
        var atkMelee = GetNPCAttackMelee();
        var atkRange = GetNPCRangeAttack();
        var atkLeap = GetNPCLeapAttack();
        var atkGrenade = GetNPCGrenadeAttack();
        var atkWeapon = GetNPCWeapon(.IsValid());
        var atkWeaponAmmo = GetNPCWeaponAmmo();

        draw_RoundedBox(box_roundness, srcW / 2.24, srcH - 130, 215, 100, color_box);
        draw_SimpleText(GetNPCName(), "VJBaseSmallMedium", srcW / 2.21, srcH - 125, color_white, 0, 0);

        // Health
        lerp_hp = Lerp(8 * FrameTime(), lerp_hp, health);
        draw_RoundedBox(box_roundness, srcW / 2.21, srcH - 105, 190, 20, color_cyan_muted);
        draw_RoundedBox(box_roundness, srcW / 2.21 + box_border_thickness, srcH - 105 + box_border_thickness, 190 - box_border_thickness * 2, 20 - box_border_thickness * 2, color_box_under);
        draw_RoundedBox(box_roundness, srcW / 2.21 + box_border_thickness, srcH - 105 + box_border_thickness, ((190 * math_clamp(lerp_hp, 0, healthMax)) / healthMax) - box_border_thickness * 2, 20 - box_border_thickness * 2, color_cyan_muted);
        draw_SimpleText(string.format("%.0f",  lerp_hp) + "/" + healthMax,  "VJBaseSmallMedium", (srcW / 1.99) - ((surface.GetTextSize(health + "/" + healthMax)) / 2), srcH - 103, color_white);

        // Attack Icons
        surface_SetMaterial(mat_icon_melee);
        surface_SetDrawColor(attack_icon_color[atkMelee] || color_green);
        surface_DrawTexturedRect(srcW / 2.21, srcH - 83, 28, 28);

        surface_SetMaterial(mat_icon_range);
        surface_SetDrawColor(attack_icon_color[atkRange] || color_green);
        surface_DrawTexturedRect(srcW / 2.14, srcH - 83, 28, 28);

        surface_SetMaterial(mat_icon_leap);
        surface_SetDrawColor(attack_icon_color[atkLeap] || color_green);
        surface_DrawTexturedRect(srcW / 2.065, srcH - 83, 28, 28);

        surface_SetMaterial(mat_icon_grenade);
        surface_SetDrawColor(attack_icon_color[atkGrenade] || color_green);
        surface_DrawTexturedRect(srcW / 2.005, srcH - 83, 28, 28);

        surface_SetMaterial(mat_icon_gun);
        surface_SetDrawColor((!atkWeapon && color_red) || ((atkWeaponAmmo <= 0 && color_orange) || color_green));
        surface_DrawTexturedRect(srcW / 1.94, srcH - 83, 28, 28);
        if (atkWeapon)
        draw_SimpleText(atkWeaponAmmo, "VJBaseMedium", srcW / 1.885, srcH - 80, (atkWeaponAmmo <= 0 && color_orange) || color_green, 0, 0);


        // Camera Mode
        surface_SetMaterial(mat_icon_camera);
        surface_SetDrawColor(color_white);
        surface_DrawTexturedRect(srcW / 2.21, srcH - 55, 22, 22);
        draw_SimpleText((GetCameraMode() == 1 && "Third") || "First", "VJBaseMedium", srcW / 2.14, srcH - 55, color_white, 0, 0);

        // Camera Zoom
        surface_SetMaterial(mat_icon_zoom);
        surface_SetDrawColor(color_white);
        surface_DrawTexturedRect(srcW / 1.94, srcH - 55, 22, 22);
        draw_SimpleText(this.VJC_Camera_Zoom, "VJBaseMedium", srcW / 1.885, srcH - 55, color_white, 0, 0);
    }

}