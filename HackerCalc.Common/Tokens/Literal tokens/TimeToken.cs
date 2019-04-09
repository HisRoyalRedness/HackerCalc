using System;
using System.Collections.Generic;
using System.Text;

/*
    TimeToken

        Time literals that are parsed from input.
        TimeToken represents a time of day, and thus is 
        independant of time zone. DateToken with a time
        portion attached should be used if you wish to 
        express a time zone bound time.

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class TimeToken : LiteralToken<TimeSpan, TimeToken>
    {
        #region Constructors
        private TimeToken(TimeSpan typedValue, string rawToken, SourcePosition position, IConfiguration configuration)
            : base(LiteralTokenType.Time, typedValue, rawToken, position)
        {
            var allowMultiDay = configuration?.AllowMultidayTimes ?? false;
            if (!allowMultiDay && (typedValue >= TimeSpan.FromDays(1) || typedValue.Ticks < 0))
                throw new ParseException("Time must be within the range of a single day");
        }
        #endregion Constructors

        #region Parsing
        public static TimeToken Parse(string value, SourcePosition position, IConfiguration configuration)
            => TimeSpan.TryParse(value, out TimeSpan time)
            ? new TimeToken(time, value, position, configuration)
            : throw new ParseException($"Invalid time format '{value}'");
        #endregion Parsing

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

        public static TimeToken Default => Now;

        public static TimeToken Now
            => new TimeToken(DateTime.Now.TimeOfDay, "time", SourcePosition.None, null);
        public static TimeToken One
            => new TimeToken(TimeSpan.FromSeconds(1), null, SourcePosition.None, null);
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
