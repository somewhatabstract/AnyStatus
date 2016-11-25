using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class TestCommandTests
    {
        [TestMethod]
        public async Task Should_ExecuteScheduledJob()
        {
            State.SetMetadata(Theme.Default.Metadata);
            bool canExecute = true;
            string message = string.Empty;
            var scheduledJob = Substitute.For<IScheduledJob>();
            Func<IScheduledJob> _jobFactory = () => { return scheduledJob; };

            scheduledJob.ExecuteAsync().Returns(Task.FromResult<object>(null));

            var item = new Item { Name = "Test" };
            var command = new TestCommand(item, c => { canExecute = c; }, m => { message = m; });
            var handler = new TestCommandHandler(_jobFactory);

            handler.Handle(command);

            await scheduledJob.Received().ExecuteAsync()
                .ConfigureAwait(true);

            Assert.AreSame(scheduledJob.Item, command.Item);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.IsTrue(canExecute);
        }
    }
}
