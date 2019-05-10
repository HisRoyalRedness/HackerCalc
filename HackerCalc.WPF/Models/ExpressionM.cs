using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class ExpressionM
    {
        public ExpressionM()
        {
            Input = string.Empty;
            IsValid = false;
            ParseError = string.Empty;
            ParsedExpression = RationalNumberToken.Default;
            Evaluation = new RationalNumberType((int)0);
        }    

        public string Input { get; set; }
        public bool IsValid { get; set; }
        public string ParseError { get; set; }
        public IToken ParsedExpression { get; set; }
        public IDataType<DataType> Evaluation { get; set; }

        public bool IsEmpty => string.IsNullOrWhiteSpace(Input);
    }
}
