using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.Features.Options
{
    public class ImportExportViewModel : INotifyPropertyChanged
    {
        private IUserSettings _userSettings;
        private string _exportSettingsFileName;
        private string _importSettingsFileName;

        public ImportExportViewModel(IUserSettings userSettings)
        {
            Preconditions.CheckNotNull(userSettings, nameof(userSettings));

            _userSettings = userSettings;

            ImportSettingsCommand = new RelayCommand(
                p => ImportSettings(),
                p => { return !string.IsNullOrWhiteSpace(ImportSettingsFileName); });

            ExportSettingsCommand = new RelayCommand(
                p => ExportSettings(),
                p => { return !string.IsNullOrWhiteSpace(ExportSettingsFileName); });

            BrowseImportSettingsFileCommand = new RelayCommand(p => BrowseImportSettingsFile());
            BrowseExportSettingsFileCommand = new RelayCommand(p => BrowseExportSettingsFile());
        }

        public string ImportSettingsFileName
        {
            get { return _importSettingsFileName; }
            set
            {
                _importSettingsFileName = value;
                OnPropertyChanged();
            }
        }

        public string ExportSettingsFileName
        {
            get { return _exportSettingsFileName; }
            set
            {
                _exportSettingsFileName = value;
                OnPropertyChanged();
            }
        }

        public ICommand ImportSettingsCommand { get; set; }

        public ICommand BrowseImportSettingsFileCommand { get; set; }

        public ICommand ExportSettingsCommand { get; set; }

        public ICommand BrowseExportSettingsFileCommand { get; set; }

        private void ImportSettings()
        {
            if (string.IsNullOrEmpty(ImportSettingsFileName))
                return;

            try
            {
                var result = MessageBox.Show("Are you sure?", "Import Settings", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                if (result == MessageBoxResult.Yes)
                    _userSettings.Import(ImportSettingsFileName);

                MessageBox.Show("Settings imported successfully", "Import Settings");
            }
            catch
            {
                MessageBox.Show("An error occurred while importing settings.", "Import Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportSettings()
        {
            if (string.IsNullOrEmpty(ExportSettingsFileName))
                return;

            try
            {
                _userSettings.Export(ExportSettingsFileName);

                MessageBox.Show("Settings exported successfully", "Export Settings");
            }
            catch
            {
                MessageBox.Show("An error occurred while exporting settings.", "Export Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseImportSettingsFile()
        {
            var fileDialog = new OpenFileDialog();

            fileDialog.ShowDialog();

            ImportSettingsFileName = fileDialog.FileName;
        }

        private void BrowseExportSettingsFile()
        {
            var fileDialog = new SaveFileDialog();

            fileDialog.ShowDialog();

            ExportSettingsFileName = fileDialog.FileName;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
