using Sandbox;

namespace VJBase;

// ═══ VJ Custom Values ═══
public static class VJCustom
{
    public const int DmgBleed       = 123454;
    public const int DmgForceFlinch = 123455;
}

// ═══ VJ Damage Tags — Source DMG_* → S&Box DamageInfo.Tags ═══
public static class VJDamageTags
{
    // Primary damage types
    public const string Slash   = "vj.slash";
    public const string Club    = "vj.club";
    public const string Generic = "vj.generic";
    public const string Bullet  = "vj.bullet";
    public const string Burn    = "vj.burn";
    public const string Blast   = "vj.blast";
    public const string Explosion = "vj.explosion";
    public const string Shock    = "vj.shock";
    public const string Poison   = "vj.poison";
    // Special types
    public const string Bleed        = "vj.bleed";
    public const string Physgun      = "vj.physgun";
    public const string ForceFlinch  = "vj.force_flinch";
    public const string CrossbowBolt = "vj.crossbow_bolt";
}

// ═══ Source Engine NPCState — VJ calls SetNPCState(NPC_STATE_COMBAT) in ForceSetEnemy ═══
public enum NPCState { None = 0, Idle = 1, Alert = 2, Combat = 3 }

// ═══ Disposition (D_* engine constants + VJ custom) ═══
public enum Disposition { Error = 0, Like = 1, Neutral = 2, Hate = 3, Fear = 4, Interest = 100 }

// ═══ Relationship Class (CLASS_* string constants) ═══
public static class RelationshipClass
{
    public const string PlayerAlly  = "CLASS_PLAYER_ALLY";
    public const string Combine     = "CLASS_COMBINE";
    public const string Zombie      = "CLASS_ZOMBIE";
    public const string Antlion     = "CLASS_ANTLION";
    public const string Xen         = "CLASS_XEN";
    public const string BlackOps    = "CLASS_BLACKOPS";
    public const string UnitedStates = "CLASS_UNITED_STATES";
    public const string Aperture    = "CLASS_APERTURE";
    public const string VJBase      = "CLASS_VJ_BASE";
}

// ═══ Navigation Type (NAV_* Source engine builtins) ═══
public enum NavType { Ground, Fly, None, Jump, Climb }

// ═══ Movement Type ═══
public enum VJMoveType { Ground = 1, Aerial = 2, Aquatic = 3, Stationary = 4, Physics = 5 }

// ═══ Behavior Type ═══
public enum VJBehavior { Aggressive = 1, Neutral = 2, Passive = 3, PassiveNature = 4 }

// ═══ AI State (VJ custom, not Source NPC_STATE) ═══
public enum VJState { None = 0, Freeze = 1, OnlyAnimation = 2, OnlyAnimationConstant = 3, OnlyAnimationNoAttack = 4 }

// ═══ Attack ═══
public enum VJAttackType { None = 0, Custom = 1, Melee = 2, Range = 3, Leap = 4, Grenade = 5 }
public enum VJAttackState { None = 0, Done = 1, Started = 2, Executed = 3, ExecutedHit = 4 }

// ═══ Facing ═══
public enum VJFaceStatus { None = 0, Enemy = 1, EnemyVisible = 2, Entity = 3, EntityVisible = 4, Position = 5, PositionVisible = 6 }

// ═══ Alert ═══
public enum VJAlertState { None = 0, Ready = 1, Enemy = 2 }

// ═══ Danger ═══
public enum VJDangerType { Entity = 1, Grenade = 2, Hint = 3 }

// ═══ Weapon ═══
public enum VJWepState { Ready = 0, Holstered = 1, Reloading = 2 }
public enum VJWepAttackState { None = 0, Aim = 1, AimMove = 2, AimOcclusion = 3, Fire = 10, FireStand = 11 }
public enum VJWepInventory { None = 0, Primary = 1, Secondary = 2, Melee = 3, AntiArmor = 4 }

// ═══ Animation ═══
public enum VJAnimType { None = 0, Activity = 1, Sequence = 2, Gesture = 3 }
public enum VJAnimSet { None = 0, Combine = 1, Metrocop = 2, Rebel = 3, Player = 4, Custom = 10 }

// ═══ Blood ═══
public enum VJBloodColor { None = 0, Red, Yellow, Green, Orange, Blue, Purple, White, Oil }

// ═══ Difficulty ═══
public enum VJDifficulty { Neanderthal = -5, Puny = -4, Trivial = -3, Easy = -2, Beginner = -1, Normal = 0, Difficult = 1, Hard = 2, Expert = 3, Insane = 4, Impossible = 5, Lunatic = 6, Nightmare = 7, HellOnEarth = 8, TotalAnnihilation = 9, Extinction = 10 }

// ═══ Projectile ═══
public enum VJProjType { Linear = 0, Gravity = 1, Prop = 2 }
public enum VJProjCollision { None = 0, Remove = 1, Persist = 2 }

// ═══ Kill Icon ═══
public enum VJKillIconType { Font = -2, Alias = -1 }
public static class VJKillIcon
{
    public const string Default    = "HUD/killicons/default";
    public const string Projectile = "vj_base/hud/range.png";
    public const string Grenade    = "vj_base/hud/grenade.png";
}

// ═══ Colors ═══
public static class VJColors
{
    public static readonly Color LogoOrange      = new(244, 102, 34);
    public static readonly Color LogoOrangeLight = new(255, 163, 121);
    public static readonly Color Server          = new(156, 241, 255, 200);
    public static readonly Color Client          = new(255, 241, 122, 200);
    public static readonly Color Black           = new(0, 0, 0);
    public static readonly Color White           = new(255, 255, 255);
    public static readonly Color Red             = new(255, 0, 0);
    public static readonly Color RedLight        = new(255, 130, 130);
    public static readonly Color Green           = new(0, 255, 0);
    public static readonly Color Blue            = new(0, 0, 255);
    public static readonly Color BlueSky         = new(135, 206, 235);
    public static readonly Color Cyan            = new(0, 255, 255);
    public static readonly Color Yellow          = new(255, 255, 0);
    public static readonly Color Orange          = new(255, 165, 0);
    public static readonly Color OrangeVivid     = new(255, 100, 0);
    public static readonly Color Purple          = new(128, 0, 128);
    public static readonly Color Pink            = new(255, 192, 203);
}

// ═══ Memory Keys ═══
public static class VJMemoryKey
{
    public const string OverrideDisposition = "override_disposition";
    public const string OverridePriority    = "override_priority";
    public const string HostilityLevel      = "hostility";
    public const string CacheClasses        = "cache_classes";
    public const string CacheDisposition    = "cache_disposition";
    public const string CacheEntType        = "cache_ent_type";
}

// ═══ Source Engine Conditions (COND_*) ═══
public enum Condition
{
    None = 0,  InPVS = 1,  IdleInterrupt = 2,
    LowPrimaryAmmo = 3,  NoPrimaryAmmo = 4,  NoSecondaryAmmo = 5,
    NoWeapon = 6,  SeeHate = 7,  SeeFear = 8,
    SeeDislike = 9,  SeeEnemy = 10,  LostEnemy = 11,
    EnemyWentNull = 12,  EnemyOccluded = 13,  TargetOccluded = 14,
    HaveEnemyLOS = 15,  HaveTargetLOS = 16,
    LightDamage = 17,  HeavyDamage = 18,  PhysicsDamage = 19,
    RepeatedDamage = 20,  CanRangeAttack1 = 21,  CanRangeAttack2 = 22,
    CanMeleeAttack1 = 23,  CanMeleeAttack2 = 24,  Provoked = 25,
    NewEnemy = 26,  EnemyTooFar = 27,  EnemyFacingMe = 28,
    BehindEnemy = 29,  EnemyDead = 30,  EnemyUnreachable = 31,
    SeePlayer = 32,  LostPlayer = 33,  SeeNemesis = 34,
    TaskFailed = 35,  ScheduleDone = 36,  Smell = 37,
    TooCloseToAttack = 38,  TooFarToAttack = 39,
    NotFacingAttack = 40,  WeaponHasLOS = 41,
    WeaponBlockedByFriend = 42,  WeaponPlayerInSpread = 43,
    WeaponPlayerNearTarget = 44,  WeaponSightOccluded = 45,
    BetterWeaponAvailable = 46,  HealthItemAvailable = 47,
    GiveWay = 48,  WayClear = 49,
    HearDanger = 50,  HearThumper = 51,  HearBugbait = 52,
    HearCombat = 53,  HearWorld = 54,  HearPlayer = 55,
    HearBulletImpact = 56,  HearPhysicsDanger = 57,
    HearMoveAway = 58,  HearSpooky = 59,  NoHearDanger = 60,
    FloatingOffGround = 61,  MobbedByEnemies = 62,
    ReceivedOrders = 63,
    PlayerAddedToSquad = 64,  PlayerRemovedFromSquad = 65,
    PlayerPushing = 66,  NPCFreeze = 67,  NPCUnfreeze = 68,
    TalkerRespondToQuestion = 69,  NoCustomInterrupts = 70
}
