using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace VJBase;

/// <summary>
/// Animation system — PlayAnim, TranslateActivity, MaintainIdleAnimation, Pose params.
/// Route A: sequence direct-play via AnimgraphDirectPlayback, no Animgraph assets needed.
/// Ported from core.lua:526-869 + funcs.lua:310-446 + creature/human TranslateActivity overrides.
/// </summary>
public partial class BaseNPC
{
    // ═══ Animation State Fields ═══
    /// <summary>Pose parameter names detected on the model (e.g. "aim_pitch", "head_yaw").</summary>
    public string[] PoseParameterLooking_Names { get; set; }
    public bool HasPoseParameterLooking { get; set; } = true;
    public float PosePitch { get; set; }
    public float PoseYaw { get; set; }
    public float PoseRoll { get; set; }

    // ═══ Animation Translation Table ═══
    /// <summary>ACT_generic → ACT_model_specific (single Activity) or Activity[] (random pick).</summary>
    public Dictionary<Activity, object> AnimationTranslations { get; set; } = new();

    // ═══ PlayAnim: core animation entry point ═══
    // Lua: core.lua:631-869 ENT:PlayAnim(animation, lockAnim, lockAnimTime, faceEnemy, animDelay, extraOptions, customFunc)

    /// <summary>
    /// Play an animation by Activity, sequence name, or "vjseq_*" name.
    /// Route A: maps Activity → sequence name, plays via AnimgraphDirectPlayback.
    /// Returns (playedActivity, animTime, animType).
    /// </summary>
    public virtual (Activity activity, float animTime, VJAnimType animType) PlayAnim(
        object animation,
        object lockAnim = null,
        object lockAnimTime = null,
        object faceEnemy = null,
        float animDelay = 0f,
        PlayAnimOptions extraOptions = null,
        Action<AISchedule, object> customFunc = null)
    {
        // ── Resolve PICK (animation can be Activity, Activity[], or string) ──
        animation = ResolveAnimationArg(animation);
        if (animation == null)
            return (Activity.Invalid, 0f, VJAnimType.None);

        var lockB = lockAnim as bool? ?? false;
        var lockStr = lockAnim as string;
        if (lockStr == "LetAttacks") lockB = true;
        var lockTime = lockAnimTime as float? ?? 0f;
        var faceEnemyB = faceEnemy as bool? ?? false;
        var faceVis = faceEnemy as string == "Visible";
        extraOptions ??= new PlayAnimOptions();

        bool isGesture = false;
        bool isSequence = false;
        bool isString = animation is string;
        bool isRecheck = false;

        // ── Parse vjges_ / vjseq_ prefixes ──
    recheck:
        var animStr = animation as string;
        if (isString && animStr != null)
        {
            var stripped = StripAnimPrefix(animStr, out isGesture, out isSequence);
            if (stripped != animStr) animation = stripped;
            animStr = stripped;

            // vjges_ activity fallback: if gesture-only and LookupSequence fails, treat as int Activity
            if (isGesture && !isSequence && int.TryParse(animStr, out var actId))
            {
                animation = (Activity)actId;
                isString = false;
            }
        }

        if (extraOptions.AlwaysUseGesture) isGesture = true;
        if (extraOptions.AlwaysUseSequence)
        {
            isSequence = true;
            if (animation is Activity act && !isString)
            {
                var seqName = VJAnimationMapper.MapActivity(GameObject, act);
                if (seqName != null) { animation = seqName; isString = true; }
            }
        }
        else if (isString && !isSequence)
        {
            // Try to convert string sequence to activity
            var renderer = Components.Get<SkinnedModelRenderer>();
            if (renderer != null)
            {
                var seqId = renderer.Sequence.SequenceNames
                    .Select((name, idx) => (name, idx))
                    .FirstOrDefault(x => string.Equals(x.name, animStr, StringComparison.OrdinalIgnoreCase));
                if (seqId.name != null)
                {
                    // Sequence exists; keep it as a named sequence
                    isSequence = true;
                }
                else
                {
                    isSequence = true; // Unknown string → treat as named sequence anyway
                }
            }
        }

        // ── TranslateActivity check ──
        if (!isString && !isRecheck && animation is Activity actBefore)
        {
            var translation = TranslateActivity(actBefore);
            if (!EqualityComparer<Activity>.Default.Equals(translation, actBefore))
            {
                animation = translation;
                if (translation is string transStr)
                {
                    isString = true;
                    isRecheck = true;
                    goto recheck;
                }
            }
        }

        // ── AnimExists check ──
        bool exists;
        if (animation is Activity actCheck)
            exists = VJAnimationMapper.AnimExists(GameObject, actCheck);
        else if (animation is string sCheck)
            exists = VJAnimationMapper.AnimExists(GameObject, sCheck);
        else
            return (Activity.Invalid, 0f, VJAnimType.None);

        if (!exists)
            return (Activity.Invalid, 0f, VJAnimType.None);

        // ── Determine anim type ──
        var animType = isGesture ? VJAnimType.Gesture
            : isSequence ? VJAnimType.Sequence
            : VJAnimType.Activity;

        var seed = (float)Time.Now; // ≈ CurTime()
        LastAnimType = animType;
        LastAnimSeed = (int)(seed * 1000);

        // ── PlayAct inner function ──
        float PlayAct()
        {
            var originalRate = AnimPlaybackRate;
            var customRate = extraOptions.PlayBackRate;
            var playbackRate = customRate > 0 ? customRate : originalRate;
            if (playbackRate <= 0f) playbackRate = 1f;

            var animTime = AnimDurationEx((Activity)(animation is Activity a ? a : Activity.Invalid), null, 0f);
            // Duration calc: use AnimgraphDirectPlayback after Play()
            var seqName = animation as string;
            if (animation is Activity actDur)
                seqName = VJAnimationMapper.MapActivity(GameObject, actDur);

            var dp = VJAnimationMapper.GetDirectPlayback(GameObject);
            if (dp != null && seqName != null)
            {
                dp.Play(seqName);
                animTime = dp.Duration;
            }

            bool doRealAnimTime = true;

            if (lockB && !isGesture)
            {
                if (lockAnimTime is bool bTime && !bTime) // false = auto-calculate
                {
                    lockTime = animTime;
                }
                else
                {
                    doRealAnimTime = false;
                    if (extraOptions.PlayBackRateCalculated != true)
                        lockTime = lockTime / playbackRate;
                    animTime = lockTime;
                }

                var curTime = (float)Time.Now;
                NextChaseTime = curTime + lockTime;
                NextIdleTime = curTime + lockTime;
                AnimLockTime = curTime + lockTime;

                if (lockStr != "LetAttacks")
                {
                    StopAttacks(true);
                    PauseAttacks = true;
                    var pauseTime = lockTime;
                    _ = Task.Delay((int)(pauseTime * 1000)).ContinueWith(_ => { PauseAttacks = false; });
                }
            }

            LastAnimSeed = (int)(seed * 1000);

            if (isGesture)
            {
                // Route A: no gesture overlay support. Play as regular sequence if possible.
                if (dp != null && seqName != null)
                    dp.Play(seqName);
            }
            else
            {
                // Activities & Sequences
                WeaponAttackState = VJWepAttackState.None;
                OnTaskComplete();
                StopMoving();
                ClearSchedule();
                ClearGoal();

                if (dp != null && seqName != null)
                {
                    dp.Play(seqName);
                }

                customFunc?.Invoke(CurrentSchedule, animation);

                if (faceEnemyB || faceVis)
                {
                    SetTurnTarget("Enemy", animTime, false, faceVis);
                }
            }

            // OnFinish callback
            if (extraOptions.OnFinish != null)
            {
                var capturedSeed = LastAnimSeed;
                var capturedAnim = animation;
                var finishTime = animTime;
                _ = Task.Delay((int)(finishTime * 1000)).ContinueWith(_ =>
                {
                    if (this.IsValid() && !Dead)
                        extraOptions.OnFinish(LastAnimSeed != capturedSeed, capturedAnim);
                });
            }

            return animTime;
        }

        // ── Delay system ──
        if (animDelay > 0f)
        {
            var capturedSeed = LastAnimSeed;
            _ = Task.Delay((int)(animDelay * 1000)).ContinueWith(_ =>
            {
                if (this.IsValid() && LastAnimSeed == capturedSeed)
                    PlayAct();
            });
            var approxTime = animDelay + AnimDurationEx(
                animation is Activity aDelay ? aDelay : Activity.Invalid, null, 0f);
            return (animation is Activity resultAct ? resultAct : Activity.Invalid, approxTime, animType);
        }
        else
        {
            var actResult = animation is Activity actR ? actR : Activity.Invalid;
            return (actResult, PlayAct(), animType);
        }
    }

    // ── PlayAnim helpers ──

    private static object ResolveAnimationArg(object anim)
    {
        if (anim is Activity[] arr)
            return arr.Length > 0 ? VJUtility.PICK(arr) : null;
        if (anim is IList<Activity> list)
            return list.Count > 0 ? VJUtility.PICK(list) : null;
        return anim; // Activity, string, or null
    }

    private static string StripAnimPrefix(string input, out bool isGesture, out bool isSequence)
    {
        isGesture = false;
        isSequence = false;
        var result = input;

        var idxGes = input.IndexOf("vjges_", StringComparison.OrdinalIgnoreCase);
        var idxSeq = input.IndexOf("vjseq_", StringComparison.OrdinalIgnoreCase);

        while (idxGes >= 0 || idxSeq >= 0)
        {
            var parts = new System.Text.StringBuilder();
            int pos = 0;
            int nextGes, nextSeq;
            while ((nextGes = input.IndexOf("vjges_", pos, StringComparison.OrdinalIgnoreCase)) >= 0
                || (nextSeq = input.IndexOf("vjseq_", pos, StringComparison.OrdinalIgnoreCase)) >= 0)
            {
                int matchPos;
                int matchEnd;
                if (nextGes >= 0 && (nextSeq < 0 || nextGes < nextSeq))
                {
                    isGesture = true;
                    matchPos = nextGes;
                    matchEnd = nextGes + 6; // "vjges_".Length
                }
                else
                {
                    isSequence = true;
                    matchPos = nextSeq;
                    matchEnd = nextSeq + 6; // "vjseq_".Length
                }
                parts.Append(input.AsSpan(pos, matchPos - pos));
                pos = matchEnd;
            }
            parts.Append(input.AsSpan(pos));
            result = parts.ToString();
            break;
        }

        // vjges_ activity fallback detection: if gesture-only, strip prefix and try parse
        if (!string.IsNullOrEmpty(result) && isGesture && !isSequence)
        {
            if (int.TryParse(result, out _))
            {
                // It's an activity number embedded in vjges_ prefix — already stripped
            }
        }

        return result;
    }

    // ── AnimDuration / AnimDurationEx ──

    /// <summary>Lua: VJ.AnimDuration — duration of the mapped sequence, or 0.</summary>
    public float AnimDuration(Activity act)
        => VJAnimationMapper.AnimDuration(GameObject, act);

    /// <summary>Lua: VJ.AnimDurationEx — duration with override/decrease/playbackRate.</summary>
    public float AnimDurationEx(Activity act, float? overrideDuration, float decrease)
    {
        var rate = AnimPlaybackRate;
        if (rate <= 0f) rate = 1f;
        if (overrideDuration.HasValue)
            return overrideDuration.Value / rate;
        return (AnimDuration(act) - decrease) / rate;
    }

    // ═══ TranslateActivity ═══
    // Lua: creature init.lua:1809 (base) + human init.lua:2417 (override with combat context)

    /// <summary>
    /// TranslateActivity — base implementation (CreatureNPC style).
    /// Looks up AnimationTranslations[act]. If value is Activity[] → random pick (or ResolveAnimation for ACT_IDLE).
    /// </summary>
    public virtual Activity TranslateActivity(Activity act)
    {
        if (!AnimationTranslations.TryGetValue(act, out var translation))
            return act;

        if (translation is Activity[] arr)
        {
            if (EqualityComparer<Activity>.Default.Equals(act, Activity.Idle))
                return ResolveAnimation(arr);
            return arr.Length > 0 ? VJUtility.PICK(arr) : act;
        }

        if (translation is List<Activity> list)
        {
            if (EqualityComparer<Activity>.Default.Equals(act, Activity.Idle))
                return ResolveAnimation(list.ToArray());
            return list.Count > 0 ? VJUtility.PICK(list) : act;
        }

        if (translation is Activity single)
            return single;

        return act;
    }

    /// <summary>
    /// ResolveAnimation — Lua: core.lua:509-520.
    /// If the current animation is in the table and hasn't finished playing, keep it.
    /// Otherwise, pick a random one.
    /// </summary>
    public virtual Activity ResolveAnimation(Activity[] tbl)
    {
        if (tbl == null || tbl.Length == 0) return Activity.Idle;

        var dp = VJAnimationMapper.GetDirectPlayback(GameObject);
        if (dp != null && dp.TimeNormalized < 0.99f)
        {
            var current = dp.Name;
            foreach (var anim in tbl)
            {
                var name = VJAnimationMapper.MapActivity(GameObject, anim);
                if (name != null && name == current)
                    return anim;
            }
        }

        return VJUtility.PICK(tbl);
    }

    // ═══ MaintainIdleAnimation ═══
    // Lua: core.lua:526-559. Override of existing stub.

    /// <summary>
    /// MaintainIdleAnimation — applies and maintains the idle animation.
    /// force=true: forcibly reset to idle. force=false: auto-restart if cycle >= 0.98 or idle changed.
    /// </summary>
    public virtual void MaintainIdleAnimation(bool force)
    {
        var dp = VJAnimationMapper.GetDirectPlayback(GameObject);
        if (dp == null) return;

        if (force)
        {
            LastAnimSeed = 0;
            var idleName = VJAnimationMapper.MapActivity(GameObject, Activity.Idle);
            if (idleName != null)
            {
                dp.Play(idleName);
            }
            return;
        }

        // Auto mode: check if we're currently playing an idle
        var currentName = dp.Name;
        var currentIdleName = VJAnimationMapper.MapActivity(GameObject, Activity.Idle);
        var translatedIdle = TranslateActivity(Activity.Idle);
        var translatedName = VJAnimationMapper.MapActivity(GameObject, translatedIdle);

        bool isPlayingIdle = currentName != null
            && (string.Equals(currentName, currentIdleName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentName, translatedName, StringComparison.OrdinalIgnoreCase));

        if (isPlayingIdle)
        {
            if (dp.TimeNormalized >= 0.98f || (translatedName != null && currentName != translatedName))
            {
                LastAnimSeed = 0;
                if (currentIdleName != null)
                    dp.Play(currentIdleName);
            }
            else
            {
                // Let the sequence loop naturally
            }
        }
    }

    // ═══ MaintainActivity — Source engine ENT:MaintainActivity() ═══
    // Lua: schedules.lua:208 — called at end of RunAI. Phase 3.

    /// <summary>
    /// MaintainActivity — Source engine callback. In Route A, Animgraph handles ongoing playback;
    /// this just ensures idle cycling continues.
    /// </summary>
    public virtual void MaintainActivity()
    {
        // AnimgraphDirectPlayback auto-maintains the current sequence.
        // No explicit cycle management needed in Route A.
    }

    // ═══ UpdateAnimationTranslations ═══
    // Lua: core.lua:485-500 — detect model set by probing known sequences.

    /// <summary>
    /// Detect the model animation set (Combine/Metrocop/Rebel/Player) and rebuild the translation table.
    /// </summary>
    public virtual void UpdateAnimationTranslations(string wepHoldType = null)
    {
        if (AnimModelSet == VJAnimSet.None)
        {
            var renderer = Components.Get<SkinnedModelRenderer>();
            if (renderer != null)
            {
                var names = renderer.Sequence.SequenceNames;
                bool has(string s) => names.Contains(s, StringComparer.OrdinalIgnoreCase);

                if (has("signal_takecover") && has("grenthrow") && has("bugbait_hit"))
                    AnimModelSet = VJAnimSet.Combine;
                else if (VJAnimationMapper.AnimExists(GameObject, Activity.WalkAimPistol)
                    && VJAnimationMapper.AnimExists(GameObject, Activity.RunAimPistol)
                    && VJAnimationMapper.AnimExists(GameObject, Activity.PoliceHarass1))
                    AnimModelSet = VJAnimSet.Metrocop;
                else if (has("coverlow_r") && has("wave_smg1")
                    && VJAnimationMapper.AnimExists(GameObject, Activity.BusySitGround))
                    AnimModelSet = VJAnimSet.Rebel;
                else if (has("gmod_breath_layer"))
                    AnimModelSet = VJAnimSet.Player;
            }
        }

        AnimationTranslations = new Dictionary<Activity, object>();
        SetAnimationTranslations(wepHoldType);
    }

    /// <summary>
    /// SetAnimationTranslations — override in model-specific NPCs to populate the translation table.
    /// Lua: human init.lua:904-1050. Called by UpdateAnimationTranslations() after model-set detection.
    /// </summary>
    public virtual void SetAnimationTranslations(string wepHoldType)
    {
        // Base implementation: no translations. Override in HumanNPC.Think.cs.
    }

    // ═══ UpdatePoseParamTracking ═══
    // Lua: human init.lua:3426-3467 (full) + creature init.lua:2752 (simpler).
    // Override of existing stub in BaseNPC.cs:575.

    /// <summary>
    /// Per-frame pose parameter update — smooth tracking toward target position.
    /// Uses SkinnedModelRenderer.ParameterAccessor.Set(name, value).
    /// </summary>
    public virtual void UpdatePoseParamTracking(bool reset)
    {
        var renderer = Components.Get<SkinnedModelRenderer>();
        if (renderer == null) return;

        if (PoseParameterLooking_Names == null)
        {
            PoseParameterLooking_Names = DetectPoseParameters(renderer);
            HasPoseParameterLooking = PoseParameterLooking_Names.Length > 0;
        }

        if (!HasPoseParameterLooking) return;
        if (reset)
        {
            foreach (var name in PoseParameterLooking_Names)
                renderer.Parameters.Set(name, 0f);
            return;
        }

        // Calculate pitch/yaw/roll from eye position to target in NPC local space
        var eyePos = WorldPosition + ViewOffset;
        var targetPos = (EnemyData?.Target?.WorldPosition)
            ?? (Enemy != null ? Enemy.WorldPosition : eyePos + Rotation.Forward * 100f);

        var delta = targetPos - eyePos;
        if (delta.Length < 1f) return;

        // Convert world-space direction to local space relative to NPC rotation
        var localDir = GameObject.WorldRotation.Inverse * delta.Normal;
        float targetPitch = MathF.Asin(Math.Clamp(localDir.z, -1f, 1f)) * (180f / MathF.PI);
        float targetYaw = MathF.Atan2(localDir.y, localDir.x) * (180f / MathF.PI);
        float targetRoll = 0f;

        float frameTime = Time.Delta;
        float approachSpeed = 5f;

        foreach (var name in PoseParameterLooking_Names)
        {
            float target = 0f;
            if (name.Contains("pitch")) target = targetPitch;
            else if (name.Contains("yaw")) target = targetYaw;
            else if (name.Contains("roll")) target = targetRoll;

            float current = renderer.Parameters.GetFloat(name);
            float next = ApproachAngle(current, target, approachSpeed * frameTime);
            renderer.Parameters.Set(name, next);
        }

        PosePitch = targetPitch;
        PoseYaw = targetYaw;
        PoseRoll = targetRoll;
    }

    // ── Pose parameter detection ──

    private static string[] DetectPoseParameters(SkinnedModelRenderer renderer)
    {
        var found = new List<string>();
        string[] candidates = { "aim_pitch", "head_pitch", "aim_yaw", "head_yaw", "aim_roll", "head_roll" };
        foreach (var c in candidates)
        {
            if (renderer.Parameters.Contains(c))
                found.Add(c);
        }
        return found.ToArray();
    }

    // ── ApproachAngle: smooth angular interpolation toward target ──
    // Lua: math.ApproachAngle — moves current toward target by at most `speed` degrees.

    private static float ApproachAngle(float current, float target, float speed)
    {
        var diff = AngleDelta(current, target);
        if (MathF.Abs(diff) <= speed) return target;
        return current + MathF.Sign(diff) * speed;
    }

    private static float AngleDelta(float a, float b)
    {
        var d = (b - a) % 360f;
        if (d > 180f) d -= 360f;
        if (d < -180f) d += 360f;
        return d;
    }
}

/// <summary>
/// Extra options for PlayAnim — mirrors Lua extraOptions table.
/// </summary>
public class PlayAnimOptions
{
    /// <summary>Called when the animation finishes: Action&lt;bool interrupted, object animation&gt;</summary>
    public Action<bool, object> OnFinish { get; set; }

    /// <summary>Force playing as a sequence even if it could be an activity.</summary>
    public bool AlwaysUseSequence { get; set; }

    /// <summary>Force playing as a gesture overlay.</summary>
    public bool AlwaysUseGesture { get; set; }

    /// <summary>Custom playback rate multiplier.</summary>
    public float PlayBackRate { get; set; } = 1f;

    /// <summary>Internal flag: playback rate already applied to lockAnimTime.</summary>
    public bool PlayBackRateCalculated { get; set; }
}
