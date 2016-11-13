using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

//todo: separate SettingsStore from UserSettings

namespace AnyStatus
{
    public class UserSettings : IUserSettings
    {
        private readonly ILogger _logger;

        private string _clientId;
        private Item _rootItem;
        private bool _debugMode;
        private bool _reportAnonymousUsage;
        private bool _showStatusIcons;
        private bool _showStatusColors;
        private Theme _theme;

        private UserSettings()
        {
        }

        public UserSettings(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        #region Properties

        public Item RootItem
        {
            get { return _rootItem; }
            set { _rootItem = value; OnPropertyChanged(); }
        }

        public bool DebugMode
        {
            get { return _debugMode; }
            set { _debugMode = value; OnPropertyChanged(); }
        }

        public bool ReportAnonymousUsage
        {
            get { return _reportAnonymousUsage; }
            set { _reportAnonymousUsage = value; OnPropertyChanged(); }
        }

        public string ClientId
        {
            get { return _clientId; }
            set { _clientId = value; OnPropertyChanged(); }
        }

        public bool ShowStatusIcons
        {
            get { return _showStatusIcons; }
            set { _showStatusIcons = value; OnPropertyChanged(); }
        }

        public bool ShowStatusColors
        {
            get { return _showStatusColors; }
            set { _showStatusColors = value; OnPropertyChanged(); }
        }

        public Theme Theme
        {
            get { return _theme; }
            set { _theme = value; OnPropertyChanged(); }
        }

        #endregion

        #region Methods

        public async Task InitializeAsync()
        {
            await Task.Run(() => Initialize()).ConfigureAwait(false);
        }

        public void Initialize()
        {
            try
            {
                LoadSettings();

                if (FirstTimeInstallation())
                {
                    RestoreDefaultSettings();
                }
                else
                {
                    Upgrade();
                }

                State.SetMetadata(Theme.Metadata);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to initialize settings");
            }
        }

        private bool FirstTimeInstallation()
        {
            return Properties.Settings.Default.FirstTimeInstallation;
        }

        public void Save(bool reload = false)
        {
            _logger.Info("Saving settings.");

            try
            {
                Retry.Do(() =>
                {
                    Properties.Settings.Default.Theme = Theme;
                    Properties.Settings.Default.RootItem = RootItem;
                    Properties.Settings.Default.ClientId = ClientId;
                    Properties.Settings.Default.DebugMode = DebugMode;
                    Properties.Settings.Default.ShowStatusIcons = ShowStatusIcons;
                    Properties.Settings.Default.ShowStatusColors = ShowStatusColors;
                    Properties.Settings.Default.ReportAnonymousUsageData = ReportAnonymousUsage;

                    Properties.Settings.Default.Save();

                    if (reload)
                    {
                        Properties.Settings.Default.Reload();
                    }
                },
                TimeSpan.FromSeconds(1), retryCount: 3);
            }
            catch (AggregateException ex)
            {
                _logger.Error(ex, "An error occurred while saving user settings.");
            }
        }

        public void RestoreDefaultSettings()
        {
            try
            {
                Properties.Settings.Default.Reset();

                ClientId = CreateClientId();
                RootItem = new RootItem();
                Theme = Theme.Default.Clone();

                Properties.Settings.Default.FirstTimeInstallation = false;

                Save();

                SettingsReset?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to restore default settings");
            }
        }

        public void Export(string filePath)
        {
            TextWriter writer = null;

            try
            {
                var serializer = new XmlSerializer(GetType());
                writer = new StreamWriter(filePath);
                serializer.Serialize(writer, this);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to export settings");
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public void Import(string filePath)
        {
            TextReader reader = null;

            try
            {
                reader = new StreamReader(filePath);

                var serializer = new XmlSerializer(GetType());

                var userSettings = (UserSettings)serializer.Deserialize(reader);

                userSettings.RootItem?.RestoreParentChildRelationship();

                ClientId = userSettings.ClientId;
                RootItem = userSettings.RootItem;
                DebugMode = userSettings.DebugMode;
                ShowStatusIcons = userSettings.ShowStatusIcons;
                ShowStatusColors = userSettings.ShowStatusColors;
                ReportAnonymousUsage = userSettings.ReportAnonymousUsage;

                //todo: is this needed?
                if (userSettings.Theme != null)
                {
                    Theme = userSettings.Theme;
                }

                Save();

                SettingsReset?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to import settings");
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        #endregion

        #region Helpers

        private void LoadSettings()
        {
            Theme = Properties.Settings.Default.Theme;
            ClientId = Properties.Settings.Default.ClientId;
            RootItem = Properties.Settings.Default.RootItem;
            DebugMode = Properties.Settings.Default.DebugMode;
            ShowStatusIcons = Properties.Settings.Default.ShowStatusIcons;
            ShowStatusColors = Properties.Settings.Default.ShowStatusColors;
            ReportAnonymousUsage = Properties.Settings.Default.ReportAnonymousUsageData;
        }

        private void Upgrade()
        {
            var isDirty = false;

            if (RootItem.ContainsElements(typeof(AppVeyorBuild)))
            {
                UpgradeAppVeyorItems(RootItem);

                isDirty = true;
            }

            if (Theme == null)
            {
                Theme = Theme.Default.Clone();

                isDirty = true;
            }

            if (isDirty) Save();
        }

        private void UpgradeAppVeyorItems(Item item)
        {
            if (item is AppVeyorBuild)
            {
                var appVeyorItem = item as AppVeyorBuild;

                if (string.IsNullOrEmpty(appVeyorItem.ProjectName) == false)
                {
                    _logger.Info("Upgrading AppVeyor item: " + appVeyorItem.Name);

                    appVeyorItem.ProjectSlug = appVeyorItem.ProjectName;

                    appVeyorItem.ProjectName = string.Empty;
                }
            }
            else if (item.ContainsElements())
            {
                foreach (var childItem in item.Items)
                {
                    UpgradeAppVeyorItems(childItem);
                }
            }
        }

        private static string CreateClientId()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsReset;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}