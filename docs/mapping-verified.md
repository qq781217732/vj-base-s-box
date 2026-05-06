# GMod Lua → S&Box C# Verified Mapping Table

> Extracted from actual Lua source: core.lua, schedules.lua, base_aa.lua, funcs.lua
> Cross-referenced with docs/api-mapping.md

## Category A: Delete — Source engine exclusive (no S&Box equivalent needed)

| Lua API | Reason to delete |
|---------|-----------------|
| `StartSchedule()`, `ClearSchedule()` | Replaced by async `ScheduleRunner.Execute()` |
| `StartEngineTask()`, `RunEngineTask()` | Replaced by async/await |
| `SetTask()`, `StartTask()`, `RunTask()`, `TaskComplete()`, `TaskFinished()`, `NextTask()` | ScheduleRunner handles this |
| `SetCondition()`, `ClearCondition()`, `HasCondition()`, `SetIgnoreConditions()`, `RemoveIgnoreConditions()` | **Keep as self-built** — HashSet<Condition> |
| `GetState()`, `SetState()` | NPCState property |
| `CapabilitiesAdd()`, `CapabilitiesRemove()` | Use bool properties |
| `AddEntityRelationship()`, `Disposition()` | DispositionSystem |
| `SetSaveValue()`, `GetSaveValue()`, `GetInternalVariable()` | Delete — no equivalent |
| `IsEFlagSet()` | Delete |
| `GetNavType()`, `GetMovementActivity()`, `SetMovementActivity()`, `GetMovementSequence()`, `SetIdealSequence()` | NavMeshAgent handles movement |
| `GetIdealActivity()`, `SetIdealActivity()`, `ResetIdealActivity()`, `GetActivity()`, `SetActivity()`, `MaintainActivity()` | AnimationDriver.PlayAnim |
| `GetIdealSequence()`, `SelectWeightedSequence()` | Delete — no ACT system |
| `GetSequenceActivity()`, `IsSequenceFinished()`, `SequenceDuration()`, `GetSequenceMoveDist()` | AnimationDriver |
| `GetCurGoalType()`, `SetArrivalDistance()`, `ClearGoal()` | NavMeshAgent manages this |
| `AutoMovement()`, `GetAnimTimeInterval()` | Delete |
| `GetIdealYaw()`, `SetIdealYawAndUpdate()`, `IsFacingIdealYaw()`, `GetTurnAngle()` | FaceTarget logic |
| `TranslateNavGoal()` | NavMeshAgent manages this |
| `TranslateActivity()` | Delete — no ACT system |
| `FrameTime()` | `Time.Delta` |
| `IsRunningBehavior()` | Delete |
| `FindMetaTable("Entity")` / `FindMetaTable("NPC")` | Delete — C# native types |
| `AccessorFunc(ENT, ...)` | `[Property]` attribute |
| `ENT.VJ_*` (data fields) | C# properties/fields |
| `DoSchedule()`, `StopCurrentSchedule()`, `ScheduleFinished()` | ScheduleRunner |
| `OnTaskFailed()`, `OnMovementFailed()`, `OnMovementComplete()`, `OnStateChange()` | Delete — reimplement in C# |
| `PlaySequence()` | AnimationDriver.PlaySequence |
| `SetMaxLookDistance()` | ViewDistance property |

## Category B: Keep as self-built (GMod utility functions → C# methods)

| Lua call | S&Box C# | Where |
|----------|----------|-------|
| `VJ.PICK(tbl)` | `RandomHelper.FromList(list)` | VJUtils |
| `VJ.STOPSOUND(sd)` | `SoundHandle.Stop()` | NPCSoundSystem |
| `VJ.CreateSound(ent, file, lvl, pitch)` | `NPCSoundSystem.Play(file, vol, pitch)` | Sound |
| `VJ.EmitSound(ent, file, lvl, pitch, vol, ch)` | `Sound.Play(file, pos)` | Sound / inline |
| `VJ.HasValue(tbl, val)` | `list.Contains(val)` | native |
| `VJ.GetMoveVelocity(ent)` | `Rigidbody.Velocity` | native |
| `VJ.GetMoveDirection(ent, ignoreZ)` | Custom calculation on Transform | Movement |
| `VJ.GetNearestPositions(e1, e2)` | Transform + Collider.Bounds calc | Combat |
| `VJ.GetNearestDistance(e1, e2)` | `Vector3.DistanceBetween(a, b)` | native |
| `VJ.TraceDirections(...)` | `Senses.TraceDirections()` | Senses |
| `VJ.AnimExists(ent, anim)` | `AnimationDriver.HasSequence(name)` | AnimationDriver |
| `VJ.AnimDuration(ent, anim)` | `SkinnedModelRenderer.SceneModel.CurrentSequence.Duration` | AnimationDriver |
| `VJ.AnimDurationEx(ent, anim, ovr, dec)` | Custom calculation | AnimationDriver |
| `VJ.SequenceToActivity(ent, anim)` | **Delete** — no ACT system | — |
| `VJ.IsCurrentAnim(ent, anim)` | `AnimationDriver.IsPlaying(name)` | AnimationDriver |
| `VJ.GetPoseParameters(ent)` | `SkinnedModelRenderer` params | AnimationDriver |
| `VJ.CalculateTrajectory(...)` | `Trajectory.Calculate()` | VJUtils |
| `VJ.ApplyRadiusDamage(...)` | `Combat.RadiusDamage()` | Combat |
| `VJ.ApplySpeedEffect(...)` | `SpeedEffect component` | Components |
| `VJ.GetName(ent)` | `GameObject.Name` | native |
| `VJ.IsProp(ent)` | Custom tag/component check | Utilities |
| `VJ.RoundToMultiple(num, mult)` | Math utility | VJUtils |
| `VJ.Color2Byte(color)` | Bit manipulation | VJUtils |

## Category C: Replace with S&Box equivalent (just rename/map)

### Transform (16 APIs)
| Lua | C# |
|-----|-----|
| `self:GetPos()` | `GameObject.Transform.Position` |
| `self:SetPos(v)` | `GameObject.Transform.Position = v` |
| `self:GetAngles()` | `GameObject.Transform.Rotation` |
| `self:SetAngles(a)` | `GameObject.Transform.Rotation = a.ToRotation()` |
| `self:GetForward()` | `GameObject.Transform.World.Forward` |
| `self:GetRight()` | `GameObject.Transform.World.Right` |
| `self:GetUp()` | `GameObject.Transform.World.Up` |
| `self:EyePos()` | `Transform.Position + Vector3.Up * EyeHeight` |
| `self:GetShootPos()` | Same as EyePos + offset |
| `self:BodyTarget(pos)` | `(Position + EyePosition) * 0.5f` |
| `self:WorldSpaceCenter()` | `Collider.Bounds.Center` or Model.Bounds |
| `self:OBBCenter()` | `Collider.Bounds.Center` |
| `self:OBBMins()` | `Collider.Bounds.Mins` |
| `self:OBBMaxs()` | `Collider.Bounds.Maxs` |
| `self:NearestPoint(pos)` | `Collider.ClosestPoint(pos)` |
| `self:WorldToLocal(pos)` | `Transform.World.PointToLocal(pos)` |

### Physics/Movement (8 APIs)
| Lua | C# |
|-----|-----|
| `self:GetVelocity()` | `Rigidbody.Velocity` |
| `self:SetVelocity(v)` | `Rigidbody.Velocity = v` |
| `self:SetLocalVelocity(v)` | `Rigidbody.Velocity = v` (world space) |
| `self:IsMoving()` | `Agent.Velocity.Length > 10f` |
| `self:GetMoveVelocity()` | `Rigidbody.Velocity` |
| `self:GetMoveDelay()` | Custom timer |
| `self:StopMoving()` | `Agent.Stop()` or `Agent.MoveTo(Position)` |
| `physenv.GetGravity()` | `Scene.PhysicsWorld.Gravity` |

### Entity/Lifecycle (10 APIs)
| Lua | C# |
|-----|-----|
| `self:GetClass()` | `GetType().Name` |
| `self:GetName()` | `GameObject.Name` |
| `self:SetName(n)` | `GameObject.Name = n` |
| `self:Remove()` | `GameObject.Destroy()` |
| `self:EntIndex()` | `GameObject.Id` |
| `ents.Create("class")` | `new GameObject("name")` |
| `ents.FindInSphere(pos, r)` | `Scene.FindInPhysics(new Sphere(pos, r))` |
| `ents.FindInCone(pos, dir, r, cos)` | Custom cone query |
| `IsValid(e)` | `e.IsValid()` |
| `self:IsNPC()` | `e.Components.TryGet<BaseNPC>(out _)` |
| `self:IsPlayer()` | `e.Components.TryGet<Player>(out _)` |

### Perception (6 APIs)
| Lua | C# |
|-----|-----|
| `self:Visible(e)` | `Senses.CanSee(self, target)` |
| `self:VisibleVec(pos)` | `Senses.CanSeePoint(self, pos)` |
| `self:IsInViewCone(e)` | `Senses.IsInViewCone(self, pos)` |
| `util.TraceLine({start, endpos, filter, mask})` | `SceneTrace.Ray(start, end).IgnoreGameObject(self).Run()` |
| `util.TraceHull({start, endpos, filter, mins, maxs})` | `SceneTrace.Box(mins, maxs, start, end).IgnoreGameObject(self).Run()` |
| `util.PointContents(pos)` | `Physics.TestPoint(pos)` |

### Model/Animation (10 APIs)
| Lua | C# |
|-----|-----|
| `self:SetModel(m)` | `Model.Model = Sandbox.Model.Load(m)` |
| `self:SetColor(c)` | `Model.Tint = c` |
| `self:SetMaterial(m)` | `Model.SetMaterialOverride(m, "skin")` |
| `self:SetSkin(n)` | `Model.MaterialGroup = n.ToString()` |
| `self:SetBodygroup(n, v)` | `Model.SetBodyGroup(name, v)` |
| `self:GetSequence()` | `Model.SceneModel.CurrentSequence.Name` |
| `self:LookupSequence(name)` | `Model.Model.AnimationNames.Contains(name)` |
| `self:GetSequenceName(id)` | `Model.Model.AnimationNames[id]` |
| `self:SetPlaybackRate(r)` | `Model.PlaybackRate = r` |
| `self:GetPlaybackRate()` | `Model.PlaybackRate` |
| `self:LookupAttachment(name)` | `Model.Model.GetAttachment(name)` |
| `self:GetAttachment(id)` | `Model.GetBoneTransform(id)` |
| `self:GetNumPoseParameters()` | Delete — AnimGraph driven |
| `self:GetPoseParameterName(i)` | Delete |
| `self:GetPoseParameterRange(i)` | Delete |

### Damage/Combat (6 APIs)
| Lua | C# |
|-----|-----|
| `self:TakeDamage(n, attacker, inflictor)` | `GameObject.TakeDamage(DamageInfo)` |
| `self:TakeDamageInfo(dmgInfo)` | `GameObject.TakeDamage(DamageInfo)` |
| `DamageInfo()` | `new DamageInfo()` |
| `dmgInfo:SetDamage(n)` | `dmgInfo.Damage = n` |
| `dmgInfo:SetAttacker(e)` | `dmgInfo.Attacker = e` |
| `dmgInfo:SetInflictor(e)` | `dmgInfo.Inflictor = e` |
| `dmgInfo:SetDamageType(t)` | `dmgInfo.Tags.Add("blast")` etc |
| `dmgInfo:SetDamagePosition(p)` | `dmgInfo.Position = p` |
| `dmgInfo:SetDamageForce(v)` | `dmgInfo.Force = v` |
| `self:Health()` | `Health` property |
| `self:SetHealth(n)` | `Health = n` |
| `self:SetMaxHealth(n)` | `MaxHealth = n` |
| `self:Alive()` | `Health > 0f` |

### Timer (4 APIs)
| Lua | C# |
|-----|-----|
| `timer.Simple(t, fn)` | `_ = GameTask.DelaySeconds(t).ContinueWith(_ => fn())` |
| `timer.Create(name, t, n, fn)` | for-loop + `GameTask.DelaySeconds(t)` |
| `timer.Remove(name)` | `CancellationTokenSource.Cancel()` |
| `CurTime()` | `Time.Now` |

### Sound (3 APIs)
| Lua | C# |
|-----|-----|
| `self:EmitSound(snd, lvl, pitch, vol, ch)` | `Sound.Play(snd, pos)` |
| `self:StopAllSounds()` | `NPCSoundSystem.StopAll()` |
| `CreateSound(ent, file, filter)` | `Sound.Play(file, pos)` |
| `sdID:SetSoundLevel(lvl)` | Delete — no radar system |
| `sdID:PlayEx(vol, pitch)` | `handle.Volume = vol; handle.Pitch = pitch` |

### Debug (4 APIs)
| Lua | C# |
|-----|-----|
| `debugoverlay.Box(pos, mins, maxs, dur, color)` | `GameObject.DebugOverlay.Box(pos, mins, maxs, color)` |
| `debugoverlay.Line(a, b, dur, color)` | `GameObject.DebugOverlay.Line(a, b, color)` |
| `debugoverlay.Text(pos, txt, dur, ...)` | `GameObject.DebugOverlay.Text(pos, txt)` |
| `debugoverlay.Cross(pos, size, dur, color)` | Lines via DebugOverlay |
| `ParticleEffect(name, pos, ang, ent)` | `VFXHelper.PlayParticles(name, pos)` |
| `util.ParticleTracerEx(name, start, end, ...)` | Custom via Particles component |
| `util.Decal(name, pos, norm)` | `VFXHelper.PlaceDecal(name, pos, norm)` |

### Utility/Math (8 APIs)
| Lua | C# |
|-----|-----|
| `math.random(a, b)` | `Game.Random.Next(a, b + 1)` |
| `math.Rand(a, b)` | `Game.Random.Float(a, b)` |
| `math.Round/M athFloor/MathMin/MathMax/Cos/Sin` | `MathF.Round/Floor/Min/Max/Cos/Sin` |
| `math.rad(deg)` / `math.deg(rad)` | `MathF.DegreesToRadians(d)` / `MathF.RadiansToDegrees(r)` |
| `math.atan2(y, x)` | `MathF.Atan2(y, x)` |
| `math.AngleDifference(a, b)` | Custom `MathX.AngleDifference(a, b)` |
| `bit.band(a, b)` | `a & b` |
| `bit.lshift(a, b)` | `a << b` |
| `Vector(x, y, z)` | `new Vector3(x, y, z)` |
| `Angle(p, y, r)` | `Rotation.FromPitchYawRoll(p, y, r)` |
| `Lerp(t, a, b)` | `MathX.Lerp(a, b, t)` |
| `LerpAngle(t, a, b)` | `Rotation.Slerp(a, b, t)` |
| `string.find/sub/gsub/Left` | C# string methods |
| `table.insert(t, v)` | `list.Add(v)` |
| `table.remove(t, i)` | `list.RemoveAt(i)` |
| `table.concat(t, sep)` | `string.Join(sep, t)` |
| `ipairs(t)` / `pairs(t)` | `for (int i = 0; i < t.Count; i++)` / `foreach` |
| `isnumber(v)` / `isvector(v)` / `isstring(v)` | `v is float` / `v is Vector3` / compile-time types |

## Coverage Summary

| Category | APIs | Status |
|----------|------|--------|
| A: Delete (Source exclusive) | ~55 | Skipped |
| B: Self-build (utility) | ~22 | Implement in VJUtils/helpers |
| C: Replace (S&Box eqv) | ~85 | Direct mapping |
| **Total** | **~162** | **107 need implementation** |

## Implementation Priority

1. **Trivial** (~45 APIs): Transform, math, basic entity — just rename
2. **Self-build logic** (~22 APIs): VJ utility functions — translate logic to C#
3. **Architecture redesign** (~20 APIs): Schedule/Task/Condition — async state machine
4. **Delete** (~55 APIs): Don't implement — S&Box handles differently
