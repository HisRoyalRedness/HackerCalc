using System;
using System.Globalization;
using System.Numerics;

namespace HisRoyalRedness.com
{
    public class UnlimitedIntegerToken : LiteralToken<BigInteger, UnlimitedIntegerToken>
    {
        #region Constructors
        public UnlimitedIntegerToken(string value, BigInteger typedValue)
            : base(TokenDataType.UnlimitedInteger, value, typedValue)
        { }

        public UnlimitedIntegerToken(BigInteger typedValue)
            : this(typedValue.ToString(), typedValue)
        { }
        #endregion Constructors

        #region Parsing
        public static UnlimitedIntegerToken Parse(string value, bool isHex)
            => isHex
                ? new UnlimitedIntegerToken(BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber))
                : new UnlimitedIntegerToken(BigInteger.Parse(value, NumberStyles.Integer));
        #endregion Parsing

        public override ILiteralToken NumericNegate()
            => new UnlimitedIntegerToken(TypedValue * -1);

        #region Operator overloads
        public static UnlimitedIntegerToken operator +(UnlimitedIntegerToken a, UnlimitedIntegerToken b)
            => new UnlimitedIntegerToken(a.TypedValue + b.TypedValue);
        public static UnlimitedIntegerToken operator -(UnlimitedIntegerToken a, UnlimitedIntegerToken b)
            => new UnlimitedIntegerToken(a.TypedValue - b.TypedValue);
        public static UnlimitedIntegerToken operator *(UnlimitedIntegerToken a, UnlimitedIntegerToken b)
            => new UnlimitedIntegerToken(a.TypedValue * b.TypedValue);
        public static UnlimitedIntegerToken operator /(UnlimitedIntegerToken a, UnlimitedIntegerToken b)
            => new UnlimitedIntegerToken(a.TypedValue / b.TypedValue);
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
