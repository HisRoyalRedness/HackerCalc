using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Linq;
using System.Text;

/*
    General helper functions

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    #region EnumExtensions
    // Mark enum types that should be ignored when enumerating
    public class DontEnumerateAttribute : Attribute
    { }

    // Signify that an operator is unary (i.e. requires a single operand)
    public class UnaryOperatorAttribute : Attribute
    { }

    // Signify that an operator is binary (i.e. requires two operands)
    public class BinaryOperatorAttribute : Attribute
    { }

    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
            => value.GetType().GetField(value.ToString())
                .GetCustomAttributes<DescriptionAttribute>(false)
                .Select(a => a.Description)
                .FirstOrDefault() ?? value.ToString();

        public static bool IsEnumBlocked(this Enum value)
            => value.GetType().GetField(value.ToString())
                .GetCustomAttributes<DontEnumerateAttribute>(false)
                .Any();

        public static bool IsUnaryOperator(this Enum value)
            => value.GetType().GetField(value.ToString())
                .GetCustomAttributes<UnaryOperatorAttribute>(false)
                .Any();


        public static bool IsBinaryOperator(this Enum value)
            => value.GetType().GetField(value.ToString())
                .GetCustomAttributes<BinaryOperatorAttribute>(false)
                .Any();

        public static IEnumerable<TEnum> GetEnumCollection<TEnum>(Predicate<TEnum> enumFilter = null)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
            return Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(e => !((e as Enum)?.IsEnumBlocked() ?? false) && (enumFilter == null || enumFilter(e)));
        }
    }
    #endregion EnumExtensions

    #region GeneralExtensions
    public static class GeneralExtensions
    {
        public static T Tap<T>(this T item, Action<T> action)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            action?.Invoke(item);
            return item;
        }

        public static string Nth(this int value)
        {
            switch(value)
            {
                case 1: return "st";
                case 2: return "nd";
                case 3: return "rd";
                default: return "th";
            }
        }
    }
    #endregion GeneralExtensions

    #region EnumerableExtensions
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextbatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch.ToArray();
                    nextbatch = new List<T>(batchSize);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch.ToArray();
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            foreach (var item in items)
                action?.Invoke(item);
        }

        // https://stackoverflow.com/a/3098381
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item })
                );
        }
    }
    #endregion EnumerableExtensions

    #region StringExtensions
    public static class StringExtensions
    {
        public static string BatchWithDelim(this string data, int batchSize, string delim = " ")
            => BatchWithDelim(data, batchSize, delim, false, ' ');
        public static string BatchWithDelim(this string data, int batchSize, string delim, char padChar)
            => BatchWithDelim(data, batchSize, delim, true, padChar);

        static string BatchWithDelim(this string data, int batchSize, string delim, bool pad, char padChar)
            => new string(
            string.Join(delim,
                (data ?? "").Trim()
                .Reverse()
                .Batch(batchSize)
                .Select(b => 
                    b.Length < batchSize && pad
                        ? new string(padChar, (batchSize - b.Length)) + new string(b)
                        : new string(b)))
            .Reverse()
            .ToArray());

        public static string JoinWith<T>(this IEnumerable<T> items, string joinWord = "and", string delim = ", ")
            => JoinWith(items.Select(item => item?.ToString()), joinWord, delim);

        public static string JoinWith(this IEnumerable<string> items, string joinWord = "and", string delim = ", ")
        {
            var itemList = items.Where(item => item != null).ToList();
            switch(itemList.Count)
            {
                case 0: return "";
                case 1: return itemList[0];
                default:
                    return $"{(string.Join(delim, itemList.Take(itemList.Count - 1).ToArray()))} {joinWord} {itemList[itemList.Count - 1]}";
            }
        }
    }
    #endregion StringExtensions

    #region BigIntegerExtensions
    public static class BigIntegerExtensions
    {
        public static BigInteger BigIntegerFromBinary(this string value)
        {
            BigInteger res = 0;

            foreach (char c in value)
            {
                res <<= 1;
                switch(c)
                {
                    case '1': res += 1; break;
                    case '0': break;
                    default: throw new ArgumentOutOfRangeException("Only expect a 1 or 0 for binary numbers");
                }
            }
            return res;
        }

        public static BigInteger BigIntegerFromOctal(this string value)
        {
            BigInteger res = 0;

            foreach (char c in value)
            {
                res *= 8;
                if (int.TryParse(c.ToString(), out int digit))
                {
                    if (digit >= 0 && digit <= 7)
                        res += digit;
                    else
                        throw new ArgumentOutOfRangeException("Only expect a digits between 0 and 7 for octal numbers");
                }
                else
                    throw new ArgumentOutOfRangeException($"Failed to parse {value} as an octal number");
            }
            return res;
        }

        public static string ToHexadecimalString(this BigInteger bigint, bool showSign = false)
            => BigInteger.Abs(bigint).ToString("X").TrimStart('0').AddSign(showSign && bigint < 0);

        public static string ToDecimalString(this BigInteger bigint, bool showSign = false)
            => BigInteger.Abs(bigint).ToString().AddSign(showSign && bigint < 0);

        public static string ToOctalString(this BigInteger bigint, bool showSign = false)
        {
            var absInt = BigInteger.Abs(bigint);
            var digits = new List<int>(10);
            while(absInt > 0)
            {
                digits.Add((int)(absInt & 0x7));
                absInt >>= 3;
            }
            return new string(digits.Reverse<int>().Select(i => (char)(i + 0x30)).ToArray()).AddSign(showSign && bigint < 0);
        }

        public static bool[] ToBinaryArray(this BigInteger bigint)
        {
            var absInt = BigInteger.Abs(bigint);
            var digits = new List<bool>(64);
            while (absInt > 0)
            {
                digits.Add((absInt & 1) == BigInteger.One);
                absInt >>= 1;
            }
            return digits.Reverse<bool>().ToArray();
        }

        public static string ToBinaryString(this BigInteger bigint, bool showSign = false)
            => new string(ToBinaryArray(bigint).Select(b => b ? '1' : '0').ToArray()).AddSign(showSign && bigint < 0);

        static string AddSign(this string numString, bool addSign)
            => addSign ? $"-{numString}" : numString;
    }
    #endregion BigIntegerExtensions
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
