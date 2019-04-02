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
    public class SymmetricalOperatorsTest
    {
        [TestMethod]
        public void AddOperationsShouldBeSymmetrical() => TestSymmetry(OperatorType.Add);


        void TestSymmetry(OperatorType opType)
        {
            var l1 = new SortedDictionary<DataType, SortedSet<DataType>>();
            var l2 = new SortedDictionary<DataType, SortedSet<DataType>>();

            var operandPairs = DataMapper.AllInputOperandTypes.Value[opType];
            foreach (var leftType in operandPairs.Keys)
            {
                if (!l1.ContainsKey(leftType))
                    l1.Add(leftType, new SortedSet<DataType>());
                foreach(var rightType in operandPairs[leftType])
                {
                    if (!l2.ContainsKey(rightType))
                        l2.Add(rightType, new SortedSet<DataType>());

                    l1[leftType].Add(rightType);
                    l2[rightType].Add(leftType);
                }
            }

            foreach (var dt in l1.Keys.Union(l2.Keys).Distinct())
            {
                Assert.IsTrue(l1.ContainsKey(dt), $"Operator {opType} has a {dt} key type for l2, but not for l1.");
                Assert.IsTrue(l2.ContainsKey(dt), $"Operator {opType} has a {dt} key type for l1, but not for l2.");

                foreach (var dt1 in l1[dt].Union(l2[dt]).Distinct())
                {
                    Assert.IsTrue(l1[dt].Contains(dt1), $"Operator {opType} has a {dt1} value type for {dt} key for l2, but not for l1.");
                    Assert.IsTrue(l2[dt].Contains(dt1), $"Operator {opType} has a {dt1} value type for {dt} key for l1, but not for l2.");
                }
            }
        }
    }
}
