using AnyStatus.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Tests
{
    [TestClass]
    public class FolderTests
    {
        [TestMethod]
        public void CalculateState_FolderStateIsCalculatedWhenItemsAreAdded()
        {
            var folder = new Folder();

            var item1 = new Item { State = State.Ok };
            var item2 = new Item { State = State.Ok };
            var item3 = new Item { State = State.Ok };

            folder.Add(item1);
            folder.Add(item2);
            folder.Add(item3);

            Assert.AreEqual(State.Ok, folder.State);

            var item4 = new Item { State = State.Failed };

            folder.Add(item4);

            Assert.AreEqual(State.Failed, folder.State);
        }

        [TestMethod]
        public void CalculateState_FolderStateIsCalculatedWhenItemsAreRemoved()
        {
            var folder = new Folder();

            var item1 = new Item { State = State.Ok };
            var item2 = new Item { State = State.Ok };
            var item3 = new Item { State = State.Failed };

            folder.Add(item1);
            folder.Add(item2);
            folder.Add(item3);

            Assert.AreEqual(State.Failed, folder.State);

            item3.Delete();

            Assert.AreEqual(State.Ok, folder.State);
        }

        [TestMethod]
        public void CalculateState_FolderStateIsOkWhenAllItemsAreOk()
        {
            var folder = new Folder();

            var item1 = new Item();
            var item2 = new Item();
            var item3 = new Item();

            folder.Add(item1);
            folder.Add(item2);
            folder.Add(item3);

            item1.State = State.Ok;
            item2.State = State.Ok;
            item3.State = State.Ok;

            Assert.AreEqual(State.Ok, folder.State);
        }

        [TestMethod]
        public void CalculateState_FolderStateIsFailedWhenOneOrMoreItemsFailed()
        {
            var folder = new Folder();

            var item1 = new Item();
            var item2 = new Item();
            var item3 = new Item();

            folder.Add(item1);
            folder.Add(item2);
            folder.Add(item3);

            item1.State = State.Ok;
            item2.State = State.Failed;
            item3.State = State.Ok;

            Assert.AreEqual(State.Failed, folder.State);
        }

        [TestMethod]
        public void CalculateState_FolderStateIsCalculatedByPriority()
        {
            var folder = new Folder();

            var item1 = new Item();
            var item2 = new Item();
            var item3 = new Item();

            folder.Add(item1);
            folder.Add(item2);
            folder.Add(item3);

            item1.State = State.Ok;
            item2.State = State.Failed;
            item3.State = State.Running;

            Assert.AreEqual(State.Running, folder.State);
        }
    }
}
