"""
验证 72 文件跳过分组 —— 列出每个 Lua 文件的精确分类
"""
import os

BASE = "F:/DevProject/Sbox/VJ-Base-master/lua"

# 分类规则
def classify(filepath):
    rel = os.path.relpath(filepath, BASE).replace("\\", "/")

    # 1. 具体武器 (20个)
    if rel.startswith("weapons/weapon_vj_") and rel != "weapons/weapon_vj_base/shared.lua":
        return "20 weapons", rel

    # 2. GMod Toolgun (9个)
    if "gmod_tool" in rel or rel.startswith("weapons/weapon_vj_controller.lua"):
        return "9 toolgun", rel

    # 3. Effects (9个 + 1 extra = 10)
    if rel.startswith("effects/"):
        return "9 effects", rel

    # 4. Shared.lua entity registration (9个) - 那些是纯粹注册文件
    # 实际上 shared.lua 包含 ENT 定义，但一部分是注册性质
    # user says 9 shared.lua are skipped

    # 5. Test NPCs (5个)
    if "test_" in rel:
        return "5 test NPCs", rel

    # 6. Menu/UI (4个)
    if rel.startswith("vj_base/menu/"):
        return "4 menu/ui", rel

    # 7. Resource data tables (4个)
    if rel.startswith("vj_base/resources/"):
        return "4 resources", rel

    # 8. Other entities (7个) - GMod specific or not needed
    # Let's see what remains in entities/ that isn't core
    skip_entities = [
        "entities/obj_vj_controller/",  # GMod controller entity
    ]
    for se in skip_entities:
        if rel.startswith(se) or rel == se.rstrip("/") + ".lua":
            return "7 other entities", rel

    # 9. Other modules (5个) - GMod specific
    skip_modules = [
        "vj_base/convars.lua",
        "vj_base/debug.lua",
        "vj_base/hooks.lua",
        "vj_base/extensions/corpse.lua",
        "vj_base/extensions/music.lua",
    ]
    if rel in skip_modules:
        return "5 other modules", rel

    # 10. Autorun (2个)
    if rel.startswith("autorun/"):
        return "2 autorun", rel

    return "??? KEPT", rel


all_files = []
for dirpath, dirnames, filenames in os.walk(BASE):
    for fn in filenames:
        if fn.endswith(".lua"):
            all_files.append(os.path.join(dirpath, fn))

print(f"Total Lua files: {len(all_files)}\n")

cats = {}
for fp in sorted(all_files):
    cat, name = classify(fp)
    cats.setdefault(cat, []).append(name)

for cat in sorted(cats.keys()):
    files = cats[cat]
    print(f"[{cat}] ({len(files)} files)")
    for f in files:
        print(f"    {f}")
    print()

total_skipped = sum(len(v) for k, v in cats.items() if k != "??? KEPT")
total_kept = len(cats.get("??? KEPT", []))
print(f"Total skipped: {total_skipped}")
print(f"Total kept: {total_kept}")
print(f"Grand total: {total_skipped + total_kept}")
