//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using FluentAssertions;

//namespace HisRoyalRedness.com
//{
//    [TestCategory(nameof(DateToken))]
//    [TestCategory(TestCommon.BASIC_PARSE)]
//    [TestCategory(TestCommon.LITERAL_TOKEN_PARSE)]
//    [TestClass]
//    public class DateParseTest
//    {
//        const double PRECISION = 0.000000000001;

//        [DataTestMethod]
//        [DataRow("18-02-01 01:01",      "2018-02-01 01:01:00" )]
//        [DataRow("2018-02-01 01:01:01", "2018-02-01 01:01:01")]
//        [DataRow("01-02-2018 01:01:01", "2018-02-01 01:01:01")]
//        public void DatesTimesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
//            => TestCommon.LiteralTokensAreParsedCorrectly<DateToken>(stringToParse, expectedTokenStr);

//        [DataTestMethod]
//        [DataRow("18-02-01",    "2018-02-01" )]
//        [DataRow("2018-02-01",  "2018-02-01" )]
//        [DataRow("01-02-2018",  "2018-02-01" )]
//        [DataRow("2018-12-01",  "2018-12-01" )]
//        [DataRow("2018-09-01",  "2018-09-01" )]
//        [DataRow("2018-12-29",  "2018-12-29" )]
//        [DataRow("2018-12-31",  "2018-12-31")]
//        public void DatesAreParsedCorrectly(string stringToParse, string expectedTokenStr)
//            => TestCommon.LiteralTokensAreParsedCorrectly<DateToken>(stringToParse, expectedTokenStr);

//        // These need to be evaluated.
//        // They're not parsed as dates, so they get interpreted as subtractions
//        [DataTestMethod]
//        [DataRow("8-2-1")]
//        [DataRow("1-2-2018")]
//        [DataRow("2018-13-01")]
//        [DataRow("2018-12-32")]
//        [DataRow("2018-12-41")]
//        [DataRow("2018-13-01")]
//        [DataRow("2018-21-01")]
//        public void InvalidDatesAreNotParsed(string stringToParse)
//        {
//            var rawToken = Parser.ParseExpression(stringToParse)?.Evaluate();
//            if (rawToken == null)
//                return;
//            else
//                rawToken.Should().NotBeOfType<DateToken>();
//        }
//    }
//}

