using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Windows.Input;

namespace AnyStatus.Features.ToolWindow.Commands
{
    public class DisableCommand : ICommand
    {
        private bool _saveChanges;
        private readonly ILogger _logger;
        private readonly IUserSettings _userSettings;

        public event EventHandler CanExecuteChanged;

        public DisableCommand(IUserSettings userSettings, ILogger logger)
        {
            _logger = logger;
            _userSettings = userSettings;
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
                _userSettings.Save();
                _saveChanges = false;
            }
        }
    }
}
