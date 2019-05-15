using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.TEST_TEST)]
    [TestClass]
    public partial class TestCommonTest
    {
        [TestMethod]
        public void MakeLiteralTokenCoversAllLiteralTokenTypes()
        {
            foreach(var tokenType in EnumExtensions.GetEnumCollection<LiteralTokenType>())
            {
                Action act = () => TestCommon.MakeLiteralToken(tokenType);
                act.Should().NotThrow<TestOperationException>();
            }
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("2018-10-23 14:24:41", "2018-10-23 14:24:41")]
        public void MakeLiteralToken_Date(string tokenValue, string expectedValue)
        {
            var token = MakeToken<DateToken>(LiteralTokenType.Date, tokenValue);
            if (string.IsNullOrWhiteSpace(tokenValue))
                token.TypedValue.Should().BeCloseTo(DateTime.Now, 50);
            else
                token.TypedValue.Should().Be(DateTime.Parse(expectedValue));
        }

        [DataTestMethod]
        [DataRow("", "1", "i", "32")]
        [DataRow("1u16", "1", "u", "16")]
        [DataRow("-65i64", "-65", "i", "64")]
        public void MakeLiteralToken_LimitedInteger(string tokenValue, string expectedValue, string sign, string bitwidth)
        {
            var token = MakeToken<LimitedIntegerToken>(LiteralTokenType.LimitedInteger, tokenValue);
            token.TypedValue.Should().Be(BigInteger.Parse(expectedValue));
            token.IsSigned.Should().Be(sign == "i");
            token.BitWidth.Should().Be(LimitedIntegerToken.ParseBitWidth(bitwidth));
        }

        [DataTestMethod]
        [DataRow("", "00:00:01")]
        [DataRow("12:34:56", "12:34:56")]
        public void MakeLiteralToken_Time(string tokenValue, string expectedValue)
        {
            var token = MakeToken<TimeToken>(LiteralTokenType.Time, tokenValue);
            token.TypedValue.Should().Be(TimeSpan.Parse(expectedValue));
        }

        [DataTestMethod]
        [DataRow("", "00:00:01")]
        [DataRow("12:34:56", "12:34:56")]
        public void MakeLiteralToken_Timespan(string tokenValue, string expectedValue)
        {
            var token = MakeToken<TimespanToken>(LiteralTokenType.Timespan, tokenValue);
            token.TypedValue.Should().Be(TimeSpan.Parse(expectedValue));
        }

        [DataTestMethod]
        [DataRow("", "1")]
        [DataRow("123456", "123456")]
        [DataRow("-48", "-48")]
        public void MakeLiteralToken_RationalNumber(string tokenValue, string expectedValue)
        {
            var token = MakeToken<RationalNumberToken>(LiteralTokenType.Rational, tokenValue);
            token.TypedValue.Should().Be(TestCommon.ParseFraction(expectedValue).ToRationalNumber());
        }

        static TToken MakeToken<TToken>(LiteralTokenType tokenType, string value = "")
            where TToken : class, ILiteralToken
        {
            var token = string.IsNullOrWhiteSpace(value)
                ? TestCommon.MakeLiteralToken(tokenType)
                : TestCommon.MakeLiteralToken(tokenType, value);
            token.Should().NotBeNull();
            token.Should().BeOfType<TToken>();
            var typedToken = token as TToken;
            typedToken.Should().NotBeNull();
            return typedToken;
        }
    }
}

