using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class AddCommandTests
    {
        ISettingsStore _settingsStore = Substitute.For<ISettingsStore>();
        IJobScheduler _jobScheduler = Substitute.For<IJobScheduler>();
        IUsageReporter _usageReporter = Substitute.For<IUsageReporter>();
        IDialogService _dialogService = Substitute.For<IDialogService>();

        [TestMethod]
        public void Should_AddToParentScheduleAndSave()
        {
            //todo: separate tests

            var viewClosed = false;
            var item = new Item { Name = "Test" };
            var parent = Substitute.For<Item>();
            var command = new AddCommand(item, parent, () => { viewClosed = true; });
            var handler = new AddCommandHandler(_settingsStore, _jobScheduler, _usageReporter, _dialogService);

            handler.Handle(command);

            Assert.IsTrue(viewClosed);
            Assert.IsTrue(item.IsSelected);
            parent.Received().Add(item);
            _settingsStore.Received(1).TrySave();
            _jobScheduler.Received(1).Schedule(item);
            _usageReporter.Received(1).ReportEvent("Items", "Add", command.Item.GetType().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_Command_Is_Null()
        {
            var handler = new AddCommandHandler(_settingsStore, _jobScheduler, _usageReporter, _dialogService);

            handler.Handle(null);
        }

        [TestMethod]
        public void Should_ShowWarning_When_ItemIsInvalid()
        {
            var viewClosed = false;
            var item = new Item();
            var parent = Substitute.For<Item>();
            var command = new AddCommand(item, parent, () => { viewClosed = true; });
            var handler = new AddCommandHandler(_settingsStore, _jobScheduler, _usageReporter, _dialogService);

            handler.Handle(command);

            Assert.IsFalse(viewClosed);

            _dialogService.Received().ShowWarning(Arg.Any<string>(), Arg.Any<string>());

            parent.DidNotReceive().Add(item);
            _settingsStore.DidNotReceive().TrySave();
            _jobScheduler.DidNotReceive().Schedule(item);
            _usageReporter.DidNotReceive().ReportEvent(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
