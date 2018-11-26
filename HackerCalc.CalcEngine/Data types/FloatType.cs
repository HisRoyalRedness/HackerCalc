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
