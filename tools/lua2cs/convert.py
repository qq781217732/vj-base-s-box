#!/usr/bin/env python
"""
VJ-Base Lua → S&Box C# converter v2.
Preserves source folder hierarchy: lua/entities/xxx/ → Entities/Xxx/
Merges shared.lua + init.lua into single partial class.
"""
import re, os, sys, json
from pathlib import Path
from collections import defaultdict

# ── GMod → S&Box API rewrites ────────────────────────────────────────
# Applied line-by-line in method bodies
API_REWRITE = [
    # self:Get*() → property
    (r"self:GetPos\(\)",                              "Transform.Position"),
    (r"self:GetForward\(\)",                          "Transform.Forward"),
    (r"self:GetRight\(\)",                            "Transform.Right"),
    (r"self:GetUp\(\)",                               "Transform.Up"),
    (r"self:GetAngles\(\)",                           "Transform.Rotation"),
    (r"self:GetLocalAngles\(\)",                      "LocalTransform.Rotation"),
    (r"self:GetVelocity\(\)",                         "Rigidbody.Velocity"),
    (r"self:GetClass\(\)",                            "GetType().Name"),
    (r"self:GetEnemy\(\)",                            "Enemy?.GameObject"),
    (r"self:GetActiveWeapon\(\)",                     "CurrentWeapon"),
    (r"self:GetOwner\(\)",                            "Owner"),
    (r"self:GetSequence\(\)",                         "SkinnedModelRenderer.CurrentSequence"),
    (r"self:GetCycle\(\)",                            "SkinnedModelRenderer.GetFloat(\"cycle\")"),
    (r"self:GetPhysicsObject\(\)",                    "Rigidbody"),
    (r"self:GetShootPos\(\)",                         "ShootPosition"),
    (r"self:GetViewModel\(\)",                        "ViewModel"),
    (r"self:GetAimVector\(\)",                        "AimVector"),
    (r"self:GetHeadDirection\(\)",                    "HeadDirection"),
    (r"self:GetBulletPos\(\)",                        "BulletPosition"),
    (r"self:GetAttachments\(\)",                      "Attachments"),
    (r"self:GetBonePosition\((\w+)\)",                r"GetBoneTransform(\1).Position"),
    (r"self:GetAttachment\((.+?)\)",                  r"SkinnedModelRenderer.GetBoneTransform(\1)"),
    (r"self:GetLocalPos\(\)",                         "LocalTransform.Position"),
    (r"self:GetMaxHealth\(\)",                        "MaxHealth"),
    (r"self:GetTable\(\)",                            ""),  # remove, direct access

    # self:Set*() → property =
    (r"self:SetPos\((.+?)\)",                         r"Transform.Position = \1"),
    (r"self:SetAngles\((.+?)\)",                      r"Transform.Rotation = (\1).ToRotation()"),
    (r"self:SetLocalAngles\((.+?)\)",                 r"LocalTransform.Rotation = (\1).ToRotation()"),
    (r"self:SetVelocity\((.+?)\)",                    r"Rigidbody.Velocity = \1"),
    (r"self:SetLocalVelocity\((.+?)\)",               r"Rigidbody.Velocity = \1"),
    (r"self:SetHealth\((.+?)\)",                      r"Health = \1"),
    (r"self:SetMaxHealth\((.+?)\)",                   r"MaxHealth = \1"),
    (r"self:SetModel\((.+?)\)",                       r"ModelRenderer.Model = \1"),
    (r"self:SetColor\((.+?)\)",                       r"ModelRenderer.Tint = \1"),
    (r"self:SetSkin\((.+?)\)",                        r"SkinnedModelRenderer.Skin = \1"),
    (r"self:SetName\((.+?)\)",                        r"GameObject.Name = \1"),
    (r"self:SetLocalPos\((.+?)\)",                    r"LocalTransform.Position = \1"),
    (r"self:SetOwner\((.+?)\)",                       r"GameObject.Parent = (\1)?.GameObject"),
    (r"self:SetParent\((.+?)\)",                      r"GameObject.Parent = (\1)?.GameObject"),
    (r"self:SetEnemy\((.+?)\)",                       r"Enemy = \1"),
    (r"self:SetTarget\((.+?)\)",                      r"Target = \1"),
    (r"self:SetCycle\((.+?)\)",                       r"SkinnedModelRenderer.Set(\"cycle\", \1)"),
    (r"self:SetPlaybackRate\((.+?)\)",                r"SkinnedModelRenderer.Set(\"speed\", \1)"),
    (r"self:SetMoveType\((.+?)\)",                    r"// SetMoveType removed: \1"),
    (r"self:SetSolid\((.+?)\)",                       r"// SetSolid removed: \1"),
    (r"self:SetCollisionGroup\((.+?)\)",              r"Collider.CollisionGroup = \1"),
    (r"self:SetCollisionBounds\((.+?), (.+?)\)",      r"Collider.SetBounds(\1, \2)"),
    (r"self:SetIK\((.+?)\)",                          r"// SetIK: \1"),

    # self:Predicate() → C# equivalent
    (r"self:Visible\((.+?)\)",                        r"Senses.CanSee(\1)"),
    (r"self:Alive\(\)",                               "Health > 0"),
    (r"self:IsNPC\(\)",                               "GameObject.Components.TryGet<BaseNPC>(out _)"),
    (r"self:IsPlayer\(\)",                            "GameObject.Components.TryGet<Player>(out _)"),
    (r"self:IsNextBot\(\)",                           "GameObject.Components.TryGet<BaseNPC>(out _)"),
    (r"self:IsOnFire\(\)",                            "IsOnFire"),
    (r"self:WaterLevel\(\)",                          "WaterLevel"),
    (r"self:IsMoving\(\)",                            "Rigidbody.Velocity.Length > 0.1f"),
    (r"self:IsValid\(\)",                             "IsValid"),
    (r"self:IsWeapon\(\)",                            "GameObject.Components.TryGet<BaseWeapon>(out _)"),

    # self:Action()
    (r"self:Remove\(\)",                              "GameObject.Destroy()"),
    (r"self:Spawn\(\)",                               "// Spawn: no-op in S&Box"),
    (r"self:Activate\(\)",                            "// Activate: no-op in S&Box"),
    (r"self:EmitSound\((.+?)\)",                      r"Sound.Play(\1, Transform.Position)"),
    (r"self:StopAllSounds\(\)",                       "SoundManager.StopAll(this)"),
    (r"self:TakeDamage\((.+?)\)",                     r"GameObject.TakeDamage(\1)"),
    (r"self:TakeDamageInfo\((.+?),\s*(.+?)\)",        r"\1.TakeDamage(\2)"),
    (r"self:FireBullets\((.+?)\)",                    r"FireBullets(\1)"),
    (r"self:DeleteOnRemove\((.+?)\)",                 r"DeleteOnRemove(\1)"),
    (r"self:SetKeyValue\((.+?),\s*(.+?)\)",           r"// SetKeyValue(\1, \2)"),
    (r"self:Ignite\((.+?)\)",                         r"Ignite(\1)"),
    (r"self:PhysicsInit\((.+?)\)",                    r"PhysicsInit(\1)"),
    (r"self:DrawShadow\((.+?)\)",                     r"Render.CastShadows = \1"),
    (r"self:SetRenderMode\((.+?)\)",                  r"// RenderMode removed: \1"),
    (r"self:FrameAdvance\((.+?)\)",                   r"FrameAdvance(\1)"),
    (r"self:SetupBones\(\)",                          "SetupBones()"),
    (r"self:SetWeaponHoldType\((.+?)\)",              r"HoldType = \1"),
    (r"self:SetNextPrimaryFire\((.+?)\)",             r"NextPrimaryFireTime = \1"),
    (r"self:SetNextSecondaryFire\((.+?)\)",           r"NextSecondaryFireTime = \1"),
    (r"self:SendWeaponAnim\((.+?)\)",                 r"SendWeaponAnim(\1)"),
    (r"self:SetClip1\((.+?)\)",                       r"Clip1 = \1"),
    (r"self:Clip1\(\)",                               "Clip1"),
    (r"self:Clip2\(\)",                               "Clip2"),
    (r"self:Ammo2\(\)",                               "Ammo2"),
    (r"self:GetPrimaryAmmoType\(\)",                  "PrimaryAmmoType"),
    (r"self:GetNextPrimaryFire\(\)",                  "NextPrimaryFireTime"),
    (r"self:GetNextSecondaryFire\(\)",                "NextSecondaryFireTime"),
    (r"self:LookupAttachment\((.+?)\)",               r"Model.GetAttachmentIndex(\1)"),
    (r"self:LookupBone\((.+?)\)",                     r"SkinnedModelRenderer.GetBoneIndex(\1)"),
    (r"self:GetAttachment\((.+?)\)",                  r"SkinnedModelRenderer.GetBoneTransform(\1)"),
    (r"self:NetworkVar\((.+?),\s*(.+?)\)",            r"[Net] \1 \2"),

    # GMod global functions → S&Box
    (r"IsValid\((.+?)\)",                             r"\1.IsValid()"),
    (r"CurTime\(\)",                                  "Time.Now"),
    (r"SysTime\(\)",                                  "Stopwatch.GetTimestamp()"),
    (r"ents\.Create\((.+?)\)",                         r"SceneUtility.CreatePrefab()"),
    (r"ents\.FindInSphere\((.+?),\s*(.+?)\)",          r"Scene.FindInPhysics(\1, \2)"),
    (r"ents\.FindByClass\((.+?)\)",                    r"Scene.FindAll<(\1)>()"),
    (r"math\.Rand\((.+?),\s*(.+?)\)",                  r"Game.Random.NextFloat(\1, \2)"),
    (r"math\.random\((.+?),\s*(.+?)\)",                r"Game.Random.NextInt(\1, \2)"),
    (r"math\.Clamp\((.+?),\s*(.+?),\s*(.+?)\)",        r"Math.Clamp(\1, \2, \3)"),
    (r"math\.cos\((.+?)\)",                           r"MathF.Cos(\1)"),
    (r"math\.rad\((.+?)\)",                           r"MathX.DegreesToRadians(\1)"),
    (r"util\.TraceLine\((.+)\)",                       r"SceneTrace.Ray(\1).Run()"),
    (r"util\.TraceHull\((.+)\)",                       r"SceneTrace.Box(\1).Run()"),
    (r"util\.BlastDamage\((.+?),\s*(.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)",
                                                      r"BlastDamage(\1, \2, \3, \4, \5)"),
    (r"util\.ScreenShake\((.+?),\s*(.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)",
                                                      r"ScreenShake(\1, \2, \3, \4, \5)"),
    (r"util\.Decal\((.+?),\s*(.+?),\s*(.+?)\)",        r"Decals.Place(\1, \2, \3)"),
    (r"util\.Effect\((.+?),\s*(.+?)\)",                r"Effects.Play(\1, \2)"),
    (r"debugoverlay\.Cross\((.+)\)",                  r"DebugOverlay.Cross(\1)"),
    (r"debugoverlay\.Line\((.+)\)",                   r"DebugOverlay.Line(\1)"),
    (r"debugoverlay\.Text\((.+)\)",                   r"DebugOverlay.Text(\1)"),
    (r"debugoverlay\.Box\((.+)\)",                    r"DebugOverlay.Box(\1)"),
    (r"hook\.Add\((.+?),\s*(.+?),\s*(.+?)\)",          r"EventSystem.Subscribe(\1, \3)"),
    (r"hook\.Remove\((.+?),\s*(.+?)\)",                r"EventSystem.Unsubscribe(\1)"),
    (r"timer\.Simple\((.+?),\s*(.+)\)",               r"GameTask.DelaySeconds(\1).ContinueWith(_ => \2)"),
    (r"timer\.Create\((.+?),\s*(.+?),\s*(.+?),\s*(.+)\)",
                                                      r"TimerLoop(\1, \2, \3, () => \4)"),
    (r"FindMetaTable\((.+?)\)",                        r"MetaTable.For(\1)"),
    (r"GetConVar\((.+?)\):GetInt\(\)",                 r"ConVar.GetInt(\1)"),
    (r"GetConVar\((.+?)\):GetFloat\(\)",               r"ConVar.GetFloat(\1)"),
    (r"GetConVar\((.+?)\):GetBool\(\)",                r"ConVar.GetBool(\1)"),
    (r"GetConVar\((.+?)\):GetString\(\)",              r"ConVar.GetString(\1)"),
    (r"ParticleEffect\((.+?),\s*(.+?),\s*(.+?)\)",     r"Particles.Play(\1, \2, \3)"),
    (r"ParticleEffectAttach\((.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)",
                                                      r"Particles.Attach(\1, \2, \3, \4)"),
    (r"EffectData\(\)",                                "new EffectData()"),

    # VJ utility
    (r"VJ\.EmitSound\((.+?),\s*(.+?)\)",              r"SoundManager.Emit(\1, \2)"),
    (r"VJ\.CreateSound\((.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)",
                                                      r"SoundManager.CreateHandle(\1, \2, \3, \4)"),
    (r"VJ\.STOPSOUND\((.+?)\)",                        r"\1?.Stop()"),
    (r"VJ\.PICK\((.+?)\)",                             r"Game.Random.FromList(\1)"),
    (r"VJ\.SET\((.+?),\s*(.+?)\)",                     r"new Vector2(\1, \2)"),
    (r"VJ\.DEBUG_Print\((.+?)\)",                     r"// VJ.DEBUG: \1"),

    # Catch-all: self:Method(args) → Method(args) for custom entity methods
    # Must come AFTER specific GMod API patterns above
    (r"self:(\w+)\(\)",                              r"\1()"),
    (r"self:(\w+)\(([^)]+)\)",                       r"\1(\2)"),
    # entity:Method() → entity.Method() (calls on other objects)
    (r"(\w+):(\w+)\(\)",                             r"\1.\2()"),
    (r"(\w+):(\w+)\(([^)]+)\)",                      r"\1.\2(\3)"),
    (r"VJ\.CalculateTrajectory\((.+?),\s*(.+?),\s*(.+?),\s*(.+?),\s*(.+?),\s*(.+?)\)",
                                                      r"Trajectory.Calculate(\1, \2, \3, \4, \5, \6)"),
    (r"VJ\.DamageSpecialEnts\((.+?),\s*(.+?),\s*(.+?)\)",
                                                      r"DamageHelper.Special(\1, \2, \3)"),
    (r"VJ\.AnimDuration\((.+?),\s*(.+?)\)",            r"AnimationHelper.Duration(\1, \2)"),
    (r"VJ\.AnimExists\((.+?),\s*(.+?)\)",              r"AnimationHelper.Exists(\1, \2)"),
    (r"VJ\.IsCurrentAnim\((.+?),\s*(.+?)\)",           r"AnimationHelper.IsPlaying(\1, \2)"),
    (r"VJ\.SequenceToActivity\((.+?),\s*(.+?)\)",      r"AnimationHelper.SeqToActivity(\1, \2)"),
]

# ── Helpers ───────────────────────────────────────────────────────────

def snake_to_pascal(name):
    """npc_vj_creature_base → NpcVjCreatureBase, VJ_VehicleExhaust → VjVehicleExhaust"""
    parts = name.replace("-", "_").split("_")
    result = ""
    for p in parts:
        if not p:
            continue
        result += p[0].upper() + p[1:]
    return result

def dir_to_pascal(name):
    """VJ_Blood1 → VjBlood1, vj_base → VjBase, VJ_VehicleExhaust → VjVehicleExhaust"""
    parts = name.replace("-", "_").split("_")
    result = ""
    for p in parts:
        if not p:
            continue
        # Keep the original casing, just ensure first char is uppercase
        result += p[0].upper() + p[1:]
    return result

def infer_cs_type(value):
    """Guess C# type from Lua literal value."""
    v = value.strip().rstrip(";")
    if v in ("true", "false"):
        return "bool"
    if re.match(r'^-?\d+$', v):
        return "int"
    if re.match(r'^-?[\d.]+$', v):
        return "float"
    if v.startswith('"') or v.startswith("'"):
        return "string"
    if v == "nil" or v == "NULL":
        return "object"
    if v.startswith("{"):
        return "object"  # table → object/dict
    if v.startswith("Vector("):
        return "Vector3"
    if v.startswith("Angle("):
        return "Rotation"
    if v.startswith("Color("):
        return "Color"
    return "object"

def rewrite_line(line):
    """Apply all API rewrites to one line of method body."""
    for pattern, replacement in API_REWRITE:
        line = re.sub(pattern, replacement, line)
    return line

def convert_body_line(line):
    """Convert one line of Lua method body to C#."""
    s = line.strip()
    if not s:
        return ""

    # Lua comment → C# comment
    if s.startswith("--"):
        return "//" + s[2:]

    # local x = y → var x = y (but not local function)
    if s.startswith("local "):
        s = re.sub(r'^local ', 'var ', s)

    # self.X = Y → this.X = Y (keep self: separate from self.)
    # self.X access → this.X (or just X if it's a property defined on the class)

    # Lua operators → C#
    s = re.sub(r'\bnil\b', 'null', s)
    s = re.sub(r'~=', '!=', s)
    s = re.sub(r'\band\b', '&&', s)
    s = re.sub(r'\bor\b', '||', s)
    s = re.sub(r'\bnot\s+', '!', s)
    s = re.sub(r'\.\.', '+', s)
    s = re.sub(r'#(\w+)', r'\1.Count', s)
    s = re.sub(r'ipairs\((.+?)\)', r'\1', s)

    # Lua control flow → C#
    s = re.sub(r'\bif\s+(.+?)\s+then\b', r'if (\1)', s)
    s = re.sub(r'\belseif\s+(.+?)\s+then\b', r'else if (\1)', s)
    s = re.sub(r'\belse\b', 'else', s)  # unchanged, just confirm

    # Inline -- comment → //
    s = re.sub(r'\s--\s+(.*)$', r'  // \1', s)

    # Block-closing end keyword → remove (C# uses braces)
    s = re.sub(r'\bend\b\s*$', '', s)
    if s.strip() == '':
        return ''

    # Apply API mapping
    s = rewrite_line(s)

    # self.X.Y → X.Y (class properties are direct)
    s = re.sub(r'\bself\.', 'this.', s)

    # Add semicolons (Lua doesn't need them, C# does)
    s = _add_semicolon(s)

    return s


def _add_semicolon(line):
    """Add trailing semicolon to C# statements that need one."""
    s = line.strip()
    if not s:
        return line
    # Don't add semicolon to these
    no_semicolon = (
        s.startswith("//") or s.startswith("/*") or s.startswith("*") or  # comments
        s.endswith("{") or s.endswith("}") or s.endswith(";") or s.endswith(":") or  # blocks
        s.startswith("#") or s.startswith("[") or  # preprocessor / attributes
        s.startswith("using ") or s.startswith("namespace ") or  # declarations
        (s.startswith("public ") and any(kw in s for kw in ["class ", "enum ", "interface ", "struct "])) or
        (s.startswith("private ") and any(kw in s for kw in ["class ", "enum "])) or
        s.startswith("if (") or s.startswith("else ") or s.startswith("else if (") or  # control flow
        s.startswith("for ") or s.startswith("foreach ") or s.startswith("while (") or
        s.startswith("try") or s.startswith("catch") or s.startswith("finally") or
        s.startswith("do ") or s.startswith("switch (") or
        s.startswith("case ") or s.startswith("default:")  # switch cases
    )
    if no_semicolon:
        return line
    return line.rstrip() + ";"

def extract_properties(content, prefix):
    """Extract ENT.X = Y style property assignments."""
    props = []
    for m in re.finditer(rf'^{prefix}\.(\w+)\s*=\s*(.+)$', content, re.MULTILINE):
        name, val = m.group(1), m.group(2).strip()
        if name == "Base":
            continue
        # Clean inline comment
        val = re.sub(r'\s*--.*$', '', val).strip()
        cstype = infer_cs_type(val)
        val_cs = val.replace("nil", "null")
        props.append((name, cstype, val_cs))
    return props

def extract_methods(content, prefix):
    """Extract function PREFIX:MethodName(args) ... end blocks using ast-grep for correct nesting."""
    methods = []
    lines = content.split('\n')

    import subprocess, tempfile
    ast_grep = 'C:\\Users\\Katana\\AppData\\Roaming\\npm\\ast-grep.cmd'
    tmp_path = None
    try:
        with tempfile.NamedTemporaryFile(mode='w', suffix='.lua', delete=False, encoding='utf-8') as f:
            f.write(content)
            tmp_path = f.name

        # Run both colon-syntax (ENT:Method) and dot-syntax (VJ.Method) patterns
        for syntax in [':', '.']:
            result = subprocess.run(
                [ast_grep, '--pattern', f'function {prefix}{syntax}$_($$$)', '--lang', 'lua',
                 '--json=stream', tmp_path],
                capture_output=True, text=True, timeout=30, shell=True
            )
            for line in result.stdout.strip().split('\n'):
                line = line.strip()
                if not line.startswith('{'):
                    continue
                m = json.loads(line)
                text = m.get('text', '')
                rng = m.get('range', {})
                start_line = rng.get('start', {}).get('line', 0)
                end_line = rng.get('end', {}).get('line', 0)

                sig_match = re.match(rf'function\s+{prefix}[{syntax}](\w+)\(([^)]*)\)', text)
                if not sig_match:
                    continue
                name, args = sig_match.group(1), sig_match.group(2)

                body_lines = []
                for i in range(start_line + 1, end_line):
                    if i < len(lines):
                        body_lines.append(lines[i])
                body = '\n'.join(body_lines)

                methods.append((name, args.strip(), body.split('\n') if body else []))
    except Exception:
        pass
    finally:
        if tmp_path:
            try:
                os.unlink(tmp_path)
            except:
                pass
    return methods

def extract_parent_class(content, prefix):
    """Extract PREFIX.Base = 'xxx' to find parent class."""
    # Match both normal and commented-out Base assignments
    m = re.search(rf'^(?://\s*)?{prefix}\.Base\s*=\s*"(.+?)"', content, re.MULTILINE)
    if m:
        return snake_to_pascal(m.group(1))
    return None

def detect_prefix(content):
    """Detect the GMod module prefix: ENT, SWEP, EFFECT, TOOL, or VJ."""
    for prefix in ["ENT", "SWEP", "EFFECT", "TOOL", "VJ"]:
        if re.search(rf'^{prefix}\.\w+\s*=', content, re.MULTILINE):
            return prefix
        if re.search(rf'^function\s+{prefix}\.\w+', content, re.MULTILINE):
            return prefix
    return None

def prefix_to_base_class(prefix, parent, class_name=""):
    """Map GMod prefix to S&Box Component base class."""
    if parent:
        # Map known GMod parents to our Component types
        KNOWN_PARENTS = {
            "NpcVjCreatureBase": "CreatureNPC",
            "NpcVjHumanBase": "CreatureNPC",
            "BaseAnim": "BaseProjectile",
            "BaseEntity": "BaseNPC",
            "WeaponBase": "BaseWeapon",
        }
        return KNOWN_PARENTS.get(parent, parent)
    return {
        "ENT": "BaseNPC", "SWEP": "BaseWeapon",
        "EFFECT": "BaseEffect", "TOOL": "BaseTool",
        "VJ": "VJUtility",
    }.get(prefix, "BaseNPC")

def method_name_cs(lua_name):
    """Map Lua method names to C# override names."""
    MAP = {
        "Init": "OnInit", "Initialize": "OnInitialize",
        "OnThink": "OnThink", "OnThinkActive": "OnThinkActive",
        "OnTouch": "OnTouch", "OnDeath": "OnDeath",
        "SelectSchedule": "SelectSchedule",
        "Draw": "OnDraw", "DrawTranslucent": "OnDrawTranslucent",
        "OnRemove": "OnRemove", "CustomOnRemove": "OnCustomRemove",
        "Equip": "OnEquip", "Deploy": "OnDeploy", "Holster": "OnHolster",
        "Reload": "OnReload", "PrimaryAttack": "OnPrimaryAttack",
        "SecondaryAttack": "OnSecondaryAttack",
    }
    return MAP.get(lua_name, lua_name)

def generate_cs(class_name, base_class, properties, methods, usings=None, is_partial=True, is_static=False):
    """Generate C# code for a class."""
    lines = []
    if usings is None:
        usings = ["System", "System.Collections.Generic", "System.Threading.Tasks", "Sandbox"]
    for u in usings:
        lines.append(f"using {u};")
    lines.append("")

    static_mod = "static " if is_static else ""
    partial = "partial " if is_partial else ""
    inherit = f" : {base_class}" if base_class and not is_static else ""
    lines.append(f"public {static_mod}{partial}class {class_name}{inherit}")
    lines.append("{")

    # Properties
    field_mod = "static " if is_static else ""
    for name, cstype, val in properties:
        prop_attr = "[Property] " if not is_static else ""
        lines.append(f"    {prop_attr}public {field_mod}{cstype} {name} = {val};")
    if properties:
        lines.append("")

    # Methods
    method_mod = "static " if is_static else "public virtual "
    for name, args, body_lines in methods:
        cs_name = method_name_cs(name)
        cs_args = args.replace(", ", ", ") if args else ""
        lines.append(f"    {method_mod}void {cs_name}({cs_args})")
        lines.append("    {")
        for bl in body_lines:
            converted = convert_body_line(bl)
            if converted:
                lines.append(f"        {converted}")
            else:
                lines.append("")
        lines.append("    }")
        lines.append("")

    lines.append("}")
    return "\n".join(lines)


# ── Main converter ────────────────────────────────────────────────────

def preprocess(content):
    """Remove C-style block comments and normalize."""
    # /* ... */ → remove (GMod extension, not valid in standard Lua)
    content = re.sub(r'/\*.*?\*/', '', content, flags=re.DOTALL)
    return content

def convert_tree(src_root, out_root):
    """Walk the Lua source tree and convert everything to C#."""
    src = Path(src_root)
    out = Path(out_root)

    # Group entity-type dirs (those with shared.lua + init.lua)
    # Standalone files get their own group
    groups = {}  # key → { "files": [(name, content)], "is_entity": bool }

    # First pass: collect all files by their directory
    dir_files = defaultdict(list)
    for lua_file in sorted(src.rglob("*.lua")):
        rel = lua_file.relative_to(src)
        if rel.parts[0] == "includes":
            continue
        with open(lua_file, "r", encoding="utf-8", errors="replace") as f:
            content = preprocess(f.read())
        dir_files[str(rel.parent)].append((lua_file.name, content, lua_file))

    # Second pass: for each directory, split into entity groups and standalone files
    for dir_path, files in dir_files.items():
        # Entity dirs: dirs that have shared.lua and/or init.lua
        has_shared = any(f[0] == "shared.lua" for f in files)
        has_init = any(f[0] == "init.lua" for f in files)

        if has_shared or has_init:
            # This is an entity/weapon/effect dir → merge shared+init
            merged = "\n".join(c for _, c, _ in files)
            class_name = snake_to_pascal(Path(dir_path).name)
            if dir_path == ".":
                out_dir = out
            else:
                parts = Path(dir_path).parts
                out_parts = [dir_to_pascal(p) for p in parts]
                out_dir = out.joinpath(*out_parts)
            groups[dir_path] = {
                "class_name": class_name,
                "out_dir": out_dir,
                "content": merged,
            }
        else:
            # Standalone files → each file is its own class
            for fname, content, full_path in files:
                stem = Path(fname).stem
                class_name = snake_to_pascal(stem)
                if dir_path == ".":
                    out_dir = out
                else:
                    parts = Path(dir_path).parts
                    out_parts = [dir_to_pascal(p) for p in parts]
                    out_dir = out.joinpath(*out_parts)
                key = f"{dir_path}/{stem}"
                groups[key] = {
                    "class_name": class_name,
                    "out_dir": out_dir,
                    "content": content,
                }

    count = 0
    for key, info in groups.items():
        class_name = info["class_name"]
        out_dir = info["out_dir"]
        content = info["content"]
        prefix = detect_prefix(content)
        _write_class(out_dir, class_name, content, prefix)
        count += 1

    print(f"Generated {count} classes from {len(dir_files)} source directories")

def _write_class(out_dir, class_name, content, prefix):
    """Write a single C# class file."""
    # Detect base class
    base_class = "BaseNPC"
    if prefix:
        parent = extract_parent_class(content, prefix)
        base_class = prefix_to_base_class(prefix, parent)

    # Extract members
    props = extract_properties(content, prefix) if prefix else []
    methods = extract_methods(content, prefix) if prefix else []

    # Generate
    is_static = (prefix == "VJ")
    cs_code = generate_cs(class_name, base_class if not is_static else None, props, methods, is_static=is_static)

    # Write
    out_dir.mkdir(parents=True, exist_ok=True)
    out_path = out_dir / f"{class_name}.cs"
    with open(out_path, "w", encoding="utf-8", newline="\n") as f:
        f.write(cs_code)
    print(f"  {out_path}")

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: python convert.py <lua_source_dir> <cs_output_dir>")
        sys.exit(1)
    convert_tree(sys.argv[1], sys.argv[2])
