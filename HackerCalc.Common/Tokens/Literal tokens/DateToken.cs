using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

/*
    DateToken

        Date literals that are parsed from input.
        The time portion is optional.

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public class DateToken : LiteralToken<DateTime, DateToken>
    {
        #region Constructors
        public DateToken(DateTime typedValue, string rawToken)
            : base(LiteralTokenType.Date, typedValue, rawToken)
        { }

        public DateToken()
            : this(DateTime.Now, "now")
        { }
        #endregion Constructors

        #region Parsing
        public static DateToken Parse(string value, bool dmy = false)
        {
            var portions = value.Split('-', '/');
            if (portions.Length != 3)
                throw new ParseException($"Invalid date format '{value}'");
            if (dmy)
                portions = portions.Reverse().ToArray();

            if (portions[0].Length == 2)
                portions[0] = "20" + portions[0];

            var dateTimeStr = string.Join("-", portions);
            if (DateTime.TryParse(dateTimeStr, out DateTime dateTime))
                return new DateToken(DateTime.SpecifyKind(dateTime, DateTimeKind.Local), value);
            else
                throw new ParseException($"Invalid date format '{dateTimeStr}'");
        }
        #endregion Parsing

        #region Equality
        public override bool Equals(object obj) => Equals(obj as DateToken);
        public override bool Equals(DateToken other) => other is null ? false : (TypedValue == other.TypedValue);
        public override int GetHashCode() => TypedValue.GetHashCode();

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

        #region Comparison
        public override int CompareTo(DateToken other) => other is null ? 1 : TypedValue.CompareTo(other.TypedValue);
        #endregion Comparison

        public static DateToken CreateDateTime(DateToken dateToken, TimeToken timeToken)
            => new DateToken(dateToken.TypedValue + timeToken.TypedValue, $"{dateToken.RawToken} {timeToken.RawToken}");
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
