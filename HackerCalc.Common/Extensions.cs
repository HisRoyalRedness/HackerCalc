using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Linq;
using System.Text;

namespace HisRoyalRedness.com
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
            => value.GetType().GetField(value.ToString())
                .GetCustomAttributes<DescriptionAttribute>(false)
                .Select(a => a.Description)
                .FirstOrDefault() ?? value.ToString();

        public static bool IsEnumIgnored(this Enum value)
            => value.GetType().GetField(value.ToString())
                .GetCustomAttributes<IgnoreEnumAttribute>(false)
                .Any();

        public static IEnumerable<TEnum> GetEnumCollection<TEnum>()
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
            return Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(e => !((e as Enum)?.IsEnumIgnored() ?? false));
        }
    }

    public static class GeneralExtensions
    {
        public static T Tap<T>(this T item, Action<T> action)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (action != null)
                action(item);
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
    }


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
    }

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
}
