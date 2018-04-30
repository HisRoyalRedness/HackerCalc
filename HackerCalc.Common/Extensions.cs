using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace HisRoyalRedness.com
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            if (value == (Enum)OperatorType.Cast)
                Console.WriteLine();
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
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
