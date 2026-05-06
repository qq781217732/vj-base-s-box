using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase;

/// <summary>
/// Aerial &amp; Aquatic movement — mechanical translation of vj_base/ai/base_aa.lua.
/// AA_MoveTo / AA_IdleWander / AA_ChaseEnemy / AA_MoveAnimation / AA_StopMoving.
/// </summary>
public partial class BaseNPC
{
	// ═══ AA Movement State Fields (base_aa.lua:9-19) ═══
	public float AA_NextMoveAnimTime { get; set; }
	public object AA_CurrentMoveAnim { get; set; } = false; // false = none, -1 = skip
	public string AA_CurrentMoveAnimType { get; set; } = "Calm";
	public float AA_CurrentMoveMaxSpeed { get; set; }
	public float AA_CurrentMoveTime { get; set; }
	public int AA_CurrentMoveType { get; set; } // 0=Undefined, 1=Wander, 2=MoveTo, 3=ChaseEnemy
	public Vector3? AA_CurrentMovePos { get; set; }
	public Vector3? AA_CurrentMovePosDir { get; set; }
	public float AA_CurrentMoveDist { get; set; } = -1;
	public Vector3? AA_LastChasePos { get; set; }
	public bool AA_DoingLastChasePos { get; set; }

	// ═══ AA Config — defaults, overridden by derived NPC classes in shared.lua ═══
	public float Aerial_FlyingSpeed_Calm { get; set; } = 200;
	public float Aerial_FlyingSpeed_Alerted { get; set; } = 400;
	public float Aquatic_SwimmingSpeed_Calm { get; set; } = 100;
	public float Aquatic_SwimmingSpeed_Alerted { get; set; } = 200;
	public float AA_GroundLimit { get; set; } = 500;
	public int AA_MinWanderDist { get; set; } = 500;
	public float AA_MoveAccelerate { get; set; }
	public bool HasMeleeAttack { get; set; }
	public List<string> Aerial_AnimTbl_Calm { get; set; } = new();
	public List<string> Aerial_AnimTbl_Alerted { get; set; } = new();
	public List<string> Aquatic_AnimTbl_Calm { get; set; } = new();
	public List<string> Aquatic_AnimTbl_Alerted { get; set; } = new();

	// ═══ Trace offset constants (base_aa.lua:54-55) ═══
	private static readonly Vector3 VecStart = new(0, 0, 30);
	private static readonly Vector3 VecEnd = new(0, 0, 40);

	// ═══════════════════════════════════════════════
	// AA_StopMoving — base_aa.lua:30-41
	// ═══════════════════════════════════════════════
	public virtual void AA_StopMoving()
	{
		// base_aa.lua:31 — GetVelocity():Length() > 0 check
		var rb = GameObject.Components.Get<Rigidbody>();
		if (rb == null || rb.Velocity.Length <= 0) return;

		// base_aa.lua:32-38 — reset movement state
		AA_CurrentMoveMaxSpeed = 0;
		AA_CurrentMoveTime = 0;
		AA_CurrentMoveType = 0;
		AA_CurrentMovePos = null;
		AA_CurrentMovePosDir = null;
		AA_CurrentMoveDist = -1;
		// base_aa.lua:39: self:SetLocalVelocity(defPos)
		rb.Velocity = Vector3.Zero;
	}

	// ═══════════════════════════════════════════════
	// AA_MoveTo — base_aa.lua:57-257
	// ═══════════════════════════════════════════════
	public virtual void AA_MoveTo(object dest, bool playAnim = true, string moveType = "Calm", Dictionary<string, object> extraOptions = null)
	{
		// base_aa.lua:58-59 — dest type check
		var destVec = dest as Vector3?;
		var destGO = (destVec == null && dest is GameObject go && go.IsValid()) ? go : null;

		// base_aa.lua:60 — Dead guard / invalid dest guard
		if (Dead || (destVec == null && destGO == null)) return;

		// base_aa.lua:61-63 — defaults
		moveType ??= "Calm";
		extraOptions ??= new Dictionary<string, object>();
		var addPos = extraOptions.TryGetValue("AddPos", out var ap) && ap is Vector3 apv ? apv : Vector3.Zero;
		var chaseEnemy = extraOptions.TryGetValue("ChaseEnemy", out var ce) && ce is bool ceb && ceb;
		var faceDest = !extraOptions.TryGetValue("FaceDest", out var fd) || fd is not bool fdb || fdb;
		var faceDestTarget = extraOptions.TryGetValue("FaceDestTarget", out var fdt) && fdt is bool fdtb && fdtb;
		var ignoreGround = extraOptions.TryGetValue("IgnoreGround", out var ig) && ig is bool igb && igb;

		// base_aa.lua:65 — move speed by type
		float moveSpeed = moveType == "Calm"
			? (MovementType == VJMoveType.Aquatic ? Aquatic_SwimmingSpeed_Calm : Aerial_FlyingSpeed_Calm)
			: (MovementType == VJMoveType.Aquatic ? Aquatic_SwimmingSpeed_Alerted : Aerial_FlyingSpeed_Alerted);

		bool debug = VJ_DEBUG;
		var myPos = GameObject.WorldPosition;

		// base_aa.lua:71-107 — Aquatic initial checks
		if (MovementType == VJMoveType.Aquatic)
		{
			moveSpeed = moveType == "Calm" ? Aquatic_SwimmingSpeed_Calm : Aquatic_SwimmingSpeed_Alerted;
			// SKIP: base_aa.lua:73-78 — WaterLevel() check + debug prints — Phase 3 water system
			// WaterLevel() is Source engine builtin. s&box has no equivalent yet.
			// SKIP: base_aa.lua:79 — WaterLevel() <= 2 → MaintainIdleBehavior(1)
			// SKIP: base_aa.lua:82-89 — aquatic vector destination water trace (MASK_WATER)
			// SKIP: base_aa.lua:92-106 — aquatic entity destination WaterLevel/reachability checks
		}

		// base_aa.lua:109-119 — Movement Calculations — TraceHull
		// NOTE: WorldSpaceCenter() currently = GameObject.WorldPosition (Phase 3: add OBBCenter offset)
		// Lua: myPos + OBBCenter() + vecStart. Since WorldSpaceCenter() already includes WorldPosition:
		var startPos = WorldSpaceCenter() + VecStart;
		var endPos = destVec ?? (WorldSpaceCenter_Entity(destGO!) + VecEnd);
		var obbMins = OBBMins();
		var obbMaxs = OBBMaxs();
		var extents = (obbMaxs - obbMins) * 0.5f;

		var traceBuilder = Game.ActiveScene.Trace.Box(extents, startPos, endPos)
			.IgnoreGameObjectHierarchy(GameObject)
			.WithoutTags("phys_bone_follower");
		if (destGO != null)
			traceBuilder = traceBuilder.IgnoreGameObjectHierarchy(destGO);
		var tr = traceBuilder.Run();
		var trHitPos = tr.HitPosition;

		// base_aa.lua:125-147 — Ground check (aerial only, non-chase or no melee)
		if (MovementType == VJMoveType.Aerial && !ignoreGround && (!chaseEnemy || (chaseEnemy && !HasMeleeAttack)))
		{
			var trCheck1 = Game.ActiveScene.Trace.Ray(startPos, startPos + new Vector3(0, 0, -AA_GroundLimit))
				.IgnoreGameObjectHierarchy(GameObject)
				.WithoutTags("phys_bone_follower")
				.Run();
			var trCheck2 = Game.ActiveScene.Trace.Ray(trHitPos, trHitPos + new Vector3(0, 0, -AA_GroundLimit))
				.IgnoreGameObjectHierarchy(GameObject)
				.WithoutTags("phys_bone_follower")
				.Run();

			// base_aa.lua:134: tr_check1.Hit == true or (tr_check2.Hit == true && !tr_check2.Entity:IsNPC())
			// trCheck1: any hit triggers ground avoidance
			// trCheck2: hit must NOT be an NPC (world or prop = ground; hitting an NPC ≠ ground)
			bool hitGround1 = trCheck1.Hit;
			bool hitGround2 = trCheck2.Hit && (trCheck2.GameObject == null || trCheck2.GameObject.Components.Get<BaseNPC>() == null);
			if (hitGround1 || hitGround2)
			{
				endPos.z = (trCheck1.Hit ? myPos.z : endPos.z) + AA_GroundLimit;
				var tr2 = Game.ActiveScene.Trace.Box(extents, startPos, endPos)
					.IgnoreGameObjectHierarchy(GameObject)
					.WithoutTags("phys_bone_follower");
				if (destGO != null)
					tr2 = tr2.IgnoreGameObjectHierarchy(destGO);
				tr = tr2.Run();
				trHitPos = tr.HitPosition;
			}
		}

		// base_aa.lua:149-179 — non-vector dest: hitworld fallback + LastChasePos
		if (destVec == null)
		{
			bool hitWorld = tr.Hit && tr.GameObject == null;
			if (hitWorld)
			{
				// base_aa.lua:154-162 — already doing last chase pos
				if (AA_DoingLastChasePos)
				{
					if (AA_CurrentMoveTime < Time.Now)
					{
						AA_DoingLastChasePos = false;
						AA_LastChasePos = null;
					}
					else return; // base_aa.lua:161 — don't interrupt
				}
				// base_aa.lua:164-174 — try last chase pos
				else if (AA_LastChasePos != null)
				{
					AA_DoingLastChasePos = true;
					var tr3 = Game.ActiveScene.Trace.Box(extents, startPos, AA_LastChasePos.Value)
						.IgnoreGameObjectHierarchy(GameObject)
						.WithoutTags("phys_bone_follower");
					if (destGO != null)
						tr3 = tr3.IgnoreGameObjectHierarchy(destGO);
					tr = tr3.Run();
				}
			}
			else
			{
				// base_aa.lua:176-177
				AA_DoingLastChasePos = false;
				AA_LastChasePos = trHitPos;
			}
		}
		trHitPos = tr.HitPosition;

		// base_aa.lua:181-195 — forward blocked check
		var trDistStart = Vector3.DistanceBetween(tr.StartPosition, trHitPos);
		var finalPos = trHitPos;
		if (trDistStart <= 16 && tr.Hit && tr.GameObject == null)
		{
			finalPos = endPos;
			// base_aa.lua:192-194 — if trace actually went somewhere, cache last chase pos
			if (tr.Fraction > 0)
				AA_LastChasePos = endPos;
		}

		// base_aa.lua:196-203 — final position offset
		if (destVec != null)
		{
			finalPos = finalPos + addPos;
		}
		// SKIP: base_aa.lua:199 — WaterLevel() check missing → all aquatic NPCs use dest origin path
		// Lua: only WaterLevel()<3 takes this branch; WaterLevel()>=3 takes else (OBB center).
		// Phase 3: add WaterLevel() system, then restore `&& dest:WaterLevel() < 3` condition.
		else if (MovementType == VJMoveType.Aquatic /* Phase 3: && WaterLevel() < 3 */)
		{
			finalPos = destGO!.WorldPosition
				+ destGO!.WorldRotation.Forward * addPos.x
				+ destGO!.WorldRotation.Right * addPos.y
				+ destGO!.WorldRotation.Up * addPos.z;
		}
		else
		{
			finalPos = finalPos
				+ destGO!.WorldRotation.Forward * addPos.x
				+ destGO!.WorldRotation.Right * addPos.y
				+ destGO!.WorldRotation.Up * addPos.z;
		}

		// base_aa.lua:220-221 — set max speed + acceleration lerp
		AA_CurrentMoveMaxSpeed = moveSpeed;
		var rbMove = GameObject.Components.Get<Rigidbody>();
		if (rbMove != null && AA_MoveAccelerate > 0)
			moveSpeed = MathX.Lerp(rbMove.Velocity.Length, moveSpeed, Time.Delta * 2f);

		// base_aa.lua:224-230 — velocity + arrival time
		var velDir = (finalPos - startPos).Normal;
		var velPos = velDir * moveSpeed;
		var velTime = Vector3.DistanceBetween(finalPos, startPos) / velPos.Length;
		var velTimeCur = Time.Now + velTime;
		if (!float.IsNaN(velTimeCur))
			AA_CurrentMoveTime = velTimeCur;

		// base_aa.lua:231-241 — facing
		if (faceDest)
		{
			if (faceDestTarget)
			{
				SetTurnTarget(chaseEnemy && CanTurnWhileMoving ? "Enemy" : destGO ?? (object)dest, velTime);
			}
			else
			{
				var offsetFacing = finalPos + (finalPos - myPos).Normal * (AA_CurrentMoveMaxSpeed / 50f);
				offsetFacing.z = finalPos.z;
				SetTurnTarget(offsetFacing, velTime);
			}
		}

		// base_aa.lua:242-245 — state
		AA_CurrentMoveType = chaseEnemy ? 3 : 2;
		AA_CurrentMovePos = finalPos;
		AA_CurrentMovePosDir = finalPos - startPos;
		AA_CurrentMoveDist = -1;

		// base_aa.lua:246 — SetLocalVelocity
		if (rbMove != null) rbMove.Velocity = velPos;

		// base_aa.lua:249-256 — animation
		if (playAnim)
		{
			if (AA_CurrentMoveAnimType != moveType)
			{
				AA_CurrentMoveAnim = false;
				AA_CurrentMoveAnimType = moveType;
			}
		}
		else
		{
			AA_CurrentMoveAnim = -1;
		}
	}

	// ═══════════════════════════════════════════════
	// AA_IdleWander — base_aa.lua:267-353
	// ═══════════════════════════════════════════════
	public virtual void AA_IdleWander(bool playAnim = true, string moveType = "Calm", Dictionary<string, object> extraOptions = null)
	{
		// base_aa.lua:268-270 — defaults
		moveType ??= "Calm";
		float moveSpeed = moveType == "Calm"
			? (MovementType == VJMoveType.Aquatic ? Aquatic_SwimmingSpeed_Calm : Aerial_FlyingSpeed_Calm)
			: (MovementType == VJMoveType.Aquatic ? Aquatic_SwimmingSpeed_Alerted : Aerial_FlyingSpeed_Alerted);
		bool moveDown = false;

		// base_aa.lua:272-282 — aquatic initial check
		if (MovementType == VJMoveType.Aquatic)
		{
			// SKIP: base_aa.lua:274 — WaterLevel() < 3 check — Phase 3 water system
			// AA_StopMoving();
			// moveDown = true;
			// SKIP: base_aa.lua:277-279 — WaterLevel() == 0 → return
			moveSpeed = moveType == "Calm" ? Aquatic_SwimmingSpeed_Calm : Aquatic_SwimmingSpeed_Alerted;
		}

		bool debug = VJ_DEBUG;
		extraOptions ??= new Dictionary<string, object>();
		var ignoreGround = extraOptions.TryGetValue("IgnoreGround", out var ig) && ig is bool igb && igb;
		var faceDest = !extraOptions.TryGetValue("FaceDest", out var fd) || fd is not bool fdb || fdb;

		// base_aa.lua:288-293 — Movement Calculations — random wander position
		var myPos = GameObject.WorldPosition;
		var trFilter = new List<string> { "phys_bone_follower" };
		float myMaxsLen = OBBMaxs().Length;
		float minDist = Game.Random.Next(AA_MinWanderDist, AA_MinWanderDist + 151);
		float randSign1 = Game.Random.Next(1, 3) == 1 ? -1f : 1f;
		float randSign2 = Game.Random.Next(1, 3) == 1 ? -1f : 1f;
		float randSign3 = Game.Random.Next(1, 3) == 1 ? -1f : 1f;
		var fwd = GameObject.WorldRotation.Forward;
		var right = GameObject.WorldRotation.Right;
		var up = GameObject.WorldRotation.Up;
		var trEndpos = myPos
			+ fwd * ((myMaxsLen + minDist) * randSign1)
			+ right * ((myMaxsLen + minDist) * randSign2)
			+ up * ((myMaxsLen + minDist) * randSign3);

		// base_aa.lua:292-294 — aquatic move down override
		if (moveDown)
			trEndpos = myPos + up * ((myMaxsLen + Game.Random.Next(100, 151)) * -1f);

		// base_aa.lua:295-296 — TraceLine
		var tr = Game.ActiveScene.Trace.Ray(myPos, trEndpos)
			.IgnoreGameObjectHierarchy(GameObject)
			.WithoutTags("phys_bone_follower")
			.Run();
		var finalPos = tr.HitPosition;

		// base_aa.lua:300-313 — ground limit check (aerial, not forced down, not ignoring ground)
		if (!ignoreGround && !moveDown && MovementType == VJMoveType.Aerial)
		{
			var trCheck = Game.ActiveScene.Trace.Ray(finalPos, finalPos + new Vector3(0, 0, -AA_GroundLimit))
				.IgnoreGameObjectHierarchy(GameObject)
				.WithoutTags("phys_bone_follower")
				.Run();

			if (trCheck.Hit && trCheck.GameObject == null)
			{
				trEndpos.z = myPos.z + AA_GroundLimit;
				tr = Game.ActiveScene.Trace.Ray(myPos, trEndpos)
					.IgnoreGameObjectHierarchy(GameObject)
					.WithoutTags("phys_bone_follower")
					.Run();
				finalPos = tr.HitPosition;
			}
		}

		// base_aa.lua:322-323 — speed + acceleration
		AA_CurrentMoveMaxSpeed = moveSpeed;
		var rb = GameObject.Components.Get<Rigidbody>();
		if (rb != null && AA_MoveAccelerate > 0)
			moveSpeed = MathX.Lerp(rb.Velocity.Length, moveSpeed, Time.Delta * 2f);

		// base_aa.lua:326-332 — velocity + time
		var velPos = (finalPos - myPos).Normal * moveSpeed;
		var velTime = Vector3.DistanceBetween(finalPos, myPos) / velPos.Length;
		var velTimeCur = Time.Now + velTime;
		if (!float.IsNaN(velTimeCur))
			AA_CurrentMoveTime = velTimeCur;

		// base_aa.lua:333-337 — facing
		if (faceDest)
			SetTurnTarget(finalPos, velTime);

		// base_aa.lua:338-341 — state
		AA_CurrentMoveType = 1;
		AA_CurrentMovePos = finalPos;
		AA_CurrentMovePosDir = finalPos - myPos;
		AA_CurrentMoveDist = -1;

		// base_aa.lua:342 — SetLocalVelocity
		if (rb != null) rb.Velocity = velPos;

		// base_aa.lua:345-352 — animation
		if (playAnim)
		{
			if (AA_CurrentMoveAnimType != moveType)
			{
				AA_CurrentMoveAnim = false;
				AA_CurrentMoveAnimType = moveType;
			}
		}
		else
		{
			AA_CurrentMoveAnim = -1;
		}
	}

	// ═══════════════════════════════════════════════
	// AA_ChaseEnemy — base_aa.lua:360-365
	// ═══════════════════════════════════════════════
	public virtual bool AA_ChaseEnemy(bool playAnim = true, string moveType = "Alert")
	{
		// base_aa.lua:361 — guard clauses
		if (Dead || NextChaseTime > Time.Now) return false;
		var ene = GetEnemy();
		if (!ene.IsValid()) return false;

		// base_aa.lua:364: AA_MoveTo(ene, playAnim != false, moveType or "Alert", ...)
		// playAnim != false converts nil→true in Lua; C# default =true covers nil case.
		// For all explicit bool args (true/false), behavior is identical.
		var opts = new Dictionary<string, object>
		{
			["FaceDestTarget"] = true,
			["ChaseEnemy"] = true
		};
		AA_MoveTo(ene, playAnim, moveType, opts);
		return true;
	}

	// ═══════════════════════════════════════════════
	// AA_MoveAnimation — base_aa.lua:373-391
	// ═══════════════════════════════════════════════
	public virtual void AA_MoveAnimation()
	{
		// Phase 3: full animation system integration
		// base_aa.lua:373-391 — picks activity/sequence animations from
		// Aerial_AnimTbl_Calm / Aerial_AnimTbl_Alerted / Aquatic_AnimTbl_*,
		// handles bad ACT_* re-override behavior, plays via PlayAnim.
		// Requires: SkinnedModelRenderer, TranslateActivity, PlayAnim, ACT_* constants.
	}

}
