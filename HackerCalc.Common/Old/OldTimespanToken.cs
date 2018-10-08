using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public class OldTimespanToken : OldLiteralToken<TimeSpan, OldTimespanToken>
    {
        #region Constructors
        public OldTimespanToken(TimeSpan typedValue)
            : base(TokenDataType.Timespan, typedValue)
        { }

        public OldTimespanToken()
            : this(TimeSpan.Zero)
        { }
        #endregion Constructors

        #region Parsing
        public static OldTimespanToken Parse(TimeSpan timespan)
            => new OldTimespanToken(timespan);

        public OldTimespanToken AddCompoundPortions(OldTimespanToken other)
            => new OldTimespanToken(TypedValue + other.TypedValue);
        #endregion Parsing

        #region Operator overrides
        public static OldTimespanToken operator +(OldTimespanToken a, OldTimespanToken b)
            => new OldTimespanToken(a.TypedValue + b.TypedValue);
        public static OldTimespanToken operator -(OldTimespanToken a, OldTimespanToken b)
            => new OldTimespanToken(a.TypedValue - b.TypedValue);
        public static OldTimespanToken operator *(OldTimespanToken a, OldFloatToken b)
            => new OldTimespanToken(TimeSpan.FromTicks((long)(a.TypedValue.Ticks * b.TypedValue)));
        public static OldTimespanToken operator *(OldFloatToken a, OldTimespanToken b) => b * a;
        public static OldTimespanToken operator /(OldTimespanToken a, OldFloatToken b)
            => new OldTimespanToken(TimeSpan.FromTicks((long)((double)a.TypedValue.Ticks / (double)b.TypedValue)));
        public static OldFloatToken operator /(OldTimespanToken a, OldTimespanToken b)
            => new OldFloatToken((double)a.TypedValue.Ticks / (double)b.TypedValue.Ticks);
        #endregion

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(OldDateToken):
                    return new OldDateToken(DateTime.Now.Date + TypedValue) as TToken;

                case nameof(OldTimeToken):
                    return new OldTimeToken(TypedValue) as TToken;

                default:
                    return null;
            }
        }
        #endregion Casting

        #region Equality
        public override bool Equals(object obj) => Equals(obj as OldTimespanToken);
        public override bool Equals(OldTimespanToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

        public static bool operator ==(OldTimespanToken a, OldTimespanToken b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TypedValue == b.TypedValue;
        }
        public static bool operator !=(OldTimespanToken a, OldTimespanToken b) => !(a == b);
        #endregion Equality

        #region Comparison
        public override int CompareTo(OldTimespanToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
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

        #region Other number bases
        public override string ToHex() => string.Empty;
        public override string ToDec() => string.Empty;
        public override string ToOct() => string.Empty;
        public override string ToBin() => string.Empty;
        #endregion Other number bases
    }
}
