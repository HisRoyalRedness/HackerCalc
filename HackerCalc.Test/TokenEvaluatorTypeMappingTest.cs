using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class TokenEvaluatorTypeMappingTest
    {
        static TokenEvaluatorTypeMappingTest()
        {
            // All possible data types
            _allDataTypes = (TestCommon.GetInstanceField(typeof(TokenEvaluator), null, "_allPossibleTypes") as Lazy<ReadOnlyCollection<TokenDataType>>).Value;
            // All possible combinations of data types
            _allDataTypePairs = (TestCommon.GetInstanceField(typeof(TokenEvaluator), null, "_allPossibleTypePairs") as Lazy<ReadOnlyCollection<TokenEvaluator.OperandTypePair>>).Value;
            // All supported operators
            _operatorProperties = TestCommon.GetInstanceField(typeof(TokenEvaluator), null, "_operatorProperties") as ReadOnlyDictionary<OperatorType, TokenEvaluator.OperatorProperties>;
        }

        [DataTestMethod]
        [DynamicData(nameof(VerifyBinaryTypeCombinationsOnEachOperatorData), DynamicDataSourceType.Method)]
        public void VerifyBinaryTypeCombinationsOnEachOperator(OperatorType opType, TokenDataType leftDataType)
        {
            var opProp = _operatorProperties[opType];
            foreach (var pair in _allDataTypePairs.Where(dt => dt.Left == leftDataType))
            {
                TestContext.WriteLine($"Testing data pair {pair.Left}, {pair.Right}");
                var expr = opProp.Operator.MakeBinaryExpression(pair.Left.MakeToken(), pair.Right.MakeToken());

                // This data type pair should be supported. Make sure the operation succeeds
                if (opProp.TypeMap.ContainsKey(pair) && opProp.TypeMap[pair].OperationSupported)
                {
                    TestContext.WriteLine($"    Supported");
                    expr.Evaluate();
                }

                // This data type pair is not supported. Make sure the operation fails
                else
                {
                    TestContext.WriteLine($"    Not supported");
                    new Action(() => expr.Evaluate()).Should().Throw<InvalidCalcOperationException>($"the {opProp.Operator} doesn't support operations on types {pair.Left} and {pair.Right}");
                }
            }
        }

        public static IEnumerable<object[]> VerifyBinaryTypeCombinationsOnEachOperatorData()
        {
            foreach (var op in _operatorProperties.Values.Where(p => p.NAry == 2).Select(p => p.Operator))
                foreach (var dt in _allDataTypes)
                    yield return new object[] { op, dt };
        }

        public TestContext TestContext { get; set; }
        static ReadOnlyCollection<TokenDataType> _allDataTypes;
        static ReadOnlyCollection<TokenEvaluator.OperandTypePair> _allDataTypePairs;
        static ReadOnlyDictionary<OperatorType, TokenEvaluator.OperatorProperties> _operatorProperties;
    }


}