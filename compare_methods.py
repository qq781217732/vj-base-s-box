"""
对比 Lua skeleton 与 C# 分析结果 —— 查找遗漏方法
"""
import sys, os, re
from collections import defaultdict


# ============================================================
# 1. 解析 lua_skeleton.txt
# ============================================================
def parse_lua_skeleton(path):
    """解析 skeleton 文件，提取所有 ENT/SWEP 方法和 FUNC 方法"""
    entities = []  # [{print_name, base, methods: [name], fields: [name]}]
    standalone_funcs = []  # standalone FUNC not tied to TABLE
    current = None
    current_methods = []
    current_fields = []

    with open(path, "r", encoding="utf-8") as f:
        for line in f:
            line = line.rstrip()

            # TABLE ENT / TABLE SWEP 开始新实体
            if line.startswith("TABLE ENT") or line.startswith("TABLE SWEP") or line.startswith("TABLE VJ") or line.startswith("TABLE "):
                # 保存上一个实体
                if current is not None:
                    current["methods"] = current_methods
                    current["fields"] = current_fields
                    entities.append(current)
                # 开始新实体
                if "ENT" in line:
                    etype = "ENT"
                elif "SWEP" in line:
                    etype = "SWEP"
                elif "VJ" in line:
                    etype = "VJ"
                else:
                    etype = "OTHER"
                current = {
                    "type": etype,
                    "print_name": "",
                    "base": "",
                }
                current_methods = []
                current_fields = []

            elif line.strip().startswith("METH "):
                m = re.match(r'\s*METH\s+(\w+)\((.*)\)', line)
                if m:
                    current_methods.append(m.group(1))
                else:
                    m2 = re.match(r'\s*METH\s+(\w+)', line)
                    if m2:
                        current_methods.append(m2.group(1))

            elif line.strip().startswith("FIELD PrintName = "):
                pn = line.split('"')[1] if '"' in line else line.split("=")[1].strip()
                if current is not None:
                    current["print_name"] = pn

            elif line.strip().startswith("FIELD Base = "):
                base = line.split('"')[1] if '"' in line else line.split("=")[1].strip()
                if current is not None:
                    current["base"] = base

            elif line.strip().startswith("FIELD ") and current:
                m = re.match(r'\s*FIELD\s+(\w+)', line)
                if m:
                    current_fields.append(m.group(1))

            # 独立 FUNC ENT:xxx() / FUNC SWEP:xxx() 不在 TABLE 里
            elif line.strip().startswith("FUNC ") and ("ENT:" in line or "SWEP:" in line):
                m = re.match(r'FUNC\s+(ENT|SWEP):(\w+)', line)
                if m:
                    standalone_funcs.append({
                        "prefix": m.group(1),
                        "method": m.group(2),
                    })

    # 最后一个
    if current is not None:
        current["methods"] = current_methods
        current["fields"] = current_fields
        entities.append(current)

    return entities


# ============================================================
# 2. 解析 C# 分析输出
# ============================================================
def parse_cs_output(path):
    """解析 C# 分析输出的 txt"""
    classes = {}  # class_name -> {methods: [name], properties: [name]}
    current_class = None
    current_section = None

    with open(path, "r", encoding="utf-8") as f:
        for line in f:
            line = line.rstrip()

            m = re.match(r'\[Class\]\s+(\S+)', line)
            if m:
                current_class = m.group(1)
                classes[current_class] = {"methods": [], "properties": []}
                current_section = None

            elif "--- Methods ---" in line:
                current_section = "methods"
            elif "--- Properties ---" in line:
                current_section = "properties"

            elif current_section and current_class and line.strip():
                if current_section == "methods":
                    # public virtual void MethodName(args)  (line N)
                    m = re.match(r'\s*(?:public |private |protected |static |virtual |override |async |abstract )*\s*(?:static |virtual |override |async |abstract )*\s*(?:\S+\s+)?(\w+)\(', line)
                    if m:
                        classes[current_class]["methods"].append(m.group(1))
                elif current_section == "properties":
                    m = re.match(r'\s*(?:public |private |protected |static |virtual |override )*\s*(\S+)\s+(\w+)', line)
                    if m:
                        classes[current_class]["properties"].append(m.group(2))

    return classes


# ============================================================
# 3. 实体映射 (Lua PrintName → C# Class)
# ============================================================
ENTITY_MAP = {
    "VJ Base Creature": "CreatureNPC",
    "VJ Base Human": "HumanNPC",
    "VJ Base Tank": "TankNPC",
    "VJ Base Tank Gunner": "TankNPC",  # Gunner 合并到 TankNPC
    "VJ Base NPC Controller": "BaseNPC",  # Controller 逻辑在 BaseNPC
}

# ============================================================
# 4. 主流程
# ============================================================
def main():
    lua_path = "Code/sharechat/lua_skeleton.txt"
    cs_path = "Code/sharechat/vjbase_cs_structure.txt"

    lua_entities = parse_lua_skeleton(lua_path)
    cs_classes = parse_cs_output(cs_path)

    print("=" * 70)
    print("  Lua vs C# 方法对比")
    print("=" * 70)

    # 统计
    all_lua_methods = {}  # map_name -> set of methods
    for ent in lua_entities:
        pn = ent["print_name"]
        if pn not in all_lua_methods:
            all_lua_methods[pn] = set()
        all_lua_methods[pn].update(ent["methods"])

    for lua_name, cs_name in ENTITY_MAP.items():
        if cs_name not in cs_classes:
            print(f"\n[!] C# class '{cs_name}' not found in analysis!")
            continue

        lua_methods = set()
        # 合并同一 PrintName 的所有 TABLE ENT
        for ent in lua_entities:
            if ent["print_name"] == lua_name or (
                lua_name == "VJ Base Creature" and ent["print_name"] == "Player Spawnpoint"
            ):
                lua_methods.update(ent["methods"])

        # 也合并没有 PrintName 但有相关 Base 的实体
        if lua_name == "VJ Base Creature":
            for ent in lua_entities:
                if not ent["print_name"] and ent["base"] in ("", "base_entity"):
                    for m in ent["methods"]:
                        if m in ("TranslateActivity", "Think", "MaintainPropInteraction",
                                 "ExecuteMeleeAttack", "ExecuteRangeAttack", "ExecuteLeapAttack",
                                 "LeapAttackJump", "StopAttacks", "UpdatePoseParamTracking",
                                 "ResetEnemy", "OnTakeDamage", "BeginDeath", "FinishDeath",
                                 "CreateDeathCorpse", "SelectSchedule", "OnThink", "OnThinkActive",
                                 "PreInit", "Init", "OnDeath", "CustomOnRemove", "DeathWeaponDrop",
                                 "GetAttackSpread", "SCHEDULE_FACE", "MaintainAlertBehavior",
                                 "OnDamaged", "Tank_AngleDiffuse", "CanFireWeapon",
                                 "VJ_CheckAllFourSides", "DoWeaponAttackMovementCode",
                                 "IsScheduleFinished", "StartTask", "RunTask", "TaskTime",
                                 "StartEngineTask", "RunEngineTask", "StartEngineSchedule",
                                 "EngineScheduleFinish", "DoingEngineSchedule", "Corpse_Add",
                                 "AA_MoveAnimation"):
                            lua_methods.add(m)

        if not lua_methods:
            print(f"\n  {lua_name} → {cs_name}")
            print("  [!] No Lua methods found for this entity!")
            continue

        cs_methods = set(cs_classes[cs_name]["methods"])

        # 方法名规范化：Lua PascalCase → 直接对比
        common = lua_methods & cs_methods
        only_lua = lua_methods - cs_methods
        only_cs = cs_methods - lua_methods

        print(f"\n{'─'*70}")
        print(f"  {lua_name} → {cs_name}")
        print(f"  Lua: {len(lua_methods)} methods | C#: {len(cs_methods)} methods")
        print(f"  匹配: {len(common)} | 仅Lua: {len(only_lua)} | 仅C#: {len(only_cs)}")

        if only_lua:
            print(f"\n  [Lua有/C#无] 可能遗漏的方法:")
            for m in sorted(only_lua):
                print(f"    - {m}")

        if only_cs:
            print(f"\n  [C#有/Lua无] C# 新增的方法:")
            for m in sorted(only_cs):
                print(f"    + {m}")

    # ============================================================
    # 5. SWEP / BaseWeapon 对比
    # ============================================================
    print(f"\n{'='*70}")
    print(f"  SWEP vs BaseWeapon 对比")
    print(f"{'='*70}")

    swe_methods = set()
    for ent in lua_entities:
        if ent["type"] == "SWEP":
            swe_methods.update(ent["methods"])

    bw_methods = set(cs_classes.get("BaseWeapon", {}).get("methods", []))

    common = swe_methods & bw_methods
    only_lua = swe_methods - bw_methods
    only_cs = bw_methods - swe_methods

    print(f"  SWEP Lua: {len(swe_methods)} methods | C# BaseWeapon: {len(bw_methods)} methods")
    print(f"  匹配: {len(common)} | 仅Lua: {len(only_lua)} | 仅C#: {len(only_cs)}")

    if only_lua:
        print(f"\n  [SWEP有/BaseWeapon无] 可能遗漏:")
        for m in sorted(only_lua):
            print(f"    - {m}")

    if only_cs:
        print(f"\n  [BaseWeapon有/SWEP无] C# 新增:")
        for m in sorted(only_cs):
            print(f"    + {m}")

    # ============================================================
    # 6. 列出所有 Lua ENT 实体
    # ============================================================
    print(f"\n{'='*70}")
    print(f"  Lua 中所有 TABLE ENT (按 PrintName)")
    print(f"{'='*70}")
    for ent in lua_entities:
        if ent["type"] in ("ENT", "SWEP"):
            nm = ent["print_name"] or "(unnamed)"
            base = f" : {ent['base']}" if ent["base"] else ""
            print(f"  [{ent['type']}] {nm}{base} | {len(ent['methods'])} methods, {len(ent['fields'])} fields")


if __name__ == "__main__":
    main()
