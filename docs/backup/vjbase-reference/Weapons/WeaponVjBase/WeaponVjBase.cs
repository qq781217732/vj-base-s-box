using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class WeaponVjBase : BaseWeapon
{
    [Property] public bool IsVJBaseWeapon = true;
    [Property] public string PrintName = "VJ Base Weapon";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Purpose = "Made for Players and NPCs.";
    [Property] public string Instructions = "";
    [Property] public string Category = "VJ Base";
    [Property] public bool MadeForNPCsOnly = false;
    [Property] public object ReplacementWeapon = null;
    [Property] public string HoldType = "ar2";
    [Property] public string WorldModel = "models/weapons/w_rif_ak47.mdl";
    [Property] public bool WorldModel_UseCustomPosition = false;
    [Property] public Vector3 WorldModel_CustomPositionAngle = Vector(0, 0, 0);
    [Property] public Vector3 WorldModel_CustomPositionOrigin = Vector(0, 0, 0);
    [Property] public string WorldModel_CustomPositionBone = "ValveBiped.Bip01_R_Hand";
    [Property] public float NPC_NextPrimaryFire = 0.11;
    [Property] public int NPC_TimeUntilFire = 0;
    [Property] public object NPC_TimeUntilFireExtraTimers = {};
    [Property] public int NPC_CustomSpread = 1;
    [Property] public string NPC_BulletSpawnAttachment = "";
    [Property] public bool NPC_CanBePickedUp = true;
    [Property] public bool NPC_StandingOnly = false;
    [Property] public int NPC_FiringDistanceScale = 1;
    [Property] public int NPC_FiringDistanceMax = 100000;
    [Property] public float NPC_FiringCone = 0.9;
    [Property] public bool NPC_HasReloadSound = true;
    [Property] public bool NPC_ReloadSound = false;
    [Property] public int NPC_ReloadSoundLevel = 60;
    [Property] public bool NPC_BeforeFireSound = false;
    [Property] public int NPC_BeforeFireSoundLevel = 70;
    [Property] public object NPC_BeforeFireSoundPitch = VJ.SET(90, 100);
    [Property] public bool NPC_ExtraFireSound = false;
    [Property] public float NPC_ExtraFireSoundTime = 0.4;
    [Property] public int NPC_ExtraFireSoundLevel = 70;
    [Property] public object NPC_ExtraFireSoundPitch = VJ.SET(90, 100);
    [Property] public bool NPC_HasSecondaryFire = false;
    [Property] public string NPC_SecondaryFireEnt = "obj_vj_grenade_rifle";
    [Property] public int NPC_SecondaryFireChance = 3;
    [Property] public object NPC_SecondaryFireNext = VJ.SET(12, 15);
    [Property] public int NPC_SecondaryFireDistance = 1000;
    [Property] public bool NPC_SecondaryFireSound = false;
    [Property] public int NPC_SecondaryFireSoundLevel = 90;
    [Property] public string ViewModel = "models/weapons/c_pistol.mdl";
    [Property] public bool UseHands = false;
    [Property] public bool ViewModelFlip = false;
    [Property] public int ViewModelFOV = 55;
    [Property] public float BobScale = 1.5;
    [Property] public int SwayScale = 1;
    [Property] public bool CSMuzzleFlashes = false;
    [Property] public bool DrawAmmo = true;
    [Property] public bool DrawCrosshair = true;
    [Property] public int Slot = 2;
    [Property] public int SlotPos = 4;
    [Property] public int Weight = 30;
    [Property] public bool AutoSwitchTo = false;
    [Property] public bool AutoSwitchFrom = false;
    [Property] public bool DrawWeaponInfoBox = true;
    [Property] public bool BounceWeaponIcon = true;
    [Property] public object AnimTbl_Deploy = ACT_VM_DRAW;
    [Property] public bool HasDeploySound = true;
    [Property] public bool DeploySound = false;
    [Property] public bool HasIdleAnimation = true;
    [Property] public object AnimTbl_Idle = ACT_VM_IDLE;
    [Property] public object AnimTbl_Reload = ACT_VM_RELOAD;
    [Property] public bool HasReloadSound = false;
    [Property] public bool ReloadSound = false;
    [Property] public int Reload_TimeUntilAmmoIsSet = 1;
    [Property] public object AnimTbl_SecondaryFire = ACT_VM_SECONDARYATTACK;
    [Property] public bool HasDryFireSound = true;
    [Property] public bool DryFireSound = false;
    [Property] public int DryFireSoundLevel = 50;
    [Property] public object DryFireSoundPitch = VJ.SET(90, 100);
    [Property] public object AnimTbl_PrimaryFire = ACT_VM_PRIMARYATTACK;
    [Property] public bool PrimaryEffects_MuzzleFlash = true;
    [Property] public object PrimaryEffects_MuzzleParticles = {"vj_rifle_full"};
    [Property] public bool PrimaryEffects_MuzzleParticlesAsOne = false;
    [Property] public string PrimaryEffects_MuzzleAttachment = "muzzle";
    [Property] public bool PrimaryEffects_SpawnShells = true;
    [Property] public string PrimaryEffects_ShellAttachment = "shell";
    [Property] public string PrimaryEffects_ShellType = "RifleShellEject";
    [Property] public bool PrimaryEffects_SpawnDynamicLight = true;
    [Property] public int PrimaryEffects_DynamicLightBrightness = 4;
    [Property] public int PrimaryEffects_DynamicLightDistance = 120;
    [Property] public Color PrimaryEffects_DynamicLightColor = Color(255, 150, 60);
    [Property] public bool IsMeleeWeapon = false;
    [Property] public int MeleeWeaponDistance = 60;
    [Property] public string MeleeWeaponSound_Hit = "physics/flesh/flesh_impact_bullet1.wav";
    [Property] public string MeleeWeaponSound_Miss = "weapons/iceaxe/iceaxe_swing1.wav";
    [Property] public object RenderGroup = RENDERGROUP_OPAQUE;
    [Property] public bool Reloading = false;
    [Property] public int PLY_NextReloadT = 0;
    [Property] public int PLY_NextIdleAnimT = 0;
    [Property] public int NPC_NextDrySoundT = 0;
    [Property] public int NPC_NextPrimaryFireT = 0;
    [Property] public object NPC_AnimationSet = VJ.ANIM_SET_CUSTOM;
    [Property] public int NPC_SecondaryFireNextT = 0;
    [Property] public bool OwnerIsNPC = false;
    [Property] public int InitTime = 0;

    public virtual void OnInit()
    {
    }

    public virtual void OnEquip(newOwner)
    {
    }

    public virtual void OnDeploy()
    {
    }

    public virtual void OnThink()
    {
    }

    public virtual void OnGetBulletPos()
    {
    }

    public virtual void OnDrawWorldModel()
    {
    }

    public virtual void OnAnimEvent(pos, ang, event, options)
    {
    }

    public virtual void OnPrimaryAttack(status, statusData)
    {
    }

    public virtual void OnPrimaryAttack_BulletCallback(attacker, tr, dmginfo)
    {
    }

    public virtual void NPC_SecondaryFire_BeforeTimer(eneEnt, fireTime)
    {
    }

    public virtual void NPC_SecondaryFire()
    {
        // Override this function if you want to make your own secondary attack!
        var owner = Owner;
        var spawnPos = BulletPosition;
        var projectile = SceneUtility.CreatePrefab();
        projectile.SetPos(spawnPos);
        projectile.SetAngles(owner.GetAngles());
        projectile.SetOwner(owner);
        projectile.Spawn();
        projectile.Activate();
        var phys = projectile.GetPhysicsObject();
        if (phys.IsValid())
        phys.Wake();
        if (phys.IsGravityEnabled())
        phys.SetVelocity(Trajectory.Calculate(owner, owner.GetEnemy(), "Curve", projectile.GetPos(), 1, 1));
        else;
        phys.SetVelocity(Trajectory.Calculate(owner, owner.GetEnemy(), "Line", projectile.GetPos(), 1, 2000));

        projectile.SetAngles(projectile.GetVelocity():GetNormal():Angle());

    }

    public virtual void OnSecondaryAttack()
    {
    }

    public virtual void OnReload(status)
    {
    }

    public virtual void OnHolster(newWep)
    {
    }

    public virtual void OnCustomRemove()
    {
    }

    public virtual void DecideAnimationLength(anim, override, decrease)
    {
    }

    public virtual void OnInitialize()
    {
        this.InitTime = Time.Now;
        this.PrimaryEffects_ShellType = oldShells[this.PrimaryEffects_ShellType] || this.PrimaryEffects_ShellType  // !!!!!!!!!!!!!! DO NOT USE THESE VALUES !!!!!!!!!!!!!! [Backwards Compatibility!];
        SetHoldType(this.HoldType);
        Clip1 = this.Primary.ClipSize;
        this.Primary.DefaultClip = this.Primary.ClipSize;
        this.NPC_SecondaryFireNextT = Time.Now + Game.Random.NextFloat(this.NPC_SecondaryFireNext.a, this.NPC_SecondaryFireNext.b);
        Init();

        // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]
        if (this.CustomOnInitialize) CustomOnInitialize() 
        if (this.CustomOnThink) this.OnThink = function() CustomOnThink() end 
        if (this.CustomOnEquip) this.OnEquip = function(_, newOwner) CustomOnEquip(newOwner) end 
        if (this.CustomOnDeploy) this.OnDeploy = function() CustomOnDeploy() end 
        if (this.CustomBulletSpawnPosition) this.OnGetBulletPos = function() return CustomBulletSpawnPosition() end 
        if (this.CustomOnDrawWorldModel) this.OnDrawWorldModel = function() return CustomOnDrawWorldModel() end 
        if (this.CustomOnFireAnimationEvent) this.OnAnimEvent = function(_, pos, ang, event, options) return CustomOnFireAnimationEvent(pos, ang, event, options) end 
        if (this.CustomOnHolster) this.OnHolster = function(_, newWep) return !CustomOnHolster(newWep) end 
        if (this.CustomOnReload || this.CustomOnReload_Finish)
        this.OnReload = function(_, status);
        if (status == "Start" && this.CustomOnReload)
        CustomOnReload();
        else if (status == "Finish" && this.CustomOnReload_Finish)
        return !CustomOnReload_Finish();



        if (this.CustomOnPrimaryAttack_BeforeShoot || this.CustomOnPrimaryAttack_AfterShoot || this.CustomOnPrimaryAttack_MeleeHit)
        this.OnPrimaryAttack = function(_, status, statusData);
        if (status == "Init" && this.CustomOnPrimaryAttack_BeforeShoot)
        return CustomOnPrimaryAttack_BeforeShoot();
        else if (status == "PostFire" && this.CustomOnPrimaryAttack_AfterShoot)
        CustomOnPrimaryAttack_AfterShoot();
        else if (status == "MeleeHit" && this.CustomOnPrimaryAttack_MeleeHit)
        CustomOnPrimaryAttack_MeleeHit(statusData);



        if (this.CustomOnPrimaryAttack_BulletCallback) this.OnPrimaryAttack_BulletCallback = function(_, attacker, tr, dmginfo) return CustomOnPrimaryAttack_BulletCallback(attacker, tr, dmginfo) end 
        if (this.CustomOnSecondaryAttack) this.OnSecondaryAttack = function() return !CustomOnSecondaryAttack() end 
        //

        //if (SERVER)
        //HoldType = this.HoldType
        //SetNPCMinBurst(10)
        //SetNPCMaxBurst(20)
        //SetNPCFireRate(10)
        //
        SetDefaultValues(this.HoldType);
        //// SetKeyValue("spawnflags", bit.bor(SF_WEAPON_NO_PLAYER_PICKUP))
    }

    public virtual void GetCapabilities()
    {
        return bit.bor(CAP_WEAPON_RANGE_ATTACK1, CAP_INNATE_RANGE_ATTACK1);
    }

    public virtual void SetDefaultValues(holdType)
    {
        if (holdType == "pistol")
        if (!Game.Random.FromList(this.DeploySound)) this.DeploySound = "VJ.Weapon.Draw_Pistol" 
        if (!Game.Random.FromList(this.DryFireSound)) this.DryFireSound = "vj_base/weapons/dryfire_pistol.wav" 
        if (!Game.Random.FromList(this.NPC_ReloadSound)) this.NPC_ReloadSound = "vj_base/weapons/reload_pistol.wav" 
        else if (holdType == "revolver")
        if (!Game.Random.FromList(this.DeploySound)) this.DeploySound = "VJ.Weapon.Draw_Pistol" 
        if (!Game.Random.FromList(this.DryFireSound)) this.DryFireSound = "vj_base/weapons/dryfire_revolver.wav" 
        if (!Game.Random.FromList(this.NPC_ReloadSound)) this.NPC_ReloadSound = "vj_base/weapons/reload_revolver.wav" 
        else if (holdType == "shotgun" || holdType == "crossbow")
        if (!Game.Random.FromList(this.DeploySound)) this.DeploySound = "VJ.Weapon.Draw_Shotgun" 
        if (!Game.Random.FromList(this.DryFireSound)) this.DryFireSound = "vj_base/weapons/dryfire_shotgun.wav" 
        if (!Game.Random.FromList(this.NPC_ReloadSound)) this.NPC_ReloadSound = "vj_base/weapons/reload_shotgun.wav" 
        else if (holdType == "rpg")
        if (!Game.Random.FromList(this.DeploySound)) this.DeploySound = "VJ.Weapon.Draw_Rifle" 
        if (!Game.Random.FromList(this.DryFireSound)) this.DryFireSound = "vj_base/weapons/dryfire_rifle.wav" 
        if (!Game.Random.FromList(this.NPC_ReloadSound)) this.NPC_ReloadSound = "vj_base/weapons/reload_rpg.wav" 
        else if (holdType == "melee" || holdType == "melee2" || holdType == "knife")
        this.DeploySound = "VJ.Weapon.Draw_Rifle";
        this.HasDryFireSound = false;
        this.NPC_HasReloadSound = false;
        else  // "smg", "ar2" && any other that didn't match
        if (!Game.Random.FromList(this.DeploySound)) this.DeploySound = "VJ.Weapon.Draw_Rifle" 
        if (!Game.Random.FromList(this.DryFireSound)) this.DryFireSound = "vj_base/weapons/dryfire_rifle.wav" 
        if (!Game.Random.FromList(this.NPC_ReloadSound)) this.NPC_ReloadSound = "vj_base/weapons/reload_rifle.wav" 

    }

    public virtual void OnEquip(newOwner)
    {
        if (newOwner.IsPlayer())
        var replacementWep = this.ReplacementWeapon;
        if (replacementWep)
        if (isstring(replacementWep)) replacementWep = {replacementWep} 
        for _, weapon in replacementWep do
        // Go in order until a weapon is valid
        if (newOwner.Give(weapon.IsValid()))
        GameObject.Destroy();
        return;
        else if (newOwner.HasWeapon(weapon))  // Failed to give weapon, check if it already has it!
        var actualWeapon = newOwner.GetWeapon(weapon);
        var ammoType = actualWeapon.GetPrimaryAmmoType();
        if (ammoType != -1)  // Give a clip of the replacement weapon
        newOwner.GiveAmmo(actualWeapon.GetMaxClip1(), ammoType);

        GameObject.Destroy();
        return;



        if (!this.IsMeleeWeapon)
        if (this.Primary.PickUpAmmoAmount == "Default")
        newOwner.GiveAmmo(this.Primary.ClipSize * 2, this.Primary.Ammo);
        else if (isnumber(this.Primary.PickUpAmmoAmount))
        newOwner.GiveAmmo(this.Primary.PickUpAmmoAmount, this.Primary.Ammo);


        //newOwner.RemoveAmmo(this.Primary.DefaultClip, this.Primary.Ammo)
        if (this.MadeForNPCsOnly)
        newOwner.PrintMessage(HUD_PRINTTALK, this.PrintName + " removed! It's made for NPCs only!");
        GameObject.Destroy();

        else if (newOwner.IsNPC())
        EventSystem.Subscribe("Think", this.NPC_Think);
        if (newOwner.IsVJBaseSNPC)
        if (newOwner.IsVJBaseSNPC_Human)
        newOwner.Weapon_OriginalFiringDistanceFar = newOwner.Weapon_OriginalFiringDistanceFar || newOwner.Weapon_MaxDistance;
        if (this.IsMeleeWeapon)
        newOwner.Weapon_MaxDistance = this.MeleeWeaponDistance;
        else;
        newOwner.Weapon_MaxDistance = Math.Clamp(newOwner.Weapon_OriginalFiringDistanceFar * this.NPC_FiringDistanceScale, newOwner.Weapon_MinDistance, this.NPC_FiringDistanceMax);


        else  // For non-VJ NPCs
        if (AnimationHelper.Exists(newOwner, ACT_WALK_AIM_PISTOL) && AnimationHelper.Exists(newOwner, ACT_RUN_AIM_PISTOL) && AnimationHelper.Exists(newOwner, ACT_POLICE_HARASS1))
        this.NPC_AnimationSet = VJ.ANIM_SET_METROCOP;
        else if (AnimationHelper.Exists(newOwner, "cheer1") && AnimationHelper.Exists(newOwner, "wave_smg1") && AnimationHelper.Exists(newOwner, ACT_BUSY_SIT_GROUND))
        this.NPC_AnimationSet = VJ.ANIM_SET_REBEL;
        else if (AnimationHelper.Exists(newOwner, "signal_takecover") && AnimationHelper.Exists(newOwner, "grenthrow") && AnimationHelper.Exists(newOwner, "bugbait_hit"))
        this.NPC_AnimationSet = VJ.ANIM_SET_COMBINE;

        if (newOwner.GetClass() == "npc_citizen") newOwner.Fire("DisableWeaponPickup") end  // If it's a citizen, disable them picking up weapons from the ground
        newOwner.SetKeyValue("spawnflags", "256")  // Long Visibility Shooting since HL2 NPCs are blind;


        OnEquip(newOwner);
    }

    public virtual void EquipAmmo(ply)
    {
        if (ply.IsPlayer())
        ply.GiveAmmo(this.Primary.ClipSize, this.Primary.Ammo);

    }

    public virtual void OnDeploy()
    {
        var owner = Owner;
        OnDeploy();
        if (owner.IsNPC())
        EventSystem.Subscribe("Think", this.NPC_Think);
        else if (owner.IsPlayer())
        if (this.HasDeploySound)
        var deploySD = Game.Random.FromList(this.DeploySound);
        if (deploySD)
        Sound.Play(deploySD, 50, Game.Random.NextInt(90, 100, Transform.Position));


        var curTime = Time.Now;
        var anim = Game.Random.FromList(this.AnimTbl_Deploy);
        var animTime = AnimationHelper.Duration(owner.GetViewModel(), anim);
        SendWeaponAnim(anim);
        NextPrimaryFireTime = curTime + animTime;
        NextSecondaryFireTime = curTime + animTime;
        this.PLY_NextIdleAnimT = curTime + animTime;
        this.PLY_NextReloadT = curTime + animTime;

        return true  // Or else the player won't be able to get the weapon!;
    }

    public virtual void GetBulletPos()
    {
        var owner = Owner;
        if (!owner.IsValid()) return Transform.Position end  // Fail safe
        if (owner.IsPlayer()) return owner.GetShootPos() end  // Players always use "GetShootPos"!

        // Custom Position
        var customPos = OnGetBulletPos();
        if (customPos)
        return customPos;


        // Attachment
        var bulletAttach = this.NPC_BulletSpawnAttachment;
        if (bulletAttach != "")
        var attach = Model.GetAttachmentIndex(bulletAttach);
        if (attach != 0 && attach != -1)
        return SkinnedModelRenderer.GetBoneTransform(attach).Pos;



        // Try to find a common attachment
        var attachments = Attachments;
        for i = 1, attachments.Count do
        var attachmentName = attachments[i].name;
        if (commonAttachmentNames[attachmentName])
        return SkinnedModelRenderer.GetBoneTransform(Model.GetAttachmentIndex(attachmentName)).Pos;



        // Try to find a common bone
        var bone = owner.LookupBone("ValveBiped.Bip01_R_Hand");
        if (bone)
        return owner.GetBonePosition(bone);


        // Everything else has failed, post a warning and use eye position!
        // VJ.DEBUG: self, "GetBulletPos", "error", "Failed to find custom position || attachment || bone! Using EyePos!"
        return owner.EyePos();

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function SWEP.Think()  // NOTE: This only runs for players !NPCs!;
        OnThink();
        if (SERVER)
        MaintainWorldModel();
        DoIdleAnimation();


        //-------------------------------------------------------------------------------------------------------------------------------------------
        function SWEP.NPC_Think();
        if (!this.IsValid()) return 
        var owner = metaEntity.GetOwner(self);
        if (!owner.IsValid() || !owner.IsNPC() || funcGetActiveWeapon(owner) != self) return 

        var selfData = funcGetTable(self);
        selfData.MaintainWorldModel(self, selfData, owner);
        selfData.OnThink(self);

        if (!selfData.IsMeleeWeapon && selfData.NPC_NextPrimaryFire && Time.Now > selfData.NPC_NextPrimaryFireT && selfData.NPC_CanFire(self, selfData, owner))
        selfData.NPCShoot_Primary(self);


        //-------------------------------------------------------------------------------------------------------------------------------------------
        function SWEP.NPC_CanFire(selfData, owner);
        selfData = selfData || funcGetTable(self);
        owner = owner || metaEntity.GetOwner(self);
        var ownerData = funcGetTable(owner);
        var ene = owner.GetEnemy();
        var isVJHuman = ownerData.IsVJBaseSNPC_Human;
        if ((isVJHuman && ene.IsValid() && !ownerData.CanFireWeapon(owner, true, true)) || (selfData.NPC_StandingOnly && owner.IsMoving()))
        return false;

        if ((isVJHuman && (ownerData.WeaponAttackState == VJ.WEP_ATTACK_STATE_FIRE || (ownerData.WeaponAttackState == VJ.WEP_ATTACK_STATE_FIRE_STAND && AnimationHelper.IsPlaying(owner, ownerData.WeaponAttackAnim)))) || (!isVJHuman))
        if (selfData.IsMeleeWeapon) return true 
        var isControlled = ownerData.VJ_IsBeingControlled;
        // For VJ Humans only, ammo check
        if (isVJHuman && ownerData.Weapon_CanReload && Clip1 <= 0)  // No ammo!
        if (isControlled) ownerData.VJ_TheController.PrintMessage(HUD_PRINTCENTER, "Press R to reload!") 
        if (selfData.HasDryFireSound && Time.Now > selfData.NPC_NextDrySoundT)
        var sdTbl = Game.Random.FromList(selfData.DryFireSound);
        if (sdTbl)
        owner.EmitSound(sdTbl, 80, Game.Random.NextInt(selfData.DryFireSoundPitch.a, selfData.DryFireSoundPitch.b), 1, CHAN_AUTO, 0, 0, VJ_RecipientFilter);

        if (selfData.NPC_NextPrimaryFire != false)
        selfData.NPC_NextDrySoundT = Time.Now + selfData.NPC_NextPrimaryFire;


        return false;

        // Check to make sure the enemy is within the firing cone!
        if (ene.IsValid() && ((!isControlled) || (isControlled && owner.VJ_TheController.KeyDown(IN_ATTACK2))))
        var spawnPos = metaEntity.GetPos(self) //BulletPosition  // Because "GetBulletPos" is VERY costly sadly =(;
        var aimPos = ownerData.IsVJBaseSNPC && ownerData.GetAimPosition(owner, ene, spawnPos, 0) || ene.BodyTarget(spawnPos);
        var aimDir = aimPos - spawnPos;
        var sightDir = owner.GetHeadDirection() // owner.GetForward()  // Owner's sight direction;
        aimDir.z = 0;
        aimDir.Normalize();
        sightDir.z = 0;
        sightDir.Normalize();
        //print(sightDir.Dot(aimDir))
        //DebugOverlay.Line(spawnPos, spawnPos + aimDir * 10000, 2, VJ.COLOR_RED, true)  // Direction to enemy
        //DebugOverlay.Line(spawnPos, spawnPos + sightDir * 10000, 2, VJ.COLOR_GREEN, true)  // Aim direction
        return sightDir.Dot(aimDir) > selfData.NPC_FiringCone;


        return false;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function SWEP.NPCShoot_Primary();
        var owner = Owner;
        if (!owner.IsValid()) return 
        var ene = owner.GetEnemy();
        if (!owner.VJ_IsBeingControlled && (!ene.IsValid() || (!owner.Visible(ene)))) return 
        if (owner.IsVJBaseSNPC)
        owner.UpdatePoseParamTracking();


        // Secondary Fire
        if (this.NPC_HasSecondaryFire && owner.Weapon_CanSecondaryFire && Time.Now > this.NPC_SecondaryFireNextT && ene.GetPos():Distance(owner.GetPos()) <= this.NPC_SecondaryFireDistance)
        if (Game.Random.NextInt(1, this.NPC_SecondaryFireChance) == 1)
        var anim, animDur, animType = owner.PlayAnim(owner.AnimTbl_WeaponAttackSecondary, true, false, true);
        if (animType != VJ.ANIM_TYPE_GESTURE)
        animDur = animDur - 0.5;

        var fireTime = (anim == ACT_INVALID && 0) || owner.Weapon_SecondaryFireTime || animDur;
        this.NPC_SecondaryFireNextT = Time.Now + fireTime + 0.5  // Prevent attempting to fire again;
        NPC_SecondaryFire_BeforeTimer(ene, fireTime);
        GameTask.DelaySeconds(fireTime).ContinueWith(_ => function();
        if (this.IsValid() && owner.IsValid() && owner.GetEnemy(.IsValid()) && (anim == ACT_INVALID || animType == VJ.ANIM_TYPE_GESTURE || (anim && AnimationHelper.IsPlaying(owner, anim))))  // ONLY check for cur anim IF it even had one!
        NPC_SecondaryFire();
        var fireSd = Game.Random.FromList(this.NPC_SecondaryFireSound);
        if (fireSd)
        Sound.Play(fireSd, this.NPC_SecondaryFireSoundLevel, Game.Random.NextInt(90, 110, Transform.Position), 1, CHAN_WEAPON, 0, 0, VJ_RecipientFilter);

        this.NPC_SecondaryFireNextT = Time.Now + Game.Random.NextFloat(this.NPC_SecondaryFireNext.a, this.NPC_SecondaryFireNext.b);

        end);
        return;
        else;
        this.NPC_SecondaryFireNextT = Time.Now + Game.Random.NextFloat(this.NPC_SecondaryFireNext.a, this.NPC_SecondaryFireNext.b);



        // Primary Fire
        GameTask.DelaySeconds(this.NPC_TimeUntilFire).ContinueWith(_ => function();
        if (!this.IsValid()) return 
        var curTime = Time.Now;
        owner = Owner;
        if (owner.IsValid() && owner.IsNPC() && NPC_CanFire() && curTime > this.NPC_NextPrimaryFireT)
        PrimaryAttack();
        owner.WeaponLastShotTime = curTime;
        if (this.NPC_NextPrimaryFire != false)  // Support for animation events
        this.NPC_NextPrimaryFireT = curTime + this.NPC_NextPrimaryFire;
        for _, tv in this.NPC_TimeUntilFireExtraTimers do
        GameTask.DelaySeconds(tv).ContinueWith(_ => function();
        if (!this.IsValid()) return 
        owner = Owner;
        if (owner.IsValid() && owner.IsNPC() && NPC_CanFire())
        PrimaryAttack();

        end);



        end);

        //-------------------------------------------------------------------------------------------------------------------------------------------
        function SWEP.PrimaryAttack();
        //if (!IsFirstTimePredicted()) return 

        var selfData = funcGetTable(self);
        var owner = metaEntity.GetOwner(self);
        var ownerData = funcGetTable(owner);

        var curTime = Time.Now;
        NextPrimaryFireTime = curTime + selfData.Primary.Delay;

        var isNPC = owner.IsNPC();
        var isPly = owner.IsPlayer();

        if (selfData.Reloading || NextSecondaryFireTime > curTime) return end // owner.KeyDown(IN_RELOAD)
        if (isNPC && !ownerData.VJ_IsBeingControlled && !owner.GetEnemy(.IsValid())) return end  // If the NPC owner isn't being controlled && doesn't have an enemy, then return
        if (!selfData.IsMeleeWeapon && ((isPly && !selfData.Primary.AllowInWater && owner.WaterLevel() == 3) || (Clip1 <= 0)))
        if (SERVER)
        var dryFireSound = Game.Random.FromList(selfData.DryFireSound);
        if (dryFireSound)
        owner.EmitSound(dryFireSound, selfData.DryFireSoundLevel, Game.Random.NextInt(selfData.DryFireSoundPitch.a, selfData.DryFireSoundPitch.b));


        return;

        if (!selfData.CanPrimaryAttack(self)) return 
        if (selfData.OnPrimaryAttack(self, "Init") == true) return 

        if (isNPC && ownerData.IsVJBaseSNPC)
        GameTask.DelaySeconds(selfData.NPC_ExtraFireSoundTime).ContinueWith(_ => function();
        if (this.IsValid() && owner.IsValid())
        SoundManager.Emit(owner, selfData.NPC_ExtraFireSound, selfData.NPC_ExtraFireSoundLevel, Game.Random.NextFloat(selfData.NPC_ExtraFireSoundPitch.a, selfData.NPC_ExtraFireSoundPitch.b));

        end);


        // Firing Sounds
        var fireSd = Game.Random.FromList(selfData.Primary.Sound);
        if (fireSd)
        Sound.Play(fireSd, selfData.Primary.SoundLevel, Game.Random.NextInt(selfData.Primary.SoundPitch.a, selfData.Primary.SoundPitch.b, Transform.Position), selfData.Primary.SoundVolume, CHAN_WEAPON, 0, 0, VJ_RecipientFilter);
        //EmitSound(fireSd, ownersPos, owner.EntIndex(), CHAN_WEAPON, 1, 140, 0, 100, 0, filter)
        //sound.Play(fireSd, ownersPos, this.Primary.SoundLevel, Game.Random.NextInt(this.Primary.SoundPitch.a, this.Primary.SoundPitch.b), this.Primary.SoundVolume)

        if (selfData.Primary.HasDistantSound)
        var fireFarSd = Game.Random.FromList(selfData.Primary.DistantSound);
        if (fireFarSd)
        // Use "CHAN_AUTO" instead of "CHAN_WEAPON" otherwise it will override primary firing sound because it's also "CHAN_WEAPON"
        Sound.Play(fireFarSd, selfData.Primary.DistantSoundLevel, Game.Random.NextInt(selfData.Primary.DistantSoundPitch.a, selfData.Primary.DistantSoundPitch.b, Transform.Position), selfData.Primary.DistantSoundVolume, CHAN_AUTO, 0, 0, VJ_RecipientFilter);



        // Firing Gesture
        if (ownerData.IsVJBaseSNPC_Human && ownerData.AnimTbl_WeaponAttackGesture)
        ownerData.PlayAnim(owner, ownerData.AnimTbl_WeaponAttackGesture, false, false, false, 0, {AlwaysUseGesture = true});


        // MELEE WEAPON
        if (selfData.IsMeleeWeapon)
        var meleeHit = false;
        var ownersPos = metaEntity.GetPos(owner);
        for _, ent in Scene.FindInPhysics(ownersPos, selfData.MeleeWeaponDistance + 20) do
        var entData = funcGetTable(ent);
        if ((entData.IsVJBaseBullseye && entData.VJ_IsBeingControlled) || (ent.IsPlayer() && entData.VJ_IsControllingNPC)) continue end  // If it's a bullseye && is controlled OR it's a player controlling then don't damage!
        if (ent != owner && (isPly || (isNPC && (ent.IsNPC() || (ent.IsPlayer() && ent.Alive() && !VJ_CVAR_IGNOREPLAYERS) || ent.IsNextBot() || entData.VJ_ID_Attackable || entData.VJ_ID_Destructible) && owner.Disposition(ent) != D_LI && ent.GetClass() != owner.GetClass() && (owner.GetForward():Dot((ent.GetPos() - ownersPos):GetNormalized()) > MathF.Cos(MathX.DegreesToRadians(owner.MeleeAttackDamageAngleRadius))))))
        var dmginfo = DamageInfo();
        var dmgAmount = isNPC && ownerData.ScaleByDifficulty(owner, selfData.Primary.Damage) || selfData.Primary.Damage;
        dmginfo.SetDamage(dmgAmount);
        if (ent.VJ_ID_Living) dmginfo.SetDamageForce(owner.GetForward() * ((dmgAmount + 100) * 70)) 
        dmginfo.SetInflictor(owner);
        dmginfo.SetAttacker(owner);
        dmginfo.SetDamageType(DMG_CLUB);
        DamageHelper.Special(owner, ent, dmginfo);
        ent.TakeDamageInfo(dmginfo, owner);
        if (ent.IsPlayer())
        ent.ViewPunch(Angle(Game.Random.NextInt(-1, 1) * dmgAmount, Game.Random.NextInt(-1, 1) * dmgAmount, Game.Random.NextInt(-1, 1) * dmgAmount));

        selfData.OnPrimaryAttack(self, "MeleeHit", ent);
        meleeHit = true;


        if (meleeHit)
        var meleeSd = Game.Random.FromList(selfData.MeleeWeaponSound_Hit);
        if (meleeSd)
        Sound.Play(meleeSd, 70, Game.Random.NextInt(90, 100, Transform.Position), 1, CHAN_AUTO, 0, 0, VJ_RecipientFilter);

        else;
        if (owner.IsVJBaseSNPC) owner.OnMeleeAttackExecute("Miss") 
        var meleeSd = Game.Random.FromList(selfData.MeleeWeaponSound_Miss);
        if (meleeSd)
        Sound.Play(meleeSd, 70, Game.Random.NextInt(90, 100, Transform.Position), 1, CHAN_AUTO, 0, 0, VJ_RecipientFilter);


        // REGULAR BULLET WEAPON
        else if (!selfData.Primary.DisableBulletCode)
        var bullet = {}
        bullet.Num = selfData.Primary.NumberOfShots;
        bullet.Tracer = selfData.Primary.Tracer;
        bullet.TracerName = selfData.Primary.TracerType;
        bullet.Force = selfData.Primary.Force;
        bullet.AmmoType = selfData.Primary.Ammo;
        bullet.Attacker = owner;
        bullet.Inflictor = self  // Sets both the GetInflictor && GetWeapon for the damage info;
        bullet.Callback = function(attacker, tr, dmginfo);
        return selfData.OnPrimaryAttack_BulletCallback(self, attacker, tr, dmginfo);
    }

    public virtual void PrimaryAttackEffects(owner)
    {
        var selfData = funcGetTable(self);
        if (selfData.IsMeleeWeapon) return 
        owner = owner || metaEntity.GetOwner(self);
        if (selfData.CustomOnPrimaryAttackEffects && selfData.CustomOnPrimaryAttackEffects(self, owner) == false) return end  // !!!!!!!!!!!!!! DO NOT USE !!!!!!!!!!!!!! [Backwards Compatibility!]

        if (vj_wep_muzzleflash.GetInt() == 1)
        owner.MuzzleFlash();

        // MUZZLE FLASH
        if (selfData.PrimaryEffects_MuzzleFlash)
        var muzzleAttach = selfData.PrimaryEffects_MuzzleAttachment;
        if (!isnumber(muzzleAttach)) muzzleAttach = Model.GetAttachmentIndex(muzzleAttach) 
        // Players
        if (owner.IsPlayer() && owner.GetViewModel() != null)
        var muzzleFlashEffect = new EffectData();
        var shootPos = owner.GetShootPos();
        muzzleFlashEffect.SetOrigin(shootPos);
        muzzleFlashEffect.SetEntity(self);
        muzzleFlashEffect.SetStart(shootPos);
        muzzleFlashEffect.SetNormal(owner.GetAimVector());
        muzzleFlashEffect.SetAttachment(muzzleAttach);
        Effects.Play("VJ_MuzzleFlash_Player", muzzleFlashEffect);
        // NPCs
        else;
        var particles = selfData.PrimaryEffects_MuzzleParticles;
        if (selfData.PrimaryEffects_MuzzleParticlesAsOne)  // Combine all of the particles in the table!
        for _, v in particles do
        Particles.Attach(v, PATTACH_POINT_FOLLOW, self, muzzleAttach);

        else;
        particles = Game.Random.FromList(particles);
        if (particles)
        Particles.Attach(particles, PATTACH_POINT_FOLLOW, self, muzzleAttach);





        // MUZZLE DYNAMIC LIGHT
        if (SERVER && selfData.PrimaryEffects_SpawnDynamicLight && vj_wep_muzzleflash_light.GetInt() == 1)
        var muzzleLight = SceneUtility.CreatePrefab();
        muzzleLight.SetKeyValue("brightness", selfData.PrimaryEffects_DynamicLightBrightness);
        muzzleLight.SetKeyValue("distance", selfData.PrimaryEffects_DynamicLightDistance);
        if (owner.IsPlayer())
        muzzleLight.SetLocalPos(owner.GetShootPos() + metaEntity.GetForward(self)*40 + metaEntity.GetUp(self)*-10);
        else;
        muzzleLight.SetLocalPos(selfData.GetBulletPos(self));

        muzzleLight.SetLocalAngles(metaEntity.GetAngles(self));
        muzzleLight.SetColor(selfData.PrimaryEffects_DynamicLightColor);
        //muzzleLight.SetParent(self)
        muzzleLight.Spawn();
        muzzleLight.Activate();
        muzzleLight.Fire("TurnOn");
        muzzleLight.Fire("Kill", null, 0.07);
        DeleteOnRemove(muzzleLight);



        // SHELL CASING
        if (!owner.IsPlayer() && selfData.PrimaryEffects_SpawnShells && vj_wep_shells.GetInt() == 1)
        var shellAttach = selfData.PrimaryEffects_ShellAttachment;
        shellAttach = SkinnedModelRenderer.GetBoneTransform(isnumber(shellAttach) && shellAttach || Model.GetAttachmentIndex(shellAttach));
        if (!shellAttach)  // No attachment found, so just use some default pos & ang
        shellAttach = {Pos = owner.GetShootPos(), Ang = metaEntity.GetAngles(self)}

        var effectData = new EffectData();
        effectData.SetEntity(self);
        effectData.SetOrigin(shellAttach.Pos);
        effectData.SetAngles(shellAttach.Ang);
        Effects.Play(selfData.PrimaryEffects_ShellType, effectData, true, true);

    }

    public virtual void CanSecondaryAttack()
    {
    }

    public virtual void OnSecondaryAttack()
    {
        if (Ammo2 <= 0 || this.Reloading) return end // !CanSecondaryAttack()
        if (OnSecondaryAttack() == true) return 

        var curTime = Time.Now;
        var owner = Owner;
        TakeSecondaryAmmo(this.Secondary.TakeAmmo);
        owner.SetAnimation(PLAYER_ATTACK1);
        var anim = Game.Random.FromList(this.AnimTbl_SecondaryFire);
        var animTime = AnimationHelper.Duration(owner.GetViewModel(), anim);
        SendWeaponAnim(anim);
        this.PLY_NextIdleAnimT = curTime + animTime;
        this.PLY_NextReloadT = curTime + animTime;

        NextSecondaryFireTime = curTime + (this.Secondary.Delay == false && animTime || this.Secondary.Delay);
    }

    public virtual void DoIdleAnimation()
    {
        var curTime = Time.Now;
        if (!this.HasIdleAnimation || curTime < this.PLY_NextIdleAnimT) return 
        var owner = Owner;
        if (owner.IsValid())
        owner.SetAnimation(PLAYER_IDLE);
        var anim = Game.Random.FromList(this.AnimTbl_Idle);
        var animTime = AnimationHelper.Duration(owner.GetViewModel(), anim);
        SendWeaponAnim(anim);
        this.PLY_NextIdleAnimT = curTime + animTime;

    }

    public virtual void TranslateActivity(act)
    {
        var selfData = funcGetTable(self);
        var owner = metaEntity.GetOwner(self);
        var ownerData = funcGetTable(owner);
        if (ownerData.IsVJBaseSNPC)
        var translation = ownerData.AnimationTranslations[act];
        if (translation)
        if (istable(translation))
        return translation[Game.Random.NextInt(1, translation.Count)] || act;

        return translation;

        // Non-VJ NPCs
        else if (owner.IsNPC() && selfData.ActivityTranslateAI[act])
        return selfData.ActivityTranslateAI[act];
        // Players
        else if (selfData.ActivityTranslate[act])
        return selfData.ActivityTranslate[act];

        return -1;
    }

    public virtual void FireAnimationEvent(pos, ang, event, options)
    {
        if (OnAnimEvent(pos, ang, event, options) == true)
        return true;
        else if (event == 22 || event == 6001)
        return true;
        else if (vj_wep_muzzleflash.GetInt() == 0 && (event == 21 || event == 5001 || event == 5003))
        return true;
        else if (vj_wep_shells.GetInt() == 0 && event == 20)
        return true;

    }

    public virtual void OnReload()
    {
        if (!this.IsValid()) return 
        var owner = Owner;
        if (!owner.IsValid() || !owner.IsPlayer() || !owner.Alive() || owner.GetAmmoCount(this.Primary.Ammo) == 0 || this.Reloading || Time.Now < this.PLY_NextReloadT) return 
        if (Clip1 < this.Primary.ClipSize)
        this.Reloading = true;
        OnReload("Start");
        if (SERVER && this.HasReloadSound)
        var reloadSD = Game.Random.FromList(this.ReloadSound);
        if (reloadSD)
        owner.EmitSound(reloadSD, 50, Game.Random.NextInt(90, 100));


        // Handle clip
        GameTask.DelaySeconds(this.Reload_TimeUntilAmmoIsSet).ContinueWith(_ => function();
        if (this.IsValid() && OnReload("Finish") != true)
        var ammoUsed = Math.Clamp(this.Primary.ClipSize - Clip1, 0, owner.GetAmmoCount(PrimaryAmmoType))  // Amount of ammo that it will use (Take from the reserve);
        owner.RemoveAmmo(ammoUsed, this.Primary.Ammo);
        Clip1 = Clip1( + ammoUsed);

        end);
        // Handle animation
        owner.SetAnimation(PLAYER_RELOAD);
        var anim = Game.Random.FromList(this.AnimTbl_Reload);
        var animTime = AnimationHelper.Duration(owner.GetViewModel(), anim);
        SendWeaponAnim(anim);
        this.PLY_NextIdleAnimT = Time.Now + animTime;
        GameTask.DelaySeconds(animTime).ContinueWith(_ => function();
        if (this.IsValid())
        this.Reloading = false;

        end);
        return true;

    }

    public virtual void NPC_Reload()
    {
        var owner = Owner;
        if (!owner.IsValid()) return 
        owner.NextThrowGrenadeT = owner.NextThrowGrenadeT + 2;
        OnReload("Start");
        if (this.NPC_HasReloadSound) SoundManager.Emit(owner, this.NPC_ReloadSound, this.NPC_ReloadSoundLevel) 
    }

    public virtual void OnHolster(newWep)
    {
        if (self == newWep || this.Reloading) return 
        EventSystem.Unsubscribe("Think")  // Otherwise "NPC_Think" will just keep running!;
        this.PLY_NextIdleAnimT = Time.Now + 2;
        //SendWeaponAnim(ACT_VM_HOLSTER)
    }

    public virtual void OnDrop()
    {
        EventSystem.Unsubscribe("Think")  // Otherwise "NPC_Think" will just keep running!;
    }

    public virtual void OwnerChanged()
    {
        var owner = Owner;
        if (owner.IsValid())
        this.OwnerIsNPC = owner.IsNPC();

    }

    public virtual void CanBePickedUpByNPCs()
    {
        return this.NPC_CanBePickedUp;
    }

    public virtual void GetWeaponCustomPosition(owner, selfData)
    {
        selfData = selfData || funcGetTable(self);
        var boneID = metaEntity.LookupBone(owner, selfData.WorldModel_CustomPositionBone);
        if (!boneID) return false 
        var customPos = selfData.WorldModel_CustomPositionOrigin;
        var customAng = selfData.WorldModel_CustomPositionAngle;
        var pos, ang = metaEntity.GetBonePosition(owner, boneID);
        metaAngle.RotateAroundAxis(ang, metaAngle.Right(ang), customAng.x);
        metaAngle.RotateAroundAxis(ang, metaAngle.Up(ang), customAng.y);
        metaAngle.RotateAroundAxis(ang, metaAngle.Forward(ang), customAng.z);
        pos = pos + (customPos.x * metaAngle.Right(ang) + customPos.y * metaAngle.Forward(ang) + customPos.z * metaAngle.Up(ang));
        return pos, ang;
    }

    public virtual void MaintainWorldModel(selfData, owner)
    {
        selfData = selfData || funcGetTable(self);
        owner = owner || metaEntity.GetOwner(self);
        if (owner.IsValid() && selfData.WorldModel_UseCustomPosition)
        var wepPos, wepAng = selfData.GetWeaponCustomPosition(self, owner, selfData);
        if (wepPos)
        metaEntity.SetPos(self, wepPos);
        metaEntity.SetAngles(self, wepAng);


    }

    public virtual void SetupDataTables()
    {
        [Net] "Bool" "DrawWorldModel"
        if (SERVER)
        SetDrawWorldModel(true);

    }

    public virtual void DrawWorldModel()
    {
        var drawMdl = true;
        var selfData = funcGetTable(self);
        if (!OnDrawWorldModel() || !GetDrawWorldModel()) drawMdl = false 

        if (selfData.WorldModel_UseCustomPosition)
        var owner = Owner;
        if (owner.IsValid())
        if (owner.IsPlayer() && owner.InVehicle()) return 
        var wepPos, wepAng = GetWeaponCustomPosition(owner);
        if (wepPos)
        SetRenderOrigin(wepPos);
        SetRenderAngles(wepAng);
        FrameAdvance(FrameTime());
        SetupBones();

        else;
        SetRenderOrigin(null);
        SetRenderAngles(null);


        if (drawMdl) funcDrawModel(self) 
    }

    public virtual void OnRemove()
    {
        StopParticles();
        CustomOnRemove();
    }

    public virtual void SetupWeaponHoldTypeForAI(holdType)
    {
        var owner = Owner;
        if (owner.IsVJBaseSNPC) return 

        // Yete NPC-en Rebel-e, ere vor medz zenki animation-ere kordzadze yerp vor ge kalegor
        var bezdigZenk_Kalel = ACT_WALK_AIM_PISTOL;
        var bezdigZenk_Vazel = ACT_RUN_AIM_PISTOL;
        if (this.NPC_AnimationSet == VJ.ANIM_SET_REBEL)
        bezdigZenk_Kalel = ACT_WALK_AIM_RIFLE;
        bezdigZenk_Vazel = ACT_RUN_AIM_RIFLE;


        // Yete NPC-en Combine-e yev bizdig zenk pernere, ere vor medz zenki animation-ere kordzadze
        var rifleOverride = false;
        var medzZenk_Genal = ACT_IDLE_SMG1;
        var medzZenk_Kalel = ACT_WALK_RIFLE;
        if (this.NPC_AnimationSet == VJ.ANIM_SET_COMBINE && (holdType == "pistol" || holdType == "revolver"))
        rifleOverride = true;
        medzZenk_Genal = AnimationHelper.SeqToActivity(owner, "idle_unarmed");
        medzZenk_Kalel = AnimationHelper.SeqToActivity(owner, "walkunarmed_all");


        // Yete NPC-en Metrocop-e gamal Rebel-e, ere vor medz zenki animation-ere kordzadze yerp vor ge kalegor
        var bonbakshen_varichadz = ACT_RANGE_ATTACK_SHOTGUN_LOW;
        var bonbakshen_Vazel = ACT_RUN_AIM_SHOTGUN;
        if (this.NPC_AnimationSet == VJ.ANIM_SET_METROCOP || this.NPC_AnimationSet == VJ.ANIM_SET_REBEL)
        bonbakshen_varichadz = ACT_RANGE_ATTACK_SMG1_LOW;
        //bonbakshen_Kalel = ACT_WALK_AIM_RIFLE
        bonbakshen_Vazel = ACT_RUN_AIM_RIFLE;


        this.ActivityTranslateAI = {}
        if (rifleOverride || holdType == "ar2" || holdType == "smg")
        if (holdType == "ar2" || rifleOverride)
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1] 				= ACT_RANGE_ATTACK_AR2;
        this.ActivityTranslateAI[ACT_GESTURE_RANGE_ATTACK1] 		= ACT_GESTURE_RANGE_ATTACK_AR2;
        this.ActivityTranslateAI[ACT_RANGE_AIM_LOW] 				= ACT_RANGE_AIM_AR2_LOW;
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1_LOW] 			= ACT_RANGE_ATTACK_AR2_LOW;
        else if (holdType == "smg")
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1] 				= ACT_RANGE_ATTACK_SMG1;
        this.ActivityTranslateAI[ACT_GESTURE_RANGE_ATTACK1] 		= ACT_GESTURE_RANGE_ATTACK_SMG1;
        this.ActivityTranslateAI[ACT_RANGE_AIM_LOW] 				= ACT_RANGE_AIM_SMG1_LOW;
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1_LOW] 			= ACT_RANGE_ATTACK_SMG1_LOW;

        this.ActivityTranslateAI[ACT_COVER_LOW] 					= ACT_COVER_SMG1_LOW;
        this.ActivityTranslateAI[ACT_RELOAD] 						= ACT_RELOAD_SMG1;
        this.ActivityTranslateAI[ACT_RELOAD_LOW] 					= ACT_RELOAD_SMG1_LOW;
        this.ActivityTranslateAI[ACT_GESTURE_RELOAD] 				= ACT_GESTURE_RELOAD_SMG1;
        this.ActivityTranslateAI[ACT_IDLE] 							= medzZenk_Genal;
        this.ActivityTranslateAI[ACT_IDLE_ANGRY] 					= ACT_IDLE_ANGRY_SMG1;
        this.ActivityTranslateAI[ACT_IDLE_RELAXED] 					= ACT_IDLE_SMG1_RELAXED;
        this.ActivityTranslateAI[ACT_IDLE_STIMULATED] 				= ACT_IDLE_SMG1_STIMULATED;
        this.ActivityTranslateAI[ACT_IDLE_AGITATED] 				= ACT_IDLE_ANGRY_SMG1;
        this.ActivityTranslateAI[ACT_IDLE_AIM_RELAXED] 				= ACT_IDLE_SMG1_RELAXED;
        this.ActivityTranslateAI[ACT_IDLE_AIM_STIMULATED] 			= ACT_IDLE_AIM_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_IDLE_AIM_AGITATED] 			= ACT_IDLE_ANGRY_SMG1;
        this.ActivityTranslateAI[ACT_WALK] 							= medzZenk_Kalel;
        this.ActivityTranslateAI[ACT_WALK_AIM] 						= ACT_WALK_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_WALK_CROUCH] 					= ACT_WALK_CROUCH_RIFLE;
        this.ActivityTranslateAI[ACT_WALK_CROUCH_AIM] 				= ACT_WALK_CROUCH_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_WALK_RELAXED] 					= ACT_WALK_RIFLE_RELAXED;
        this.ActivityTranslateAI[ACT_WALK_STIMULATED] 				= ACT_WALK_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_WALK_AGITATED] 				= ACT_WALK_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_WALK_AIM_RELAXED] 				= ACT_WALK_RIFLE_RELAXED;
        this.ActivityTranslateAI[ACT_WALK_AIM_STIMULATED] 			= ACT_WALK_AIM_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_WALK_AIM_AGITATED] 			= ACT_WALK_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_RUN] 							= ACT_RUN_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_AIM] 						= ACT_RUN_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_CROUCH] 					= ACT_RUN_CROUCH_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_CROUCH_AIM] 				= ACT_RUN_CROUCH_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_RELAXED] 					= ACT_RUN_RIFLE_RELAXED;
        this.ActivityTranslateAI[ACT_RUN_STIMULATED] 				= ACT_RUN_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_RUN_AGITATED] 					= ACT_RUN_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_AIM_RELAXED] 				= ACT_RUN_RIFLE_RELAXED;
        this.ActivityTranslateAI[ACT_RUN_AIM_STIMULATED] 			= ACT_RUN_AIM_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_RUN_AIM_AGITATED] 				= ACT_RUN_AIM_RIFLE;
        else if (holdType == "crossbow" || holdType == "shotgun")
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1] 				= ACT_RANGE_ATTACK_SHOTGUN;
        this.ActivityTranslateAI[ACT_GESTURE_RANGE_ATTACK1] 		= ACT_GESTURE_RANGE_ATTACK_SHOTGUN;
        this.ActivityTranslateAI[ACT_RANGE_AIM_LOW] 				= ACT_RANGE_AIM_AR2_LOW;
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1_LOW] 			= bonbakshen_varichadz;
        this.ActivityTranslateAI[ACT_COVER_LOW] 					= ACT_COVER_SMG1_LOW;
        this.ActivityTranslateAI[ACT_RELOAD] 						= ACT_RELOAD_SHOTGUN;
        this.ActivityTranslateAI[ACT_RELOAD_LOW] 					= ACT_RELOAD_SMG1_LOW //ACT_RELOAD_SHOTGUN_LOW;
        this.ActivityTranslateAI[ACT_GESTURE_RELOAD] 				= ACT_GESTURE_RELOAD_SHOTGUN;

        this.ActivityTranslateAI[ACT_IDLE] 							= ACT_IDLE_SMG1;
        this.ActivityTranslateAI[ACT_IDLE_ANGRY] 					= ACT_IDLE_ANGRY_SHOTGUN;
        this.ActivityTranslateAI[ACT_IDLE_RELAXED] 					= ACT_IDLE_SHOTGUN_RELAXED;
        this.ActivityTranslateAI[ACT_IDLE_STIMULATED] 				= ACT_IDLE_SHOTGUN_STIMULATED;
        this.ActivityTranslateAI[ACT_IDLE_AGITATED] 				= ACT_IDLE_SHOTGUN_AGITATED;
        this.ActivityTranslateAI[ACT_IDLE_AIM_RELAXED] 				= ACT_SHOTGUN_IDLE_DEEP;
        this.ActivityTranslateAI[ACT_IDLE_AIM_STIMULATED] 			= ACT_SHOTGUN_IDLE_DEEP;
        this.ActivityTranslateAI[ACT_IDLE_AIM_AGITATED] 			= ACT_SHOTGUN_IDLE_DEEP;

        this.ActivityTranslateAI[ACT_WALK] 							= ACT_WALK_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_WALK_AIM] 						= ACT_WALK_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_WALK_CROUCH] 					= ACT_WALK_CROUCH_RIFLE;
        this.ActivityTranslateAI[ACT_WALK_CROUCH_AIM] 				= ACT_WALK_CROUCH_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_WALK_RELAXED] 					= ACT_WALK_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_WALK_STIMULATED] 				= ACT_WALK_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_WALK_AGITATED] 				= ACT_WALK_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_WALK_AIM_RELAXED] 				= ACT_WALK_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_WALK_AIM_STIMULATED] 			= ACT_WALK_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_WALK_AIM_AGITATED] 			= ACT_WALK_AIM_SHOTGUN;

        this.ActivityTranslateAI[ACT_RUN] 							= ACT_RUN_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_AIM] 						= bonbakshen_Vazel;
        this.ActivityTranslateAI[ACT_RUN_CROUCH] 					= ACT_RUN_CROUCH_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_CROUCH_AIM] 				= ACT_RUN_CROUCH_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_RELAXED] 					= ACT_RUN_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_STIMULATED] 				= ACT_RUN_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_AGITATED] 					= ACT_RUN_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_AIM_RELAXED] 				= ACT_RUN_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_RUN_AIM_STIMULATED] 			= ACT_RUN_AIM_SHOTGUN;
        this.ActivityTranslateAI[ACT_RUN_AIM_AGITATED] 				= ACT_RUN_AIM_SHOTGUN;
        else if (holdType == "rpg")
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1] 				= ACT_CROUCHIDLE;
        this.ActivityTranslateAI[ACT_GESTURE_RANGE_ATTACK1] 		= ACT_GESTURE_RANGE_ATTACK_SMG1;
        this.ActivityTranslateAI[ACT_RANGE_AIM_LOW] 				= ACT_RANGE_AIM_SMG1_LOW;
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1_LOW] 			= ACT_RANGE_ATTACK_SMG1_LOW;
        this.ActivityTranslateAI[ACT_COVER_LOW] 					= ACT_COVER_LOW_RPG;
        this.ActivityTranslateAI[ACT_RELOAD] 						= ACT_RELOAD_SMG1;
        this.ActivityTranslateAI[ACT_RELOAD_LOW] 					= ACT_RELOAD_SMG1_LOW;
        this.ActivityTranslateAI[ACT_GESTURE_RELOAD] 				= ACT_GESTURE_RELOAD_SMG1;
        this.ActivityTranslateAI[ACT_IDLE] 							= ACT_IDLE_RPG;
        this.ActivityTranslateAI[ACT_IDLE_ANGRY] 					= ACT_IDLE_ANGRY_RPG;
        this.ActivityTranslateAI[ACT_IDLE_RELAXED] 					= ACT_IDLE_RPG_RELAXED;
        this.ActivityTranslateAI[ACT_IDLE_STIMULATED] 				= ACT_IDLE_SMG1_STIMULATED;
        this.ActivityTranslateAI[ACT_IDLE_AGITATED] 				= ACT_IDLE_ANGRY_RPG;
        this.ActivityTranslateAI[ACT_IDLE_AIM_RELAXED] 				= ACT_IDLE_RPG_RELAXED;
        this.ActivityTranslateAI[ACT_IDLE_AIM_STIMULATED] 			= ACT_IDLE_AIM_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_IDLE_AIM_AGITATED] 			= ACT_IDLE_ANGRY_RPG;
        this.ActivityTranslateAI[ACT_WALK] 							= ACT_WALK_RPG;
        this.ActivityTranslateAI[ACT_WALK_AIM] 						= ACT_WALK_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_WALK_CROUCH] 					= ACT_WALK_CROUCH_RPG;
        this.ActivityTranslateAI[ACT_WALK_CROUCH_AIM] 				= ACT_WALK_CROUCH_RPG;
        this.ActivityTranslateAI[ACT_WALK_RELAXED] 					= ACT_WALK_RPG_RELAXED;
        this.ActivityTranslateAI[ACT_WALK_STIMULATED] 				= ACT_WALK_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_WALK_AGITATED] 				= ACT_WALK_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_WALK_AIM_RELAXED] 				= ACT_WALK_RPG_RELAXED;
        this.ActivityTranslateAI[ACT_WALK_AIM_STIMULATED] 			= ACT_WALK_AIM_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_WALK_AIM_AGITATED] 			= ACT_WALK_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_RUN] 							= ACT_RUN_RPG;
        this.ActivityTranslateAI[ACT_RUN_AIM] 						= ACT_RUN_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_CROUCH] 					= ACT_RUN_CROUCH_RPG;
        this.ActivityTranslateAI[ACT_RUN_CROUCH_AIM] 				= ACT_RUN_CROUCH_RPG;
        this.ActivityTranslateAI[ACT_RUN_RELAXED] 					= ACT_RUN_RPG_RELAXED;
        this.ActivityTranslateAI[ACT_RUN_STIMULATED] 				= ACT_RUN_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_RUN_AGITATED] 					= ACT_RUN_AIM_RIFLE;
        this.ActivityTranslateAI[ACT_RUN_AIM_RELAXED] 				= ACT_RUN_RPG_RELAXED;
        this.ActivityTranslateAI[ACT_RUN_AIM_STIMULATED] 			= ACT_RUN_AIM_RIFLE_STIMULATED;
        this.ActivityTranslateAI[ACT_RUN_AIM_AGITATED] 				= ACT_RUN_AIM_RIFLE;
        else;
        // revolver or pistol
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1] 				= ACT_RANGE_ATTACK_PISTOL;
        this.ActivityTranslateAI[ACT_GESTURE_RANGE_ATTACK1] 		= ACT_GESTURE_RANGE_ATTACK_PISTOL;
        this.ActivityTranslateAI[ACT_RANGE_AIM_LOW] 				= ACT_RANGE_AIM_PISTOL_LOW;
        this.ActivityTranslateAI[ACT_RANGE_ATTACK1_LOW] 			= ACT_RANGE_ATTACK_PISTOL_LOW;
        this.ActivityTranslateAI[ACT_COVER_LOW] 					= ACT_COVER_PISTOL_LOW;
        this.ActivityTranslateAI[ACT_RELOAD] 						= ACT_RELOAD_PISTOL;
        this.ActivityTranslateAI[ACT_RELOAD_LOW] 					= ACT_RELOAD_PISTOL_LOW;
        this.ActivityTranslateAI[ACT_GESTURE_RELOAD] 				= ACT_GESTURE_RELOAD_PISTOL;
        this.ActivityTranslateAI[ACT_IDLE] 							= ACT_IDLE_PISTOL;
        this.ActivityTranslateAI[ACT_IDLE_ANGRY] 					= ACT_IDLE_ANGRY_PISTOL;

        this.ActivityTranslateAI[ACT_WALK] 							= ACT_WALK_PISTOL;
        this.ActivityTranslateAI[ACT_WALK_AIM] 						= bezdigZenk_Kalel;

        this.ActivityTranslateAI[ACT_RUN] 							= ACT_RUN_PISTOL;
        this.ActivityTranslateAI[ACT_RUN_AIM] 						= bezdigZenk_Vazel;

        return;
    }

}