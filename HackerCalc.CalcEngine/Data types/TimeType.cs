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
            : base(value, DataType.Time)
        { }

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>()
        {
            switch (typeof(TNewType).Name)
            {
                case nameof(TimespanType):
                    return new TimespanType(Value) as TNewType;
            }
            return null;
        }
        #endregion Type casting

        #region Operator overloads
        public static TimeType operator +(TimeType a, TimespanType b)
            => new TimeType(a.Value + b.Value);
        public static TimeType operator +(TimespanType a, TimeType b) => b + a;
        public static TimeType operator -(TimeType a, TimespanType b)
            => new TimeType(a.Value - b.Value);
        #endregion Operator overloads
    }
}
