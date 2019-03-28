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
        //[TypedDataRow("2u4 + 2019-01-23 12:34:56",      "2019-01-23 12:34:58",      DataType.Date)]
        //[TypedDataRow("2u4 + 3.1",                      "5.1f",                     DataType.Float)]
        //[TypedDataRow("2u4 + 3u4",                      "5u4",                      DataType.LimitedInteger)]
        //[TypedDataRow("2u4 + 12:34:56",                 "12:34:58",                 DataType.Time)]
        //[TypedDataRow("2u4 + 10hrs",                    "10hrs 2 sec",              DataType.Timespan)]
        //[TypedDataRow("2u4 + 3",                        "5u4",                      DataType.LimitedInteger)]
        //// Unlimited
        //[TypedDataRow("2 + 2019-01-23 12:34:56",        "2019-01-23 12:34:58",      DataType.Date)]
        //[TypedDataRow("2 + 3.1",                        "5.1f",                     DataType.Float)]
        //[TypedDataRow("2 + 3u4",                        "5u4",                      DataType.LimitedInteger)]
        //[TypedDataRow("2 + 12:34:56",                   "12:34:58",                 DataType.Time)]
        //[TypedDataRow("2 + 10hrs",                      "10hrs 2 sec",              DataType.Timespan)]
        //[TypedDataRow("2 + 3",                          "5",                        DataType.UnlimitedInteger)]
        //// Float
        //[TypedDataRow("2.6 + 2019-01-23 12:34:56",      "2019-01-23 12:34:58.6",    DataType.Date)]
        //[TypedDataRow("2.6 + 3.1",                      "5.7f",                     DataType.Float)]
        //[TypedDataRow("2.6 + 3u4",                      "5.6f",                     DataType.Float)]
        //[TypedDataRow("2.6 + 12:34:56",                 "12:34:58.6",               DataType.Time)]
        //[TypedDataRow("2.6 + 10hrs",                    "10hrs 2sec 600ms",         DataType.Timespan)]
        //[TypedDataRow("2.6 + 3",                        "5.6f",                     DataType.Float)]
        //// Date
        //[TypedDataRow("2019-01-23 12:34:56 + 3.1",      "2019-01-23 12:34:59.1",    DataType.Date)]
        //[TypedDataRow("2019-01-23 12:34:56 + 3u4",      "2019-01-23 12:34:59",      DataType.Date)]
        //[TypedDataRow("2019-01-23 12:34:56 + 12:34:56", "2019-01-24 01:09:52",      DataType.Date)]
        //[TypedDataRow("2019-01-23 12:34:56 + 10hrs",    "2019-01-23 22:34:56",      DataType.Date)]
        //[TypedDataRow("2019-01-23 12:34:56 + 3",        "2019-01-23 12:34:59",      DataType.Date)]
        //// Time
        //[TypedDataRow("12:34:56 + 2019-01-23 12:34:56", "2019-01-24 01:09:52",      DataType.Date)]
        //[TypedDataRow("12:34:56 + 3.1",                 "12:34:59.1",               DataType.Time)]
        //[TypedDataRow("12:34:56 + 3u4",                 "12:34:59",                 DataType.Time)]
        [TypedDataRow("12:34:56 + 12:34:56",            "1.01:09:52",               DataType.Time)] // 2nd addend is cast to a timespan
        //[TypedDataRow("12:34:56 + 10hrs",               "22:34:56",                 DataType.Time)]
        //[TypedDataRow("12:34:56 + 3",                   "12:34:59",                 DataType.Time)]
        //// Timespan
        //[TypedDataRow("12h34m56s + 2019-01-23 12:34:56","2019-01-24 01:09:52",      DataType.Date)]
        //[TypedDataRow("12h34m56s + 3.1",                "12:34:59.1",               DataType.Timespan)]
        //[TypedDataRow("12h34m56s + 3u4",                "12:34:59",                 DataType.Timespan)]
        //[TypedDataRow("12h34m56s + 12:34:56",           "1.01:09:52",               DataType.Timespan)]
        //[TypedDataRow("12h34m56s + 10hrs",              "22:34:56",                 DataType.Timespan)]
        //[TypedDataRow("12h34m56s + 3",                  "12:34:59",                 DataType.Timespan)]
        public void AddOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr);

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