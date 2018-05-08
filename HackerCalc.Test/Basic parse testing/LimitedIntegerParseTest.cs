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

        //[DataRow("16u4", "")]
        //[DataRow("256u8", "")]
        //[DataRow("65536u16", "")]
        //[DataRow("4294967296u32", "")]
        //[DataRow("18446744073709551616u64", "")]
        //[DataRow("340282366920938463463374607431768211456u128", "")]
        //[DataRow("-9i4", "")]
        //[DataRow("8i4", "")]
        //[DataRow("-129i8", "")]
        //[DataRow("128i8", "")]
        //[DataRow("-32769i16", "")]
        //[DataRow("32768i16", "")]
        //[DataRow("-2147483649i32", "")]
        //[DataRow("2147483648i32", "")]
        //[DataRow("-9223372036854775809i64", "")]
        //[DataRow("9223372036854775808i64", "")]
        //[DataRow("-170141183460469231731687303715884105729i128", "")]
        //[DataRow("170141183460469231731687303715884105728i128", "")]

        [DataTestMethod]
        [DataRow("0u4", "0" )]
        [DataRow("15u4", "15" )]
        [DataRow("0u8", "0" )]
        [DataRow("255u8", "255" )]
        [DataRow("0u16", "0" )]
        [DataRow("65535u16", "65535" )]
        [DataRow("0u32", "0" )]
        [DataRow("4294967295u32", "4294967295" )]
        [DataRow("0u64", "0" )]
        [DataRow("18446744073709551615u64", "18446744073709551615" )]
        [DataRow("0u128", "0" )]
        [DataRow("340282366920938463463374607431768211455u128", "340282366920938463463374607431768211455" )]
        [DataRow("-8i4", "-8" )]
        [DataRow("7i4", "7" )]
        [DataRow("-128i8", "-128" )]
        [DataRow("127i8", "127" )]
        [DataRow("-32768i16", "-32768" )]
        [DataRow("32767i16", "32767" )]
        [DataRow("-2147483648i32", "-2147483648" )]
        [DataRow("2147483647i32", "2147483647" )]
        [DataRow("-9223372036854775808i64", "-9223372036854775808" )]
        [DataRow("9223372036854775807i64", "9223372036854775807" )]
        [DataRow("-170141183460469231731687303715884105728i128", "-170141183460469231731687303715884105728" )]
        [DataRow("170141183460469231731687303715884105727i128", "170141183460469231731687303715884105727" )]
        public void LimitedIntegerValuesWithinRangeAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));
    }
}
