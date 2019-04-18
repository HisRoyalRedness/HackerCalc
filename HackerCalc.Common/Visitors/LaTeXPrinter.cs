using System.Diagnostics;
using System.Linq;

/*
    LaTeXPrinter Visitor

        Walks the token parse tree and prints out the expression
        formatted as a LaTeX string, for presentation by WpfMath

    Keith Fletcher
    Apr 2019

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class LaTeXPrinter : ITokenVisitor<string>
    {
        public string Visit<TToken>(TToken token)
            where TToken : IToken
        {
            switch (token?.Category)
            {
                case null:
                    return null;

                case TokenCategory.LiteralToken: return $"{{{PrintLiteral(token as ILiteralToken)}}}";
                case TokenCategory.FunctionToken: return $"{{{PrintFunction(token as IFunctionToken)}}}";
                case TokenCategory.OperatorToken: return $"{{{PrintOperator(token as IOperatorToken)}}}";

                default:
                    throw new UnrecognisedTokenException($"Unrecognised token type {token.GetType().Name}");
            }
        }

        string PrintLiteral(ILiteralToken litToken)
        {
            if (litToken == null)
                return null;

            return litToken.LiteralType == LiteralTokenType.Timespan
                ? $"'{litToken}'"
                : $"{litToken}";
        }

        string PrintFunction(IFunctionToken funcToken)
        {
            if (funcToken == null)
                return null;

            string parms = null;
            parms = string.Join(
                ",",
                funcToken.Parameters.Select(p =>
                    p.Category == TokenCategory.OperatorToken
                        ? $"\\left({p.Accept(this)}\\right)"
                        : p.Accept(this)));

            return $"{funcToken.Name}\\left({parms}\\right)";
        }

        string PrintOperator(IOperatorToken opToken)
        {
            if (opToken == null)
                return null;

            var left = opToken.Left?.Accept(this);
            var right = opToken.IsUnary ? null : opToken.Right?.Accept(this);
            var rightIsNull = right == null;
            if (left == null)
                left = "{}";
            if (right == null)
                right = "{}";

            switch (opToken.Operator)
            {
                case OperatorType.Grouping: return $"{{\\left({left}\\right)}}";

                case OperatorType.Add:  return $"{left}+{right}";
                case OperatorType.Subtract: return $"{left}-{right}";
                case OperatorType.Multiply: return $"{left}\\times{right}";
                case OperatorType.Divide: return $"\\frac{left}{right}";
                case OperatorType.Power: return $"{left}^{right}";
                case OperatorType.Root:
                    return rightIsNull
                        ? $"\\sqrt{left}"
                        : $"\\sqrt[{right}]{left}";
                case OperatorType.Modulo: return $"{left}\\text{{ mod }}{right}";
                case OperatorType.LeftShift: return $"{left}\\ll{right}";
                case OperatorType.RightShift: return $"{left}\\gg{right}";
                case OperatorType.And: return $"{left}\\land{right}";
                case OperatorType.Or: return $"{left}\\lor{right}";
                case OperatorType.Xor: return $"{left}\\oplus{right}";

                default:
                    return opToken.IsUnary
                        ? $"?{left}"
                        : $"{left}?{right}";
            }
        }
    }

    public static class LaTeXPrinterExtensions
    {
        public static string ToLaTeX(this IToken token)
        {
            var expr = token.Accept(new LaTeXPrinter());
            Debug.WriteLine(expr);
            return expr;
        }
    }
}

/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
*/
