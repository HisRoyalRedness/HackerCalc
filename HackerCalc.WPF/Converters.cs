using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HisRoyalRedness.com
{
    public class LiteralTokenToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as ILiteralToken)?.ObjectValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotSupportedException();
    }

    public class LiteralTokenToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as ILiteralToken)?.DataType;

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
