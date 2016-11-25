using FluentScheduler;
using System;
using System.Linq;

namespace AnyStatus
{
    public class DisableCommand : ItemCommand
    {
        public DisableCommand(Item item) : base(item) { }
    }

    public class DisableCommandHandler : IHandler<DisableCommand>
    {
        private bool _saveChanges;
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;

        public DisableCommandHandler(ISettingsStore settingsStore, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        public void Handle(DisableCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            Disable(command.Item);

            SaveChanges();
        }

        private void Disable(Item item)
        {
            if (item.ContainsElements())
                foreach (var child in item.Items)
                    Disable(child);

            if (item.IsEnabled && item is IScheduledItem)
            {
                DisableSchedule(item.Id.ToString());

                item.IsEnabled = false;

                _saveChanges = true;
            }
        }

        private static void DisableSchedule(string name)
        {
            var schedule = JobManager.AllSchedules.FirstOrDefault(k => k.Name == name);

            if (schedule != null && !schedule.Disabled)
                schedule.Disable();
        }

        private void SaveChanges()
        {
            if (_saveChanges)
            {
                _settingsStore.TrySave();

                _saveChanges = false;
            }
        }
    }
}
