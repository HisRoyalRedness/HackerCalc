using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class TimeType : DataTypeBase<TimeSpan, TimeType>
    {
        public TimeType(TimeSpan value)
            : base(value, DataType.Time)
        { }

        protected override int InternalGetHashCode() => Value.GetHashCode();
        protected override string InternalTypeName => nameof(TimeType);

        #region Equality
        protected override bool InternalEquals(IDataType other)
        {
            if (other is TimeType dt)
                return dt.Value == Value;
            return false;
        }
        #endregion Equality

        #region Comparison
        protected override int InternalCompareTo(IDataType other)
        {
            if (other is null)
                return 1;
            else if (other is FloatType dt1)
                return Value.TotalSeconds.CompareTo(dt1.Value);
            else if (other is LimitedIntegerType dt2)
                return new BigInteger(Value.TotalSeconds).CompareTo(dt2.Value);
            else if (other is TimespanType dt3)
                return Value.CompareTo(dt3.Value);
            else if (other is TimeType dt4)
                return Value.CompareTo(dt4.Value);
            else if (other is UnlimitedIntegerType dt5)
                return new BigInteger(Value.TotalSeconds).CompareTo(dt5.Value);
            throw new InvalidCalcOperationException($"Can't compare a {GetType().Name} to a {other.GetType().Name}.");
        }
        #endregion Comparison

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
