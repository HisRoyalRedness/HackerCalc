using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class TokenPrinter : ITokenVisitor<string>
    {
        public enum FixType
        {
            Infix,
            Prefix,
            Postfix
        }

        public TokenPrinter(FixType fixType = FixType.Postfix)
        {
            _fixType = fixType;
        }

        public void Visit<TToken>(TToken token)
            where TToken : IToken
        {
            switch (token.GetType().Name)
            {
                case nameof(OperatorToken):
                    Console.WriteLine();
                    break;
            }
        }

        public string VisitAndAggregate<TToken>(TToken token)
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
                        throw new ApplicationException($"Unhandled token type {token.GetType().Name}");
                    break;
            }
        }

        FixType _fixType = FixType.Postfix;
    }
}
