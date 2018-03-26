using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class ParseException : ApplicationException
    {
        public ParseException(string message)
            : base(message)
        { }

        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
