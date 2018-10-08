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
    using BinaryOperandResultTypeMap = Dictionary<TokenEvaluator.OperandTypePair, TokenDataType>;

    public interface ILiteralTokenEval : IOldLiteralToken
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
            // Create a readonly static list of all possible types
            _allPossibleTypes = new Lazy<ReadOnlyCollection<TokenDataType>>(() 
                => new ReadOnlyCollection<TokenDataType>(EnumExtensions.GetEnumCollection<TokenDataType>().ToList()));

            // Create a readonly static list of all possible type pairs
            _allPossibleTypePairs = new Lazy<ReadOnlyCollection<OperandTypePair>>(() =>
            {
                var typeList = new List<OperandTypePair>();
                foreach (var left in _allPossibleTypes.Value)
                    foreach (var right in _allPossibleTypes.Value)
                        typeList.Add(new OperandTypePair(left, right));
                return new ReadOnlyCollection<OperandTypePair>(typeList);
            });

            _operatorProperties = new ReadOnlyDictionary<OperatorType, OperatorProperties>(new[]
            {
                new OperatorProperties(OperatorType.Add, _addMapping, _addResultMapping, Add, AddErrorMessage),
                new OperatorProperties(OperatorType.Subtract, _subtractMapping, _subtractResultMapping, Subtract, SubtractErrorMessage),
                new OperatorProperties(OperatorType.Multiply, _multiplyMapping, _multiplyResultMapping, Multiply, MultiplyErrorMessage),
                new OperatorProperties(OperatorType.Divide, _divideMapping, _divideResultMapping, Divide, DivideErrorMessage),
                new OperatorProperties(OperatorType.LeftShift, _leftShiftMapping, null, LeftShift, LeftShiftErrorMessage),
                new OperatorProperties(OperatorType.RightShift, _rightShiftMapping, null, RightShift, RightShiftErrorMessage),
                new OperatorProperties(OperatorType.Power, _powerMapping, null, Power, PowerErrorMessage),
                new OperatorProperties(OperatorType.Root, _rootMapping, null, Root, RootErrorMessage),
            }.ToDictionary(op => op.Operator));
        }

        public IToken Visit<TToken>(TToken token)
            where TToken : IToken
        {
#if INCOMPLETE_EQ
            if (token == null)
                return null;
#endif

            if (token is IOldLiteralToken)
                return token;

            else if (token is IOperatorToken)
            {
                var opToken = token as OperatorToken;

                if (opToken.IsUnary)
                {
                    var litToken = Visit(opToken.Left) as IOldLiteralToken;

                    switch (opToken.Operator)
                    {
                        case OperatorType.Cast:
                            {
                                var castOp = opToken as CastOperatorToken;
                                var castToken = litToken.CastTo(castOp.CastToType);
                                return castOp.CastToType == TokenDataType.LimitedInteger
                                    ? (castToken as OldLimitedIntegerToken).Upcast(castOp.BitWidth, castOp.IsSigned)
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
                        Visit(opToken.Left) as IOldLiteralToken,
                        Visit(opToken.Right) as IOldLiteralToken);

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
                throw new InvalidCalcOperationException(opProp.ErrorMessageFunction(opProp, pair));
            var token = opProp.OperationFunction(castPair);
            if (token == null)
                throw new InvalidCalcOperationException(opProp.ErrorMessageFunction(opProp, castPair));
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
                            return ((OldDateToken)pair.Left) + ((OldTimespanToken)pair.Right);
                        break;
                    case TokenDataType.Float:
                        if (pair.Right.DataType == TokenDataType.Float)
                            return ((OldFloatToken)pair.Left) + ((OldFloatToken)pair.Right);
                        break;
                    case TokenDataType.LimitedInteger:
                        if (pair.Right.DataType == TokenDataType.LimitedInteger)
                            return ((OldLimitedIntegerToken)pair.Left) + ((OldLimitedIntegerToken)pair.Right);
                        break;
                    case TokenDataType.Time:
                        if (pair.Right.DataType == TokenDataType.Timespan)
                            return ((OldTimeToken)pair.Left) + ((OldTimespanToken)pair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (pair.Right.DataType)
                        {
                            case TokenDataType.Date:
                                return ((OldDateToken)pair.Right) + ((OldTimespanToken)pair.Left);
                            case TokenDataType.Time:
                                return ((OldTimespanToken)pair.Left) + ((OldTimeToken)pair.Right);
                            case TokenDataType.Timespan:
                                return ((OldTimespanToken)pair.Left) + ((OldTimespanToken)pair.Right);
                        }
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((OldUnlimitedIntegerToken)pair.Left) + ((OldUnlimitedIntegerToken)pair.Right);
                        break;
                }
            return null;
        }

        static string AddErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't add a {pair.Left.DataType} ({pair.Left.ObjectValue}) to a {pair.Right.DataType} ({pair.Right.ObjectValue})";

        static readonly BinaryOperandTypeMap _addMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        };

        // Used for unit testing, to validate the return type after an operation on two input types
        static readonly BinaryOperandResultTypeMap _addResultMapping = new BinaryOperandResultTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          TokenDataType.Date },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          TokenDataType.Date },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            TokenDataType.Float },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          TokenDataType.LimitedInteger },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          TokenDataType.Time },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          TokenDataType.Time },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      TokenDataType.Timespan },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      TokenDataType.UnlimitedInteger },
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
                                return ((OldDateToken)pair.Left) - ((OldTimespanToken)pair.Right);
                            case TokenDataType.Date:
                                return ((OldDateToken)pair.Left) - ((OldDateToken)pair.Right);
                        }
                        break;
                    case TokenDataType.Float:
                        if (pair.Right.DataType == TokenDataType.Float)
                            return ((OldFloatToken)pair.Left) - ((OldFloatToken)pair.Right);
                        break;
                    case TokenDataType.LimitedInteger:
                        if (pair.Right.DataType == TokenDataType.LimitedInteger)
                            return ((OldLimitedIntegerToken)pair.Left) - ((OldLimitedIntegerToken)pair.Right);
                        break;
                    case TokenDataType.Time:
                        if (pair.Right.DataType == TokenDataType.Timespan)
                            return ((OldTimeToken)pair.Left) - ((OldTimespanToken)pair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (pair.Right.DataType)
                        {
                            case TokenDataType.Date:
                                return ((OldDateToken)pair.Right) - ((OldTimespanToken)pair.Left);
                            case TokenDataType.Timespan:
                                return ((OldTimespanToken)pair.Left) - ((OldTimespanToken)pair.Right);
                        }
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((OldUnlimitedIntegerToken)pair.Left) - ((OldUnlimitedIntegerToken)pair.Right);
                        break;
                }
            }
            return null;
        }

        static string SubtractErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't subtract a {pair.Right.DataType} ({pair.Right.ObjectValue}) from a {pair.Left.DataType} ({pair.Left.ObjectValue})";

        static readonly BinaryOperandTypeMap _subtractMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              new OperandTypePair(TokenDataType.Date, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        };

        // Used for unit testing, to validate the return type after an operation on two input types
        static readonly BinaryOperandResultTypeMap _subtractResultMapping = new BinaryOperandResultTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              TokenDataType.Timespan },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          TokenDataType.Date },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          TokenDataType.Date },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            TokenDataType.Float },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          TokenDataType.LimitedInteger },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          TokenDataType.Time },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          TokenDataType.Time },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      TokenDataType.Timespan },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      TokenDataType.UnlimitedInteger },
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
                                return ((OldFloatToken)pair.Left) * ((OldFloatToken)pair.Right);
                            case TokenDataType.Timespan:
                                return ((OldFloatToken)pair.Left) * ((OldTimespanToken)pair.Right);
                        }
                        break;
                    case TokenDataType.LimitedInteger:
                        if (pair.Right.DataType == TokenDataType.LimitedInteger)
                            return ((OldLimitedIntegerToken)pair.Left) * ((OldLimitedIntegerToken)pair.Right);
                        break;
                    case TokenDataType.Timespan:
                        if (pair.Right.DataType == TokenDataType.Float)
                            return ((OldTimespanToken)pair.Left) * ((OldFloatToken)pair.Right);
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((OldUnlimitedIntegerToken)pair.Left) * ((OldUnlimitedIntegerToken)pair.Right);
                        break;
                }
            }
            return null;
        }

        static string MultiplyErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't multiply a {pair.Left.DataType} ({pair.Left.ObjectValue}) by a {pair.Right.DataType} ({pair.Right.ObjectValue})";

        static readonly BinaryOperandTypeMap _multiplyMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        };

        // Used for unit testing, to validate the return type after an operation on two input types
        static readonly BinaryOperandResultTypeMap _multiplyResultMapping = new BinaryOperandResultTypeMap()
        {
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            TokenDataType.Float },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         TokenDataType.Timespan },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         TokenDataType.Timespan },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          TokenDataType.LimitedInteger },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      TokenDataType.UnlimitedInteger },
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
                            return ((OldFloatToken)pair.Left) / ((OldFloatToken)pair.Right);
                        break;
                    case TokenDataType.LimitedInteger:
                        if (pair.Right.DataType == TokenDataType.LimitedInteger)
                            return ((OldLimitedIntegerToken)pair.Left) / ((OldLimitedIntegerToken)pair.Right);
                        break;
                    case TokenDataType.Timespan:
                        switch (pair.Right.DataType)
                        {
                            case TokenDataType.Float:
                                return ((OldTimespanToken)pair.Left) / ((OldFloatToken)pair.Right);
                            case TokenDataType.Timespan:
                                return ((OldTimespanToken)pair.Left) / ((OldTimespanToken)pair.Right);
                        }
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((OldUnlimitedIntegerToken)pair.Left) / ((OldUnlimitedIntegerToken)pair.Right);
                        break;
                }
            }
            return null;
        }

        static string DivideErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't divide a {pair.Left.DataType} ({pair.Left.ObjectValue}) by a {pair.Right.DataType} ({pair.Right.ObjectValue})";

        static readonly BinaryOperandTypeMap _divideMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        };

        // Used for unit testing, to validate the return type after an operation on two input types
        static readonly BinaryOperandResultTypeMap _divideResultMapping = new BinaryOperandResultTypeMap()
        {
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            TokenDataType.Float },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         TokenDataType.Timespan },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          TokenDataType.LimitedInteger },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      TokenDataType.Float },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      TokenDataType.UnlimitedInteger },
        };
        #endregion Divide

        #region LeftShift
        static IToken LeftShift(TokenPair pair)
        {
            if (pair != null)
                switch (pair.Left.DataType)
                {
                    case TokenDataType.LimitedInteger:
                        if (pair.Right.DataType == TokenDataType.LimitedInteger)
                            return ((OldLimitedIntegerToken)pair.Left).LeftShift((int)((OldLimitedIntegerToken)pair.Right).TypedValue);
                        else if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((OldLimitedIntegerToken)pair.Left).LeftShift((int)((OldLimitedIntegerToken)pair.Right).TypedValue);
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.LimitedInteger)
                            return ((OldUnlimitedIntegerToken)pair.Left).LeftShift((int)((OldLimitedIntegerToken)pair.Right).TypedValue);
                        else if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((OldUnlimitedIntegerToken)pair.Left).LeftShift((int)((OldLimitedIntegerToken)pair.Right).TypedValue);
                        break;
                }
            return null;
        }

        static string LeftShiftErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't left shift a {pair.Left.DataType} ({pair.Left.ObjectValue}) by a {pair.Right.DataType} ({pair.Right.ObjectValue})";

        static readonly BinaryOperandTypeMap _leftShiftMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger) },
        };
        #endregion LeftShift

        #region RightShift
        static IToken RightShift(TokenPair pair)
        {
            if (pair != null)
                switch (pair.Left.DataType)
                {
                    case TokenDataType.LimitedInteger:
                        if (pair.Right.DataType == TokenDataType.LimitedInteger)
                            return ((OldLimitedIntegerToken)pair.Left).RightShift((int)((OldLimitedIntegerToken)pair.Right).TypedValue);
                        else if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((OldLimitedIntegerToken)pair.Left).RightShift((int)((OldLimitedIntegerToken)pair.Right).TypedValue);
                        break;
                    case TokenDataType.UnlimitedInteger:
                        if (pair.Right.DataType == TokenDataType.LimitedInteger)
                            return ((OldUnlimitedIntegerToken)pair.Left).RightShift((int)((OldLimitedIntegerToken)pair.Right).TypedValue);
                        else if (pair.Right.DataType == TokenDataType.UnlimitedInteger)
                            return ((OldUnlimitedIntegerToken)pair.Left).RightShift((int)((OldLimitedIntegerToken)pair.Right).TypedValue);
                        break;
                }
            return null;
        }

        static string RightShiftErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't right shift a {pair.Left.DataType} ({pair.Left.ObjectValue}) by a {pair.Right.DataType} ({pair.Right.ObjectValue})";

        static readonly BinaryOperandTypeMap _rightShiftMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger) },
        };
        #endregion RightShift

        #region Power
        static IToken Power(TokenPair pair)
        {
            if (pair != null)
            {

            }
            return null;
        }

        static string PowerErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't exponentiate {pair.Left.DataType} ({pair.Left.ObjectValue}) with exponent {pair.Right.DataType} ({pair.Right.ObjectValue})";

        static readonly BinaryOperandTypeMap _powerMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger) },
        };
        #endregion Power

        #region Root
        static IToken Root(TokenPair pair)
        {
            if (pair != null)
            {

            }
            return null;
        }

        static string RootErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't nth root {pair.Left.DataType} ({pair.Left.ObjectValue}) with root degree {pair.Right.DataType} ({pair.Right.ObjectValue})";

        static readonly BinaryOperandTypeMap _rootMapping = new BinaryOperandTypeMap()
        {
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              OperandTypePair.Unsupported },

            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger) },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              OperandTypePair.Unsupported },
            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger) },
        };
        #endregion Root

        #region TokenPair
        public class TokenPair
        {
            public TokenPair(IOldLiteralToken left, IOldLiteralToken right)
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

            public IOldLiteralToken Left { get; set; }
            public IOldLiteralToken Right { get; set; }

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

        #region OperandTypePairWithResult
        [DebuggerDisplay("{Left} \u2295 {Right} = {Result}")]
        public struct OperandTypePairWithResult
        {
            public OperandTypePairWithResult(TokenDataType left, TokenDataType right, TokenDataType result)
                : this(left, right, result, true)
            { }

            private OperandTypePairWithResult(TokenDataType left, TokenDataType right, TokenDataType result, bool supported)
            {
                Left = left;
                Right = right;
                Result = result;
                OperationSupported = supported;
            }

            public TokenDataType Left { get; private set; }
            public TokenDataType Right { get; private set; }
            public TokenDataType Result { get; private set; }
            public bool OperationSupported { get; private set; }

            public static OperandTypePairWithResult Unsupported => _unsupportedPair;
            static readonly OperandTypePairWithResult _unsupportedPair = new OperandTypePairWithResult() { OperationSupported = false };
        }
        #endregion OperandTypePairWithResult

        #region OperatorProperties
        public class OperatorProperties
        {
            public OperatorProperties(OperatorType opType, BinaryOperandTypeMap typeMap, Func<TokenPair, IToken> operationFunc, Func<OperatorProperties, TokenPair, string> errorMessageFunc)
            {
                Operator = opType;
                NAry = 2;
                TypeMap = typeMap;
                ResultMap = null;
                OperationFunction = operationFunc;
                ErrorMessageFunction = errorMessageFunc;
            }

            public OperatorProperties(OperatorType opType, BinaryOperandTypeMap typeMap, BinaryOperandResultTypeMap resultMap, Func<TokenPair, IToken> operationFunc, Func<OperatorProperties, TokenPair, string> errorMessageFunc)
            {
                Operator = opType;
                NAry = 2;
                TypeMap = typeMap;
                ResultMap = resultMap;
                OperationFunction = operationFunc;
                ErrorMessageFunction = errorMessageFunc;
            }

            public OperatorType Operator { get; private set; }
            public int NAry { get; private set; }
            public BinaryOperandTypeMap TypeMap { get; private set; }
            public BinaryOperandResultTypeMap ResultMap { get; private set; } // Only needed for unit testing
            public Func<TokenPair, IToken> OperationFunction { get; private set; }
            public Func<OperatorProperties, TokenPair, string> ErrorMessageFunction { get; set; }
        }
        #endregion OperatorProperties

        readonly static Lazy<ReadOnlyCollection<TokenDataType>> _allPossibleTypes;
        readonly static Lazy<ReadOnlyCollection<OperandTypePair>> _allPossibleTypePairs;
        readonly static ReadOnlyDictionary<OperatorType, OperatorProperties> _operatorProperties;
        readonly EvaluatorSettings _settings;
    }

    public static class TokenEvaluatorExtensions
    {
        public static IOldLiteralToken Evaluate(this IToken token, EvaluatorSettings settings = null)
            => token.Accept(new TokenEvaluator(settings ?? new EvaluatorSettings())) as IOldLiteralToken;
    }

    public class EvaluatorSettings
    {
        public bool ErrorOnOverflow { get; set; }
    }
}
