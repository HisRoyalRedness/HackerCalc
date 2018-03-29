using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class FloatParseTest
    {
        [DataTestMethod]
        [DataRow("1.0")]
        public void TrueFloatValueIsParsedCorrectly(string input)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as FloatToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid FloatToken.");

            token.TypedValue.Should().Be(1.0);
        }

        [DataTestMethod]
        [DataRow("1F")]
        public void TypedFloatValueIsParsedCorrectly(string input)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as FloatToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid FloatToken.");

            token.TypedValue.Should().Be(1.0);

        }
    }
}
