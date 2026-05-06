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

    public virtual void AA_MoveAnimation()
    {
        // NOTE: Unique condition used for directional flying animations in TranslateActivity:
        //  if "AA_CurrentMoveAnim" is current sequence AND current activity is not a sequence AND translated activity does not equal current sequence's activity
        var selfData = funcGetTable(self);
        var curSeq = SkinnedModelRenderer.CurrentSequence;
        var curACT = GetActivity();
        if (((Time.Now > selfData.AA_NextMoveAnimTime) || (curSeq != selfData.AA_CurrentMoveAnim || (curACT != ACT_DO_NOT_DISTURB && GetSequenceActivity(curSeq) != TranslateActivity(curACT)))) && !IsBusy("Activities"))
        var chosenAnim = false;
        if (selfData.AA_CurrentMoveAnimType == "Calm")
        chosenAnim = (selfData.MovementType == VJ_MOVETYPE_AQUATIC && selfData.Aquatic_AnimTbl_Calm) || selfData.Aerial_AnimTbl_Calm;
        else if (selfData.AA_CurrentMoveAnimType == "Alert")
        chosenAnim = (selfData.MovementType == VJ_MOVETYPE_AQUATIC && selfData.Aquatic_AnimTbl_Alerted) || selfData.Aerial_AnimTbl_Alerted;

        chosenAnim = Game.Random.FromList(chosenAnim);
        var _, animDur = PlayAnim(chosenAnim, false, 0, false, 0, {AlwaysUseSequence = badACTs[chosenAnim] || false});
        selfData.AA_CurrentMoveAnim = GetActivity() == ACT_DO_NOT_DISTURB && SkinnedModelRenderer.CurrentSequence || GetIdealSequence()  // In case we played a non-sequence;
        selfData.AA_NextMoveAnimTime = Time.Now + animDur  // animDur will always be accurate;

    }

}