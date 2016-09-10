using AnyStatus.ViewModels;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AnyStatus.Views
{
    /// <summary>
    /// Interaction logic for OptionsDialogControl.xaml
    /// </summary>
    public partial class OptionsDialogControl : UserControl
    {
        public OptionsDialogControl(OptionsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        /// <summary>
        /// Used for opening help link
        /// </summary>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }
    }
}
