using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using System.Globalization;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_PARSE)]
    [TestClass]
    public class NegativeParseTest
    {
        // 1 - 2
        [DataTestMethod]
        [DataRow("1-2", "1 2 -")]
        [DataRow("1i4-2i4", "1i4 2i4 -")]
        [DataRow("1.0-2.0", "1 2 -")]
        public void ASingleMinusIsASimpleExpression(string stringToParse, string expectedParseString)
            => TestCommon.CompareParseTree(stringToParse, expectedParseString);

        // 1 - (-2)
        [DataTestMethod]
        [DataRow("1--2", "1 -2 -")]
        [DataRow("1i4--2i4", "1i4 -2i4 -")]
        [DataRow("1.0--2.0", "1 -2 -")]
        public void ADoubleMinusIsANegativeNumber(string stringToParse, string expectedParseString)
            => TestCommon.CompareParseTree(stringToParse, expectedParseString);

        // 1 - - (-2) ??
        [DataTestMethod]
        [DataRow("1---2")]
        [DataRow("1i4---2i4")]
        [DataRow("1.0---2.0")]
        public void ATripleMinusIsInvalid(string stringToParse)
            => TestCommon.CompareParseTree(stringToParse);

        [DataTestMethod]
        [DataRow("-1-2", "-1 2 -")]
        [DataRow("-1i4-2i4", "-1i4 2i4 -")]
        [DataRow("-1.0-2.0", "-1 2 -")]
        [DataRow("-1--2", "-1 -2 -")]
        [DataRow("-1i4--2i4", "-1i4 -2i4 -")]
        [DataRow("-1.0--2.0", "-1 -2 -")]
        [DataRow("(-1-2)", "-1 2 -")]
        [DataRow("(-1i4-2i4)", "-1i4 2i4 -")]
        [DataRow("(-1.0-2.0)", "-1 2 -")]
        [DataRow("-(-1-2)", "-1 2 - !-")]
        [DataRow("-(-1i4-2i4)", "-1i4 2i4 - !-")]
        [DataRow("-(-1.0-2.0)", "-1 2 - !-")]
        public void AMinusCanLeadTheExpression(string stringToParse, string expectedParseString)
            => TestCommon.CompareParseTree(stringToParse, expectedParseString);

        [DataTestMethod]
        [DataRow("-1")]
        [DataRow("-1i4")]
        [DataRow("-1.0")]
        public void AMinusInFrontOfALiteralIsPartOfTheLiteral(string stringToParse)
        {
            var rawToken = Parser.ParseExpression(stringToParse);
            rawToken.Should().NotBeNull();
            rawToken.Should().BeAssignableTo<ILiteralToken>();

            var litToken = rawToken as ILiteralToken;
            var val = double.Parse(litToken.ObjectValue.ToString());
            val.Should().BeApproximately(-1.0, float.Epsilon);
        }

        [DataTestMethod]
        [DataRow("-(1)")]
        [DataRow("-(1i4)")]
        [DataRow("-(1.0)")]
        [DataRow("-(2018-01-01)")]
        [DataRow("-(01:01:01)")]
        [DataRow("-(1 hour 1 sec)")]
        public void AMinusInFrontOfBracketsIsANegateOperation(string stringToParse)
        {
            var rawToken = Parser.ParseExpression(stringToParse);
            rawToken.Should().NotBeNull();
            rawToken.Should().BeAssignableTo<IOperatorToken>();
            var opToken = rawToken as IOperatorToken;
            opToken.Operator.Should().Be(OperatorType.NumericNegate);

            opToken.Left.Should().NotBeNull();
            opToken.Left.Should().BeAssignableTo<IOperatorToken>();
            var grpToken = opToken.Left as IOperatorToken;
            grpToken.Operator.Should().Be(OperatorType.Grouping);

            grpToken.Left.Should().NotBeNull();
            grpToken.Left.Should().BeAssignableTo<ILiteralToken>();
        }
    }
}
