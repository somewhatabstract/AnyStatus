using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace AnyStatus.VSPackage
{
    public class ConfigurationChangedInfoBar : InfoBarModel, IInfoBar
    {
        public ConfigurationChangedInfoBar() : base(textSpans: new[]
                {
                    new InfoBarTextSpan("The configuration file has been changed.")
                },
                actionItems: new[]
                {
                    new InfoBarButton("Reload"),
                    new InfoBarButton("Dismiss")
                },
                image: KnownMonikers.StatusInformation,
                isCloseButtonVisible: true)
        {
        }
    }
}
