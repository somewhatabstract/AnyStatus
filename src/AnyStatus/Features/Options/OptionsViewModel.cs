using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class OptionsViewModel
    {
        private IUserSettings _userSettings;

        public OptionsViewModel() : this(new UserSettings(new NullLogger())) { }

        public OptionsViewModel(IUserSettings userSettings)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            _userSettings = userSettings;

            RestoreDefaultSettingsCommand = new RelayCommand(p => RestoreDefaultSettings());
        }

        private void RestoreDefaultSettings()
        {
            var result = MessageBox.Show("Are you sure?", "Restore Default Settings", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes) return;

            _userSettings.Reset();
            _userSettings.Save();
        }

        public ICommand RestoreDefaultSettingsCommand { get; set; }
    }
}
