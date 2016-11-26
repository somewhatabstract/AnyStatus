using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Commands
{
    [TestClass]
    public class MoveDownCommandTests
    {
        [TestMethod]
        public void Should_MoveDown_And_Save()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var item = Substitute.For<Item>();
            var command = new MoveDownCommand(item);
            var handler = new MoveDownCommandHandler(settingsStore);

            handler.Handle(command);

            item.Received().MoveDown();
            settingsStore.Received().TrySave();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_CommandIsNull()
        {
            var settingsStore = Substitute.For<ISettingsStore>();

            var handler = new MoveDownCommandHandler(settingsStore);

            handler.Handle(null);
        }
    }
}
