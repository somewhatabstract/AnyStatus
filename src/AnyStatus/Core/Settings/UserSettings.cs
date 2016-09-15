using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.IO;
using System.Xml.Serialization;

namespace AnyStatus
{
    public class UserSettings : IUserSettings
    {
        private ILogger _logger;

        private UserSettings()
        {
        }

        public UserSettings(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;

            Initialize();
        }

        #region Properties

        public event EventHandler SettingsReset;

        public Item RootItem { get; set; }

        public bool DebugMode { get; set; }

        public bool ReportAnonymousUsage { get; set; }

        public string ClientId { get; set; }

        #endregion

        #region Methods

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
                _logger.Error(ex, "Failed to save settings.");
            }
        }

        public void RestoreDefaultSettings()
        {
            _logger.Info("Restoring default settings.");

            try
            {
                Properties.Settings.Default.Reset();

                ClientId = CreateClientId();
                RootItem = new RootItem();
                DebugMode = false;
                ReportAnonymousUsage = true;

                Save();

                SettingsReset?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to restore default settings.");
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
                _logger.Error(ex, "Failed to export settings.");
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

                RootItem = userSettings.RootItem;
                DebugMode = userSettings.DebugMode;
                ReportAnonymousUsage = userSettings.ReportAnonymousUsage;
                ClientId = userSettings.ClientId;

                Save();

                SettingsReset?.Invoke(this, EventArgs.Empty);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Failed to import settings.");
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

        private void Initialize()
        {
            _logger.Info("Initializing...");

            try
            {
                LoadSettings();

                if (RootItem == null)
                {
                    RestoreDefaultSettings();
                }

                Upgrade();

                SettingsReset += (s, e) => LoadSettings();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to initialize settings.");
            }
        }

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
                _logger.Info("Upgrading settings.");

                ClientId = CreateClientId();

                Save();
            }
        }

        private string CreateClientId()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion
    }
}
