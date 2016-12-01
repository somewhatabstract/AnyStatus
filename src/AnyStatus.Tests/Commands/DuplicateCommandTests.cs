using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Commands
{
    [TestClass]
    public class DuplicateCommandTests
    {
        [TestMethod]
        public void Should_Duplicate_Schedule_And_Save()
        {
            var jobScheduler = Substitute.For<IJobScheduler>();
            var settingsStore = Substitute.For<ISettingsStore>();
            var item = Substitute.For<Item>();
            var clone = new Item();
            var command = new DuplicateCommand(item);
            var handler = new DuplicateCommandHandler(jobScheduler, settingsStore);

            item.Duplicate().Returns(clone);

            handler.Handle(command);

            item.Received().Duplicate();

            jobScheduler.Received().Schedule(clone, true);

            settingsStore.Received().TrySave();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_CommandIsNull()
        {
            var jobScheduler = NSubstitute.Substitute.For<IJobScheduler>();
            var settingsStore = NSubstitute.Substitute.For<ISettingsStore>();

            var handler = new DuplicateCommandHandler(jobScheduler, settingsStore);

            handler.Handle(null);
        }
    }
}
