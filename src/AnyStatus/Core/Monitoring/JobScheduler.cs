using FluentScheduler;
using System;

namespace AnyStatus
{
    public class JobScheduler : IJobScheduler
    {
        private readonly ILogger _logger;
        private ISettingsStore _settingsStore;
        private readonly Func<ScheduledJob> _jobFactory;

        public JobScheduler(Func<ScheduledJob> jobFactory, ISettingsStore settingsStore, ILogger logger)
        {
            _jobFactory = Preconditions.CheckNotNull(jobFactory, nameof(jobFactory));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));

            _settingsStore.SettingsReset += OnSettingsReset;
        }

        public void Start()
        {
            Schedule(_settingsStore.Settings.RootItem, includeChildren: true);
        }

        public void Stop()
        {
            JobManager.Stop();
        }

        public void Reschedule(Item item, bool includeChildren = false)
        {
            //todo: remove includeChildren

            JobManager.RemoveJob(item.Id.ToString());

            Schedule(item, includeChildren);
        }

        public void Execute(Item item)
        {
            if (item == null) return;

            if (item is IScheduledItem)
            {
                var schedule = JobManager.GetSchedule(item.Id.ToString());

                if (schedule != null) schedule.Execute();
            }

            if (item.ContainsElements())
            {
                foreach (var child in item.Items)
                {
                    Execute(child);
                }
            }
        }

        public void Schedule(Item item, bool includeChildren = false)
        {
            if (item == null)
                return;

            if (item.IsSchedulable())
            {
                Schedule(item);
            }

            if (includeChildren && item.ContainsElements())
                foreach (var child in item.Items)
                {
                    Schedule(child, includeChildren);
                }
        }

        public void Remove(Item item)
        {
            JobManager.RemoveJob(item.Id.ToString());
        }

        public void ExecuteAll()
        {
            foreach (var schedule in JobManager.AllSchedules)
            {
                schedule.Execute();
            }
        }

        private void RemoveAll()
        {
            foreach (var schedule in JobManager.AllSchedules)
            {
                JobManager.RemoveJob(schedule.Name);
            }
        }

        private void Schedule(Item item)
        {
            var job = _jobFactory();

            job.Item = item;

            JobManager.AddJob(job, s => s
                        .WithName(item.Id.ToString())
                        .NonReentrant()
                        .ToRunNow()
                        .AndEvery(item.Interval).Minutes());

            _logger.Info($"Item {item.Name} scheduled to run every {item.Interval} minutes.");
        }

        private void OnSettingsReset(object sender, EventArgs e)
        {
            try
            {
                RemoveAll();

                Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while restarting job scheduler.");
            }
        }
    }
}
