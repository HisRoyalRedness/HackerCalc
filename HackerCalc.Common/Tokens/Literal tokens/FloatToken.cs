using System;

/*
    FloatToken

        Numeric floating-point literals that are parsed from input.

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class FloatToken : LiteralToken<double, FloatToken>
    {
        #region Constructors
        private FloatToken(double typedValue, string rawToken, SourcePosition position)
            : base(LiteralTokenType.Float, typedValue, rawToken, position)
        { }
        #endregion Constructors

        #region Parsing
        public static FloatToken Parse(string value, IConfiguration configuration)
            => FloatToken.Parse(value, false, value, SourcePosition.None, configuration);

        public static FloatToken Parse(string value, bool isNeg, string rawToken, SourcePosition position, IConfiguration configuration)
            => new FloatToken(double.Parse(isNeg ? $"-{value}" : value), $"{(isNeg ? "-" : "")}{rawToken}", position);
        #endregion Parsing

        #region Equality
        public override bool Equals(object obj) => Equals(obj as FloatToken);
        public override bool Equals(FloatToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(FloatToken a, FloatToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(FloatToken a, FloatToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(FloatToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        #region ToString
        public override string ToString() => $"{TypedValue:0.000}";
        #endregion ToString

        public static FloatToken Default
            => new FloatToken(0.0, null, SourcePosition.None);
        public static FloatToken One
            => new FloatToken(1.0, null, SourcePosition.None);
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
