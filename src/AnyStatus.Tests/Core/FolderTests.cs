using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Linq;

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
        public void State_Should_Be_Failed_When_OneOrMoreItemsFailed()
        {
            State.SetMetadata(Theme.Default.Metadata); //set states priority

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
        public void Should_Aggregate_State_By_Priority()
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

        [TestMethod]
        public void Should_Aggregate_State_When_Moving_Items()
        {
            var folder1 = new Folder();
            var folder2 = new Folder();

            var item1 = new Item();
            var item2 = new Item();

            folder1.Add(item1);
            folder1.Add(item2);

            item1.State = State.Ok;
            item2.State = State.Failed;

            Assert.AreSame(State.Failed, folder1.State);

            item2.MoveTo(folder2);

            Assert.AreSame(State.Ok, folder1.State);
        }

        [TestMethod]
        public void State_Should_Be_None_When_No_Items()
        {
            var folder1 = new Folder();
            var folder2 = new Folder();

            var item1 = new Item();
            var item2 = new Item();

            folder1.Add(item1);
            folder1.Add(item2);

            item1.State = State.Ok;
            item2.State = State.Failed;

            Assert.AreSame(State.Failed, folder1.State);

            item1.MoveTo(folder2);
            item2.MoveTo(folder2);

            Assert.AreSame(State.None, folder1.State);
        }

        [TestMethod]
        public void Clone_Should_DeepCopy()
        {
            var folder = new Folder();
            var subFolder = new Folder();
            var item = new Item();

            folder.Add(subFolder);
            subFolder.Add(item);

            var clone = (Folder)folder.Clone();

            Assert.AreNotEqual(clone, folder);

            Assert.AreNotEqual(clone.Items, folder.Items);

            Assert.AreNotEqual(clone.Items[0], subFolder);
            Assert.AreNotEqual(clone.Items[0].Items[0], item);
        }
    }
}
