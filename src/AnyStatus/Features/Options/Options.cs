using Microsoft.VisualStudio.Shell;
using System.Windows;

namespace AnyStatus.Views
{
    public class Options : UIElementDialogPage
    {
        private OptionsDialogControl _optionsDialogControl;

        public Options()
        {
            _optionsDialogControl = new OptionsDialogControl();
        }

        protected override UIElement Child
        {
            get
            {
                return _optionsDialogControl;
            }
        }
    }
}
