﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    using TypeTuple = Tuple<TokenDataType, TokenDataType>;
    using TypeMap = Dictionary<Tuple<TokenDataType, TokenDataType>, Tuple<TokenDataType, TokenDataType>>;

    public interface ILiteralTokenEval : ILiteralToken
    {
        // Returns true if we have an incomplete equation, and this token
        // is the last token that we can evaluate
        bool IsTermToken { get; set; }
    }


    public class TokenEvaluator : ITokenVisitor<IToken>
    {
        public TokenEvaluator(EvaluatorSettings settings)
        {
            _settings = settings;
        }

        public IToken Visit<TToken>(TToken token)
            where TToken : IToken
        {
#if INCOMPLETE_EQ
            if (token == null)
                return null;
#endif

            if (token is ILiteralToken)
                return token;

            else if (token is IOperatorToken)
            {
                var opToken = token as OperatorToken;

                if (opToken.IsUnary)
                {
                    var litToken = Visit(opToken.Left) as ILiteralToken;

                    switch (opToken.Operator)
                    {
                        case OperatorType.Cast:
                            {
                                var castOp = opToken as CastOperatorToken;
                                var castToken = litToken.CastTo(castOp.CastToType);
                                return castOp.CastToType == TokenDataType.Integer
                                    ? (castToken as IntegerToken).Cast(castOp.IsSigned, castOp.BitWidth)
                                    : castToken;
                            }

                        default:
                            throw new ApplicationException($"Unhandled unary operator type {opToken.Operator}");
                    }
                }
                else
                {
                    var pair = new TokenPair(
                        Visit(opToken.Left) as ILiteralToken,
                        Visit(opToken.Right) as ILiteralToken);

#if INCOMPLETE_EQ
                    if (pair.Left as ILiteralTokenEval != null)
                    {
                        if ((pair.Left as ILiteralTokenEval).IsTermToken)
                            return pair.Left;

                        if ((pair.Right as ILiteralTokenEval)?.IsTermToken ?? false)
                            return pair.Right;

                        if (pair.Right == null)
                            return pair.Left.Tap(t => ((ILiteralTokenEval)t).IsTermToken = true);
                    }
#endif

                    switch (opToken.Operator)
                    {
                        case OperatorType.Add: return Add(pair);
                        case OperatorType.Subtract: return Subtract(pair);
                        case OperatorType.Multiply: return Multiply(pair);
                        case OperatorType.Divide: return Divide(pair);

                        default:
                            throw new ApplicationException($"Unhandled operator type {opToken.Operator}");
                    }
                }
            }
            else
                throw new ApplicationException($"Unhandled token type {token.GetType().Name}");
        }

        #region TokenPair
        public class TokenPair
        {
            public TokenPair(ILiteralToken left, ILiteralToken right)
            {
                if (left == null)
                    throw new ArgumentNullException(nameof(left));
#if INCOMPLETE_EQ
#else
                if (right == null)
                    throw new ArgumentNullException(nameof(right));
#endif
                Left = left;
                Right = right;
            }

            public ILiteralToken Left { get; set; }
            public ILiteralToken Right { get; set; }

            public TypeTuple TypesFromMap(TypeMap map)
            {
                var key = new TypeTuple(Left.DataType, Right.DataType);
                return map.ContainsKey(key)
                    ? map[key]
                    : null;
            }

            public TokenPair CastFromMap(TypeMap map)
            {
                var newPairType = TypesFromMap(map);
                if (newPairType == null)
                    return null;
                return new TokenPair(Left.CastTo(newPairType.Item1), Right.CastTo(newPairType.Item2));
            }
        }
#endregion TokenPair

#region Add
        static IToken Add(TokenPair pair)
        {
            var opTypes = pair.TypesFromMap(_addMapping);
            var castPair = pair.CastFromMap(_addMapping);
            if (opTypes != null && castPair != null)
            {
                switch (opTypes.Item1)
                {
                    case TokenDataType.Date:
                        if (opTypes.Item2 == TokenDataType.Timespan)
                            return ((DateToken)castPair.Left) + ((TimespanToken)castPair.Right);
                        break;
                    case TokenDataType.Float:
                        if (opTypes.Item2 == TokenDataType.Float)
                            return ((FloatToken)castPair.Left) + ((FloatToken)castPair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (opTypes.Item2 == TokenDataType.Integer)
                            return ((IntegerToken)castPair.Left) + ((IntegerToken)castPair.Right);
                        break;
                    case TokenDataType.Time:
                        if (opTypes.Item2 == TokenDataType.Timespan)
                            return ((TimeToken)castPair.Left) + ((TimespanToken)castPair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (opTypes.Item2)
                        {
                            case TokenDataType.Date:
                                return ((DateToken)castPair.Right) + ((TimespanToken)castPair.Left);
                            case TokenDataType.Time:
                                return ((TimespanToken)castPair.Left) + ((TimeToken)castPair.Right);
                            case TokenDataType.Timespan:
                                return ((TimespanToken)castPair.Left) + ((TimespanToken)castPair.Right);
                        }
                        break;
                }
            }
            throw new InvalidCalcOperationException($"Can't add a {pair.Left.DataType} ({pair.Left.ObjectValue}) to a {pair.Right.DataType} ({pair.Right.ObjectValue})");
        }

        static readonly TypeMap _addMapping = new TypeMap()
        {
            { new TypeTuple(TokenDataType.Date, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Float),  new TypeTuple(TokenDataType.Date, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Integer),  new TypeTuple(TokenDataType.Date, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Time),  new TypeTuple(TokenDataType.Date, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Date, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Float, TokenDataType.Date),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Float),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Integer),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Time),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Time) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Integer, TokenDataType.Date),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Float),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Integer),  new TypeTuple(TokenDataType.Integer, TokenDataType.Integer) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Time),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Time) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Time, TokenDataType.Date),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Float),  new TypeTuple(TokenDataType.Time, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Integer),  new TypeTuple(TokenDataType.Time, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Time),  new TypeTuple(TokenDataType.Time, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Time, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Date),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Float),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Integer),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Time),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Time) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) }
        };
#endregion Add

#region Subtract
        static IToken Subtract(TokenPair pair)
        {
            var opTypes = pair.TypesFromMap(_subtractMapping);
            var castPair = pair.CastFromMap(_subtractMapping);
            if (opTypes != null && castPair != null)
            {
                switch (opTypes.Item1)
                {
                    case TokenDataType.Date:
                        switch(opTypes.Item2)
                        {
                            case TokenDataType.Timespan:
                                return ((DateToken)castPair.Left) - ((TimespanToken)castPair.Right);
                            case TokenDataType.Date:
                                return ((DateToken)castPair.Left) - ((DateToken)castPair.Right);
                        }
                        break;
                    case TokenDataType.Float:
                        if (opTypes.Item2 == TokenDataType.Float)
                            return ((FloatToken)castPair.Left) - ((FloatToken)castPair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (opTypes.Item2 == TokenDataType.Integer)
                            return ((IntegerToken)castPair.Left) - ((IntegerToken)castPair.Right);
                        break;
                    case TokenDataType.Time:
                        if (opTypes.Item2 == TokenDataType.Timespan)
                            return ((TimeToken)castPair.Left) - ((TimespanToken)castPair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (opTypes.Item2)
                        {
                            case TokenDataType.Date:
                                return ((DateToken)castPair.Right) - ((TimespanToken)castPair.Left);
                            case TokenDataType.Timespan:
                                return ((TimespanToken)castPair.Left) - ((TimespanToken)castPair.Right);
                        }
                        break;
                }
            }
            throw new InvalidCalcOperationException($"Can't subtract a {pair.Left.DataType} ({pair.Left.ObjectValue}) from a {pair.Right.DataType} ({pair.Right.ObjectValue})");
        }

        static readonly TypeMap _subtractMapping = new TypeMap()
        {
            { new TypeTuple(TokenDataType.Date, TokenDataType.Date),  new TypeTuple(TokenDataType.Date, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Float),  new TypeTuple(TokenDataType.Date, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Integer),  new TypeTuple(TokenDataType.Date, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Time),  new TypeTuple(TokenDataType.Date, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Date, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Float, TokenDataType.Date),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Float),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Integer),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Time),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Time) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Integer, TokenDataType.Date),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Float),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Integer),  new TypeTuple(TokenDataType.Integer, TokenDataType.Integer) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Time),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Time) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Time, TokenDataType.Date),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Float),  new TypeTuple(TokenDataType.Time, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Integer),  new TypeTuple(TokenDataType.Time, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Time),  new TypeTuple(TokenDataType.Time, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Time, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Date),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Date) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Float),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Integer),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Time),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Time) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) }
        };
#endregion Subtract

#region Multiply
        static IToken Multiply(TokenPair pair)
        {
            var opTypes = pair.TypesFromMap(_multiplyMapping);
            var castPair = pair.CastFromMap(_multiplyMapping);
            if (opTypes != null && castPair != null)
            {
                switch (opTypes.Item1)
                {
                    case TokenDataType.Float:
                        switch (opTypes.Item2)
                        {
                            case TokenDataType.Float:
                                return ((FloatToken)castPair.Left) * ((FloatToken)castPair.Right);
                            case TokenDataType.Timespan:
                                return ((FloatToken)castPair.Left) * ((TimespanToken)castPair.Right);
                        }
                        break;
                    case TokenDataType.Integer:
                        if (opTypes.Item2 == TokenDataType.Integer)
                            return ((IntegerToken)castPair.Left) * ((IntegerToken)castPair.Right);
                        break;
                    case TokenDataType.Timespan:
                        if (opTypes.Item2 == TokenDataType.Float)
                            return ((TimespanToken)castPair.Left) * ((FloatToken)castPair.Right);
                        break;
                }
            }
            throw new InvalidCalcOperationException($"Can't multiply a {pair.Left.DataType} ({pair.Left.ObjectValue}) with a {pair.Right.DataType} ({pair.Right.ObjectValue})");
        }

        static readonly TypeMap _multiplyMapping = new TypeMap()
        {
            { new TypeTuple(TokenDataType.Date, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Float),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Integer),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Timespan),  null },

            { new TypeTuple(TokenDataType.Float, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Float),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Integer),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Float, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Integer, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Float),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Integer),  new TypeTuple(TokenDataType.Integer, TokenDataType.Integer) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Float, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Time, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Float),  null },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Integer),  null },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Timespan),  null },

            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Float),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Integer),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan),  null }
        };
#endregion Multiply

#region Divide
        static IToken Divide(TokenPair pair)
        {
            var opTypes = pair.TypesFromMap(_divideMapping);
            var castPair = pair.CastFromMap(_divideMapping);
            if (opTypes != null && castPair != null)
            {
                switch (opTypes.Item1)
                {
                    case TokenDataType.Float:
                        if (opTypes.Item2 == TokenDataType.Float)
                            return ((FloatToken)castPair.Left) / ((FloatToken)castPair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (opTypes.Item2 == TokenDataType.Integer)
                            return ((IntegerToken)castPair.Left) / ((IntegerToken)castPair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (opTypes.Item2)
                        {
                            case TokenDataType.Float:
                                return ((TimespanToken)castPair.Left) / ((FloatToken)castPair.Right);
                            case TokenDataType.Timespan:
                                return ((TimespanToken)castPair.Left) / ((TimespanToken)castPair.Right);
                        }
                        break;
                }
            }
            throw new InvalidCalcOperationException($"Can't divide a {pair.Left.DataType} ({pair.Left.ObjectValue}) by a {pair.Right.DataType} ({pair.Right.ObjectValue})");
        }

        static readonly TypeMap _divideMapping = new TypeMap()
        {
            { new TypeTuple(TokenDataType.Date, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Float),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Integer),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Date, TokenDataType.Timespan),  null },

            { new TypeTuple(TokenDataType.Float, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Float),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Integer),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Float, TokenDataType.Timespan),  null },

            { new TypeTuple(TokenDataType.Integer, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Float),  new TypeTuple(TokenDataType.Float, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Integer),  new TypeTuple(TokenDataType.Integer, TokenDataType.Integer) },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Integer, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Float, TokenDataType.Timespan) },

            { new TypeTuple(TokenDataType.Time, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Float),  null },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Integer),  null },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Time, TokenDataType.Timespan),  null },

            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Date),  null },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Float),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Integer),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Float) },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Time),  null },
            { new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan),  new TypeTuple(TokenDataType.Timespan, TokenDataType.Timespan) }
        };
#endregion Divide

        readonly EvaluatorSettings _settings;
    }

    public static class TokenEvaluatorExtensions
    {
        public static IToken Evaluate(this IToken token, EvaluatorSettings settings = null)
            => token.Accept(new TokenEvaluator(settings ?? new EvaluatorSettings()));
    }

    public class EvaluatorSettings
    {
        public bool ErrorOnOverflow { get; set; }
    }
}
