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

        protected override TNewType InternalCastTo<TNewType>() => null;

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
