using HisRoyalRedness.com;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

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
    }
}

