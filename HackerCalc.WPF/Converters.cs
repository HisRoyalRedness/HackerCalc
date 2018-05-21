using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HisRoyalRedness.com
{
    public class LiteralTokenToValueConverter : IValueConverter
    {
        const string TIME_FORMAT = "hh\\:mm\\:ss";
        const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        const string FLOAT_FORMAT = "0.000000";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value?.GetType()?.Name)
            {
                case nameof(UnlimitedIntegerToken):
                    return ((UnlimitedIntegerToken)value).TypedValue.ToString();

                case nameof(LimitedIntegerToken):
                    return ((LimitedIntegerToken)value).TypedValue.ToString();

                case nameof(DateToken):
                    return ((DateToken)value).TypedValue.ToString(DATE_FORMAT);

                case nameof(TimeToken):
                    return ((TimeToken)value).TypedValue.ToString(TIME_FORMAT);

                case nameof(TimespanToken):
                    return ((TimespanToken)value).ToString();

                case nameof(FloatToken):
                    return ((FloatToken)value).TypedValue.ToString(FLOAT_FORMAT);

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotSupportedException();
    }

    public class LiteralTokenToXamlConverter : IValueConverter
    {
        const int DEFAULT_FONT_SIZE = 40;
        const int SUB_FONT_SIZE = DEFAULT_FONT_SIZE / 2;
        const string SUB_FOREGROUND = "#aaaaaa";
        const string TIME_FORMAT = "hh\\:mm\\:ss";
        const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        const string FLOAT_FORMAT = "0.000000";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value?.GetType()?.Name)
            {
                case nameof(UnlimitedIntegerToken):
                    return GetValue(((UnlimitedIntegerToken)value).TypedValue);

                case nameof(LimitedIntegerToken):
                    var token = (LimitedIntegerToken)value;
                    var bitWidth = token.SignAndBitWidth.BitWidth.GetEnumDescription();
                    var isSigned = token.SignAndBitWidth.IsSigned ? "signed" : "unsigned";
                    return GetValue(token, $"{bitWidth}-bit {isSigned}");

                case nameof(DateToken):
                    return GetValue(((DateToken)value).TypedValue.ToString(DATE_FORMAT), "date");

                case nameof(TimeToken):
                    return GetValue(((TimeToken)value).TypedValue.ToString(TIME_FORMAT), "time");

                case nameof(TimespanToken):
                    return GetValue(((TimespanToken)value).ToString(), "timepan");

                case nameof(FloatToken):
                    return GetValue(((FloatToken)value).TypedValue.ToString(FLOAT_FORMAT), "float");

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotSupportedException();

        string GetValue<TValue>(TValue value)
            => _sectionTemplate.Replace(_paraToken, $"<Run>{value}</Run>");
        string GetValue<TValue, TSub>(TValue value, TSub subValue)
            => _sectionTemplate.Replace(_paraToken, $"<Run>{value}</Run> <Run FontSize=\"{SUB_FONT_SIZE}\" Foreground=\"{SUB_FOREGROUND}\" BaselineAlignment=\"Subscript\">{subValue}</Run>");

        string ReplaceTokenRaw(string value)
            => _sectionTemplate.Replace(_paraToken, value);

        const string _paraToken = "@@PARA";
        static string _sectionTemplate =
            "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
                "FontFamily=\"Segoe UI\" " +
                $"FontSize=\"{DEFAULT_FONT_SIZE}\"> " +
                "<Paragraph>" +
                    _paraToken +
                "</Paragraph>" +
            "</Section>";
    }

    public class LiteralTokenToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value?.GetType()?.Name)
            {
                case nameof(UnlimitedIntegerToken):
                    return "int";

                case nameof(LimitedIntegerToken):
                    var token = (LimitedIntegerToken)value;
                    var bitWidth = token.SignAndBitWidth.BitWidth.GetEnumDescription();
                    var isSigned = token.SignAndBitWidth.IsSigned ? "signed" : "unsigned";
                    return $"{bitWidth}-bit {isSigned}";

                case nameof(DateToken):
                    return "date";

                case nameof(TimeToken):
                    return "time";

                case nameof(TimespanToken):
                    return "timespan";

                case nameof(FloatToken):
                    return "float";

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotSupportedException();
    }

    public class HiddenIfEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty(value as string) ? Visibility.Hidden : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }

    public class RelativeSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as double?;
            return val.HasValue && double.TryParse((parameter as string ?? "1.0"), out double scale)
                ? (val * scale).ToString()
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotSupportedException();
    }

    public class NullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
