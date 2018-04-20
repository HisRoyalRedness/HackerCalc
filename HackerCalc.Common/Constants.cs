using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public static class Constants
    {
        public const bool IsPartialEquationAllowed
#if INCOMPLETE_EQ
            = true;
#else
            = false;
#endif
    }
}
