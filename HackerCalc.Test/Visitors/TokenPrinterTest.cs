using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.VISITOR)]
    [TestCategory(TestCommon.TOKEN_PRINTER)]
    [TestClass]
    public class TokenPrinterTest
    {
        [DataTestMethod]
        [DataRow("1", "1")]
        [DataRow("-1", "-1")]
        [DataRow("1i4", "1I4")]
        [DataRow("-1i4", "-1I4")]
        [DataRow("1u4", "1U4")]
        [DataRow("2018/02/23", "2018-02-23 00:00:00")]
        [DataRow("2018/02/23 12:34:56", "2018-02-23 12:34:56")]
        [DataRow("12:34", "12:34:00")]
        [DataRow("12:34:56", "12:34:56")]
        [DataRow("12:34:56", "12:34:56")]
        [DataRow("1f", "1.000")]
        [DataRow(".5", "0.500")]
        public void TestTokenPrintingWithBasicLiteralTypes(string input, string output)
            => TestTokenPrinterAllFix(input, output);

        [DataTestMethod]
        [DataRow("1d", "'1 day'")]
        [DataRow("2d", "'2 days'")]
        [DataRow("1h", "'1 hour'")]
        [DataRow("2h", "'2 hours'")]
        [DataRow("1m", "'1 minute'")]
        [DataRow("2m", "'2 minutes'")]
        [DataRow("1s", "'1 second'")]
        [DataRow("2s", "'2 seconds'")]
        public void TestTimestampPluralPrinting(string input, string output)
            => TestTokenPrinterAllFix(input, output);

        [DataTestMethod]
        [DataRow("1s", "'1 second'")]
        [DataRow("1m", "'1 minute'")]
        [DataRow("1m1s", "'1 minute 1 second'")]
        [DataRow("1h", "'1 hour'")]
        [DataRow("1h1s", "'1 hour 1 second'")]
        [DataRow("1h1m", "'1 hour 1 minute'")]
        [DataRow("1h1m1s", "'1 hour 1 minute 1 second'")]
        [DataRow("1d1s", "'1 day 1 second'")]
        [DataRow("1d1m", "'1 day 1 minute'")]
        [DataRow("1d1m1s", "'1 day 1 minute 1 second'")]
        [DataRow("1d1h", "'1 day 1 hour'")]
        [DataRow("1d1h1s", "'1 day 1 hour 1 second'")]
        [DataRow("1d1h1m", "'1 day 1 hour 1 minute'")]
        [DataRow("1d1h1m1s", "'1 day 1 hour 1 minute 1 second'")]
        public void TestTimestampCompoundValues(string input, string output)
            => TestTokenPrinterAllFix(input, output);

        [DataTestMethod]
        [DataRow("1.0s", "'1 second'")]
        [DataRow(".4s", "'0.400 seconds'")]
        [DataRow(".4m", "'24 seconds'")]
        [DataRow(".44h", "'26 minutes 24 seconds'")]
        [DataRow(".444h", "'26 minutes 38.400 seconds'")]
        [DataRow(".444d", "'10 hours 39 minutes 21.600 seconds'")]
        public void TestTimestampFloatingValues(string input, string output)
            => TestTokenPrinterAllFix(input, output);

        [DataTestMethod]
        [DataRow("1+2", "+ 1 2")]
        [DataRow("1-2", "- 1 2")]
        [DataRow("1*2", "* 1 2")]
        [DataRow("1\\2", "/ 1 2")]
        [DataRow("1**2", "** 1 2")]
        [DataRow("1//2", "// 1 2")]
        [DataRow("1%2", "% 1 2")]
        [DataRow("1<<2", "<< 1 2")]
        [DataRow("1>>2", ">> 1 2")]
        [DataRow("1&2", "& 1 2")]
        [DataRow("1|2", "| 1 2")]
        [DataRow("1^2", "^ 1 2")]
        public void TestBinaryOperatorPrefixPrinting(string input, string output)
            => TestTokenPrinter(input, TokenPrinter.FixType.Prefix, output);

        [DataTestMethod]
        [DataRow("~1", "~ 1")]
        [DataRow("-1", "-1")] // minus 1
        [DataRow("-(1)", "!- 1")] // negated 1
#if SUPPORT_NOT
        [DataRow("!1", "! 1")]
#endif
        public void TestUnaryOperatorPrefixPrinting(string input, string output)
            => TestTokenPrinter(input, TokenPrinter.FixType.Prefix, output);

        [DataTestMethod]
        [DataRow("1+2", "1 + 2")]
        [DataRow("1-2", "1 - 2")]
        [DataRow("1*2", "1 * 2")]
        [DataRow("1\\2", "1 / 2")]
        [DataRow("1**2", "1 ** 2")]
        [DataRow("1//2", "1 // 2")]
        [DataRow("1%2", "1 % 2")]
        [DataRow("1<<2", "1 << 2")]
        [DataRow("1>>2", "1 >> 2")]
        [DataRow("1&2", "1 & 2")]
        [DataRow("1|2", "1 | 2")]
        [DataRow("1^2", "1 ^ 2")]
        public void TestBinaryOperatorInfixPrinting(string input, string output)
            => TestTokenPrinter(input, TokenPrinter.FixType.Infix, output);

        [DataTestMethod]
        [DataRow("~1", "~ 1")]
        [DataRow("-1", "-1")] // minus 1
        [DataRow("-(1)", "!- ( 1 )")] // negated 1
#if SUPPORT_NOT
        [DataRow("!1", "! 1")]
#endif
        public void TestUnaryOperatorInfixPrinting(string input, string output)
            => TestTokenPrinter(input, TokenPrinter.FixType.Infix, output);

        [DataTestMethod]
        [DataRow("1+2", "1 2 +")]
        [DataRow("1-2", "1 2 -")]
        [DataRow("1*2", "1 2 *")]
        [DataRow("1\\2", "1 2 /")]
        [DataRow("1**2", "1 2 **")]
        [DataRow("1//2", "1 2 //")]
        [DataRow("1%2", "1 2 %")]
        [DataRow("1<<2", "1 2 <<")]
        [DataRow("1>>2", "1 2 >>")]
        [DataRow("1&2", "1 2 &")]
        [DataRow("1|2", "1 2 |")]
        [DataRow("1^2", "1 2 ^")]
        public void TestBinaryOperatorPostfixPrinting(string input, string output)
            => TestTokenPrinter(input, TokenPrinter.FixType.Postfix, output);

        [DataTestMethod]
        [DataRow("~1", "1 ~")]
        [DataRow("-1", "-1")] // minus 1
        [DataRow("-(1)", "1 !-")] // negated 1
#if SUPPORT_NOT
        [DataRow("!1", "1 !")]
#endif
        public void TestUnaryOperatorPostfixPrinting(string input, string output)
            => TestTokenPrinter(input, TokenPrinter.FixType.Postfix, output);

        void TestTokenPrinterAllFix(string expresion, string expectedOutput)
        { 
            TestTokenPrinter(expresion, TokenPrinter.FixType.Prefix, expectedOutput);
            TestTokenPrinter(expresion, TokenPrinter.FixType.Infix, expectedOutput);
            TestTokenPrinter(expresion, TokenPrinter.FixType.Postfix, expectedOutput);
        }

        void TestTokenPrinter(string expresion, string fixType, string expectedOutput)
            => TestTokenPrinter(expresion, (TokenPrinter.FixType)Enum.Parse(typeof(TokenPrinter.FixType), fixType), expectedOutput);
        void TestTokenPrinter(string expresion, TokenPrinter.FixType fixType, string expectedOutput)
        {
            var expr = Parser.ParseExpression(expresion);
            expr.Should().NotBeNull($"parsing '{expresion}' should succeed.");
            expr.Accept(new TokenPrinter(fixType)).Trim().Should().Be(expectedOutput.Trim(), $"output should match with {fixType}");
        }

    }
}
