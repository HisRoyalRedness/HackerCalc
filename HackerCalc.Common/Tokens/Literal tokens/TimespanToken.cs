using System;
using System.Collections.Generic;
using System.Text;

/*
    TimespanToken

        Timespan literals that are parsed from input.
        TimespanToken represents a period of time, in units
        ranging from days to seconds.

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class TimespanToken : LiteralToken<TimeSpan, TimespanToken>
    {
        #region Constructors
        private TimespanToken(TimeSpan typedValue, string rawToken, SourcePosition position)
            : base(LiteralTokenType.Timespan, typedValue, rawToken, position)
        { }
        #endregion Constructors

        #region Parsing
        public static TimespanToken Parse(TimeSpan timespan, string rawToken, SourcePosition position, IConfiguration configuration)
            => new TimespanToken(timespan, rawToken, position);

        public TimespanToken AddCompoundPortions(TimespanToken other)
            => new TimespanToken(TypedValue + other.TypedValue, $"{RawToken}{(string.IsNullOrEmpty(RawToken) ? "" : ", ")}{other.RawToken}", Position);
        #endregion Parsing

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

        public static TimespanToken Default => Zero;
        public static TimespanToken Zero
            => new TimespanToken(TimeSpan.Zero, null, SourcePosition.None);
        public static TimespanToken One
            => new TimespanToken(TimeSpan.FromSeconds(1), null, SourcePosition.None);
    }
}

/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
*/
