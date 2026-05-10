using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace VJBase;

/// <summary>
/// Route A: Activity → sequence name mapping without Animgraph assets.
/// Scans model sequence names at runtime, builds mapping via heuristics.
/// Manual per-model override available via RegisterModel().
/// </summary>
public static class VJAnimationMapper
{
    // model path → Activity → sequence name
    private static readonly Dictionary<string, Dictionary<Activity, string>> _cache = new();

    // Per-Activity name candidates tried in order. Generated once.
    private static Dictionary<Activity, string[]> _candidates;

    // ── Public API ──

    /// <summary>Register manual mappings for a model (call in NPC spawn).</summary>
    public static void RegisterModel(string modelPath, Dictionary<Activity, string> map)
    {
        _cache[modelPath] = map;
    }

    /// <summary>Map a generic Activity to a model-specific sequence name, or null.</summary>
    public static string MapActivity(GameObject obj, Activity act)
    {
        if (obj == null) return null;
        var map = GetMap(obj);
        return map.TryGetValue(act, out var name) ? name : null;
    }

    /// <summary>Lua: VJ.AnimExists(ent, act) — does this activity resolve to a known sequence?</summary>
    public static bool AnimExists(GameObject obj, Activity act)
        => MapActivity(obj, act) != null;

    /// <summary>Lua: VJ.AnimExists(ent, "vjseq_*") — does this named sequence exist?</summary>
    public static bool AnimExists(GameObject obj, string sequenceName)
    {
        if (string.IsNullOrEmpty(sequenceName)) return false;
        var renderer = obj?.Components.Get<SkinnedModelRenderer>();
        if (renderer == null) return false;
        // Strip vjges_ prefix — gesture names resolve to activities via LookupSequence
        var name = sequenceName.StartsWith("vjges_") ? sequenceName[6..] : sequenceName;
        return renderer.Sequence.SequenceNames.Contains(name);
    }

    /// <summary>Lua: VJ.AnimDuration(ent, act) — duration of the mapped sequence, or 0.</summary>
    public static float AnimDuration(GameObject obj, Activity act)
    {
        var dp = GetDirectPlayback(obj);
        if (dp == null) return 0f;
        var name = MapActivity(obj, act);
        if (name == null) return 0f;
        dp.Play(name);
        return dp.Duration;
    }

    /// <summary>Lua: VJ.AnimDurationEx(ent, act, override, decrease).</summary>
    public static float AnimDurationEx(GameObject obj, Activity act, float? overrideDuration, float decrease)
    {
        var rate = obj?.Components.Get<SkinnedModelRenderer>()?.PlaybackRate ?? 1f;
        if (rate <= 0f) rate = 1f;
        if (overrideDuration.HasValue)
            return overrideDuration.Value / rate;
        return (AnimDuration(obj, act) - decrease) / rate;
    }

    /// <summary>
    /// Lua: VJ.SequenceToActivity(ent, "sequence_name") → Activity or null.
    /// Checks if the named sequence exists on the model, then reverse-looks-up which Activity maps to it.
    /// If no Activity maps to this sequence, returns null (caller should skip the translation entry).
    /// </summary>
    public static Activity? SequenceToActivity(GameObject obj, string sequenceName)
    {
        if (string.IsNullOrEmpty(sequenceName) || obj == null) return null;
        var renderer = obj.Components.Get<SkinnedModelRenderer>();
        if (renderer == null) return null;

        // Check if the sequence actually exists on this model
        if (!renderer.Sequence.SequenceNames.Contains(sequenceName, StringComparer.OrdinalIgnoreCase))
            return null;

        // Reverse lookup: find which Activity maps to this sequence name
        var map = GetMap(obj);
        var cachedReverse = GetReverseMap(obj);

        if (cachedReverse.TryGetValue(sequenceName, out var cached))
            return cached == Activity.Invalid ? null : cached;

        // Try each Activity to find which one maps to this sequence name
        foreach (var act in Enum.GetValues(typeof(Activity)))
        {
            var activity = (Activity)act;
            if (map.TryGetValue(activity, out var mappedName)
                && string.Equals(mappedName, sequenceName, StringComparison.OrdinalIgnoreCase))
            {
                _reverseCache[GetModelPath(obj)][sequenceName] = activity;
                return activity;
            }
        }

        // Not found in our mapping — sequence exists but unknown Activity
        _reverseCache[GetModelPath(obj)][sequenceName] = Activity.Invalid;
        return null;
    }

    // Reverse cache: model path → sequence name → Activity
    private static readonly Dictionary<string, Dictionary<string, Activity>> _reverseCache = new();

    private static Dictionary<string, Activity> GetReverseMap(GameObject obj)
    {
        var path = GetModelPath(obj);
        if (!_reverseCache.TryGetValue(path, out var map))
        {
            map = new Dictionary<string, Activity>(StringComparer.OrdinalIgnoreCase);
            _reverseCache[path] = map;
        }
        return map;
    }

    private static string GetModelPath(GameObject obj)
    {
        var renderer = obj?.Components.Get<SkinnedModelRenderer>();
        return renderer?.Model?.ResourcePath ?? renderer?.Model?.Name ?? "";
    }

    /// <summary>Get the animgraph DirectPlayback node for this object, or null.</summary>
    public static AnimGraphDirectPlayback GetDirectPlayback(GameObject obj)
    {
        return obj?.Components.Get<SkinnedModelRenderer>()?.AnimationGraph?.GetDirectPlayback();
    }

    /// <summary>Check if a given activity matches the currently playing sequence.</summary>
    public static bool IsCurrentAnim(GameObject obj, Activity act)
    {
        var dp = GetDirectPlayback(obj);
        if (dp == null) return false;
        var expected = MapActivity(obj, act);
        return expected != null && dp.Name == expected;
    }

    /// <summary>Lua: VJ.IsCurrentAnim(ent, string) — check if current anim matches a named sequence.</summary>
    public static bool IsCurrentAnim(GameObject obj, string sequenceName)
    {
        if (string.IsNullOrEmpty(sequenceName)) return false;
        var dp = GetDirectPlayback(obj);
        return dp != null && string.Equals(dp.Name, sequenceName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Lua: VJ.IsCurrentAnim(ent, table) — check if current anim matches any in the set.</summary>
    public static bool IsCurrentAnim(GameObject obj, Activity[] acts)
    {
        if (acts == null) return false;
        var dp = GetDirectPlayback(obj);
        if (dp == null) return false;
        foreach (var act in acts)
        {
            var name = MapActivity(obj, act);
            if (name != null && dp.Name == name) return true;
        }
        return false;
    }

    /// <summary>Clear cached mappings (e.g., after hot-reload).</summary>
    public static void ClearCache() => _cache.Clear();

    // ── Internal: build/get Activity→name map for a GameObject ──

    private static Dictionary<Activity, string> GetMap(GameObject obj)
    {
        var renderer = obj.Components.Get<SkinnedModelRenderer>();
        if (renderer == null) return _empty;

        var path = renderer.Model?.ResourcePath ?? renderer.Model?.Name ?? "";
        if (_cache.TryGetValue(path, out var cached)) return cached;

        var map = BuildMap(renderer);
        _cache[path] = map;
        return map;
    }

    private static readonly Dictionary<Activity, string> _empty = new();

    // ── Map builder: scan SequenceNames, match against Activity→name candidates ──

    private static Dictionary<Activity, string> BuildMap(SkinnedModelRenderer renderer)
    {
        var map = new Dictionary<Activity, string>();
        var names = renderer.Sequence.SequenceNames;

        foreach (var (act, candidates) in GetCandidates())
        {
            foreach (var c in candidates)
            {
                // Case-insensitive match against actual sequence names
                var match = names.FirstOrDefault(n => string.Equals(n, c, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    map[act] = match;
                    break;
                }
            }
        }

        return map;
    }

    // ── Candidate name table: Activity → ordered list of likely sequence names ──

    private static Dictionary<Activity, string[]> GetCandidates()
    {
        if (_candidates != null) return _candidates;

        _candidates = new Dictionary<Activity, string[]>();

        // Helper: common Source naming patterns for an activity constant name
        string[] P(string actName, params string[] extras)
        {
            var list = new List<string>();
            // Direct ACT_* name as sequence name
            list.Add(actName);
            // Lowercase variants
            list.Add(actName.ToLowerInvariant());
            // Stripped ACT_ prefix
            if (actName.StartsWith("ACT_")) list.Add(actName[4..]);
            if (actName.StartsWith("ACT_")) list.Add(actName[4..].ToLowerInvariant());
            // Extras (common HL2 model-specific names)
            list.AddRange(extras);
            return list.ToArray();
        }

        // Idle
        _candidates[Activity.Invalid]   = new[] { "" };
        _candidates[Activity.Reset]     = P("ACT_RESET");
        _candidates[Activity.Transition]= P("ACT_TRANSITION");

        _candidates[Activity.Idle]              = P("ACT_IDLE", "idle_all", "Idle01", "idle");
        _candidates[Activity.IdleAgitated]      = P("ACT_IDLE_AGITATED", "idle_agitated");
        _candidates[Activity.IdleStimulated]    = P("ACT_IDLE_STIMULATED", "idle_stimulated");
        _candidates[Activity.IdleRelaxed]       = P("ACT_IDLE_RELAXED", "idle_relaxed");
        _candidates[Activity.IdleAngry]         = P("ACT_IDLE_ANGRY", "idle_angry");
        _candidates[Activity.IdleAngryMelee]    = P("ACT_IDLE_ANGRY_MELEE");
        _candidates[Activity.IdleAngryPistol]   = P("ACT_IDLE_ANGRY_PISTOL");
        _candidates[Activity.IdleAngryRpg]      = P("ACT_IDLE_ANGRY_RPG");
        _candidates[Activity.IdleAngryShotgun]  = P("ACT_IDLE_ANGRY_SHOTGUN");
        _candidates[Activity.IdleAngrySmg1]     = P("ACT_IDLE_ANGRY_SMG1");
        _candidates[Activity.IdleAimRelaxed]    = P("ACT_IDLE_AIM_RELAXED");
        _candidates[Activity.IdleAimStimulated] = P("ACT_IDLE_AIM_STIMULATED");
        _candidates[Activity.IdleAimAgitated]   = P("ACT_IDLE_AIM_AGITATED");
        _candidates[Activity.IdleAimRifleStimulated] = P("ACT_IDLE_AIM_RIFLE_STIMULATED");
        _candidates[Activity.IdlePistol]        = P("ACT_IDLE_PISTOL");
        _candidates[Activity.IdleRpg]           = P("ACT_IDLE_RPG");
        _candidates[Activity.IdleRpgRelaxed]    = P("ACT_IDLE_RPG_RELAXED");
        _candidates[Activity.IdleShotgunAgitated]   = P("ACT_IDLE_SHOTGUN_AGITATED");
        _candidates[Activity.IdleShotgunRelaxed]    = P("ACT_IDLE_SHOTGUN_RELAXED");
        _candidates[Activity.IdleShotgunStimulated] = P("ACT_IDLE_SHOTGUN_STIMULATED");
        _candidates[Activity.IdleSmg1]          = P("ACT_IDLE_SMG1");
        _candidates[Activity.IdleSmg1Relaxed]   = P("ACT_IDLE_SMG1_RELAXED");
        _candidates[Activity.IdleSmg1Stimulated]= P("ACT_IDLE_SMG1_STIMULATED");
        _candidates[Activity.Cower]             = P("ACT_COWER", "cower");
        _candidates[Activity.CrouchIdle]        = P("ACT_CROUCHIDLE");
        _candidates[Activity.DoNotDisturb]      = P("ACT_DO_NOT_DISTURB");
        _candidates[Activity.BusySitGround]     = P("ACT_BUSY_SIT_GROUND");
        _candidates[Activity.ShotgunIdleDeep]   = P("ACT_SHOTGUN_IDLE_DEEP");

        // Walk
        _candidates[Activity.Walk]          = P("ACT_WALK", "walk_all", "Walk01", "walk");
        _candidates[Activity.WalkAgitated]  = P("ACT_WALK_AGITATED");
        _candidates[Activity.WalkStimulated]= P("ACT_WALK_STIMULATED");
        _candidates[Activity.WalkRelaxed]   = P("ACT_WALK_RELAXED");
        _candidates[Activity.WalkAngry]     = P("ACT_WALK_ANGRY");
        _candidates[Activity.WalkAim]       = P("ACT_WALK_AIM");
        _candidates[Activity.WalkAimAgitated]=P("ACT_WALK_AIM_AGITATED");
        _candidates[Activity.WalkAimRelaxed] =P("ACT_WALK_AIM_RELAXED");
        _candidates[Activity.WalkAimStimulated]=P("ACT_WALK_AIM_STIMULATED");
        _candidates[Activity.WalkAimRifle]  = P("ACT_WALK_AIM_RIFLE");
        _candidates[Activity.WalkAimRifleStimulated]=P("ACT_WALK_AIM_RIFLE_STIMULATED");
        _candidates[Activity.WalkAimPistol] = P("ACT_WALK_AIM_PISTOL");
        _candidates[Activity.WalkAimShotgun]= P("ACT_WALK_AIM_SHOTGUN");
        _candidates[Activity.WalkRifle]     = P("ACT_WALK_RIFLE");
        _candidates[Activity.WalkRifleRelaxed]=P("ACT_WALK_RIFLE_RELAXED");
        _candidates[Activity.WalkRifleStimulated]=P("ACT_WALK_RIFLE_STIMULATED");
        _candidates[Activity.WalkPistol]    = P("ACT_WALK_PISTOL");
        _candidates[Activity.WalkRpg]       = P("ACT_WALK_RPG");
        _candidates[Activity.WalkRpgRelaxed]= P("ACT_WALK_RPG_RELAXED");
        _candidates[Activity.WalkCrouch]    = P("ACT_WALK_CROUCH", "walk_crouch");
        _candidates[Activity.WalkCrouchAim] = P("ACT_WALK_CROUCH_AIM");
        _candidates[Activity.WalkCrouchAimRifle]=P("ACT_WALK_CROUCH_AIM_RIFLE");
        _candidates[Activity.WalkCrouchRifle]=P("ACT_WALK_CROUCH_RIFLE");
        _candidates[Activity.WalkCrouchRpg] = P("ACT_WALK_CROUCH_RPG");

        // Run
        _candidates[Activity.Run]           = P("ACT_RUN", "run_all", "Run01", "run");
        _candidates[Activity.RunAgitated]   = P("ACT_RUN_AGITATED");
        _candidates[Activity.RunStimulated] = P("ACT_RUN_STIMULATED");
        _candidates[Activity.RunRelaxed]    = P("ACT_RUN_RELAXED");
        _candidates[Activity.RunAim]        = P("ACT_RUN_AIM");
        _candidates[Activity.RunAimAgitated]= P("ACT_RUN_AIM_AGITATED");
        _candidates[Activity.RunAimRelaxed] = P("ACT_RUN_AIM_RELAXED");
        _candidates[Activity.RunAimStimulated]=P("ACT_RUN_AIM_STIMULATED");
        _candidates[Activity.RunAimRifle]   = P("ACT_RUN_AIM_RIFLE");
        _candidates[Activity.RunAimRifleStimulated]=P("ACT_RUN_AIM_RIFLE_STIMULATED");
        _candidates[Activity.RunAimPistol]  = P("ACT_RUN_AIM_PISTOL");
        _candidates[Activity.RunAimShotgun] = P("ACT_RUN_AIM_SHOTGUN");
        _candidates[Activity.RunRifle]      = P("ACT_RUN_RIFLE");
        _candidates[Activity.RunRifleRelaxed]=P("ACT_RUN_RIFLE_RELAXED");
        _candidates[Activity.RunRifleStimulated]=P("ACT_RUN_RIFLE_STIMULATED");
        _candidates[Activity.RunPistol]     = P("ACT_RUN_PISTOL");
        _candidates[Activity.RunRpg]        = P("ACT_RUN_RPG");
        _candidates[Activity.RunRpgRelaxed] = P("ACT_RUN_RPG_RELAXED");
        _candidates[Activity.RunProtected]  = P("ACT_RUN_PROTECTED");
        _candidates[Activity.RunCrouch]     = P("ACT_RUN_CROUCH");
        _candidates[Activity.RunCrouchAim]  = P("ACT_RUN_CROUCH_AIM");
        _candidates[Activity.RunCrouchAimRifle]=P("ACT_RUN_CROUCH_AIM_RIFLE");
        _candidates[Activity.RunCrouchRifle]= P("ACT_RUN_CROUCH_RIFLE");
        _candidates[Activity.RunCrouchRpg]  = P("ACT_RUN_CROUCH_RPG");

        // Special movement
        _candidates[Activity.Fly]     = P("ACT_FLY", "fly");
        _candidates[Activity.Swim]    = P("ACT_SWIM", "swim");
        _candidates[Activity.Glide]   = P("ACT_GLIDE", "glide");
        _candidates[Activity.Jump]    = P("ACT_JUMP", "jump");
        _candidates[Activity.Land]    = P("ACT_LAND", "land");
        _candidates[Activity.ClimbUp] = P("ACT_CLIMB_UP", "climbup");

        // Melee
        _candidates[Activity.MeleeAttack1]     = P("ACT_MELEE_ATTACK1", "Melee_Attack1");
        _candidates[Activity.MeleeAttackSwing] = P("ACT_MELEE_ATTACK_SWING");

        // Range
        _candidates[Activity.RangeAttack1]         = P("ACT_RANGE_ATTACK1", "Range_Attack1");
        _candidates[Activity.RangeAttack1Low]      = P("ACT_RANGE_ATTACK1_LOW");
        _candidates[Activity.RangeAttack2]         = P("ACT_RANGE_ATTACK2");
        _candidates[Activity.RangeAttackAr2]       = P("ACT_RANGE_ATTACK_AR2");
        _candidates[Activity.RangeAttackAr2Low]    = P("ACT_RANGE_ATTACK_AR2_LOW");
        _candidates[Activity.RangeAttackPistol]    = P("ACT_RANGE_ATTACK_PISTOL");
        _candidates[Activity.RangeAttackPistolLow] = P("ACT_RANGE_ATTACK_PISTOL_LOW");
        _candidates[Activity.RangeAttackShotgun]   = P("ACT_RANGE_ATTACK_SHOTGUN");
        _candidates[Activity.RangeAttackShotgunLow]= P("ACT_RANGE_ATTACK_SHOTGUN_LOW");
        _candidates[Activity.RangeAttackSmg1]      = P("ACT_RANGE_ATTACK_SMG1");
        _candidates[Activity.RangeAttackSmg1Low]   = P("ACT_RANGE_ATTACK_SMG1_LOW");
        _candidates[Activity.RangeAttackRpg]       = P("ACT_RANGE_ATTACK_RPG");
        _candidates[Activity.RangeAimLow]          = P("ACT_RANGE_AIM_LOW");
        _candidates[Activity.RangeAimAr2Low]       = P("ACT_RANGE_AIM_AR2_LOW");
        _candidates[Activity.RangeAimPistolLow]    = P("ACT_RANGE_AIM_PISTOL_LOW");
        _candidates[Activity.RangeAimSmg1Low]      = P("ACT_RANGE_AIM_SMG1_LOW");
        _candidates[Activity.SpecialAttack1]       = P("ACT_SPECIAL_ATTACK1");

        // Reload
        _candidates[Activity.Reload]           = P("ACT_RELOAD", "reload");
        _candidates[Activity.ReloadLow]        = P("ACT_RELOAD_LOW");
        _candidates[Activity.ReloadPistol]     = P("ACT_RELOAD_PISTOL");
        _candidates[Activity.ReloadPistolLow]  = P("ACT_RELOAD_PISTOL_LOW");
        _candidates[Activity.ReloadShotgun]    = P("ACT_RELOAD_SHOTGUN");
        _candidates[Activity.ReloadShotgunLow] = P("ACT_RELOAD_SHOTGUN_LOW");
        _candidates[Activity.ReloadSmg1]       = P("ACT_RELOAD_SMG1");
        _candidates[Activity.ReloadSmg1Low]    = P("ACT_RELOAD_SMG1_LOW");
        _candidates[Activity.ShotgunPump]      = P("ACT_SHOTGUN_PUMP");

        // Cover
        _candidates[Activity.Cover]          = P("ACT_COVER");
        _candidates[Activity.CoverLow]       = P("ACT_COVER_LOW");
        _candidates[Activity.CoverLowRpg]    = P("ACT_COVER_LOW_RPG");
        _candidates[Activity.CoverPistolLow] = P("ACT_COVER_PISTOL_LOW");
        _candidates[Activity.CoverSmg1Low]   = P("ACT_COVER_SMG1_LOW");

        // Flinch
        _candidates[Activity.FlinchPhysics]  = P("ACT_FLINCH_PHYSICS");
        _candidates[Activity.FlinchHead]     = P("ACT_FLINCH_HEAD");
        _candidates[Activity.FlinchLeftArm]  = P("ACT_FLINCH_LEFTARM");
        _candidates[Activity.FlinchLeftLeg]  = P("ACT_FLINCH_LEFTLEG");
        _candidates[Activity.FlinchRightArm] = P("ACT_FLINCH_RIGHTARM");
        _candidates[Activity.FlinchRightLeg] = P("ACT_FLINCH_RIGHTLEG");

        // ViewModel
        _candidates[Activity.VmDraw]           = P("ACT_VM_DRAW");
        _candidates[Activity.VmHolster]        = P("ACT_VM_HOLSTER");
        _candidates[Activity.VmIdle]           = P("ACT_VM_IDLE");
        _candidates[Activity.VmFidget]         = P("ACT_VM_FIDGET");
        _candidates[Activity.VmPrimaryAttack]  = P("ACT_VM_PRIMARYATTACK");
        _candidates[Activity.VmSecondaryAttack]= P("ACT_VM_SECONDARYATTACK");
        _candidates[Activity.VmReload]         = P("ACT_VM_RELOAD");
        _candidates[Activity.VmIdleToLowered]  = P("ACT_VM_IDLE_TO_LOWERED");

        // Gesture
        _candidates[Activity.GestureRangeAttack1]       = P("ACT_GESTURE_RANGE_ATTACK1");
        _candidates[Activity.GestureRangeAttackAr2]     = P("ACT_GESTURE_RANGE_ATTACK_AR2");
        _candidates[Activity.GestureRangeAttackPistol]  = P("ACT_GESTURE_RANGE_ATTACK_PISTOL");
        _candidates[Activity.GestureRangeAttackRpg]     = P("ACT_GESTURE_RANGE_ATTACK_RPG");
        _candidates[Activity.GestureRangeAttackShotgun] = P("ACT_GESTURE_RANGE_ATTACK_SHOTGUN");
        _candidates[Activity.GestureRangeAttackSmg1]    = P("ACT_GESTURE_RANGE_ATTACK_SMG1");
        _candidates[Activity.GestureReload]             = P("ACT_GESTURE_RELOAD");
        _candidates[Activity.GestureReloadPistol]       = P("ACT_GESTURE_RELOAD_PISTOL");
        _candidates[Activity.GestureReloadShotgun]      = P("ACT_GESTURE_RELOAD_SHOTGUN");
        _candidates[Activity.GestureReloadSmg1]         = P("ACT_GESTURE_RELOAD_SMG1");
        _candidates[Activity.MeleeAttackSwingGesture]   = P("ACT_MELEE_ATTACK_SWING_GESTURE");

        // Signal
        _candidates[Activity.SignalAdvance] = P("ACT_SIGNAL_ADVANCE");
        _candidates[Activity.SignalForward] = P("ACT_SIGNAL_FORWARD");
        _candidates[Activity.SignalGroup]   = P("ACT_SIGNAL_GROUP");

        // Misc
        _candidates[Activity.Dmg]        = P("ACT_DMG");
        _candidates[Activity.Disarm]     = P("ACT_DISARM");
        _candidates[Activity.Arm]        = P("ACT_ARM");
        _candidates[Activity.PlayActivity]=P("ACT_PLAYACTIVITY");
        _candidates[Activity.ToSource]   = P("ACT_TO_SOURCE");
        _candidates[Activity.TurnLeft]   = P("ACT_TURN_LEFT");
        _candidates[Activity.TurnRight]  = P("ACT_TURN_RIGHT");
        _candidates[Activity.PoliceHarass1]=P("ACT_POLICE_HARASS1");

        // HL2MP — simple base mappings; handgun/rifle variants resolved via AnimationTranslations table
        _candidates[Activity.Hl2mpIdle]    = P("ACT_HL2MP_IDLE", "Idle");
        _candidates[Activity.Hl2mpIdleAngry]=P("ACT_HL2MP_IDLE_ANGRY");
        _candidates[Activity.Hl2mpIdleCower]=P("ACT_HL2MP_IDLE_COWER");
        _candidates[Activity.Hl2mpIdlePassive]=P("ACT_HL2MP_IDLE_PASSIVE");
        _candidates[Activity.Hl2mpRun]     = P("ACT_HL2MP_RUN", "Run");
        _candidates[Activity.Hl2mpRunFast] = P("ACT_HL2MP_RUN_FAST");
        _candidates[Activity.Hl2mpRunProtected]=P("ACT_HL2MP_RUN_PROTECTED");
        _candidates[Activity.Hl2mpRunPassive]=P("ACT_HL2MP_RUN_PASSIVE");
        _candidates[Activity.Hl2mpWalk]    = P("ACT_HL2MP_WALK", "Walk");
        _candidates[Activity.Hl2mpWalkCrouch]=P("ACT_HL2MP_WALK_CROUCH");
        _candidates[Activity.Hl2mpWalkPassive]=P("ACT_HL2MP_WALK_PASSIVE");
        _candidates[Activity.Hl2mpWalkCrouchPassive]=P("ACT_HL2MP_WALK_CROUCH_PASSIVE");

        // HL2MP per-weapon: defer to AnimationTranslations; just try the ACT_ name directly
        foreach (Activity act in Enum.GetValues(typeof(Activity)))
        {
            if (!_candidates.ContainsKey(act))
                _candidates[act] = new[] { act.ToString() };
        }

        return _candidates;
    }
}
