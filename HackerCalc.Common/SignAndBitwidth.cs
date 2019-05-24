using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

/*
    Common types for handling sign and bitwidth
    for limited integer types (both tokens and data types)

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
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
            => (obj as BitWidthAndSignPair) is null ? false : Equals((BitWidthAndSignPair)obj);

        public static bool operator ==(BitWidthAndSignPair a, BitWidthAndSignPair b)
            => a.BitWidth == b.BitWidth && a.IsSigned == b.IsSigned;
        public static bool operator !=(BitWidthAndSignPair a, BitWidthAndSignPair b)
            => !(a == b);
        #endregion Equality

        public override int GetHashCode() => IsSigned ? -(int)BitWidth : (int)BitWidth;

        string DisplayString => $"{(IsSigned ? "I" : "U")}{(BitWidth == IntegerBitWidth.Unlimited ? "" : BitWidth.GetEnumDescription())}";
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
        _128 = 128,
        [Description("Unlimited")]
        Unlimited
    }
    #endregion IntegerBitWidth

    #region MinAndMax
    [DebuggerDisplay("{DisplayString}")]
    public struct MinAndMax
    {
        public MinAndMax(IntegerBitWidth bitWidth, bool isSigned)
        {
            if (bitWidth == IntegerBitWidth.Unlimited)
            {
                Mask = 0;
                Min = 0;
                Max = 0;
                IsUnlimited = true;
            }
            else
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
                IsUnlimited = false;
            }
        }

        public bool IsInRange(BigInteger value)
            => IsUnlimited || (value >= Min && value <= Max);

        public BigInteger Min { get; }
        public BigInteger Max { get; }
        public BigInteger Mask { get; }
        public bool IsUnlimited { get; }

        string DisplayString => IsUnlimited ? "Unlimited" : $"Min: {Min}, Max: {Max}, Mask: {Mask}";
        public override string ToString() => DisplayString;
    }

    public class MinAndMaxMap : IReadOnlyDictionary<BitWidthAndSignPair, MinAndMax>
    {
        #region Constructors
        private MinAndMaxMap(Dictionary<BitWidthAndSignPair, MinAndMax> content)
        {
            _minAndMax = new Dictionary<BitWidthAndSignPair, MinAndMax>(content);
        }

        static MinAndMaxMap()
        {
            _instance = new Lazy<MinAndMaxMap>(() =>
            {
                var content = new Dictionary<BitWidthAndSignPair, MinAndMax>();
                // Build the map of min and max values for each bitwidth and sign combination
                foreach (var bitWidth in Enum.GetValues(typeof(IntegerBitWidth)).Cast<IntegerBitWidth>())
                {
                    content.Add(new BitWidthAndSignPair(bitWidth, false), new MinAndMax(bitWidth, false));
                    content.Add(new BitWidthAndSignPair(bitWidth, true), new MinAndMax(bitWidth, true));
                }
                return new MinAndMaxMap(content);
            });
        }
        #endregion Constructors

        public static MinAndMaxMap Instance => _instance.Value;

        public BigInteger Min(BitWidthAndSignPair bitWidthAndSign) => Instance[bitWidthAndSign].Min;
        public BigInteger Max(BitWidthAndSignPair bitWidthAndSign) => Instance[bitWidthAndSign].Max;
        public BigInteger Mask(BitWidthAndSignPair bitWidthAndSign) => Instance[bitWidthAndSign].Mask;

        #region IReadOnlyDictionary implementation
        public IEnumerable<BitWidthAndSignPair> Keys => _minAndMax.Keys;
        public IEnumerable<MinAndMax> Values => _minAndMax.Values;
        public int Count => _minAndMax.Count;
        public MinAndMax this[BitWidthAndSignPair key] => _minAndMax[key];
        public bool ContainsKey(BitWidthAndSignPair key) => _minAndMax.ContainsKey(key);
        public bool TryGetValue(BitWidthAndSignPair key, out MinAndMax value) => _minAndMax.TryGetValue(key, out value);
        public IEnumerator<KeyValuePair<BitWidthAndSignPair, MinAndMax>> GetEnumerator() => _minAndMax.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _minAndMax.GetEnumerator();
        #endregion IReadOnlyDictionary implementation

        static Lazy<MinAndMaxMap> _instance;
        readonly Dictionary<BitWidthAndSignPair, MinAndMax> _minAndMax;
    }
    #endregion MinAndMax
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
