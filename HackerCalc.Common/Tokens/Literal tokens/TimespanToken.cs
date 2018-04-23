using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class TimespanToken : LiteralToken<TimeSpan, TimespanToken>
    {
        #region Constructors
        public TimespanToken(string value, TimeSpan typedValue)
            : base(TokenDataType.Timespan, value, typedValue)
        { }

        public TimespanToken(TimeSpan typedValue)
            : this(typedValue.ToString(), typedValue)
        { }

        public TimespanToken()
            : this(TimeSpan.Zero)
        { }
        #endregion Constructors

        #region Parsing
        public static TimespanToken Parse(string value, TimeSpan timespan)
            => new TimespanToken(value, timespan);
        #endregion Parsing

        #region Operator overrides
        // Used for parsing
        public static TimespanToken operator +(TimespanToken a, TimespanToken b)
            => new TimespanToken(string.Join(" ", a.Value, b.Value), a.TypedValue + b.TypedValue);

        public static TimespanToken operator -(TimespanToken a, TimespanToken b)
            => new TimespanToken(a.TypedValue - b.TypedValue);
        public static TimespanToken operator *(TimespanToken a, FloatToken b)
            => new TimespanToken(TimeSpan.FromTicks((long)(a.TypedValue.Ticks * b.TypedValue)));
        public static TimespanToken operator *(FloatToken a, TimespanToken b) => b * a;
        public static TimespanToken operator /(TimespanToken a, FloatToken b)
            => new TimespanToken(TimeSpan.FromTicks((long)((double)a.TypedValue.Ticks / (double)b.TypedValue)));
        public static FloatToken operator /(TimespanToken a, TimespanToken b)
            => new FloatToken((double)a.TypedValue.Ticks / (double)b.TypedValue.Ticks);
        #endregion

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

        #region Equality
        public override bool Equals(object obj) => Equals(obj as TimespanToken);
        public override bool Equals(TimespanToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(TimespanToken a, TimespanToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(TimespanToken a, TimespanToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(TimespanToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        #region ToString
        public override string ToString() => ToLongString();

        public string ToLongString()
            => string.Join(" ", LongStringPortions(TypedValue));

        IEnumerable<string> LongStringPortions(TimeSpan ts)
        {
            if (ts.Days > 0)
                yield return LongStringPortions(ts.Days, "day");
            if (ts.Hours > 0)
                yield return LongStringPortions(ts.Hours, "hour");
            if (ts.Minutes > 0)
                yield return LongStringPortions(ts.Minutes, "minute");
            if (ts.Seconds > 0 || ts.Milliseconds > 0)
                yield return ts.Milliseconds > 0
                    ? $"{ts.Seconds}.{ts.Milliseconds} seconds"
                    : LongStringPortions(ts.Seconds, "second");
        }

        string LongStringPortions(int value, string singular)
            => value == 1 ? $"1 {singular}" : $"{value} {singular}s";
        #endregion ToString
    }
}
