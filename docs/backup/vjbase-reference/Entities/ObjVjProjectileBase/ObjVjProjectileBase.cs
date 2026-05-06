using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjProjectileBase : BaseProjectile
{
    [Property] public bool Model = false;
    [Property] public object ProjectileType = VJ.PROJ_TYPE_LINEAR;
    [Property] public object CollisionBehavior = VJ.PROJ_COLLISION_REMOVE;
    [Property] public bool CollisionFilter = true;
    [Property] public bool CollisionDecal = false;
    [Property] public int RemoveDelay = 0;
    [Property] public bool DoesRadiusDamage = false;
    [Property] public int RadiusDamageRadius = 250;
    [Property] public bool RadiusDamageUseRealisticRadius = true;
    [Property] public int RadiusDamage = 30;
    [Property] public object RadiusDamageType = DMG_BLAST;
    [Property] public bool RadiusDamageForce = false;
    [Property] public bool RadiusDamageForce_Up = false;
    [Property] public bool RadiusDamageDisableVisibilityCheck = false;
    [Property] public bool DoesDirectDamage = false;
    [Property] public int DirectDamage = 30;
    [Property] public object DirectDamageType = DMG_SLASH;
    [Property] public bool HasStartupSounds = true;
    [Property] public bool HasIdleSounds = true;
    [Property] public bool HasOnCollideSounds = true;
    [Property] public bool HasOnRemoveSounds = true;
    [Property] public bool SoundTbl_Startup = false;
    [Property] public bool SoundTbl_Idle = false;
    [Property] public bool SoundTbl_OnCollide = false;
    [Property] public bool SoundTbl_OnRemove = false;
    [Property] public int StartupSoundChance = 1;
    [Property] public int IdleSoundChance = 1;
    [Property] public int OnCollideSoundChance = 1;
    [Property] public int OnRemoveSoundChance = 1;
    [Property] public object NextSoundTime_Idle = VJ.SET(0.2, 0.5);
    [Property] public int StartupSoundLevel = 80;
    [Property] public int IdleSoundLevel = 80;
    [Property] public int OnCollideSoundLevel = 80;
    [Property] public int OnRemoveSoundLevel = 90;
    [Property] public object StartupSoundPitch = VJ.SET(90, 100);
    [Property] public object IdleSoundPitch = VJ.SET(90, 100);
    [Property] public object OnCollideSoundPitch = VJ.SET(90, 100);
    [Property] public object OnRemoveSoundPitch = VJ.SET(90, 100);
    [Property] public bool Dead = false;
    [Property] public int NextIdleSoundT = 0;
    [Property] public bool PaintedFinalDecal = false;
    [Property] public int NextPersistCollisionT = 0;
    [Property] public string Type = "anim";
    [Property] public string PrintName = "VJ Base Projectile";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool IsVJBaseProjectile = true;

    public virtual void PreInit()
    {
    }

    public virtual void InitPhys()
    {
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnThink()
    {
    }

    public virtual void OnDamaged(dmginfo)
    {
    }

    public virtual void OnCollision(data, phys)
    {
    }

    public virtual void OnCollisionPersist(data, phys)
    {
    }

    public virtual void OnDealDamage(data, phys, hitEnts)
    {
    }

    public virtual void OnDestroy(data, phys)
    {
    }

    public virtual void OnCustomRemove()
    {
    }

    public virtual void OnInitialize()
    {
        PreInit();
        if (this.CustomOnPreInitialize) CustomOnPreInitialize() end  // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (GetModel() == "models/error.mdl" && PICK(this.Model)) ModelRenderer.Model = PICK(this.Model) 

        var projType = this.ProjectileType;
        // Some models do NOT have a physics mesh, so let's initialize a basic sphere physics
        if (!PhysicsInit(MOVETYPE_VPHYSICS))
        var boundsMin, boundsMax = GetModelRenderBounds();
        var radius = (boundsMax - boundsMin):Length() * 0.5;
        PhysicsInitSphere(radius, "metal_bouncy");

        // SetMoveType removed: MOVETYPE_VPHYSICS
        SetMoveCollide(MOVECOLLIDE_FLY_BOUNCE);
        // SetSolid removed: SOLID_BBOX

        if (this.CustomOnInitializeBeforePhys) CustomOnInitializeBeforePhys() end  // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (this.CustomPhysicsObjectOnInitialize) local phys = Rigidbody if (phys.IsValid()) this.InitPhys = function() return true end CustomPhysicsObjectOnInitialize(phys) end 
        if (!InitPhys())
        var phys = Rigidbody;
        if (phys.IsValid())
        phys.Wake();
        if (projType == VJ.PROJ_TYPE_LINEAR)
        phys.SetMass(1);
        phys.EnableGravity(false);
        phys.EnableDrag(false);
        else if (projType == VJ.PROJ_TYPE_GRAVITY)
        phys.SetMass(1);
        phys.EnableGravity(true);
        phys.EnableDrag(false);
        else if (projType == VJ.PROJ_TYPE_PROP)
        phys.EnableGravity(true);
        phys.EnableDrag(true);

        //phys.AddGameFlag(FVPHYSICS_NO_IMPACT_DMG)
        phys.SetBuoyancyRatio(0);



        if (projType == VJ.PROJ_TYPE_PROP)
        Collider.CollisionGroup = COLLISION_GROUP_NONE;
        else;
        SetTrigger(true);
        // Set a trigger bound for models that do NOT have a physics mesh, otherwise they will not hit collision-based entities (ex: NPCs, players)
        if (!util.IsValidProp(GetModel()))
        var boundsMin, boundsMax = GetModelRenderBounds();
        var radius = math.max(2, (boundsMax - boundsMin):Length() * 0.05);
        UseTriggerBounds(true, radius);

        Collider.CollisionGroup = COLLISION_GROUP_PROJECTILE;

        AddEFlags(EFL_DONTBLOCKLOS);
        AddEFlags(EFL_DONTWALKON);
        AddSolidFlags(FSOLID_NOT_STANDABLE);
        SetUseType(SIMPLE_USE);
        PlaySound("Startup");
        Init();

        // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (this.IdleSoundPitch1) this.IdleSoundPitch = new Vector2(this.IdleSoundPitch1, this.IdleSoundPitch2) 
        if (this.CustomOnInitialize) CustomOnInitialize() 
        if (this.CustomOnThink) this.OnThink = function() CustomOnThink() end 
        if (this.CustomOnPhysicsCollide) this.OnCollision = function(_, data, phys2) CustomOnPhysicsCollide(data, phys2) end 
        if (this.CustomOnCollideWithoutRemove) this.OnCollisionPersist = function(_, data, phys2) CustomOnCollideWithoutRemove(data, phys2) end 
        if (this.DeathEffects) this.OnDestroy = function(_, data, phys2) DeathEffects(data, phys2) end 
        if (this.CustomOnDoDamage) this.OnDealDamage = function(_, data, phys2, hitEnts) CustomOnDoDamage(data, phys2, hitEnts) end 
        if (this.CustomOnDoDamage_Direct) this.OnDealDamage = function(_, data, phys2, hitEnts) CustomOnDoDamage_Direct(data, phys2, hitEnts && hitEnts[1] || null) end 
        if (this.DecalTbl_OnCollideDecals) this.CollisionDecal = this.DecalTbl_OnCollideDecals 
        if (this.DecalTbl_DeathDecals) this.CollisionDecal = this.DecalTbl_DeathDecals 
        if (this.RemoveOnHit) this.CollisionBehavior = PROJ_COLLISION_REMOVE 
        if (this.CollideCodeWithoutRemoving) this.CollisionBehavior = PROJ_COLLISION_PERSIST 
        //
    }

    public virtual void Think()
    {
        if (this.Dead) this.CurrentIdleSound?.Stop() return 
        //Transform.Rotation = (Rigidbody.Velocity.GetNormal().ToRotation():Angle())
        OnThink();
        PlaySound("Idle");
    }

    public virtual void OnTakeDamage(dmginfo)
    {
        OnDamaged(dmginfo);
    }

    public virtual void DealDamage(data, phys)
    {
        var owner = Owner;
        var ownerValid = owner.IsValid();
        var dataEnt = data && data.HitEntity;
        var hitEnts = false  // Entities that have been damaged (direct || radius);
        var dmgPos = (data && data.HitPos) || Transform.Position;
        if (dataEnt.IsValid() && ((dataEnt.IsVJBaseBullseye && dataEnt.VJ_IsBeingControlled) || dataEnt.VJ_IsControllingNPC)) return end  // Don't damage bulleyes used by the NPC controller OR entities that are controlling others (Usually players)
        var selfData =;

        if (selfData.DoesRadiusDamage)
        var attackEnt = ownerValid && owner || self  // The entity that will be set as the attacker;
        // If the projectile is picked up (Such as a grenade picked up by a human NPC), then the damage position is the parent's position
        if (selfData.VJ_ST_Grabbed)
        var parent = GetParent();
        if (parent.IsValid() && parent.IsNPC())
        dmgPos = parent.GetPos();


        hitEnts = VJ.ApplyRadiusDamage(attackEnt, self, dmgPos, selfData.RadiusDamageRadius, selfData.RadiusDamage, selfData.RadiusDamageType, ownerValid && !owner.IsPlayer(), selfData.RadiusDamageUseRealisticRadius, {DisableVisibilityCheck=selfData.RadiusDamageDisableVisibilityCheck, Force=selfData.RadiusDamageForce, UpForce=selfData.RadiusDamageForce_Up, DamageAttacker=owner.IsPlayer()});


        if (selfData.DoesDirectDamage)
        if (ownerValid)
        // Accepts one of the 3 cases:
        // Entity is not NPC/player
        // Entity is NPC and not same class and (owner is a player OR not an ally NPC -- Players can still damage NPCs while NPCs can't damage other friendly NPCs)
        // Entity is player and alive and (owner is player OR (ignore players is off and no target is off) -- Players can still damage each other while NPCs can't when ignore players is on)
        if (dataEnt.IsValid() && ((!dataEnt.IsNPC() && !dataEnt.IsPlayer()) || (dataEnt.IsNPC() && dataEnt.GetClass() != owner.GetClass() && (owner.IsPlayer() || (owner.IsNPC() && owner.Disposition(dataEnt) != D_LI))) || (dataEnt.IsPlayer() && dataEnt.Alive() && (owner.IsPlayer() || (!VJ_CVAR_IGNOREPLAYERS && !dataEnt.IsFlagSet(FL_NOTARGET))))))
        if (hitEnts)
        hitEnts[hitEnts.Count + 1] = dataEnt;
        else;
        hitEnts = {dataEnt}

        var dmgInfo = DamageInfo();
        dmgInfo.SetDamage(selfData.DirectDamage);
        dmgInfo.SetDamageType(selfData.DirectDamageType);
        dmgInfo.SetAttacker(owner);
        dmgInfo.SetInflictor(self);
        dmgInfo.SetDamagePosition(dmgPos);
        DamageHelper.Special(owner, dataEnt, dmgInfo);
        dataEnt.TakeDamageInfo(dmgInfo, self);

        else;
        if (hitEnts)
        hitEnts[hitEnts.Count + 1] = dataEnt;
        else;
        hitEnts = {dataEnt}

        var dmgInfo = DamageInfo();
        dmgInfo.SetDamage(selfData.DirectDamage);
        dmgInfo.SetDamageType(selfData.DirectDamageType);
        dmgInfo.SetAttacker(self);
        dmgInfo.SetInflictor(self);
        dmgInfo.SetDamagePosition(dmgPos);
        DamageHelper.Special(self, dataEnt, dmgInfo);
        dataEnt.TakeDamageInfo(dmgInfo, self);



        OnDealDamage(data, phys, hitEnts);
    }

    public virtual void StartTouch(ent)
    {
        //print("START TOUCH", ent)
        // Filter out entities that shouldn't be hit (such as clips or triggers)
        if (!ent.IsPlayer() && !ent.IsNPC() && !ent.IsNextBot() && !ent.IsFlagSet(FL_OBJECT)) return 
        if (ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled) return 
        var owner = Owner;
        // Skip the following cases:
        // Owner is the ent
        // Owner is an NPC:
        // Owner is the same class as ent
        // Owner is friendly to ent
        // Ent is a parent of the owner
        // Ent is a player AND is dead OR ignore players is on OR has no target
        if (owner.IsValid() && owner == ent || (this.CollisionFilter && owner.IsNPC() && (owner.GetClass() == ent.GetClass() || owner.Disposition(ent) == D_LI || owner.GetParent() == ent || (ent.IsPlayer() && (!ent.Alive() || VJ_CVAR_IGNOREPLAYERS || ent.IsFlagSet(FL_NOTARGET))))))
        //print("START TOUCH - SKIPPPPP")
        return;

        //print("PASS", ent)

        // Translate TraceResult --> CollisionData
        var trace = GetTouchTrace();
        var myPhys = Rigidbody;
        var myVel = myPhys.GetVelocity();
        var myAngVel = myPhys.GetAngleVelocity();
        var entPhys = ent.GetPhysicsObject();
        var entVel;
        var entAngVel;
        if (entPhys.IsValid())
        entVel = entPhys.GetVelocity();
        entAngVel = entPhys.GetAngleVelocity();
        else;
        entVel = ent.GetVelocity();
        entAngVel = entVel;

        if (trace.HitNormal == defVec)  // Touch functions tend to return an invalid normal, so calculate it using the velocity
        trace.HitNormal = myVel.GetNormalized();

        trace.PhysObject = myPhys;
        trace.HitEntity = ent;
        trace.HitObject = entPhys;
        trace.HitSpeed = (myVel - entVel):Length();
        trace.Speed = myVel.Length();
        trace.DeltaTime = 1;
        trace.OurSurfaceProps = trace.SurfaceProps;
        trace.OurOldVelocity = myVel;
        trace.OurOldAngularVelocity = myAngVel;
        trace.OurNewVelocity = myVel;
        trace.TheirSurfaceProps = trace.SurfaceProps;
        trace.TheirOldVelocity = entVel;
        trace.TheirOldAngularVelocity = entAngVel;
        trace.TheirNewVelocity = entVel;
        trace.HitPos = Transform.Position  // Fake it until you make it;

        PhysicsCollide(trace, myPhys);
    }

    public virtual void PhysicsCollide(data, phys)
    {
        var selfData =;
        if (selfData.Dead) return 

        if (!OnCollision(data, phys))
        var colBehavior = selfData.CollisionBehavior;
        if (!colBehavior) return 
        if (colBehavior == PROJ_COLLISION_REMOVE)
        selfData.Dead = true;
        DealDamage(data, phys);
        PlaySound("OnCollide");
        if (!selfData.PaintedFinalDecal)
        var decals = PICK(selfData.CollisionDecal);
        if (decals)
        selfData.PaintedFinalDecal = true;
        Decals.Place(decals, data.HitPos + data.HitNormal * -15, data.HitPos - data.HitNormal * -2);



        // Remove the entity
        if (selfData.ShakeWorldOnDeath) ScreenShake(data.HitPos, selfData.ShakeWorldOnDeathAmplitude || 16, selfData.ShakeWorldOnDeathFrequency || 200, selfData.ShakeWorldOnDeathDuration || 1, selfData.ShakeWorldOnDeathRadius || 3000) end  // !!!!!!!!!!!!!! DO NOT USE THIS VARIABLE !!!!!!!!!!!!!! [Backwards Compatibility!]
        Destroy(data, phys);
        else if (colBehavior == PROJ_COLLISION_PERSIST)
        if (Time.Now < selfData.NextPersistCollisionT) return 
        DealDamage(data, phys);
        PlaySound("OnCollide");
        if (!selfData.PaintedFinalDecal)
        var decals = PICK(selfData.CollisionDecal);
        if (decals)
        Decals.Place(decals, data.HitPos + data.HitNormal * -15, data.HitPos - data.HitNormal * -2);


        // Avoids "Changing collision rules within a callback is likely to cause crashes!"
        GameTask.DelaySeconds(0).ContinueWith(_ => function();
        if (this.IsValid())
        OnCollisionPersist(data, phys);

        end);
        selfData.NextPersistCollisionT = Time.Now + 1  // Add a delay so we don't spam it!;


    }

    public virtual void Destroy(data, phys)
    {
        phys = phys || Rigidbody;
        this.Dead = true;
        StopParticles();
        this.CurrentIdleSound?.Stop();
        OnDestroy(data, phys);

        // Handle removal
        if (this.RemoveDelay > 0)
        SetNoDraw(true);
        // Avoids "Changing collision rules within a callback is likely to cause crashes!"
        GameTask.DelaySeconds(0).ContinueWith(_ => function();
        if (this.IsValid())
        // SetMoveType removed: MOVETYPE_NONE
        // SetSolid removed: SOLID_NONE
        AddSolidFlags(FSOLID_NOT_SOLID);

        end);
        phys.EnableMotion(false);
        phys.SetVelocityInstantaneous(defVec);
        GameTask.DelaySeconds(this.RemoveDelay).ContinueWith(_ => function();
        if (this.IsValid())
        GameObject.Destroy();

        end);
        OnRemove();
        else;
        GameObject.Destroy();

    }

    public virtual void OnRemove()
    {
        this.Dead = true;
        this.CurrentIdleSound?.Stop();
        PlaySound("OnRemove");
        CustomOnRemove();
    }

    public virtual void PlaySound(sdSet)
    {
        if (!sdSet) return 
        var selfData =;
        if (sdSet == "Startup")
        if (selfData.HasStartupSounds && Game.Random.NextInt(1, selfData.StartupSoundChance) == 1)
        SoundManager.Emit(self, selfData.SoundTbl_Startup, selfData.StartupSoundLevel, Game.Random.NextInt(selfData.StartupSoundPitch.a, selfData.StartupSoundPitch.b));

        else if (sdSet == "Idle")
        var curIdleSD = selfData.CurrentIdleSound;
        if (selfData.HasIdleSounds && (!curIdleSD || (curIdleSD && !curIdleSD.IsPlaying())) && Time.Now > selfData.NextIdleSoundT)
        if (Game.Random.NextInt(1, selfData.IdleSoundChance) == 1)
        selfData.CurrentIdleSound?.Stop();
        selfData.CurrentIdleSound = SoundManager.CreateHandle(self, selfData.SoundTbl_Idle, selfData.IdleSoundLevel, Game.Random.NextInt(selfData.IdleSoundPitch.a, selfData.IdleSoundPitch.b));

        selfData.NextIdleSoundT = Time.Now + Game.Random.NextFloat(selfData.NextSoundTime_Idle.a , selfData.NextSoundTime_Idle.b);

        else if (sdSet == "OnCollide")
        if (selfData.HasOnCollideSounds && Game.Random.NextInt(1, selfData.OnCollideSoundChance) == 1)
        SoundManager.Emit(self, selfData.SoundTbl_OnCollide, selfData.OnCollideSoundLevel, Game.Random.NextInt(selfData.OnCollideSoundPitch.a, selfData.OnCollideSoundPitch.b));

        else if (sdSet == "OnRemove")
        if (selfData.HasOnRemoveSounds && Game.Random.NextInt(1, selfData.OnRemoveSoundChance) == 1)
        SoundManager.Emit(self, selfData.SoundTbl_OnRemove, selfData.OnRemoveSoundLevel, Game.Random.NextInt(selfData.OnRemoveSoundPitch.a, selfData.OnRemoveSoundPitch.b));


    }

    public virtual void OnCollideSoundCode()
    {
    }

    public virtual void DoDamageCode(data, phys)
    {
    }

    public virtual void SetDeathVariablesTrue(data, phys, runOnDestroy)
    {
        this.Dead = true;
        StopParticles();
        this.CurrentIdleSound?.Stop();
        if (runOnDestroy) OnDestroy(data, phys || Rigidbody) 
    }

    public virtual void OnDraw()
    {
    }

}