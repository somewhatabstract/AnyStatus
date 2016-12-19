using FluentScheduler;
using System;
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
        private readonly IJobScheduler _jobScheduler;
        private readonly ISettingsStore _settingsStore;

        public EnableCommandHandler(ISettingsStore settingsStore, IJobScheduler jobScheduler)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        public void Handle(EnableCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            Enable(command.Item);

            SaveChanges();
        }

        private void Enable(Item item)
        {
            if (item.ContainsElements())
                foreach (var child in item.Items)
                    Enable(child);

            if (item.IsDisabled)
            {
                item.IsEnabled = true;

                item.State = State.None;

                if (item is IScheduledItem)
                    EnableOrAddSchedule(item);

                _saveChanges = true;
            }
        }

        private void EnableOrAddSchedule(Item item)
        {
            if (_jobScheduler.Contains(item))
            {
                _jobScheduler.Enable(item);

                _jobScheduler.Execute(item);
            }
            else
            {
                _jobScheduler.Schedule(item, includeChildren: false);
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
