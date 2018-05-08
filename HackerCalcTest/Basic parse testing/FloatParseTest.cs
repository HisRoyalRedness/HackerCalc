using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(FloatToken))]
    [TestCategory("Basic parse")]
    [TestCategory("Literal token parse")]
    [TestClass]
    public class FloatParseTest
    {
        [DataTestMethod]
        [DataRow("1.0", "1.0" )]
        public void TrueFloatValueIsParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<FloatToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("1F", "1.0" )]
        public void TypedFloatValueIsParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<FloatToken>(stringToParse, expectedTokenStr);
    }
}
