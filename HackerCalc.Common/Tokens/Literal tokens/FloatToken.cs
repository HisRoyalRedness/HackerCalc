using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HisRoyalRedness.com
{
    public class FloatToken : LiteralToken<double, FloatToken>
    {
        public FloatToken(string value, double typedValue)
            : base(TokenDataType.Float, value, typedValue)
        { }

        public FloatToken(double typedValue)
            : this(typedValue.ToString(), typedValue)
        { }

        #region Parsing
        public static FloatToken Parse(string value)
            => new FloatToken(value, double.Parse(value));
        #endregion Parsing

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
        public override bool Equals(FloatToken other) => other == null ? false : (TypedValue == other.TypedValue);
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

        public override string ToString() => $"{TypedValue:0.000}F";
    }
}
