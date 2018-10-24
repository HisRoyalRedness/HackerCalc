using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class TimespanType : DataTypeBase<TimeSpan, TimespanType>
    {
        public TimespanType(TimeSpan value)
            : base(value, DataType.Timespan)
        { }

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>() => null;
        #endregion Type casting
    }
}
