# AI Core (core.lua) — Manual Function Index

> Auto-extraction failed (GMod syntax breaks tree-sitter). Manually indexed from source.
> C# Target column shows which Component already covers each function.

## Lifecycle + Data

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:CreateExtraDeathCorpse(class, models, ...)` | 209 | Spawn additional corpse pieces on death | `BaseNPC` |
| `ENT:CreateGibEntity(class, models, ...)` | 295 | Spawn individual gib with physics | `BaseNPC` |
| `ENT:GetSoundInterests()` | 391 | Returns sound interest bitmask | `CreatureNPC` |
| `ENT:ResetEatingBehavior()` | 399 | Full eating state machine reset | `CreatureNPC` |
| `ENT:OnEat(status, statusData)` | 445 | Eating behavior state machine | `CreatureNPC` |
| `ENT:OnRemove()` | 3492 | Cleanup: timers, sounds, medic, eating | `BaseNPC` |
| `ENT:CreateDeathLoot()` | 3470 | Spawn loot on death | `BaseNPC` |
| `ENT:SetupBloodColor(blColor)` | 2725 | Set blood particle/decal/pool from color name | `BaseNPC` |
| `ENT:SpawnBloodParticles(pos)` | 2744 | Spawn blood impact particles | `BaseNPC` |
| `ENT:SpawnBloodDecals(pos, ...)` | 2760 | Spawn blood decals | `BaseNPC` |
| `ENT:SpawnBloodPool()` | 2780 | Spawn blood pool on ground | `BaseNPC` |

## Animation

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:UpdateAnimationTranslations()` | 485 | Detect model type + set animation tables | `AnimationDriver` |
| `ENT:ResolveAnimation(animation)` | 509 | Pick random anim from table, keep current if playing | `AnimationDriver` |
| `ENT:MaintainIdleAnimation()` | 526 | Cycle idle anims, set cycle=0 on transitions, loop flags | `AnimationDriver` |
| `ENT:PlayAnim(animation, lockAnim, lockAnimTime, faceEnemy, animDelay, extraOptions, customFunc)` | 631 | **Core animation** (~250 lines). Handles activities, sequences, gestures, vjges_/vjseq_ prefixes, locks, face-enemy, OnFinish callbacks | `AnimationDriver.PlayAnim()` |
| `ENT:PlaySequence(sequenceName)` | 2004 | Play raw sequence with ACT_DO_NOT_DISTURB | `AnimationDriver.PlaySequence()` |
| `ENT:IsBusy(behaviors, activities)` | 881 | Check if NPC busy with given behaviors/activities | `BaseNPC` |
| `ENT:GetAttackTimer(mainTime, executionTime, animDur)` | 972 | Calculate attack timer accounting for playback rate | `BaseNPC` |
| `ENT:AddExtraAttackTimer(name, time, func)` | 2030 | Named, playback-rate-scaled timer for multi-hit attacks | `BaseNPC` |

## Movement + Navigation

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:MaintainIdleBehavior()` | 561 | Wander vs idle stand decision (DisableWandering, IsGuard, movement type) | `CreatureNPC` |
| `ENT:DoGroupFormation(formType, baseEnt, it, spacing)` | 1261 | Diamond formation positioning | `HumanNPC` |
| `ENT:ForceMoveJump(vel)` | 1348 | Force NPC jump | `BaseNPC` |
| `ENT:IsJumpLegal()` | 1451 | Check jump params + ground trace | `BaseNPC` |
| `ENT:OverrideMoveFacing()` | 1129 | Override engine move facing; player pose params + footstep | `BaseNPC` |
| `ENT:OverrideMove()` | 1158 | Handle NAV_JUMP during non-task jumps | `BaseNPC` |

## Turning / Facing

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:GetTurnAngle(resultAng)` | 1014 | Patch angle to allowed rotations | `BaseNPC` |
| `ENT:ResetTurnTarget()` | 1021 | Reset facing/turning data | `BaseNPC` |
| `ENT:SetTurnTarget(type, target, stopOnFace)` | 1043 | **Main facing** (~70 lines). Enemy/Vector/Entity facing with stopOnFace, visibleOnly, timer reset | `BaseNPC.SetTurnTarget()` |
| `ENT:DeltaIdealYaw()` | 1115 | Calculate yaw difference from ideal yaw | `BaseNPC` |
| `ENT:MaintainConstantlyFaceEnemy()` | 1945 | Face enemy if within min distance (posture/visibility/attacking) | `BaseNPC` |

## Combat: Aim + Range

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:GetAimPosition(target, aimOrigin, predictionRate, projectileSpeed)` | 1185 | Aim pos with prediction (BodyTarget/HeadTarget/VisiblePos fallback) | `BaseNPC.GetAimPosition()` |
| `ENT:GetAimSpread(target, goalPos, modifier)` | 1244 | Spread from distance + target movement + suppression | `BaseNPC.GetAimSpread()` |
| `ENT:ScaleByDifficulty(base)` | 1411 | 16-level difficulty scaling (0.01x to 10x) | `BaseNPC.ScaleByDifficulty()` |
| `ENT:StopAttacks(checkTimers)` | (in creature init) | Reset AttackType/State/Seed | `CreatureNPC.StopAttacks()` |

## Combat: Damage + Flinch

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:Flinch(dmginfo, hitgroup)` | 2594 | **Hitgroup flinch** (~50 lines). DMG_FORCE_FLINCH, hitgroup maps, cooldown, PlayAnim | `CreatureNPC.Flinch()` |
| `ENT:IsGibDamage(dmginfo)` | 3430 | Check damage type against gibbing mask | `BaseNPC` |
| `ENT:GibOnDeath(dmginfo, hitgroup)` | 3434 | Gib death handler with filter + overrides | `BaseNPC` |

## Combat: Melee + Range + Leap (wrappers)

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:MeleeAttackCode(isPropAttack)` | 3532 | Wrapper → ExecuteMeleeAttack | `CreatureNPC.ExecuteMeleeAttack()` |
| `ENT:RangeAttackCode()` | 3533 | Wrapper → ExecuteRangeAttack | `CreatureNPC.ExecuteRangeAttack()` |
| `ENT:LeapDamageCode()` | 3534 | Wrapper → ExecuteLeapAttack | `CreatureNPC.ExecuteLeapAttack()` |

## Enemy Detection + Relationships

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:ForceSetEnemy(ent, forceSound, ...)` | 2043 | Force enemy assignment with relationship/memory/state/alert | `BaseNPC` |
| `ENT:DoReadyAlert()` | 2066 | Ready alert state machine | `BaseNPC` |
| `ENT:DoEnemyAlert()` | 2076 | Enemy alert: sound + state | `BaseNPC` |
| `ENT:SetRelationshipMemory(ent, key, value)` | 2100 | Per-entity relationship data | `BaseNPC` |
| `ENT:CheckRelationship(ent)` | 2112 | Convert VJ dispositions to engine D_* enums | `BaseNPC` |
| `ENT:MaintainRelationships()` | 2127 | **THE enemy detection** (~300 lines). Iterates RelationshipEnts, class alliances, HandlePerceivedRelationship, sight cone + visible check, CanBeEngaged, investigation detection, OnPlayerSight, YieldToAlliedPlayers push | `BaseNPC.UpdateSenses()` |

## Allies + Squad

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:Allies_CallHelp()` | 2438 | Call nearby allies to help fight enemy | `HumanNPC.CallAlliesForHelp()` |
| `ENT:Allies_Check()` | 2507 | Count nearby allies | `HumanNPC` |
| `ENT:Allies_Bring(formType, bringRadius)` | 2542 | Bring allies to formation (respects guard/follow/busy) | `HumanNPC` |

## Follow + Medic

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:ResetFollowBehavior()` | 1727 | Stop following, notify player | `HumanNPC` |
| `ENT:Follow(ent, doToggle)` | 1759 | **Following system** (~70 lines). Refusals, toggle off, schedule face+goto | `HumanNPC.Follow()` |
| `ENT:ResetMedicBehavior()` | 1829 | Reset medic state, remove prop, cooldown | `HumanNPC` |
| `ENT:MaintainMedicBehavior()` | 1839 | **Medic state machine** (~100 lines). Find wounded, move to them, heal anim, apply health | `HumanNPC.MedicCheck()` |

## Sound System

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:PlayFootstepSound(customSD)` | 2807 | Footstep sound timing (run vs walk) | `CreatureNPC` |
| `ENT:PlayIdleSound()` | 2836 | Idle sound with idle dialogue (speak + answer between NPCs) | `CreatureNPC.PlayAmbientSounds()` |
| `ENT:PlaySoundSystem(sdSet)` | 2944 | **MASSIVE** (~430 lines). 30 categories: IdleDialogue, Follow, Alert, CallForHelp, Melee, Pain, Death, Gib, Range, Leap, Suppressing, Reload, Grenade, DangerSight, KilledEnemy, AllyDeath, DamageByPlayer, Impact, Speech, etc. | `CreatureNPC.PlayAttackSound()` + `CreatureNPC.PlaySoundFromTable()` |

## Cover + Utility

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:DoCoverTrace(startPos, endPos, acceptWorld, extraOptions)` | 1294 | Cover detection: trace from self to enemy, validate hiding zone, sphere invalidation | `HumanNPC.IsCoverBetween()` |
| `ENT:GetLastDamageHitGroup()` | 1358 | Engine internal m_LastHitGroup | `BaseNPC` |
| `ENT:GetState()` / `ENT:SetState()` | 910 | State management (freeze, only-animation) | `BaseNPC` |
| `ENT:AcceptInput(inputName, activator, called, value)` | 1637 | Input handling (Use for follow, StartScripting/StopScripting, break) | `BaseNPC` |
| `ENT:Touch(ent)` | 1672 | Passive run-on-touch, enemy touch detection, yield-to-allied-players | `BaseNPC` |
| `ENT:Controller_Movement(enabled)` | 1964 | Player-controlled movement (WASD + sprint) | `BaseNPC` |
| `ENT:ValidateNoCollide()` | 3393 | Apply no-collide for EntitiesToNoCollide list | `BaseNPC` |
| `ENT:StartSoundTrack()` | 3456 | Background music system | `CreatureNPC` |

---

**Total: 60 functions indexed from core.lua (3,470 lines)**

C# Coverage: ~70% have a corresponding method in our Components.
Remaining ~30% are blood effects, gib spawning, eating system, dialogue system — lower priority.
