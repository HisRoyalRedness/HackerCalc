using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class OperatorCastTest
    {
        [DataTestMethod]
        [DataRow(new [] { "2018-01-01 + 2018-01-01", null })]
        [DataRow(new[] { "2018-01-01 + 3.0", "date 2018-01-01 00:00:03" })]
        public void AddOperations(string[] input)
        {
            input.Should().HaveCount(2);

            var expr = input[0];
            var expectedResult = TestCommon.MakeToken(input[1]);

            var rootToken = Parser.ParseExpression(expr);
            rootToken.Should().NotBeNull($"input {input[0]} should always parse");

            if (expectedResult == null)
            {
                Action a = () => rootToken.Evaluate();
                a.Should().Throw<InvalidCalcOperationException>($"for input {input[0]}");
            }
            else
            {
                var actualResult = rootToken.Evaluate() as ILiteralToken;
                actualResult.Should().Be(expectedResult, $"for input {input[0]}");
            }
        }
    }
}