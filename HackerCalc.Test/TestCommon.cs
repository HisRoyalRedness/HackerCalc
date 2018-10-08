using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using FluentAssertions;
using System.Numerics;

namespace HisRoyalRedness.com
{
    public static class TestCommon
    {
        static TestCommon()
        {
            _integerBitWidths = Enum.GetValues(typeof(OldLimitedIntegerToken.IntegerBitWidth)).Cast<OldLimitedIntegerToken.IntegerBitWidth>().ToList().AsReadOnly();
        }

        #region MakeToken
        public static IEnumerable<IOldLiteralToken> MakeTokens(this IEnumerable<string> tokenStrings)
            => tokenStrings.Select(ts => MakeToken(ts));

        public static TToken MakeToken<TToken>(this string tokenString)
            where TToken : class, IOldLiteralToken
            => MakeToken(tokenString) as TToken;

        public static IOldLiteralToken MakeToken(this string tokenString)
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
                    return OldDateToken.Parse(tokenArg);

                case "float":
                case "floattoken":
                    return OldFloatToken.Parse(tokenArg);

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
                            case "": numBase = IntegerBase.Decimal; break;
                            default: throw new ArgumentOutOfRangeException($"Unhandled numeric base indicator '{portions.Groups[2].Value}'");
                        }
                        var num = portions.Groups[3].Value;
                        var isSigned = portions.Groups[4].Value.ToLower() != "u";
                        var bitWidth = OldLimitedIntegerToken.ParseBitWidth(portions.Groups[5].Value);
                        return OldLimitedIntegerToken.Parse((isNeg ? $"-{num}" : num), numBase, bitWidth, isSigned);
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
                            case "": numBase = IntegerBase.Decimal; break;
                            default: throw new ArgumentOutOfRangeException($"Unhandled numeric base indicator '{portions.Groups[2].Value}'");
                        }
                        var num = portions.Groups[3].Value;
                        return OldUnlimitedIntegerToken.Parse((isNeg ? $"-{num}" : num), numBase);
                    }

                case "timespan":
                case "timespantoken":
                    return OldTimeToken.Parse(tokenArg).CastTo(TokenDataType.Timespan) as OldTimespanToken;

                case "time":
                case "timetoken":
                    return OldTimeToken.Parse(tokenArg);

                default:
                    throw new NotSupportedException($"Unrecognised token type {tokenType}");
            }            
        }

        public static IOldLiteralToken MakeToken(this TokenDataType dataType, string value = null)
        {
            switch(dataType)
            {
                case TokenDataType.Date:
                    return string.IsNullOrEmpty(value)
                        ? new OldDateToken()
                        : MakeToken($"date {value}");

                case TokenDataType.Float:
                    return string.IsNullOrEmpty(value)
                        ? new OldFloatToken(1.0)
                        : MakeToken($"float {value}");

                case TokenDataType.LimitedInteger:
                    return string.IsNullOrEmpty(value)
                        ? new OldLimitedIntegerToken(1, OldLimitedIntegerToken.IntegerBitWidth._32, true)
                        : MakeToken($"integer {value}");

                case TokenDataType.Time:
                    return string.IsNullOrEmpty(value)
                        ? new OldTimeToken(TimeSpan.FromSeconds(1))
                        : MakeToken($"time {value}");

                case TokenDataType.Timespan:
                    return string.IsNullOrEmpty(value)
                        ? new OldTimespanToken(TimeSpan.FromSeconds(1))
                        : MakeToken($"timespan {value}");

                case TokenDataType.UnlimitedInteger:
                    return string.IsNullOrEmpty(value)
                        ? new OldUnlimitedIntegerToken(1)
                        : MakeToken($"unlimitedinteger {value}");

                case TokenDataType.RationalNumber:
                case TokenDataType.IrrationalNumber:
                case TokenDataType.DigitalInteger:
                default:
                    throw new NotSupportedException($"Unsupported token data type {dataType}");
            }
        }

        public static IOperatorToken MakeBinaryExpression(this OperatorType opType, IOldLiteralToken left, IOldLiteralToken right)
            => new OperatorToken(opType).Tap(ot => ot.Left = left).Tap(ot => ot.Right = right);
        #endregion MakeToken

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
                //case nameof(DateToken):
                //    LiteralTokenValueCheck(token as DateToken, DateTime.Parse(expectedValue));
                //    break;
                case nameof(FloatToken):
                    LiteralTokenValueCheck(token as FloatToken, float.Parse(expectedValue));
                    break;
                case nameof(LimitedIntegerToken):
                    LiteralTokenValueCheck(token as LimitedIntegerToken, BigInteger.Parse(expectedValue));
                    break;
                //case nameof(TimespanToken):
                //    LiteralTokenValueCheck(token as TimespanToken, TimeSpan.Parse(expectedValue));
                //    break;
                //case nameof(TimeToken):
                //    LiteralTokenValueCheck(token as TimeToken, TimeSpan.Parse(expectedValue));
                //    break;
                case nameof(UnlimitedIntegerToken):
                    LiteralTokenValueCheck(token as UnlimitedIntegerToken, BigInteger.Parse(expectedValue));
                    break;
                default:
                    throw new NotSupportedException($"Could not do a comparison of {typeof(TToken).Name}. The type is unsuppported.");
            }
        }

        static void LiteralTokenValueCheck<TToken, TValueType>(TToken token, TValueType expectedValue)
            where TToken : class, ILiteralToken
        {
            switch (typeof(TToken).Name)
            {
                //case nameof(DateToken):
                //    (token as DateToken).Should().BeTypedValue((expectedValue as DateTime?).Value);
                //    break;
                case nameof(FloatToken):
                    (token as FloatToken).Should().BeTypedValue((expectedValue as float?).Value);
                    break;
                case nameof(LimitedIntegerToken):
                    (token as LimitedIntegerToken).Should().BeTypedValue((expectedValue as BigInteger?).Value);
                    break;
                //case nameof(TimespanToken):
                //    (token as TimespanToken).Should().BeTypedValue((expectedValue as TimeSpan?).Value);
                //    break;
                //case nameof(TimeToken):
                //    (token as TimeToken).Should().BeTypedValue((expectedValue as TimeSpan?).Value);
                //    break;
                case nameof(UnlimitedIntegerToken):
                    (token as UnlimitedIntegerToken).Should().BeTypedValue((expectedValue as BigInteger?).Value);
                    break;
                default:
                    throw new NotSupportedException($"Could not do a comparison of {typeof(TToken).Name}. The type is unsuppported.");
            }
        }
        #endregion Literal token parsing

        #region Operator parsing
        public static void BinaryOperatorParsesCorrectly(string input, OperatorType expectedType)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as OperatorToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid OperatorToken.");

            token.IsUnary.Should().BeFalse("we expect these to be binary operators.");

            var leftToken = token.Left as OldUnlimitedIntegerToken;
            leftToken.Should().NotBeNull($"the left token is expected to be a {nameof(OldUnlimitedIntegerToken)}");
            leftToken.TypedValue.Should().Be(1);

            var rightToken = token.Right as OldUnlimitedIntegerToken;
            rightToken.Should().NotBeNull($"the right token is expected to be an {nameof(OldUnlimitedIntegerToken)}");
            rightToken.TypedValue.Should().Be(2);

            token.Operator.Should().Be(expectedType);
        }

        public static void UnaryOperatorParsesCorrectly(string input, OperatorType expectedType)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as OperatorToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid OperatorToken.");

            token.IsUnary.Should().BeTrue("we expect these to be unary operators.");

            var leftToken = token.Left as OldUnlimitedIntegerToken;
            leftToken.Should().NotBeNull($"the left token is expected to be an {nameof(OldUnlimitedIntegerToken)}");
            leftToken.TypedValue.Should().Be(1);

            token.Right.Should().BeNull("the right token is always null for a unary operator");

            token.Operator.Should().Be(expectedType);
        }
        #endregion Operator parsing

        #region Evaluation checking
        public static void ExpressionEvaluatesTo<TToken>(string stringToParse, string expectedValue)
            where TToken : class, ILiteralToken
        {
            var expr = Parser.ParseExpression(stringToParse);
            expr.Should().NotBeNull("the expression is expected to parse correctly");

            var token = expr.Evaluate();


            // If expectedValue is null, then we expect the evaluation to fail. 
            // No need to check anything else after that
            if (string.IsNullOrWhiteSpace(expectedValue))
            {
                token.Should().BeNull();
                return;
            }

            token.Should().NotBeNull("the expression should evaluate correctly");
            token.Should().BeOfType<TToken>();

            var typedToken = token as TToken;
            typedToken.Should().NotBeNull($"the token should cast to {typeof(TToken).Name}");
            LiteralTokenValueParseAndCheck<TToken>(typedToken, expectedValue);
        }
        #endregion Evaluation checking

        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        #region Test trait descriptions
        public const string INCOMPLETE = "Incomplete";
        public const string BASIC_PARSE = "Basic parse";
        public const string BASIC_OPERATION = "Basic operation";
        public const string LITERAL_TOKEN_PARSE = "Literal token parse";
        public const string OPERATOR_TOKEN_PARSE = "Operator token parse";
        #endregion Test trait descriptions

        public static IReadOnlyList<OldLimitedIntegerToken.IntegerBitWidth> IntegerBitWidths => _integerBitWidths;

        readonly static IReadOnlyList<OldLimitedIntegerToken.IntegerBitWidth> _integerBitWidths;
        static Regex _limitedIntegerRegex = new Regex(@"(-)?(0x|b)?([0-9a-f]+)([iu])(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex _unlimitedIntegerRegex = new Regex(@"(-)?(0x|b)?([0-9a-f]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
