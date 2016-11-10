using AnyStatus.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.Features.ToolWindow.Commands
{
    public class DeleteCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly IUserSettings _userSettings;
        private readonly IJobScheduler _jobScheduler;

        public event EventHandler CanExecuteChanged;

        public DeleteCommand(IJobScheduler jobScheduler, IUserSettings userSettings, ILogger logger)
        {
            _logger = logger;
            _userSettings = userSettings;
            _jobScheduler = jobScheduler;
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

                _userSettings.Save();
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
