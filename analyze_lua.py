"""
Lua 代码结构分析 (GMod 风格 ENT/SWEP)
使用 tree-sitter AST 提取: 实体类型、属性、方法、函数调用
输出: 与 C# 分析一致的格式
"""
import sys, os, re
from collections import defaultdict
from tree_sitter import Parser, Language
import tree_sitter_lua


LUA_LANG = Language(tree_sitter_lua.language())


def preprocess_gmod_lua(code: str) -> str:
    """将 GMod Lua 语法转换为标准 Lua 5.x: // -> --,  /* */ -> --[=[ ]=],  != -> ~=,  && -> and,  || -> or,  ! -> not"""
    # 1. 块注释 /* ... */ -> --[=[ ... ]=]
    code = re.sub(r'/\*', '--[=[', code)
    code = re.sub(r'\*/', ']=]', code)

    # 2. 行注释 // -> -- (跳过 http://, https://)
    def replace_line_comment(line):
        if 'http://' in line or 'https://' in line:
            return line
        return re.sub(r'(?<![:\w])//(.*)$', r'--\1', line)
    code = '\n'.join(replace_line_comment(l) for l in code.split('\n'))

    # 3. != -> ~=
    code = code.replace('!=', '~=')

    # 4. && -> and
    code = re.sub(r'(?<![a-zA-Z0-9_])&&(?![a-zA-Z0-9_])', 'and', code)

    # 5. || -> or
    code = re.sub(r'(?<![a-zA-Z0-9_|])\|\|(?![a-zA-Z0-9_|])', 'or', code)

    # 6. ! -> not
    code = re.sub(r'\b!\s*\(', 'not (', code)
    code = re.sub(r'\b!\s*([a-zA-Z_][a-zA-Z0-9_]*)', r'not \1', code)
    code = re.sub(r'\b!\s*\{', 'not {', code)

    return code


def parse_file(filepath):
    with open(filepath, "r", encoding="utf-8", errors="replace") as f:
        raw = f.read()
    clean = preprocess_gmod_lua(raw)
    source = clean.encode("utf-8")
    parser = Parser(LUA_LANG)
    tree = parser.parse(source)
    return tree, source


def text_of(node, source):
    return source[node.start_byte : node.end_byte].decode()


# 支持的实体前缀
ENTITY_PREFIXES = {"ENT", "SWEP", "EFFECT", "GM", "TOOL", "WEAPON",
                    "NPC", "PANEL", "VGUI", "Derma", "HUD", "CL", "SENT",
                    "VJ", "hook", "net", "timer"}


def extract_symbols(tree, source, filepath):
    """从 Lua AST 提取 GMod 实体结构"""
    symbols = {
        "file": filepath,
        "entities": [],    # 相当于 C# 的 classes
        "calls": [],
        "includes": [],
        "local_functions": [],
        "global_vars": [],
    }

    entity_map = {}  # prefix -> {name, properties, methods, line}
    current_entity = None  # (prefix, entity_name)

    def walk(node):
        nonlocal current_entity

        # ---- 属性赋值: ENT.X = value ----
        if node.type == "assignment_statement":
            # tree-sitter-lua 的 assignment_statement 子节点没有 field name
            # 需按 type 查找 variable_list / expression_list
            var_list = None
            expr_list = None
            for child in node.children:
                if child.type == "variable_list":
                    var_list = child
                elif child.type == "expression_list":
                    expr_list = child

            if var_list and expr_list:
                for var_node in var_list.children:
                    if not var_node.is_named:
                        continue
                    # ENT.xxx = value
                    if var_node.type == "dot_index_expression":
                        table_node = var_node.child_by_field_name("table")
                        field_node = var_node.child_by_field_name("field")
                        if table_node and field_node:
                            prefix = text_of(table_node, source)
                            field = text_of(field_node, source)
                            if prefix in ENTITY_PREFIXES or prefix.startswith("VJ."):
                                # 确保实体存在
                                if prefix not in entity_map:
                                    entity_map[prefix] = {
                                        "name": prefix,
                                        "full_name": prefix,
                                        "line": node.start_point[0] + 1,
                                        "properties": [],
                                        "methods": [],
                                    }
                                # 获取值文本
                                val_text = ""
                                for en in expr_list.named_children:
                                    val_text = text_of(en, source)
                                    break
                                entity_map[prefix]["properties"].append({
                                    "name": field,
                                    "value": val_text[:80],
                                    "line": node.start_point[0] + 1,
                                })

                    # local xxx = ... (全局作用域)
                    elif var_node.type == "identifier":
                        name = text_of(var_node, source)
                        val = ""
                        for en in expr_list.named_children:
                            val = text_of(en, source)
                            break
                        symbols["global_vars"].append({
                            "name": name,
                            "value": val[:80],
                            "line": node.start_point[0] + 1,
                        })

        # ---- 方法定义: function ENT:MethodName(params) ----
        elif node.type == "function_declaration":
            name_node = node.child_by_field_name("name")
            params_node = node.child_by_field_name("parameters")
            body_node = node.child_by_field_name("body")

            if name_node:
                name_text = text_of(name_node, source)

                # GMod 风格: function ENT:MethodName()  /  function ENT.MethodName()
                # tree-sitter-lua 中 method 解析为 dot_index 或 method_index
                if name_node.type == "method_index_expression":
                    table_node = name_node.child_by_field_name("table")
                    method_node = name_node.child_by_field_name("method")
                    if table_node and method_node:
                        prefix = text_of(table_node, source)
                        method_name = text_of(method_node, source)

                        params = []
                        if params_node:
                            for p in params_node.named_children:
                                params.append(text_of(p, source))

                        if prefix in ENTITY_PREFIXES or prefix.startswith("VJ."):
                            if prefix not in entity_map:
                                entity_map[prefix] = {
                                    "name": prefix,
                                    "full_name": prefix,
                                    "line": node.start_point[0] + 1,
                                    "properties": [],
                                    "methods": [],
                                }
                            entity_map[prefix]["methods"].append({
                                "name": f":{method_name}",  # : 表示 method call
                                "method_name": method_name,
                                "params": params,
                                "line": node.start_point[0] + 1,
                            })

                # 普通函数: function name(params)
                elif name_node.type == "identifier":
                    params = []
                    if params_node:
                        for p in params_node.named_children:
                            params.append(text_of(p, source))
                    symbols["local_functions"].append({
                        "name": name_text,
                        "params": params,
                        "line": node.start_point[0] + 1,
                    })

                # ENT.MethodName 风格 (dot_index)
                elif name_node.type == "dot_index_expression":
                    table_node = name_node.child_by_field_name("table")
                    field_node = name_node.child_by_field_name("field")
                    if table_node and field_node:
                        prefix = text_of(table_node, source)
                        method_name = text_of(field_node, source)
                        params = []
                        if params_node:
                            for p in params_node.named_children:
                                params.append(text_of(p, source))
                        if prefix in ENTITY_PREFIXES:
                            if prefix not in entity_map:
                                entity_map[prefix] = {
                                    "name": prefix, "full_name": prefix,
                                    "line": node.start_point[0] + 1,
                                    "properties": [], "methods": [],
                                }
                            entity_map[prefix]["methods"].append({
                                "name": f".{method_name}",
                                "method_name": method_name,
                                "params": params,
                                "line": node.start_point[0] + 1,
                            })

        # ---- 局部函数: local function name(params) ----
        elif node.type == "local_function_declaration":
            name_node = node.child_by_field_name("name")
            params_node = node.child_by_field_name("parameters")
            if name_node:
                name = text_of(name_node, source)
                params = []
                if params_node:
                    for p in params_node.named_children:
                        params.append(text_of(p, source))
                symbols["local_functions"].append({
                    "name": name,
                    "params": params,
                    "line": node.start_point[0] + 1,
                })

        # ---- 函数调用 ----
        elif node.type == "function_call":
            fn_node = node.child_by_field_name("name")
            args_node = node.child_by_field_name("arguments")
            called = text_of(fn_node, source) if fn_node else "?"
            args = []
            if args_node:
                for a in args_node.named_children:
                    args.append(text_of(a, source)[:50])
            symbols["calls"].append({
                "called": called,
                "args": args,
                "line": node.start_point[0] + 1,
            })

        # ---- include("...") ----
        if node.type == "function_call":
            fn_node = node.child_by_field_name("name")
            if fn_node:
                called = text_of(fn_node, source)
                if called == "include" or called == "AddCSLuaFile":
                    args_node = node.child_by_field_name("arguments")
                    if args_node:
                        for a in args_node.named_children:
                            path = text_of(a, source).strip('"').strip("'")
                            symbols["includes"].append({
                                "type": called,
                                "path": path,
                                "line": node.start_point[0] + 1,
                            })

        # ---- self:method() 调用 (方法内) ----
        if node.type == "function_call":
            fn_node = node.child_by_field_name("name")
            if fn_node and fn_node.type == "method_index_expression":
                table_node = fn_node.child_by_field_name("table")
                method_node = fn_node.child_by_field_name("method")
                if table_node and method_node:
                    obj = text_of(table_node, source)
                    method = text_of(method_node, source)
                    if obj == "self":
                        symbols["calls"].append({
                            "called": f"self:{method}",
                            "args": [],
                            "line": node.start_point[0] + 1,
                            "self_call": True,
                        })

        for child in node.children:
            walk(child)

    walk(tree.root_node)

    # 把 entity_map 转到 symbols["entities"]
    symbols["entities"] = list(entity_map.values())
    return symbols


def analyze_project(root_dir):
    all_symbols = []
    for dirpath, dirnames, filenames in os.walk(root_dir):
        for fn in filenames:
            if fn.endswith(".lua"):
                fp = os.path.join(dirpath, fn)
                try:
                    tree, source = parse_file(fp)
                    sym = extract_symbols(tree, source, fp)
                    all_symbols.append(sym)
                except Exception as e:
                    pass
    return all_symbols


def print_tree(all_symbols):
    """与 C# 输出格式一致的终端输出"""
    for sym in all_symbols:
        rel = os.path.relpath(sym["file"], sym["file"].split("lua")[0] + "lua" if "lua" in sym["file"] else ".")
        header = False

        if sym["entities"]:
            if not header:
                print(f"\n{'='*70}")
                print(f"  FILE: {rel}")
                print(f"{'='*70}")
                header = True

            for ent in sym["entities"]:
                print(f"[Entity] {ent['name']}  (line {ent['line']})")

                if ent["properties"]:
                    print(f"  --- Properties ---")
                    for p in ent["properties"]:
                        val = f" = {p['value']}" if p.get("value") else ""
                        print(f"    {p['name']}{val}  (line {p['line']})")

                if ent["methods"]:
                    print(f"  --- Methods ---")
                    for m in ent["methods"]:
                        params = ", ".join(m["params"])
                        print(f"    {ent['name']}{m['name']}({params})  (line {m['line']})")

        if sym["local_functions"]:
            if not header:
                print(f"\n{'='*70}")
                print(f"  FILE: {rel}")
                print(f"{'='*70}")
                header = True
            for lf in sym["local_functions"]:
                params = ", ".join(lf["params"])
                print(f"  [LocalFn] {lf['name']}({params})  (line {lf['line']})")

        if sym["includes"]:
            if not header:
                print(f"\n{'='*70}")
                print(f"  FILE: {rel}")
                print(f"{'='*70}")
                header = True
            for inc in sym["includes"]:
                print(f"  [Include] {inc['type']}({inc['path']})  (line {inc['line']})")

    # ---- 汇总 ----
    print(f"\n{'='*70}")
    print(f"  方法调用关系")
    print(f"{'='*70}")

    # 建立方法索引
    all_methods = set()
    for sym in all_symbols:
        for ent in sym["entities"]:
            for m in ent["methods"]:
                all_methods.add(m["method_name"])
        for lf in sym["local_functions"]:
            all_methods.add(lf["name"])

    call_graph = defaultdict(set)
    self_calls = defaultdict(set)

    for sym in all_symbols:
        ent_name = sym["entities"][0]["name"] if sym["entities"] else "(none)"
        for c in sym["calls"]:
            target = c["called"]
            # self:method() 调用
            if c.get("self_call"):
                self_calls[ent_name].add(target.replace("self:", ""))
            # 普通调用
            if target in all_methods:
                call_graph[ent_name].add(target)

    if self_calls:
        print("\nSelf 方法调用:")
        for ent, methods in sorted(self_calls.items()):
            print(f"  {ent} --> self:{', self:'.join(sorted(methods))}")

    if call_graph:
        print("\n跨实体/跨函数调用:")
        for caller, callees in sorted(call_graph.items()):
            print(f"  {caller} --> {', '.join(sorted(callees))}()")


if __name__ == "__main__":
    import argparse
    ap = argparse.ArgumentParser(description="Lua Code Structure Analyzer (GMod)")
    ap.add_argument("path", nargs="?", default=".", help="Project root path")
    ap.add_argument("--output", "-o", default=None, help="Output file path")
    args = ap.parse_args()

    print(f"Analyzing (Lua): {args.path}")
    all_symbols = analyze_project(args.path)
    total_ents = sum(len(s["entities"]) for s in all_symbols)
    total_methods = sum(
        len(m) for s in all_symbols for e in s["entities"] for m in e["methods"]
    )
    total_props = sum(
        len(p) for s in all_symbols for e in s["entities"] for p in e["properties"]
    )
    print(f"Found: {len(all_symbols)} files, {total_ents} entities, {total_props} properties, {total_methods} methods")

    if args.output:
        with open(args.output, "w", encoding="utf-8") as f:
            import io
            old_stdout = sys.stdout
            sys.stdout = f
            print_tree(all_symbols)
            sys.stdout = old_stdout
        print(f"Written to: {args.output}")
    else:
        print_tree(all_symbols)
