using AnyStatus.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class OptionsViewModel
    {
        private IUserSettings _userSettings;

        public OptionsViewModel() : this(new UserSettings()) { }

        public OptionsViewModel(IUserSettings userSettings)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            _userSettings = userSettings;

            ResetAllSettingsCommand = new RelayCommand(p => ResetAllSettings());
        }

        private void ResetAllSettings()
        {
            var result = MessageBox.Show("Are you sure?", "Reset All Settings", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes) return;

            _userSettings.Reset();
        }

        public ICommand ResetAllSettingsCommand { get; set; }
    }
}
