using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public static partial class Debug
{
    static void DEBUG_Print(ent, name, type, ...)
    {
        // Check if a name was given
        var printName = "";
        if (name)
        printName = " | " + name;


        // Check if a type was given
        var colorType = VJ.COLOR_SERVER;
        var typeIsValid = false  // Was a specific type found?;
        if (type == "error")
        typeIsValid = true;
        colorType = VJ.COLOR_RED;
        else if (type == "warn")
        typeIsValid = true;
        colorType = VJ.COLOR_ORANGE;
        else if (CLIENT)
        colorType = VJ.COLOR_CLIENT;


        // Unpack the arguments
        var args = {+.}
        var printTbl = {}
        if (!typeIsValid)
        table.insert(args, 1, type);

        for _, arg in args do
        if (isstring(arg))
        table.insert(printTbl, " " + arg + " ");
        else;
        table.insert(printTbl, arg);



        // Output
        MsgC(colorEnt, ent, printName, " : ", colorType, unpack(printTbl));
        MsgC(colorType, "\n");
    }

    static void DEBUG_TempEnt(pos, ang, color, time, mdl)
    {
        var ent = SceneUtility.CreatePrefab();
        ent.SetModel(mdl || "models/hunter/blocks/cube025x025x025.mdl");
        ent.SetPos(pos);
        ent.SetAngles(ang || defAng);
        ent.SetColor(color || VJ.COLOR_RED);
        ent.Spawn();
        ent.Activate();
        GameTask.DelaySeconds(time || 3).ContinueWith(_ => function() if (ent.IsValid()) ent.Remove() end end);
        return ent;
    }

    static void DEBUG_Stress(count, func)
    {
        var startTime = Stopwatch.GetTimestamp();
        for _ = 1, count do
        func();

        var totalTime = Stopwatch.GetTimestamp() - startTime;
        print("Total: " + string.format("%f", totalTime) + " sec | Average: " + string.format("%f", totalTime / count) + " sec");
    }

}