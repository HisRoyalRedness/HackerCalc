using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_OPERATION)]
    [TestCategory(TestCommon.ROOT_OPERATION)]
    [TestClass]
    public class RootOperatorTest
    {
        [TestMethod]
        public void AllRootOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l, r) => TestCommon.Operate(null, OperatorType.Root, l, r), "root with index");

        [DataTestMethod]
        // Limited
        [TypedDataRow("8u4 // 3u4",                     "2u4",                      DataType.LimitedInteger)]
        [TypedDataRow("8u4 // 3.1",                     "1.95577707267f",           DataType.IrrationalNumber)]
        [TypedDataRow("8u4 // 2019-01-23 12:34:56",     null,                       null)]
        [TypedDataRow("8u4 // 12:34:56",                null,                       null)]
        [TypedDataRow("8u4 // 10hrs",                   null,                       null)]
        // Rational
        [TypedDataRow("8.3 // 3u4",                     "2u4",                      DataType.LimitedInteger)]
        [TypedDataRow("8.3 // 3.1",                     "1.95577707267f",           DataType.IrrationalNumber)]
        [TypedDataRow("8.3 // 2019-01-23 12:34:56",     null,                       null)]
        [TypedDataRow("8.3 // 12:34:56",                null,                       null)]
        [TypedDataRow("8.3 // 10hrs",                     null,                       null)]
        //// Irrational
        //[TypedDataRow("8.1 // 3u4",                     "2.00829885025f",           DataType.IrrationalNumber)]
        //[TypedDataRow("8.1 // 3.1",                     "1.96363011283f",           DataType.IrrationalNumber)]
        //[TypedDataRow("8.1 // 2019-01-23 12:34:56",     null,                       null)]
        //[TypedDataRow("8.1 // 12:34:56",                null,                       null)]
        //[TypedDataRow("8.1 // 10hrs",                   null,                       null)]
        // Date
        [TypedDataRow("2019-01-23 12:34:56 // 3u4",     null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 // 3.1",     null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 // now",     null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 // 12:34:56",null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 // 10hrs",   null,                       null)]
        // Time
        [TypedDataRow("12:34:56 // 3u4",                null,                       null)]
        [TypedDataRow("12:34:56 // 3.1",                null,                       null)]
        [TypedDataRow("12:34:56 // 2019-01-23 12:34:56",null,                       null)]
        [TypedDataRow("12:34:56 // 12:34:56",           null,                       null)]
        [TypedDataRow("12:34:56 // 10hrs",              null,                       null)]
        // Timespan 
        [TypedDataRow("12h34m56s // 3u4",               null,                       null)]
        [TypedDataRow("12h34m56s // 3.1",               null,                       null)]
        [TypedDataRow("12h34m56s // 2019-01-23 12:34:56",null,                      null)]
        [TypedDataRow("12h34m56s // 12:34:56",          null,                       null)]
        [TypedDataRow("12h34m56s // 10hrs",             null,                       null)]
        public void RootOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr, new Configuration() { AllowMultidayTimes = true });

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Root (//)           LimitedInteger      LimitedInteger, RationalNumber, IrrationalNumber
                                RationalNumber      LimitedInteger, RationalNumber, IrrationalNumber
                                IrrationalNumber    LimitedInteger, RationalNumber, IrrationalNumber
            */
            { DataType.LimitedInteger,      new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.RationalNumber,      new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.IrrationalNumber,    new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.Date,                new HashSet<DataType> { } },
            { DataType.Time,                new HashSet<DataType> { } },
            { DataType.Timespan,            new HashSet<DataType> { } },
        };

        [TestMethod]
        public void TestFractionalRoots()
        {
            "27f//(1f/3f)".Evaluate<IrrationalNumberType>().Value.Should().Be(19683);
            "27//(1f/3f)".Evaluate<IrrationalNumberType>().Value.Should().Be(19683);
        }

    }
}