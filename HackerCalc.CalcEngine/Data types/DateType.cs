using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class DateType : DataTypeBase<DateTime, DateType>
    {
        public DateType(DateTime value)
            : base(value, DataType.Date)
        { }

        protected override int InternalGetHashCode() => Value.GetHashCode();
        protected override string InternalTypeName => nameof(DateType);

        #region Equality
        protected override bool InternalEquals(IDataType other)
            => other is DateType dt
                ? dt.Value == Value
                : false;
        #endregion Equality

        #region Comparison
        protected override int InternalCompareTo(DateType other)
            => other is null
                ? 1
                : Value.CompareTo(other.Value);
        #endregion Comparison

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>(IConfiguration configuration) => null;
        #endregion Type casting

        #region Operate
        protected override IDataType<DataType> OperateInternal(IConfiguration configuration, OperatorType opType, IDataType<DataType>[] operands) => OperateStatic(configuration, opType, operands);

        static IDataType<DataType> OperateStatic(IConfiguration configuration, OperatorType opType, params IDataType<DataType>[] operands)
        {
            OperateValidate(opType, DataType.Date, operands);
            switch (opType)
            {
                case OperatorType.Add:
                    switch(operands[1].DataType)
                    {
                        case DataType.Timespan:
                            return new DateType(((DateType)operands[0]).Value + ((TimespanType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Subtract:
                    switch (operands[1].DataType)
                    {
                        case DataType.Date:
                            return new TimespanType(((DateType)operands[0]).Value - ((DateType)operands[1]).Value);
                        case DataType.Timespan:
                            return new DateType(((DateType)operands[0]).Value - ((TimespanType)operands[1]).Value);
                    }
                    break;
            }
            return null;
        }
        #endregion Operate
    }
}
