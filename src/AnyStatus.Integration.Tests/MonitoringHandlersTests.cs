using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Integration.Tests
{
    [TestClass]
    public class MonitoringHandlersTests
    {
        [TestMethod]
        public void HttpHandler()
        {
            var request = new HttpStatus { Url = "http://www.microsoft.com" };

            var handler = new HttpStatusHandler();

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        public void PingHandler()
        {
            var request = new Ping { Host = "localhost" };

            var handler = new PingHandler();

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        public void TcpHandler()
        {
            var request = new TcpPort
            {
                Host = "www.microsoft.com",
                Port = 80
            };

            var handler = new TcpPortHandler();

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        public void WindowsServiceHandler()
        {
            var request = new WindowsService
            {
                ServiceName = "Dhcp"
            };

            var handler = new WindowsServiceHandler();

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }
    }
}
