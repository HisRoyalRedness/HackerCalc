using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace HisRoyalRedness.com
{
    public class IntegerToken : LiteralToken<BigInteger, IntegerToken>
    {
        #region Constructors
        public IntegerToken(string value, BigInteger typedValue, bool isSigned = true, IntegerBitWidth bitWidth = IntegerBitWidth.Unbound, bool errorOnOverflow = true)
            : base(TokenDataType.Integer, value, typedValue)
        {
            BitWidth = bitWidth;

            if (IsUnbound)
                IsSigned = true;
            else
            {
                IsSigned = isSigned;
                NormaliseInternal(errorOnOverflow);
            }
        }

        public IntegerToken(BigInteger typedValue, bool isSigned = true, IntegerBitWidth bitWidth = IntegerBitWidth.Unbound)
            : this(typedValue.ToString(), typedValue, isSigned, bitWidth)
        { }

        static IntegerToken()
        {
            var bitWidths = Enum.GetValues(typeof(IntegerBitWidth)).Cast<IntegerBitWidth>().Where(bw => bw != IntegerBitWidth.Unbound);
            foreach (var bitWidth in bitWidths)
            {
                _minAndMax.Add(new SignAndBitwidthPair(false, bitWidth), new MinAndMax(bitWidth, false));
                _minAndMax.Add(new SignAndBitwidthPair(true, bitWidth), new MinAndMax(bitWidth, true));
            }
            //foreach(var key in _minAndMax.Keys)
            //    Console.WriteLine($"{key, 8}: {_minAndMax[key].Min, -40}  {_minAndMax[key].Max,-40}  {_minAndMax[key].Mask,-40}");
        }
        #endregion Constructors

        #region Parsing
        public static IntegerToken Parse(string value, IntegerBase numBase)
            => Parse(value, numBase, true, IntegerBitWidth.Unbound);

        public static IntegerToken Parse(string value, IntegerBase numBase, bool isSigned, IntegerBitWidth bitWidth)
        {
            switch (numBase)
            {
                case IntegerBase.Decimal:
                    return new IntegerToken(value, BigInteger.Parse(value, NumberStyles.Integer), isSigned, bitWidth);
                case IntegerBase.Hexadecimal:
                    return new IntegerToken(value, BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), isSigned, bitWidth);
                default:
                    throw new ApplicationException($"Unhandled integer base {numBase}.");
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
        public static IntegerToken operator +(IntegerToken a, IntegerToken b)
            => new IntegerToken(a.TypedValue + b.TypedValue, a.IsSigned, a.BitWidth);
        public static IntegerToken operator -(IntegerToken a, IntegerToken b)
            => new IntegerToken(a.TypedValue - b.TypedValue, a.IsSigned, a.BitWidth);
        public static IntegerToken operator *(IntegerToken a, IntegerToken b)
            => new IntegerToken(a.TypedValue * b.TypedValue, a.IsSigned, a.BitWidth);
        public static IntegerToken operator /(IntegerToken a, IntegerToken b)
            => new IntegerToken(a.TypedValue / b.TypedValue, a.IsSigned, a.BitWidth);
        #endregion Operator overloads

        public IntegerToken LeftShift(int shift)
            => new IntegerToken(TypedValue << shift);

        public IntegerToken RightShift(int shift)
            => new IntegerToken(TypedValue >> shift);

        #region Casting
        public IntegerToken Cast(bool isSigned = true, IntegerBitWidth bitWidth = IntegerBitWidth.Unbound, bool errorOnOverflow = true)
        {
            if (bitWidth == BitWidth && isSigned == IsSigned)
                    return this;
            return new IntegerToken(Value, TypedValue, isSigned, bitWidth, errorOnOverflow);
        }

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

                case nameof(LimitedIntegerToken):
                    return new LimitedIntegerToken(TypedValue) as TToken;

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

        #region Equality
        public override bool Equals(object obj) => Equals(obj as IntegerToken);
        public override bool Equals(IntegerToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(IntegerToken a, IntegerToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            if (a.IsUnbound && b.IsUnbound)
                return a.TypedValue == b.TypedValue;
            return a.TypedValue == b.TypedValue;

        }
        public static bool operator !=(IntegerToken a, IntegerToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(IntegerToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        public static BigInteger MinValue(bool isSigned, IntegerBitWidth bitWidth)
        {
            if (bitWidth == IntegerBitWidth.Unbound)
                throw new ArgumentOutOfRangeException("No limit defined for Unbound IntegerTokens");
            return _minAndMax[new SignAndBitwidthPair(isSigned, bitWidth)].Min;

        }

        public static BigInteger MaxValue(bool isSigned, IntegerBitWidth bitWidth)
        {
            if (bitWidth == IntegerBitWidth.Unbound)
                throw new ArgumentOutOfRangeException("No limit defined for Unbound IntegerTokens");
            return _minAndMax[new SignAndBitwidthPair(isSigned, bitWidth)].Max;
        }

        void NormaliseInternal(bool errorOnOverflow)
        {
            if (IsUnbound)
                return;



            var key = new SignAndBitwidthPair(IsSigned, BitWidth);
            if (!_minAndMax.ContainsKey(key))
                throw new ArgumentOutOfRangeException($"Could not find a minimum and maximum value for {(IsSigned ? "a signed" : "an unsigned")} {nameof(IntegerToken)} with a bit width of {(int)BitWidth}");

            if (errorOnOverflow)
            {
                if (TypedValue > _minAndMax[key].Max)
                    throw new IntegerOverflowException($"The value {TypedValue} is greater than the maximum of {_minAndMax[key].Max} for {(IsSigned ? "a signed" : "an unsigned")} {nameof(IntegerToken)} with a bit width of {(int)BitWidth}");
                else if (TypedValue < _minAndMax[key].Min)
                    throw new IntegerOverflowException($"The value {TypedValue} is less than the minimum of {_minAndMax[key].Min} for {(IsSigned ? "a signed" : "an unsigned")} {nameof(IntegerToken)} with a bit width of {(int)BitWidth}");
            }
            else
            {
                TypedValue &= _minAndMax[key].Mask;
                //if (!IsSigned && TypedValue < 0)
                //    TypedValue += _minAndMax[key].Mask + 1;
                //else if (IsSigned && TypedValue > _minAndMax[key].Max)
                //{
                //    if (TypedValue < 0)
                //        TypedValue += _minAndMax[key].Mask + 1;

                //}
            }
        }

        [DebuggerDisplay("{DisplayString}")]
        struct SignAndBitwidthPair
        {
            public SignAndBitwidthPair(bool isSigned, IntegerBitWidth bitWidth)
            {
                IsSigned = isSigned;
                BitWidth = bitWidth;
            }

            public bool IsSigned;
            public IntegerBitWidth BitWidth;
            string DisplayString => $"{(IsSigned ? "I" : "U")}{(int)BitWidth}";
            public override string ToString() => DisplayString;
        }

        [DebuggerDisplay("{DisplayString}")]
        struct MinAndMax
        {
            public MinAndMax(BigInteger min, BigInteger max, BigInteger mask)
            {
                Min = min;
                Max = max;
                Mask = mask;
            }

            public MinAndMax(IntegerBitWidth bitWidth, bool isSigned)
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

        static readonly Dictionary<SignAndBitwidthPair, MinAndMax> _minAndMax = new Dictionary<SignAndBitwidthPair, MinAndMax>();

        public bool IsSigned { get; private set; }
        public IntegerBitWidth BitWidth { get; private set; }
        public bool IsUnbound => BitWidth == IntegerBitWidth.Unbound;

        #region ToString
        public override string ToString() => IsUnbound
            ? $"{TypedValue}"
            : $"{TypedValue}{(IsSigned ? "I" : "U")}{BitWidth.GetEnumDescription()}";
        #endregion ToString

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
    }
}
