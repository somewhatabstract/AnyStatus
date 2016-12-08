using System;
using System.Globalization;
using System.Windows.Data;

namespace AnyStatus
{
    public class CanTriggerBuildConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ICanTriggerBuild;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
