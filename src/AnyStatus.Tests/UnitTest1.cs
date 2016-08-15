using AnyStatus.Models;
using AnyStatus.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace AnyStatus.Tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            var server = new Server { Url = "https://ci.jenkins-ci.org" };

            var request = new ViewsQuery.Request() { Server = server };

            var query = new ViewsQuery.Handler();

            var result = query.Handle(request);

            Assert.IsNotNull(result);
        }
    }
}
