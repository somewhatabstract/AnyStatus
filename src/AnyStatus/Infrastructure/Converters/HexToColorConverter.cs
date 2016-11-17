using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AnyStatus
{
    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hex = value as string;

            if (string.IsNullOrEmpty(hex))
                return null;

            return (Color)ColorConverter.ConvertFromString(hex);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;

            return color.ToString();
        }
    }
}
