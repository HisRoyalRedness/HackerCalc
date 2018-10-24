using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.TYPE_CASTING)]
    [TestCategory(TestCommon.DATA_MAPPING)]
    [TestCategory(nameof(LimitedIntegerType))]
    [TestClass]
    public class LimitedIntegerTypeRangeTest
    {
        [DataTestMethod]
        [DataRow("0", "u", "4")]
        public void LimitedIntegerIsCreatedWithinRange()
        {
            create tests here

            // Test that LimitedIntegerType.CreateLimitedIntegerType generates types that 
            // have IsSigned and BitWidth values that are appropriate for the input value.
            // Also test that out of range values are handled
        }
    }
}

