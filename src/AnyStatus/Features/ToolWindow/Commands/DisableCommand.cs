using FluentScheduler;

namespace AnyStatus
{
    public class DisableCommand : BaseItemCommand
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
            var item = command.Item;

            if (item == null)
                return;

            Disable(item);

            SaveChanges();
        }

        private void Disable(Item item)
        {
            if (item.ContainsElements())
                foreach (var child in item.Items)
                    Disable(child);

            if (item.IsEnabled && item is IScheduledItem)
            {
                JobManager.RemoveJob(item.Id.ToString());

                item.IsEnabled = false;

                _saveChanges = true;
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
