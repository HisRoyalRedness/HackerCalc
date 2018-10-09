//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;

//namespace HisRoyalRedness.com
//{
//    public class OldDateToken : OldLiteralToken<DateTime, OldDateToken>
//    {
//        #region Constructors
//        public OldDateToken(DateTime typedValue)
//            : base(TokenDataType.Date, typedValue)
//        { }

//        public OldDateToken()
//            : this(DateTime.Now)
//        { }
//        #endregion Constructors

//        #region Parsing
//        public static OldDateToken Parse(string value, bool dmy = false)
//        {
//            var portions = value.Split('-');
//            if (portions.Length != 3)
//                throw new ParseException($"Invalid date format '{value}'");
//            if (dmy)
//                portions = portions.Reverse().ToArray();

//            if (portions[0].Length == 2)
//                portions[0] = "20" + portions[0];

//            var dateTimeStr = string.Join("-", portions);
//            if (DateTime.TryParse(dateTimeStr, out DateTime dateTime))
//                return new OldDateToken(DateTime.SpecifyKind(dateTime, DateTimeKind.Local));
//            else
//                throw new ParseException($"Invalid date format '{dateTimeStr}'");
//        }
//        #endregion Parsing

//        #region Operator overrides
//        // Used while parsing
//        public static OldDateToken operator +(OldDateToken a, OldTimeToken b)
//            => new OldDateToken(a.TypedValue + b.TypedValue);
//        public static OldDateToken operator +(OldTimeToken a, OldDateToken b) => b + a;


//        public static OldDateToken operator +(OldDateToken a, OldTimespanToken b)
//            => new OldDateToken(a.TypedValue + b.TypedValue);
//        public static OldDateToken operator +(OldTimespanToken a, OldDateToken b) => b + a;
//        public static OldDateToken operator -(OldDateToken a, OldTimespanToken b)
//            => new OldDateToken(a.TypedValue - b.TypedValue);
//        public static OldTimespanToken operator -(OldDateToken a, OldDateToken b)
//            => new OldTimespanToken(a.TypedValue - b.TypedValue);
//        #endregion Operator overrides

//        #region Casting
//        protected override TToken InternalCastTo<TToken>()
//        {
//            if (typeof(TToken).Name == GetType().Name)
//                return this as TToken;

//            switch (typeof(TToken).Name)
//            {
//                case nameof(OldTimeToken):
//                    return new OldTimeToken(TypedValue.TimeOfDay) as TToken;
//                default:
//                    return null;
//            }
//        }
//        #endregion Casting

//        #region Equality
//        public override bool Equals(object obj) => Equals(obj as OldDateToken);
//        public override bool Equals(OldDateToken other) => other is null ? false : (TypedValue == other.TypedValue);
//        public override int GetHashCode() => TypedValue.GetHashCode();

//        public static bool operator ==(OldDateToken a, OldDateToken b)
//        {
//            if (a is null && b is null)
//                return true;
//            if (a is null || b is null)
//                return false;
//            return a.TypedValue == b.TypedValue;
//        }
//        public static bool operator !=(OldDateToken a, OldDateToken b) => !(a == b);
//        #endregion Equality

//        #region Comparison
//        public override int CompareTo(OldDateToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
//        #endregion Comparison

//        #region Other number bases
//        public override string ToHex() => string.Empty;
//        public override string ToDec() => string.Empty;
//        public override string ToOct() => string.Empty;
//        public override string ToBin() => string.Empty;
//        #endregion Other number bases
//    }
//}
