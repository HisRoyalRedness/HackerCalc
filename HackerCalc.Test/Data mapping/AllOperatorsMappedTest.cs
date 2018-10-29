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
            var binaryDataTypes = EnumExtensions.GetEnumCollection<OperatorType>(e => e.IsBinaryOperator())
                .OrderBy(e => e)
                .ToArray();

            var allMappings = DataMapper.OperandTypeCastMap
                .Select(map => map.Key)
                .OrderBy(e => e)
                .ToArray();

            binaryDataTypes.Should().BeSubsetOf(allMappings);
        }

        [TestMethod]
        public void AllUnaryOperatorsMapped()
        {
            var unaryDataTypes = EnumExtensions.GetEnumCollection<OperatorType>(e => e.IsUnaryOperator())
                .OrderBy(e => e)
                .ToArray();

            var allMappings = DataMapper.OperandTypeCastMap
                .Select(map => map.Key)
                .OrderBy(e => e)
                .ToArray();

            unaryDataTypes.Should().BeSubsetOf(allMappings);
        }

        [TestMethod]
        public void AllUnaryOperatorsMappingsShouldHaveTheSameTypeLeftAndRight()
        {
            var unaryDataTypes = EnumExtensions.GetEnumCollection<OperatorType>(e => e.IsUnaryOperator()).ToHashSet();

            var unaryMappings = DataMapper.OperandTypeCastMap
                .Where(map => unaryDataTypes.Contains(map.Key))
                .OrderBy(map => map.Key)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach(var opType in unaryMappings.Keys)
            {
                var mapping = unaryMappings[opType];
                foreach(var dataMapping in mapping)
                {
                    dataMapping.Key.Left.Should().Be(dataMapping.Key.Right, $"unary mappings should be the same for data type {dataMapping.Key.Left} for operator {opType}.");
                    dataMapping.Value.Left.Should().Be(dataMapping.Value.Right, $"unary mappings should be the same for data type {dataMapping.Value.Left} for operator {opType}.");
                }
            }
        }
    }
}
