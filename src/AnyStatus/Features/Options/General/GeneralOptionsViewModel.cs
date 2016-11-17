using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus
{
    public class GeneralOptionsViewModel : INotifyPropertyChanged
    {
        private bool _debugMode;
        private bool _reportAnonymousUsage;

        private ILogger _logger;
        private ISettingsStore _userSettings;

        public GeneralOptionsViewModel(ISettingsStore userSettings, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _userSettings = Preconditions.CheckNotNull(userSettings, nameof(userSettings));

            ApplyCommand = new RelayCommand(p => Save());
            ActivateCommand = new RelayCommand(p => Load());
            RestoreDefaultSettingsCommand = new RelayCommand(p => RestoreDefaultSettings());
            ImportSettingsCommand = new RelayCommand(p => ImportSettings());
            ExportSettingsCommand = new RelayCommand(p => ExportSettings());
        }

        #region Commands

        public ICommand ApplyCommand { get; set; }
        public ICommand ActivateCommand { get; set; }
        public ICommand RestoreDefaultSettingsCommand { get; set; }
        public ICommand ImportSettingsCommand { get; set; }
        public ICommand ExportSettingsCommand { get; set; }

        #endregion

        #region Properties

        public bool DebugMode
        {
            get { return _debugMode; }
            set
            {
                _debugMode = value;
                OnPropertyChanged();
            }
        }

        public bool ReportAnonymousUsage
        {
            get { return _reportAnonymousUsage; }
            set
            {
                _reportAnonymousUsage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Helpers

        private void Save()
        {
            _logger.IsEnabled = DebugMode; //move out 

            _userSettings.Settings.DebugMode = DebugMode;
            _userSettings.Settings.ReportAnonymousUsage = ReportAnonymousUsage;

            _userSettings.TrySave();
        }

        private void Load()
        {
            DebugMode = _userSettings.Settings.DebugMode;
            ReportAnonymousUsage = _userSettings.Settings.ReportAnonymousUsage;
        }

        private void RestoreDefaultSettings()
        {
            var result = MessageBox.Show("Are you sure?", "Restore Default Settings", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result == MessageBoxResult.Yes)
            {
                _userSettings.TryRestoreDefaultSettings();

                Load();
            }
        }

        private void ImportSettings()
        {
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = "XML|*.xml";

            var dialogResult = fileDialog.ShowDialog();

            if (dialogResult == false || string.IsNullOrEmpty(fileDialog.FileName))
                return;

            if (_userSettings.TryImport(fileDialog.FileName))
            {
                MessageBox.Show("Settings imported successfully.", "Import Settings", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("An error occurred while importing settings.", "Import Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportSettings()
        {
            var fileDialog = new SaveFileDialog();

            fileDialog.Filter = "XML|*.xml";

            var dialogResult = fileDialog.ShowDialog();

            if (dialogResult == false || string.IsNullOrEmpty(fileDialog.FileName))
                return;

            if (_userSettings.TryExport(fileDialog.FileName))
            {
                MessageBox.Show("Settings exported successfully.", "Export Settings", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("An error occurred while exporting settings.", "Export Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
