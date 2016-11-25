using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Windows;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class TestCommandTests
    {
        [TestMethod]
        public async void Should_ExecuteScheduledJob()
        {
            var toggleCanTestCalled = false;
            var logger = Substitute.For<ILogger>();
            var messageBox = Substitute.For<IMessageBox>();
            var mediator = Substitute.For<IMediator>();
            var scheduledJob = Substitute.For<IScheduledJob>();
            Func<IScheduledJob> _jobFactory = () => { return scheduledJob; };

            var item = new Item { Name = "Test" };
            var command = new TestCommand(item, () => { toggleCanTestCalled = true; });
            var handler = new TestCommandHandler(_jobFactory, messageBox);

            handler.Handle(command);

            await scheduledJob.Received(1).ExecuteAsync();

            Assert.AreSame(scheduledJob.Item, command.Item);
            Assert.IsTrue(toggleCanTestCalled);
            messageBox.Received(1).Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>());
        }
    }
}
