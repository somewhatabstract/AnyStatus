using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;

namespace AnyStatus.Integration.Tests
{
    [TestClass]
    public class MonitoringTests
    {
        [TestMethod]
        public void HttpMonitor()
        {
            var request = new HttpStatus { Url = "http://www.microsoft.com" };

            var handler = new HttpStatusMonitor();

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        public void PingHandler()
        {
            var request = new Ping { Host = "localhost" };

            var handler = new PingMonitor();

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

            var handler = new TcpPortMonitor();

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

            var handler = new WindowsServiceMonitor();

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [Ignore]
        [TestMethod]
        public void GitHubIssueHandler()
        {
            var request = new GitHubIssue
            {
                IssueNumber = 1,
                Repository = "AnyStatus",
                Owner = "AlonAm"
            };

            var handler = new GitHubIssueMonitor();

            handler.Handle(request);

            Assert.AreSame(State.Closed, request.State);
        }

        [TestMethod]
        public void CoverallsHandler()
        {
            var request = new CoverallsCoveredPercent
            {
                Url = "https://coveralls.io/github/AlonAm/AnyStatus?branch=master"
            };

            var handler = new CoverallsMonitor();

            handler.Handle(request);

            Assert.AreNotSame(State.None, request.State);
        }
    }
}
