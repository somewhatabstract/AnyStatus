using FluentScheduler;
using System;
using System.Windows.Input;

namespace AnyStatus
{
    public class EnableCommand : ICommand
    {
        private bool _saveChanges;
        private readonly ILogger _logger;
        private readonly IUserSettings _userSettings;

        public event EventHandler CanExecuteChanged;

        public EnableCommand(IUserSettings userSettings, ILogger logger)
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

                Enable(item);

                SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Info("Failed to enable item. Exception: " + ex.ToString());
            }
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
                _userSettings.Save();
                _saveChanges = false;
            }
        }
    }
}
