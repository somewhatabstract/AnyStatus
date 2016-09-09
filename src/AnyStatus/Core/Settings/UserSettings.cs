using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;

#warning settings-reset event is static

namespace AnyStatus
{
    public class UserSettings : IUserSettings
    {
        private ILogger _logger;

        public UserSettings(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;

            Initialize();
        }

        #region Properties

        public event EventHandler SettingsReset;

        public Item RootItem { get; private set; }

        public bool DebugMode { get; set; }

        public bool ReportAnonymousUsage { get; set; }

        #endregion

        #region Methods

        public void Save(bool reload = false)
        {
            _logger.Info("Saving user settings.");

            try
            {
                Retry.Do(() =>
                {
                    Properties.Settings.Default.RootItem = RootItem;
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
                _logger.Error(ex, "Could not save user settings.");
            }
        }

        public void RestoreDefault()
        {
            _logger.Info("Reseting user settings.");

            try
            {
                Properties.Settings.Default.Reset();

                RootItem = new RootItem { Name = "Root Item" };
                DebugMode = true;
                ReportAnonymousUsage = true;

                Save();

                SettingsReset?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not reset user settings.");
            }
        }

        #endregion

        #region Helpers

        private void Initialize()
        {
            _logger.Info("Initializing user settings.");

            try
            {
                LoadSettings();

                if (RootItem == null)
                {
                    RestoreDefault();
                }

                Upgrade();

                SettingsReset += (s, e) => LoadSettings();
            }
            catch (Exception ex)
            {
                _logger.Info("Could not initialize user settings. Exception: " + ex.ToString());
            }
        }

        private void LoadSettings()
        {
            RootItem = Properties.Settings.Default.RootItem;
            DebugMode = Properties.Settings.Default.DebugMode;
            ReportAnonymousUsage = Properties.Settings.Default.ReportAnonymousUsageData;
        }

        private void Upgrade()
        {
            if (Properties.Settings.Default.Items == null)
            {
                return;
            }

            _logger.Info("Upgrading user settings.");

            RootItem.Items = Properties.Settings.Default.Items;

            Properties.Settings.Default.Items = null;

            Save();
        }

        #endregion
    }
}
