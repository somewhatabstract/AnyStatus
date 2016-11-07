using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AnyStatus
{
    public class StateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ItemState)value;
            
            switch (state)
            {
                //Brushes List: https://i-msdn.sec.s-msft.com/dynimg/IC24340.jpeg

                default:
                case ItemState.None:
                case ItemState.Unknown:
                case ItemState.Disabled:
                    return Brushes.Silver;

                case ItemState.Canceled:
                    return Brushes.Gray;

                case ItemState.Ok:
                    return Brushes.Green;

                case ItemState.Open:
                    return Brushes.Green;

                case ItemState.Closed:
                    return Brushes.Red;

                case ItemState.PartiallySucceeded:
                    return Brushes.Orange;

                case ItemState.Failed:
                    return Brushes.Red;

                case ItemState.Invalid:
                    return Brushes.OrangeRed;

                case ItemState.Error:
                    return Brushes.DarkRed;

                case ItemState.InProgress:
                    return Brushes.DodgerBlue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
