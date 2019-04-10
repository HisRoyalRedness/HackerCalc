using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_OPERATION)]
    [TestCategory(TestCommon.MULTIPLY_OPERATION)]
    [TestClass]
    public class MultiplyOperatorTest
    {
        [TestMethod]
        public void AllMultiplyOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l, r) => TestCommon.Operate(null, OperatorType.Multiply, l, r), "multiplied");

        [DataTestMethod]
        // Limited
        [TypedDataRow("2u4 * 3u4",                      "6u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2u4 * 3",                        "6u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2u4 * 3.1",                      "6.2f",                     DataType.Float)]
        [TypedDataRow("2u4 * 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("2u4 * 12:34:56",                 null,                       null)]
        [TypedDataRow("2u4 * 10hrs",                    null,                       null)]
        // Unlimited
        [TypedDataRow("2 * 3u4",                        "6u4",                      DataType.LimitedInteger)]
        [TypedDataRow("2 * 3",                          "6",                        DataType.UnlimitedInteger)]
        [TypedDataRow("2 * 3.1",                        "6.2f",                     DataType.Float)]
        [TypedDataRow("2 * 2019-01-23 12:34:56",        null,                       null)]
        [TypedDataRow("2 * 12:34:56",                   null,                       null)]
        [TypedDataRow("2 * 10hrs",                      null,                       null)]
        // Float
        [TypedDataRow("2.6 * 3u4",                      "7.8f",                     DataType.Float)]
        [TypedDataRow("2.6 * 3",                        "7.8f",                     DataType.Float)]
        [TypedDataRow("2.6 * 3.1",                      "8.06f",                    DataType.Float)]
        [TypedDataRow("2.6 * 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("2.6 * 12:34:56",                 null,                       null)]
        [TypedDataRow("2.6 * 10hrs",                    null,                       null)]
        // Date
        [TypedDataRow("2019-01-23 12:34:56 * 3u4",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 * 3",        null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 * 3.1",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 * now",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 * 12:34:56", null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 * 10hrs",    null,                       null)]
        // Time
        [TypedDataRow("12:34:56 * 3u4",                 null,                       null)]
        [TypedDataRow("12:34:56 * 3",                   null,                       null)]
        [TypedDataRow("12:34:56 * 3.1",                 null,                       null)]
        [TypedDataRow("12:34:56 * 2019-01-23 12:34:56", null,                       null)]
        [TypedDataRow("12:34:56 * 12:34:56",            null,                       null)]
        [TypedDataRow("12:34:56 * 10hrs",               null,                       null)]
        // Timespan 
        [TypedDataRow("12h34m56s * 3u4",                "1day13h44m48s",            DataType.Timespan)]
        [TypedDataRow("12h34m56s * 3",                  "1day13h44m48s",            DataType.Timespan)]
        [TypedDataRow("12h34m56s * 3.1",                "1day15h0m17.6s",           DataType.Timespan)]
        [TypedDataRow("12h34m56s * 2019-01-23 12:34:56",null,                       null)]
        [TypedDataRow("12h34m56s * 12:34:56",           null,                       null)]
        [TypedDataRow("12h34m56s * 10hrs",              null,                       null)]
        public void MultiplyOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr, new Configuration() { AllowMultidayTimes = true });

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Multiply (*)        LimitedInteger      LimitedInteger, UnlimitedInteger, Float
                                UnlimitedInteger    LimitedInteger, UnlimitedInteger, Float
                                Float               LimitedInteger, UnlimitedInteger, Float
                                Timespan            LimitedInteger, UnlimitedInteger, Float
            */
            { DataType.LimitedInteger,      new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.UnlimitedInteger,    new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.Float,               new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
            { DataType.Date,                new HashSet<DataType> { } },
            { DataType.Time,                new HashSet<DataType> { } },
            { DataType.Timespan,            new HashSet<DataType> { DataType.LimitedInteger, DataType.UnlimitedInteger, DataType.Float } },
        };


    }
}