#!/usr/bin/env python
"""
VJ-Base Lua call graph analyzer.
Uses ast-grep to extract function definitions and call relationships.
Outputs a markdown reference indexed by subsystem.
"""
import subprocess, json, re, sys, os, tempfile
from pathlib import Path
from collections import defaultdict

AST_GREP = "C:/Users/Katana/AppData/Roaming/npm/ast-grep.cmd"

def preprocess(content):
    """Convert GMod C-style syntax to standard Lua for ast-grep parsing."""
    # /* */ → --[[ ]]
    content = re.sub(r'/\*.*?\*/', '', content, flags=re.DOTALL)
    # // → --
    lines = []
    for line in content.split('\n'):
        stripped = line.lstrip()
        if stripped.startswith('//'):
            leading = line[:len(line) - len(stripped)]
            line = leading + '--' + stripped[2:]
        lines.append(line)
    content = '\n'.join(lines)
    # !x → not x, a != b → a ~= b, a && b → a and b, continue → do return end
    content = re.sub(r'([\s\(])!([\s]*)([a-zA-Z_])', r'\1not \2\3', content)
    content = re.sub(r'([\s\)])!=([\s\(])', r'\1~=\2', content)
    content = re.sub(r'([\s\)])&&([\s\(])', r'\1and\2', content)
    content = re.sub(r'\bcontinue\b', 'do return end', content)
    return content

def run_ast_grep(pattern, lang, path):
    """Run ast-grep and return JSON matches. Preprocesses GMod syntax."""
    try:
        # Read and preprocess
        with open(path, 'r', encoding='utf-8', errors='replace') as f:
            raw = f.read()
        cleaned = preprocess(raw)
        import tempfile, os
        tmp = tempfile.NamedTemporaryFile(mode='w', suffix='.lua', delete=False, encoding='utf-8')
        tmp.write(cleaned)
        tmp_path = tmp.name
        tmp.close()

        result = subprocess.run(
            [AST_GREP, "--pattern", pattern, "--lang", lang, "--json=stream", tmp_path],
            capture_output=True, text=True, timeout=15, shell=True,
            encoding='utf-8', errors='replace'
        )
        matches = []
        for line in result.stdout.strip().split('\n'):
            line = line.strip()
            if line.startswith('{'):
                try: matches.append(json.loads(line))
                except: pass
        os.unlink(tmp_path)
        return matches
    except:
        return []

def extract_all(src_root, out_path):
    """Extract function definitions and calls from all Lua files."""
    src = Path(src_root)

    # { file: { functions: [(name, line, args)], calls: [(caller, callee, line)] } }
    data = {}

    for lua_file in sorted(src.rglob("*.lua")):
        if "includes" in str(lua_file): continue
        rel = str(lua_file.relative_to(src))
        fpath = str(lua_file)

        # Find all function definitions
        funcs = run_ast_grep("function $_($$$)", "lua", fpath)
        # Find all function calls
        calls = run_ast_grep("$_($$$)", "lua", fpath)
        # Find all method calls
        methods = run_ast_grep("$_:$_($$$)", "lua", fpath)

        defined = set()
        func_list = []
        for m in funcs:
            text = m.get("text", "")
            # Parse: function NAME(args) or function PREFIX:NAME(args) or function PREFIX.NAME(args)
            sig = re.match(r'function\s+(?:(\w+)[:\.])?(\w+)\(([^)]*)\)', text)
            if sig:
                prefix, name, args = sig.group(1), sig.group(2), sig.group(3)
                full_name = f"{prefix}:{name}" if prefix else name
                line = m.get("range", {}).get("start", {}).get("line", 0) + 1
                if full_name not in defined:
                    func_list.append((full_name, line, args))
                    defined.add(full_name)

        call_list = []
        for m in calls:
            text = m.get("text", "")
            # Skip function definitions themselves
            if text.startswith("function "): continue
            # Parse simple calls: name(args)
            for match in re.finditer(r'\b(\w+)\(', text):
                name = match.group(1)
                if name not in ("if", "for", "while", "local", "return", "end", "then",
                               "else", "elseif", "and", "or", "not", "nil", "true", "false",
                               "type", "tonumber", "tostring", "pairs", "ipairs", "next",
                               "select", "unpack", "setmetatable", "getmetatable",
                               "print", "error", "assert", "pcall", "xpcall", "math",
                               "table", "string", "Vector", "Angle", "Color", "IsValid",
                               "CurTime", "ents", "util", "hook", "timer", "net", "file",
                               "game", "player", "team", "sound", "surface", "render",
                               "cam", "vgui", "derma", "constraint", "duplicator",
                               "saverestore", "undo", "navmesh", "ai", "engine",
                               "include", "AddCSLuaFile", "Msg", "GetConVar"):
                    continue
                line = m.get("range", {}).get("start", {}).get("line", 0) + 1
                call_list.append((name, line))

        # Method calls: obj:Method(args) → Method
        for m in methods:
            text = m.get("text", "")
            for match in re.finditer(r'(\w+):(\w+)\(', text):
                obj, method = match.group(1), match.group(2)
                if method not in ("GetPos", "SetPos", "GetAngles", "SetAngles", "Remove",
                                 "SetModel", "SetColor", "SetSkin", "Spawn", "Activate",
                                 "SetMoveType", "SetSolid", "SetOwner", "SetParent",
                                 "SetName", "SetHealth", "SetMaxHealth", "GetEnemy",
                                 "SetEnemy", "GetForward", "GetRight", "GetUp",
                                 "GetVelocity", "SetVelocity", "EmitSound", "Visible",
                                 "TakeDamage", "EyePos", "GetShootPos", "WorldSpaceCenter",
                                 "IsNPC", "IsPlayer", "Alive", "Health", "GetClass",
                                 "GetActiveWeapon", "GetSequence", "SetCycle", "GetCycle",
                                 "SetPlaybackRate", "GetPhysicsObject", "GetAttachment"):
                    line = m.get("range", {}).get("start", {}).get("line", 0) + 1
                    call_list.append((method, line))

        # Deduplicate calls
        unique_calls = list(set(call_list))
        unique_calls.sort()

        if func_list or unique_calls:
            data[rel] = {"functions": func_list, "calls": unique_calls}

    # Build reverse index: who calls each function?
    called_by = defaultdict(list)
    for file, info in data.items():
        for func_name, func_line, _ in info["functions"]:
            # Find callers
            for other_file, other_info in data.items():
                for call_name, call_line in other_info["calls"]:
                    if call_name == func_name or call_name == func_name.split(":")[-1]:
                        called_by[func_name].append((other_file, call_line))

    # Generate markdown
    lines = []
    lines.append("# VJ-Base Lua Call Graph Reference")
    lines.append("")
    lines.append("Auto-generated by `tools/analyze_callgraph.py`.")
    lines.append("Use this to trace Lua logic when writing C# equivalents.")
    lines.append("")
    lines.append("---")
    lines.append("")

    # Group by subsystem
    subsystems = {
        "AI Core (vj_base/ai/core.lua)": [],
        "AI Schedules (vj_base/ai/schedules.lua)": [],
        "AI Base AA (vj_base/ai/base_aa.lua)": [],
        "AI Base Tank (vj_base/ai/base_tank.lua)": [],
        "Utility Functions (vj_base/funcs.lua)": [],
        "VJ Base Other": [],
        "Creature Base": [],
        "Human Base": [],
        "Tank Base": [],
        "Weapons": [],
        "Effects": [],
        "Other": [],
    }

    for file, info in sorted(data.items()):
        if "core.lua" in file and "ai" in file:
            subsystems["AI Core (vj_base/ai/core.lua)"].append((file, info))
        elif "schedules.lua" in file:
            subsystems["AI Schedules (vj_base/ai/schedules.lua)"].append((file, info))
        elif "base_aa.lua" in file:
            subsystems["AI Base AA (vj_base/ai/base_aa.lua)"].append((file, info))
        elif "base_tank.lua" in file:
            subsystems["AI Base Tank (vj_base/ai/base_tank.lua)"].append((file, info))
        elif "funcs.lua" in file:
            subsystems["Utility Functions (vj_base/funcs.lua)"].append((file, info))
        elif "vj_base" in file:
            subsystems["VJ Base Other"].append((file, info))
        elif "creature_base" in file:
            subsystems["Creature Base"].append((file, info))
        elif "human_base" in file:
            subsystems["Human Base"].append((file, info))
        elif "tank_base" in file:
            subsystems["Tank Base"].append((file, info))
        elif "weapon" in file:
            subsystems["Weapons"].append((file, info))
        elif "effect" in file.lower():
            subsystems["Effects"].append((file, info))
        else:
            subsystems["Other"].append((file, info))

    for system, entries in subsystems.items():
        if not entries: continue
        lines.append(f"## {system}")
        lines.append("")
        for file, info in entries:
            lines.append(f"### `{file}`")
            lines.append("")
            if info["functions"]:
                lines.append("| Function | Line | Called By |")
                lines.append("|----------|------|-----------|")
                for func_name, func_line, args in info["functions"]:
                    callers = called_by.get(func_name, [])
                    caller_str = ", ".join(f"{f}:{l}" for f, l in callers[:3])
                    if len(callers) > 3: caller_str += f" (+{len(callers)-3} more)"
                    if not caller_str: caller_str = "—"
                    args_short = args[:30] + "..." if len(args) > 30 else args
                    lines.append(f"| `{func_name}({args_short})` | {func_line} | {caller_str} |")
                lines.append("")
            if info["calls"]:
                pass  # Call lists are in reverse index above
        lines.append("---")
        lines.append("")

    # Stats
    total_funcs = sum(len(i["functions"]) for i in data.values())
    total_files = len(data)
    lines.append(f"*{total_funcs} functions in {total_files} files*")
    lines.append("")

    with open(out_path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))

    print(f"Generated {out_path}")
    print(f"  {total_funcs} functions in {total_files} files")
    return data

if __name__ == "__main__":
    src = sys.argv[1] if len(sys.argv) > 1 else "f:/DevProject/Sbox/VJ-Base-master/lua"
    out = sys.argv[2] if len(sys.argv) > 2 else "f:/DevProject/Sbox/testzombie/docs/vjbase-callgraph.md"
    extract_all(src, out)
