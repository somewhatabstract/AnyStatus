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
        private IUserSettings _userSettings;

        public UserInterfaceOptionsViewModel(IUserSettings userSettings, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _userSettings = Preconditions.CheckNotNull(userSettings, nameof(userSettings));

            ApplyCommand = new RelayCommand(p => Save());
            ActivateCommand = new RelayCommand(p => Load());
        }

        #region Commands

        public ICommand ApplyCommand { get; set; }

        public ICommand ActivateCommand { get; set; }

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
            _userSettings.ShowStatusIcons = ShowStatusIcons;
            _userSettings.ShowStatusColors = ShowStatusColors;
            _userSettings.Theme = Theme;
            _userSettings.Save();

            State.SetMetadata(Theme.Metadata);
        }

        private void Load()
        {
            try
            {
                Theme = _userSettings.Theme.Clone();
                ShowStatusIcons = _userSettings.ShowStatusIcons;
                ShowStatusColors = _userSettings.ShowStatusColors;
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
