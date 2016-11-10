using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AnyStatus
{
    public class StateToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is State))
                return null;

            var state = (State)value;

            var image = GetImageName(state);

            return new BitmapImage(new Uri("pack://application:,,,/AnyStatus;component/Resources/Icons/" + image));
        }

        private static string GetImageName(State state)
        {
            switch (state)
            {
                default:
                case State.None:
                    return "Blank.png";
                case State.Unknown:
                    return "StatusHelp_gray_16x.png";
                case State.Disabled:
                    return "StatusPause_grey_16x.png";
                case State.Canceled:
                    return "StatusStop_grey_16x.png";
                case State.Ok:
                    return "StatusOK_grey_16x.png";
                case State.Open:
                    return "StatusInformation_grey_16x.png";
                case State.Closed:
                    return "StatusOK_grey_16x.png";
                case State.PartiallySucceeded:
                    return "StatusInvalid_grey_16x";
                case State.Failed:
                    return "StatusCriticalError_grey_16x.png";
                case State.Invalid:
                    return "StatusWarning_grey_16x.png";
                case State.Error:
                    return "StatusWarning_grey_16x.png";
                case State.Running:
                    return "StatusRun_grey_16x.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
