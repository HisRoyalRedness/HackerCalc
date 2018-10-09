using System;
using System.Collections.Generic;
using System.Text;

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
            StringBuilder sb = new StringBuilder();
            InternalVisitAndAggregate(sb, token);
            return sb.ToString();
        }

        static bool IsLiteralToken<TToken>(TToken token) => IsTokenOfType<TToken, ILiteralToken>(token);
        static bool IsFunctionToken<TToken>(TToken token) => IsTokenOfType<TToken, IFunctionToken>(token);
        static bool IsOperatorToken<TToken>(TToken token) => IsTokenOfType<TToken, IOperatorToken>(token);
        static bool IsTokenOfType<TToken, TTestType>(TToken token) => typeof(TTestType).IsAssignableFrom(token.GetType());

        void InternalVisitAndAggregate<TToken>(StringBuilder sb, TToken token)
        {
#if INCOMPLETE_EQ
            if (token == null)
            {
                sb.Append("<null> ");
                return;
            }
#endif

            if (IsLiteralToken(token))
                sb.Append($"{((ILiteralToken)token)} ");

            else if (IsFunctionToken(token))
            {
                var funcToken = token as IFunctionToken;
                switch (_fixType)
                {
                    case FixType.Prefix:
                        sb.Append($"{funcToken.Name}[{funcToken.Parameters.Count}] ");
                        foreach (var p in funcToken.Parameters)
                            InternalVisitAndAggregate(sb, p);
                        break;

                    case FixType.Infix:
                        sb.Append($"{funcToken.Name}( ");
                        var first = true;
                        foreach (var p in funcToken.Parameters)
                        {
                            if (!first)
                                sb.Append(", ");
                            if (IsLiteralToken(p) || IsFunctionToken(p))
                                InternalVisitAndAggregate(sb, p);
                            else
                            {
                                sb.Append("( ");
                                InternalVisitAndAggregate(sb, p);
                                sb.Append(") ");
                            }
                            first = false;
                        }
                        sb.Append(") ");
                        break;

                    case FixType.Postfix:
                        foreach (var p in funcToken.Parameters)
                            InternalVisitAndAggregate(sb, p);
                        sb.Append($"{funcToken.Name}[{funcToken.Parameters.Count}] ");
                        break;

                    default:
                        throw new UnrecognisedTokenException($"Unrecognised fix type {_fixType}");
                }
            }

            else if (IsOperatorToken(token))
            {
                var opToken = token as IOperatorToken;
                switch (opToken.Operator)
                {
                    case OperatorType.Grouping:
                        switch (_fixType)
                        {
                            case FixType.Prefix:
                                InternalVisitAndAggregate(sb, opToken.Left);
                                break;

                            case FixType.Infix:
                                sb.Append("( ");
                                InternalVisitAndAggregate(sb, opToken.Left);
                                sb.Append(") ");
                                break;

                            case FixType.Postfix:
                                InternalVisitAndAggregate(sb, opToken.Left);
                                break;

                            default:
                                throw new UnrecognisedTokenException($"Unrecognised fix type {_fixType}");
                        }
                        break;

                    default:
                        switch (_fixType)
                        {
                            case FixType.Prefix:
                                sb.Append($"{opToken.Operator.GetEnumDescription()} ");
                                InternalVisitAndAggregate(sb, opToken.Left);
                                if (!opToken.IsUnary)
                                    InternalVisitAndAggregate(sb, opToken.Right);
                                break;

                            case FixType.Infix:
                                if (opToken.IsUnary)
                                    sb.Append($"{opToken.Operator.GetEnumDescription()} ");
                                InternalVisitAndAggregate(sb, opToken.Left);
                                if (!opToken.IsUnary)
                                {
                                    sb.Append($"{opToken.Operator.GetEnumDescription()} ");
                                    InternalVisitAndAggregate(sb, opToken.Right);
                                }
                                break;

                            case FixType.Postfix:
                                InternalVisitAndAggregate(sb, opToken.Left);
                                if (!opToken.IsUnary)
                                    InternalVisitAndAggregate(sb, opToken.Right);
                                sb.Append($"{opToken.Operator.GetEnumDescription()} ");
                                break;

                            default:
                                throw new UnrecognisedTokenException($"Unrecognised fix type {_fixType}");
                        }
                        break;
                        //default:
                        //    throw new UnrecognisedTokenException($"Unrecognised operator token type {opToken.Operator}");
                }
            }

            else
                throw new UnrecognisedTokenException($"Unrecognised token type {token.GetType().Name}");
        }

        readonly FixType _fixType = FixType.Postfix;
    }

    public static class TokenPrinterExtensions
    {
        public static string Print(this IToken token, TokenPrinter.FixType fixType = TokenPrinter.DefaultFixType)
            => token.Accept(new TokenPrinter(fixType));
    }
}
