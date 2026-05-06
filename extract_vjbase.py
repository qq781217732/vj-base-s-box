"""
VJ-Base 代码结构提取器 —— C# + Lua
使用 tree-sitter AST 提取: 类/表、方法/函数、属性/字段、调用关系
输出: AI 友好的结构化 JSON，用于 C# vs Lua 对比
"""
import sys, os, json, re
from collections import defaultdict
from tree_sitter import Parser, Language
import tree_sitter_c_sharp, tree_sitter_lua


CS_LANG = Language(tree_sitter_c_sharp.language())
LUA_LANG = Language(tree_sitter_lua.language())

# ============================================================
# C# 端
# ============================================================
def analyze_csharp_file(filepath):
    with open(filepath, "rb") as f:
        source = f.read()
    parser = Parser(CS_LANG)
    tree = parser.parse(source)

    def text_of(node):
        return source[node.start_byte:node.end_byte].decode()

    classes = []
    call_edges = []
    class_stack = []

    def walk(node):
        if node.type == "class_declaration":
            name_node = node.child_by_field_name("name")
            cls_name = text_of(name_node) if name_node else "?"
            base_node = node.child_by_field_name("base_list")
            bases = []
            if base_node:
                for c in base_node.named_children:
                    bases.append(text_of(c))
            cls = {
                "name": cls_name,
                "bases": bases,
                "line": node.start_point[0] + 1,
                "methods": [],
                "properties": [],
                "fields": [],
            }
            class_stack.append(cls)

        elif node.type == "method_declaration" and class_stack:
            name_node = node.child_by_field_name("name")
            mname = text_of(name_node) if name_node else "?"
            params_node = node.child_by_field_name("parameters")
            params = []
            if params_node:
                for p in params_node.named_children:
                    params.append(text_of(p).replace("\n", " "))
            ret_node = node.child_by_field_name("returns")
            returns = text_of(ret_node) if ret_node else "void"
            modifiers = [text_of(c) for c in node.children if c.type == "modifier"]
            class_stack[-1]["methods"].append({
                "name": mname, "params": params, "returns": returns,
                "modifiers": modifiers, "line": node.start_point[0] + 1,
            })

        elif node.type == "property_declaration" and class_stack:
            name_node = node.child_by_field_name("name")
            pname = text_of(name_node) if name_node else "?"
            type_node = node.child_by_field_name("type")
            ptype = text_of(type_node) if type_node else "?"
            modifiers = [text_of(c) for c in node.children if c.type == "modifier"]
            class_stack[-1]["properties"].append({
                "name": pname, "type": ptype, "modifiers": modifiers,
                "line": node.start_point[0] + 1,
            })

        elif node.type == "field_declaration" and class_stack:
            type_node = node.child_by_field_name("type")
            ftype = text_of(type_node) if type_node else "?"
            for child in node.named_children:
                if child.type == "variable_declarator":
                    fn = child.child_by_field_name("name")
                    fname = text_of(fn) if fn else "?"
                    modifiers = [text_of(c) for c in node.children if c.type == "modifier"]
                    class_stack[-1]["fields"].append({
                        "name": fname, "type": ftype, "modifiers": modifiers,
                        "line": node.start_point[0] + 1,
                    })

        elif node.type == "invocation_expression":
            fn_node = node.child_by_field_name("function")
            called = text_of(fn_node) if fn_node else "?"
            # 找最近的 class context
            cls_name = class_stack[-1]["name"] if class_stack else "?"
            call_edges.append({
                "class": cls_name,
                "called": called,
                "line": node.start_point[0] + 1,
            })

        for child in node.children:
            walk(child)

        if node.type == "class_declaration" and class_stack:
            classes.append(class_stack.pop())

    walk(tree.root_node)
    return {"file": filepath, "classes": classes, "calls": call_edges}


def analyze_csharp_project(root_dir):
    results = []
    for dirpath, dirnames, filenames in os.walk(root_dir):
        dirnames[:] = [d for d in dirnames if d not in ("obj", "bin", ".git", "Properties")]
        for fn in filenames:
            if fn.endswith(".cs"):
                fp = os.path.join(dirpath, fn)
                try:
                    results.append(analyze_csharp_file(fp))
                except Exception:
                    pass
    return results


# ============================================================
# Lua 端
# ============================================================
def analyze_lua_file(filepath):
    with open(filepath, "rb") as f:
        source = f.read()
    parser = Parser(LUA_LANG)
    tree = parser.parse(source)

    def text_of(node):
        return source[node.start_byte:node.end_byte].decode()

    # 提取 ENT/VJ 表的字段和方法
    tables = defaultdict(lambda: {"fields": [], "methods": [], "line": None})
    global_functions = []
    global_vars = []
    call_edges = []
    includes = []

    # Helpers: find child by type (since child_by_field_name unreliable in Lua grammar)
    def child_by_type(node, child_type):
        for c in node.children:
            if c.type == child_type:
                return c
        return None

    def named_children_of_type(node, child_type):
        return [c for c in node.named_children if c.type == child_type]

    def walk(node):
        # --- Table 字段赋值: ENT.Xxx = value ---
        if node.type == "assignment_statement":
            var_list = child_by_type(node, "variable_list")
            expr_list = child_by_type(node, "expression_list")
            if var_list and expr_list:
                dot_idx = child_by_type(var_list, "dot_index_expression")
                if dot_idx:
                    # dot_index_expression children: [identifier, ., identifier]
                    idents = named_children_of_type(dot_idx, "identifier")
                    if len(idents) >= 2:
                        table_name = text_of(idents[0])
                        field_name = text_of(idents[1])
                        val_text = text_of(named_children_of_type(expr_list, "string")[0]) if named_children_of_type(expr_list, "string") else (
                            text_of(named_children_of_type(expr_list, "true")[0]) if named_children_of_type(expr_list, "true") else
                            text_of(named_children_of_type(expr_list, "false")[0]) if named_children_of_type(expr_list, "false") else
                            text_of(named_children_of_type(expr_list, "number")[0]) if named_children_of_type(expr_list, "number") else
                            text_of(expr_list.named_children[0]) if expr_list.named_children else "?")
                        if table_name in ("ENT", "VJ"):
                            tables[table_name]["fields"].append({
                                "name": field_name, "value": val_text[:100],
                                "line": node.start_point[0] + 1,
                            })
                # 全局变量: 纯标识符赋值 (VJBASE_INSTALLED = true)
                else:
                    ident = child_by_type(var_list, "identifier")
                    if ident and expr_list.named_children:
                        vname = text_of(ident)
                        val_text = text_of(expr_list.named_children[0])[:80]
                        if vname.upper() == vname or vname.startswith("VJ_"):
                            global_vars.append({
                                "name": vname, "value": val_text, "line": node.start_point[0] + 1,
                            })

        # --- 函数定义: function ENT:Method(...) 或 function VJ.Func(...) ---
        elif node.type == "function_declaration":
            params_list = child_by_type(node, "parameters")
            params = [text_of(p) for p in (params_list.named_children if params_list else [])]

            # 找到函数名 (可能是 dot_index_expression, method_index_expression, identifier)
            # function ENT:Method -> method_index_expression
            # function VJ.Func -> dot_index_expression
            # function GlobalFunc -> identifier
            name_nodes = [c for c in node.named_children
                          if c.type in ("dot_index_expression", "method_index_expression", "identifier")
                          and c.prev_sibling and c.prev_sibling.type == "function"]
            if name_nodes:
                fname = text_of(name_nodes[0])

                if ":" in fname:
                    tbl, method = fname.split(":", 1)
                    if tbl in ("ENT", "self"):
                        tables[tbl]["methods"].append({
                            "name": method, "params": params, "line": node.start_point[0] + 1,
                        })
                    else:
                        global_functions.append({
                            "name": fname, "params": params, "line": node.start_point[0] + 1,
                        })
                elif "." in fname:
                    tbl, method = fname.rsplit(".", 1)
                    if tbl in ("ENT", "VJ"):
                        tables[tbl]["methods"].append({
                            "name": method, "params": params, "line": node.start_point[0] + 1,
                        })
                    else:
                        global_functions.append({
                            "name": fname, "params": params, "line": node.start_point[0] + 1,
                        })
                else:
                    global_functions.append({
                        "name": fname, "params": params, "line": node.start_point[0] + 1,
                    })

        # --- 方法调用 ---
        elif node.type == "function_call":
            args = child_by_type(node, "arguments")
            # 调用名可以是 identifier / dot_index_expression / method_index_expression
            for prefix_type in ("dot_index_expression", "method_index_expression", "identifier"):
                fn_node = child_by_type(node, prefix_type)
                if fn_node:
                    called = text_of(fn_node)
                    if len(called) < 120:
                        call_edges.append({
                            "called": called,
                            "line": node.start_point[0] + 1,
                        })
                    # include/AddCSLuaFile/require
                    if called in ("include", "AddCSLuaFile", "require") and args:
                        inc_paths = [text_of(a)[:120] for a in args.named_children]
                        for p in inc_paths:
                            includes.append({
                                "type": called, "path": p.strip('"').strip("'"),
                                "line": node.start_point[0] + 1,
                            })
                    break

        # --- 局部变量声明 ---
        elif node.type == "local_variable_declaration":
            for child in node.named_children:
                if child.type == "assignment_statement":
                    vl = child_by_type(child, "variable_list")
                    el = child_by_type(child, "expression_list")
                    if vl:
                        for ident in named_children_of_type(vl, "identifier"):
                            vname = text_of(ident)
                            val = text_of(el.named_children[0])[:80] if el and el.named_children else "?"
                            global_vars.append({
                                "name": vname, "value": val, "line": node.start_point[0] + 1,
                            })

        for child in node.children:
            walk(child)

    walk(tree.root_node)

    # 转换 tables defaultdict 为普通 dict
    tables_dict = {k: dict(v) for k, v in tables.items()}

    return {
        "file": filepath,
        "tables": tables_dict,
        "global_functions": global_functions,
        "global_vars": global_vars,
        "calls": call_edges[:200],  # 限制调用数
        "includes": includes,
    }


def analyze_lua_project(root_dir):
    results = []
    for dirpath, dirnames, filenames in os.walk(root_dir):
        for fn in filenames:
            if fn.endswith(".lua"):
                fp = os.path.join(dirpath, fn)
                try:
                    results.append(analyze_lua_file(fp))
                except Exception:
                    pass
    return results


# ============================================================
# 汇总 & 输出
# ============================================================
def summarize_csharp(all_data):
    """汇总 C# 项目的类、方法、属性统计"""
    all_classes = []
    all_methods = []
    for f in all_data:
        for cls in f["classes"]:
            cls["_file"] = f["file"]
            all_classes.append(cls)
            for m in cls.get("methods", []):
                m["_class"] = cls["name"]
                m["_file"] = f["file"]
                all_methods.append(m)

    # 调用关系简化
    all_calls = []
    for f in all_data:
        all_calls.extend(f["calls"])

    return {
        "total_files": len(all_data),
        "total_classes": len(all_classes),
        "total_methods": len(all_methods),
        "classes": all_classes,
        "call_graph": all_calls,
    }


def summarize_lua(all_data):
    """汇总 Lua 项目的表、函数、变量统计"""
    all_tables = {}
    all_global_funcs = []
    all_global_vars = []
    all_includes = []

    for f in all_data:
        for tname, tdata in f.get("tables", {}).items():
            if tname not in all_tables:
                all_tables[tname] = {"fields": [], "methods": [], "files": []}
            all_tables[tname]["fields"].extend(tdata.get("fields", []))
            all_tables[tname]["methods"].extend(tdata.get("methods", []))
            if f["file"] not in all_tables[tname]["files"]:
                all_tables[tname]["files"].append(f["file"])
        all_global_funcs.extend(f.get("global_functions", []))
        all_global_vars.extend(f.get("global_vars", []))
        all_includes.extend(f.get("includes", []))

    all_calls = []
    for f in all_data:
        all_calls.extend(f["calls"])

    return {
        "total_files": len(all_data),
        "total_tables": len(all_tables),
        "tables": {k: dict(v) for k, v in all_tables.items()},
        "global_functions": all_global_funcs,
        "global_variables": all_global_vars,
        "call_graph": all_calls,
        "includes": all_includes,
    }


# ============================================================
# 主入口
# ============================================================
if __name__ == "__main__":
    output_dir = sys.argv[1] if len(sys.argv) > 1 else "."
    cs_path = sys.argv[2] if len(sys.argv) > 2 else None
    lua_path = sys.argv[3] if len(sys.argv) > 3 else None

    os.makedirs(output_dir, exist_ok=True)

    if cs_path and os.path.isdir(cs_path):
        print(f"[C#] Analyzing: {cs_path}")
        cs_data = analyze_csharp_project(cs_path)
        cs_summary = summarize_csharp(cs_data)

        # 精简输出: 每个文件只保留 rel path
        for cls in cs_summary["classes"]:
            cls["_file"] = os.path.relpath(cls["_file"], cs_path)

        cs_json_path = os.path.join(output_dir, "vjbase_csharp.json")
        with open(cs_json_path, "w", encoding="utf-8") as f:
            json.dump(cs_summary, f, indent=2, ensure_ascii=False, default=str)
        print(f"[C#] Written: {cs_json_path}")
        print(f"       {cs_summary['total_files']} files, {cs_summary['total_classes']} classes, {cs_summary['total_methods']} methods")

    if lua_path and os.path.isdir(lua_path):
        print(f"[Lua] Analyzing: {lua_path}")
        lua_data = analyze_lua_project(lua_path)
        lua_summary = summarize_lua(lua_data)

        # 精简输出
        for tname in lua_summary["tables"]:
            lua_summary["tables"][tname]["files"] = [
                os.path.relpath(f, lua_path) for f in lua_summary["tables"][tname]["files"]
            ]

        lua_json_path = os.path.join(output_dir, "vjbase_lua.json")
        with open(lua_json_path, "w", encoding="utf-8") as f:
            json.dump(lua_summary, f, indent=2, ensure_ascii=False, default=str)
        print(f"[Lua] Written: {lua_json_path}")
        print(f"       {lua_summary['total_files']} files, {lua_summary['total_tables']} main tables (ENT, VJ)")
        print(f"       ENT: {len(lua_summary['tables'].get('ENT',{}).get('methods',[]))} methods, {len(lua_summary['tables'].get('ENT',{}).get('fields',[]))} fields")
        print(f"       VJ:  {len(lua_summary['tables'].get('VJ',{}).get('methods',[]))} methods, {len(lua_summary['tables'].get('VJ',{}).get('fields',[]))} fields")
        print(f"       Global functions: {len(lua_summary['global_functions'])}")
        print(f"       Global variables: {len(lua_summary['global_variables'])}")
