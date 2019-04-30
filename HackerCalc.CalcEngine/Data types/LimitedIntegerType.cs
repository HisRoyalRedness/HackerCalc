using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class LimitedIntegerType : DataTypeBase<BigInteger, LimitedIntegerType>
    {
        public LimitedIntegerType(BigInteger value, BitWidthAndSignPair signAndBitwidth, IConfiguration configuration)
            : base(Normalise(value, signAndBitwidth, configuration), DataType.LimitedInteger)
        {
            SignAndBitWidth = signAndBitwidth;
            _minAndMax = MinAndMaxMap.Instance[SignAndBitWidth];
        }

        static BigInteger Normalise(BigInteger value, BitWidthAndSignPair signAndBitwidth, IConfiguration configuration)
        {
            var minAndMax = MinAndMaxMap.Instance[signAndBitwidth];
            var oldValue = value;
            value &= minAndMax.Mask;

            var state = (configuration?.State as CalcState) ?? new CalcState();

            if (oldValue != value)
            {
                if (configuration?.AllowOverOrUnderflow ?? true)
                    state.OverOrUnderflowOccurred = true;
                else
                    throw new InvalidCalcOperationException("Overflow or underflow of LimitedIntegerTypes is not permitted.");
            }

            if (signAndBitwidth.IsSigned)
            {
                if (value > minAndMax.Max)
                    value -= (minAndMax.Mask  + 1);
            }

            if ((oldValue >= 0) != (value >= 0))
            {
                if (configuration?.AllowSignChange ?? true)
                    state.SignChangedOccurred = true;
                else
                    throw new InvalidCalcOperationException("Sign change through overflow or underflow of LimitedIntegerTypes is not permitted.");
            }

            return value;
        }

        public static LimitedIntegerType CreateLimitedIntegerType(BigInteger value, IConfiguration configuration)
        {
            var isSigned = value < 0;
            var bitWidth = EnumExtensions
                .GetEnumCollection<IntegerBitWidth>()
                .FirstOrDefault(bw => isSigned 
                    ? MinAndMaxMap.Instance[new BitWidthAndSignPair(bw, isSigned)].Min <= value
                    : MinAndMaxMap.Instance[new BitWidthAndSignPair(bw, isSigned)].Max >= value);
            if (bitWidth == 0)
                throw new TypeConversionException($"{value} is out of range of a {nameof(LimitedIntegerType)}.");
            return new LimitedIntegerType(value, new BitWidthAndSignPair(bitWidth, isSigned), configuration);
        }

        public BitWidthAndSignPair SignAndBitWidth { get; private set; }
        public bool IsSigned => SignAndBitWidth.IsSigned;
        public IntegerBitWidth BitWidth => SignAndBitWidth.BitWidth;

        public BigInteger Min => _minAndMax.Min;
        public BigInteger Max => _minAndMax.Max;
        public BigInteger Mask => _minAndMax.Mask;

        protected override int InternalGetHashCode() => (Value.GetHashCode() << 1) ^ (SignAndBitWidth.GetHashCode());
        protected override string InternalTypeName => nameof(LimitedIntegerType);

        #region Equality
        protected override bool InternalEquals(IDataType other)
        {
            if (other is LimitedIntegerType dt)
                return dt.Value == Value && dt.SignAndBitWidth == SignAndBitWidth;
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
                case nameof(UnlimitedIntegerType):
                    return new UnlimitedIntegerType(Value) as TNewType;
            }
            return null;
        }
        #endregion Type casting

        static BitWidthAndSignPair GetCommonBitWidthAndSign(LimitedIntegerType a, LimitedIntegerType b)
        {
            // It makes it easier if this method can assume that a is always the greater rank
            if (b.BitWidth > a.BitWidth)
                return GetCommonBitWidthAndSign(b, a);

            // http://c0x.coding-guidelines.com/6.3.1.8.html

            //713 Otherwise, if both operands have signed integer types or both have unsigned integer types, 
            //    the operand with the type of lesser integer conversion rank is converted to the type of the 
            //    operand with greater rank.
            if (a.IsSigned == b.IsSigned)
                return a.SignAndBitWidth;

            //714 Otherwise, if the operand that has unsigned integer type has rank greater or equal to the 
            //    rank of the type of the other operand, then the operand with signed integer type is converted 
            //    to the type of the operand with unsigned integer type.
            //if (!a.IsSigned)
            //    return a.SignAndBitWidth;
            // By experimentation, is doesn't appear that the MS C++ compiler does this...

            //715 Otherwise, if the type of the operand with signed integer type can represent all of the 
            //    values of the type of the operand with unsigned integer type, then the operand with unsigned 
            //    integer type is converted to the type of the operand with signed integer type.
            if (a.IsSigned && b.Value <= a.Max)
                return a.SignAndBitWidth;
            if (b.IsSigned && a.Value <= b.Max)
                return b.SignAndBitWidth;

            //716 Otherwise, both operands are converted to the unsigned integer type corresponding to the 
            //    type of the operand with signed integer type.
            return new BitWidthAndSignPair(a.BitWidth, false);
        }

        #region Operate
        protected override IDataType<DataType> OperateInternal(IConfiguration configuration, OperatorType opType, IDataType<DataType>[] operands) => OperateStatic(configuration, opType, operands);

        static IDataType<DataType> OperateStatic(IConfiguration configuration, OperatorType opType, params IDataType<DataType>[] operands)
        {
            OperateValidate(opType, DataType.LimitedInteger, operands);
            switch (opType)
            {
                case OperatorType.Add:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new LimitedIntegerType(
                                ((LimitedIntegerType)operands[0]).Value + ((LimitedIntegerType)operands[1]).Value,
                                GetCommonBitWidthAndSign((LimitedIntegerType)operands[0], (LimitedIntegerType)operands[1]),
                                configuration);
                    }
                    break;
                case OperatorType.Subtract:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new LimitedIntegerType(
                                ((LimitedIntegerType)operands[0]).Value - ((LimitedIntegerType)operands[1]).Value,
                                GetCommonBitWidthAndSign((LimitedIntegerType)operands[0], (LimitedIntegerType)operands[1]),
                                configuration);
                    }
                    break;
                case OperatorType.Multiply:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new LimitedIntegerType(
                                ((LimitedIntegerType)operands[0]).Value * ((LimitedIntegerType)operands[1]).Value,
                                GetCommonBitWidthAndSign((LimitedIntegerType)operands[0], (LimitedIntegerType)operands[1]),
                                configuration);
                    }
                    break;
                case OperatorType.Divide:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new LimitedIntegerType(
                                ((LimitedIntegerType)operands[0]).Value / ((LimitedIntegerType)operands[1]).Value,
                                GetCommonBitWidthAndSign((LimitedIntegerType)operands[0], (LimitedIntegerType)operands[1]),
                                configuration);
                    }
                    break;
                case OperatorType.Power:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new LimitedIntegerType(
                                BigInteger.Pow(((LimitedIntegerType)operands[0]).Value, (int)((LimitedIntegerType)operands[1]).Value),
                                ((LimitedIntegerType)operands[0]).SignAndBitWidth,
                                configuration);
                    }
                    break;
                case OperatorType.Root:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new LimitedIntegerType(
                                Algoritms.IntegerNthRoot(((LimitedIntegerType)operands[0]).Value, (int)((LimitedIntegerType)operands[1]).Value),
                                ((LimitedIntegerType)operands[0]).SignAndBitWidth,
                                configuration);
                    }
                    break;
                case OperatorType.Modulo:
                    switch (operands[1].DataType)
                    {
                        case DataType.LimitedInteger:
                            return new LimitedIntegerType(
                                ((LimitedIntegerType)operands[0]).Value % ((LimitedIntegerType)operands[1]).Value,
                                GetCommonBitWidthAndSign((LimitedIntegerType)operands[0], (LimitedIntegerType)operands[1]),
                                configuration);
                    }
                    break;
            }
            return null;
        }
        #endregion Operate

        public override string ToString(Verbosity verbosity)
        {
            switch (verbosity)
            {
                case Verbosity.ValueOnly: return $"{Value}";
                case Verbosity.ValueAndBitwidth: return $"{Value}{SignAndBitWidth}";
                case Verbosity.ValueAndType: return $"{Value}{SignAndBitWidth}: {(GetType().Name)}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(verbosity));
            }
        }

        readonly MinAndMax _minAndMax;
    }
}
