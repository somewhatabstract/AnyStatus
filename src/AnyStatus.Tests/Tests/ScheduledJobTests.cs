using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace AnyStatus.Tests
{
    [TestClass]
    public class ScheduledJobTests
    {
        private ILogger _logger = Substitute.For<ILogger>();

        [TestMethod]
        public void Execute_Should_Invoke_Handler()
        {
            var item = new Dummy { Name = "Test", Id = Guid.NewGuid() };

            var scheduledJob = new ScheduledJob(_logger, new Mediator()) { Item = item };

            scheduledJob.Execute();

            Assert.AreEqual(1, item.Counter);
        }

        [TestMethod]
        public async Task ExecuteAsync_Should_Invoke_Handler()
        {
            var item = new Dummy { Name = "Test", Id = Guid.NewGuid() };

            var scheduledJob = new ScheduledJob(_logger, new Mediator()) { Item = item };

            await scheduledJob.ExecuteAsync();

            Assert.AreEqual(1, item.Counter);
        }

        [TestMethod]
        public void Execute_Should_Validate_Item()
        {
            var mediator = Substitute.For<IMediator>();

            var invalidItem = new Dummy();

            var scheduledJob = new ScheduledJob(_logger, mediator) { Item = invalidItem };

            scheduledJob.Execute();

            Assert.AreEqual(State.Invalid, invalidItem.State);
        }

        [TestMethod]
        public void Execute_Should_SetErrorState_When_ExceptionOccurres()
        {
            var item = new Dummy
            {
                Name = "Throws Exception",
                ThrowException = true,
                Id = Guid.NewGuid()
            };

            var scheduledJob = new ScheduledJob(_logger, new Mediator()) { Item = item };

            scheduledJob.Execute();

            Assert.AreEqual(State.Error, item.State);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Execute_Should_Throw_When_ItemIsNull()
        {
            var mediator = Substitute.For<IMediator>();

            var scheduledJob = new ScheduledJob(_logger, mediator);

            scheduledJob.Item = null;

            scheduledJob.Execute();
        }
    }
}
