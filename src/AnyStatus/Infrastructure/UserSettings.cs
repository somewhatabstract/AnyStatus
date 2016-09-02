using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.Diagnostics;

//todo: write debug log

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
            _logger.Log("Initializing user settings.");

            try
            {
                if (Properties.Settings.Default.RootItem == null)
                {
                    Reset();
                    Save(reload: true);
                }

                LoadSettings();

                Upgrade();
            }
            catch (Exception ex)
            {
                _logger.Log("Failed to save user settings. Exception: " + ex.ToString());
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

            _logger.Log("Upgrading user settings.");

            RootItem.Items = Properties.Settings.Default.Items;

            RootItem.RestoreParentChildRelationship();

            Properties.Settings.Default.Items = null;

            Save(reload: true);
        }

        public void Save(bool reload = false)
        {
            _logger.Log("Saving user settings.");

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

                        LoadSettings();
                    }
                },
                TimeSpan.FromSeconds(1), retryCount: 3);
            }
            catch (AggregateException ex)
            {
                _logger.Log("Failed to save user settings. Exception: " + ex.Flatten());
            }
        }

        public void Reset()
        {
            _logger.Log("Reseting user settings.");

            try
            {
                Properties.Settings.Default.Reset();

                Properties.Settings.Default.RootItem = new Folder { Name = "Root Item" };
                Properties.Settings.Default.DebugMode = true;
                Properties.Settings.Default.ReportAnonymousUsageData = true;

                LoadSettings();
            }
            catch (Exception ex)
            {
                _logger.Log("Failed to reset user settings. Exception: " + ex.ToString());
            }
        }
    }
}
