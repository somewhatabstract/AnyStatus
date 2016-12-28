using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Integration.Tests
{
    [TestClass]
    public class MonitoringTests
    {
        [TestMethod]
        public void HttpMonitor()
        {
            var request = new HttpStatus { Url = "http://www.microsoft.com" };

            new HttpStatusMonitor().Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        public void PingHandler()
        {
            var request = new Ping { Host = "localhost" };

            new PingMonitor().Handle(request);

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

            new TcpPortMonitor().Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        public void WindowsServiceHandler()
        {
            var request = new WindowsService
            {
                ServiceName = "Dhcp"
            };

            new WindowsServiceMonitor().Handle(request);

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

            new GitHubIssueMonitor().Handle(request);

            Assert.AreSame(State.Closed, request.State);
        }

        [TestMethod]
        public void CoverallsHandler()
        {
            var request = new CoverallsCoveredPercent
            {
                Url = "https://coveralls.io/github/AlonAm/AnyStatus?branch=master"
            };

            new CoverallsMonitor().Handle(request);

            Assert.AreNotSame(State.None, request.State);
        }
    }
}
