using System;

namespace AnyStatus
{
    public class UserSettings : NotifyPropertyChanged, ISettings
    {
        private string _clientId;
        private bool _debugMode;
        private bool _reportAnonymousUsage;
        private bool _rightToLeft;
        private bool _showStatusColors;
        private bool _showStatusIcons;
        private Theme _theme;
        private Item _rootItem;

        #region Properties

        public string ClientId
        {
            get
            {
                return _clientId;
            }
            set
            {
                _clientId = value;
                OnPropertyChanged();
            }
        }

        public bool DebugMode
        {
            get
            {
                return _debugMode;
            }
            set
            {
                _debugMode = value;
                OnPropertyChanged();
            }
        }

        public bool ReportAnonymousUsage
        {
            get
            {
                return _reportAnonymousUsage;
            }
            set
            {
                _reportAnonymousUsage = value;
                OnPropertyChanged();
            }
        }

        public Item RootItem
        {
            get
            {
                return _rootItem;
            }
            set
            {
                _rootItem = value;
                OnPropertyChanged();
            }
        }

        public bool ShowStatusColors
        {
            get
            {
                return _showStatusColors;
            }
            set
            {
                _showStatusColors = value;
                OnPropertyChanged();
            }
        }

        public bool ShowStatusIcons
        {
            get
            {
                return _showStatusIcons;
            }
            set
            {
                _showStatusIcons = value;
                OnPropertyChanged();
            }
        }

        public Theme CustomTheme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        public bool RightToLeft {
            get
            {
                return _rightToLeft;
            }
            set
            {
                _rightToLeft = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public static UserSettings Create()
        {
            return new UserSettings
            {
                DebugMode = true,
                RightToLeft = false,
                ShowStatusIcons = true,
                ShowStatusColors = true,
                ReportAnonymousUsage = true,
                RootItem = new RootItem(),
                CustomTheme = Theme.Default.Clone(),
                ClientId = Guid.NewGuid().ToString(),
            };
        }
    }
}
