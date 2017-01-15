using System;

namespace AnyStatus
{
    public class AnyStatusApp
    {
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;
        private readonly IUsageReporter _usageReporter;
        private readonly IJobScheduler _jobScheduler;

        public AnyStatusApp(ILogger logger,
                            ISettingsStore settingsStore,
                            IUsageReporter usageReporter,
                            IJobScheduler jobScheduler)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
        }

        public void Start()
        {
            try
            {
                _logger.Info("Starting AnyStatus...");

                LoadSettings();

                _settingsStore.SettingsReset += OnSettingsReset;

                _settingsStore.SettingsChanged += (s, e) => {

                    _logger.Info("The configuration file has been changed.");

                    var infoBarService = TinyIoCContainer.Current.Resolve<IInfoBarService>();

                    infoBarService.ShowInfoBar();
                };

                _jobScheduler.Start();

                _jobScheduler.Schedule(_settingsStore.Settings.RootItem, true);

                _usageReporter.ReportStartSession();

                _logger.Info("AnyStatus started.");
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
                _settingsStore.SettingsReset -= OnSettingsReset;

                _jobScheduler.Stop();

                _usageReporter.ReportEndSession();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while stopping AnyStatus");
            }
        }

        private void LoadSettings()
        {
            if (!_settingsStore.TryInitialize())
                return;

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