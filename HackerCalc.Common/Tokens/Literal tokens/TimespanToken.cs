using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class TimespanToken : LiteralToken<TimeSpan>
    {
        public TimespanToken(string value, TimeSpan typedValue)
            : base(TokenDataType.Timespan, value, typedValue)
        { }

        public TimespanToken(TimeSpan typedValue)
            : base(TokenDataType.Timespan, typedValue)
        { }

        public TimespanToken()
            : base(TokenDataType.Timespan, "", TimeSpan.Zero)
        { }

        #region Parsing
        public static TimespanToken Parse(string value, TimeSpan timespan)
            => new TimespanToken(value, timespan);

        public static TimespanToken operator +(TimespanToken a, TimespanToken b)
            => new TimespanToken(string.Join(" ", a.Value, b.Value), a.TypedValue + b.TypedValue);
        #endregion Parsing

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(DateToken):
                    return new DateToken(DateTime.Now.Date + TypedValue) as TToken;

                case nameof(TimeToken):
                    return new TimeToken(Value, TypedValue) as TToken;

                default:
                    return null;
            }
        }
        #endregion Casting

        public override string ToString() => $"{Value}  -  {TypedValue}";
    }
}
