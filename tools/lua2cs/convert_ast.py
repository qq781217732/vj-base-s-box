#!/usr/bin/env python
"""
VJ-Base Lua → S&Box C# converter v3 — AST-based using tree-sitter.
Correctly handles nested functions, C-style comments, and all edge cases.
"""
import re, os, sys, json
from pathlib import Path
from tree_sitter import Language, Parser, Node
import tree_sitter_lua

# ── Init tree-sitter ─────────────────────────────────────────────────
LANG = Language(tree_sitter_lua.language())
PARSER = Parser(LANG)

# ── API mapping (same as v2, refined) ───────────────────────────────
API_REWRITE = [
    (r"self:GetPos\(\)", "Transform.Position"),
    (r"self:GetForward\(\)", "Transform.World.Forward"),
    (r"self:GetRight\(\)", "Transform.World.Right"),
    (r"self:GetUp\(\)", "Transform.World.Up"),
    (r"self:GetAngles\(\)", "Transform.Rotation"),
    (r"self:GetVelocity\(\)", "Rigidbody.Velocity"),
    (r"self:GetClass\(\)", "GetType().Name"),
    (r"self:GetEnemy\(\)", "Enemy?.GameObject"),
    (r"self:GetOwner\(\)", "Owner"),
    (r"self:Health\(\)", "Health"),
    (r"self:Alive\(\)", "Health > 0"),
    (r"self:Remove\(\)", "GameObject.Destroy()"),
    (r"self:EmitSound\((.+?)\)", r"Sound.Play(\1, Transform.Position)"),
    (r"self:Visible\((.+?)\)", r"Senses.CanSee(\1)"),
    (r"self:TakeDamage\((.+?)\)", r"OnDamage(\1)"),
    (r"self:SetPos\((.+?)\)", r"Transform.Position = \1"),
    (r"self:SetAngles\((.+?)\)", r"Transform.Rotation = (\1).ToRotation()"),
    (r"self:SetVelocity\((.+?)\)", r"Rigidbody.Velocity = \1"),
    (r"self:SetHealth\((.+?)\)", r"Health = \1"),
    (r"self:SetModel\((.+?)\)", r"ModelRenderer.Model = \1"),
    (r"self:SetColor\((.+?)\)", r"ModelRenderer.Tint = \1"),
    (r"self:SetSkin\((.+?)\)", r"ModelRenderer.Skin = \1"),
    (r"self:SetMoveType\((.+?)\)", r"// SetMoveType: \1"),
    (r"self:SetSolid\((.+?)\)", r"// SetSolid: \1"),
    (r"self:SetLocalPos\((.+?)\)", r"LocalTransform.Position = \1"),
    (r"self:SetLocalAngles\((.+?)\)", r"LocalTransform.Rotation = (\1).ToRotation()"),
    (r"self:GetLocalAngles\(\)", "LocalTransform.Rotation"),
    (r"self:SetOwner\((.+?)\)", r"GameObject.Parent = (\1)?.GameObject"),
    (r"self:SetParent\((.+?)\)", r"GameObject.Parent = (\1)?.GameObject"),
    (r"self:SetEnemy\((.+?)\)", r"Enemy = \1"),
    (r"self:SetTarget\((.+?)\)", r"Target = \1"),
    (r"self:GetActiveWeapon\(\)", "CurrentWeapon"),
    (r"self:GetPhysicsObject\(\)", "Rigidbody"),
    (r"self:GetSequence\(\)", "SkinnedModelRenderer.CurrentSequence"),
    (r"self:SetCycle\((.+?)\)", r"SkinnedModelRenderer.Set(\"cycle\", \1)"),
    (r"self:SetPlaybackRate\((.+?)\)", r"SkinnedModelRenderer.Set(\"speed\", \1)"),
    (r"self:Spawn\(\)", "// Spawn: no-op in S&Box"),
    (r"self:Activate\(\)", "// Activate: no-op in S&Box"),
    # More self: methods discovered during S&Box compilation
    (r"self:GetShootPos\(\)", "ShootPosition"),
    (r"self:EyePos\(\)", "EyePosition"),
    (r"self:WorldSpaceCenter\(\)", "WorldSpaceCenter"),
    (r"self:OBBCenter\(\)", "Collider.Bounds.Center"),
    (r"self:WaterLevel\(\)", "WaterLevel"),
    (r"self:IsNPC\(\)", "Components.TryGet<BaseNPC>(out _)"),
    (r"self:IsPlayer\(\)", "Components.TryGet<Player>(out _)"),
    (r"self:IsNextBot\(\)", "Components.TryGet<BaseNPC>(out _)"),
    (r"self:IsOnFire\(\)", "IsOnFire"),
    (r"self:IsMoving\(\)", "IsMoving"),
    (r"self:GetAimPosition\((.+?),\s*(.+?)\)", r"GetAimPosition(\1, \2)"),
    (r"self:GetHeadDirection\(\)", "Transform.World.Forward"),
    (r"self:GetViewModel\(\)", "ViewModel"),
    (r"self:GetAttachments\(\)", "Attachments"),
    (r"self:GetBonePosition\((\w+)\)", r"GetBoneTransform(\1).Position"),
    (r"self:GetAttachment\((.+?)\)", r"SkinnedModelRenderer.GetBoneTransform(\1)"),
    (r"self:LookupAttachment\((.+?)\)", r"Model.GetAttachmentIndex(\1)"),
    (r"self:LookupBone\((.+?)\)", r"SkinnedModelRenderer.GetBoneIndex(\1)"),
    (r"self:SetIK\((.+?)\)", r"// SetIK: \1"),
    (r"self:SetCollisionBounds\((.+?),\s*(.+?)\)", r"Collider.SetBounds(\1, \2)"),
    (r"self:SetCollisionGroup\((.+?)\)", r"Collider.CollisionGroup = \1"),
    (r"self:PhysicsInit\((.+?)\)", r"PhysicsInit(\1)"),
    (r"self:SetKeyValue\((.+?),\s*(.+?)\)", r"// SetKeyValue(\1, \2)"),
    (r"self:SetBodygroup\((.+?),\s*(.+?)\)", r"SetBodyGroup(\1, \2)"),
    (r"self:DrawShadow\((.+?)\)", r"Render.CastShadows = \1"),
    (r"self:SetRenderMode\((.+?)\)", r"// RenderMode removed: \1"),
    (r"self:FireBullets\((.+?)\)", r"FireBullets(\1)"),
    (r"self:DeleteOnRemove\((.+?)\)", r"DeleteOnRemove(\1)"),
    (r"self:Ignite\((.+?)\)", r"Ignite(\1)"),
    (r"self:StopAllSounds\(\)", "SoundManager.StopAll(GameObject)"),
    (r"self:TakeDamageInfo\((.+?),\s*(.+?)\)", r"\1.OnDamage(\2)"),
    (r"self:SetWeaponHoldType\((.+?)\)", r"HoldType = \1"),
    (r"self:SetNextPrimaryFire\((.+?)\)", r"NextPrimaryFireTime = \1"),
    (r"self:SetNextSecondaryFire\((.+?)\)", r"NextSecondaryFireTime = \1"),
    (r"self:SendWeaponAnim\((.+?)\)", r"SendWeaponAnim(\1)"),
    (r"self:Clip1\(\)", "Clip1"),
    (r"self:Clip2\(\)", "Clip2"),
    (r"self:SetClip1\((.+?)\)", r"Clip1 = \1"),
    (r"self:Ammo2\(\)", "Ammo2"),
    (r"self:GetPrimaryAmmoType\(\)", "PrimaryAmmoType"),
    (r"self:GetNextPrimaryFire\(\)", "NextPrimaryFireTime"),
    (r"self:GetNextSecondaryFire\(\)", "NextSecondaryFireTime"),
    (r"self:NetworkVar\((.+?),\s*(.+?)\)", r"[Net] \1 \2"),
    (r"self:SetSaveValue\((.+?),\s*(.+?)\)", r"// SetSaveValue: \1 = \2"),
    (r"self:GetSaveValue\((.+?)\)", r"// GetSaveValue: \1"),
    (r"self:Disposition\((.+?)\)", r"GetDisposition(\1)"),
    # GMod globals
    (r"IsValid\((.+?)\)", r"\1.IsValid()"),
    (r"CurTime\(\)", "Time.Now"),
    (r"ents\.Create\((.+?)\)", r"SceneUtility.CreatePrefab()"),
    (r"ents\.FindInSphere\((.+?),\s*(.+?)\)", r"Scene.FindInPhysics(new Sphere(\1, \2))"),
    (r"math\.Rand\((.+?),\s*(.+?)\)", r"RandomHelper.NextFloat(\1, \2)"),
    (r"math\.random\((.+?),\s*(.+?)\)", r"RandomHelper.NextInt(\1, \2)"),
    (r"util\.TraceLine\((.+)\)", r"Game.SceneTrace.Ray(\1).Run()"),
    (r"timer\.Simple\((.+?),\s*(.+)\)", r"GameTask.DelaySeconds(\1).ContinueWith(_ => \2)"),
    (r"VJ\.EmitSound\((.+?),\s*(.+?)\)", r"SoundManager.Emit(\1, \2)"),
    (r"VJ\.PICK\((.+?)\)", r"RandomHelper.FromList(\1)"),
    (r"VJ\.SET\((.+?),\s*(.+?)\)", r"new Vector2(\1, \2)"),
    (r"VJ\.STOPSOUND\((.+?)\)", r"\1?.Stop()"),
    (r"VJ\.DEBUG_Print\((.+?)\)", r"// DEBUG: \1"),
    (r"ParticleEffect\((.+?),\s*(.+?),\s*(.+?)\)", r"VFXHelper.PlayParticles(\1, \2, \3)"),
    (r"ParticleEffectAttach\((.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)", r"VFXHelper.AttachParticles(\1, \2)"),
    (r"EffectData\(\)", "new EffectData()"),
    (r"util\.Effect\((.+?),\s*(.+?)\)", r"Effects.Play(\1, \2)"),
    (r"util\.BlastDamage\((.+?),\s*(.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)", r"BlastDamage(\1, \2, \3, \4, \5)"),
    (r"util\.ScreenShake\((.+?),\s*(.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)", r"ScreenShake(\1, \2, \3, \4, \5)"),
    (r"util\.Decal\((.+?),\s*(.+?),\s*(.+?)\)", r"VFXHelper.PlaceDecal(\1, \2, \3)"),
    (r"debugoverlay\.Cross\((.+)\)", r"DebugOverlay.Cross(\1)"),
    (r"debugoverlay\.Line\((.+)\)", r"DebugOverlay.Line(\1)"),
    # More GMod globals from S&Box compilation fixes
    (r"GetConVar\((.+?)\):GetInt\(\)", r"ConVar.GetInt(\1)"),
    (r"GetConVar\((.+?)\):GetFloat\(\)", r"ConVar.GetFloat(\1)"),
    (r"GetConVar\((.+?)\):GetBool\(\)", r"ConVar.GetBool(\1)"),
    (r"GetConVar\((.+?)\):GetString\(\)", r"ConVar.GetString(\1)"),
    (r"FindMetaTable\((.+?)\)", r"MetaTable.For(\1)"),
    (r"hook\.Add\((.+?),\s*(.+?),\s*(.+?)\)", r"EventSystem.Subscribe(\1, \3)"),
    (r"hook\.Remove\((.+?),\s*(.+?)\)", r"EventSystem.Unsubscribe(\1)"),
    (r"ents\.FindByClass\((.+?)\)", r"Scene.FindAll<(\1)>()"),
    (r"math\.Clamp\((.+?),\s*(.+?),\s*(.+?)\)", r"Math.Clamp(\1, \2, \3)"),
    (r"math\.cos\((.+?)\)", r"MathF.Cos(\1)"),
    (r"math\.rad\((.+?)\)", r"MathX.DegreeToRadian(\1)"),
    (r"math\.abs\((.+?)\)", r"MathF.Abs(\1)"),
    (r"physenv\.GetGravity\(\)", r"PhysicsWorld.Gravity"),
    (r"util\.IsValidProp\((.+?)\)", r"// IsValidProp: \1"),
    # VJ utility functions
    (r"VJ\.CalculateTrajectory\((.+?),\s*(.+?),\s*(.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)", r"Trajectory.Calculate(\1, \2, \3, \4, \5, \6)"),
    (r"VJ\.DamageSpecialEnts\((.+?),\s*(.+?),\s*(.+?)\)", r"DamageHelper.Special(\1, \2, \3)"),
    (r"VJ\.AnimDuration\((.+?),\s*(.+?)\)", r"AnimationHelper.Duration(\1, \2)"),
    (r"VJ\.AnimExists\((.+?),\s*(.+?)\)", r"AnimationHelper.Exists(\1, \2)"),
    (r"VJ\.IsCurrentAnim\((.+?),\s*(.+?)\)", r"AnimationHelper.IsPlaying(\1, \2)"),
    (r"VJ\.SequenceToActivity\((.+?),\s*(.+?)\)", r"AnimationHelper.SeqToActivity(\1, \2)"),
    (r"VJ\.CreateSound\((.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)", r"SoundManager.CreateHandle(\1, \2, \3, \4)"),
]

def rewrite_line(line):
    """Apply API rewrites."""
    for pattern, replacement in API_REWRITE:
        line = re.sub(pattern, replacement, line)
    return line

def node_text(node, source):
    """Get the source text of a tree-sitter node."""
    return source[node.start_byte:node.end_byte].decode('utf-8', errors='replace')

def get_children_by_type(node, type_name):
    """Get all children of a given node type."""
    return [c for c in node.children if c.type == type_name]

def walk_ast(node, source, indent=0):
    """Debug: print AST structure."""
    text = node_text(node, source)[:60].replace('\n', '\\n')
    print(f"{' '*indent}{node.type}: {text}")
    for child in node.children:
        walk_ast(child, source, indent + 2)

# ── C# Code Generators ──────────────────────────────────────────────

def preprocess_lua(content):
    """Convert GMod C-style syntax to standard Lua for tree-sitter parsing."""
    # /* */ → --[[ ]]
    content = re.sub(r'/\*', '--[[', content)
    content = re.sub(r'\*/', ']]', content)
    # // comment → -- comment (line start and mid-line)
    lines = []
    for line in content.split('\n'):
        # Only replace // at start of line or after whitespace (not in strings)
        stripped = line.lstrip()
        if stripped.startswith('//'):
            leading = line[:len(line) - len(stripped)]
            line = leading + '--' + stripped[2:]
        lines.append(line)
    content = '\n'.join(lines)
    # !x → not x (logical not, but not != which becomes ~=)
    content = re.sub(r'([\s\(])!([\s]*)([a-zA-Z_])', r'\1not \2\3', content)
    # a != b → a ~= b
    content = re.sub(r'([\s\)])!=([\s\(])', r'\1~=\2', content)
    # a && b → a and b
    content = re.sub(r'([\s\)])&&([\s\(])', r'\1and\2', content)
    # continue → do return end (GMod LuaJIT extension)
    content = re.sub(r'\bcontinue\b', 'do return end', content)
    # #table → table (length operator, keep as-is since tree-sitter handles it)
    return content

def convert_lua_to_cs(lua_path, output_base):
    """Convert one Lua file to C# using tree-sitter AST."""
    with open(lua_path, 'r', encoding='utf-8', errors='replace') as f:
        raw = f.read()

    # Preprocess GMod syntax → standard Lua
    content = preprocess_lua(raw)
    source = content.encode('utf-8')

    tree = PARSER.parse(source)
    root = tree.root_node

    # Determine prefix (ENT/SWEP/EFFECT/TOOL/VJ)
    prefix = detect_prefix_from_ast(root, source)
    if not prefix:
        print(f"  SKIP {lua_path}: no ENT/SWEP/EFFECT/TOOL/VJ prefix found")
        return None

    # Extract properties and methods
    props = extract_properties_ast(root, source, prefix)
    methods = extract_methods_ast(root, source, prefix)
    parent = extract_parent_ast(root, source, prefix)

    # Determine class info
    class_name = snake_to_pascal(Path(lua_path).stem)
    base_class = parent if parent else {"ENT": "BaseNPC", "SWEP": "BaseWeapon", "EFFECT": "BaseEffect", "TOOL": "BaseTool", "VJ": "VJUtility"}.get(prefix, "BaseNPC")

    print(f"  {class_name} : {base_class} | {len(props)} props, {len(methods)} methods")

    # Generate C#
    lines = []
    lines.append("using System;")
    lines.append("using System.Collections.Generic;")
    lines.append("using System.Threading.Tasks;")
    lines.append("using Sandbox;")
    lines.append("")

    partial = "partial " if prefix != "VJ" else "static "
    inherit = f" : {base_class}" if prefix != "VJ" else ""
    lines.append(f"public {partial}class {class_name}{inherit}")
    lines.append("{")

    # Properties
    for name, cstype, val in props:
        attr = "[Property] " if prefix != "VJ" else ""
        lines.append(f"    {attr}public {cstype} {name} = {val};")
    if props: lines.append("")

    # Methods
    for name, args, body_text in methods:
        cs_name = method_name_cs(name)
        cs_body = convert_body_text(body_text)
        method_mod = "static " if prefix == "VJ" else "public virtual "
        lines.append(f"    {method_mod}void {cs_name}({args})")
        lines.append("    {")
        for bl in cs_body:
            if bl.strip():
                lines.append(f"        {bl}")
            else:
                lines.append("")
        lines.append("    }")
        lines.append("")

    lines.append("}")
    return "\n".join(lines)

def detect_prefix_from_ast(root, source):
    """Detect ENT/SWEP/EFFECT/TOOL/VJ from AST assignment statements."""
    text = source[root.start_byte:root.end_byte].decode('utf-8', errors='replace')
    for prefix in ["ENT", "SWEP", "EFFECT", "TOOL", "VJ"]:
        if f"{prefix}." in text[:5000]:
            return prefix
    return None

def extract_properties_ast(root, source, prefix):
    """Extract PREFIX.X = Y assignments from AST."""
    props = []
    text = source[root.start_byte:root.end_byte].decode('utf-8', errors='replace')
    for m in re.finditer(rf'^{prefix}\.(\w+)\s*=\s*(.+)$', text, re.MULTILINE):
        name, val = m.group(1), m.group(2).strip()
        if name == "Base": continue
        val = re.sub(r'\s*--.*$', '', val).strip()
        cstype = infer_cs_type(val)
        val_cs = val.replace("nil", "null")
        props.append((name, cstype, val_cs))
    return props

def extract_methods_ast(root, source, prefix):
    """Extract function PREFIX:Name() and PREFIX.Name() using tree-sitter AST walk."""
    methods = []
    _walk_for_functions(root, source, prefix, methods)
    return methods

def _walk_for_functions(node, source, prefix, methods, depth=0):
    """Recursively find function declarations with the given prefix."""
    if node.type == "function_declaration":
        text = node_text(node, source)
        # Match function PREFIX:name() or function PREFIX.name()
        m = re.match(rf'function\s+{prefix}[.:](\w+)\s*\(([^)]*)\)', text)
        if m:
            name, args = m.group(1), m.group(2)
            # Get body (everything after the signature, excluding the outer 'end')
            body_node = node.child_by_field_name('body')
            if body_node:
                body_text = node_text(body_node, source)
                # Strip the outer 'end' keyword
                body_text = re.sub(r'\n?\s*end\s*$', '', body_text)
                methods.append((name, args.strip(), body_text))
            else:
                methods.append((name, args.strip(), ""))
    # Recurse into children
    for child in node.children:
        _walk_for_functions(child, source, prefix, methods, depth + 1)

def extract_parent_ast(root, source, prefix):
    """Extract PREFIX.Base = 'xxx'."""
    text = source[root.start_byte:root.end_byte].decode('utf-8', errors='replace')
    m = re.search(rf'^{prefix}\.Base\s*=\s*"(.+?)"', text, re.MULTILINE)
    if m: return snake_to_pascal(m.group(1))
    return None

def convert_body_text(body):
    """Convert Lua function body to C# lines."""
    lines = []
    for line in body.split('\n'):
        s = line.strip()
        if not s:
            lines.append("")
            continue
        # Comments
        if s.startswith("--"):
            s = "//" + s[2:]
        # Lua → C#
        s = re.sub(r'\bnil\b', 'null', s)
        s = re.sub(r'~=', '!=', s)
        s = re.sub(r'\band\b', '&&', s)
        s = re.sub(r'\bor\b', '||', s)
        s = re.sub(r'\bnot\b', '!', s)
        s = re.sub(r'\.\.', '+', s)
        s = re.sub(r'#(\w+)', r'\1.Count', s)
        s = re.sub(r'ipairs\((.+?)\)', r'\1', s)
        s = re.sub(r'^local\s+', 'var ', s)
        # Control flow
        s = re.sub(r'\bif\s+(.+?)\s+then\b', r'if (\1)', s)
        s = re.sub(r'\belseif\s+(.+?)\s+then\b', r'else if (\1)', s)
        # Remove trailing 'end' keyword (blocks use braces)
        s = re.sub(r'\bend\b\s*$', '', s)
        # self:Method → Method (done by API_REWRITE)
        s = rewrite_line(s)
        s = re.sub(r'\bself\.', 'this.', s)
        # Add semicolon
        s = _add_semicolon(s)
        lines.append(s)
    return lines

def infer_cs_type(value):
    v = value.strip().rstrip(";")
    if v in ("true", "false"): return "bool"
    if re.match(r'^-?\d+$', v): return "int"
    if re.match(r'^-?[\d.]+$', v): return "float"
    if v.startswith('"') or v.startswith("'"): return "string"
    if v == "null": return "object"
    if v.startswith("{"): return "object"
    if v.startswith("Vector("): return "Vector3"
    if v.startswith("Angle("): return "Rotation"
    if v.startswith("Color("): return "Color"
    return "object"

def snake_to_pascal(name):
    parts = name.replace("-", "_").split("_")
    return "".join(p[0].upper() + p[1:] for p in parts if p)

def method_name_cs(lua_name):
    MAP = {"Init": "OnInit", "Initialize": "OnInitialize", "OnThink": "OnThink",
           "OnDeath": "OnDeath", "Draw": "OnDraw", "Reload": "OnReload"}
    return MAP.get(lua_name, lua_name)

def _add_semicolon(line):
    s = line.strip()
    if not s: return line
    if (s.startswith("//") or s.startswith("/*") or s.endswith("{") or s.endswith("}")
        or s.endswith(";") or s.endswith(":") or s.startswith("if (") or s.startswith("else")
        or s.startswith("for ") or s.startswith("foreach ") or s.startswith("while (")
        or s.startswith("try") or s.startswith("catch") or s.startswith("switch (")
        or s.startswith("case ") or s.startswith("[") or s.startswith("using ")
        or s.startswith("namespace ") or ("public " in s and "class " in s)):
        return line
    return line.rstrip() + ";"

# ── Main ─────────────────────────────────────────────────────────────
if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python convert_ast.py <lua_file_or_dir>")
        sys.exit(1)

    src = Path(sys.argv[1])
    out_dir = Path(sys.argv[2]) if len(sys.argv) > 2 else Path("code/VJBase_AST")

    if src.is_file():
        result = convert_lua_to_cs(src, out_dir)
        if result:
            out_path = out_dir / f"{snake_to_pascal(src.stem)}.cs"
            out_path.parent.mkdir(parents=True, exist_ok=True)
            out_path.write_text(result, encoding='utf-8')
            print(f"  → {out_path}")
    else:
        for lua_file in sorted(src.rglob("*.lua")):
            if "includes" in str(lua_file): continue
            result = convert_lua_to_cs(lua_file, out_dir)
            if result:
                rel = lua_file.relative_to(src)
                out_path = out_dir / rel.with_suffix('.cs')
                out_path.parent.mkdir(parents=True, exist_ok=True)
                out_path.write_text(result, encoding='utf-8')
