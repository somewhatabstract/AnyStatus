using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace AnyStatus.Tests
{
    //[TestClass]
    public class AnalyticsReporterTests
    {
        //[TestMethod]
        public void ReportScreen()
        {
            var reporter = new AnalyticsReporter();

            reporter.ClientId = "UnitTest";
            reporter.IsEnabled = true;

            reporter.ReportStartSession();

            Thread.Sleep(1000);

            reporter.ReportScreen("Integration Test");

            Thread.Sleep(1000);

            reporter.ReportEndSession();
        }
    }
}
