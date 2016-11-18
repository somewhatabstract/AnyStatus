using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AnyStatus
{
    public class UserInterfaceOptionsViewModel : INotifyPropertyChanged
    {
        private Theme _theme;
        private bool _showStatusIcons;
        private bool _showStatusColors;

        private ILogger _logger;
        private ISettingsStore _settingsStore;

        public UserInterfaceOptionsViewModel(ISettingsStore settingsStore, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));

            ApplyCommand = new RelayCommand(p => Save());
            ActivateCommand = new RelayCommand(p => Load());
            ResetColorThemeCommand = new RelayCommand(p => Theme = Theme.Default.Clone());
        }

        #region Commands

        public ICommand ApplyCommand { get; set; }

        public ICommand ActivateCommand { get; set; }

        public ICommand ResetColorThemeCommand { get; set; }

        #endregion

        #region Properties

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

        public Theme Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Helpers

        private void Save()
        {
            _settingsStore.Settings.ShowStatusIcons = ShowStatusIcons;
            _settingsStore.Settings.ShowStatusColors = ShowStatusColors;
            _settingsStore.Settings.CustomTheme = Theme;
            _settingsStore.TrySave();

            if (Theme?.Metadata != null)
                State.SetMetadata(Theme.Metadata);
        }

        private void Load()
        {
            try
            {
                Theme = _settingsStore.Settings.CustomTheme.Clone();
                ShowStatusIcons = _settingsStore.Settings.ShowStatusIcons;
                ShowStatusColors = _settingsStore.Settings.ShowStatusColors;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "An error occurred while loading settings.");
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
