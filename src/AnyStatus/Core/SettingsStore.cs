using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnyStatus
{
    public class SettingsStore : NotifyPropertyChanged, ISettingsStore
    {
        private readonly ILogger _logger;
        private UserSettings _settings;
        private DateTime _lastSaved;
        
        public event EventHandler SettingsChanged;
        public event EventHandler SettingsSourceChanged;

        public SettingsStore(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));

            PropertyChanged += OnPropertyChanged;
        }

        public UserSettings Settings
        {
            get { return _settings; }
            private set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        #region ISettingsStore

        public bool TryInitialize()
        {
            try
            {
                Initialize();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to initialize settings");

                return false;
            }
        }

        public async Task<bool> TryInitializeAsync()
        {
            try
            {
                await Task.Run(() => Initialize()).ConfigureAwait(false);

                return true;
            }
            catch (AggregateException ex)
            {
                _logger.Error(ex.Flatten(), "Failed to asynchronously initialize settings.");

                return false;
            }
        }

        public bool TrySave()
        {
            try
            {
                Retry.Do(Save, TimeSpan.FromSeconds(1), retryCount: 3);

                _logger.Info("Saved.");

                return true;
            }
            catch (AggregateException ex)
            {
                _logger.Error(ex, "An error occurred while saving user settings.");

                return false;
            }
        }

        public bool TryRestoreDefaultSettings()
        {
            try
            {
                RestoreDefaultSettings();

                _logger.Info("Restored default settings.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while restoring default settings.");

                return false;
            }
        }

        public bool TryImport(string filePath)
        {
            try
            {
                Import(filePath);

                _logger.Info("Settings imported successfully.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while importing settings.");

                return false;
            }
        }

        public bool TryExport(string filePath)
        {
            try
            {
                Export(filePath);

                _logger.Info("Settings exported successfully.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while exporting settings.");

                return false;
            }
        }

        public void TryReload()
        {
            try
            {
                Properties.Settings.Default.Reload();

                Settings = Properties.Settings.Default.UserSettings;

                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while reloading user settings.");
            }
        }

        #endregion

        #region Helpers

        private void Initialize()
        {
            Upgrade();

            Load();

            if (Settings == null)
            {
                RestoreDefaultSettings();
            }

            WatchConfigurationFile();
        }

        private void Load()
        {
            Settings = Properties.Settings.Default.UserSettings;
        }

        private void WatchConfigurationFile()
        {
            try
            {
                var filePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

                _logger.Info("Configuration: " + filePath);

                var watcher = new FileSystemWatcher();

                watcher.Path = Path.GetDirectoryName(filePath);

                watcher.Filter = Path.GetFileName(filePath);

                watcher.NotifyFilter = NotifyFilters.LastWrite;

                watcher.IncludeSubdirectories = false;

                watcher.Changed += (s, e) =>
                {
                    if (DateTime.Now.Subtract(_lastSaved) > TimeSpan.FromMilliseconds(500))
                    {
                        SettingsSourceChanged?.Invoke(s, e);
                    }
                };

                watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error watching configuration file for changes");
            }
        }

        private void Save()
        {
            _lastSaved = DateTime.Now; //must be before .Save() to be able to ignore changes by current process

            Properties.Settings.Default.UserSettings = Settings;

            Properties.Settings.Default.Save();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings):
                    OnSettingsChanged();
                    break;
                default:
                    break;
            }
        }

        private void OnSettingsChanged()
        {
            if (Settings == null)
                return;

            if (Settings.RootItem != null)
                Settings.RootItem.RestoreParentChildRelationship();

            //load theme
            if (Settings.CustomTheme?.Metadata != null)
                State.SetMetadata(Settings.CustomTheme.Metadata);
        }

        private void Import(string filePath)
        {
            TextReader reader = null;

            //todo: ask what to import. items, theme...

            try
            {
                reader = new StreamReader(filePath);

                var serializer = new XmlSerializer(typeof(UserSettings));

                var settings = (UserSettings)serializer.Deserialize(reader);

                if (settings.CustomTheme == null)
                {
                    settings.CustomTheme = Settings.CustomTheme ?? Theme.Default.Clone();
                }

                Settings = settings;

                Save();

                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        private void Export(string filePath)
        {
            TextWriter writer = null;

            try
            {
                var serializer = new XmlSerializer(typeof(UserSettings));
                writer = new StreamWriter(filePath);
                serializer.Serialize(writer, Settings);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        private void RestoreDefaultSettings()
        {
            Properties.Settings.Default.Reset();

            Settings = UserSettings.Create();

            Save();

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Upgrade()
        {
            try
            {
                if (Properties.Settings.Default.RootItem == null)
                    return;

                _logger.Info("Upgrading settings...");

                var settings = UserSettings.Create();

                settings.ClientId = Properties.Settings.Default.ClientId;
                settings.RootItem = Properties.Settings.Default.RootItem;
                settings.DebugMode = Properties.Settings.Default.DebugMode;
                settings.ShowStatusIcons = Properties.Settings.Default.ShowStatusIcons;
                settings.ShowStatusColors = Properties.Settings.Default.ShowStatusColors;
                settings.ReportAnonymousUsage = Properties.Settings.Default.ReportAnonymousUsageData;

                Settings = settings;

                Properties.Settings.Default.Reset();

                TrySave();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to upgrade settings.");
            }
        }

        #endregion
    }
}
