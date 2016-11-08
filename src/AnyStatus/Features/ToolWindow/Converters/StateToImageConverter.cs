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
            if (!(value is ItemState))
                return null;

            var state = (ItemState)value;

            var image = GetImageName(state);

            return new BitmapImage(new Uri("pack://application:,,,/AnyStatus;component/Resources/Icons/" + image));
        }

        private static string GetImageName(ItemState state)
        {
            switch (state)
            {
                default:
                case ItemState.None:
                    return "Blank.png";
                case ItemState.Unknown:
                    return "StatusHelp_gray_16x.png";
                case ItemState.Disabled:
                    return "StatusPause_grey_16x.png";
                case ItemState.Canceled:
                    return "StatusStop_grey_16x.png";
                case ItemState.Ok:
                    return "StatusOK_grey_16x.png";
                case ItemState.Open:
                    return "StatusInformation_grey_16x.png";
                case ItemState.Closed:
                    return "StatusOK_grey_16x.png";
                case ItemState.PartiallySucceeded:
                    return "StatusInvalid_grey_16x";
                case ItemState.Failed:
                    return "StatusCriticalError_grey_16x.png";
                case ItemState.Invalid:
                    return "StatusWarning_grey_16x.png";
                case ItemState.Error:
                    return "StatusWarning_grey_16x.png";
                case ItemState.Running:
                    return "StatusRun_grey_16x.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
