using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_OPERATION)]
    [TestClass]
    public class SubtractOperatorTest
    {

        [TestMethod]
        public void AllSubtractOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l, r) => l - r, "subtraced");

        [DataTestMethod]
        [DataRow("2u4+3u4", "7u4")]
        public void SubtractOperandTests(string actual, string expected)
        {

        }

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Subtract (-)        LimitedInteger      Float, LimitedInteger, Timespan, UnlimitedInteger
                                UnlimitedInteger    Float, LimitedInteger, Timespan, UnlimitedInteger
                                Float               Float, LimitedInteger, Timespan, UnlimitedInteger
                                Date                Date, Float, LimitedInteger, Time, Timespan, UnlimitedInteger
                                Time                Float, LimitedInteger, Timespan, UnlimitedInteger
                                Timespan            Float, LimitedInteger, Timespan, UnlimitedInteger
            */
            { DataType.LimitedInteger, new HashSet<DataType> { DataType.Float, DataType.LimitedInteger, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.UnlimitedInteger, new HashSet<DataType> { DataType.Float, DataType.LimitedInteger, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.Float, new HashSet<DataType> { DataType.Float, DataType.LimitedInteger, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.Date, new HashSet<DataType> { DataType.Date, DataType.Float, DataType.LimitedInteger, DataType.Time, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.Time, new HashSet<DataType> { DataType.Float, DataType.LimitedInteger, DataType.Timespan, DataType.UnlimitedInteger } },
            { DataType.Timespan, new HashSet<DataType> { DataType.Float, DataType.LimitedInteger, DataType.Timespan, DataType.UnlimitedInteger } },
        };


    }
}