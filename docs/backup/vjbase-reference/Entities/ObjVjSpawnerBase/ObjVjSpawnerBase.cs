using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjSpawnerBase : BaseProjectile
{
    [Property] public object Model = {};
    [Property] public bool SingleSpawner = false;
    [Property] public bool PauseSpawning = false;
    [Property] public int RespawnCooldown = 3;
    [Property] public object EntitiesToSpawn = {};
    [Property] public bool SoundTbl_Idle = false;
    [Property] public int IdleSoundChance = 1;
    [Property] public int IdleSoundLevel = 80;
    [Property] public object IdleSoundPitch = VJ.SET(80, 100);
    [Property] public object IdleSoundCooldown = VJ.SET(4, 10);
    [Property] public bool SoundTbl_SpawnEntity = false;
    [Property] public int SpawnEntitySoundChance = 1;
    [Property] public int SpawnEntitySoundLevel = 80;
    [Property] public object SpawnEntitySoundPitch = VJ.SET(80, 100);
    [Property] public bool Dead = false;
    [Property] public int NextIdleSoundT = 0;
    [Property] public string Type = "anim";
    [Property] public string PrintName = "VJ Base Spawner";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool IsVJBaseSpawner = true;

    public virtual void OnInit()
    {
    }

    public virtual void OnSpawnEntity(ent, spawnKey, spawnTbl, initSpawn)
    {
    }

    public virtual void OnThink()
    {
    }

    public virtual void OnCustomRemove()
    {
    }

    public virtual void SpawnEntity(spawnKey, spawnTbl, initSpawn)
    {
        if (this.PauseSpawning)
        // Add to the queue to spawn later
        this.CurrentQueue = this.CurrentQueue || {}
        table.insert(this.CurrentQueue, {spawnKey = spawnKey, spawnTbl = spawnTbl});
        return;


        var spawnCreator = GetCreator();
        var spawnPos = spawnTbl.SpawnPosition || defPos;
        var spawnAng = spawnTbl.SpawnAngle || defAng;
        var spawnEnts = spawnTbl.Entities;
        var spawnNPCClass = spawnTbl.NPC_Class || false;
        var spawnFriToPlyAllies = spawnTbl.FriToPlyAllies || false;
        var spawnWepPicked = Game.Random.FromList(spawnTbl.WeaponsList);
        var entPicked;  // The entity that we will spawn;
        var entsNum = spawnEnts.Count  // The number of entities;
        var i = 0  // If this number equals entsNum, then its the last entity;
        for _, v in RandomPairs(spawnEnts) do
        i = i + 1;
        var strExp = string_explode(":", v)  // Separates the entity class && the number after ":";
        //PrintTable(strExp)
        if (strExp[2])
        if (i == entsNum)  // If we are the last entity, then just spawn it anyway
        entPicked = strExp[1];
        break;
        else if (Game.Random.NextInt(1, strExp[2]) == 1)
        entPicked = strExp[1];
        break;

        else  // String does NOT contain ":", so just pick this
        entPicked = v;
        break;



        // Create the entity
        var ent = SceneUtility.CreatePrefab();
        if (spawnCreator.IsValid())
        ent.SetCreator(spawnCreator);

        ent.SetPos(Transform.Position + spawnPos);
        ent.SetAngles(spawnAng + Transform.Rotation);
        ent.Spawn();
        ent.Activate();
        if (spawnNPCClass)
        ent.VJ_NPC_Class = istable(spawnNPCClass) && spawnNPCClass || {spawnNPCClass}

        if (spawnFriToPlyAllies)
        ent.AlliedWithPlayerAllies = true;

        if (ent.IsNPC() && spawnWepPicked != false && string.lower(spawnWepPicked) != "none")
        if (string.lower(spawnWepPicked) == "default")  // Default weapon from the spawn menu
        var getDefWep = Game.Random.FromList(list.Get("NPC")[ent.GetClass()].Weapons);
        if (getDefWep)
        ent.Give(getDefWep);

        else;
        ent.Give(spawnWepPicked);


        if (this.SingleSpawner)
        GameTask.DelaySeconds(0.1).ContinueWith(_ => function();
        if (this.IsValid())
        GameObject.Destroy();

        end);
        else;
        ent.CallOnRemove("vj_spawner_remove", function(dataEnt, dataSpawnKey);
        if (this.IsValid())
        var entSpawnTbl = this.CurrentEntities[dataSpawnKey];
        if (!entSpawnTbl.Respawning)
        entSpawnTbl.Respawning = true  // To make sure it only respawns it once!;
        GameTask.DelaySeconds(this.RespawnCooldown).ContinueWith(_ => function();
        if (this.IsValid())
        SpawnEntity(dataSpawnKey, entSpawnTbl, false);

        end);


        end, spawnKey);


        this.CurrentEntities[spawnKey] = {Entities = spawnEnts, SpawnPosition = spawnPos, SpawnAngle = spawnAng, WeaponsList = spawnTbl.WeaponsList, NPC_Class = spawnNPCClass, FriToPlyAllies = spawnFriToPlyAllies, Ent = ent, Respawning = false}
        OnSpawnEntity(ent, spawnKey, spawnTbl, initSpawn);

        // Play spawn sound
        var sdTbl = this.SoundTbl_SpawnEntity;
        if (sdTbl && Game.Random.NextInt(1, this.SpawnEntitySoundChance))
        SoundManager.Emit(self, sdTbl, this.SpawnEntitySoundLevel, Game.Random.NextInt(this.SpawnEntitySoundPitch.a, this.SpawnEntitySoundPitch.b));

    }

    public virtual void OnInitialize()
    {
        Init();
        if (this.CustomOnInitialize) CustomOnInitialize() end  // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (this.CustomOnThink) this.OnThink = function() CustomOnThink() end end  // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (GetModel() == "models/error.mdl")  // No model was detected
        var mdl = Game.Random.FromList(this.Model);
        if (mdl && mdl != "models/props_junk/popcan01a.mdl")
        ModelRenderer.Model = mdl;
        else  // No models found in this.Model
        Render.CastShadows = false;
        SetNoDraw(true);
        SetNotSolid(true);


        this.CurrentEntities = {}

        // Delay to avoid issues such as the position of the spawner being offset
        GameTask.DelaySeconds(0.1).ContinueWith(_ => function();
        if (!this.IsValid()) return 
        for spawnKey, spawnTbl in this.EntitiesToSpawn do
        var spawnPos = spawnTbl.SpawnPosition;
        if (istable(spawnPos))  // !!!!!!!!!!!!!! DO NOT USE THESE VARIABLES !!!!!!!!!!!!!! [Backwards Compatibility!]
        spawnTbl.SpawnPosition = Vector(spawnPos.vForward || 0, spawnPos.vRight || 0, spawnPos.vUp || 0);

        SpawnEntity(spawnKey, spawnTbl, true);

        end);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.Think();
        var curTime = Time.Now;
        var selfData = funcGetTable(self);
        OnThink();

        // Idle sound
        var sdTbl = selfData.SoundTbl_Idle;
        if (sdTbl && curTime > selfData.NextIdleSoundT)
        if (Game.Random.NextInt(1, selfData.IdleSoundChance) == 1)
        selfData.CurrentIdleSound?.Stop();
        selfData.CurrentIdleSound = SoundManager.CreateHandle(self, sdTbl, selfData.IdleSoundLevel, Game.Random.NextInt(selfData.IdleSoundPitch.a, selfData.IdleSoundPitch.b));

        selfData.NextIdleSoundT = curTime + Game.Random.NextFloat(selfData.IdleSoundCooldown.a, selfData.IdleSoundCooldown.b);


        // Handle queued entities in case we were paused when they were supposed to spawn
        if (!selfData.PauseSpawning)
        var queue = selfData.CurrentQueue;
        if (queue)
        for _, backData in queue do
        SpawnEntity(backData.spawnKey, backData.spawnTbl, false);

        this.CurrentQueue = null;



        NextThink(curTime + 0.5);
        return true;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function ENT.OnRemove();
        CustomOnRemove();
        this.Dead = true;
        this.CurrentIdleSound?.Stop();

        var curEnts = this.CurrentEntities;
        if (curEnts)
        // SINGLE SPAWNERS: Add all the spawned entities the player's undo list
        if (this.SingleSpawner)
        var creator = GetCreator();
        if (creator.IsValid())
        for _, spawnTbl in curEnts do
        var ent = spawnTbl.Ent;
        if (ent.IsValid())
        undo.Create(ent.GetName());
        undo.AddEntity(ent);
        undo.SetPlayer(creator);
        undo.Finish();



        // CONTINUOUS SPAWNERS: Remove all spawned entities
        else;
        for _, spawnTbl in this.CurrentEntities do
        if (spawnTbl.Ent.IsValid())
        spawnTbl.Ent.Remove();





        ENT.Base 			= "base_anim";
        ENT.Type 			= "anim";
        ENT.PrintName 		= "VJ Base Spawner";
        ENT.Author 			= "DrVrej";
        ENT.Contact 		= "http://steamcommunity.com/groups/vrejgaming";
        ENT.Category		= "VJ Base";

        ENT.IsVJBaseSpawner = true;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        if (CLIENT)
        var metaEntity = MetaTable.For("Entity");
        var funcDrawModel = metaEntity.DrawModel;
        function ENT.Draw() funcDrawModel(self);
    }

    public virtual void OnDraw()
    {
    }

}