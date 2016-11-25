using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class EditCommandTests
    {
        ISettingsStore _settingsStore = Substitute.For<ISettingsStore>();
        IJobScheduler _jobScheduler = Substitute.For<IJobScheduler>();
        IUsageReporter _usageReporter = Substitute.For<IUsageReporter>();

        [TestMethod]
        public void Should_Reschedule_Save_And_Close()
        {
            var closeRequested = false;

            var parent = new Item();
            var source = new Item { Name = "Test" };

            parent.Add(source);

            var item = source.Clone() as Item;

            var command = new EditCommand(item, source, () => { closeRequested = true; });

            var handler = new EditCommandHandler(_settingsStore, _jobScheduler);

            handler.Handle(command);

            _jobScheduler.Received().Remove(source);
            _jobScheduler.Received().Schedule(item);

            _settingsStore.Received().TrySave();

            Assert.IsTrue(item.IsSelected);
            Assert.IsTrue(closeRequested);
        }
    }
}
