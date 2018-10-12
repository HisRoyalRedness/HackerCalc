using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.BASIC_PARSE)]
    [TestCategory(TestCommon.FUNCTION_TOKEN_PARSE)]
    [TestClass]
    public class FunctionParseTest
    {
        [DataTestMethod]
        [DataRow("_func(1)")]
        [DataRow("_(1)")]
        public void FunctionNameCanStartWithAnUnderscore(string input)
            => FunctionParsesCorrectly(input);

        [DataTestMethod]
        [DataRow("i(1)")]
        public void FunctionNameCanBeASingleLetter(string input)
            => FunctionParsesCorrectly(input);

        [DataTestMethod]
        [DataRow("i_(1)")]
        [DataRow("i_abc(1)")]
        public void FunctionNameCanStartWithALetterAndUnderscore(string input)
            => FunctionParsesCorrectly(input);

        [DataTestMethod]
        [DataRow("ii(1)")]
        [DataRow("iabc(1)")]
        public void FunctionNameCanStartWithTwoLetters(string input)
            => FunctionParsesCorrectly(input);

        [DataTestMethod]
        [DataRow("ii1(1)")]
        [DataRow("i_1(1)")]
        public void FunctionNameCanContainNumbers(string input)
            => FunctionParsesCorrectly(input);

        [DataTestMethod]
        [DataRow("1(1)")]
        [DataRow("i1(1)")]
        [DataRow("1iii(1)")]
        [DataRow("1_(1)")]
        public void InvalidFunctionNamesDontParse(string input)
            => Parser.ParseExpression(input).Should().BeNull($"parsing '{input}' should fail.");

        [DataTestMethod]
        [DataRow("func()", "0")]
        [DataRow("func(1)", "1")]
        [DataRow("func(1,2)", "2")]
        [DataRow("func(1,2,3)", "3")]
        public void FunctionsCanHaveAnArbitraryNumberOfParameters(string input, string paramCount)
            => FunctionParsesCorrectly(input, int.Parse(paramCount));

        [DataTestMethod]
        [DataRow("func(1,2,3)", "1 2 3 func[3]")]
        [DataRow("func(1,1+2-3, 1day 1sec)", "1 1 2 + 3 - '1 day 1 second' func[3]")]
        public void FunctionsParametersCanBeExpressions(string input, string expectedParseString)
            => TestCommon.CompareParseTree(input, expectedParseString);


        public static void FunctionParsesCorrectly(string input, int parameterCount)
            => FunctionParsesCorrectly(input, null, parameterCount);

        public static void FunctionParsesCorrectly(string input, string expectedName = null, int parameterCount = -1)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().NotBeNull($"parsing '{input}' should succeed.");

            var token = expr as IFunctionToken;
            token.Should().NotBeNull($"parsing {input} should result in a valid IFunctionToken.");

            var funcToken = token as IFunctionToken;
            if (!string.IsNullOrWhiteSpace(expectedName))
                funcToken.Name.Should().BeEquivalentTo(expectedName);

            if (parameterCount >= 0)
                funcToken.Parameters.Count.Should().Be(parameterCount);
        }

        public static void FunctionDoesntParse(string input)
        {
            var expr = Parser.ParseExpression(input);
            expr.Should().BeNull($"parsing '{input}' should fail.");
        }
    }
}
