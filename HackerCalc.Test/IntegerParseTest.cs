using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;

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
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            token.TypedValue.Should().Be(255);
        }

        [DataTestMethod]
        [DataRow("0xff")]
        [DataRow("0XFF")]
        [DataRow("0xffU32")]
        [DataRow("0xffI32")]
        public void HexadecimalIntegerValueIsParsedCorrectly(string input)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            token.TypedValue.Should().Be(255);
        }

        [DataTestMethod]
        [DataRow("255U32")]
        [DataRow("255I32")]
        [DataRow("0xffI32")]
        [DataRow("0xffU32")]
        public void IntegerWithAttachedSignFlagAndBitwidthIsParsedCorrectly(string input)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            token.TypedValue.Should().Be(255);
        }

        [DataTestMethod]
        [DataRow("1 U32")]
        [DataRow("1 I32")]
        [DataRow("0xff I32")]
        [DataRow("0xff U32")]
        public void IntegerWithUnattachedSignFlagIsNotParsed(string input)
        {
            Parser.ParseExpression(input).Should().BeNull($"'{input}' should not parse");
        }

        [DataTestMethod]
        [DataRow("1U 32")]
        [DataRow("1I 32")]
        [DataRow("0xffI 32")]
        [DataRow("0xffU 32")]
        public void IntegerWithUnattachedBitwidthIsNotParsed(string input)
        {
            Parser.ParseExpression(input).Should().BeNull($"'{input}' should not parse");
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
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            token.IsSigned.Should().BeTrue();
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
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            token.IsSigned.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("1")]
        [DataRow("0xff")]        
        public void IntegerDefaultSignFlagIsSigned(string input)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            token.IsSigned.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("255")]
        [DataRow("0xff")]
        public void IntegerDefaultBitWidthIsUnbound(string input)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            token.BitWidth.Should().Be(IntegerToken.IntegerBitWidth.Unbound);
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
            input.Should().HaveCount(2);

            var expr = Parser.ParseExpression(input[0]);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            var expectedBitwidth = (IntegerToken.IntegerBitWidth)(int.Parse(input[1]));

            token.BitWidth.Should().Be(expectedBitwidth);
        }

        [DataTestMethod]
        [DataRow("4")]
        [DataRow("8")]
        [DataRow("16")]
        [DataRow("32")]
        [DataRow("64")]
        [DataRow("128")]
        public void IntegersAreNotParsedAsBitwidthLiterals(string input)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IntegerToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IntegerToken.");

            var expectedValue = int.Parse(input);

            token.TypedValue.Should().Be(expectedValue);
        }
        /*
              Min                                       Max                                      Mask
          U4: 0                                         15                                       15
          I4: -8                                        7                                        15
          U8: 0                                         255                                      255
          I8: -128                                      127                                      255
         U16: 0                                         65535                                    65535
         I16: -32768                                    32767                                    65535
         U32: 0                                         4294967295                               4294967295
         I32: -2147483648                               2147483647                               4294967295
         U64: 0                                         18446744073709551615                     18446744073709551615
         I64: -9223372036854775808                      9223372036854775807                      18446744073709551615
        U128: 0                                         340282366920938463463374607431768211455  340282366920938463463374607431768211455
        I128: -170141183460469231731687303715884105728  170141183460469231731687303715884105727  340282366920938463463374607431768211455
        */

        [DataTestMethod]
        [DataRow(new[] { "integer 1i4", "1" })]
        [DataRow(new[] { "integer -1i4", "15" })]
        [DataRow(new[] { "integer 9i4", "9" })]
        [DataRow(new[] { "integer -9i4", "7" })]
        [DataRow(new[] { "integer 1u4", "1" })]
        [DataRow(new[] { "integer -1u4", "" })]
        [DataRow(new[] { "integer 17u4", "1" })]
        [DataRow(new[] { "integer 1i8", "1" })]
        [DataRow(new[] { "integer -1i8", "255" })]
        [DataRow(new[] { "integer -129i8", "127" })]
        [DataRow(new[] { "integer 1u8", "1" })]
        [DataRow(new[] { "integer -1u8", "" })]
        [DataRow(new[] { "integer 257u8", "1" })]
        [DataRow(new[] { "integer 1i16", "1" })]
        [DataRow(new[] { "integer -1i16", "65535" })]
        [DataRow(new[] { "integer -32769i16", "32767" })]
        [DataRow(new[] { "integer 1u16", "1" })]
        [DataRow(new[] { "integer -1u16", "" })]
        [DataRow(new[] { "integer 65537u16", "1" })]
        [DataRow(new[] { "integer 1i32", "1" })]
        [DataRow(new[] { "integer -1i32", "4294967295" })]
        [DataRow(new[] { "integer -2147483649i32", "2147483647" })]
        [DataRow(new[] { "integer 1u32", "1" })]
        [DataRow(new[] { "integer -1u32", "" })]
        [DataRow(new[] { "integer 4294967297u32", "1" })]
        [DataRow(new[] { "integer 1i64", "1" })]
        [DataRow(new[] { "integer -1i64", "18446744073709551615" })]
        [DataRow(new[] { "integer -9223372036854775809i64", "9223372036854775807" })]
        [DataRow(new[] { "integer 1u64", "1" })]
        [DataRow(new[] { "integer -1u64", "" })]
        [DataRow(new[] { "integer 18446744073709551617u64", "1" })]
        [DataRow(new[] { "integer 1i128", "1" })]
        [DataRow(new[] { "integer -1i128", "340282366920938463463374607431768211455" })]
        [DataRow(new[] { "integer -170141183460469231731687303715884105729i128", "170141183460469231731687303715884105727" })]
        [DataRow(new[] { "integer 1u128", "1" })]
        [DataRow(new[] { "integer -1u128", "" })]
        [DataRow(new[] { "integer 340282366920938463463374607431768211457u128", "1" })]
        public void IntegersAreNormalisedCorrectly(string[] input)
        {
            if (string.IsNullOrWhiteSpace(input[1]))
            {
                ((Action)(() => TestCommon.MakeToken<IntegerToken>(input[0]))).Should().Throw<ArgumentOutOfRangeException>("negatives aren't allowed for unsigned ints");
                return;
            }

            var token = TestCommon.MakeToken<IntegerToken>(input[0]);
            token.Should().NotBeNull($"making '{input[0]}' should result in a valid IntegerToken.");

            var expectedValue = BigInteger.Parse(input[1]);
            token.TypedValue.Should().Be(expectedValue, $"for input {input[0]}");
        }
    }
}
