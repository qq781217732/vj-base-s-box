#!/usr/bin/env python3
"""
Semi-automatic Lua→C# migration inspector.
Input: Lua source file
Output: migration card (Markdown) with dependencies, API inventory, type guesses,
        risk flags, C# skeleton, and manual check items.

Usage:
  python lua-inspector.py path/to/file.lua              → print card
  python lua-inspector.py path/to/file.lua --json       → machine-readable
  python lua-inspector.py path/to/file.lua > card.md    → save to file
"""

import re
import sys
import json
from pathlib import Path
from dataclasses import dataclass, field
from typing import List, Dict, Optional, Tuple

# ═══════════════════════════════════════════════════════
# GMod → S&Box API mapping (from mapping-verified.md)
# ═══════════════════════════════════════════════════════

GMOD_TO_SBOX = {
    # Transform
    "GetPos":           ("Transform.Position", "direct"),
    "SetPos":           ("Transform.Position =", "direct"),
    "GetAngles":        ("Transform.Rotation", "direct"),
    "SetAngles":        ("Transform.Rotation =", "direct"),
    "GetForward":       ("Transform.World.Forward", "direct"),
    "GetRight":         ("Transform.World.Right", "direct"),
    "GetUp":            ("Transform.World.Up", "direct"),
    "EyePos":           ("EyePosition (Transform.Position + Vector3.Up * EyeHeight)", "calc"),
    "GetShootPos":      ("ShootPosition", "method"),
    "WorldSpaceCenter": ("Collider.Bounds.Center", "direct"),
    "OBBCenter":        ("Collider.Bounds.Center", "direct"),
    "NearestPoint":     ("Collider.ClosestPoint", "direct"),
    # Entity
    "GetVelocity":      ("Rigidbody.Velocity", "direct"),
    "SetVelocity":      ("Rigidbody.Velocity =", "direct"),
    "GetClass":         ("GetType().Name", "direct"),
    "GetName":          ("GameObject.Name", "direct"),
    "Remove":           ("GameObject.Destroy()", "direct"),
    "IsNPC":            ("Components.Get<BaseNPC>() != null", "check"),
    "IsPlayer":         ("Tags.Has(\"player\")", "check"),
    "IsValid":          (".IsValid()", "direct"),
    "Alive":            ("Health > 0", "direct"),
    "Health":           ("Health", "field"),
    "SetHealth":        ("Health =", "field"),
    "SetMaxHealth":     ("MaxHealth =", "field"),
    # Perception
    "Visible":          ("Senses.CanSee(self, target)", "bridge"),
    "VisibleVec":       ("Senses.CanSeePoint(self, pos)", "bridge"),
    # Model
    "SetModel":         ("Model.Model = Sandbox.Model.Load(path)", "direct"),
    "SetColor":         ("Model.Tint = color", "direct"),
    "SetSkin":          ("Model.MaterialGroup", "direct"),
    "SetBodygroup":     ("Model.SetBodyGroup(name, val)", "method"),
    "LookupSequence":   ("Model.Model.AnimationNames.Contains(name)", "direct"),
    "GetSequence":      ("Model.SceneModel.CurrentSequence.Name", "direct"),
    "SetPlaybackRate":  ("Model.PlaybackRate =", "direct"),
    "LookupAttachment": ("Model.Model.GetAttachment(name)", "direct"),
    "GetAttachment":    ("Model.GetBoneTransform(id)", "direct"),
    # Combat
    "TakeDamage":       ("GameObject.TakeDamage(DamageInfo)", "direct"),
    "TakeDamageInfo":   ("GameObject.TakeDamage(DamageInfo)", "direct"),
    "Fire":             ("GameObject.TriggerIO", "bridge"),
    "GetEnemy":         ("Enemy", "field"),
    "SetEnemy":         ("Enemy =", "field"),
    # Animation (Source engine → delete/replace)
    "GetActivity":      ("[DELETE] Animgraph替代", "delete"),
    "SetActivity":      ("[DELETE] Animgraph替代", "delete"),
    "SelectWeightedSequence": ("[DELETE] AnimGraph替代", "delete"),
    "GetSequenceActivity":    ("[DELETE] 无ACT体系", "delete"),
    "TranslateActivity":      ("[DELETE] 无ACT体系", "delete"),
    "IsSequenceFinished":     ("SceneModel.CurrentSequence.IsFinished", "direct"),
    "SequenceDuration":       ("SceneModel.CurrentSequence.Duration", "direct"),
    # Schedule/Task (Source engine → delete)
    "StartSchedule":    ("Runner.Execute(schedule)", "bridge"),
    "ClearSchedule":    ("Runner.Cancel()", "bridge"),
    "StartEngineTask":  ("[DELETE] async/await替代", "delete"),
    "RunEngineTask":    ("[DELETE] async/await替代", "delete"),
    "TaskFinished":     ("[DELETE] Task完成即return", "delete"),
    "SetCondition":     ("SetCondition(Condition.X)", "method"),
    "ClearCondition":   ("ClearCondition(Condition.X)", "method"),
    "HasCondition":     ("HasCondition(Condition.X)", "method"),
    "Disposition":      ("DispositionSystem.Evaluate(self, other)", "bridge"),
    "AddEntityRelationship": ("DispositionSystem.SetClassRelation(...)", "bridge"),
    # Movement
    "IsMoving":         ("Agent.Velocity.Length > 10f", "direct"),
    "SetLastPosition":  ("LastPosition =", "field"),
    "GetCurWaypointPos": ("[DELETE] NavMeshAgent管理", "delete"),
    "GetNavType":       ("MovementType enum", "field"),
    "WaterLevel":       ("Physics.TestPoint", "bridge"),
    # Sound
    "EmitSound":        ("Sound.Play(snd, pos)", "direct"),
    "StopAllSounds":    ("SoundSystem.StopAll()", "method"),
    # Save system
    "SetSaveValue":     ("[DELETE] S&Box自动序列化", "delete"),
    "GetSaveValue":     ("[DELETE] S&Box自动序列化", "delete"),
    # Misc
    "GetPhysicsObject": ("Rigidbody", "direct"),
    "GetParent":        ("GameObject.Parent", "direct"),
    "SetParent":        ("GameObject.Parent =", "direct"),
    "SetOwner":         ("GameObject.Parent =", "direct"),
    "EntIndex":         ("GameObject.Id", "direct"),
}

GLOBALS = {
    "util.TraceLine":       ("SceneTrace.Ray(start, end).Run()", "direct"),
    "util.TraceHull":       ("SceneTrace.Box(mins, maxs, start, end).Run()", "direct"),
    "util.PointContents":   ("Physics.TestPoint(pos)", "bridge"),
    "timer.Simple":         ("GameTask.DelaySeconds(t)", "direct"),
    "timer.Create":         ("for + GameTask.DelaySeconds", "direct"),
    "timer.Remove":         ("CancellationToken.Cancel()", "bridge"),
    "ents.Create":          ("SceneUtility.CreatePrefab() or new GameObject()", "direct"),
    "ents.FindInSphere":    ("Scene.FindInPhysics(new Sphere(pos, r))", "direct"),
    "CurTime":              ("Time.Now", "direct"),
    "FrameTime":            ("Time.Delta", "direct"),
    "IsValid":              (".IsValid()", "direct"),
    "CreateSound":          ("Sound.Play(file, pos)", "direct"),
    "physenv.GetGravity":   ("Scene.PhysicsWorld.Gravity", "direct"),
    "ParticleEffect":       ("VFXHelper.PlayParticles(name, pos)", "bridge"),
    "ParticleEffectAttach": ("VFXHelper.AttachParticles(name, parent)", "bridge"),
    "debugoverlay.Box":     ("DebugOverlay.Box(...)", "direct"),
    "debugoverlay.Line":    ("DebugOverlay.Line(...)", "direct"),
    "debugoverlay.Text":    ("DebugOverlay.Text(...)", "direct"),
    "debugoverlay.Cross":   ("DebugOverlay.Line × 2", "direct"),
    "util.Decal":           ("VFXHelper.PlaceDecal(name, pos, norm)", "bridge"),
    "util.ScreenShake":     ("[TODO] ScreenShake API", "bridge"),
    "hook.Add":             ("Component lifecycle (OnUpdate)", "delete"),
    "hook.Remove":          ("CancellationToken", "delete"),
    "GetConVar":            ("ConVar or static VJConfig property", "bridge"),
    "math.random":          ("Game.Random.Next", "direct"),
    "math.Rand":            ("Game.Random.Float", "direct"),
    "math.Round":           ("MathF.Round", "direct"),
    "math.floor":           ("MathF.Floor", "direct"),
    "math.min":             ("MathF.Min", "direct"),
    "math.max":             ("MathF.Max", "direct"),
    "math.cos":             ("MathF.Cos", "direct"),
    "math.sin":             ("MathF.Sin", "direct"),
    "math.rad":             ("MathX.DegreeToRadian", "direct"),
    "math.deg":             ("MathX.RadianToDegree", "direct"),
    "math.atan2":           ("MathF.Atan2", "direct"),
    "bit.band":             ("&", "direct"),
    "bit.lshift":           ("<<", "direct"),
    "table.insert":         ("list.Add", "direct"),
    "table.remove":         ("list.RemoveAt", "direct"),
    "ipairs":               ("for (int i = 0; i < count; i++)", "direct"),
    "pairs":                ("foreach (var kv in dict)", "direct"),
    "string.find":          (".IndexOf / .Contains", "direct"),
    "string.sub":           (".Substring", "direct"),
    "string.gsub":          (".Replace", "direct"),
}

# ═══════════════════════════════════════════════════════
# Type inference from Lua usage patterns
# ═══════════════════════════════════════════════════════

TYPE_PATTERNS = {
    "float": [
        r"\b\d+\.\d+\b",              # 1.5, 0.3
        r"math\.(rand|Round|floor|min|max|cos|sin|abs)",
        r"CurTime\(\)", r"FrameTime\(\)",
        r"\.Distance\(", r":Length\(",
        r"ScaleByDifficulty\(",
        r"(Walk|Run|Fly|Swim|MeleeAttack|RangeAttack|LeapAttack).*Speed",
        r"(Attack|Bleed|Heal|Regen|Flinch).*(Damage|Amount|Time|Rate|Delay|Cooldown|Interval)",
        r"(Sight|Hearing|Melee|Range|Leap|Grenade|Cover|Call|Follow).*Distance",
    ],
    "int": [
        r"(Sound|Attack|Bleed).*Level\b",
        r"(Sound|Attack|Bleed).*Chance\b",
        r"\bCount\b", r"\bReps\b", r"\bIndex\b",
        r"math\.random\(1,\s*\d+\)",
    ],
    "string": [
        r"SoundTbl_\w+", r"AnimTbl_\w+", r"DecalTbl_\w+",
        r"\bModel\b.*=.*\".*\"", r"\bPrefab\b",
        r"(Print)?Name\b.*=.*\"",
        r"\bClass\b.*=.*\"",
    ],
    "bool": [
        r"\b(Has|Can|Is|Allow|Disable|Enable|Use|Should|Force)\w+\b",
        r"\bGodMode\b", r"\bDead\b", r"\bAlive\b", r"\bImmune_\w+\b",
        r"\bConstantlyFaceEnemy\b",
        r"= true\b", r"= false\b",
    ],
    "Vector3": [
        r"Vector\([^)]+\)", r"vec\w+",
        r"\.(GetPos|GetForward|GetRight|GetUp|EyePos|GetShootPos|WorldSpaceCenter|OBBCenter)",
    ],
    "Vector2": [
        r"VJ\.SET\(\d+,\s*\d+\)", r"\w+SoundPitch\b",
    ],
}

RISK_RULES = [
    ("nil_return",    r"if\s+!(\w+)\s+then\s+return\b", "C# 需改为 if (x == null) return"),
    ("coroutine",     r"\bcoroutine\.(wait|yield)\b", "coroutine.wait → await GameTask.DelaySeconds"),
    ("table_as_array", r"table\.insert", "table.insert → List<T>.Add，需确认元素类型"),
    ("table_as_dict", r"\w+\[(\".*?\"|\w+)\]", "table[x] → Dictionary<K,V> 或属性和字段"),
    ("gmod_metatable",r"FindMetaTable", "GMod 元表扩展 → C# 类方法"),
    ("global_var",    r"^[A-Z_]{3,}\s*=", "全局变量 → static 类字段或 ConVar"),
    ("timer_leak",    r"timer\.Create\(\"(\w+)\"", "timer.Create(name...) → CancellationTokenSource，需管理生命周期"),
    ("net_message",   r"\bnet\.(Start|Write|Read|Broadcast)\b", "net.* → [Rpc] 属性"),
    ("concommand",    r"\bconcommand\.Add\b", "concommand → [ConCmd] 属性"),
]

# ═══════════════════════════════════════════════════════
# Core analysis
# ═══════════════════════════════════════════════════════

@dataclass
class ApiCall:
    name: str
    csharp: str
    category: str           # direct | bridge | delete | calc | method | field | check
    line: int

@dataclass
class Dependency:
    name: str
    path: str
    kind: str               # require | include | module

@dataclass
class TypeGuess:
    name: str
    guessed_type: str
    evidence: str

@dataclass
class RiskFlag:
    rule: str
    description: str
    locations: List[int]

@dataclass
class MigrationCard:
    source_file: str
    source_lines: int
    module_purpose: str
    module_description: str
    dependencies: List[Dependency] = field(default_factory=list)
    api_calls: List[ApiCall] = field(default_factory=list)
    type_guesses: List[TypeGuess] = field(default_factory=list)
    risk_flags: List[RiskFlag] = field(default_factory=list)
    csharp_skeleton: str = ""
    manual_checks: List[str] = field(default_factory=list)
    log_points: List[str] = field(default_factory=list)
    regression_notes: List[str] = field(default_factory=list)


def parse_lua(filepath: str) -> MigrationCard:
    path = Path(filepath)
    text = path.read_text(encoding="utf-8", errors="ignore")
    lines = text.split("\n")
    total_lines = len(lines)

    card = MigrationCard(
        source_file=str(path.relative_to(path.parent.parent) if path.parent.parent.name else path.name),
        source_lines=total_lines,
        module_purpose="",
        module_description="",
    )

    # ── Extract module header ──
    header_lines = []
    in_header = False
    for i, line in enumerate(lines[:80]):
        stripped = line.strip()
        if stripped.startswith("--[[") or stripped.startswith("---"):
            in_header = True
            if stripped == "*/" or stripped == "*/": continue
            header_lines.append(stripped.lstrip("-[").lstrip("- ").strip())
        elif in_header and stripped.startswith("--"):
            if stripped == "*/" or stripped == "*/": continue
            header_lines.append(stripped.lstrip("- ").strip())
            if stripped == "*/": break
        elif in_header and not stripped.startswith("--"):
            break

    card.module_description = "\n".join(header_lines[:15])

    # ── Purpose guess ──
    card.module_purpose = _guess_purpose(path, header_lines)

    # ── Dependencies ──
    for i, line in enumerate(lines):
        m = re.search(r'(?:require|include)\s*\(\s*"([^"]+)"\s*\)', line)
        if m:
            card.dependencies.append(Dependency(
                name=m.group(1).split("/")[-1],
                path=m.group(1),
                kind="require"
            ))
        m2 = re.search(r'module\s*\(\s*"([^"]+)"', line)
        if m2:
            card.dependencies.append(Dependency(
                name=m2.group(1).split("/")[-1],
                path=m2.group(1),
                kind="module"
            ))

    # ── API calls ──
    seen = set()
    for i, line in enumerate(lines):
        # ENT:Method pattern
        for m in re.finditer(r'(?:self|ent\d*|ene|target|owner|ply)[\)\]]*:(\w+)\s*\(', line):
            name = m.group(1)
            if name in GMOD_TO_SBOX and name not in seen:
                csharp, cat = GMOD_TO_SBOX[name]
                card.api_calls.append(ApiCall(name, csharp, cat, i + 1))
                seen.add(name)

        # Global function patterns
        for m in re.finditer(
            r'(util\.\w+|timer\.\w+|ents\.\w+|hook\.\w+|CurTime|FrameTime|IsValid|'
            r'CreateSound|ParticleEffect|ParticleEffectAttach|'
            r'debugoverlay\.\w+|math\.\w+|bit\.\w+|table\.\w+|'
            r'ipairs|pairs|string\.\w+|GetConVar|physenv\.GetGravity|'
            r'net\.\w+|concommand\.\w+|gameevent\.\w+|'
            r'VJ\.\w+|list\.\w+|language\.\w+|'
            r'ents\.Find\w+|DamageInfo)', line):
            name = m.group(1)
            key = name.replace("(", "")
            if key in GLOBALS and key not in seen:
                csharp, cat = GLOBALS[key]
                card.api_calls.append(ApiCall(key, csharp, cat, i + 1))
                seen.add(key)

    # ── Type guesses ──
    guessed_names = set()
    for i, line in enumerate(lines):
        # [Property]-style variable declarations
        for m in re.finditer(
            r'(?:ENT\.|self\.|selfData\.)(\w+)\s*=\s*([^;]+)', line):
            name = m.group(1)
            value = m.group(2)
            if name in guessed_names:
                continue
            guessed_names.add(name)
            typ = _infer_type(name, value, i)
            if typ:
                card.type_guesses.append(TypeGuess(name, typ, f"line {i+1}: = {value[:60]}"))

    # ── Risk detection ──
    for rule_name, pattern, desc in RISK_RULES:
        locs = []
        for i, line in enumerate(lines):
            if re.search(pattern, line):
                locs.append(i + 1)
        if locs:
            card.risk_flags.append(RiskFlag(rule_name, desc, locs))

    # ── C# skeleton ──
    card.csharp_skeleton = _generate_skeleton(path, card)

    # ── Manual checks ──
    card.manual_checks = _generate_checks(card)

    # ── Log points ──
    card.log_points = _generate_log_points(card)

    return card


def _guess_purpose(path: Path, header: List[str]) -> str:
    name = path.stem.lower()
    if "core" in name:
        return "Think 循环 + 状态机 + 感知"
    if "schedule" in name:
        return "Schedule 定义 (行为模式)"
    if "task" in name:
        return "Task 定义 (原子操作)"
    if "base_aa" in name:
        return "AA 移动系统 (空中/水中)"
    if "base_tank" in name:
        return "坦克移动系统"
    if "creature" in name:
        return "生物 NPC 基类"
    if "human" in name:
        return "人类 NPC 基类 (武器/掩护/小队)"
    if "tank" in name or "tankg" in name:
        return "坦克/载具 NPC"
    if "funcs" in name:
        return "工具函数库"
    if "enums" in name:
        return "枚举定义"
    if "convars" in name:
        return "控制台变量"
    return "未知 (从文件头提取)"


def _infer_type(name: str, value: str, line_num: int) -> Optional[str]:
    combined = f"{name} = {value}"
    for typ, patterns in TYPE_PATTERNS.items():
        for pat in patterns:
            if re.search(pat, combined):
                return typ
    return None


def _generate_skeleton(path: Path, card: MigrationCard) -> str:
    name = path.stem
    class_name = _to_pascal(name)

    lines = []
    if card.module_purpose.startswith("枚举"):
        lines.append(f"public enum {class_name}")
        lines.append("{")
        lines.append("    // Values from Lua source")
        lines.append("}")
    elif any(d.kind == "module" for d in card.dependencies):
        lines.append(f"// Standalone utility — no Component base")
        lines.append(f"public static class {class_name}")
        lines.append("{")
        lines.append("    // Static methods from Lua source")
        lines.append("}")
    else:
        base = "BaseNPC"
        if "tankg" in name:
            base = "TankNPC"
        elif "tank" in name:
            base = "CreatureNPC"
        elif "human" in name:
            base = "CreatureNPC"
        elif "creature" in name:
            base = "BaseNPC"
        elif "grenade" in name or "gib" in name or "projectile" in name:
            base = "Component"

        lines.append(f"public partial class {class_name} : {base}")
        lines.append("{")
        lines.append("    // Properties inferred from Lua source:")
        for tg in card.type_guesses[:15]:
            cstype = _to_csharp_type(tg.guessed_type)
            lines.append(f"    [Property] public {cstype} {tg.name} {{ get; set; }}  // {tg.evidence}")
        lines.append("")
        lines.append("    // Key methods to implement:")
        for ac in [a for a in card.api_calls if a.category == "bridge"]:
            lines.append(f"    // TODO: {ac.name} → {ac.csharp}")
        lines.append("}")

    return "\n".join(lines)


def _to_pascal(snake: str) -> str:
    parts = snake.replace("_", " ").split()
    return "".join(p.capitalize() for p in parts)


def _to_csharp_type(guessed: str) -> str:
    return {"float": "float", "int": "int", "string": "string",
            "bool": "bool", "Vector3": "Vector3", "Vector2": "Vector2"}.get(guessed, "string")


def _generate_checks(card: MigrationCard) -> List[str]:
    checks = []
    # Bridge APIs need manual verification
    bridges = [a for a in card.api_calls if a.category == "bridge"]
    if bridges:
        checks.append("## 桥接 API (需人工确认语义)")
        for a in bridges:
            checks.append(f"- [ ] `{a.name}` → `{a.csharp}` (Lua line {a.line})")

    # Deleted APIs — verify no logic loss
    deleted = [a for a in card.api_calls if a.category == "delete"]
    if deleted:
        checks.append("## 删除的 API (确认无逻辑丢失)")
        for a in deleted:
            checks.append(f"- [ ] `{a.name}` — 原行 {a.line}，S&Box 无对应物")

    # Risks
    if card.risk_flags:
        checks.append("## 风险项")
        for r in card.risk_flags:
            checks.append(f"- [ ] **{r.rule}**: {r.description} (行: {', '.join(map(str, r.locations[:5]))})")

    # Dependencies
    if card.dependencies:
        checks.append("## 依赖确认")
        for d in card.dependencies:
            checks.append(f"- [ ] `{d.name}` ({d.kind}) — C# 中已实现？")

    return checks


def _generate_log_points(card: MigrationCard) -> List[str]:
    name = Path(card.source_file).stem
    logs = []
    if "core" in name:
        logs.extend([
            f'Log.Info($"[{{GetType().Name}}] Init: HP={{Health}}");',
            f'Log.Info($"[{{GetType().Name}}] State: {{old}}→{{new}}");',
            f'Log.Info($"[{{GetType().Name}}] Enemy: {{enemy.Name}} dist={{d}}");',
        ])
    if "creature" in name or "human" in name:
        logs.extend([
            f'Log.Info($"[{{GetType().Name}}] Attack={{type}} target={{e}}");',
            f'Log.Info($"[{{GetType().Name}}] Dmg={{d}} from={{src}} HP={{h}}");',
            f'Log.Info($"[{{GetType().Name}}] Killed by={{a}}");',
            f'Log.Info($"[{{GetType().Name}}] Immune={{tagType}}");',
        ])
    if not logs:
        logs.append(f'Log.Info($"[{_to_pascal(name)}] Loaded");')
    return logs


# ═══════════════════════════════════════════════════════
# Output formatters
# ═══════════════════════════════════════════════════════

def print_markdown(card: MigrationCard):
    print(f"# 迁移卡片: {card.source_file}")
    print()
    print(f"| 属性 | 值 |")
    print(f"|------|-----|")
    print(f"| 源文件 | `{card.source_file}` |")
    print(f"| 源行数 | {card.source_lines} |")
    print(f"| 模块用途 | {card.module_purpose} |")
    print(f"| API 调用 | {len(card.api_calls)} 个 |")
    print(f"| 类型推断 | {len(card.type_guesses)} 个 |")
    print(f"| 风险项 | {len(card.risk_flags)} 个 |")
    print()

    if card.module_description:
        print("## 模块说明")
        print(card.module_description)
        print()

    # ── API Inventory ──
    print(f"## API 清单 ({len(card.api_calls)} 个)")
    print()
    print("| API 调用 | C# 等价物 | 类型 | 行号 |")
    print("|----------|----------|------|------|")
    for a in sorted(card.api_calls, key=lambda x: (x.category, x.name)):
        cat_emoji = {"direct": "direct ", "bridge": "bridge", "delete": "delete",
                     "calc": "calc  ", "method": "direct ", "field": "direct ", "check": "direct "}
        print(f"| `{a.name}` | {a.csharp} | {cat_emoji.get(a.category, '')} {a.category} | {a.line} |")

    # Summary counts
    direct = sum(1 for a in card.api_calls if a.category == "direct")
    bridge = sum(1 for a in card.api_calls if a.category == "bridge")
    delete = sum(1 for a in card.api_calls if a.category == "delete")
    print()
    print(f"> direct  直接映射: {direct} | bridge 需桥接: {bridge} | delete 删除: {delete}")
    print()

    # ── Dependencies ──
    if card.dependencies:
        print(f"## 依赖 ({len(card.dependencies)} 个)")
        print()
        for d in card.dependencies:
            print(f"- `{d.path}` → {d.name} ({d.kind})")
        print()

    # ── Risk flags ──
    if card.risk_flags:
        print(f"## 风险标记 ({len(card.risk_flags)} 个)")
        print()
        for r in card.risk_flags:
            locs = ", ".join(map(str, r.locations[:5]))
            if len(r.locations) > 5:
                locs += f" ... ({len(r.locations)} total)"
            print(f"- ⚠️ **{r.rule}**: {r.description}")
            print(f"  - 位置: 行 {locs}")
        print()

    # ── Type guesses ──
    if card.type_guesses:
        print(f"## 类型推断 ({len(card.type_guesses)} 个)")
        print()
        print("| 变量名 | 推断类型 | 证据 |")
        print("|--------|---------|------|")
        for tg in card.type_guesses[:30]:
            cstype = _to_csharp_type(tg.guessed_type)
            print(f"| `{tg.name}` | `{cstype}` | {tg.evidence} |")
        print()

    # ── C# skeleton ──
    print("## C# 骨架")
    print()
    print("```csharp")
    print(card.csharp_skeleton)
    print("```")
    print()

    # ── Manual checks ──
    if card.manual_checks:
        print("## 人工检查项")
        print()
        for check in card.manual_checks:
            print(check)
        print()

    # ── Log points ──
    if card.log_points:
        print("## 建议日志点")
        print()
        for log in card.log_points:
            print(f"- `{log}`")
        print()


def print_json(card: MigrationCard):
    data = {
        "source_file": card.source_file,
        "source_lines": card.source_lines,
        "module_purpose": card.module_purpose,
        "api_count": len(card.api_calls),
        "direct_apis": sum(1 for a in card.api_calls if a.category == "direct"),
        "bridge_apis": sum(1 for a in card.api_calls if a.category == "bridge"),
        "delete_apis": sum(1 for a in card.api_calls if a.category == "delete"),
        "risk_count": len(card.risk_flags),
        "dependencies": [{"name": d.name, "path": d.path, "kind": d.kind} for d in card.dependencies],
        "api_calls": [{"name": a.name, "csharp": a.csharp, "category": a.category, "line": a.line} for a in card.api_calls],
        "type_guesses": [{"name": t.name, "type": t.guessed_type, "evidence": t.evidence} for t in card.type_guesses],
        "risks": [{"rule": r.rule, "desc": r.description, "locs": r.locations} for r in card.risk_flags],
        "csharp_skeleton": card.csharp_skeleton,
    }
    print(json.dumps(data, indent=2, ensure_ascii=False))


# ═══════════════════════════════════════════════════════
# CLI
# ═══════════════════════════════════════════════════════

def main():
    if len(sys.argv) < 2:
        print("Usage: python lua-inspector.py <file.lua> [--json]")
        print("       python lua-inspector.py <file.lua> > card.md")
        sys.exit(1)

    filepath = sys.argv[1]
    use_json = "--json" in sys.argv

    if not Path(filepath).exists():
        print(f"Error: {filepath} not found")
        sys.exit(1)

    card = parse_lua(filepath)

    if use_json:
        print_json(card)
    else:
        print_markdown(card)


if __name__ == "__main__":
    main()
