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
    public class LimitedIntegerParseTest
    {
        [DataTestMethod]
        [DataRow("0u4", "0" )]
        [DataRow("255u32", "255" )]
        public void DecLimitedIntegerValueIsParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("0x00u4", "0")]
        [DataRow("0xffu16", "255")]
        [DataRow("0XFFu32", "255")]
        public void HexLimitedIntegerValueIsParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("B110u4", "6")]
        [DataRow("b01010101u128", "85")]
        public void BinLimitedIntegerValueIsParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-b1010i32", "-10")]
        [DataRow("-123456i64", "-123456")]
        [DataRow("-0xaci16", "-172")]
        public void LimitedIntegerNegativeValuesAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue), true);

    }
}
