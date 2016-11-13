using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

//todo: cache bitmaps

namespace AnyStatus
{
    public class StateToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = value as State;

            if (state == null) return null;

            return new BitmapImage(new Uri("pack://application:,,,/AnyStatus;component/Resources/Icons/" + state.Metadata.Icon));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
