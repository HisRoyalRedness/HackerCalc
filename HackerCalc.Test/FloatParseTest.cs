using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class FloatParseTest
    {
        [DataTestMethod]
        [DataRow("1.0")]
        public void TrueFloatValueIsParsedCorrectly(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as FloatToken).Where(t => t != null).ToList();

            Assert.AreEqual(1, tokens.Count, "only a single token is expected");
            Assert.AreEqual(1.0, tokens.First()?.TypedValue);
        }

        [DataTestMethod]
        [DataRow("1F")]
        public void TypedFloatValueIsParsedCorrectly(string input)
        {
            var tokens = Parser.ParseExpression(input).Select(t => t as FloatToken).Where(t => t != null).ToList();

            Assert.AreEqual(1, tokens.Count, "only a single token is expected");
            Assert.AreEqual(1.0, tokens.First()?.TypedValue);
        }
    }
}
