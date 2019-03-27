using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class FloatType : DataTypeBase<double, FloatType>
    {
        public FloatType(double value)
            : base(value, DataType.Float)
        { }

        protected override int InternalGetHashCode() => Value.GetHashCode();
        protected override string InternalTypeName => nameof(FloatType);

        #region Equality
        protected override bool InternalEquals(IDataType other)
        {
            if (other is FloatType dt)
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
                return Value.CompareTo(dt1.Value);
            else if (other is LimitedIntegerType dt2)
                return Value.CompareTo((float)dt2.Value);
            else if (other is TimespanType dt3)
                return Value.CompareTo(dt3.Value.TotalSeconds);
            else if (other is TimeType dt4)
                return Value.CompareTo(dt4.Value.TotalSeconds);
            else if (other is UnlimitedIntegerType dt5)
                return Value.CompareTo((float)dt5.Value);
            throw new InvalidCalcOperationException($"Can't compare a {GetType().Name} to a {other.GetType().Name}.");
        }
        #endregion Comparison

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>()
        {
            switch(typeof(TNewType).Name)
            {
                case nameof(TimespanType):
                    return new TimespanType(TimeSpan.FromSeconds(Value)) as TNewType;
                case nameof(UnlimitedIntegerType):
                    return new UnlimitedIntegerType(new BigInteger(Value)) as TNewType;
            }
            return null;
        }
        #endregion Type casting

        #region Operator overloads
        public static FloatType operator +(FloatType a, FloatType b)
            => new FloatType(a.Value + b.Value);
        public static FloatType operator -(FloatType a, FloatType b)
            => new FloatType(a.Value - b.Value);
        public static FloatType operator *(FloatType a, FloatType b)
            => new FloatType(a.Value * b.Value);
        public static FloatType operator /(FloatType a, FloatType b)
            => new FloatType(a.Value / b.Value);
        public static FloatType operator %(FloatType a, FloatType b)
            => new FloatType(a.Value % b.Value);

        public static FloatType operator %(TimespanType a, FloatType b)
            => new FloatType(a.Value.TotalSeconds % b.Value);

        // Unary
        public static FloatType operator -(FloatType a)
            => new FloatType(-a.Value);
        #endregion Operator overloads

        #region Operator implementations
        public static FloatType pow(FloatType a, FloatType b)
            => new FloatType(Math.Pow(a.Value, b.Value));
        public static FloatType root(FloatType a, FloatType b)
            => new FloatType(Math.Pow(a.Value, 1.0 / b.Value));
        #endregion Operator implementations
    }
}
