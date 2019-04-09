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
        protected override TNewType InternalCastTo<TNewType>(IConfiguration configuration)
        {
            switch (typeof(TNewType).Name)
            {
                case nameof(TimespanType):
                    return new TimespanType(Value) as TNewType;
            }
            return null;
        }
        #endregion Type casting

        #region Operate
        protected override IDataType<DataType> OperateInternal(IConfiguration configuration, OperatorType opType, IDataType<DataType>[] operands) => OperateStatic(configuration, opType, operands);

        static IDataType<DataType> OperateStatic(IConfiguration configuration, OperatorType opType, params IDataType<DataType>[] operands)
        {
            OperateValidate(opType, DataType.Time, operands);
            switch (opType)
            {
                case OperatorType.Add:
                    switch (operands[1].DataType)
                    {
                        case DataType.Timespan:
                            return new TimeType(((TimeType)operands[0]).Value + ((TimespanType)operands[1]).Value);
                    }
                    break;
            }
            return null;
        }
        #endregion Operate
    }
}
