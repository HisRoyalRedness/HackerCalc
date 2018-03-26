using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

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
            where TToken : class, ILiteralToken, ILiteralToken<TType>
        {
            var expectedRepl = input[1]?.Replace("<DATE>", DateTime.Now.ToString("yyyy-MM-dd"));

            var token = Parser.ParseExpression(input[0]) as ILiteralToken;
            Assert.IsNotNull(token, $"while parsing {input[0]}");

            TType expectedValue = default(TType);
            if (input[1] != null)
            {
                ILiteralToken<TType> expectedToken = Parser.ParseExpression(expectedRepl) as ILiteralToken<TType>;

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

                Assert.IsFalse(expectedValue.Equals(default(TType)), $"while parsing expected value {input[1]} as a {typeof(TToken).Name}");
            }

            if (input[1] == null)
            {
                // If expected is null, we expect the cast to fail
                Assert.ThrowsException<InvalidCastException>(() => token.CastTo<TToken>(), $"Expected the cast to fail when casting input {input[0]} as a {typeof(TToken).Name}");
            }
            else
            {
                // ...else we expect the cast to succeed and we should test the result
                var castToken = token.CastTo<TToken>();
                Assert.IsNotNull(castToken, $"Expected the cast to fail when casting input {input[0]} as a {typeof(TToken).Name}");
                Assert.IsInstanceOfType(castToken, typeof(TToken), $"for input {input[0]}");
                Assert.AreEqual(expectedValue, castToken.TypedValue, $"for input {input[0]}");
            }
        }
    }
}
