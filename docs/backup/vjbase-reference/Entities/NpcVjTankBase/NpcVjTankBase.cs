using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class NpcVjTankBase : CreatureNPC
{
    [Property] public int StartHealth = 200;
    [Property] public object MovementType = VJ_MOVETYPE_PHYSICS;
    [Property] public bool ForceDamageFromBosses = true;
    [Property] public int DeathDelayTime = 2;
    [Property] public int BreathSoundLevel = 80;
    [Property] public int IdleSoundLevel = 70;
    [Property] public int CombatIdleSoundLevel = 70;
    [Property] public int AlertSoundLevel = 70;
    [Property] public int DeathSoundLevel = 100;
    [Property] public string SoundTbl_Breath = "vj_base/vehicles/armored/engine_idle.wav";
    [Property] public string SoundTbl_Death = "VJ.Explosion";
    [Property] public bool Tank_GunnerENT = false;
    [Property] public int Tank_AngleOffset = 0;
    [Property] public int Tank_DriveAwayDistance = 1000;
    [Property] public int Tank_DriveTowardsDistance = 2000;
    [Property] public int Tank_RanOverDistance = 500;
    [Property] public float Tank_TurningSpeed = 1.5;
    [Property] public int Tank_DrivingSpeed = 100;
    [Property] public int Tank_CollisionBoundSize = 90;
    [Property] public int Tank_CollisionBoundUp = 100;
    [Property] public int Tank_CollisionBoundDown = -10;
    [Property] public bool Tank_DeathDriverCorpse = false;
    [Property] public int Tank_DeathDriverCorpseChance = 3;
    [Property] public string Tank_DeathDecal = "Scorch";
    [Property] public bool HasMoveSound = true;
    [Property] public bool Tank_SoundTbl_DrivingEngine = false;
    [Property] public bool Tank_SoundTbl_Track = false;
    [Property] public bool HasRunOverSound = true;
    [Property] public bool Tank_SoundTbl_RunOver = false;
    [Property] public bool Tank_IsMoving = false;
    [Property] public int Tank_Status = 1;
    [Property] public int Tank_NextLowHealthSparkT = 0;
    [Property] public int Tank_NextRunOverSoundT = 0;
    [Property] public int Tank_NextIdleParticles = 0;
    [Property] public string Type = "ai";
    [Property] public string PrintName = "VJ Base Tank";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool IsVJBaseSNPC_Tank = true;
    [Property] public bool IsVJBaseSNPC_TankChassis = true;
    [Property] public bool VJ_ID_Vehicle = true;

    public virtual void Tank_Init()
    {
    }

    public virtual void Tank_GunnerSpawnPosition()
    {
        return Transform.Position;
    }

    public virtual void Tank_OnThink()
    {
    }

    public virtual void Tank_OnThinkActive()
    {
    }

    public virtual void Tank_OnRunOver(ent)
    {
    }

    public virtual void GetNearDeathSparkPositions()
    {
        var randPos = Game.Random.NextInt(1, 2);
        if (randPos == 1)
        this.Spark1.SetLocalPos(Transform.Position + Transform.Forward * 100 + Transform.Up * 60);
        else if (randPos == 2)
        this.Spark1.SetLocalPos(Transform.Position + Transform.Forward * -100 + Transform.Up * 60);

    }

    public virtual void Tank_OnInitialDeath(dmginfo, hitgroup)
    {
    }

    public virtual void Tank_OnDeathCorpse(dmginfo, hitgroup, corpse, status, statusData)
    {
    }

    public virtual void Tank_UpdateIdleParticles()
    {
        // Example:
        //local effectData = new EffectData()
        //effectData.SetScale(1)
        //effectData.SetEntity(self)
        //effectData.SetOrigin(Transform.Position + Transform.Forward * -130 + Transform.Right * 25  + Transform.Up * 45)
        //Effects.Play("VJ_VehicleExhaust", effectData, true, true)
        //effectData.SetOrigin(Transform.Position + Transform.Forward * -130 + Transform.Right * -28 + Transform.Up * 45)
        //Effects.Play("VJ_VehicleExhaust", effectData, true, true)
    }

    public virtual void Tank_UpdateMoveParticles()
    {
        // Example:
        //local effectData = new EffectData()
        //effectData.SetScale(1)
        //effectData.SetEntity(self)
        //effectData.SetOrigin(Transform.Position + Transform.Forward * -115 + Transform.Right * 58)
        //Effects.Play("VJ_VehicleMove", effectData, true, true)
        //effectData.SetOrigin(Transform.Position + Transform.Forward * -115 + Transform.Right * -58)
        //Effects.Play("VJ_VehicleMove", effectData, true, true)
    }

    public virtual void OnInit()
    {
        SetPhysicsDamageScale(0)  // Take no physics damage;
        this.Tank_NextIdleParticles = Time.Now + 1;
        this.DeathAnimationCodeRan = true  // So corpse doesn't fly away on death (Take this out if !using death explosion sequence);
        Tank_Init();
        // !!!!!!!!!!!!!! DO NOT USE THESE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (this.CustomInitialize_CustomTank) CustomInitialize_CustomTank() 
        if (this.Tank_DeathSoldierModels) this.Tank_DeathDriverCorpse = this.Tank_DeathSoldierModels 
        //
        PhysicsInit(SOLID_VPHYSICS) // SOLID_BBOX;
        //// SetSolid removed: SOLID_VPHYSICS
        Transform.Rotation = (Transform.Rotation + Angle(0, -this.Tank_AngleOffset, 0).ToRotation());
        Collider.SetBounds(Vector(this.Tank_CollisionBoundSize, this.Tank_CollisionBoundSize, this.Tank_CollisionBoundUp), Vector(-this.Tank_CollisionBoundSize, -this.Tank_CollisionBoundSize, this.Tank_CollisionBoundDown));

        var phys = Rigidbody;
        if (phys.IsValid())
        phys.Wake();
        phys.SetMass(30000);


        // Create the gunner NPC
        if (this.Tank_GunnerENT)
        var gunner = SceneUtility.CreatePrefab();
        if (gunner.IsValid())
        gunner.SetPos(Tank_GunnerSpawnPosition());
        gunner.SetAngles(Transform.Rotation);
        gunner.SetOwner(self);
        gunner.SetParent(self);
        gunner.DoNotDuplicate = true  // Otherwise you will have double gunners;
        gunner.VJ_NPC_Class = this.VJ_NPC_Class;
        gunner.Spawn();
        gunner.Activate();
        this.Gunner = gunner;


    }

    public virtual void OnTouch(ent)
    {
        if (!VJ_CVAR_AI_ENABLED) return 
        if (this.Tank_Status == 0)
        Tank_RunOver(ent);

    }

    public virtual void Tank_RunOver(ent)
    {
        if (!this.Tank_IsMoving || !ent.IsValid() || (vj_npc_melee.GetInt() == 0 ) || (ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled)) return 
        if (Disposition(ent) == D_HT && ent.Health() > 0 && !ent.VJ_ID_Boss && !ent.VJ_ID_Vehicle && !ent.VJ_ID_Aircraft && ((ent.IsNPC() && !runoverException[ent.GetClass()]) || (ent.IsPlayer() && !VJ_CVAR_IGNOREPLAYERS) || ent.IsNextBot()))
        Tank_OnRunOver(ent);
        Tank_PlaySoundSystem("RunOver");
        ent.TakeDamage(ScaleByDifficulty(8), self, self);
        DamageHelper.Special(self, ent, null);
        ent.SetVelocity(ent.GetForward() * -200);

    }

    public virtual void OnThink()
    {
        if (Tank_OnThink() != true && vj_npc_reduce_vfx.GetInt() == 0)
        var selfData = funcGetTable(self);
        if (selfData.Tank_NextIdleParticles < Time.Now)
        Tank_UpdateIdleParticles();
        selfData.Tank_NextIdleParticles = Time.Now + 0.1;


        if (Health() < (selfData.StartHealth * 0.30) && Time.Now > selfData.Tank_NextLowHealthSparkT)
        //Particles.Attach("vj_rocket_idle2_smoke2", PATTACH_ABSORIGIN_FOLLOW, self, 0)

        selfData.Spark1 = SceneUtility.CreatePrefab();
        selfData.Spark1.SetKeyValue("MaxDelay", 0.01);
        selfData.Spark1.SetKeyValue("Magnitude", "8");
        selfData.Spark1.SetKeyValue("Spark Trail Length", "3");
        GetNearDeathSparkPositions();
        selfData.Spark1.SetAngles(Transform.Rotation);
        //selfData.Spark1.Fire("LightColor", "255 255 255")
        selfData.Spark1.SetParent(self);
        selfData.Spark1.Spawn();
        selfData.Spark1.Activate();
        selfData.Spark1.Fire("StartSpark");
        selfData.Spark1.Fire("kill", null, 0.1);
        DeleteOnRemove(selfData.Spark1);


        selfData.Tank_NextLowHealthSparkT = Time.Now + Game.Random.NextInt(4, 6);


    }

    public virtual void OnThinkActive()
    {
        var selfData = funcGetTable(self);
        if (selfData.Dead) return 
        selfData.TurnData.Type = FACE_NONE  // This effectively makes it never face anything through Lua;
        Tank_OnThinkActive();
        SelectSchedule();

        var hasMoved = false;
        var myPos = Transform.Position;
        var tr = SceneTrace.Ray({start = myPos + Transform.Up * 20, endpos = myPos + Transform.Up * -50, filter = self}).Run();
        if (selfData.VJ_DEBUG)
        DebugOverlay.Cross(tr.StartPos, 4, 2, VJ.COLOR_GREEN, true);
        DebugOverlay.Cross(myPos + Transform.Up * -50, 4, 2, VJ.COLOR_YELLOW, true);
        DebugOverlay.Cross(tr.HitPos, 4, 2, VJ.COLOR_RED, true);
        DebugOverlay.Line(tr.StartPos, tr.HitPos, 2, null, true);
        // VJ.DEBUG: self, false, "Tank Status = ", selfData.Tank_Status, " | Trace HitNormal = ", tr.HitNormal

        if (tr.Hit && selfData.Tank_Status == 0)
        var phys = Rigidbody;
        if (phys.IsValid() && phys.Count.GetFrictionSnapshot() > 0)
        var eneData = selfData.EnemyData;
        var ene = eneData.Target;
        if (ene.IsValid())
        var plyControlled = selfData.VJ_IsBeingControlled;
        var enePos = ene.GetPos();
        var angEne = (enePos - myPos + vec80z):Angle();
        var angDiffuse = Tank_AngleDiffuse(angEne.y, Transform.Rotation.y + selfData.Tank_AngleOffset);
        var heightRatio = plyControlled && 1 || ((enePos.z - myPos.z) / myPos.Distance(Vector(enePos.x, enePos.y, myPos.z)));
        var enemyIsHighUp = heightRatio > 0.15;
        // If the enemy is very high up, then move away from it to help the gunner fire!
        // OR
        // If the enemy's height isn't very high AND the enemy is (within run over distance OR far away), then move towards the enemy!
        if (enemyIsHighUp || (heightRatio < 0.15 && ((eneData.Distance < selfData.Tank_RanOverDistance) || (eneData.Distance > selfData.Tank_DriveTowardsDistance))))
        // Turning
        if (plyControlled)
        var reverse = selfData.VJ_TheController.KeyDown(IN_BACK) && -1 || 1  // If we are reversing, then turn the opposite way to make it easier for the player to control;
        if (selfData.VJ_TheController.KeyDown(IN_MOVERIGHT))
        LocalTransform.Rotation = (LocalTransform.Rotation + Angle(0, -selfData.Tank_TurningSpeed * reverse, 0).ToRotation());
        phys.SetAngles(Transform.Rotation);
        else if (selfData.VJ_TheController.KeyDown(IN_MOVELEFT))
        LocalTransform.Rotation = (LocalTransform.Rotation + Angle(0, selfData.Tank_TurningSpeed * reverse, 0).ToRotation());
        phys.SetAngles(Transform.Rotation);

        else;
        if (angDiffuse > 15)
        LocalTransform.Rotation = (LocalTransform.Rotation + Angle(0, selfData.Tank_TurningSpeed, 0).ToRotation());
        phys.SetAngles(Transform.Rotation);
        else if (angDiffuse < -15)
        LocalTransform.Rotation = (LocalTransform.Rotation + Angle(0, -selfData.Tank_TurningSpeed, 0).ToRotation());
        phys.SetAngles(Transform.Rotation);



        // Movement : Have a little grace zone so it doesn't constantly switch between forward and backwards driving
        if (enemyIsHighUp || heightRatio < 0.1490)
        var driveSpeed = selfData.Tank_DrivingSpeed;
        var moveVel = Transform.Forward;
        moveVel.Rotate(Angle(0, selfData.Tank_AngleOffset, 0));

        // Increase speed based on how steep the slope is
        var slopeFactor = tr.HitNormal.z;
        if (slopeFactor < 1)
        driveSpeed = driveSpeed * (1.1 + (1 - slopeFactor));


        if (plyControlled)
        // Increase speed if the player is holding the sprint key
        if (selfData.VJ_TheController.KeyDown(IN_SPEED))
        driveSpeed = driveSpeed * 1.8;

        // Reverse if player is holding the back key
        if (selfData.VJ_TheController.KeyDown(IN_BACK))
        driveSpeed = -driveSpeed;

        else;
        // Move away instead of towards the enemy!
        if (enemyIsHighUp)
        driveSpeed = -driveSpeed;



        if (selfData.VJ_DEBUG) // VJ.DEBUG: self, false, "Driving Speed = ", driveSpeed 
        phys.SetVelocity(moveVel.GetNormal() * driveSpeed);
        hasMoved = true;




        if (hasMoved || phys.GetVelocity():Length() > 10)
        hasMoved = true;
        selfData.Tank_IsMoving = true;
        Tank_PlaySoundSystem("Movement");
        Tank_UpdateMoveParticles();




        // Not moving
        if (!hasMoved)
        selfData.CurrentTankMovingSound?.Stop();
        selfData.CurrentTankTrackSound?.Stop();
        selfData.Tank_IsMoving = false;


        for _, v in Scene.FindInPhysics(myPos, 100) do
        Tank_RunOver(v);

    }

    public virtual void SelectSchedule()
    {
        var selfData = funcGetTable(self);
        if (selfData.Dead) return 

        var eneValid = Enemy?.GameObject.IsValid();
        PlayIdleSound(null, null, eneValid);
        MaintainIdleBehavior();

        if (eneValid)
        if (selfData.VJ_IsBeingControlled)
        if (selfData.VJ_TheController.KeyDown(IN_FORWARD) || selfData.VJ_TheController.KeyDown(IN_BACK))
        selfData.Tank_Status = 0;
        else;
        selfData.Tank_Status = 1;

        else;
        var eneData = selfData.EnemyData;
        if ((eneData.Distance < selfData.Tank_DriveTowardsDistance && eneData.Distance > selfData.Tank_DriveAwayDistance) || selfData.IsGuard)  // If between this two numbers, stay still
        selfData.Tank_Status = 1;
        else;
        selfData.Tank_Status = 0;


        else;
        selfData.Tank_Status = 1;

    }

    public virtual void OnDeath(dmginfo, hitgroup, status)
    {
        if (status == "Init")
        if (this.Gunner.IsValid())
        this.Gunner.Dead = true;
        if (IsOnFire) this.Gunner.Ignite(Game.Random.NextFloat(8, 10), 0) 


        if (Tank_OnInitialDeath(dmginfo, hitgroup) != true)
        for i=0, 1.5, 0.5 do
        GameTask.DelaySeconds(i).ContinueWith(_ => function();
        if (this.IsValid())
        var myPos = Transform.Position;
        SoundManager.Emit(self, "VJ.Explosion");
        BlastDamage(self, self, myPos, 200, 40);
        ScreenShake(myPos, 100, 200, 1, 2500);
        if (this.HasGibOnDeathEffects) Particles.Play("vj_explosion2", myPos, defAng) 

        end);



    }

    public virtual void OnCreateDeathCorpse(dmginfo, hitgroup, corpse)
    {
        // Spawn the gunner corpse
        if (this.Gunner.IsValid())
        this.Gunner.SavedDmgInfo = this.SavedDmgInfo;
        var gunCorpse = this.Gunner.CreateDeathCorpse(dmginfo, hitgroup);
        if (gunCorpse.IsValid()) corpse.ChildEnts[corpse.Count.ChildEnts + 1] = gunCorpse 


        if (Tank_OnDeathCorpse(dmginfo, hitgroup, corpse, "Override") != true)
        var myPos = Transform.Position;
        SoundManager.Emit(self, "VJ.Explosion");
        BlastDamage(self, self, myPos, 400, 40);
        ScreenShake(myPos, 100, 200, 1, 2500);
        var tr = util.TraceLine({
        start = myPos + Transform.Up * 4,;
        endpos = myPos - vec500z,;
        filter = self;
        });
        Decals.Place(Game.Random.FromList(this.Tank_DeathDecal), tr.HitPos + tr.HitNormal, tr.HitPos - tr.HitNormal);

        // Create soldier corpse
        if (Game.Random.NextInt(1, this.Tank_DeathDriverCorpseChance) == 1)
        var soldierMDL = Game.Random.FromList(this.Tank_DeathDriverCorpse);
        if (soldierMDL)
        CreateExtraDeathCorpse("prop_ragdoll", soldierMDL, {Pos = myPos + Transform.Up * 90 + Transform.Right * -30, Vel = Vector(Game.Random.NextFloat(-600, 600), Game.Random.NextFloat(-600, 600), 500)}, function(ent);
        ent.Ignite(Game.Random.NextFloat(8, 10), 0);
        ent.SetColor(colorGray);
        Tank_OnDeathCorpse(dmginfo, hitgroup, corpse, "Soldier", ent);
        end);



        // Effects / Particles
        if (this.HasGibOnDeathEffects && Tank_OnDeathCorpse(dmginfo, hitgroup, corpse, "Effects") != true)
        Particles.Play("vj_explosion3", myPos, defAng);
        Particles.Play("vj_explosion2", myPos + Transform.Forward*-130, defAng);
        Particles.Play("vj_explosion2", myPos + Transform.Forward*130, defAng);
        Particles.Attach("smoke_burning_engine_01", PATTACH_ABSORIGIN_FOLLOW, corpse, 0);
        var effectData = new EffectData();
        effectData.SetOrigin(myPos);
        Effects.Play("VJ_Medium_Explosion1", effectData);
        effectData.SetScale(800);
        Effects.Play("ThumperDust", effectData);


    }

    public virtual void OnCustomRemove()
    {
        this.CurrentTankMovingSound?.Stop();
        this.CurrentTankTrackSound?.Stop();
        if (this.Gunner.IsValid())
        this.Gunner.Remove();

    }

    public virtual void Tank_PlaySoundSystem(sdSet)
    {
        var selfData = funcGetTable(self);
        if (!selfData.HasSounds || !sdSet) return 
        if (sdSet == "Movement")
        if (selfData.HasMoveSound)
        // Movement sound
        var curMoveSD = selfData.CurrentTankMovingSound;
        if (!curMoveSD || (curMoveSD && !curMoveSD.IsPlaying()))
        curMoveSD?.Stop();
        selfData.CurrentTankMovingSound = SoundManager.CreateHandle(self, Game.Random.FromList(selfData.Tank_SoundTbl_DrivingEngine) || "vj_base/vehicles/armored/engine_drive.wav", 80, 100);

        // Track sound
        var curTrackSD = selfData.CurrentTankTrackSound;
        if (!curTrackSD || (curTrackSD && !curTrackSD.IsPlaying()))
        curTrackSD?.Stop();
        selfData.CurrentTankTrackSound = SoundManager.CreateHandle(self, Game.Random.FromList(selfData.Tank_SoundTbl_Track) || "vj_base/vehicles/armored/chassis_tracks.wav", 70, 100);


        else if (sdSet == "RunOver")
        if (selfData.HasRunOverSound && Time.Now > selfData.Tank_NextRunOverSoundT)
        Sound.Play(Game.Random.FromList(selfData.Tank_SoundTbl_RunOver, Transform.Position) || "VJ.Gib.Bone_Snap", 80, Game.Random.NextInt(80, 100));
        selfData.Tank_NextRunOverSoundT = Time.Now + 0.2;


    }

    public virtual void PhysicsCollide(data, physobj)
    {
    }

    public virtual void PhysicsUpdate(physobj)
    {
    }

}