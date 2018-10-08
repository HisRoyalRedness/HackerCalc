using System;
using System.Globalization;
using System.Numerics;

namespace HisRoyalRedness.com
{
    public class OldRationalNumberToken : OldLiteralToken<BigInteger, OldRationalNumberToken>
    {
        #region Constructors
        public OldRationalNumberToken(BigInteger typedValue)
            : base(TokenDataType.RationalNumber, typedValue)
        { }
        #endregion Constructors

        public override IOldLiteralToken NumericNegate()
            => new OldRationalNumberToken(TypedValue * -1);

        public override IOldLiteralToken BitwiseNegate()
            => throw new InvalidOperationException($"{nameof(OldRationalNumberToken)} does not support {nameof(BitwiseNegate)}, as it doesn't have a fixed bit width.");

        #region Operator overloads
        public static OldRationalNumberToken operator +(OldRationalNumberToken a, OldRationalNumberToken b)
            => new OldRationalNumberToken(a.TypedValue + b.TypedValue);
        public static OldRationalNumberToken operator -(OldRationalNumberToken a, OldRationalNumberToken b)
            => new OldRationalNumberToken(a.TypedValue - b.TypedValue);
        public static OldRationalNumberToken operator *(OldRationalNumberToken a, OldRationalNumberToken b)
            => new OldRationalNumberToken(a.TypedValue * b.TypedValue);
        public static OldRationalNumberToken operator /(OldRationalNumberToken a, OldRationalNumberToken b)
            => new OldRationalNumberToken(a.TypedValue / b.TypedValue);
        #endregion Operator overloads

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(OldFloatToken):
                    return new OldFloatToken((double)TypedValue) as TToken;

                case nameof(OldTimespanToken):
                    return new OldTimespanToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                case nameof(OldTimeToken):
                    return new OldTimeToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                case nameof(OldDateToken):
                    return new OldDateToken(DateTime.Now.Date + TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                default:
                    return null;
            }
        }
        #endregion Casting

        #region Equality
        public override bool Equals(object obj) => Equals(obj as OldRationalNumberToken);
        public override bool Equals(OldRationalNumberToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(OldRationalNumberToken a, OldRationalNumberToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(OldRationalNumberToken a, OldRationalNumberToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(OldRationalNumberToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
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
