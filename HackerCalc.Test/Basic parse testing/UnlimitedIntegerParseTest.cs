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
    [TestCategory("Literal token parse")]
    [TestClass]
    public class UnlimitedIntegerParseTest
    {
        [DataTestMethod]
        [DataRow("0", "0" )]
        [DataRow("255", "255" )]
        public void DecUnlimitedIntegerValueIsParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("0x00", "0" )]
        [DataRow("0xff", "255" )]
        [DataRow("0XFF", "255" )]
        public void HexUnlimitedIntegerValueIsParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("B110", "6" )]
        [DataRow("b01010101", "85" )]
        public void BinUnlimitedIntegerValueIsParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("-b1010", "-10" )]
        [DataRow("-123456", "-123456" )]
        [DataRow("-0xac", "-172")]
        public void UnlimitedIntegerNegativeValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);

    }
}
