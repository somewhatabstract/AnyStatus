using System.Windows.Controls;

namespace AnyStatus.Features.Options
{
    /// <summary>
    /// Interaction logic for ImportExportSettingsControl.xaml
    /// </summary>
    public partial class ImportExportControl : UserControl
    {
        public ImportExportControl(ImportExportViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
