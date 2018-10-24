using System;
using System.Collections.Generic;
using System.Linq;
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
        #endregion Operator overloads
    }
}
