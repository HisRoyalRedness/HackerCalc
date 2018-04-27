using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(TimeToken))]
    [TestCategory("Basic parse")]
    [TestClass]
    public class TimeParseTest
    {
        const double PRECISION = 0.000000000001;

        [DataTestMethod]
        [DataRow(new[] { "0:00", "0" })]
        [DataRow(new[] { "00:00", "0" })]
        [DataRow(new[] { "1:01", "3660" })]
        [DataRow(new[] { "23:59", "86340" })]
        public void HoursAndMinutesAreParsedCorrectly(string[] input)
        {
            var token = Parser.ParseExpression(input[0]) as TimeToken;
            Assert.IsNotNull(token);

            var actualSeconds = token.TypedValue.TotalSeconds;
            var expectedSeconds = int.Parse(input[1]);

            Assert.AreEqual(expectedSeconds, actualSeconds, PRECISION);
        }

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
        public void InvalidTimesAreNotParsed(string input)
        {
            var token = Parser.ParseExpression(input);
            Assert.IsNull(token);
        }

        [DataTestMethod]
        [DataRow(new[] { "0:00:00", "0" })]
        [DataRow(new[] { "00:00:00", "0" })]
        [DataRow(new[] { "01:01:01", "3661" })]
        [DataRow(new[] { "23:59:59", "86399" })]
        public void HoursMinutesAndSecondsAreParsedCorrectly(string[] input)
        {
            var token = Parser.ParseExpression(input[0]) as TimeToken;
            Assert.IsNotNull(token);

            var actualSeconds = token.TypedValue.TotalSeconds;
            var expectedSeconds = int.Parse(input[1]);

            Assert.AreEqual(expectedSeconds, actualSeconds, PRECISION);
        }
    }
}

