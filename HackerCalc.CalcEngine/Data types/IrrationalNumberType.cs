using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class IrrationalNumberType : DataTypeBase<double, IrrationalNumberType>
    {
        public IrrationalNumberType(double value)
            : base(value, DataType.IrrationalNumber)
        { }

        protected override int InternalGetHashCode() => Value.GetHashCode();
        protected override string InternalTypeName => nameof(IrrationalNumberType);

        #region Equality
        protected override bool InternalEquals(IDataType other)
        {
            if (other is IrrationalNumberType dt)
                return dt.Value == Value;
            return false;
        }
        #endregion Equality

        #region Comparison
        protected override int InternalCompareTo(IDataType other)
        {
            if (other is null)
                return 1;
            else if (other is IrrationalNumberType dt1)
                return Value.CompareTo(dt1.Value);
            else if (other is LimitedIntegerType dt2)
                return Value.CompareTo((float)dt2.Value);
            else if (other is TimespanType dt3)
                return Value.CompareTo(dt3.Value.TotalSeconds);
            else if (other is TimeType dt4)
                return Value.CompareTo(dt4.Value.TotalSeconds);
            //else if (other is UnlimitedIntegerType dt5)
            //    return Value.CompareTo((float)dt5.Value);
            throw new InvalidCalcOperationException($"Can't compare a {GetType().Name} to a {other.GetType().Name}.");
        }
        #endregion Comparison

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>(IConfiguration configuration)
        {
            switch (typeof(TNewType).Name)
            {
                case nameof(LimitedIntegerType):
                    return LimitedIntegerType.CreateLimitedIntegerType((BigInteger)Value, true, configuration) as TNewType;
                case nameof(RationalNumberType):
                    return new RationalNumberType(Value) as TNewType;
            }
            return null;
        }
        #endregion Type casting

        #region Operate
        protected override IDataType<DataType> OperateInternal(IConfiguration configuration, OperatorType opType, IDataType<DataType>[] operands) => OperateStatic(configuration, opType, operands);

        static IDataType<DataType> OperateStatic(IConfiguration configuration, OperatorType opType, params IDataType<DataType>[] operands)
        {
            OperateValidate(opType, DataType.IrrationalNumber, operands);
            switch (opType)
            {
                case OperatorType.Add:
                    switch (operands[1].DataType)
                    {
                        case DataType.IrrationalNumber:
                            return new IrrationalNumberType(((IrrationalNumberType)operands[0]).Value + ((IrrationalNumberType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Subtract:
                    switch (operands[1].DataType)
                    {
                        case DataType.IrrationalNumber:
                            return new IrrationalNumberType(((IrrationalNumberType)operands[0]).Value - ((IrrationalNumberType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Multiply:
                    switch (operands[1].DataType)
                    {
                        case DataType.IrrationalNumber:
                            return new IrrationalNumberType(((IrrationalNumberType)operands[0]).Value * ((IrrationalNumberType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Divide:
                    switch (operands[1].DataType)
                    {
                        case DataType.IrrationalNumber:
                            return new IrrationalNumberType(((IrrationalNumberType)operands[0]).Value / ((IrrationalNumberType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Power:
                    switch (operands[1].DataType)
                    {
                        case DataType.IrrationalNumber:
                            return new IrrationalNumberType(Math.Pow(((IrrationalNumberType)operands[0]).Value, ((IrrationalNumberType)operands[1]).Value));
                    }
                    break;
                case OperatorType.Root:
                    switch (operands[1].DataType)
                    {
                        case DataType.IrrationalNumber:
                            return new IrrationalNumberType(Math.Pow(((IrrationalNumberType)operands[0]).Value, 1.0 / ((IrrationalNumberType)operands[1]).Value));
                    }
                    break;
                case OperatorType.Modulo:
                    switch (operands[1].DataType)
                    {
                        case DataType.IrrationalNumber:
                            return new IrrationalNumberType(((IrrationalNumberType)operands[0]).Value % ((IrrationalNumberType)operands[1]).Value);
                    }
                    break;
            }
            return null;
        }
        #endregion Operate
    }
}
