using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class FloatType : DataTypeBase<double, FloatType>
    {
        public FloatType(double value)
            : base(value, DataType.Float)
        { }
    }
}
