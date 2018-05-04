using System;
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
        #region MakeToken
        public static IEnumerable<ILiteralToken> MakeTokens(this IEnumerable<string> tokenStrings)
            => tokenStrings.Select(ts => MakeToken(ts));

        public static TToken MakeToken<TToken>(this string tokenString)
            where TToken : class, ILiteralToken
            => MakeToken(tokenString) as TToken;

        public static ILiteralToken MakeToken(this string tokenString)
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
                    return DateToken.Parse(tokenArg);

                case "float":
                case "floattoken":
                    return FloatToken.Parse(tokenArg);

                case "integer":
                case "integertoken":
                    {
                        var portions = _integerRegex.Match(tokenArg);
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
                        var bitWidth = string.IsNullOrEmpty(portions.Groups[5].Value)
                            ? IntegerToken.IntegerBitWidth.Unbound
                            : IntegerToken.ParseBitWidth(portions.Groups[5].Value);
                        return IntegerToken.Parse((isNeg ? $"-{num}" : num), numBase, isSigned, bitWidth);
                    }

                case "unlimitedinteger":
                case "unlimitedintegertoken":
                    {
                        var portions = _integerRegex.Match(tokenArg);
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
                        return UnlimitedIntegerToken.Parse((isNeg ? $"-{num}" : num), numBase);
                    }

                case "timespan":
                case "timespantoken":
                    return TimeToken.Parse(tokenArg).CastTo(TokenDataType.Timespan) as TimespanToken;

                case "time":
                case "timetoken":
                    return TimeToken.Parse(tokenArg);

                default:
                    throw new NotSupportedException($"Unrecognised token type {tokenType}");
            }            
        }

        public static ILiteralToken MakeToken(this TokenDataType dataType, string value = null)
        {
            switch(dataType)
            {
                case TokenDataType.Date:
                    return string.IsNullOrEmpty(value)
                        ? new DateToken()
                        : MakeToken($"date {value}");

                case TokenDataType.Float:
                    return string.IsNullOrEmpty(value)
                        ? new FloatToken(1.0)
                        : MakeToken($"float {value}");

                case TokenDataType.Integer:
                    return string.IsNullOrEmpty(value)
                        ? new IntegerToken(1)
                        : MakeToken($"integer {value}");

                case TokenDataType.Time:
                    return string.IsNullOrEmpty(value)
                        ? new TimeToken(TimeSpan.FromSeconds(1))
                        : MakeToken($"time {value}");

                case TokenDataType.Timespan:
                    return string.IsNullOrEmpty(value)
                        ? new TimespanToken(TimeSpan.FromSeconds(1))
                        : MakeToken($"timespan {value}");

                case TokenDataType.UnlimitedInteger:
                    return string.IsNullOrEmpty(value)
                        ? new UnlimitedIntegerToken(1)
                        : MakeToken($"unlimitedinteger {value}");

                case TokenDataType.RationalNumber:
                case TokenDataType.LimitedInteger:
                case TokenDataType.IrrationalNumber:
                case TokenDataType.DigitalInteger:
                default:
                    throw new NotSupportedException($"Unsupported token data type {dataType}");
            }
        }

        public static IOperatorToken MakeBinaryExpression(this OperatorType opType, ILiteralToken left, ILiteralToken right)
            => new OperatorToken(opType).Tap(ot => ot.Left = left).Tap(ot => ot.Right = right);
        #endregion MakeToken

        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }


        public static void LiteralTokensAreParsedCorrectly<TToken>(string stringToParse, string expectedValue)
            where TToken : class, ILiteralToken
        {
            var rawToken = Parser.ParseExpression(stringToParse);

            // If expectedTokenStr is null, then we expect the parse to fail. 
            // No need to check anything else after that
            if (string.IsNullOrWhiteSpace(expectedValue))
            {
                if (rawToken != null)
                    rawToken.Should().NotBeOfType<TToken>();
                return;
            }
            else
            {
                rawToken.Should().NotBeNull("the token should parse correctly");
                rawToken.Should().BeOfType<TToken>();
            }


            var typedToken = rawToken as TToken;
            typedToken.Should().NotBeNull($"the token should cast to {typeof(TToken).Name}");

            switch (typeof(TToken).Name)
            {
                case nameof(DateToken):
                    (typedToken as DateToken).Should().BeTypedValue(DateTime.Parse(expectedValue));
                    break;
                case nameof(FloatToken):
                    (typedToken as FloatToken).Should().BeTypedValue(float.Parse(expectedValue));
                    break;
                case nameof(IntegerToken):
                    (typedToken as IntegerToken).Should().BeTypedValue(BigInteger.Parse(expectedValue));
                    break;
                case nameof(TimespanToken):
                    (typedToken as TimespanToken).Should().BeTypedValue(TimeSpan.Parse(expectedValue));
                    break;
                case nameof(TimeToken):
                    (typedToken as TimeToken).Should().BeTypedValue(TimeSpan.Parse(expectedValue));
                    break;
                case nameof(UnlimitedIntegerToken):
                    (typedToken as UnlimitedIntegerToken).Should().BeTypedValue(BigInteger.Parse(expectedValue));
                    break;

                default:
                    throw new NotSupportedException($"Could not do a comparison of {typeof(TToken).Name}. The type is unsuppported.");
            }
            //Assert.AreEqual(expectedDate, actualDate);
        }

        static Regex _integerRegex = new Regex(@"(-)?(0x|b)?([0-9a-f]+)([iu])?(\d+)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex _unlimitedIntegerRegex = new Regex(@"(-)?(0x|b)?([0-9a-f]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
