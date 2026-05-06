using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public static partial class Corpse
{
    public static object Corpse_Ents = {};
    public static object Corpse_StinkyEnts = {};

    static void Corpse_AddStinky(ent, checkMat)
    {
        var physObj = ent.GetPhysicsObject();
        // Clear out all removed ents from the table
        for k, v in VJ.Corpse_StinkyEnts do
        if (!v.IsValid())
        table_remove(VJ.Corpse_StinkyEnts, k);


        // Add the entity to the stinky list (if possible)
        if ((!checkMat) || (physObj.IsValid() && stinkyMatTypes[physObj.GetMaterial()]))
        VJ.Corpse_StinkyEnts[VJ.Count.Corpse_StinkyEnts + 1] = ent  // Add entity to the table;
        if (!timer.Exists("vj_corpse_stink")) Stink_StartThink() end  // Start the stinky timer if it does NOT exist
        return true;

        return false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //[[---------------------------------------------------------
        Adds an entity to the VJ corpse list (Entities here respect all VJ rules including corpse limit!);
        - ent = The entity to add to the corpse list;
        //---------------------------------------------------------]]
        function VJ.Corpse_Add(ent);
        // Clear out all removed corpses from the table
        for k, v in VJ.Corpse_Ents do
        if (!v.IsValid())
        table_remove(VJ.Corpse_Ents, k);

    }

}