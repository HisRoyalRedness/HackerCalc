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
            foreach(var cast in _allTypeCasts.Value)
            {
                var dataValue = TestCommon.MakeDataType(cast.Key);
                foreach (var val in cast.Value)
                {
                    ((Action)(() => dataValue.CastTo(val))).Should().NotThrow($"a cast from {cast.Key} to {val} should be supported");
                    dataValue.CastTo(val).DataType.Should().Be(val);
                }
            }
        }



        static TypeCastTest()
        {
            _allTypeCasts = new Lazy<Dictionary<DataType, HashSet<DataType>>>(() =>
            {
                var allPairCasts = DataMapper.OperandTypeCastMap
                    .SelectMany(op => op.Value)
                    .Where(pairs => pairs.Value.OperationSupported)
                    .ToList();
                var allTypeCasts =
                    allPairCasts.Select(pair => new KeyValuePair<DataType, DataType>(pair.Key.Left, pair.Value.Left)).Concat(
                    allPairCasts.Select(pair => new KeyValuePair<DataType, DataType>(pair.Key.Right, pair.Value.Right)))
                    .ToList();
                var allCasts = new Dictionary<DataType, HashSet<DataType>>();
                foreach (var cast in allTypeCasts)
                {
                    if (!allCasts.ContainsKey(cast.Key))
                        allCasts.Add(cast.Key, new HashSet<DataType>());
                    if (cast.Key != cast.Value)
                        allCasts[cast.Key].Add(cast.Value);
                }
                return allCasts;
            });
        }

        static Lazy<Dictionary<DataType, HashSet<DataType>>> _allTypeCasts;
    }
}

