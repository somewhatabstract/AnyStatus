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
        public void Should_Duplication_Schedule_And_Save()
        {
            var jobScheduler = Substitute.For<IJobScheduler>();
            var settingsStore = Substitute.For<ISettingsStore>();

            var parent = new Item();
            var item = new Item();

            parent.Add(item);

            var command = new DuplicateCommand(item);
            var handler = new DuplicateCommandHandler(jobScheduler, settingsStore);

            handler.Handle(command);

            Assert.IsNotNull(parent.Items.ElementAtOrDefault(1));

            jobScheduler.Received().Schedule(parent.Items[1], false);

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
