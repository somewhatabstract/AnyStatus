using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace AnyStatus.VSPackage
{
    public class ConfigurationChangedInfoBar : InfoBarModel, IInfoBar
    {
        public const string ReloadButtonContext = "refresh";
        public const string DismissButtonContext = "dismiss";

        public ConfigurationChangedInfoBar() : base(textSpans: new[]
                {
                    new InfoBarTextSpan("The configuration file has been changed. "),
                    
                },
                actionItems: new[]
                {
                    new InfoBarHyperlink("Reload", ReloadButtonContext),
                    new InfoBarHyperlink("Dismiss", DismissButtonContext),
                },
                image: KnownMonikers.StatusInformation,
                isCloseButtonVisible: true)
        {
        }
    }
}
