using Sandbox;

namespace VJBase;

// ═══ Animation Type — VJ.ANIM_TYPE_* from enums.lua:128-132 ═══
public enum VJAnimType
{
    None     = 0,  // No type / fail / reset
    Activity = 1,  // Animation is an ACT_* activity
    Sequence = 2,  // Animation is a vjseq_* sequence
    Gesture  = 3,  // Animation is a vjges_* gesture
}

// ═══ Model Animation Set — VJ.ANIM_SET_* from enums.lua:135-140 ═══
public enum VJAnimSet
{
    None     = 0,  // No model animation set detected
    Combine  = 1,  // HL2 Combine soldier / elite
    Metrocop = 2,  // HL2 Metropolice
    Rebel    = 3,  // HL2 citizen / rebel
    Player   = 4,  // Default player model
    Custom   = 10, // Developer-defined custom model set
}

// ═══ Activity Constants — Source Engine ACT_* (~175 values) ═══
// Mapped from lua/ entities; grouped by semantic category.
// Values start from 0 for dictionary-key use; original Source ints are not preserved.
public enum Activity
{
    // ── Special ──
    Invalid    = 0,   // ACT_INVALID — sentinel for "no valid animation"
    Reset      = 1,   // ACT_RESET
    Transition = 2,   // ACT_TRANSITION

    // ── Idle ──
    Idle                     = 10,
    IdleAgitated             = 11,  // ACT_IDLE_AGITATED
    IdleStimulated           = 12,  // ACT_IDLE_STIMULATED
    IdleRelaxed              = 13,  // ACT_IDLE_RELAXED
    IdleAngry                = 14,  // ACT_IDLE_ANGRY
    IdleAngryMelee           = 15,  // ACT_IDLE_ANGRY_MELEE
    IdleAngryPistol          = 16,  // ACT_IDLE_ANGRY_PISTOL
    IdleAngryRpg             = 17,  // ACT_IDLE_ANGRY_RPG
    IdleAngryShotgun         = 18,  // ACT_IDLE_ANGRY_SHOTGUN
    IdleAngrySmg1            = 19,  // ACT_IDLE_ANGRY_SMG1
    IdleAimRelaxed           = 20,  // ACT_IDLE_AIM_RELAXED
    IdleAimStimulated        = 21,  // ACT_IDLE_AIM_STIMULATED
    IdleAimAgitated          = 22,  // ACT_IDLE_AIM_AGITATED
    IdleAimRifleStimulated   = 23,  // ACT_IDLE_AIM_RIFLE_STIMULATED
    IdlePistol               = 24,  // ACT_IDLE_PISTOL
    IdleRpg                  = 25,  // ACT_IDLE_RPG
    IdleRpgRelaxed           = 26,  // ACT_IDLE_RPG_RELAXED
    IdleShotgunAgitated      = 27,  // ACT_IDLE_SHOTGUN_AGITATED
    IdleShotgunRelaxed       = 28,  // ACT_IDLE_SHOTGUN_RELAXED
    IdleShotgunStimulated    = 29,  // ACT_IDLE_SHOTGUN_STIMULATED
    IdleSmg1                 = 30,  // ACT_IDLE_SMG1
    IdleSmg1Relaxed          = 31,  // ACT_IDLE_SMG1_RELAXED
    IdleSmg1Stimulated       = 32,  // ACT_IDLE_SMG1_STIMULATED
    ShotgunIdleDeep          = 33,  // ACT_SHOTGUN_IDLE_DEEP
    Cower                    = 34,  // ACT_COWER
    CrouchIdle               = 35,  // ACT_CROUCHIDLE
    DoNotDisturb             = 36,  // ACT_DO_NOT_DISTURB
    BusySitGround            = 37,  // ACT_BUSY_SIT_GROUND

    // ── Movement: Walk ──
    Walk                  = 40,
    WalkAgitated          = 41,  // ACT_WALK_AGITATED
    WalkStimulated        = 42,  // ACT_WALK_STIMULATED
    WalkRelaxed           = 43,  // ACT_WALK_RELAXED
    WalkAngry             = 44,  // ACT_WALK_ANGRY
    WalkAim               = 45,  // ACT_WALK_AIM
    WalkAimAgitated       = 46,  // ACT_WALK_AIM_AGITATED
    WalkAimRelaxed        = 47,  // ACT_WALK_AIM_RELAXED
    WalkAimStimulated     = 48,  // ACT_WALK_AIM_STIMULATED
    WalkAimRifle          = 49,  // ACT_WALK_AIM_RIFLE
    WalkAimRifleStimulated = 50, // ACT_WALK_AIM_RIFLE_STIMULATED
    WalkAimPistol         = 51,  // ACT_WALK_AIM_PISTOL
    WalkAimShotgun        = 52,  // ACT_WALK_AIM_SHOTGUN
    WalkRifle             = 53,  // ACT_WALK_RIFLE
    WalkRifleRelaxed      = 54,  // ACT_WALK_RIFLE_RELAXED
    WalkRifleStimulated   = 55,  // ACT_WALK_RIFLE_STIMULATED
    WalkPistol            = 56,  // ACT_WALK_PISTOL
    WalkRpg               = 57,  // ACT_WALK_RPG
    WalkRpgRelaxed        = 58,  // ACT_WALK_RPG_RELAXED
    WalkCrouch            = 59,  // ACT_WALK_CROUCH
    WalkCrouchAim         = 60,  // ACT_WALK_CROUCH_AIM
    WalkCrouchAimRifle    = 61,  // ACT_WALK_CROUCH_AIM_RIFLE
    WalkCrouchRifle       = 62,  // ACT_WALK_CROUCH_RIFLE
    WalkCrouchRpg         = 63,  // ACT_WALK_CROUCH_RPG

    // ── Movement: Run ──
    Run                     = 70,
    RunAgitated             = 71,  // ACT_RUN_AGITATED
    RunStimulated           = 72,  // ACT_RUN_STIMULATED
    RunRelaxed              = 73,  // ACT_RUN_RELAXED
    RunAim                  = 74,  // ACT_RUN_AIM
    RunAimAgitated          = 75,  // ACT_RUN_AIM_AGITATED
    RunAimRelaxed           = 76,  // ACT_RUN_AIM_RELAXED
    RunAimStimulated        = 77,  // ACT_RUN_AIM_STIMULATED
    RunAimRifle             = 78,  // ACT_RUN_AIM_RIFLE
    RunAimRifleStimulated   = 79,  // ACT_RUN_AIM_RIFLE_STIMULATED
    RunAimPistol            = 80,  // ACT_RUN_AIM_PISTOL
    RunAimShotgun           = 81,  // ACT_RUN_AIM_SHOTGUN
    RunRifle                = 82,  // ACT_RUN_RIFLE
    RunRifleRelaxed         = 83,  // ACT_RUN_RIFLE_RELAXED
    RunRifleStimulated      = 84,  // ACT_RUN_RIFLE_STIMULATED
    RunPistol               = 85,  // ACT_RUN_PISTOL
    RunRpg                  = 86,  // ACT_RUN_RPG
    RunRpgRelaxed           = 87,  // ACT_RUN_RPG_RELAXED
    RunProtected            = 88,  // ACT_RUN_PROTECTED
    RunCrouch               = 89,  // ACT_RUN_CROUCH
    RunCrouchAim            = 90,  // ACT_RUN_CROUCH_AIM
    RunCrouchAimRifle       = 91,  // ACT_RUN_CROUCH_AIM_RIFLE
    RunCrouchRifle          = 92,  // ACT_RUN_CROUCH_RIFLE
    RunCrouchRpg            = 93,  // ACT_RUN_CROUCH_RPG

    // ── Movement: Special ──
    Fly      = 100, // ACT_FLY
    Swim     = 101, // ACT_SWIM
    Glide    = 102, // ACT_GLIDE
    Jump     = 103, // ACT_JUMP
    Land     = 104, // ACT_LAND
    ClimbUp  = 105, // ACT_CLIMB_UP

    // ── Combat: Melee ──
    MeleeAttack1      = 110, // ACT_MELEE_ATTACK1
    MeleeAttackSwing  = 111, // ACT_MELEE_ATTACK_SWING

    // ── Combat: Range ──
    RangeAttack1                = 120, // ACT_RANGE_ATTACK1
    RangeAttack1Low             = 121, // ACT_RANGE_ATTACK1_LOW
    RangeAttack2                = 122, // ACT_RANGE_ATTACK2
    RangeAttackAr2              = 123, // ACT_RANGE_ATTACK_AR2
    RangeAttackAr2Low           = 124, // ACT_RANGE_ATTACK_AR2_LOW
    RangeAttackPistol           = 125, // ACT_RANGE_ATTACK_PISTOL
    RangeAttackPistolLow        = 126, // ACT_RANGE_ATTACK_PISTOL_LOW
    RangeAttackShotgun          = 127, // ACT_RANGE_ATTACK_SHOTGUN
    RangeAttackShotgunLow       = 128, // ACT_RANGE_ATTACK_SHOTGUN_LOW
    RangeAttackSmg1             = 129, // ACT_RANGE_ATTACK_SMG1
    RangeAttackSmg1Low          = 130, // ACT_RANGE_ATTACK_SMG1_LOW
    RangeAttackRpg              = 131, // ACT_RANGE_ATTACK_RPG
    RangeAimLow                 = 132, // ACT_RANGE_AIM_LOW
    RangeAimAr2Low              = 133, // ACT_RANGE_AIM_AR2_LOW
    RangeAimPistolLow           = 134, // ACT_RANGE_AIM_PISTOL_LOW
    RangeAimSmg1Low             = 135, // ACT_RANGE_AIM_SMG1_LOW

    // ── Combat: Special Attack ──
    SpecialAttack1 = 140, // ACT_SPECIAL_ATTACK1

    // ── Reload ──
    Reload           = 150, // ACT_RELOAD
    ReloadLow        = 151, // ACT_RELOAD_LOW
    ReloadPistol     = 152, // ACT_RELOAD_PISTOL
    ReloadPistolLow  = 153, // ACT_RELOAD_PISTOL_LOW
    ReloadShotgun    = 154, // ACT_RELOAD_SHOTGUN
    ReloadShotgunLow = 155, // ACT_RELOAD_SHOTGUN_LOW
    ReloadSmg1       = 156, // ACT_RELOAD_SMG1
    ReloadSmg1Low    = 157, // ACT_RELOAD_SMG1_LOW
    ShotgunPump      = 158, // ACT_SHOTGUN_PUMP

    // ── Cover ──
    Cover          = 160, // ACT_COVER
    CoverLow       = 161, // ACT_COVER_LOW
    CoverLowRpg    = 162, // ACT_COVER_LOW_RPG
    CoverPistolLow = 163, // ACT_COVER_PISTOL_LOW
    CoverSmg1Low   = 164, // ACT_COVER_SMG1_LOW

    // ── Flinch ──
    FlinchPhysics  = 170, // ACT_FLINCH_PHYSICS
    FlinchHead     = 171, // ACT_FLINCH_HEAD
    FlinchLeftArm  = 172, // ACT_FLINCH_LEFTARM
    FlinchLeftLeg  = 173, // ACT_FLINCH_LEFTLEG
    FlinchRightArm = 174, // ACT_FLINCH_RIGHTARM
    FlinchRightLeg = 175, // ACT_FLINCH_RIGHTLEG

    // ── ViewModel (weapon viewmodel animations) ──
    VmDraw          = 180, // ACT_VM_DRAW
    VmHolster       = 181, // ACT_VM_HOLSTER
    VmIdle          = 182, // ACT_VM_IDLE
    VmFidget        = 183, // ACT_VM_FIDGET
    VmPrimaryAttack = 184, // ACT_VM_PRIMARYATTACK
    VmSecondaryAttack = 185, // ACT_VM_SECONDARYATTACK
    VmReload        = 186, // ACT_VM_RELOAD
    VmIdleToLowered = 187, // ACT_VM_IDLE_TO_LOWERED

    // ── Gesture (weapon firing gesture overlays) ──
    GestureRangeAttack1        = 190, // ACT_GESTURE_RANGE_ATTACK1
    GestureRangeAttackAr2      = 191, // ACT_GESTURE_RANGE_ATTACK_AR2
    GestureRangeAttackPistol   = 192, // ACT_GESTURE_RANGE_ATTACK_PISTOL
    GestureRangeAttackRpg      = 193, // ACT_GESTURE_RANGE_ATTACK_RPG
    GestureRangeAttackShotgun  = 194, // ACT_GESTURE_RANGE_ATTACK_SHOTGUN
    GestureRangeAttackSmg1     = 195, // ACT_GESTURE_RANGE_ATTACK_SMG1
    GestureReload              = 196, // ACT_GESTURE_RELOAD
    GestureReloadPistol        = 197, // ACT_GESTURE_RELOAD_PISTOL
    GestureReloadShotgun       = 198, // ACT_GESTURE_RELOAD_SHOTGUN
    GestureReloadSmg1          = 199, // ACT_GESTURE_RELOAD_SMG1
    MeleeAttackSwingGesture    = 200, // ACT_MELEE_ATTACK_SWING_GESTURE

    // ── Signal ──
    SignalAdvance = 210, // ACT_SIGNAL_ADVANCE
    SignalForward = 211, // ACT_SIGNAL_FORWARD
    SignalGroup   = 212, // ACT_SIGNAL_GROUP

    // ── Misc ──
    Dmg        = 220, // ACT_DMG
    Disarm     = 221, // ACT_DISARM
    Arm        = 222, // ACT_ARM
    PlayActivity = 223, // ACT_PLAYACTIVITY
    ToSource   = 224, // ACT_TO_SOURCE
    TurnLeft   = 225, // ACT_TURN_LEFT
    TurnRight  = 226, // ACT_TURN_RIGHT
    PoliceHarass1 = 227, // ACT_POLICE_HARASS1

    // ── HL2MP (Half-Life 2 Multiplayer player model animations) ──
    Hl2mpIdle                     = 300, // ACT_HL2MP_IDLE
    Hl2mpIdleAngry                = 301, // ACT_HL2MP_IDLE_ANGRY
    Hl2mpIdleCower                = 302, // ACT_HL2MP_IDLE_COWER
    Hl2mpIdlePassive              = 303, // ACT_HL2MP_IDLE_PASSIVE
    Hl2mpRun                      = 304, // ACT_HL2MP_RUN
    Hl2mpRunFast                  = 305, // ACT_HL2MP_RUN_FAST
    Hl2mpRunProtected             = 306, // ACT_HL2MP_RUN_PROTECTED
    Hl2mpRunPassive               = 307, // ACT_HL2MP_RUN_PASSIVE
    Hl2mpWalk                     = 308, // ACT_HL2MP_WALK
    Hl2mpWalkCrouch               = 309, // ACT_HL2MP_WALK_CROUCH
    Hl2mpWalkPassive              = 310, // ACT_HL2MP_WALK_PASSIVE
    Hl2mpWalkCrouchPassive        = 311, // ACT_HL2MP_WALK_CROUCH_PASSIVE

    // HL2MP weapon-specific idles
    Hl2mpIdleAr2        = 320, Hl2mpIdleCamera = 321, Hl2mpIdleCrossbow = 322,
    Hl2mpIdleDuel       = 323, Hl2mpIdleGrenade = 324, Hl2mpIdleKnife   = 325,
    Hl2mpIdleMelee      = 326, Hl2mpIdleMelee2  = 327, Hl2mpIdlePhysgun = 328,
    Hl2mpIdlePistol     = 329, Hl2mpIdleRevolver = 330, Hl2mpIdleRpg    = 331,
    Hl2mpIdleShotgun    = 332, Hl2mpIdleSlam     = 333, Hl2mpIdleSmg1   = 334,

    // HL2MP weapon-specific crouch idles
    Hl2mpIdleCrouchAr2     = 340, Hl2mpIdleCrouchCamera   = 341, Hl2mpIdleCrouchCrossbow = 342,
    Hl2mpIdleCrouchDuel    = 343, Hl2mpIdleCrouchGrenade  = 344, Hl2mpIdleCrouchKnife   = 345,
    Hl2mpIdleCrouchMelee   = 346, Hl2mpIdleCrouchMelee2   = 347, Hl2mpIdleCrouchPhysgun = 348,
    Hl2mpIdleCrouchPistol  = 349, Hl2mpIdleCrouchRevolver = 350, Hl2mpIdleCrouchRpg    = 351,
    Hl2mpIdleCrouchShotgun = 352, Hl2mpIdleCrouchSlam     = 353, Hl2mpIdleCrouchSmg1   = 354,

    // HL2MP weapon-specific runs
    Hl2mpRunAr2      = 360, Hl2mpRunCamera   = 361, Hl2mpRunCrossbow = 362,
    Hl2mpRunDuel     = 363, Hl2mpRunGrenade  = 364, Hl2mpRunKnife   = 365,
    Hl2mpRunMelee    = 366, Hl2mpRunMelee2   = 367, Hl2mpRunPhysgun = 368,
    Hl2mpRunPistol   = 369, Hl2mpRunRevolver = 370, Hl2mpRunRpg     = 371,
    Hl2mpRunShotgun  = 372, Hl2mpRunSlam     = 373, Hl2mpRunSmg1    = 374,

    // HL2MP weapon-specific walks
    Hl2mpWalkAr2      = 380, Hl2mpWalkCamera   = 381, Hl2mpWalkCrossbow = 382,
    Hl2mpWalkDuel     = 383, Hl2mpWalkGrenade  = 384, Hl2mpWalkKnife   = 385,
    Hl2mpWalkMelee    = 386, Hl2mpWalkMelee2   = 387, Hl2mpWalkPhysgun = 388,
    Hl2mpWalkPistol   = 389, Hl2mpWalkRevolver = 390, Hl2mpWalkRpg     = 391,
    Hl2mpWalkShotgun  = 392, Hl2mpWalkSlam     = 393, Hl2mpWalkSmg1    = 394,

    // HL2MP weapon-specific crouch walks
    Hl2mpWalkCrouchAr2     = 400, Hl2mpWalkCrouchCamera   = 401, Hl2mpWalkCrouchCrossbow = 402,
    Hl2mpWalkCrouchDuel    = 403, Hl2mpWalkCrouchGrenade  = 404, Hl2mpWalkCrouchKnife   = 405,
    Hl2mpWalkCrouchMelee   = 406, Hl2mpWalkCrouchMelee2   = 407, Hl2mpWalkCrouchPhysgun = 408,
    Hl2mpWalkCrouchPistol  = 409, Hl2mpWalkCrouchRevolver = 410, Hl2mpWalkCrouchRpg    = 411,
    Hl2mpWalkCrouchShotgun = 412, Hl2mpWalkCrouchSlam     = 413, Hl2mpWalkCrouchSmg1   = 414,

    // HL2MP weapon-specific jumps
    Hl2mpJumpAr2      = 420, Hl2mpJumpCamera   = 421, Hl2mpJumpCrossbow = 422,
    Hl2mpJumpDuel     = 423, Hl2mpJumpGrenade  = 424, Hl2mpJumpKnife   = 425,
    Hl2mpJumpMelee    = 426, Hl2mpJumpMelee2   = 427, Hl2mpJumpPhysgun = 428,
    Hl2mpJumpPistol   = 429, Hl2mpJumpRevolver = 430, Hl2mpJumpRpg     = 431,
    Hl2mpJumpShotgun  = 432, Hl2mpJumpSlam     = 433, Hl2mpJumpSmg1    = 434,

    // HL2MP gestures
    Hl2mpGestureRangeAttackAr2      = 440, Hl2mpGestureRangeAttackCamera   = 441,
    Hl2mpGestureRangeAttackCrossbow = 442, Hl2mpGestureRangeAttackDuel     = 443,
    Hl2mpGestureRangeAttackGrenade  = 444, Hl2mpGestureRangeAttackKnife    = 445,
    Hl2mpGestureRangeAttackMelee    = 446, Hl2mpGestureRangeAttackMelee2   = 447,
    Hl2mpGestureRangeAttackPistol   = 448, Hl2mpGestureRangeAttackRevolver = 449,
    Hl2mpGestureRangeAttackRpg      = 450, Hl2mpGestureRangeAttackShotgun  = 451,
    Hl2mpGestureRangeAttackSlam     = 452, Hl2mpGestureRangeAttackSmg1     = 453,
}
