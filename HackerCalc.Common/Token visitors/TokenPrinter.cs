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

        void InternalVisitAndAggregate<TToken>(StringBuilder sb, TToken token)
        {
            switch (token.GetType().Name)
            {
                case nameof(OperatorToken):
                    var opToken = token as OperatorToken;
                    switch (_fixType)
                    {
                        case FixType.Prefix:
                            sb.Append($"{opToken.Operator.GetEnumDescription()} ");
                            InternalVisitAndAggregate(sb, opToken.Left);
                            if (!opToken.IsUnary)
                                InternalVisitAndAggregate(sb, opToken.Right);
                            break;

                        case FixType.Infix:
                            sb.Append("( ");
                            if (opToken.IsUnary)
                                sb.Append($"{opToken.Operator.GetEnumDescription()} ");
                            InternalVisitAndAggregate(sb, opToken.Left);
                            if (!opToken.IsUnary)
                            {
                                sb.Append($"{opToken.Operator.GetEnumDescription()} ");
                                InternalVisitAndAggregate(sb, opToken.Right);
                            }
                            sb.Append(") ");
                            break;

                        case FixType.Postfix:
                            InternalVisitAndAggregate(sb, opToken.Left);
                            if (!opToken.IsUnary)
                                InternalVisitAndAggregate(sb, opToken.Right);
                            sb.Append($"{opToken.Operator.GetEnumDescription()} ");
                            break;
                    }
                    break;

                default:
                    if (typeof(ILiteralToken).IsAssignableFrom(token.GetType()))
                        sb.Append($"{((ILiteralToken)token).ObjectValue} ");

                    else
                        throw new UnrecognisedTokenException($"Unrecognised token type {token.GetType().Name}");
                    break;
            }
        }

        FixType _fixType = FixType.Postfix;
    }

    public static class TokenPrinterExtensions
    {
        public static string Print(this IToken token, TokenPrinter.FixType fixType = TokenPrinter.DefaultFixType)
            => token.Accept(new TokenPrinter(fixType));
    }
}
