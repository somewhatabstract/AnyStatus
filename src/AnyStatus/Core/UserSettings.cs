using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;

namespace AnyStatus
{
    /// <summary>
    /// Encapsulates access to Properties.Settings.Default
    /// </summary>
    public class UserSettings : IUserSettings
    {
        private ILogger _logger;

        public Item RootItem { get; private set; }
        public bool DebugMode { get; set; }
        public bool ReportAnonymousUsageData { get; set; }

        public UserSettings(ILogger logger)
        {
            _logger = logger ?? new NullLogger();

            Initialize();
        }

        private void Initialize()
        {
            _logger.Info("Initializing user settings.");

            try
            {

                LoadSettings();

                if (RootItem == null)
                {
                    Reset();
                }

                Upgrade();
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
            ReportAnonymousUsageData = Properties.Settings.Default.ReportAnonymousUsageData;
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

        public void Save(bool reload = false)
        {
            _logger.Info("Saving user settings.");

            try
            {
                Retry.Do(() =>
                {
                    Properties.Settings.Default.RootItem = RootItem;
                    Properties.Settings.Default.DebugMode = DebugMode;
                    Properties.Settings.Default.ReportAnonymousUsageData = ReportAnonymousUsageData;

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
                _logger.Info("Could not save user settings. Exception: " + ex.Flatten());
            }
        }

        public void Reset()
        {
            _logger.Info("Reseting user settings.");

            try
            {
                Properties.Settings.Default.Reset();

                RootItem = new RootItem { Name = "Root Item" };
                DebugMode = true;
                ReportAnonymousUsageData = true;

                Save();
            }
            catch (Exception ex)
            {
                _logger.Info("Could not reset user settings. Exception: " + ex.ToString());
            }
        }
    }
}
