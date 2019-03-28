using HisRoyalRedness.com;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(TimeToken))]
    [TestCategory(TestCommon.BASIC_PARSE)]
    [TestCategory(TestCommon.LITERAL_TOKEN_PARSE)]
    [TestClass]
    public class TimeParseTest
    {
        [DataTestMethod]
        [DataRow("0:00", "0")]
        [DataRow("00:00", "0")]
        [DataRow("1:01", "1:01:00")]
        [DataRow("23:59", "23:59:00")]
        public void HoursAndMinutesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<TimeToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("0:0")]
        [DataRow("00:0")]
        [DataRow("0:0:0")]
        [DataRow("0:0:00")]
        [DataRow("0:00:0")]
        [DataRow("00:0:0")]
        [DataRow("00:00:0")]
        [DataRow("1:60")]
        [DataRow("24:00")]
        [DataRow("30:01")]
        [DataRow("1:00:60")]
        public void InvalidTimesAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<TimeToken>(stringToParse, null);

        [DataTestMethod]
        [DataRow("0:00:00", "0")]
        [DataRow("00:00:00", "0")]
        [DataRow("01:01:01", "01:01:01")]
        [DataRow("23:59:59", "23:59:59")]
        public void HoursMinutesAndSecondsAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<TimeToken>(stringToParse, expectedTokenStr);

        [TestMethod]
        public void TimeParsesToCurrentTime()
        {
            var tToken = Parser.ParseExpression("time") as TimeToken ?? throw new ParseException("Couldn't parse TIME");
            (DateTime.Now.TimeOfDay - tToken.TypedValue).Should().BeCloseTo(TimeSpan.Zero, 20);
        }

        [TestMethod]
        public void DaysAreExcludedByDefault()
        {
            new Action(() => Parser.ParseExpression("1.10:34:45")).Should().Throw<ParseException>();
        }

        [TestMethod]
        public void DaysCanBeIncludedWithConfiguration()
        {
            var config = new Configuration()
            {
                AllowMultidayTimes = true
            };

            var token = Parser.ParseExpression("1.10:34:45", config) as TimeToken;
            token.Should().NotBeNull();
            token.TypedValue.Days.Should().Be(1);

            config.AllowMultidayTimes = false;
            new Action(() => Parser.ParseExpression("1.10:34:45", config)).Should().Throw<ParseException>();
        }
    }
}

