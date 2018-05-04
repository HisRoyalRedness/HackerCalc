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
    }
}
