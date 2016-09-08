using AnyStatus.Infrastructure;
using AnyStatus.Models;
using FluentScheduler;
using System;

namespace AnyStatus
{
    public class JobScheduler : IJobScheduler
    {
        private readonly Func<ScheduledJob> _jobFactory;

        public JobScheduler(Func<ScheduledJob> jobFactory)
        {
            if (jobFactory == null)
                throw new ArgumentNullException(nameof(jobFactory));

            _jobFactory = jobFactory;
        }

        public void Schedule(Item item)
        {
            var job = _jobFactory();

            job.Item = item;

            JobManager.AddJob(job, s => s.WithName(item.Id.ToString()).ToRunNow().AndEvery(item.Interval).Minutes());
        }

        public void Reschedule(Item item)
        {
            JobManager.RemoveJob(item.Id.ToString());

            Schedule(item);
        }
    }
}
