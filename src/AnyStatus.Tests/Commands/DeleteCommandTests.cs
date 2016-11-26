using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Windows;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class DeleteCommandTests
    {
        [TestMethod]
        public void Should_RemoveItemFromParentAndJobScheduler()
        {
            var jobScheduler = Substitute.For<IJobScheduler>();
            var settingsStore = Substitute.For<ISettingsStore>();
            var dialogService = Substitute.For<IDialogService>();
            var item = Substitute.For<Dummy>();

            dialogService.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>())
                .Returns(MessageBoxResult.Yes);

            var command = new DeleteCommand(item);
            var handler = new DeleteCommandHandler(jobScheduler, settingsStore, dialogService);

            handler.Handle(command);

            item.Received().Delete();

            jobScheduler.Received().Remove(item);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_CommandIsNull()
        {
            var jobScheduler = Substitute.For<IJobScheduler>();
            var settingsStore = Substitute.For<ISettingsStore>();
            var dialogService = Substitute.For<IDialogService>();

            var handler = new DeleteCommandHandler(jobScheduler, settingsStore, dialogService);

            handler.Handle(null);
        }
    }
}
