using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class ParserLiteralFormatTest
    {
        [DataTestMethod]
        [DataRow("255")]
        [DataRow("255U")]
        [DataRow("255I")]
        [DataRow("0xff")]
        [DataRow("0xffI")]
        [DataRow("0xffU")]
        public void IntegerIsParsedWithAttachedSignFlag(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as LiteralToken<BigInteger>).ToList();

            Assert.AreEqual(1, tokens.Count, "only a single token is expected");
            Assert.AreEqual(255, tokens.First()?.TypedValue);
        }

        [DataTestMethod]
        [DataRow("255 U")]
        [DataRow("255 I")]
        [DataRow("0xff I")]
        [DataRow("0xff U")]
        public void IntegerIsNotParsedWithSeparatedSignFlag(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as LiteralToken<BigInteger>).ToList();

            Assert.AreEqual(0, tokens.Count, "no tokens are expected");
        }
    }
}
