using AnyStatus.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private bool _debugMode;
        private bool _reportAnonymousUsage;

        private ILogger _logger;
        private IUserSettings _userSettings;

        public OptionsViewModel(IUserSettings userSettings, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            _logger = logger;
            _userSettings = userSettings;

            ApplyCommand = new RelayCommand(p => Save());
            ActivateCommand = new RelayCommand(p => Load());
            RestoreDefaultSettingsCommand = new RelayCommand(p => RestoreDefaultSettings());
        }

        #region Commands

        public ICommand ApplyCommand { get; set; }

        public ICommand ActivateCommand { get; set; }

        public ICommand RestoreDefaultSettingsCommand { get; set; }

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
            _logger.IsEnabled = DebugMode;

            _userSettings.DebugMode = DebugMode;
            _userSettings.ReportAnonymousUsage = ReportAnonymousUsage;
            _userSettings.Save();
        }

        private void Load()
        {
            DebugMode = _userSettings.DebugMode;
            ReportAnonymousUsage = _userSettings.ReportAnonymousUsage;
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
