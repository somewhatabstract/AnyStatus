using System;
using System.Globalization;
using System.Windows.Data;

namespace AnyStatus
{
    public class CanStartWindowsServiceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ICanStartWindowsService;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
