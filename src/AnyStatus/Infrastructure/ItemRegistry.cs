using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Collections.Generic;

namespace AnyStatus.Infrastructure
{
    public class ItemRegistry : Registry
    {
        private ILogger _logger;
        private IUserSettings _userSettings;

        public ItemRegistry(IUserSettings userSettings, ILogger logger)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
            _userSettings = userSettings;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _logger.Log("Initializing scheduler.");

                NonReentrantAsDefault();

                Schedule(_userSettings?.RootItem?.Items);
            }
            catch (Exception ex)
            {
                _logger.Log("Failed to initialize scheduler. Exception: " + ex.ToString());
            }
        }

        private void Schedule(IEnumerable<Item> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (item is Folder)
                    Schedule(item.Items);
                else
                    Schedule(item);
            }
        }

        private void Schedule(Item item)
        {
            if (item.Id == Guid.Empty || !item.IsEnabled)
                return;

            var job = new ScheduledJob(item);
            var jobName = item.Id.ToString();

            Schedule(job)
                 .WithName(jobName)
                 .ToRunNow()
                 .AndEvery(item.Interval).Minutes();
        }
    }
}
