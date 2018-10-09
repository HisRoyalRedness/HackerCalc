//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace HisRoyalRedness.com
//{
//    public class OldTimeToken : OldLiteralToken<TimeSpan, OldTimeToken>
//    {
//        #region Constructors
//        public OldTimeToken(TimeSpan typedValue)
//            : base(TokenDataType.Time, typedValue)
//        {
//            if (typedValue >= TimeSpan.FromDays(1) || typedValue.Ticks < 0)
//                throw new TimeOverflowException("Time must be within the range of a single day");
//        }

//        public OldTimeToken()
//            : this(TimeSpan.Zero)
//        { }
//        #endregion Constructors

//        #region Parsing
//        public static OldTimeToken Parse(string value)
//        {
//            if (TimeSpan.TryParse(value, out TimeSpan time))
//                return new OldTimeToken(time);
//            else
//                throw new ParseException($"Invalid time format '{value}'");
//        }
//        #endregion Parsing

//        #region Operator overloads
//        public static OldTimeToken operator +(OldTimeToken a, OldTimespanToken b)
//            => new OldTimeToken(a.TypedValue + b.TypedValue);
//        public static OldTimeToken operator +(OldTimespanToken a, OldTimeToken b) => b + a;
//        public static OldTimeToken operator -(OldTimeToken a, OldTimespanToken b)
//            => new OldTimeToken(a.TypedValue - b.TypedValue);
//        #endregion Operator overloads

//        #region Casting
//        protected override TToken InternalCastTo<TToken>()
//        {
//            if (typeof(TToken).Name == GetType().Name)
//                return this as TToken;

//            switch (typeof(TToken).Name)
//            {
//                case nameof(OldDateToken):
//                    return new OldDateToken(DateTime.Now.Date + TypedValue) as TToken;

//                case nameof(OldTimespanToken):
//                    return new OldTimespanToken(TypedValue) as TToken;

//                default:
//                    return null;
//            }
//        }
//        #endregion Casting

//        #region Equality
//        public override bool Equals(object obj) => Equals(obj as OldTimeToken);
//        public override bool Equals(OldTimeToken other) => other is null ? false : (TypedValue == other.TypedValue);
//        public override int GetHashCode() => TypedValue.GetHashCode();

//        public static bool operator ==(OldTimeToken a, OldTimeToken b)
//        {
//            if (a is null && b is null)
//                return true;
//            if (a is null || b is null)
//                return false;
//            return a.TypedValue == b.TypedValue;
//        }
//        public static bool operator !=(OldTimeToken a, OldTimeToken b) => !(a == b);
//        #endregion Equality

//        #region Comparison
//        public override int CompareTo(OldTimeToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
//        #endregion Comparison

//        #region Other number bases
//        public override string ToHex() => string.Empty;
//        public override string ToDec() => string.Empty;
//        public override string ToOct() => string.Empty;
//        public override string ToBin() => string.Empty;
//        #endregion Other number bases
//    }
//}
