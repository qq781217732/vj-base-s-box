#!/usr/bin/env python3
"""
Cross-reference the API mapping document against actual Lua source files.
Finds: hallucinated methods, missing constants, wrong enum values, etc.
"""
import re, os, sys
from collections import defaultdict
from pathlib import Path

LUA_DIR = Path(r"f:\DevProject\Sbox\VJ-Base-master")

# Gather ALL .lua file contents
lua_files = {}
for f in LUA_DIR.rglob("*.lua"):
    try:
        lua_files[str(f)] = f.read_text(encoding="utf-8", errors="replace")
    except:
        pass

ALL_LUA = "\n".join(lua_files.values())

def search_lua(pattern, capture=False):
    """Search all Lua source. Returns list of matches or True/False."""
    if capture:
        return re.findall(pattern, ALL_LUA, re.IGNORECASE)
    return bool(re.search(pattern, ALL_LUA, re.IGNORECASE))

# ============================================================
# 1. IEngineEntity methods — check if called in Lua as self:Method or ent:Method
# ============================================================
print("=" * 70)
print("SECTION 1: IEngineEntity methods — checking Lua usage")
print("=" * 70)

entity_methods = [
    # Transform
    ("GetPos", "Sw"), ("SetPos", "Sw"), ("GetAngles", "Sw"), ("SetAngles", "Sw"),
    ("GetForward", "Sw"), ("GetRight", "Sw"), ("GetUp", "Sw"),
    ("GetVelocity", "Sw"), ("SetVelocity", "M"),
    ("EyePos", "C"), ("WorldSpaceCenter", "Sw"), ("OBBCenter", "Sw"),
    ("NearestPoint", "Sw"), ("BoundingRadius", "Sw"),
    # Lifecycle
    ("Spawn", "Sw"), ("Remove", "Sw"),
    # Model/Appearance
    ("SetModel", "Sw"), ("GetModel", "Sw"), ("SetColor", "Sw"),
    ("SetMaterial", "Sw"), ("SetModelScale", "Sw"),
    ("SetBodygroup", "Sw"), ("SetSkin", "Sw"),
    # Type/Identity
    ("GetClass", "Sw"), ("EntIndex", "Sw"), ("GetName", "Sw"), ("SetName", "Sw"),
    ("IsPlayer", "Sw"), ("IsNPC", "Sw"),
    # Parent/Owner
    ("SetParent", "Sw"), ("GetParent", "Sw"),
    ("SetOwner", "C"), ("GetOwner", "C"),
    # Collision/Physics
    ("SetCollisionGroup", "Sw"), ("GetCollisionGroup", "Sw"),
    ("SetSolid", "Sw"), ("GetSolid", "Sw"),
    ("GetPhysicsObject", "Sw"), ("IsOnGround", "Sw"),
    ("WaterLevel", "C"),
    # Health/Damage
    ("Health", "C"), ("SetHealth", "C"),
    ("GetMaxHealth", "C"), ("SetMaxHealth", "C"),
    ("Alive", "C"), ("TakeDamage", "Sw"),
    ("Ignite", "C"), ("Extinguish", "C"),
    # Flags/Save
    ("IsFlagSet", "M"), ("AddFlags", "M"), ("RemoveFlags", "M"),
    ("SetSaveValue", "M"), ("GetSaveValue", "M"),
    # Animation
    ("SetMovementActivity", "M"), ("GetMovementActivity", "M"),
    ("GetSequence", "M"), ("SetSequence", "M"),
    ("GetSequenceName", "M"), ("GetSequenceActivity", "M"),
    ("IsSequenceFinished", "M"), ("GetCycle", "M"), ("SetCycle", "M"),
    ("SequenceDuration", "M"), ("GetSequenceMoveDist", "M"),
    ("LookupSequence", "M"), ("FrameAdvance", "M"),
    ("AutoMovement", "M"), ("SetPlaybackRate", "M"), ("GetPlaybackRate", "M"),
    ("SetPoseParameter", "M"), ("GetPoseParameter", "M"),
    ("LookupAttachment", "M"), ("GetAttachment", "M"),
]

missing_methods = []
found_methods = []
for method, tag in entity_methods:
    # Search for :MethodName( calls
    found = search_lua(rf':{re.escape(method)}\s*\(')
    if found:
        found_methods.append(method)
    else:
        missing_methods.append(method)

print(f"\nFOUND in Lua ({len(found_methods)}/{len(entity_methods)}):")
for m in found_methods:
    print(f"  OK  {m}")

print(f"\nNOT FOUND in Lua ({len(missing_methods)}/{len(entity_methods)}):")
for m in missing_methods:
    print(f"  ??  {m}")

# ============================================================
# 2. TASK_* constants
# ============================================================
print("\n" + "=" * 70)
print("SECTION 2: TASK_* constants — checking Lua references")
print("=" * 70)

mapping_tasks = [
    "TASK_GET_PATH_TO_LASTPOSITION", "TASK_GET_PATH_TO_TARGET",
    "TASK_GET_PATH_TO_ENEMY", "TASK_GET_PATH_TO_ENEMY_LOS",
    "TASK_GET_PATH_TO_RANDOM_NODE",
    "TASK_RUN_PATH", "TASK_WALK_PATH", "TASK_RUN_PATH_FLEE",
    "TASK_RUN_PATH_TIMED", "TASK_WALK_PATH_TIMED",
    "TASK_RUN_PATH_FOR_UNITS", "TASK_WALK_PATH_FOR_UNITS",
    "TASK_RUN_PATH_WITHIN_DIST", "TASK_WALK_PATH_WITHIN_DIST",
    "TASK_WEAPON_RUN_PATH", "TASK_ITEM_RUN_PATH",
    "TASK_MOVE_TO_TARGET_RANGE", "TASK_MOVE_TO_GOAL_RANGE",
    "TASK_MOVE_AWAY_PATH",
    "TASK_FACE_TARGET", "TASK_FACE_ENEMY", "TASK_FACE_PLAYER",
    "TASK_FACE_LASTPOSITION", "TASK_FACE_SAVEPOSITION",
    "TASK_FACE_PATH", "TASK_FACE_HINTNODE",
    "TASK_FACE_IDEAL", "TASK_FACE_REASONABLE",
    "TASK_FIND_COVER_FROM_ORIGIN", "TASK_FIND_COVER_FROM_ENEMY",
    "TASK_FIND_COVER_FROM_BEST_SOUND",
    "TASK_WAIT", "TASK_WAIT_FOR_MOVEMENT",
    "TASK_SET_TOLERANCE_DISTANCE", "TASK_SET_ROUTE_SEARCH_TIME",
    "TASK_STOP_MOVING", "TASK_FORGET", "TASK_IGNORE_OLD_ENEMIES",
    "TASK_STORE_BESTSOUND_REACTORIGIN_IN_SAVEPOSITION",
    "TASK_PLAY_SEQUENCE", "TASK_PLAY_SEQUENCE_FACE_ENEMY",
    "TASK_SET_ACTIVITY", "TASK_RESET_ACTIVITY",
    "TASK_VJ_PLAY_ACTIVITY", "TASK_VJ_PLAY_SEQUENCE",
]

found_tasks = []
missing_tasks = []
for task in mapping_tasks:
    found = search_lua(re.escape(task))
    if found:
        found_tasks.append(task)
    else:
        missing_tasks.append(task)

print(f"\nFOUND in Lua ({len(found_tasks)}/{len(mapping_tasks)}):")
for t in found_tasks:
    print(f"  OK  {t}")

print(f"\nNOT FOUND in Lua ({len(missing_tasks)}/{len(mapping_tasks)}):")
for t in missing_tasks:
    print(f"  ??  {t}")

# ============================================================
# 3. COND_* (Condition) enums
# ============================================================
print("\n" + "=" * 70)
print("SECTION 3: Condition enums — checking Lua definitions & usage")
print("=" * 70)

# These are the ones from the API mapping document (as C# enum values)
mapping_conditions = {
    "None": 0, "InPVS": 1, "IdleInterrupt": 2,
    "LowPrimaryAmmo": 3, "NoPrimaryAmmo": 4, "NoSecondaryAmmo": 5,
    "NoWeapon": 6, "SeeHate": 7, "SeeFear": 8,
    "SeeDislike": 9, "SeeEnemy": 10, "LostEnemy": 11,
    "EnemyWentNull": 12, "EnemyOccluded": 13, "TargetOccluded": 14,
    "HaveEnemyLOS": 15, "HaveTargetLOS": 16,
    "LightDamage": 17, "HeavyDamage": 18, "PhysicsDamage": 19,
    "RepeatedDamage": 20, "CanRangeAttack1": 21, "CanRangeAttack2": 22,
    "CanMeleeAttack1": 23, "CanMeleeAttack2": 24, "Provoked": 25,
    "NewEnemy": 26, "EnemyTooFar": 27, "EnemyFacingMe": 28,
    "BehindEnemy": 29, "EnemyDead": 30, "EnemyUnreachable": 31,
    "SeePlayer": 32, "LostPlayer": 33, "SeeNemesis": 34,
    "TaskFailed": 35, "ScheduleDone": 36, "Smell": 37,
    "TooCloseToAttack": 38, "TooFarToAttack": 39,
    "NotFacingAttack": 40, "WeaponHasLOS": 41,
    "WeaponBlockedByFriend": 42, "WeaponPlayerInSpread": 43,
    "WeaponPlayerNearTarget": 44, "WeaponSightOccluded": 45,
    "BetterWeaponAvailable": 46, "HealthItemAvailable": 47,
    "GiveWay": 48, "WayClear": 49,
    "HearDanger": 50, "HearThumper": 51, "HearBugbait": 52,
    "HearCombat": 53, "HearWorld": 54, "HearPlayer": 55,
    "HearBulletImpact": 56, "HearPhysicsDanger": 57,
    "HearMoveAway": 58, "HearSpooky": 59, "NoHearDanger": 60,
    "FloatingOffGround": 61, "MobbedByEnemies": 62,
    "ReceivedOrders": 63,
    "PlayerAddedToSquad": 64, "PlayerRemovedFromSquad": 65,
    "PlayerPushing": 66, "NPCFreeze": 67, "NPCUnfreeze": 68,
    "TalkerRespondToQuestion": 69, "NoCustomInterrupts": 70,
}

# Extract actual COND_* definitions from Lua
lua_conditions = {}
for match in re.finditer(r'COND_(\w+)\s*=\s*(\d+)', ALL_LUA):
    lua_conditions[match.group(1)] = int(match.group(2))

# Map C# PascalCase names to Lua UPPER_CASE names
def pascal_to_cond(name):
    """Convert PascalCase to COND_EXPECTED_NAME"""
    # Special mappings known from Source SDK
    special = {
        "InPVS": "IN_PVS", "IdleInterrupt": "IDLE_INTERRUPT",
        "LowPrimaryAmmo": "LOW_PRIMARY_AMMO", "NoPrimaryAmmo": "NO_PRIMARY_AMMO",
        "NoSecondaryAmmo": "NO_SECONDARY_AMMO", "NoWeapon": "NO_WEAPON",
        "SeeHate": "SEE_HATE", "SeeFear": "SEE_FEAR",
        "SeeDislike": "SEE_DISLIKE", "SeeEnemy": "SEE_ENEMY",
        "LostEnemy": "LOST_ENEMY", "EnemyWentNull": "ENEMY_WENT_NULL",
        "EnemyOccluded": "ENEMY_OCCLUDED", "TargetOccluded": "TARGET_OCCLUDED",
        "HaveEnemyLOS": "HAVE_ENEMY_LOS", "HaveTargetLOS": "HAVE_TARGET_LOS",
        "LightDamage": "LIGHT_DAMAGE", "HeavyDamage": "HEAVY_DAMAGE",
        "PhysicsDamage": "PHYSICS_DAMAGE", "RepeatedDamage": "REPEATED_DAMAGE",
        "CanRangeAttack1": "CAN_RANGE_ATTACK1", "CanRangeAttack2": "CAN_RANGE_ATTACK2",
        "CanMeleeAttack1": "CAN_MELEE_ATTACK1", "CanMeleeAttack2": "CAN_MELEE_ATTACK2",
        "Provoked": "PROVOKED", "NewEnemy": "NEW_ENEMY",
        "EnemyTooFar": "ENEMY_TOO_FAR", "EnemyFacingMe": "ENEMY_FACING_ME",
        "BehindEnemy": "BEHIND_ENEMY", "EnemyDead": "ENEMY_DEAD",
        "EnemyUnreachable": "ENEMY_UNREACHABLE",
        "SeePlayer": "SEE_PLAYER", "LostPlayer": "LOST_PLAYER",
        "SeeNemesis": "SEE_NEMESIS",
        "TaskFailed": "TASK_FAILED", "ScheduleDone": "SCHEDULE_DONE",
        "Smell": "SMELL",
        "TooCloseToAttack": "TOO_CLOSE_TO_ATTACK", "TooFarToAttack": "TOO_FAR_TO_ATTACK",
        "NotFacingAttack": "NOT_FACING_ATTACK", "WeaponHasLOS": "WEAPON_HAS_LOS",
        "WeaponBlockedByFriend": "WEAPON_BLOCKED_BY_FRIEND",
        "WeaponPlayerInSpread": "WEAPON_PLAYER_IN_SPREAD",
        "WeaponPlayerNearTarget": "WEAPON_PLAYER_NEAR_TARGET",
        "WeaponSightOccluded": "WEAPON_SIGHT_OCCLUDED",
        "BetterWeaponAvailable": "BETTER_WEAPON_AVAILABLE",
        "HealthItemAvailable": "HEALTH_ITEM_AVAILABLE",
        "GiveWay": "GIVE_WAY", "WayClear": "WAY_CLEAR",
        "HearDanger": "HEAR_DANGER", "HearThumper": "HEAR_THUMPER",
        "HearBugbait": "HEAR_BUGBAIT", "HearCombat": "HEAR_COMBAT",
        "HearWorld": "HEAR_WORLD", "HearPlayer": "HEAR_PLAYER",
        "HearBulletImpact": "HEAR_BULLET_IMPACT",
        "HearPhysicsDanger": "HEAR_PHYSICS_DANGER",
        "HearMoveAway": "HEAR_MOVE_AWAY", "HearSpooky": "HEAR_SPOOKY",
        "NoHearDanger": "NO_HEAR_DANGER",
        "FloatingOffGround": "FLOATING_OFF_GROUND",
        "MobbedByEnemies": "MOBBED_BY_ENEMIES",
        "ReceivedOrders": "RECEIVED_ORDERS",
        "PlayerAddedToSquad": "PLAYER_ADDED_TO_SQUAD",
        "PlayerRemovedFromSquad": "PLAYER_REMOVED_FROM_SQUAD",
        "PlayerPushing": "PLAYER_PUSHING",
        "NPCFreeze": "NPC_FREEZE", "NPCUnfreeze": "NPC_UNFREEZE",
        "TalkerRespondToQuestion": "TALKER_RESPOND_TO_QUESTION",
        "NoCustomInterrupts": "NO_CUSTOM_INTERRUPTS",
    }
    return special.get(name)

print("\nChecking COND_* values in Lua vs mapping document:")
mismatches_cond = []
not_defined = []
ok_cond = []
for pascal_name, expected_value in mapping_conditions.items():
    lua_name = pascal_to_cond(pascal_name)
    if lua_name and lua_name in lua_conditions:
        actual = lua_conditions[lua_name]
        if actual == expected_value:
            ok_cond.append(f"COND_{lua_name} = {actual}")
        else:
            mismatches_cond.append(f"COND_{lua_name}: mapping says {expected_value}, Lua has {actual}")
    elif lua_name:
        not_defined.append(f"COND_{lua_name} — not defined in Lua (value {expected_value})")
    else:
        not_defined.append(f"{pascal_name} — no Lua name mapping (value {expected_value})")

if ok_cond:
    print(f"  OK ({len(ok_cond)}): all match")
if mismatches_cond:
    print(f"  VALUE MISMATCH ({len(mismatches_cond)}):")
    for m in mismatches_cond:
        print(f"    !!  {m}")
if not_defined:
    print(f"  NOT IN LUA ({len(not_defined)}):")
    for m in not_defined:
        print(f"    ??  {m}")

# ============================================================
# 4. CLASS_* relationship constants
# ============================================================
print("\n" + "=" * 70)
print("SECTION 4: CLASS_* relationship constants — checking Lua")
print("=" * 70)

mapping_classes = [
    "CLASS_PLAYER_ALLY", "CLASS_COMBINE", "CLASS_ZOMBIE",
    "CLASS_ANTLION", "CLASS_XEN", "CLASS_VJ_BASE",
]

found_classes = []
missing_classes = []
for cls in mapping_classes:
    found = search_lua(re.escape(cls))
    if found:
        found_classes.append(cls)
    else:
        missing_classes.append(cls)

print(f"\nFOUND in Lua ({len(found_classes)}/{len(mapping_classes)}):")
for c in found_classes:
    print(f"  OK  {c}")
print(f"\nNOT FOUND in Lua ({len(missing_classes)}/{len(mapping_classes)}):")
for c in missing_classes:
    print(f"  ??  {c}")

# Also check what class constants ARE defined
print("\nAll CLASS_* found in Lua source:")
for match in re.finditer(r'CLASS_(\w+)', ALL_LUA):
    print(f"  CLASS_{match.group(1)}")

# ============================================================
# 5. Schedule/Task methods used in Lua
# ============================================================
print("\n" + "=" * 70)
print("SECTION 5: Schedule/Task methods — AISchedule & INPCSchedule")
print("=" * 70)

schedule_methods = [
    # AISchedule methods
    ("EngTask", "M"), ("AddTask", "M"),
    ("IsInterrupted", "M"), ("IsFinished", "M"),
    # INPCSchedule methods (called as self:Method)
    ("StartSchedule", "M"), ("ClearSchedule", "M"),
    ("StopCurrentSchedule", "M"), ("NextTask", "M"),
]

for method, tag in schedule_methods:
    found = search_lua(rf':{re.escape(method)}\s*\(')
    if found:
        print(f"  OK  :{method}() — found in Lua")
    else:
        print(f"  ??  :{method}() — NOT found in Lua")

# AISchedule properties accessed in Lua
print("\nAISchedule properties used in Lua:")
for prop in ["CanBeInterrupted", "ResetOnFail", "HasMovement", "RunCode_OnFail",
             "RunCode_OnFinish", "IgnoreConditions", "TurnData",
             "CanShootWhenMoving", "Tasks", "Name", "IsPlayActivity"]:
    found = search_lua(rf'schedule\.{re.escape(prop)}') or search_lua(rf'sched[a-z]*\.{re.escape(prop)}')
    status = "OK" if found else "??"
    print(f"  {status}  .{prop}")

# ============================================================
# 6. IEngineAITaskSystem methods
# ============================================================
print("\n" + "=" * 70)
print("SECTION 6: IEngineAITaskSystem methods")
print("=" * 70)

task_system_methods = [
    ("GetTaskID", "M"), ("StartEngineTask", "M"), ("RunEngineTask", "M"),
    ("TaskComplete", "M"), ("ScheduleComplete", "M"), ("GetCurGoalType", "M"),
]
for method, tag in task_system_methods:
    found = search_lua(rf':{re.escape(method)}\s*\(')
    if found:
        print(f"  OK  :{method}() — found in Lua")
    else:
        print(f"  ??  :{method}() — NOT found in Lua")

# TASKSTATUS_*
print("\nTaskStatus enum values in Lua:")
for val in ["TASKSTATUS_NEW", "TASKSTATUS_RUN_TASK"]:
    found = search_lua(re.escape(val))
    print(f"  {'OK' if found else '??'}  {val}")

# ============================================================
# 7. INPCConditions methods
# ============================================================
print("\n" + "=" * 70)
print("SECTION 7: INPCConditions methods — SetCondition/ClearCondition/HasCondition")
print("=" * 70)

for method in ["SetCondition", "ClearCondition", "HasCondition"]:
    found = search_lua(rf':{re.escape(method)}\s*\(')
    if found:
        print(f"  OK  :{method}() — found in Lua")
    else:
        print(f"  ??  :{method}() — NOT found in Lua")

# ============================================================
# 8. INPCAttributes methods
# ============================================================
print("\n" + "=" * 70)
print("SECTION 8: INPCAttributes methods")
print("=" * 70)

attr_methods = [
    "GetNPCState", "SetNPCState",
    "Disposition", "AddEntityRelationship",
    "SetLastPosition", "GetLastPosition",
    "GetMaxYawSpeed", "SetMaxYawSpeed",
    "IsMoving", "IsBusy",
    "GetEnemy", "SetEnemy",
]
for method in attr_methods:
    found = search_lua(rf':{re.escape(method)}\s*\(')
    if found:
        print(f"  OK  :{method}() — found in Lua")
    else:
        print(f"  ??  :{method}() — NOT found in Lua")

# ============================================================
# 9. NPCState / Disposition / NavType / MoveType enums
# ============================================================
print("\n" + "=" * 70)
print("SECTION 9: NPCState, Disposition, NavType, MoveType enums")
print("=" * 70)

# Check NPCState values
print("\nNPCState — mapping says: None=0, Idle=1, Alert=2, Combat=3, Dead=4")
print("Actual Lua state defines:")
for match in re.finditer(r'VJ_STATE_(\w+)\s*=\s*(\d+)', ALL_LUA):
    print(f"  VJ_STATE_{match.group(1)} = {match.group(2)}")

# Check Disposition values
print("\nDisposition — mapping says: Error=0, Like=1, Neutral=2, Hate=3, Fear=4")
print("Actual Lua D_* defines (from Garry's Mod, used in code):")
for match in re.finditer(r'\bD_(\w+)\b', ALL_LUA):
    d_name = f"D_{match.group(1)}"
    if d_name not in ["D_VJ_INTEREST"]:
        pass  # D_LI, D_HT, D_FR, D_NU are GMOD builtins
# Check what D_ values are actually used
d_used = set()
for match in re.finditer(r'\b(D_\w+)\b', ALL_LUA):
    d_used.add(match.group(1))
print(f"  D_* values used in Lua: {sorted(d_used)}")

# Check NavType
print("\nNavType — mapping says: Ground, Fly, None, Jump, Climb")
nav_used = set()
for match in re.finditer(r'\b(NAV_\w+)\b', ALL_LUA):
    nav_used.add(match.group(1))
print(f"  NAV_* values used in Lua: {sorted(nav_used)}")

# Check MoveType
print("\nMoveType — mapping says: None, VPhysics, Step, Fly, FlyGravity, NoClip, Push, Walk, Observer")
movetype_used = set()
for match in re.finditer(r'\b(MOVETYPE_\w+|VJ_MOVETYPE_\w+)\b', ALL_LUA):
    movetype_used.add(match.group(1))
print(f"  MOVETYPE_* values used in Lua: {sorted(movetype_used)}")

# ============================================================
# 10. GlobalEngine methods
# ============================================================
print("\n" + "=" * 70)
print("SECTION 10: GlobalEngine methods — checking Lua usage")
print("=" * 70)

global_funcs = [
    ("CurTime", "GetCurrentTime"), ("FrameTime", "GetFrameTime"),
    ("SysTime", "GetSystemTime"), ("math.random", "RandomInt/RandomFloat"),
    ("game.Random", None),  # S&Box specific - won't be in Lua
    ("IsValid", "IsValid"),
    ("util.TraceLine", "TraceLine"), ("util.TraceHull", "TraceHull"),
    ("ents.FindInSphere", "FindInSphere"), ("ents.FindInCone", "FindInCone"),
    ("ents.GetAll", "GetAllComponents"),
    ("print", "Print"), ("PrintTable", "PrintTable"),
    ("MsgC", "MsgC"),
    ("Lerp", "Lerp"), ("LerpAngle", "LerpAngle"), ("LerpVector", "LerpVector"),
    ("Clamp", "Clamp"),
]
for lua_func, csharp_name in global_funcs:
    # Search for the Lua function being called
    if lua_func in ["math.random"]:
        found = search_lua(r'\bmath\.random\b')
    elif lua_func == "game.Random":
        found = False  # S&Box API, not in GMOD
    else:
        found = search_lua(rf'\b{re.escape(lua_func)}\s*\(')
    label = csharp_name or "(S&Box only)"
    print(f"  {'OK' if found else '??'}  Lua: {lua_func}() → C#: {label}")

# ============================================================
# 11. DamageType & HitGroup enums
# ============================================================
print("\n" + "=" * 70)
print("SECTION 11: DamageType / HitGroup enums")
print("=" * 70)

# These are GMOD builtins (DMG_*, HITGROUP_*)
print("DamageType — mapping says: Blast=1, Club=2, Bullet=4, Dissolve=8, etc.")
dmg_used = set()
for match in re.finditer(r'\b(DMG_\w+)\b', ALL_LUA):
    dmg_used.add(match.group(1))
print(f"  DMG_* values used in Lua: {sorted(dmg_used)}")

print("\nHitGroup — mapping says: Generic=0, Head=1, Chest=2, Stomach=3, etc.")
hit_used = set()
for match in re.finditer(r'\b(HITGROUP_\w+)\b', ALL_LUA):
    hit_used.add(match.group(1))
print(f"  HITGROUP_* values used in Lua: {sorted(hit_used)}")

# ============================================================
# 12. SoundType enum
# ============================================================
print("\n" + "=" * 70)
print("SECTION 12: SoundType enum")
print("=" * 70)

sound_types_used = set()
for match in re.finditer(r'\b(SOUND_\w+|SND_\w+)\b', ALL_LUA):
    sound_types_used.add(match.group(1))
print(f"  SOUND_/SND_ values used in Lua: {sorted(sound_types_used)}")

# ============================================================
# 13. Peripheral APIs
# ============================================================
print("\n" + "=" * 70)
print("SECTION 13: Peripheral APIs (timer, sound, net, NavMesh)")
print("=" * 70)

peripheral_checks = [
    ("timer.Simple", r'\btimer\.Simple\s*\('),
    ("timer.Create", r'\btimer\.Create\s*\('),
    ("timer.Remove", r'\btimer\.Remove\s*\('),
    ("Sound.Play", r'\b[Ss]ound\s*\('),
    ("VJ.STOPSOUND", r'\bVJ\.STOPSOUND\b'),
    ("net.Start", r'\bnet\.Start\s*\('),
    ("net.Receive", r'\bnet\.Receive\s*\('),
    ("ply:KeyDown", r':KeyDown\s*\('),
    ("IN_ATTACK", r'\bIN_ATTACK\b'),
    ("IN_JUMP", r'\bIN_JUMP\b'),
    ("NavMesh.GetRandomPoint", r'\b[Nn]av[._\s]*[Mm]esh.*[Rr]andom'),
    ("NavMesh.GetClosestPoint", r'\b[Nn]av[._\s]*[Mm]esh.*[Cc]losest'),
]
for name, pattern in peripheral_checks:
    found = bool(re.search(pattern, ALL_LUA))
    print(f"  {'OK' if found else '??'}  {name}")

# ============================================================
# 14. Summary
# ============================================================
print("\n\n" + "=" * 70)
print("SUMMARY")
print("=" * 70)

print(f"""
Total entity methods:      {len(entity_methods)}
  Found in Lua:             {len(found_methods)}
  NOT found in Lua:         {len(missing_methods)}

Total TASK_* constants:    {len(mapping_tasks)}
  Found in Lua:             {len(found_tasks)}
  NOT found in Lua:         {len(missing_tasks)}

Total Condition enums:     {len(mapping_conditions)}
  Value match:              {len(ok_cond)}
  Value mismatch:           {len(mismatches_cond)}
  Not in Lua:               {len(not_defined)}

Total CLASS_* constants:   {len(mapping_classes)}
  Found in Lua:             {len(found_classes)}
  NOT found in Lua:         {len(missing_classes)}
""")

print("\nItems that might be AI HALLUCINATIONS (NOT found in Lua source):")
if missing_tasks:
    print("\n  TASK_* constants not in Lua:")
    for t in missing_tasks:
        print(f"    - {t}")
if missing_methods:
    print("\n  Entity methods not called in Lua:")
    for m in missing_methods:
        print(f"    - {m}")
if mismatches_cond:
    print("\n  Condition value mismatches:")
    for m in mismatches_cond:
        print(f"    - {m}")
if missing_classes:
    print("\n  CLASS_* not found:")
    for c in missing_classes:
        print(f"    - {c}")
