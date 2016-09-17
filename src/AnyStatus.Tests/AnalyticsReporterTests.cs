using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace AnyStatus.Tests
{
    [TestClass]
    public class AnalyticsReporterTests
    {
        [Ignore]
        [TestMethod]
        public void ReportScreen()
        {
            var reporter = new AnalyticsReporter("UA-83802855-1", "AnyStatus", "AnyStatus", Guid.NewGuid().ToString(), "0.7", false);

            reporter.ReportStartSession();

            Thread.Sleep(1000);

            reporter.ReportScreen("Integration Test");

            Thread.Sleep(1000);

            reporter.ReportEndSession();

            Thread.Sleep(1000);
        }
    }
}
