using FluentScheduler;
using System;
using System.Windows.Input;

namespace AnyStatus
{
    public class DisableCommand : ICommand
    {
        private bool _saveChanges;
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning disable 67

        public DisableCommand(ISettingsStore settingsStore, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
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

                Disable(item);

                SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Info("Failed to disable item. Exception: " + ex.ToString());
            }
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
