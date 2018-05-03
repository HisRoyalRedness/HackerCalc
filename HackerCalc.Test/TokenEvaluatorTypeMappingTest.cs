using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class TokenEvaluatorTypeMappingTest
    {
        [TestMethod]
        public void VerifyTypeCombinationsOnEachOperator()
        {
            var allDataTypes = (TestCommon.GetInstanceField(typeof(TokenEvaluator), null, "_allPossibleTypePairs") as Lazy<ReadOnlyCollection<TokenEvaluator.OperandTypePair>>).Value;
            var operatorProperties = TestCommon.GetInstanceField(typeof(TokenEvaluator), null, "_operatorProperties") as ReadOnlyDictionary<OperatorType, TokenEvaluator.OperatorProperties>;
            foreach(var opProp in operatorProperties.Values)
            {
                opProp.TypeMap
            }
            Console.WriteLine();
        }
    }
}