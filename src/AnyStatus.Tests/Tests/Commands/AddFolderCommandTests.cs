using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class AddFolderCommandTests
    {
        [TestMethod]
        public void Should_AddFolder_And_Save()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var item = new Item { IsExpanded = false };
            var command = new AddFolderCommand(item);
            var handler = new AddFolderCommandHandler(settingsStore);

            handler.Handle(command);

            Assert.IsTrue(item.IsExpanded);

            var folder = item.Items[0];

            Assert.IsNotNull(folder);
            Assert.AreEqual("New Folder", folder.Name);
            Assert.AreEqual(true, folder.IsEditing);

            settingsStore.Received(1).TrySave();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_CommandIsNull()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            
            var handler = new AddFolderCommandHandler(settingsStore);

            handler.Handle(null);
        }
    }
}
