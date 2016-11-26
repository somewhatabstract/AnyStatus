using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AnyStatus.Tests.Commands
{
    [TestClass]
    public class EnableCommandTests
    {
        [TestMethod]
        public void Should_Schedule_And_Save()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();

            var item = new Dummy { IsEnabled = false, State = State.Disabled };
            var command = new EnableCommand(item);
            var handler = new EnableCommandHandler(settingsStore, jobScheduler);

            handler.Handle(command);

            Assert.IsTrue(item.IsEnabled);

            settingsStore.Received().TrySave();

            jobScheduler.Received().Schedule(item, false);
        }

        [TestMethod]
        public void Should_Enable_And_Save()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();
            
            var item = new Dummy { IsEnabled = false, State = State.Disabled };
            var command = new EnableCommand(item);
            var handler = new EnableCommandHandler(settingsStore, jobScheduler);

            jobScheduler.Contains(item).Returns(true);

            handler.Handle(command);

            Assert.IsTrue(item.IsEnabled);

            settingsStore.Received().TrySave();

            jobScheduler.Received().Enable(item);
        }

        [TestMethod]
        public void Should_Schedule_FolderItems()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();

            var folder = new Folder();
            var item = new Dummy { IsEnabled = false, State = State.Disabled };

            folder.Add(item);

            var command = new EnableCommand(folder);
            var handler = new EnableCommandHandler(settingsStore, jobScheduler);

            handler.Handle(command);

            Assert.IsTrue(item.IsEnabled);
            Assert.IsTrue(folder.IsEnabled);

            settingsStore.Received().TrySave();

            jobScheduler.Received().Schedule(item);
            jobScheduler.DidNotReceive().Schedule(folder, false);
        }

        [TestMethod]
        public void Should_Enable_FolderItems()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();

            var folder = new Folder();
            var item = new Dummy { IsEnabled = false, State = State.Disabled };

            folder.Add(item);

            var command = new EnableCommand(folder);
            var handler = new EnableCommandHandler(settingsStore, jobScheduler);

            jobScheduler.Contains(item).Returns(true);

            handler.Handle(command);

            Assert.IsTrue(item.IsEnabled);
            Assert.IsTrue(folder.IsEnabled);

            settingsStore.Received().TrySave();

            jobScheduler.Received().Enable(item);
            jobScheduler.DidNotReceive().Enable(folder);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_CommandIsNull()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var jobScheduler = Substitute.For<IJobScheduler>();

            var handler = new EnableCommandHandler(settingsStore, jobScheduler);

            handler.Handle(null);
        }
    }
}
