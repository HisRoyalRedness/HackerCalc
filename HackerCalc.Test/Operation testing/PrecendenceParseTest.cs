using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestClass]
    [TestCategory("Incomplete")]
    public class PrecendenceParseTest
    {
        [DataTestMethod]
        [DataRow(new[] { "1+2*3", "1 2 3 * +" })]
        [DataRow(new[] { "1+(2*3)", "1 2 3 * +" })]
        [DataRow(new[] { "(1+2*3)", "1 2 3 * +" })]
        [DataRow(new[] { "(1+2)*3", "1 2 + 3 *" })]
        [DataRow(new[] { "1*2+3", "1 2 * 3 +" })]
        [DataRow(new[] { "1*(2+3)", "1 2 3 + *" })]
        [DataRow(new[] { "(1*2)+3", "1 2 * 3 +" })]
        [DataRow(new[] { "(1*2+3)", "1 2 * 3 +" })]
        [DataRow(new[] { "1-2/3", "1 2 3 / -" })]
        [DataRow(new[] { "1-(2/3)", "1 2 3 / -" })]
        [DataRow(new[] { "(1-2/3)", "1 2 3 / -" })]
        [DataRow(new[] { "(1-2)/3", "1 2 - 3 /" })]
        [DataRow(new[] { "1/2-3", "1 2 / 3 -" })]
        [DataRow(new[] { "1/(2-3)", "1 2 3 - /" })]
        [DataRow(new[] { "(1/2)-3", "1 2 / 3 -" })]
        [DataRow(new[] { "(1/2-3)", "1 2 / 3 -" })]
        public void BasicPrecedenceIsDeterminedCorrectly(string[] input)
        {
            input.Should().HaveCount(2);

            var token = Parser.ParseExpression(input[0]) as OperatorToken;
            token.Should().NotBeNull($"{input[0]} should always parse");

            var expr = token.Print(TokenPrinter.FixType.Postfix).Trim();
            expr.Should().Be(input[1]);
        }
    }
}
