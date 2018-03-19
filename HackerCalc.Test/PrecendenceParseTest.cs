using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HisRoyalRedness.com
{
    [TestClass]
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
        public void AddAndMultiplyPrecedenceIsDeterminedCorrectly(string[] input)
        {
            var token = Parser.ParseExpression(input[0]) as OperatorToken;
            Assert.IsNotNull(token);

            var expr = token.Aggregate(new TokenPrinter(TokenPrinter.FixType.Postfix)).Trim();
            Assert.AreEqual(input[1], expr);
        }
    }
}
