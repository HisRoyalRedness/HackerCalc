using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(DateToken))]
    [TestCategory("Basic parse")]
    [TestClass]
    public class DateParseTest
    {
        const double PRECISION = 0.000000000001;

        [DataTestMethod]
        [DataRow(new[] { "18-02-01 01:01", "2018-02-01 01:01:00" })]
        [DataRow(new[] { "2018-02-01 01:01:01", "2018-02-01 01:01:01" })]
        [DataRow(new[] { "01-02-2018 01:01:01", "2018-02-01 01:01:01" })]
        public void DatesAreParsedCorrectly(string[] input)
        {
            var token = Parser.ParseExpression(input[0]) as DateToken;
            Assert.IsNotNull(token);

            var actualDate = token.TypedValue;
            var expectedDate = DateTime.Parse(input[1]);

            Assert.AreEqual(expectedDate, actualDate);
        }

        [DataTestMethod]
        [DataRow("8-2-1")]
        [DataRow("2018-13-01")]
        [DataRow("2018-12-32")]
        [DataRow("2018-12-41")]
        [DataRow("2018-13-01")]
        [DataRow("2018-21-01")]
        public void InvalidDatesAreNotParsed(string input)
        {
            var token = Parser.ParseExpression(input) as DateToken;
            Assert.IsNull(token);
        }

        [DataTestMethod]
        [DataRow(new[] { "18-02-01", "2018-02-01" })]
        [DataRow(new[] { "18-2-1", "2018-02-01" })]
        [DataRow(new[] { "2018-02-01", "2018-02-01" })]
        [DataRow(new[] { "01-02-2018", "2018-02-01" })]
        [DataRow(new[] { "1-2-2018", "2018-02-01" })]
        [DataRow(new[] { "2018-12-01", "2018-12-01" })]
        [DataRow(new[] { "2018-09-01", "2018-09-01" })]
        [DataRow(new[] { "2018-12-29", "2018-12-29" })]
        [DataRow(new[] { "2018-12-31", "2018-12-31" })]
        public void DateTimesAreParsedCorrectly(string[] input)
        {
            var token = Parser.ParseExpression(input[0]) as DateToken;
            Assert.IsNotNull(token);

            var actualDate = token.TypedValue;
            var expectedDate = DateTime.Parse(input[1]);

            Assert.AreEqual(expectedDate, actualDate);
        }
    }
}

