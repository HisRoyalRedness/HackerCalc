using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class TimeToken : LiteralToken<TimeSpan, TimeToken>
    {
        #region Constructors
        public TimeToken(TimeSpan typedValue)
            : base(TokenDataType.Time, typedValue)
        {
            if (typedValue >= TimeSpan.FromDays(1) || typedValue.Ticks < 0)
                throw new OverflowException("Time must be within the range of a single day");
        }

        public TimeToken()
            : this(TimeSpan.Zero)
        { }
        #endregion Constructors

        #region Parsing
        public static TimeToken Parse(string value)
        {
            if (TimeSpan.TryParse(value, out TimeSpan time))
                return new TimeToken(time);
            else
                throw new ParseException($"Invalid time format '{value}'");
        }
        #endregion Parsing

        #region Operator overloads
        public static TimeToken operator +(TimeToken a, TimespanToken b)
            => new TimeToken(a.TypedValue + b.TypedValue);
        public static TimeToken operator +(TimespanToken a, TimeToken b) => b + a;
        public static TimeToken operator -(TimeToken a, TimespanToken b)
            => new TimeToken(a.TypedValue - b.TypedValue);
        #endregion Operator overloads

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
                    return new TimespanToken(TypedValue) as TToken;

                default:
                    return null;
            }
        }
        #endregion Casting

        #region Equality
        public override bool Equals(object obj) => Equals(obj as TimeToken);
        public override bool Equals(TimeToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

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

        #region Comparison
        public override int CompareTo(TimeToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison
    }
}
