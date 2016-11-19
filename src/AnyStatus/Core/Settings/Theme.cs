using System;
using System.Linq;

namespace AnyStatus
{
    [Serializable]
    public class Theme
    {
        public static Theme Default = new Theme
        {
            Metadata = new[]
                    {
                        new StateMetadata(State.None, 0, "None", "Transparent", string.Empty),
                        new StateMetadata(State.Unknown, 1, "Unknown", "Silver", "HelpIcon"),
                        new StateMetadata(State.Disabled, 2, "Disabled", "Gray", "PauseIcon"),
                        new StateMetadata(State.Canceled, 3, "Canceled", "Gray", "StopIcon"),
                        new StateMetadata(State.Ok, 4, "Ok", "Green", "OkIcon"),
                        new StateMetadata(State.Open, 5, "Open", "Green", "InfoIcon"),
                        new StateMetadata(State.Closed, 6, "Closed", "Red", "OkIcon"),
                        new StateMetadata(State.PartiallySucceeded, 7, "Partially Succeeded", "Orange", "PartiallySucceededIcon"),
                        new StateMetadata(State.Failed, 8, "Failed", "Red", "FailedIcon"),
                        new StateMetadata(State.Invalid, 9, "Invalid", "DarkRed", "WarningIcon"),
                        new StateMetadata(State.Error, 10, "Error", "DarkRed", "WarningIcon"),
                        new StateMetadata(State.Running, 11, "Running", "DodgerBlue", "RunIcon")
                    }
        };

        public StateMetadata[] Metadata { get; set; }

        public Theme Clone()
        {
            var theme = (Theme)MemberwiseClone();

            theme.Metadata = Metadata.Select(k => k.Clone()).ToArray();

            return theme;
        }
    }
}
