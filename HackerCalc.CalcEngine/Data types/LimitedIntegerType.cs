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
        public LimitedIntegerType(BigInteger value, BitWidthAndSignPair signAndBitwidth)
            : base(Normalise(value, signAndBitwidth), DataType.LimitedInteger)
        {
            SignAndBitWidth = signAndBitwidth;
            _minAndMax = MinAndMaxMap.Instance[SignAndBitWidth];

        }

        static BigInteger Normalise(BigInteger value, BitWidthAndSignPair signAndBitwidth)
        {
            var minAndMax = MinAndMaxMap.Instance[signAndBitwidth];
            var oldValue = value;
            value &= minAndMax.Mask;

            if (oldValue != value)
            {
                if (DataMapper.Settings.AllowOverOrUnderflow)
                    DataMapper.State.OverOrUnderflowOccurred = true;
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
                if (DataMapper.Settings.AllowSignChange)
                    DataMapper.State.SignChangedOccurred = true;
                else
                    throw new InvalidCalcOperationException("Sign change through overflow or underflow of LimitedIntegerTypes is not permitted.");
            }

            return value;
        }

        public static LimitedIntegerType CreateLimitedIntegerType(BigInteger value)
        {
            var isSigned = value < 0;
            var bitWidth = EnumExtensions
                .GetEnumCollection<IntegerBitWidth>()
                .FirstOrDefault(bw => isSigned 
                    ? MinAndMaxMap.Instance[new BitWidthAndSignPair(bw, isSigned)].Min <= value
                    : MinAndMaxMap.Instance[new BitWidthAndSignPair(bw, isSigned)].Max >= value);
            if (bitWidth == 0)
                throw new TypeConversionException($"{value} is out of range of a {nameof(LimitedIntegerType)}.");
            return new LimitedIntegerType(value, new BitWidthAndSignPair(bitWidth, isSigned));
        }

        public BitWidthAndSignPair SignAndBitWidth { get; private set; }
        public bool IsSigned => SignAndBitWidth.IsSigned;
        public IntegerBitWidth BitWidth => SignAndBitWidth.BitWidth;

        public BigInteger Min => _minAndMax.Min;
        public BigInteger Max => _minAndMax.Max;
        public BigInteger Mask => _minAndMax.Mask;

        #region Type casting
        protected override TNewType InternalCastTo<TNewType>()
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

        #region Operator overloads
        public static LimitedIntegerType operator +(LimitedIntegerType a, LimitedIntegerType b)
            => new LimitedIntegerType(a.Value + b.Value, GetCommonBitWidthAndSign(a, b));
        public static LimitedIntegerType operator -(LimitedIntegerType a, LimitedIntegerType b)
            => new LimitedIntegerType(a.Value - b.Value, GetCommonBitWidthAndSign(a, b));
        public static LimitedIntegerType operator *(LimitedIntegerType a, LimitedIntegerType b)
            => new LimitedIntegerType(a.Value * b.Value, GetCommonBitWidthAndSign(a, b));
        public static LimitedIntegerType operator /(LimitedIntegerType a, LimitedIntegerType b)
            => new LimitedIntegerType(a.Value / b.Value, GetCommonBitWidthAndSign(a, b));
        public static LimitedIntegerType operator %(LimitedIntegerType a, LimitedIntegerType b)
            => new LimitedIntegerType(a.Value % b.Value, GetCommonBitWidthAndSign(a, b));
        public static LimitedIntegerType operator &(LimitedIntegerType a, LimitedIntegerType b)
            => new LimitedIntegerType(a.Value & b.Value, GetCommonBitWidthAndSign(a, b));
        public static LimitedIntegerType operator |(LimitedIntegerType a, LimitedIntegerType b)
            => new LimitedIntegerType(a.Value | b.Value, GetCommonBitWidthAndSign(a, b));
        public static LimitedIntegerType operator ^(LimitedIntegerType a, LimitedIntegerType b)
            => new LimitedIntegerType(a.Value ^ b.Value, GetCommonBitWidthAndSign(a, b));
        public static LimitedIntegerType operator <<(LimitedIntegerType a, int b)
            => new LimitedIntegerType(a.Value << b, a.SignAndBitWidth);
        public static LimitedIntegerType operator >>(LimitedIntegerType a, int b)
            => new LimitedIntegerType(a.Value >> b, a.SignAndBitWidth);

        // Unary
        public static LimitedIntegerType operator ~(LimitedIntegerType a)
            => new LimitedIntegerType(~a.Value, a.SignAndBitWidth);
        public static LimitedIntegerType operator -(LimitedIntegerType a)
            => new LimitedIntegerType(-a.Value, a.SignAndBitWidth);
        #endregion Operator overloads

        public override string ToString(Verbosity verbosity)
        {
            switch (verbosity)
            {
                case Verbosity.ValueOnly: return $"{Value}";
                case Verbosity.ValueAndType: return $"{Value}{SignAndBitWidth}: {(GetType().Name)}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(verbosity));
            }
        }

        //#region Operator implementations
        //public static FloatType pow(FloatType a, FloatType b)
        //    => new FloatType(Math.Pow(a.Value, b.Value));
        //public static FloatType root(FloatType a, FloatType b)
        //    => new FloatType(Math.Pow(a.Value, 1.0 / b.Value));
        //public static FloatType neg(FloatType a)
        //    => new FloatType(-a.Value);
        //#endregion Operator implementations

        //LimitedInteger      LimitedInteger      +, -, *, /, **, //, %, <<, >>, &, |, ^, ~, !-
        //                    Float               **, //
        //                    UnlimitedInteger    %, &, |, ^

        readonly MinAndMax _minAndMax;
    }
}
