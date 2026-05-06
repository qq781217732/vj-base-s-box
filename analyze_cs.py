"""
C# 代码结构分析 + 引用关系图生成
使用 tree-sitter AST 提取: 类、方法、属性、字段、方法调用、成员访问
输出: ASCII树、Mermaid图、DOT图
"""
import sys, os, json, re
from collections import defaultdict
from tree_sitter import Parser, Language
import tree_sitter_c_sharp


# ============================================================
# 1. 解析器设置
# ============================================================
CS_LANG = Language(tree_sitter_c_sharp.language())


def parse_file(filepath):
    """解析单个 C# 文件，返回 AST + 源码"""
    with open(filepath, "rb") as f:
        source = f.read()
    parser = Parser(CS_LANG)
    tree = parser.parse(source)
    return tree, source


def text_of(node, source):
    """提取节点对应的源码文本"""
    return source[node.start_byte : node.end_byte].decode()


# ============================================================
# 2. 结构化提取
# ============================================================
def extract_symbols(tree, source, filepath):
    """从 AST 提取所有符号: 类/方法/属性/字段, 以及调用/引用关系"""
    symbols = {
        "file": filepath,
        "classes": [],
        "calls": [],       # (caller_member, called_name, line)
        "member_refs": [],  # (member, target_name, line)
    }
    class_stack = []   # 当前嵌套类
    method_stack = []  # 当前方法

    def walk(node):
        nonlocal class_stack, method_stack

        # ---- 类声明 ----
        if node.type == "class_declaration":
            name_node = node.child_by_field_name("name")
            cls_name = text_of(name_node, source) if name_node else "?"
            base_node = node.child_by_field_name("base_list")
            bases = []
            if base_node:
                for c in base_node.named_children:
                    bases.append(text_of(c, source))
            cls = {
                "name": cls_name,
                "bases": bases,
                "full_name": ".".join([c["name"] for c in class_stack] + [cls_name]),
                "line": node.start_point[0] + 1,
                "methods": [],
                "properties": [],
                "fields": [],
                "nested_classes": [],
            }
            if class_stack:
                class_stack[-1]["nested_classes"].append(cls)
            else:
                symbols["classes"].append(cls)
            class_stack.append(cls)

        # ---- 方法声明 ----
        elif node.type == "method_declaration":
            if class_stack:
                name_node = node.child_by_field_name("name")
                mname = text_of(name_node, source) if name_node else "?"
                params_node = node.child_by_field_name("parameters")
                params = []
                if params_node:
                    for p in params_node.named_children:
                        params.append(text_of(p, source).replace("\n", " "))
                ret_node = node.child_by_field_name("returns")
                returns = text_of(ret_node, source) if ret_node else "void"
                modifiers = []
                for c in node.children:
                    if c.type == "modifier":
                        modifiers.append(text_of(c, source))
                method = {
                    "name": mname,
                    "params": params,
                    "returns": returns,
                    "modifiers": modifiers,
                    "line": node.start_point[0] + 1,
                }
                class_stack[-1]["methods"].append(method)
                method_stack.append(method)

        # ---- 属性声明 ----
        elif node.type == "property_declaration":
            if class_stack:
                name_node = node.child_by_field_name("name")
                pname = text_of(name_node, source) if name_node else "?"
                type_node = node.child_by_field_name("type")
                ptype = text_of(type_node, source) if type_node else "?"
                modifiers = []
                for c in node.children:
                    if c.type == "modifier":
                        modifiers.append(text_of(c, source))
                prop = {
                    "name": pname,
                    "type": ptype,
                    "modifiers": modifiers,
                    "line": node.start_point[0] + 1,
                }
                class_stack[-1]["properties"].append(prop)

        # ---- 字段声明 ----
        elif node.type == "field_declaration":
            if class_stack:
                type_node = node.child_by_field_name("type")
                ftype = text_of(type_node, source) if type_node else "?"
                for child in node.named_children:
                    if child.type == "variable_declarator":
                        fname = text_of(child.child_by_field_name("name"), source) if child.child_by_field_name("name") else "?"
                        modifiers = []
                        for c in node.children:
                            if c.type == "modifier":
                                modifiers.append(text_of(c, source))
                        class_stack[-1]["fields"].append({
                            "name": fname, "type": ftype,
                            "modifiers": modifiers,
                            "line": node.start_point[0] + 1,
                        })

        # ---- 方法调用 ----
        elif node.type == "invocation_expression":
            # 找到 caller context
            caller = method_stack[-1]["name"] if method_stack else "(global)"
            fn_node = node.child_by_field_name("function")
            called = text_of(fn_node, source) if fn_node else "?"
            args_node = node.child_by_field_name("arguments")
            args = []
            if args_node:
                for a in args_node.named_children:
                    args.append(text_of(a, source)[:40])
            symbols["calls"].append({
                "caller": caller,
                "called": called,
                "args": args,
                "line": node.start_point[0] + 1,
            })

        # ---- 成员访问 (obj.Member / obj?.Member) ----
        elif node.type == "member_access_expression":
            expr_node = node.child_by_field_name("expression")
            name_node = node.child_by_field_name("name")
            if expr_node and name_node:
                obj = text_of(expr_node, source)
                member = text_of(name_node, source)
                caller = method_stack[-1]["name"] if method_stack else "(global)"
                symbols["member_refs"].append({
                    "caller": caller,
                    "object": obj,
                    "member": member,
                    "line": node.start_point[0] + 1,
                })

        for child in node.children:
            walk(child)

        # 退出
        if node.type == "class_declaration" and class_stack:
            class_stack.pop()
        if node.type == "method_declaration" and method_stack:
            method_stack.pop()

    walk(tree.root_node)
    return symbols


# ============================================================
# 3. 分析整个项目
# ============================================================
def analyze_project(root_dir, ignore_patterns=None):
    """递归分析项目下所有 .cs 文件"""
    if ignore_patterns is None:
        ignore_patterns = ["/obj/", "/bin/", "/.git/", "/Properties/"]
    all_symbols = []
    for dirpath, dirnames, filenames in os.walk(root_dir):
        dirnames[:] = [d for d in dirnames if not any(p in f"/{d}/" for p in ignore_patterns)]
        for fn in filenames:
            if fn.endswith(".cs"):
                fp = os.path.join(dirpath, fn)
                try:
                    tree, source = parse_file(fp)
                    sym = extract_symbols(tree, source, fp)
                    all_symbols.append(sym)
                except Exception as e:
                    pass  # 跳过解析失败的文件
    return all_symbols


# ============================================================
# 4. 输出: ASCII 树
# ============================================================
def print_tree(all_symbols):
    """在终端打印类/方法/属性/字段树"""
    for sym in all_symbols:
        if not sym["classes"]:
            continue
        rel = os.path.relpath(sym["file"], sym["file"].split("Code")[0] + "Code" if "Code" in sym["file"] else ".")
        print(f"\n{'='*70}")
        print(f"  FILE: {rel}")
        print(f"{'='*70}")

        def print_class(cls, depth=0):
            prefix = "  " * depth
            bases = f" : {', '.join(cls['bases'])}" if cls["bases"] else ""
            print(f"{prefix}[Class] {cls['name']}{bases}  (line {cls['line']})")

            if cls["fields"]:
                print(f"{prefix}  --- Fields ---")
                for f in cls["fields"]:
                    mods = " ".join(f["modifiers"]) + " " if f["modifiers"] else ""
                    print(f"{prefix}    {mods}{f['type']} {f['name']}  (line {f['line']})")

            if cls["properties"]:
                print(f"{prefix}  --- Properties ---")
                for p in cls["properties"]:
                    mods = " ".join(p["modifiers"]) + " " if p["modifiers"] else ""
                    print(f"{prefix}    {mods}{p['type']} {p['name']}  (line {p['line']})")

            if cls["methods"]:
                print(f"{prefix}  --- Methods ---")
                for m in cls["methods"]:
                    mods = " ".join(m["modifiers"]) + " " if m["modifiers"] else ""
                    params = ", ".join(m["params"])
                    print(f"{prefix}    {mods}{m['returns']} {m['name']}({params})  (line {m['line']})")

            for nc in cls.get("nested_classes", []):
                print_class(nc, depth + 1)

        for cls in sym["classes"]:
            print_class(cls)

    # 汇总调用关系
    print(f"\n{'='*70}")
    print(f"  跨类引用关系 (方法调用 + 成员访问)")
    print(f"{'='*70}")

    # 建立 class.method 索引
    all_methods = {}  # method_name -> class_name
    for sym in all_symbols:
        for cls in sym["classes"]:
            for m in cls["methods"]:
                all_methods[m["name"]] = cls["name"]

    call_graph = defaultdict(set)
    member_graph = defaultdict(set)

    for sym in all_symbols:
        for c in sym["calls"]:
            # 过滤系统调用
            target = c["called"].split(".")[-1].split("?.")[-1]
            if target in all_methods and target != c["caller"]:
                call_graph[c["caller"]].add(target)
        for mr in sym["member_refs"]:
            obj = mr["object"]
            mem = mr["member"]
            if obj in ("this", "base"):
                continue
            # 跟踪对已知类实例的成员访问
            member_graph[obj].add(mem)

    if call_graph:
        print("\n方法调用关系:")
        for caller, callees in sorted(call_graph.items()):
            print(f"  {caller}() --> {', '.join(sorted(callees))}()")


# ============================================================
# 5. 输出: Mermaid
# ============================================================
def to_mermaid(all_symbols, top_n=None):
    """生成 Mermaid classDiagram 格式"""
    lines = ["```mermaid", "classDiagram"]

    # 收集所有类
    class_set = {}
    for sym in all_symbols:
        for cls in sym["classes"]:
            full = cls["full_name"]
            if full not in class_set:
                class_set[full] = cls
            else:
                # 合并 (partial class)
                existing = class_set[full]
                existing["methods"].extend(cls["methods"])
                existing["properties"].extend(cls["properties"])
                existing["fields"].extend(cls["fields"])

    # 类定义
    for full_name, cls in sorted(class_set.items()):
        lines.append(f"  class {cls['name']} {{")
        for f in cls["fields"]:
            lines.append(f"    +{f['type']} {f['name']}")
        for p in cls["properties"]:
            lines.append(f"    +{p['type']} {p['name']}")
        for m in cls["methods"]:
            params = ", ".join(m["params"])
            lines.append(f"    +{m['returns']} {m['name']}({params})")
        lines.append("  }")

    # 继承关系
    for full_name, cls in class_set.items():
        for base in cls["bases"]:
            base_name = base.split("<")[0].split(".")[-1]
            if base_name in class_set and base_name != cls["name"]:
                lines.append(f"  {base_name} <|-- {cls['name']}")

    # 调用关系
    all_methods_map = {}
    for full_name, cls in class_set.items():
        for m in cls["methods"]:
            all_methods_map[m["name"]] = cls["name"]

    for sym in all_symbols:
        for c in sym["calls"]:
            target = c["called"].split(".")[-1].split("?.")[-1]
            if target in all_methods_map:
                caller_cls = None
                for cls in sym["classes"]:
                    if any(m["name"] == c["caller"] for m in cls["methods"]):
                        caller_cls = cls["name"]
                        break
                target_cls = all_methods_map[target]
                if caller_cls and caller_cls != target_cls:
                    lines.append(f"  {caller_cls} --> {target_cls} : {c['caller']}()→{target}()")

    lines.append("```")
    return "\n".join(lines)


# ============================================================
# 6. 输出: DOT (Graphviz)
# ============================================================
def to_dot(all_symbols):
    """生成 Graphviz DOT 格式"""
    lines = ["digraph G {",
             '  rankdir=TB;',
             '  node [shape=record, style=filled, fillcolor="#f0f0f0"];',
             '  edge [color="#555555"];']

    class_set = {}
    for sym in all_symbols:
        for cls in sym["classes"]:
            if cls["full_name"] not in class_set:
                class_set[cls["full_name"]] = cls

    for full_name, cls in sorted(class_set.items()):
        label_parts = [f"<b>{cls['name']}</b>"]
        for m in cls["methods"]:
            mod = "S" if "static" in m.get("modifiers", []) else ""
            label_parts.append(f"+ {mod} {m['name']}()"[:50])
        for p in cls["properties"]:
            label_parts.append(f"  {p['name']} : {p['type']}"[:50])
        label = "\\n".join(label_parts)
        cid = cls["name"].replace("<", "_").replace(">", "_")
        lines.append(f'  "{cid}" [label="{label}"];')

    # 继承边
    for full_name, cls in class_set.items():
        for base in cls["bases"]:
            base_name = base.split("<")[0].split(".")[-1]
            if base_name in class_set and base_name != cls["name"]:
                cid = cls["name"].replace("<", "_").replace(">", "_")
                bid = base_name.replace("<", "_").replace(">", "_")
                lines.append(f'  "{bid}" -> "{cid}" [style=dashed, color="#8888cc"];')

    lines.append("}")
    return "\n".join(lines)


# ============================================================
# 7. 主入口
# ============================================================
if __name__ == "__main__":
    import argparse
    ap = argparse.ArgumentParser(description="C# Code Structure Analyzer")
    ap.add_argument("path", nargs="?", default=".", help="Project root path")
    ap.add_argument("--format", "-f", choices=["tree", "mermaid", "dot", "all"], default="tree",
                    help="Output format (default: tree)")
    ap.add_argument("--output", "-o", default=None, help="Output file path")
    ap.add_argument("--top", "-n", type=int, default=0, help="Limit to top N classes by method count")
    args = ap.parse_args()

    print(f"Analyzing: {args.path}")
    all_symbols = analyze_project(args.path)
    total_classes = sum(len(s["classes"]) for s in all_symbols)
    total_methods = sum(
        len(m) for s in all_symbols for c in s["classes"] for m in c["methods"]
    )
    print(f"Found: {len(all_symbols)} files, {total_classes} classes, {total_methods} methods")

    output = ""

    if args.format in ("tree", "all"):
        print_tree(all_symbols)

    if args.format in ("mermaid", "all"):
        mermaid = to_mermaid(all_symbols)
        if args.format != "all":
            print(mermaid)
        output += mermaid + "\n\n"

    if args.format in ("dot", "all"):
        dot = to_dot(all_symbols)
        if args.format != "all":
            print(dot)
        output += dot + "\n"

    if args.output and output:
        with open(args.output, "w", encoding="utf-8") as f:
            f.write(output)
        print(f"\nWritten to: {args.output}")
