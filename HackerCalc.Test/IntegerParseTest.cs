using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class IntegerParseTest
    {
        [DataTestMethod]
        [DataRow("255")]
        [DataRow("255U32")]
        [DataRow("255I32")]
        public void DecimalIntegerValueIsParsedCorrectly(string input)
        {
            var token = Parser.ParseExpression(input) as IntegerToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(255, token.TypedValue);
        }

        [DataTestMethod]
        [DataRow("0xff")]
        [DataRow("0XFF")]
        [DataRow("0xffU32")]
        [DataRow("0xffI32")]
        public void HexadecimalIntegerValueIsParsedCorrectly(string input)
        {
            var token = Parser.ParseExpression(input) as IntegerToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(255, token.TypedValue);
        }

        [DataTestMethod]
        [DataRow("255U32")]
        [DataRow("255I32")]
        [DataRow("0xffI32")]
        [DataRow("0xffU32")]
        public void IntegerWithAttachedSignFlagAndBitwidthIsParsedCorrectly(string input)
        {
            var token = Parser.ParseExpression(input) as IntegerToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(255, token.TypedValue);
        }

        [DataTestMethod]
        [DataRow("1 U32")]
        [DataRow("1 I32")]
        [DataRow("0xff I32")]
        [DataRow("0xff U32")]
        public void IntegerWithUnattachedSignFlagIsNotParsed(string input)
        {
            var token = Parser.ParseExpression(input);
            Assert.IsNull(token);
        }

        [DataTestMethod]
        [DataRow("1U 32")]
        [DataRow("1I 32")]
        [DataRow("0xffI 32")]
        [DataRow("0xffU 32")]
        public void IntegerWithUnattachedBitwidthIsNotParsed(string input)
        {
            var token = Parser.ParseExpression(input) as IntegerToken;
            Assert.IsNull(token);
        }

        [DataTestMethod]
        [DataRow("1I4")]
        [DataRow("1I8")]
        [DataRow("1I16")]
        [DataRow("1I32")]
        [DataRow("1I64")]
        [DataRow("1I128")]
        [DataRow("1i4")]
        [DataRow("1i8")]
        [DataRow("1i16")]
        [DataRow("1i32")]
        [DataRow("1i64")]
        [DataRow("1i128")]
        [DataRow("0xffI4")]
        [DataRow("0xffI8")]
        [DataRow("0xffI16")]
        [DataRow("0xffI32")]
        [DataRow("0xffI64")]
        [DataRow("0xffI128")]
        [DataRow("0xffi4")]
        [DataRow("0xffi8")]
        [DataRow("0xffi16")]
        [DataRow("0xffi32")]
        [DataRow("0xffi64")]
        [DataRow("0xffi128")]
        public void SignedIntegerSignFlagsAreParsedCorrectly(string input)
        {
            var token = Parser.ParseExpression(input) as IntegerToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(true, token.IsSigned);
        }

        [DataTestMethod]
        [DataRow("1U4")]
        [DataRow("1U8")]
        [DataRow("1U16")]
        [DataRow("1U32")]
        [DataRow("1U64")]
        [DataRow("1U128")]
        [DataRow("1u4")]
        [DataRow("1u8")]
        [DataRow("1u16")]
        [DataRow("1u32")]
        [DataRow("1u64")]
        [DataRow("1u128")]
        [DataRow("0xffU4")]
        [DataRow("0xffU8")]
        [DataRow("0xffU16")]
        [DataRow("0xffU32")]
        [DataRow("0xffU64")]
        [DataRow("0xffU128")]
        [DataRow("0xffu4")]
        [DataRow("0xffu8")]
        [DataRow("0xffu16")]
        [DataRow("0xffu32")]
        [DataRow("0xffu64")]
        [DataRow("0xffu128")]
        public void UnsignedIntegerSignFlagsAreParsedCorrectly(string input)
        {
            var token = Parser.ParseExpression(input) as IntegerToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(false, token.IsSigned);
        }

        [DataTestMethod]
        [DataRow("1")]
        [DataRow("0xff")]        
        public void IntegerDefaultSignFlagIsSigned(string input)
        {
            var token = Parser.ParseExpression(input) as IntegerToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(true, token.IsSigned);
        }

        [DataTestMethod]
        [DataRow("255")]
        [DataRow("0xff")]
        public void IntegerDefaultBitWidthIs32(string input)
        {
            var token = Parser.ParseExpression(input) as IntegerToken;

            Assert.IsNotNull(token);
            Assert.AreEqual(IntegerToken.IntegerBitWidth._32, token.BitWidth);
        }

        [DataTestMethod]
        [DataRow(new[] { "1U4", "4" })]
        [DataRow(new[] { "1U8", "8" })]
        [DataRow(new[] { "1U16", "16" })]
        [DataRow(new[] { "1U32", "32" })]
        [DataRow(new[] { "1U64", "64" })]
        [DataRow(new[] { "1U128", "128" })]
        [DataRow(new[] { "1u4", "4" })]
        [DataRow(new[] { "1u8", "8" })]
        [DataRow(new[] { "1u16", "16" })]
        [DataRow(new[] { "1u32", "32" })]
        [DataRow(new[] { "1u64", "64" })]
        [DataRow(new[] { "1u128", "128" })]
        [DataRow(new[] { "0xffU4", "4" })]
        [DataRow(new[] { "0xffU8", "8" })]
        [DataRow(new[] { "0xffU16", "16" })]
        [DataRow(new[] { "0xffU32", "32" })]
        [DataRow(new[] { "0xffU64", "64" })]
        [DataRow(new[] { "0xffU128", "128" })]
        [DataRow(new[] { "0xffu4", "4" })]
        [DataRow(new[] { "0xffu8", "8" })]
        [DataRow(new[] { "0xffu16", "16" })]
        [DataRow(new[] { "0xffu32", "32" })]
        [DataRow(new[] { "0xffu64", "64" })]
        [DataRow(new[] { "0xffu128", "128" })]
        public void IntegerBitwidthIsParsedCorrectly(string[] input)
        {
            var token = Parser.ParseExpression(input[0]) as IntegerToken;
            var expectedBitwidth = (IntegerToken.IntegerBitWidth)(int.Parse(input[1]));

            Assert.IsNotNull(token);
            Assert.AreEqual(expectedBitwidth, token.BitWidth);
        }
    }
}
