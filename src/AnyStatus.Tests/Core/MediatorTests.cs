using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Tests.Tests
{
    [TestClass]
    public class MediatorTests
    {
        [TestMethod]
        public void Send_Should_Invoke_Default_Handler_Type()
        {
            var logger = Substitute.For<ILogger>();

            var mediator = new Mediator(logger);

            var item = new Dummy { Name = "dummy" };

            mediator.Send(item, typeof(IMonitor<>));

            Assert.AreSame(State.Ok, item.State);
        }

        [TestMethod]
        public void Send_Should_Invoke_Specified_Handler_Type()
        {
            var logger = Substitute.For<ILogger>();

            var mediator = new Mediator(logger);

            var item = new Dummy { Name = "dummy" };

            mediator.Send(item, typeof(IMonitor<>));

            Assert.AreSame(State.Ok, item.State);
        }
    }
}
