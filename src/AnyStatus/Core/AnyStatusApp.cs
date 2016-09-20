using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using System;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class AnyStatusApp
    {
        private readonly ILogger _logger;
        private readonly IUserSettings _userSettings;
        private readonly IUsageReporter _usageReporter;
        private readonly IJobScheduler _jobScheduler;
        private readonly ICommandRegistry _commandRegistry;

        public AnyStatusApp(ILogger logger,
                            IUserSettings userSettings,
                            IUsageReporter usageReporter,
                            IJobScheduler jobScheduler,
                            ICommandRegistry commandRegistry)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _userSettings = Preconditions.CheckNotNull(userSettings, nameof(userSettings));
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

                await _userSettings.InitializeAsync().ConfigureAwait(false);

                _logger.IsEnabled = _userSettings.DebugMode;

                _usageReporter.ClientId = _userSettings.ClientId;

                _usageReporter.IsEnabled = _userSettings.ReportAnonymousUsage;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AnyStatus failed to initialize.");
            }
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
                _logger.Error(ex, "AnyStatus failed to start.");
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
                _logger.Error(ex, "AnyStatus failed to stop.");
            }
        }
    }
}