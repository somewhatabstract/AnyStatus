using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class EditCommandTests
    {
        [TestMethod]
        public void Should_Reschedule_Save_And_Close()
        {
            var closeRequested = false;
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();
            var usageReporter = Substitute.For<IUsageReporter>();
            var target = new Item { Name = "Test" };
            var source = Substitute.For<Item>();
            var command = new EditCommand(target, source, () => { closeRequested = true; });
            var handler = new EditCommandHandler(settingsStore, jobScheduler);

            handler.Handle(command);

            source.Received().ReplaceWith(target);
            jobScheduler.Received().Remove(source);
            jobScheduler.Received().Schedule(target);
            settingsStore.Received().TrySave();

            Assert.IsTrue(target.IsSelected);
            Assert.IsTrue(closeRequested);
        }
    }
}
