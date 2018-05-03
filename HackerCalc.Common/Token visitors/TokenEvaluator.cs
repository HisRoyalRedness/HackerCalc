using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HisRoyalRedness.com
{
    // key: The actual types of the two operands.
    // value: The types the operands should be cast to before performing the operation.
    using BinaryOperandTypeMap = Dictionary<TokenEvaluator.OperandTypePair, TokenEvaluator.OperandTypePair>;

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

        static TokenEvaluator()
        {
            // Create a readonly static list of all possible type pairs
            _allPossibleTypePairs = new Lazy<ReadOnlyCollection<OperandTypePair>>(() =>
            {
                var typeList = new List<OperandTypePair>();
                var dataTypes = EnumExtensions.GetEnumCollection<TokenDataType>().ToList();
                foreach (var left in dataTypes)
                    foreach (var right in dataTypes)
                        typeList.Add(new OperandTypePair(left, right));
                return new ReadOnlyCollection<OperandTypePair>(typeList);
            });

            _operatorProperties = new ReadOnlyDictionary<OperatorType, OperatorProperties>(new[]
            {
                new OperatorProperties(OperatorType.Add, "add", "to", _addMapping, Add),
                new OperatorProperties(OperatorType.Subtract, "subtract", "from", _subtractMapping, Subtract),
                new OperatorProperties(OperatorType.Multiply, "multiply", "by", _multiplyMapping, Multiply),
                new OperatorProperties(OperatorType.Divide, "divide", "by", _divideMapping, Divide),
                new OperatorProperties(OperatorType.LeftShift, "left shift", "by", _leftShiftMapping, LeftShift),
                new OperatorProperties(OperatorType.RightShift, "right shift", "by", _rightShiftMapping, RightShift),
            }.ToDictionary(op => op.Operator));
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

                        case OperatorType.NumericNegate:
                            return litToken.NumericNegate();

                        case OperatorType.BitwiseNegate:
                            return litToken.BitwiseNegate();

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
                    // If the expression is only partially entered, try calculate the value 
                    // up to the last completed portion of the expression
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

                    if (!_operatorProperties.ContainsKey(opToken.Operator))
                        throw new ApplicationException($"{nameof(TokenEvaluator)} currently doesn't support the {opToken.Operator} operator. Has it been added to the {nameof(OperatorProperties)} dictionary?");

                    var prop = _operatorProperties[opToken.Operator];
                    if (prop.NAry != 2)
                        throw new ApplicationException($"Operator type {opToken.Operator} is not a binary operator.");

                    return BinaryOperate(prop, pair, _settings);
                }
            }
            else
                throw new ApplicationException($"Unhandled token type {token.GetType().Name}");
        }

        IToken BinaryOperate(OperatorProperties opProp, TokenPair pair, EvaluatorSettings settings)
        {
            var opTypes = pair.TypesFromMap(opProp.TypeMap);
            var castPair = pair.CastFromMap(opProp.TypeMap, settings);
            if (castPair == null)
                throw new InvalidCalcOperationException($"Can't {opProp.Description} a {pair.Left.DataType} ({pair.Left.ObjectValue}) {opProp.OperandConjuctDesciption} a {pair.Right.DataType} ({pair.Right.ObjectValue})");
            var token = opProp.Operation(castPair);
            if (token == null)
                throw new InvalidCalcOperationException($"Can't {opProp.Description} a {castPair.Left.DataType} ({castPair.Left.ObjectValue}) {opProp.OperandConjuctDesciption} a {castPair.Right.DataType} ({castPair.Right.ObjectValue})");
            return token;
        }

        #region Add
        static IToken Add(TokenPair pair)
        {
            if (pair != null)
                switch (pair.Left.DataType)
                {
                    case TokenDataType.Date:
                        if (pair.Right.DataType == TokenDataType.Timespan)
                            return ((DateToken)pair.Left) + ((TimespanToken)pair.Right);
                        break;
                    case TokenDataType.Float:
                        if (pair.Right.DataType == TokenDataType.Float)
                            return ((FloatToken)pair.Left) + ((FloatToken)pair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (pair.Right.DataType == TokenDataType.Integer)
                            return ((IntegerToken)pair.Left) + ((IntegerToken)pair.Right);
                        break;
                    case TokenDataType.Time:
                        if (pair.Right.DataType == TokenDataType.Timespan)
                            return ((TimeToken)pair.Left) + ((TimespanToken)pair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (pair.Right.DataType)
                        {
                            case TokenDataType.Date:
                                return ((DateToken)pair.Right) + ((TimespanToken)pair.Left);
                            case TokenDataType.Time:
                                return ((TimespanToken)pair.Left) + ((TimeToken)pair.Right);
                            case TokenDataType.Timespan:
                                return ((TimespanToken)pair.Left) + ((TimespanToken)pair.Right);
                        }
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((UnlimitedIntegerToken)pair.Left) + ((UnlimitedIntegerToken)pair.Right);
                        break;
                }
            return null;
        }

        static readonly BinaryOperandTypeMap _addMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),                           new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),                          new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),                           new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),                          new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),                        new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),                           new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),                       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.UnlimitedInteger),               new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),                           new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),                       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Integer),               new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        };
        #endregion Add

        #region Subtract
        static IToken Subtract(TokenPair pair)
        {
            if (pair != null)
            {
                switch (pair.Left.DataType)
                {
                    case TokenDataType.Date:
                        switch (pair.Right.DataType)
                        {
                            case TokenDataType.Timespan:
                                return ((DateToken)pair.Left) - ((TimespanToken)pair.Right);
                            case TokenDataType.Date:
                                return ((DateToken)pair.Left) - ((DateToken)pair.Right);
                        }
                        break;
                    case TokenDataType.Float:
                        if (pair.Right.DataType == TokenDataType.Float)
                            return ((FloatToken)pair.Left) - ((FloatToken)pair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (pair.Right.DataType == TokenDataType.Integer)
                            return ((IntegerToken)pair.Left) - ((IntegerToken)pair.Right);
                        break;
                    case TokenDataType.Time:
                        if (pair.Right.DataType == TokenDataType.Timespan)
                            return ((TimeToken)pair.Left) - ((TimespanToken)pair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (pair.Right.DataType)
                        {
                            case TokenDataType.Date:
                                return ((DateToken)pair.Right) - ((TimespanToken)pair.Left);
                            case TokenDataType.Timespan:
                                return ((TimespanToken)pair.Left) - ((TimespanToken)pair.Right);
                        }
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((UnlimitedIntegerToken)pair.Left) - ((UnlimitedIntegerToken)pair.Right);
                        break;
                }
            }
            return null;
        }

        static readonly BinaryOperandTypeMap _subtractMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              new OperandTypePair(TokenDataType.Date, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),                           new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),                          new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),                           new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),                          new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),                        new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),                           new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),                       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.UnlimitedInteger),               new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),                           new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),                       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Integer),               new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        };
        #endregion Subtract

        #region Multiply
        static IToken Multiply(TokenPair pair)
        {
            if (pair != null)
            {
                switch (pair.Left.DataType)
                {
                    case TokenDataType.Float:
                        switch (pair.Right.DataType)
                        {
                            case TokenDataType.Float:
                                return ((FloatToken)pair.Left) * ((FloatToken)pair.Right);
                            case TokenDataType.Timespan:
                                return ((FloatToken)pair.Left) * ((TimespanToken)pair.Right);
                        }
                        break;
                    case TokenDataType.Integer:
                        if (pair.Right.DataType == TokenDataType.Integer)
                            return ((IntegerToken)pair.Left) * ((IntegerToken)pair.Right);
                        break;
                    case TokenDataType.Timespan:
                        if (pair.Right.DataType == TokenDataType.Float)
                            return ((TimespanToken)pair.Left) * ((FloatToken)pair.Right);
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((UnlimitedIntegerToken)pair.Left) * ((UnlimitedIntegerToken)pair.Right);
                        break;
                }
            }
            return null;
        }

        static readonly BinaryOperandTypeMap _multiplyMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),                          new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),                          new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),                        new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),                       new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.UnlimitedInteger),               new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),                       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Integer),               new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        };
        #endregion Multiply

        #region Divide
        static IToken Divide(TokenPair pair)
        {
            if (pair != null)
            {
                switch (pair.Left.DataType)
                {
                    case TokenDataType.Float:
                        if (pair.Right.DataType == TokenDataType.Float)
                            return ((FloatToken)pair.Left) / ((FloatToken)pair.Right);
                        break;
                    case TokenDataType.Integer:
                        if (pair.Right.DataType == TokenDataType.Integer)
                            return ((IntegerToken)pair.Left) / ((IntegerToken)pair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (pair.Right.DataType)
                        {
                            case TokenDataType.Float:
                                return ((TimespanToken)pair.Left) / ((FloatToken)pair.Right);
                            case TokenDataType.Timespan:
                                return ((TimespanToken)pair.Left) / ((TimespanToken)pair.Right);
                        }
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((UnlimitedIntegerToken)pair.Left) / ((UnlimitedIntegerToken)pair.Right);
                        break;
                }
            }
            return null;
        }

        static readonly BinaryOperandTypeMap _divideMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),                          new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),                          new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),                        new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),                       new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.UnlimitedInteger),               new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),                       new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Integer),               new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        };
        #endregion Divide

        #region LeftShift
        static IToken LeftShift(TokenPair pair)
        {
            if (pair != null)
            {

            }
            return null;
        }

        static readonly BinaryOperandTypeMap _leftShiftMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),                        OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),                       OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),                       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),   OperandTypePair.Unsupported },
        };
        #endregion LeftShift

        #region RightShift
        static IToken RightShift(TokenPair pair)
        {
            if (pair != null)
            {

            }
            return null;
        }

        static readonly BinaryOperandTypeMap _rightShiftMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Integer),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Integer),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Date),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Float),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Integer),                        OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Time),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Integer, TokenDataType.Timespan),                       OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Integer),                           OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Integer),                       OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),   OperandTypePair.Unsupported },
        };
        #endregion RightShift

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

            public OperandTypePair TypesFromMap(BinaryOperandTypeMap map)
            {
                var key = new OperandTypePair(Left.DataType, Right.DataType);
                return map.ContainsKey(key)
                    ? map[key]
                    : OperandTypePair.Unsupported;
            }

            public TokenPair CastFromMap(BinaryOperandTypeMap map, EvaluatorSettings settings)
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
                        return new TokenPair(left.Cast(left.IsSigned, bitWidth, settings.ErrorOnOverflow), right.Cast(right.IsSigned, bitWidth, settings.ErrorOnOverflow));

                    // else, use the greater bitwidth and convert to unsigned
                    return new TokenPair(left.Cast(false, bitWidth, settings.ErrorOnOverflow), right.Cast(false, bitWidth, settings.ErrorOnOverflow));
                }

                return new TokenPair(Left.CastTo(newPairType.Left), Right.CastTo(newPairType.Right));
            }
        }
        #endregion TokenPair

        #region OperandTypePair
        [DebuggerDisplay("{Left}, {Right}")]
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

        #region OperatorProperties
        public class OperatorProperties
        {
            public OperatorProperties(OperatorType opType, string description, string conjunctDescription, BinaryOperandTypeMap typeMap, Func<TokenPair, IToken> operation)
            {
                Operator = opType;
                Description = description;
                NAry = 2;
                TypeMap = typeMap;
                Operation = operation;
                OperandConjuctDesciption = conjunctDescription;
            }
            
            public OperatorType Operator { get; private set; }
            public string Description { get; private set; }
            public string OperandConjuctDesciption { get; private set; }
            public int NAry { get; private set; }
            public BinaryOperandTypeMap TypeMap { get; private set; }
            public Func<TokenPair, IToken> Operation { get; private set; }
        }
        #endregion OperatorProperties

        readonly static Lazy<ReadOnlyCollection<OperandTypePair>> _allPossibleTypePairs;
        readonly static ReadOnlyDictionary<OperatorType, OperatorProperties> _operatorProperties;
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
