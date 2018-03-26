using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HisRoyalRedness.com
{
    public class DateToken : LiteralToken<DateTime>
    {
        public DateToken(string value, DateTime typedValue)
            : base(TokenDataType.Date, value, typedValue)
        { }

        public DateToken(DateTime typedValue)
            : base(TokenDataType.Date, typedValue)
        { }


        public DateToken()
            : base(TokenDataType.Date, "now", DateTime.Now)
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

        public override string ToString() => $"{Value}  -  {TypedValue}";
    }
}
