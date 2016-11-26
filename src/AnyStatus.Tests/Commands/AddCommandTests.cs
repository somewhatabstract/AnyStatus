using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class AddCommandTests
    {
        //todo: separate tests

        [TestMethod]
        public void Should_AddToParentScheduleAndSave()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();
            var usageReporter = Substitute.For<IUsageReporter>();
            var closed = false;
            var item = new Item { Name = "Test" };
            var parent = Substitute.For<Item>();
            var command = new AddCommand(item, parent, () => { closed = true; });
            var handler = new AddCommandHandler(settingsStore, jobScheduler, usageReporter);

            handler.Handle(command);

            Assert.IsTrue(closed);
            Assert.IsTrue(item.IsSelected);
            parent.Received().Add(item);
            settingsStore.Received(1).TrySave();
            jobScheduler.Received(1).Schedule(item);
            usageReporter.Received(1).ReportEvent("Items", "Add", command.Item.GetType().Name);
        }
    }
}
