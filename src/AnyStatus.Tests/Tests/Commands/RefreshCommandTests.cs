using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class RefreshCommandTests
    {
        [TestMethod]
        public void Should_Execute_Command_Item()
        {
            var jobScheduler = Substitute.For<IJobScheduler>();

            var command = new RefreshCommand(new Item());

            var handler = new RefreshCommandHandler(jobScheduler);

            handler.Handle(command);

            jobScheduler.Received(1).Execute(command.Item);
        }

        [TestMethod]
        public void Should_Not_Execute_When_ItemIsNull()
        {
            var jobScheduler = Substitute.For<IJobScheduler>();

            var command = new RefreshCommand(null);

            var handler = new RefreshCommandHandler(jobScheduler);

            handler.Handle(command);

            jobScheduler.DidNotReceive().Execute(Arg.Any<Item>());
        }
    }
}
