using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AnyStatus
{
    //todo: use state machine

    public class AnyStatusApp
    {
        private bool _started;
        private bool _initialized;
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

                LogConfigurationFilePath();

                _commandRegistry.RegisterCommands();

                await Task.Run(() => Initialize()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Initialization failed.");
            }
        }

        private void Initialize()
        {
            _userSettings.Initialize();

            _logger.IsEnabled = _userSettings.DebugMode;

            _usageReporter.ClientId = _userSettings.ClientId;

            _usageReporter.IsEnabled = _userSettings.ReportAnonymousUsage;

            _initialized = true;
        }

        public void Start()
        {
            if (_initialized)
                try
                {
                    _jobScheduler.Start();

                    _usageReporter.ReportStartSession();

                    _usageReporter.ReportScreen("Tool Window");

                    _started = true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "AnyStatus failed to start.");
                }
        }

        public void Stop()
        {
            if (_started)
                try
                {
                    _usageReporter.ReportEndSession();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "AnyStatus failed to stop.");
                }
        }

        [Conditional("DEBUG")]
        private void LogConfigurationFilePath()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

            _logger.Info("Configuration File Path: " + config.FilePath);
        }
    }
}
