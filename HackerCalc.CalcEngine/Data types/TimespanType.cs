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
            //else if (other is UnlimitedIntegerType dt5)
            //    return new BigInteger(Value.TotalSeconds).CompareTo(dt5.Value);
            throw new InvalidCalcOperationException($"Can't compare a {GetType().Name} to a {other.GetType().Name}.");
        }
        #endregion Comparison

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>(IConfiguration configuration) => null;
        #endregion Type casting

        #region Operate
        protected override IDataType<DataType> OperateInternal(IConfiguration configuration, OperatorType opType, IDataType<DataType>[] operands) => OperateStatic(configuration, opType, operands);

        static IDataType<DataType> OperateStatic(IConfiguration configuration, OperatorType opType, params IDataType<DataType>[] operands)
        {
            OperateValidate(opType, DataType.Timespan, operands);
            switch (opType)
            {
                case OperatorType.Add:
                    switch (operands[1].DataType)
                    {
                        case DataType.Date:
                            // Note op1 and op2 are reversed, coz C# doesn't like Timespan + Date
                            return new DateType(((DateType)operands[1]).Value + ((TimespanType)operands[0]).Value); 
                        case DataType.Time:
                            return new TimeType(((TimespanType)operands[0]).Value + ((TimeType)operands[1]).Value);
                        case DataType.Timespan:
                            return new TimespanType(((TimespanType)operands[0]).Value + ((TimespanType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Subtract:
                    switch (operands[1].DataType)
                    {
                        case DataType.Timespan:
                            return new TimespanType(((TimespanType)operands[0]).Value - ((TimespanType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Multiply:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new TimespanType(TimeSpan.FromTicks((long)(((TimespanType)operands[0]).Value.Ticks * ((LimitedIntegerType)operands[1]).Value)));
                        case DataType.RationalNumber:
                            return new TimespanType(TimeSpan.FromTicks((long)(BigInteger)(((TimespanType)operands[0]).Value.Ticks * ((RationalNumberType)operands[1]).Value)));
                        case DataType.Float:
                            return new TimespanType(TimeSpan.FromMilliseconds(((TimespanType)operands[0]).Value.TotalMilliseconds * ((FloatType)operands[1]).Value));
                    }
                    break;
                case OperatorType.Divide:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new TimespanType(TimeSpan.FromTicks((long)(((TimespanType)operands[0]).Value.Ticks / ((LimitedIntegerType)operands[1]).Value)));
                        case DataType.RationalNumber:
                            return new TimespanType(TimeSpan.FromTicks((long)(BigInteger)(((TimespanType)operands[0]).Value.Ticks / ((RationalNumberType)operands[1]).Value)));
                        case DataType.Float:
                            return new TimespanType(TimeSpan.FromMilliseconds(((TimespanType)operands[0]).Value.TotalMilliseconds / ((FloatType)operands[1]).Value));
                        case DataType.Timespan:
                            return new FloatType(((TimespanType)operands[0]).Value.TotalMilliseconds / ((TimespanType)operands[1]).Value.TotalMilliseconds);
                    }
                    break;
            }
            return null;
        }
        #endregion Operate
    }
}
