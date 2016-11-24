using FluentScheduler;
using System.Linq;

namespace AnyStatus
{
    public class EnableCommand : ItemCommand
    {
        public EnableCommand(Item item) : base(item) { }
    }

    public class EnableCommandHandler : IHandler<EnableCommand>
    {
        private bool _saveChanges;
        private readonly ILogger _logger;
        private readonly IJobScheduler _jobScheduler;
        private readonly ISettingsStore _settingsStore;

        public EnableCommandHandler(ISettingsStore settingsStore, IJobScheduler jobScheduler, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        public void Handle(EnableCommand command)
        {
            if (command?.Item == null)
                return;

            Enable(command.Item);

            SaveChanges();
        }

        private void Enable(Item item)
        {
            if (item.ContainsElements())
                foreach (var child in item.Items)
                    Enable(child);

            if (item.IsDisabled && item is IScheduledItem)
            {
                item.IsEnabled = true;

                item.State = State.Unknown;

                EnableOrAddSchedule(item);

                _saveChanges = true;
            }
        }

        private void EnableOrAddSchedule(Item item)
        {
            var schedule = JobManager.AllSchedules.FirstOrDefault(k => k.Name == item.Id.ToString());

            if (schedule == null)
            {
                _jobScheduler.Schedule(item, includeChildren: false);

                return;
            }

            if (schedule.Disabled)
            {
                schedule.Enable();

                schedule.Execute();
            }
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
