"""Tree-sitter Lua/C# 代码分析测试 —— 变量/引用/方法提取 + AST 对比"""
from tree_sitter import Parser, Language
import tree_sitter_lua

# ============================================================
# 1. Lua 代码分析
# ============================================================

LUA_CODE = b"""
local Player = {}
Player.Health = 100
Player.Name = "Test"

local function Attack(attacker, target)
    local damage = attacker.Damage or 10
    target.Health = target.Health - damage
    if target.Health <= 0 then
        print(target.Name .. " died!")
        return true
    end
    return false
end

local p1 = { Name = "Hero", Health = 100, Damage = 25 }
local p2 = { Name = "Zombie", Health = 80 }
local killed = Attack(p1, p2)
print(killed)
"""

lang = Language(tree_sitter_lua.language())
parser = Parser(lang)
tree = parser.parse(LUA_CODE)
root = tree.root_node

print("=" * 60)
print("LUA AST 完整树")
print("=" * 60)
print(str(root))
print()

# --- 递归提取结构化信息 ---
print("=" * 60)
print("变量声明 / 赋值")
print("=" * 60)
vars_declared = set()

def extract_info(node, depth=0, source=LUA_CODE):
    """提取变量声明、函数定义、函数调用"""
    indent = "  " * depth
    t = node.type
    text = source[node.start_byte:node.end_byte].decode()

    if t == "function_declaration":
        name_node = node.child_by_field_name("name")
        params_node = node.child_by_field_name("parameters")
        param_texts = []
        if params_node:
            for c in params_node.named_children:
                param_texts.append(source[c.start_byte:c.end_byte].decode())
        print(f"\n[函数] {source[name_node.start_byte:name_node.end_byte].decode()}"
              f"({', '.join(param_texts)})  @ line {node.start_point[0]+1}")

    elif t == "local_variable_declaration":
        # var 名 + 值
        for child in node.named_children:
            if child.type == "variable_declaration":
                for vc in child.named_children:
                    if vc.type == "assignment_statement":
                        var_list = vc.child_by_field_name("variable_list")
                        expr_list = vc.child_by_field_name("expression_list")
                        var_name = ""
                        if var_list:
                            for vn in var_list.named_children:
                                var_name = source[vn.start_byte:vn.end_byte].decode()
                                vars_declared.add(var_name)
                        val_text = ""
                        if expr_list:
                            for en in expr_list.named_children:
                                val_text = source[en.start_byte:en.end_byte].decode()
                        print(f"[变量声明] {var_name} = {val_text}  @ line {node.start_point[0]+1}")

    elif t == "function_call":
        name = ""
        args = []
        for child in node.named_children:
            if child.type == "identifier" and child.prev_named_sibling is None:
                name = source[child.start_byte:child.end_byte].decode()
            elif child.type == "arguments":
                for ac in child.named_children:
                    args.append(source[ac.start_byte:ac.end_byte].decode())
        if name:
            print(f"[函数调用] {name}({', '.join(args)})  @ line {node.start_point[0]+1}")

    elif t == "identifier":
        # 变量引用 —— 不在声明位置
        parent_type = node.parent.type
        id_text = source[node.start_byte:node.end_byte].decode()
        # 简化判断：如果不在变量声明的 variable_list 中，就是引用
        if id_text in vars_declared and parent_type != "variable_list":
            pass  # 懒得太细，下面用遍历

    for child in node.children:
        extract_info(child, depth + 1)

extract_info(root)

# --- 变量引用提取 ---
print()
print("=" * 60)
print("变量引用关系")
print("=" * 60)

def collect_references(node, source=LUA_CODE):
    """收集变量引用（简化版：遍历所有 identifier）"""
    refs = []
    if node.type == "identifier":
        text = source[node.start_byte:node.end_byte].decode()
        parent_type = node.parent.type
        # 排除函数声明中的 name 字段（那是定义）
        grandparent = node.parent
        is_def = False
        if parent_type == "variable_list":
            is_def = True
        elif parent_type == "function_declaration":
            fn = node.parent.child_by_field_name("name")
            if fn and fn.id == node.id:
                is_def = True
        elif parent_type == "parameters":
            is_def = True  # 参数是定义
        elif parent_type == "field" and node.prev_named_sibling is None:
            is_def = True  # table 字段定义

        if not is_def:
            refs.append((text, node.start_point[0] + 1))
    for child in node.children:
        refs.extend(collect_references(child))
    return refs

refs = collect_references(root)
# 去重 + 统计
from collections import Counter
ref_counts = Counter(r[0] for r in refs)
print("变量引用次数:")
for name, count in ref_counts.most_common():
    lines = [str(r[1]) for r in refs if r[0] == name]
    print(f"  {name}: {count} 次 (lines: {', '.join(lines)})")

# ============================================================
# 2. 简单对比演示：两份 Lua 代码的 AST diff
# ============================================================
print()
print("=" * 60)
print("AST 结构对比 (同一文件的两个版本)")
print("=" * 60)

CODE_V2 = b"""
local Player = {}
Player.Health = 80
Player.Name = "Test"

local function Attack(attacker, target)
    local damage = attacker.Damage or 15
    target.Health = target.Health - damage * 2
    if target.Health <= 0 then
        print(target.Name .. " eliminated!")
        return true, target.Name
    end
    return false
end
"""

tree1 = parser.parse(LUA_CODE)
tree2 = parser.parse(CODE_V2)


def count_nodes_by_type(node):
    counts = Counter()
    if node.is_named:
        counts[node.type] += 1
    for child in node.children:
        counts.update(count_nodes_by_type(child))
    return counts

c1 = count_nodes_by_type(tree1.root_node)
c2 = count_nodes_by_type(tree2.root_node)

all_types = sorted(set(c1.keys()) | set(c2.keys()))
print(f"{'NodeType':<30} {'V1':>6} {'V2':>6} {'Diff':>6}")
print("-" * 50)
for t in all_types:
    if c1.get(t) != c2.get(t) or t in ('function_declaration', 'local_variable_declaration', 'binary_expression', 'return_statement'):
        d = c2.get(t, 0) - c1.get(t, 0)
        marker = " <--" if d != 0 else ""
        print(f"{t:<30} {c1.get(t,0):>6} {c2.get(t,0):>6} {d:+>5}{marker}")

print()
print("结论: 可通过 AST 节点统计 + 字段值对比找到真正的语义差异")
