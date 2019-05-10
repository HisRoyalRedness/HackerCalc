using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;
using System.Numerics;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.DATA_MAPPING)]
    [TestClass]
    public class MinMaxMapTest
    {
        [TestMethod]
        public void AllBitWidthsHaveAMinAndMax()
        {
            foreach(var signAndBitwidth in EnumExtensions.GetEnumCollection<IntegerBitWidth>()
                .SelectMany(bw => new[] { false, true }.Select(s => new BitWidthAndSignPair(bw, s))))
            {
                var isSigned = signAndBitwidth.IsSigned;
                var bitWidth = signAndBitwidth.BitWidth;
                var expectedMin = signAndBitwidth.BitWidth == IntegerBitWidth.Unlimited
                    ? 0
                    : (isSigned ? BigInteger.Pow(new BigInteger(2), (int)bitWidth - 1) * -1 : 0);
                var expectedMax = signAndBitwidth.BitWidth == IntegerBitWidth.Unlimited
                    ? 0
                    : (isSigned ? BigInteger.Pow(new BigInteger(2), (int)bitWidth - 1) -1 : BigInteger.Pow(new BigInteger(2), (int)bitWidth) - 1);

                MinAndMaxMap.Instance.ContainsKey(signAndBitwidth).Should().BeTrue();
                var minMax = MinAndMaxMap.Instance[signAndBitwidth];

                minMax.Min.Should().Be(expectedMin, $"a {(isSigned ? "signed" : "unsigned")} {(int)bitWidth}-bit integer should have a minimum of {expectedMin}");
                minMax.Max.Should().Be(expectedMax, $"a {(isSigned ? "signed" : "unsigned")} {(int)bitWidth}-bit integer should have a maximum of {expectedMax}");
            }
        }
    }
}
