using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

//todo: cache brushes

namespace AnyStatus
{
    public class StateToBrushConverter : IValueConverter
    {
        private readonly BrushConverter _brushConverter = new BrushConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = value as State;

            if (state == null) return null;

            try
            {
                return _brushConverter.ConvertFromString(state.Metadata.Color);
            }
            catch (NotSupportedException)
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
