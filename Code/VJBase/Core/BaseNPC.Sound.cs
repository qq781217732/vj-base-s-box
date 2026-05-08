using System;
using System.Collections.Generic;
using Sandbox;

namespace VJBase;

/// <summary>
/// Sound system — ported from vj_base/ai/core.lua ENT:PlaySoundSystem (line 2944-3375)
/// + footstep/idle/breath sound helpers. All sound config fields live on BaseNPC.
/// </summary>
public partial class BaseNPC
{
    // ═══════════════════════════════════════════════
    // Sound Config — Has* flags (core.lua + entity init.lua)
    // ═══════════════════════════════════════════════

    public bool HasSounds { get; set; } = true;

    // Footstep
    public bool HasFootstepSounds { get; set; } = true;
    public bool DisableFootStepSoundTimer { get; set; }

    // Breath
    public bool HasBreathSound { get; set; } = true;

    // Idle
    public bool HasIdleSounds { get; set; } = true;
    public bool IdleSoundsWhileAttacking { get; set; }
    public bool HasIdleDialogueSounds { get; set; } = true;
    public bool HasIdleDialogueAnswerSounds { get; set; } = true;

    // Event / Speech
    public bool HasAlertSounds { get; set; } = true;
    public bool HasDeathSounds { get; set; } = true;
    public bool HasPainSounds { get; set; } = true;
    public bool HasImpactSounds { get; set; } = true;
    public bool HasLostEnemySounds { get; set; } = true;
    public bool HasInvestigateSounds { get; set; } = true;
    public bool HasOnPlayerSightSounds { get; set; } = true;
    public bool HasCallForHelpSounds { get; set; } = true;
    public bool HasBecomeEnemyToPlayerSounds { get; set; } = true;
    public bool HasKilledEnemySounds { get; set; } = true;
    public bool HasAllyDeathSounds { get; set; } = true;
    public bool HasDamageByPlayerSounds { get; set; } = true;
    public bool HasMeleeAttackSounds { get; set; } = true;
    public bool HasExtraMeleeAttackSounds { get; set; }
    public bool HasMeleeAttackMissSounds { get; set; } = true;
    public bool HasRangeAttackSounds { get; set; } = true;
    public bool HasBeforeLeapAttackSounds { get; set; } = true;
    public bool HasLeapAttackJumpSounds { get; set; } = true;
    public bool HasLeapAttackDamageSounds { get; set; } = true;
    public bool HasLeapAttackDamageMissSounds { get; set; } = true;
    public bool HasMedicSounds { get; set; } = true;
    public bool HasReceiveOrderSounds { get; set; } = true;
    public bool HasFollowPlayerSounds { get; set; } = true;
    public bool HasYieldToPlayerSounds { get; set; } = true;
    public bool HasGibOnDeathSounds { get; set; } = true;
    public bool HasSoundTrack { get; set; }

    // Human-only (default false on BaseNPC, overridden in HumanNPC constructor)
    public bool HasSuppressingSounds { get; set; }
    public bool HasWeaponReloadSounds { get; set; }
    public bool HasGrenadeAttackSounds { get; set; }
    public bool HasDangerSightSounds { get; set; }

    // ═══════════════════════════════════════════════
    // Sound Config — SoundTbl_* (List<string> of sound event names)
    // Lua false → C# null (no sounds configured)
    // ═══════════════════════════════════════════════

    public List<string> SoundTbl_Breath { get; set; }
    public List<string> SoundTbl_Idle { get; set; }
    public List<string> SoundTbl_IdleDialogue { get; set; }
    public List<string> SoundTbl_IdleDialogueAnswer { get; set; }
    public List<string> SoundTbl_CombatIdle { get; set; }
    public List<string> SoundTbl_ReceiveOrder { get; set; }
    public List<string> SoundTbl_FollowPlayer { get; set; }
    public List<string> SoundTbl_UnFollowPlayer { get; set; }
    public List<string> SoundTbl_YieldToPlayer { get; set; }
    public List<string> SoundTbl_MedicBeforeHeal { get; set; }
    public List<string> SoundTbl_MedicOnHeal { get; set; } = new() { "items/smallmedkit1.wav" };
    public List<string> SoundTbl_MedicReceiveHeal { get; set; }
    public List<string> SoundTbl_OnPlayerSight { get; set; }
    public List<string> SoundTbl_Investigate { get; set; }
    public List<string> SoundTbl_LostEnemy { get; set; }
    public List<string> SoundTbl_Alert { get; set; }
    public List<string> SoundTbl_CallForHelp { get; set; }
    public List<string> SoundTbl_BecomeEnemyToPlayer { get; set; }
    public List<string> SoundTbl_BeforeMeleeAttack { get; set; }
    public List<string> SoundTbl_MeleeAttack { get; set; }
    public List<string> SoundTbl_MeleeAttackExtra { get; set; } = new() { "Zombie.AttackHit" };
    public List<string> SoundTbl_MeleeAttackMiss { get; set; }
    public List<string> SoundTbl_BeforeRangeAttack { get; set; }
    public List<string> SoundTbl_RangeAttack { get; set; }
    public List<string> SoundTbl_BeforeLeapAttack { get; set; }
    public List<string> SoundTbl_LeapAttackJump { get; set; }
    public List<string> SoundTbl_LeapAttackDamage { get; set; }
    public List<string> SoundTbl_LeapAttackDamageMiss { get; set; }
    public List<string> SoundTbl_KilledEnemy { get; set; }
    public List<string> SoundTbl_AllyDeath { get; set; }
    public List<string> SoundTbl_Pain { get; set; }
    public List<string> SoundTbl_Impact { get; set; } = new() { "VJ.Impact.Flesh_Alien" };
    public List<string> SoundTbl_DamageByPlayer { get; set; }
    public List<string> SoundTbl_Death { get; set; }
    public List<string> SoundTbl_Suppressing { get; set; }
    public List<string> SoundTbl_WeaponReload { get; set; }
    public List<string> SoundTbl_GrenadeAttack { get; set; }
    public List<string> SoundTbl_DangerSight { get; set; }
    public List<string> SoundTbl_GrenadeSight { get; set; }
    public List<string> SoundTbl_FootStep { get; set; }
    public List<string> SoundTbl_MeleeAttackPlayerSpeed { get; set; } = new() { "vj_base/player/heartbeat_loop.wav" };
    public List<string> SoundTbl_SoundTrack { get; set; }

    // ═══════════════════════════════════════════════
    // Sound Config — *SoundChance (higher = less likely)
    // ═══════════════════════════════════════════════

    public int IdleSoundChance { get; set; } = 2;
    public int IdleDialogueAnswerSoundChance { get; set; } = 1;
    public int CombatIdleSoundChance { get; set; } = 1;
    public int ReceiveOrderSoundChance { get; set; } = 1;
    public int FollowPlayerSoundChance { get; set; } = 1;
    public int YieldToPlayerSoundChance { get; set; } = 2;
    public int MedicBeforeHealSoundChance { get; set; } = 1;
    public int MedicOnHealSoundChance { get; set; } = 1;
    public int MedicReceiveHealSoundChance { get; set; } = 1;
    public int OnPlayerSightSoundChance { get; set; } = 1;
    public int InvestigateSoundChance { get; set; } = 1;
    public int LostEnemySoundChance { get; set; } = 1;
    public int AlertSoundChance { get; set; } = 1;
    public int CallForHelpSoundChance { get; set; } = 1;
    public int BecomeEnemyToPlayerChance { get; set; } = 1;
    public int BeforeMeleeAttackSoundChance { get; set; } = 1;
    public int MeleeAttackSoundChance { get; set; } = 1;
    public int ExtraMeleeSoundChance { get; set; } = 1;
    public int MeleeAttackMissSoundChance { get; set; } = 1;
    public int BeforeRangeAttackSoundChance { get; set; } = 1;
    public int RangeAttackSoundChance { get; set; } = 1;
    public int BeforeLeapAttackSoundChance { get; set; } = 1;
    public int LeapAttackJumpSoundChance { get; set; } = 1;
    public int LeapAttackDamageSoundChance { get; set; } = 1;
    public int LeapAttackDamageMissSoundChance { get; set; } = 1;
    public int KilledEnemySoundChance { get; set; } = 1;
    public int AllyDeathSoundChance { get; set; } = 4;
    public int PainSoundChance { get; set; } = 1;
    public int ImpactSoundChance { get; set; } = 1;
    public int DamageByPlayerSoundChance { get; set; } = 1;
    public int DeathSoundChance { get; set; } = 1;
    public int SuppressingSoundChance { get; set; } = 2;
    public int WeaponReloadSoundChance { get; set; } = 1;
    public int GrenadeAttackSoundChance { get; set; } = 1;
    public int DangerSightSoundChance { get; set; } = 1;

    // ═══════════════════════════════════════════════
    // Sound Config — *SoundLevel (0-180 dB scale)
    // ═══════════════════════════════════════════════

    public int FootstepSoundLevel { get; set; } = 70;
    public int BreathSoundLevel { get; set; } = 60;
    public int IdleSoundLevel { get; set; } = 75;
    public int IdleDialogueSoundLevel { get; set; } = 75;
    public int CombatIdleSoundLevel { get; set; } = 80;
    public int ReceiveOrderSoundLevel { get; set; } = 80;
    public int FollowPlayerSoundLevel { get; set; } = 75;
    public int YieldToPlayerSoundLevel { get; set; } = 75;
    public int MedicBeforeHealSoundLevel { get; set; } = 75;
    public int MedicOnHealSoundLevel { get; set; } = 75;
    public int MedicReceiveHealSoundLevel { get; set; } = 75;
    public int OnPlayerSightSoundLevel { get; set; } = 75;
    public int InvestigateSoundLevel { get; set; } = 80;
    public int LostEnemySoundLevel { get; set; } = 75;
    public int AlertSoundLevel { get; set; } = 80;
    public int CallForHelpSoundLevel { get; set; } = 80;
    public int BecomeEnemyToPlayerSoundLevel { get; set; } = 75;
    public int BeforeMeleeAttackSoundLevel { get; set; } = 75;
    public int MeleeAttackSoundLevel { get; set; } = 75;
    public int ExtraMeleeAttackSoundLevel { get; set; } = 75;
    public int MeleeAttackMissSoundLevel { get; set; } = 75;
    public int BeforeRangeAttackSoundLevel { get; set; } = 75;
    public int RangeAttackSoundLevel { get; set; } = 75;
    public int BeforeLeapAttackSoundLevel { get; set; } = 75;
    public int LeapAttackJumpSoundLevel { get; set; } = 75;
    public int LeapAttackDamageSoundLevel { get; set; } = 75;
    public int LeapAttackDamageMissSoundLevel { get; set; } = 75;
    public int KilledEnemySoundLevel { get; set; } = 80;
    public int AllyDeathSoundLevel { get; set; } = 80;
    public int PainSoundLevel { get; set; } = 80;
    public int ImpactSoundLevel { get; set; } = 60;
    public int DamageByPlayerSoundLevel { get; set; } = 75;
    public int DeathSoundLevel { get; set; } = 80;
    public int SuppressingSoundLevel { get; set; } = 80;
    public int WeaponReloadSoundLevel { get; set; } = 80;
    public int GrenadeAttackSoundLevel { get; set; } = 80;
    public int DangerSightSoundLevel { get; set; } = 80;

    // ═══════════════════════════════════════════════
    // Sound Config — *SoundPitch / *Pitch (object: null, float, or (float,float))
    // Lua false → C# null (use MainSoundPitch)
    // ═══════════════════════════════════════════════

    public object MainSoundPitch { get; set; } = ((float a, float b))(90f, 100f);
    public bool MainSoundPitchStatic { get; set; } = true;
    public object FootstepSoundPitch { get; set; } = ((float a, float b))(80f, 100f);
    public object BreathSoundPitch { get; set; } = 100f;
    public object IdleSoundPitch { get; set; }
    public object IdleDialogueSoundPitch { get; set; }
    public object CombatIdleSoundPitch { get; set; }
    public object ReceiveOrderSoundPitch { get; set; }
    public object FollowPlayerPitch { get; set; }
    public object YieldToPlayerSoundPitch { get; set; }
    public object MedicBeforeHealSoundPitch { get; set; }
    public object MedicOnHealSoundPitch { get; set; } = 100f;
    public object MedicReceiveHealSoundPitch { get; set; }
    public object OnPlayerSightSoundPitch { get; set; }
    public object InvestigateSoundPitch { get; set; }
    public object LostEnemySoundPitch { get; set; }
    public object AlertSoundPitch { get; set; }
    public object CallForHelpSoundPitch { get; set; }
    public object BecomeEnemyToPlayerPitch { get; set; }
    public object BeforeMeleeAttackSoundPitch { get; set; }
    public object MeleeAttackSoundPitch { get; set; }
    public object ExtraMeleeSoundPitch { get; set; } = ((float a, float b))(80f, 100f);
    public object MeleeAttackMissSoundPitch { get; set; } = ((float a, float b))(90f, 100f);
    public object BeforeRangeAttackPitch { get; set; }
    public object RangeAttackPitch { get; set; }
    public object BeforeLeapAttackSoundPitch { get; set; }
    public object LeapAttackJumpSoundPitch { get; set; }
    public object LeapAttackDamageSoundPitch { get; set; }
    public object LeapAttackDamageMissSoundPitch { get; set; }
    public object KilledEnemySoundPitch { get; set; }
    public object AllyDeathSoundPitch { get; set; }
    public object PainSoundPitch { get; set; }
    public object ImpactSoundPitch { get; set; } = ((float a, float b))(80f, 100f);
    public object DamageByPlayerPitch { get; set; }
    public object DeathSoundPitch { get; set; }
    public object SuppressingPitch { get; set; }
    public object WeaponReloadSoundPitch { get; set; }
    public object GrenadeAttackSoundPitch { get; set; }
    public object DangerSightSoundPitch { get; set; }

    // ═══════════════════════════════════════════════
    // Sound Config — NextSoundTime_* cooldown ranges
    // ═══════════════════════════════════════════════

    public (float a, float b) NextSoundTime_Idle { get; set; } = (4f, 11f);
    public (float a, float b) NextSoundTime_Investigate { get; set; } = (5f, 5f);
    public (float a, float b) NextSoundTime_LostEnemy { get; set; } = (5f, 6f);
    public (float a, float b) NextSoundTime_Alert { get; set; } = (2f, 3f);
    public (float a, float b) NextSoundTime_KilledEnemy { get; set; } = (3f, 5f);
    public (float a, float b) NextSoundTime_AllyDeath { get; set; } = (3f, 5f);
    public (float a, float b) NextSoundTime_Suppressing { get; set; } = (7f, 15f);
    public float NextSoundTime_Breath { get; set; } = 10f;

    // ═══════════════════════════════════════════════
    // Miscellaneous sound config
    // ═══════════════════════════════════════════════

    public int IdleDialogueDistance { get; set; } = 400;
    public float FootstepSoundTimerWalk { get; set; } = 1f;
    public float FootstepSoundTimerRun { get; set; } = 0.5f;
    public float SoundTrackVolume { get; set; } = 1f;
    public float SoundTrackPlaybackRate { get; set; } = 1f;

    // ═══════════════════════════════════════════════
    // Missing timers (used by PlaySoundSystem)
    // ═══════════════════════════════════════════════

    public float NextGrenadeAttackSoundT { get; set; }
    public float NextSuppressingSoundT { get; set; }
    public float NextDangerSightSoundT { get; set; }

    // ═══════════════════════════════════════════════
    // Runtime sound handle state
    // ═══════════════════════════════════════════════

    public SoundHandle CurrentSpeechSound { get; set; }
    public SoundHandle CurrentExtraSpeechSound { get; set; }
    public SoundHandle CurrentIdleSound { get; set; }
    public SoundHandle CurrentMeleeAttackMissSound { get; set; }
    public SoundHandle CurrentImpactSound { get; set; }
    public SoundHandle CurrentLeapAttackDamageMissSound { get; set; }
    public SoundHandle CurrentMedicAfterHealSound { get; set; }
    public SoundHandle CurrentBreathSound { get; set; }

    // ═══════════════════════════════════════════════
    // Sound helpers — core.lua:66, funcs.lua:70-98
    // ═══════════════════════════════════════════════

    /// <summary>StopSD — core.lua:66 → VJ.STOPSOUND (funcs.lua:70)</summary>
    protected static void StopSD(SoundHandle sd)
    {
        if (sd is { IsValid: true })
            sd.Stop();
    }

    /// <summary>PickSound — wrapper around VJ.PICK for sound tables, handles null/empty</summary>
    protected static string PickSound(List<string> tbl)
    {
        if (tbl == null || tbl.Count == 0) return null;
        return VJUtility.PICK<string>(tbl);
    }

    /// <summary>
    /// CreateSound — funcs.lua:74-87 VJ.CreateSound.
    /// Creates a sound attached to this GameObject, sets pitch, returns handle.
    /// NOTE: Uses Sound.Play() + manual Parent binding. S&Box also has GameObject.PlaySound()
    /// which auto-parents; if that API is stable, switching to it would be cleaner (Phase 3).
    /// </summary>
    protected SoundHandle CreateSound(string sdFile, int sdLevel, float sdPitch)
    {
        if (string.IsNullOrEmpty(sdFile)) return null;
        sdFile = OnPlaySound(sdFile);
        if (string.IsNullOrEmpty(sdFile)) return null;

        var handle = Sound.Play(sdFile);
        if (handle is { IsValid: true })
        {
            handle.Parent = GameObject;
            handle.Pitch = sdPitch;
            // Phase 3: sdLevel (dB) → SoundHandle.Distance / Decibels mapping
            OnCreateSound(handle, sdFile);
        }
        return handle;
    }

    /// <summary>
    /// EmitSound — funcs.lua:89-98 VJ.EmitSound.
    /// Fire-and-forget sound at entity position. Returns handle for optional storage/stopping.
    /// </summary>
    protected SoundHandle EmitSound(string sdFile, int sdLevel, float sdPitch)
    {
        if (string.IsNullOrEmpty(sdFile)) return null;
        sdFile = OnPlaySound(sdFile);
        if (string.IsNullOrEmpty(sdFile)) return null;

        var handle = Sound.Play(sdFile, GameObject.WorldPosition);
        if (handle is { IsValid: true })
        {
            handle.Pitch = sdPitch;
            // Phase 3: sdLevel (dB) → SoundHandle.Distance / Decibels mapping
        }
        OnEmitSound(sdFile);
        return handle;
    }

    /// <summary>GetSoundPitch — core.lua:940-961. Resolves pitchVar to a float.</summary>
    public virtual float GetSoundPitch(object pitchVar = null)
    {
        return pitchVar switch
        {
            float f => f,
            ValueTuple<float, float> range => VJUtility.Rand(range.Item1, range.Item2),
            _ => ResolveMainPitch(),
        };
    }

    private float ResolveMainPitch()
    {
        if (MainSoundPitchStatic && MainSoundPitchValue > 0)
            return MainSoundPitchValue;
        return MainSoundPitch switch
        {
            ValueTuple<float, float> range => VJUtility.Rand(range.Item1, range.Item2),
            float f => f,
            _ => 100f,
        };
    }

    /// <summary>SoundDuration fallback — S&Box has no SoundDuration(). Returns hardcoded defaults per sdSet.</summary>
    private static float GetSoundDuration(string sdSet)
    {
        return sdSet switch
        {
            "IdleDialogueAnswer" => 0f,
            "OnPlayerSight" => 3.5f,
            "Alert" => 2f,
            "BecomeEnemyToPlayer" => 2f,
            "Pain" => 2f,
            "DamageByPlayer" => 2f,
            "WeaponReload" => 3.5f,
            "DangerSight" => 3f,
            "GrenadeSight" => 3f,
            "Speech" => 2f,
            _ => 0f,
        };
    }

    // ═══════════════════════════════════════════════
    // Callbacks — core.lua equivalents
    // ═══════════════════════════════════════════════

    /// <summary>Called before every sound play. Return modified sound name, or null to cancel.</summary>
    public virtual string OnPlaySound(string sdFile) => sdFile;

    /// <summary>Called after CreateSound creates a handle.</summary>
    public virtual void OnCreateSound(SoundHandle sd, string sdFile) { }

    /// <summary>Called after EmitSound plays a fire-and-forget sound.</summary>
    public virtual void OnEmitSound(string sdFile) { }

    // ═══════════════════════════════════════════════
    // StopAllSounds — stops all active sound handles
    // ═══════════════════════════════════════════════

    public virtual void StopAllSounds()
    {
        StopSD(CurrentSpeechSound);
        StopSD(CurrentExtraSpeechSound);
        StopSD(CurrentIdleSound);
        StopSD(CurrentBreathSound);
        StopSD(CurrentMeleeAttackMissSound);
        StopSD(CurrentImpactSound);
        StopSD(CurrentLeapAttackDamageMissSound);
        StopSD(CurrentMedicAfterHealSound);
    }

    // ═══════════════════════════════════════════════
    // PlaySoundSystem — core.lua:2944-3375
    // Centralized sound dispatch. Returns sound duration (0 = no sound played).
    // sdType: null → CreateSound (default); non-null → use provided func
    //
    // DESIGN NOTE (Lua→C# type narrowing):
    //   Lua customSD can be string OR table (PICK randomly from table).
    //   C# customSD is string only. Callers must PickSound() before passing.
    //   The Lua StartsWith("{") table-string hack has been removed.
    //
    // DESIGN NOTE (Random chance):
    //   Lua math.random(1, 1) returns 1 (safe).
    //   .NET Random.Next(1, 1) throws. All calls use Next(1, chance + 1)
    //   to match Lua's inclusive-max semantics without throwing.
    // ═══════════════════════════════════════════════

    public virtual float PlaySoundSystem(string sdSet, string customSD = null,
        Func<string, int, float, SoundHandle> sdType = null)
    {
        if (!HasSounds || string.IsNullOrEmpty(sdSet)) return 0f;

        var curTime = Time.Now;

        // core.lua:2951 — IdleDialogueAnswer
        if (sdSet == "IdleDialogueAnswer")
        {
            if (HasIdleDialogueAnswerSounds)
            {
                var pickedSD = PickSound(SoundTbl_IdleDialogueAnswer);
                if ((pickedSD != null && Game.Random.Next(1, IdleDialogueAnswerSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentExtraSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + Game.Random.Next(2, 4);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, IdleDialogueSoundLevel, GetSoundPitch(IdleDialogueSoundPitch))
                        : CreateSound(pickedSD, IdleDialogueSoundLevel, GetSoundPitch(IdleDialogueSoundPitch)));
                    return GetSoundDuration("IdleDialogueAnswer");
                }
                return 0f;
            }
            return 0f;
        }
        // core.lua:2966 — FollowPlayer
        else if (sdSet == "FollowPlayer")
        {
            if (HasFollowPlayerSounds)
            {
                var pickedSD = PickSound(SoundTbl_FollowPlayer);
                if ((pickedSD != null && Game.Random.Next(1, FollowPlayerSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + Game.Random.Next(3, 5);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, FollowPlayerSoundLevel, GetSoundPitch(FollowPlayerPitch))
                        : CreateSound(pickedSD, FollowPlayerSoundLevel, GetSoundPitch(FollowPlayerPitch)));
                }
            }
        }
        // core.lua:2977 — UnFollowPlayer
        else if (sdSet == "UnFollowPlayer")
        {
            if (HasFollowPlayerSounds)
            {
                var pickedSD = PickSound(SoundTbl_UnFollowPlayer);
                if ((pickedSD != null && Game.Random.Next(1, FollowPlayerSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + Game.Random.Next(3, 5);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, FollowPlayerSoundLevel, GetSoundPitch(FollowPlayerPitch))
                        : CreateSound(pickedSD, FollowPlayerSoundLevel, GetSoundPitch(FollowPlayerPitch)));
                }
            }
        }
        // core.lua:2988 — ReceiveOrder
        else if (sdSet == "ReceiveOrder")
        {
            if (HasReceiveOrderSounds)
            {
                var pickedSD = PickSound(SoundTbl_ReceiveOrder);
                if ((pickedSD != null && Game.Random.Next(1, ReceiveOrderSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    NextIdleSoundT += 2;
                    NextAlertSoundT = curTime + 2;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, ReceiveOrderSoundLevel, GetSoundPitch(ReceiveOrderSoundPitch))
                        : CreateSound(pickedSD, ReceiveOrderSoundLevel, GetSoundPitch(ReceiveOrderSoundPitch)));
                }
            }
        }
        // core.lua:3000 — YieldToPlayer
        else if (sdSet == "YieldToPlayer")
        {
            if (HasYieldToPlayerSounds)
            {
                var pickedSD = PickSound(SoundTbl_YieldToPlayer);
                if ((pickedSD != null && Game.Random.Next(1, YieldToPlayerSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + Game.Random.Next(3, 5);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, YieldToPlayerSoundLevel, GetSoundPitch(YieldToPlayerSoundPitch))
                        : CreateSound(pickedSD, YieldToPlayerSoundLevel, GetSoundPitch(YieldToPlayerSoundPitch)));
                }
            }
        }
        // core.lua:3011 — MedicBeforeHeal
        else if (sdSet == "MedicBeforeHeal")
        {
            if (HasMedicSounds)
            {
                var pickedSD = PickSound(SoundTbl_MedicBeforeHeal);
                if ((pickedSD != null && Game.Random.Next(1, MedicBeforeHealSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + Game.Random.Next(3, 5);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, MedicBeforeHealSoundLevel, GetSoundPitch(MedicBeforeHealSoundPitch))
                        : CreateSound(pickedSD, MedicBeforeHealSoundLevel, GetSoundPitch(MedicBeforeHealSoundPitch)));
                }
            }
        }
        // core.lua:3022 — MedicOnHeal
        else if (sdSet == "MedicOnHeal")
        {
            if (HasMedicSounds)
            {
                var pickedSD = PickSound(SoundTbl_MedicOnHeal);
                if ((pickedSD != null && Game.Random.Next(1, MedicOnHealSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    IdleSoundBlockTime = curTime + Game.Random.Next(3, 5);
                    CurrentMedicAfterHealSound = (sdType != null ? sdType(pickedSD, MedicOnHealSoundLevel, GetSoundPitch(MedicOnHealSoundPitch))
                        : EmitSound(pickedSD, MedicOnHealSoundLevel, GetSoundPitch(MedicOnHealSoundPitch)));
                }
            }
        }
        // core.lua:3031 — MedicReceiveHeal
        else if (sdSet == "MedicReceiveHeal")
        {
            if (HasMedicSounds)
            {
                var pickedSD = PickSound(SoundTbl_MedicReceiveHeal);
                if ((pickedSD != null && Game.Random.Next(1, MedicReceiveHealSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + Game.Random.Next(3, 5);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, MedicReceiveHealSoundLevel, GetSoundPitch(MedicReceiveHealSoundPitch))
                        : CreateSound(pickedSD, MedicReceiveHealSoundLevel, GetSoundPitch(MedicReceiveHealSoundPitch)));
                }
            }
        }
        // core.lua:3042 — OnPlayerSight
        else if (sdSet == "OnPlayerSight")
        {
            if (HasOnPlayerSightSounds)
            {
                var pickedSD = PickSound(SoundTbl_OnPlayerSight);
                if ((pickedSD != null && Game.Random.Next(1, OnPlayerSightSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    var dur = curTime + GetSoundDuration("OnPlayerSight") + 1;
                    IdleSoundBlockTime = dur;
                    NextAlertSoundT = curTime + Game.Random.Next(1, 3);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, OnPlayerSightSoundLevel, GetSoundPitch(OnPlayerSightSoundPitch))
                        : CreateSound(pickedSD, OnPlayerSightSoundLevel, GetSoundPitch(OnPlayerSightSoundPitch)));
                }
            }
        }
        // core.lua:3055 — Investigate
        else if (sdSet == "Investigate")
        {
            if (HasInvestigateSounds && curTime > NextInvestigateSoundT)
            {
                var pickedSD = PickSound(SoundTbl_Investigate);
                if ((pickedSD != null && Game.Random.Next(1, InvestigateSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    NextIdleSoundT += 2;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, InvestigateSoundLevel, GetSoundPitch(InvestigateSoundPitch))
                        : CreateSound(pickedSD, InvestigateSoundLevel, GetSoundPitch(InvestigateSoundPitch)));
                }
                NextInvestigateSoundT = curTime + VJUtility.Rand(NextSoundTime_Investigate.a, NextSoundTime_Investigate.b);
            }
        }
        // core.lua:3067 — LostEnemy
        else if (sdSet == "LostEnemy")
        {
            if (HasLostEnemySounds && curTime > NextLostEnemySoundT)
            {
                var pickedSD = PickSound(SoundTbl_LostEnemy);
                if ((pickedSD != null && Game.Random.Next(1, LostEnemySoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    NextIdleSoundT += 2;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, LostEnemySoundLevel, GetSoundPitch(LostEnemySoundPitch))
                        : CreateSound(pickedSD, LostEnemySoundLevel, GetSoundPitch(LostEnemySoundPitch)));
                }
                NextLostEnemySoundT = curTime + VJUtility.Rand(NextSoundTime_LostEnemy.a, NextSoundTime_LostEnemy.b);
            }
        }
        // core.lua:3079 — Alert
        else if (sdSet == "Alert")
        {
            if (HasAlertSounds)
            {
                var pickedSD = PickSound(SoundTbl_Alert);
                if ((pickedSD != null && Game.Random.Next(1, AlertSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    var dur = curTime + GetSoundDuration("Alert") + 1;
                    NextIdleSoundT = dur;
                    NextPainSoundT = dur;
                    NextSuppressingSoundT = curTime + 4;
                    NextAlertSoundT = curTime + VJUtility.Rand(NextSoundTime_Alert.a, NextSoundTime_Alert.b);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, AlertSoundLevel, GetSoundPitch(AlertSoundPitch))
                        : CreateSound(pickedSD, AlertSoundLevel, GetSoundPitch(AlertSoundPitch)));
                }
            }
        }
        // core.lua:3094 — CallForHelp
        else if (sdSet == "CallForHelp")
        {
            if (HasCallForHelpSounds)
            {
                var pickedSD = PickSound(SoundTbl_CallForHelp);
                if ((pickedSD != null && Game.Random.Next(1, CallForHelpSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    NextIdleSoundT += 2;
                    NextSuppressingSoundT = curTime + VJUtility.Rand(2.5f, 4f); // Lua math.random(2.5,4) is float
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, CallForHelpSoundLevel, GetSoundPitch(CallForHelpSoundPitch))
                        : CreateSound(pickedSD, CallForHelpSoundLevel, GetSoundPitch(CallForHelpSoundPitch)));
                }
            }
        }
        // core.lua:3106 — BeforeMeleeAttack
        else if (sdSet == "BeforeMeleeAttack")
        {
            if (HasMeleeAttackSounds)
            {
                var pickedSD = PickSound(SoundTbl_BeforeMeleeAttack);
                if ((pickedSD != null && Game.Random.Next(1, BeforeMeleeAttackSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentExtraSpeechSound);
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentExtraSpeechSound = (sdType != null ? sdType(pickedSD, BeforeMeleeAttackSoundLevel, GetSoundPitch(BeforeMeleeAttackSoundPitch))
                        : CreateSound(pickedSD, BeforeMeleeAttackSoundLevel, GetSoundPitch(BeforeMeleeAttackSoundPitch)));
                }
            }
        }
        // core.lua:3118 — MeleeAttack
        else if (sdSet == "MeleeAttack")
        {
            if (HasMeleeAttackSounds)
            {
                var pickedSD = PickSound(SoundTbl_MeleeAttack);
                if ((pickedSD != null && Game.Random.Next(1, MeleeAttackSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, MeleeAttackSoundLevel, GetSoundPitch(MeleeAttackSoundPitch))
                        : CreateSound(pickedSD, MeleeAttackSoundLevel, GetSoundPitch(MeleeAttackSoundPitch)));
                }
                // core.lua:3128 — extra melee attack sounds
                if (HasExtraMeleeAttackSounds)
                {
                    var extraSD = PickSound(SoundTbl_MeleeAttackExtra);
                    if ((extraSD != null && Game.Random.Next(1, ExtraMeleeSoundChance + 1) == 1) || customSD != null)
                    {
                        if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                        EmitSound(customSD ?? extraSD, ExtraMeleeAttackSoundLevel, GetSoundPitch(ExtraMeleeSoundPitch));
                    }
                }
            }
        }
        // core.lua:3136 — MeleeAttackMiss
        else if (sdSet == "MeleeAttackMiss")
        {
            if (HasMeleeAttackMissSounds)
            {
                var pickedSD = PickSound(SoundTbl_MeleeAttackMiss);
                if ((pickedSD != null && Game.Random.Next(1, MeleeAttackMissSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    StopSD(CurrentMeleeAttackMissSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentMeleeAttackMissSound = (sdType != null ? sdType(pickedSD, MeleeAttackMissSoundLevel, GetSoundPitch(MeleeAttackMissSoundPitch))
                        : EmitSound(pickedSD, MeleeAttackMissSoundLevel, GetSoundPitch(MeleeAttackMissSoundPitch)));
                }
            }
        }
        // core.lua:3147 — BecomeEnemyToPlayer
        else if (sdSet == "BecomeEnemyToPlayer")
        {
            if (HasBecomeEnemyToPlayerSounds)
            {
                var pickedSD = PickSound(SoundTbl_BecomeEnemyToPlayer);
                if ((pickedSD != null && Game.Random.Next(1, BecomeEnemyToPlayerChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    var dur = curTime + GetSoundDuration("BecomeEnemyToPlayer") + 1;
                    NextPainSoundT = dur;
                    NextAlertSoundT = dur;
                    NextInvestigateSoundT = curTime + 2;
                    IdleSoundBlockTime = curTime + Game.Random.Next(2, 4);
                    NextSuppressingSoundT = curTime + VJUtility.Rand(2.5f, 4f); // Lua math.random(2.5,4) is float
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, BecomeEnemyToPlayerSoundLevel, GetSoundPitch(BecomeEnemyToPlayerPitch))
                        : CreateSound(pickedSD, BecomeEnemyToPlayerSoundLevel, GetSoundPitch(BecomeEnemyToPlayerPitch)));
                }
            }
        }
        // core.lua:3163 — KilledEnemy
        else if (sdSet == "KilledEnemy")
        {
            if (HasKilledEnemySounds && curTime > NextKilledEnemySoundT)
            {
                var pickedSD = PickSound(SoundTbl_KilledEnemy);
                if ((pickedSD != null && Game.Random.Next(1, KilledEnemySoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    NextIdleSoundT += 2;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, KilledEnemySoundLevel, GetSoundPitch(KilledEnemySoundPitch))
                        : CreateSound(pickedSD, KilledEnemySoundLevel, GetSoundPitch(KilledEnemySoundPitch)));
                }
                NextKilledEnemySoundT = curTime + VJUtility.Rand(NextSoundTime_KilledEnemy.a, NextSoundTime_KilledEnemy.b);
            }
        }
        // core.lua:3175 — AllyDeath
        else if (sdSet == "AllyDeath")
        {
            if (HasKilledEnemySounds && curTime > NextAllyDeathSoundT)
            {
                var pickedSD = PickSound(SoundTbl_AllyDeath);
                if ((pickedSD != null && Game.Random.Next(1, AllyDeathSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    NextIdleSoundT += 2;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, AllyDeathSoundLevel, GetSoundPitch(AllyDeathSoundPitch))
                        : CreateSound(pickedSD, AllyDeathSoundLevel, GetSoundPitch(AllyDeathSoundPitch)));
                }
                NextAllyDeathSoundT = curTime + VJUtility.Rand(NextSoundTime_AllyDeath.a, NextSoundTime_AllyDeath.b);
            }
        }
        // core.lua:3187 — Pain
        else if (sdSet == "Pain")
        {
            if (HasPainSounds && curTime > NextPainSoundT)
            {
                var pickedSD = PickSound(SoundTbl_Pain);
                var sdDur = GetSoundDuration("Pain");
                if ((pickedSD != null && Game.Random.Next(1, PainSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, PainSoundLevel, GetSoundPitch(PainSoundPitch))
                        : CreateSound(pickedSD, PainSoundLevel, GetSoundPitch(PainSoundPitch)));
                }
                NextPainSoundT = curTime + sdDur;
            }
        }
        // core.lua:3201 — Impact
        else if (sdSet == "Impact")
        {
            if (HasImpactSounds)
            {
                var pickedSD = PickSound(SoundTbl_Impact);
                if ((pickedSD != null && Game.Random.Next(1, ImpactSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    CurrentImpactSound = EmitSound(pickedSD, ImpactSoundLevel, GetSoundPitch(ImpactSoundPitch));
                }
            }
        }
        // core.lua:3209 — DamageByPlayer
        else if (sdSet == "DamageByPlayer")
        {
            if (HasDamageByPlayerSounds)
            {
                var pickedSD = PickSound(SoundTbl_DamageByPlayer);
                var sdDur = GetSoundDuration("DamageByPlayer");
                if ((pickedSD != null && Game.Random.Next(1, DamageByPlayerSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    NextPainSoundT = curTime + sdDur;
                    IdleSoundBlockTime = curTime + sdDur;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, DamageByPlayerSoundLevel, GetSoundPitch(DamageByPlayerPitch))
                        : CreateSound(pickedSD, DamageByPlayerSoundLevel, GetSoundPitch(DamageByPlayerPitch)));
                }
                NextDamageByPlayerSoundT = curTime + sdDur;
            }
        }
        // core.lua:3224 — Death
        else if (sdSet == "Death")
        {
            if (HasDeathSounds)
            {
                var pickedSD = PickSound(SoundTbl_Death);
                if ((pickedSD != null && Game.Random.Next(1, DeathSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    EmitSound(pickedSD, DeathSoundLevel, GetSoundPitch(DeathSoundPitch));
                }
            }
        }
        // core.lua:3232 — Gib
        else if (sdSet == "Gib")
        {
            if (HasGibOnDeathSounds)
            {
                if (customSD != null)
                {
                    EmitSound(customSD, 80, VJUtility.RandInt(80, 100));
                }
                else
                {
                    EmitSound("vj_base/gib/splat.wav", 80, VJUtility.RandInt(85, 100));
                    EmitSound("vj_base/gib/break1.wav", 80, VJUtility.RandInt(85, 100));
                    EmitSound("vj_base/gib/break2.wav", 80, VJUtility.RandInt(85, 100));
                    EmitSound("vj_base/gib/break3.wav", 80, VJUtility.RandInt(85, 100));
                }
            }
        }
        // core.lua:3244 — BeforeRangeAttack (Creature)
        else if (sdSet == "BeforeRangeAttack")
        {
            if (HasRangeAttackSounds)
            {
                var pickedSD = PickSound(SoundTbl_BeforeRangeAttack);
                if ((pickedSD != null && Game.Random.Next(1, BeforeRangeAttackSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentExtraSpeechSound);
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentExtraSpeechSound = (sdType != null ? sdType(pickedSD, BeforeRangeAttackSoundLevel, GetSoundPitch(BeforeRangeAttackPitch))
                        : CreateSound(pickedSD, BeforeRangeAttackSoundLevel, GetSoundPitch(BeforeRangeAttackPitch)));
                }
            }
        }
        // core.lua:3257 — RangeAttack (Creature)
        else if (sdSet == "RangeAttack")
        {
            if (HasRangeAttackSounds)
            {
                var pickedSD = PickSound(SoundTbl_RangeAttack);
                if ((pickedSD != null && Game.Random.Next(1, RangeAttackSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, RangeAttackSoundLevel, GetSoundPitch(RangeAttackPitch))
                        : CreateSound(pickedSD, RangeAttackSoundLevel, GetSoundPitch(RangeAttackPitch)));
                }
            }
        }
        // core.lua:3268 — BeforeLeapAttack (Creature)
        else if (sdSet == "BeforeLeapAttack")
        {
            if (HasBeforeLeapAttackSounds)
            {
                var pickedSD = PickSound(SoundTbl_BeforeLeapAttack);
                if ((pickedSD != null && Game.Random.Next(1, BeforeLeapAttackSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentExtraSpeechSound);
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentExtraSpeechSound = (sdType != null ? sdType(pickedSD, BeforeLeapAttackSoundLevel, GetSoundPitch(BeforeLeapAttackSoundPitch))
                        : CreateSound(pickedSD, BeforeLeapAttackSoundLevel, GetSoundPitch(BeforeLeapAttackSoundPitch)));
                }
            }
        }
        // core.lua:3280 — LeapAttackJump (Creature)
        else if (sdSet == "LeapAttackJump")
        {
            if (HasLeapAttackJumpSounds)
            {
                var pickedSD = PickSound(SoundTbl_LeapAttackJump);
                if ((pickedSD != null && Game.Random.Next(1, LeapAttackJumpSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, LeapAttackJumpSoundLevel, GetSoundPitch(LeapAttackJumpSoundPitch))
                        : CreateSound(pickedSD, LeapAttackJumpSoundLevel, GetSoundPitch(LeapAttackJumpSoundPitch)));
                }
            }
        }
        // core.lua:3291 — LeapAttackDamage (Creature)
        else if (sdSet == "LeapAttackDamage")
        {
            if (HasLeapAttackDamageSounds)
            {
                var pickedSD = PickSound(SoundTbl_LeapAttackDamage);
                if ((pickedSD != null && Game.Random.Next(1, LeapAttackDamageSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    StopSD(CurrentSpeechSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, LeapAttackDamageSoundLevel, GetSoundPitch(LeapAttackDamageSoundPitch))
                        : EmitSound(pickedSD, LeapAttackDamageSoundLevel, GetSoundPitch(LeapAttackDamageSoundPitch)));
                }
            }
        }
        // core.lua:3302 — LeapAttackDamageMiss (Creature)
        else if (sdSet == "LeapAttackDamageMiss")
        {
            if (HasLeapAttackDamageMissSounds)
            {
                var pickedSD = PickSound(SoundTbl_LeapAttackDamageMiss);
                if ((pickedSD != null && Game.Random.Next(1, LeapAttackDamageMissSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 1;
                    CurrentLeapAttackDamageMissSound = (sdType != null ? sdType(pickedSD, LeapAttackDamageMissSoundLevel, GetSoundPitch(LeapAttackDamageMissSoundPitch))
                        : EmitSound(pickedSD, LeapAttackDamageMissSoundLevel, GetSoundPitch(LeapAttackDamageMissSoundPitch)));
                }
            }
        }
        // core.lua:3312 — Suppressing (Human)
        else if (sdSet == "Suppressing")
        {
            if (HasSuppressingSounds && curTime > NextSuppressingSoundT)
            {
                var pickedSD = PickSound(SoundTbl_Suppressing);
                if ((pickedSD != null && Game.Random.Next(1, SuppressingSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + 2;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, SuppressingSoundLevel, GetSoundPitch(SuppressingPitch))
                        : CreateSound(pickedSD, SuppressingSoundLevel, GetSoundPitch(SuppressingPitch)));
                }
                NextSuppressingSoundT = curTime + VJUtility.Rand(NextSoundTime_Suppressing.a, NextSoundTime_Suppressing.b);
            }
        }
        // core.lua:3325 — WeaponReload (Human)
        else if (sdSet == "WeaponReload")
        {
            if (HasWeaponReloadSounds)
            {
                var pickedSD = PickSound(SoundTbl_WeaponReload);
                if ((pickedSD != null && Game.Random.Next(1, WeaponReloadSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + GetSoundDuration("WeaponReload");
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, WeaponReloadSoundLevel, GetSoundPitch(WeaponReloadSoundPitch))
                        : CreateSound(pickedSD, WeaponReloadSoundLevel, GetSoundPitch(WeaponReloadSoundPitch)));
                }
            }
        }
        // core.lua:3336 — GrenadeAttack (Human)
        else if (sdSet == "GrenadeAttack")
        {
            if (HasGrenadeAttackSounds && curTime > NextGrenadeAttackSoundT)
            {
                var pickedSD = PickSound(SoundTbl_GrenadeAttack);
                if ((pickedSD != null && Game.Random.Next(1, GrenadeAttackSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    if (!IdleSoundsWhileAttacking) StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + Game.Random.Next(3, 5);
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, GrenadeAttackSoundLevel, GetSoundPitch(GrenadeAttackSoundPitch))
                        : CreateSound(pickedSD, GrenadeAttackSoundLevel, GetSoundPitch(GrenadeAttackSoundPitch)));
                }
            }
        }
        // core.lua:3347 — DangerSight / GrenadeSight (Human)
        else if (sdSet == "DangerSight" || sdSet == "GrenadeSight")
        {
            if (HasDangerSightSounds && curTime > NextDangerSightSoundT)
            {
                var pickedSD = PickSound(SoundTbl_DangerSight);
                if (sdSet == "GrenadeSight")
                {
                    var grenSDs = PickSound(SoundTbl_GrenadeSight);
                    if (grenSDs != null) pickedSD = grenSDs;
                }
                var sdDur = GetSoundDuration(sdSet);
                if ((pickedSD != null && Game.Random.Next(1, DangerSightSoundChance + 1) == 1) || customSD != null)
                {
                    if (customSD != null) pickedSD = customSD;
                    StopSD(CurrentSpeechSound);
                    StopSD(CurrentIdleSound);
                    IdleSoundBlockTime = curTime + sdDur;
                    CurrentSpeechSound = (sdType != null ? sdType(pickedSD, DangerSightSoundLevel, GetSoundPitch(DangerSightSoundPitch))
                        : CreateSound(pickedSD, DangerSightSoundLevel, GetSoundPitch(DangerSightSoundPitch)));
                }
                NextDangerSightSoundT = curTime + sdDur;
            }
        }
        // core.lua:3367 — else (Speech fallback)
        else
        {
            if (customSD != null)
            {
                StopSD(CurrentSpeechSound);
                StopSD(CurrentIdleSound);
                IdleSoundBlockTime = curTime + GetSoundDuration("Speech") + 1;
                CurrentSpeechSound = (sdType != null ? sdType(customSD, 80, GetSoundPitch(false))
                    : CreateSound(customSD, 80, GetSoundPitch(false)));
            }
        }

        return 0f;
    }

}
