using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AnyStatus.Tests.Commands
{
    [TestClass]
    public class DisableCommandTests
    {
        [TestMethod]
        public void Should_Disable_Unschedule_And_Save()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();

            var item = new Dummy();
            var command = new DisableCommand(item);
            var handler = new DisableCommandHandler(settingsStore, jobScheduler);

            handler.Handle(command);

            Assert.IsFalse(item.IsEnabled);

            settingsStore.Received().TrySave();

            jobScheduler.Received().Disable(item);
        }

        [TestMethod]
        public void Should_Disable_FolderItems()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();

            var folder = new Folder();
            var item = new Dummy();

            folder.Add(item);

            var command = new DisableCommand(folder);
            var handler = new DisableCommandHandler(settingsStore, jobScheduler);

            handler.Handle(command);

            Assert.IsTrue(item.IsDisabled);
            Assert.IsTrue(folder.IsDisabled);

            settingsStore.Received().TrySave();

            jobScheduler.Received(1).Disable(item);
            jobScheduler.DidNotReceive().Disable(folder);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_CommandIsNull()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();

            var handler = new DisableCommandHandler(settingsStore, jobScheduler);

            handler.Handle(null);
        }
    }
}
