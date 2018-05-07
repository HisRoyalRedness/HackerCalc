﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace HisRoyalRedness.com
{
    public class LimitedIntegerToken : LiteralToken<BigInteger, LimitedIntegerToken>
    {
        #region Constructors
        public LimitedIntegerToken(string value, BigInteger typedValue, IntegerBitWidth bitWidth, bool isSigned)
            : base(TokenDataType.LimitedInteger, value, typedValue)
        {
            SignAndBitWidth = new SignAndBitWidthPair(bitWidth, isSigned);
        }

        public LimitedIntegerToken(BigInteger typedValue, IntegerBitWidth bitWidth, bool isSigned = true)
            : this(typedValue.ToString(), typedValue, bitWidth, isSigned)
        { }

        LimitedIntegerToken(BigInteger typedValue, SignAndBitWidthPair signAndBitWidth)
            : this(typedValue.ToString(), typedValue, signAndBitWidth.BitWidth, signAndBitWidth.IsSigned)
        { }
        #endregion Constructors

        #region Parsing
        public static LimitedIntegerToken Parse(string value, IntegerBase numBase, string bitWidth, bool isSigned)
            => Parse(value, numBase, ParseBitWidth(bitWidth), isSigned);
        public static LimitedIntegerToken Parse(string value, IntegerBase numBase, IntegerBitWidth bitWidth, bool isSigned)
        {
            switch (numBase)
            {
                case IntegerBase.Binary:
                    return new LimitedIntegerToken(value.Replace("b", "").Replace("B", "").BigIntegerFromBinary(), bitWidth, isSigned);
                case IntegerBase.Decimal:
                    return new LimitedIntegerToken(value, BigInteger.Parse(value, NumberStyles.Integer), bitWidth, isSigned);
                case IntegerBase.Hexadecimal:
                    return new LimitedIntegerToken(value, BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), bitWidth, isSigned);
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
            var signAndBitWidth = Upcast(SignAndBitWidth, new SignAndBitWidthPair(bitWidth, IsSigned));
            return signAndBitWidth == SignAndBitWidth
                ? this
                : new LimitedIntegerToken(TypedValue, signAndBitWidth);
        }

        static SignAndBitWidthPair Upcast(SignAndBitWidthPair left, SignAndBitWidthPair right)
        {
            // Loosely based on the C++ operator arithmetic operations
            // http://en.cppreference.com/w/cpp/language/operator_arithmetic

            // Get the greater of the 2 bitwidths
            var bitWidth = left.BitWidth >= right.BitWidth ? left.BitWidth : right.BitWidth;

            // If both are signed or both unsigned, just use the greater of the bitwidths
            if (!(left.IsSigned ^ right.IsSigned))
                return new SignAndBitWidthPair(bitWidth, left.IsSigned);

            // else, use the greater bitwidth and convert to unsigned
            return new SignAndBitWidthPair(bitWidth, false);
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

        public SignAndBitWidthPair SignAndBitWidth { get; private set; }
        public bool IsSigned => SignAndBitWidth.IsSigned;
        public IntegerBitWidth BitWidth => SignAndBitWidth.BitWidth;

        #region SignAndBitWidthPair
        public class SignAndBitWidthPair : IEquatable<SignAndBitWidthPair>
        {
            public SignAndBitWidthPair(IntegerBitWidth bitWidth, bool isSigned = true)
            {
                BitWidth = bitWidth;
                IsSigned = isSigned;
            }

            public IntegerBitWidth BitWidth { get; private set; }
            public bool IsSigned { get; private set; }

            #region Equality
            public bool Equals(SignAndBitWidthPair other)
                => BitWidth == other.BitWidth && IsSigned == other.IsSigned;
            public override bool Equals(object obj)
                => obj as SignAndBitWidthPair == null ? false : Equals((SignAndBitWidthPair)obj);

            public static bool operator==(SignAndBitWidthPair a, SignAndBitWidthPair b)
                => a.BitWidth == b.BitWidth && a.IsSigned == b.IsSigned;
            public static bool operator !=(SignAndBitWidthPair a, SignAndBitWidthPair b)
                => !(a == b);
            #endregion Equality

            public override int GetHashCode() => IsSigned ? -(int)BitWidth : (int)BitWidth;
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
    }
}
