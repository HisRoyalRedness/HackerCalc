using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class TimeToken : LiteralToken<TimeSpan, TimeToken>
    {
        public TimeToken(string value, TimeSpan typedValue)
            : base(TokenDataType.Time, value, typedValue)
        {
            if (typedValue >= TimeSpan.FromDays(1) || typedValue.Ticks < 0)
                throw new OverflowException("Time must be within the range of a single day");
        }

        public TimeToken(TimeSpan typedValue)
            : this(typedValue.ToString(), typedValue)
        { }

        public TimeToken()
            : this(TimeSpan.Zero)
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

        #region Equality
        public override bool Equals(TimeToken other) => other == null ? false : (TypedValue == other.TypedValue);
        public static bool operator ==(TimeToken a, TimeToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(TimeToken a, TimeToken b) => !(a == b);
        #endregion Equality

        public override string ToString() => $"{Value}";
    }
}
