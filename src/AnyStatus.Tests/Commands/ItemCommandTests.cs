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
            var command = new TestCommand(null);
        }

        class TestCommand : ItemCommand
        {
            public TestCommand(Item item) : base(item)
            {
            }
        }
    }
}
