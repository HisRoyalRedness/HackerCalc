using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

/*
    LimitedIntegerToken

        Literals that are parsed from input, and determined to be integers
        that have a bitwidth attached.

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class LimitedIntegerToken : LiteralToken<BigInteger, LimitedIntegerToken>
    {
        #region Constructors
        public LimitedIntegerToken(BigInteger typedValue, IntegerBitWidth bitWidth, bool isSigned, bool isNeg, string rawToken)
            : base(LiteralTokenType.LimitedInteger, (isNeg ? typedValue * -1 : typedValue), rawToken)
        {
            if (isNeg)
                typedValue *= -1;
            SignAndBitWidth = new BitWidthAndSignPair(bitWidth, isSigned);
            var limit = _minAndMax[SignAndBitWidth];
            if (typedValue < limit.Min)
                throw new ParseException($"{typedValue} is less than the minimum of {limit.Min} for a {bitWidth.GetEnumDescription()}-bit {(isSigned ? "signed" : "unsigned")} {nameof(LimitedIntegerToken)} ");
            else if (typedValue > limit.Max)
                throw new ParseException($"{typedValue} is greater than the maximum of {limit.Max} for a {bitWidth.GetEnumDescription()}-bit {(isSigned ? "signed" : "unsigned")} {nameof(LimitedIntegerToken)} ");

            Min = _minAndMax[SignAndBitWidth].Min;
            Max = _minAndMax[SignAndBitWidth].Max;
            Mask = _minAndMax[SignAndBitWidth].Mask;
        }

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
        public static LimitedIntegerToken Parse(string value, IntegerBase numBase, IntegerBitWidth bitWidth, bool isSigned, bool isNeg, string rawToken = null)
        {
            switch (numBase)
            {
                case IntegerBase.Binary:
                    return new LimitedIntegerToken(value.Replace("b", "").Replace("B", "").BigIntegerFromBinary(), bitWidth, isSigned, isNeg, $"{(isNeg ? "-" : "")}{rawToken}");
                case IntegerBase.Octal:
                    return new LimitedIntegerToken(value.Replace("o", "").Replace("O", "").BigIntegerFromOctal(), bitWidth, isSigned, isNeg, $"{(isNeg ? "-" : "")}{rawToken}");
                case IntegerBase.Decimal:
                    return new LimitedIntegerToken(BigInteger.Parse(value, NumberStyles.Integer), bitWidth, isSigned, isNeg, $"{(isNeg ? "-" : "")}{rawToken}");
                case IntegerBase.Hexadecimal:
                    return new LimitedIntegerToken(BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), bitWidth, isSigned, isNeg, $"{(isNeg ? "-" : "")}{rawToken}");
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
                default: throw new ParseException("Invalid bit width");
            }
        }
        #endregion Parsing

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

        public BitWidthAndSignPair SignAndBitWidth { get; private set; }
        public bool IsSigned => SignAndBitWidth.IsSigned;
        public IntegerBitWidth BitWidth => SignAndBitWidth.BitWidth;

        public BigInteger Min { get; private set; }
        public BigInteger Max { get; private set; }
        public BigInteger Mask { get; private set; }

        static BigInteger MinValue(BitWidthAndSignPair signAndbitWidth) => _minAndMax[signAndbitWidth].Min;
        static BigInteger MaxValue(BitWidthAndSignPair signAndbitWidth) => _minAndMax[signAndbitWidth].Max;

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

            public static bool operator ==(BitWidthAndSignPair a, BitWidthAndSignPair b)
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

/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
*/
