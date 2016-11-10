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
            var state = (State)value;
            
            switch (state)
            {
                //Brushes List: https://i-msdn.sec.s-msft.com/dynimg/IC24340.jpeg

                default:
                case State.None:
                case State.Unknown:
                case State.Disabled:
                    return Brushes.Silver;

                case State.Canceled:
                    return Brushes.Gray;

                case State.Ok:
                    return Brushes.Green;

                case State.Open:
                    return Brushes.Green;

                case State.Closed:
                    return Brushes.Red;

                case State.PartiallySucceeded:
                    return Brushes.Orange;

                case State.Failed:
                    return Brushes.Red;

                case State.Invalid:
                    return Brushes.OrangeRed;

                case State.Error:
                    return Brushes.DarkRed;

                case State.Running:
                    return Brushes.DodgerBlue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
