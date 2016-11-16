﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnyStatus
{
    public class SettingsStore : NotifyPropertyChanged, ISettingsStore
    {
        private readonly ILogger _logger;
        private AppSettings _settings;

        public event EventHandler SettingsReset;

        public SettingsStore(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        public AppSettings Settings
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
            else
            {

                State.SetMetadata(Settings.Theme?.Metadata);
            }
        }

        private void Load()
        {
            Settings = Properties.Settings.Default.AppSettings;
        }

        private void Save()
        {
            Properties.Settings.Default.AppSettings = Settings;

            Properties.Settings.Default.Save();
        }

        private void Import(string filePath)
        {
            TextReader reader = null;

            try
            {
                reader = new StreamReader(filePath);

                var serializer = new XmlSerializer(typeof(AppSettings));

                var settings = (AppSettings)serializer.Deserialize(reader);

                settings.RootItem?.RestoreParentChildRelationship();

                Settings = settings;

                State.SetMetadata(Settings.Theme?.Metadata);

                Save();

                SettingsReset?.Invoke(this, EventArgs.Empty);
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
                var serializer = new XmlSerializer(typeof(AppSettings));
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

            Settings = AppSettings.Default();

            Save();

            SettingsReset?.Invoke(this, EventArgs.Empty);
        }

        private void Upgrade()
        {
            try
            {
                if (Properties.Settings.Default.RootItem == null)
                {
                    return;
                }

                _logger.Info("Upgrading settings...");

                Settings = AppSettings.Default();
                Settings.RootItem = Properties.Settings.Default.RootItem;
                Settings.ClientId = Properties.Settings.Default.ClientId;
                Settings.DebugMode = Properties.Settings.Default.DebugMode;
                Settings.ReportAnonymousUsage = Properties.Settings.Default.ReportAnonymousUsageData;
                Settings.ShowStatusColors = Properties.Settings.Default.ShowStatusColors;
                Settings.ShowStatusIcons = Properties.Settings.Default.ShowStatusIcons;

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
