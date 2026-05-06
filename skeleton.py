"""
VJ-Base 代码骨架提取 —— 紧凑 diffable 文本格式
每行一条签名，适合直接 diff 对比 C# vs Lua
"""
import sys, os
from collections import Counter
from tree_sitter import Parser, Language
import tree_sitter_c_sharp, tree_sitter_lua

CS = Language(tree_sitter_c_sharp.language())
LUA = Language(tree_sitter_lua.language())

def T(node, src):
    return src[node.start_byte:node.end_byte].decode()


# ============================================================
# C# 骨架
# ============================================================
def skeleton_csharp(filepath, rel_root):
    with open(filepath, "rb") as f:
        src = f.read()
    tree = Parser(CS).parse(src)
    lines = []
    cls_stack = []

    def walk(node):
        if node.type == "class_declaration":
            name = T(node.child_by_field_name("name"), src) if node.child_by_field_name("name") else "?"
            base = node.child_by_field_name("base_list")
            bases = T(base, src) if base else ""
            lines.append(f"\n>>> {os.path.relpath(filepath, rel_root)}")
            lines.append(f"CLASS {name}{' : ' + bases if bases else ''}")
            cls_stack.append(name)

        elif node.type in ("method_declaration", "constructor_declaration") and cls_stack:
            name = T(node.child_by_field_name("name"), src) if node.child_by_field_name("name") else ".ctor"
            params = node.child_by_field_name("parameters")
            plist = f"({T(params, src)})" if params else "()"
            ret = node.child_by_field_name("returns")
            returns = T(ret, src) if ret else "void"
            mods = " ".join(T(c, src) for c in node.children if c.type == "modifier")
            lines.append(f"  METH {mods} {returns} {name}{plist}")

        elif node.type == "property_declaration" and cls_stack:
            name = T(node.child_by_field_name("name"), src) if node.child_by_field_name("name") else "?"
            ptype = T(node.child_by_field_name("type"), src) if node.child_by_field_name("type") else "?"
            mods = " ".join(T(c, src) for c in node.children if c.type == "modifier")
            lines.append(f"  PROP {mods} {ptype} {name}")

        elif node.type == "field_declaration" and cls_stack:
            ftype = T(node.child_by_field_name("type"), src) if node.child_by_field_name("type") else "?"
            mods = " ".join(T(c, src) for c in node.children if c.type == "modifier")
            for child in node.named_children:
                if child.type == "variable_declarator":
                    fn = child.child_by_field_name("name")
                    fname = T(fn, src) if fn else "?"
                    lines.append(f"  FIELD {mods} {ftype} {fname}")

        elif node.type == "invocation_expression" and cls_stack:
            fn = node.child_by_field_name("function")
            if fn:
                called = T(fn, src)
                # 只记录有意义的调用（过滤掉太短的/系统调用）
                if len(called) > 2 and not called.startswith("base."):
                    lines.append(f"  CALL {called}()")

        for child in node.children:
            walk(child)

        if node.type == "class_declaration" and cls_stack:
            cls_stack.pop()

    walk(tree.root_node)
    return [l for l in lines if l.strip()]


# ============================================================
# Lua 骨架
# ============================================================
def _child_type(node, t):
    for c in node.children:
        if c.type == t:
            return c
    return None

def _nameds_of_type(node, t):
    return [c for c in node.named_children if c.type == t]

def skeleton_lua(filepath, rel_root):
    with open(filepath, "rb") as f:
        src = f.read()
    tree = Parser(LUA).parse(src)
    lines = [f"\n>>> {os.path.relpath(filepath, rel_root)}"]
    inside_ent = False
    function_depth = 0
    seen_includes = set()  # 去重

    def walk(node):
        nonlocal inside_ent, function_depth

        # 表字段赋值: ENT.Xxx = value (仅顶层)
        if node.type == "assignment_statement" and function_depth == 0:
            vl = _child_type(node, "variable_list")
            el = _child_type(node, "expression_list")
            if vl and el:
                dot = _child_type(vl, "dot_index_expression")
                if dot:
                    idents = _nameds_of_type(dot, "identifier")
                    if len(idents) >= 2:
                        tbl, fname = T(idents[0], src), T(idents[1], src)
                        val_node = el.named_children[0] if el.named_children else None
                        val = T(val_node, src) if val_node else "?"
                        if tbl in ("ENT", "VJ"):
                            if not inside_ent:
                                lines.append(f"TABLE {tbl}")
                                inside_ent = True
                            # 截断多行值
                            val = val.replace("\n", " ")[:100]
                            lines.append(f"  FIELD {fname} = {val}")
                else:
                    ident = _child_type(vl, "identifier")
                    if ident and el.named_children:
                        vname = T(ident, src)
                        val = T(el.named_children[0], src).replace("\n", " ")[:100]
                        # 过滤噪音：不是纯标识符、太短、或者条件表达式残留
                        if vname.isidentifier() and len(vname) > 1 and "=" not in vname:
                            lines.append(f"VAR {vname} = {val}")

        # 函数定义
        elif node.type == "function_declaration":
            function_depth += 1
            params_node = _child_type(node, "parameters")
            params = ",".join(T(p, src) for p in (params_node.named_children if params_node else []))
            # 找函数名
            name_node = None
            for c in node.named_children:
                if c.type in ("dot_index_expression", "method_index_expression", "identifier"):
                    if c.prev_sibling and c.prev_sibling.type == "function":
                        name_node = c
                        break
            if name_node:
                fname = T(name_node, src)
                if ":" in fname:
                    tbl, method = fname.split(":", 1)
                    if tbl == "ENT" or tbl == "self":
                        lines.append(f"  METH {method}({params})")
                    else:
                        lines.append(f"FUNC {fname}({params})")
                elif "." in fname:
                    tbl, method = fname.rsplit(".", 1)
                    if tbl in ("ENT", "VJ"):
                        lines.append(f"  METH {method}({params})")
                    else:
                        lines.append(f"FUNC {fname}({params})")
                else:
                    lines.append(f"FUNC {fname}({params})")

        # include / require (仅顶层)
        elif node.type == "function_call" and function_depth == 0:
            ident = _child_type(node, "identifier")
            if ident:
                fn_name = T(ident, src)
                if fn_name in ("include", "AddCSLuaFile", "require"):
                    args = _child_type(node, "arguments")
                    if args and args.named_children:
                        arg_text = T(args.named_children[0], src).strip('"').strip("'")
                        if arg_text.endswith(".lua") and arg_text not in seen_includes:
                            seen_includes.add(arg_text)
                            lines.append(f"INCLUDE {arg_text}")

        for child in node.children:
            walk(child)

        if node.type == "function_declaration":
            function_depth -= 1

    walk(tree.root_node)
    return [l for l in lines if l.strip() and l.strip() != f">>> {os.path.relpath(filepath, rel_root)}"]


# ============================================================
# 入口
# ============================================================
if __name__ == "__main__":
    output_dir = sys.argv[1]
    cs_path = sys.argv[2] if len(sys.argv) > 2 else None
    lua_path = sys.argv[3] if len(sys.argv) > 3 else None

    if cs_path:
        print(f"[C#] {cs_path}")
        with open(os.path.join(output_dir, "cs_skeleton.txt"), "w", encoding="utf-8") as out:
            for dirpath, dirnames, filenames in os.walk(cs_path):
                dirnames[:] = [d for d in dirnames if d not in ("obj","bin",".git","Properties")]
                for fn in sorted(filenames):
                    if fn.endswith(".cs"):
                        fp = os.path.join(dirpath, fn)
                        try:
                            sk = skeleton_csharp(fp, cs_path)
                            out.write("\n".join(sk) + "\n")
                        except Exception as e:
                            pass
        print(f"  -> {os.path.join(output_dir, 'cs_skeleton.txt')}")

    if lua_path:
        print(f"[Lua] {lua_path}")
        with open(os.path.join(output_dir, "lua_skeleton.txt"), "w", encoding="utf-8") as out:
            for dirpath, dirnames, filenames in os.walk(lua_path):
                for fn in sorted(filenames):
                    if fn.endswith(".lua"):
                        fp = os.path.join(dirpath, fn)
                        try:
                            sk = skeleton_lua(fp, lua_path)
                            out.write("\n".join(sk) + "\n")
                        except Exception as e:
                            pass
        print(f"  -> {os.path.join(output_dir, 'lua_skeleton.txt')}")
