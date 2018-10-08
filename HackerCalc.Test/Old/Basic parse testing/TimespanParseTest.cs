//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Linq;
//using System.Numerics;

//namespace HisRoyalRedness.com
//{
//    [TestCategory(nameof(TimespanToken))]
//    [TestCategory(TestCommon.BASIC_PARSE)]
//    [TestCategory(TestCommon.LITERAL_TOKEN_PARSE)]
//    [TestClass]
//    public class TimespanParseTest
//    {
//        [DataTestMethod]
//        [DataRow("100ts")]
//        [DataRow("100s")]
//        [DataRow("100se")]
//        [DataRow("100sec")]
//        [DataRow("100secs")]
//        [DataRow("100seco")]
//        [DataRow("100secon")]
//        [DataRow("100second")]
//        [DataRow("100seconds")]
//        [DataRow("100 s")]
//        [DataRow("100 se")]
//        [DataRow("100 sec")]
//        [DataRow("100 secs")]
//        [DataRow("100 seco")]
//        [DataRow("100 secon")]
//        [DataRow("100 second")]
//        [DataRow("100 seconds")]
//        public void DecimalSecondsAreParsedCorrectly(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken, TimeSpan>(stringToParse, TimeSpan.FromSeconds(100));

//        [DataTestMethod]
//        [DataRow("100.1ts")]
//        [DataRow("100.1s")]
//        [DataRow("100.1se")]
//        [DataRow("100.1sec")]
//        [DataRow("100.1secs")]
//        [DataRow("100.1seco")]
//        [DataRow("100.1secon")]
//        [DataRow("100.1second")]
//        [DataRow("100.1seconds")]
//        [DataRow("100.1 s")]
//        [DataRow("100.1 se")]
//        [DataRow("100.1 sec")]
//        [DataRow("100.1 secs")]
//        [DataRow("100.1 seco")]
//        [DataRow("100.1 secon")]
//        [DataRow("100.1 second")]
//        [DataRow("100.1 seconds")]
//        public void FloatSecondsAreParsedCorrectly(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken, TimeSpan>(stringToParse, TimeSpan.FromSeconds(100.1));

//        [DataTestMethod]
//        [DataRow("100 ts")]
//        [DataRow("100.1 ts")]
//        public void SeperatedTimeStampFlagDoesntParse(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken>(stringToParse, null);

//        [DataTestMethod]
//        [DataRow("100m")]
//        [DataRow("100mi")]
//        [DataRow("100min")]
//        [DataRow("100mins")]
//        [DataRow("100minu")]
//        [DataRow("100minut")]
//        [DataRow("100minute")]
//        [DataRow("100minutes")]
//        [DataRow("100 m")]
//        [DataRow("100 mi")]
//        [DataRow("100 min")]
//        [DataRow("100 mins")]
//        [DataRow("100 minu")]
//        [DataRow("100 minut")]
//        [DataRow("100 minute")]
//        [DataRow("100 minutes")]
//        public void DecimalMinutesAreParsedCorrectly(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken, TimeSpan>(stringToParse, TimeSpan.FromMinutes(100));

//        [DataTestMethod]
//        [DataRow("100.1m")]
//        [DataRow("100.1mi")]
//        [DataRow("100.1min")]
//        [DataRow("100.1mins")]
//        [DataRow("100.1minu")]
//        [DataRow("100.1minut")]
//        [DataRow("100.1minute")]
//        [DataRow("100.1minutes")]
//        [DataRow("100.1 m")]
//        [DataRow("100.1 mi")]
//        [DataRow("100.1 min")]
//        [DataRow("100.1 mins")]
//        [DataRow("100.1 minu")]
//        [DataRow("100.1 minut")]
//        [DataRow("100.1 minute")]
//        [DataRow("100.1 minutes")]
//        public void FloatMinutesAreParsedCorrectly(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken, TimeSpan>(stringToParse, TimeSpan.FromMinutes(100.1));

//        [DataTestMethod]
//        [DataRow("100h")]
//        [DataRow("100ho")]
//        [DataRow("100hou")]
//        [DataRow("100hour")]
//        [DataRow("100hours")]
//        [DataRow("100hr")]
//        [DataRow("100hrs")]
//        [DataRow("100 h")]
//        [DataRow("100 ho")]
//        [DataRow("100 hou")]
//        [DataRow("100 hour")]
//        [DataRow("100 hours")]
//        [DataRow("100 hr")]
//        [DataRow("100 hrs")]
//        public void DecimalHoursAreParsedCorrectly(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken, TimeSpan>(stringToParse, TimeSpan.FromHours(100));

//        [DataTestMethod]
//        [DataRow("100.1h")]
//        [DataRow("100.1ho")]
//        [DataRow("100.1hou")]
//        [DataRow("100.1hour")]
//        [DataRow("100.1hours")]
//        [DataRow("100.1hr")]
//        [DataRow("100.1hrs")]
//        [DataRow("100.1 h")]
//        [DataRow("100.1 ho")]
//        [DataRow("100.1 hou")]
//        [DataRow("100.1 hour")]
//        [DataRow("100.1 hours")]
//        [DataRow("100.1 hr")]
//        [DataRow("100.1 hrs")]
//        public void FloatHoursAreParsedCorrectly(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken, TimeSpan>(stringToParse, TimeSpan.FromHours(100.1));

//        [DataTestMethod]
//        [DataRow("100d")]
//        [DataRow("100da")]
//        [DataRow("100day")]
//        [DataRow("100days")]
//        [DataRow("100 d")]
//        [DataRow("100 da")]
//        [DataRow("100 day")]
//        [DataRow("100 days")]
//        public void DecimalDaysAreParsedCorrectly(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken, TimeSpan>(stringToParse, TimeSpan.FromDays(100));

//        [DataTestMethod]
//        [DataRow("100.1d")]
//        [DataRow("100.1da")]
//        [DataRow("100.1day")]
//        [DataRow("100.1days")]
//        [DataRow("100.1 d")]
//        [DataRow("100.1 da")]
//        [DataRow("100.1 day")]
//        [DataRow("100.1 days")]
//        public void FloatDaysAreParsedCorrectly(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken, TimeSpan>(stringToParse, TimeSpan.FromDays(100.1));

//        [DataTestMethod]
//        [DataRow("10 sec",                      "00:00:10" )]
//        [DataRow("10 min",                      "00:10:00" )]
//        [DataRow("10 min 10 sec",               "00:10:10" )]
//        [DataRow("10 hrs",                      "10:00:00" )]
//        [DataRow("10 hrs 10 sec",               "10:00:10" )]
//        [DataRow("10 hrs 10 min",               "10:10:00" )]
//        [DataRow("10 hrs 10 min 10 sec",        "10:10:10" )]
//        [DataRow("10 day",                      "10" )]
//        [DataRow("10 day 10 sec",               "10.00:00:10" )]
//        [DataRow("10 day 10 min",               "10.00:10:00" )]
//        [DataRow("10 day 10 min 10 sec",        "10.00:10:10" )]
//        [DataRow("10 day 10 hrs",               "10.10:00:00" )]
//        [DataRow("10 day 10 hrs 10 sec",        "10.10:00:10" )]
//        [DataRow("10 day 10 hrs 10 min",        "10.10:10:00" )]
//        [DataRow("10 day 10 hrs 10 min 10 sec", "10.10:10:10" )]
//        public void CompoundTimePartsCombineCorrectly(string stringToParse, string expectedValue)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken>(stringToParse, expectedValue);

//        [DataTestMethod]
//        [DataRow("10 sec 10 sec")]
//        [DataRow("10 min 10 min")]
//        [DataRow("10 hour 10 hour")]
//        [DataRow("10 day 10 day")]
//        [DataRow("10 sec 10 day")]
//        [DataRow("10 min 10 hour")]
//        public void InvalidCompoundCombinationsDontParse(string stringToParse)
//            => TestCommon.LiteralTokensAreParsedCorrectly<TimespanToken>(stringToParse, null);
//    }
//}
