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

        #region Operator overloads
        public static TimespanType operator +(TimespanType a, TimespanType b)
            => new TimespanType(a.Value + b.Value);
        public static TimespanType operator -(TimespanType a, TimespanType b)
            => new TimespanType(a.Value - b.Value);
        public static TimespanType operator *(TimespanType a, FloatType b) => b * a;
        public static TimespanType operator *(FloatType a, TimespanType b)
            => new TimespanType(TimeSpan.FromSeconds(a.Value * b.Value.TotalSeconds));
        public static TimespanType operator /(TimespanType a, FloatType b)
            => new TimespanType(TimeSpan.FromSeconds(a.Value.TotalSeconds / b.Value));

        public static FloatType operator /(TimespanType a, TimespanType b)
            => new FloatType(a.Value.TotalSeconds / b.Value.TotalSeconds);
        public static FloatType operator %(TimespanType a, TimespanType b)
            => new FloatType(a.Value.TotalSeconds % b.Value.TotalSeconds);

        // Unary
        public static TimespanType operator -(TimespanType a)
            => new TimespanType(-a.Value);
        #endregion Operator overloads
    }
}
