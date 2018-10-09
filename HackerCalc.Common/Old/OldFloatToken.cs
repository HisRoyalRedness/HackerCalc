//using System;
//using System.Collections.Generic;
//using System.Numerics;
//using System.Text;

//namespace HisRoyalRedness.com
//{
//    public class OldFloatToken : OldLiteralToken<double, OldFloatToken>
//    {
//        #region Constructors
//        public OldFloatToken(double typedValue)
//            : base(TokenDataType.Float, typedValue)
//        { }
//        #endregion Constructors

//        #region Parsing
//        public static OldFloatToken Parse(string value, bool isNeg = false)
//            => new OldFloatToken(double.Parse(isNeg ? $"-{value}" : value));
//        #endregion Parsing

//        #region Operator overloads
//        public static OldFloatToken operator +(OldFloatToken a, OldFloatToken b)
//            => new OldFloatToken(a.TypedValue + b.TypedValue);
//        public static OldFloatToken operator -(OldFloatToken a, OldFloatToken b)
//            => new OldFloatToken(a.TypedValue - b.TypedValue);
//        public static OldFloatToken operator *(OldFloatToken a, OldFloatToken b)
//            => new OldFloatToken(a.TypedValue * b.TypedValue);
//        public static OldFloatToken operator /(OldFloatToken a, OldFloatToken b)
//            => new OldFloatToken(a.TypedValue / b.TypedValue);
//        #endregion Operator overloads

//        public override IOldLiteralToken NumericNegate()
//            => new OldFloatToken(TypedValue * -1.0);

//        #region Casting
//        protected override TToken InternalCastTo<TToken>()
//        {
//            if (typeof(TToken).Name == GetType().Name)
//                return this as TToken;

//            switch (typeof(TToken).Name)
//            {
//                case nameof(OldUnlimitedIntegerToken):
//                    return new OldUnlimitedIntegerToken(new BigInteger(TypedValue)) as TToken;

//                //case nameof(LimitedIntegerToken):
//                //    return new LimitedIntegerToken(new BigInteger(TypedValue)) as TToken;

//                case nameof(OldTimespanToken):
//                    return new OldTimespanToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

//                case nameof(OldTimeToken):
//                    return new OldTimeToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

//                case nameof(OldDateToken):
//                    return new OldDateToken(DateTime.Now.Date + TimeSpan.FromSeconds(TypedValue)) as TToken;

//                default:
//                    return null;
//            }
//        }
//        #endregion Casting

//        #region Equality
//        public override bool Equals(object obj) => Equals(obj as OldFloatToken);
//        public override bool Equals(OldFloatToken other) => other is null ? false : (TypedValue == other.TypedValue);
//        public override int GetHashCode() => TypedValue.GetHashCode();

//        public static bool operator ==(OldFloatToken a, OldFloatToken b)
//        {
//            if (a is null && b is null)
//                return true;
//            if (a is null || b is null)
//                return false;
//            return a.TypedValue == b.TypedValue;
//        }
//        public static bool operator !=(OldFloatToken a, OldFloatToken b) => !(a == b);
//        #endregion Equality

//        #region Comparison
//        public override int CompareTo(OldFloatToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
//        #endregion Comparison

//        #region ToString
//        public override string ToString() => $"{TypedValue:0.000}";
//        #endregion ToString

//        #region Other number bases
//        public override string ToHex() => string.Empty;
//        public override string ToDec() => string.Empty;
//        public override string ToOct() => string.Empty;
//        public override string ToBin() => string.Empty;
//        #endregion Other number bases
//    }
//}
