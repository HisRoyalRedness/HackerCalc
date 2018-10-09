﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

/*
    UnlimitedIntegerToken

        Literals that are parsed from input, and determined to not be
        bound by any particular bitwidth. They could in theory be infinite.

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class UnlimitedIntegerToken : LiteralToken<BigInteger, UnlimitedIntegerToken>
    {
        #region Constructors
        public UnlimitedIntegerToken(BigInteger typedValue)
            : base(LiteralTokenType.UnlimitedInteger, typedValue, typedValue.ToString())
        { }

        public UnlimitedIntegerToken(BigInteger typedValue, bool isNeg, string rawToken)
            : base(LiteralTokenType.UnlimitedInteger, (isNeg ? typedValue * -1 : typedValue), rawToken)
        { }
        #endregion Constructors

        #region Parsing
        public static UnlimitedIntegerToken Parse(string value, IntegerBase numBase, bool isNeg)
        {
            switch (numBase)
            {
                case IntegerBase.Binary:
                    return new UnlimitedIntegerToken(value.Replace("b", "").Replace("B", "").BigIntegerFromBinary(), isNeg, $"{(isNeg ? "-" : "")}{value}");
                case IntegerBase.Octal:
                    return new UnlimitedIntegerToken(value.Replace("o", "").Replace("O", "").BigIntegerFromOctal(), isNeg, $"{(isNeg ? "-" : "")}{value}");
                case IntegerBase.Decimal:
                    return new UnlimitedIntegerToken(BigInteger.Parse(value, NumberStyles.Integer), isNeg, $"{(isNeg ? "-" : "")}{value}");
                case IntegerBase.Hexadecimal:
                    return new UnlimitedIntegerToken(BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), isNeg, $"{(isNeg ? "-" : "")}{value}");
                default:
                    throw new ParseException($"Unhandled integer base {numBase}.");
            }
        }
        #endregion Parsing

        #region Equality
        public override bool Equals(object obj) => Equals(obj as UnlimitedIntegerToken);
        public override bool Equals(UnlimitedIntegerToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(UnlimitedIntegerToken a, UnlimitedIntegerToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(UnlimitedIntegerToken a, UnlimitedIntegerToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(UnlimitedIntegerToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        #region ToString
        public override string ToString() => TypedValue.ToString();
        #endregion ToString 
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
