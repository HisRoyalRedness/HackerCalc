using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(LimitedIntegerType))]
    [TestClass]
    public class LimitedIntegerTypeNormaliseTest
    {
        [DataTestMethod]
        [DataRow("0u4", "0")]
        [DataRow("15u4", "15")]
        [DataRow("-8i4", "-8")]
        [DataRow("7i4", "7")]
        [DataRow("0u8", "0")]
        [DataRow("255u8", "255")]
        [DataRow("-128i8", "-128")]
        [DataRow("127i8", "127")]
        [DataRow("0u16", "0")]
        [DataRow("65535u16", "65535")]
        [DataRow("-32768i16", "-32768")]
        [DataRow("32767i16", "32767")]
        [DataRow("0u32", "0")]
        [DataRow("4294967295u32", "4294967295")]
        [DataRow("-2147483648i32", "-2147483648")]
        [DataRow("2147483647i32", "2147483647")]
        [DataRow("0u64", "0")]
        [DataRow("18446744073709551615u64", "18446744073709551615")]
        [DataRow("-9223372036854775808i64", "-9223372036854775808")]
        [DataRow("9223372036854775807i64", "9223372036854775807")]
        [DataRow("0u128", "0")]
        [DataRow("340282366920938463463374607431768211455u128", "340282366920938463463374607431768211455")]
        [DataRow("-170141183460469231731687303715884105728i128", "-170141183460469231731687303715884105728")]
        [DataRow("170141183460469231731687303715884105727i128", "170141183460469231731687303715884105727")]
        public void LimitedIntegerTypeNormaliseTestsWithinRange(string value, string expectedStr)
            => LimitedIntegerTypeNormaliseTests(value, expectedStr);

        [DataTestMethod]
        [DataRow("-1u4", "15")]
        [DataRow("17u4", "1")]
        [DataRow("-9i4", "7")]
        [DataRow("9i4", "-7")]
        [DataRow("-1u8", "255")]
        [DataRow("257u8", "1")]
        [DataRow("-129i8", "127")]
        [DataRow("129i8", "-127")]
        [DataRow("-1u16", "65535")]
        [DataRow("65537u16", "1")]
        [DataRow("-32769i16", "32767")]
        [DataRow("32769i16", "-32767")]
        [DataRow("-1u32", "4294967295")]
        [DataRow("4294967297u32", "1")]
        [DataRow("-2147483649i32", "2147483647")]
        [DataRow("2147483649i32", "-2147483647")]
        [DataRow("-1u64", "18446744073709551615")]
        [DataRow("18446744073709551617u64", "1")]
        [DataRow("-9223372036854775809i64", "9223372036854775807")]
        [DataRow("9223372036854775809i64", "-9223372036854775807")]
        [DataRow("-1u128", "340282366920938463463374607431768211455")]
        [DataRow("340282366920938463463374607431768211457u128", "1")]
        [DataRow("-170141183460469231731687303715884105729i128", "170141183460469231731687303715884105727")]
        [DataRow("170141183460469231731687303715884105729i128", "-170141183460469231731687303715884105727")]
        public void LimitedIntegerTypeNormaliseTestsOutOfRangeRange(string value, string expectedStr)
            => LimitedIntegerTypeNormaliseTests(value, expectedStr);

        static void LimitedIntegerTypeNormaliseTests(string value, string expectedStr)
        {
            var actual = (LimitedIntegerType)TestCommon.MakeDataType(DataType.LimitedInteger, value);
            var expected = BigInteger.Parse(expectedStr);

            actual.Value.Should().Be(expected);

        }

        //Unsigned
        //Bits  Min                                       Max                                       Mask
        //4     0                                         15                                        15
        //8     0                                         255                                       255
        //16    0                                         65535                                     65535
        //32    0                                         4294967295                                4294967295
        //64    0                                         18446744073709551615                      18446744073709551615
        //128   0                                         340282366920938463463374607431768211455   340282366920938463463374607431768211455

        //Signed
        //Bits  Min                                       Max                                       Mask
        //4     -8                                        7                                         7
        //8     -128                                      127                                       127
        //16    -32768                                    32767                                     32767
        //32    -2147483648                               2147483647                                2147483647
        //64    -9223372036854775808                      9223372036854775807                       9223372036854775807
        //128   -170141183460469231731687303715884105728  170141183460469231731687303715884105727   170141183460469231731687303715884105727

    }
}

