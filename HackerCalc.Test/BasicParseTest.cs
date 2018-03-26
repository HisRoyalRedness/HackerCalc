using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class BasicParseTest
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
        public void BitwiseNegateParsesCorrectly(string input) => UnaryOperatorParsesCorrectly(input, OperatorType.Negate);


        void BinaryOperatorParsesCorrectly(string input, OperatorType expectedType)
        {
            var token = Parser.ParseExpression(input) as OperatorToken;
            Assert.IsNotNull(token);

            Assert.IsFalse(token.IsUnary);

            var leftToken = token.Left as IntegerToken;
            Assert.IsNotNull(leftToken);
            Assert.AreEqual(1, leftToken.TypedValue);

            var rightToken = token.Right as IntegerToken;
            Assert.IsNotNull(rightToken);
            Assert.AreEqual(2, rightToken.TypedValue);

            Assert.AreEqual(expectedType, token.Operator);
        }

        void UnaryOperatorParsesCorrectly(string input, OperatorType expectedType)
        {
            var token = Parser.ParseExpression(input) as OperatorToken;
            Assert.IsNotNull(token);

            Assert.IsTrue(token.IsUnary);

            var leftToken = token.Left as IntegerToken;
            Assert.IsNotNull(leftToken);
            Assert.AreEqual(1, leftToken.TypedValue);

            Assert.IsNull(token.Right);

            Assert.AreEqual(expectedType, token.Operator);
        }
    }
}
