using System;

namespace AnyStatus
{
    public class AnyStatusApp
    {
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;
        private readonly IUsageReporter _usageReporter;
        private readonly IJobScheduler _jobScheduler;
        private readonly IInfoBarService _infoBarService;

        public AnyStatusApp(ILogger logger,
                            ISettingsStore settingsStore,
                            IUsageReporter usageReporter,
                            IJobScheduler jobScheduler,
                            IInfoBarService infoBarService)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
            _infoBarService = Preconditions.CheckNotNull(infoBarService, nameof(infoBarService));
        }

        public void Start()
        {
            try
            {
                _logger.Info("Starting AnyStatus...");

                if (_settingsStore.TryInitialize() == false)
                    return;

                AssignSettings();

                _settingsStore.SettingsChanged += OnSettingsChanged;

                _settingsStore.SettingsSourceChanged += OnSettingsSourceChanged;

                _jobScheduler.Start();

                _jobScheduler.Schedule(_settingsStore.Settings.RootItem, includeChildren: true);

                _usageReporter.ReportStartSession();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while starting AnyStatus");
            }
        }

        private void AssignSettings()
        {
            //todo: use PropertyChanged event.
            _logger.IsEnabled = _settingsStore.Settings.DebugMode;
            _usageReporter.ClientId = _settingsStore.Settings.ClientId;
            _usageReporter.IsEnabled = _settingsStore.Settings.ReportAnonymousUsage;
        }

        public void Stop()
        {
            try
            {
                _settingsStore.SettingsChanged -= OnSettingsChanged;

                _settingsStore.SettingsSourceChanged -= OnSettingsSourceChanged;

                _jobScheduler.Stop();

                _usageReporter.ReportEndSession();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while stopping AnyStatus");
            }
        }

        private void OnSettingsSourceChanged(object sender, EventArgs e)
        {
            _logger.Info("The configuration file has been changed.");

            _infoBarService.ShowSettingsChangedInfoBar();
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            try
            {
                AssignSettings();

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