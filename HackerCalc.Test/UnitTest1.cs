using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HisRoyalRedness.com
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var token = Parser.ParseExpression("1U").ToList();
            Assert.Fail($"{token.Count}");
        }
    }
}
