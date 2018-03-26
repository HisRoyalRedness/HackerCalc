using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class TimeToken : LiteralToken<TimeSpan>
    {
        public TimeToken(string value, TimeSpan typedValue)
            : base(TokenDataType.Time, value, typedValue)
        {
            if (typedValue >= TimeSpan.FromDays(1) || typedValue.Ticks < 0)
                throw new OverflowException("Time must be within the range of a single day");
        }

        public TimeToken(TimeSpan typedValue)
            : base(TokenDataType.Time, typedValue)
        {
            if (typedValue >= TimeSpan.FromDays(1) || typedValue.Ticks < 0)
                throw new OverflowException("Time must be within the range of a single day");
        }

        public TimeToken()
            : base(TokenDataType.Time, "", TimeSpan.Zero)
        { }

        #region Parsing
        public static TimeToken Parse(string value)
        {
            var time = TimeSpan.Parse(value);
            return new TimeToken(value, time);
        }
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

                case nameof(TimespanToken):
                    return new TimespanToken(Value, TypedValue) as TToken;

                default:
                    return null;
            }
        }
        #endregion Casting

        public override string ToString() => $"{Value}  -  {TypedValue}";
    }
}
