using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Commands
{
    [TestClass]
    public class ShowToolWindowCommandTests
    {
        [TestMethod]
        public void Should_ShowToolWindow_When_Invoked()
        {
            var package = Substitute.For<IPackage>();

            var command = new ShowToolWindowCommand(package);

            command.MenuCommand.Invoke();

            package.Received().ShowToolWindow();
        }
    }
}
