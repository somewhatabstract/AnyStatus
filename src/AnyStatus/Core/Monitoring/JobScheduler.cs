using FluentScheduler;
using System;
using System.Linq;

namespace AnyStatus
{
    public class JobScheduler : IJobScheduler
    {
        private readonly ILogger _logger;
        private readonly Func<IScheduledJob> _jobFactory;

        public JobScheduler(Func<IScheduledJob> jobFactory, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _jobFactory = Preconditions.CheckNotNull(jobFactory, nameof(jobFactory));
        }

        public void Start()
        {
            JobManager.Start();
        }

        public void Stop()
        {
            JobManager.Stop();
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

        public void RemoveAll()
        {
            foreach (var schedule in JobManager.AllSchedules)
            {
                JobManager.RemoveJob(schedule.Name);
            }
        }

        public void Restart()
        {
            Stop();

            RemoveAll();

            Start();
        }

        private void Schedule(Item item)
        {
            if (JobManager.AllSchedules.Any(k => k.Name == item.Id.ToString()))
            {
                _logger.Info($"\"{item.Name}\" is already scheduled.");

                return;
            }

            var job = _jobFactory();

            job.Item = item;

            JobManager.AddJob(job, s => s
                        .WithName(item.Id.ToString())
                        .NonReentrant()
                        .ToRunNow()
                        .AndEvery(item.Interval).Minutes());

            _logger.Info($"\"{item.Name}\" scheduled to run now and every {item.Interval} minutes.");
        }
    }
}
