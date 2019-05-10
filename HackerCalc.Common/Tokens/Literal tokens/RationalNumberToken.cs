using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

/*
    RationalNumberToken

        Rational literals that are parsed from input.

    Keith Fletcher
    May 2019

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class RationalNumberToken : LiteralToken<RationalNumber, RationalNumberToken>
    {
        #region Constructors
        private RationalNumberToken(RationalNumber typedValue, string rawToken, SourcePosition position)
            : base(LiteralTokenType.Rational, typedValue, rawToken, position)
        { }
        #endregion Constructors

        #region Parsing
        public static RationalNumberToken Parse(string value, IntegerBase numBase, bool isNeg, bool isFloat, SourcePosition position, IConfiguration configuration)
        {
            if (isFloat)
                throw new NotImplementedException();
            else
            {
                BigInteger val;
                switch (numBase)
                {
                    case IntegerBase.Binary: val = value.StripLeadingType("b").BigIntegerFromBinary(); break;
                    case IntegerBase.Octal: val = value.StripLeadingType("o").BigIntegerFromOctal(); break;
                    case IntegerBase.Decimal: val = BigInteger.Parse(value, NumberStyles.Integer); break;
                    case IntegerBase.Hexadecimal: val = BigInteger.Parse(value.StripLeadingType("0x", "00"), NumberStyles.HexNumber); break;
                    default:
                        throw new ParseException($"Unhandled integer base {numBase}.");
                }
                return new RationalNumberToken(new RationalNumber(isNeg ? BigInteger.Negate(val) : val), $"{(isNeg ? "-" : "")}{value}", position);
            }
        }
        #endregion Parsing

        #region Equality
        public override bool Equals(object obj) => obj is RationalNumberToken rt ? Equals(rt) : false;
        public override bool Equals(RationalNumberToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(RationalNumberToken a, RationalNumberToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(RationalNumberToken a, RationalNumberToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(RationalNumberToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        #region ToString
        public override string ToString()
            => TypedValue.Denominator == BigInteger.One
                ? TypedValue.Numerator.ToString()
                : $"{((double)TypedValue.Numerator / (double)TypedValue.Denominator):0.###}";
        #endregion ToString

        public static RationalNumberToken Default
            => new RationalNumberToken(RationalNumber.Zero, "0", SourcePosition.None);
        public static RationalNumberToken One
            => new RationalNumberToken(RationalNumber.One, "1", SourcePosition.None);
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
