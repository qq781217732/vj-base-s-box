using System;
using Sandbox;
using SWB.Player;

namespace VJBase;

/// <summary>
/// HumanNPC Think methods — ported from npc_vj_human_base/init.lua.
/// Initialize, movement type, chase/alert behavior, grenade system, weapon state.
/// </summary>
public partial class HumanNPC
{
    // ═══ Initialize — human_base/init.lua:2131-2282 ═══
    public virtual void Initialize()
    {
        // lua:2132: PreInit + CustomOnPreInitialize (backwards compat)
        // Phase 3: delegate to OnInit/OnSpawn lifecycle

        // lua:2135-2137: SetSpawnEffect(false) + SetRenderMode + AddEFlags(EFL_NO_DISSOLVE)
        // SKIP: lua:2135-2137 — Source engine render/effects system
        // lua:2138: SetUseType(SIMPLE_USE)
        // SKIP: lua:2138 — Source engine use type

        // lua:2139-2144: self:GetModel() → PICK(self.Model) → SetModel
        // SKIP: lua:2139-2144 — model post-spawn assignment (S&Box prefab handles model) — Phase 3

        // lua:2145-2148: SetHullType / SetHullSizeNormal / SetSolid(SOLID_BBOX) / SetCollisionGroup
        // SKIP: lua:2145-2148 — Source engine collision/hull system
        // lua:2149: SetMaxYawSpeed(self.TurningSpeed)
        // SKIP: lua:2149 — Source engine yaw speed
        // lua:2150: SetSaveValue("m_HackedGunPos", defShootVec)
        // SKIP: lua:2150 — Source engine save value

        // lua:2152-2158: Set a name if it doesn't have one
        // SKIP: lua:2152-2158 — Source engine NPC naming (list.Get("NPC"))

        // lua:2160-2170: Initialize variables
        // SKIP: lua:2161 — InitConvars(self) — Phase 3 convar system
        NextProcessTime = 0.1f; // lua:2162: vj_npc_processtime:GetInt()
        // SKIP: lua:2163 — SelectedDifficulty = vj_npc_difficulty:GetInt() — Phase 3 difficulty system
        RelationshipEnts ??= new();
        RelationshipMemory ??= new();
        AnimationTranslations = new();
        WeaponInventory = new();
        IdleSoundBlockTime = Time.Now + Game.Random.Next(3, 61) / 10f; // lua:2168: math.random(0.3, 6)
        MainSoundPitchValue = 0; // lua:2169: default 0
        // SKIP: lua:2170 — vj_npc_sight_distance convar override — Phase 3 convar

        // lua:2173: DoChangeMovementType(MovementType)
        DoChangeMovementType(MovementType);

        // lua:2174-2175: CapabilitiesAdd(capBitsDefault) + door caps
        // SKIP: lua:2174-2175 — Source engine bit-flag capabilities (CAP_MOVE_*, capBitsDefault)
        // lua:2176-2179: LookupAttachment("eyes"/"forward") → CAP_ANIMATEDFACE
        // SKIP: lua:2176-2179 — Source engine attachment-based capability
        // lua:2180-2182: Passive → Weapon_Disabled + Weapon_IgnoreSpawnMenu
        // lua:2183-2184: elseif CapabilitiesAdd(capBitsWeapons) — SKIP (Source engine bit-flag caps)
        if (Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature)
        {
            Weapon_Disabled = true;
            Weapon_IgnoreSpawnMenu = true;
        }

        // lua:2187-2191: Health setup
        var hp = ScaleByDifficulty(StartHealth); // lua:2189: convar override or ScaleByDifficulty
        // SKIP: lua:2190 — SetHealth(hp) — S&Box health via HealthComponent
        StartHealth = (int)hp; // lua:2191: StartHealth = hp

        // lua:2193-2194: Init() callback + ApplyBackwardsCompatibility
        // Phase 3: delegate to child class override; backwards compat not needed

        // lua:2196-2205: Collision-based computations
        // SKIP: lua:2196-2205 — GetCollisionBounds / SetSurroundingBounds / WorldSpaceAABB — Source engine collision
        // lua:2203-2204: Auto-compute MeleeAttackDistance / MeleeAttackDamageDistance from collision bounds
        // Phase 3: compute from model bounds when animation system is in place

        // lua:2205: SetupBloodColor(BloodColor)
        // SKIP: lua:2205 — Source engine blood system

        // lua:2207: preserve spawner-set value, else compute default
        NextWanderTime = (NextWanderTime != 0 ? NextWanderTime : Time.Now + (IdleAlwaysWander ? 0 : 1));

        // lua:2209-2281: Delayed init (timer.Simple 0.1)
        // SKIP: lua:2211 — SetMaxLookDistance(SightDistance) — S&Box engine handles vision range
        // SKIP: lua:2212 — SetFOV(SightAngle) — S&Box engine handles FOV
        // SKIP: lua:2213 — SetNPCState / GetNPCState — Source engine NPC state
        // SKIP: lua:2214 — GetCreator / IsGuard from creator — Phase 3 spawn system
        // SKIP: lua:2215 — StartSoundTrack() — Phase 3 sound system
        // SKIP: lua:2218-2235 — LookupPoseParameter("aim_pitch"/"head_pitch"/"aim_yaw"/"head_yaw"/"aim_roll"/"head_roll") — Phase 3 animation pose params
        // lua:2237-2265: Weapon setup
        if (Weapon_Disabled)
        {
            // SKIP: lua:2238 — UpdateAnimationTranslations() — Phase 3 animation
        }
        else
        {
            // SKIP: lua:2240 — funcGetActiveWeapon(self) — Phase 3 weapon system
            // lua:2247-2258: AntiArmor + Melee weapon inventory from PICK of lists
            // SKIP: lua:2247-2258 — Give(weaponClass) / SelectWeapon / Equip — Phase 3 weapon inventory
            // SKIP: lua:2261-2263 — CanChatMessage / PrintMessage warnings — Phase 3 messaging
        }
        // SKIP: lua:2266-2268 — GetIdealActivity() / MaintainIdleAnimation(true) — Phase 3 animation
        // SKIP: lua:2269-2279 — hook.Add("Think", self, funcAnimThink) — Phase 3 animation hook system
    }

    // ═══ DoChangeMovementType — human_base/init.lua:2287-2319 ═══
    public virtual void DoChangeMovementType(VJMoveType movType)
    {
        // lua:2288-2289
        MovementType = movType;

        if (movType == VJMoveType.Ground)
        {
            // SKIP: lua:2291 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2292 — CapabilitiesRemove(CAP_MOVE_FLY) — Source engine caps
            // SKIP: lua:2293 — SetNavType(NAV_GROUND) — handled by NavMeshAgent in S&Box
            // SKIP: lua:2294 — SetMoveType(MOVETYPE_STEP) — S&Box physics handles this
            // SKIP: lua:2295 — CapabilitiesAdd(CAP_MOVE_GROUND) — Source engine caps
            // SKIP: lua:2296 — CapabilitiesAdd(CAP_MOVE_JUMP) — Source engine caps (AnimExists ACT_JUMP/pj_npc_human_jump/PoseParamMovement)
            // SKIP: lua:2298 — CapabilitiesAdd(CAP_MOVE_SHOOT) if !Weapon_Disabled && Weapon_CanMoveFire
        }
        else if (movType == VJMoveType.Aerial || movType == VJMoveType.Aquatic)
        {
            // SKIP: lua:2300 — CapabilitiesRemove(capBitsGround) — Source engine caps
            // SKIP: lua:2301 — SetGroundEntity(NULL) — Source engine ground entity
            // SKIP: lua:2302 — AddFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2303 — SetNavType(NAV_FLY) — handled by AA system
            // SKIP: lua:2304 — SetMoveType(MOVETYPE_STEP) — AA system handles this
            // SKIP: lua:2305 — CapabilitiesAdd(CAP_MOVE_FLY) — Source engine caps
        }
        else if (movType == VJMoveType.Stationary)
        {
            // SKIP: lua:2307 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2308 — CapabilitiesRemove(capBitsShared) — Source engine caps
            // SKIP: lua:2309 — SetNavType(NAV_NONE)
            // SKIP: lua:2310-2312 — GetParent check / SetMoveType(MOVETYPE_FLY)
        }
        else if (movType == VJMoveType.Physics)
        {
            // SKIP: lua:2314 — RemoveFlags(FL_FLY) — Source engine flags
            // SKIP: lua:2315 — CapabilitiesRemove(capBitsShared) — Source engine caps
            // SKIP: lua:2316 — SetNavType(NAV_NONE)
            // SKIP: lua:2317 — SetMoveType(MOVETYPE_VPHYSICS)
        }
    }

    // ═══ ProcessAttackTimers override — adds grenade execution ═══
    public override void ProcessAttackTimers(float curTime)
    {
        // Handle grenade start timer (attack_grenade_start)
        if (GrenadeExecTime > 0 && curTime > GrenadeExecTime)
        {
            GrenadeExecTime = 0;
            ExecuteGrenadeAttack(StashedGrenadeEnt, StashedGrenadeDisableOwner, StashedGrenadeLandDir);
        }

        base.ProcessAttackTimers(curTime);
    }

    // ═══ SetWeaponState / GetWeaponState ═══
    public virtual void SetWeaponState(VJWepState state = VJWepState.Ready, float time = -1)
    {
        WeaponState = state;
        // Phase 3: timer-based state reset
    }

    public virtual VJWepState GetWeaponState() => WeaponState;

    // ═══ DoChangeWeapon — human_base/init.lua:2470-2518 ═══
    /// <summary>
    /// Give or switch weapon. wep = weapon class string (nil = setup current only).
    /// invSwitch = true to switch without removing previous weapon.
    /// Returns the active weapon (null if none).
    /// </summary>
    public virtual GameObject DoChangeWeapon(string wep = null, bool invSwitch = false)
    {
        // lua:2471 — wep = wep or nil (C# null default)
        // lua:2472 — invSwitch = invSwitch or false (C# false default)
        // lua:2473 — curWep = funcGetActiveWeapon(self)
        var curWep = GetActiveWeapon(); // Phase 3 stub: returns null

        // ---- Block 1: Weapon disabled → remove (lua:2476-2479) ----
        // lua:2476 — if self.Weapon_Disabled && IsValid(curWep) then
        if (Weapon_Disabled && curWep.IsValid())
        {
            // lua:2477 — curWep:Remove()
            // SKIP: lua:2477 — curWep:Remove() — Phase 3 weapon entity removal
            // lua:2478 — return NULL
            return null;
        }

        // ---- Block 2: Give or switch weapon (lua:2482-2494) ----
        // lua:2482 — if wep != nil then
        if (wep != null)
        {
            // lua:2483 — if invSwitch then
            if (invSwitch)
            {
                // lua:2484 — self:SelectWeapon(wep)
                // SKIP: lua:2484 — self:SelectWeapon(wep) — Phase 3 weapon selection
                // lua:2485 — VJ.EmitSound(self, sdWepSwitch, 70)
                // SKIP: lua:2485 — VJ.EmitSound(sdWepSwitch, 70) — Phase 3 weapon sound
                // lua:2486 — curWep = wep
                curWep = null; // wep is string, curWep is GameObject — Phase 3: resolve via weapon system
            }
            // lua:2487 — else
            else
            {
                // lua:2488 — if IsValid(curWep) && self.WeaponInventoryStatus <= VJ.WEP_INVENTORY_PRIMARY then
                if (curWep.IsValid() && WeaponInventoryStatus <= VJWepInventory.Primary)
                {
                    // lua:2489 — curWep:Remove()
                    // SKIP: lua:2489 — curWep:Remove() — Phase 3 weapon entity removal
                }
                // lua:2491 — curWep = self:Give(wep)
                // SKIP: lua:2491 — self:Give(wep) — Phase 3 weapon Give (Source engine NPC:Give)
                // lua:2492 — self.WeaponInventory.Primary = curWep
                WeaponInventory.Primary = curWep;
            }
        }

        // ---- Block 3: Setup valid weapon (lua:2497-2516) ----
        // lua:2497 — if IsValid(curWep) then
        if (curWep.IsValid())
        {
            // lua:2498 — self.WeaponAttackAnim = ACT_INVALID
            WeaponAttackAnim = null; // Phase 3: ACT_INVALID = -1, in C# null = no valid animation
            // lua:2499 — self:SetWeaponState() — reset state
            SetWeaponState();
            // lua:2500 — if invSwitch then
            if (invSwitch)
            {
                // lua:2501 — if curWep.IsVJBaseWeapon then curWep:Equip(self) end
                // SKIP: lua:2501 — curWep.IsVJBaseWeapon + Equip() — Phase 3 weapon interface
            }
            // lua:2502 — else
            else
            {
                // lua:2503 — self.WeaponInventoryStatus = VJ.WEP_INVENTORY_PRIMARY
                WeaponInventoryStatus = VJWepInventory.Primary;
                // lua:2505-2509 — Replace old primary if different
                var curPrimary = WeaponInventory.Primary;
                if (curWep != WeaponInventory.Primary)
                {
                    // lua:2507 — if IsValid(curPrimary) then curPrimary:Remove() end
                    // SKIP: lua:2507 — curPrimary:Remove() — Phase 3 weapon entity removal
                    // lua:2508 — self.WeaponInventory.Primary = curWep
                    WeaponInventory.Primary = curWep;
                }
            }
            // lua:2511 — self:UpdateAnimationTranslations(curWep:GetHoldType())
            // SKIP: lua:2511 — UpdateAnimationTranslations(curWep:GetHoldType()) — Phase 3 animation system
            // lua:2512 — self:OnWeaponChange(curWep, self.WeaponEntity, invSwitch)
            OnWeaponChange(curWep, WeaponEntity, invSwitch);
            // lua:2513 — self.WeaponEntity = curWep
            WeaponEntity = curWep;
        }
        // lua:2514 — else
        else
        {
            // lua:2515 — self.WeaponInventoryStatus = VJ.WEP_INVENTORY_NONE
            WeaponInventoryStatus = VJWepInventory.None;
        }
        // lua:2517 — return curWep
        return curWep;
    }

    // ═══ SCHEDULE_ALERT_CHASE — human_base/init.lua:2340 ═══
    public virtual void SCHEDULE_ALERT_CHASE(bool doLOSChase)
    {
        // init.lua:2341: self:ClearCondition(COND_ENEMY_UNREACHABLE)
        ClearCondition(Condition.EnemyUnreachable);

        // init.lua:2342: AA branch
        if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
        { AA_ChaseEnemy(); return; }

        // init.lua:2343: CurrentScheduleName guard
        if (CurrentScheduleName == "SCHEDULE_ALERT_CHASE") return;

        // init.lua:2344: navtype check
        var navType = GetNavType();
        if (navType == (int)NavType.Jump || navType == (int)NavType.Climb) return;

        if (doLOSChase)
        {
            // init.lua:2345-2353 — schedule_alert_chaseLOS + RunCode_OnFinish
            var sched = new AISchedule();
            sched.Init("SCHEDULE_ALERT_CHASE_LOS");
            sched.EngTask(EngineTask.GetPathToEnemyLOS, 0);
            sched.EngTask(EngineTask.WaitForMovement, 0);
            sched.CanShootWhenMoving = true;
            sched.CanBeInterrupted = true;
            sched.RunCodeOnFinish = () =>
            {
                var ene = GetEnemy();
                if (ene.IsValid())
                {
                    // SKIP: RememberUnreachable(ene, 0) — Source engine API, Phase 3 enemy memory
                    SCHEDULE_ALERT_CHASE(false);
                }
            };
            StartSchedule(sched);
        }
        else
        {
            // init.lua:2354-2356 — schedule_alert_chase (no RunCode_OnFail in human version)
            var sched = new AISchedule();
            sched.Init("SCHEDULE_ALERT_CHASE");
            sched.EngTask(EngineTask.GetPathToEnemy, 0);
            sched.EngTask(EngineTask.RunPath, 0);
            sched.EngTask(EngineTask.WaitForMovement, 0);
            sched.CanShootWhenMoving = true;
            sched.CanBeInterrupted = true;
            StartSchedule(sched);
        }
    }

    // ═══ MaintainAlertBehavior (human version) ═══
    public override void MaintainAlertBehavior(bool alwaysChase)
    {
        var curTime = Time.Now;
        if (NextChaseTime > curTime || Dead || VJ_IsBeingControlled || Flinching || GetState() == VJState.OnlyAnimationConstant) return;

        var ene = Enemy.Target;
        if (!ene.IsValid() || TakingCoverT > curTime || (AttackAnimTime > curTime && MovementType != VJMoveType.Aerial && MovementType != VJMoveType.Aquatic)) return;

        // Melee range check — stand and attack (lua:2368-2375)
        if (HasMeleeAttack && Enemy.DistanceNearest < MeleeAttackDistance && Enemy.Visible)
        {
            // lua:2369 — angle check: enemy within melee angle radius
            var toEnemy = (ene.WorldPosition - WorldPosition).Normal;
            var headDir = WorldRotation.Forward; // GetHeadDirection() → Phase 3 skeleton
            bool inAngle = headDir.Dot(toEnemy) > MathF.Cos(MathF.PI / 180f * MeleeAttackAngleRadius);
            if (inAngle)
            {
                if (MovementType == VJMoveType.Aerial || MovementType == VJMoveType.Aquatic)
                    AA_StopMoving();
                // lua:2373: self:SCHEDULE_IDLE_STAND()
                SCHEDULE_IDLE_STAND();
                return;
            }
        }

        if (MovementType == VJMoveType.Stationary || IsFollowing || Medic.Status != "false" || GetState() == VJState.OnlyAnimation)
            return;

        if (Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature)
        {
            NextChaseTime = curTime + 3;
            return;
        }

        if (!alwaysChase && (DisableChasingEnemy || IsGuard)) return;

        // init.lua:2394-2399 — If enemy unreachable and we have a ranged weapon → LOS chase
        // SKIP: IsUnreachable(ene) — Source engine API, Phase 3 enemy memory
        // SKIP: IsMeleeWeapon check — Phase 3 weapon system
        if (HasCondition(Condition.EnemyUnreachable) && HasWeapon)
        {
            SCHEDULE_ALERT_CHASE(true);
            // SKIP: RememberUnreachable(ene, 2) — Source engine API, Phase 3 enemy memory
        }
        else
        {
            SCHEDULE_ALERT_CHASE(false);
        }

        // init.lua:2402-2403
        if (NextChaseTime > curTime) return;
        NextChaseTime = curTime + (Enemy.Distance > 2000 ? 1f : 0.1f);
    }

    // ═══ CanFireWeapon — human_base/init.lua:3476-3510 ═══
    /// <summary>
    /// Returns whether this NPC can fire its weapon right now.
    /// checkDistance: also check range/distance constraints.
    /// checkDistanceOnly: only check distance, skip attack-busy/activity checks.
    /// </summary>
    public override bool CanFireWeapon(bool checkDistance, bool checkDistanceOnly)
    {
        // lua:3477 — if self:OnWeaponCanFire() == false then return false end
        if (OnWeaponCanFire() == false) return false;

        // lua:3478 — hasDist = false
        bool hasDist = false;
        // lua:3479 — hasChecks = false
        bool hasChecks = false;
        // lua:3480 — selfData = funcGetTable(self) → this
        // lua:3481 — curWep = selfData.WeaponEntity
        var curWep = WeaponEntity;

        // lua:3483 — if selfData.PauseAttacks or !IsValid(curWep) or self:GetWeaponState() != VJ.WEP_STATE_READY then return false end
        if (PauseAttacks || !curWep.IsValid() || GetWeaponState() != VJWepState.Ready) return false;

        // lua:3484 — if selfData.VJ_IsBeingControlled then
        if (VJ_IsBeingControlled)
        {
            // lua:3485 — checkDistance = false
            checkDistance = false;
            // lua:3486-3488 — if checkDistanceOnly then return true end
            if (checkDistanceOnly) return true;
        }
        // lua:3489 — else
        else
        {
            // lua:3490 — enemyDist = selfData.EnemyData.Distance
            float enemyDist = Enemy.Distance;
            // lua:3491 — if checkDistance && CurTime() > selfData.NextWeaponAttackT then
            if (checkDistance && Time.Now > NextWeaponAttackT)
            {
                // lua:3492 — if curWep.IsMeleeWeapon then
                // SKIP: lua:3492 — curWep.IsMeleeWeapon — Phase 3 weapon interface (always false → takes elseif)
                // lua:3493-3496 — Melee: if VJ.IsCurrentAnim(self, WeaponAttackAnim) or enemyDist < Weapon_MaxDistance then hasDist = true end
                // SKIP: lua:3494 — VJ.IsCurrentAnim(self, WeaponAttackAnim) — Phase 3 animation (VJUtility stub false)
                // lua:3497-3499 — elseif (ranged): enemyDist < Weapon_MaxDistance && enemyDist > Weapon_MinDistance then hasDist = true
                if (enemyDist < Weapon_MaxDistance && enemyDist > Weapon_MinDistance)
                {
                    hasDist = true;
                }
                // Phase 3: restore melee branch above the elseif
                //   if (curWep.IsMeleeWeapon) {
                //     if (VJUtility.IsCurrentAnim(GameObject, WeaponAttackAnim) || enemyDist < Weapon_MaxDistance) hasDist = true; }
                //   else if (enemyDist < Weapon_MaxDistance && enemyDist > Weapon_MinDistance) { hasDist = true; }
            }
            // lua:3501-3503 — if checkDistanceOnly then return hasDist end
            if (checkDistanceOnly) return hasDist;
        }

        // lua:3505 — if !selfData.AttackType && !self:IsBusy("Activities") then
        if (AttackType == VJAttackType.None && !IsBusy("Activities"))
        {
            // lua:3506 — hasChecks = true
            hasChecks = true;
            // lua:3507 — if !checkDistance then return true end
            if (!checkDistance) return true;
        }
        // lua:3509 — return hasDist && hasChecks
        return hasDist && hasChecks;
    }

    // ═══ CheckForDangers — human_base/init.lua:3356-3403 ═══
    /// <summary>
    /// Scans for nearby danger entities (grenades, bombs, etc.) and reacts:
    /// redirect/throw grenades, or take cover.
    /// </summary>
    public virtual void CheckForDangers()
    {
        // lua:3357 — selfData = funcGetTable(self) → this
        // lua:3358 — if !selfData.CanDetectDangers or selfData.AttackType == VJ.ATTACK_TYPE_GRENADE or selfData.NextDangerDetectionT > CurTime() then return end
        if (!CanDetectDangers || AttackType == VJAttackType.Grenade || NextDangerDetectionT > Time.Now) return;

        // lua:3359 — regDangerDetected = false (non-grenade danger found; grenades take priority)
        GameObject regDangerDetected = null;

        // lua:3360 — for _, ent in ipairs(ents.FindInSphere(self:GetPos(), selfData.DangerDetectionDistance)) do
        var nearby = Game.ActiveScene.FindInPhysics(new Sphere(WorldPosition, DangerDetectionDistance));
        foreach (var ent in nearby)
        {
            // lua:3361 — if (ent.VJ_ID_Danger or ent.VJ_ID_Grenade) && self:Visible(ent) then
            // SKIP: lua:3361 — ent.VJ_ID_Danger / ent.VJ_ID_Grenade — Phase 3 entity flags system
            //        No component-based equivalent yet; stub: always false
            bool isDanger = false; // Phase 3: ent.Components.Get<???>()?.VJ_ID_Danger
            bool isGrenade = false; // Phase 3: ent.Components.Get<???>()?.VJ_ID_Grenade
            if ((isDanger || isGrenade) && Visible(ent))
            {
                // lua:3362 — owner = ent:GetOwner()
                // SKIP: lua:3362 — ent:GetOwner() — Phase 3 entity ownership system
                // lua:3363 — if !(IsValid(owner) && owner.IsVJBaseSNPC && ((self:GetClass() == owner:GetClass()) or (self:Disposition(owner) == D_LI))) then
                // SKIP: lua:3363 — owner.IsVJBaseSNPC + GetClass() + Disposition — Phase 3 ownership + entity type
                {
                    // lua:3364 — if ent.VJ_ID_Danger then regDangerDetected = ent continue end
                    if (isDanger)
                    {
                        regDangerDetected = ent;
                        continue;
                    }
                    // lua:3365 — local funcCustom = self.OnDangerDetected; if funcCustom && funcCustom(self, VJ.DANGER_TYPE_GRENADE, ent) then continue end
                    if (OnDangerDetected(VJDangerType.Grenade, ent)) continue;
                    // lua:3366 — curTime = CurTime()
                    float curTime = Time.Now;
                    // lua:3367 — if !self:PlaySoundSystem("GrenadeSight") then self:PlaySoundSystem("DangerSight") end
                    if (!PlaySoundSystem("GrenadeSight"))
                        PlaySoundSystem("DangerSight");
                    // lua:3368 — selfData.NextDangerDetectionT = curTime + 4
                    NextDangerDetectionT = curTime + 4;
                    // lua:3369 — selfData.TakingCoverT = curTime + 4
                    TakingCoverT = curTime + 4;
                    // lua:3371-3374 — Grenade redirect attempt
                    // SKIP: lua:3371 — CanRedirectGrenades && HasGrenadeAttack && ent.VJ_ID_Grabbable && !ent.VJ_ST_Grabbed
                    //        && ent:GetVelocity():Length() < 400 && VJ.GetNearestDistance(self, ent) < 100 && self:GrenadeAttack(ent, true)
                    //        → NextGrenadeAttackSoundT = curTime + 3; return
                    //        Phase 3 entity flags (VJ_ID_Grabbable, VJ_ST_Grabbed) + physics (GetVelocity)
                    // lua:3376-3379 — SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH", ...)
                    // SKIP: lua:3376-3379 — SCHEDULE_COVER_ORIGIN with FACE_ENEMY turn — Phase 3 cover system
                    return;
                }
            }
        }

        // lua:3384 — if regDangerDetected or funcHasCondition(self, COND_HEAR_DANGER) or funcHasCondition(self, COND_HEAR_PHYSICS_DANGER) or funcHasCondition(self, COND_HEAR_MOVE_AWAY) then
        if (regDangerDetected != null
            || HasCondition(Condition.HearDanger)
            || HasCondition(Condition.HearPhysicsDanger)
            || HasCondition(Condition.HearMoveAway))
        {
            // lua:3385-3392 — OnDangerDetected custom callback
            if (regDangerDetected != null)
            {
                // lua:3388 — if funcCustom(self, VJ.DANGER_TYPE_ENTITY, regDangerDetected) then return end
                if (OnDangerDetected(VJDangerType.Entity, regDangerDetected)) return;
            }
            else
            {
                // lua:3390 — if funcCustom(self, VJ.DANGER_TYPE_HINT, nil) then return end
                if (OnDangerDetected(VJDangerType.Hint, null)) return;
            }
            // lua:3393 — self:PlaySoundSystem("DangerSight")
            PlaySoundSystem("DangerSight");
            // lua:3394 — curTime = CurTime()
            float curTime = Time.Now;
            // lua:3395 — selfData.NextDangerDetectionT = curTime + 4
            NextDangerDetectionT = curTime + 4;
            // lua:3396 — selfData.TakingCoverT = curTime + 4
            TakingCoverT = curTime + 4;
            // lua:3397-3400 — self:SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH", function(x) ... end)
            SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH", schedule =>
            {
                schedule.CanShootWhenMoving = true;
                schedule.TurnData = new TurnData { Type = VJFaceStatus.Enemy };
            });
        }
    }

    // ═══ SelectSchedule — human_base/init.lua:3520-3838 ═══
    public override void SelectSchedule()
    {
        // lua:3520-3522: entry guard
        if (VJ_IsBeingControlled || Dead) return;

        // lua:3524-3527: local variables
        float curTime = Time.Now;
        var ene = GetEnemy();
        bool eneValid = ene.IsValid();
        PlayIdleSound(null, null, eneValid);

        // ═══ Idle Behavior (lua:3529-3577) ═══
        if (!eneValid)
        {
            // lua:3531-3532: MaintainIdleBehavior for non-grenade attack types
            if (AttackType != VJAttackType.Grenade)
                MaintainIdleBehavior();

            // lua:3534-3535: Reset TakingCoverT if not alerted
            if (Alerted == VJAlertState.None)
                TakingCoverT = 0;

            // lua:3537
            Weapon_UnarmedBehavior_Active = false;

            // lua:3539-3576: Investigation block
            // SKIP: lua:3518 — bitsDanger = bit.bor(SOUND_BULLET_IMPACT, SOUND_COMBAT, SOUND_WORLD, SOUND_DANGER) — Phase 3 sound constants
            if (CanInvestigate
                && (HasCondition(Condition.HearBulletImpact)
                    || HasCondition(Condition.HearCombat)
                    || HasCondition(Condition.HearWorld)
                    || HasCondition(Condition.HearDanger))
                && NextInvestigationMove < curTime
                && TakingCoverT < curTime
                && !IsBusy())
            {
                // lua:3541: GetBestSoundHint(bitsDanger) — Source engine sound location
                var sdSrc = GetBestSoundHint(0); // SKIP: bitsDanger mask — Phase 3 sound system
                // lua:3542
                if (sdSrc != null)
                {
                    // lua:3544: allowed flag
                    bool allowed = true;
                    // SKIP: lua:3545-3553 — sdSrc.owner, .type, IsVehicle(), GetDriver(), Disposition checks — Phase 3 sound event system
                    // lua:3556-3560 — commented-out player sound check

                    // lua:3561-3574: Execute investigation
                    if (allowed)
                    {
                        DoReadyAlert();                              // lua:3562
                        StopMoving();                                 // lua:3563
                        // SKIP: lua:3564 — SetLastPosition(sdSrc.origin) — sdSrc is null (Phase 3 sound)
                        SCHEDULE_FACE("TASK_FACE_LASTPOSITION");     // lua:3565
                        // lua:3567-3571: commented-out custom schedule
                        // SKIP: lua:3572 — OnInvestigate(sdOwner) — sdOwner is null (Phase 3)
                        PlaySoundSystem("Investigate");              // lua:3573
                        TakingCoverT = curTime + 1;                  // lua:3574
                    }
                }
            }
        }
        // ═══ Combat Behavior (lua:3579-3819) ═══
        else
        {
            // lua:3581-3582: Get active weapon + enemy data
            var wep = GetActiveWeapon(); // SKIP: returns null — Phase 3 weapon system
            var eneData = Enemy;

            // ═══ C1: No valid weapon (lua:3585-3603) ═══
            if (!wep.IsValid())
            {
                // lua:3587-3594: Scared behavior (Weapon_UnarmedBehavior)
                if (Weapon_UnarmedBehavior)
                {
                    if (!IsBusy() && curTime > NextChaseTime)
                    {
                        Weapon_UnarmedBehavior_Active = true;               // lua:3589
                        if (!IsFollowing && eneData.Visible)
                        {
                            SCHEDULE_COVER_ENEMY("TASK_RUN_PATH");          // lua:3591
                            return;                                         // lua:3592
                        }
                    }
                }
                // lua:3596-3600: No scared behavior but has melee — chase
                else if (HasMeleeAttack)
                {
                    Weapon_UnarmedBehavior_Active = false;                  // lua:3597
                    NextDangerDetectionT = curTime + 4;                    // lua:3598
                    MaintainAlertBehavior();                                // lua:3599
                    return;                                                 // lua:3600
                }
                // lua:3602: Fallback — idle, then fall through to goto_conditions
                MaintainIdleBehavior(2);
                // lua:3603: //return — Allow other behaviors (COND_PLAYER_PUSHING) to run
            }
            // ═══ C2: Has valid weapon (lua:3604-3816) ═══
            else
            {
                if (wep == null) goto goto_conditions; // compiler guard (wep is null from stub)

                // lua:3605
                Weapon_UnarmedBehavior_Active = false;

                // lua:3607-3609: Position calculations
                // SKIP: lua:3607 — enePos_Eye = ene:EyePos() — Phase 3 eye position
                var myPos = WorldPosition;                                  // lua:3608
                // SKIP: lua:3609 — myPosCentered = myPos + OBBCenter() — Phase 3 OBB/GetBonePosition
                var myPosCentered = myPos;

                // ═══ C2a: Retreat from too-close enemy (lua:3611-3620) ═══
                // lua:3612: if eneData.Distance <= Weapon_RetreatDistance && !wep.IsMeleeWeapon
                //          && curTime > TakingCoverT && curTime > NextChaseTime && !AttackType
                //          && !IsFollowing && ene.Behavior != VJ_BEHAVIOR_PASSIVE
                //          && !DoCoverTrace(myPosCentered, enePos_Eye)
                // SKIP: lua:3612 — wep.IsMeleeWeapon, ene.Behavior, DoCoverTrace — Phase 3 weapon + cover
                {
                    // SKIP: lua:3613 — moveCheck = PICK(VJ.TraceDirections(self, "Quick", 200, true, false, 8, true)) — Phase 3 utility
                    // SKIP: lua:3614 — if moveCheck then
                    // SKIP: lua:3615 — SetLastPosition(moveCheck) — Phase 3
                    // SKIP: lua:3616 — GetWeaponState() == VJ.WEP_STATE_RELOADING → SetWeaponState() — Phase 3 weapon state
                    // SKIP: lua:3617 — TakingCoverT = curTime + 2
                    // SKIP: lua:3618 — SCHEDULE_GOTO_POSITION("TASK_RUN_PATH", function(x) x:EngTask("TASK_FACE_ENEMY", 0) x.CanShootWhenMoving = true x.TurnData = {Type = VJ.FACE_ENEMY} end)
                    // SKIP: lua:3619 — goto goto_conditions
                }

                // ═══ C2b: CanFireWeapon checks + occlusion (lua:3623-3651) ═══
                // lua:3623: if CanFireWeapon(false, false) && GetState() != VJ_STATE_ONLY_ANIMATION_NOATTACK then
                // SKIP: lua:3623 — CanFireWeapon(false, false) — Phase 3 weapon
                {
                    // lua:3625: if eneData.Distance > Weapon_MaxDistance or curTime < NextWeaponAttackT then
                    // SKIP: lua:3625 — Weapon_MaxDistance, NextWeaponAttackT — Phase 3 weapon config
                    // SKIP: lua:3626 — MaintainAlertBehavior() — Phase 3
                    // SKIP: lua:3627 — AllowWeaponOcclusionDelay = false

                    // lua:3629: elseif CanFireWeapon(true, true) then
                    // SKIP: lua:3629 — CanFireWeapon(true, true) — Phase 3 weapon
                    {
                        // lua:3631: if DoCoverTrace(EyePos, enePos_Eye, true) then
                        // SKIP: lua:3631 — DoCoverTrace(EyePos, enePos_Eye, true) — Phase 3 cover + eye position
                        {
                            // SKIP: lua:3632 — TakingCoverT > curTime → return — Phase 3
                            // lua:3633: if GetWeaponState() != VJ.WEP_STATE_RELOADING then
                            // SKIP: lua:3633 — GetWeaponState() — Phase 3 weapon state
                            {
                                // lua:3635-3638: Occlusion delay
                                // SKIP: lua:3635 — Weapon_OcclusionDelay && WeaponAttackState != VJ.WEP_ATTACK_STATE_AIM_OCCLUSION && !wep.IsMeleeWeapon && AllowWeaponOcclusionDelay && (curTime - WeaponLastShotTime) <= 4.5 && eneData.Distance > Weapon_OcclusionDelayMinDist
                                // SKIP: lua:3636 — WeaponAttackState = VJ.WEP_ATTACK_STATE_AIM_OCCLUSION
                                // SKIP: lua:3637 — MaintainIdleBehavior(2) → ACT_IDLE_ANGRY
                                // SKIP: lua:3638 — NextChaseTime = curTime + math.Rand(Weapon_OcclusionDelayTime.a, Weapon_OcclusionDelayTime.b)

                                // lua:3640-3641: Hidden zone stand-up
                                // SKIP: lua:3640 — curTime < LastHiddenZoneT && !DoCoverTrace(myPosCentered + GetUp()*30, enePos_Eye + GetUp()*30, true) — Phase 3 cover + GetUp
                                // SKIP: lua:3641 — MaintainIdleBehavior(2) → ACT_IDLE_ANGRY
                                // SKIP: lua:3642 — goto goto_checkwep

                                // lua:3643-3649: Everything failed → chase
                                // SKIP: lua:3645-3646 — WeaponAttackState >= VJ.WEP_ATTACK_STATE_FIRE && CurrentScheduleName != "SCHEDULE_ALERT_CHASE" → WeaponAttackState = VJ.WEP_ATTACK_STATE_NONE
                                // SKIP: lua:3648 — MaintainAlertBehavior() — Phase 3
                            }
                            // SKIP: lua:3651 — goto goto_conditions
                        }
                        // lua:3653: -- I can see the enemy...
                        // ═══ C2c: Enemy visible — weapon combat loop (lua:3654-3816) ═══
                        // lua:3654: ::goto_checkwep::
                    goto_checkwep:
                        _ = 0;

                        // lua:3655: if wep.IsVJBaseWeapon then — VJ Base weapons
                        // SKIP: lua:3655 — wep.IsVJBaseWeapon — Phase 3 weapon interface
                        {
                            // ═══ C2c-i: Aim turning (lua:3656-3670) ═══
                            // lua:3657: if !HasPoseParameterLooking then
                            // SKIP: lua:3657-3669 — HasPoseParameterLooking, FInAimCone, SetTurnTarget("Enemy"), GetAngles():Forward(), Dot, UpdatePoseParamTracking(true) — Phase 3 animation + turning
                            // lua:3671: // self:MaintainAlertBehavior() — commented out

                            // ═══ C2c-ii: Cover/obstruction check (lua:3673-3734) ═══
                            // lua:3673-3676: inCover + wepInCover via DoCoverTrace
                            // SKIP: lua:3673 — DoCoverTrace(myPosCentered, enePos_Eye, false, {SetLastHiddenTime = true}) — Phase 3 cover
                            // SKIP: lua:3675 — DoCoverTrace(wep:GetBulletPos(), enePos_Eye, false) — Phase 3 weapon + cover
                            // lua:3679-3682: inCoverEntLiving = IsValid(inCoverEnt) && inCoverEnt.VJ_ID_Living
                            // SKIP: lua:3679-3682 — inCoverEnt.VJ_ID_Living — Phase 3 entity flags

                            // lua:3683: if !wep.IsMeleeWeapon then — ranged weapons only
                            // SKIP: lua:3683 — wep.IsMeleeWeapon — Phase 3 weapon interface
                            {
                                // lua:3685-3693: Friendly in line of fire → move
                                // SKIP: lua:3685 — inCoverEntLiving && WeaponAttackState == VJ.WEP_ATTACK_STATE_FIRE_STAND && IsValid(wepInCoverEnt) && wepInCoverEnt:IsNPC() && Disposition checks — Phase 3
                                // SKIP: lua:3686 — moveCheck = PICK(VJ.TraceDirections(self, "Quick", 50, ...)) — Phase 3 utility
                                // SKIP: lua:3688-3692 — StopMoving / IsGuard guard data / SetLastPosition / SCHEDULE_GOTO_POSITION("TASK_WALK_PATH", lambda) — Phase 3

                                // lua:3697-3734: Behind cover logic
                                // SKIP: lua:3697 — if inCover then
                                // SKIP: lua:3699 — curTime < TakingCoverT → goto goto_conditions
                                // SKIP: lua:3701 — curTime > NextMoveOnGunCoveredT && distance > 150 || wepInCover → reposition
                                // SKIP: lua:3703-3710 — nearestPos/nearestEntPos via VJ.GetNearestPositions / NearestPoint — Phase 3 utility
                                // SKIP: lua:3716-3727 — custom vj_ai_schedule.New("SCHEDULE_GOTO_POSITION") + TranslateActivity(PICK(AnimTbl_MoveToCover)) + AnimExists + SetMovementActivity + StartSchedule — Phase 3 animation + schedule
                                // SKIP: lua:3730 — NextMoveOnGunCoveredT = curTime + 2
                                // SKIP: lua:3731 — return
                            }

                            // ═══ C2c-iii: Weapon attack (lua:3737-3794) ═══
                            // lua:3737: if curTime > NextWeaponAttackT && curTime > NextWeaponAttackT_Base then
                            // SKIP: lua:3737 — NextWeaponAttackT, NextWeaponAttackT_Base — Phase 3 weapon timers
                            {
                                // lua:3739-3751: Melee weapons
                                // SKIP: lua:3739 — wep.IsMeleeWeapon — Phase 3 weapon
                                // SKIP: lua:3740 — OnWeaponAttack() — Phase 3 weapon callback
                                // SKIP: lua:3741 — TranslateActivity(PICK(AnimTbl_WeaponAttack)) — Phase 3 animation
                                // SKIP: lua:3742-3743 — AnimExists + AnimDuration — Phase 3 animation
                                // SKIP: lua:3744 — wep.NPC_NextPrimaryFire = animDur — Phase 3 weapon
                                // SKIP: lua:3745 — wep:NPCShoot_Primary() — Phase 3 weapon
                                // SKIP: lua:3746 — VJ.EmitSound(self, wep.NPC_BeforeFireSound, ...) — Phase 3 sound
                                // SKIP: lua:3747 — NextMeleeWeaponAttackT = curTime + animDur
                                // SKIP: lua:3748 — WeaponAttackAnim = finalAnim
                                // SKIP: lua:3749 — PlayAnim(finalAnim, "LetAttacks", false, true) — Phase 3 animation
                                // SKIP: lua:3750 — WeaponAttackState = VJ.WEP_ATTACK_STATE_FIRE_STAND

                                // lua:3753-3793: Ranged weapons
                                // SKIP: lua:3754 — AllowWeaponOcclusionDelay = true
                                // SKIP: lua:3755 — hasAmmo = wep:Clip1() > 0 — Phase 3 weapon
                                // SKIP: lua:3756-3757 — !hasAmmo && WeaponAttackState != VJ.WEP_ATTACK_STATE_AIM → WeaponAttackAnim = ACT_INVALID
                                // SKIP: lua:3760 — VJ.IsCurrentAnim(self, TranslateActivity(WeaponAttackAnim)) — Phase 3 animation
                                // SKIP: lua:3763 — GetActivity() != WeaponAttackAnim && GetActivity() != ACT_TRANSITION — Phase 3 animation
                                // SKIP: lua:3764 — OnWeaponAttack() — Phase 3 weapon callback
                                // SKIP: lua:3765-3766 — WeaponAttackState == VJ.WEP_ATTACK_STATE_AIM_OCCLUSION → WeaponAttackState = VJ.WEP_ATTACK_STATE_NONE
                                // SKIP: lua:3768 — WeaponLastShotTime = curTime
                                // SKIP: lua:3772-3775 — !hasAmmo → MaintainIdleBehavior(2) + WeaponAttackState = VJ.WEP_ATTACK_STATE_AIM
                                // SKIP: lua:3778-3783 — TranslateActivity(PICK(AnimTbl_WeaponAttackCrouch)) vs TranslateActivity(PICK(AnimTbl_WeaponAttack)) — Phase 3 animation
                                // SKIP: lua:3779 — crouch condition: Weapon_CanCrouchAttack && !inCover && !wepInCover && distance > 500 && AnimExists + random + DoCoverTrace — Phase 3
                                // SKIP: lua:3786-3792 — AnimExists + VJ.EmitSound + PlayAnim + WeaponAttackAnim + WeaponAttackState + NextWeaponAttackT_Base — Phase 3 animation + sound
                            }

                            // ═══ C2c-iv: Random strafing while shooting (lua:3797-3806) ═══
                            // lua:3797: if Weapon_Strafe && !inCover && !IsGuard && !IsFollowing && !wep.IsMeleeWeapon
                            //          && (!wep.NPC_StandingOnly) && WeaponAttackState == VJ.WEP_ATTACK_STATE_FIRE_STAND
                            //          && curTime > NextWeaponStrafeT && (curTime - eneData.TimeAcquired) > 2
                            //          && eneData.Distance < (Weapon_MaxDistance / 1.25)
                            // SKIP: lua:3797 — all weapon/cover/state checks — Phase 3 weapon + cover
                            {
                                // SKIP: lua:3798 — OnWeaponStrafe() != false — Phase 3 weapon callback
                                // SKIP: lua:3799 — moveCheck = PICK(VJ.TraceDirections(self, "Radial", math.random(150, 400), true, false, 12, true)) — Phase 3 utility
                                // SKIP: lua:3801-3803 — StopMoving / SetLastPosition / SCHEDULE_GOTO_POSITION(random walk/run, lambda with FACE_ENEMY) — Phase 3
                                // SKIP: lua:3806 — NextWeaponStrafeT = curTime + math.Rand(Weapon_StrafeCooldown.a, Weapon_StrafeCooldown.b)
                            }

                            // ═══ C2c-v: Non-VJ weapons (lua:3808-3816) ═══
                            // lua:3808: else — None VJ Base weapons
                            // SKIP: lua:3809 — SetTurnTarget("Enemy") — Phase 3 turning
                            // SKIP: lua:3810 — WeaponAttackState = VJ.WEP_ATTACK_STATE_FIRE_STAND
                            // SKIP: lua:3811 — OnWeaponAttack() — Phase 3 weapon callback
                            // SKIP: lua:3812 — WeaponLastShotTime = curTime
                            // SKIP: lua:3814 — self:SetSchedule(SCHED_RANGE_ATTACK1) — Source engine schedule, no S&Box equivalent
                        }
                    }
                }
            }
        }

        // ═══ goto_conditions: Handle player pushing yield (lua:3821-3837) ═══
        goto_conditions:
        // lua:3823
        if (HasCondition(Condition.PlayerPushing) && curTime > TakingCoverT && !IsBusy("Activities"))
        {
            // lua:3824
            PlaySoundSystem("YieldToPlayer");

            // Build schedule_yield_player inline (lua:3512-3517 — module-level shared in Lua)
            var yieldSched = new AISchedule();
            yieldSched.Init("SCHEDULE_YIELD_PLAYER");
            yieldSched.EngTask(EngineTask.MoveAwayPath, 120);
            yieldSched.EngTask(EngineTask.RunPath, 0);
            yieldSched.EngTask(EngineTask.WaitForMovement, 0);
            yieldSched.CanShootWhenMoving = true;

            // lua:3825-3834: Set turn data based on enemy/target status
            if (eneValid)
            {
                // lua:3825-3826: Face current enemy (visible only)
                yieldSched.TurnData = new TurnData { Type = VJFaceStatus.EnemyVisible, Target = null };
            }
            else if (GetTarget().IsValid())
            {
                // lua:3828-3829: Face current target (visible only)
                yieldSched.TurnData = new TurnData { Type = VJFaceStatus.EntityVisible, Target = GetTarget() };
            }
            else
            {
                // lua:3831-3832: Reset turn data (nil = none)
                yieldSched.TurnData = new TurnData { Type = VJFaceStatus.None };
            }

            // lua:3835
            StartSchedule(yieldSched);
            TakingCoverT = curTime + 2; // lua:3836
        }
    }

    // ═══ GrenadeAttack — human_base/init.lua:3070-3186 ═══
    public virtual bool GrenadeAttack(GameObject customEnt = null, bool disableOwner = false)
    {
        // lua:3071: guard
        if (Dead || Flinching || AttackType == VJAttackType.Melee) return false;
        var eneData = Enemy;
        var ene = eneData.Target;
        var isLiveEnt = customEnt != null && customEnt.IsValid();
        var landDir = "FindBest";

        // lua:3083-3097: Determine landing direction
        if (ene.IsValid())
        {
            if (eneData.Visible)
            {
                landDir = "Enemy";
            }
            else
            {
                // lua:3089: attempt to flush enemy from hiding
                // SKIP: lua:3089 — self:VisibleVec(eneData.VisiblePos) — Phase 3 visibility
                // SKIP: lua:3089 — ene:GetPos():Distance(eneData.VisiblePos) <= GrenadeAttackMaxDistance
                bool canFlush = false;
                if (canFlush)
                {
                    landDir = "EnemyLastVis";
                }
                else if (!isLiveEnt)
                {
                    return false;
                }
            }
        }

        // lua:3099: OnGrenadeAttack("Init")
        if (OnGrenadeAttack("Init", customEnt)) return false;
        var seed = Time.Now;
        AttackSeed = (int)(seed * 1000);

        // SKIP: lua:3103-3113 — AnimTbl_GrenadeAttack / PlayAnim / ACT_INVALID / AttackAnim — Phase 3 animation system

        // lua:3115-3127: Turn toward target
        if (landDir == "Enemy")
        {
            if (GrenadeAttackAnimationFaceEnemy)
                SetTurnTarget("Enemy");
        }
        else if (landDir == "EnemyLastVis")
        {
            // SKIP: lua:3120 — SetTurnTarget(eneData.VisiblePos, AttackAnimDuration or 1.5) — Phase 3
        }
        else
        {
            // SKIP: lua:3122-3126 — VJ.TraceDirections / PICK / SetTurnTarget(bestPos) — Phase 3
        }

        // lua:3130-3164: Handle live entity positioning
        if (isLiveEnt)
        {
            // SKIP: lua:3131-3164 — customEnt.VJ_ST_Grabbed / SetMoveType / FollowBone / GetAttachment / LookupBone / GetBonePosition — Phase 3 entity parenting
        }

        // lua:3167-3169: Attack state + sound
        AttackType = VJAttackType.Grenade;
        AttackState = VJAttackState.Started;
        PlaySoundSystem("GrenadeAttack");

        // lua:3171-3183: Timer-based release
        var releaseTime = GrenadeAttackThrowTime;
        if (releaseTime <= 0)
        {
            // lua:3173: event-based attack — execute immediately + schedule reset timer
            ScheduleAttackTimers();
            ExecuteGrenadeAttack(customEnt, disableOwner, landDir);
        }
        else
        {
            // lua:3178-3183 — attack_grenade_start timer → polling via GrenadeExecTime
            float rate = MathF.Max(AnimPlaybackRate, 0.01f);
            StashedGrenadeEnt = customEnt;
            StashedGrenadeDisableOwner = disableOwner;
            StashedGrenadeLandDir = landDir;
            GrenadeExecTime = Time.Now + releaseTime / rate;
        }
        // lua:3184: OnGrenadeAttack("PostInit")
        OnGrenadeAttack("PostInit", customEnt, landDir);
        return true;
    }

    // ═══ ExecuteGrenadeAttack — human_base/init.lua:3204-3331 ═══
    public virtual GameObject ExecuteGrenadeAttack(GameObject customEnt = null, bool disableOwner = false, object landDir = null)
    {
        // lua:3205: guard
        if (Dead || PauseAttacks || Flinching || AttackType == VJAttackType.Melee) return null;
        GameObject grenade;
        var isLiveEnt = customEnt != null && customEnt.IsValid();
        var fuseTime = GrenadeAttackFuseTime;

        // lua:3215-3234: Determine spawn position and angle
        Vector3? spawnPos = null;
        Angles spawnAng = Angles.Zero;
        // SKIP: lua:3215 — OnGrenadeAttack("SpawnPos") custom position — Phase 3
        // SKIP: lua:3218-3233 — LookupAttachment / GetAttachment / LookupBone / GetBonePosition / GetShootPos — Phase 3 bone/animation system
        spawnPos = WorldPosition + WorldRotation.Forward * 30;

        // lua:3238-3254: Determine landing position
        var landingPos = WorldPosition + WorldRotation.Forward * 200;
        if (landDir is string ld)
        {
            if (ld == "Enemy")
            {
                // SKIP: landingPos = GetEnemyLastKnownPos() — Phase 3
            }
            else if (ld == "EnemyLastVis")
            {
                // SKIP: landingPos = Enemy.VisiblePos — Phase 3
            }
        }
        else if (landDir is Vector3 vec)
        {
            landingPos = vec;
        }
        else
        {
            // SKIP: lua:3249-3253 — VJ.TraceDirections / PICK — Phase 3 utility
        }

        // lua:3258-3274: Create or reuse grenade entity
        if (isLiveEnt)
        {
            // lua:3259-3263: clean up parent/move type/follow bone
            // SKIP: lua:3259-3263 — SetParent(NULL) / SetMoveType / RemoveEffects(EF_FOLLOWBONE) — Phase 3 entity system
            grenade = customEnt;
        }
        else
        {
            // lua:3267-3273: ents.Create(customEnt or PICK(GrenadeAttackEntity)) + SetModel
            // SKIP: lua:3267-3273 — ents.Create / PICK(GrenadeAttackEntity) / SetModel — Phase 3 spawning system
            return null;
        }

        // lua:3276-3278: SetOwner + position
        if (!disableOwner)
        {
            // SKIP: lua:3276 — grenade:SetOwner(self) — Phase 3 entity ownership
        }
        grenade.WorldPosition = spawnPos.Value;
        grenade.WorldRotation = spawnAng;

        // SKIP: lua:3280-3308 — GetClass-based fuse timer dispatch (npc_grenade_frag / obj_vj_grenade / etc.) — Phase 3 grenade class system

        // lua:3305-3307: OnGrenadeAttackExecute + Spawn
        // SKIP: lua:3305-3307 — OnGrenadeAttackExecute("PreSpawn") / grenade:Spawn() / grenade:Activate() — Phase 3 spawning

        // lua:3311-3322: Throw velocity
        {
            // SKIP: lua:3312 — OnGrenadeAttackExecute("PostSpawn") — Phase 3
            var vel = (landingPos - grenade.WorldPosition) + (Vector3.Up * 200 + WorldRotation.Forward * 500 + WorldRotation.Right * Game.Random.Next(-20, 21));
            var rb = grenade.Components.Get<Rigidbody>();
            if (rb != null)
            {
                rb.Wake();
                // SKIP: lua:3317 — AddAngleVelocity — Phase 3
                rb.Velocity = vel;
            }
        }

        // lua:3324-3329: AttackState
        if (AttackState < VJAttackState.Executed)
        {
            AttackState = VJAttackState.Executed;
            // lua:3326-3328 — attackTimers[VJ.ATTACK_TYPE_GRENADE](self)
            ScheduleAttackTimers();
        }
        return grenade;
    }

    // ═══ ResetEnemy — human_base/init.lua:3840-3916 ═══
    /// <summary>
    /// Human override of ResetEnemy (replaces core.lua version).
    /// Lua signature: ENT:ResetEnemy(checkAllies, checkVis)
    /// C# maps checkAllies→param1, checkVis→param2 via base virtual signature.
    /// </summary>
    protected override void ResetEnemy(bool checkAllies, bool checkVis)
    {
        // lua:3841 — selfData = funcGetTable(self) → this
        // lua:3842 — if selfData.Dead or (selfData.VJ_IsBeingControlled && selfData.VJ_TheControllerBullseye == funcGetEnemy(self)) then selfData.EnemyData.Reset = false return false end
        // SKIP: lua:3842 — Dead || (VJ_IsBeingControlled && VJ_TheControllerBullseye == GetEnemy()) — Phase 3 tool system
        //        (VJ_TheControllerBullseye not ported; Dead guard partially — Dead NPCs shouldn't be in Think anyway)

        // lua:3843 — ene = funcGetEnemy(self)
        var ene = GetEnemy();
        // lua:3844 — eneValid = IsValid(ene)
        bool eneValid = ene.IsValid();
        // lua:3845 — eneData = selfData.EnemyData
        var eneData = Enemy;
        // lua:3846 — curTime = CurTime()
        float curTime = Time.Now;

        // ---- Block 1: Ally enemy inheritance (lua:3847-3861) ----
        // lua:3847 — if checkAllies then
        if (checkAllies)
        {
            // lua:3848 — getAllies = self:Allies_Check(1000)
            // SKIP: lua:3848 — Allies_Check(1000) returns allies list — Phase 3 ally system (stub returns void)
            // lua:3849 — if getAllies then
            // lua:3850-3859 — for _, ally in ipairs(getAllies) do
            //   allyEne = funcGetEnemy(ally)
            //   if IsValid(allyEne) && (curTime - ally.EnemyData.VisibleTime) < selfData.EnemyTimeout
            //      && allyEne:Alive() && self:GetPos():Distance(allyEne:GetPos()) <= self:GetMaxLookDistance()
            //      && self:CheckRelationship(allyEne) == D_HT then
            //     selfData.AllowWeaponOcclusionDelay = false
            //     self:ForceSetEnemy(allyEne, false)
            //     eneData.VisibleTime = curTime
            //     eneData.Reset = false
            //     return false
            // SKIP: lua:3847-3861 — full ally-enemy-inheritance block — Phase 3 ally system
        }

        // ---- Block 2: VisibleCount / reachable enemies guard (lua:3862-3874) ----
        // lua:3862 — if checkVis then
        if (checkVis)
        {
            // lua:3864 — curEnemies = eneData.VisibleCount // selfData.CurrentReachableEnemies
            // SKIP: lua:3864 — VisibleCount // CurrentReachableEnemies (Lua integer divide) — Phase 3 reachability
            // lua:3865 — if (eneValid && (curEnemies - 1) >= 1) or (!eneValid && curEnemies >= 1) then
            // SKIP: lua:3865 — reachable enemies guard — Phase 3
            // lua:3866 — self:MaintainRelationships() — Select a new enemy
            // SKIP: lua:3866 — MaintainRelationships — Phase 3
            // lua:3869-3872 — if eneData.VisibleCount > 0 then eneData.Reset = false return false end
            // SKIP: lua:3869-3872 — VisibleCount > 0 guard — Phase 3
        }

        // ---- Block 3: Debug print (lua:3876) ----
        // lua:3876 — if selfData.VJ_DEBUG && GetConVar("vj_npc_debug_resetenemy"):GetInt() == 1 then VJ.DEBUG_Print(self, "ResetEnemy", tostring(ene)) end
        // SKIP: lua:3876 — VJ_DEBUG + convar + VJ.DEBUG_Print — Phase 3 debug system

        // ---- Block 4: Reset state + alert timeout timer (lua:3877-3879) ----
        // lua:3877 — eneData.Reset = true
        eneData.Reset = true;
        // lua:3878 — self:SetNPCState(NPC_STATE_ALERT)
        SetNPCState((int)NPCState.Alert);
        // lua:3879 — timer.Create("alert_reset" .. self:EntIndex(), math.Rand(selfData.AlertTimeout.a, selfData.AlertTimeout.b), 1, function()
        //   if !IsValid(funcGetEnemy(self)) then selfData.Alerted = false self:SetNPCState(NPC_STATE_IDLE) end end)
        // SKIP: lua:3879 — timer.Create (Source engine one-shot named timer) — Phase 3 timer system
        //        Equivalent: NextAlertResetT = curTime + AlertTimeout random; poll in Think → if no enemy → Alerted=false, SetNPCState(Idle)

        // ---- Block 5: OnResetEnemy callback (lua:3880) ----
        // lua:3880 — self:OnResetEnemy()
        OnResetEnemy();

        // ---- Block 6: Move to last known position (lua:3881-3915) ----
        // lua:3881 — moveToEnemy = false
        Vector3? moveToEnemy = null;
        // lua:3882 — if eneValid then
        if (eneValid)
        {
            // lua:3883 — if !selfData.IsFollowing && !selfData.IsGuard && !selfData.IsVJBaseSNPC_Tank && !selfData.VJ_IsBeingControlled
            //   && selfData.LastHiddenZone_CanWander == true && !selfData.Weapon_UnarmedBehavior_Active
            //   && selfData.Behavior != VJ_BEHAVIOR_PASSIVE && selfData.Behavior != VJ_BEHAVIOR_PASSIVE_NATURE
            //   && !self:IsBusy() && !self:Visible(ene) && self:GetEnemyLastKnownPos() != defPos then
            // SKIP: lua:3883a — IsVJBaseSNPC_Tank cross-entity check — Phase 3 tank flag (always false for HumanNPC)
            bool canMoveToEnemy = !IsFollowing
                && !IsGuard
                // && !IsVJBaseSNPC_Tank // SKIP (always false for HumanNPC, TankNPC has its own override)
                && !VJ_IsBeingControlled
                && LastHiddenZone_CanWander == true
                && !Weapon_UnarmedBehavior_Active
                && Behavior != VJBehavior.Passive
                && Behavior != VJBehavior.PassiveNature
                && !IsBusy()
                && !Visible(ene);
            if (canMoveToEnemy)
            {
                // SKIP: lua:3883b — GetEnemyLastKnownPos() != defPos — Phase 3 enemy memory (returns Vector3.Zero stub)
                // lua:3884 — moveToEnemy = self:GetEnemyLastKnownPos()
                var lastKnownPos = GetEnemyLastKnownPos();
                if (lastKnownPos != Vector3.Zero)
                    moveToEnemy = lastKnownPos;
            }

            // lua:3886 — self:MarkEnemyAsEluded(ene)
            MarkEnemyAsEluded(ene);
            // lua:3887 — //self:ClearEnemyMemory(ene) — commented out in Lua
            // lua:3888 — self:AddEntityRelationship(ene, D_NU, 10)
            AddEntityRelationship(ene, (int)VJBase.Disposition.Neutral, 10);
        }

        // ---- Block 7: LastHiddenZone cleanup (lua:3891-3892) ----
        // lua:3891 — selfData.LastHiddenZone_CanWander = curTime > selfData.LastHiddenZoneT and true or false
        LastHiddenZone_CanWander = curTime > LastHiddenZoneT;
        // lua:3892 — selfData.LastHiddenZoneT = 0
        LastHiddenZoneT = 0;

        // ---- Block 8: Clear dead non-player enemy memory (lua:3894-3898) ----
        // lua:3895 — if eneValid && !ene:IsPlayer() && !ene:Alive() then
        if (eneValid && !ene.Components.Get<PlayerBase>().IsValid() && !Alive(ene))
        {
            // lua:3897 — self:ClearEnemyMemory(ene)
            ClearEnemyMemory(ene);
        }

        // ---- Block 9: Cover schedule stuck-loop fix (lua:3899-3902) ----
        // lua:3900 — if selfData.CurrentScheduleName == "SCHEDULE_COVER_ENEMY" or selfData.CurrentScheduleName == "SCHEDULE_COVER_ENEMY_FAIL" then
        if (CurrentScheduleName == "SCHEDULE_COVER_ENEMY" || CurrentScheduleName == "SCHEDULE_COVER_ENEMY_FAIL")
        {
            // lua:3901 — self:StopMoving()
            StopMoving();
        }

        // ---- Block 10: Wander time + SetEnemy(null) (lua:3903-3904) ----
        // lua:3903 — selfData.NextWanderTime = curTime + math.Rand(3, 5)
        NextWanderTime = curTime + 3 + (float)Game.Random.NextDouble() * 2;
        // lua:3904 — self:SetEnemy(NULL)
        SetEnemy(null);

        // ---- Block 11: GOTO last known position (lua:3905-3915) ----
        // lua:3905 — if moveToEnemy then
        if (moveToEnemy.HasValue)
        {
            // lua:3906 — self:SetLastPosition(moveToEnemy)
            SetLastPosition(moveToEnemy.Value);
            // lua:3907-3914 — self:SCHEDULE_GOTO_POSITION("TASK_WALK_PATH", function(schedule) ... end)
            SCHEDULE_GOTO_POSITION("TASK_WALK_PATH", schedule =>
            {
                // lua:3908 — //if eneValid then schedule:EngTask("TASK_FORGET", ene) end — commented out
                // lua:3909 — //schedule:EngTask("TASK_IGNORE_OLD_ENEMIES", 0) — commented out
                // lua:3910 — schedule.ResetOnFail = true
                schedule.ResetOnFail = true;
                // lua:3911 — schedule.CanShootWhenMoving = true
                schedule.CanShootWhenMoving = true;
                // lua:3912 — schedule.CanBeInterrupted = true
                schedule.CanBeInterrupted = true;
                // lua:3913 — schedule.TurnData = {Type = VJ.FACE_ENEMY}
                schedule.TurnData = new TurnData { Type = VJFaceStatus.Enemy };
            });
        }
    }

    // ═══ OnTakeDamage — human_base/init.lua:3918-4172 ═══
    /// <summary>
    /// Source engine damage callback. Returns 0 to block damage, 1 to allow.
    /// dmginfo: Source CTakeDamageInfo (Phase 3 → S&Box DamageInfo).
    /// hitgroup: LastDamageHitGroup (pre-extracted from engine, matches Lua:3932).
    /// </summary>
    public virtual int OnTakeDamage(object dmginfo, int hitgroup)
    {
        // ---- Block A: Entry guards (lua:3918-3923) ----
        // lua:3919 — dmgAttacker = dmginfo:GetAttacker()
        // SKIP: lua:3919 — dmginfo:GetAttacker() — Source engine CTakeDamageInfo API
        // lua:3920 — if !IsValid(dmgAttacker) then dmgAttacker = false end
        // SKIP: lua:3920 — IsValid(dmgAttacker) guard — Phase 3 S&Box DamageInfo
        // lua:3923 — Don't take bullet damage from friendly NPCs
        // SKIP: lua:3923 — dmgAttacker && dmginfo:IsBulletDamage() && dmgAttacker:IsNPC()
        //        && dmgAttacker:Disposition(self) != D_HT
        //        && (dmgAttacker:GetClass() == self:GetClass() || self:Disposition(dmgAttacker) == D_LI)
        //        → return 0 — Phase 3 DamageInfo + entity type system

        // ---- Block B: Inflictor + ragdoll guard (lua:3925-3929) ----
        // lua:3925 — dmgInflictor = dmginfo:GetInflictor()
        // SKIP: lua:3925 — dmginfo:GetInflictor() — Source engine CTakeDamageInfo API
        // lua:3926 — if !IsValid(dmgInflictor) then dmgInflictor = false end
        // SKIP: lua:3926 — IsValid(dmgInflictor) guard — Phase 3 S&Box DamageInfo
        // lua:3929 — Attempt to avoid taking damage when walking on ragdolls
        // SKIP: lua:3929 — dmgInflictor && dmgInflictor:GetClass() == "prop_ragdoll"
        //        && dmgInflictor:GetVelocity():Length() <= 100 → return 0 — Phase 3 entity type + physics

        // ---- Block C: Init + Guard (lua:3931-3934) ----
        // lua:3931 — selfData = funcGetTable(self) → in C#: this
        // lua:3932 — hitgroup = self:GetLastDamageHitGroup() → passed as parameter
        // lua:3933
        OnDamaged(dmginfo, hitgroup, "Init");
        // lua:3934 — if selfData.GodMode or dmginfo:GetDamage() <= 0 then return 0 end
        // SKIP: lua:3934 — GodMode || dmginfo:GetDamage() <= 0 → return 0 — Phase 3 godmode + DamageInfo

        // ---- Block D: Fire entity detection (lua:3936-3942) ----
        // lua:3936 — dmgType = dmginfo:GetDamageType()
        // SKIP: lua:3936 — dmginfo:GetDamageType() — Source engine CTakeDamageInfo API
        // lua:3937 — curTime = CurTime()
        float curTime = Time.Now;
        // lua:3938
        bool isFireEnt = false;
        // lua:3939 — if self:IsOnFire() then
        // SKIP: lua:3939 — self:IsOnFire() — Phase 3 fire system
        // lua:3940 — isFireEnt = dmgInflictor && dmgAttacker && dmgInflictor:GetClass() == "entityflame" && dmgAttacker:GetClass() == "entityflame"
        // SKIP: lua:3940 — entityflame class check — Phase 3 fire entity type
        // lua:3941 — if self:WaterLevel() > 1 then self:Extinguish() end
        // SKIP: lua:3941 — WaterLevel() > 1 → Extinguish() — Phase 3 water/fire system

        // ---- Block E: Boss bypass (lua:3944-3947) ----
        // lua:3945-3946 — if dmgAttacker && selfData.ForceDamageFromBosses && dmgAttacker.VJ_ID_Boss then goto skip_immunity end
        // SKIP: lua:3945-3946 — dmgAttacker.VJ_ID_Boss cross-entity read → goto skip_immunity — Phase 3 boss flag system

        // ---- Block F: Immunity chain (lua:3949-3951) ----
        // lua:3950 — if isFireEnt && !selfData.AllowIgnition then self:Extinguish() return 0 end
        // SKIP: lua:3950 — AllowIgnition fire guard → Extinguish() + return 0 — Phase 3 fire immunity
        // lua:3951 — Full immunity OR-chain (8 types)
        // SKIP: lua:3951 — Immune_Fire && (DMG_BURN||DMG_SLOWBURN||isFireEnt)
        //        || Immune_Toxic && (DMG_ACID||DMG_RADIATION||DMG_POISON||DMG_NERVEGAS||DMG_PARALYZE)
        //        || Immune_Bullet && (dmginfo:IsBulletDamage()||DMG_BULLET||DMG_AIRBOAT||DMG_BUCKSHOT||DMG_SNIPER)
        //        || Immune_Explosive && (DMG_BLAST||DMG_BLAST_SURFACE||DMG_MISSILEDEFENSE)
        //        || Immune_Dissolve && dmginfo:IsDamageType(DMG_DISSOLVE)
        //        || Immune_Electricity && (DMG_SHOCK||DMG_ENERGYBEAM||DMG_PHYSGUN)
        //        || Immune_Melee && (DMG_CLUB||DMG_SLASH)
        //        || Immune_Sonic && DMG_SONIC → return 0
        //        — Phase 3 DamageInfo.Tags (use VJDamageTags constants)

        // ---- Block G: skip_immunity label + combine ball (lua:3953-3964) ----
        // lua:3953 — ::skip_immunity::
        skip_immunity:
        // lua:3954 — if (dmgInflictor && dmgInflictor:GetClass() == "prop_combine_ball") or (dmgAttacker && dmgAttacker:GetClass() == "prop_combine_ball") then
        // SKIP: lua:3954 — prop_combine_ball class check — Phase 3 entity type system
        // lua:3955 — if selfData.Immune_Dissolve then return 0 end
        // SKIP: lua:3955 — Immune_Dissolve dissolve block — Phase 3 immunity
        // lua:3956-3959 — if curTime > selfData.NextCombineBallDmgT then dmginfo:SetDamage(math.random(400,500)) dmginfo:SetDamageType(DMG_DISSOLVE) selfData.NextCombineBallDmgT = curTime + 0.2
        // SKIP: lua:3956-3959 — combine ball damage scaling — Phase 3 DamageInfo
        // lua:3960-3962 — else return 0 end
        // SKIP: lua:3960-3962 — combine ball spam prevention → return 0 — Phase 3

        // ---- Block H: DoBleed helper (lua:3966-3974) ----
        // lua:3966 — local function DoBleed()
        void DoBleed()
        {
            // lua:3967 — if selfData.Bleeds then
            if (Bleeds)
            {
                // lua:3968 — self:OnBleed(dmginfo, hitgroup)
                OnBleed(dmginfo, hitgroup);
                // lua:3970 — if selfData.HasBloodParticle && !isFireEnt then self:SpawnBloodParticles(dmginfo, hitgroup) end
                // SKIP: lua:3970 — HasBloodParticle && !isFireEnt → SpawnBloodParticles(dmginfo, hitgroup) — Phase 3 blood effects
                // lua:3971 — if selfData.HasBloodDecal then self:SpawnBloodDecals(dmginfo, hitgroup) end
                // SKIP: lua:3971 — HasBloodDecal → SpawnBloodDecals(dmginfo, hitgroup) — Phase 3 blood effects
                // lua:3972 — self:PlaySoundSystem("Impact")
                PlaySoundSystem("Impact");
            }
        }

        // ---- Block I: Dead guard (lua:3975) ----
        // lua:3975 — if selfData.Dead then DoBleed() return 0 end
        if (Dead) { DoBleed(); return 0; }

        // ---- Block J: PreDamage + damage application (lua:3977-4000) ----
        // lua:3977 — self:OnDamaged(dmginfo, hitgroup, "PreDamage")
        OnDamaged(dmginfo, hitgroup, "PreDamage");
        // lua:3978 — if dmginfo:GetDamage() <= 0 then return 0 end
        // SKIP: lua:3978 — dmginfo:GetDamage() <= 0 → return 0 — Phase 3 DamageInfo
        // lua:3980-3990 — selfData.SavedDmgInfo = { dmginfo, attacker, inflictor, amount, pos, type, force, ammoType, hitgroup }
        // SKIP: lua:3980-3990 — SavedDmgInfo snapshot table (GMod resets dmginfo after tick) — Phase 3
        // lua:3991 — self:SetHealth(self:Health() - dmginfo:GetDamage())
        // SKIP: lua:3991 — SetHealth(Health() - damage) — Phase 3 HealthComponent
        // lua:3992 — VJ_DEBUG damage print
        // SKIP: lua:3992 — VJ_DEBUG && vj_npc_debug_damage:GetInt()==1 → VJ.DEBUG_Print — Phase 3 debug
        // lua:3993-3995 — healthRegen = selfData.HealthRegenParams; if healthRegen.Enabled && healthRegen.ResetOnDmg then HealthRegenDelayT = ...
        // SKIP: lua:3993-3995 — HealthRegenParams (Enabled, ResetOnDmg, Delay) — Phase 3 health regen
        // lua:3997-3998 — self:SetSaveValue("m_iDamageCount", ...) / self:SetSaveValue("m_flLastDamageTime", curTime)
        // SKIP: lua:3997-3998 — SetSaveValue (Source engine save/restore) — Phase 3 persistence
        // lua:3999 — self:OnDamaged(dmginfo, hitgroup, "PostDamage")
        OnDamaged(dmginfo, hitgroup, "PostDamage");
        // lua:4000 — DoBleed()
        DoBleed();

        // ---- Block K: I/O events (lua:4002-4008) ----
        // lua:4003-4007 — if dmgAttacker then self:TriggerOutput("OnDamaged", dmgAttacker) self:MarkTookDamageFromEnemy(dmgAttacker) else self:TriggerOutput("OnDamaged", self) end
        // SKIP: lua:4003-4007 — TriggerOutput / MarkTookDamageFromEnemy — Phase 3 I/O system (stubs exist)

        // ---- Block L: Pain sounds (lua:4010-4011) ----
        // lua:4010 — stillAlive = self:Health() > 0
        // SKIP: lua:4010 — Health() > 0 guard — Phase 3 HealthComponent
        // lua:4011 — if stillAlive then self:PlaySoundSystem("Pain") end
        PlaySoundSystem("Pain");

        // ---- Block M: AI response (lua:4013-4151) ----
        // lua:4013 — if VJ_CVAR_AI_ENABLED && self:GetState() != VJ_STATE_FREEZE then
        // SKIP: lua:4013 — VJ_CVAR_AI_ENABLED convar check — Phase 3 convar system (assume enabled)
        if (GetState() != VJState.Freeze)
        {
            // lua:4014 — isPassive = selfData.Behavior == VJ_BEHAVIOR_PASSIVE or selfData.Behavior == VJ_BEHAVIOR_PASSIVE_NATURE
            var isPassive = Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature;

            // lua:4015 — if stillAlive then
            // SKIP: lua:4015 — stillAlive guard (outer) — Phase 3 HealthComponent
            {
                // ---- M1: Flinch (lua:4016-4017) ----
                // lua:4016 — if !isFireEnt then
                // SKIP: lua:4016 — !isFireEnt guard — Phase 3 fire system
                // lua:4017 — self:Flinch(dmginfo, hitgroup)
                Flinch(dmginfo, hitgroup);

                // ---- M2: Player attacker → BecomeEnemyToPlayer (lua:4020-4052) ----
                // lua:4021 — if dmgAttacker && dmgAttacker:IsPlayer() then
                // SKIP: lua:4021 — dmgAttacker:IsPlayer() — Phase 3 DamageInfo + player detection
                // lua:4023-4041 — BecomeEnemyToPlayer hostility counter:
                //   if selfData.BecomeEnemyToPlayer && self:CheckRelationship(dmgAttacker) == D_LI then
                //     self:SetRelationshipMemory(dmgAttacker, VJ.MEM_HOSTILITY_LEVEL, ...)
                //     if relationMemory[VJ.MEM_HOSTILITY_LEVEL] > selfData.BecomeEnemyToPlayer && self:Disposition(dmgAttacker) != D_HT then
                //       self:OnBecomeEnemyToPlayer(dmginfo, hitgroup)
                //       if selfData.IsFollowing && selfData.FollowData.Target == dmgAttacker then self:ResetFollowBehavior() end
                //       self:SetRelationshipMemory(dmgAttacker, VJ.MEM_OVERRIDE_DISPOSITION, D_HT)
                //       self:AddEntityRelationship(dmgAttacker, D_HT, 2)
                //       selfData.TakingCoverT = curTime + 2
                //       self:PlaySoundSystem("BecomeEnemyToPlayer")
                //       if !IsValid(funcGetEnemy(self)) then self:StopMoving() self:SetTarget(dmgAttacker) self:SCHEDULE_FACE("TASK_FACE_TARGET") end
                //       if selfData.CanChatMessage then dmgAttacker:PrintMessage(HUD_PRINTTALK, ...) end
                // lua:4044-4051 — DamageByPlayer sounds:
                //   if selfData.HasDamageByPlayerSounds && curTime > selfData.NextDamageByPlayerSoundT && self:Visible(dmgAttacker) then
                //     dispLvl = selfData.DamageByPlayerDispositionLevel
                //     if dispLvl == 0 or (dispLvl == 1 && Disposition == D_LI) or (dispLvl == 2 && Disposition != D_HT) then
                //       self:PlaySoundSystem("DamageByPlayer")
                // SKIP: lua:4020-4051 — full M2 player-attacker block — Phase 3 DamageInfo + player + relationship

                // ---- M2.5: Pain sound inside AI block (lua:4054) ----
                // lua:4054 — self:PlaySoundSystem("Pain")
                PlaySoundSystem("Pain");

                // ---- M3: Combat damage response — take cover from visible enemy (lua:4056-4076) [HUMAN ONLY] ----
                // lua:4057 — eneData = selfData.EnemyData
                // lua:4058 — if !isPassive && selfData.CombatDamageResponse && IsValid(eneData.Target) && curTime > selfData.NextCombatDamageResponseT && !selfData.IsFollowing && !selfData.AttackType && !self:IsBusy() && curTime > selfData.TakingCoverT && eneData.Visible && self:GetWeaponState() != VJ.WEP_STATE_RELOADING && eneData.Distance < selfData.Weapon_MaxDistance then
                // SKIP: lua:4058 — CombatDamageResponse multi-guard — Phase 3 combat damage response
                // lua:4059 — wep = funcGetActiveWeapon(self)
                // SKIP: lua:4059 — funcGetActiveWeapon — Phase 3 weapon system
                // lua:4060 — canMove = true
                // lua:4061 — if self:DoCoverTrace(self:GetPos() + self:OBBCenter(), eneData.Target:EyePos()) then
                // SKIP: lua:4061 — DoCoverTrace + OBBCenter + EyePos — Phase 3 cover + collision
                // lua:4062-4069 — AnimTbl_TakingCover play + timer setup (hideTime, NextChaseTime, TakingCoverT, WeaponAttackState, NextCombatDamageResponseT)
                // SKIP: lua:4062-4069 — PlayAnim(AnimTbl_TakingCover) → ACT_INVALID guard — Phase 3 animation
                // lua:4072 — if canMove && !self:IsMoving() && (!IsValid(wep) or (IsValid(wep) && !wep.IsMeleeWeapon)) then
                // SKIP: lua:4072-4075 — SCHEDULE_COVER_ENEMY("TASK_RUN_PATH") with FACE_ENEMY turn + NextCombatDamageResponseT — Phase 3 cover + weapon
                // SKIP: lua:4056-4076 — full M3 combat-damage-response block — Phase 3 combat damage response

                // ---- M4: No enemy response — ally alert + damage response (lua:4078-4128) ----
                // lua:4078 — if !isPassive && !IsValid(funcGetEnemy(self)) then
                // SKIP: lua:4078 — !isPassive && !GetEnemy() guard — Phase 3
                // lua:4079 — canMove = true
                // lua:4082-4102 — DamageAllyResponse:
                //   if selfData.DamageAllyResponse && curTime > selfData.NextDamageAllyResponseT && !selfData.IsFollowing then
                //     responseDist = math_max(800, self:OBBMaxs():Distance(self:OBBMins()) * 12)
                //     allies = self:Allies_Check(responseDist)
                //     if allies != false then
                //       if !isFireEnt then self:Allies_Bring("Diamond", responseDist, allies, 4) end
                //       for _, ally in ipairs(allies) do ally:DoReadyAlert() end
                //       if !isFireEnt && !self:IsBusy("Activities") then
                //         self:DoReadyAlert()
                //         anim = self:PlayAnim(selfData.AnimTbl_DamageAllyResponse, true, false, true)
                //         if anim != ACT_INVALID then canMove = false; selfData.NextFlinchT = curTime + 1 end
                //       end
                //       selfData.NextDamageAllyResponseT = curTime + math.Rand(selfData.DamageAllyResponse_Cooldown.a, selfData.DamageAllyResponse_Cooldown.b)
                // lua:4104-4128 — DamageResponse:
                //   dmgResponse = selfData.DamageResponse
                //   if dmgResponse && curTime > selfData.TakingCoverT && !self:IsBusy("Activities") then
                //     -- Attempt to find who damaged me
                //     if dmgAttacker && dmgAttacker.VJ_ID_Living && (dmgResponse == true or dmgResponse == "OnlySearch") then
                //       sightDist = math_min(math_max(sightDist / 2, sightDist <= 1000 and sightDist or 1000), sightDist)
                //       if self:GetPos():Distance(dmgAttacker:GetPos()) <= sightDist && self:Visible(dmgAttacker) then
                //         dispLvl = self:CheckRelationship(dmgAttacker)
                //         if dispLvl == D_HT or dispLvl == D_NU then
                //           self:OnSetEnemyFromDamage(dmginfo, hitgroup)
                //           selfData.NextCallForHelpT = curTime + 1
                //           self:ForceSetEnemy(dmgAttacker, true)
                //           self:MaintainAlertBehavior()
                //           canMove = false
                //     -- If all else failed then take cover!
                //     if canMove && (dmgResponse == true or dmgResponse == "OnlyMove") && !selfData.IsFollowing && selfData.MovementType != VJ_MOVETYPE_STATIONARY && dmginfo:GetDamageCustom() != VJ.DMG_BLEED then
                //       self:SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH", function(x) x.CanShootWhenMoving = true x.TurnData = {Type = VJ.FACE_ENEMY} end)
                //       selfData.TakingCoverT = curTime + 5
                // SKIP: lua:4078-4128 — full M4 ally-response + damage-response block — Phase 3 ally system + damage response

                // ---- M5: Passive NPC run away (lua:4130-4134) ----
                // lua:4131-4134 — elseif isPassive && curTime > selfData.TakingCoverT then if selfData.DamageResponse && !self:IsBusy() then self:SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH") end end
                // SKIP: lua:4131-4134 — isPassive run-away via SCHEDULE_COVER_ORIGIN — Phase 3 passive behavior
            }

            // ---- M6: Passive allies signal danger (lua:4138-4151) — OUTSIDE stillAlive block ----
            // lua:4139 — if isPassive && curTime > selfData.TakingCoverT then
            // SKIP: lua:4139 — isPassive + TakingCoverT guard — Phase 3
            // lua:4140-4149 — if selfData.Passive_AlliesRunOnDamage then
            //   allies = self:Allies_Check(math_max(800, self:OBBMaxs():Distance(self:OBBMins()) * 20))
            //   if allies != false then
            //     for _, ally in ipairs(allies) do
            //       ally.TakingCoverT = curTime + math.Rand(6, 7)
            //       ally:SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH")
            //       ally:PlaySoundSystem("Alert")
            //     end
            //   end
            // lua:4150 — selfData.TakingCoverT = curTime + math.Rand(6, 7)
            // SKIP: lua:4138-4151 — full M6 passive-allies-signal block — Phase 3 ally system
        }

        // ---- Block N: Stop eating (lua:4154-4158) ----
        // lua:4155-4157 — if selfData.CanEat && selfData.VJ_ST_Eating then selfData.EatingData.NextCheck = curTime + 15; self:ResetEatingBehavior("Injured") end
        // SKIP: lua:4155-4157 — CanEat && VJ_ST_Eating → ResetEatingBehavior("Injured") — Phase 3 eating system

        // ---- Block O: Death (lua:4160-4171) ----
        // lua:4160 — if self:Health() <= 0 && !selfData.Dead then
        // SKIP: lua:4160 — Health() <= 0 && !Dead — Phase 3 HealthComponent
        // lua:4161 — self:RemoveEFlags(EFL_NO_DISSOLVE)
        // SKIP: lua:4161 — RemoveEFlags(EFL_NO_DISSOLVE) — Phase 3 flags system
        // lua:4162-4168 — if IsDamageType(DMG_DISSOLVE) or prop_combine_ball then dissolve DamageInfo + TakeDamageInfo
        // SKIP: lua:4162-4168 — dissolve damage path — Phase 3 damage system
        // lua:4169 — self:BeginDeath(dmginfo, hitgroup)
        // SKIP: lua:4169 — BeginDeath(dmginfo, hitgroup) — Phase 3 death system (stub in CreatureNPC.Think.cs)

        // lua:4171 — return 1
        return 1;
    }

    // ═══ GetAttackSpread — human_base/init.lua:4515 ═══
    /// <summary>Weapon spread modifier. Lua returns nil (= 0 spread). Override in derived types.</summary>
    public virtual float GetAttackSpread(GameObject wep, GameObject target) => 0f;

    // ═══ playReloadAnimation (local func) — human_base/init.lua:2562-2582 ═══
    /// <summary>
    /// Play reload animation and schedule reload-complete timer.
    /// Source: init.lua local function, used by SelectSchedule reload logic.
    /// </summary>
    protected virtual bool PlayReloadAnimation(GameObject anims)
    {
        // lua:2563 — anim, animDur, animType = self:PlayAnim(anims, true, false, "Visible")
        // SKIP: lua:2563 — PlayAnim(anims, true, false, "Visible") — Phase 3 animation
        // lua:2564 — if anim != ACT_INVALID then
        // SKIP: lua:2564 — ACT_INVALID comparison — Phase 3 animation
        // lua:2565 — wep = self.WeaponEntity
        // lua:2566 — if wep.IsVJBaseWeapon then wep:NPC_Reload() end
        // SKIP: lua:2566 — IsVJBaseWeapon + NPC_Reload() — Phase 3 weapon interface
        // lua:2567-2573 — timer.Create("wep_reload_reset", animDur, ...) reload-complete
        // SKIP: lua:2567-2573 — timer.Create + SetClip1 + GetMaxClip1 + OnReload + SetWeaponState — Phase 3 timer + weapon
        // lua:2574 — self.AllowWeaponOcclusionDelay = false
        AllowWeaponOcclusionDelay = false;
        // lua:2576-2578 — if !VJ_IsBeingControlled && animType == ANIM_TYPE_GESTURE then StopMoving() end
        // SKIP: lua:2576 — animType == ANIM_TYPE_GESTURE — Phase 3 animation
        // lua:2579 — return true (reload animation ran successfully)
        // lua:2581 — return false (animation invalid)
        return false; // Phase 3: PlayAnim returns real animation
    }

    // ═══ attackTimers (local table) — human_base/init.lua:2536-2560 ═══
    // Replaced by BaseNPC.ScheduleAttackTimers() + ProcessAttackTimers() (timer→polling).
    // MELEE timer → AttackResetTime/AttackReEnableTime in ScheduleAttackTimers()
    // GRENADE timer → AttackResetTime/GrenadeExecTime/NextThrowGrenadeT in ScheduleAttackTimers()
    // Both polled in ProcessAttackTimers() called from Think.
}
