"""
GMod Lua → Standard Lua 预处理器
转换: // → --,  /* */ → --[[ ]],  != → ~=,  && → and,  || → or,  ! → not
"""
import re


def preprocess(code: str) -> str:
    """将 GMod Lua 源码转换为标准 Lua 5.x 语法"""

    # 1. 替换 /* ... */ 为 --[[ ... ]] (先做，避免与 // 冲突)
    code = re.sub(r'/\*', '--[[', code)
    code = re.sub(r'\*/', ']]', code)

    # 2. 替换 // 注释为 -- (跳过 http:// 和 https://)
    # 方法：逐行处理，匹配行中第一个 // 不在字符串中
    lines = code.split('\n')
    out_lines = []
    for line in lines:
        # 简单策略：如果 // 在 "http:" 后面则跳过
        if 'http://' in line or 'https://' in line:
            out_lines.append(line)
        else:
            # 找到第一个 // (不在字符串内)
            # 简化处理：只处理行首或前面是空白/运算符的 //
            out_lines.append(re.sub(r'(?<![:\w])//(.*)$', r'--\1', line))
    code = '\n'.join(out_lines)

    # 3. 替换 != 为 ~=
    code = code.replace('!=', '~=')

    # 4. 替换 && 为 and (确保单词边界)
    code = re.sub(r'(?<![a-zA-Z0-9_])&&(?![a-zA-Z0-9_])', 'and', code)

    # 5. 替换 || 为 or
    code = re.sub(r'(?<![a-zA-Z0-9_|])\|\|(?![a-zA-Z0-9_|])', 'or', code)

    # 6. 替换 ! 为 not (但不在 !=/~= 后，不在字符串中)
    # ! 通常用于 !expr 或 if !... then
    # 简化：!( → not (
    code = re.sub(r'\b!\s*\(', 'not (', code)
    # !identifier → not identifier
    code = re.sub(r'\b!\s*([a-zA-Z_][a-zA-Z0-9_]*)', r'not \1', code)
    # !{ → not {
    code = re.sub(r'\b!\s*\{', 'not {', code)

    return code
