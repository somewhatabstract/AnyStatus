﻿using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;

namespace AnyStatus
{
    public class JobScheduler : IJobScheduler
    {
        private readonly ILogger _logger;
        private IUserSettings _userSettings;
        private readonly Func<ScheduledJob> _jobFactory;

        public JobScheduler(Func<ScheduledJob> jobFactory, IUserSettings userSettings, ILogger logger)
        {
            _jobFactory = Preconditions.CheckNotNull(jobFactory, nameof(jobFactory));
            _userSettings = Preconditions.CheckNotNull(userSettings, nameof(userSettings));
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));

            _userSettings.SettingsReset += OnSettingsReset;
        }

        public void Start()
        {
            try
            {
                Schedule(_userSettings.RootItem, includeChildren: true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while initializing the job scheduler.");
            }
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
                return;

            if (item.IsSchedulable())
            {
                Schedule(item);
            }

            if (includeChildren && item.HasChildren())
                foreach (var child in item.Items)
                {
                    Schedule(child, includeChildren);
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
                _logger.Error(ex, "An error occurred while resetting the job scheduler.");
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
    }
}
