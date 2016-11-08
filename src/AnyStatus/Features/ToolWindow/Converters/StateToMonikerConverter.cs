using Microsoft.VisualStudio.Imaging;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AnyStatus
{
    public class StateToMonikerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ItemState)value;
            
            switch (state)
            {
                //Monikers List: http://glyphlist.azurewebsites.net/knownmonikers/

                default:
                case ItemState.None:
                    return KnownMonikers.Blank;

                case ItemState.Unknown:
                    return KnownMonikers.StatusHelp;

                case ItemState.Disabled:
                    return KnownMonikers.StatusPaused;

                case ItemState.Canceled:
                    return KnownMonikers.StatusStoppedOutline;

                case ItemState.Ok:
                    return KnownMonikers.StatusOKNoColor;

                case ItemState.Open:
                    return KnownMonikers.StatusInformationOutline;

                case ItemState.Closed:
                    return KnownMonikers.StatusInvalidOutline;

                case ItemState.PartiallySucceeded:
                    return KnownMonikers.StatusWarningNoColor;

                case ItemState.Failed:
                    return KnownMonikers.StatusErrorNoColor;

                case ItemState.Invalid:
                    return KnownMonikers.StatusInformationNoColor;

                case ItemState.Error:
                    return KnownMonikers.StatusWarningNoColor;

                case ItemState.Running:
                    return KnownMonikers.StatusRunningNoColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
