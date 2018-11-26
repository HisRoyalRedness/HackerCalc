using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class UnlimitedIntegerType : DataTypeBase<BigInteger, UnlimitedIntegerType>
    {
        public UnlimitedIntegerType(BigInteger value)
            : base(value, DataType.UnlimitedInteger)
        { }

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>()
        {
            switch (typeof(TNewType).Name)
            {
                case nameof(TimespanType):
                    return new TimespanType(TimeSpan.FromSeconds((double)Value)) as TNewType;
                case nameof(FloatType):
                    return new FloatType((double)Value) as TNewType;
                case nameof(LimitedIntegerType):
                    return LimitedIntegerType.CreateLimitedIntegerType(Value) as TNewType;
            }
            return null;
        }
        #endregion Type casting

        #region Operator overloads
        public static UnlimitedIntegerType operator +(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(a.Value + b.Value);
        public static UnlimitedIntegerType operator -(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(a.Value - b.Value);
        public static UnlimitedIntegerType operator *(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(a.Value * b.Value);
        public static UnlimitedIntegerType operator /(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(a.Value / b.Value);
        public static UnlimitedIntegerType operator %(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(a.Value % b.Value);
        public static UnlimitedIntegerType operator &(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(a.Value & b.Value);
        public static UnlimitedIntegerType operator |(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(a.Value | b.Value);
        public static UnlimitedIntegerType operator ^(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(a.Value ^ b.Value);
        public static UnlimitedIntegerType operator <<(UnlimitedIntegerType a, int b)
            => new UnlimitedIntegerType(a.Value << b);
        public static UnlimitedIntegerType operator >>(UnlimitedIntegerType a, int b)
            => new UnlimitedIntegerType(a.Value >> b);

        // Unary
        public static UnlimitedIntegerType operator ~(UnlimitedIntegerType a)
            => new UnlimitedIntegerType(~a.Value);
        public static UnlimitedIntegerType operator -(UnlimitedIntegerType a)
            => new UnlimitedIntegerType(-a.Value);
        #endregion Operator overloads

        #region Operator implementations
        public static UnlimitedIntegerType pow(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(BigInteger.Pow(a.Value, (int)b.Value));
        public static FloatType root(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => root(a, b.CastTo<FloatType>());

        public static FloatType pow(UnlimitedIntegerType a, FloatType b)
            => new FloatType(Math.Pow((double)a.Value, b.Value));
        public static FloatType root(UnlimitedIntegerType a, FloatType b)
            => new FloatType(Math.Pow((double)a.Value, 1.0 / b.Value));
        #endregion Operator implementations
    }
}
