using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Linq;


namespace HisRoyalRedness.com
{
    public class LimitedIntegerToken : LiteralToken<BigInteger, LimitedIntegerToken>
    {
        #region Constructors
        public LimitedIntegerToken(BigInteger typedValue, IntegerBitWidth bitWidth, bool isSigned)
            : base(TokenDataType.LimitedInteger, typedValue)
        {
            SignAndBitWidth = new BitWidthAndSignPair(bitWidth, isSigned);
            var limit = _minAndMax[SignAndBitWidth];
            if (typedValue < limit.Min)
                throw new ParseException($"{typedValue} is less than the minimum of {limit.Min} for a {bitWidth.GetEnumDescription()}-bit {(isSigned ? "signed" : "unsigned")} {nameof(LimitedIntegerToken)} ");
            else if (typedValue > limit.Max)
                throw new ParseException($"{typedValue} is greater than the maximum of {limit.Max} for a {bitWidth.GetEnumDescription()}-bit {(isSigned ? "signed" : "unsigned")} {nameof(LimitedIntegerToken)} ");
        }

        LimitedIntegerToken(BigInteger typedValue, BitWidthAndSignPair signAndBitWidth)
            : this(typedValue, signAndBitWidth.BitWidth, signAndBitWidth.IsSigned)
        { }

        LimitedIntegerToken(BigInteger typedValue, IntegerBitWidth bitWidth, bool isSigned, bool isNeg)
            : this((isNeg ? typedValue * -1 : typedValue), bitWidth, isSigned)
        { }

        static LimitedIntegerToken()
        {
            // Build the map of min and max values for each bitwidth and sign combination
            foreach (var bitWidth in Enum.GetValues(typeof(IntegerBitWidth)).Cast<IntegerBitWidth>())
            {
                _minAndMax.Add(new BitWidthAndSignPair(bitWidth, false), new MinAndMax(bitWidth, false));
                _minAndMax.Add(new BitWidthAndSignPair(bitWidth, true), new MinAndMax(bitWidth, true));
            }
        }
        #endregion Constructors

        #region Parsing
        public static LimitedIntegerToken Parse(string value, IntegerBase numBase, string bitWidth, bool isSigned, bool isNeg = false)
            => Parse(value, numBase, ParseBitWidth(bitWidth), isSigned, isNeg);
        public static LimitedIntegerToken Parse(string value, IntegerBase numBase, IntegerBitWidth bitWidth, bool isSigned,bool isNeg = false)
        {
            switch (numBase)
            {
                case IntegerBase.Binary:
                    return new LimitedIntegerToken(value.Replace("b", "").Replace("B", "").BigIntegerFromBinary(), bitWidth, isSigned, isNeg);
                case IntegerBase.Decimal:
                    return new LimitedIntegerToken(BigInteger.Parse(value, NumberStyles.Integer), bitWidth, isSigned, isNeg);
                case IntegerBase.Hexadecimal:
                    return new LimitedIntegerToken(BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), bitWidth, isSigned, isNeg);
                default:
                    throw new ParseException($"Unhandled integer base {numBase}.");
            }
        }

        public static IntegerBitWidth ParseBitWidth(string bitWidth)
        {
            switch (bitWidth.ToLower())
            {
                case "4":
                case "(i4)":
                case "(u4)":
                    return IntegerBitWidth._4;
                case "8":
                case "(i8)":
                case "(u8)":
                    return IntegerBitWidth._8;
                case "16":
                case "(i16)":
                case "(u16)":
                    return IntegerBitWidth._16;
                case "32":
                case "(i32)":
                case "(u32)":
                    return IntegerBitWidth._32;
                case "64":
                case "(i64)":
                case "(u64)":
                    return IntegerBitWidth._64;
                case "128":
                case "(i128)":
                case "(u128)":
                    return IntegerBitWidth._128;
                default: throw new ArgumentOutOfRangeException("Invalid bit width");
            }
        }
        #endregion Parsing

        #region Operator overloads
        public static LimitedIntegerToken operator +(LimitedIntegerToken a, LimitedIntegerToken b)
            => new LimitedIntegerToken(a.TypedValue + b.TypedValue, Upcast(a.SignAndBitWidth, b.SignAndBitWidth));
        public static LimitedIntegerToken operator -(LimitedIntegerToken a, LimitedIntegerToken b)
            => new LimitedIntegerToken(a.TypedValue - b.TypedValue, Upcast(a.SignAndBitWidth, b.SignAndBitWidth));
        public static LimitedIntegerToken operator *(LimitedIntegerToken a, LimitedIntegerToken b)
            => new LimitedIntegerToken(a.TypedValue * b.TypedValue, Upcast(a.SignAndBitWidth, b.SignAndBitWidth));
        public static LimitedIntegerToken operator /(LimitedIntegerToken a, LimitedIntegerToken b)
            => new LimitedIntegerToken(a.TypedValue / b.TypedValue, Upcast(a.SignAndBitWidth, b.SignAndBitWidth));
        #endregion Operator overloads

        public override ILiteralToken NumericNegate()
            => new LimitedIntegerToken(TypedValue * -1, SignAndBitWidth);

        public override ILiteralToken BitwiseNegate()
            => throw new InvalidOperationException($"{nameof(LimitedIntegerToken)} does not support {nameof(BitwiseNegate)}, as it doesn't have a fixed bit width.");

        public LimitedIntegerToken LeftShift(int shift)
            => new LimitedIntegerToken(TypedValue << shift, SignAndBitWidth);

        public LimitedIntegerToken RightShift(int shift)
            => new LimitedIntegerToken(TypedValue >> shift, SignAndBitWidth);

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(FloatToken):
                    return new FloatToken((double)TypedValue) as TToken;

                case nameof(UnlimitedIntegerToken):
                    return new UnlimitedIntegerToken(TypedValue) as TToken;

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

        public LimitedIntegerToken Upcast(IntegerBitWidth bitWidth, bool isSigned)
        {
            var signAndBitWidth = Upcast(SignAndBitWidth, new BitWidthAndSignPair(bitWidth, IsSigned));
            return signAndBitWidth == SignAndBitWidth
                ? this
                : new LimitedIntegerToken(TypedValue, signAndBitWidth);
        }

        static BitWidthAndSignPair Upcast(BitWidthAndSignPair left, BitWidthAndSignPair right)
        {
            // Loosely based on the C++ operator arithmetic operations
            // http://en.cppreference.com/w/cpp/language/operator_arithmetic

            // Get the greater of the 2 bitwidths
            var bitWidth = left.BitWidth >= right.BitWidth ? left.BitWidth : right.BitWidth;

            // If both are signed or both unsigned, just use the greater of the bitwidths
            if (!(left.IsSigned ^ right.IsSigned))
                return new BitWidthAndSignPair(bitWidth, left.IsSigned);

            // else, use the greater bitwidth and convert to unsigned
            return new BitWidthAndSignPair(bitWidth, false);
        }
        #endregion Casting

        #region Equality
        public override bool Equals(object obj) => Equals(obj as LimitedIntegerToken);
        public override bool Equals(LimitedIntegerToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(LimitedIntegerToken a, LimitedIntegerToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(LimitedIntegerToken a, LimitedIntegerToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(LimitedIntegerToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        #region ToString
        public override string ToString() => TypedValue.ToString();
        #endregion ToString 

        #region Other number bases
        public override string ToHex() => TypedValue.ToHexadecimalString().PadLeft((int)BitWidth / 4, '0').BatchWithDelim(4);
        public override string ToDec() => TypedValue.ToString().BatchWithDelim(3, ",");
        public override string ToOct() => TypedValue.ToOctalString().PadLeft(((int)BitWidth - 1) / 3 + 1, '0').BatchWithDelim(3);
        public override string ToBin() => TypedValue.ToBinaryString().PadLeft((int)BitWidth, '0').BatchWithDelim(4);
        #endregion Other number bases

        public BitWidthAndSignPair SignAndBitWidth { get; private set; }
        public bool IsSigned => SignAndBitWidth.IsSigned;
        public IntegerBitWidth BitWidth => SignAndBitWidth.BitWidth;

        public BigInteger Min => _minAndMax[SignAndBitWidth].Min;
        public BigInteger Max => _minAndMax[SignAndBitWidth].Max;
        public BigInteger Mask => _minAndMax[SignAndBitWidth].Mask;

        public static BigInteger MinValue(BitWidthAndSignPair signAndbitWidth) => _minAndMax[signAndbitWidth].Min;
        public static BigInteger MaxValue(BitWidthAndSignPair signAndbitWidth) => _minAndMax[signAndbitWidth].Max;

        #region SignAndBitWidthPair
        [DebuggerDisplay("{DisplayString}")]
        public class BitWidthAndSignPair : IEquatable<BitWidthAndSignPair>
        {
            public BitWidthAndSignPair(IntegerBitWidth bitWidth, bool isSigned = true)
            {
                BitWidth = bitWidth;
                IsSigned = isSigned;
            }

            public IntegerBitWidth BitWidth { get; private set; }
            public bool IsSigned { get; private set; }

            #region Equality
            public bool Equals(BitWidthAndSignPair other)
                => BitWidth == other.BitWidth && IsSigned == other.IsSigned;
            public override bool Equals(object obj)
                => obj as BitWidthAndSignPair == null ? false : Equals((BitWidthAndSignPair)obj);

            public static bool operator==(BitWidthAndSignPair a, BitWidthAndSignPair b)
                => a.BitWidth == b.BitWidth && a.IsSigned == b.IsSigned;
            public static bool operator !=(BitWidthAndSignPair a, BitWidthAndSignPair b)
                => !(a == b);
            #endregion Equality

            public override int GetHashCode() => IsSigned ? -(int)BitWidth : (int)BitWidth;

            string DisplayString => $"{(IsSigned ? "I" : "U")}{BitWidth.GetEnumDescription()}";
            public override string ToString() => DisplayString;
        }
        #endregion SignAndBitWidthPair

        #region IntegerBitWidth
        public enum IntegerBitWidth
        {
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
        #endregion IntegerBitWidth

        #region MinAndMax
        [DebuggerDisplay("{DisplayString}")]
        struct MinAndMax
        {
            public MinAndMax(BigInteger min, BigInteger max, BigInteger mask)
            {
                Min = min;
                Max = max;
                Mask = mask;
            }

            public MinAndMax(LimitedIntegerToken.IntegerBitWidth bitWidth, bool isSigned)
            {
                Mask = BigInteger.Pow(2, (int)bitWidth) - 1;
                if (isSigned)
                {
                    var num = BigInteger.Pow(2, (int)bitWidth - 1);
                    Min = -num;
                    Max = num - 1;
                }
                else
                {
                    Min = 0;
                    Max = Mask;
                }
            }

            public BigInteger Min;
            public BigInteger Max;
            public BigInteger Mask;

            string DisplayString => $"Min: {Min}, Max: {Max}, Mask: {Mask}";
            public override string ToString() => DisplayString;
        }
        #endregion MinAndMax

        static readonly Dictionary<BitWidthAndSignPair, MinAndMax> _minAndMax = new Dictionary<BitWidthAndSignPair, MinAndMax>();
    }
}
