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
                        new StateMetadata(0, 0, "None", "Silver", "Blank.png"),
                        new StateMetadata(1, 1, "Unknown", "Silver", "StatusHelp_gray_16x.png"),
                        new StateMetadata(2, 2, "Disabled", "Silver", "StatusPause_grey_16x.png"),
                        new StateMetadata(3, 3, "Canceled", "Gray", "StatusStop_grey_16x.png"),
                        new StateMetadata(4, 4, "Ok", "Green", "StatusOK_grey_16x.png"),
                        new StateMetadata(5, 5, "Open", "Green", "StatusInformation_grey_16x.png"),
                        new StateMetadata(6, 6, "Closed", "Red", "StatusOK_grey_16x.png"),
                        new StateMetadata(7, 7, "Partially Succeeded", "Orange", "StatusInvalid_grey_16x"),
                        new StateMetadata(8, 8, "Failed", "Red", "StatusCriticalError_grey_16x.png"),
                        new StateMetadata(9, 9, "Invalid", "DarkRed", "StatusWarning_grey_16x.png"),
                        new StateMetadata(10, 10, "Error", "DarkRed", "StatusWarning_grey_16x.png"),
                        new StateMetadata(11, 11, "Running", "DodgerBlue", "StatusRun_grey_16x.png")
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
