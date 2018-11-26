using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class DateType : DataTypeBase<DateTime, DateType>
    {
        public DateType(DateTime value)
            : base(value, DataType.Date)
        { }

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>() => null;
        #endregion Type casting

        #region Operator overloads
        public static DateType operator +(DateType a, TimespanType b)
            => new DateType(a.Value + b.Value);
        public static DateType operator +(TimespanType a, DateType b) => b + a;
        public static DateType operator -(DateType a, TimespanType b)
            => new DateType(a.Value - b.Value);

        public static TimespanType operator -(DateType a, DateType b)
            => new TimespanType(a.Value - b.Value);
        #endregion Operator overloads
    }
}
