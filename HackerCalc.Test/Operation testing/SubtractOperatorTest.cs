using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_OPERATION)]
    [TestCategory(TestCommon.SUBTRACT_OPERATION)]
    [TestClass]
    public class SubtractOperatorTest
    {
        [TestMethod]
        public void AllSubtractOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l, r) => l - r, "subtracted");

        [DataTestMethod]
        // Limited
        [TypedDataRow("2u4 - 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("2u4 - 3.1",                      "-1.1f",                    DataType.Float)]
        [TypedDataRow("2u4 - 1u4",                      "1u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2u4 - 12:34:56",                 null,                       null)]
        [TypedDataRow("2u4 - 10hrs",                    null,                       null)]
        [TypedDataRow("2u4 - 1",                        "1u4",                      DataType.LimitedInteger)]
        // Unlimited
        [TypedDataRow("2 - 2019-01-23 12:34:56",        null,                       null)]
        [TypedDataRow("2 - 3.1",                        "-1.1f",                    DataType.Float)]
        [TypedDataRow("2 - 1u4",                        "1u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2 - 12:34:56",                   null,                       null)]
        [TypedDataRow("2 - 10hrs",                      null,                       null)]
        [TypedDataRow("2 - 3",                          "-1",                       DataType.UnlimitedInteger)]
        // Float
        [TypedDataRow("2.6 - 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("2.6 - 3.1",                      "-0.5f",                    DataType.Float)]
        [TypedDataRow("2.6 - 1u4",                      "1.6f",                     DataType.Float)]
        [TypedDataRow("2.6 - 12:34:56",                 null,                       null)]
        [TypedDataRow("2.6 - 10hrs",                    null,                       null)]
        [TypedDataRow("2.6 - 1",                        "1.6f",                     DataType.Float)]
        // Date
        [TypedDataRow("2019-01-23 12:34:56 - 2019-01-22 11:34:56", "1day 1hr",      DataType.Timespan)]
        [TypedDataRow("2019-01-23 12:34:56 - 3.1",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 - 3u4",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 - 12:34:56", null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 - 10hrs",    "2019-01-23 02:34:56",      DataType.Date)]
        [TypedDataRow("2019-01-23 12:34:56 - 3",        null,                       null)]
        // Time
        [TypedDataRow("12:34:56 - 2019-01-23 12:34:56", null,                       null)]
        [TypedDataRow("12:34:56 - 3.1",                 null,                       null)]
        [TypedDataRow("12:34:56 - 3u4",                 null,                       null)]
        [TypedDataRow("12:34:56 - 11:33:55",            "1hr 1min 1sec",            DataType.Timespan)]
        [TypedDataRow("12:34:56 - 10hrs",               "02:34:56",                 DataType.Time)]
        [TypedDataRow("12:34:56 - 3",                   null,                       null)]
        // Timespan
        [TypedDataRow("12h34m56s - 2019-01-23 12:34:56",null,                       null)]
        [TypedDataRow("12h34m56s - 3.1",                null,                       null)]
        [TypedDataRow("12h34m56s - 3u4",                null,                       null)]
        [TypedDataRow("12h34m56s - 12:34:56",           null,                       null)]
        [TypedDataRow("12h34m56s - 10hrs",              "2h34m56s",                 DataType.Timespan)]
        [TypedDataRow("12h34m56s - 3",                  null,                       null)]
        public void SubtractOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr, new Configuration() { AllowMultidayTimes = true });

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Subtract (-)        LimitedInteger      LimitedInteger, UnlimitedInteger, Float
                                UnlimitedInteger    LimitedInteger, UnlimitedInteger, Float
                                Float               LimitedInteger, UnlimitedInteger, Float
                                Date                Date, Timespan
                                Time                Time, Timespan
                                Timespan            Timespan
            */
            { DataType.LimitedInteger,      new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.UnlimitedInteger,    new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.Float,               new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.Date,                new HashSet<DataType> { DataType.Date, DataType.Timespan } },
            { DataType.Time,                new HashSet<DataType> { DataType.Time, DataType.Timespan } },
            { DataType.Timespan,            new HashSet<DataType> { DataType.Timespan } },
        };


    }
}