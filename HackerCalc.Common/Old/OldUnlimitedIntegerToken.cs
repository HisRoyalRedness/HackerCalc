//using System;
//using System.Globalization;
//using System.Numerics;
//using System.Linq;

//namespace HisRoyalRedness.com
//{
//    public class OldUnlimitedIntegerToken : OldLiteralToken<BigInteger, OldUnlimitedIntegerToken>
//    {
//        #region Constructors
//        public OldUnlimitedIntegerToken(BigInteger typedValue)
//            : base(TokenDataType.UnlimitedInteger, typedValue)
//        { }

//        OldUnlimitedIntegerToken(BigInteger typedValue, bool isNeg)
//            : this(isNeg ? typedValue * -1 : typedValue)
//        { }

//        #endregion Constructors

//        #region Parsing
//        public static OldUnlimitedIntegerToken Parse(string value, IntegerBase numBase, bool isNeg = false)
//        {
//            switch (numBase)
//            {
//                case IntegerBase.Binary:
//                    return new OldUnlimitedIntegerToken(value.Replace("b", "").Replace("B", "").BigIntegerFromBinary(), isNeg);
//                case IntegerBase.Octal:
//                    return new OldUnlimitedIntegerToken(value.Replace("o", "").Replace("O", "").BigIntegerFromOctal(), isNeg);
//                case IntegerBase.Decimal:
//                    return new OldUnlimitedIntegerToken(BigInteger.Parse(value, NumberStyles.Integer), isNeg);
//                case IntegerBase.Hexadecimal:
//                    return new OldUnlimitedIntegerToken(BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), isNeg);
//                default:
//                    throw new ParseException($"Unhandled integer base {numBase}.");
//            }
//        }
//        #endregion Parsing

//        #region Operator overloads
//        public static OldUnlimitedIntegerToken operator +(OldUnlimitedIntegerToken a, OldUnlimitedIntegerToken b)
//            => new OldUnlimitedIntegerToken(a.TypedValue + b.TypedValue);
//        public static OldUnlimitedIntegerToken operator -(OldUnlimitedIntegerToken a, OldUnlimitedIntegerToken b)
//            => new OldUnlimitedIntegerToken(a.TypedValue - b.TypedValue);
//        public static OldUnlimitedIntegerToken operator *(OldUnlimitedIntegerToken a, OldUnlimitedIntegerToken b)
//            => new OldUnlimitedIntegerToken(a.TypedValue * b.TypedValue);
//        public static OldUnlimitedIntegerToken operator /(OldUnlimitedIntegerToken a, OldUnlimitedIntegerToken b)
//            => new OldUnlimitedIntegerToken(a.TypedValue / b.TypedValue);
//        #endregion Operator overloads

//        public override IOldLiteralToken NumericNegate()
//            => new OldUnlimitedIntegerToken(TypedValue * -1);

//        public override IOldLiteralToken BitwiseNegate()
//            => throw new InvalidOperationException($"{nameof(OldUnlimitedIntegerToken)} does not support {nameof(BitwiseNegate)}, as it doesn't have a fixed bit width.");

//        public OldUnlimitedIntegerToken LeftShift(int shift)
//            => new OldUnlimitedIntegerToken(TypedValue << shift);

//        public OldUnlimitedIntegerToken RightShift(int shift)
//            => new OldUnlimitedIntegerToken(TypedValue >> shift);

//        #region Casting
//        protected override TToken InternalCastTo<TToken>()
//        {
//            if (typeof(TToken).Name == GetType().Name)
//                return this as TToken;

//            switch (typeof(TToken).Name)
//            {
//                case nameof(OldFloatToken):
//                    return new OldFloatToken((double)TypedValue) as TToken;

//                case nameof(OldLimitedIntegerToken):
//                    {
//                        if (TypedValue < 0)
//                        {
//                            foreach (var signAndBitwidth in EnumExtensions.GetEnumCollection<OldLimitedIntegerToken.IntegerBitWidth>().Select(bw => new OldLimitedIntegerToken.BitWidthAndSignPair(bw, true)))
//                                if (TypedValue >= OldLimitedIntegerToken.MinValue(signAndBitwidth))
//                                    return new OldLimitedIntegerToken(TypedValue, signAndBitwidth.BitWidth, signAndBitwidth.IsSigned) as TToken;
//                        }
//                        else
//                        {
//                            foreach (var bw in EnumExtensions.GetEnumCollection<OldLimitedIntegerToken.IntegerBitWidth>())
//                                foreach (var signAndBitwidth in new[] { true, false }.Select(s => new OldLimitedIntegerToken.BitWidthAndSignPair(bw, s)))
//                                    if (TypedValue <= OldLimitedIntegerToken.MaxValue(signAndBitwidth))
//                                        return new OldLimitedIntegerToken(TypedValue, signAndBitwidth.BitWidth, signAndBitwidth.IsSigned) as TToken;
//                        }
//                        throw new IntegerOverflowException($"The value '{TypedValue}' is out of range of {nameof(OldLimitedIntegerToken)}");
//                    }

//                case nameof(OldTimespanToken):
//                    return new OldTimespanToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

//                case nameof(OldTimeToken):
//                    return new OldTimeToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

//                case nameof(OldDateToken):
//                    return new OldDateToken(DateTime.Now.Date + TimeSpan.FromSeconds((double)TypedValue)) as TToken;

//                default:
//                    return null;
//            }
//        }
//        #endregion Casting

//        #region Equality
//        public override bool Equals(object obj) => Equals(obj as OldUnlimitedIntegerToken);
//        public override bool Equals(OldUnlimitedIntegerToken other) => other is null ? false : (TypedValue == other.TypedValue);
//        public override int GetHashCode() => TypedValue.GetHashCode();

//        public static bool operator ==(OldUnlimitedIntegerToken a, OldUnlimitedIntegerToken b)
//        {
//            if (a is null && b is null)
//                return true;
//            if (a is null || b is null)
//                return false;
//            return a.TypedValue == b.TypedValue;
//        }
//        public static bool operator !=(OldUnlimitedIntegerToken a, OldUnlimitedIntegerToken b) => !(a == b);
//        #endregion Equality

//        #region Comparison
//        public override int CompareTo(OldUnlimitedIntegerToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
//        #endregion Comparison

//        #region ToString
//        public override string ToString() => TypedValue.ToString();
//        #endregion ToString 

//        #region Other number bases
//        public override string ToHex() => TypedValue.ToHexadecimalString().BatchWithDelim(4);
//        public override string ToDec() => TypedValue.ToString().BatchWithDelim(3, ",");
//        public override string ToOct() => TypedValue.ToOctalString().BatchWithDelim(3);
//        public override string ToBin() => TypedValue.ToBinaryString().BatchWithDelim(4);
//        #endregion Other number bases
//    }
//}
