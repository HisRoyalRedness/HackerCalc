using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_PARSE)]
    [TestCategory(TestCommon.OPERATOR_TOKEN_PARSE)]
    [TestClass]
    public class BasicOperatorParseTest
    {
        [DataTestMethod]
        [DataRow("1+2")]
        [DataRow("1+ 2")]
        [DataRow("1 +2")]
        [DataRow("1 + 2")]
        public void AdditionParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Add);

        [DataTestMethod]
        [DataRow("1-2")]
        [DataRow("1- 2")]
        [DataRow("1 -2")]
        [DataRow("1 - 2")]
        public void SubtractionParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Subtract);

        [DataTestMethod]
        [DataRow("1*2")]
        [DataRow("1* 2")]
        [DataRow("1 *2")]
        [DataRow("1 * 2")]
        public void MultiplicationParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Multiply);

        [DataTestMethod]
        [DataRow("1/2")]
        [DataRow("1/ 2")]
        [DataRow("1 /2")]
        [DataRow("1 / 2")]
        [DataRow("1\\2")]
        [DataRow("1\\ 2")]
        [DataRow("1 \\2")]
        [DataRow("1 \\ 2")]
        public void DivisionParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Divide);

        [DataTestMethod]
        [DataRow("1%2")]
        [DataRow("1% 2")]
        [DataRow("1 %2")]
        [DataRow("1 % 2")]
        public void ModuloParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Modulo);

        [DataTestMethod]
        [DataRow("1<<2")]
        [DataRow("1<< 2")]
        [DataRow("1 <<2")]
        [DataRow("1 << 2")]
        public void ShiftLeftParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.LeftShift);

        [DataTestMethod]
        [DataRow("1>>2")]
        [DataRow("1>> 2")]
        [DataRow("1 >>2")]
        [DataRow("1 >> 2")]
        public void ShiftRightParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.RightShift);

        [DataTestMethod]
        [DataRow("1&2")]
        [DataRow("1& 2")]
        [DataRow("1 &2")]
        [DataRow("1 & 2")]
        public void AndParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.And);

        [DataTestMethod]
        [DataRow("1|2")]
        [DataRow("1| 2")]
        [DataRow("1 |2")]
        [DataRow("1 | 2")]
        public void OrParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Or);

        [DataTestMethod]
        [DataRow("1^2")]
        [DataRow("1^ 2")]
        [DataRow("1 ^2")]
        [DataRow("1 ^ 2")]
        public void XorParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Xor);

        [DataTestMethod]
        [DataRow("1**2")]
        [DataRow("1** 2")]
        [DataRow("1 **2")]
        [DataRow("1 ** 2")]
        public void PowerParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Power);

        [DataTestMethod]
        [DataRow("1//2")]
        [DataRow("1// 2")]
        [DataRow("1 //2")]
        [DataRow("1 // 2")]
        [DataRow("1\\\\2")]
        [DataRow("1\\\\ 2")]
        [DataRow("1 \\\\2")]
        [DataRow("1 \\\\ 2")]
        public void RootParsesCorrectly(string input) 
            => TestCommon.BinaryOperatorParsesCorrectly(input, OperatorType.Root);

        [DataTestMethod]
        [DataRow("!1")]
        [DataRow("! 1")]
        public void LogicalNotParsesCorrectly(string input) 
            => TestCommon.UnaryOperatorParsesCorrectly(input, OperatorType.Not);

        [DataTestMethod]
        [DataRow("~1")]
        [DataRow("~ 1")]
        public void BitwiseNegateParsesCorrectly(string input) 
            => TestCommon.UnaryOperatorParsesCorrectly(input, OperatorType.BitwiseNegate);
    }
}
