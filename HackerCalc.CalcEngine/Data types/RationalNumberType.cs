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
        {
            if (other is RationalNumberType dt)
                return dt.Value == Value;
            return false;
        }
        #endregion Equality

        #region Comparison
        protected override int InternalCompareTo(IDataType other)
        {
            throw new NotImplementedException();
            //if (other is null)
            //    return 1;
            //else if (other is FloatType dt1)
            //    return Value.CompareTo(dt1.Value);
            //else if (other is LimitedIntegerType dt2)
            //    return Value.CompareTo((float)dt2.Value);
            //else if (other is TimespanType dt3)
            //    return Value.CompareTo(dt3.Value.TotalSeconds);
            //else if (other is TimeType dt4)
            //    return Value.CompareTo(dt4.Value.TotalSeconds);
            //else if (other is UnlimitedIntegerType dt5)
            //    return Value.CompareTo((float)dt5.Value);
            //throw new InvalidCalcOperationException($"Can't compare a {GetType().Name} to a {other.GetType().Name}.");
        }
        #endregion Comparison

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>(IConfiguration configuration)
        {
            throw new NotImplementedException();
            //switch (typeof(TNewType).Name)
            //{
            //    case nameof(TimespanType):
            //        return new TimespanType(TimeSpan.FromSeconds(Value)) as TNewType;
            //    case nameof(UnlimitedIntegerType):
            //        return new UnlimitedIntegerType(new BigInteger(Value)) as TNewType;
            //}
            //return null;
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
                    throw new NotImplementedException();
                    //switch (operands[1].DataType)
                    //{
                    //    case DataType.RationalNumber:
                    //        return new RationalNumberType(BigInteger.Pow(((RationalNumberType)operands[0]).Value, (int)((RationalNumberType)operands[1]).Value));
                    //}
                    //break;
                case OperatorType.Root:
                    throw new NotImplementedException();
                    //switch (operands[1].DataType)
                    //{
                    //    case DataType.RationalNumber:
                    //        return new RationalNumberType(Algoritms.IntegerNthRoot(((RationalNumberType)operands[0]).Value, (int)((RationalNumberType)operands[1]).Value));
                    //}
                    //break;
                case OperatorType.Modulo:
                    throw new NotImplementedException();
                    //switch (operands[1].DataType)
                    //{
                    //    case DataType.RationalNumber:
                    //        return new RationalNumberType(((RationalNumberType)operands[0]).Value % ((RationalNumberType)operands[1]).Value);
                    //}
                    //break;
            }
            return null;
        }
        #endregion Operate
    }
}
