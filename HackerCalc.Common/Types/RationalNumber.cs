using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

/*
    RationalNumber

        Representation of a rational number, 
        using an integer numerator and denominator

    Keith Fletcher
    May 2019

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    [DebuggerDisplay("{Fraction}")]
    public readonly struct RationalNumber : IEquatable<RationalNumber>, IComparable<RationalNumber>, IComparable
    {
        #region Constructors
        public RationalNumber(BigInteger number)
            : this(number, BigInteger.One)
        { }

        public RationalNumber(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == BigInteger.Zero)
            {
                if (numerator == BigInteger.Zero)
                {
                    Numerator = BigInteger.Zero;
                    Denominator = BigInteger.One;
                }
                else
                    throw new ParseException($"Invalid {nameof(RationalNumber)}. The denominator cannot be zero.");
            }
            else
            {
                var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
                var isNeg = numerator < 0 ^ denominator < 0;
                var num = BigInteger.Abs(numerator) / gcd;
                Numerator = isNeg ? BigInteger.Negate(num) : num;
                Denominator = BigInteger.Abs(denominator) / gcd;
            }

            var nHash = Numerator.GetHashCode();
            var dHash = Denominator.GetHashCode();
            unchecked
            {
                // https://stackoverflow.com/a/263416
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ nHash.GetHashCode();
                hash = (hash * 16777619) ^ dHash.GetHashCode();
                _hash = hash;
            }
        }
        #endregion Constructors

        #region Equality
        public override bool Equals(object obj) => obj is RationalNumber rn ? Equals(rn) : false;
        public bool Equals(RationalNumber other) => Numerator.Equals(other.Numerator) && Denominator.Equals(other.Denominator);
        public override int GetHashCode() => _hash;
        readonly int _hash;

        public static bool operator ==(RationalNumber a, RationalNumber b) => a.Equals(b);
        public static bool operator !=(RationalNumber a, RationalNumber b) => !(a == b);
        #endregion Equality

        #region IComparable
        public int CompareTo(RationalNumber other) => (Numerator * other.Denominator).CompareTo(other.Numerator * Denominator);

        public int CompareTo(object obj) => obj is RationalNumber rn ? CompareTo(rn) : 1;
        #endregion IComparable

        #region ToString
        public override string ToString() => ToString(true);

        public string ToString(bool includeFraction) => Denominator == 1
            ? $"{Numerator}"
            : (includeFraction
                ? $"{((double)Numerator / (double)Denominator).ToString("0.#####")} ({Fraction})"
                : $"{((double)Numerator / (double)Denominator).ToString("0.#####")}");

        public string Fraction => $"{Numerator}/{Denominator}";
        #endregion ToString

        #region Constants
        public static RationalNumber Zero { get; } = new RationalNumber(0);
        public static RationalNumber One { get; } = new RationalNumber(1);
        #endregion Constants

        public BigInteger Numerator { get; }
        public BigInteger Denominator { get; }

        static BigInteger LCM(BigInteger a, BigInteger b)
            => BigInteger.Abs(a * b) / BigInteger.GreatestCommonDivisor(a, b);

        public RationalNumber Reciprocal => new RationalNumber(Denominator, Numerator);

        #region Operators
        public static RationalNumber operator +(RationalNumber rn, RationalNumber other) => new RationalNumber(rn.Numerator * other.Denominator + other.Numerator * rn.Denominator, rn.Denominator * other.Denominator);
        public static RationalNumber operator +(RationalNumber rn, BigInteger other) => new RationalNumber(rn.Numerator + other * rn.Denominator, rn.Denominator);
        public static RationalNumber operator +(BigInteger other, RationalNumber rn) => rn + other;

        public static RationalNumber operator -(RationalNumber rn, RationalNumber other) => new RationalNumber(rn.Numerator * other.Denominator - other.Numerator * rn.Denominator, rn.Denominator * other.Denominator);
        public static RationalNumber operator -(RationalNumber rn, BigInteger other) => new RationalNumber(rn.Numerator - other * rn.Denominator, rn.Denominator);
        public static RationalNumber operator -(BigInteger other, RationalNumber rn) => new RationalNumber(other * rn.Denominator - rn.Numerator, rn.Denominator);

        public static RationalNumber operator *(RationalNumber rn, RationalNumber other) => new RationalNumber(rn.Numerator * other.Numerator, rn.Denominator * other.Denominator);
        public static RationalNumber operator *(RationalNumber rn, BigInteger other) => new RationalNumber(rn.Numerator * other, rn.Denominator);
        public static RationalNumber operator *(BigInteger other, RationalNumber rn) => rn * other;

        public static RationalNumber operator /(RationalNumber rn, RationalNumber other) => new RationalNumber(rn.Numerator * other.Denominator, rn.Denominator * other.Numerator);
        public static RationalNumber operator /(RationalNumber rn, BigInteger other) => new RationalNumber(rn.Numerator, rn.Denominator * other);
        public static RationalNumber operator /(BigInteger other, RationalNumber rn) => new RationalNumber(rn.Denominator * other, rn.Numerator);

        public static RationalNumber operator %(RationalNumber rn, RationalNumber other) => rn - ((BigInteger)(rn / other)) * other;

        public static RationalNumber Pow(RationalNumber num, RationalNumber exp)
        {
            var calcNum = num;
            if (exp.Numerator != BigInteger.One)
                calcNum = new RationalNumber(BigInteger.Pow(calcNum.Numerator, (int)exp.Numerator), BigInteger.Pow(calcNum.Denominator, (int)exp.Numerator));
            if (exp.Denominator != BigInteger.One)
            {
                throw new NotImplementedException("Haven't implemented RationalNumber roots yet");
                // TODO: Calculate root
            }

            return calcNum;
        }

        public static RationalNumber Root(RationalNumber num, RationalNumber exp) => Pow(num, exp.Reciprocal);
        #endregion Operators

        public static implicit operator RationalNumber(byte num) => new RationalNumber(num);
        public static implicit operator RationalNumber(sbyte num) => new RationalNumber(num);
        public static implicit operator RationalNumber(short num) => new RationalNumber(num);
        public static implicit operator RationalNumber(ushort num) => new RationalNumber(num);
        public static implicit operator RationalNumber(int num) => new RationalNumber(num);
        public static implicit operator RationalNumber(uint num) => new RationalNumber(num);
        public static implicit operator RationalNumber(long num) => new RationalNumber(num);
        public static implicit operator RationalNumber(ulong num) => new RationalNumber(num);
        public static implicit operator RationalNumber(BigInteger num) => new RationalNumber(num);
        public static implicit operator RationalNumber(double num) => new RationalNumber((BigInteger)num);

        public static explicit operator BigInteger(RationalNumber rn) => rn.Numerator / rn.Denominator;
        public static explicit operator int(RationalNumber rn) => (int)(rn.Numerator / rn.Denominator);
        public static explicit operator uint(RationalNumber rn) => (uint)(rn.Numerator / rn.Denominator);
        public static explicit operator long(RationalNumber rn) => (long)(rn.Numerator / rn.Denominator);
        public static explicit operator ulong(RationalNumber rn) => (ulong)(rn.Numerator / rn.Denominator);
        public static explicit operator double(RationalNumber rn) => (double)rn.Numerator / (double)rn.Denominator;
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
