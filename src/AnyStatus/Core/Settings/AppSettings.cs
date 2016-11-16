using System;
using System.Linq;

namespace AnyStatus
{
    public class AppSettings : NotifyPropertyChanged, ISettings
    {
        private string _clientId;
        private bool _debugMode;
        private bool _reportAnonymousUsage;
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

        public Theme Theme
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

        #endregion

        public void Initialize()
        {
            var rootItem = RootItem as RootItem;

            if (rootItem == null)
                return;

            rootItem.RestoreParentChildRelationship();

            //if (rootItem.Items == null)
            //    return;

            //rootItem.CalculateState();

            foreach (Folder folder in rootItem.Items.Where(k => k is Folder))
            {
                folder.CalculateState();
            }
        }

        public static AppSettings Default()
        {
            return new AppSettings
            {
                DebugMode = true,
                ShowStatusIcons = true,
                ShowStatusColors = true,
                ReportAnonymousUsage = true,
                RootItem = new RootItem(),
                Theme = Theme.Default.Clone(),
                ClientId = Guid.NewGuid().ToString(),
            };
        }
    }
}
