using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HisRoyalRedness.com
{
    public class FloatToken : LiteralToken<double, FloatToken>
    {
        #region Constructors
        public FloatToken(string value, double typedValue)
            : base(TokenDataType.Float, value, typedValue)
        { }

        public FloatToken(double typedValue)
            : this(typedValue.ToString(), typedValue)
        { }
        #endregion Constructors

        #region Parsing
        public static FloatToken Parse(string value)
            => new FloatToken(value, double.Parse(value));
        #endregion Parsing

        #region Operator overloads
        public static FloatToken operator +(FloatToken a, FloatToken b)
            => new FloatToken(a.TypedValue + b.TypedValue);
        public static FloatToken operator -(FloatToken a, FloatToken b)
            => new FloatToken(a.TypedValue - b.TypedValue);
        public static FloatToken operator *(FloatToken a, FloatToken b)
            => new FloatToken(a.TypedValue * b.TypedValue);
        public static FloatToken operator /(FloatToken a, FloatToken b)
            => new FloatToken(a.TypedValue / b.TypedValue);
        #endregion Operator overloads

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(IntegerToken):
                    return new IntegerToken(new BigInteger(TypedValue)) as TToken;

                case nameof(TimespanToken):
                    return new TimespanToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                case nameof(TimeToken):
                    return new TimeToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                case nameof(DateToken):
                    return new DateToken(DateTime.Now.Date + TimeSpan.FromSeconds(TypedValue)) as TToken;

                default:
                    return null;
            }
        }
        #endregion Casting

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
        public override string ToString() => $"{TypedValue:0.000}F";
        #endregion ToString
    }
}
