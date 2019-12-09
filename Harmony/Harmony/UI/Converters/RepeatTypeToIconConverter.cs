using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using Harmony.Models.Player.Enums;

namespace Harmony.UI.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class RepeatTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var repeatType = (RepeatType)value;

            var path = "Repeat";

            switch (repeatType)
            {
                case RepeatType.Repeat:
                    path = "Repeat";
                    break;
                case RepeatType.RepeatOff:
                    path = "RepeatOff";
                    break;
                case RepeatType.RepeatOnce:
                    path = "RepeatOnce";
                    break;
            }

            return (Application.Current.FindResource(path) as Path).Data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}