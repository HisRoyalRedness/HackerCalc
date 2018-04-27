using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(FloatToken))]
    [TestCategory("Basic parse")]
    [TestClass]
    public class FloatParseTest
    {
        [DataTestMethod]
        [DataRow(new[] { "1.0", "1.0 " })]
        public void TrueFloatValueIsParsedCorrectly(string[] input) => FloatValueIsParsedCorrectly(input[0], double.Parse(input[1]));

        [DataTestMethod]
        [DataRow(new[] { "1F", "1.0" })]
        public void TypedFloatValueIsParsedCorrectly(string[] input) => FloatValueIsParsedCorrectly(input[0], double.Parse(input[1]));

        public void FloatValueIsParsedCorrectly(string input, double expectedValue)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as FloatToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid FloatToken.");

            token.TypedValue.Should().BeApproximately(expectedValue, float.Epsilon);
        }
    }
}
