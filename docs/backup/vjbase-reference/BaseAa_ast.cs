using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class BaseAa : BaseNPC
{
    [Property] public int AA_NextMoveAnimTime = 0;
    [Property] public bool AA_CurrentMoveAnim = false;
    [Property] public string AA_CurrentMoveAnimType = "Calm";
    [Property] public int AA_CurrentMoveMaxSpeed = 0;
    [Property] public int AA_CurrentMoveTime = 0;
    [Property] public int AA_CurrentMoveType = 0;
    [Property] public object AA_CurrentMovePos = null;
    [Property] public object AA_CurrentMovePosDir = null;
    [Property] public int AA_CurrentMoveDist = -1;
    [Property] public object AA_LastChasePos = null;
    [Property] public bool AA_DoingLastChasePos = false;

    public virtual void AA_StopMoving()
    {
        if (Rigidbody.Velocity:Length() > 0)
        var selfData = funcGetTable(self);
        selfData.AA_CurrentMoveMaxSpeed = 0;
        selfData.AA_CurrentMoveTime = 0;
        selfData.AA_CurrentMoveType = 0;
        selfData.AA_CurrentMovePos = null;
        selfData.AA_CurrentMovePosDir = null;
        selfData.AA_CurrentMoveDist = -1;
        self:SetLocalVelocity(defPos);
    }

    public virtual void AA_MoveTo(dest, playAnim, moveType, extraOptions)
    {
        var destVec = isvector(dest) && dest;
        var selfData = funcGetTable(self);
        if (selfData.Dead || (! destVec && ! dest.IsValid())) return 
        moveType = moveType || "Calm" -- "Calm" | "Alert";
        extraOptions = extraOptions || {}
        var addPos = extraOptions.AddPos || defPos -- This will be added to the given entity's position;
        var chaseEnemy = extraOptions.ChaseEnemy || false -- Used internally by ChaseEnemy, enables code that's used only for that;
        var moveSpeed = (moveType == "Calm" && selfData.Aerial_FlyingSpeed_Calm) || selfData.Aerial_FlyingSpeed_Alerted;
        var debug = selfData.VJ_DEBUG;
        var myPos = Transform.Position;
        var trFilter = {self, isentity(dest) && dest || NULL, "phys_bone_follower"} -- Pass in NULL when "dest" is ! an entity otherwise filter will ignore everything after it!;

        // Initial checks for aquatic NPCs
        if (selfData.MovementType == VJ_MOVETYPE_AQUATIC)
        moveSpeed = (moveType == "Calm" && selfData.Aquatic_SwimmingSpeed_Calm) || selfData.Aquatic_SwimmingSpeed_Alerted;
        if (debug)
        print("----------------");
        print("[MoveTo] My WaterLevel: " + self:WaterLevel());
        if (! destVec) print("[MoveTo] dest WaterLevel: " + dest:WaterLevel()) 

        // NPC ! fully in water, so forget the destination, instead wander OR go deeper into the war
        if (self:WaterLevel() <= 2) self:MaintainIdleBehavior(1) return 
        // If the destination is a vector then make sure it's in the water
        if (destVec)
        var tr_aquatic = util.TraceLine({
        start = myPos,;
        endpos = destVec,;
        filter = trFilter,;
        mask = MASK_WATER;
        });
        if (! tr_aquatic.Hit) self:MaintainIdleBehavior(1) return 
        //print(tr_aquatic.Hit)
        //debugoverlay.Box(tr_aquatic.HitPos, Vector(-2, -2, -2), Vector(2, 2, 2), 5, VJ.COLOR_YELLOW)
        // If the destination is ! a vector, then make sure it's reachable
        else
        if (dest:WaterLevel() <= 1)
        // Destination ! in water, so forget the destination, instead wander OR go deeper into the war
        if (dest:WaterLevel() == 0) self:MaintainIdleBehavior(1) return 
        var trene = util.TraceLine({
        start = dest:GetPos() + self:OBBCenter(),;
        endpos = (dest:GetPos() + self:OBBCenter()) + dest:GetUp()*-20,;
        filter = trFilter;
        });
        //PrintTable(trene)
        //debugoverlay.Box(trene.HitPos, Vector(-2, -2, -2), Vector(2, 2, 2), 5, VJ.COLOR_GREEN)
        if (trene.Hit == true) return 
        //if (trene.Entity.IsValid() && trene.Entity == dest) return 




        // Movement Calculations
        // local nearpos = self:GetNearestPositions(dest)
        var startPos = myPos + self:OBBCenter() + vecStart // nearpos.MyPosition;
        var endPos = destVec || dest:GetPos() + dest:OBBCenter() + vecEnd // nearpos.EnemyPosition;
        var tr = util.TraceHull({
        start = startPos,;
        endpos = endPos,;
        filter = trFilter,;
        mins = self:OBBMins(),;
        maxs = self:OBBMaxs();
        });
        var trHitPos = tr.HitPos;
        //local groundLimited = false -- If true, it limited the ground because it was too close
        // Preform ground check if:
        // It's an aerial NPC AND it is ! ignoring ground AND-
        // It's NOT a chase enemy OR it is but the NPC doesn't have a melee attack
        if (selfData.MovementType == VJ_MOVETYPE_AERIAL && extraOptions.IgnoreGround != true && ((! chaseEnemy) || (chaseEnemy && ! selfData.HasMeleeAttack)))
        var tr_check1 = Game.SceneTrace.Ray({start = startPos, endpos = startPos + Vector(0, 0, -selfData.AA_GroundLimit), filter = trFilter}).Run();
        var tr_check2 = Game.SceneTrace.Ray({start = trHitPos, endpos = trHitPos + Vector(0, 0, -selfData.AA_GroundLimit), filter = trFilter}).Run();
        if (debug)
        print("[MoveTo] checking+.");
        debugoverlay.Box(startPos, Vector(-2, -2, -2), Vector(2, 2, 2), 5, Color(145, 255, 0));
        debugoverlay.Box(tr_check1.HitPos, Vector(-2, -2, -2), Vector(2, 2, 2), 5, Color(0, 183, 255));

        // If it hit the world, then we are too close to the ground, replace "tr" with a new position!
        if (tr_check1.Hit == true || (tr_check2.Hit == true && ! tr_check2.Entity:IsNPC()))
        if (debug) print("[MoveTo] Ground Hit!", tr_check1.HitPos:Distance(startPos)) 
        //groundLimited = true
        endPos.z = (tr_check1.Hit && myPos.z || endPos.z) + selfData.AA_GroundLimit;
        tr = util.TraceHull({
        start = startPos,;
        endpos = endPos,;
        filter = trFilter,;
        mins = self:OBBMins(),;
        maxs = self:OBBMaxs();
        });
        trHitPos = tr.HitPos;



        if (! destVec)
        // If world is hit then our hitbox can't fully fit through the path to the destination
        if (tr.HitWorld)
        if (debug) print("[MoveTo] hitworld") 
        // If we are already going to the last destination+.
        if (selfData.AA_DoingLastChasePos)
        // Its movement is finished, therefore it's ! moving there anymore!
        if (selfData.AA_CurrentMoveTime < Time.Now)
        selfData.AA_DoingLastChasePos = false;
        selfData.AA_LastChasePos = null;
        // It's moving there, don't interrupt!
        else
        return;

        // If we have a last destination then move there!
        else if (selfData.AA_LastChasePos != null)
        if (debug) debugoverlay.Box(selfData.AA_LastChasePos, Vector(-2, -2, -2), Vector(2, 2, 2), 5, Color(0, 68, 255)) 
        selfData.AA_DoingLastChasePos = true;
        tr = util.TraceHull({
        start = startPos,;
        endpos = selfData.AA_LastChasePos,;
        filter = trFilter,;
        mins = self:OBBMins(),;
        maxs = self:OBBMaxs();
        });

        else
        selfData.AA_DoingLastChasePos = false;
        selfData.AA_LastChasePos = trHitPos;


        trHitPos = tr.HitPos;
        var trDistStart = startPos:Distance(trHitPos);
    }

    public virtual void AA_MoveAnimation()
    {
        var selfData = funcGetTable(self);
        var curSeq = SkinnedModelRenderer.CurrentSequence;
        var curACT = self:GetActivity();
        if (((Time.Now > selfData.AA_NextMoveAnimTime) || (curSeq != selfData.AA_CurrentMoveAnim || (curACT != ACT_DO_NOT_DISTURB && self:GetSequenceActivity(curSeq) != self:TranslateActivity(curACT)))) && ! self:IsBusy("Activities"))
        var chosenAnim = false;
        if (selfData.AA_CurrentMoveAnimType == "Calm")
        chosenAnim = (selfData.MovementType == VJ_MOVETYPE_AQUATIC && selfData.Aquatic_AnimTbl_Calm) || selfData.Aerial_AnimTbl_Calm;
        else if (selfData.AA_CurrentMoveAnimType == "Alert")
        chosenAnim = (selfData.MovementType == VJ_MOVETYPE_AQUATIC && selfData.Aquatic_AnimTbl_Alerted) || selfData.Aerial_AnimTbl_Alerted;

        chosenAnim = RandomHelper.FromList(chosenAnim);
        var _, animDur = self:PlayAnim(chosenAnim, false, 0, false, 0, {AlwaysUseSequence = badACTs[chosenAnim] || false});
        selfData.AA_CurrentMoveAnim = self:GetActivity() == ACT_DO_NOT_DISTURB && SkinnedModelRenderer.CurrentSequence || self:GetIdealSequence() -- In case we played a non-sequence;
        selfData.AA_NextMoveAnimTime = Time.Now + animDur -- animDur will always be accurate;
    }

}