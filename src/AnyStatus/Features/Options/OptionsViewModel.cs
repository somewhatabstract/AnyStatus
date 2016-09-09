using AnyStatus.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private bool _debugMode;
        private bool _reportAnonymousUsage;
        private IUserSettings _userSettings;

        public OptionsViewModel(IUserSettings userSettings)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            _userSettings = userSettings;

            SaveCommand = new RelayCommand(p => Save());
            RestoreDefaultSettingsCommand = new RelayCommand(p => RestoreDefaultSettings());
        }

        #region Properties

        public ICommand RestoreDefaultSettingsCommand { get; set; }

        public ICommand SaveCommand { get; set; }

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
            try
            {
                _userSettings.DebugMode = DebugMode;
                _userSettings.ReportAnonymousUsage = ReportAnonymousUsage;

                _userSettings.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void RestoreDefaultSettings()
        {
            var result = MessageBox.Show("Are you sure?", "Restore Default Settings", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result == MessageBoxResult.Yes)
            {
                _userSettings.RestoreDefault();

                DebugMode = _userSettings.DebugMode;
                ReportAnonymousUsage = _userSettings.ReportAnonymousUsage;
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
