using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_OPERATION)]
    [TestCategory(TestCommon.POWER_OPERATION)]
    [TestClass]
    public class PowerOperatorTest
    {
        [TestMethod]
        public void AllPowerOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l, r) => TestCommon.Operate(null, OperatorType.Power, l, r), "raised to the power of");

        [DataTestMethod]
        // Limited
        [TypedDataRow("2u4 ** 3u4",                     "8u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2u4 ** 3",                       "8u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2u4 ** 3.1",                     "8.5741877f",               DataType.Float)]
        [TypedDataRow("2u4 ** 2019-01-23 12:34:56",     null,                       null)]
        [TypedDataRow("2u4 ** 12:34:56",                null,                       null)]
        [TypedDataRow("2u4 ** 10hrs",                   null,                       null)]
        // Unlimited
        [TypedDataRow("2 ** 3u4",                       "8u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2 ** 3",                         "8",                        DataType.RationalNumber)]
        [TypedDataRow("2 ** 3.1",                       "8.5741877f",                     DataType.Float)]
        [TypedDataRow("2 ** 2019-01-23 12:34:56",       null,                       null)]
        [TypedDataRow("2 ** 12:34:56",                  null,                       null)]
        [TypedDataRow("2 ** 10hrs",                     null,                       null)]
        // Float
        [TypedDataRow("2.6 ** 3u4",                     "17.576f",                  DataType.Float)]
        [TypedDataRow("2.6 ** 3",                       "17.576f",                  DataType.Float)]
        [TypedDataRow("2.6 ** 3.1",                     "19.3382592764f",           DataType.Float)]
        [TypedDataRow("2.6 ** 2019-01-23 12:34:56",     null,                       null)]
        [TypedDataRow("2.6 ** 12:34:56",                null,                       null)]
        [TypedDataRow("2.6 ** 10hrs",                   null,                       null)]
        // Date
        [TypedDataRow("2019-01-23 12:34:56 ** 3u4",     null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 ** 3",       null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 ** 3.1",     null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 ** now",     null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 ** 12:34:56",null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 ** 10hrs",   null,                       null)]
        // Time
        [TypedDataRow("12:34:56 ** 3u4",                null,                       null)]
        [TypedDataRow("12:34:56 ** 3",                  null,                       null)]
        [TypedDataRow("12:34:56 ** 3.1",                null,                       null)]
        [TypedDataRow("12:34:56 ** 2019-01-23 12:34:56",null,                       null)]
        [TypedDataRow("12:34:56 ** 12:34:56",           null,                       null)]
        [TypedDataRow("12:34:56 ** 10hrs",              null,                       null)]
        // Timespan 
        [TypedDataRow("12h34m56s ** 3u4",               null,                       null)]
        [TypedDataRow("12h34m56s ** 3",                 null,                       null)]
        [TypedDataRow("12h34m56s ** 3.1",               null,                       null)]
        [TypedDataRow("12h34m56s ** 2019-01-23 12:34:56",null,                      null)]
        [TypedDataRow("12h34m56s ** 12:34:56",          null,                       null)]
        [TypedDataRow("12h34m56s ** 10hrs",             null,                       null)]
        public void PowerOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr, new Configuration() { AllowMultidayTimes = true });

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Power (**)          LimitedInteger      LimitedInteger, UnlimitedInteger, Float
                                UnlimitedInteger    LimitedInteger, UnlimitedInteger, Float
                                Float               LimitedInteger, UnlimitedInteger, Float
            */
            { DataType.LimitedInteger,      new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.Float } },
            { DataType.RationalNumber,      new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.Float } },
            { DataType.Float,               new HashSet<DataType> { DataType.LimitedInteger, DataType.RationalNumber, DataType.Float } },
            { DataType.Date,                new HashSet<DataType> { } },
            { DataType.Time,                new HashSet<DataType> { } },
            { DataType.Timespan,            new HashSet<DataType> { } },
        };


    }
}