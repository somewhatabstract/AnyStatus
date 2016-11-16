using System;
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
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _commandRegistry = Preconditions.CheckNotNull(commandRegistry, nameof(commandRegistry));
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.Info("Initializing...");

                _commandRegistry.RegisterCommands();

                await LoadSettings().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while initializing AnyStatus");
            }
        }

        private async Task LoadSettings()
        {
            await _settingsStore.TryInitializeAsync().ConfigureAwait(false);

            _logger.IsEnabled = _settingsStore.Settings.DebugMode;
            _usageReporter.IsEnabled = _settingsStore.Settings.ReportAnonymousUsage;
            _usageReporter.ClientId = _settingsStore.Settings.ClientId;
        }

        public void Start()
        {
            try
            {
                _jobScheduler.Start();

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
    }
}