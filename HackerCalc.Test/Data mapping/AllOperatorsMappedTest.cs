using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;

namespace HisRoyalRedness.com
{
    [TestCategory(TestCommon.DATA_MAPPING)]
    [TestCategory(TestCommon.INCOMPLETE)]    
    [TestClass]
    public class AllOperatorsMappedTest
    {
        [TestMethod]
        public void AllBinaryOperatorsMapped()
        {
            var dataTypes = EnumExtensions.GetEnumCollection<OperatorType>(e => e.IsBinaryOperator()).ToList();
            Console.WriteLine();
        }

        
    }
}
