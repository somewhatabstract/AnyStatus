using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Commands
{
    [TestClass]
    public class MoveUpCommandTests
    {
        [TestMethod]
        public void Should_MoveUp_And_Save()
        {
            var settingsStore = Substitute.For<ISettingsStore>();
            var item = Substitute.For<Item>();
            var command = new MoveUpCommand(item);
            var handler = new MoveUpCommandHandler(settingsStore);

            handler.Handle(command);

            item.Received().MoveUp();

            settingsStore.Received().TrySave();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_CommandIsNull()
        {
            var settingsStore = Substitute.For<ISettingsStore>();

            var handler = new MoveUpCommandHandler(settingsStore);

            handler.Handle(null);
        }
    }
}
