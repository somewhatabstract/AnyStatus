using System;

namespace AnyStatus
{
    public class DuplicateCommand : ItemCommand
    {
        public DuplicateCommand(Item item) : base(item) { }
    }

    public class DuplicateCommandHandler : IHandler<DuplicateCommand>
    {
        private readonly IJobScheduler _jobScheduler;
        private readonly ISettingsStore _settingsStore;

        public DuplicateCommandHandler(IJobScheduler jobScheduler, ISettingsStore settingsStore)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore)); ;
        }

        public void Handle(DuplicateCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            var clone = command.Item.Duplicate();

            _jobScheduler.Schedule(clone, includeChildren: true);

            _settingsStore.TrySave();
        }
    }
}