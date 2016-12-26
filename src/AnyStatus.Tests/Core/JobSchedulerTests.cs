using FluentScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using System.Threading;

namespace AnyStatus.Tests
{
    [TestClass]
    public class JobSchedulerTests
    {
        private ILogger _logger = Substitute.For<ILogger>();
        private Func<IScheduledJob> _jobFactory = () => { return new ScheduledJob(Substitute.For<ILogger>(), new Mediator(Substitute.For<ILogger>())); };
        private Dummy _item = new Dummy { Id = Guid.NewGuid(), Name = "Test" };

        [TestMethod]
        public void Should_Unschedule_When_RemovingItem()
        {
            var jobScheduler = new JobScheduler(_jobFactory, _logger);

            jobScheduler.Schedule(_item);

            jobScheduler.Remove(_item);

            var schedule = JobManager.AllSchedules.FirstOrDefault(k => k.Name == _item.Id.ToString());

            Assert.IsNull(schedule);
        }

        [TestMethod]
        public void Should_Execute_When_SchedulingAndExecutingItem()
        {
            var jobScheduler = new JobScheduler(_jobFactory, _logger);

            Assert.AreEqual(0, _item.Counter);

            jobScheduler.Schedule(_item);

            Thread.Sleep(100);

            Assert.AreEqual(1, _item.Counter);

            jobScheduler.Execute(_item);

            Thread.Sleep(100);

            Assert.AreEqual(2, _item.Counter);

            jobScheduler.Remove(_item);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_When_ExecutingNullItem()
        {
            var jobScheduler = new JobScheduler(_jobFactory, _logger);

            jobScheduler.Execute(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_When_SchedulingNullItem()
        {
            var jobScheduler = new JobScheduler(_jobFactory, _logger);

            jobScheduler.Schedule(null);
        }

        [TestMethod]
        public void Should_ExecuteChildItems_When_ExecutingFolder()
        {
            var folder = new Folder();
            var item = new Dummy { Name = "Test" };

            folder.Add(item);

            var jobScheduler = new JobScheduler(_jobFactory, _logger);

            jobScheduler.Schedule(item);

            Thread.Sleep(100);

            Assert.AreEqual(1, item.Counter);

            jobScheduler.Execute(folder);

            Thread.Sleep(100);

            Assert.AreEqual(2, item.Counter);
        }

        [TestMethod]
        public void Should_NotExecuteItem_When_ItemNotScheduled()
        {
            var item = new Dummy { Name = "test" };

            var jobScheduler = new JobScheduler(_jobFactory, _logger);

            jobScheduler.Execute(item);

            Thread.Sleep(100);

            Assert.AreEqual(0, item.Counter);
        }
    }
}
