using System;
using System.Globalization;
using System.Windows.Data;

namespace Harmony.UI.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class TimeSpanToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is TimeSpan)) return "00:00:00";

            return TimeSpanToTime((TimeSpan)value);
        }

        public string TimeSpanToTime(TimeSpan timeSpan)
        {
            return timeSpan.Hours > 0 ?
            string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) :
            string.Format("{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value;
            //throw new NotSupportedException();
        }
    }
}