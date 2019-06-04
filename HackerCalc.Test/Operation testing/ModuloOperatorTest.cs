using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_OPERATION)]
    [TestCategory(TestCommon.MODULO_OPERATION)]
    [TestClass]
    public class ModuloOperatorTest
    {
        [TestMethod]
        public void AllModuloOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, (l, r) => TestCommon.Operate(null, OperatorType.Modulo, l, r), "mod");

        [DataTestMethod]
        // Limited
        [TypedDataRow("7u4 % 2u4",                      "1u4",                      DataType.LimitedInteger)]
        [TypedDataRow("7u4 % 2.5",                      "1u4",                      DataType.LimitedInteger)]
        [TypedDataRow("7u4 % 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("7u4 % 12:34:56",                 null,                       null)]
        [TypedDataRow("7u4 % 10hrs",                    null,                       null)]
        // Rational
        [TypedDataRow("7.2 % 2u4",                      "1u4",                      DataType.LimitedInteger)]
        [TypedDataRow("7.2 % 2.5",                      "2.2",                      DataType.RationalNumber)]
        [TypedDataRow("7.2 % 2019-01-23 12:34:56",      null,                       null)]
        [TypedDataRow("7.2 % 12:34:56",                 null,                       null)]
        [TypedDataRow("7.2 % 10hrs",                    null,                       null)]
        //// Irrational
        //[TypedDataRow("7.2 % 2u4",                      "1.2f",                     DataType.IrrationalNumber)]
        //[TypedDataRow("7.2 % 3.1",                      "1.0f",                     DataType.IrrationalNumber)]
        //[TypedDataRow("7.2 % 2019-01-23 12:34:56",      null,                       null)]
        //[TypedDataRow("7.2 % 12:34:56",                 null,                       null)]
        //[TypedDataRow("7.2 % 10hrs",                    null,                       null)]
        // Date
        [TypedDataRow("2019-01-23 12:34:56 % 3u4",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 % 3.1",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 % now",      null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 % 12:34:56", null,                       null)]
        [TypedDataRow("2019-01-23 12:34:56 % 10hrs",    null,                       null)]
        // Time
        [TypedDataRow("12:34:56 % 3u4",                 null,                       null)]
        [TypedDataRow("12:34:56 % 3.1",                 null,                       null)]
        [TypedDataRow("12:34:56 % 2019-01-23 12:34:56", null,                       null)]
        [TypedDataRow("12:34:56 % 12:34:56",            null,                       null)]
        [TypedDataRow("12:34:56 % 10hrs",               null,                       null)]
        // Timespan 
        [TypedDataRow("12h35m59s % 3u4",                null,                       null)]
        [TypedDataRow("12h35m59s % 3.1",                null,                       null)]
        [TypedDataRow("12h35m59s % 2019-01-23 12:34:56",null,                       null)]
        [TypedDataRow("12h35m59s % 12:34:56",           null,                       null)]
        [TypedDataRow("12h35m59s % 20mins",             null,                       null)]
        public void ModuloOperandTypeTests(string actualStr, string expectedStr, string expectedTypeStr)
            => TestCommon.EvaluateActualAndExpected(actualStr, expectedStr, expectedTypeStr, new Configuration() { AllowMultidayTimes = true });

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Modulo (%)          LimitedInteger      LimitedInteger, RationalNumber, IrrationalNumber
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


    }
}