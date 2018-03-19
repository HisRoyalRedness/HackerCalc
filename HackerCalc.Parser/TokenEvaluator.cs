using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class TokenEvaluator : ITokenVisitor<IToken>
    {
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

        public IToken VisitAndAggregate<TToken>(TToken token)
            where TToken : IToken
        {
            //switch (token.GetType().Name)
            //{
            //    case nameof(OperatorToken):
            //        var opToken = token as OperatorToken;
            //        var tree = new TokenTree
            //        {
            //            Left = VisitAndAggregate(opToken.Left),
            //            Right = VisitAndAggregate(opToken.Right)
            //        };

            //        switch (opToken.Operator)
            //        {
            //            case TokenType.Add: return Add(leftToken, rightToken);
            //            case TokenType.Subtract: return Subtract(leftToken, rightToken);
            //            case TokenType.Multiply: return Subtract(leftToken, rightToken);
            //            case TokenType.Divide: return Subtract(leftToken, rightToken);

            //            default:
            //                throw new ApplicationException($"Unhandled operator type {opToken.Operator}");
            //        }

            //    case nameof(IntegerToken):
            //        return token;

            //    default:
            //        throw new ApplicationException($"Unhandled token type {token.GetType().Name}");
            //}

            return null;
        }

        public class TokenTree
        {
            public TokenTree(ILiteralToken left, ILiteralToken right)
            {
                Left = left;
                Right = right;
            }

            public ILiteralToken Left { get; set; }
            public ILiteralToken Right { get; set; }

            public TokenTree ConvertToCommonType()
            {
                if (Left.DataType == DataType.Float || Right.DataType == DataType.Float)
                    return new TokenTree(Left.CastTo<FloatToken>(), Right.CastTo<FloatToken>());

                return null;
            }
        }

        IToken Add(IToken left, IToken right)
        {
            var leftI = left as IntegerToken;
            var rightI = right as IntegerToken;
            var result = leftI.TypedValue + rightI.TypedValue;
            return new IntegerToken(result.ToString(), result);
        }

        IToken Subtract(IToken left, IToken right)
        {
            var leftI = left as IntegerToken;
            var rightI = right as IntegerToken;
            var result = leftI.TypedValue - rightI.TypedValue;
            return new IntegerToken(result.ToString(), result);
        }

        IToken Multiply(IToken left, IToken right)
        {
            var leftI = left as IntegerToken;
            var rightI = right as IntegerToken;
            var result = leftI.TypedValue * rightI.TypedValue;
            return new IntegerToken(result.ToString(), result);
        }

        IToken Divide(IToken left, IToken right)
        {
            var leftI = left as IntegerToken;
            var rightI = right as IntegerToken;
            var result = leftI.TypedValue / rightI.TypedValue;
            return new IntegerToken(result.ToString(), result);
        }

    }
}
