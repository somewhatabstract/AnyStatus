using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private bool _debugMode;
        private bool _reportAnonymousUsage;
        private bool _showStatusIcons;
        private bool _showStatusColors;

        private ILogger _logger;
        private IUserSettings _userSettings;

        public OptionsViewModel(IUserSettings userSettings, ILogger logger)
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

        public bool ShowStatusIcons
        {
            get { return _showStatusIcons; }
            set
            {
                _showStatusIcons = value;
                OnPropertyChanged();
            }
        }

        public bool ShowStatusColors
        {
            get { return _showStatusColors; }
            set
            {
                _showStatusColors = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Helpers

        private void Save()
        {
            _logger.IsEnabled = DebugMode;

            _userSettings.DebugMode = DebugMode;
            _userSettings.ReportAnonymousUsage = ReportAnonymousUsage;
            _userSettings.ShowStatusIcons = ShowStatusIcons;
            _userSettings.ShowStatusColors = ShowStatusColors;

            _userSettings.Save();
        }

        private void Load()
        {
            DebugMode = _userSettings.DebugMode;
            ReportAnonymousUsage = _userSettings.ReportAnonymousUsage;
            ShowStatusIcons = _userSettings.ShowStatusIcons;
            ShowStatusColors = _userSettings.ShowStatusColors;
        }

        private void RestoreDefaultSettings()
        {
            var result = MessageBox.Show("Are you sure?", "Restore Default Settings", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result == MessageBoxResult.Yes)
            {
                _userSettings.RestoreDefaultSettings();

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

            try
            {
                _userSettings.Import(fileDialog.FileName);

                MessageBox.Show("Settings imported successfully.", "Import Settings", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
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

            try
            {
                _userSettings.Export(fileDialog.FileName);

                MessageBox.Show("Settings exported successfully.", "Export Settings", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
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
