using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HisRoyalRedness.com
{
    public class DateToken : LiteralToken<DateTime, DateToken>
    {
        #region Constructors
        public DateToken(string value, DateTime typedValue)
            : base(TokenDataType.Date, value, typedValue)
        { }

        public DateToken(DateTime typedValue)
            : this(typedValue.ToString("yyyy-MM-dd HH:mm:ss"), typedValue)
        { }

        public DateToken()
            : this("now", DateTime.Now)
        { }
        #endregion Constructors

        #region Parsing
        public static DateToken Parse(string value, bool dmy = false)
        {
            var dateTime = DateTime.Parse(dmy ? string.Join("-", value.Split('-').Reverse()) : value);
            return new DateToken(value, DateTime.SpecifyKind(dateTime, DateTimeKind.Local));
        }
        #endregion Parsing

        #region Operator overrides
        // Used while parsing
        public static DateToken operator +(DateToken a, TimeToken b)
            => new DateToken(string.Join(" ", a.Value, b.Value), a.TypedValue + b.TypedValue);
        public static DateToken operator +(TimeToken a, DateToken b) => b + a;


        public static DateToken operator +(DateToken a, TimespanToken b)
            => new DateToken(a.TypedValue + b.TypedValue);
        public static DateToken operator +(TimespanToken a, DateToken b) => b + a;
        public static DateToken operator -(DateToken a, TimespanToken b)
            => new DateToken(a.TypedValue - b.TypedValue);
        public static TimespanToken operator -(DateToken a, DateToken b)
            => new TimespanToken(a.TypedValue - b.TypedValue);
        #endregion Operator overrides

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                default:
                    return null;
            }
        }
        #endregion Casting

        #region Equality
        public override bool Equals(object obj) => Equals(obj as DateToken);
        public override bool Equals(DateToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(DateToken a, DateToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(DateToken a, DateToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(DateToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison
    }
}
