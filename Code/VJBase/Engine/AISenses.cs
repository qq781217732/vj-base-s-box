using System;
using System.Collections.Generic;
using Sandbox;
using SWB.Player;

namespace VJBase;

// ═══════════════════════════════════════════════════════════
// Source C++ → C# machine translation: ai_senses.cpp
// Repository: ValveSoftware/source-sdk-2013 master
// Path: sp/src/game/server/ai_senses.cpp
//
// Architecture:
//   Engine/AISenses.cs = perception layer (produces conditions)
//   Core/BaseNPC.cs = translation layer (INPCConditions stores, Enemy Management consumes)
//   Lua 翻译层调用 Cond.SetCondition()，Engine 层负责把条件生产出来
// ═══════════════════════════════════════════════════════════

// ═══ Search interval constants — ai_senses.cpp:33-36 ═══
public static class AISensesTimings
{
	public const float StandardNPCSearchTime = 0.25f;
	public const float EfficientNPCSearchTime = 0.35f;
	public const float HighPrioritySearchTime = 0.15f;
	public const float MiscSearchTime = 0.45f;
}

// ═══ Sensing flags — ai_senses.h ═══
public static class SensingFlags
{
	public const int DontLook = 0x01;
	public const int DontListen = 0x02;
}

// ═══ Seen entity categories — ai_senses.h: seentype_t ═══
public enum SeenType
{
	SeenAll = 0,
	SeenHighPriority = 1,
	SeenNPC = 2,
	SeenMisc = 3
}

// ═══ Sound types — from soundent.h / Source engine ═══
public enum SoundType
{
	None = 0,
	Combat = 1,
	World = 2,
	Player = 4,
	Danger = 8,
	BulletImpact = 16,
	Thumper = 32,
	Bugbait = 64,
	PhysicsDanger = 128,
	MoveAway = 256
}

// ═══ Sound priority — from soundent.h ═══
public enum SoundPriority
{
	VeryLow = 0,
	Low = 1,
	Medium = 2,
	High = 3,
	VeryHigh = 4
}

// ═══ SoundEvent — CSound stub, Phase 3 ═══
// Source: CSound in soundent.h — linked list of active sounds
public class SoundEvent
{
	public GameObject Owner { get; set; }
	public Vector3 Origin { get; set; }
	public float Volume { get; set; }
	public SoundType Type { get; set; }
	public SoundPriority Priority { get; set; }
	public int NextAudible { get; set; } = SoundSystem.SoundListEmpty;

	public bool IsSound() => true;
	public bool IsScent() => false;

	public bool IsSoundType(int validTypes) => ((int)Type & validTypes) != 0;

	public Vector3 GetSoundOrigin() => Origin;
}

// ═══ SoundSystem — CSoundEnt stub, Phase 3 ═══
// Source: CSoundEnt in soundent.h — global active sound list
public static class SoundSystem
{
	public const int SoundListEmpty = -1;

	private static readonly List<SoundEvent> _activeSounds = new();
	private static int _nextIndex;

	/// <summary>CSoundEnt::ActiveList() — returns first sound index</summary>
	public static int ActiveList()
	{
		if (_activeSounds.Count == 0) return SoundListEmpty;
		return 0;
	}

	/// <summary>CSoundEnt::SoundPointerForIndex(int)</summary>
	public static SoundEvent SoundPointerForIndex(int index)
	{
		if (index < 0 || index >= _activeSounds.Count) return null;
		return _activeSounds[index];
	}

	/// <summary>Emit a new sound into the world</summary>
	public static int EmitSound(GameObject owner, Vector3 origin, float volume, SoundType type, SoundPriority priority = SoundPriority.Medium)
	{
		var snd = new SoundEvent
		{
			Owner = owner,
			Origin = origin,
			Volume = volume,
			Type = type,
			Priority = priority
		};
		_activeSounds.Add(snd);
		return _activeSounds.Count - 1;
	}

	/// <summary>Remove a sound by index</summary>
	public static void RemoveSound(int index)
	{
		if (index >= 0 && index < _activeSounds.Count)
			_activeSounds.RemoveAt(index);
	}

	/// <summary>CSound::NextSound() — returns index of next sound in active list</summary>
	public static int NextSound(int currentIndex)
	{
		int next = currentIndex + 1;
		return next < _activeSounds.Count ? next : SoundListEmpty;
	}

	/// <summary>Phase 3: age-based expiration, like NoiseSystem</summary>
	public static void ExpireOldSounds(float maxAge = 3f) { /* Phase 3 */ }
}

// ═══════════════════════════════════════════════════════════
// AISenses — CAI_Senses translated from ai_senses.cpp
// ═══════════════════════════════════════════════════════════
public class AISenses
{
	// ═══ DataDesc fields — ai_senses.cpp:67-83 ═══
	public float LookDist { get; set; }              // m_LookDist
	public float LastLookDist { get; private set; }   // m_LastLookDist
	public float TimeLastLook { get; private set; }   // m_TimeLastLook
	public int SensingFlagBits { get; set; }             // m_iSensingFlags

	// Seen entity cache — CUtlVector<EHANDLE> arrays
	public List<GameObject> SeenHighPriority { get; private set; } = new();  // m_SeenHighPriority
	public List<GameObject> SeenNPCs { get; private set; } = new();          // m_SeenNPCs
	public List<GameObject> SeenMisc { get; private set; } = new();          // m_SeenMisc
	// Sensed objects manager — ai_senses.cpp:673: g_AI_SensedObjectsManager
	public AISensedObjectsManager SensedObjectsManager { get; private set; } = new();

	// Time-of-last-update per category
	public float TimeLastLookHighPriority { get; private set; }  // m_TimeLastLookHighPriority
	public float TimeLastLookNPCs { get; private set; }          // m_TimeLastLookNPCs
	public float TimeLastLookMisc { get; private set; }          // m_TimeLastLookMisc

	// Hearing
	private int _audibleList = SoundSystem.SoundListEmpty;  // m_iAudibleList
	public int AudibleList => _audibleList;

	// SeenArrays convenience — maps to m_SeenArrays[3] in C++
	private List<GameObject>[] SeenArrays => new[] { SeenHighPriority, SeenNPCs, SeenMisc };

	// ═══ Outer NPC — GetOuter() in Source engine ═══
	public BaseNPC Outer { get; set; }

	// ═══ Seen entity tracking — replaces m_pLink intrusive linked list ═══
	private readonly List<GameObject> _gatherList = new();

	// ═══════════════════════════════════════════════════════════
	// Hearing — ai_senses.cpp:88-145
	// ═══════════════════════════════════════════════════════════

	/// <summary>CanHearSound — ai_senses.cpp:88</summary>
	public virtual bool CanHearSound(SoundEvent pSound)
	{
		// ai_senses.cpp:90
		if (pSound.Owner == Outer?.GameObject)
			return false;

		// ai_senses.cpp:92-98 — skip hearing danger in script state
		if (Outer != null && (NPCState)Outer.GetNPCState() == NPCState.None /* NPC_STATE_SCRIPT placeholder — Phase 3 */)
		{
			if (pSound.IsSoundType((int)SoundType.Danger))
				return false;
		}

		// ai_senses.cpp:100 — skip hearing while in script
		// Phase 3: IsInAScript check

		// ai_senses.cpp:104-108
		float hearDist = pSound.Volume * (Outer?.HearingSensitivity() ?? 1f);
		if (Vector3.DistanceBetween(pSound.GetSoundOrigin(), EarPosition()) <= hearDist)
		{
			return Outer?.QueryHearSound(pSound) ?? true;
		}

		return false;
	}

	/// <summary>Listen — ai_senses.cpp:119</summary>
	public virtual void Listen()
	{
		// ai_senses.cpp:121
		_audibleList = SoundSystem.SoundListEmpty;

		// ai_senses.cpp:123
		int iSoundMask = Outer?.GetSoundInterests() ?? 0;

		// ai_senses.cpp:125
		if (iSoundMask != (int)SoundType.None && !(Outer?.HasSpawnFlag(256 /*SF_NPC_WAIT_TILL_SEEN*/) ?? false))
		{
			// ai_senses.cpp:127
			int iSound = SoundSystem.ActiveList();

			// ai_senses.cpp:129
			while (iSound != SoundSystem.SoundListEmpty)
			{
				// ai_senses.cpp:131
				SoundEvent pCurrentSound = SoundSystem.SoundPointerForIndex(iSound);

				// ai_senses.cpp:133
				if (pCurrentSound != null
					&& (iSoundMask & (int)pCurrentSound.Type) != 0
					&& CanHearSound(pCurrentSound))
				{
					// ai_senses.cpp:136 — the npc cares about this sound
					pCurrentSound.NextAudible = _audibleList;
					_audibleList = iSound;
				}

				// ai_senses.cpp:140
				iSound = SoundSystem.NextSound(iSound);
			}
		}

		// ai_senses.cpp:144
		Outer?.OnListened();
	}

	// ═══════════════════════════════════════════════════════════
	// Vision pipeline — ai_senses.cpp:149-248
	// ═══════════════════════════════════════════════════════════

	/// <summary>ShouldSeeEntity — ai_senses.cpp:149</summary>
	public virtual bool ShouldSeeEntity(GameObject pSightEnt)
	{
		// ai_senses.cpp:151
		if (pSightEnt == Outer?.GameObject || !EntityAlive(pSightEnt))
			return false;

		// ai_senses.cpp:154 — FL_NOTARGET check
		// Phase 3: flag system
		if (EntityIsPlayer(pSightEnt) && HasEntityFlag(pSightEnt, BaseNPC.FL_NOTARGET_BIT))
			return false;

		// ai_senses.cpp:158 — SF_NPC_WAIT_TILL_SEEN
		// Phase 3: spawn flag system
		if (HasSpawnFlag(pSightEnt, 256 /*SF_NPC_WAIT_TILL_SEEN*/))
			return false;

		// ai_senses.cpp:161 — CanBeSeenBy callback
		if (!CanBeSeenBy(pSightEnt))
			return false;

		// ai_senses.cpp:164 — QuerySeeEntity callback
		if (Outer != null && !Outer.QuerySeeEntity(pSightEnt, true))
			return false;

		return true;
	}

	/// <summary>CanSeeEntity — ai_senses.cpp:172</summary>
	// Source: return GetOuter()->FInViewCone(pSightEnt) && GetOuter()->FVisible(pSightEnt);
	public virtual bool CanSeeEntity(GameObject pSightEnt)
	{
		return FInViewCone(pSightEnt) && FVisible(pSightEnt);
	}

	/// <summary>DidSeeEntity — ai_senses.cpp:186</summary>
	public bool DidSeeEntity(GameObject pSightEnt)
	{
		// Iterate all seen arrays
		for (int arr = 0; arr < SeenArrays.Length; arr++)
		{
			foreach (var ent in SeenArrays[arr])
			{
				if (ent == pSightEnt)
					return true;
			}
		}
		return false;
	}

	/// <summary>NoteSeenEntity — ai_senses.cpp:204</summary>
	// Source: intrusive linked list via m_pLink
	// S&Box: add to gather list (used during BeginGather/EndGather cycle)
	public void NoteSeenEntity(GameObject pSightEnt)
	{
		_gatherList.Add(pSightEnt);
	}

	/// <summary>WaitingUntilSeen — ai_senses.cpp:212</summary>
	public virtual bool WaitingUntilSeen(GameObject pSightEnt)
	{
		// ai_senses.cpp:214
		if (Outer != null && Outer.HasSpawnFlag(256 /*SF_NPC_WAIT_TILL_SEEN*/))
		{
			// ai_senses.cpp:216
			if (EntityIsPlayer(pSightEnt))
			{
				// ai_senses.cpp:219 — don't link if player isn't facing NPC
				Vector3 zero = Vector3.Zero;
				// ai_senses.cpp:222-224
				if (FInViewCone_Reverse(pSightEnt, Outer.GameObject)
					&& FBoxVisible(pSightEnt, Outer.GameObject, zero))
				{
					// ai_senses.cpp:227 — player sees us, become normal
					Outer.ClearSpawnFlag(256 /*SF_NPC_WAIT_TILL_SEEN*/);
					return false;
				}
			}
			// ai_senses.cpp:231
			return true;
		}

		// ai_senses.cpp:234
		return false;
	}

	/// <summary>SeeEntity — ai_senses.cpp:239</summary>
	public virtual bool SeeEntity(GameObject pSightEnt)
	{
		// ai_senses.cpp:241
		Outer?.OnSeeEntity(pSightEnt);

		// ai_senses.cpp:244 — insert at head of sight list
		NoteSeenEntity(pSightEnt);

		return true;
	}

	// ═══════════════════════════════════════════════════════════
	// Seen entity iteration — ai_senses.cpp:251-303
	// ═══════════════════════════════════════════════════════════

	// AISightIter_t — (array, iNext, SeenArray) tuple, replaces C++ union
	private (int array, int next, SeenType seenType) _sightIter;

	/// <summary>GetFirstSeenEntity — ai_senses.cpp:251</summary>
	public GameObject GetFirstSeenEntity(SeenType iSeenType = SeenType.SeenAll)
	{
		_sightIter.seenType = iSeenType;
		int iFirstArray = (iSeenType == SeenType.SeenAll) ? 0 : (int)iSeenType;

		for (int i = iFirstArray; i < SeenArrays.Length; i++)
		{
			if (SeenArrays[i].Count != 0)
			{
				_sightIter.array = i;
				_sightIter.next = 1;
				return SeenArrays[i][0];
			}
		}

		_sightIter = (-1, 0, SeenType.SeenAll);
		return null;
	}

	/// <summary>GetNextSeenEntity — ai_senses.cpp:277</summary>
	public GameObject GetNextSeenEntity()
	{
		if (_sightIter.array != -1)
		{
			for (int i = _sightIter.array; i < SeenArrays.Length; i++)
			{
				for (int j = _sightIter.next; j < SeenArrays[i].Count; j++)
				{
					if (SeenArrays[i][j].IsValid())
					{
						_sightIter.array = i;
						_sightIter.next = j + 1;
						return SeenArrays[i][j];
					}
				}
				_sightIter.next = 0;

				// If searching for a specific type, don't move to next array
				if (_sightIter.seenType != SeenType.SeenAll)
					break;
			}
			_sightIter = (-1, 0, SeenType.SeenAll);
		}
		return null;
	}

	/// <summary>BeginGather — ai_senses.cpp:307</summary>
	// Source: clears m_pLink (intrusive linked list head)
	// S&Box: clears our gather list
	public void BeginGather()
	{
		_gatherList.Clear();
	}

	/// <summary>EndGather — ai_senses.cpp:315</summary>
	// Source: copies m_pLink chain into pResult CUtlVector
	public void EndGather(List<GameObject> pResult)
	{
		pResult.Clear();
		if (_gatherList.Count > 0)
		{
			pResult.AddRange(_gatherList);
		}
		_gatherList.Clear();
	}

	// ═══════════════════════════════════════════════════════════
	// Look (vision scanning) — ai_senses.cpp:332-388
	// ═══════════════════════════════════════════════════════════

	/// <summary>Look(int) — ai_senses.cpp:343</summary>
	// Base class npc function to find enemies or food by sight.
	// iDistance is distance (in units) that the npc can see.
	// Sets sight bits of m_afConditions mask to indicate which types were sighted.
	public virtual void Look(int iDistance)
	{
		// ai_senses.cpp:345 — cache check: skip if same frame + same distance
		float curTime = Time.Now;
		if (TimeLastLook != curTime || LastLookDist != iDistance)
		{
			// ai_senses.cpp:349-351
			LookForHighPriorityEntities(iDistance);
			LookForNPCs(iDistance);
			LookForObjects(iDistance);

			// ai_senses.cpp:355-356
			LastLookDist = iDistance;
			TimeLastLook = curTime;
		}

		// ai_senses.cpp:359
		Outer?.OnLooked(iDistance);
	}

	/// <summary>Look(CBaseEntity*) — ai_senses.cpp:364</summary>
	// Single-entity sight check
	public virtual bool Look(GameObject pSightEnt)
	{
		// ai_senses.cpp:366
		if (WaitingUntilSeen(pSightEnt))
			return false;

		// ai_senses.cpp:369
		if (ShouldSeeEntity(pSightEnt) && CanSeeEntity(pSightEnt))
		{
			return SeeEntity(pSightEnt);
		}
		return false;
	}

	// ═══════════════════════════════════════════════════════════
	// LookFor* — ai_senses.cpp:392-550
	// ═══════════════════════════════════════════════════════════

	/// <summary>LookForHighPriorityEntities — ai_senses.cpp:392</summary>
	public virtual int LookForHighPriorityEntities(int iDistance)
	{
		int nSeen = 0;
		float curTime = Time.Now;

		// ai_senses.cpp:395
		if (curTime - TimeLastLookHighPriority > AISensesTimings.HighPrioritySearchTime)
		{
			TimeLastLookHighPriority = curTime;

			BeginGather();

				Vector3 origin = GetAbsOrigin();

			// ai_senses.cpp:406 — scan all players
			// Source: for (int i = 1; i <= gpGlobals->maxClients; i++)
			// S&Box: GetAllComponents<PlayerBase>() → each player has one
			var players = Game.ActiveScene.GetAllComponents<PlayerBase>()
				.Select(p => p.GameObject);

			foreach (var pPlayer in players)
			{
				if (pPlayer == null || !pPlayer.IsValid()) continue;

				if (Vector3.DistanceBetween(origin, GetAbsOrigin_Entity(pPlayer)) < iDistance
					&& Look(pPlayer))
				{
					nSeen++;
				}
			}

			EndGather(SeenHighPriority);
		}
		else
		{
			// ai_senses.cpp:433 — use cache, clean stale
			for (int i = SeenHighPriority.Count - 1; i >= 0; --i)
			{
				if (SeenHighPriority[i] == null || !SeenHighPriority[i].IsValid())
					SeenHighPriority.RemoveAt(i);
			}
			nSeen = SeenHighPriority.Count;
		}

		return nSeen;
	}

	/// <summary>LookForNPCs — ai_senses.cpp:446</summary>
	public virtual int LookForNPCs(int iDistance)
	{
		bool bRemoveStaleFromCache = false;
		Vector3 origin = GetAbsOrigin();
		float curTime = Time.Now;

		// ai_senses.cpp:451 — efficiency-based timing
		// Phase 3: AI_Efficiency_t — for now use standard time
		float timeNPCs = AISensesTimings.StandardNPCSearchTime;

		if (curTime - TimeLastLookNPCs > timeNPCs)
		{
			TimeLastLookNPCs = curTime;

			int nSeen = 0;
			BeginGather();

			// ai_senses.cpp:465-467 — iterate all AI-managed NPCs
			// Source: g_AI_Manager.AccessAIs()
			// S&Box: iterate scene NPCs
			var npcs = Game.ActiveScene.GetAllComponents<BaseNPC>()
				.Where(n => n.GameObject != Outer?.GameObject)
				.Select(n => n.GameObject);

			foreach (var pNPC in npcs)
			{
				if (pNPC == null || !pNPC.IsValid()) continue;

				// ai_senses.cpp:469 — distance cull or within range
				if (Vector3.DistanceBetween(origin, GetAbsOrigin_Entity(pNPC)) < iDistance)
				{
					if (Look(pNPC))
					{
						nSeen++;
					}
				}
			}

			EndGather(SeenNPCs);
			return nSeen;
		}

		// ai_senses.cpp:487 — use cache, clean stale
		for (int i = SeenNPCs.Count - 1; i >= 0; --i)
		{
			var ent = SeenNPCs[i];
			if (ent == null || !ent.IsValid())
			{
				SeenNPCs.RemoveAt(i);
			}
			else if (bRemoveStaleFromCache)
			{
				if (Vector3.DistanceBetween(origin, GetAbsOrigin_Entity(ent)) > iDistance
					|| !Look(ent))
				{
					SeenNPCs.RemoveAt(i);
				}
			}
		}

		return SeenNPCs.Count;
	}

	/// <summary>LookForObjects — ai_senses.cpp:509</summary>
	public virtual int LookForObjects(int iDistance)
	{
		int nSeen = 0;
		float curTime = Time.Now;

		if (curTime - TimeLastLookMisc > AISensesTimings.MiscSearchTime)
		{
			TimeLastLookMisc = curTime;

			BeginGather();
			// ai_senses.cpp:524-535 — iterate g_AI_SensedObjectsManager
			// S&Box: FL_OBJECT membership implicit via SensedObjectsManager (non-player, non-NPC entities)
			float distSq = iDistance * iDistance;
			Vector3 origin = GetAbsOrigin();
			var pEnt = SensedObjectsManager.GetFirst();
			while (pEnt != null)
			{
				if (Vector3.DistanceBetweenSquared(origin, GetAbsOrigin_Entity(pEnt)) < distSq && Look(pEnt))
				{
					nSeen++;
				}
				pEnt = SensedObjectsManager.GetNext();
			}

			EndGather(SeenMisc);
		}
		else
		{
			// ai_senses.cpp:541 — use cache, clean stale
			for (int i = SeenMisc.Count - 1; i >= 0; --i)
			{
				if (SeenMisc[i] == null || !SeenMisc[i].IsValid())
					SeenMisc.RemoveAt(i);
			}
			nSeen = SeenMisc.Count;
		}

		return nSeen;
	}

	// ═══════════════════════════════════════════════════════════
	// Time query — ai_senses.cpp:554-563
	// ═══════════════════════════════════════════════════════════

	/// <summary>GetTimeLastUpdate — ai_senses.cpp:554</summary>
	public float GetTimeLastUpdate(GameObject pEntity)
	{
		if (pEntity == null || !pEntity.IsValid())
			return 0;
		if (EntityIsPlayer(pEntity))
			return TimeLastLookHighPriority;
		if (EntityIsNPC(pEntity))
			return TimeLastLookNPCs;
		return TimeLastLookMisc;
	}

	// ═══════════════════════════════════════════════════════════
	// Heard sound iteration — ai_senses.cpp:567-649
	// ═══════════════════════════════════════════════════════════

	/// <summary>GetFirstHeardSound — ai_senses.cpp:567</summary>
	public SoundEvent GetFirstHeardSound()
	{
		int iFirst = _audibleList;

		if (iFirst == SoundSystem.SoundListEmpty)
		{
			return null;
		}

		return SoundSystem.SoundPointerForIndex(iFirst);
	}

	/// <summary>GetNextHeardSound — ai_senses.cpp:583</summary>
	public SoundEvent GetNextHeardSound(ref int currentIndex)
	{
		if (currentIndex == SoundSystem.SoundListEmpty)
			return null;

		SoundEvent pCurrent = SoundSystem.SoundPointerForIndex(currentIndex);
		if (pCurrent == null)
		{
			currentIndex = SoundSystem.SoundListEmpty;
			return null;
		}

		currentIndex = pCurrent.NextAudible;
		if (currentIndex == SoundSystem.SoundListEmpty)
			return null;

		return SoundSystem.SoundPointerForIndex(currentIndex);
	}

	/// <summary>GetClosestSound — ai_senses.cpp:610</summary>
	public virtual SoundEvent GetClosestSound(bool fScent, int validTypes, bool bUsePriority = false)
	{
		// ai_senses.cpp:612
		float flBestDist = float.MaxValue;
		float flDist;
		int iBestPriority = (int)SoundPriority.VeryLow;

		SoundEvent pResult = null;
		Vector3 earPosition = EarPosition();

		// ai_senses.cpp:618
		int iter = _audibleList;
		SoundEvent pCurrent = GetFirstHeardSound();

		// ai_senses.cpp:623
		while (pCurrent != null)
		{
			// ai_senses.cpp:625-626
			if ((!fScent && pCurrent.IsSound()) ||
				 (fScent && pCurrent.IsScent()))
			{
				// ai_senses.cpp:628
				if (pCurrent.IsSoundType(validTypes)
					&& (Outer == null || !Outer.ShouldIgnoreSound(pCurrent)))
				{
					// ai_senses.cpp:630
					if (!bUsePriority || (Outer?.GetSoundPriority(pCurrent) ?? 0) >= iBestPriority)
					{
						// ai_senses.cpp:632
						flDist = (pCurrent.GetSoundOrigin() - earPosition).LengthSquared;

						// ai_senses.cpp:634
						if (flDist < flBestDist)
						{
							pResult = pCurrent;
							flBestDist = flDist;

							// ai_senses.cpp:639
							iBestPriority = Outer?.GetSoundPriority(pCurrent) ?? 0;
						}
					}
				}
			}

			// ai_senses.cpp:644
			iter = pCurrent.NextAudible;
			pCurrent = GetNextHeardSound(ref iter);
		}

		return pResult;
	}

	// ═══════════════════════════════════════════════════════════
	// PerformSensing — ai_senses.cpp:653
	// ═══════════════════════════════════════════════════════════

	/// <summary>PerformSensing — ai_senses.cpp:653</summary>
	// Master tick: runs Look + Listen each frame
	public virtual void PerformSensing()
	{
		// ai_senses.cpp:660
		if (!HasSensingFlags(SensingFlags.DontLook))
			Look((int)LookDist);

		// ai_senses.cpp:666
		if (!HasSensingFlags(SensingFlags.DontListen))
			Listen();
	}

	// ═══════════════════════════════════════════════════════════
	// Engine API helpers — replaces CBaseEntity methods called by CAI_Senses
	// These are the "M" (Source engine) methods that must be hand-rolled.
	// Phase 3: unify with BaseNPC Senses hooks / Enemy Management
	// ═══════════════════════════════════════════════════════════

	/// <summary>FInViewCone — CBaseEntity::FInViewCone</summary>
	// Checks whether pTarget is within the NPC's vision cone (FOV angle check).
	// Source engine uses this internally; we implement via dot product.
	protected virtual bool FInViewCone(GameObject pTarget)
	{
		if (Outer == null || pTarget == null || !pTarget.IsValid())
			return false;

		var myEye = EarPosition();
		var targetPos = GetAbsOrigin_Entity(pTarget); // Source uses target center, not eye

		var dirToTarget = (targetPos - myEye).Normal;
		var forward = Outer.GameObject.WorldRotation.Forward.Normal;

		// Default FOV: SightAngle from BaseNPC config (default 156°)
		float sightAngle = Outer.SightAngle;
		float dot = Vector3.Dot(forward, dirToTarget);
		float halfAngleRad = MathF.PI * sightAngle / 360f;

		return dot >= MathF.Cos(halfAngleRad);
	}

	/// <summary>FInViewCone_Reverse — check if pViewer can see pTarget</summary>
	// Used by WaitingUntilSeen: checks if PLAYER sees the NPC
	protected virtual bool FInViewCone_Reverse(GameObject pViewer, GameObject pTarget)
	{
		if (pViewer == null || pTarget == null || !pViewer.IsValid() || !pTarget.IsValid())
			return false;

		var viewerEye = GetEyePos_Entity(pViewer);
		var targetPos = GetAbsOrigin_Entity(pTarget);
		var dirToTarget = (targetPos - viewerEye).Normal;
		var forward = pViewer.WorldRotation.Forward.Normal;

		float dot = Vector3.Dot(forward, dirToTarget);
		// 90° FOV — WaitingUntilSeen checks if player can see the NPC, not vice versa.
		// Typical player FOV is ~90°; narrower than NPC's SightAngle (default 156°).
		return dot >= MathF.Cos(MathF.PI * 90f / 360f);
	}

	/// <summary>FVisible — CBaseEntity::FVisible</summary>
	// Ray trace from NPC eye to target to check line-of-sight.
	protected virtual bool FVisible(GameObject pTarget)
	{
		if (Outer == null || pTarget == null || !pTarget.IsValid())
			return false;

		var myEye = EarPosition();
		var targetEye = GetEyePos_Entity(pTarget);

		var tr = Game.ActiveScene.Trace.Ray(myEye, targetEye)
			.WithoutTags("npc")
			.IgnoreGameObjectHierarchy(Outer.GameObject)
			.Run();

		return !tr.Hit || tr.GameObject == pTarget;
	}

	/// <summary>FBoxVisible — CBaseEntity::FBoxVisible</summary>
	// Checks if pTarget's bounding box is visible from pViewer.
	// Phase 3: proper box visibility check with multiple trace points.
	// For now, simplified to single ray trace.
	protected virtual bool FBoxVisible(GameObject pViewer, GameObject pTarget, Vector3 zero)
	{
		if (pViewer == null || pTarget == null) return false;
		var viewerEye = GetEyePos_Entity(pViewer);
		var targetCenter = GetAbsOrigin_Entity(pTarget);
		var tr = Game.ActiveScene.Trace.Ray(viewerEye, targetCenter)
			.IgnoreGameObjectHierarchy(pViewer)
			.Run();
		return !tr.Hit || tr.GameObject == pTarget;
	}

	// ═══════════════════════════════════════════════════════════
	// Utility helpers
	// ═══════════════════════════════════════════════════════════

	/// <summary>GetAbsOrigin — equivalent to CBaseEntity::GetAbsOrigin()</summary>
	protected Vector3 GetAbsOrigin()
	{
		return Outer?.GameObject?.WorldPosition ?? Vector3.Zero;
	}

	protected Vector3 GetAbsOrigin_Entity(GameObject ent)
	{
		return ent?.WorldPosition ?? Vector3.Zero;
	}

	/// <summary>EarPosition — equivalent to CBaseEntity::EarPosition()</summary>
	protected Vector3 EarPosition()
	{
		if (Outer == null) return Vector3.Zero;
		return Outer.EyePosition();
	}

	/// <summary>Eye position for any entity</summary>
	protected Vector3 GetEyePos_Entity(GameObject ent)
	{
		if (ent == null) return Vector3.Zero;
			var baseNPC = ent.Components.Get<BaseNPC>();
			float viewOffset = baseNPC?.ViewOffset ?? 64f;
			// Phase 3: use model attachment "eyes" or player ViewOffset
			return ent.WorldPosition + Vector3.Up * viewOffset;
	}

	protected bool HasSensingFlags(int flags) => (SensingFlagBits & flags) != 0;

	protected bool EntityIsPlayer(GameObject ent)
		=> ent != null && ent.Tags.Has("player");

	protected bool EntityIsNPC(GameObject ent)
		=> ent != null && ent.Tags.Has("npc");

	protected bool EntityAlive(GameObject ent)
	{
		// Phase 3: IDamageable integration
		return ent != null && ent.IsValid();
	}

	// ═══ Flag stubs — Phase 3: proper flag system ═══
	protected bool HasEntityFlag(GameObject ent, int flag)
	{
		var npc = ent?.Components.Get<BaseNPC>();
		return npc != null && npc.HasEntityFlag(ent, flag);
	}
	protected bool HasSpawnFlag(GameObject ent, int flag)
	{
		var npc = ent?.Components.Get<BaseNPC>();
		return npc != null && npc.HasSpawnFlag(flag);
	}

	/// <summary>CanBeSeenBy callback — Phase 3</summary>
	protected virtual bool CanBeSeenBy(GameObject pSightEnt)
	{
		// Phase 3: delegate to entity's CanBeSeenBy method
		return true;
	}
}

// ═══════════════════════════════════════════════════════════
// CAI_SensedObjectsManager — ai_senses.cpp:673-755
// Tracks FL_OBJECT entities for NPC sensing (physics props, etc.)
// Phase 3: implement properly with S&Box entity lifecycle hooks
// ═══════════════════════════════════════════════════════════
public class AISensedObjectsManager
{
	private readonly List<GameObject> _sensedObjects = new();
	private int _iterIndex;

	/// <summary>Init — ai_senses.cpp:673</summary>
	public void Init()
	{
		// ai_senses.cpp:675-684 — iterate gEntList, add FL_OBJECT entities
		// S&Box: scan Rigidbody components as proxy for physics objects (FL_OBJECT ≈ non-character physics entity)
		_sensedObjects.Clear();
		foreach (var rb in Game.ActiveScene.GetAllComponents<Rigidbody>())
		{
			var obj = rb.GameObject;
			if (obj.Tags.Has("player") || obj.Tags.Has("npc")) continue;
			if (!_sensedObjects.Contains(obj))
				_sensedObjects.Add(obj);
		}
		}

	/// <summary>Term — ai_senses.cpp:686</summary>
	public void Term()
	{
		_sensedObjects.Clear();
	}

	/// <summary>GetFirst — ai_senses.cpp:694</summary>
	public GameObject GetFirst()
	{
		if (_sensedObjects.Count > 0)
		{
			_iterIndex = 1;
			return _sensedObjects[0];
		}
		_iterIndex = 0;
		return null;
	}

	/// <summary>GetNext — ai_senses.cpp:708</summary>
	public GameObject GetNext()
	{
		int i = _iterIndex;
		if (i > 0 && i < _sensedObjects.Count)
		{
			_iterIndex++;
			return _sensedObjects[i];
		}
		_iterIndex = 0;
		return null;
	}

	/// <summary>OnEntitySpawned — ai_senses.cpp:723</summary>
	// Source: adds to list if FL_OBJECT && !IsPlayer() && !IsNPC()
	public void OnEntitySpawned(GameObject pEntity)
	{
		if (pEntity == null) return;
		// Phase 3: FL_OBJECT flag check
		if (!pEntity.Tags.Has("player") && !pEntity.Tags.Has("npc"))
		{
			_sensedObjects.Add(pEntity);
		}
	}

	/// <summary>OnEntityDeleted — ai_senses.cpp:733</summary>
	public void OnEntityDeleted(GameObject pEntity)
	{
		_sensedObjects.Remove(pEntity);
	}

	/// <summary>AddEntity — ai_senses.cpp:743</summary>
	public void AddEntity(GameObject pEntity)
	{
		if (_sensedObjects.Contains(pEntity))
			return;
		_sensedObjects.Add(pEntity);
	}
}
