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

        protected override int InternalGetHashCode() => Value.GetHashCode();
        protected override string InternalTypeName => nameof(UnlimitedIntegerType);

        #region Equality
        protected override bool InternalEquals(IDataType other)
        {
            if (other is UnlimitedIntegerType dt)
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
                return Value.CompareTo(new BigInteger(dt1.Value));
            else if (other is LimitedIntegerType dt2)
                return Value.CompareTo(dt2.Value);
            else if (other is TimespanType dt3)
                return Value.CompareTo(new BigInteger(dt3.Value.TotalSeconds));
            else if (other is TimeType dt4)
                return Value.CompareTo(new BigInteger(dt4.Value.TotalSeconds));
            else if (other is UnlimitedIntegerType dt5)
                return Value.CompareTo(dt5.Value);
            throw new InvalidCalcOperationException($"Can't compare a {GetType().Name} to a {other.GetType().Name}.");
        }
        #endregion Comparison

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>(IConfiguration configuration)
        {
            switch (typeof(TNewType).Name)
            {
                case nameof(TimespanType):
                    return new TimespanType(TimeSpan.FromSeconds((double)Value)) as TNewType;
                case nameof(FloatType):
                    return new FloatType((double)Value) as TNewType;
                case nameof(LimitedIntegerType):
                    return LimitedIntegerType.CreateLimitedIntegerType(Value, configuration) as TNewType;
            }
            return null;
        }
        #endregion Type casting

        #region Operate
        protected override IDataType<DataType> OperateInternal(IConfiguration configuration, OperatorType opType, IDataType<DataType>[] operands) => OperateStatic(configuration, opType, operands);

        static IDataType<DataType> OperateStatic(IConfiguration configuration, OperatorType opType, params IDataType<DataType>[] operands)
        {
            OperateValidate(opType, DataType.UnlimitedInteger, operands);
            switch (opType)
            {
                case OperatorType.Add:
                    switch (operands[1].DataType)
                    {
                        case DataType.UnlimitedInteger:
                            return new UnlimitedIntegerType(((UnlimitedIntegerType)operands[0]).Value + ((UnlimitedIntegerType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Subtract:
                    switch (operands[1].DataType)
                    {
                        case DataType.UnlimitedInteger:
                            return new UnlimitedIntegerType(((UnlimitedIntegerType)operands[0]).Value - ((UnlimitedIntegerType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Multiply:
                    switch (operands[1].DataType)
                    {
                        case DataType.UnlimitedInteger:
                            return new UnlimitedIntegerType(((UnlimitedIntegerType)operands[0]).Value * ((UnlimitedIntegerType)operands[1]).Value);
                    }
                    break;
            }
            return null;
        }
        #endregion Operate

        #region Operator implementations
        public static UnlimitedIntegerType pow(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => new UnlimitedIntegerType(BigInteger.Pow(a.Value, (int)b.Value));
        public static FloatType root(UnlimitedIntegerType a, UnlimitedIntegerType b)
            => root(a, b.CastTo<FloatType>(null));

        public static FloatType pow(UnlimitedIntegerType a, FloatType b)
            => new FloatType(Math.Pow((double)a.Value, b.Value));
        public static FloatType root(UnlimitedIntegerType a, FloatType b)
            => new FloatType(Math.Pow((double)a.Value, 1.0 / b.Value));
        #endregion Operator implementations
    }
}
