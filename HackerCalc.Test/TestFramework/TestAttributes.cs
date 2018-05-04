using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Linq;

namespace HisRoyalRedness.com
{
    [AttributeUsageAttribute(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class TokenParseDataTestAttribute : DataRowAttribute
    {
        public TokenParseDataTestAttribute(string tokenToParse, string expectedToken)
            : base(new object[] { tokenToParse, TestCommon.MakeToken(expectedToken) })
        { }
    }

    public class TokenParseTestMethodAttribute : DataTestMethodAttribute, ITestDataSource
    {
        public TokenParseTestMethodAttribute()
        {
            Console.WriteLine();
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            => new[] { new object[] { "1", "2"} };

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
            => $"{methodInfo.Name} ({string.Join(", ", data.Select(d => $"\"{d}\""))})";

        //readonly List<object[]> _testData;
    }

    //public abstract class TestDataSourceBase : Attribute, ITestDataSource
    //{
    //    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    //        => TestData;
    //    public string GetDisplayName(MethodInfo methodInfo, object[] data)
    //        => $"{methodInfo.Name} ({string.Join(",", data)})";

    //    protected abstract IEnumerable<object[]> TestData { get; }
    //}
}
