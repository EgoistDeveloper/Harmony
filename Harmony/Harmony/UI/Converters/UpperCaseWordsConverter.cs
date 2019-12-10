using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

namespace Harmony.UI.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class UpperCaseWordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}