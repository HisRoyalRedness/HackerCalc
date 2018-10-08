using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HisRoyalRedness.com
{
    public class FloatToken : LiteralToken<double, FloatToken>
    {
        #region Constructors
        public FloatToken(double typedValue, string rawToken = null)
            : base(LiteralTokenType.Float, typedValue, rawToken)
        { }
        #endregion Constructors

        #region Parsing
        public static FloatToken Parse(string value, bool isNeg, string rawToken)
            => new FloatToken(double.Parse(isNeg ? $"-{value}" : value), $"{(isNeg ? "-" : "")}{rawToken}");
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
    }
}
