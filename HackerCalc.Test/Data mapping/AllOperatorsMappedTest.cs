using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.DATA_MAPPING)]
    [TestClass]
    public class AllOperatorsMappedTest
    {
        [TestMethod]
        public void AllBinaryOperatorsMapped()
        {
            var dataTypes = EnumExtensions.GetEnumCollection<OperatorType>(e => e.IsBinaryOperator())
                .OrderBy(e => e)
                .ToArray();

            DataMapper.OperandTypeCastMap
                .Select(map => map.Key)
                .OrderBy(e => e)
                .ToArray()
                .Should().BeEquivalentTo(dataTypes);
        }

        [TestMethod]
        public void AllUnaryOperatorsMapped()
        {
            Assert.Fail("Not implemented");
        }
    }
}
