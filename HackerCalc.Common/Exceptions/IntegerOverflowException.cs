using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class IntegerOverflowException : ApplicationException
    {
        public IntegerOverflowException(string message)
            : base(message)
        { }

        public IntegerOverflowException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
