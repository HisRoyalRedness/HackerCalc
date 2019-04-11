using System.Linq;

/*
    TokenPrinter Visitor

        Walks the token parse tree and prints out the expression
        in one of three ways, namely prefix, infix and postfix

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class TokenPrinter : ITokenVisitor<string>
    {
        public const FixType DefaultFixType = FixType.Postfix;

        public enum FixType
        {
            Infix,
            Prefix,
            Postfix
        }

        public TokenPrinter(FixType fixType = DefaultFixType)
        {
            _fixType = fixType;
        }

        public string Visit<TToken>(TToken token)
            where TToken : IToken
        {
            switch (token?.Category)
            {
                case null:
#if INCOMPLETE_EQ
                    Builder.Append("<null> ");
#endif
                    return null;

                case TokenCategory.LiteralToken: return PrintLiteral(token as ILiteralToken);
                case TokenCategory.FunctionToken: return PrintFunction(token as IFunctionToken);
                case TokenCategory.OperatorToken: return PrintOperator(token as IOperatorToken);

                default: 
                    throw new UnrecognisedTokenException($"Unrecognised token type {token.GetType().Name}");
            }
        }

        string PrintLiteral(ILiteralToken litToken)
        {
            if (litToken == null)
                return null;

            return litToken.LiteralType == LiteralTokenType.Timespan
                ? $"'{litToken}' "
                : $"{litToken} ";
        }

        string PrintFunction(IFunctionToken funcToken)
        {
            if (funcToken == null)
                return null;

            string parms = null;
            if (_fixType == FixType.Infix)
                parms = string.Join(
                    ", ", 
                    funcToken.Parameters.Select(p => 
                        p.Category == TokenCategory.OperatorToken
                            ? $"( {p.Accept(this)})"
                            : p.Accept(this)));
            else
                parms = string.Join("", funcToken.Parameters.Select(p => p.Accept(this)));

            switch (_fixType)
            {
                case FixType.Prefix: return $"{funcToken.Name}[{funcToken.Parameters.Count}] {parms}";
                case FixType.Infix: return $"{funcToken.Name}( {parms} ) ";
                case FixType.Postfix: return $"{parms}{funcToken.Name}[{funcToken.Parameters.Count}] ";

                default:
                    throw new UnrecognisedTokenException($"Unrecognised fix type {_fixType}");
            }
        }

        string PrintOperator(IOperatorToken opToken)
        {
            if (opToken == null)
                return null;

            var left = opToken.Left?.Accept(this);
            var right = opToken.IsUnary ? null : opToken.Right?.Accept(this);

            switch (opToken.Operator)
            {
                case OperatorType.Grouping:
                    switch (_fixType)
                    {
                        case FixType.Prefix:
                        case FixType.Postfix:
                            return left;

                        case FixType.Infix:
                            return $"( {left}) ";

                        default:
                            throw new UnrecognisedTokenException($"Unrecognised fix type {_fixType}");
                    }

                default:
                    var enumDesc = opToken.Operator.GetEnumDescription();
                    switch (_fixType)
                    {
                        case FixType.Prefix:
                            return $"{enumDesc} {left}{right}";

                        case FixType.Infix:
                            return opToken.IsUnary
                                ? $"{enumDesc} {left}"
                                : $"{left}{enumDesc} {right}";

                        case FixType.Postfix:
                            return $"{left}{right}{enumDesc} ";

                        default:
                            throw new UnrecognisedTokenException($"Unrecognised fix type {_fixType}");
                    }
            }
        }

        readonly FixType _fixType = FixType.Postfix;
    }

    public static class TokenPrinterExtensions
    {
        public static string Print(this IToken token, TokenPrinter.FixType fixType = TokenPrinter.DefaultFixType)
            => token.Accept(new TokenPrinter(fixType));
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
