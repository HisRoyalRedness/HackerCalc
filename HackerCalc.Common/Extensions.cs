using System;
using System.Collections.Generic;
using System.ComponentModel;
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
}
