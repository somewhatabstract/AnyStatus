namespace AnyStatus
{
    public class DuplicateCommand : BaseItemCommand
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
            var item = command.Item;

            var clone = item.Duplicate();

            _jobScheduler.Schedule(clone);

            _settingsStore.TrySave();
        }
    }
}