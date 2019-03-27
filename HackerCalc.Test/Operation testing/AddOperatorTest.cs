using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_OPERATION)]
    [TestCategory(TestCommon.ADD_OPERATION)]
    [TestClass]
    public class AddOperatorTest
    {

        [TestMethod]
        public void AllAddOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l,r) => l + r, "added");

        [DataTestMethod]
        // Limited
        [DataRow("2u4+2019-01-01 10:00:00", "2019-01-01 10:00:02")]
        [DataRow("2u4+3.1", "5.1f")]
        [DataRow("2u4+3u4", "5u4")]
        [DataRow("2u4+10:00:00", "10:00:02")]
        [DataRow("2u4+10hrs", "10hrs 2 sec")]
        [DataRow("2u4+3", "5u4")]
        // Unlimited
        [DataRow("2+2019-01-01 10:00:00", "2019-01-01 10:00:02")]
        [DataRow("2+3.1", "5.1f")]
        [DataRow("2+3u4", "5u4")]
        [DataRow("2+10:00:00", "10:00:02")]
        [DataRow("2+10hrs", "10hrs 2 sec")]
        [DataRow("2+3", "5")]
        // Float
        //[DataRow("2.1+2019-01-01 10:00:00", "2019-01-01 10:00:02.1")]
        [DataRow("2.1+3.1", "5.2f")]
        [DataRow("2.1+3u4", "5.1f")]
        //[DataRow("2.1+10:00:00", "10:00:02.1")]
        [DataRow("2.1+10hrs", "10hrs 2.1 sec")]
        [DataRow("2.1+3", "5.1f")]
        public void AddOperandTypeTests(string actual, string expected)
            => actual.Evaluate().Should().Be(expected.Evaluate());

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Add (+)             LimitedInteger      Date, Float, LimitedInteger, Time, Timespan, UnlimitedInteger
                                UnlimitedInteger    Date, Float, LimitedInteger, Time, Timespan, UnlimitedInteger
                                Float               Date, Float, LimitedInteger, Time, Timespan, UnlimitedInteger
                                Date                Float, LimitedInteger, Time, Timespan, UnlimitedInteger
                                Time                Date, Float, LimitedInteger, Time, Timespan, UnlimitedInteger
                                Timespan            Date, Float, LimitedInteger, Time, Timespan, UnlimitedInteger
            */
            { DataType.LimitedInteger, new HashSet<DataType> { DataType.Date, DataType.Float, DataType.LimitedInteger, DataType.Time, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.UnlimitedInteger, new HashSet<DataType> { DataType.Date, DataType.Float, DataType.LimitedInteger, DataType.Time, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.Float, new HashSet<DataType> { DataType.Date, DataType.Float, DataType.LimitedInteger, DataType.Time, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.Date, new HashSet<DataType> { DataType.Float, DataType.LimitedInteger, DataType.Time, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.Time, new HashSet<DataType> { DataType.Date, DataType.Float, DataType.LimitedInteger, DataType.Time, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.Timespan, new HashSet<DataType> { DataType.Date, DataType.Float, DataType.LimitedInteger, DataType.Time, DataType.Timespan, DataType.UnlimitedInteger } },
        };


    }
}