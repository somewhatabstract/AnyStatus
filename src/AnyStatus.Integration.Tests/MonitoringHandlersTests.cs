using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;

namespace AnyStatus.Integration.Tests
{
    [TestClass]
    public class MonitoringHandlersTests
    {
        private static TestContext _testContext;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

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

        [TestMethod]
        public void GitHubIssueHandler()
        {
            var request = new GitHubIssue
            {
                IssueNumber = 1,
                Repository = "AnyStatus",
                Owner = "AlonAm"
            };

            var handler = new GitHubIssueHandler();

            handler.Handle(request);

            Assert.AreSame(State.Closed, request.State);
        }

        [TestMethod]
        [DeploymentItem(@"Scripts\BatchScript.cmd")]
        public void BatchScriptHandler()
        {
            var request = new BatchFile
            {
                FileName = Path.Combine(_testContext.TestRunDirectory, "Out", "BatchScript.cmd")
            };

            var handler = new BatchFileHandler();

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        [DeploymentItem(@"Scripts\PowerShell.ps1")]
        public void PowerShellHandler()
        {
            var logger = Substitute.For<ILogger>();

            var request = new PowerShellScript
            {
                FileName = Path.Combine(_testContext.TestRunDirectory, "Out", "PowerShell.ps1")
            };

            var handler = new PowerShellScriptHandler(logger);

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }
    }
}
