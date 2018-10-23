using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.DATA_MAPPING)]
    [TestCategory(TestCommon.CALC_ENGINE)]
    [TestClass]
    public class LiteralTokenMapTest
    {
        [TestMethod]
        public void ConvertDateTokenToDateType() 
            => ConvertToDataType<DateType>(LiteralTokenType.Date);

        [TestMethod]
        public void ConvertFloatTokenToFloatType() 
            => ConvertToDataType<FloatType>(LiteralTokenType.Float);

        [TestMethod]
        public void ConvertLimitedIntegerTokenToLimitedIntegerType() 
            => ConvertToDataType<LimitedIntegerType>(LiteralTokenType.LimitedInteger);

        [TestMethod]
        public void ConvertTimeTokenToTimeType()
            => ConvertToDataType<TimeType>(LiteralTokenType.Time);

        [TestMethod]
        public void ConvertTimespanTokenToTimespanType()
            => ConvertToDataType<TimespanType>(LiteralTokenType.Timespan);

        [TestMethod]
        public void ConvertUnlimitedIntegerTokenToUnlimitedIntegerType()
            => ConvertToDataType<UnlimitedIntegerType>(LiteralTokenType.UnlimitedInteger);

        static TDataType ConvertToDataType<TDataType>(LiteralTokenType tokenType)
            where TDataType : class, IDataType<DataType>
        {
            ICalcEngine calcEngine = new CalcEngine();
            ILiteralToken literalToken = TestCommon.MakeLiteralToken(tokenType);
            var dataType = calcEngine.ConvertToDataType(literalToken);

            dataType.Should().NotBeNull();
            dataType.Should().BeOfType<TDataType>();
            var typedDataType = dataType as TDataType;
            typedDataType.Should().NotBeNull();
            return typedDataType;
        }
    }
}
