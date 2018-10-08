//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Linq;
//using System.Numerics;
//using FluentAssertions;
//using System.Globalization;

//namespace HisRoyalRedness.com
//{
//    [TestCategory(TestCommon.BASIC_PARSE)]
//    [TestClass]
//    public class NegativeParseTest
//    {
//        // 1 - 2
//        [DataTestMethod]
//        [DataRow("1-2")]
//        [DataRow("1i4-2i4")]
//        [DataRow("1.0-2.0")]
//        public void ASingleMinusIsASimpleExpression(string stringToParse)
//            => TestEvaluationResult(stringToParse, "-1");

//        // 1 - (-2)
//        [DataTestMethod]
//        [DataRow("1--2")] 
//        [DataRow("1i4--2i4")]
//        [DataRow("1.0--2.0")]
//        public void ADoubleMinusIsANegativeNumber(string stringToParse)
//            => TestEvaluationResult(stringToParse, "3");

//        // 1 - - (-2) ??
//        [DataTestMethod]
//        [DataRow("1---2")]
//        [DataRow("1i4---2i4")]
//        [DataRow("1.0---2.0")]
//        public void ATripleMinusIsInvalid(string stringToParse)
//            => TestEvaluationResult(stringToParse, null);


//        [DataTestMethod]
//        [DataRow("-1-2", "-3")]
//        [DataRow("-1i4-2i4", "-3")]
//        [DataRow("-1.0-2.0", "-3")]
//        [DataRow("-1--2", "1")]
//        [DataRow("-1i4--2i4", "1")]
//        [DataRow("-1.0--2.0", "1")]
//        [DataRow("(-1-2)", "-3")]
//        [DataRow("(-1i4-2i4)", "-3")]
//        [DataRow("(-1.0-2.0)", "-3")]
//        [DataRow("-(-1-2)", "3")]
//        [DataRow("-(-1i4-2i4)", "3")]
//        [DataRow("-(-1.0-2.0)", "3")]
//        public void AMinusCanLeadTheExpression(string stringToParse, string expectedValueStr)
//            => TestEvaluationResult(stringToParse, expectedValueStr);

//        [DataTestMethod]
//        [DataRow("-1", "-1")]
//        [DataRow("-1i4", "-1")]
//        [DataRow("-1.0", "-1")]
//        public void AMinusInFrontOfALiteralIsPartOfTheLiteral(string stringToParse, string expectedValueStr)
//        {
//            var rawToken = Parser.ParseExpression(stringToParse);
//            rawToken.Should().NotBeNull();
//            rawToken.Should().BeAssignableTo<IOldLiteralToken>();

//            var expectedValue = double.Parse(expectedValueStr);
//            var actualValue = double.Parse(rawToken.ToString());
//            actualValue.Should().BeApproximately(expectedValue, float.Epsilon);
//        }

//        [DataTestMethod]
//        [DataRow("-(1)")]
//        [DataRow("-(1i4)")]
//        [DataRow("-(1.0)")]
//        [DataRow("-(2018-01-01)")]
//        [DataRow("-(01:01:01)")]
//        [DataRow("-(1 hour 1 sec)")]
//        public void AMinusInFrontOfBracketsIsANegateOperation(string stringToParse)
//        {
//            var rawToken = Parser.ParseExpression(stringToParse);
//            rawToken.Should().NotBeNull();
//            rawToken.Should().BeAssignableTo<IOperatorToken>();
//            var opToken = rawToken as OperatorToken;
//            opToken.Operator.Should().Be(OperatorType.NumericNegate);

//            opToken.Left.Should().NotBeNull();
//            opToken.Left.Should().BeAssignableTo<IOldLiteralToken>();
//        }


//        void TestEvaluationResult(string stringToParse, string expectedValueStr)
//        {
//            IToken rawToken = null;
//            try
//            {
//                rawToken = Parser.ParseExpression(stringToParse)?.Evaluate();
//            }
//            catch (ParseException)
//            {
//                if (string.IsNullOrWhiteSpace(expectedValueStr))
//                    return;
//            }

//            if (string.IsNullOrWhiteSpace(expectedValueStr))
//            {
//                rawToken.Should().BeNull();
//                return;
//            }

//            rawToken.Should().NotBeNull();

//            var expectedValue = double.Parse(expectedValueStr);
//            var actualValue = double.Parse(rawToken.ToString());

//            actualValue.Should().BeApproximately(expectedValue, float.Epsilon);
            
//        }
//    }
//}
