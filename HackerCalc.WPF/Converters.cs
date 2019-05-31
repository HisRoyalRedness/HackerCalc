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
    #region OneWayConverter
    public abstract class OneWayConverter<TType> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value == null || !typeof(TType).IsAssignableFrom(value?.GetType())
                ? ConvertTo(parameter, culture)
                : ConvertTo((TType)value, parameter, culture);

        protected abstract string ConvertTo(TType value, object parameter, CultureInfo culture);
        protected virtual string ConvertTo(object parameter, CultureInfo culture) => null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotSupportedException();
    }

    public abstract class OneWayConverter<TTypeIn, TTypeOut> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value == null || !typeof(TTypeIn).IsAssignableFrom(value?.GetType())
                ? ConvertTo(parameter, culture)
                : ConvertTo((TTypeIn)value, parameter, culture);

        protected abstract TTypeOut ConvertTo(TTypeIn value, object parameter, CultureInfo culture);
        protected virtual TTypeOut ConvertTo(object parameter, CultureInfo culture) => default(TTypeOut);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotSupportedException();
    }
    #endregion OneWayConverter

    public class DataTypeToValueConverter : OneWayConverter<IDataType<DataType>, string>
    {
        const string TIME_FORMAT = "hh\\:mm\\:ss";
        const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        const string INT_FORMAT = "#,0";
        const string FLOAT_FORMAT = "#,0.############";

        protected override string ConvertTo(IDataType<DataType> value, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            switch (value.DataType)
            {
                case DataType.LimitedInteger:
                    return ((LimitedIntegerType)value).Value.ToString(INT_FORMAT);

                case DataType.RationalNumber:
                    var rat = ((RationalNumberType)value).Value;
                    return rat.Denominator == 1
                        ? rat.Numerator.ToString(INT_FORMAT)
                        : ((double)rat.Numerator / (double)rat.Denominator).ToString(FLOAT_FORMAT);

                case DataType.IrrationalNumber:
                    return ((IrrationalNumberType)value).Value.ToString(FLOAT_FORMAT);

                case DataType.Date:
                    return ((DateType)value).Value.ToString(DATE_FORMAT);

                case DataType.Time:
                    return ((TimeType)value).Value.ToString(TIME_FORMAT);

                case DataType.Timespan:
                    return ((TimespanType)value).ToString();

                default:
                    if (value is DisplayType)
                        return ((DisplayType)value).DisplayValue;
                    return string.Empty;
            }
        }
    }

    public class TokenToLaTeXConverter : OneWayConverter<IToken>
    {
        protected override string ConvertTo(IToken value, object parameter, CultureInfo culture)
            => value?.ToLaTeX() ?? string.Empty;

        protected override string ConvertTo(object parameter, CultureInfo culture) => string.Empty;
    }

    public class DataTypeToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IDataType<DataType> dataType))
                return null;
            switch (dataType.DataType)
            {
                case DataType.LimitedInteger:
                    var signAndBitwidth = ((LimitedIntegerType)dataType).SignAndBitWidth;
                    var isUnlimited = signAndBitwidth.BitWidth == IntegerBitWidth.Unlimited;
                    var bitWidth = signAndBitwidth.BitWidth.GetEnumDescription();
                    var isSigned = signAndBitwidth.IsSigned ? "signed" : "unsigned";
                    return $"{bitWidth}{(isUnlimited ? "" : "-bit")} {isSigned}";

                case DataType.IrrationalNumber:
                case DataType.Date:
                case DataType.Time:
                case DataType.Timespan:
                    return dataType.DataType.ToString().ToLower();

                default:
                    return string.Empty;
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

    public class RelativeSizeConverter : OneWayConverter<double>
    {
        protected override string ConvertTo(double value, object parameter, CultureInfo culture)
            => double.TryParse((parameter as string ?? "1.0"), out double scale)
                ? (value * scale).ToString()
                : value.ToString();

        protected override string ConvertTo(object parameter, CultureInfo culture) => "1.0";
    }

    public class EmptyStringToCollapsedConverter : OneWayConverter<string, Visibility>
    {
        protected override Visibility ConvertTo(string value, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty(value)
                ? Visibility.Collapsed
                : Visibility.Visible;
    }


    public class NullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
