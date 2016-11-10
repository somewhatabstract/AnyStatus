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
            var state = (State)value;
            
            switch (state)
            {
                //Monikers List: http://glyphlist.azurewebsites.net/knownmonikers/

                default:
                case State.None:
                    return KnownMonikers.Blank;

                case State.Unknown:
                    return KnownMonikers.StatusHelp;

                case State.Disabled:
                    return KnownMonikers.StatusPaused;

                case State.Canceled:
                    return KnownMonikers.StatusStoppedOutline;

                case State.Ok:
                    return KnownMonikers.StatusOKNoColor;

                case State.Open:
                    return KnownMonikers.StatusInformationOutline;

                case State.Closed:
                    return KnownMonikers.StatusInvalidOutline;

                case State.PartiallySucceeded:
                    return KnownMonikers.StatusWarningNoColor;

                case State.Failed:
                    return KnownMonikers.StatusErrorNoColor;

                case State.Invalid:
                    return KnownMonikers.StatusInformationNoColor;

                case State.Error:
                    return KnownMonikers.StatusWarningNoColor;

                case State.Running:
                    return KnownMonikers.StatusRunningNoColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
