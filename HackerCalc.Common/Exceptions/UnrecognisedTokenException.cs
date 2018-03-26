using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class UnrecognisedTokenException : ApplicationException
    {
        public UnrecognisedTokenException(string message)
            : base(message)
        { }

        public UnrecognisedTokenException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
