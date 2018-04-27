using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using System.Globalization;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(UnlimitedIntegerToken))]
    [TestCategory("Basic parse")]
    [TestClass]
    public class UnlimitedIntegerParseTest
    {
        [DataTestMethod]
        [DataRow(new[] { "-123456", "-123456" })]
        [DataRow(new[] { "0", "0" })]
        [DataRow(new[] { "255", "255" })]
        public void DecUnlimitedIntegerValueIsParsedCorrectly(string[] input) 
            => IntegerValueIsParsedCorrectly(input[0], BigInteger.Parse(input[1], NumberStyles.Integer));

        [DataTestMethod]
        [DataRow(new[] { "0x00", "0x0" })]
        [DataRow(new[] { "0xff", "0xff" })]
        [DataRow(new[] { "0XFF", "0xff" })]
        public void HexUnlimitedIntegerValueIsParsedCorrectly(string[] input) 
            => IntegerValueIsParsedCorrectly(input[0], BigInteger.Parse(input[1].Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber));

        public void IntegerValueIsParsedCorrectly(string input, BigInteger expectedValue)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var evalAction = new Action(() => expr.Evaluate());
            evalAction.Should().NotThrow("it should evaluate correctly");

            var token = expr.Evaluate() as UnlimitedIntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid {nameof(UnlimitedIntegerToken)}.");

            token.TypedValue.Should().Be(expectedValue);
        }
    }
}
