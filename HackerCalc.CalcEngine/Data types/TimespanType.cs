using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class TimespanType : DataTypeBase<TimeSpan, TimespanType>
    {
        public TimespanType(TimeSpan value)
            : base(value, DataType.Timespan)
        { }

        protected override int InternalGetHashCode() => Value.GetHashCode();
        protected override string InternalTypeName => nameof(TimespanType);

        #region Equality
        protected override bool InternalEquals(IDataType other)
        {
            if (other is TimespanType dt)
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
