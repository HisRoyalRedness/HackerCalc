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
        [TypedDataRow("2u4 + 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("2u4 + 3.1",                      "5.1f",                     DataType.Float)]
        [TypedDataRow("2u4 + 3u4",                      "5u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2u4 + 12:34:56",                 null,                       null)]
        [TypedDataRow("2u4 + 10hrs",                    null,                       null)]
        [TypedDataRow("2u4 + 3",                        "5u4",                      DataType.LimitedInteger)]
        // Unlimited
        [TypedDataRow("2 + 2019-01-23 12:34:56",        null,                       null)]
        [TypedDataRow("2 + 3.1",                        "5.1f",                     DataType.Float)]
        [TypedDataRow("2 + 3u4",                        "5u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2 + 12:34:56",                   null,                       null)]
        [TypedDataRow("2 + 10hrs",                      null,                       null)]
        [TypedDataRow("2 + 3",                          "5",                        DataType.UnlimitedInteger)]
        // Float
        [TypedDataRow("2.6 + 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("2.6 + 3.1",                      "5.7f",                     DataType.Float)]
        [TypedDataRow("2.6 + 3u4",                      "5.6f",                     DataType.Float)]
        [TypedDataRow("2.6 + 12:34:56",                 null,                       null)]
        [TypedDataRow("2.6 + 10hrs",                    null,                       null)]
        [TypedDataRow("2.6 + 3",                        "5.6f",                     DataType.Float)]
        // Date
        [TypedDataRow("2019-01-23 12:34:56 + now",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 + 3.1",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 + 3u4",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 + 12:34:56", null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 + 10hrs",    "2019-01-23 22:34:56",      DataType.Date)]
        [TypedDataRow("2019-01-23 12:34:56 + 3",        null,                       null)]
        // Time
        [TypedDataRow("12:34:56 + 2019-01-23 12:34:56", null,                       null)]
        [TypedDataRow("12:34:56 + 3.1",                 null,                       null)]
        [TypedDataRow("12:34:56 + 3u4",                 null,                       null)]
        [TypedDataRow("12:34:56 + 12:34:56",            null,                       null)]
        [TypedDataRow("12:34:56 + 10hrs",               "22:34:56",                 DataType.Time)]
        [TypedDataRow("12:34:56 + 3",                   null,                       null)]
        // Timespan
        [TypedDataRow("12h34m56s + 2019-01-23 12:34:56","2019-01-24 01:09:52",      DataType.Date)]
        [TypedDataRow("12h34m56s + 3.1",                null,                       null)]
        [TypedDataRow("12h34m56s + 3u4",                null,                       null)]
        [TypedDataRow("12h34m56s + 12:34:56",           "1.01:09:52",               DataType.Time)]
        [TypedDataRow("12h34m56s + 10hrs",              "22h34m56s",                DataType.Timespan)]
        [TypedDataRow("12h34m56s + 3",                  null,                       null)]
        public void AddOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr, new Configuration() { AllowMultidayTimes = true });

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Add (+)             LimitedInteger      LimitedInteger, UnlimitedInteger, Float
                                UnlimitedInteger    LimitedInteger, UnlimitedInteger, Float
                                Float               LimitedInteger, UnlimitedInteger, Float
                                Date                Timespan
                                Time                Timespan
                                Timespan            Date, Time, Timespan
            */
            { DataType.LimitedInteger,      new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.UnlimitedInteger,    new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.Float,               new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.Date,                new HashSet<DataType> { DataType.Timespan } },
            { DataType.Time,                new HashSet<DataType> { DataType.Timespan } },
            { DataType.Timespan,            new HashSet<DataType> { DataType.Date, DataType.Time, DataType.Timespan } },
        };
    }
}