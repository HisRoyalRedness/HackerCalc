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
    [TestClass]
    public class TypeCastTest
    {
        [TestMethod]
        public void AllSupportedTypeCastsWorkCorrectly()
        {
            foreach(var cast in DataMapper.AllTypeCasts.Value)
            {
                var dataValue = TestCommon.MakeDataType(cast.Key);
                foreach (var val in cast.Value)
                {
                    ((Action)(() => dataValue.CastTo(val))).Should().NotThrow($"a cast from {cast.Key} to {val} should be supported");
                    dataValue.CastTo(val).DataType.Should().Be(val);
                }
            }
        }
    }
}

