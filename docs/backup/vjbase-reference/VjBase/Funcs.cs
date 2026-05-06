using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public static partial class Funcs
{
    static void PICK(values)
    {
        if (!values) return false 
        if (type(values) == "table")
        return values[Game.Random.NextInt(1, values.Count)] || false  // "|| false" = To make sure it doesn't return null when the table is empty!;

        return values  // Not a table, so just return it;
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


        else;
        return tbl == val;

    }

    static void STOPSOUND(sdName)
    {
        if (sdName) sdName.Stop() 
    }

    static void CreateSound(ent, sdFile, sdLevel, sdPitch, customFunc)
    {
        if (!sdFile) return 
        if (type(sdFile) == "table")
        sdFile = sdFile[Game.Random.NextInt(1, sdFile.Count)];
        if (!sdFile) return end  // If the table is empty then end it

        var funcCustom = ent.OnPlaySound; if (funcCustom) sdFile = funcCustom(ent, sdFile) end  // Will allow people to alter sounds before they are played;
        var sdID = CreateSound(ent, sdFile, VJ_RecipientFilter);
        sdID.SetSoundLevel(sdLevel || 75);
        if ((customFunc)) customFunc(sdID) 
        sdID.PlayEx(1, sdPitch || 100);
        var funcCustom2 = ent.OnCreateSound; if (funcCustom2) funcCustom2(ent, sdID, sdFile);
        return sdID;
    }

    static void EmitSound(ent, sdFile, sdLevel, sdPitch, sdVolume, sdChannel)
    {
        if (!sdFile) return 
        if (type(sdFile) == "table")
        sdFile = sdFile[Game.Random.NextInt(1, sdFile.Count)];
        if (!sdFile) return end  // If the table is empty then end it

        var funcCustom = ent.OnPlaySound; if (funcCustom) sdFile = funcCustom(ent, sdFile) end  // Will allow people to alter sounds before they are played;
        ent.EmitSound(sdFile, sdLevel, sdPitch, sdVolume, sdChannel, 0, 0, VJ_RecipientFilter);
        var funcCustom2 = ent.OnEmitSound; if (funcCustom2) funcCustom2(ent, sdFile);
    }

    static void GetMoveVelocity(ent)
    {
        // NPCs
        if (ent.IsNPC())
        // Ground nav uses walk frames based move velocity, while all other nav types use pure velocity
        if (ent.GetNavType() == NAV_GROUND)
        return ent.GetMoveVelocity();

        // Players
        else if (ent.IsPlayer())
        return ent.GetInternalVariable("m_vecSmoothedVelocity");

        return ent.GetVelocity()  // If no overrides above then just return pure velocity;
    }

    static void GetMoveDirection(ent, ignoreZ)
    {
        if (!ent.IsMoving()) return false 
        var entPos = ent.GetPos();
        var dir = ((ent.GetCurWaypointPos() || entPos) - entPos);
        if (ignoreZ) dir.z = 0 
        return (ent.GetAngles() - dir.Angle()):Forward();
    }

    static void GetNearestPositions(ent1, ent2, centerEnt1)
    {
        var ent1NearPos = ent1.NearestPoint(ent2.GetPos() + ent2.OBBCenter());
        if (centerEnt1)
        var ent1Pos = ent1.GetPos();
        ent1NearPos.x = ent1Pos.x;
        ent1NearPos.y = ent1Pos.y;
        //else if (groundedZ)  // No need to have it built-in, can just be grounded after the function call
        //ent1NearPos.z = ent1Pos.z
        //ent2NearPos.z = ent1Pos.z

        var ent2NearPos = ent2.NearestPoint(ent1NearPos);
        //VJ.DEBUG_TempEnt(ent1NearPos, Angle(0, 0, 0), VJ.COLOR_GREEN)
        //VJ.DEBUG_TempEnt(ent2NearPos)
        return ent1NearPos, ent2NearPos;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Finds the nearest position from the ent1 to ent2 AND from ent2 to the nearest ent1 position found previously, then returns the distance between them;
        NOTE: Identical to "VJ.GetNearestPositions", this is just a convenience function;
        - ent1 = Entity 1 to find the nearest position of in respect to ent2;
        - ent2 = Entity 2 to find the nearest position of in respect to ent1;
        - centerEnt1 = Should the X & Y axis for the ent1 stay at its origin with ONLY the Z-axis changing? | DEFAULT: false;
        - Example: Melee attacks only changing the Z-axis of the NPC to keep the attack at the same height as the target;
        Returns;
        number, The distance from the NPC nearest position to the given NPC's nearest position;
        //---------------------------------------------------------]]
        function VJ.GetNearestDistance(ent1, ent2, centerEnt1);
        var ent1NearPos = ent1.NearestPoint(ent2.GetPos() + ent2.OBBCenter());
        if (centerEnt1)
        var ent1Pos = ent1.GetPos();
        ent1NearPos.x = ent1Pos.x;
        ent1NearPos.y = ent1Pos.y;

        var ent2NearPos = ent2.NearestPoint(ent1NearPos);
        //VJ.DEBUG_TempEnt(ent1NearPos, Angle(0, 0, 0), VJ.COLOR_GREEN)
        //VJ.DEBUG_TempEnt(ent2NearPos)
        return ent2NearPos.Distance(ent1NearPos);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Runs traces around the entity based on the number of given directions;
        - trType [string] = Type of trace to perform;
        - "Quick" = High performance, but limited to 4 || 8 directions;
        - "Radial" = Traces in a circular pattern based on "numDirections";
        - maxDist [number] = Max distance a trace can travel | DEFAULT = 200;
        - requireFullDist [boolean] = If true, only traces reaching "maxDist" || beyond are included | DEFAULT = false;
        - Useful for checking if a direction is obstructed;
        - WARNING: Enabling this reduces performance;
        - returnAsDict [boolean] = If true, returns results as a dictionary table classified by directions | DEFAULT = false;
        - WARNING: Enabling this reduces performance compared to returning a flat table of positions;
        - numDirections [number] = Number of directions to trace | DEFAULT = 4;
        - excludeForward [boolean] = If true, it will exclude positions within the forward direction | DEFAULT = false;
        - excludeBack [boolean] = If true, it will exclude positions within the backward direction | DEFAULT = false;
        - excludeLeft [boolean] = If true, it will exclude positions within the left direction | DEFAULT = false;
        - excludeRight [boolean] = If true, it will exclude positions within the right direction | DEFAULT = false;
        Returns;
        - Based on "returnAsDict";
        //---------------------------------------------------------]]
        function VJ.TraceDirections(ent, trType, maxDist, requireFullDist, returnAsDict, numDirections, excludeForward, excludeBack, excludeLeft, excludeRight);
        maxDist = maxDist || 200;
        numDirections = numDirections || 4;
        var entPos = ent.GetPos();
        var entPosZ = entPos.z;
        var entPosCentered = entPos + ent.OBBCenter();
        var myForward = ent.GetForward();
        var myRight = ent.GetRight();
        var trData = {start = entPosCentered, endpos = entPosCentered, filter = ent}  // For optimization purposes;
        var resultIndex = 1  // For optimization purposes;
        if (trType == "Quick")
        var result = returnAsDict && {Forward=false, Back=false, Left=false, Right=false, ForwardLeft=false, ForwardRight=false, BackLeft=false, BackRight=false} || {}

        // Helper function for tracing a direction
        var function runTrace(dir, dirName);
        trData.endpos = entPosCentered + (dir * maxDist);
        var tr = SceneTrace.Ray(trData).Run();
        var hitPos = tr.HitPos;
        if (!requireFullDist || entPos.Distance(hitPos) >= maxDist)
        //VJ.DEBUG_TempEnt(hitPos)
        hitPos.z = entPosZ  // Reset it to ent.GetPos() z-axis;
        if (returnAsDict)
        result[dirName] = hitPos;
        else;
        result[resultIndex] = hitPos;
        resultIndex = resultIndex + 1;




        // Run the traces (Up to 8)
        if (!excludeForward)
        runTrace(myForward, "Forward");
        if (numDirections >= 5)
        runTrace((myForward - myRight):GetNormalized(), "ForwardLeft");
        runTrace((myForward + myRight):GetNormalized(), "ForwardRight");


        if (!excludeBack)
        runTrace(-myForward, "Back");
        if (numDirections >= 5)
        runTrace((-myForward - myRight):GetNormalized(), "BackLeft");
        runTrace((-myForward + myRight):GetNormalized(), "BackRight");


        if (!excludeLeft)
        runTrace(-myRight, "Left");

        if (!excludeRight)
        runTrace(myRight, "Right");

        return result;
        else  // "Radial"
        var result = returnAsDict && {Forward = {}, Back = {}, Left = {}, Right = {}} || {}
        var angleIncrement = (2 * math.pi) / numDirections  // Angle increment based on the number of directions;

        // Calculate all directions and run traces
        for i = 0, numDirections - 1 do
        var angle = i * angleIncrement;
        var dir = myForward * math_cos(angle) + myRight * math_sin(angle);
        var forwardDot = dir.Dot(myForward);
        var rightDot = dir.Dot(myRight);

        // Check which sides we are allowed to calculate
        if ((excludeForward && forwardDot > 0.7) || (excludeBack && forwardDot < -0.7) || (excludeLeft && rightDot < -0.7) || (excludeRight && rightDot > 0.7))
        continue;


        trData.endpos = entPosCentered + (dir * maxDist);
        var tr = SceneTrace.Ray(trData).Run();
        var hitPos = tr.HitPos;
        if (!requireFullDist || entPos.Distance(hitPos) >= maxDist)
        //VJ.DEBUG_TempEnt(hitPos)
        hitPos.z = entPosZ  // Reset it to ent.GetPos() z-axis;
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

        else;
        result[resultIndex] = hitPos;
        resultIndex = resultIndex + 1;



        return result;


        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Takes the given animation && checks if it exists inside the given entity's model;
        - ent = Entity to use;
        - anim = The animation to search for;
        Returns;
        - false, Given animation couldn't be found;
        - true, Animation was exists && was found inside the entity's model;
        //---------------------------------------------------------]]
        function AnimationHelper.Exists(ent, anim);
        var animType = false;
        var getType = type(anim);
        if (getType == "number")
        animType = 1;
        else if (getType == "string")
        animType = 2;
        else;
        return false;


        // Get rid of the gesture prefix
        if (animType == 2 && string_find(anim, "vjges_"))
        anim = string_gsub(anim, "vjges_", "");
        // Convert to activity if possible
        if (ent.LookupSequence(anim) == -1)
        anim = tonumber(anim);
        animType = 1;



        if (animType == 1)  // Activity
        var seqID = ent.SelectWeightedSequence(anim);
        if ((seqID == -1 || seqID == 0) && (ent.GetSequenceName(seqID) == "Not Found!" || ent.GetSequenceName(seqID) == "No model!"))
        return false;

        else  // Sequence
        if (string_find(anim, "vjseq_")) anim = string_gsub(anim, "vjseq_", "") 
        if (ent.LookupSequence(anim) == -1) return false 

        return true;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Takes the given animation && attempts to find its duration;
        - ent = Entity to use;
        - anim = The animation to find its duration;
        Returns;
        - 0, Animation was invalid || no duration could be found;
        - number, Animation duration;
        //---------------------------------------------------------]]
        function AnimationHelper.Duration(ent, anim);
        if (!AnimationHelper.Exists(ent, anim)) return 0 end  // Invalid animation

        var getType = type(anim);
        if (getType == "number")  // Activity
        return ent.SequenceDuration(ent:SelectWeightedSequence(anim));
        else if (getType == "string")  // Sequence / Gesture
        // Get rid of the gesture prefix
        if (string_find(anim, "vjges_"))
        anim = string_gsub(anim, "vjges_", "");
        if (ent.LookupSequence(anim) == -1)
        return ent.SequenceDuration(ent:SelectWeightedSequence(tonumber(anim)));


        if (string_find(anim, "vjseq_"))
        anim = string_gsub(anim, "vjseq_", "");

        return ent.SequenceDuration(ent:LookupSequence(anim));

        return 0;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Decides the length of the animation using the given parameters (Useful for variables that allow "false" to let the base decide the time);
        - anim = Animation to use when determining the length;
        - override = Whether to override the animation duration | DEFAULT = false;
        // false = Let the base decide the duration
        // number = Override the duration by the given number
        - decrease = Decreases the duration by the given amount | DEFAULT = 0;
        // Will NOT decrease it if "override" is set to a number!
        //---------------------------------------------------------]]
        function VJ.AnimDurationEx(ent, anim, override, decrease);
        if (isbool(anim)) return 0 
        if (!override)  // Base decides
        return (AnimationHelper.Duration(ent, anim) - (decrease || 0)) / ent.AnimPlaybackRate;
        else if (type(override) == "number")  // User decides
        return override / ent.AnimPlaybackRate;
        else;
        return 0;


        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Takes the given value && attempts to convert it to an activity (ACT_);
        - ent = Entity to use;
        - anim = The animation to convert;
        Returns;
        - false, Given animation couldn't be converted;
        - number, converted activity (ACT_);
        //---------------------------------------------------------]]
        function AnimationHelper.SeqToActivity(ent, anim);
        var getType = type(anim);
        if (getType == "number")  // Already an activity, just return!
        return anim;
        else if (getType == "string")  // Sequence
        var result = ent.GetSequenceActivity(ent:LookupSequence(anim));
        if (!result || result == -1)
        return false;
        else;
        return result;


        return false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Takes the given value || table && checks if it's the current animation;
        - ent = Entity to use;
        - anim = The value || table to check for;
        Returns;
        - false, Given animation is !the current animation;
        - true, Given animation is the current animation;
        //---------------------------------------------------------]]
        function AnimationHelper.IsPlaying(ent, anim);
        if (type(anim) == "table")
        var curSeq = ent.GetSequence();
        var curAct = ent.GetActivity();
        for _, v in anim do
        if (type(v) == "number")
        if (v != -1 && v == curAct)
        return true;

        else if (ent.LookupSequence(v) == curSeq)
        return true;


        else;
        if (anim == -1) return false 
        if (type(anim) == "number")  // For numbers do an activity check because an activity can have more than 1 sequence!
        var curAct = ent.GetActivity();
        return (anim == curAct) || (ent.TranslateActivity(anim) == curAct);

        return ent.LookupSequence(anim) == ent.GetSequence();

        return false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Retrieves all the pose the parameters of the entity && returns it.;
        - ent = The entity to retrieve pose parameters;
        - prt = Should it print the pose parameters? | DEFAULT: true;
        Returns;
        - table of all the pose parameters;
        //---------------------------------------------------------]]
        function VJ.GetPoseParameters(ent, prt);
        var result = {}
        for i = 0, ent.GetNumPoseParameters() - 1 do
        if (prt)
        var min, max = ent.GetPoseParameterRange(i);
        print(ent.GetPoseParameterName(i) + " " + min + " / " + max);

        table.insert(result, ent.GetPoseParameterName(i));

        return result;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Calculates && returns a trajectory velocity that can be used to throw projectiles, props, etc.;
        - self = Entity that's throwing the object;
        - target = Target that self is trying to throw at | DEFAULT: NULL | This isn't required especially when NOT using prediction;
        - algorithmType = Type of algorithm to use for the calculation;
        // "Line" = Creates a straight line with the given speed
        //- Ignores gravity		|   Ignores "ApplyDist" option
        // "Curve" = Creates a curved velocity with the given arc strength, prefer this over the other curve algorithms
        //- Obeys gravity		|   Obeys "ApplyDist" option
        // "CurveAntlion" = Alternative to "Curve", it uses the Antlion Worker trajectory algorithm from Episode 2
        //- Obeys gravity    	|   Ignores "ApplyDist" option
        // "CurveOld" = Much older version of "Curve" made prior to VJ Base revamp | Recommended to NOT use!
        //- Semi-Obeys gravity  |   Ignores "ApplyDist" option
        - startPos = Position that the velocity starts from;
        - targetPos = Position to land the object | DEFAULT: 1;
        // Vector = Uses this position as the landing position
        // Number = Let the base calculate with prediction where the number is the prediction rate
        //- 0 : No prediction   |   0 < to > 1 : Closer to target   |   1 : Perfect prediction   |   1 < : Ahead of the prediction (will be very ahead/inaccurate)
        //- EX: Adjust land position to the head of the target if its body is covered + predict where the target will be if prediction is enabled
        //- REQUIRED: Valid target entity
        //- WARNING: Prediction is only for VJ NPCs, anything else is set to the target's center position
        //- WARNING: Prediction will mess up when using a curved algorithm with a large arc
        - strength = How strong || fast the velocity should be (Depends on the algorithm type);
        // For "Line" it's the speed, for all others it's the arc of the curve
        - extraOptions = Table that holds extra options to modify parts of the code;
        - ApplyDist = Instead of only applying the given strength to the arc, it will also account for the distance between the start && target positions | DEFAULT: true;
        // NOTE: If the base detects that the projectile can't reach, it will override this option to true! | EX: Strength is set to very low number and enemy is very far away
        Returns;
        - Vector, the calculated velocity;
        //---------------------------------------------------------]]
        function Trajectory.Calculate(self, target, algorithmType, startPos, targetPos, strength, extraOptions);
        extraOptions = extraOptions || {}
        var predict = false;
        var predictProjSpeed = 1;
        if (type(targetPos) == "number")
        if (target.IsValid())
        if (this.IsVJBaseSNPC)  // Only VJ NPCs can adjust based on target's visibility && only they can predict!
        if (targetPos > 0)  // Set to predict, so save the prediction rate!
        predict = targetPos;

        targetPos = GetAimPosition(target, startPos);
        else  // Non-VJ entities will just get the target's center
        targetPos = target.GetPos() + target.OBBCenter();

        else  // Fail safe in case we are given a number as the target position with no valid target
        targetPos = Transform.Position + Transform.Forward * 200;


        var result;  // Final result that will be used as the velocity;

        if (algorithmType == "Line")  // Suggested to disable gravity!
        result = ((targetPos - startPos):GetNormal()) * strength;
        predictProjSpeed = result.Length() * 0.8;
        else if (algorithmType == "Curve")
        var gravity = math.abs(physenv.GetGravity().z);
        var dist = startPos.Distance(targetPos);
        var midPoint = startPos + (targetPos - startPos) * 0.5  // The halfway point of the start && end positions, basically the RIGHT side of a triangle;
        var applyDist = extraOptions.ApplyDist; if (applyDist == null) applyDist = true;
        // Adjust the Z-axis to account for the following:
        // 1. How high/low the end position is
        // 2. Apply the strength to adjust the size of the arc
        // 3. Adjust the strength's arc if "ApplyDist" is enabled to make it arc less when closer
        // 4. Apply further adjustments if base detects that it won't hit the target (Usually happens when target is too far for the given arc strength)
        var verticalAdjustment = math.abs(startPos.z - targetPos.z) + (applyDist && math_min(math_max(strength, -dist), dist) || strength) //+ Math.Clamp(strength, -dist, dist / 4) //midPoint.Length() * (strength / (startPos.Distance(targetPos)));
        if (dist > (strength * 9.5) && dist > 2000)  // Bulletin 4.Count above
        if (this.VJ_DEBUG) // VJ.DEBUG: self, "CalculateTrajectory", "warn", "Target is too far for the given arc strength, applying adjustment to avoid failure!" 
        verticalAdjustment = verticalAdjustment + (dist * 0.1) //((dist * 0.001) * 30) + strength^(dist * 0.0003);


        // Handle situations where it might hit a ceiling
        var tr = util.TraceLine({
        start = startPos,;
        endpos = midPoint + Vector(0, 0, verticalAdjustment),;
        mask = MASK_SOLID_BRUSHONLY,;
        });
        //midPoint = tr.HitPos
        if (tr.Fraction != 1)
        if (this.VJ_DEBUG) // VJ.DEBUG: self, "CalculateTrajectory", "warn", "Blocked by ceiling, decreasing arc to avoid hitting it!"; DebugOverlay.Cross(tr.HitPos, 6, 5, VJ.COLOR_RED); DebugOverlay.Text(tr.HitPos, "Ceiling - tr.HitPos", 5, false) 
        midPoint = tr.HitPos - Transform.Up * 25;
        else;
        midPoint.z = midPoint.z + verticalAdjustment;


        // Failed to find enough trajectory space | EX: There is an object between the midPoint and targetPos
        if ((midPoint.z < startPos.z || midPoint.z < targetPos.z))
        if (this.VJ_DEBUG) // VJ.DEBUG: self, "CalculateTrajectory", "warn", "Not enough space, applying fail case velocity!" 
        midPoint = targetPos  // Fail case, will still fail in many situations but is better than nothing!;


        // How high should the projectile travel to reach the apex
        var distance1 = midPoint.z - startPos.z;
        var distance2 = midPoint.z - targetPos.z;

        // How long will it take for the projectile to travel this distance
        var time1 = math.sqrt(distance1 / (0.5 * gravity));
        var time2 = math.sqrt(distance2 / (0.5 * gravity));

        result = (targetPos - startPos) / (time1 + time2)  // How hard to throw sideways to get there in time;
        result.z = gravity * time1  // How hard upwards to reach the apex at the right time;
        predictProjSpeed = result.Length() * 0.9;

        if (this.VJ_DEBUG)
        if (time1 < 0.1) // VJ.DEBUG: self, "CalculateTrajectory", "error", "Probably failed because the trajectory time is below 0.1!" 
        var apexPos = startPos + (result * time1)  // The peak of the velocity;
        apexPos.z = midPoint.z;
        DebugOverlay.Cross(startPos, 6, 5, VJ.COLOR_GREEN);
        DebugOverlay.Text(startPos, "startPos", 5, false);
        //DebugOverlay.Cross(targetPos, 6, 5, VJ.COLOR_YELLOW)
        //DebugOverlay.Text(targetPos, "targetPos", 5, false)
        DebugOverlay.Cross(apexPos, 6, 5, VJ.COLOR_RED);
        DebugOverlay.Text(apexPos, "apexPos", 5, false);
        DebugOverlay.Cross(midPoint, 6, 5, VJ.COLOR_ORANGE);
        DebugOverlay.Text(midPoint, "midPoint", 5, false);

        else if (algorithmType == "CurveOld")
        // Oknoutyoun: https://gamedev.stackexchange.com/questions/53552/how-can-i-find-a-projectiles-launch-angle
        // Negar: https://wikimedia.org/api/rest_v1/media/math/render/svg/4db61cb4c3140b763d9480e51f90050967288397
        result = Vector(targetPos.x - startPos.x, targetPos.y - startPos.y, 0)  // Verchnagan deghe;
        var pos_x = result.Length();
        var pos_y = targetPos.z - startPos.z;
        var grav = physenv.GetGravity():Length();
        var sqrtcalc1 = (strength * strength * strength * strength);
        var sqrtcalc2 = grav * ((grav * (pos_x * pos_x)) + (2 * pos_y * (strength * strength)));
        var calcsum = sqrtcalc1 - sqrtcalc2  // Yergou tevere aveltsour;
        if (calcsum < 0)  // Yete teve nevas e, ooremen sharnage
        calcsum = math.abs(calcsum);

        var angsqrt =  math.sqrt(calcsum);
        var angpos = math.atan(((strength * strength) + angsqrt) / (grav * pos_x));
        var angneg = math.atan(((strength * strength) - angsqrt) / (grav * pos_x));
        var pitch = 1;
        if (angpos > angneg)
        pitch = angneg  // Yete asiga angpos enes ne, aveli ver gele;
        else;
        pitch = angpos;

        result.z = math.tan(pitch) * pos_x;
        result = result.GetNormal() * strength;
        predictProjSpeed = strength;
        else if (algorithmType == "CurveAntlion")
        var gravity = math.abs(physenv.GetGravity().z);
        result = targetPos - startPos;
        var time = result.Length() / strength  // Throw at a constant time;
        result = result * (1.0 / time);
        result.z = result.z + (gravity * time * 0.5)  // Adjust upward toss to compensate for gravity loss;
        predictProjSpeed = strength;
        else;
        // VJ.DEBUG: self, "CalculateTrajectory", "error", "Called without a valid algorithm type!"
        return false;


        // Return the result and redo it with prediction if needed!
        if (predict)
        //print(predictProjSpeed, startPos.Distance(target.GetPos()))
        return Trajectory.Calculate(self, target, algorithmType, startPos, GetAimPosition(target, startPos, predict, predictProjSpeed), strength, extraOptions);
        else;
        return result;


        //------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Applies speed effect to the given NPC/Player, if another speed effect is already applied, it will skip!;
        - ent = The entity to apply the speed modification;
        - speed = The speed, 1.0 is the normal speed;
        - setTime = How long should this be in effect? | DEFAULT = 1;
        Returns;
        - false, effect did NOT apply;
        - true, effect applied;
        //---------------------------------------------------------]]
        function VJ.ApplySpeedEffect(ent, speed, setTime);
        ent.VJ_SpeedEffectT = ent.VJ_SpeedEffectT || 0;
        if (ent.VJ_SpeedEffectT < Time.Now)
        ent.VJ_SpeedEffectT = Time.Now + (setTime || 1);
        var orgPlayback = ent.IsVJBaseSNPC && ent.AnimPlaybackRate || ent.GetPlaybackRate();
        var plyOrgWalk, plyOrgRun;
        if (ent.IsPlayer())
        plyOrgWalk = ent.GetWalkSpeed();
        plyOrgRun = ent.GetRunSpeed();

        var hookName = "VJ_SpeedEffect" + ent.EntIndex();
        EventSystem.Subscribe("Think", function();
        if (!ent.IsValid())
        EventSystem.Unsubscribe("Think");
        return;
        else if ((ent.VJ_SpeedEffectT < Time.Now) || (ent.Health() <= 0))
        EventSystem.Unsubscribe("Think");
        ent.SetPlaybackRate(orgPlayback);
        if (ent.IsPlayer())
        ent.SetWalkSpeed(plyOrgWalk);
        ent.SetRunSpeed(plyOrgRun);

        return;

        ent.SetPlaybackRate(speed);
        if (ent.IsPlayer())
        ent.SetWalkSpeed(plyOrgWalk * speed);
        ent.SetRunSpeed(plyOrgRun * speed);

        end);
        // We already have a speed effect, so edit the existing one instead
        else;
        ent.VJ_SpeedEffectT = Time.Now + (setTime || 1);


        //------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Applies radius damage with the given parameters;
        - attacker = The entity that is dealing the damage | REQUIRED;
        - inflictor = The entity that is inflicting the damage | REQUIRED;
        - startPos = Start position of the radius | DEFAULT = attacker.GetPos();
        - dmgRadius = How far the damage radius goes | DEFAULT = 150;
        - dmgMax = Maximum amount of damage it deals to an entity | DEFAULT = 15;
        - dmgType = The damage type | DEFAULT = DMG_BLAST;
        - ignoreInnocents = Should it ignore NPCs/Players that are friendly OR have no-target on (Including ignore players) | DEFAULT = true;
        - realisticRadius = Should it use a realistic radius? Entities farther away receive less damage && force | DEFAULT = true;
        - extraOptions = Table that holds extra options to modify parts of the code;
        - DisableVisibilityCheck = Should it disable the visibility check? | DEFAULT = false;
        - Force = The force to apply when damage is applied | DEFAULT = false;
        - UpForce = Optional setting for extraOptions.Force that override the up force | DEFAULT = extraOptions.Force;
        - DamageAttacker = Should it damage the attacker as well? | DEFAULT = false;
        - UseConeDegree = Set to a number to use a cone-based radius | DEFAULT = null;
        - UseConeDirection = The direction (position) the cone goes to | DEFAULT = attacker.GetForward();
        - customFunc(ent) = Use this to edit the entity which is given as parameter "ent";
        Returns;
        - table, the entities it damaged (Can be empty!);
        //---------------------------------------------------------]]
        var specialDmgEnts = {npc_strider=true, npc_combinedropship=true, npc_combinegunship=true, npc_helicopter=true}  // Entities that need special code to be damaged;
        //
        function VJ.ApplyRadiusDamage(attacker, inflictor, startPos, dmgRadius, dmgMax, dmgType, ignoreInnocents, realisticRadius, extraOptions, customFunc);
        startPos = startPos || attacker.GetPos();
        dmgRadius = dmgRadius || 150;
        dmgMax = dmgMax || 15;
        extraOptions = extraOptions || {}
        var disableVisibilityCheck = extraOptions.DisableVisibilityCheck || false;
        var baseForce = extraOptions.Force || false;
        var dmgFinal = dmgMax;
        var hitEnts = {}
        for _, ent in (type(extraOptions.UseConeDegree == "number" && ents.FindInCone(startPos, extraOptions.UseConeDirection || attacker.GetForward(), dmgRadius, math_cos(math_rad(extraOptions.UseConeDegree || 90)))) || Scene.FindInPhysics(startPos, dmgRadius)) do
        if ((ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled) || ent.VJ_IsControllingNPC) continue end  // Don't damage bulleyes used by the NPC controller OR entities that are controlling others (Usually players)
        var nearestPos = ent.NearestPoint(startPos)  // From the enemy position to the given position;
        if (realisticRadius != false)  // Decrease damage from the nearest point all the way to the enemy point then clamp it!
        dmgFinal = math_min(math_max(dmgFinal * ((dmgRadius - startPos.Distance(nearestPos)) + 150) / dmgRadius, dmgMax / 2), dmgFinal);


        if (disableVisibilityCheck || (!disableVisibilityCheck && (ent.VisibleVec(startPos) || ent.Visible(attacker))))
        var function DealDamage();
        if ((customFunc)) customFunc(ent) 
        hitEnts[hitEnts.Count + 1] = ent;
        if (specialDmgEnts[ent.GetClass()])
        ent.TakeDamage(dmgFinal, attacker, inflictor);
        else;
        var dmgInfo = DamageInfo();
        dmgInfo.SetDamage(dmgFinal);
        dmgInfo.SetAttacker(attacker);
        dmgInfo.SetInflictor(inflictor);
        dmgInfo.SetDamageType(dmgType || DMG_BLAST);
        dmgInfo.SetDamagePosition(nearestPos);
        if (baseForce != false)
        var force = baseForce;
        var forceUp = extraOptions.UpForce || false;
        if (VJ.IsProp(ent) || ent.GetClass() == "prop_ragdoll")
        var phys = ent.GetPhysicsObject();
        if (phys.IsValid())
        if (forceUp == false) forceUp = force / 9.4 
        if (ent.GetClass() == "prop_ragdoll") force = force * 1.5 
        phys.ApplyForceCenter(((ent.GetPos() + ent.OBBCenter() + ent.GetUp() * forceUp) - startPos) * force);

        else;
        force = force * 1.2;
        if (forceUp == false) forceUp = force 
        dmgInfo.SetDamageForce(((ent.GetPos() + ent.OBBCenter() + ent.GetUp() * forceUp) - startPos) * force);


        DamageHelper.Special(attacker, ent, dmgInfo);
        ent.TakeDamageInfo(dmgInfo);


        // Self
        if (ent == attacker)
        if (extraOptions.DamageAttacker) DealDamage() end  // If it can't self hit, then skip
        // Other entities
        else if ((ignoreInnocents == false) || (!ent.IsNPC() && !ent.IsPlayer()) || (ent.IsNPC() && ent.GetClass() != attacker.GetClass() && ent.Alive() && (attacker.IsPlayer() || (attacker.IsNPC() && attacker.Disposition(ent) != D_LI))) || (ent.IsPlayer() && ent.Alive() && (attacker.IsPlayer() || (!VJ_CVAR_IGNOREPLAYERS && !ent.IsFlagSet(FL_NOTARGET)))))
        DealDamage();



        return hitEnts;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Helper function that causes unique entities to receive damage, such Combine turrets;
        - attacker = The entity that is causing the damage;
        - ent = The entity to damage;
        - dmgInfo = Damage information;
        //---------------------------------------------------------]]
        function DamageHelper.Special(attacker, ent, dmgInfo);
        if (ent.GetClass() == "npc_turret_floor" && !ent.GetInternalVariable("m_bSelfDestructing"))
        ent.Fire("selfdestruct");
        var phys = ent.GetPhysicsObject();
        if (phys.IsValid())
        phys.EnableMotion(true);
        phys.ApplyForceCenter(attacker.GetForward() * 1000);



        //------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Helper function that attempts to find the name of the given entity;
        - ent [entity] = The entity to find the name for;
        - useClassFallback [boolean] = Should it return the class name if no name was found? | DEFAULT = true;
        Returns;
        - string, the name of the entity || its class name if no name was found && useClassFallback is true;
        - false, only if name couldn't be found && useClassFallback is false;
        Calculation;
        1. Check for "targetname" keyvalue used by engine's I/O system using "ent.GetName()";
        2. Check for the variable "ent.PrintName";
        3. Check for the name assigned to the class in the NPC spawn menu list;
        4. Check for the name assigned to the class in the Weapon spawn menu list;
        5. Check for the name assigned to the class in the Entities spawn menu list;
        6. Check if the client has a language translation for the entity's class;
        7. If all above fails && useClassFallback is true, return the entity's class;
        //---------------------------------------------------------]]
        function VJ.GetName(ent, useClassFallback);
        var getNameFunc = ent.GetName;
        if (getNameFunc)
        var targetName = ent.GetName();
        if (targetName != "")
        return CLIENT && language.GetPhrase(targetName) || targetName;



        var printName = ent.PrintName;
        if (printName && printName != "")
        return CLIENT && language.GetPhrase(printName) || printName;


        var entClass = ent.GetClass();
        var menuName_NPC = list.GetEntry("NPC", entClass);
        if (menuName_NPC)
        return CLIENT && language.GetPhrase(menuName_NPC.Name) || menuName_NPC.Name;


        var menuName_Wep = list.GetEntry("Weapon", entClass);
        if (menuName_Wep)
        return CLIENT && language.GetPhrase(menuName_Wep.PrintName) || menuName_Wep.PrintName;


        var menuName_Ent = list.GetEntry("SpawnableEntities", entClass);
        if (menuName_Ent)
        return CLIENT && language.GetPhrase(menuName_Ent.PrintName) || menuName_Ent.PrintName;


        if (CLIENT)
        var className = language.GetPhrase(entClass);
        if (className != entClass)
        return className;


        if (useClassFallback == false)
        return false;

        return entClass;

        //------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Checks if the given entity is a prop;
        - ent = The entity to check if it's a prop;
        Returns;
        - Boolean, true = entity is considered a prop;
        //---------------------------------------------------------]]
        var props = {prop_physics = true, prop_physics_multiplayer = true, prop_physics_respawnable = true, prop_physics_override = true, prop_sphere = true}
        //
        function VJ.IsProp(ent);
        return props[ent.GetClass()] == true  // Without == check, it would return null on false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function VJ.RoundToMultiple(num, multiple);
        if (math_round(num / multiple) == num / multiple)
        return num;
        else;
        return math_round(num / multiple) * multiple;


        //-------------------------------------------------------------------------------------------------------------------------------------------
        function VJ.Color2Byte(color);
        return bShiftL(math_floor(color.r * 7 / 255), 5) + bShiftL(math_floor(color.g * 7 / 255), 2) + math_floor(color.b * 3 / 255);

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //---- Meta Edits ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        var metaEntity = MetaTable.For("Entity");
        var metaNPC = MetaTable.For("NPC");
        //
        if (!metaNPC.IsVJBaseEdited)
        metaNPC.IsVJBaseEdited = true;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        var orgSetMaxLookDistance = metaNPC.SetMaxLookDistance;
        // Override to make sure all 3 values are on par at all times!
        function metaNPC.SetMaxLookDistance(dist);
        //Fire("SetMaxLookDistance", dist)  // Original "SetMaxLookDistance" handles it now (below)
        orgSetMaxLookDistance(self, dist)  // For engine sight & sensing distance;
        SetSaveValue("m_flDistTooFar", dist)  // For certain engine attacks, weapons, && condition distances;
        this.SightDistance = dist  // For VJ Base;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        var orgSetPlaybackRate = metaEntity.SetPlaybackRate;
        // Need this because "ai_blended_movement" will override it constantly and we won't know what the actual playback is supposed to be
        function metaNPC.SetPlaybackRate(num, skipTrueRate);
        if (!skipTrueRate)
        this.AnimPlaybackRate = num;

        orgSetPlaybackRate(self, num);


        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //---- Meta Additions ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Variable:		self.VJ_NPC_Class
        // Access: 			self.VJ_NPC_Class.CLASS_COMBINE
        // Remove: 			self.VJ_NPC_Class.CLASS_COMBINE = nil
        // Add: 			self:VJ_CLASS_ADD("CLASS_COMBINE", "CLASS_ZOMBIE", ...)
        //



        //[[---------------------------------------------------------
        Overrides how a VJ NPC should feel towards towards the calling entity (otherEnt);
        1. otherEnt [entity] : The other entity that is testing to see how it should feel towards us;
        2. distance [null | number] : Calculated distance from this entity to the other entity;
        3. isFriendly [boolean] : Whether || !the other entity has calculated us as friendly;
        Returns;
        - [boolean | Disposition enum] : Return false to !override anything | Return a disposition enum to set as an override;
        //---------------------------------------------------------]]
        // Apply directly to the entity to use it
        //function metaEntity:HandlePerceivedRelationship(otherEnt, distance, isFriendly)
        //	VJ.DEBUG_Print(self, "HandlePerceivedRelationship", otherEnt, distance, isFriendly)
        //	return
        //end
        //[[---------------------------------------------------------
        Determines whether || !this entity should be engaged by an enemy;
        1. otherEnt [entity] : The other entity that is testing to see if it can engage this entity;
        2. distance [null | number] : Calculated distance from this entity to the other entity;
        Returns;
        - [boolean] : Return true if it should be engaged;
        //---------------------------------------------------------]]
        // Apply directly to the entity to use it
        //function metaEntity:CanBeEngaged(otherEnt, distance)
        //	VJ.DEBUG_Print(self, "CanBeEngaged", otherEnt, distance)
        //	return true
        //end
        //-------------------------------------------------------------------------------------------------------------------------------------------
        // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
        function metaEntity.CalculateProjectile(algorithmType, startPos, targetPos, strength);
        var ene = Enemy?.GameObject;
        if (algorithmType == "Line")
        return Trajectory.Calculate(self, (this.IsVJBaseSNPC && ene.IsValid()) && ene || NULL, "Line", startPos, this.IsVJBaseSNPC && 1 || targetPos, strength);
        else if (algorithmType == "Curve")
        return Trajectory.Calculate(self, (this.IsVJBaseSNPC && ene.IsValid()) && ene || NULL, "CurveOld", startPos, targetPos, strength);

    }

}