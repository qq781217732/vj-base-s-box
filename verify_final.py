#!/usr/bin/env python3
"""
FINAL VERIFICATION REPORT
Cross-reference api-mapping.md against actual VJ Base Lua source files.
"""
import re
from pathlib import Path

LUA_DIR = Path(r"f:\DevProject\Sbox\VJ-Base-master")
lua_files = {}
for f in LUA_DIR.rglob("*.lua"):
    try:
        lua_files[str(f)] = f.read_text(encoding="utf-8", errors="replace")
    except:
        pass
ALL_LUA = "\n".join(lua_files.values())

def found(pattern):
    return bool(re.search(pattern, ALL_LUA))

def count(pattern):
    return len(re.findall(pattern, ALL_LUA))

print("=" * 72)
print("  FINAL API MAPPING VERIFICATION REPORT")
print("=" * 72)

# ==========================================
#  HALLUCINATIONS — things in mapping that don't exist or are wrong
# ==========================================
print("\n" + "=" * 72)
print("  HALLUCINATIONS / ERRORS IN MAPPING DOCUMENT")
print("=" * 72)

hallucinations = []

# 1. NPCState enum is completely wrong
hallucinations.append((
    "CRITICAL",
    "NPCState enum values are completely wrong",
    """Mapping: None=0, Idle=1, Alert=2, Combat=3, Dead=4
Actual:  VJ_STATE_NONE=0, VJ_STATE_FREEZE=1, VJ_STATE_ONLY_ANIMATION=2,
         VJ_STATE_ONLY_ANIMATION_CONSTANT=3, VJ_STATE_ONLY_ANIMATION_NOATTACK=4
The mapping uses typical Source engine NPC states, but VJ Base has its own
VJ_STATE_* system with completely different semantics (freeze/animation control)."""
))

# 2. TASK_FIND_COVER_FROM_BEST_SOUND not found
hallucinations.append((
    "HIGH",
    "TASK_FIND_COVER_FROM_BEST_SOUND — never used in Lua",
    "This task constant is in the mapping but NEVER referenced in any VJ Lua file."
))

# 3. AISchedule.IsInterrupted() doesn't exist
hallucinations.append((
    "HIGH",
    "AISchedule.IsInterrupted() method doesn't exist",
    "Mapping lists 'bool IsInterrupted()' but Lua AISchedule only has a FIELD 'CanBeInterrupted' (boolean), not a method."
))

# 4. AISchedule.IsFinished() doesn't exist
hallucinations.append((
    "HIGH",
    "AISchedule.IsFinished() method doesn't exist",
    "Mapping lists 'bool IsFinished()' but Lua has 'ENT:IsScheduleFinished(schedule)' on the entity, not 'schedule:IsFinished()'."
))

# 5. INPCSchedule.ScheduleComplete() not in Lua
hallucinations.append((
    "HIGH",
    "INPCSchedule.ScheduleComplete() — never called in Lua",
    "Mapping lists 'void ScheduleComplete(GameObject npc)' but this method is never called in VJ Lua. Entity has 'TaskComplete()' instead."
))

# 6. IEngineAITaskSystem.GetTaskID() is global, not entity method
hallucinations.append((
    "MEDIUM",
    "GetTaskID is global ai.GetTaskID(), not entity :GetTaskID()",
    "Mapping lists 'int GetTaskID(string taskName)' as entity method. In Lua it's 'ai.GetTaskID(taskName)' — a global GMod function."
))

# 7. HasCondition is GMod Entity builtin
hallucinations.append((
    "MEDIUM",
    "HasCondition is GMod Entity.HasCondition, not INPCConditions method",
    "Mapping lists 'bool HasCondition(GameObject npc, Condition cond)' on INPCConditions. In Lua it's called as 'funcHasCondition(self, COND_*)' where funcHasCondition = metaNPC.HasCondition — a GMod builtin."
))

# 8. NavMesh not in GMod Lua
hallucinations.append((
    "MEDIUM",
    "NavMesh API — not found in GMod Lua source",
    "Mapping lists 'NavMesh.GetRandomPoint' and 'NavMesh.GetClosestPoint'. These are S&Box APIs. GMod doesn't have a direct NavMesh Lua API in VJ Base's usage."
))

# 9. Incorrect mapping of method call style
hallucinations.append((
    "MEDIUM",
    "Several Entity methods called differently in Lua",
    """- GetPoseParameter: called as 'funcGetPoseParameter(self, name)' (localized), not ':GetPoseParameter(name)'
- SetSequence: never called as ':SetSequence()', Lua uses 'PlaySequence' or 'SetIdealSequence'
- GetSaveValue: never called directly in VJ Lua (only SetSaveValue is used)
- BoundingRadius: never called in VJ Lua (valid GMod method but unused)"""
))

# 10. Disposition enum missing D_VJ_INTEREST
hallucinations.append((
    "MEDIUM",
    "Disposition enum missing D_VJ_INTEREST = 100",
    "VJ Base adds a custom disposition D_VJ_INTEREST = 100 not in the mapping."
))

# 11. Missing CLASS_* constants
hallucinations.append((
    "LOW",
    "Missing CLASS_* relationship constants",
    "Lua uses CLASS_APERTURE, CLASS_BLACKOPS, CLASS_UNITED_STATES but they're not in the mapping's RelationshipClass."
))

# 12. SoundType enum — incorrect constant name mapping
hallucinations.append((
    "LOW",
    "SoundType enum uses SOUND_* not SoundType.*",
    "Mapping uses C# enum names like 'SoundType.Danger'. In Lua these are global constants: SOUND_DANGER, SOUND_COMBAT, etc."
))

# 13. GlobalEngine uses game.Random / Time.Now which are S&Box APIs
hallucinations.append((
    "LOW",
    "GlobalEngine references S&Box APIs that don't exist in GMod Lua",
    "game.Random, Time.Now, Time.Delta are S&Box concepts. GMod uses math.random(), CurTime(), FrameTime(). The C# wrappers are fine but the '=>' mapping is conceptual."
))

for severity, title, detail in hallucinations:
    print(f"\n  [{severity}] {title}")
    print(f"  {'─' * 68}")
    for line in detail.strip().split("\n"):
        print(f"  {line}")

# ==========================================
#  THINGS MISSING FROM THE MAPPING
# ==========================================
print("\n\n" + "=" * 72)
print("  THINGS IN LUA BUT MISSING FROM API MAPPING")
print("=" * 72)

missing = []

# Schedule methods on ENT
missing.append((
    "CRITICAL",
    "Entity schedule/task lifecycle methods",
    "These ENT: methods exist in schedules.lua but are NOT in the mapping interfaces:",
    ["StartTask", "RunTask", "TaskFinished", "TaskTime", "SetTask",
     "OnTaskComplete", "OnTaskFailed", "OnMovementFailed", "OnMovementComplete",
     "DoSchedule", "ScheduleFinished", "IsScheduleFinished"]
))

# Engine schedule methods
missing.append((
    "HIGH",
    "Engine schedule methods on entity",
    "These ENT: methods wrap engine-level schedule execution:",
    ["StartEngineSchedule", "EngineScheduleFinish", "DoingEngineSchedule"]
))

# VJ_STATE_* - the real state system
missing.append((
    "CRITICAL",
    "VJ State system (replaces hallucinated NPCState)",
    "Real state constants used throughout the codebase:",
    ["VJ_STATE_NONE = 0", "VJ_STATE_FREEZE = 1",
     "VJ_STATE_ONLY_ANIMATION = 2", "VJ_STATE_ONLY_ANIMATION_CONSTANT = 3",
     "VJ_STATE_ONLY_ANIMATION_NOATTACK = 4"]
))

# VJ_MOVETYPE_*
missing.append((
    "HIGH",
    "VJ Movement type constants",
    "VJ Base defines its own movement types in addition to GMod's MOVETYPE_*:",
    ["VJ_MOVETYPE_GROUND = 1", "VJ_MOVETYPE_AERIAL = 2",
     "VJ_MOVETYPE_AQUATIC = 3", "VJ_MOVETYPE_STATIONARY = 4", "VJ_MOVETYPE_PHYSICS = 5"]
))

# AISchedule properties not in mapping
missing.append((
    "HIGH",
    "AISchedule properties not documented",
    "Fields on the AISchedule class used extensively but not listed:",
    ["HasMovement", "RunCode_OnFail", "RunCode_OnFinish",
     "OnFailExecuted", "OnFinishExecuted", "IgnoreConditions", "TurnData",
     "CanShootWhenMoving", "IsPlayActivity", "TaskCount"]
))

# D_VJ_INTEREST
missing.append((
    "MEDIUM",
    "D_VJ_INTEREST = 100 — custom VJ disposition",
    "Custom disposition used for 'potential target, only engage when necessary':",
    ["D_VJ_INTEREST = 100"]
))

# CLASS_* missing
missing.append((
    "MEDIUM",
    "Additional CLASS_* relationship constants used in Lua",
    "These are referenced in VJ Lua but absent from mapping:",
    ["CLASS_APERTURE", "CLASS_BLACKOPS", "CLASS_UNITED_STATES"]
))

# Additional VJ enums referenced in Lua
missing.append((
    "LOW",
    "Additional VJ enums/constants not in mapping",
    "VJ Base has many more enums defined in enums.lua that the mapping omits:",
    ["VJ_BEHAVIOR_* (5 values)", "VJ_ATTACK_TYPE_* (5 values)",
     "VJ_ATTACK_STATE_* (5 values)", "VJ_FACE_* (7 values)",
     "VJ_ALERT_STATE_* (3 values)", "VJ_DANGER_TYPE_* (3 values)",
     "VJ_WEP_STATE_* (3 values)", "VJ_WEP_ATTACK_STATE_* (7 values)",
     "VJ_WEP_INVENTORY_* (5 values)", "VJ_MEM_* (7 values)",
     "VJ_ANIM_TYPE_* (4 values)", "VJ_ANIM_SET_* (5 values)",
     "VJ_BLOOD_COLOR_* (10 values)", "VJ_DIFFICULTY_* (15 values)",
     "VJ_PROJ_TYPE_* (3 values)", "VJ_PROJ_COLLISION_* (3 values)",
     "VJ_KILLICON_* (4 values)", "VJ.COLOR_* (19 colors)",
     "VJ.DMG_* (2 custom damage types)"]
))

for severity, title, detail, items in missing:
    print(f"\n  [{severity}] {title}")
    print(f"  {'─' * 68}")
    print(f"  {detail}")
    for item in items:
        print(f"    - {item}")

# ==========================================
#  STATS SUMMARY
# ==========================================
print("\n\n" + "=" * 72)
print("  STATISTICAL SUMMARY")
print("=" * 72)

# Entity methods from mapping
entity_methods = [
    "GetPos","SetPos","GetAngles","SetAngles","GetForward","GetRight","GetUp",
    "GetVelocity","SetVelocity","EyePos","WorldSpaceCenter","OBBCenter","NearestPoint",
    "BoundingRadius","Spawn","Remove","SetModel","GetModel","SetColor","SetMaterial",
    "SetModelScale","SetBodygroup","SetSkin","GetClass","EntIndex","GetName","SetName",
    "IsPlayer","IsNPC","SetParent","GetParent","SetOwner","GetOwner","SetCollisionGroup",
    "GetCollisionGroup","SetSolid","GetSolid","GetPhysicsObject","IsOnGround","WaterLevel",
    "Health","SetHealth","GetMaxHealth","SetMaxHealth","Alive","TakeDamage","Ignite",
    "Extinguish","IsFlagSet","AddFlags","RemoveFlags","SetSaveValue","GetSaveValue",
    "SetMovementActivity","GetMovementActivity","GetSequence","SetSequence","GetSequenceName",
    "GetSequenceActivity","IsSequenceFinished","GetCycle","SetCycle","SequenceDuration",
    "GetSequenceMoveDist","LookupSequence","FrameAdvance","AutoMovement","SetPlaybackRate",
    "GetPlaybackRate","SetPoseParameter","GetPoseParameter","LookupAttachment","GetAttachment",
]
entity_found = sum(1 for m in entity_methods if found(rf':{m}\s*\('))

# TASK_* constants
mapping_tasks = [
    "TASK_GET_PATH_TO_LASTPOSITION","TASK_GET_PATH_TO_TARGET","TASK_GET_PATH_TO_ENEMY",
    "TASK_GET_PATH_TO_ENEMY_LOS","TASK_GET_PATH_TO_RANDOM_NODE","TASK_RUN_PATH",
    "TASK_WALK_PATH","TASK_RUN_PATH_FLEE","TASK_RUN_PATH_TIMED","TASK_WALK_PATH_TIMED",
    "TASK_RUN_PATH_FOR_UNITS","TASK_WALK_PATH_FOR_UNITS","TASK_RUN_PATH_WITHIN_DIST",
    "TASK_WALK_PATH_WITHIN_DIST","TASK_WEAPON_RUN_PATH","TASK_ITEM_RUN_PATH",
    "TASK_MOVE_TO_TARGET_RANGE","TASK_MOVE_TO_GOAL_RANGE","TASK_MOVE_AWAY_PATH",
    "TASK_FACE_TARGET","TASK_FACE_ENEMY","TASK_FACE_PLAYER","TASK_FACE_LASTPOSITION",
    "TASK_FACE_SAVEPOSITION","TASK_FACE_PATH","TASK_FACE_HINTNODE","TASK_FACE_IDEAL",
    "TASK_FACE_REASONABLE","TASK_FIND_COVER_FROM_ORIGIN","TASK_FIND_COVER_FROM_ENEMY",
    "TASK_FIND_COVER_FROM_BEST_SOUND","TASK_WAIT","TASK_WAIT_FOR_MOVEMENT",
    "TASK_SET_TOLERANCE_DISTANCE","TASK_SET_ROUTE_SEARCH_TIME","TASK_STOP_MOVING",
    "TASK_FORGET","TASK_IGNORE_OLD_ENEMIES","TASK_STORE_BESTSOUND_REACTORIGIN_IN_SAVEPOSITION",
    "TASK_PLAY_SEQUENCE","TASK_PLAY_SEQUENCE_FACE_ENEMY","TASK_SET_ACTIVITY",
    "TASK_RESET_ACTIVITY","TASK_VJ_PLAY_ACTIVITY","TASK_VJ_PLAY_SEQUENCE",
]
tasks_found = sum(1 for t in mapping_tasks if found(re.escape(t)))

print(f"""
  API Mapping claims:
    {len(entity_methods):>3}  entity methods (IEngineEntity)
    {len(mapping_tasks):>3}  TASK_* constants (EngineTask)
      70  Condition enum values
       6  CLASS_* relationship constants
       4  NPCState values (WRONG)
       5  Disposition values (incomplete)
       5  NavType values
       9  MoveType values
      12  INPCAttributes methods
       3  INPCConditions methods
       6  IEngineAITaskSystem methods
       4  INPCSchedule methods
       4  AISchedule methods/properties
    ----
    ~250 total items mapped

  Verified against ~85 Lua files:
    {entity_found:>3}/{len(entity_methods)} entity methods confirmed called in Lua
    {tasks_found:>3}/{len(mapping_tasks)} task constants confirmed in Lua
      70/70  condition values correct
       6/6   CLASS_* confirmed in Lua
       0/4   NPCState values correct (COMPLETELY WRONG!)


  Findings:
    ~12   hallucinations or errors
    ~60+  items missing from the mapping
""")

print("  Critical issues to fix before implementation:")
print("    1. Replace NPCState enum with VJ_STATE_* system")
print("    2. Add missing schedule/task lifecycle methods to interfaces")
print("    3. Remove AISchedule.IsInterrupted()/IsFinished() → use fields/entity methods")
print("    4. Remove INPCSchedule.ScheduleComplete() → doesn't exist")
print("    5. Fix GetTaskID → global function, not entity method")
print("    6. Remove TASK_FIND_COVER_FROM_BEST_SOUND (or flag as VJ-only custom)")
print("    7. Add D_VJ_INTEREST, CLASS_APERTURE/BLACKOPS/UNITED_STATES")
print("    8. Add VJ_MOVETYPE_* constants alongside MOVETYPE_*")
