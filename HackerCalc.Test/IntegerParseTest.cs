using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class IntegerParseTest
    {
        [DataTestMethod]
        [DataRow("255")]
        [DataRow("255U")]
        [DataRow("255I")]
        public void DecimalIntegerIsParsedCorrectly(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as IntegerToken).ToList();

            Assert.AreEqual(1, tokens.Count, "only a single token is expected");
            Assert.AreEqual(255, tokens.First()?.TypedValue);
        }

        [DataTestMethod]
        [DataRow("0xff")]
        [DataRow("0XFF")]
        [DataRow("0xffU")]
        [DataRow("0xffI")]
        public void HexadecimalIntegerIsParsedCorrectly(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as IntegerToken).ToList();

            Assert.AreEqual(1, tokens.Count, "only a single token is expected");
            Assert.AreEqual(255, tokens.First()?.TypedValue);
        }

        [DataTestMethod]
        [DataRow("255")]
        [DataRow("255U")]
        [DataRow("255I")]
        [DataRow("0xff")]
        [DataRow("0xffI")]
        [DataRow("0xffU")]
        public void IntegerIsParsedWithAttachedSignFlag(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as IntegerToken).ToList();

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
            var tokens = Parser.ParseExpression(input).Select(t => t as IntegerToken).ToList();

            Assert.AreEqual(0, tokens.Count, "no tokens are expected");
        }

        [DataTestMethod]
        [DataRow("255")]
        [DataRow("255I")]
        [DataRow("255i")]
        [DataRow("0xff")]
        [DataRow("0xffI")]
        [DataRow("0xffi")]
        public void SignedIntegerAreParsedCorrectly(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as IntegerToken).ToList();

            Assert.AreEqual(1, tokens.Count, "only a single token is expected");
            Assert.AreEqual(255, tokens.First()?.TypedValue);
            Assert.AreEqual(true, tokens.First()?.IsSigned);
        }

        [DataTestMethod]
        [DataRow("255U")]
        [DataRow("255u")]
        [DataRow("0xffU")]
        [DataRow("0xffu")]
        public void UnsignedIntegerAreParsedCorrectly(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as IntegerToken).ToList();

            Assert.AreEqual(1, tokens.Count, "only a single token is expected");
            Assert.AreEqual(255, tokens.First()?.TypedValue);
            Assert.AreEqual(false, tokens.First()?.IsSigned);
        }
    }
}
