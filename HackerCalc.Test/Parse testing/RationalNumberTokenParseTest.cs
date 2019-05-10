using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using System.Globalization;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(RationalNumberToken))]
    [TestCategory(TestCommon.BASIC_PARSE)]
    [TestCategory(TestCommon.LITERAL_TOKEN_PARSE)]
    [TestClass]
    public class RationalNumberTokenParseTest
    {
        [DataTestMethod]
        [DataRow("B0", "0")]
        [DataRow("B110", "6")]
        [DataRow("b01010101", "85")]
        public void BinRationalNumberValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<RationalNumberToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("o0", "0")]
        [DataRow("O125", "85")]
        [DataRow("o125", "85")]
        public void OctRationalNumberValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<RationalNumberToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("0", "0")]
        [DataRow("255", "255")]
        public void DecRationalNumberValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<RationalNumberToken>(stringToParse, expectedTokenStr);

        [DataTestMethod]
        [DataRow("0x00", "0")]
        [DataRow("0xff", "255")]
        [DataRow("0XFF", "255")]
        public void HexRationalNumberValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<RationalNumberToken>(stringToParse, expectedTokenStr);


        [DataTestMethod]
        [DataRow("-b1010", "-10")]
        [DataRow("-o377", "-255")]
        [DataRow("-123456", "-123456")]
        [DataRow("-0xac", "-172")]
        public void RationalNumberNegativeValuesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
            => TestCommon.LiteralTokensAreParsedCorrectly<RationalNumberToken>(stringToParse, expectedTokenStr);
    }
}
