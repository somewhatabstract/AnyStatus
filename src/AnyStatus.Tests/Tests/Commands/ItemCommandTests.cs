using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class ItemCommandTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_When_ItemIsNull()
        {
            var command = new ItemCommand(null);
        }
    }
}
