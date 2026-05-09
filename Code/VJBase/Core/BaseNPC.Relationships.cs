using System;
using System.Collections.Generic;
using Sandbox;
using SWB.Player;

namespace VJBase;

/// <summary>
/// Relationship maintenance — core.lua ENT:MaintainRelationships (line ~2127-2426).
/// Iterates known entities, evaluates alliance/enemy/investigation, picks nearest enemy.
/// </summary>
public partial class BaseNPC
{
    // ═══ Entity type constants (core.lua local variables) ═══
    private const int ENT_TYPE_NPC = 1;
    private const int ENT_TYPE_PLAYER = 2;
    private const int ENT_TYPE_NEXTBOT = 3;
    private const int ENT_TYPE_OTHER = 4;
    private const int FL_NOTARGET = 1 << 16; // Source engine: entity ignored by AI targeting

    // ═══════════════════════════════════════════════
    // Perception helpers — Source engine equivalent
    // ═══════════════════════════════════════════════

    // funcIsInViewCone(self, pos) → dot product FOV check
    protected virtual bool IsInViewCone(Vector3 targetPos)
    {
        var myEye = EyePosition();
        var dir = (targetPos - myEye).Normal;
        var forward = GameObject.WorldRotation.Forward.Normal;
        float halfAngleRad = MathF.PI * SightAngle / 360f;
        return Vector3.Dot(forward, dir) >= MathF.Cos(halfAngleRad);
    }

    // funcVisible(self, ent) → can see through ray trace
    protected virtual bool CanSee(GameObject target)
    {
        if (target == null || !target.IsValid()) return false;
        var myEye = EyePosition();
        var targetEye = target.WorldPosition + Vector3.Up * 64f; // Phase 3: EyePosition() on target
        var tr = Game.ActiveScene.Trace.Ray(myEye, targetEye)
            .WithoutTags("npc")
            .IgnoreGameObjectHierarchy(GameObject)
            .Run();
        return !tr.Hit || tr.GameObject == target;
    }

    // OBBMaxs/OBBMins — Phase 3: model/collider bounds from renderer or collider component
    protected virtual Vector3 OBBMaxs() => new(32, 32, 72);
    protected virtual Vector3 OBBMins() => new(-32, -32, 0);

    // WorldSpaceCenter = WorldPosition + OBB center offset. Source: GetAbsOrigin() + (Mins+Maxs)/2
    public Vector3 WorldSpaceCenter()
    {
        var obbCenter = (OBBMins() + OBBMaxs()) * 0.5f;
        return GameObject.WorldPosition + obbCenter;
    }
    public Vector3 WorldSpaceCenter_Entity(GameObject ent)
    {
        if (ent == null || !ent.IsValid()) return Vector3.Zero;
        var baseNPC = ent.Components.Get<BaseNPC>();
        if (baseNPC != null)
        {
            var obbCenter = (baseNPC.OBBMins() + baseNPC.OBBMaxs()) * 0.5f;
            return ent.WorldPosition + obbCenter;
        }
        // Phase 3: read collider/model bounds for non-VJ entities
        return ent.WorldPosition;
    }

    // ═══════════════════════════════════════════════
    // ResetEnemy — core.lua ENT:ResetEnemy
    // ═══════════════════════════════════════════════

    protected virtual void ResetEnemy(bool lost, bool schedule)
    {
        // core.lua: core enemy reset — clear enemy + conditions + schedule cleanup
        SetEnemy(null);
        Enemy.Reset = true;
        ClearCondition(Condition.SeeEnemy);
        ClearCondition(Condition.HaveEnemyLOS);
        SetCondition(Condition.LostEnemy);
        // SKIP: full ResetEnemy — PlaySoundSystem, schedule handling, enemy memory cleanup Phase 3
    }

    // ═══════════════════════════════════════════════
    // MaintainRelationships — core.lua ENT:MaintainRelationships
    // ═══════════════════════════════════════════════

    /// <summary>
    /// Iterates all known entities (RelationshipEnts), handles alliance/enemy detection,
    /// picks the nearest valid enemy, checks investigation triggers, OnPlayerSight, player push.
    /// Returns: whether an enemy was found this tick.
    /// </summary>
    protected virtual bool MaintainRelationships()
    {
        // core.lua:2127-2128 — passive nature guard
        if (Behavior == VJBehavior.PassiveNature) return false;

        var entities = RelationshipEnts;
        if (entities == null || entities.Count == 0) return false;
        var memories = RelationshipMemory;

        // core.lua:2136-2140 — class change detection for alliance cache invalidation
        var myClasses = VJ_NPC_Class;
        var myClassesChanged = false;
        if (_cacheRelationshipClasses == null || !ListsEqual(_cacheRelationshipClasses, myClasses))
        {
            myClassesChanged = true;
            _cacheRelationshipClasses = myClasses != null ? new List<string>(myClasses) : null;
        }

        int eneVisCount = 0;
        var myPos = GameObject.WorldPosition;
        var mySightDist = SightDistance;
        var myCanAlly = CanAlly;
        var myFriPlyAllies = AlliedWithPlayerAllies;
        var notIsNeutral = Behavior != VJBehavior.Neutral;
        var customFunc = OnMaintainRelationships;
        GameObject nearestEnt = null;
        float? nearestDist = null;

        // core.lua:2145 — iterate RelationshipEnts
        int it = 0;
        while (it < entities.Count)
        {
            var ent = entities[it];

            // core.lua:2147-2150 — remove invalid entities
            if (!ent.IsValid())
            {
                entities.RemoveAt(it);
                memories.Remove(ent);
                continue;
            }
            it++;

            // core.lua:2151 — get or create entity memory
            if (!memories.TryGetValue(ent, out var entMemory))
                memories[ent] = entMemory = new Dictionary<string, object>();

            // core.lua:2167: ent:IsFlagSet(FL_NOTARGET) or !ent:Alive()
            // Phase 3: FL_NOTARGET flag check via HasEntityFlag stub (always false until flag system)
            if (HasEntityFlag(ent, FL_NOTARGET) || !Alive(ent))
            {
                if (GetEnemy() == ent)
                    ResetEnemy(true, false);
                AddEntityRelationship(ent, (int)VJBase.Disposition.Neutral, 0);
                continue;
            }

            // core.lua:2161-2171 — distance culling
            var entPos = ent.WorldPosition;
            float distanceToEnt = Vector3.DistanceBetween(myPos, entPos);
            if (distanceToEnt > mySightDist)
            {
                if (GetEnemy() == ent)
                {
                    PlaySoundSystem("LostEnemy");
                    ResetEnemy(true, false);
                }
                continue;
            }

            // core.lua:2172 — override disposition from memory
            int? calculatedDisp = null;
            if (entMemory.TryGetValue(VJMemoryKey.OverrideDisposition, out var ovr) && ovr is int ovrInt && ovrInt != 0)
                calculatedDisp = ovrInt;

            // core.lua:2191-2204 — entity type caching: ent:IsNPC() / ent:IsPlayer() / ent:IsNextBot()
            // Component-based detection: matches Lua type semantics, more reliable than tag matching
            if (!entMemory.TryGetValue(VJMemoryKey.CacheEntType, out _))
            {
                int entType;
                if (ent.Components.Get<BaseNPC>() != null)
                    entType = ENT_TYPE_NPC;
                else if (ent.Components.Get<PlayerBase>() != null)
                    entType = ENT_TYPE_PLAYER;
                else
                    entType = ENT_TYPE_OTHER;
                // SKIP: ent:IsNextBot() → ENT_TYPE_NEXTBOT — Phase 3
                entMemory[VJMemoryKey.CacheEntType] = entType;
            }
            int entTypeVal = entMemory.TryGetValue(VJMemoryKey.CacheEntType, out var et) && et is int eti ? eti : ENT_TYPE_OTHER;

            // ═══════════════════════════════════════════
            // core.lua:2191-2261 — Alliance system
            // ═══════════════════════════════════════════
            if (myCanAlly && !calculatedDisp.HasValue)
            {
                var entClasses = ent.Components.Get<BaseNPC>()?.VJ_NPC_Class;
                var entCachedClasses = entMemory.TryGetValue(VJMemoryKey.CacheClasses, out var ecc) ? ecc as List<string> : null;

                if (myClassesChanged || !ListsEqual(entCachedClasses, entClasses))
                {
                    // Iterate our classes, check if entity shares any
                    foreach (var friClass in myClasses)
                    {
                        if (entClasses != null && entClasses.Contains(friClass))
                        {
                            if (entTypeVal == ENT_TYPE_PLAYER)
                            {
                                calculatedDisp = (int)VJBase.Disposition.Like;
                            }
                            else
                            {
                                if (friClass == "CLASS_PLAYER_ALLY")
                                {
                                    // core.lua:2227 — both have CLASS_PLAYER_ALLY: need AlliedWithPlayerAllies or IsDefaultNPC
                                    var entBase = ent.Components.Get<BaseNPC>();
                                    bool entAllyWithPlayers = entBase != null && entBase.AlliedWithPlayerAllies;
                                    bool entIsDefault = entBase != null && entBase.IsDefaultNPC;
                                    if ((myFriPlyAllies && entAllyWithPlayers) || entIsDefault)
                                        calculatedDisp = (int)VJBase.Disposition.Like;
                                }
                                else
                                {
                                    calculatedDisp = (int)VJBase.Disposition.Like;
                                }
                            }
                        }
                    }

                    // Cache results
                    entMemory[VJMemoryKey.CacheClasses] = entClasses;
                    if (calculatedDisp.HasValue)
                        entMemory[VJMemoryKey.CacheDisposition] = calculatedDisp.Value;
                    else
                        entMemory.Remove(VJMemoryKey.CacheDisposition);
                }
                else
                {
                    // Use cached disposition
                    if (entMemory.TryGetValue(VJMemoryKey.CacheDisposition, out var cachedDisp) && cachedDisp is int cd)
                        calculatedDisp = cd;
                }
            }

            // core.lua:2264-2272 — ent.HandlePerceivedRelationship: entity's custom perception callback
            // Can override disposition before friend/enemy handling
            var entHandlePerceived = ent.Components.Get<BaseNPC>()?.HandlePerceivedRelationship;
            if (entHandlePerceived != null)
            {
                var result = entHandlePerceived(ent, GameObject, distanceToEnt, calculatedDisp == (int)VJBase.Disposition.Like);
                if (result.HasValue)
                {
                    AddEntityRelationship(ent, result.Value, 0);
                    calculatedDisp = result;
                }
            }

            // ═══════════════════════════════════════════
            // core.lua:2275-2335 — Friend handling (D_LI)
            // ═══════════════════════════════════════════
            if (calculatedDisp == (int)VJBase.Disposition.Like)
            {
                // Reset enemy if currently targeting this friend
                if (GetEnemy() == ent)
                    ResetEnemy(true, false);

                AddEntityRelationship(ent, (int)VJBase.Disposition.Like, 0);

                // core.lua:2303-2326 — YieldToAlliedPlayers push detection
                if (entTypeVal == ENT_TYPE_PLAYER && YieldToAlliedPlayers && !IsGuard)
                {
                    var rb = ent.Components.Get<Rigidbody>();
                    if (rb != null)
                    {
                        var plyVel = rb.Velocity;
                        // core.lua:2306 — velocity threshold: 140² = 19600
                        if (plyVel.LengthSquared >= 19600f)
                        {
                            var delta = WorldSpaceCenter() - (WorldSpaceCenter_Entity(ent) + plyVel * 0.4f);
                            var myMaxs = OBBMaxs();
                            var myMins = OBBMins();
                            float zCalc = (myMaxs.z - myMins.z) * 0.5f;
                            float yCalc = myMaxs.y - myMins.y;

                            // core.lua:2313-2314 — push collision check
                            if (delta.z < zCalc && (delta.z + zCalc + 150) > zCalc
                                && (delta.x * delta.x + delta.y * delta.y) < (yCalc * yCalc * 1.999396f))
                            {
                                SetCondition(Condition.PlayerPushing);
                                if (!GetTarget().IsValid())
                                    SetTarget(ent);
                            }
                        }
                    }
                    // core.lua:2303: GetMoveType() != MOVETYPE_NOCLIP → Rigidbody check above is equivalent
                    // Phase 3: m_vecSmoothedVelocity → currently using rb.Velocity (instantaneous)
                }
            }
            // ═══════════════════════════════════════════
            // core.lua:2336-2410 — Enemy handling (D_HT / D_VJ_INTEREST / D_NU)
            // ═══════════════════════════════════════════
            else
            {
                // core.lua:2321-2334 — non-VJ NPC: tell them how to feel about us
                if (entTypeVal == ENT_TYPE_NPC)
                {
                    var entBase = ent.Components.Get<BaseNPC>();
                    bool isNonVJ = entBase == null || !entBase.IsVJBaseSNPC;
                    if (isNonVJ && entBase != null)
                    {
                        var myHandle = HandlePerceivedRelationship;
                        if (myHandle != null)
                        {
                            var result = myHandle(this.GameObject, ent, distanceToEnt, false);
                            entBase.AddEntityRelationship(GameObject, result ?? (int)VJBase.Disposition.Hate, 0);
                        }
                        else
                        {
                            entBase.AddEntityRelationship(GameObject, (int)VJBase.Disposition.Hate, 0);
                        }
                    }
                }

                var ene = GetEnemy();
                bool eneValid = ene.IsValid();

                if (!calculatedDisp.HasValue
                    || calculatedDisp == (int)VJBase.Disposition.Interest
                    || calculatedDisp == (int)VJBase.Disposition.Hate)
                {
                    // core.lua:2341-2345 — CanBeEngaged: let entity veto engagement
                    var entBaseNPC = ent.Components.Get<BaseNPC>();
                    var entCanEngage = entBaseNPC?.CanBeEngaged;
                    if (entCanEngage != null && !entCanEngage(ent, GameObject, distanceToEnt) && (!eneValid || ene != ent))
                    {
                        AddEntityRelationship(ent, (int)VJBase.Disposition.Interest, 0);
                        calculatedDisp = (int)VJBase.Disposition.Interest;
                    }
                    else
                    {
                    // core.lua:2364-2378 — core enemy detection
                    // Conditions: EnemyDetection ON + (not neutral OR already enemy-alerted) + (XRay OR visible) + in view cone
                    if (EnemyDetection
                        && (notIsNeutral || Alerted == VJAlertState.Enemy)
                        && (EnemyXRayDetection || CanSee(ent))
                        && IsInViewCone(entPos))
                    {
                        AddEntityRelationship(ent, (int)VJBase.Disposition.Hate, 0);
                        eneValid = true;
                        eneVisCount++;

                        // Pick nearest enemy
                        if (!nearestDist.HasValue || distanceToEnt < nearestDist.Value)
                        {
                            nearestDist = distanceToEnt;
                            nearestEnt = ent;
                            ForceSetEnemy(ent, true, true, eneValid);
                        }
                    }
                    // core.lua:2380-2388 — Disposition fallback (enemy detection failed, check existing relationship)
                    else if (Disposition(ent) != (int)VJBase.Disposition.Hate)
                    {
                        if (!notIsNeutral)
                        {
                            // Neutral NPCs don't engage without reason → mark as neutral
                            AddEntityRelationship(ent, (int)VJBase.Disposition.Neutral, 0);
                            calculatedDisp = (int)VJBase.Disposition.Neutral;
                        }
                        else
                        {
                            // Non-neutral NPCs mark potential enemies as interest
                            AddEntityRelationship(ent, (int)VJBase.Disposition.Interest, 0);
                            calculatedDisp = (int)VJBase.Disposition.Interest;
                        }
                    }
                    } // closes else (CanBeEngaged passed → run enemy detection)
                }
                else
                {
                    // core.lua:2375 — calculatedDisp is some other value, default to neutral
                    calculatedDisp = (int)VJBase.Disposition.Neutral;
                }

                // ═══════════════════════════════════════════
                // core.lua:2378-2408 — Investigation system
                // ═══════════════════════════════════════════
                if (!eneValid && CanInvestigate && NextInvestigationMove < Time.Now)
                {
                    // core.lua:2381 — Sound investigation: VJ_SD_InvestLevel
                    // Phase 3: VJ_SD_InvestLevel/VJ_SD_InvestTime from entity sound data
                    float sdLevel = GetEntitySoundInvestLevel(ent);
                    float sdTime = GetEntitySoundInvestTime(ent);
                    if (sdLevel > 0 && distanceToEnt < (InvestigateSoundMultiplier * sdLevel) && (Time.Now - sdTime) <= 1f)
                    {
                        DoReadyAlert();
                        if (CanSee(ent))
                        {
                            StopMoving();
                            SetTarget(ent);
                            SCHEDULE_FACE(EngineTask.FaceTarget);
                            NextInvestigationMove = Time.Now + 0.3f;
                        }
                        else if (!IsFollowing)
                        {
                            SetLastPosition(entPos);
                            SCHEDULE_GOTO_POSITION(EngineTask.WalkPath, schedule =>
                            {
                                schedule.CanShootWhenMoving = true;
                                schedule.TurnData = new TurnData { Type = VJFaceStatus.Enemy };
                            });
                            NextInvestigationMove = Time.Now + 2f;
                        }
                        OnInvestigate?.Invoke(ent);
                        PlaySoundSystem("Investigate");
                    }
                    // core.lua:2402 — Flashlight investigation
                    else if (entTypeVal == ENT_TYPE_PLAYER && distanceToEnt < 350f && IsEntityShiningFlashlightOnMe(ent, myPos, entPos))
                    {
                        StopMoving();
                        SetTarget(ent);
                        SCHEDULE_FACE(EngineTask.FaceTarget);
                        NextInvestigationMove = Time.Now + 0.1f;
                    }
                }
            }

            // ═══════════════════════════════════════════
            // core.lua:2412-2421 — OnPlayerSight system
            // ═══════════════════════════════════════════
            if (entTypeVal == ENT_TYPE_PLAYER
                && HasOnPlayerSight
                && Time.Now > NextOnPlayerSightT
                && distanceToEnt < OnPlayerSightDistance
                && CanSee(ent)
                && IsInViewCone(entPos))
            {
                int disp = calculatedDisp ?? 0;
                bool shouldFire = OnPlayerSightDispositionLevel == 0
                    || (OnPlayerSightDispositionLevel == 1 && (disp == (int)VJBase.Disposition.Like || disp == (int)VJBase.Disposition.Neutral))
                    || (OnPlayerSightDispositionLevel == 2 && disp != (int)VJBase.Disposition.Like);

                if (shouldFire)
                {
                    OnPlayerSight(ent);
                    PlaySoundSystem("OnPlayerSight");
                    if (OnPlayerSightOnlyOnce)
                        HasOnPlayerSight = false;
                    else
                        NextOnPlayerSightT = Time.Now + VJUtility.Rand(OnPlayerSightNextTime.a, OnPlayerSightNextTime.b);
                }
            }

            // core.lua:2426 — customFunc(self, ent, calculatedDisp, distanceToEnt)
            customFunc?.Invoke(this, ent, calculatedDisp, distanceToEnt);
        }

        // core.lua:2425-2426
        Enemy.VisibleCount = eneVisCount;
        return eneVisCount > 0;
    }

    // ═══ Utility: compare two lists ═══
    private static bool ListsEqual<T>(List<T> a, List<T> b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
            if (!EqualityComparer<T>.Default.Equals(a[i], b[i]))
                return false;
        return true;
    }

    // ═══ Investigation helpers ═══

    // core.lua: entity.VJ_SD_InvestLevel — sound data set by noise-emitting entities
    protected virtual float GetEntitySoundInvestLevel(GameObject ent)
    {
        // Phase 3: read from ent.Components or EntityMemory
        return 0f;
    }

    // core.lua: entity.VJ_SD_InvestTime — timestamp of last sound from entity
    protected virtual float GetEntitySoundInvestTime(GameObject ent)
    {
        // Phase 3: read from ent.Components or EntityMemory
        return 0f;
    }

    // core.lua:2402 — player shining flashlight at NPC
    protected virtual bool IsEntityShiningFlashlightOnMe(GameObject player, Vector3 myPos, Vector3 playerPos)
    {
        // Phase 3: need FlashlightIsOn() from player input/component system
        // Full Lua check: ent:FlashlightIsOn() && (ent:GetForward():Dot((myPos-entPos):Normalized) > cosRad20)
        return false;
    }
}
