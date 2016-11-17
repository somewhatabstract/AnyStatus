using System;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus
{
    public class DeleteCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;
        private readonly IJobScheduler _jobScheduler;

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning disable 67

        public DeleteCommand(IJobScheduler jobScheduler, ISettingsStore settingsStore, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger)); ;
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore)); ;
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler)); ;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                var item = parameter as Item;

                if (item == null)
                    return;

                var result = MessageBox.Show($"Are you sure you want to delete {item.Name}?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                if (result != MessageBoxResult.Yes)
                    return;

                Unschedule(item);

                item.Delete();

                _settingsStore.TrySave();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete item.");
            }
        }

        private void Unschedule(Item item)
        {
            if (item.ContainsElements())
                foreach (var child in item.Items)
                    Unschedule(child);

            if (item is IScheduledItem)
            {
                _jobScheduler.Remove(item);
            }
        }
    }
}
