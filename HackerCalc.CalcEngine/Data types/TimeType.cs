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
            => other is TimeType dt
                ? dt.Value == Value
                : false;
        #endregion Equality

        #region Comparison
        protected override int InternalCompareTo(TimeType other)
            => other is null
                ? 1
                : Value.CompareTo(other.Value);
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
                case OperatorType.Subtract:
                    switch (operands[1].DataType)
                    {
                        case DataType.Time:
                            return new TimespanType(((TimeType)operands[0]).Value - ((TimeType)operands[1]).Value);
                        case DataType.Timespan:
                            return new TimeType(((TimeType)operands[0]).Value - ((TimespanType)operands[1]).Value);
                    }
                    break;
            }
            return null;
        }
        #endregion Operate
    }
}
