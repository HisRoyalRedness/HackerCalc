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
            : base(value, DataType.LimitedInteger)
        {
            SignAndBitWidth = signAndBitwidth;
            _minAndMax = MinAndMaxMap.Instance[SignAndBitWidth];
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

        //LimitedInteger      LimitedInteger      +, -, *, /, **, //, %, <<, >>, &, |, ^, ~, !-
        //                    Float               **, //
        //                    UnlimitedInteger    %, &, |, ^

        readonly MinAndMax _minAndMax;
    }
}
