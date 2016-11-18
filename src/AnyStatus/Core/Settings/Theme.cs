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
                        new StateMetadata(0, 0, "None", "Silver", string.Empty),
                        new StateMetadata(1, 1, "Unknown", "Silver", "HelpIcon"),
                        new StateMetadata(2, 2, "Disabled", "Silver", "PauseIcon"),
                        new StateMetadata(3, 3, "Canceled", "Gray", "StopIcon"),
                        new StateMetadata(4, 4, "Ok", "Green", "OkIcon"),
                        new StateMetadata(5, 5, "Open", "Green", "InfoIcon"),
                        new StateMetadata(6, 6, "Closed", "Red", "OkIcon"),
                        new StateMetadata(7, 7, "Partially Succeeded", "Orange", "PartiallySucceededIcon"),
                        new StateMetadata(8, 8, "Failed", "Red", "FailedIcon"),
                        new StateMetadata(9, 9, "Invalid", "DarkRed", "WarningIcon"),
                        new StateMetadata(10, 10, "Error", "DarkRed", "WarningIcon"),
                        new StateMetadata(11, 11, "Running", "DodgerBlue", "RunIcon")
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
