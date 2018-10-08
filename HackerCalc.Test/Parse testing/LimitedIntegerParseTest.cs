using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using System.Globalization;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(LimitedIntegerToken))]
    [TestCategory(TestCommon.BASIC_PARSE)]
    [TestCategory(TestCommon.LITERAL_TOKEN_PARSE)]
    [TestClass]
    public class LimitedIntegerParseTest
    {
        #region Binary
        [DataTestMethod]
        [DataRow("b0u4", "0")]
        [DataRow("B0U8", "0")]
        [DataRow("b0u16", "0")]
        [DataRow("B0U32", "0")]
        [DataRow("b0u64", "0")]
        [DataRow("B0U128", "0")]
        [DataRow("b1111u4", "15")]
        [DataRow("B11111111u8", "255")]
        [DataRow("b1111111111111111u16", "65535")]
        [DataRow("B11111111111111111111111111111111u32", "4294967295")]
        [DataRow("b1111111111111111111111111111111111111111111111111111111111111111u64", "18446744073709551615")]
        [DataRow("B11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111u128", "340282366920938463463374607431768211455")]
        public void BinUnsignedLimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-b1000i4", "-8")]
        [DataRow("-B10000000I8", "-128")]
        [DataRow("-b1000000000000000i16", "-32768")]
        [DataRow("-B10000000000000000000000000000000I32", "-2147483648")]
        [DataRow("-b1000000000000000000000000000000000000000000000000000000000000000i64", "-9223372036854775808")]
        [DataRow("-B10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000I128", "-170141183460469231731687303715884105728")]
        [DataRow("b111i4", "7")]
        [DataRow("B1111111i8", "127")]
        [DataRow("b111111111111111i16", "32767")]
        [DataRow("B1111111111111111111111111111111i32", "2147483647")]
        [DataRow("b111111111111111111111111111111111111111111111111111111111111111i64", "9223372036854775807")]
        [DataRow("B1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111i128", "170141183460469231731687303715884105727")]
        public void BinSignedLimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-b1u4")]
        [DataRow("-B1u8")]
        [DataRow("-b1u16")]
        [DataRow("-B1u32")]
        [DataRow("-b1u64")]
        [DataRow("-B1u128")]
        [DataRow("b10000u4")]
        [DataRow("B100000000u8")]
        [DataRow("b10000000000000000u16")]
        [DataRow("B100000000000000000000000000000000u32")]
        [DataRow("b10000000000000000000000000000000000000000000000000000000000000000u64")]
        [DataRow("B100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000u128")]
        [DataRow("-b1001i4")]
        [DataRow("-B10000001i8")]
        [DataRow("-b1000000000000001i16")]
        [DataRow("-B10000000000000000000000000000001i32")]
        [DataRow("-b1000000000000000000000000000000000000000000000000000000000000001i64")]
        [DataRow("-B10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001i128")]
        [DataRow("b1000i4")]
        [DataRow("B10000000i8")]
        [DataRow("b1000000000000000i16")]
        [DataRow("B10000000000000000000000000000000i32")]
        [DataRow("b1000000000000000000000000000000000000000000000000000000000000000i64")]
        [DataRow("B10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000i128")]
        public void BinLimitedIntegerValuesOutOfRangeAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken>(stringToParse, null);

        [DataTestMethod]
        [DataRow("b2u4")]
        [DataRow("B2i4")]
        public void BinLimitedIntegerValuesWithInvalidDigitsAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken>(stringToParse, null);
        #endregion Binary

        #region Octal
        [DataTestMethod]
        [DataRow("o0u4", "0")]
        [DataRow("O0u8", "0")]
        [DataRow("o0u16", "0")]
        [DataRow("O0u32", "0")]
        [DataRow("o0u64", "0")]
        [DataRow("O0u128", "0")]
        [DataRow("o17u4", "15")]
        [DataRow("O377u8", "255")]
        [DataRow("o177777u16", "65535")]
        [DataRow("O37777777777u32", "4294967295")]
        [DataRow("o1777777777777777777777u64", "18446744073709551615")]
        [DataRow("O3777777777777777777777777777777777777777777u128", "340282366920938463463374607431768211455")]
        public void OctUnsignedLimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-o10i4", "-8")]
        [DataRow("-O200i8", "-128")]
        [DataRow("-o100000i16", "-32768")]
        [DataRow("-O20000000000i32", "-2147483648")]
        [DataRow("-o1000000000000000000000i64", "-9223372036854775808")]
        [DataRow("-O2000000000000000000000000000000000000000000i128", "-170141183460469231731687303715884105728")]
        [DataRow("o7i4", "7")]
        [DataRow("O177i8", "127")]
        [DataRow("o77777i16", "32767")]
        [DataRow("O17777777777i32", "2147483647")]
        [DataRow("o777777777777777777777i64", "9223372036854775807")]
        [DataRow("O1777777777777777777777777777777777777777777i128", "170141183460469231731687303715884105727")]
        public void OctSignedLimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-o1u4")]
        [DataRow("-O1u8")]
        [DataRow("-o1u16")]
        [DataRow("-O1u32")]
        [DataRow("-o1u64")]
        [DataRow("-O1u128")]
        [DataRow("o20u4")]
        [DataRow("O400u8")]
        [DataRow("o200000u16")]
        [DataRow("O40000000000u32")]
        [DataRow("o2000000000000000000000u64")]
        [DataRow("O4000000000000000000000000000000000000000000u128")]
        [DataRow("-o11i4")]
        [DataRow("-O201i8")]
        [DataRow("-o100001i16")]
        [DataRow("-O20000000001i32")]
        [DataRow("-o1000000000000000000001i64")]
        [DataRow("-O2000000000000000000000000000000000000000001i128")]
        [DataRow("o10i4")]
        [DataRow("O200i8")]
        [DataRow("o100000i16")]
        [DataRow("O20000000000i32")]
        [DataRow("o1000000000000000000000i64")]
        [DataRow("O2000000000000000000000000000000000000000000i128")]
        public void OctLimitedIntegerValuesOutOfRangeAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken>(stringToParse, null);

        [DataTestMethod]
        [DataRow("o8u4")]
        [DataRow("O8i4")]
        public void OctLimitedIntegerValuesWithInvalidDigitsAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken>(stringToParse, null);
        #endregion Octal

        #region Decimal
        [DataTestMethod]
        [DataRow("0u4", "0")]
        [DataRow("0u8", "0")]
        [DataRow("0u16", "0")]
        [DataRow("0u32", "0")]
        [DataRow("0u64", "0")]
        [DataRow("0u128", "0")]
        [DataRow("15u4", "15")]
        [DataRow("255u8", "255")]
        [DataRow("65535u16", "65535")]
        [DataRow("4294967295u32", "4294967295")]
        [DataRow("18446744073709551615u64", "18446744073709551615")]
        [DataRow("340282366920938463463374607431768211455u128", "340282366920938463463374607431768211455")]
        public void DecUnsignedLimitedIntegerValueaAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-8i4", "-8")]
        [DataRow("-128i8", "-128")]
        [DataRow("-32768i16", "-32768")]
        [DataRow("-2147483648i32", "-2147483648")]
        [DataRow("-9223372036854775808i64", "-9223372036854775808")]
        [DataRow("-170141183460469231731687303715884105728i128", "-170141183460469231731687303715884105728")]
        [DataRow("7i4", "7")]
        [DataRow("127i8", "127")]
        [DataRow("32767i16", "32767")]
        [DataRow("2147483647i32", "2147483647")]
        [DataRow("9223372036854775807i64", "9223372036854775807")]
        [DataRow("170141183460469231731687303715884105727i128", "170141183460469231731687303715884105727")]
        public void DecSignedLimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-1u4")]
        [DataRow("-1u8")]
        [DataRow("-1u16")]
        [DataRow("-1u32")]
        [DataRow("-1u64")]
        [DataRow("-1u128")]
        [DataRow("16u4")]
        [DataRow("256u8")]
        [DataRow("65536u16")]
        [DataRow("4294967296u32")]
        [DataRow("18446744073709551616u64")]
        [DataRow("340282366920938463463374607431768211456u128")]
        [DataRow("-9i4")]
        [DataRow("-129i8")]
        [DataRow("-32769i16")]
        [DataRow("-2147483649i32")]
        [DataRow("-9223372036854775809i64")]
        [DataRow("-170141183460469231731687303715884105729i128")]
        [DataRow("8i4")]
        [DataRow("128i8")]
        [DataRow("32768i16")]
        [DataRow("2147483648i32")]
        [DataRow("9223372036854775808i64")]
        [DataRow("170141183460469231731687303715884105728i128")]
        public void DecLimitedIntegerValuesOutOfRangeAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken>(stringToParse, null);

        [DataTestMethod]
        [DataRow("au4")]
        [DataRow("Ai4")]
        public void DecLimitedIntegerValuesWithInvalidDigitsAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken>(stringToParse, null);
        #endregion Decimal

        #region Hexadecimal
        [DataTestMethod]
        [DataRow("0x0u4", "0")]
        [DataRow("0x0u8", "0")]
        [DataRow("0x0u16", "0")]
        [DataRow("0x0u32", "0")]
        [DataRow("0x0u64", "0")]
        [DataRow("0x0u128", "0")]
        [DataRow("0xfu4", "15")]
        [DataRow("0xFFu8", "255")]
        [DataRow("0xffffu16", "65535")]
        [DataRow("0xFFFFFFFFu32", "4294967295")]
        [DataRow("0xffffffffffffffffu64", "18446744073709551615")]
        [DataRow("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFu128", "340282366920938463463374607431768211455")]
        public void HexUnsignedLimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-0x8i4", "-8")]
        [DataRow("-0x80i8", "-128")]
        [DataRow("-0x8000i16", "-32768")]
        [DataRow("-0x80000000i32", "-2147483648")]
        [DataRow("-0x8000000000000000i64", "-9223372036854775808")]
        [DataRow("-0x80000000000000000000000000000000i128", "-170141183460469231731687303715884105728")]
        [DataRow("0x7i4", "7")]
        [DataRow("0x7Fi8", "127")]
        [DataRow("0x7fffi16", "32767")]
        [DataRow("0x7fffffffi32", "2147483647")]
        [DataRow("0x7fffffffffffffffi64", "9223372036854775807")]
        [DataRow("0x7fffffffffffffffffffffffffffffffi128", "170141183460469231731687303715884105727")]
        public void HexSignedLimitedIntegerValuesAreParsedCorrectly(string stringToParse, string expectedValue)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken, BigInteger>(stringToParse, BigInteger.Parse(expectedValue));

        [DataTestMethod]
        [DataRow("-0x1u4")]
        [DataRow("-0x1u8")]
        [DataRow("-0x1u16")]
        [DataRow("-0x1u32")]
        [DataRow("-0x1u64")]
        [DataRow("-0x1u128")]
        [DataRow("0x10u4")]
        [DataRow("0x100u8")]
        [DataRow("0x10000u16")]
        [DataRow("0x100000000u32")]
        [DataRow("0x10000000000000000u64")]
        [DataRow("0x100000000000000000000000000000000u128")]
        [DataRow("-0x9i4")]
        [DataRow("-0x81i8")]
        [DataRow("-0x8001i16")]
        [DataRow("-0x80000001i32")]
        [DataRow("-0x8000000000000001i64")]
        [DataRow("-0x80000000000000000000000000000001i128")]
        [DataRow("0x8i4")]
        [DataRow("0x80i8")]
        [DataRow("0x8000i16")]
        [DataRow("0x80000000i32")]
        [DataRow("0x8000000000000000i64")]
        [DataRow("0x80000000000000000000000000000000i128")]
        public void HexLimitedIntegerValuesOutOfRangeAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken>(stringToParse, null);

        [DataTestMethod]
        [DataRow("0xgu4")]
        [DataRow("0XGi4")]
        public void HexLimitedIntegerValuesWithInvalidDigitsAreNotParsed(string stringToParse)
            => TestCommon.LiteralTokensAreParsedCorrectly<LimitedIntegerToken>(stringToParse, null);
        #endregion Hexadecimal
    }
}
