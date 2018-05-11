using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestClass]
    [TestCategory(TestCommon.INCOMPLETE)]
    public class EqualityTest
    {
        [DataTestMethod]
        [DataRow(new[] { null, null, "y" })]
        [DataRow(new[] { "date 2018-01-01", null, "n" })]
        [DataRow(new[] { null, "date 2018-01-01", "n" })]
        [DataRow(new[] { "date 2018-01-01", "date 2018-01-01", "y" })]
        [DataRow(new[] { "date 2018-01-01", "date 2018-01-02", "n" })]
        public void TestDateTokenEquality(string[] input)
        {
            input.Should().HaveCount(3);

            var op1 = TestCommon.MakeToken(input[0]) as DateToken;
            var op2 = TestCommon.MakeToken(input[1]) as DateToken;
            var shouldBeEqual = input[2] == "y";

            ((op1 == op2) ^ !shouldBeEqual).Should().BeTrue($"for op1==op2. Op1 = {op1}, Op2 = {op2}");
            ((op2 == op1) ^ !shouldBeEqual).Should().BeTrue($"for op2==op1. Op1 = {op1}, Op2 = {op2}");
            ((op1 != op2) ^ !shouldBeEqual).Should().BeFalse($"for op1!=op2. Op1 = {op1}, Op2 = {op2}");
            ((op2 != op1) ^ !shouldBeEqual).Should().BeFalse($"for op2!=op1. Op1 = {op1}, Op2 = {op2}");

            if (op1 != null)
                (op1.Equals(op2) ^ !shouldBeEqual).Should().BeTrue($"for op1.Equals(op2). Op1 = {op1}, Op2 = {op2}");
            if (op2 != null)
                (op2.Equals(op1) ^ !shouldBeEqual).Should().BeTrue($"for op2.Equals(op1). Op1 = {op1}, Op2 = {op2}");
            if (op1 != null && op2 != null)
                ((op1.GetHashCode() == op2.GetHashCode()) ^ !shouldBeEqual).Should().BeTrue($"for op1.GetHashCode()==op2.GetHashCode(). Op1 = {op1}, Op2 = {op2}");
        }

        [DataTestMethod]
        [DataRow(new[] { null, null, "y" })]
        [DataRow(new[] { "float 1.5", null, "n" })]
        [DataRow(new[] { null, "float 1.5", "n" })]
        [DataRow(new[] { "float 1.5", "float 1.5", "y" })]
        [DataRow(new[] { "float 1.5", "float 1.6", "n" })]
        public void TestFloatTokenEquality(string[] input)
        {
            input.Should().HaveCount(3);

            var op1 = TestCommon.MakeToken(input[0]) as FloatToken;
            var op2 = TestCommon.MakeToken(input[1]) as FloatToken;
            var shouldBeEqual = input[2] == "y";

            ((op1 == op2) ^ !shouldBeEqual).Should().BeTrue($"for op1==op2. Op1 = {op1}, Op2 = {op2}");
            ((op2 == op1) ^ !shouldBeEqual).Should().BeTrue($"for op2==op1. Op1 = {op1}, Op2 = {op2}");
            ((op1 != op2) ^ !shouldBeEqual).Should().BeFalse($"for op1!=op2. Op1 = {op1}, Op2 = {op2}");
            ((op2 != op1) ^ !shouldBeEqual).Should().BeFalse($"for op2!=op1. Op1 = {op1}, Op2 = {op2}");

            if (op1 != null)
                (op1.Equals(op2) ^ !shouldBeEqual).Should().BeTrue($"for op1.Equals(op2). Op1 = {op1}, Op2 = {op2}");
            if (op2 != null)
                (op2.Equals(op1) ^ !shouldBeEqual).Should().BeTrue($"for op2.Equals(op1). Op1 = {op1}, Op2 = {op2}");
            if (op1 != null && op2 != null)
                ((op1.GetHashCode() == op2.GetHashCode()) ^ !shouldBeEqual).Should().BeTrue($"for op1.GetHashCode()==op2.GetHashCode(). Op1 = {op1}, Op2 = {op2}");
        }

        //[DataTestMethod]
        //[DataRow(new[] { null, null, "y" })]
        //[DataRow(new[] { "integer 1", null, "n" })]
        //[DataRow(new[] { null, "integer 1", "n" })]
        //[DataRow(new[] { "integer 1", "integer 1", "y" })]
        //[DataRow(new[] { "integer 1", "integer 2", "n" })]
        //[DataRow(new[] { "integer 1i32", "integer 1u32", "y" })]
        //[DataRow(new[] { "integer 15u32", "integer -1i4", "y" })]
        //public void TestIntegerTokenEquality(string[] input)
        //{
        //    input.Should().HaveCount(3);

        //    var op1 = TestCommon.MakeToken(input[0]) as IntegerToken;
        //    var op2 = TestCommon.MakeToken(input[1]) as IntegerToken;
        //    var shouldBeEqual = input[2] == "y";

        //    ((op1 == op2) ^ !shouldBeEqual).Should().BeTrue($"for op1==op2. Op1 = {op1}, Op2 = {op2}");
        //    ((op2 == op1) ^ !shouldBeEqual).Should().BeTrue($"for op2==op1. Op1 = {op1}, Op2 = {op2}");
        //    ((op1 != op2) ^ !shouldBeEqual).Should().BeFalse($"for op1!=op2. Op1 = {op1}, Op2 = {op2}");
        //    ((op2 != op1) ^ !shouldBeEqual).Should().BeFalse($"for op2!=op1. Op1 = {op1}, Op2 = {op2}");

        //    if (op1 != null)
        //        (op1.Equals(op2) ^ !shouldBeEqual).Should().BeTrue($"for op1.Equals(op2). Op1 = {op1}, Op2 = {op2}");
        //    if (op2 != null)
        //        (op2.Equals(op1) ^ !shouldBeEqual).Should().BeTrue($"for op2.Equals(op1). Op1 = {op1}, Op2 = {op2}");
        //    if (op1 != null && op2 != null)
        //        ((op1.GetHashCode() == op2.GetHashCode()) ^ !shouldBeEqual).Should().BeTrue($"for op1.GetHashCode()==op2.GetHashCode(). Op1 = {op1}, Op2 = {op2}");
        //}

        [DataTestMethod]
        [DataRow(new[] { null, null, "y" })]
        [DataRow(new[] { "time 11:23", null, "n" })]
        [DataRow(new[] { null, "time 11:23", "n" })]
        [DataRow(new[] { "time 11:23", "time 11:23", "y" })]
        [DataRow(new[] { "time 11:23", "time 11:24", "n" })]
        public void TestTimeTokenEquality(string[] input)
        {
            input.Should().HaveCount(3);

            var op1 = TestCommon.MakeToken(input[0]) as TimeToken;
            var op2 = TestCommon.MakeToken(input[1]) as TimeToken;
            var shouldBeEqual = input[2] == "y";

            ((op1 == op2) ^ !shouldBeEqual).Should().BeTrue($"for op1==op2. Op1 = {op1}, Op2 = {op2}");
            ((op2 == op1) ^ !shouldBeEqual).Should().BeTrue($"for op2==op1. Op1 = {op1}, Op2 = {op2}");
            ((op1 != op2) ^ !shouldBeEqual).Should().BeFalse($"for op1!=op2. Op1 = {op1}, Op2 = {op2}");
            ((op2 != op1) ^ !shouldBeEqual).Should().BeFalse($"for op2!=op1. Op1 = {op1}, Op2 = {op2}");

            if (op1 != null)
                (op1.Equals(op2) ^ !shouldBeEqual).Should().BeTrue($"for op1.Equals(op2). Op1 = {op1}, Op2 = {op2}");
            if (op2 != null)
                (op2.Equals(op1) ^ !shouldBeEqual).Should().BeTrue($"for op2.Equals(op1). Op1 = {op1}, Op2 = {op2}");
            if (op1 != null && op2 != null)
                ((op1.GetHashCode() == op2.GetHashCode()) ^ !shouldBeEqual).Should().BeTrue($"for op1.GetHashCode()==op2.GetHashCode(). Op1 = {op1}, Op2 = {op2}");
        }

        [DataTestMethod]
        [DataRow(new[] { null, null, "y" })]
        [DataRow(new[] { "timespan 11:23", null, "n" })]
        [DataRow(new[] { null, "timespan 11:23", "n" })]
        [DataRow(new[] { "timespan 11:23", "timespan 11:23", "y" })]
        [DataRow(new[] { "timespan 11:23", "timespan 11:24", "n" })]
        public void TestTimespanTokenEquality(string[] input)
        {
            input.Should().HaveCount(3);

            var op1 = TestCommon.MakeToken(input[0]) as TimespanToken;
            var op2 = TestCommon.MakeToken(input[1]) as TimespanToken;
            var shouldBeEqual = input[2] == "y";

            ((op1 == op2) ^ !shouldBeEqual).Should().BeTrue($"for op1==op2. Op1 = {op1}, Op2 = {op2}");
            ((op2 == op1) ^ !shouldBeEqual).Should().BeTrue($"for op2==op1. Op1 = {op1}, Op2 = {op2}");
            ((op1 != op2) ^ !shouldBeEqual).Should().BeFalse($"for op1!=op2. Op1 = {op1}, Op2 = {op2}");
            ((op2 != op1) ^ !shouldBeEqual).Should().BeFalse($"for op2!=op1. Op1 = {op1}, Op2 = {op2}");

            if (op1 != null)
                (op1.Equals(op2) ^ !shouldBeEqual).Should().BeTrue($"for op1.Equals(op2). Op1 = {op1}, Op2 = {op2}");
            if (op2 != null)
                (op2.Equals(op1) ^ !shouldBeEqual).Should().BeTrue($"for op2.Equals(op1). Op1 = {op1}, Op2 = {op2}");
            if (op1 != null && op2 != null)
                ((op1.GetHashCode() == op2.GetHashCode()) ^ !shouldBeEqual).Should().BeTrue($"for op1.GetHashCode()==op2.GetHashCode(). Op1 = {op1}, Op2 = {op2}");
        }
    }
}