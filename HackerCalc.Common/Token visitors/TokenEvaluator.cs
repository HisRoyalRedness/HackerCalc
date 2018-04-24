using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    // key: The actual types of the two operands.
    // value: The types the operands should be cast to before performing the operation.
    using OperandTypeMap = Dictionary<TokenEvaluator.OperandTypePair, TokenEvaluator.OperandTypePair>;

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

            public OperandTypePair TypesFromMap(OperandTypeMap map)
            {
                var key = new OperandTypePair(Left.DataType, Right.DataType);
                return map.ContainsKey(key)
                    ? map[key]
                    : OperandTypePair.Unsupported;
            }

            public TokenPair CastFromMap(OperandTypeMap map, EvaluatorSettings settings)
            {
                var newPairType = TypesFromMap(map);
                if (!newPairType.OperationSupported)
                    return null;

                // If both types are integers, we need to take bitwidth and sign into account
                if (newPairType.Left == TokenDataType.Integer && newPairType.Right == TokenDataType.Integer)
                {
                    // Loosely based on the C++ operator arithmetic operations
                    // http://en.cppreference.com/w/cpp/language/operator_arithmetic

                    var left = Left.CastTo(newPairType.Left) as IntegerToken;
                    var right = Right.CastTo(newPairType.Right) as IntegerToken;

                    // Nothing more to do if both are unbound
                    if (left.IsUnbound && right.IsUnbound)
                        return new TokenPair(left, right);

                    // Get the greater of the 2 bitwidths
                    var bitWidth = left.BitWidth >= right.BitWidth ? left.BitWidth : right.BitWidth;

                    // If both are signed or both unsigned, just use the greater of the bitwidths
                    if (!(left.IsSigned ^ right.IsSigned))
                        return new TokenPair(left.Cast(left.IsSigned, bitWidth), right.Cast(right.IsSigned, bitWidth));

                    // else, use the greater bitwidth and convert to unsigned
                    return new TokenPair(left.Cast(false, bitWidth), right.Cast(false, bitWidth));
                }

                return new TokenPair(Left.CastTo(newPairType.Left), Right.CastTo(newPairType.Right));
            }
        }
#endregion TokenPair

#region Add
        IToken Add(TokenPair pair)
        {
            var opTypes = pair.TypesFromMap(_addMapping);
            var castPair = pair.CastFromMap(_addMapping, _settings);
            if (castPair != null)
            {
                switch (opTypes.Left)
                {
                    case TokenDataType.Date:
                        if (opTypes.Right == TokenDataType.Timespan)
                            return ((DateToken)castPair.Left) + ((TimespanToken)castPair.Right);
                        break;
                    case TokenDataType.Float:
                        if (opTypes.Right == TokenDataType.Float)
                            return ((FloatToken)castPair.Left) + ((FloatToken)castPair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (opTypes.Right == TokenDataType.Integer)
                            return ((IntegerToken)castPair.Left) + ((IntegerToken)castPair.Right);
                        break;
                    case TokenDataType.Time:
                        if (opTypes.Right == TokenDataType.Timespan)
                            return ((TimeToken)castPair.Left) + ((TimespanToken)castPair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (opTypes.Right)
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

        static readonly OperandTypeMap _addMapping = new OperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),         new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),       new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),          new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),      new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),        new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),      new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),     new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),      new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),    new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),   new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),         new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),       new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),          new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),      new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),     new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),   new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) }
        };
#endregion Add

#region Subtract
        IToken Subtract(TokenPair pair)
        {
            var opTypes = pair.TypesFromMap(_subtractMapping);
            var castPair = pair.CastFromMap(_subtractMapping, _settings);
            if (castPair != null)
            {
                switch (opTypes.Left)
                {
                    case TokenDataType.Date:
                        switch(opTypes.Right)
                        {
                            case TokenDataType.Timespan:
                                return ((DateToken)castPair.Left) - ((TimespanToken)castPair.Right);
                            case TokenDataType.Date:
                                return ((DateToken)castPair.Left) - ((DateToken)castPair.Right);
                        }
                        break;
                    case TokenDataType.Float:
                        if (opTypes.Right == TokenDataType.Float)
                            return ((FloatToken)castPair.Left) - ((FloatToken)castPair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (opTypes.Right == TokenDataType.Integer)
                            return ((IntegerToken)castPair.Left) - ((IntegerToken)castPair.Right);
                        break;
                    case TokenDataType.Time:
                        if (opTypes.Right == TokenDataType.Timespan)
                            return ((TimeToken)castPair.Left) - ((TimespanToken)castPair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (opTypes.Right)
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

        static readonly OperandTypeMap _subtractMapping = new OperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),          new OperandTypePair(TokenDataType.Date, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),         new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),       new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),          new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),      new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),        new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),      new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),     new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),      new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),    new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),   new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),         new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),       new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),          new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),      new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),     new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),   new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) }
        };
#endregion Subtract

#region Multiply
        IToken Multiply(TokenPair pair)
        {
            var opTypes = pair.TypesFromMap(_multiplyMapping);
            var castPair = pair.CastFromMap(_multiplyMapping, _settings);
            if (castPair != null)
            {
                switch (opTypes.Left)
                {
                    case TokenDataType.Float:
                        switch (opTypes.Right)
                        {
                            case TokenDataType.Float:
                                return ((FloatToken)castPair.Left) * ((FloatToken)castPair.Right);
                            case TokenDataType.Timespan:
                                return ((FloatToken)castPair.Left) * ((TimespanToken)castPair.Right);
                        }
                        break;
                    case TokenDataType.Integer:
                        if (opTypes.Right == TokenDataType.Integer)
                            return ((IntegerToken)castPair.Left) * ((IntegerToken)castPair.Right);
                        break;
                    case TokenDataType.Timespan:
                        if (opTypes.Right == TokenDataType.Float)
                            return ((TimespanToken)castPair.Left) * ((FloatToken)castPair.Right);
                        break;
                }
            }
            throw new InvalidCalcOperationException($"Can't multiply a {pair.Left.DataType} ({pair.Left.ObjectValue}) with a {pair.Right.DataType} ({pair.Right.ObjectValue})");
        }

        static readonly OperandTypeMap _multiplyMapping = new OperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),      OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),        new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),      new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),     new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),      new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),    new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),   new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),      OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),     new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),   new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),  OperandTypePair.Unsupported }
        };
#endregion Multiply

#region Divide
        IToken Divide(TokenPair pair)
        {
            var opTypes = pair.TypesFromMap(_divideMapping);
            var castPair = pair.CastFromMap(_divideMapping, _settings);
            if (castPair != null)
            {
                switch (opTypes.Left)
                {
                    case TokenDataType.Float:
                        if (opTypes.Right == TokenDataType.Float)
                            return ((FloatToken)castPair.Left) / ((FloatToken)castPair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (opTypes.Right == TokenDataType.Integer)
                            return ((IntegerToken)castPair.Left) / ((IntegerToken)castPair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (opTypes.Right)
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

        static readonly OperandTypeMap _divideMapping = new OperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),      OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),        new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),      new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),     OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),      new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),    new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),   new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),      OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),     new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),   new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) }
        };
        #endregion Divide

        #region OperandTypePair
        public struct OperandTypePair
        {
            public OperandTypePair(TokenDataType left, TokenDataType right)
                : this(left, right, true)
            { }

            private OperandTypePair(TokenDataType left, TokenDataType right, bool supported)
            {
                Left = left;
                Right = right;
                OperationSupported = supported;
            }

            public TokenDataType Left { get; private set; }
            public TokenDataType Right { get; private set; }
            public bool OperationSupported { get; private set; }

            public static OperandTypePair Unsupported => _unsupportedPair;
            static readonly OperandTypePair _unsupportedPair = new OperandTypePair() { OperationSupported = false };
        }
        #endregion OperandTypePair

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
