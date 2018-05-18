using System;
using System.Globalization;
using System.Numerics;

namespace HisRoyalRedness.com
{
    public class RationalNumberToken : LiteralToken<BigInteger, RationalNumberToken>
    {
        #region Constructors
        public RationalNumberToken(BigInteger typedValue)
            : base(TokenDataType.RationalNumber, typedValue)
        { }
        #endregion Constructors

        public override ILiteralToken NumericNegate()
            => new RationalNumberToken(TypedValue * -1);

        public override ILiteralToken BitwiseNegate()
            => throw new InvalidOperationException($"{nameof(RationalNumberToken)} does not support {nameof(BitwiseNegate)}, as it doesn't have a fixed bit width.");

        #region Operator overloads
        public static RationalNumberToken operator +(RationalNumberToken a, RationalNumberToken b)
            => new RationalNumberToken(a.TypedValue + b.TypedValue);
        public static RationalNumberToken operator -(RationalNumberToken a, RationalNumberToken b)
            => new RationalNumberToken(a.TypedValue - b.TypedValue);
        public static RationalNumberToken operator *(RationalNumberToken a, RationalNumberToken b)
            => new RationalNumberToken(a.TypedValue * b.TypedValue);
        public static RationalNumberToken operator /(RationalNumberToken a, RationalNumberToken b)
            => new RationalNumberToken(a.TypedValue / b.TypedValue);
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
        public override bool Equals(object obj) => Equals(obj as RationalNumberToken);
        public override bool Equals(RationalNumberToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(RationalNumberToken a, RationalNumberToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(RationalNumberToken a, RationalNumberToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(RationalNumberToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        #region ToString
        public override string ToString() => TypedValue.ToString();
        #endregion ToString 

        #region Other number bases
        public override string ToHex() => string.Empty;
        public override string ToDec() => string.Empty;
        public override string ToOct() => string.Empty;
        public override string ToBin() => string.Empty;
        #endregion Other number bases
    }
}
