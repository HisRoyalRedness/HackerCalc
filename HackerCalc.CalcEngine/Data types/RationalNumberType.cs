using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class RationalNumberType : DataTypeBase<RationalNumber, RationalNumberType>
    {
        public RationalNumberType(RationalNumber value)
            : base(value, DataType.RationalNumber)
        { }

        protected override int InternalGetHashCode() => Value.GetHashCode();
        protected override string InternalTypeName => nameof(RationalNumberType);

        #region Equality
        protected override bool InternalEquals(IDataType other)
            => other is RationalNumberType dt
                ? dt.Value == Value
                : false;
        #endregion Equality

        #region Comparison
        protected override int InternalCompareTo(RationalNumberType other)
            => other is null
                ? 1
                : Value.CompareTo(other.Value);
        #endregion Comparison

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>(IConfiguration configuration)
        {
            switch (typeof(TNewType).Name)
            {
                case nameof(LimitedIntegerType):
                    return LimitedIntegerType.CreateLimitedIntegerType((BigInteger)Value, true, configuration) as TNewType;
                case nameof(IrrationalNumberType):
                    return new IrrationalNumberType((double)Value) as TNewType;
            }
            return null;
        }
        #endregion Type casting

        #region Operate
        protected override IDataType<DataType> OperateInternal(IConfiguration configuration, OperatorType opType, IDataType<DataType>[] operands) => OperateStatic(configuration, opType, operands);

        static IDataType<DataType> OperateStatic(IConfiguration configuration, OperatorType opType, params IDataType<DataType>[] operands)
        {
            OperateValidate(opType, DataType.RationalNumber, operands);
            switch (opType)
            {
                case OperatorType.Add:
                    switch (operands[1].DataType)
                    {
                        case DataType.RationalNumber:
                            return new RationalNumberType(((RationalNumberType)operands[0]).Value + ((RationalNumberType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Subtract:
                    switch (operands[1].DataType)
                    {
                        case DataType.RationalNumber:
                            return new RationalNumberType(((RationalNumberType)operands[0]).Value - ((RationalNumberType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Multiply:
                    switch (operands[1].DataType)
                    {
                        case DataType.RationalNumber:
                            return new RationalNumberType(((RationalNumberType)operands[0]).Value * ((RationalNumberType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Divide:
                    switch (operands[1].DataType)
                    {
                        case DataType.RationalNumber:
                            return new RationalNumberType(((RationalNumberType)operands[0]).Value / ((RationalNumberType)operands[1]).Value);
                    }
                    break;
                case OperatorType.Power:
                    switch (operands[1].DataType)
                    {
                        case DataType.RationalNumber:
                            return new RationalNumberType(RationalNumber.Pow(((RationalNumberType)operands[0]).Value, ((RationalNumberType)operands[1]).Value));
                    }
                    break;
                case OperatorType.Root:
                    switch (operands[1].DataType)
                    {
                        case DataType.RationalNumber:
                            return new RationalNumberType(RationalNumber.Root(((RationalNumberType)operands[0]).Value, ((RationalNumberType)operands[1]).Value));
                    }
                    break;
                case OperatorType.Modulo:
                    switch (operands[1].DataType)
                    {
                        case DataType.RationalNumber:
                            return new RationalNumberType(((RationalNumberType)operands[0]).Value % ((RationalNumberType)operands[1]).Value);
                    }
                    break;
            }
            return null;
        }
        #endregion Operate
    }
}
