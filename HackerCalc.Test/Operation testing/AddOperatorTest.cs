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
    public class AddOperatorTest
    {

        [TestMethod]
        public void AllAddOperandTypesAreSupported()
            => TestCommon.TestThatAllPossibleOperandTypesAreSupported(_supportedOperatorPairs, "added");

        static readonly Dictionary<DataType, HashSet<DataType>> _supportedOperatorPairs = new Dictionary<DataType, HashSet<DataType>>
        {
            /*
            Add (+)             LimitedInteger      LimitedInteger
                                UnlimitedInteger    UnlimitedInteger
                                Float               Float
                                Date                Timespan
                                Time                Timespan
                                Timespan            Date, Time, Timespan
            */
            { DataType.LimitedInteger, new HashSet<DataType> { DataType.LimitedInteger} },
            { DataType.UnlimitedInteger, new HashSet<DataType> { DataType.UnlimitedInteger} },
            { DataType.Float, new HashSet<DataType> { DataType.Float} },
            { DataType.Date, new HashSet<DataType> { DataType.Timespan} },
            { DataType.Time, new HashSet<DataType> { DataType.Timespan} },
            { DataType.Timespan, new HashSet<DataType> { DataType.Date, DataType.Time, DataType.Timespan } },
        };


    }
}