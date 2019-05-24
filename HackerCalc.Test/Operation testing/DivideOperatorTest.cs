using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_OPERATION)]
    [TestCategory(TestCommon.DIVIDE_OPERATION)]
    [TestClass]
    public class DivideOperatorTest
    {
        [TestMethod]
        public void AllDivideOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l, r) => TestCommon.Operate(null, OperatorType.Divide, l, r), "divided");

        [DataTestMethod]
        // Limited
        [TypedDataRow("6u4 / 2u4",                      "3u4",                      DataType.LimitedInteger)]
        [TypedDataRow("6u4 / 2.5",                      "3u4",                      DataType.LimitedInteger)]
        [TypedDataRow("6u4 / 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("6u4 / 12:34:56",                 null,                       null)]
        [TypedDataRow("6u4 / 10hrs",                    null,                       null)]
        // Rational
        [TypedDataRow("6.2 / 2u4",                      "3u4",                      DataType.LimitedInteger)]
        [TypedDataRow("6.2 / 2.5",                      "2.48",                     DataType.RationalNumber)]
        [TypedDataRow("6.2 / 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("6.2 / 12:34:56",                 null,                       null)]
        [TypedDataRow("6.2 / 10hrs",                    null,                       null)]
        //// Irrational
        //[TypedDataRow("6.2 / 2u4",                      "3.1f",                     DataType.IrrationalNumber)]
        //[TypedDataRow("6.2 / 3.1",                      "2.0f",                     DataType.IrrationalNumber)]
        //[TypedDataRow("6.2 / 2019-01-23 12:34:56",      null,                       null)]
        //[TypedDataRow("6.2 / 12:34:56",                 null,                       null)]
        //[TypedDataRow("6.2 / 10hrs",                    null,                       null)]
        // Date
        [TypedDataRow("2019-01-23 12:34:56 / 3u4",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 / 3.1",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 / now",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 / 12:34:56", null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 / 10hrs",    null,                       null)]
        // Time
        [TypedDataRow("12:34:56 / 3u4",                 null,                       null)]
        [TypedDataRow("12:34:56 / 3.1",                 null,                       null)]
        [TypedDataRow("12:34:56 / 2019-01-23 12:34:56", null,                       null)]
        [TypedDataRow("12:34:56 / 12:34:56",            null,                       null)]
        [TypedDataRow("12:34:56 / 10hrs",               null,                       null)]
        // Timespan 
        [TypedDataRow("12h34m45s / 3u4",                "4h11m35s",                 DataType.Timespan)]
        [TypedDataRow("12h34m46s / 3.2",                "3h55m51.875s",             DataType.Timespan)]
        [TypedDataRow("12h34m45s / 2019-01-23 12:34:56",null,                       null)]
        [TypedDataRow("12h34m45s / 12:34:56",           null,                       null)]
        [TypedDataRow("12h34m45s / 20mins",             "37.7375",                  DataType.RationalNumber)]
        public void DivideOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr, new Configuration() { AllowMultidayTimes = true });

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Divide (/)          LimitedInteger      LimitedInteger, RationalNumber, IrrationalNumber
                                RationalNumber      LimitedInteger, RationalNumber, IrrationalNumber
                                IrrationalNumber    LimitedInteger, RationalNumber, IrrationalNumber
                                Timespan            LimitedInteger, RationalNumber, IrrationalNumber, Timespan
            */
            { DataType.LimitedInteger,      new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.RationalNumber,      new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.IrrationalNumber,    new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber } },
            { DataType.Date,                new HashSet<DataType> { } },
            { DataType.Time,                new HashSet<DataType> { } },
            { DataType.Timespan,            new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.IrrationalNumber, DataType.Timespan } },
        };


    }
}