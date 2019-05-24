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
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l,r) => TestCommon.Operate(null, OperatorType.Add, l, r), "added");

        [DataTestMethod]
        // Limited
        [TypedDataRow("2u4 + 3u4",                      "5u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2u4 + 3.1",                      "5u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2u4 + 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("2u4 + 12:34:56",                 null,                       null)]
        [TypedDataRow("2u4 + 10hrs",                    null,                       null)]
        // Rational
        [TypedDataRow("2.6 + 3u4",                      "5u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2.6 + 3.1",                      "5.7f",                     DataType.RationalNumber)]
        [TypedDataRow("2.6 + 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("2.6 + 12:34:56",                 null,                       null)]
        [TypedDataRow("2.6 + 10hrs",                    null,                       null)]
        //// Irrational
        //[TypedDataRow("2.6 + 3u4",                      "5u4",                      DataType.LimitedInteger)]
        //[TypedDataRow("2.6 + 3",                        "5.6f",                     DataType.RationalNumber)]
        //[TypedDataRow("2.6 + 3.1",                      "5.7f",                     DataType.RationalNumber)]
        //[TypedDataRow("2.6 + 2019-01-23 12:34:56",      null,                       null)]
        //[TypedDataRow("2.6 + 12:34:56",                 null,                       null)]
        //[TypedDataRow("2.6 + 10hrs",                    null,                       null)]
        // Date
        [TypedDataRow("2019-01-23 12:34:56 + 3u4",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 + 3.1",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 + now",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 + 12:34:56", null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 + 10hrs",    "2019-01-23 22:34:56",      DataType.Date)]
        // Time
        [TypedDataRow("12:34:56 + 3u4",                 null,                       null)]
        [TypedDataRow("12:34:56 + 3.1",                 null,                       null)]
        [TypedDataRow("12:34:56 + 2019-01-23 12:34:56", null,                       null)]
        [TypedDataRow("12:34:56 + 12:34:56",            null,                       null)]
        [TypedDataRow("12:34:56 + 10hrs",               "22:34:56",                 DataType.Time)]
        // Timespan
        [TypedDataRow("12h34m56s + 3u4",                null,                       null)]
        [TypedDataRow("12h34m56s + 3.1",                null,                       null)]
        [TypedDataRow("12h34m56s + 2019-01-23 12:34:56","2019-01-24 01:09:52",      DataType.Date)]
        [TypedDataRow("12h34m56s + 12:34:56",           "1.01:09:52",               DataType.Time)]
        [TypedDataRow("12h34m56s + 10hrs",              "22h34m56s",                DataType.Timespan)]
        public void AddOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr, new Configuration() { AllowMultidayTimes = true });

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Add (+)             LimitedInteger      LimitedInteger, RationalNumber, IrrationalNumber
                                RationalNumber      LimitedInteger, RationalNumber, IrrationalNumber
                                IrrationalNumber    LimitedInteger, RationalNumber, IrrationalNumber
                                Date                Timespan
                                Time                Timespan
                                Timespan            Date, Time, Timespan
            */
            { DataType.LimitedInteger,      new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.RationalNumber,      new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.IrrationalNumber,    new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.Date,                new HashSet<DataType> { DataType.Timespan } },
            { DataType.Time,                new HashSet<DataType> { DataType.Timespan } },
            { DataType.Timespan,            new HashSet<DataType> { DataType.Date, DataType.Time, DataType.Timespan } },
        };
    }
}