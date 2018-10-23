using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    public partial class TestCommonTest
    {
        [TestMethod]
        public void MakeDataTypeCoversAllDataTypes()
        {
            foreach (var dataType in EnumExtensions.GetEnumCollection<DataType>())
            {
                Action act = () => TestCommon.MakeDataType(dataType);
                act.Should().NotThrow<TestOperationException>();
            }
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("2018-10-23 14:24:41", "2018-10-23 14:24:41")]
        public void MakeDataType_Date(string tokenValue, string expectedValue)
        {
            var dataType = MakeDataType<DateType>(DataType.Date, tokenValue);
            if (string.IsNullOrWhiteSpace(tokenValue))
                dataType.Value.Should().BeCloseTo(DateTime.Now, 50);
            else
                dataType.Value.Should().Be(DateTime.Parse(expectedValue));
        }

        [DataTestMethod]
        [DataRow("", "1.0")]
        [DataRow("1.234", "1.234")]
        [DataRow("-1.234", "-1.234")]
        public void MakeDataType_Float(string tokenValue, string expectedValue)
        {
            var dataType = MakeDataType<FloatType>(DataType.Float, tokenValue);
            dataType.Value.Should().Be(double.Parse(expectedValue));
        }

        [DataTestMethod]
        [DataRow("", "1", "i", "32")]
        [DataRow("1u16", "1", "u", "16")]
        [DataRow("-65i64", "-65", "i", "64")]
        public void MakeDataType_LimitedInteger(string tokenValue, string expectedValue, string sign, string bitwidth)
        {
            var dataType = MakeDataType<LimitedIntegerType>(DataType.LimitedInteger, tokenValue);
            dataType.Value.Should().Be(BigInteger.Parse(expectedValue));
            dataType.IsSigned.Should().Be(sign == "i");
            dataType.BitWidth.Should().Be(LimitedIntegerToken.ParseBitWidth(bitwidth));
        }

        [DataTestMethod]
        [DataRow("", "00:00:01")]
        [DataRow("12:34:56", "12:34:56")]
        public void MakeDataType_Time(string tokenValue, string expectedValue)
        {
            var dataType = MakeDataType<TimeType>(DataType.Time, tokenValue);
            dataType.Value.Should().Be(TimeSpan.Parse(expectedValue));
        }

        [DataTestMethod]
        [DataRow("", "00:00:01")]
        [DataRow("12:34:56", "12:34:56")]
        public void MakeDataType_Timespan(string tokenValue, string expectedValue)
        {
            var dataType = MakeDataType<TimespanType>(DataType.Timespan, tokenValue);
            dataType.Value.Should().Be(TimeSpan.Parse(expectedValue));
        }

        [DataTestMethod]
        [DataRow("", "1")]
        [DataRow("123456", "123456")]
        [DataRow("-48", "-48")]
        public void MakeDataType_UnlimitedInteger(string tokenValue, string expectedValue)
        {
            var dataType = MakeDataType<UnlimitedIntegerType>(DataType.UnlimitedInteger, tokenValue);
            dataType.Value.Should().Be(BigInteger.Parse(expectedValue));
        }

        static internal TDataType MakeDataType<TDataType>(DataType dataType, string value = "")
            where TDataType : class, IDataType<DataType>
        {
            var dataTypeInst = string.IsNullOrWhiteSpace(value)
                ? TestCommon.MakeDataType(dataType)
                : TestCommon.MakeDataType(dataType, value);
            dataTypeInst.Should().NotBeNull();
            dataTypeInst.Should().BeOfType<TDataType>();
            var typedDataType = dataTypeInst as TDataType;
            typedDataType.Should().NotBeNull();
            return typedDataType;
        }
    }
}

