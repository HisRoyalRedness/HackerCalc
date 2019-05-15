using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(nameof(RationalNumber))]
    [TestClass]
    public class RationalNumberTests
    {
        [TestMethod]
        public void TestDefaultConstruction()
        {
            var rat = new RationalNumber();
            rat.Numerator.Should().Be(0);
            rat.Denominator.Should().Be(0);
        }

        [TestMethod]
        public void TestSingleArgConstruction()
        {
            var rat = new RationalNumber(5);
            rat.Numerator.Should().Be(5);
            rat.Denominator.Should().Be(1);
        }

        [TestMethod]
        public void TestDoubleArgConstruction()
        {
            var rat = new RationalNumber(5,7);
            rat.Numerator.Should().Be(5);
            rat.Denominator.Should().Be(7);
        }

        [DataTestMethod]
        [DataRow("0/4", "0/1")]
        [DataRow("1/4", "1/4")]
        [DataRow("2/4", "1/2")]
        [DataRow("3/4", "3/4")]
        [DataRow("4/4", "1/1")]
        [DataRow("5/4", "5/4")]
        [DataRow("6/4", "3/2")]
        [DataRow("7/4", "7/4")]
        [DataRow("8/4", "2/1")]
        public void TestNormalisation(string actual, string expected)
            => TestCommon.TestRationalNumber(actual, expected);

        [DataTestMethod]
        [DataRow("0/0", "0/1")]
        [DataRow("0/1", "0/1")]
        [DataRow("1/0", null)]
        [DataRow("1/1", "1/1")]
        public void TestSpecialValues(string actual, string expected)
            => TestCommon.TestRationalNumber(actual, expected);

        [DataTestMethod]
        [DataRow("1/2", "1/2")]
        [DataRow("-1/2", "-1/2")]
        [DataRow("1/-2", "-1/2")]
        [DataRow("-1/-2", "1/2")]
        public void TestSigns(string actual, string expected)
            => TestCommon.TestRationalNumber(actual, expected);

        #region Add
        [DataTestMethod]
        [DataRow("1/2", "1/2", "1")]
        [DataRow("1/3", "1/2", "5/6")]
        [DataRow("1/3", "-1/2", "-1/6")]
        public void TestAddOperationRR(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 + r2);

        [DataTestMethod]
        [DataRow("1/2", "2", "5/2")]
        public void TestAddOperationRI(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 + r2);

        [DataTestMethod]
        [DataRow("2", "1/2", "5/2")]
        public void TestAddOperationIR(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 + r2);
        #endregion Add

        #region Subtract
        [DataTestMethod]
        [DataRow("1/2", "1/4", "1/4")]
        [DataRow("1/2", "1/2", "0")]
        [DataRow("1/3", "-1/2", "5/6")]
        public void TestSubtractOperationRR(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 - r2);

        [DataTestMethod]
        [DataRow("1/2", "2", "-3/2")]
        public void TestSubtractOperationRI(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 - r2);

        [DataTestMethod]
        [DataRow("2", "1/2", "3/2")]
        public void TestSubtractOperationIR(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 - r2);
        #endregion Subtract

        #region Multiply
        [DataTestMethod]
        [DataRow("1/2", "1/2", "1/4")]
        [DataRow("2/3", "1/2", "2/6")]
        [DataRow("1/3", "-1/2", "-1/6")]
        public void TestMultiplyOperationRR(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 * r2);

        [DataTestMethod]
        [DataRow("1/2", "2", "1")]
        public void TestMultiplyOperationRI(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 * r2);

        [DataTestMethod]
        [DataRow("2", "1/2", "1")]
        public void TestMultiplyOperationIR(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 * r2);
        #endregion Multiply

        #region Divide
        [DataTestMethod]
        [DataRow("1/2", "1/2", "1")]
        [DataRow("1/3", "1/2", "2/3")]
        [DataRow("1/3", "-1/2", "-2/3")]
        public void TestDivideOperationRR(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 / r2);

        [DataTestMethod]
        [DataRow("1/2", "2", "1")]
        public void TestDivideOperationRI(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 / r2);

        [DataTestMethod]
        [DataRow("2", "1/2", "4")]
        public void TestDivideOperationIR(string a, string b, string expected)
            => OperateOnRationals(a, b, expected, (r1, r2) => r1 / r2);
        #endregion Divide

        void OperateOnRationals(string rat1, string rat2, string expected, Func<RationalNumber, RationalNumber, RationalNumber> operation)
            => operation(rat1.ToRationalNumber(), rat2.ToRationalNumber()).Should().Be(expected.ToRationalNumber());
        void OperateOnRationalAndBigInteger(string rat1, string int2, string expected, Func<RationalNumber, BigInteger, RationalNumber> operation)
            => operation(rat1.ToRationalNumber(), BigInteger.Parse(int2)).Should().Be(expected.ToRationalNumber());
        void OperateOnRationalAndBigInteger(string int1, string rat2, string expected, Func<BigInteger, RationalNumber, RationalNumber> operation)
            => operation(BigInteger.Parse(int1), rat2.ToRationalNumber()).Should().Be(expected.ToRationalNumber());
    }
}

