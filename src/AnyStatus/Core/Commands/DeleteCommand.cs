using System;
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
        private readonly IDialogService _dialogService;

        public DeleteCommandHandler(IJobScheduler jobScheduler, ISettingsStore settingsStore, IDialogService dialogService)
        {
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public void Handle(DeleteCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            var result = _dialogService.Show($"Are you sure you want to delete {command.Item.Name}?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            Unschedule(command.Item);

            command.Item.Delete();

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
