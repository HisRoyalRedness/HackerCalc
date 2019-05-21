using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.TOKEN_EVALUATOR)]
    [TestClass]
    public class EvaluatorTests
    {
        [TestMethod]
        public void UnlimitedIntegerTokenEvaluatesCorrectly()
            => "1".Evaluate<RationalNumberType>().Value.Should().Be(1);

        [TestMethod]
        public void LimitedIntegerTokenEvaluatesCorrectly()
            => "1i32".Evaluate<LimitedIntegerType>().Value.Should().Be(1);

        [TestMethod]
        public void FloatTokenEvaluatesCorrectly()
            => "1f".Evaluate<IrrationalNumberType>().Value.Should().Be(1.0);

        [TestMethod]
        public void GroupingTokenEvaluatesCorrectly()
            => "(1+2)".Evaluate<RationalNumberType>().Value.Should().Be(3);
    }
}
