#!/usr/bin/env python3
"""
VJBase Lua->C# Migration Gap Analyzer
=====================================
Extracts [Property] names from reference files (docs/vjbase-reference/)
and implementation files (Code/VJBase/), then cross-references.

Output: per-class gap report showing what's in Lua reference but
missing from C# implementation, filtered by relevance.

Usage:
  python compare_migration.py          # full markdown report
  python compare_migration.py --focus  # key classes only
  python compare_migration.py --json   # machine-readable
"""

import re
import sys
import json
from pathlib import Path
from typing import Dict, List, Set, Tuple
from dataclasses import dataclass

ROOT = Path(__file__).resolve().parent.parent
REF_DIR = ROOT / "docs" / "vjbase-reference"
IMPL_DIR = ROOT / "Code" / "VJBase"

# ── Class mapping: ref class -> impl files ──────────────────────

CLASS_MAP: Dict[str, List[str]] = {
    "Core":                  ["Core/BaseNPC.cs", "Core/AIConfig.cs", "Core/NPCState.cs",
                              "Core/ConditionFlags.cs", "Core/SenseMemory.cs"],
    "Schedules":             ["Schedule/AISchedule.cs", "Schedule/ScheduleRunner.cs", "Schedule/AITask.cs"],
    "BaseAa":                ["Bases/CreatureNPC.cs", "Movement/NPCNavigation.cs"],
    "BaseTank":              ["Bases/TankNPC.cs"],
    "NpcVjHumanBase":        ["Bases/HumanNPC.cs", "Bases/CreatureNPC.cs", "Core/BaseNPC.cs", "Core/AIConfig.cs"],
    "NpcVjCreatureBase":     ["Bases/CreatureNPC.cs", "Core/BaseNPC.cs", "Core/AIConfig.cs"],
    "NpcVjTankBase":         ["Bases/TankNPC.cs", "Bases/CreatureNPC.cs", "Core/BaseNPC.cs", "Core/AIConfig.cs"],
    "NpcVjTankgBase":        ["Bases/TankNPC.cs", "Bases/CreatureNPC.cs", "Core/BaseNPC.cs", "Core/AIConfig.cs", "Components/TankGunner.cs"],
    "ObjVjProjectileBase":   ["Components/Projectile.cs"],
    "ObjVjGib":              ["Components/Gib.cs"],
    "ObjVjGrenade":          ["Components/Grenade.cs"],
    "WeaponVjBase":          ["Components/BaseWeapon.cs"],
    "Funcs":                 ["Utilities/VJUtils.cs"],
    "Enums":                 ["VJEnums.cs"],
}

# ── Skip patterns: properties intentionally not migrated ────────

SKIP_PATTERNS = [
    # Sound system — different architecture in C#
    r"SoundTbl_", r"NextSoundTime_", r"Next.*SoundT$",
    r"(Idle|CombatIdle|ReceiveOrder|FollowPlayer|YieldToPlayer|Medic|OnPlayerSight"
    r"|Investigate|LostEnemy|Alert|CallForHelp|BecomeEnemyToPlayer|BeforeMeleeAttack"
    r"|MeleeAttack|ExtraMelee|GrenadeAttack|DangerSight|Suppressing|WeaponReload"
    r"|KilledEnemy|AllyDeath|Pain|Impact|DamageByPlayer|Death|SoundTrack|Footstep"
    r"|Breath|IdleDialogue)Sound(Level|Pitch|Chance)",
    r"Has.*Sounds?$", r"Has.*Sound$", r"Disable.*Sound",
    r"FootstepSoundTimer", r"MainSoundPitch", r"MainSoundPitchStatic",
    r"IdleDialogue(AnswerSoundChance|Distance|CanTurn)",

    # GMod metadata
    r"PrintName$|Author$|Contact$|Category$|AutomaticFrameAdvance$",
    r"IsVJBaseSNPC|VJ_NPC_Class$|Type$|IsVJBaseWeapon$",
    r"Purpose$|Instructions$|ReplacementWeapon$",

    # GMod engine-specific
    r"HullType$|EntitiesToNoCollide$|ControllerParams$",
    r"DeathCorpse|HasDeathCorpse$|HasDeathAnimation$|DeathAnimation",
    r"HasDeathRagdoll$|DropDeathLoot$|DeathLoot",
    r"Weapon_Unarmed|Weapon_IgnoreSpawnMenu$|WeaponInventory_",
    r"CanGib|HasGib|GibOnDeath",
    r"HasBloodPool$|BloodPool$|BloodDecalUseGMod$|BloodDecal$",
    r"BloodColor$|Bleeds$|UpdatedPoseParam$",
    r"DamageByPlayerDispositionLevel$|AllowWeaponOcclusionDelay$",
    r"WeaponEntity$|PoseParameterLooking_Names$",
    r"Passive_|CanRedirectGrenades$",

    # GMod-specific animation/schedule/task system
    r"AnimTbl_|ConstantlyFaceEnemy$|ConstantlyFaceEnemy_",
    r"FlinchHitGroupMap$|FlinchHitGroupPlayDefault$",
    r"DamageResponse$|DamageAllyResponse$|CombatDamageResponse$",
    r"DeathAllyResponse|DeathAllyResponse_MoveLimit$",
    r"MeleeAttackAnimationFaceEnemy$|MeleeAttackAnimationDecreaseLengthAmount$",
    r"NextAnyAttackTime_|DisableDefaultMeleeAttackDamageCode$",
    r"MeleeAttackStopOnHit$|MeleeAttackExtraTimers$|MeleeAttackReps$",
    r"Weapon_AimTurnDiff$|Weapon_SecondaryFireTime$|Weapon_OcclusionDelay",
    r"Weapon_StrafeCooldown$|Weapon_RetreatDistance$",
    r"TurningUseAllAxis$|UsePoseParameterMovement$|TurningSpeed$",
    r"CanTurnWhileStationary$|CanTurnWhileMoving$|JumpParams$",
    r"IdleAlwaysWander$|IdleSoundsWhileAttacking$|IdleSoundsRegWhileAlert$",

    # Internal runtime state — replaced by C# fields / TimeUntil
    r"VJ_ID_Healable$|VJ_DEBUG$|VJ_IsBeingControlled|VJ_TheController",
    r"SelectedDifficulty$|AIState$|MedicData$|IsFollowing$|FollowData$",
    r"EnemyData$|TurnData$|GuardData$|AnimLockTime$|AnimPlaybackRate$",
    r"AnimModelSet$|LastAnimSeed$|LastAnimType$|AttackSeed$|AttackAnim$",
    r"AttackAnimDuration$|AttackAnimTime$|IsAbleToMeleeAttack$",
    r"MeleeAttack_IsPropAttack$|Alerted$|Flinching$|HealthRegenDelayT$",
    r"GibbedOnDeath$|TakingCoverT$|LastHiddenZone|NextInvestigationMove$",
    r"IdleSoundBlockTime$|MainSoundPitchValue$|TimersToRemove$",
    r"LatestEnemyDistance$|NearestPointToEnemyDistance$",
    r"WeaponState$|WeaponInventoryStatus$|WeaponLastShotTime$",
    r"WeaponAttackState$|WeaponAttackAnim$|Weapon_AimTurnDiff_Def$",
    r"NextWeaponAttackT|NextMeleeWeaponAttackT|NextMoveOnGunCoveredT",
    r"NextThrowGrenadeT|NextDangerDetectionT|NextCombatDamageResponseT",
    r"NextProcessTime$|Next.*SoundT$",

    # ViewModel — player-only
    r"ViewModel|UseHands$|BobScale$|SwayScale$|CSMuzzleFlashes$",
    r"DrawAmmo$|DrawCrosshair$|DrawWeaponInfoBox$|BounceWeaponIcon$",
    r"HasIdleAnimation$|Slot$|SlotPos$|Weight$|AutoSwitchTo$|AutoSwitchFrom$",
    r"WorldModel|WorldModel_",

    # Weapon internal NPC timers
    r"NPC_NextPrimaryFire$|NPC_TimeUntilFire|NPC_BeforeFireSound$",
    r"NPC_ExtraFireSound|NPC_SecondaryFireNext$|NPC_ReloadSound$",
    r"NPC_HasReloadSound$|NPC_HasSecondaryFire$",

    # Effects/Menu/Localization — intentionally skipped
    r"^VJUtility|^Convars|^Debug$|^Hooks|^Corpse|^Music",
    r"^Particles|^Sounds|^Localization|^Main$|^Spawn$",
    r"^EntityConfigures|^EntityProperties",

    # Weapon effects — handled by VFXHelper
    r"PrimaryEffects_|SecondaryEffects_",
    r"Reload_TimeUntilAmmoIsSet$",
]

# ── Alias mappings: ref_name -> [possible C# names] ─────────────

NAME_ALIASES = {
    "StartHealth":              ["Health", "MaxHealth"],
    "SightDistance":            ["ViewDistance", "SightDistance"],
    "SightAngle":               ["FieldOfView", "SightAngle"],
    "EnemyTimeout":             ["LoseEnemyTime"],
    "AlertTimeout":             ["LoseEnemyTime"],
    "Behavior":                 ["Behavior", "DefaultBehavior"],
    "HealthRegenParams":        ["HealthRegenRate", "HealthRegenDelay"],
    "Tank_HasShellAttack":            ["HasShellAttack"],
    "Tank_Shell_FireMin":             ["Shell_FireMin"],
    "Tank_Shell_FireMax":             ["Shell_FireMax"],
    "Tank_Shell_NextFireTime":        ["Shell_NextFireTime"],
    "Tank_Shell_TimeUntilFire":       ["Shell_TimeUntilFire"],
    "Tank_Shell_SpawnPos":            ["Shell_SpawnPos"],
    "Tank_Shell_Entity":              ["Shell_Entity"],
    "Tank_Shell_VelocitySpeed":       ["Shell_VelocitySpeed"],
    "Tank_Shell_MuzzleFlashPos":      ["Shell_MuzzleFlashPos"],
    "Tank_Shell_ParticlePos":         ["Shell_ParticlePos"],
    "Tank_AngleDiffuseFiringLimit":   ["AngleDiffuseFiringLimit"],
    "Tank_ReloadShellSoundLevel":     ["ReloadShellSoundLevel"],
    "Tank_FireShellSoundLevel":       ["FireShellSoundLevel"],
    "MeleeAttackBleedEnemy":          ["CanBleed"],
    "MeleeAttackBleedEnemyDamage":    ["BleedDamage"],
    "MeleeAttackBleedEnemyTime":      ["BleedDuration"],
    "MeleeAttackBleedEnemyChance":    ["BleedChance"],
    "MeleeAttackBleedEnemyReps":      ["BleedReps"],
    "RangeAttackMaxDistance":         ["RangeDistance"],
    "RangeAttackMinDistance":         ["RangeToMeleeDistance"],
    "HasGrenadeAttack":         ["CanThrowGrenades"],
    "GrenadeAttackEntity":      ["GrenadePrefab"],
    "GrenadeAttackMinDistance": ["GrenadeMinDistance"],
    "GrenadeAttackMaxDistance": ["GrenadeMaxDistance"],
    "GrenadeAttackChance":      ["GrenadeThrowChance"],
    "GrenadeAttackFuseTime":    ["GrenadeFuseTime"],
    "GrenadeAttackAttachment":  ["GrenadeAttachment"],
    "GrenadeAttackBone":        ["GrenadeBone"],
    "GrenadeAttackThrowTime":   ["GrenadeThrowTime"],
    "NextGrenadeAttackTime":    ["GrenadeCooldown"],
    "FollowPlayer":             ["CanFollow"],
    "FollowMinDistance":        ["FollowMinDistance"],
    "IsMedic":                  ["IsMedic"],
    "Medic_CheckDistance":      ["MedicCheckDistance"],
    "Medic_HealDistance":       ["MedicHealDistance"],
    "Medic_HealAmount":         ["MedicHealAmount"],
    "Medic_NextHealTime":       ["MedicHealCooldown"],
    "Medic_TimeUntilHeal":      ["Medic_TimeUntilHeal"],
    "Medic_SpawnPropOnHeal":    ["Medic_SpawnPropOnHeal"],
    "Medic_SpawnPropOnHealModel":       ["Medic_SpawnPropOnHealModel"],
    "Medic_SpawnPropOnHealAttachment":  ["Medic_SpawnPropOnHealAttachment"],
    "CallForHelp":              ["CallForHelp"],
    "CallForHelpDistance":      ["CallForHelpDistance"],
    "CallForHelpCooldown":      ["CallForHelpCooldown"],
    "CallForHelpAnimCooldown":  ["CallForHelpAnimCooldown"],
    "CallForHelpAnimFaceEnemy": ["CallForHelpAnimFaceEnemy"],
    "HasOnPlayerSight":         ["HasOnPlayerSight"],
    "OnPlayerSightDistance":    ["OnPlayerSightDistance"],
    "OnPlayerSightDispositionLevel":  ["OnPlayerSightDispositionLevel"],
    "OnPlayerSightOnlyOnce":    ["OnPlayerSightOnlyOnce"],
    "OnPlayerSightNextTime":    ["OnPlayerSightNextTime"],
    "CanInvestigate":           ["CanInvestigate"],
    "InvestigateSoundMultiplier":     ["InvestigateSoundMultiplier"],
    "CanDetectDangers":         ["CanDetectDangers"],
    "DangerDetectionDistance":  ["DangerDetectionDistance"],
    "CanFlinch":                ["CanFlinch"],
    "FlinchChance":             ["FlinchChance"],
    "FlinchCooldown":           ["FlinchCooldown"],
    "FlinchDamageTypes":        ["FlinchDamageTypes"],
    "Immune_Bullet":            ["Immune_Bullet"],
    "Immune_Melee":             ["Immune_Melee"],
    "Immune_Explosive":         ["Immune_Explosive"],
    "Immune_Dissolve":          ["Immune_Dissolve"],
    "Immune_Toxic":             ["Immune_Toxic"],
    "Immune_Fire":              ["Immune_Fire"],
    "Immune_Electricity":       ["Immune_Electricity"],
    "Immune_Sonic":             ["Immune_Sonic"],
    "GodMode":                  ["GodMode"],
    "ForceDamageFromBosses":    ["ForceDamageFromBosses"],
    "AllowIgnition":            ["AllowIgnition"],
    "HasBloodDecal":            ["HasBloodDecal"],
    "HasBloodParticle":         ["HasBloodParticle"],
    "DeathDelayTime":           ["DeathDelayTime"],
    "DropWeaponOnDeath":        ["DropWeaponOnDeath"],
    "EnemyDetection":           ["EnemyDetection"],
    "EnemyTouchDetection":      ["EnemyTouchDetection"],
    "EnemyXRayDetection":       ["EnemyXRayDetection"],
    "AlertTimeout":             ["AlertTimeout"],
    "CanChatMessage":           ["CanChatMessage"],
    "DisableChasingEnemy":      ["DisableChasingEnemy"],
    "CanOpenDoors":             ["CanOpenDoors"],
    "CanAlly":                  ["CanAlly"],
    "AlliedWithPlayerAllies":   ["AlliedWithPlayerAllies"],
    "YieldToAlliedPlayers":     ["YieldToAlliedPlayers"],
    "BecomeEnemyToPlayer":      ["BecomeEnemyToPlayer"],
    "CanReceiveOrders":         ["CanReceiveOrders"],
    "IsGuard":                  ["IsGuard"],
    "Behavior":                 ["Behavior", "DefaultBehavior"],
    "MovementType":             ["MovementType"],
    "HasMeleeAttack":           ["HasMeleeAttack"],
    "MeleeAttackDamage":        ["MeleeAttackDamage"],
    "MeleeAttackDamageType":    ["MeleeAttackDamageType"],
    "MeleeAttackDistance":      ["MeleeAttackDistance"],
    "MeleeAttackAngleRadius":   ["MeleeAttackAngleRadius"],
    "MeleeAttackDamageDistance":      ["MeleeAttackDamageDistance"],
    "MeleeAttackDamageAngleRadius":   ["MeleeAttackDamageAngleRadius"],
    "TimeUntilMeleeAttackDamage":     ["MeleeAttackDelay"],
    "NextMeleeAttackTime":      ["NextMeleeAttack"],
    "HasMeleeAttackKnockBack":  ["HasMeleeAttackKnockBack"],
    "Weapon_Disabled":          ["Weapon_Disabled"],
    "Weapon_Accuracy":          ["WeaponAccuracy"],
    "Weapon_CanMoveFire":       ["Weapon_CanMoveFire"],
    "Weapon_Strafe":            ["Weapon_CanStrafe"],
    "Weapon_MinDistance":       ["Weapon_MinDistance"],
    "Weapon_MaxDistance":       ["Weapon_MaxDistance"],
    "Weapon_CanCrouchAttack":   ["Weapon_CanCrouch"],
    "Weapon_CrouchAttackChance":     ["Weapon_CrouchChance"],
    "Weapon_CanSecondaryFire":  ["Weapon_CanSecondaryFire"],
    "Weapon_CanReload":         ["Weapon_CanReload"],
    "Weapon_FindCoverOnReload": ["Weapon_CoverOnReload"],
    "DisableWeaponReloadAnimation":   ["Weapon_DisableReloadAnim"],
    "HasPoseParameterLooking":        ["HasPoseParameterLooking"],
    "PoseParameterLooking_TurningSpeed":  ["PoseParameterLooking_TurningSpeed"],
    "PoseParameterLooking_InvertPitch":   ["PoseParameterLooking_InvertPitch"],
    "PoseParameterLooking_InvertYaw":     ["PoseParameterLooking_InvertYaw"],
    "PoseParameterLooking_InvertRoll":    ["PoseParameterLooking_InvertRoll"],
    "PoseParameterLooking_CanReset":      ["PoseParameterLooking_CanReset"],
    "CanPickupWeapons":           ["Weapon_CanPickup"],
    "Model":                      ["Model", "ModelPath"],
    "GrenadeAttackModel":         ["GrenadeModel"],
    "BloodParticle":              ["BloodParticle"],
}

# ── Core logic ──────────────────────────────────────────────────

def extract_properties(filepath: Path) -> List[str]:
    names = []
    try:
        text = filepath.read_text(encoding="utf-8", errors="ignore")
    except Exception:
        return names
    for m in re.finditer(r'\[Property\]\s+public\s+\S+\s+(\w+)', text):
        names.append(m.group(1))
    return names


def is_skipped(name: str) -> bool:
    for pat in SKIP_PATTERNS:
        if re.search(pat, name):
            return True
    return False


def find_aliases(name: str) -> List[str]:
    return NAME_ALIASES.get(name, [name])


def find_ref_files() -> Dict[str, Path]:
    result = {}
    for f in REF_DIR.rglob("*.cs"):
        result[f.stem] = f
    return result


def find_impl_files() -> Dict[str, Path]:
    result = {}
    for f in IMPL_DIR.rglob("*.cs"):
        result[str(f.relative_to(IMPL_DIR)).replace("\\", "/")] = f
    return result


@dataclass
class GapReport:
    ref_class: str
    ref_file: str
    impl_files: List[str]
    total_ref_props: int
    total_impl_props: int
    missing: List[str]
    aliases: List[Tuple[str, str]]


def compare(ref_name: str, ref_file: Path, impl_rels: List[str],
            impl_map: Dict[str, Path]) -> GapReport:
    ref_props = extract_properties(ref_file)

    impl_props: Set[str] = set()
    found = []
    for rel in impl_rels:
        if rel in impl_map:
            found.append(rel)
            impl_props.update(extract_properties(impl_map[rel]))

    missing = []
    aliases = []

    for prop in ref_props:
        if prop in impl_props:
            continue
        matched = False
        for alias in find_aliases(prop):
            if alias != prop and alias in impl_props:
                aliases.append((prop, alias))
                matched = True
                break
        if matched:
            continue
        if not is_skipped(prop):
            missing.append(prop)

    return GapReport(ref_class=ref_name,
                     ref_file=str(ref_file.relative_to(ROOT)),
                     impl_files=found,
                     total_ref_props=len(ref_props),
                     total_impl_props=len(impl_props),
                     missing=missing,
                     aliases=aliases)


# ── Output ─────────────────────────────────────────────────────

def print_report(reports: List[GapReport], focus: bool = False):
    if focus:
        key = {"NpcVjHumanBase", "NpcVjCreatureBase", "NpcVjTankBase",
               "NpcVjTankgBase", "Core", "WeaponVjBase", "ObjVjProjectileBase"}
        reports = [r for r in reports if r.ref_class in key]

    total_missing = sum(len(r.missing) for r in reports)
    total_alias = sum(len(r.aliases) for r in reports)

    print("# VJBase Migration Gap Report\n")
    print(f"**Generated by `compare_migration.py`**\n")
    print(f"| Metric | Value |")
    print(f"|--------|-------|")
    print(f"| Classes analyzed | {len(reports)} |")
    print(f"| Real gaps (missing props) | **{total_missing}** |")
    print(f"| Alias-matched (different name) | {total_alias} |")
    print()

    for r in sorted(reports, key=lambda x: len(x.missing), reverse=True):
        if not r.missing and not r.aliases:
            continue

        print(f"## {r.ref_class}")
        print(f"- Ref: `{r.ref_file}` ({r.total_ref_props} props)")
        impl_str = ', '.join(f'`{f}`' for f in r.impl_files) if r.impl_files else '[NOT FOUND]'
        print(f"- Impl: {impl_str} ({r.total_impl_props} props)")
        print()

        if r.missing:
            print(f"### Missing ({len(r.missing)})")
            for p in r.missing:
                print(f"- `{p}`")
            print()

        if r.aliases:
            print(f"### Alias-Matched ({len(r.aliases)})")
            for ref_name, impl_name in r.aliases:
                print(f"- `{ref_name}` -> `{impl_name}`")
            print()

        print("---\n")

    # Summary
    print("## Summary\n")
    print("| Class | Ref | Impl | Gaps | Aliases |")
    print("|-------|-----|------|------|---------|")
    for r in sorted(reports, key=lambda x: len(x.missing), reverse=True):
        print(f"| {r.ref_class} | {r.total_ref_props} | {r.total_impl_props} | **{len(r.missing)}** | {len(r.aliases)} |")


def print_json(reports: List[GapReport]):
    data = []
    for r in reports:
        data.append({
            "class": r.ref_class, "ref_file": r.ref_file,
            "impl_files": r.impl_files,
            "total_ref_props": r.total_ref_props,
            "total_impl_props": r.total_impl_props,
            "gaps": r.missing,
            "aliases": [(a, b) for a, b in r.aliases],
        })
    print(json.dumps(data, indent=2, ensure_ascii=False))


def main():
    use_json = "--json" in sys.argv
    focus = "--focus" in sys.argv

    ref_map = find_ref_files()
    impl_map = find_impl_files()

    reports = []
    for ref_name, ref_path in sorted(ref_map.items()):
        impl_rels = CLASS_MAP.get(ref_name)
        if impl_rels is None:
            for mk in CLASS_MAP:
                if mk in ref_name or ref_name in mk:
                    impl_rels = CLASS_MAP[mk]
                    break
            else:
                impl_rels = []

        reports.append(compare(ref_name, ref_path, impl_rels, impl_map))

    if use_json:
        print_json(reports)
    else:
        print_report(reports, focus=focus)


if __name__ == "__main__":
    main()
