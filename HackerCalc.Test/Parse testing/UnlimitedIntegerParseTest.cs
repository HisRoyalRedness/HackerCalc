using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using System.Globalization;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(UnlimitedIntegerToken))]
    [TestCategory(TestCommon.BASIC_PARSE)]
    [TestCategory(TestCommon.LITERAL_TOKEN_PARSE)]
    [TestClass]
    public class UnlimitedIntegerParseTest
    {
        [DataTestMethod]
        [DataRow("B0", "0")]
        [DataRow("B110", "6")]
        [DataRow("b01010101", "85")]
        public void BinUnlimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("o0", "0")]
        [DataRow("O125", "85")]
        [DataRow("o125", "85")]
        public void OctUnlimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("0", "0")]
        [DataRow("255", "255")]
        public void DecUnlimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("0x00", "0")]
        [DataRow("0xff", "255")]
        [DataRow("0XFF", "255")]
        public void HexUnlimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);


        [DataTestMethod]
        [DataRow("-b1010", "-10")]
        [DataRow("-o377", "-255")]
        [DataRow("-123456", "-123456")]
        [DataRow("-0xac", "-172")]
        public void UnlimitedIntegerNegativeValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<UnlimitedIntegerToken>(stringToParse, expectedTokenStr);
    }
}
