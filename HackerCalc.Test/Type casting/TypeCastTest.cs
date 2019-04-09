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
    [TestClass]
    public class TypeCastTest
    {
        [TestMethod]
        public void AllSupportedTypeCastsWorkCorrectly()
        {
            foreach(var cast in DataMapper.AllTypeCasts.Value)
            {
                var dataValue = TestCommon.MakeDataType(cast.Key);
                foreach (var val in cast.Value)
                {
                    ((Action)(() => dataValue.CastTo(val, null))).Should().NotThrow($"a cast from {cast.Key} to {val} should be supported");
                    dataValue.CastTo(val, null).DataType.Should().Be(val);
                }
            }
        }

        //713 Otherwise, if both operands have signed integer types or both have unsigned integer types, 
        //    the operand with the type of lesser integer conversion rank is converted to the type of the 
        //    operand with greater rank.
        [DataTestMethod]
        [DataRow("1u4", "1u8", "2u8")]
        [DataRow("1u8", "1u4", "2u8")]
        [DataRow("1u8", "1u16", "2u16")]
        [DataRow("1u16", "1u8", "2u16")]
        [DataRow("1u4", "1u128", "2u128")]
        [DataRow("1u128", "1u4", "2u128")]
        public void LimitedIntegerTypesWithTheSameSign(string a, string b, string expected)
            => TypeCastTest.LimitedIntegerTypesBitwidthCasts(a, b, expected);


        //715 Otherwise, if the type of the operand with signed integer type can represent all of the 
        //    values of the type of the operand with unsigned integer type, then the operand with unsigned 
        //    integer type is converted to the type of the operand with signed integer type.
        [DataTestMethod]
        [DataRow("1i4", "6u128", "7i4")]
        [DataRow("1i8", "126u128", "127i8")]
        [DataRow("1i16", "32766u128", "32767i16")]
        [DataRow("1i32", "2147483646u128", "2147483647i32")]
        [DataRow("1i64", "9223372036854775806u128", "9223372036854775807i64")]
        [DataRow("1i128", "170141183460469231731687303715884105726u128", "170141183460469231731687303715884105727i128")]
        [DataRow("6u128", "1i4", "7i4")]
        [DataRow("126u128", "1i8", "127i8")]
        [DataRow("32766u128", "1i16", "32767i16")]
        [DataRow("2147483646u128", "1i32", "2147483647i32")]
        [DataRow("9223372036854775806u128", "1i64", "9223372036854775807i64")]
        [DataRow("170141183460469231731687303715884105726u128", "1i128", "170141183460469231731687303715884105727i128")]
        public void LimitedIntegerTypesRangeWithinSignedIntRange(string a, string b, string expected)
            => TypeCastTest.LimitedIntegerTypesBitwidthCasts(a, b, expected);

        //716 Otherwise, both operands are converted to the unsigned integer type corresponding to the 
        //    type of the operand with signed integer type.
        [DataTestMethod]
        [DataRow("1i4", "8u8", "9u8")]
        [DataRow("8u8", "1i4", "9u8")]
        public void LimitedIntegerTypesConvertToUnsignedWithBiggestBitwidth(string a, string b, string expected)
            => TypeCastTest.LimitedIntegerTypesBitwidthCasts(a, b, expected);

        static void LimitedIntegerTypesBitwidthCasts(string a, string b, string expected)
        {
            var intA = (LimitedIntegerType)TestCommon.MakeDataType(DataType.LimitedInteger, a);
            var intB = (LimitedIntegerType)TestCommon.MakeDataType(DataType.LimitedInteger, b);
            var intExp = (LimitedIntegerType)TestCommon.MakeDataType(DataType.LimitedInteger, expected);

            var actual = (LimitedIntegerType)TestCommon.Operate(null, OperatorType.Add, intA, intB);
            actual.Value.Should().Be(intExp.Value);
            actual.SignAndBitWidth.Should().Be(intExp.SignAndBitWidth);
        }
    }
}

