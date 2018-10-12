using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class TimeType : DataTypeBase<TimeSpan, TimeType>
    {
        public TimeType(TimeSpan value)
            : base(value, DataType.Timespan)
        { }
    }
}
