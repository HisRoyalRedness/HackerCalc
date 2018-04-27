using System;
using System.Globalization;
using System.Numerics;

namespace HisRoyalRedness.com
{
    public class LimitedIntegerToken : LiteralToken<BigInteger, LimitedIntegerToken>
    {
        #region Constructors
        public LimitedIntegerToken(string value, BigInteger typedValue)
            : base(TokenDataType.LimitedInteger, value, typedValue)
        { }

        public LimitedIntegerToken(BigInteger typedValue)
            : this(typedValue.ToString(), typedValue)
        { }
        #endregion Constructors

        #region Parsing
        public static LimitedIntegerToken Parse(string value, bool isHex)
            => isHex
                ? new LimitedIntegerToken(BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber))
                : new LimitedIntegerToken(BigInteger.Parse(value, NumberStyles.Integer));
        #endregion Parsing

        //public override ILiteralToken Negate(NegationType negType)
        //    => new LimitedIntegerToken(TypedValue * -1);

        #region Operator overloads
        public static LimitedIntegerToken operator +(LimitedIntegerToken a, LimitedIntegerToken b)
            => new LimitedIntegerToken(a.TypedValue + b.TypedValue);
        public static LimitedIntegerToken operator -(LimitedIntegerToken a, LimitedIntegerToken b)
            => new LimitedIntegerToken(a.TypedValue - b.TypedValue);
        public static LimitedIntegerToken operator *(LimitedIntegerToken a, LimitedIntegerToken b)
            => new LimitedIntegerToken(a.TypedValue * b.TypedValue);
        public static LimitedIntegerToken operator /(LimitedIntegerToken a, LimitedIntegerToken b)
            => new LimitedIntegerToken(a.TypedValue / b.TypedValue);
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
    }
}
