using FluentScheduler;
using System;
using System.Windows.Input;

namespace AnyStatus
{
    public class EnableCommand : ICommand
    {
        private bool _saveChanges;
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning disable 67

        public EnableCommand(ISettingsStore settingsStore, ILogger logger)
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
