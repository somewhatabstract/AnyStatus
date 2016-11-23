using System.Windows;

namespace AnyStatus
{
    public class DeleteCommand : ItemCommand
    {
        public DeleteCommand(Item item) : base(item) { }
    }

    public class DeleteCommandHandler : IHandler<DeleteCommand>
    {
        private readonly ISettingsStore _settingsStore;
        private readonly IJobScheduler _jobScheduler;

        public DeleteCommandHandler(IJobScheduler jobScheduler, ISettingsStore settingsStore)
        {
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore)); ;
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler)); ;
        }

        public void Handle(DeleteCommand command)
        {
            var item = command.Item;

            if (item == null)
                return;

            var result = MessageBox.Show($"Are you sure you want to delete {item.Name}?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            Unschedule(item);

            item.Delete();

            _settingsStore.TrySave();
        }

        private void Unschedule(Item item)
        {
            if (item.ContainsElements())
            {
                foreach (var child in item.Items)
                    Unschedule(child);
            }

            if (item is IScheduledItem)
            {
                _jobScheduler.Remove(item);
            }
        }
    }
}
