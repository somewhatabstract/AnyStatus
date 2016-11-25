﻿using System;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class AnyStatusApp
    {
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;
        private readonly IUsageReporter _usageReporter;
        private readonly IJobScheduler _jobScheduler;
        private readonly ICommandRegistry _commandRegistry;

        public AnyStatusApp(ILogger logger,
                            ISettingsStore settingsStore,
                            IUsageReporter usageReporter,
                            IJobScheduler jobScheduler,
                            ICommandRegistry commandRegistry)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
            _commandRegistry = Preconditions.CheckNotNull(commandRegistry, nameof(commandRegistry));
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.Info("Initializing...");

                _commandRegistry.RegisterCommands();

                await LoadSettings().ConfigureAwait(false);

                _settingsStore.SettingsReset += OnSettingsReset;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while initializing AnyStatus");
            }
        }

        public void Start()
        {
            try
            {
                _jobScheduler.Start();

                _jobScheduler.Schedule(_settingsStore.Settings.RootItem, true);

                _usageReporter.ReportStartSession();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while starting AnyStatus");
            }
        }

        public void Stop()
        {
            try
            {
                _jobScheduler.Stop();

                _usageReporter.ReportEndSession();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while stopping AnyStatus");
            }
        }

        private async Task LoadSettings()
        {
            await _settingsStore.TryInitializeAsync().ConfigureAwait(false);

            _logger.IsEnabled = _settingsStore.Settings.DebugMode;
            _usageReporter.ClientId = _settingsStore.Settings.ClientId;
            _usageReporter.IsEnabled = _settingsStore.Settings.ReportAnonymousUsage;
        }

        private void OnSettingsReset(object sender, EventArgs e)
        {
            try
            {
                _jobScheduler.Restart();

                _jobScheduler.Schedule(_settingsStore.Settings.RootItem, true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while restarting job scheduler.");
            }
        }
    }
}