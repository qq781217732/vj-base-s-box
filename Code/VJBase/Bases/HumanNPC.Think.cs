using System;
using System.Linq;
using System.Threading.Tasks;
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
        EFL_NO_DISSOLVE = true; // Prevent dissolve until death; cleared in Block O
        // lua:2138: SetUseType(SIMPLE_USE) — Source engine; S&Box uses Component interactions

        // lua:2139-2144: if !self:GetModel() then model = PICK(self.Model); self:SetModel(model) end
        if (VJEntitySpawner.GetModelPath(GameObject) == null)
        {
            var modelPath = VJUtility.PICK(Model);
            if (!string.IsNullOrEmpty(modelPath))
            {
                var renderer = Components.GetOrCreate<ModelRenderer>();
                renderer.Model = Model.Load(modelPath);
            }
        }

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
        if (Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature)
        {
            Weapon_Disabled = true;
            Weapon_IgnoreSpawnMenu = true;
        }
        // lua:2183-2184 — elseif !Weapon_Disabled && !Weapon_IgnoreSpawnMenu then CapabilitiesAdd(capBitsWeapons) — SKIP (Source engine bit-flag caps)

        // lua:2187-2191: Health setup
        var hp = ScaleByDifficulty(StartHealth); // lua:2189: convar override or ScaleByDifficulty
        CurrentHealth = hp; // lua:2190 — SetHealth(hp) → basic float tracking (Phase 3→HealthComponent)
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
        // lua:2214 — GetCreator / IsGuard from creator
        if (Creator.IsValid() && Creator.Tags.Has("vj_spawn_guard"))
            IsGuard = true;
        // lua:2215 — StartSoundTrack()
        StartSoundTrack();
        // SKIP: lua:2218-2235 — LookupPoseParameter("aim_pitch"/"head_pitch"/"aim_yaw"/"head_yaw"/"aim_roll"/"head_roll") — Phase 3 animation pose params
        // lua:2237-2265: Weapon setup
        if (Weapon_Disabled)
        {
            // SKIP: lua:2238 — UpdateAnimationTranslations() — Phase 3 animation
        }
        else
        {
            // lua:2240 — local wep = funcGetActiveWeapon(self)
            var wep = GetActiveWeapon();
            // Fallback: search children for equipped VJBaseWeapon component
            if (!wep.IsValid())
            {
                foreach (var child in GameObject.Children)
                {
                    if (child.Components.Get<VJBaseWeapon>() != null)
                    { wep = child; break; }
                }
            }

            if (wep.IsValid())
            {
                // lua:2242 — self.WeaponEntity = self:DoChangeWeapon()
                WeaponEntity = DoChangeWeapon();
                // lua:2243 — self.WeaponInventory.Primary = wep
                WeaponInventory.Primary = wep;
                // lua:2244-2246 — VJBaseWeapon check + warning (Phase 3 messaging SKIP)

                // lua:2247-2251 — AntiArmor weapon from inventory list
                var antiArmorClass = VJUtility.PICK(WeaponInventory_AntiArmorList);
                if (antiArmorClass != null && wep.Name != antiArmorClass)
                {
                    var aaWep = GiveWeapon(antiArmorClass);
                    if (aaWep.IsValid())
                    {
                        WeaponInventory.AntiArmor = aaWep;
                        // lua:2250-2251 — SelectWeapon(wep) + wep:Equip(self); DoChangeWeapon(invSwitch) calls Equip internally
                        DoChangeWeapon(wep.Name, invSwitch: true);
                    }
                }

                // lua:2253-2258 — Melee weapon from inventory list
                var meleeClass = VJUtility.PICK(WeaponInventory_MeleeList);
                if (meleeClass != null && wep.Name != meleeClass)
                {
                    var meleeWep = GiveWeapon(meleeClass);
                    if (meleeWep.IsValid())
                    {
                        WeaponInventory.Melee = meleeWep;
                        DoChangeWeapon(wep.Name, invSwitch: true);
                    }
                }
            }
            else
            {
                // SKIP: lua:2260 — UpdateAnimationTranslations() — Phase 3 animation
                // lua:2261-2263 — CanChatMessage + PrintMessage warning — Phase 3 messaging
            }
        }
        // SKIP: lua:2266-2268 — GetIdealActivity() / MaintainIdleAnimation(true) — Phase 3 animation
        // SKIP: lua:2269-2279 — hook.Add("Think", self, funcAnimThink) — Phase 3 animation hook system
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

        // lua:2526-2530 — if time >= 0 then timer.Create else timer.Remove
        if (time >= 0)
        {
            NextWeaponStateChangeT = Time.Now + time;
        }
        else
        {
            NextWeaponStateChangeT = 0; // equivalent to timer.Remove — state persists until next explicit SetWeaponState
        }
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
        var curWep = GetActiveWeapon();

        // ---- Block 1: Weapon disabled → remove (lua:2476-2479) ----
        // lua:2476 — if self.Weapon_Disabled && IsValid(curWep) then
        if (Weapon_Disabled && curWep.IsValid())
        {
            // lua:2477 — curWep:Remove()
            curWep.Destroy();
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
                // lua:2484 — self:SelectWeapon(wep) → search inventory slots, then children
                GameObject targetWep = null;
                if (WeaponInventory.Primary?.Name == wep)
                    targetWep = WeaponInventory.Primary;
                else if (WeaponInventory.AntiArmor?.Name == wep)
                    targetWep = WeaponInventory.AntiArmor;
                else if (WeaponInventory.Melee?.Name == wep)
                    targetWep = WeaponInventory.Melee;
                if (targetWep == null)
                {
                    foreach (var child in GameObject.Children)
                    {
                        if (child.Name == wep) { targetWep = child; break; }
                    }
                }
                // lua:2485 — VJ.EmitSound(self, sdWepSwitch, 70)
                var wepSwitchSd = VJUtility.PICK(_sdWepSwitch);
                if (wepSwitchSd != null)
                {
                    var handle = Sound.Play(wepSwitchSd, WorldPosition);
                    handle.Distance = BaseNPC.DbToDistance(70); // lua:2485 — VJ.EmitSound(sdWepSwitch, 70)
                }
                // lua:2486 — curWep = wep
                curWep = targetWep; // null if not found → falls through to Block 3 else → None
            }
            // lua:2487 — else
            else
            {
                // lua:2488 — if IsValid(curWep) && self.WeaponInventoryStatus <= VJ.WEP_INVENTORY_PRIMARY then
                if (curWep.IsValid() && WeaponInventoryStatus <= VJWepInventory.Primary)
                {
                    // lua:2489 — curWep:Remove()
                    curWep.Destroy();
                }
                // lua:2491 — curWep = self:Give(wep) → create weapon GameObject as child of NPC
                curWep = new GameObject(GameObject, true, wep);
                var wepComp = curWep.Components.Create<VJBaseWeapon>();
                wepComp.Equip(GameObject);
                // lua:2492 — self.WeaponInventory.Primary = curWep
                WeaponInventory.Primary = curWep;
            }
        }

        // ---- Block 3: Setup valid weapon (lua:2497-2516) ----
        // lua:2497 — if IsValid(curWep) then
        if (curWep.IsValid())
        {
            // lua:2498 — self.WeaponAttackAnim = ACT_INVALID
            WeaponAttackAnim = null;
            // lua:2499 — self:SetWeaponState() — reset state
            SetWeaponState();
            // lua:2500 — if invSwitch then
            if (invSwitch)
            {
                // lua:2501 — if curWep.IsVJBaseWeapon then curWep:Equip(self) end
                var ivj = curWep.Components.Get<IVJBaseWeapon>();
                if (ivj != null && ivj.IsVJBaseWeapon)
                {
                    ivj.Equip(GameObject);
                }
            }
            // lua:2502 — else
            else
            {
                // lua:2503 — self.WeaponInventoryStatus = VJ.WEP_INVENTORY_PRIMARY
                WeaponInventoryStatus = VJWepInventory.Primary;
                // lua:2505-2509 — Replace old primary if different
                var curPrimary = WeaponInventory.Primary;
                if (curWep != curPrimary)
                {
                    // lua:2507 — if IsValid(curPrimary) then curPrimary:Remove() end
                    if (curPrimary.IsValid())
                        curPrimary.Destroy();
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

    /// <summary>
    /// GiveWeapon — Source ENT:Give(weaponClassName). Creates a weapon GameObject as child of NPC,
    /// adds VJBaseWeapon component, and returns it. Does NOT modify WeaponInventory.Primary.
    /// Used for AntiArmor / Melee inventory slot creation.
    /// </summary>
    public virtual GameObject GiveWeapon(string weaponClass)
    {
        if (string.IsNullOrEmpty(weaponClass)) return null;
        var wep = new GameObject(GameObject, true, weaponClass);
        var wepComp = wep.Components.Create<VJBaseWeapon>();
        wepComp.Equip(GameObject);
        wep.WorldPosition = WorldPosition;
        wep.WorldRotation = WorldRotation;
        return wep;
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
                    // lua:2349 — RememberUnreachable(ene, 0) = clear unreachable (LOS chase brought us closer)
                    RememberUnreachable(ene, 0);
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
            var headDir = GetHeadDirection();
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
        if ((HasCondition(Condition.EnemyUnreachable) || IsUnreachable(ene))
            && GetActiveWeapon().IsValid()
            && !IsWeaponMelee(GetActiveWeapon()))
        {
            SCHEDULE_ALERT_CHASE(true);
            // lua:2396 — RememberUnreachable(ene, 2) — mark unreachable so we don't re-evaluate immediately
            RememberUnreachable(ene, 2);
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
                if (IsWeaponMelee(curWep))
                {
                    // lua:3493-3496 — Melee: if VJ.IsCurrentAnim(self, WeaponAttackAnim) or enemyDist < Weapon_MaxDistance then hasDist = true end
                    // SKIP: lua:3494 — VJ.IsCurrentAnim(self, WeaponAttackAnim) — Phase 3 animation (VJUtility stub false)
                    if (enemyDist < Weapon_MaxDistance)
                    {
                        hasDist = true;
                    }
                }
                // lua:3497-3499 — elseif (ranged): enemyDist < Weapon_MaxDistance && enemyDist > Weapon_MinDistance then hasDist = true
                else if (enemyDist < Weapon_MaxDistance && enemyDist > Weapon_MinDistance)
                {
                    hasDist = true;
                }
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
            bool isDanger = BaseNPC.HasEntityFlag(ent, "VJ_ID_Danger");
            bool isGrenade = BaseNPC.HasEntityFlag(ent, "VJ_ID_Grenade");
            if ((isDanger || isGrenade) && Visible(ent))
            {
                // lua:3362-3363 — owner check via Tags (SetOwner stored as tag in ExecuteGrenadeAttack)
                {
                    // lua:3364 — if ent.VJ_ID_Danger then regDangerDetected = ent continue end
                    if (isDanger)
                    {
                        regDangerDetected = ent;
                        continue;
                    }
                    // lua:3365 — OnDangerDetected custom callback
                    if (OnDangerDetected(VJDangerType.Grenade, ent)) continue;
                    // lua:3366-3369 — curTime + sound + timers
                    float curTime = Time.Now;
                    if (PlaySoundSystem("GrenadeSight") == 0f)
                        PlaySoundSystem("DangerSight");
                    NextDangerDetectionT = curTime + 4;
                    TakingCoverT = curTime + 4;
                    // lua:3371-3374 — Grenade redirect: grab and throw back
                    if (CanRedirectGrenades && HasGrenadeAttack
                        && BaseNPC.HasEntityFlag(ent, "VJ_ID_Grabbable")
                        && !BaseNPC.HasEntityFlag(ent, "VJ_ST_Grabbed")
                        && (ent.Components.Get<Rigidbody>()?.Velocity.Length ?? 9999) < 400
                        && VJUtility.GetNearestDistance(GameObject, ent, false) < 100
                        && GrenadeAttack(ent, true))
                    {
                        NextGrenadeAttackSoundT = curTime + 3;
                        return;
                    }
                    // lua:3376-3379 — take cover from grenade
                    SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH", schedule =>
                    {
                        schedule.CanShootWhenMoving = true;
                        schedule.TurnData = new TurnData { Type = VJFaceStatus.Enemy };
                    });
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

            // lua:3518+3539-3576: Investigation block — hear danger → look toward sound source
            var bitsDanger = VJSoundType.BulletImpact | VJSoundType.Combat | VJSoundType.World | VJSoundType.Danger;
            if (CanInvestigate
                && (HasCondition(Condition.HearBulletImpact)
                    || HasCondition(Condition.HearCombat)
                    || HasCondition(Condition.HearWorld)
                    || HasCondition(Condition.HearDanger))
                && NextInvestigationMove < curTime
                && TakingCoverT < curTime
                && !IsBusy())
            {
                // lua:3541 — GetBestSoundHint(bitsDanger)
                var sdSrc = GetBestSoundHint(bitsDanger);
                if (sdSrc != null)
                {
                    // lua:3544-3554 — owner/type/vehicle/driver/disposition checks
                    bool allowed = true;
                    var sdOwner = sdSrc.Owner;
                    if (sdOwner.IsValid())
                    {
                        // lua:3548 — ignore danger sounds from vehicles driven by allies
                        // SKIP: lua:3548 — IsVehicle() + GetDriver() — Phase 3 vehicle system (no S&Box vehicle component yet)
                        //    if sdSrc.type == SOUND_DANGER && sdOwner:IsVehicle() && IsValid(sdOwner:GetDriver())
                        //        && Disposition(GetDriver()) == D_LI → allowed = false
                        // lua:3551 — ignore sounds from allies OR combat/death sounds from dead NPCs
                        if (Disposition(sdOwner) == (int)VJBase.Disposition.Like
                            || (sdSrc.Type == VJSoundType.Combat
                                && sdOwner.Components.Get<BaseNPC>() != null
                                && !Alive(sdOwner)))
                            allowed = false;
                    }
                    // lua:3561-3574 — Execute investigation
                    if (allowed)
                    {
                        DoReadyAlert();
                        StopMoving();
                        SetLastPosition(sdSrc.Origin);
                        SCHEDULE_FACE("TASK_FACE_LASTPOSITION");
                        OnInvestigate?.Invoke(sdOwner);
                        PlaySoundSystem("Investigate");
                        TakingCoverT = curTime + 1;
                    }
                }
            }
        }
        // ═══ Combat Behavior (lua:3579-3819) ═══
        else
        {
            // lua:3581-3582: Get active weapon + enemy data
            var wep = GetActiveWeapon(); // returns WeaponEntity (null until DoChangeWeapon initializes)
            var wepComp = wep.IsValid() ? wep.Components.Get<IVJBaseWeapon>() : null;
            var eneData = Enemy;

            // ═══ Weapon Reloading (lua:2777-2827) ═══
            // Guard: Weapon_CanReload, no active attack, not melee, weapon state READY
            bool plyControlled = VJ_IsBeingControlled;
            if (Weapon_CanReload && AttackType == VJAttackType.None && !IsWeaponMelee(wep)
                && GetWeaponState() == VJWepState.Ready && wepComp != null)
            {
                bool shouldReload = false;
                if (!plyControlled)
                {
                    if (!eneValid)
                    {
                        // No enemy: reload when partial mag && idle long enough && not moving
                        shouldReload = wepComp.GetMaxClip1() > wepComp.GetClip1()
                            && (curTime - eneData.TimeSet) > VJUtility.Rand(3f, 8f)
                            && !IsMoving();
                    }
                    else
                    {
                        // Has enemy: reload when empty
                        shouldReload = wepComp.GetClip1() <= 0;
                    }
                }
                else
                {
                    // Player-controlled: R key + partial mag
                    // SKIP: lua:2778 — KeyDown(IN_RELOAD) — Phase 3 player controller input
                    shouldReload = false; // wepComp.GetMaxClip1() > wepComp.GetClip1()
                }

                if (shouldReload)
                {
                    // lua:2779-2783
                    WeaponAttackState = VJWepAttackState.None;
                    NextChaseTime = curTime + 2;
                    if (!plyControlled) SetWeaponState(VJWepState.Reloading);
                    if (eneValid) PlaySoundSystem("WeaponReload");
                    OnWeaponReload();

                    // lua:2784-2787 — No animation: instant reload
                    if (DisableWeaponReloadAnimation)
                    {
                        if (GetWeaponState() == VJWepState.Reloading)
                            SetWeaponState();
                        wepComp.SetClip1(wepComp.GetMaxClip1());
                        if (IsWeaponVJBase(wep)) wepComp.NPC_Reload();
                    }
                    // lua:2788-2825 — Use reload animation
                    else
                    {
                        // Controlled by player
                        if (plyControlled)
                        {
                            SetWeaponState(VJWepState.Reloading);
                            // SKIP: lua:2792 — playReloadAnimation(self, TranslateActivity(PICK(AnimTbl_WeaponReload))) — Phase 3 animation
                        }
                        else
                        {
                            // NPC hidden → crouch reload
                            // lua:2796 — if eneValid && self:DoCoverTrace(myPos + OBBCenter(), ene:EyePos(), false, {SetLastHiddenTime = true}) then
                            if (eneValid && DoCoverTrace(WorldSpaceCenter(), ene.WorldPosition + Vector3.Up * 64f, false, true).isCover)
                            {
                                // SKIP: lua:2797-2799 — crouch reload animation (AnimTbl_WeaponReloadCovered) — Phase 3 animation
                            }
                            else
                            {
                                // Standing reload (no cover needed) or fallback
                                // lua:2804 — guards against cover-finding
                                if (!Weapon_FindCoverOnReload || IsGuard || IsFollowing
                                    || VJ_IsBeingControlled_Tool || !eneValid
                                    || MovementType == VJMoveType.Stationary
                                    || eneData.Distance < 650)
                                {
                                    // SKIP: lua:2805 — playReloadAnimation(self, TranslateActivity(PICK(AnimTbl_WeaponReload))) — Phase 3 animation
                                    PlayReloadAnimation(null);
                                }
                                // lua:2806-2822 — SCHEDULE_COVER_RELOAD: find cover position, run, reload
                                else
                                {
                                    var moveCheck = VJUtility.PICK(VJUtility.TraceDirections(this, "Quick", 200f, true, 8, true, false, false, false));
                                    if (moveCheck != default)
                                    {
                                        SCHEDULE_GOTO_POSITION("TASK_RUN_PATH", schedule =>
                                        {
                                            schedule.EngTask(EngineTask.FaceEnemy, 0);
                                            schedule.RunCodeOnFinish = () => PlayReloadAnimation(null);
                                        });
                                        SetLastPosition(moveCheck);
                                    }
                                    else
                                    {
                                        PlayReloadAnimation(null);
                                    }
                                }
                            }
                        }
                    }
                }
            }

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
                    MaintainAlertBehavior(false);                           // lua:3599
                    return;                                                 // lua:3600
                }
                // lua:3602: Fallback — idle, then fall through to goto_conditions
                MaintainIdleBehavior(2);
                // lua:3603: //return — Allow other behaviors (COND_PLAYER_PUSHING) to run
            }
            // ═══ C2: Has valid weapon (lua:3604-3816) ═══
            else
            {

                // lua:3605
                Weapon_UnarmedBehavior_Active = false;

                // lua:3607-3609: Position calculations
                // lua:3607 — approximate eye position (Phase 3: EyePosition() via skeletal head bone)
                var enePos_Eye = ene.WorldPosition + Vector3.Up * 64f;
                var myPos = WorldPosition;                                  // lua:3608
                // SKIP: lua:3609 — myPosCentered = myPos + OBBCenter() — Phase 3 OBB/GetBonePosition
                var myPosCentered = myPos;

                // ═══ C2a: Retreat from too-close enemy (lua:3611-3620) ═══
                // lua:3612: if eneData.Distance <= Weapon_RetreatDistance && !wep.IsMeleeWeapon
                //          && curTime > TakingCoverT && curTime > NextChaseTime && !AttackType
                //          && !IsFollowing && ene.Behavior != VJ_BEHAVIOR_PASSIVE
                //          && !DoCoverTrace(myPosCentered, enePos_Eye)
                if (eneData.Distance <= Weapon_RetreatDistance
                    && !IsWeaponMelee(wep)
                    && curTime > TakingCoverT
                    && curTime > NextChaseTime
                    && AttackType == VJAttackType.None
                    && !IsFollowing
                    && eneData.Target?.Components.Get<BaseNPC>()?.Behavior != VJBehavior.Passive
                    && !DoCoverTrace(myPosCentered, enePos_Eye).isCover
                    )
                {
                    // lua:3613 — moveCheck = PICK(VJ.TraceDirections(self, "Quick", 200, true, false, 8, true))
                    var moveCheck = VJUtility.PICK(VJUtility.TraceDirections(this, "Quick", 200f, true, 8, true, false, false, false));
                    if (moveCheck != default)
                    {
                        // lua:3615 — SetLastPosition(moveCheck)
                        SetLastPosition(moveCheck);
                        // lua:3616 — GetWeaponState() == VJ.WEP_STATE_RELOADING → SetWeaponState()
                        if (GetWeaponState() == VJWepState.Reloading)
                            SetWeaponState();
                        // lua:3617 — TakingCoverT = curTime + 2
                        TakingCoverT = curTime + 2f;
                        // lua:3618 — SCHEDULE_GOTO_POSITION("TASK_RUN_PATH", lambda with FACE_ENEMY + CanShootWhenMoving)
                        SCHEDULE_GOTO_POSITION("TASK_RUN_PATH", sched =>
                        {
                            sched.EngTask(EngineTask.FaceEnemy, 0);
                            sched.CanShootWhenMoving = true;
                            sched.TurnData = new TurnData { Type = VJFaceStatus.Enemy };
                        });
                        // lua:3619 — goto goto_conditions
                        goto goto_conditions;
                    }
                }

                // ═══ C2b: CanFireWeapon checks + occlusion (lua:3623-3651) ═══
                // lua:3623: if CanFireWeapon(false, false) && !VJ_STATE_ONLY_ANIMATION_NOATTACK then
                if (CanFireWeapon(false, false) && GetState() != VJState.OnlyAnimationNoAttack)
                {
                    // lua:3625: if eneData.Distance > Weapon_MaxDistance or curTime < NextWeaponAttackT then
                    if (eneData.Distance > Weapon_MaxDistance || curTime < NextWeaponAttackT)
                    {
                        // lua:3626 — MaintainAlertBehavior()
                        MaintainAlertBehavior(false);
                        AllowWeaponOcclusionDelay = false;
                    }
                    // lua:3629: elseif CanFireWeapon(true, true) then
                    else if (CanFireWeapon(true, true))
                    {
                        // lua:3631: if DoCoverTrace(EyePos, enePos_Eye, true) then — can't see enemy (enemy behind cover)
                        var (cantSeeEnemy, _) = DoCoverTrace(EyePosition(), enePos_Eye, true);
                        if (cantSeeEnemy)
                        {
                            // lua:3632 — if TakingCoverT > curTime then return (already covering)
                            if (TakingCoverT > curTime) return;
                            // lua:3633-3634 — reload state check
                            if (GetWeaponState() != VJWepState.Reloading)
                            {
                                // lua:3635-3638 — Weapon occlusion delay
                                if (Weapon_OcclusionDelay && WeaponAttackState != VJWepAttackState.AimOcclusion
                                    && !IsWeaponMelee(wep) && AllowWeaponOcclusionDelay
                                    && (curTime - WeaponLastShotTime) <= 4.5f
                                    && eneData.Distance > Weapon_OcclusionDelayMinDist)
                                {
                                    // lua:3636 — WeaponAttackState = AIM_OCCLUSION
                                    WeaponAttackState = VJWepAttackState.AimOcclusion;
                                    // lua:3637 — MaintainIdleBehavior(2) (Phase 3: ACT_IDLE_ANGRY)
                                    MaintainIdleBehavior(2);
                                    // lua:3638 — NextChaseTime delay
                                    NextChaseTime = curTime + VJUtility.Rand(Weapon_OcclusionDelayTime.a, Weapon_OcclusionDelayTime.b);
                                }
                                // lua:3640-3641 — Hidden zone stand-up
                                else if (curTime < LastHiddenZoneT
                                    && !DoCoverTrace(myPosCentered + Vector3.Up * 30f, enePos_Eye + Vector3.Up * 30f, true).isCover)
                                {
                                    // lua:3641 — Hidden zone stand-up (Phase 3: ACT_IDLE_ANGRY animation)
                                    MaintainIdleBehavior(2);
                                    // lua:3642 — Jump to weapon combat loop
                                    goto goto_checkwep;
                                }
                                // lua:3643-3649 — Else: Failed everything → fall back to chase
                                else
                                {
                                    // lua:3645-3646 — Reset weapon state if actively firing and not already chasing
                                    if (WeaponAttackState >= VJWepAttackState.Fire
                                        && CurrentScheduleName != "SCHEDULE_ALERT_CHASE"
                                        && CurrentScheduleName != "SCHEDULE_ALERT_CHASE_LOS")
                                    {
                                        WeaponAttackState = VJWepAttackState.None;
                                    }
                                    // lua:3648 — MaintainAlertBehavior()
                                    MaintainAlertBehavior(false);
                                }
                            }
                            // lua:3651 — goto goto_conditions
                            return;
                        }
                        // lua:3653: -- I can see the enemy...
                        // ═══ C2c: Enemy visible — weapon combat loop (lua:3654-3816) ═══
                        // lua:3654: ::goto_checkwep::
                    goto_checkwep:
                        _ = 0;

                        // lua:3655: if wep.IsVJBaseWeapon then — VJ Base weapons
                        if (IsWeaponVJBase(wep))
                        {
                            // ═══ C2c-i: Aim turning — FInAimCone logic (lua:3656-3670) ═══
                            // lua:3657 — if !HasPoseParameterLooking then (pose param disabled → always face)
                            if (!HasPoseParameterLooking)
                            {
                                // lua:3658 — self:SetTurnTarget("Enemy")
                                SetTurnTarget("Enemy");
                            }
                            else
                            {
                                // lua:3660 — wepDif = Weapon_AimTurnDiff or Weapon_AimTurnDiff_Def
                                var wepDif = Weapon_AimTurnDiff ?? Weapon_AimTurnDiff_Def;
                                // lua:3661-3662 — los = ene:GetPos() - myPos; los.z = 0
                                var los = ene.WorldPosition - myPos;
                                los = los.WithZ(0);
                                // lua:3663-3664 — facingDir = self:GetAngles():Forward(); facingDir.z = 0
                                // NOTE: GetAngles() ≠ sight dir (eyes). Lua explicitly warns against using sight dir here.
                                var facingDir = WorldRotation.Forward;
                                facingDir = facingDir.WithZ(0);
                                // lua:3665 — coneCalc = facingDir:Dot(los:GetNormalized())
                                var coneCalc = Vector3.Dot(facingDir, los.Normal);
                                // lua:3666 — if coneCalc < wepDif then
                                if (coneCalc < wepDif)
                                {
                                    // lua:3667 — self:SetTurnTarget("Enemy")
                                    SetTurnTarget("Enemy");
                                    // lua:3668 — self:UpdatePoseParamTracking(true)
                                    UpdatePoseParamTracking(true);
                                }
                            }
                            // lua:3671: // self:MaintainAlertBehavior() — commented out in Lua

                            // ═══ C2c-ii: Cover/obstruction check (lua:3673-3734) ═══
                            // lua:3673 — inCover, inCoverTrace = self:DoCoverTrace(myPosCentered, enePos_Eye, false, {SetLastHiddenTime = true})
                            var (inCover, inCoverTrace) = DoCoverTrace(myPosCentered, enePos_Eye, false, true);
                            // lua:3675 — wepInCover, wepInCoverTrace = self:DoCoverTrace(wep:GetBulletPos(), enePos_Eye, false)
                            // Phase 3: wep:GetBulletPos() approximated with WorldPosition + Up*60
                            var bulletPos = wep.WorldPosition + Vector3.Up * 60f;
                            var (wepInCover, wepInCoverTrace) = DoCoverTrace(bulletPos, enePos_Eye, false);
                            // lua:3679 — inCoverEnt = inCoverTrace.Entity / wepInCoverEnt = wepInCoverTrace.Entity
                            var inCoverEnt = inCoverTrace.GameObject;
                            var wepInCoverEnt = wepInCoverTrace.GameObject;
                            // lua:3679-3682 — inCoverEntLiving / wepInCoverEntLiving
                            var inCoverEntLiving = inCoverEnt.IsValid() && HasEntityFlag(inCoverEnt, "VJ_ID_Living");
                            var wepInCoverEntLiving = wepInCoverEnt.IsValid() && HasEntityFlag(wepInCoverEnt, "VJ_ID_Living");

                            // lua:3683: if !wep.IsMeleeWeapon then — ranged weapons only
                            if (!IsWeaponMelee(wep))
                            {
                                // lua:3685 — Friendly in line of fire → move!
                                if (inCoverEntLiving
                                    && WeaponAttackState == VJWepAttackState.FireStand
                                    && curTime > TakingCoverT
                                    && wepInCoverEnt.IsValid()
                                    && wepInCoverEnt.Components.Get<BaseNPC>() != null
                                    && wepInCoverEnt != GameObject
                                    && (Disposition(wepInCoverEnt) == (int)VJBase.Disposition.Like
                                        || Disposition(wepInCoverEnt) == (int)VJBase.Disposition.Neutral)
                                    // lua:3685 — wepInCoverTrace.HitPos:Distance(StartPos) <= 3000 → entity distance
                                    && wepInCoverEnt.WorldPosition.Distance(bulletPos) <= 3000f
                                    )
                                {
                                    // lua:3686 — moveCheck = PICK(VJ.TraceDirections(self, "Quick", 50, true, false, 4, true, true))
                                    // lua:3686 — excludeForward=true, excludeBack=true → only strafe sideways
                                    var directions = VJUtility.TraceDirections(this, "Quick", 50f, true, 4, true, true);
                                    var moveCheck = VJUtility.PICK(directions);
                                    // lua:3687 — if moveCheck then
                                    if (moveCheck != default)
                                    {
                                        // lua:3688 — self:StopMoving()
                                        StopMoving();
                                        // lua:3689 — if IsGuard then GuardData.Position = moveCheck; GuardData.Direction = moveCheck + GetForward()*51
                                        if (IsGuard)
                                        {
                                            Guard.Position = moveCheck;
                                            Guard.Direction = moveCheck + WorldRotation.Forward * 51f;
                                        }
                                        // lua:3690 — self:SetLastPosition(moveCheck)
                                        SetLastPosition(moveCheck);
                                        // lua:3691 — NextChaseTime = curTime + 1
                                        NextChaseTime = curTime + 1;
                                        // lua:3692 — SCHEDULE_GOTO_POSITION("TASK_WALK_PATH", lambda FACE_ENEMY + CanShootWhenMoving)
                                        SCHEDULE_GOTO_POSITION("TASK_WALK_PATH", schedule =>
                                        {
                                            schedule.EngTask(EngineTask.FaceEnemy, 0);
                                            schedule.CanShootWhenMoving = true;
                                            schedule.TurnData = new TurnData { Type = VJFaceStatus.Enemy };
                                        });
                                    }
                                }

                                // lua:3697: if inCover then
                                if (inCover)
                                {
                                    // lua:3699 — if curTime < TakingCoverT then goto goto_conditions
                                    if (curTime < TakingCoverT) goto goto_conditions;
                                    // lua:3701: (coverDist > 150 && !inCoverEntLiving) || (wepInCover && !wepInCoverEntLiving)
                                    var coverDist = inCoverTrace.HitPosition.Distance(myPosCentered);
                                    if (curTime > NextMoveOnGunCoveredT
                                        && ((coverDist > 150f && !inCoverEntLiving) || (wepInCover && !wepInCoverEntLiving)))
                                    {
                                        // lua:3703-3710 — nearestPos/nearestEntPos — Phase 3 utility (skip NearestPoint positioning)
                                        // lua:3716-3727 — SCHEDULE_GOTO_POSITION + animation — skip Phase 3 animation
                                        // SKIP: lua:3716-3727 — TranslateActivity(PICK(AnimTbl_MoveToCover)) + AnimExists + SetMovementActivity + StartSchedule — Phase 3 animation
                                        // lua:3730 — NextMoveOnGunCoveredT = curTime + 2
                                        NextMoveOnGunCoveredT = curTime + 2f;
                                        goto goto_conditions;
                                    }
                                }
                            }

                            // ═══ C2c-iii: Weapon attack (lua:3737-3794) ═══
                            // lua:3737: if curTime > NextWeaponAttackT && curTime > NextWeaponAttackT_Base then
                            if (curTime > NextWeaponAttackT && curTime > NextWeaponAttackT_Base)
                            {
                                // lua:3739-3751: Melee weapons
                                if (IsWeaponMelee(wep))
                                {
                                    // lua:3740 — self:OnWeaponAttack()
                                    OnWeaponAttack();
                                    // lua:3741 — finalAnim = TranslateActivity(PICK(AnimTbl_WeaponAttack))
                                    // SKIP: lua:3741-3743 — TranslateActivity/PICK(AnimTbl_WeaponAttack) + AnimExists + AnimDuration — Phase 3 animation
                                    // lua:3742 — if curTime > NextMeleeWeaponAttackT && VJ.AnimExists(self, finalAnim) then
                                    // NOTE: AnimExists guard missing — Phase 3 animation system needed
                                    // Without animations: always enters (conservative — melee NPCs still functional)
                                    if (curTime > NextMeleeWeaponAttackT)
                                    {
                                        // lua:3744 — wep.NPC_NextPrimaryFire = animDur (dynamic, based on AnimDuration) → Phase 3
                                        // lua:3745 — wep:NPCShoot_Primary()
                                        wepComp.NPCShoot_Primary();
                                        // lua:3746 — VJ.EmitSound(self, wep.NPC_BeforeFireSound, ...)
                                        EmitWeaponSound(wepComp);
                                        // lua:3747 — NextMeleeWeaponAttackT = curTime + animDur → Phase 3
                                        NextMeleeWeaponAttackT = curTime + 0.5f; // animDur fallback
                                        // lua:3748 — WeaponAttackAnim = finalAnim → Phase 3
                                        // SKIP: lua:3749 — PlayAnim(finalAnim, "LetAttacks", false, true) — Phase 3 animation
                                        // lua:3750 — WeaponAttackState = FIRE_STAND
                                        WeaponAttackState = VJWepAttackState.FireStand;
                                    }
                                }
                                // lua:3753-3793: Ranged weapons
                                else
                                {
                                    AllowWeaponOcclusionDelay = true;
                                    // lua:3755 — hasAmmo = wep:Clip1() > 0
                                    bool hasAmmo = wepComp.GetClip1() > 0;
                                    // lua:3756-3757 — !hasAmmo && WeaponAttackState != AIM → WeaponAttackAnim = ACT_INVALID
                                    if (!hasAmmo && WeaponAttackState != VJWepAttackState.Aim)
                                    {
                                        WeaponAttackAnim = null; // ACT_INVALID sentinel
                                    }
                                    // Re-entry guard: replaces Lua animation guards (IsCurrentAnim/GetActivity)
                                    // Once state is FireStand/Aim, auto-fire loop handles it — don't re-init every frame
                                    if (WeaponAttackState != VJWepAttackState.FireStand
                                        && WeaponAttackState != VJWepAttackState.Aim)
                                    {
                                        // lua:3764 — OnWeaponAttack()
                                        OnWeaponAttack();
                                        // lua:3765-3766 — WeaponAttackState == AIM_OCCLUSION → NONE
                                        if (WeaponAttackState == VJWepAttackState.AimOcclusion)
                                            WeaponAttackState = VJWepAttackState.None;
                                        // lua:3768 — WeaponLastShotTime = curTime
                                        WeaponLastShotTime = curTime;
                                        // lua:3771-3775 — !hasAmmo → MaintainIdleBehavior(2) + AIM state
                                        if (!hasAmmo)
                                        {
                                            MaintainIdleBehavior(2); // Phase 3: ACT_IDLE_ANGRY animation
                                            WeaponAttackState = VJWepAttackState.Aim;
                                        }
                                        else
                                        {
                                            // SKIP: lua:3778-3786 — TranslateActivity/PICK/AnimExists/crouch condition + PlayAnim — Phase 3 animation
                                            // lua:3787 — EmitSound(BeforeFireSound)
                                            EmitWeaponSound(wepComp);
                                        }
                                        // lua:3791 — NextWeaponAttackT_Base = curTime + 0.2
                                        NextWeaponAttackT_Base = curTime + 0.2f;
                                        // Ranged: weapon auto-fires via its own NPC_Think after attack state is set
                                        if (hasAmmo)
                                        {
                                            WeaponAttackState = VJWepAttackState.FireStand;
                                        }
                                    }
                                }
                            }  // C2c-iii

                            // ═══ C2c-iv: Random strafing while shooting (lua:3797-3806) ═══
                            // lua:3797 — guard: Weapon_Strafe, not in cover, not guard/following, ranged weapon,
                            //          NPC_StandingOnly=false, WeaponAttackState==FIRE_STAND,
                            //          strafe timer expired, enemy acquired > 2s, within max range
                            if (Weapon_Strafe
                                && !inCover
                                && !IsGuard
                                && !IsFollowing
                                && !IsWeaponMelee(wep)
                                && (wepComp == null || !wepComp.NPC_StandingOnly)
                                && WeaponAttackState == VJWepAttackState.FireStand
                                && curTime > NextWeaponStrafeT
                                && (curTime - eneData.TimeAcquired) > 2f
                                && eneData.Distance < (Weapon_MaxDistance / 1.25f))
                            {
                                // lua:3798 — OnWeaponStrafe() != false (virtual callback, default return true)
                                if (OnWeaponStrafe() != false)
                                {
                                    // lua:3799 — moveCheck = PICK(VJ.TraceDirections(self, "Radial", rand(150,400), true, false, 12, true))
                                    var strafeDist = Game.Random.Float(150f, 400f);
                                    var directions = VJUtility.TraceDirections(this, "Radial", strafeDist, true, 12, true, false, false, false);
                                    var moveCheck = VJUtility.PICK(directions);
                                    if (moveCheck != default)
                                    {
                                        // lua:3801-3803 — StopMoving / SetLastPosition / SCHEDULE_GOTO_POSITION
                                        StopMoving();
                                        SetLastPosition(moveCheck);
                                        var taskType = Game.Random.Next(1, 3) == 1 ? "TASK_RUN_PATH" : "TASK_WALK_PATH";
                                        SCHEDULE_GOTO_POSITION(taskType, schedule =>
                                        {
                                            schedule.EngTask(EngineTask.FaceEnemy, 0);
                                            schedule.CanShootWhenMoving = true;
                                            schedule.TurnData = new TurnData { Type = VJFaceStatus.Enemy };
                                        });
                                    }
                                }
                                // lua:3806 — NextWeaponStrafeT = curTime + math.Rand(Weapon_StrafeCooldown.a, Weapon_StrafeCooldown.b)
                                NextWeaponStrafeT = curTime + VJUtility.Rand(Weapon_StrafeCooldown.a, Weapon_StrafeCooldown.b);
                            }
                        }
                        // lua:3808: else — None VJ Base weapons
                        else
                        {
                            // ═══ C2c-v: Non-VJ weapons (lua:3808-3816) ═══
                            // lua:3809 — SetTurnTarget("Enemy")
                            SetTurnTarget("Enemy");
                            // lua:3810 — WeaponAttackState = FIRE_STAND
                            WeaponAttackState = VJWepAttackState.FireStand;
                            // lua:3811 — OnWeaponAttack()
                            OnWeaponAttack();
                            // lua:3812 — WeaponLastShotTime = curTime
                            WeaponLastShotTime = curTime;
                            // lua:3814 — SCHED_RANGE_ATTACK1 (Source engine schedule, no S&Box equivalent)
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
        object landDir = "FindBest";

        // lua:3083-3097: Determine landing direction
        if (ene.IsValid())
        {
            if (eneData.Visible)
            {
                landDir = "Enemy";
            }
            else
            {
                // lua:3089 — attempt to flush enemy from hiding
                bool canFlush = VisibleVec(eneData.VisiblePos)
                    && ene.WorldPosition.Distance(eneData.VisiblePos) <= GrenadeAttackMaxDistance;
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
        if (OnGrenadeAttack("Init", customEnt) is true) return false;
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
            // lua:3120 — SetTurnTarget(eneData.VisiblePos, AttackAnimDuration or 1.5)
            SetTurnTarget(Enemy.VisiblePos, AttackAnimDuration > 0 ? AttackAnimDuration : 1.5f);
        }
        else
        {
            // lua:3122-3126 — bestPos = PICK(VJ.TraceDirections(self, "Quick", 200, true, false, 8))
            var trs = VJUtility.TraceDirections(this, "Quick", 200f, requireFullDist: true, numDirections: 8);
            if (trs != null && trs.Count > 0)
            {
                var bestPosStr = VJUtility.PICK(trs);
                landDir = bestPosStr;  // lua:3124 — save the position so ExecuteGrenadeAttack can use it directly
                SetTurnTarget(bestPosStr, AttackAnimDuration > 0 ? AttackAnimDuration : 1.5f);  // lua:3125
            }
        }

        // lua:3130-3164: Handle live entity positioning
        if (isLiveEnt)
        {
            // lua:3131 — customEnt.VJ_ST_Grabbed = true
            var flags = customEnt.Components.GetOrCreate<VJEntityFlags>();
            flags.VJ_ST_Grabbed = true;

            // lua:3132 — customEnt.VJ_ST_GrabOrgMoveType = customEnt:GetMoveType()
            // Phase 3: store original Rigidbody.Enabled / NavMeshAgent.Enabled state for restore

            // lua:3138 — customPos = self:OnGrenadeAttack("SpawnPos", customEnt, landDir)
            var customPos = OnGrenadeAttack("SpawnPos", customEnt, landDir) as Vector3?;
            if (!customPos.HasValue)
            {
                // lua:3140-3152 — attachment / bone / fallback hierarchy
                // SKIP: lua:3140-3152 — LookupAttachment/GetAttachment/LookupBone/GetBonePosition/FollowBone — Phase 3 animation bone system
                customEnt.Parent = GameObject;                              // lua:3151 — SetParent(self) (fallback)
                customEnt.WorldPosition = WorldPosition + WorldRotation.Forward * 30; // lua:3149 — GetShootPos() fallback
            }
            else
            {
                customEnt.WorldPosition = customPos.Value;                  // lua:3161 — SetPos(customPos)
                customEnt.WorldRotation = customPos.Value.Normal.EulerAngles; // lua:3162 — SetAngles(customPos:Angle())
                customEnt.Parent = GameObject;                              // lua:3163 — SetParent(self)
            }
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
        // Priority: 1.Custom callback → 2.Attachment → 3.Bone → 4.Fallback(GetShootPos)
        var spawnPos = OnGrenadeAttack("SpawnPos", customEnt, landDir) as Vector3?;  // lua:3215
        Angles spawnAng = Angles.Zero;
        if (!spawnPos.HasValue)
        {
            // lua:3218-3223 — LookupAttachment / LookupBone → Phase 3 animation bone system
            // SKIP: lua:3218-3223 — LookupAttachment/GetAttachment/LookupBone/GetBonePosition — Phase 3 animation
            spawnPos = WorldPosition + WorldRotation.Forward * 30;                   // lua:3224 — GetShootPos() fallback
        }
        spawnAng = ((Vector3)spawnPos - WorldPosition).Normal.EulerAngles;           // lua:3233 — spawnPos:Angle()

        // lua:3238-3254: Determine landing position
        var landingPos = WorldPosition + WorldRotation.Forward * 200;
        if (landDir is string ld)
        {
            if (ld == "Enemy")                                              // lua:3239-3241
            {
                landingPos = GetEnemyLastKnownPos();
            }
            else if (ld == "EnemyLastVis")                                  // lua:3242-3245
            {
                if (Enemy.Visible) landingPos = Enemy.VisiblePos;
            }
            else                                                            // lua:3248-3253 — "FindBest" or unrecognized
            {
                var trs = VJUtility.TraceDirections(this, "Quick", 200f, requireFullDist: true, numDirections: 8);
                if (trs != null && trs.Count > 0)
                    landingPos = VJUtility.PICK(trs);
            }
        }
        else if (landDir is Vector3 vec)                                    // lua:3246-3247 — isvector(landDir)
        {
            landingPos = vec;
        }

        // lua:3258-3274: Create or reuse grenade entity
        if (isLiveEnt)
        {
            // lua:3259-3263: clean up parent/move type/follow bone
            // lua:3259 — customEnt.VJ_ST_Grabbed = false
            var flags = customEnt.Components.Get<VJEntityFlags>();
            if (flags != null) flags.VJ_ST_Grabbed = false;
            // lua:3261 — customEnt:SetParent(NULL)
            customEnt.Parent = null;
            // SKIP: lua:3262 — SetMoveType(VJ_ST_GrabOrgMoveType) restore — Phase 3 move type tracking
            // SKIP: lua:3263 — RemoveEffects(EF_FOLLOWBONE) — Phase 3 effects system
            grenade = customEnt;
        }
        else
        {
            // lua:3267-3273: ents.Create(customEnt or PICK(GrenadeAttackEntity)) + SetModel
            // lua:3268 — if !customEnt then SetModel (skip model override for custom entity classes)
            string grenadeClass;
            string grenadeModel = null;
            if (customEnt is string customClass)
            {
                grenadeClass = customClass;
            }
            else
            {
                grenadeClass = VJUtility.PICK(GrenadeAttackEntity);
                grenadeModel = VJUtility.PICK(GrenadeAttackModel);  // lua:3269-3271
            }
            grenade = VJEntitySpawner.CreateModelEntity(grenadeModel, spawnPos.Value, spawnAng, withPhysics: true);
            grenade.Name = $"VJ_Grenade_{grenadeClass}";
        }

        // lua:3276-3278: SetOwner + position
        if (!disableOwner)
        {
            // lua:3276 — grenade:SetOwner(self) → S&Box: tag with owner reference
            grenade.Tags.Add("owner_vj_npc");
            grenade.WorldPosition = spawnPos.Value;
            grenade.WorldRotation = spawnAng;
        }
        grenade.WorldPosition = spawnPos.Value;
        grenade.WorldRotation = spawnAng;

        // lua:3280-3308: Fuse timer dispatch
        if (isLiveEnt)
        {
            // lua:3282-3284 — if GetClass()=="npc_grenade_frag" && detonateTime<0 → Input("SetTimer")
            // Phase 3: component-based live grenade activation (check for VJGrenade component + IsActive)
            SetupGrenadeFuse(grenade, fuseTime);
        }
        else
        {
            // lua:3286-3303 — GetClass-based dispatch of fuse times/explode delay per grenade type
            // S&Box: class check replaced with virtual method; grenade components override SetupGrenadeFuse
            SetupGrenadeFuse(grenade, fuseTime);

            // lua:3305-3307: OnGrenadeAttackExecute("PreSpawn") + Spawn + Activate
            OnGrenadeAttackExecute("PreSpawn", grenade, customEnt, landDir, landingPos);
            // grenade:Spawn() / grenade:Activate() — S&Box GameObjects are live on creation, no spawn/activate needed
        }

        // lua:3311-3322: Throw velocity
        {
            var postSpawnResult = OnGrenadeAttackExecute("PostSpawn", grenade, customEnt, landDir, landingPos);  // lua:3312
            if (postSpawnResult is true)
            {
                // Callback handled velocity — skip default throw
            }
            else
            {
                // lua:3313 — postSpawnResult or (default velocity calc)
                var vel = (postSpawnResult as Vector3?) ?? ((landingPos - grenade.WorldPosition) + (Vector3.Up * 200 + WorldRotation.Forward * 500 + WorldRotation.Right * Game.Random.Next(-20, 21)));
                var rb = grenade.Components.Get<Rigidbody>();
                if (rb != null)
                {
                    rb.Sleeping = false;                                // lua:3316 — phys:Wake()
                    rb.AngularVelocity += new Vector3(500, 500, 500);  // lua:3317 — AddAngleVelocity(math.Rand(500,500), ...) → spin tumble
                    rb.Velocity = vel;                                 // lua:3318 — SetVelocity(vel)
                }
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
        if (checkAllies)
        {
            // lua:3848 — getAllies = self:Allies_Check(1000)
            var getAllies = Allies_Check(1000);
            // lua:3849-3859 — inherit ally's enemy if visible and hostile
            if (getAllies != null)
            {
                foreach (var ally in getAllies)
                {
                    var allyBase = ally.Components.Get<BaseNPC>();
                    if (allyBase == null) continue;
                    var allyEne = allyBase.GetEnemy();
                    if (!allyEne.IsValid()) continue;
                    if ((curTime - allyBase.Enemy.VisibleTime) >= EnemyTimeout) continue;
                    var allyEneBase = allyEne.Components.Get<BaseNPC>();
                    if (allyEneBase != null && allyEneBase.Dead) continue;
                    if (WorldPosition.Distance(allyEne.WorldPosition) > SightDistance) continue;
                    if (CheckRelationship(allyEne) != (int)VJBase.Disposition.Hate) continue;

                    AllowWeaponOcclusionDelay = false;
                    ForceSetEnemy(allyEne, false);
                    eneData.VisibleTime = curTime;
                    eneData.Reset = false;
                    return;
                }
            }
        }

        // ---- Block 2: VisibleCount / reachable enemies guard (lua:3862-3874) ----
        if (checkVis)
        {
            // lua:3864 — curEnemies = VisibleCount // CurrentReachableEnemies
            int curEnemies = CurrentReachableEnemies > 0 ? Enemy.VisibleCount / CurrentReachableEnemies : 0;
            if ((eneValid && (curEnemies - 1) >= 1) || (!eneValid && curEnemies >= 1))
            {
                MaintainRelationships();
                if (Enemy.VisibleCount > 0) { Enemy.Reset = false; return; }
            }
        }

        // ---- Block 3: Debug print (lua:3876) ----
        // lua:3876 — if selfData.VJ_DEBUG && GetConVar("vj_npc_debug_resetenemy"):GetInt() == 1 then VJ.DEBUG_Print(self, "ResetEnemy", tostring(ene)) end
        // SKIP: lua:3876 — VJ_DEBUG + convar + VJ.DEBUG_Print — Phase 3 debug system

        // ---- Block 4: Reset state + alert timeout timer (lua:3877-3879) ----
        // lua:3877 — eneData.Reset = true
        eneData.Reset = true;
        // lua:3878 — self:SetNPCState(NPC_STATE_ALERT)
        SetNPCState((int)NPCState.Alert);
        // lua:3879 — timer.Create alert_reset → NextAlertResetT polling (polled in Think)
        NextAlertResetT = curTime + VJUtility.Rand(AlertTimeout.a, AlertTimeout.b);

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
            bool canMoveToEnemy = !IsFollowing
                && !IsGuard
                && Components.Get<TankNPC>() == null
                && !VJ_IsBeingControlled
                && LastHiddenZone_CanWander == true
                && !Weapon_UnarmedBehavior_Active
                && Behavior != VJBehavior.Passive
                && Behavior != VJBehavior.PassiveNature
                && !IsBusy()
                && !Visible(ene);
            if (canMoveToEnemy)
            {
                // lua:3883b-3884 — GetEnemyLastKnownPos() != defPos → moveToEnemy
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
        NextWanderTime = curTime + VJUtility.Rand(3, 5);
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
    /// Damage callback. Returns 0 to block damage, 1 to allow.
    /// hitgroup: LastDamageHitGroup (pre-extracted, matches Lua:3932).
    /// </summary>
    public override int OnTakeDamage(DamageInfo dmgInfo, int hitgroup)
    {
        // ---- Block A: Entry guards (lua:3918-3923) ----
        // lua:3919-3920 — dmgAttacker = dmginfo:GetAttacker() / IsValid guard
        var dmgAttacker = dmgInfo.Attacker;
        // lua:3923 — Don't take bullet damage from friendly NPCs
        if (dmgAttacker.IsValid()
            && IsBulletDamage(dmgInfo)
            && dmgAttacker.Components.Get<BaseNPC>() is { } attackerBase
            && attackerBase.Disposition(GameObject) != (int)VJBase.Disposition.Hate
            && (VJ_NPC_Class.Any(c => attackerBase.VJ_NPC_Class.Contains(c)) || Disposition(dmgAttacker) == (int)VJBase.Disposition.Like))
            return 0;

        // ---- Block B: Inflictor + ragdoll guard (lua:3925-3929) ----
        // lua:3925-3926 — dmgInflictor = dmginfo:GetInflictor() → S&Box: Weapon
        var dmgInflictor = dmgInfo.Weapon;
        // lua:3929 — ragdoll damage avoidance (walking over corpses)
        if (dmgInflictor.IsValid())
        {
            // lua:3929 — ragdoll damage avoidance: GetClass()=="prop_ragdoll" && Velocity <= 100
            var rb = dmgInflictor.Components.Get<Rigidbody>();
            bool isRagdoll = dmgInflictor.Components.Get<ModelPhysics>() != null;
            if (isRagdoll && rb != null && rb.Velocity.Length <= 100)
                return 0; // lua:3929 — slow ragdoll (walking over corpse) → no damage
            // Low-velocity non-NPC prop guard (C# expansion: any slow prop, not just ragdoll)
            bool isLowVelocityProp = rb != null && rb.Velocity.Length <= 100
                && dmgInflictor.Components.Get<BaseNPC>() == null;
            if (isLowVelocityProp)
                return 0;
        }

        // ---- Block C: Init + Guard (lua:3931-3934) ----
        // lua:3931 — selfData = funcGetTable(self) → in C#: this
        // lua:3932 — hitgroup = self:GetLastDamageHitGroup() → passed as parameter
        // lua:3933
        OnDamaged(dmgInfo, hitgroup, "Init");
        // lua:3934 — GodMode or zero/negative damage → block
        if (GodMode || dmgInfo.Damage <= 0) return 0;

        // ---- Block D: Fire entity detection (lua:3936-3942) ----
        // lua:3936 — dmgType = dmginfo:GetDamageType()
        // SKIP: lua:3936 — dmginfo:GetDamageType() — Source engine CTakeDamageInfo API
        // lua:3937 — curTime = CurTime()
        float curTime = Time.Now;
        // lua:3938 — local isFireEnt = false
        bool isFireEnt = false;
        // lua:3939 — if self:IsOnFire() then
        if (IsOnFire())
        {
            // lua:3940 — isFireEnt = entityflame class check → S&Box: tag check
            var dmgInflictor = dmgInfo.Weapon;
            isFireEnt = dmgInflictor.IsValid() && dmgInflictor.Tags.Has("entityflame")
                && dmgAttacker.IsValid() && dmgAttacker.Tags.Has("entityflame");
            // lua:3941 — if self:WaterLevel() > 1 then self:Extinguish() end
            if (WaterLevel() > 1) Extinguish();
        }

        // ---- Block E: Boss bypass (lua:3944-3947) ----
        // lua:3945-3946 — if dmgAttacker && ForceDamageFromBosses && dmgAttacker.VJ_ID_Boss then goto skip_immunity
        if (ForceDamageFromBosses && dmgAttacker.IsValid() && BaseNPC.HasEntityFlag(dmgAttacker, "VJ_ID_Boss"))
            goto skip_immunity;

        // ---- Block F: Immunity chain (lua:3949-3951) ----
        // lua:3950 — fire entity with ignition disabled → extinguish and block
        if (isFireEnt && !AllowIgnition) { Extinguish(); return 0; }
        // lua:3951 — Full immunity OR-chain (8 types)
        if ((Immune_Fire && (IsFireDamage(dmgInfo) || isFireEnt))
            || (Immune_Toxic && IsToxicDamage(dmgInfo))
            || (Immune_Bullet && IsBulletDamage(dmgInfo))
            || (Immune_Explosive && IsExplosiveDamage(dmgInfo))
            || (Immune_Dissolve && IsDissolveDamage(dmgInfo))
            || (Immune_Electricity && IsElectricDamage(dmgInfo))
            || (Immune_Melee && IsMeleeDamage(dmgInfo))
            || (Immune_Sonic && IsSonicDamage(dmgInfo)))
            return 0;

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
                OnBleed(dmgInfo, hitgroup);
                // lua:3970 — if selfData.HasBloodParticle && !isFireEnt then self:SpawnBloodParticles(dmginfo, hitgroup) end
                if (HasBloodParticle && !isFireEnt) SpawnBloodParticles(dmgInfo, hitgroup);
                // lua:3971 — if selfData.HasBloodDecal then self:SpawnBloodDecals(dmginfo, hitgroup) end
                if (HasBloodDecal) SpawnBloodDecals(dmgInfo, hitgroup);
                // lua:3972 — self:PlaySoundSystem("Impact")
                PlaySoundSystem("Impact");
            }
        }

        // ---- Block I: Dead guard (lua:3975) ----
        // lua:3975 — if selfData.Dead then DoBleed() return 0 end
        if (Dead) { DoBleed(); return 0; }

        // ---- Block J: PreDamage + damage application (lua:3977-4000) ----
        // lua:3977 — self:OnDamaged(dmginfo, hitgroup, "PreDamage")
        OnDamaged(dmgInfo, hitgroup, "PreDamage");
        // lua:3978 — only take damage if above 0
        if (dmgInfo.Damage <= 0) return 0;
        // lua:3980-3990 — SavedDmgInfo snapshot (GMod resets dmginfo after tick). S&Box: DamageInfo persistent, no snapshot needed.
        // lua:3991 — self:SetHealth(self:Health() - dmginfo:GetDamage())
        CurrentHealth -= dmgInfo.Damage;
        // lua:3992 — VJ_DEBUG damage print
        // SKIP: lua:3992 — VJ_DEBUG && vj_npc_debug_damage:GetInt()==1 → VJ.DEBUG_Print — Phase 3 debug
        // lua:3993-3995 — HealthRegenParams: if Enabled && ResetOnDmg then delay next regen
        if (HealthRegenEnabled && HealthRegenResetOnDmg)
            HealthRegenDelayT = curTime + HealthRegenDelay;
        // lua:3997-3998 — self:SetSaveValue("m_iDamageCount", ...) / self:SetSaveValue("m_flLastDamageTime", curTime)
        // SKIP: lua:3997-3998 — SetSaveValue (Source engine save/restore) — Phase 3 persistence
        // lua:3999 — self:OnDamaged(dmginfo, hitgroup, "PostDamage")
        OnDamaged(dmgInfo, hitgroup, "PostDamage");
        // lua:4000 — DoBleed()
        DoBleed();

        // ---- Block K: I/O events (lua:4002-4008) ----
        // lua:4003-4007 — if dmgAttacker then self:TriggerOutput("OnDamaged", dmgAttacker);
        //     self:MarkTookDamageFromEnemy(dmgAttacker) else self:TriggerOutput("OnDamaged", self) end
        if (dmgAttacker.IsValid())
        {
            TriggerOutput("OnDamaged", dmgAttacker);
            MarkTookDamageFromEnemy(dmgAttacker);
        }
        else
        {
            TriggerOutput("OnDamaged", GameObject);
        }

        // ---- Block L: Pain sounds (lua:4010-4011) ----
        // lua:4010 — stillAlive = self:Health() > 0
        // lua:4011 — if stillAlive then self:PlaySoundSystem("Pain") end
        if (CurrentHealth > 0)
            PlaySoundSystem("Pain");

        // ---- Block M: AI response (lua:4013-4151) ----
        // lua:4013 — if VJ_CVAR_AI_ENABLED && self:GetState() != VJ_STATE_FREEZE then
        // SKIP: lua:4013 — VJ_CVAR_AI_ENABLED convar check — Phase 3 convar system (assume enabled)
        if (GetState() != VJState.Freeze)
        {
            // lua:4014 — isPassive = selfData.Behavior == VJ_BEHAVIOR_PASSIVE or selfData.Behavior == VJ_BEHAVIOR_PASSIVE_NATURE
            var isPassive = Behavior == VJBehavior.Passive || Behavior == VJBehavior.PassiveNature;

            // lua:4015 — if stillAlive then
            if (CurrentHealth > 0)
            {
                // ---- M1: Flinch (lua:4016-4017) ----
                // lua:4016 — if !isFireEnt then
                // SKIP: lua:4016 — !isFireEnt guard — Phase 3 fire system
                // lua:4017 — self:Flinch(dmginfo, hitgroup)
                Flinch(dmgInfo, hitgroup);

                // ---- M2: Player attacker → BecomeEnemyToPlayer (lua:4020-4052) ----
                // lua:4021 — if dmgAttacker && dmgAttacker:IsPlayer() then (no VJ_CVAR_IGNOREPLAYERS guard — NPC attacked by player always reacts)
                bool isPlayerAttacker = dmgAttacker.IsValid() && dmgAttacker.Components.Get<PlayerBase>() != null;
                if (isPlayerAttacker)
                {
                    // lua:4023 — if self.BecomeEnemyToPlayer && self:CheckRelationship(dmgAttacker) == D_LI then
                    if (BecomeEnemyToPlayer > 0 && CheckRelationship(dmgAttacker) == (int)VJBase.Disposition.Like)
                    {
                        // lua:4024 — self:SetRelationshipMemory(dmgAttacker, VJ.MEM_HOSTILITY_LEVEL, 1)
                        SetRelationshipMemory(dmgAttacker, "hostility", 1f);
                        var hostility = GetRelationshipMemory(dmgAttacker, "hostility");
                        // lua:4025-4026 — if relationMemory[VJ.MEM_HOSTILITY_LEVEL] > self.BecomeEnemyToPlayer && self:Disposition(dmgAttacker) != D_HT then
                        if (hostility > BecomeEnemyToPlayer && Disposition(dmgAttacker) != (int)VJBase.Disposition.Hate)
                        {
                            // lua:4027 — self:OnBecomeEnemyToPlayer(dmginfo, hitgroup)
                            OnBecomeEnemyToPlayer(dmgInfo, hitgroup);
                            // lua:4028 — if self.IsFollowing && self.FollowData.Target == dmgAttacker then self:ResetFollowBehavior() end
                            // SKIP: lua:4028 — IsFollowing / FollowData / ResetFollowBehavior — Phase 3 follow system
                            // lua:4029 — self:SetRelationshipMemory(dmgAttacker, VJ.MEM_OVERRIDE_DISPOSITION, D_HT)
                            SetRelationshipMemory(dmgAttacker, "override_disposition", (int)VJBase.Disposition.Hate);
                            // lua:4030 — self:AddEntityRelationship(dmgAttacker, D_HT, 2)
                            AddEntityRelationship(dmgAttacker, (int)VJBase.Disposition.Hate, 2);
                            // lua:4031 — self.TakingCoverT = curTime + 2
                            TakingCoverT = curTime + 2f;
                            // lua:4032 — self:PlaySoundSystem("BecomeEnemyToPlayer")
                            PlaySoundSystem("BecomeEnemyToPlayer");
                            // lua:4033 — if !IsValid(funcGetEnemy(self)) then self:StopMoving() self:SetTarget(dmgAttacker) self:SCHEDULE_FACE("TASK_FACE_TARGET") end
                            if (!GetEnemy().IsValid())
                            {
                                StopMoving();
                                SetTarget(dmgAttacker);
                                SCHEDULE_FACE("TASK_FACE_TARGET");
                            }
                            // SKIP: lua:4036-4041 — CanChatMessage / PrintMessage — Phase 3 chat system
                        }
                    }
                    // lua:4044-4051 — DamageByPlayer sounds
                    // NOTE: Lua checks NextDamageByPlayerSoundT but never sets it (variable stays 0 → always true).
                    //       This is a Lua oversight — translating 1:1 without adding cooldown.
                    if (HasDamageByPlayerSounds && curTime > NextDamageByPlayerSoundT && Visible(dmgAttacker))
                    {
                        var dispLvl = DamageByPlayerDispositionLevel;
                        var disp = Disposition(dmgAttacker);
                        if (dispLvl == 0 || (dispLvl == 1 && disp == (int)VJBase.Disposition.Like) || (dispLvl == 2 && disp != (int)VJBase.Disposition.Hate))
                        {
                            PlaySoundSystem("DamageByPlayer");
                        }
                    }
                }

                // ---- M2.5: Pain sound inside AI block (lua:4054) ----
                // lua:4054 — self:PlaySoundSystem("Pain")
                PlaySoundSystem("Pain");

                // ---- M3: Combat damage response — take cover from visible enemy (lua:4056-4076) [HUMAN ONLY] ----
                var eneData = Enemy;
                if (!isPassive && CombatDamageResponse && GetEnemy().IsValid()
                    && curTime > NextCombatDamageResponseT && !IsFollowing
                    && AttackType == VJAttackType.None && !IsBusy() && curTime > TakingCoverT
                    && eneData.Visible && GetWeaponState() != VJWepState.Reloading
                    && eneData.Distance < Weapon_MaxDistance)
                {
                    var wep = GetActiveWeapon();
                    bool canMove = true;
                    if (DoCoverTrace(WorldSpaceCenter(), GetAimPosition(GetEnemy())).isCover)
                    {
                        // SKIP: lua:4062-4069 — AnimTbl_TakingCover + ACT_INVALID guard + hideTime + NextChaseTime — Phase 3 animation
                    }
                    // lua:4072-4075 — if canMove && !IsMoving && (!wep or !wep.IsMeleeWeapon) then SCHEDULE_COVER_ENEMY
                    if (canMove && !IsMoving()
                        && (!wep.IsValid() || !VJUtility.HasValue(WeaponInventory_MeleeList, wep.Name)))
                    {
                        SCHEDULE_COVER_ENEMY("TASK_RUN_PATH", schedule =>
                        {
                            schedule.EngineTask.FaceEnemy = true;
                        });
                        NextCombatDamageResponseT = curTime + VJUtility.Rand(CombatDamageResponse_Cooldown.a, CombatDamageResponse_Cooldown.b);
                    }
                }

                // ---- M4: No enemy response — ally alert + damage response (lua:4078-4128) ----
                // lua:4078 — if !isPassive && !IsValid(funcGetEnemy(self)) then
                if (!isPassive && !GetEnemy().IsValid())
                {
                    // lua:4082-4102 — DamageAllyResponse: alert nearby allies
                    if (DamageAllyResponse && curTime > NextDamageAllyResponseT && !IsFollowing)
                    {
                        // lua:4083 — OBBMaxs/OBBMins distance calc (OBB helpers on BaseNPC)
                        float responseDist = Math.Max(800, Vector3.DistanceBetween(OBBMaxs(), OBBMins()) * 12);
                        var allies = Allies_Check(responseDist);
                        if (allies != null)
                        {
                            // lua:4086 — bring allies in Diamond formation
                            if (!isFireEnt) Allies_Bring("Diamond", responseDist, allies, 4);
                            // lua:4089-4090 — alert each ally
                            foreach (var ally in allies)
                            {
                                ally.Components.Get<BaseNPC>()?.DoReadyAlert();
                            }
                            // lua:4092-4098 — alert self + play response animation
                            if (!isFireEnt && !IsBusy("Activities"))
                            {
                                DoReadyAlert();
                                // SKIP: lua:4094 — PlayAnim(AnimTbl_DamageAllyResponse) — Phase 3 animation
                                // lua:4096 — if anim valid: NextFlinchT = curTime + 1
                            }
                            // lua:4100 — cooldown
                            NextDamageAllyResponseT = curTime + VJUtility.Rand(DamageAllyResponse_Cooldown.a, DamageAllyResponse_Cooldown.b);
                        }
                    }
                    // lua:4104-4128 — DamageResponse: find attacker or take cover
                    var dmgResponse = DamageResponse;
                    if (dmgResponse is not false and not null && curTime > TakingCoverT && !IsBusy("Activities"))
                    {
                        bool canMove = true;

                        // lua:4107-4122 — Attempt to find who damaged me
                        if (dmgAttacker.IsValid() && HasEntityFlag(dmgAttacker, "VJ_ID_Living")
                            && (dmgResponse is true or "OnlySearch"))
                        {
                            var sightDist = SightDistance;
                            sightDist = MathF.Min(MathF.Max(sightDist / 2, sightDist <= 1000 ? sightDist : 1000), sightDist);
                            if (WorldPosition.Distance(dmgAttacker.WorldPosition) <= sightDist && Visible(dmgAttacker))
                            {
                                var dispLvl = Disposition(dmgAttacker);
                                if (dispLvl == (int)VJBase.Disposition.Hate || dispLvl == (int)VJBase.Disposition.Neutral)
                                {
                                    OnSetEnemyFromDamage(dmgInfo, hitgroup);
                                    NextCallForHelpT = curTime + 1;
                                    ForceSetEnemy(dmgAttacker, true);
                                    MaintainAlertBehavior();
                                    canMove = false;
                                }
                            }
                        }

                        // lua:4125-4128 — If all else failed then take cover
                        if (canMove && (dmgResponse is true or "OnlyMove") && !IsFollowing
                            && MovementType != VJMoveType.Stationary
                            && !dmgInfo.Tags.Has("bleed"))
                        {
                            SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH", schedule =>
                            {
                                schedule.CanShootWhenMoving = true;
                                schedule.TurnData = new TurnData { Type = VJFaceStatus.Enemy };
                            });
                            TakingCoverT = curTime + 5;
                        }
                    }
                }

                // ---- M5: Passive NPC run away (lua:4130-4134) ----
                // lua:4131-4134 — elseif isPassive && curTime > selfData.TakingCoverT then if selfData.DamageResponse && !self:IsBusy() then self:SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH") end end
                // lua: selfData.DamageResponse is truthy — accepts true, "OnlySearch", "OnlyMove", etc.
                if (isPassive && curTime > TakingCoverT && DamageResponse is not false and not null && !IsBusy("Activities"))
                    SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH");
            }

            // ---- M6: Passive allies signal danger (lua:4138-4151) ----
            // lua:4139 — if isPassive && curTime > selfData.TakingCoverT then
            if (isPassive && curTime > TakingCoverT)
            {
                // lua:4140 — if selfData.Passive_AlliesRunOnDamage then
                if (Passive_AlliesRunOnDamage)
                {
                    // lua:4140 — OBBMaxs/OBBMins * 20 (OBB helpers on BaseNPC)
                    var allies = Allies_Check(Math.Max(800, Vector3.DistanceBetween(OBBMaxs(), OBBMins()) * 20));
                    if (allies != null)
                    {
                        foreach (var ally in allies)
                        {
                            var allyBase = ally.Components.Get<BaseNPC>();
                            if (allyBase == null) continue;
                            allyBase.TakingCoverT = curTime + VJUtility.Rand(6, 7);
                            allyBase.SCHEDULE_COVER_ORIGIN("TASK_RUN_PATH");
                            allyBase.PlaySoundSystem("Alert");
                        }
                    }
                }
                // lua:4150 — selfData.TakingCoverT = curTime + math.Rand(6, 7)
                TakingCoverT = curTime + VJUtility.Rand(6, 7);
            }
        }

        // ---- Block N: Stop eating (lua:4154-4158) ----
        // lua:4155-4157 — if selfData.CanEat && selfData.VJ_ST_Eating then selfData.EatingData.NextCheck = curTime + 15; self:ResetEatingBehavior("Injured") end
        // SKIP: lua:4155-4157 — CanEat && VJ_ST_Eating → ResetEatingBehavior("Injured") — Phase 3 eating system

        // ---- Block O: Death (lua:4160-4171) ----
        // lua:4160 — if self:Health() <= 0 && !selfData.Dead then
        if (CurrentHealth <= 0 && !Dead)
        {
            // lua:4161 — self:RemoveEFlags(EFL_NO_DISSOLVE) → clear the flag
            EFL_NO_DISSOLVE = false;

            // lua:4162-4168 — DMG_DISSOLVE or prop_combine_ball inflictor → re-tag as dissolve death
            // Source: creates new DamageInfo with DMG_DISSOLVE + TakeDamageInfo (recursive).
            // S&Box: replace dmgInfo so CreateDeathCorpse sees DMG_DISSOLVE tag, avoiding recursion.
            if ( dmgInfo.Tags.Has( VJDamageTags.Dissolve ) ||
                 (dmgInfo.Weapon.IsValid() && dmgInfo.Weapon.Tags.Has( "prop_combine_ball" )) )
            {
                var dissolveDmg = new DamageInfo();
                dissolveDmg.Damage = dmgInfo.Damage;
                dissolveDmg.Attacker = dmgInfo.Attacker;
                dissolveDmg.Position = dmgInfo.Position;
                dissolveDmg.Weapon = dmgInfo.Weapon;
                dissolveDmg.Tags.Add( VJDamageTags.Dissolve );
                dmgInfo = dissolveDmg;
            }

            // lua:4169 — self:BeginDeath(dmginfo, hitgroup)
            BeginDeath( dmgInfo, hitgroup );
        }

        // lua:4171 — return 1
        return 1;
    }

    // ═══ BeginDeath — human_base/init.lua:4177-4298 ═══
    /// <summary>
    /// Human override of BeginDeath. Full override (no base call) matching Lua pattern.
    /// Adds human-specific weapon cleanup; omits creature-only HasRangeAttack/HasLeapAttack reset.
    /// </summary>
    public override void BeginDeath(DamageInfo dmginfo, int hitgroup)
    {
        // lua:4178 — self.Dead = true
        Dead = true;
        // lua:4179 — self.DoNotDuplicate = true
        DoNotDuplicate = true;
        // lua:4180 — self:SetSaveValue("m_lifeState", 1) — LIFE_DYING
        SetSaveValue("m_lifeState", 1);
        // lua:4181 — self:OnDeath(dmginfo, hitgroup, "Init")
        OnDeath(dmginfo, hitgroup, "Init");

        // lua:4182 — if self.MedicData.Status then self:ResetMedicBehavior() end
        if (Medic.Status != "false" && Medic.Status != null)
            ResetMedicBehavior();
        // lua:4183 — if self.IsFollowing then self:ResetFollowBehavior() end
        if (IsFollowing) ResetFollowBehavior();

        // lua:4184-4185 — dmgInflictor/dmgAttacker
        // SKIP: lua:4184 — dmginfo:GetInflictor() — use dmginfo.Weapon (S&Box equivalent)
        // lua:4186 — myPos = self:GetPos()
        Vector3 myPos = WorldPosition;

        // ---- Ally death response (lua:4188-4238) ----
        {
            // lua:4189 — responseDist = math_max(800, self:OBBMaxs():Distance(self:OBBMins()) * 12)
            float responseDist = Math.Max(800, Vector3.DistanceBetween(OBBMaxs(), OBBMins()) * 12);
            // lua:4190 — allies = self:Allies_Check(responseDist)
            var allies = Allies_Check(responseDist);
            if (allies != null)
            {
                // lua:4192 — doBecomeEnemyToPlayer = (self.BecomeEnemyToPlayer && dmgAttacker:IsPlayer() && !VJ_CVAR_IGNOREPLAYERS)
                var doBecomeEnemyToPlayer = BecomeEnemyToPlayer > 0
                    && dmgAttacker.IsValid()
                    && dmgAttacker.Components.Get<PlayerBase>() != null
                    && !VJInit.vj_npc_ignoreplayers;
                var responseType = DeathAllyResponse;
                foreach (var ally in allies)
                {
                    var allyBase = ally.Components.Get<BaseNPC>();
                    if (allyBase == null) continue;
                    // lua:4196-4197 — OnAllyKilled callback + PlaySoundSystem("AllyDeath")
                    allyBase.OnAllyKilled(GameObject);
                    allyBase.PlaySoundSystem("AllyDeath");
                    // lua:4202-4206 — bring ally
                    if (responseType != "OnlyAlert")
                        Allies_Bring("Diamond", responseDist, new List<GameObject> { ally }, 4);
                    // lua:4209-4215 — alert ally
                    allyBase.DoReadyAlert();
                    allyBase.SetTurnTarget("Enemy");
                    // lua:4220-4235 — BecomeEnemyToPlayer chain
                    if (doBecomeEnemyToPlayer && allyBase.BecomeEnemyToPlayer > 0
                        && allyBase.CheckRelationship(dmgAttacker) == (int)VJBase.Disposition.Like)
                    {
                        allyBase.SetRelationshipMemory(dmgAttacker, "hostility", 1f);
                        var hostility = allyBase.GetRelationshipMemory(dmgAttacker, "hostility");
                        if (hostility > allyBase.BecomeEnemyToPlayer)
                        {
                            // lua:4223 — if ally:Disposition(dmgAttacker) != D_HT then
                            if (allyBase.Disposition(dmgAttacker) != (int)VJBase.Disposition.Hate)
                            {
                                allyBase.OnBecomeEnemyToPlayer(dmginfo, hitgroup);
                                allyBase.SetRelationshipMemory(dmgAttacker, "override_disposition", (int)VJBase.Disposition.Hate);
                                allyBase.AddEntityRelationship(dmgAttacker, (int)VJBase.Disposition.Hate, 2);
                                allyBase.PlaySoundSystem("BecomeEnemyToPlayer");
                                // SKIP: lua:4225 — ResetFollowBehavior — Phase 3 follow system
                                // SKIP: lua:4232 — CanChatMessage — Phase 3 chat system
                            }
                            // lua:4235 — ally.Alerted = true
                            allyBase.Alerted = VJAlertState.Enemy;
                        }
                    }
                }
            }
        }

        // ---- Blood decal (lua:4241-4250) ----
        // lua:4242 — if self.Bleeds && self.HasBloodDecal then
        if (Bleeds && HasBloodDecal)
        {
            // lua:4243 — bloodDecal = PICK(self.BloodDecal)
            var bloodDecal = VJUtility.PICK(BloodDecal);
            // lua:4244 — if bloodDecal then
            if (bloodDecal != null)
            {
                // lua:4245-4248 — decalPos = myPos + vecZ4, TraceLine downward 500, util.Decal
                var decalPos = myPos + Vector3.Up * 4f;
                // lua:4245 — SetLocalPos(decalPos) Source decal positioning; S&Box uses world-space traces
                var tr = Game.ActiveScene.Trace.Ray(decalPos, decalPos + Vector3.Down * 500f)
                    .IgnoreGameObjectHierarchy(GameObject)
                    .Run();
                if (tr.Hit)
                    PlaceBloodDecal(bloodDecal, tr.HitPosition + tr.Normal, tr.HitPosition - tr.Normal);
            }
        }

        // ---- Cleanup (lua:4252-4255) ----
        // lua:4252 — self:RemoveTimers()
        RemoveTimers();
        // lua:4253 — self:StopAllSounds()
        StopAllSounds();
        // lua:4254 — self.AttackType = VJ.ATTACK_TYPE_NONE
        AttackType = VJAttackType.None;
        // lua:4255 — self.HasMeleeAttack = false
        HasMeleeAttack = false;
        // NOTE: human_base does NOT reset HasRangeAttack/HasLeapAttack (human attacks use weapons, not creature attacks)

        // ---- Attacker check (lua:4256-4259) ----
        // lua:4256 — if IsValid(dmgAttacker) then
        // SKIP: lua:4256-4259 — dmgAttacker:GetClass()=="npc_barnacle" / vj_npc_ply_frag / AddFrags — Phase 3 DamageInfo + Source engine

        // lua:4260 — gamemode.Call("OnNPCKilled", self, dmgAttacker, dmgInflictor)
        // SKIP: lua:4260 — gamemode.Call("OnNPCKilled") — S&Box has no gamemode.Call

        // ---- Post-death setup (lua:4261-4264) ----
        // lua:4261 — self:SetCollisionGroup(COLLISION_GROUP_DEBRIS)
        SetCollisionGroup(1); // COLLISION_GROUP_DEBRIS
        // lua:4262 — self:GibOnDeath(dmginfo, hitgroup)
        GibOnDeath(dmginfo, hitgroup);
        // lua:4263 — self:PlaySoundSystem("Death")
        PlaySoundSystem("Death");
        // lua:4264 — //AA_StopMoving() commented out

        // ---- I/O events (lua:4266-4272) ----
        // lua:4267-4270 — if dmgAttacker:IsValid() then TriggerOutput("OnDeath", dmgAttacker) else TriggerOutput("OnDeath", self)
        var deathAttacker = dmginfo.Attacker;
        if (deathAttacker.IsValid())
            TriggerOutput("OnDeath", deathAttacker);
        else
            TriggerOutput("OnDeath", GameObject);
        // lua:4271 — self:Fire("KilledNPC") — PX: Source I/O, use OnTriggerOutput("OnDeath") instead

        // ---- Death animation + delay → FinishDeath (lua:4274-4297) ----
        // lua:4275 — deathTime = self.DeathDelayTime
        float deathTime = DeathDelayTime;
        // lua:4276 — combine ball → covered by Dissolve tag check below
        // lua:4277 — HasDeathAnimation && !DMG_REMOVENORAGDOLL && !DMG_DISSOLVE && NavType!=CLIMB
        if (HasDeathAnimation
            && !dmginfo.Tags.Has(VJDamageTags.Dissolve)
            && !dmginfo.Tags.Has(VJDamageTags.RemoveNoRagdoll)
            && GetNavType() != (int)NavType.Climb
            && Game.Random.Next(1, Math.Max(1, DeathAnimationChance) + 1) == 1)
        {
            // lua:4278 — self:RemoveAllGestures()
            RemoveAllGestures();
            // lua:4279 — self:OnDeath(dmginfo, hitgroup, "DeathAnim")
            OnDeath(dmginfo, hitgroup, "DeathAnim");
            // lua:4280-4283 — PICK(AnimTbl_Death) / AnimDurationEx / PlayAnim
            // SKIP: lua:4280-4283 — PICK/AnimDurationEx/PlayAnim — Phase 3 animation
            // lua:4284 — self.DeathAnimationCodeRan = true
            DeathAnimationCodeRan = true;
        }
        // lua:4285 — else
        else
        {
            // lua:4287 — self:SetSaveValue("m_lifeState", 2) — LIFE_DEAD
            SetSaveValue("m_lifeState", 2);
        }

        // lua:4289-4297 — if deathTime > 0 then timer.Simple → FinishDeath else FinishDeath
        // S&Box: polling pattern → NextDeathFinishT (polled in Think)
        PendingDeathDmgInfo = dmginfo;
        PendingDeathHitgroup = hitgroup;
        if (deathTime > 0)
            NextDeathFinishT = Time.Now + deathTime;
        else
            FinishDeath(dmginfo, hitgroup);
    }

    // ═══ FinishDeath — human_base/init.lua:4300-4310 ═══
    /// <summary>
    /// Human override: adds DeathWeaponDrop before CreateDeathCorpse.
    /// </summary>
    public override void FinishDeath(DamageInfo dmginfo, int hitgroup)
    {
        // lua:4301 — VJ_DEBUG + GetConVar debug print
        // SKIP: lua:4301 — VJ_DEBUG / GetConVar — Phase 3 debug system

        // lua:4302 — self:SetSaveValue("m_lifeState", 2) — LIFE_DEAD
        SetSaveValue("m_lifeState", 2);
        // lua:4303 — //SetNPCState(NPC_STATE_DEAD) — commented out
        // lua:4304 — self:OnDeath(dmginfo, hitgroup, "Finish")
        OnDeath(dmginfo, hitgroup, "Finish");

        // lua:4305 — if self.DropDeathLoot then
        if (DropDeathLoot)
        {
            // lua:4306 — self:CreateDeathLoot(dmginfo, hitgroup)
            CreateDeathLoot(dmginfo, hitgroup);
        }

        // lua:4308 — if not DMG_REMOVENORAGDOLL then DeathWeaponDrop + CreateDeathCorpse
        // Note: Lua only checks DMG_REMOVENORAGDOLL (not DMG_DISSOLVE) for corpse creation
        if (!dmginfo.Tags.Has(VJDamageTags.RemoveNoRagdoll))
        {
            DeathWeaponDrop(dmginfo, hitgroup);
            CreateDeathCorpse(dmginfo, hitgroup);
        }

        // lua:4309 — self:Remove() — S&Box: GameObject.Destroy() handles cleanup, children auto-destroyed
    }

    // ═══ CreateDeathCorpse — human_base/init.lua:4314-4482 ═══
    /// <summary>
    /// Human override: adds WeaponEntity tracking in ChildEnts, dissolve weapon handling,
    /// and weapon cleanup in the no-corpse else path.
    /// </summary>
    public override GameObject CreateDeathCorpse(DamageInfo dmginfo, int hitgroup)
    {
        // ---- SavedDmgInfo guard (lua:4317-4329) ----
        // lua:4317 — if !self.SavedDmgInfo then
        if (SavedDmgInfo == null)
        {
            // lua:4318-4328 — SavedDmgInfo snapshot (GMod resets dmginfo after tick)
            SavedDmgInfo = new SavedDmgInfoData
            {
                dmginfo = dmginfo,
                attacker = dmginfo.Attacker,        // lua:4320 — GetAttacker()
                inflictor = dmginfo.Weapon,          // lua:4321 — GetInflictor() → S&Box Weapon
                amount = dmginfo.Damage,             // lua:4322 — GetDamage()
                pos = dmginfo.Position,              // lua:4323 — GetDamagePosition()
                // lua:4324 — GetDamageType() → S&Box no DMG_* bitmask; use Tags for type checks
                // lua:4325 — GetDamageForce() → S&Box no direct force on DamageInfo
                // lua:4326 — GetAmmoType() → S&Box no ammo type on DamageInfo
                hitgroup = hitgroup,
            };
        }

        // ---- Corpse gate (lua:4331) ----
        // lua:4331 — if self.HasDeathCorpse && self.HasDeathRagdoll != false then
        if (!HasDeathCorpse || HasDeathRagdoll == false)
        {
            // lua:4474 — if IsValid(self.WeaponEntity) then self.WeaponEntity:Remove() end
            if (WeaponEntity.IsValid())
                WeaponEntity.Destroy();
            // lua:4475-4480 — remove child ents (DeathCorpse_ChildEnts loop)
            // lua:4475-4480 — child ent removal handled by S&Box auto-cleanup on parent destroy
            return null;
        }

        // ---- Model selection (lua:4332-4335) ----
        // lua:4332 — corpseMdl = self:GetModel()
        var corpseMdl = VJEntitySpawner.GetModelPath(GameObject);
        // lua:4333-4334 — corpseMdlCustom = PICK(self.DeathCorpseModel) → override
        var corpseMdlCustom = VJUtility.PICK(DeathCorpseModel);
        if (!string.IsNullOrEmpty(corpseMdlCustom)) corpseMdl = corpseMdlCustom;

        // ---- Entity class selection (lua:4335-4345) ----
        // lua:4335 — corpseClass = "prop_physics"
        var corpseClass = "prop_physics";
        if (!string.IsNullOrEmpty(DeathCorpseEntityClass))
            corpseClass = DeathCorpseEntityClass;
        else
        {
            // lua:4339-4340 — util.IsValidRagdoll → "prop_ragdoll"
            Model loaded;
            if (VJEntitySpawner.TryLoadModel(corpseMdl, out loaded))
            {
                if (VJEntitySpawner.IsValidRagdollModel(loaded))
                    corpseClass = "prop_ragdoll";
            }
            // lua:4341-4343 — !util.IsValidProp || !util.IsValidModel → WeaponEntity:Remove() + return false
            else if (!string.IsNullOrEmpty(corpseMdl))
            {
                if (WeaponEntity.IsValid()) WeaponEntity.Destroy();
                return null;
            }
        }

        // ---- Entity creation (lua:4346-4364) ----
        // lua:4346-4364 — ents.Create(corpseClass)/SetModel/SetPos/SetAngles/Spawn/Activate
        GameObject corpse;
        if (corpseClass == "prop_ragdoll")
            corpse = VJEntitySpawner.CreateRagdollEntity(corpseMdl, WorldPosition, WorldRotation);
        else
            corpse = VJEntitySpawner.CreateModelEntity(corpseMdl, WorldPosition, WorldRotation, withPhysics: true);

        // ---- Copy appearance (lua:4365-4383) ----
        // lua:4365-4383 — SetSkin/bodygroup loop/SetColor/SetMaterial/submaterials
        VJEntitySpawner.CopyAppearance(GameObject, corpse);

        // ---- Corpse metadata (lua:4386-4391) ----
        // lua:4386 — FadeCorpseType assign
        // lua:4387 — IsVJBaseCorpse
        corpse.Tags.Add("vj_corpse");
        // lua:4388 — DamageInfo assign
        // lua:4389 — ChildEnts assign
        // lua:4390 — BloodData assign

        // ---- Blood pool (lua:4392-4394) ----
        // lua:4392-4394 — if self.Bleeds && self.HasBloodPool && vj_npc_blood_pool:GetInt()==1 then self:SpawnBloodPool(...)
        if (Bleeds && HasBloodPool)
        {
            // lua:4393 — vj_npc_blood_pool convar (S&Box: always enabled via Bleeds+HasBloodPool)
            SpawnBloodPool(dmginfo, hitgroup, corpse);
        }

        // ---- Collision (lua:4397-4404) ----
        var corpseNPC = corpse?.Components.Get<BaseNPC>();
        // lua:4397 — corpse:SetCollisionGroup(self.DeathCorpseCollisionType)
        corpseNPC?.SetCollisionGroup(DeathCorpseCollisionType);
        // lua:4400-4403 — VJ.Corpse_Add(corpse)
        VJUtility.Corpse_Add(corpse);

        // ---- On fire (lua:4407-4413) ----
        // lua:4407 — if self:IsOnFire() then
        if (IsOnFire())
        {
            // lua:4408 — corpse:Ignite(math.Rand(8, 10), 0)
            VJEntitySpawner.IgniteEntity(corpse);
            // lua:4409-4411 — if !self.Immune_Fire then corpse:SetColor(colorGrey)
            if (!Immune_Fire)
            {
                var corpseRenderer = corpse.Components.Get<ModelRenderer>();
                if (corpseRenderer != null) corpseRenderer.Tint = new Color(0.35f, 0.35f, 0.35f, 1f);
            }
        }

        // ---- Dissolve (lua:4416-4418) ----
        // lua:4416-4418 — DMG_DISSOLVE type or prop_combine_ball inflictor → corpse:Dissolve(0, 1)
        if (dmginfo.Tags.Has(VJDamageTags.Dissolve) || (dmginfo.Weapon.IsValid() && dmginfo.Weapon.Tags.Has("prop_combine_ball")))
        {
            VJEntitySpawner.DissolveEntity(corpse, 2f, 1f);
            FL_DISSOLVING = true;
        }

        // ---- Bone physics (lua:4422-4448) ----
        // lua:4422-4428 — useLocalVel + dmgForce = (SavedDmgInfo.force/40) + move vel + rb vel
        if (DeathCorpseApplyForce && SavedDmgInfo != null)
        {
            var selfRb = Components.Get<Rigidbody>();
            var selfVel = selfRb?.Velocity ?? Vector3.Zero;
            var dmgForce = SavedDmgInfo.force / 40f + selfVel;
            var corpseRb = corpse.Components.Get<Rigidbody>();
            if (corpseRb != null)
            {
                corpseRb.ApplyForce(dmgForce * 50f);
            }
            // lua:4429-4448 — per-bone physics loop (Source GetPhysicsObjectCount/GetSurfaceArea/TranslatePhysBoneToBone)
            // S&Box ModelPhysics differs; simplified to whole-body Rigidbody force. Per-bone deferred.
        }

        // ---- Health & stink (lua:4451-4456) ----
        // lua:4451-4455 — corpse:SetMaxHealth/SetHealth(totalSurface/60) — S&Box HealthComponent deferred
        // lua:4456 — VJ.Corpse_AddStinky(corpse, true)
        VJUtility.Corpse_AddStinky(corpse, true);

        // ---- WeaponEntity → ChildEnts (HUMAN ONLY: lua:4458) ----
        // lua:4458 — if IsValid(self.WeaponEntity) then corpse.ChildEnts[#corpse.ChildEnts+1] = self.WeaponEntity end
        if (WeaponEntity.IsValid())
        {
            WeaponEntity.SetParent(corpse);
            DeathCorpse_ChildEnts.Add(WeaponEntity);
        }

        // ---- Fade (lua:4459-4460) ----
        // lua:4459-4460 — DeathCorpseFade timer
        if (DeathCorpseFade > 0)
        {
            _ = Task.Delay((int)(DeathCorpseFade * 1000f)).ContinueWith(_ => { if (corpse.IsValid()) corpse.Destroy(); });
        }

        // lua:4461 — self:OnCreateDeathCorpse(dmginfo, hitgroup, corpse)
        OnCreateDeathCorpse(dmginfo, hitgroup, corpse);

        // ---- Dissolve weapon + children (HUMAN ONLY: lua:4462-4471) ----
        // lua:4462 — if corpse:IsFlagSet(FL_DISSOLVING) then
        if (FL_DISSOLVING)
        {
            // lua:4463-4465 — if IsValid(WeaponEntity) then WeaponEntity:Dissolve(0, 1)
            if (WeaponEntity.IsValid()) VJEntitySpawner.DissolveEntity(WeaponEntity, 2f, 1f);
            // lua:4466-4470 — for child in ChildEnts → child:Dissolve
            foreach (var child in DeathCorpse_ChildEnts)
                VJEntitySpawner.DissolveEntity(child, 2f, 1f);
        }

        // ---- CallOnRemove (lua:4472-4480) ----
        // lua:4472-4480 — corpse:CallOnRemove callback / child cleanup / hook.Call("CreateEntityRagdoll")
        // (S&Box: child entities auto-destroyed when parent is destroyed)

        // lua:4481 — return corpse
        return corpse;
    }

    // ═══ DeathWeaponDrop — human_base/init.lua:4484-4513 ═══
    /// <summary>
    /// Drop the active weapon on death. Human-only (no creature equivalent).
    /// </summary>
    public virtual void DeathWeaponDrop(DamageInfo dmginfo, int hitgroup)
    {
        // lua:4485 — activeWep = funcGetActiveWeapon(self)
        var activeWep = GetActiveWeapon();
        // lua:4486 — if !self.DropWeaponOnDeath or !IsValid(activeWep) then return end
        if (!DropWeaponOnDeath || !activeWep.IsValid()) return;

        // lua:4488-4490 — Save original pos & ang before DropWeapon changes them
        var orgPos = activeWep.WorldPosition;
        var orgAng = activeWep.WorldRotation;

        // lua:4491 — DropWeapon: un-parent from NPC
        activeWep.Parent = null;
        // lua:4492-4495 — if WorldModel_UseCustomPosition then SetPos(orgPos); SetAngles(orgAng)
        // S&Box: weapon model follows parent transform; un-parenting preserves world pos/ang automatically

        // lua:4496-4508 — Apply drop physics
        if (activeWep.Components.TryGet<Sandbox.Rigidbody>(out var rb))
        {
            rb.Enabled = true;
            // lua:4498 — DMG_DISSOLVE → anti-gravity launch
            if (dmginfo != null && dmginfo.Tags.Has(VJDamageTags.Dissolve))
            {
                rb.Gravity = false;
                rb.Velocity = WorldRotation.Forward * -150f + WorldRotation.Right * Game.Random.Float(-100f, 100f) + WorldRotation.Up * 50f;
            }
            else
            {
                // lua:4502-4505 — dmgForce = (SavedDmgInfo.force / 40) + moveVelocity
                var dmgForce = (dmginfo?.Force ?? 0) / 40f;
                // Phase 3: DeathAnimationCodeRan → use GroundSpeedVelocity
                rb.MassOverride = 1f;
                rb.ApplyForce(dmgForce * WorldRotation.Forward);
            }
        }
        // lua:4510 — self.WeaponEntity = activeWep
        WeaponEntity = activeWep;
        // lua:4512 — self:OnDeathWeaponDrop(dmginfo, hitgroup, activeWep)
        OnDeathWeaponDrop(dmginfo, hitgroup, activeWep);
    }

    // ═══ GetAttackSpread — human_base/init.lua:4515 ═══
    /// <summary>Weapon spread modifier. Lua returns nil (= 0 spread). Override in derived types.</summary>
    public virtual float GetAttackSpread(GameObject wep, GameObject target) => 0f;

    // ═══ ExecuteMeleeAttack (human override) — human_base/init.lua:2993-3058 ═══
    /// <summary>
    /// Human-specific melee attack override. Simpler than CreatureNPC version:
    /// adds GRENADE guard, removes prop interaction, bleed, player DSP/speed effects.
    /// </summary>
    public override void ExecuteMeleeAttack(bool isPropAttack)
    {
        // lua:2995 — guard: Dead / PauseAttacks / Flinching / ATTACK_TYPE_GRENADE / (StopOnHit && EXECUTED_HIT)
        if (Dead || PauseAttacks || Flinching
            || AttackType == VJAttackType.Grenade
            || (MeleeAttackStopOnHit && AttackState == VJAttackState.ExecutedHit)) return;

        // lua:2996 — OnMeleeAttackExecute("Init")
        var skip = OnMeleeAttackExecute("Init");
        var hitRegistered = false;

        if (!skip) // lua:2998
        {
            var myPos = WorldPosition;                              // lua:2999
            var traceOrigin = MeleeAttackTraceOrigin();             // lua:3001 — MeleeAttackTraceOrigin()
            var traceDir = MeleeAttackTraceDirection();             // for angle check (lua:3004)
            var hits = Scene.FindInPhysics(new Sphere(traceOrigin, MeleeAttackDamageDistance > 0 ? MeleeAttackDamageDistance : 80));

            foreach (var ent in hits) // lua:3001
            {
                // lua:3002 — skip self, same NPC type, bullseye controlled
                if (ent == GameObject) continue;
                var entBase = ent.Components.Get<BaseNPC>();
                if (entBase != null && entBase.VJ_NPC_Class.Any(c => VJ_NPC_Class.Contains(c))) continue;
                // SKIP: lua:3002 — ent.IsVJBaseBullseye && ent.VJ_IsBeingControlled — Phase 3 bullseye system

                // lua:3003 — player skip: VJ_IsControllingNPC / dead / ignoreplayers
                bool isPlayer = ent.Components.Get<PlayerBase>() != null;
                if (isPlayer)
                {
                    // SKIP: ent.VJ_IsControllingNPC — Source player field, no S&Box equivalent
                    if (!Alive(ent) || VJInit.vj_npc_ignoreplayers) continue;
                }
                else if (entBase?.VJ_IsBeingControlled == true) continue;

                // lua:3004 — ((Living && Disp!=D_LI) || Attackable || Destructible) && angle check
                bool isLiving = HasEntityFlag(ent, "VJ_ID_Living");
                if (isPlayer && !isLiving) isLiving = true;
                bool isAttackable = HasEntityFlag(ent, "VJ_ID_Attackable");
                bool isDestructible = HasEntityFlag(ent, "VJ_ID_Destructible");
                var delta = new Vector3(ent.WorldPosition.x - myPos.x, ent.WorldPosition.y - myPos.y, 0);
                bool inAngle = traceDir.Dot(delta.Normal) > MathF.Cos(MathF.PI / 180f * MeleeAttackDamageAngleRadius);

                if (((isLiving && Disposition(ent) != (int)VJBase.Disposition.Like) || isAttackable || isDestructible) && inAngle)
                {
                    // lua:3005 — isProp = ent.VJ_ID_Attackable
                    bool isProp = isAttackable;

                    // lua:3006 — OnMeleeAttackExecute("PreDamage")
                    if (OnMeleeAttackExecute("PreDamage", ent, isProp)) continue;

                    var dmgAmount = ScaleByDifficulty(MeleeAttackDamage); // lua:3007

                    // lua:3009-3021 — Knockback (skip doors, trains, stationary, Boss non-Tank)
                    if (HasMeleeAttackKnockBack
                        // lua:3009 — GetMoveType()!=MOVETYPE_PUSH → S&Box: skip kinematic Rigidbody (doors/elevators)
                        && (ent.Components.Get<Rigidbody>() is not { IsKinematic: true })
                        && entBase?.MovementType != VJMoveType.Stationary
                        && (!HasEntityFlag(ent, "VJ_ID_Boss") /* || ent.IsVJBaseSNPC_Tank — Phase 3 */))
                    {
                        // SKIP: lua:3010-3019 — IsNextBot / SetGroundEntity(NULL) / loco:Approach/Jump/SetVelocity — S&Box Rigidbody.Velocity accomplishes same knockback without Source NextBot
                        var vel = MeleeAttackKnockbackVelocity(ent); // lua:3014
                        var rb = ent.Components.Get<Rigidbody>();
                        if (rb != null)
                        {
                            rb.Velocity = vel;
                        }
                    }

                    // lua:3023-3032 — Apply damage
                    if (!DisableDefaultMeleeAttackDamageCode)
                    {
                        var dmgInfo = new DamageInfo();
                        dmgInfo.Damage = dmgAmount;                     // lua:3025 — SetDamage(dmgAmount) (already scaled)
                        dmgInfo.Tags.Add(MapDamageTypeToTag(MeleeAttackDamageType)); // lua:3026 — SetDamageType
                        // lua:3027 — SetDamageForce(forward * ((dmg+100)*70)) → S&Box Rigidbody.ApplyForce
                        if (BaseNPC.HasEntityFlag(ent, "VJ_ID_Living"))
                            ent.Components.Get<Rigidbody>()?.ApplyForce(WorldRotation.Forward * ((dmgInfo.Damage + 100) * 70));
                        // LIMITATION: S&Box DamageInfo has no Inflictor; Weapon=null means attacker-is-inflictor
                        dmgInfo.Attacker = GameObject;                  // lua:3029 — SetAttacker(self)
                        VJUtility.DamageSpecialEnts(GameObject, ent, dmgInfo); // lua:3030
                        foreach (var d in ent.Components.GetAll<IDamageable>()) // lua:3031 — TakeDamageInfo
                            d.OnDamage(dmgInfo);
                    }

                    // lua:3033-3035 — Player ViewPunch
                    // SKIP: lua:3033-3035 — ViewPunch(Angle(...)) — Phase 3 player camera

                    // lua:3036-3038 — non-prop → hitRegistered, stop on hit
                    if (!isProp)
                    {
                        hitRegistered = true;
                        if (MeleeAttackStopOnHit) break;                // lua:3037
                    }
                }
            }
        }

        // lua:3043-3044 — AttackState < EXECUTED → set EXECUTED
        // lua:3045-3046 — attackTimers[VJ.ATTACK_TYPE_MELEE](self) — C# handles via ProcessAttackTimers Think poll
        if (AttackState < VJAttackState.Executed)
            AttackState = VJAttackState.Executed;

        // lua:3049-3057 — Post-loop: hit → MeleeAttack sound, miss → OnMiss + MeleeAttackMiss sound
        if (!skip)
        {
            if (hitRegistered)
            {
                PlaySoundSystem("MeleeAttack");                          // lua:3051
                AttackState = VJAttackState.ExecutedHit;                 // lua:3052
            }
            else
            {
                OnMeleeAttackExecute("Miss");                            // lua:3054
                PlaySoundSystem("MeleeAttackMiss");                      // lua:3055
            }
        }
    }

    // ═══ EmitWeaponSound — helper for NPC_BeforeFireSound (weapon_vj_base/shared.lua:3746/3787) ═══
    /// <summary>
    /// Play the weapon's NPC_BeforeFireSound if configured. (Phase 3: use S&Box Sound.Play)
    /// </summary>
    protected virtual void EmitWeaponSound(IVJBaseWeapon wepComp)
    {
        if (wepComp == null) return;
        var soundName = wepComp.NPC_BeforeFireSound;
        if (string.IsNullOrEmpty(soundName)) return;
        Sound.Play(soundName, WorldPosition);
    }

    // ═══ playReloadAnimation (local func) — human_base/init.lua:2562-2582 ═══
    /// <summary>
    /// Play reload animation and schedule reload-complete timer.
    /// Source: init.lua local function, used by SelectSchedule reload logic.
    /// </summary>
    protected virtual bool PlayReloadAnimation(object anims)
    {
        // lua:2563 — anim, animDur, animType = self:PlayAnim(anims, true, false, "Visible")
        // SKIP: lua:2563 — PlayAnim(anims, true, false, "Visible") — Phase 3 animation
        // lua:2564 — if anim != ACT_INVALID then
        // SKIP: lua:2564 — ACT_INVALID comparison — Phase 3 animation
        {
            // lua:2565 — wep = self.WeaponEntity
            var wep = WeaponEntity;
            // lua:2566 — if wep.IsVJBaseWeapon then wep:NPC_Reload() end
            // Called BEFORE timer — NPC_Reload is a reload-start callback (sends OnReload("Start"),
            // pushes grenade timer, plays reload sound), not reload-complete. Timer handles completion.
            if (wep.IsValid())
            {
                var wepComp = wep.Components.Get<IVJBaseWeapon>();
                if (wepComp != null && wepComp.IsVJBaseWeapon)
                {
                    wepComp.NPC_Reload();
                }
            }
            // lua:2567-2573 — timer.Create("wep_reload_reset", animDur, ...) reload-complete
            // SKIP: animDur from PlayAnim — Phase 3 animation (use fallback 2.0s)
            float animDur = 2.0f; // Phase 3: get from PlayAnim return
            float reloadCompleteTime = Time.Now + animDur;
            // Schedule reload completion: SetClip1(MaxClip1) + OnReload("Finish") + SetWeaponState(READY)
            // Phase 3: use async timer instead of polling; for now, poll in CheckWeaponState
            NextReloadCompleteT = reloadCompleteTime;
            ReloadingWeapon = wep;
            // lua:2574 — self.AllowWeaponOcclusionDelay = false
            AllowWeaponOcclusionDelay = false;
            // lua:2576-2578 — if !VJ_IsBeingControlled && animType == ANIM_TYPE_GESTURE then StopMoving() end
            // SKIP: lua:2576 — animType == ANIM_TYPE_GESTURE — Phase 3 animation
            // lua:2579 — return true
            return true;
        }
        // lua:2581 — return false (animation invalid, reached if PlayAnim returned ACT_INVALID)
        // Phase 3: return false only when PlayAnim returns ACT_INVALID; currently always enters the block
    }

    // ═══ attackTimers (local table) — human_base/init.lua:2536-2560 ═══
    // Replaced by BaseNPC.ScheduleAttackTimers() + ProcessAttackTimers() (timer→polling).
    // MELEE timer → AttackResetTime/AttackReEnableTime in ScheduleAttackTimers()
    // GRENADE timer → AttackResetTime/GrenadeExecTime/NextThrowGrenadeT in ScheduleAttackTimers()
    // Both polled in ProcessAttackTimers() called from Think.
}
