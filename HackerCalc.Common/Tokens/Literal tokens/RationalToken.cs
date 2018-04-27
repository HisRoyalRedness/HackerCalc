using System;
using System.Globalization;
using System.Numerics;

namespace HisRoyalRedness.com
{
    public class RationalToken : LiteralToken<BigInteger, RationalToken>
    {
        #region Constructors
        public RationalToken(string value, BigInteger typedValue)
            : base(TokenDataType.RationalNumber, value, typedValue)
        { }

        public RationalToken(BigInteger typedValue)
            : this(typedValue.ToString(), typedValue)
        { }
        #endregion Constructors

        #region Parsing
        public static RationalToken Parse(string value)
            => new RationalToken(BigInteger.Parse(value, NumberStyles.Integer));
        #endregion Parsing

        //public override ILiteralToken Negate()
        //    => new RationalToken(TypedValue * -1);

        #region Operator overloads
        public static RationalToken operator +(RationalToken a, RationalToken b)
            => new RationalToken(a.TypedValue + b.TypedValue);
        public static RationalToken operator -(RationalToken a, RationalToken b)
            => new RationalToken(a.TypedValue - b.TypedValue);
        public static RationalToken operator *(RationalToken a, RationalToken b)
            => new RationalToken(a.TypedValue * b.TypedValue);
        public static RationalToken operator /(RationalToken a, RationalToken b)
            => new RationalToken(a.TypedValue / b.TypedValue);
        #endregion Operator overloads

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(FloatToken):
                    return new FloatToken((double)TypedValue) as TToken;

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
        #endregion Casting

        #region Equality
        public override bool Equals(object obj) => Equals(obj as RationalToken);
        public override bool Equals(RationalToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(RationalToken a, RationalToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(RationalToken a, RationalToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(RationalToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        #region ToString
        public override string ToString() => TypedValue.ToString();
        #endregion ToString 
    }
}
