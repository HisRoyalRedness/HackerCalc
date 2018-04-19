using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HisRoyalRedness.com
{
    public class DateToken : LiteralToken<DateTime, DateToken>
    {
        public DateToken(string value, DateTime typedValue)
            : base(TokenDataType.Date, value, typedValue)
        { }

        public DateToken(DateTime typedValue)
            : this(typedValue.ToString("yyyy-MM-dd HH:mm:ss"), typedValue)
        { }

        public DateToken()
            : this("now", DateTime.Now)
        { }

        #region Parsing
        public static DateToken Parse(string value, bool dmy = false)
        {
            var dateTime = DateTime.Parse(dmy ? string.Join("-", value.Split('-').Reverse()) : value);
            return new DateToken(value, DateTime.SpecifyKind(dateTime, DateTimeKind.Local));
        }

        public static DateToken operator +(DateToken a, TimeToken b)
            => new DateToken(string.Join(" ", a.Value, b.Value), a.TypedValue + b.TypedValue);

        public static DateToken operator +(TimeToken b, DateToken a)
            => new DateToken(string.Join(" ", a.Value, b.Value), a.TypedValue + b.TypedValue);
        #endregion Parsing

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
        public override bool Equals(DateToken other) => other == null ? false : (TypedValue == other.TypedValue);
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

        public override string ToString() => $"{TypedValue}";
    }
}
