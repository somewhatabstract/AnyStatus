using FluentScheduler;

namespace AnyStatus
{
    public class EnableCommand : BaseItemCommand
    {
        public EnableCommand(Item item) : base(item) { }
    }

    public class EnableCommandHandler : IHandler<EnableCommand>
    {
        private bool _saveChanges;
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;


        public EnableCommandHandler(ISettingsStore settingsStore, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        public void Handle(EnableCommand command)
        {
            var item = command.Item;

            if (item == null)
                return;

            Enable(item);

            SaveChanges();
        }

        private void Enable(Item item)
        {
            if (item.ContainsElements())
                foreach (var child in item.Items)
                    Enable(child);

            if (item.IsDisabled && item is IScheduledItem)
            {
                AddScheduledJob(item);

                item.IsEnabled = true;

                item.State = State.None;

                _saveChanges = true;
            }
        }

        private static void AddScheduledJob(Item item)
        {
            JobManager.RemoveJob(item.Id.ToString());

            var job = TinyIoCContainer.Current.Resolve<ScheduledJob>();

            job.Item = item;

            JobManager.AddJob(job, s => s.WithName(item.Id.ToString()).ToRunNow().AndEvery(item.Interval).Minutes());
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
