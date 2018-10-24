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
                case nameof(FloatToken):
                    return new FloatToken((double)Value) as TNewType;
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
        #endregion Operator overloads
    }
}
