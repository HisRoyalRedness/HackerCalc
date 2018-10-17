using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class LimitedIntegerType : DataTypeBase<BigInteger, LimitedIntegerType>
    {
        public LimitedIntegerType(BigInteger value)
            : base(value, DataType.LimitedInteger)
        { }
    }
}
