using AnyStatus.Infrastructure;
using AnyStatus.Models;
using FluentScheduler;
using System;

namespace AnyStatus
{
    public class JobScheduler : IJobScheduler
    {
        private readonly ILogger _logger;
        private readonly Func<ScheduledJob> _jobFactory;

        public JobScheduler(Func<ScheduledJob> jobFactory, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (jobFactory == null)
                throw new ArgumentNullException(nameof(jobFactory));

            _logger = logger;
            _jobFactory = jobFactory;
        }

        public void Reschedule(Item item, bool includeChildren = false)
        {
            JobManager.RemoveJob(item.Id.ToString());

            Schedule(item, includeChildren);
        }

        public void Execute(Item item)
        {
            if (item == null)
                return;

            if (item is IScheduledItem)
            {
                var schedule = JobManager.GetSchedule(item.Id.ToString());

                if (schedule != null) schedule.Execute();
            }
        }

        public void Schedule(Item item, bool includeChildren = false)
        {
            if (item == null)
                throw new InvalidOperationException();

            if (item.IsSchedulable())
            {
                ScheduleItem(item);
            }

            if (includeChildren && item.HasChildren())
                foreach (var child in item.Items)
                {
                    Schedule(child, includeChildren);
                }
        }

        private void ScheduleItem(Item item)
        {
            var job = _jobFactory();

            job.Item = item;

            JobManager.AddJob(job, s => s
                        .WithName(item.Id.ToString())
                        .ToRunNow()
                        .AndEvery(item.Interval).Minutes());

            _logger.Info($"Item {item.Name} scheduled to run every {item.Interval} minutes.");
        }
    }
}
