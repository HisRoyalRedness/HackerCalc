using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace HisRoyalRedness.com
{
    public class IntegerToken : LiteralToken<BigInteger>
    {
        public enum IntegerBitWidth
        {
            [Description("Unbound")]
            Unbound = 0,
            [Description("4")]
            _4 = 4,
            [Description("8")]
            _8 = 8,
            [Description("16")]
            _16 = 16,
            [Description("32")]
            _32 = 32,
            [Description("64")]
            _64 = 64,
            [Description("128")]
            _128 = 128
        }

        public IntegerToken(string value, BigInteger typedValue, bool isSigned = true, IntegerBitWidth bitWidth = IntegerBitWidth.Unbound)
            : base(TokenDataType.Integer, value, typedValue)
        {
            IsSigned = isSigned;
            BitWidth = bitWidth;
        }

        public IntegerToken(BigInteger typedValue, bool isSigned = true, IntegerBitWidth bitWidth = IntegerBitWidth.Unbound)
            : base(TokenDataType.Integer, typedValue)
        {
            IsSigned = isSigned;
            BitWidth = bitWidth;
        }

        #region Parsing
        public static IntegerToken Parse(string value, bool isHex)
            => isHex
                ? new IntegerToken(value, BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), true, IntegerBitWidth.Unbound)
                : new IntegerToken(value, BigInteger.Parse(value, NumberStyles.Integer), true, IntegerBitWidth.Unbound);

        public static IntegerToken Parse(string value, bool isHex, bool isSigned, IntegerBitWidth bitWidth)
            => isHex
                ? new IntegerToken(value, BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), isSigned, bitWidth)
                : new IntegerToken(value, BigInteger.Parse(value, NumberStyles.Integer), isSigned, bitWidth);

        public static IntegerBitWidth ParseBitWidth(string bitWidth)
        {
            switch (bitWidth)
            {
                case "4": return IntegerBitWidth._4;
                case "8": return IntegerBitWidth._8;
                case "16": return IntegerBitWidth._16;
                case "32": return IntegerBitWidth._32;
                case "64": return IntegerBitWidth._64;
                case "128": return IntegerBitWidth._128;
                default: throw new ArgumentOutOfRangeException("Invalid bit width");
            }
        }
        #endregion Parsing

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(FloatToken):
                    return new FloatToken((double)TypedValue) as TToken;

                case nameof(TimespanToken):
                    return new TimespanToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                case nameof(TimeToken):
                    return new TimeToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                case nameof(DateToken):
                    return new DateToken(DateTime.Now.Date + TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                default:
                    return null;
            }
        }
        #endregion Casting

        public bool IsSigned { get; private set; }
        public IntegerBitWidth BitWidth { get; private set; }

        public override string ToString() => $"{Value}_{(IsSigned ? "I" : "U")}{BitWidth.GetEnumDescription()}  -  {TypedValue}";
    }
}
