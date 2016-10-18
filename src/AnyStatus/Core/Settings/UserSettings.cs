using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnyStatus
{
    public class UserSettings : IUserSettings
    {
        private readonly ILogger _logger;

        private string _clientId;
        private Item _rootItem;
        private bool _debugMode;
        private bool _reportAnonymousUsage;

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
                    Properties.Settings.Default.RootItem = RootItem;
                    Properties.Settings.Default.ClientId = ClientId;
                    Properties.Settings.Default.DebugMode = DebugMode;
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
                _logger.Error(ex, "Failed to save settings");
            }
        }

        public void RestoreDefaultSettings()
        {
            try
            {
                Properties.Settings.Default.Reset();

                ClientId = CreateClientId();
                RootItem = new RootItem();

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
                ReportAnonymousUsage = userSettings.ReportAnonymousUsage;

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
            ClientId = Properties.Settings.Default.ClientId;
            RootItem = Properties.Settings.Default.RootItem;
            DebugMode = Properties.Settings.Default.DebugMode;
            ReportAnonymousUsage = Properties.Settings.Default.ReportAnonymousUsageData;
        }

        private void Upgrade()
        {
            if (string.IsNullOrEmpty(ClientId))
            {
                ClientId = CreateClientId();

                Save();
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