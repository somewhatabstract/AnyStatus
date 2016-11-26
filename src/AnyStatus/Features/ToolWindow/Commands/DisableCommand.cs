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
        private readonly IJobScheduler _jobScheduler;
        private readonly ISettingsStore _settingsStore;

        public DisableCommandHandler(ISettingsStore settingsStore, IJobScheduler jobScheduler)
        {
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
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

            if (item.IsDisabled)
                return;

            if (item is IScheduledItem)
                _jobScheduler.Disable(item);

            item.IsEnabled = false;

            _saveChanges = true;
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
