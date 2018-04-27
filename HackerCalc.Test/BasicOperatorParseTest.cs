using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class BasicOperatorParseTest
    {
        [DataTestMethod]
        [DataRow("1+2")]
        [DataRow("1+ 2")]
        [DataRow("1 +2")]
        [DataRow("1 + 2")]
        public void AdditionParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.Add);

        [DataTestMethod]
        [DataRow("1-2")]
        [DataRow("1- 2")]
        [DataRow("1 -2")]
        [DataRow("1 - 2")]
        public void SubtractionParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.Subtract);

        [DataTestMethod]
        [DataRow("1*2")]
        [DataRow("1* 2")]
        [DataRow("1 *2")]
        [DataRow("1 * 2")]
        public void MultiplicationParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.Multiply);

        [DataTestMethod]
        [DataRow("1/2")]
        [DataRow("1/ 2")]
        [DataRow("1 /2")]
        [DataRow("1 / 2")]
        [DataRow("1\\2")]
        public void DivisionParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.Divide);

        [DataTestMethod]
        [DataRow("1%2")]
        [DataRow("1% 2")]
        [DataRow("1 %2")]
        [DataRow("1 % 2")]
        public void ModuloParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.Modulo);

        [DataTestMethod]
        [DataRow("1<<2")]
        [DataRow("1<< 2")]
        [DataRow("1 <<2")]
        [DataRow("1 << 2")]
        public void ShiftLeftParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.LeftShift);

        [DataTestMethod]
        [DataRow("1>>2")]
        [DataRow("1>> 2")]
        [DataRow("1 >>2")]
        [DataRow("1 >> 2")]
        public void ShiftRightParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.RightShift);

        [DataTestMethod]
        [DataRow("1&2")]
        [DataRow("1& 2")]
        [DataRow("1 &2")]
        [DataRow("1 & 2")]
        public void AndParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.And);

        [DataTestMethod]
        [DataRow("1|2")]
        [DataRow("1| 2")]
        [DataRow("1 |2")]
        [DataRow("1 | 2")]
        public void OrParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.Or);

        [DataTestMethod]
        [DataRow("1^2")]
        [DataRow("1^ 2")]
        [DataRow("1 ^2")]
        [DataRow("1 ^ 2")]
        public void XorParsesCorrectly(string input) => BinaryOperatorParsesCorrectly(input, OperatorType.Xor);

        [DataTestMethod]
        [DataRow("!1")]
        [DataRow("! 1")]
        public void LogicalNotParsesCorrectly(string input) => UnaryOperatorParsesCorrectly(input, OperatorType.Not);

        [DataTestMethod]
        [DataRow("~1")]
        [DataRow("~ 1")]
        public void BitwiseNegateParsesCorrectly(string input) => UnaryOperatorParsesCorrectly(input, OperatorType.BitwiseNegate);


        void BinaryOperatorParsesCorrectly(string input, OperatorType expectedType)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as OperatorToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid OperatorToken.");

            token.IsUnary.Should().BeFalse("we expect these to be binary operators.");

            var leftToken = token.Left as IntegerToken;
            leftToken.Should().NotBeNull("the left token is expected to be an IntegerToken");
            leftToken.TypedValue.Should().Be(1);

            var rightToken = token.Right as IntegerToken;
            rightToken.Should().NotBeNull("the right token is expected to be an IntegerToken");
            rightToken.TypedValue.Should().Be(2);

            token.Operator.Should().Be(expectedType);
        }

        void UnaryOperatorParsesCorrectly(string input, OperatorType expectedType)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as OperatorToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid OperatorToken.");

            token.IsUnary.Should().BeTrue("we expect these to be unary operators.");

            var leftToken = token.Left as IntegerToken;
            leftToken.Should().NotBeNull("the left token is expected to be an IntegerToken");
            leftToken.TypedValue.Should().Be(1);

            token.Right.Should().BeNull("the right token is always null for a unary operator");

            token.Operator.Should().Be(expectedType);
        }
    }
}
