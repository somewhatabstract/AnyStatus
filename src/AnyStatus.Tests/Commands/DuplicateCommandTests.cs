using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

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
        public void Should_Duplicate_FoldersAndChildItems()
        {
            var jobScheduler = Substitute.For<IJobScheduler>();
            var settingsStore = Substitute.For<ISettingsStore>();

            var rootItem = new RootItem();
            var folder = new Folder();
            var item1 = new Item();
            var item2 = new Item();

            rootItem.Add(folder);
            folder.Add(item1);
            folder.Add(item2);

            var command = new DuplicateCommand(folder);
            var handler = new DuplicateCommandHandler(jobScheduler, settingsStore);

            handler.Handle(command);

            Assert.IsTrue(rootItem.Items.Count == 2);

            var newFolder = rootItem.Items[1];

            Assert.IsTrue(newFolder.Items.Count == 2);

            Assert.AreSame(newFolder, newFolder.Items[0].Parent);
            Assert.AreSame(newFolder, newFolder.Items[1].Parent);

            jobScheduler.Received(1).Schedule(newFolder, true);

            settingsStore.Received(1).TrySave();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_CommandIsNull()
        {
            var jobScheduler = Substitute.For<IJobScheduler>();
            var settingsStore = Substitute.For<ISettingsStore>();

            var handler = new DuplicateCommandHandler(jobScheduler, settingsStore);

            handler.Handle(null);
        }
    }
}
