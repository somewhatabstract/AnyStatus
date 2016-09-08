using AnyStatus.Interfaces;
using FluentScheduler;
using System;

namespace AnyStatus.Infrastructure
{
    public class ItemRegistry : Registry
    {
        private readonly ILogger _logger;
        private readonly IUserSettings _userSettings;
        private readonly IJobScheduler _jobScheduler;

        public ItemRegistry(IUserSettings userSettings, IJobScheduler jobScheduler, ILogger logger)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));
            if (jobScheduler == null)
                throw new ArgumentNullException(nameof(jobScheduler));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _userSettings = userSettings;
            _jobScheduler = jobScheduler;
            _logger = logger;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _logger.Info("Initializing scheduler.");

                NonReentrantAsDefault();

                if (_userSettings.RootItem != null)
                {
                    _jobScheduler.Schedule(_userSettings.RootItem, includeChildren: true);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to initialize scheduler.");
            }
        }
    }
}
