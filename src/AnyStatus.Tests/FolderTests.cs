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

            var item1 = new Item { State = ItemState.Ok };
            var item2 = new Item { State = ItemState.Ok };
            var item3 = new Item { State = ItemState.Ok };

            folder.Add(item1);
            folder.Add(item2);
            folder.Add(item3);

            Assert.AreEqual(ItemState.Ok, folder.State);

            var item4 = new Item { State = ItemState.Failed };

            folder.Add(item4);

            Assert.AreEqual(ItemState.Failed, folder.State);
        }

        [TestMethod]
        public void CalculateState_FolderStateIsCalculatedWhenItemsAreRemoved()
        {
            var folder = new Folder();

            var item1 = new Item { State = ItemState.Ok };
            var item2 = new Item { State = ItemState.Ok };
            var item3 = new Item { State = ItemState.Failed };

            folder.Add(item1);
            folder.Add(item2);
            folder.Add(item3);

            Assert.AreEqual(ItemState.Failed, folder.State);

            item3.Delete();

            Assert.AreEqual(ItemState.Ok, folder.State);
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

            item1.State = ItemState.Ok;
            item2.State = ItemState.Ok;
            item3.State = ItemState.Ok;

            Assert.AreEqual(ItemState.Ok, folder.State);
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

            item1.State = ItemState.Ok;
            item2.State = ItemState.Failed;
            item3.State = ItemState.Ok;

            Assert.AreEqual(ItemState.Failed, folder.State);
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

            item1.State = ItemState.Ok;
            item2.State = ItemState.Failed;
            item3.State = ItemState.Running;

            Assert.AreEqual(ItemState.Running, folder.State);
        }
    }
}
