using FluentScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace AnyStatus.Tests
{
    [TestClass]
    public class JobSchedulerTests
    {
        private ILogger _logger = Substitute.For<ILogger>();
        private Func<ScheduledJob> _jobFactory = () => { return new ScheduledJob(Substitute.For<ILogger>(), new Mediator(Substitute.For<ILogger>())); };
        private ISettingsStore _settingsStore = Substitute.For<ISettingsStore>();
        private Dummy _item = new Dummy { Id = Guid.NewGuid(), Name = "Test" };

        public JobSchedulerTests()
        {
            var settings = new UserSettings
            {
                RootItem = new RootItem
                {
                    Items = new ObservableCollection<Item> { _item }
                }
            };

            _settingsStore.Settings.Returns(settings);
        }

        [TestMethod]
        public void Should_Schedule_When_Starting()
        {
            var jobScheduler = new JobScheduler(_jobFactory, _settingsStore, _logger);

            jobScheduler.Start();

            var schedule = JobManager.AllSchedules.FirstOrDefault(k => k.Name == _item.Id.ToString());

            Assert.IsNotNull(schedule);

            JobManager.RemoveJob(_item.Id.ToString());
        }

        [TestMethod]
        public void Should_Unschedule_When_RemovingItem()
        {
            var jobScheduler = new JobScheduler(_jobFactory, _settingsStore, _logger);

            jobScheduler.Schedule(_item);

            jobScheduler.Remove(_item);

            var schedule = JobManager.AllSchedules.FirstOrDefault(k => k.Name == _item.Id.ToString());

            Assert.IsNull(schedule);
        }

        [TestMethod]
        public void Should_Execute_When_SchedulingAndExecutingItem()
        {
            var jobScheduler = new JobScheduler(_jobFactory, _settingsStore, _logger);

            Assert.AreEqual(0, _item.Counter);

            jobScheduler.Schedule(_item);

            Thread.Sleep(50);

            Assert.AreEqual(1, _item.Counter);

            jobScheduler.Execute(_item);

            Thread.Sleep(50);

            Assert.AreEqual(2, _item.Counter);

            jobScheduler.Remove(_item);
        }
    }
}
