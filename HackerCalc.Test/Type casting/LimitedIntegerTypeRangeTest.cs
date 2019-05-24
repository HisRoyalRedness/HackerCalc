using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.TYPE_CASTING)]
    [TestCategory(TestCommon.DATA_MAPPING)]
    [TestCategory(nameof(LimitedIntegerType))]
    [TestClass]
    public class LimitedIntegerTypeRangeTest
    {
        [DataTestMethod]
        [DataRow("0", "u", "4")]
        [DataRow("15", "u", "4")]
        [DataRow("255", "u", "8")]
        [DataRow("65535", "u", "16")]
        [DataRow("4294967295", "u", "32")]
        [DataRow("18446744073709551615", "u", "64")]
        [DataRow("340282366920938463463374607431768211455", "u", "128")]
        [DataRow("34028236692093846346337460743176821145523123", "u", "0")]
        [DataRow("-8", "i", "4")]
        [DataRow("-128", "i", "8")]
        [DataRow("-32768", "i", "16")]
        [DataRow("-2147483648", "i", "32")]
        [DataRow("-9223372036854775808", "i", "64")]
        [DataRow("-170141183460469231731687303715884105728", "i", "128")]
        [DataRow("-1701411834604692317316873037158841057284332423", "i", "0")]
        public void LimitedIntegerIsCreatedWithinRange(string value, string sign, string bitwidth)
            => TestLimitedIntegerRange(BigInteger.Parse(value), new BitWidthAndSignPair(LimitedIntegerToken.ParseBitWidth(bitwidth), sign.ToLower() == "i"));

        [DataTestMethod]
        [DataRow("340282366920938463463374607431768211456")]
        [DataRow("-170141183460469231731687303715884105729")]
        public void OutOfRangeLimitedIntegers(string value)
            => new Action(() => LimitedIntegerType.CreateLimitedIntegerType(BigInteger.Parse(value), false, null)).Should().Throw<TypeConversionException>();

        static void TestLimitedIntegerRange(BigInteger value, BitWidthAndSignPair signAndBitwidth)
        {
            var ltdInt = LimitedIntegerType.CreateLimitedIntegerType(value, true, null);
            ltdInt.Value.Should().Be(value);
            ltdInt.IsSigned.Should().Be(signAndBitwidth.IsSigned);
            ltdInt.BitWidth.Should().Be(signAndBitwidth.BitWidth);
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

