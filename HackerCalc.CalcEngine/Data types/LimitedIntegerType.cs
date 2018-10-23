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

        public BitWidthAndSignPair SignAndBitWidth { get; private set; }
        public bool IsSigned => SignAndBitWidth.IsSigned;
        public IntegerBitWidth BitWidth => SignAndBitWidth.BitWidth;

        public BigInteger Min => _minAndMax.Min;
        public BigInteger Max => _minAndMax.Max;
        public BigInteger Mask => _minAndMax.Mask;

        readonly MinAndMax _minAndMax;
        protected override TNewType InternalCastTo<TNewType>() => null;
    }
}
