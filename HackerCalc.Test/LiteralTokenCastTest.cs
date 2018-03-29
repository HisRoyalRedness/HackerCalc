﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class LiteralTokenCastTest
    {
        [DataTestMethod]
        [DataRow(new[] { "2015-02-02 09:39:12", "2015-02-02 09:39:12" })]
        [DataRow(new[] { "13.4F", "<DATE> 00:00:13.4" })]
        [DataRow(new[] { "13I32", "<DATE> 00:00:13" })]
        [DataRow(new[] { "2min 13 sec", "<DATE> 00:02:13" })]
        [DataRow(new[] { "12:34:58", "<DATE> 12:34:58" })]
        public void CastToDateToken(string[] input)
            => TokenCastBase<DateToken, DateTime>(input);


        [DataTestMethod]
        [DataRow(new[] { "2015-02-02 09:39:12", null })]
        [DataRow(new[] { "13.4F", "13.4F" })]
        [DataRow(new[] { "13I32", "13F" })]
        [DataRow(new[] { "2min 13 sec", null })]
        [DataRow(new[] { "12:34:58", null })]
        public void CastToFloatToken(string[] input)
            => TokenCastBase<FloatToken, double>(input);


        [DataTestMethod]
        [DataRow(new[] { "2015-02-02 09:39:12", null })]
        [DataRow(new[] { "13.4F", "13" })]
        [DataRow(new[] { "13I32", "13I32" })]
        [DataRow(new[] { "2min 13 sec", null })]
        [DataRow(new[] { "12:34:58", null })]
        public void CastToIntegerToken(string[] input)
            => TokenCastBase<IntegerToken, BigInteger>(input);


        [DataTestMethod]
        [DataRow(new[] { "2015-02-02 09:39:12", null })]
        [DataRow(new[] { "13.4F", "13.4 sec" })]
        [DataRow(new[] { "13I32", "13 sec" })]
        [DataRow(new[] { "2min 13 sec", "2min 13 sec" })]
        [DataRow(new[] { "12:34:58", "12:34:58" })]
        public void CastToTimespanToken(string[] input)
            => TokenCastBase<TimespanToken, TimeSpan>(input);


        [DataTestMethod]
        [DataRow(new[] { "2015-02-02 09:39:12", null })]
        [DataRow(new[] { "13.4F", "00:00:13.4" })]
        [DataRow(new[] { "13I32", "00:00:13" })]
        [DataRow(new[] { "2min 13 sec", "00:02:13" })]
        [DataRow(new[] { "12:34:58", "12:34:58" })]
        public void CastToTimeToken(string[] input)
            => TokenCastBase<TimeToken, TimeSpan>(input);


        public void TokenCastBase<TToken, TType>(string[] input)
            where TToken : class, ILiteralToken, ILiteralToken<TType, TToken>
        {
            input.Should().HaveCount(2);

            var expectedRepl = input[1]?.Replace("<DATE>", DateTime.Now.ToString("yyyy-MM-dd"));

            var expr = Parser.ParseExpression(input[0]);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as ILiteralToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid ILiteralToken.");

            TType expectedValue = default(TType);
            if (input[1] != null)
            {
                var expectedToken = Parser.ParseExpression(expectedRepl) as TToken;

                if (expectedToken != null)
                    expectedValue = expectedToken.TypedValue;
                
                // Maybe the token parsing doesn't support this? Try parsing the raw value
                else
                {
                    var methodInfo = typeof(TType).GetMethods().Where(m => m.Name == "Parse" && m.GetParameters().Length == 1).FirstOrDefault();
                    if (methodInfo == null)
                        throw new InvalidOperationException("Could not find a Parse method");

                    expectedValue = (TType)methodInfo.Invoke(null, new[] { expectedRepl });
                }

                expectedValue.Should().NotBe(default(TType), $"value {input[1]} should not parse as a {typeof(TToken).Name} into its default value");
            }

            // If expected is null, we expect the cast to fail
            if (input[1] == null)
            {
                Action cast = () => token.CastTo<TToken>();
                cast.Should().Throw<InvalidCastException>($"{input[0]} should not be able to be cast to a {typeof(TToken).Name}");
            }

            // ...else we expect the cast to succeed and we should test the result
            else
            {
                var castToken = token.CastTo<TToken>();
                castToken.Should().NotBeNull($"input {input[0]} should be able to cast to a {typeof(TToken).Name}");
                castToken.Should().BeAssignableTo<TToken>($"for input {input[0]}");
                castToken.TypedValue.Should().Be(expectedValue, $"for input {input[0]}");
            }
        }
    }
}
