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
            switch (token.GetType().Name)
            {
                case nameof(OperatorToken):
                    var opToken = token as OperatorToken;
                    var pair = new TokenPair(
                        VisitAndAggregate(opToken.Left) as ILiteralToken,
                        VisitAndAggregate(opToken.Right) as ILiteralToken);

                    switch (opToken.Operator)
                    {
                        case TokenType.Add: return Add(pair);
                        case TokenType.Subtract: return Subtract(pair);
                        case TokenType.Multiply: return Subtract(pair);
                        case TokenType.Divide: return Subtract(pair);

                        default:
                            throw new ApplicationException($"Unhandled operator type {opToken.Operator}");
                    }

                case nameof(IntegerToken):
                    return token;

                default:
                    throw new ApplicationException($"Unhandled token type {token.GetType().Name}");
            }
        }

        public class TokenPair
        {
            public TokenPair(ILiteralToken left, ILiteralToken right)
            {
                if (left == null)
                    throw new ArgumentNullException(nameof(left));
                if (right == null)
                    throw new ArgumentNullException(nameof(right));
                Left = left;
                Right = right;
            }

            public ILiteralToken Left { get; set; }
            public ILiteralToken Right { get; set; }

            public TokenPair ConvertToCommonType()
            {
                if (Left.DataType == DataType.Float && Right.DataType == DataType.Float)
                    return this;

                if (Left.DataType == DataType.Float && Right.DataType == DataType.Integer)
                    return new TokenPair(Left, Right.CastTo<FloatToken>());

                if (Left.DataType == DataType.Integer && Right.DataType == DataType.Float)
                    return new TokenPair(Left.CastTo<FloatToken>(), Right);

                if (Left.DataType == DataType.Integer && Right.DataType == DataType.Integer)
                {
                    var leftI = Left as IntegerToken;
                    var rightI = Right as IntegerToken;

                    if (leftI.IsSigned == rightI.IsSigned)
                    {

                    }

                }


                return null;
            }
        }

        IToken Add(TokenPair pair)
        {
            pair = pair.ConvertToCommonType();
            //var leftI = left as IntegerToken;
            //var rightI = right as IntegerToken;
            //var result = leftI.TypedValue + rightI.TypedValue;
            //return new IntegerToken(result.ToString(), result);
            return null;
        }

        IToken Subtract(TokenPair pair)
        {
            pair = pair.ConvertToCommonType();
            //var leftI = left as IntegerToken;
            //var rightI = right as IntegerToken;
            //var result = leftI.TypedValue - rightI.TypedValue;
            //return new IntegerToken(result.ToString(), result);
            return null;
        }

        IToken Multiply(TokenPair pair)
        {
            pair = pair.ConvertToCommonType();
            //var leftI = left as IntegerToken;
            //var rightI = right as IntegerToken;
            //var result = leftI.TypedValue * rightI.TypedValue;
            //return new IntegerToken(result.ToString(), result);
            return null;
        }

        IToken Divide(TokenPair pair)
        {
            pair = pair.ConvertToCommonType();
            //var leftI = left as IntegerToken;
            //var rightI = right as IntegerToken;
            //var result = leftI.TypedValue / rightI.TypedValue;
            //return new IntegerToken(result.ToString(), result);
            return null;
        }

    }
}
