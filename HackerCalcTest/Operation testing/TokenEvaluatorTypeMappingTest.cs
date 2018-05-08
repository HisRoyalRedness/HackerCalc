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
        public void VerifyBinaryTypeCombinationsOnEachOperator(OperatorType opType, TokenDataType leftDataType, TokenDataType rightDataType)
        {
            var opProp = _operatorProperties[opType];
            var pair = new TokenEvaluator.OperandTypePair(leftDataType, rightDataType);
            var expr = opProp.Operator.MakeBinaryExpression(leftDataType.MakeToken(), rightDataType.MakeToken());

            // This data type pair should be supported. Make sure the operation succeeds
            if (opProp.TypeMap.ContainsKey(pair) && opProp.TypeMap[pair].OperationSupported)
            {
                var castPair = opProp.TypeMap[pair];
                var result = expr.Evaluate();
                result.Should().NotBeNull();
                result.Should().BeAssignableTo<ILiteralToken>();

                if (opProp.ResultMap != null)
                {
                    opProp.ResultMap.ContainsKey(castPair).Should().BeTrue("if we have a result map, it should have a result type for a supported operation.");
                    ((ILiteralToken)result).DataType.Should().Be(opProp.ResultMap[castPair], "the result type should match the specified type in the result map");
                }
            }

            // This data type pair is not supported. Make sure the operation fails
            else
                new Action(() => expr.Evaluate()).Should().Throw<InvalidCalcOperationException>($"the {opProp.Operator} doesn't support operations on types {pair.Left} and {pair.Right}");
        }


        // The combination of all data types, on all binary operators
        public static IEnumerable<object[]> VerifyBinaryTypeCombinationsOnEachOperatorData()
        {
            foreach (var op in _operatorProperties.Values.Where(p => p.NAry == 2).Select(p => p.Operator))
                foreach (var lt in _allDataTypes)
                    foreach (var rt in _allDataTypes)
                        yield return new object[] { op, lt, rt };
        }

        public TestContext TestContext { get; set; }
        static ReadOnlyCollection<TokenDataType> _allDataTypes;
        static ReadOnlyCollection<TokenEvaluator.OperandTypePair> _allDataTypePairs;
        static ReadOnlyDictionary<OperatorType, TokenEvaluator.OperatorProperties> _operatorProperties;
    }


}