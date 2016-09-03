using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;

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

            _userSettings = userSettings;
            _logger = logger;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _logger.Log("Initializing scheduler.");

                NonReentrantAsDefault();

                Schedule(_userSettings.RootItem);
            }
            catch (Exception ex)
            {
                _logger.Log("Failed to initialize scheduler. Exception: " + ex.ToString());
            }
        }

        private void Schedule(Item item)
        {
            if (item == null)
                return;

            if (item is IScheduledItem && item.IsEnabled && item.Id != Guid.Empty)
            {
                Schedule(new ScheduledJob(item))
                     .WithName(item.Id.ToString())
                     .ToRunNow()
                     .AndEvery(item.Interval).Minutes();

                _logger.Log($"Item {item.Name} scheduled to run every {item.Interval} minutes.");
            }

            if (item.Items == null)
                return;

            foreach (var child in item.Items)
            {
                Schedule(child);
            }
        }
    }
}
