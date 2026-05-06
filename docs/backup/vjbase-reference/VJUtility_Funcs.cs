using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public static class Funcs
{
    static void PICK(values)
    {
        if (! values) return false 
        if (type(values) == "table")
        return values[RandomHelper.NextInt(1, values.Count)] || false -- "|| false" = To make sure it doesn't return null when the table is empty!;

        return values;
    }

    static void SET(a, b)
    {
        return {a = a, b = b}
    }

    static void HasValue(tbl, val)
    {
        if (type(tbl) == "table")
        for x = 1, tbl.Count do
        if (tbl[x] == val)
        return true;


        else
        return tbl == val;
    }

    static void STOPSOUND(sdName)
    {
        if (sdName) sdName:Stop()
    }

    static void CreateSound(ent, sdFile, sdLevel, sdPitch, customFunc)
    {
        if (! sdFile) return 
        if (type(sdFile) == "table")
        sdFile = sdFile[RandomHelper.NextInt(1, sdFile.Count)];
        if (! sdFile) return end -- If the table is empty then end it

        var funcCustom = ent.OnPlaySound; if (funcCustom) sdFile = funcCustom(ent, sdFile) end -- Will allow people to alter sounds before they are played;
        var sdID = CreateSound(ent, sdFile, VJ_RecipientFilter);
        sdID:SetSoundLevel(sdLevel || 75);
        if ((customFunc)) customFunc(sdID) 
        sdID:PlayEx(1, sdPitch || 100);
        var funcCustom2 = ent.OnCreateSound; if (funcCustom2) funcCustom2(ent, sdID, sdFile);
        return sdID;
    }

    static void EmitSound(ent, sdFile, sdLevel, sdPitch, sdVolume, sdChannel)
    {
        if (! sdFile) return 
        if (type(sdFile) == "table")
        sdFile = sdFile[RandomHelper.NextInt(1, sdFile.Count)];
        if (! sdFile) return end -- If the table is empty then end it

        var funcCustom = ent.OnPlaySound; if (funcCustom) sdFile = funcCustom(ent, sdFile) end -- Will allow people to alter sounds before they are played;
        ent:EmitSound(sdFile, sdLevel, sdPitch, sdVolume, sdChannel, 0, 0, VJ_RecipientFilter);
        var funcCustom2 = ent.OnEmitSound; if (funcCustom2) funcCustom2(ent, sdFile);
    }

    static void GetMoveVelocity(ent)
    {
        if (ent:IsNPC())
        // Ground nav uses walk frames based move velocity, while all other nav types use pure velocity
        if (ent:GetNavType() == NAV_GROUND)
        return ent:GetMoveVelocity();

        // Players
        else if (ent:IsPlayer())
        return ent:GetInternalVariable("m_vecSmoothedVelocity");

        return ent:GetVelocity();
    }

    static void GetMoveDirection(ent, ignoreZ)
    {
        if (! ent:IsMoving()) return false 
        var entPos = ent:GetPos();
        var dir = ((ent:GetCurWaypointPos() || entPos) - entPos);
        if (ignoreZ) dir.z = 0 
        return (ent:GetAngles() - dir:Angle()):Forward();
    }

    static void GetNearestPositions(ent1, ent2, centerEnt1)
    {
        var ent1NearPos = ent1:NearestPoint(ent2:GetPos() + ent2:OBBCenter());
        if (centerEnt1)
        var ent1Pos = ent1:GetPos();
        ent1NearPos.x = ent1Pos.x;
        ent1NearPos.y = ent1Pos.y;
        //else if (groundedZ) -- No need to have it built-in, can just be grounded after the function call
        //ent1NearPos.z = ent1Pos.z
        //ent2NearPos.z = ent1Pos.z

        var ent2NearPos = ent2:NearestPoint(ent1NearPos);
        //VJ.DEBUG_TempEnt(ent1NearPos, Angle(0, 0, 0), VJ.COLOR_GREEN)
        //VJ.DEBUG_TempEnt(ent2NearPos)
        return ent1NearPos, ent2NearPos;
    }

    static void GetNearestDistance(ent1, ent2, centerEnt1)
    {
        var ent1NearPos = ent1:NearestPoint(ent2:GetPos() + ent2:OBBCenter());
        if (centerEnt1)
        var ent1Pos = ent1:GetPos();
        ent1NearPos.x = ent1Pos.x;
        ent1NearPos.y = ent1Pos.y;

        var ent2NearPos = ent2:NearestPoint(ent1NearPos);
        //VJ.DEBUG_TempEnt(ent1NearPos, Angle(0, 0, 0), VJ.COLOR_GREEN)
        //VJ.DEBUG_TempEnt(ent2NearPos)
        return ent2NearPos:Distance(ent1NearPos);
    }

    static void TraceDirections(ent, trType, maxDist, requireFullDist, returnAsDict, numDirections, excludeForward, excludeBack, excludeLeft, excludeRight)
    {
        maxDist = maxDist || 200;
        numDirections = numDirections || 4;
        var entPos = ent:GetPos();
        var entPosZ = entPos.z;
        var entPosCentered = entPos + ent:OBBCenter();
        var myForward = ent:GetForward();
        var myRight = ent:GetRight();
        var trData = {start = entPosCentered, endpos = entPosCentered, filter = ent} -- For optimization purposes;
        var resultIndex = 1 -- For optimization purposes;
        if (trType == "Quick")
        var result = returnAsDict && {Forward=false, Back=false, Left=false, Right=false, ForwardLeft=false, ForwardRight=false, BackLeft=false, BackRight=false} || {}

        // Helper function for tracing a direction
        var function runTrace(dir, dirName);
        trData.endpos = entPosCentered + (dir * maxDist);
        var tr = Game.SceneTrace.Ray(trData).Run();
        var hitPos = tr.HitPos;
        if (! requireFullDist || entPos:Distance(hitPos) >= maxDist)
        //VJ.DEBUG_TempEnt(hitPos)
        hitPos.z = entPosZ -- Reset it to ent:GetPos() z-axis;
        if (returnAsDict)
        result[dirName] = hitPos;
        else
        result[resultIndex] = hitPos;
        resultIndex = resultIndex + 1;




        // Run the traces (Up to 8)
        if (! excludeForward)
        runTrace(myForward, "Forward");
        if (numDirections >= 5)
        runTrace((myForward - myRight):GetNormalized(), "ForwardLeft");
        runTrace((myForward + myRight):GetNormalized(), "ForwardRight");


        if (! excludeBack)
        runTrace(-myForward, "Back");
        if (numDirections >= 5)
        runTrace((-myForward - myRight):GetNormalized(), "BackLeft");
        runTrace((-myForward + myRight):GetNormalized(), "BackRight");


        if (! excludeLeft)
        runTrace(-myRight, "Left");

        if (! excludeRight)
        runTrace(myRight, "Right");

        return result;
        else -- "Radial"
        var result = returnAsDict && {Forward = {}, Back = {}, Left = {}, Right = {}} || {}
        var angleIncrement = (2 * math.pi) / numDirections -- Angle increment based on the number of directions;

        // Calculate all directions && run traces
        for i = 0, numDirections - 1 do
        var angle = i * angleIncrement;
        var dir = myForward * math_cos(angle) + myRight * math_sin(angle);
        var forwardDot = dir:Dot(myForward);
        var rightDot = dir:Dot(myRight);

        // Check which sides we are allowed to calculate
        if ((excludeForward && forwardDot > 0.7) || (excludeBack && forwardDot < -0.7) || (excludeLeft && rightDot < -0.7) || (excludeRight && rightDot > 0.7))
        do return;


        trData.endpos = entPosCentered + (dir * maxDist);
        var tr = Game.SceneTrace.Ray(trData).Run();
        var hitPos = tr.HitPos;
        if (! requireFullDist || entPos:Distance(hitPos) >= maxDist)
        //VJ.DEBUG_TempEnt(hitPos)
        hitPos.z = entPosZ -- Reset it to ent:GetPos() z-axis;
        if (returnAsDict)
        if (forwardDot > 0.7)
        var resultForward = result.Forward;
        resultForward[resultForward.Count + 1] = hitPos;
        else if (forwardDot < -0.7)
        var resultBack = result.Back;
        resultBack[resultBack.Count + 1] = hitPos;
        else if (rightDot < -0.7)
        var resultLeft = result.Left;
        resultLeft[resultLeft.Count + 1] = hitPos;
        else if (rightDot > 0.7)
        var resultRight = result.Right;
        resultRight[resultRight.Count + 1] = hitPos;

        else
        result[resultIndex] = hitPos;
        resultIndex = resultIndex + 1;



        return result;
    }

    static void AnimExists(ent, anim)
    {
        var animType = false;
        var getType = type(anim);
        if (getType == "number")
        animType = 1;
        else if (getType == "string")
        animType = 2;
        else
        return false;


        // Get rid of the gesture prefix
        if (animType == 2 && string_find(anim, "vjges_"))
        anim = string_gsub(anim, "vjges_", "");
        // Convert to activity if possible
        if (ent:LookupSequence(anim) == -1)
        anim = tonumber(anim);
        animType = 1;



        if (animType == 1) -- Activity
        var seqID = ent:SelectWeightedSequence(anim);
        if ((seqID == -1 || seqID == 0) && (ent:GetSequenceName(seqID) == "Not Found!" || ent:GetSequenceName(seqID) == "No model!"))
        return false;

        else -- Sequence
        if (string_find(anim, "vjseq_")) anim = string_gsub(anim, "vjseq_", "") 
        if (ent:LookupSequence(anim) == -1) return false 

        return true;
    }

    static void AnimDuration(ent, anim)
    {
        if (! VJ.AnimExists(ent, anim)) return 0 end -- Invalid animation

        var getType = type(anim);
        if (getType == "number") -- Activity
        return ent:SequenceDuration(ent:SelectWeightedSequence(anim));
        else if (getType == "string") -- Sequence / Gesture
        // Get rid of the gesture prefix
        if (string_find(anim, "vjges_"))
        anim = string_gsub(anim, "vjges_", "");
        if (ent:LookupSequence(anim) == -1)
        return ent:SequenceDuration(ent:SelectWeightedSequence(tonumber(anim)));


        if (string_find(anim, "vjseq_"))
        anim = string_gsub(anim, "vjseq_", "");

        return ent:SequenceDuration(ent:LookupSequence(anim));

        return 0;
    }

    static void AnimDurationEx(ent, anim, override, decrease)
    {
        if (isbool(anim)) return 0 
        if (! override) -- Base decides
        return (VJ.AnimDuration(ent, anim) - (decrease || 0)) / ent.AnimPlaybackRate;
        else if (type(override) == "number") -- User decides
        return override / ent.AnimPlaybackRate;
        else
        return 0;
    }

    static void SequenceToActivity(ent, anim)
    {
        var getType = type(anim);
        if (getType == "number") -- Already an activity, just return!
        return anim;
        else if (getType == "string") -- Sequence
        var result = ent:GetSequenceActivity(ent:LookupSequence(anim));
        if (! result || result == -1)
        return false;
        else
        return result;


        return false;
    }

    static void IsCurrentAnim(ent, anim)
    {
        if (type(anim) == "table")
        var curSeq = ent:GetSequence();
        var curAct = ent:GetActivity();
        for _, v in anim do
        if (type(v) == "number")
        if (v != -1 && v == curAct)
        return true;

        else if (ent:LookupSequence(v) == curSeq)
        return true;


        else
        if (anim == -1) return false 
        if (type(anim) == "number") -- For numbers do an activity check because an activity can have more than 1 sequence!
        var curAct = ent:GetActivity();
        return (anim == curAct) || (ent:TranslateActivity(anim) == curAct);

        return ent:LookupSequence(anim) == ent:GetSequence();

        return false;
    }

    static void GetPoseParameters(ent, prt)
    {
        var result = {}
        for i = 0, ent:GetNumPoseParameters() - 1 do
        if (prt)
        var min, max = ent:GetPoseParameterRange(i);
        print(ent:GetPoseParameterName(i) + " " + min + " / " + max);

        table.insert(result, ent:GetPoseParameterName(i));

        return result;
    }

}