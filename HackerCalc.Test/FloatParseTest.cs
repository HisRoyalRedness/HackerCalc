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
            var token = Parser.ParseExpression(input) as FloatToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(1.0, token.TypedValue);
        }

        [DataTestMethod]
        [DataRow("1F")]
        public void TypedFloatValueIsParsedCorrectly(string input)
        {
            var token = Parser.ParseExpression(input) as FloatToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(1.0, token.TypedValue);
        }
    }
}
