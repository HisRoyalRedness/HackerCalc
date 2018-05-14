using System;
using System.Globalization;
using System.Numerics;
using System.Linq;

namespace HisRoyalRedness.com
{
    public class UnlimitedIntegerToken : LiteralToken<BigInteger, UnlimitedIntegerToken>
    {
        #region Constructors
        public UnlimitedIntegerToken(BigInteger typedValue)
            : base(TokenDataType.UnlimitedInteger, typedValue)
        { }

        UnlimitedIntegerToken(BigInteger typedValue, bool isNeg)
            : this(isNeg ? typedValue * -1 : typedValue)
        { }

        #endregion Constructors

        #region Parsing
        public static UnlimitedIntegerToken Parse(string value, IntegerBase numBase, bool isNeg = false)
        {
            switch (numBase)
            {
                case IntegerBase.Binary:
                    return new UnlimitedIntegerToken(value.Replace("b", "").Replace("B", "").BigIntegerFromBinary(), isNeg);
                case IntegerBase.Decimal:
                    return new UnlimitedIntegerToken(BigInteger.Parse(value, NumberStyles.Integer), isNeg);
                case IntegerBase.Hexadecimal:
                    return new UnlimitedIntegerToken(BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), isNeg);
                default:
                    throw new ParseException($"Unhandled integer base {numBase}.");
            }
        }
        #endregion Parsing

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

        public override ILiteralToken NumericNegate()
            => new UnlimitedIntegerToken(TypedValue * -1);

        public override ILiteralToken BitwiseNegate()
            => throw new InvalidOperationException($"{nameof(UnlimitedIntegerToken)} does not support {nameof(BitwiseNegate)}, as it doesn't have a fixed bit width.");

        public UnlimitedIntegerToken LeftShift(int shift)
            => new UnlimitedIntegerToken(TypedValue << shift);

        public UnlimitedIntegerToken RightShift(int shift)
            => new UnlimitedIntegerToken(TypedValue >> shift);

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(FloatToken):
                    return new FloatToken((double)TypedValue) as TToken;

                case nameof(LimitedIntegerToken):
                    {
                        if (TypedValue < 0)
                        {
                            foreach (var signAndBitwidth in EnumExtensions.GetEnumCollection<LimitedIntegerToken.IntegerBitWidth>().Select(bw => new LimitedIntegerToken.BitWidthAndSignPair(bw, true)))
                                if (TypedValue >= LimitedIntegerToken.MinValue(signAndBitwidth))
                                    return new LimitedIntegerToken(TypedValue, signAndBitwidth.BitWidth, signAndBitwidth.IsSigned) as TToken;
                        }
                        else
                        {
                            foreach (var bw in EnumExtensions.GetEnumCollection<LimitedIntegerToken.IntegerBitWidth>())
                                foreach (var signAndBitwidth in new[] { true, false }.Select(s => new LimitedIntegerToken.BitWidthAndSignPair(bw, s)))
                                    if (TypedValue <= LimitedIntegerToken.MaxValue(signAndBitwidth))
                                        return new LimitedIntegerToken(TypedValue, signAndBitwidth.BitWidth, signAndBitwidth.IsSigned) as TToken;
                        }
                        throw new IntegerOverflowException($"The value '{TypedValue}' is out of range of {nameof(LimitedIntegerToken)}");
                    }

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
