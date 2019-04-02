using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using FluentAssertions;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HisRoyalRedness.com
{
    public static class TestCommon
    {
        #region MakeLiteralToken
        public static IEnumerable<ILiteralToken> MakeLiteralTokens(this IEnumerable<string> tokenStrings)
            => tokenStrings.Select(ts => MakeLiteralToken(ts));

        public static TToken MakeLiteralToken<TToken>(this string tokenString)
            where TToken : class, ILiteralToken
            => MakeLiteralToken(tokenString) as TToken;

        public static ILiteralToken MakeLiteralToken(this string tokenString, IConfiguration configuration = null)
        {
            if (string.IsNullOrWhiteSpace(tokenString))
                return null;

            var firstSpace = tokenString.IndexOf(' ');
            var tokenType = firstSpace <= 0
                ? tokenString
                : tokenString.Substring(0, firstSpace).Trim();
            var tokenArg = firstSpace <= 0
                ? ""
                : tokenString.Substring(firstSpace).Trim();

            switch (tokenType.ToLower())
            {
                case "date":
                case "datetoken":
                    return DateToken.Parse(tokenArg, false, SourcePosition.None, configuration);

                case "float":
                case "floattoken":
                    return FloatToken.Parse(tokenArg, configuration);

                case "limitedinteger":
                case "limitedintegertoken":
                    {
                        var portions = _limitedIntegerRegex.Match(tokenArg);
                        var isNeg = portions.Groups[1].Value == "-";
                        var numBase = IntegerBase.Decimal;
                        switch (portions.Groups[2].Value.ToLower())
                        {
                            case "0x": numBase = IntegerBase.Hexadecimal; break;
                            case "b": numBase = IntegerBase.Decimal; break;
                            case "o": numBase = IntegerBase.Decimal; break;
                            case "": numBase = IntegerBase.Decimal; break;
                            default: throw new ArgumentOutOfRangeException($"Unhandled numeric base indicator '{portions.Groups[2].Value}'");
                        }
                        var num = portions.Groups[3].Value;
                        var isSigned = portions.Groups[4].Value.ToLower() != "u";
                        var bitWidth = LimitedIntegerToken.ParseBitWidth(portions.Groups[5].Value);
                        return LimitedIntegerToken.Parse(num, numBase, bitWidth, isSigned, isNeg, tokenArg, SourcePosition.None, configuration ?? new Configuration() { IgnoreLimitedIntegerMaxMinRange = true });
                    }

                case "unlimitedinteger":
                case "unlimitedintegertoken":
                    {
                        var portions = _unlimitedIntegerRegex.Match(tokenArg);
                        var isNeg = portions.Groups[1].Value == "-";
                        var numBase = IntegerBase.Decimal;
                        switch (portions.Groups[2].Value.ToLower())
                        {
                            case "0x": numBase = IntegerBase.Hexadecimal; break;
                            case "b": numBase = IntegerBase.Decimal; break;
                            case "o": numBase = IntegerBase.Decimal; break;
                            case "": numBase = IntegerBase.Decimal; break;
                            default: throw new ArgumentOutOfRangeException($"Unhandled numeric base indicator '{portions.Groups[2].Value}'");
                        }
                        var num = portions.Groups[3].Value;
                        return UnlimitedIntegerToken.Parse(num, numBase, isNeg, SourcePosition.None, configuration);
                    }

                case "timespan":
                case "timespantoken":
                    return TimespanToken.Parse(TimeToken.Parse(tokenArg, SourcePosition.None, configuration).TypedValue, tokenArg, SourcePosition.None, configuration);

                case "time":
                case "timetoken":
                    return TimeToken.Parse(tokenArg, SourcePosition.None, configuration);

                default:
                    throw new TestOperationException($"Unrecognised token type {tokenType}");
            }            
        }

        public static ILiteralToken MakeLiteralToken(this LiteralTokenType tokenType, string value = null)
        {
            switch(tokenType)
            {
                case LiteralTokenType.Date:
                    return string.IsNullOrEmpty(value)
                        ? DateToken.Now
                        : MakeLiteralToken($"date {value}");

                case LiteralTokenType.Float:
                    return string.IsNullOrEmpty(value)
                        ? FloatToken.One
                        : MakeLiteralToken($"float {value}");

                case LiteralTokenType.LimitedInteger:
                    return string.IsNullOrEmpty(value)
                        ? LimitedIntegerToken.One
                        : MakeLiteralToken($"limitedinteger {value}");

                case LiteralTokenType.Time:
                    return string.IsNullOrEmpty(value)
                        ? TimeToken.One
                        : MakeLiteralToken($"time {value}");

                case LiteralTokenType.Timespan:
                    return string.IsNullOrEmpty(value)
                        ? TimespanToken.One
                        : MakeLiteralToken($"timespan {value}");

                case LiteralTokenType.UnlimitedInteger:
                    return string.IsNullOrEmpty(value)
                        ? UnlimitedIntegerToken.One
                        : MakeLiteralToken($"unlimitedinteger {value}");

                default:
                    throw new TestOperationException($"Unsupported literal token type {tokenType}");
            }
        }
        #endregion MakeLiteralToken

        #region Literal token parsing
        public static void LiteralTokensAreParsedCorrectly<TToken>(string stringToParse, string expectedValue)
            where TToken : class, ILiteralToken
        {
            IToken rawToken = null;
            try
            {
                rawToken = Parser.ParseExpression(stringToParse);
            }
            catch(ParseException)
            {
                if (string.IsNullOrWhiteSpace(expectedValue))
                    return;
            }

            // If expectedTokenStr is null, then we expect the parse to fail. 
            // No need to check anything else after that
            if (string.IsNullOrWhiteSpace(expectedValue))
            { 
                rawToken.Should().BeNull();
                return;
            }

            rawToken.Should().NotBeNull("the token should parse correctly");
            rawToken.Should().BeOfType<TToken>();

            var typedToken = rawToken as TToken;
            typedToken.Should().NotBeNull($"the token should cast to {typeof(TToken).Name}");
            LiteralTokenValueParseAndCheck<TToken>(typedToken, expectedValue);
        }

        public static void LiteralTokensAreParsedCorrectly<TToken, TTypedValue>(string stringToParse, TTypedValue expectedValue)
            where TToken : class, ILiteralToken
        {
            var rawToken = Parser.ParseExpression(stringToParse);
            rawToken.Should().NotBeNull("the token should parse correctly");
            rawToken.Should().BeOfType<TToken>();

            var typedToken = rawToken as TToken;
            typedToken.Should().NotBeNull($"the token should cast to {typeof(TToken).Name}");
            LiteralTokenValueCheck<TToken, TTypedValue>(typedToken, expectedValue);
        }

        static void LiteralTokenValueParseAndCheck<TToken>(TToken token, string expectedValue)
                    where TToken : class, ILiteralToken
        {
            switch (typeof(TToken).Name)
            {
                case nameof(DateToken):
                    LiteralTokenValueCheck(token as DateToken, DateTime.Parse(expectedValue));
                    break;
                case nameof(FloatToken):
                    LiteralTokenValueCheck(token as FloatToken, float.Parse(expectedValue));
                    break;
                case nameof(LimitedIntegerToken):
                    LiteralTokenValueCheck(token as LimitedIntegerToken, BigInteger.Parse(expectedValue));
                    break;
                case nameof(TimespanToken):
                    LiteralTokenValueCheck(token as TimespanToken, TimeSpan.Parse(expectedValue));
                    break;
                case nameof(TimeToken):
                    LiteralTokenValueCheck(token as TimeToken, TimeSpan.Parse(expectedValue));
                    break;
                case nameof(UnlimitedIntegerToken):
                    LiteralTokenValueCheck(token as UnlimitedIntegerToken, BigInteger.Parse(expectedValue));
                    break;
                default:
                    throw new TestOperationException($"Could not do a comparison of {typeof(TToken).Name}. The type is unsuppported.");
            }
        }

        static void LiteralTokenValueCheck<TToken, TValueType>(TToken token, TValueType expectedValue)
            where TToken : class, ILiteralToken
        {
            switch (typeof(TToken).Name)
            {
                case nameof(DateToken):
                    (token as DateToken).Should().BeTypedValue((expectedValue as DateTime?).Value);
                    break;
                case nameof(FloatToken):
                    (token as FloatToken).Should().BeTypedValue((expectedValue as float?).Value);
                    break;
                case nameof(LimitedIntegerToken):
                    (token as LimitedIntegerToken).Should().BeTypedValue((expectedValue as BigInteger?).Value);
                    break;
                case nameof(TimespanToken):
                    (token as TimespanToken).Should().BeTypedValue((expectedValue as TimeSpan?).Value);
                    break;
                case nameof(TimeToken):
                    (token as TimeToken).Should().BeTypedValue((expectedValue as TimeSpan?).Value);
                    break;
                case nameof(UnlimitedIntegerToken):
                    (token as UnlimitedIntegerToken).Should().BeTypedValue((expectedValue as BigInteger?).Value);
                    break;
                default:
                    throw new TestOperationException($"Could not do a comparison of {typeof(TToken).Name}. The type is unsuppported.");
            }
        }
        #endregion Literal token parsing

        #region Operator parsing
        public static void BinaryOperatorParsesCorrectly(string input, OperatorType expectedType, int leftValue = 1, int rightValue = 2)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as OperatorToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid OperatorToken.");

            token.IsUnary.Should().BeFalse("we expect these to be binary operators.");

            var leftToken = token.Left as UnlimitedIntegerToken;
            leftToken.Should().NotBeNull($"the left token is expected to be a {nameof(UnlimitedIntegerToken)}");
            leftToken.TypedValue.Should().Be(leftValue);

            var rightToken = token.Right as UnlimitedIntegerToken;
            rightToken.Should().NotBeNull($"the right token is expected to be an {nameof(UnlimitedIntegerToken)}");
            rightToken.TypedValue.Should().Be(rightValue);

            token.Operator.Should().Be(expectedType);
        }

        public static void UnaryOperatorParsesCorrectly(string input, OperatorType expectedType)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as OperatorToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid OperatorToken.");

            token.IsUnary.Should().BeTrue("we expect these to be unary operators.");

            var leftToken = token.Left as UnlimitedIntegerToken;
            leftToken.Should().NotBeNull($"the left token is expected to be an {nameof(UnlimitedIntegerToken)}");
            leftToken.TypedValue.Should().Be(1);

            token.Right.Should().BeNull("the right token is always null for a unary operator");

            token.Operator.Should().Be(expectedType);
        }
        #endregion Operator parsing

        #region MakeDataType
        public static IDataType<DataType> MakeDataType(DataType dataType, string tokenString = null)
        {
            ILiteralToken token;
            switch (dataType)
            {
                case DataType.Date: token = MakeLiteralToken(LiteralTokenType.Date, tokenString); break;
                case DataType.Float: token = MakeLiteralToken(LiteralTokenType.Float, tokenString); break;
                case DataType.LimitedInteger: token = MakeLiteralToken(LiteralTokenType.LimitedInteger, tokenString); break;
                case DataType.Time: token = MakeLiteralToken(LiteralTokenType.Time, tokenString); break;
                case DataType.Timespan: token = MakeLiteralToken(LiteralTokenType.Timespan, tokenString); break;
                case DataType.UnlimitedInteger: token = MakeLiteralToken(LiteralTokenType.UnlimitedInteger, tokenString); break;
                default:
                    throw new TestOperationException($"Unsupported data type '{dataType}'.");
            }
            return CalcEngine.Instance.ConvertToTypedDataType(token);
        }
        #endregion MakeDataType

        public static IDataType<DataType> Evaluate(this string input, IConfiguration configuration = null)
            => (IDataType<DataType>)_evaluator.Value.Evaluate(Parser.ParseExpression(input, configuration));

        public static IDataType<TDataEnum> Evaluate<TDataEnum>(this string input, ICalcEngine<TDataEnum> calcEngine, IConfiguration configuration = null)
            where TDataEnum : Enum
            => (IDataType<TDataEnum>)new Evaluator<TDataEnum>(calcEngine).Evaluate(Parser.ParseExpression(input, configuration));

        public static void CompareParseTree(string input, string expectedParseString = null, TokenPrinter.FixType fixType = TokenPrinter.FixType.Postfix)
        {
            IToken rawToken = null;
            try
            {
                rawToken = Parser.ParseExpression(input);
            }
            catch (ParseException)
            {
                if (string.IsNullOrWhiteSpace(expectedParseString))
                    return;
            }

            // If expectedTokenStr is null, then we expect the parse to fail. 
            // No need to check anything else after that
            if (string.IsNullOrWhiteSpace(expectedParseString))
            {
                rawToken.Should().BeNull();
                return;
            }

            rawToken.Should().NotBeNull("the token should parse correctly");

            var parseText = rawToken.Print(fixType).Trim();
            parseText.Should().BeEquivalentTo(expectedParseString.Trim());
        }

        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        public static void TestThatAllPossibleOperandTypesAreSupported(Dictionary<DataType, HashSet<DataType>> supportedOperandTypes, Func<InternalDataTypeBase, InternalDataTypeBase, InternalDataTypeBase> operation, string operationDescription)
        {
            var allTypePairs = (new[] { EnumExtensions.GetEnumCollection<DataType>(), EnumExtensions.GetEnumCollection<DataType>() })
                .CartesianProduct()
                .Select(r => new DataTypePair<DataType>(r.First(), r.Last()))
                .Distinct();

            var supportedTypes = allTypePairs
                .Where(p => supportedOperandTypes.ContainsKey(p.Left) && supportedOperandTypes[p.Left].Contains(p.Right))
                .ToList();

            foreach (var pair in allTypePairs)
            {
                var valuePair = new DataTypeValuePair<DataType>(MakeDataType(pair.Left), MakeDataType(pair.Right));
                var act = new Action(() => _ = operation(valuePair.Left as InternalDataTypeBase, valuePair.Right as InternalDataTypeBase));
                if (supportedTypes.Contains(pair))
                    act.Should().NotThrow($"a {pair.Left} and {pair.Right} should be able to be {operationDescription}");
                else
                    act.Should().Throw<Exception>($"a {pair.Left} and {pair.Right} should not be able to be {operationDescription}");
            }
        }

        public static void EvaluateActualAndExpected(string actualStr, string expectedStr, string expectedTypeStr = null, IConfiguration configuration = null)
        {
            if (string.IsNullOrEmpty(expectedStr))
            {
                new Action(() => actualStr.Evaluate(configuration ?? Configuration.Default)).Should().Throw<InvalidCalcOperationException>();
                return;
            }

            var actual = actualStr.Evaluate(configuration ?? Configuration.Default);
            var expected = expectedStr.Evaluate(configuration ?? Configuration.Default);

            // Fluent assertions don't print the message correctly
            //Assert.IsNotNull(actual, $"{nameof(actualStr)} should evaluate to a valid {nameof(IDataType)}.");
            //Assert.IsNotNull(expected, $"{nameof(expectedStr)} should evaluate to a valid {nameof(IDataType)}.");
            actual.Should().NotBeNull($"{nameof(actualStr)} should evaluate to a valid {nameof(IDataType)}.");
            expected.Should().NotBeNull($"{nameof(expectedStr)} should evaluate to a valid {nameof(IDataType)}.");

            if (!string.IsNullOrEmpty(expectedTypeStr))
            {
                var expectedType = (DataType)Enum.Parse(typeof(DataType), expectedTypeStr);
                actual.DataType.Should().Be(expectedType);
                expected.DataType.Should().Be(expectedType);
            }

            actual.Should().Be(expected);
        }

        #region Test trait descriptions
        public const string INCOMPLETE = "Incomplete";
        public const string BASIC_PARSE = "Basic parse";
        public const string BASIC_OPERATION = "Basic operation";
        public const string DATA_MAPPING = "Data mapping";
        public const string CALC_ENGINE = "CalcEngine";
        public const string TYPE_CASTING = "Type casting";
        public const string TEST_TEST = "Test tests";
        public const string VISITOR = "Visitors";
        public const string TOKEN_PRINTER = "Token printer";
        public const string LITERAL_TOKEN_PARSE = "Literal token parse";
        public const string OPERATOR_TOKEN_PARSE = "Operator token parse";
        public const string FUNCTION_TOKEN_PARSE = "Function token parse";

        public const string ADD_OPERATION = "Addition";
        public const string SUBTRACT_OPERATION = "Subtraction";
        #endregion Test trait descriptions

        public static IReadOnlyList<IntegerBitWidth> IntegerBitWidths { get; }
            = Enum.GetValues(typeof(IntegerBitWidth)).Cast<IntegerBitWidth>().ToList().AsReadOnly();

        static Lazy<Evaluator<DataType>> _evaluator = new Lazy<Evaluator<DataType>>(() => new Evaluator<DataType>(CalcEngine.Instance));

        static Regex _limitedIntegerRegex = new Regex(@"(-)?(0x|b|o)?([0-9a-f]+)([iu])(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex _unlimitedIntegerRegex = new Regex(@"(-)?(0x|b|o)?([0-9a-f]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    #region Test exceptions
    public class TestOperationException : ApplicationException
    {
        public TestOperationException(string message)
            : base(message)
        { }

        public TestOperationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
    #endregion Test exceptions

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TypedDataRowAttribute : DataRowAttribute
    {
        public TypedDataRowAttribute(object data1)
            : base(data1?.ToString())
        { }

        public TypedDataRowAttribute(object data1, params object[] moreData)
            : base(data1?.ToString(), moreData.Select(d => d?.ToString()).ToArray())
        { }
    }
}
