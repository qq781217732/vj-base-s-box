using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class NpcVjTankgBase : CreatureNPC
{
    [Property] public int StartHealth = 0;
    [Property] public object MovementType = VJ_MOVETYPE_STATIONARY;
    [Property] public bool CanTurnWhileStationary = false;
    [Property] public bool GodMode = true;
    [Property] public bool EnemyDetection = false;
    [Property] public int Tank_AngleOffset = 0;
    [Property] public int Tank_AngleDiffuseFiringLimit = 5;
    [Property] public int Tank_TurningSpeed = 5;
    [Property] public bool Tank_HasShellAttack = true;
    [Property] public int Tank_Shell_FireMin = 350;
    [Property] public object Tank_Shell_FireMax = ENT.SightDistance;
    [Property] public int Tank_Shell_NextFireTime = 0;
    [Property] public float Tank_Shell_TimeUntilFire = 2.5;
    [Property] public Vector3 Tank_Shell_SpawnPos = Vector(-170, 0, 65);
    [Property] public string Tank_Shell_Entity = "obj_vj_rocket";
    [Property] public int Tank_Shell_VelocitySpeed = 4000;
    [Property] public Vector3 Tank_Shell_MuzzleFlashPos = Vector(0, -235, 18);
    [Property] public Vector3 Tank_Shell_ParticlePos = Vector(-205, 0, 72);
    [Property] public bool HasMoveSound = true;
    [Property] public bool Tank_SoundTbl_Turning = false;
    [Property] public int Tank_TurningSoundLevel = 80;
    [Property] public object Tank_TurningSoundPitch = VJ.SET(100, 100);
    [Property] public bool HasReloadShellSound = true;
    [Property] public bool Tank_SoundTbl_ReloadShell = false;
    [Property] public int Tank_ReloadShellSoundLevel = 75;
    [Property] public object Tank_ReloadShellSoundPitch = VJ.SET(90, 100);
    [Property] public bool HasFireShellSound = true;
    [Property] public bool Tank_SoundTbl_FireShell = false;
    [Property] public int Tank_FireShellSoundLevel = 140;
    [Property] public object Tank_FireShellSoundPitch = VJ.SET(90, 100);
    [Property] public bool Tank_FacingTarget = false;
    [Property] public bool Tank_ReachableHeight = false;
    [Property] public int Tank_Status = 0;
    [Property] public int Tank_Shell_NextFireT = 0;
    [Property] public object Tank_TurningLerp = null;
    [Property] public int Tank_NextIdleParticles = 0;
    [Property] public object Tank_Shell_Status = TANK_SHELL_STATUS_EMPTY;
    [Property] public string Type = "ai";
    [Property] public string PrintName = "VJ Base Tank Gunner";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool IsVJBaseSNPC_Tank = true;
    [Property] public bool IsVJBaseSNPC_TankGun = true;
    [Property] public bool VJ_ID_Vehicle = true;

    public virtual void Tank_Init()
    {
    }

    public virtual void Tank_OnThink()
    {
    }

    public virtual void Tank_OnThinkActive()
    {
    }

    public virtual void Tank_OnPrepareShell()
    {
    }

    public virtual void Tank_OnFireShell(status, statusData)
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

    public virtual void OnInit()
    {
        // SetSolid removed: SOLID_NONE
        this.Tank_NextIdleParticles = Time.Now + 1;
        this.DeathAnimationCodeRan = true  // So corpse doesn't fly away on death (Take this out if !using death explosion sequence);
        SetPhysicsDamageScale(0)  // Take no physics damage;
        if (vj_npc_range.GetInt() == 0) this.Tank_HasShellAttack = false 
        Tank_Init();
        if (this.CustomInitialize_CustomTank) CustomInitialize_CustomTank() end  // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
    }

    public virtual void OnThink()
    {
        if (Tank_OnThink() != true && vj_npc_reduce_vfx.GetInt() == 0 && this.Tank_NextIdleParticles < Time.Now)
        Tank_UpdateIdleParticles();
        this.Tank_NextIdleParticles = Time.Now + 0.1;

    }

    public virtual void OnThinkActive()
    {
        var selfData = funcGetTable(self);
        if (selfData.Dead) return 
        var parent = GetParent();
        if (!parent.IsValid()) return 
        if (selfData.VJ_NPC_Class != parent.VJ_NPC_Class)
        selfData.VJ_NPC_Class = parent.VJ_NPC_Class;

        var turning = false;
        var ene = parent.GetEnemy();
        Enemy = ene;
        Tank_OnThinkActive();
        SelectSchedule();

        if (selfData.Tank_Status == 0)
        if (ene.IsValid())
        turning = false;
        var myPos = Transform.Position;
        var enePos = ene.GetPos();
        var angEne = (enePos - myPos):Angle();
        var angDiffuse = Tank_AngleDiffuse(angEne.y, Transform.Rotation.y + selfData.Tank_AngleOffset)  // Cannon looking direction;
        var heightRatio = (enePos.z - myPos.z) / myPos.Distance(Vector(enePos.x, enePos.y, myPos.z));
        selfData.Tank_ReachableHeight = math.abs(heightRatio) < 0.15 && true || false  // How high it can fire;
        // If the enemy is within the barrel firing limit AND not already firing a shell AND its height is is reachable AND the enemy is not extremely close, then FIRE!
        if (math.abs(angDiffuse) < selfData.Tank_AngleDiffuseFiringLimit && selfData.Tank_ReachableHeight && selfData.EnemyData.Distance > selfData.Tank_Shell_FireMin)
        selfData.Tank_FacingTarget = true;
        if (this.Tank_HasShellAttack && Senses.CanSee(ene))
        Tank_PrepareShell();

        // Turn Left
        else if (angDiffuse > selfData.Tank_AngleDiffuseFiringLimit)
        if (selfData.Tank_TurningLerp == null) selfData.Tank_TurningLerp = LocalTransform.Rotation 
        selfData.Tank_TurningLerp = LerpAngle(1, selfData.Tank_TurningLerp, selfData.Tank_TurningLerp + Angle(0, Math.Clamp(angDiffuse, 0, selfData.Tank_TurningSpeed), 0));
        LocalTransform.Rotation = (selfData.Tank_TurningLerp).ToRotation();
        turning = true;
        selfData.Tank_FacingTarget = false;
        // Turn Right
        else if (angDiffuse < -selfData.Tank_AngleDiffuseFiringLimit)
        if (selfData.Tank_TurningLerp == null) selfData.Tank_TurningLerp = LocalTransform.Rotation 
        selfData.Tank_TurningLerp = LerpAngle(1, selfData.Tank_TurningLerp, selfData.Tank_TurningLerp + Angle(0, -Math.Clamp(math.abs(angDiffuse), 0, selfData.Tank_TurningSpeed), 0));
        LocalTransform.Rotation = (selfData.Tank_TurningLerp).ToRotation();
        turning = true;
        selfData.Tank_FacingTarget = false;

        else;
        selfData.Tank_Status = 1;
        turning = false;



        if (turning) Tank_PlaySoundSystem("Movement") else selfData.CurrentTankMovingSound?.Stop() 
    }

    public virtual void SelectSchedule()
    {
        var selfData = funcGetTable(self);
        if (selfData.Dead) return 

        var eneValid = Enemy?.GameObject.IsValid();
        PlayIdleSound(null, null, eneValid);
        MaintainIdleBehavior();

        if (eneValid)
        // Can always fire when being controlled
        if (GetParent().VJ_IsBeingControlled)
        selfData.Tank_Status = 0;
        else;
        // Between these 2 limits it can fire! --
        var eneData = selfData.EnemyData;
        if (eneData.Distance < selfData.Tank_Shell_FireMax && eneData.Distance > selfData.Tank_Shell_FireMin)
        selfData.Tank_Status = 0;
        // Out of range, can't fire!
        else;
        selfData.Tank_Status = 1;



    }

    public virtual void Tank_PrepareShell()
    {
        if ((Time.Now < this.Tank_Shell_NextFireT) || (GetParent().VJ_IsBeingControlled && !GetParent().VJ_TheController.KeyDown(IN_ATTACK2))) return 

        // If it's already ready, then just fire it!
        if (this.Tank_Shell_Status == TANK_SHELL_STATUS_READY)
        Tank_FireShell();
        // Otherwise reload and fire
        else if (this.Tank_Shell_Status == TANK_SHELL_STATUS_EMPTY)
        Tank_OnPrepareShell();
        Tank_PlaySoundSystem("ShellReload");
        this.Tank_Shell_Status = TANK_SHELL_STATUS_RELOADING;
        var ene = Enemy?.GameObject;
        if (!ene.IsNPC() || (ene.IsNPC() && ene.GetEnemy() == GetParent()))  // Don't run away when you don't even know that the tank exists!
        sound.EmitHint(SOUND_DANGER, ene.GetPos() + ene.OBBCenter(), 80, this.Tank_Shell_TimeUntilFire, self);

        TimerLoop("timer_shell_attack" + EntIndex(), this.Tank_Shell_TimeUntilFire, 1, () => function();
        this.Tank_Shell_Status = TANK_SHELL_STATUS_READY;
        Tank_FireShell();
        end);

    }

    public virtual void Tank_FireShell()
    {
        var selfData = funcGetTable(self);
        var ene = Enemy?.GameObject;
        if (!VJ_CVAR_AI_ENABLED || selfData.Dead || !selfData.Tank_ReachableHeight || !selfData.Tank_FacingTarget || !ene.IsValid()) return end // selfData.Tank_FacingTarget != true
        if (Senses.CanSee(ene))
        Tank_PlaySoundSystem("ShellFire");

        if (Tank_OnFireShell("Init") != true)
        var shell = SceneUtility.CreatePrefab();
        var spawnPos;
        var onCreateCall = Tank_OnFireShell("OnCreate", shell);
        if (isvector(onCreateCall))
        spawnPos = onCreateCall;
        else;
        spawnPos = LocalToWorld(selfData.Tank_Shell_SpawnPos);

        var calculatedVel = (ene.GetPos() + ene.OBBCenter() - spawnPos):GetNormal()*selfData.Tank_Shell_VelocitySpeed;
        // If not facing, then just shoot straight ahead
        if (!selfData.Tank_FacingTarget)
        calculatedVel = Transform.Forward;
        calculatedVel.Rotate(Angle(0, selfData.Tank_AngleOffset, 0));
        calculatedVel = calculatedVel * selfData.Tank_Shell_VelocitySpeed;

        shell.SetPos(spawnPos);
        shell.SetAngles(calculatedVel.Angle());
        shell.Spawn();
        shell.Activate();
        shell.SetOwner(self);
        if (Tank_OnFireShell("OnSpawn", shell) != true)
        var phys = shell.GetPhysicsObject();
        if (phys.IsValid())
        phys.SetVelocity(calculatedVel);



        if (Tank_OnFireShell("Effects") != true)
        var myAng = Transform.Rotation;
        var myAngForward = myAng + Angle(0, selfData.Tank_AngleOffset, 0);
        ScreenShake(Transform.Position, 100, 200, 1, 2500);

        // Muzzle flash
        var muzzleFlashPos = LocalToWorld(selfData.Tank_Shell_MuzzleFlashPos);
        var muzzleFlash = SceneUtility.CreatePrefab();
        muzzleFlash.SetPos(muzzleFlashPos);
        muzzleFlash.SetAngles(myAngForward);
        muzzleFlash.SetKeyValue("scale", "10");
        muzzleFlash.Fire("Fire");
        var lightFire = SceneUtility.CreatePrefab();
        lightFire.SetKeyValue("brightness", "4");
        lightFire.SetKeyValue("distance", "400");
        lightFire.SetPos(muzzleFlashPos);
        lightFire.SetLocalAngles(myAng);
        lightFire.Fire("Color", "255 150 60");
        lightFire.SetParent(self);
        lightFire.Spawn();
        lightFire.Activate();
        lightFire.Fire("TurnOn");
        lightFire.Fire("Kill", null, 0.1);
        DeleteOnRemove(lightFire);

        // Smoke effect
        var smokePos = LocalToWorld(selfData.Tank_Shell_ParticlePos);
        var smokeWhite = SceneUtility.CreatePrefab();
        smokeWhite.SetKeyValue("effect_name", "vj_smoke_white_medium");
        smokeWhite.SetPos(smokePos);
        smokeWhite.SetAngles(myAngForward);
        smokeWhite.SetParent(self);
        smokeWhite.Spawn();
        smokeWhite.Activate();
        smokeWhite.Fire("Start");
        smokeWhite.Fire("Kill", null, 6);

        // Dust effect
        var dust = new EffectData();
        dust.SetOrigin(GetParent():GetPos());
        dust.SetScale(800);
        Effects.Play("ThumperDust", dust);

        //local smoke = SceneUtility.CreatePrefab()
        //smoke.SetPos(LocalToWorld(selfData.Tank_Shell_ParticlePos))
        //smoke.SetAngles(myAngForward)
        //smoke.SetKeyValue("opacity", "1")
        //smoke.SetKeyValue("spawnrate", "15")
        //smoke.SetKeyValue("lifetime", "5")
        //smoke.SetKeyValue("startsize", "1")
        //smoke.SetKeyValue("endsize", "50")
        //smoke.SetKeyValue("spawnradius", "5")
        //smoke.SetKeyValue("startcolor", "255 255 255 255")
        //smoke.SetKeyValue("endcolor", "255 255 255 255")
        //smoke.SetKeyValue("minspeed", "30")
        //smoke.SetKeyValue("maxspeed", "50")
        //smoke.SetKeyValue("mindirectedspeed", "50")
        //smoke.SetKeyValue("maxdirectedspeed", "75")
        //smoke.SetParent(self)
        //smoke.Spawn()
        //smoke.Activate()
        //smoke.Fire("Kill", null, 4)

        selfData.Tank_Shell_Status = TANK_SHELL_STATUS_EMPTY;
        selfData.Tank_Shell_NextFireT = Time.Now + selfData.Tank_Shell_NextFireTime;
        else  // Not visible
        selfData.Tank_FacingTarget = false;

    }

    public virtual void OnCreateDeathCorpse(dmginfo, hitgroup, corpse)
    {
        var corpsePhys = corpse.GetPhysicsObject();
        if (corpsePhys.IsValid())
        corpsePhys.AddVelocity(Vector(Game.Random.NextFloat(-200, 200), Game.Random.NextFloat(-200, 200), Game.Random.NextFloat(200, 400)));
        corpsePhys.AddAngleVelocity(Vector(Game.Random.NextFloat(-100, 100), Game.Random.NextFloat(-100, 100), Game.Random.NextFloat(-100, 100)));

    }

    public virtual void OnCustomRemove()
    {
        this.CurrentTankMovingSound?.Stop();
        timer.Destroy("timer_shell_attack" + EntIndex());
    }

    public virtual void Tank_PlaySoundSystem(sdSet)
    {
        var selfData = funcGetTable(self);
        if (!selfData.HasSounds || !sdSet) return 
        if (sdSet == "Movement")
        if (selfData.HasMoveSound)
        var curMoveSD = selfData.CurrentTankMovingSound;
        if (!curMoveSD || (curMoveSD && !curMoveSD.IsPlaying()))
        curMoveSD?.Stop();
        selfData.CurrentTankMovingSound = SoundManager.CreateHandle(self, Game.Random.FromList(selfData.Tank_SoundTbl_Turning) || "vj_base/vehicles/armored/gun_move2.wav", selfData.Tank_TurningSoundLevel, Game.Random.NextInt(selfData.Tank_TurningSoundPitch.a, selfData.Tank_TurningSoundPitch.b));


        else if (sdSet == "ShellFire")
        if (selfData.HasFireShellSound)
        SoundManager.Emit(self, Game.Random.FromList(selfData.Tank_SoundTbl_FireShell) || "VJ.NPC_Tank.Fire", selfData.Tank_FireShellSoundLevel, Game.Random.NextInt(selfData.Tank_FireShellSoundPitch.a, selfData.Tank_FireShellSoundPitch.b));

        else if (sdSet == "ShellReload")
        if (selfData.HasReloadShellSound)
        SoundManager.Emit(self, Game.Random.FromList(selfData.Tank_SoundTbl_ReloadShell) || "vj_base/vehicles/armored/gun_reload.wav", selfData.Tank_ReloadShellSoundLevel, Game.Random.NextInt(selfData.Tank_ReloadShellSoundPitch.a, selfData.Tank_ReloadShellSoundPitch.b));


    }

}